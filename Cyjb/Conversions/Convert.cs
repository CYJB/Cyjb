using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;
using Cyjb.Conversions;
using Cyjb.Reflection;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供将一种类型转换为另一种类型的方法。
	/// </summary>
	/// <remarks><para>支持预定义的隐式、显式类型转换，用户自定义类型转换（Implicit 和 Explicit 运算符），
	/// 并可以通过 <see cref="AddConverter{TInput, TOutput}"/> 和 <see cref="AddConverterProvider"/> 
	/// 方法注入额外的类型转换方法。</para>
	/// <para>默认添加了所有类型转换为字符串，和字符串转换为数字、布尔和日期/时间的额外方法。</para>
	/// <para>所有数值类型转换，都会对溢出进行检查，如果数值不在范围内则会抛出异常。</para>
	/// <para>explicit 和 implicit 方法缓存的键为 Cyjb.UserConversionCache，默认使用上限为 <c>256</c> 的 
	/// <see cref="LruCache{TKey, TValue}"/>。动态生成的类型转换方法缓存的键为 Cyjb.ConverterCache，
	/// 默认使用无上限的 <see cref="SimplyCache{TKey, TValue}"/>。关于如何设置缓存，
	/// 可以参见 <see cref="CacheFactory"/>。</para>
	/// </remarks>
	public static partial class Convert
	{
		/// <summary>
		/// 返回具有指定输入和输出类型的 <see cref="Converter{TInput, TOutput}"/> 类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>具有指定输入和输出类型的 <see cref="Converter{TInput, TOutput}"/> 类型。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		public static Type GetConverterType(Type inputType, Type outputType)
		{
			if (inputType == null)
			{
				throw CommonExceptions.ArgumentNull("inputType");
			}
			if (outputType == null)
			{
				throw CommonExceptions.ArgumentNull("outputType");
			}
			Contract.EndContractBlock();
			return typeof(Converter<,>).MakeGenericType(inputType, outputType);
		}

		#region 转换器

		/// <summary>
		/// 类型的转换器。
		/// </summary>
		private sealed class Converter
		{
			/// <summary>
			/// 泛型的类型转换器。
			/// </summary>
			public Delegate GenericConverter;
			/// <summary>
			/// 非泛型的类型转换器。
			/// </summary>
			public Converter<object, object> ObjectConverter;
		}
		/// <summary>
		/// 直接返回对象的类型转换器，用于隐式引用转换。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Converter<object, object> defaultObjectConverter = obj => obj;
		/// <summary>
		/// 类型转换器的存储字典。
		/// </summary>
		private static readonly ICache<Tuple<Type, Type>, Converter> converterCache =
			CacheFactory.Create<Tuple<Type, Type>, Converter>("Cyjb.ConverterCache") ??
			new SimplyCache<Tuple<Type, Type>, Converter>();
		/// <summary>
		/// 获取将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="buildGeneric"><c>true</c> 表示需要生成泛型类型转换委托；<c>false</c> 
		/// 表示需要生成非泛型的类型转换委托。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。
		/// 如果不存在则为 <c>null</c>。</returns>
		private static Converter GetConverterInternal(Type inputType, Type outputType, bool buildGeneric)
		{
			Contract.Requires(inputType != null && outputType != null);
			return converterCache.GetOrAdd(new Tuple<Type, Type>(inputType, outputType), types =>
			{
				Conversion conversion = ConversionFactory.GetConversion(inputType, outputType);
				if (conversion == null)
				{
					return null;
				}
				Converter converter = new Converter();
				DelegateConversion dlgConversion = conversion as DelegateConversion;
				if (dlgConversion != null)
				{
					converter.GenericConverter = dlgConversion.Converter;
					if (buildGeneric)
					{
						return converter;
					}
				}
				if (conversion.ConversionType == ConversionType.ImplicitReferenceConversion)
				{
					converter.ObjectConverter = defaultObjectConverter;
					if (!buildGeneric)
					{
						return converter;
					}
				}
				Delegate dlg = BuildConverter(conversion, inputType, outputType, false, buildGeneric);
				if (buildGeneric)
				{
					converter.GenericConverter = dlg;
				}
				else
				{
					converter.ObjectConverter = (Converter<object, object>)dlg;
				}
				return converter;
			});
		}
		/// <summary>
		/// 构造类型转换器。
		/// </summary>
		/// <param name="conversion">使用的类型转换。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		/// <param name="buildGeneric"><c>true</c> 表示需要生成泛型类型转换委托；<c>false</c> 
		/// 表示需要生成非泛型的类型转换委托。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。</returns>
		private static Delegate BuildConverter(Conversion conversion, Type inputType, Type outputType, bool isChecked,
			bool buildGeneric)
		{
			Contract.Requires(conversion != null && inputType != null && outputType != null);
			Contract.Ensures(Contract.Result<Delegate>() != null);
			DynamicMethod method = new DynamicMethod("Converter", buildGeneric ? outputType : typeof(object),
				new[] { buildGeneric ? inputType : typeof(object) }, true);
			ILGenerator il = method.GetILGenerator();
			bool passByAddress = conversion is FromNullableConversion;
			if (passByAddress && (buildGeneric || !inputType.IsValueType))
			{
				il.Emit(OpCodes.Ldarga_S, (byte)0);
			}
			else
			{
				il.Emit(OpCodes.Ldarg_0);
			}
			if (buildGeneric)
			{
				conversion.Emit(il, inputType, outputType, isChecked);
				il.Emit(OpCodes.Ret);
				return method.CreateDelegate(typeof(Converter<,>).MakeGenericType(inputType, outputType));
			}
			// 从 object 拆箱得到值类型。
			ConversionFactory.GetPreDefinedConversion(typeof(object), inputType).Emit(il, typeof(object), inputType, isChecked);
			if (passByAddress)
			{
				il.EmitGetAddress(inputType);
			}
			conversion.Emit(il, inputType, outputType, isChecked);
			// 值类型装箱为 object。
			if (outputType.IsValueType)
			{
				il.Emit(OpCodes.Box, outputType);
			}
			il.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof(Converter<object, object>));
		}
		/// <summary>
		/// 获取将对象从 <typeparamref name="TInput"/> 类型转换为 <typeparamref name="TOutput"/> 类型的转换器。
		/// </summary>
		/// <returns>将对象从 <typeparamref name="TInput"/> 类型转换为 <typeparamref name="TOutput"/> 类型的转换器。
		/// 如果不存在则为 <c>null</c>。</returns>
		public static Converter<TInput, TOutput> GetConverter<TInput, TOutput>()
		{
			Type inputType = typeof(TInput);
			Type outputType = typeof(TOutput);
			Converter converter = GetConverterInternal(inputType, outputType, true);
			if (converter == null)
			{
				return null;
			}
			if (converter.GenericConverter == null)
			{
				converter.GenericConverter = BuildConverter(ConversionFactory.GetConversion(inputType, outputType),
					inputType, outputType, false, true);
			}
			return converter.GenericConverter as Converter<TInput, TOutput>;
		}
		/// <summary>
		/// 获取将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。
		/// 如果不存在则为 <c>null</c>。</returns>
		/// <remarks>尽可能使用泛型方法 <see cref="GetConverter{TInput,TOutput}"/>，这样可以避免额外的类型转换。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		public static Converter<object, object> GetConverter(Type inputType, Type outputType)
		{
			if (inputType == null)
			{
				throw CommonExceptions.ArgumentNull("inputType");
			}
			if (outputType == null)
			{
				throw CommonExceptions.ArgumentNull("outputType");
			}
			Contract.EndContractBlock();
			Converter converter = GetConverterInternal(inputType, outputType, false);
			if (converter == null)
			{
				return null;
			}
			return converter.ObjectConverter ?? (converter.ObjectConverter = (Converter<object, object>)BuildConverter(
				ConversionFactory.GetConversion(inputType, outputType), inputType, outputType, false, false));
		}
		/// <summary>
		/// 返回对象能否从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>如果对象能够从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型，
		/// 则为 <c>true</c>。否则为 <c>false</c>。</returns>
		/// <remarks>与 <see cref="TypeExt.IsImplicitFrom"/> 或 <see cref="TypeExt.IsExplicitFrom"/> 不同，
		/// 这里会考虑预定义的隐式、显式类型转换，用户自定义的类型转换（包括 <c>implicit</c> 和 <c>explicit</c>），
		/// 以及通过 <see cref="AddConverter{TInput, TOutput}"/> 和 <see cref="AddConverterProvider"/> 
		/// 方法注册的类型转换方法。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static bool CanChangeType(Type inputType, Type outputType)
		{
			if (inputType == null)
			{
				throw CommonExceptions.ArgumentNull("inputType");
			}
			if (outputType == null)
			{
				throw CommonExceptions.ArgumentNull("outputType");
			}
			Contract.EndContractBlock();
			try
			{
				return ConversionFactory.GetConversion(inputType, outputType) != null;
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// 添加将对象从 <typeparamref name="TInput"/> 类型转换为 <typeparamref name="TOutput"/> 类型的转换器。
		/// </summary>
		/// <param name="converter">将对象从 <typeparamref name="TInput"/> 类型转换为 
		/// <typeparamref name="TOutput"/> 类型的转换器。</param>
		/// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <c>null</c>。</exception>
		/// <remarks><paramref name="converter"/> 不会覆盖预定义的隐式、显式类型转换和用户自定义的类型转换。
		/// 对于相同输入/输出类型的 <paramref name="converter"/>，后设置的会覆盖先设置的，以及任何 
		/// <see cref="IConverterProvider"/> 提供的类型转换方法。</remarks>
		public static void AddConverter<TInput, TOutput>(Converter<TInput, TOutput> converter)
		{
			if (converter == null)
			{
				throw CommonExceptions.ArgumentNull("converter");
			}
			Contract.EndContractBlock();
			ConversionFactory.AddConverterProvider(new ConverterProvider(converter, typeof(TInput), typeof(TOutput)));
		}
		/// <summary>
		/// 添加指定的类型转换器提供者。
		/// </summary>
		/// <param name="provider">要添加的类型转换器提供者。</param>
		/// <exception cref="ArgumentNullException"><paramref name="provider"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="provider"/> 的源类型为 <c>null</c>。</exception>
		/// <remarks><paramref name="provider"/> 提供的类型转换方法不会覆盖预定义的隐式、显式类型转换和用户自定义的类型转换，
		/// 以及任何通过 <see cref="AddConverter{TInput, TOutput}"/> 方法设置的类型转换方法。
		/// 对于相同源类型的 <see cref="IConverterProvider"/>，<see cref="IConverterProvider.GetConverterTo"/> 
		/// 方法提供的类型转换方法优先级更高，且后设置的优先级更高。</remarks>
		public static void AddConverterProvider(IConverterProvider provider)
		{
			if (provider == null)
			{
				throw CommonExceptions.ArgumentNull("provider");
			}
			if (provider.OriginType == null)
			{
				throw CommonExceptions.ArgumentNull("provider.OriginType");
			}
			Contract.EndContractBlock();
			ConversionFactory.AddConverterProvider(provider);
		}

		#endregion // 转换器

		#region 类型转换

		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。
		/// </summary>
		/// <typeparam name="TInput">要转换的对象的类型。</typeparam>
		/// <typeparam name="TOutput">要将 <paramref name="value"/> 转换到的类型。</typeparam>
		/// <param name="value">要转换的对象。</param>
		/// <returns>一个对象，其类型为 <typeparamref name="TOutput"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		/// <exception cref="InvalidCastException">不支持此转换。</exception>
		public static TOutput ChangeType<TInput, TOutput>(TInput value)
		{
			Converter<TInput, TOutput> converter = GetConverter<TInput, TOutput>();
			if (converter == null)
			{
				throw CommonExceptions.InvalidCast(typeof(TInput), typeof(TOutput));
			}
			return converter(value);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。
		/// </summary>
		/// <typeparam name="TOutput">要转换到的类型。</typeparam>
		/// <param name="value">要将 <paramref name="value"/> 转换的对象。</param>
		/// <returns>一个对象，其类型为 <typeparamref name="TOutput"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		/// <remarks>尽可能使用泛型方法 <see cref="ChangeType{TInput,TOutput}"/>，这样可以避免额外的类型转换。</remarks>
		/// <exception cref="InvalidCastException"><paramref name="value"/> 为 <c>null</c>，
		/// 而且 <typeparamref name="TOutput"/> 是值类型。</exception>
		/// <exception cref="InvalidCastException">不支持此转换。</exception>
		public static TOutput ChangeType<TOutput>(object value)
		{
			if (value == null)
			{
				if (typeof(TOutput).IsValueType)
				{
					throw CommonExceptions.CannotCastNullToValueType();
				}
				return default(TOutput);
			}
			Converter<object, object> converter = GetConverter(value.GetType(), typeof(TOutput));
			if (converter == null)
			{
				throw CommonExceptions.InvalidCast(value.GetType(), typeof(TOutput));
			}
			return (TOutput)converter(value);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。
		/// </summary>
		/// <param name="value">要转换的对象。</param>
		/// <param name="outputType">要将 <paramref name="value"/> 转换到的类型。</param>
		/// <returns>一个对象，其类型为 <paramref name="outputType"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		/// <remarks>尽可能使用泛型方法 <see cref="ChangeType{TInput,TOutput}"/>，这样可以避免额外的类型转换。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidCastException"><paramref name="value"/> 为 <c>null</c>，
		/// 而且 <paramref name="outputType"/> 是值类型。</exception>
		/// <exception cref="InvalidCastException">不支持此转换。</exception>
		public static object ChangeType(object value, Type outputType)
		{
			if (outputType == null)
			{
				throw CommonExceptions.ArgumentNull("outputType");
			}
			Contract.EndContractBlock();
			if (value == null)
			{
				if (outputType.IsValueType)
				{
					throw CommonExceptions.CannotCastNullToValueType();
				}
				return null;
			}
			Converter<object, object> converter = GetConverter(value.GetType(), outputType);
			if (converter == null)
			{
				throw CommonExceptions.InvalidCast(value.GetType(), outputType);
			}
			return converter(value);
		}

		#endregion // 类型转换

	}
}
