using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;
using Cyjb.Conversions;
using Cyjb.Reflection;

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
	/// </remarks>
	public static class Convert
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
		private static readonly ConcurrentDictionary<Tuple<Type, Type>, Converter> converterDict =
			new ConcurrentDictionary<Tuple<Type, Type>, Converter>();
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
			return converterDict.GetOrAdd(new Tuple<Type, Type>(inputType, outputType), types =>
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
			string methodName = string.Concat("Converter_", inputType.FullName(), "_To_", outputType.FullName());
			DynamicMethod method = new DynamicMethod(methodName, buildGeneric ? outputType : typeof(object),
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
		/// 获取将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的转换器类型。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static ConversionType GetConvertType(Type inputType, Type outputType)
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
				Conversion conversion = ConversionFactory.GetConversion(inputType, outputType);
				if (conversion != null)
				{
					return conversion.ConversionType;
				}
			}
			catch { }
			return ConversionType.None;
		}
		/// <summary>
		/// 返回表示对象能否从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的布尔值。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>如果对象能够从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型，
		/// 则为 <c>true</c>。否则为 <c>false</c>。</returns>
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
				throw CommonExceptions.InvalidCastFromTo(typeof(TInput), typeof(TOutput));
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
				throw CommonExceptions.InvalidCastFromTo(value.GetType(), typeof(TOutput));
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
				throw CommonExceptions.InvalidCastFromTo(value.GetType(), outputType);
			}
			return converter(value);
		}

		#endregion // 类型转换

		#region 进制转换

		#region ToSByte

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>8</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>8</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 
		/// <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 
		/// <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 
		/// <see cref="SByte.MinValue"/> 或大于 <see cref="SByte.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(string value, int fromBase)
		{
			return unchecked((sbyte)ToByte(value, fromBase));
		}

		#endregion // ToInt16

		#region ToInt16

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>16</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>16</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="Int16.MinValue"/> 或大于 
		/// <see cref="Int16.MaxValue"/> 的数字。</exception>
		public static short ToInt16(string value, int fromBase)
		{
			return unchecked((short)ToUInt16(value, fromBase));
		}

		#endregion // ToInt16

		#region ToInt32

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>32</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>32</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="Int32.MinValue"/> 或大于 
		/// <see cref="Int32.MaxValue"/> 的数字。</exception>
		public static int ToInt32(string value, int fromBase)
		{
			return unchecked((int)ToUInt32(value, fromBase));
		}

		#endregion // ToInt32

		#region ToInt64

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>64</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>64</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="Int64.MinValue"/> 或大于 
		/// <see cref="Int64.MaxValue"/> 的数字。</exception>
		public static long ToInt64(string value, int fromBase)
		{
			return unchecked((long)ToUInt64(value, fromBase));
		}

		#endregion // ToInt64

		#region ToByte

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>8</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>8</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="Byte.MinValue"/> 或大于 
		/// <see cref="Byte.MaxValue"/> 的数字。</exception>
		public static byte ToByte(string value, int fromBase)
		{
			uint result = ToUInt32(value, fromBase);
			if (result > byte.MaxValue)
			{
				throw CommonExceptions.OverflowByte();
			}
			return unchecked((byte)result);
		}

		#endregion // ToInt16

		#region ToUInt16

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>16</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>16</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="UInt16.MinValue"/> 或大于 
		/// <see cref="UInt16.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(string value, int fromBase)
		{
			uint result = ToUInt32(value, fromBase);
			if (result > ushort.MaxValue)
			{
				throw CommonExceptions.OverflowUInt16();
			}
			return unchecked((ushort)result);
		}

		#endregion // ToUInt64

		#region ToUInt32

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>32</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>32</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="UInt32.MinValue"/> 或大于 
		/// <see cref="UInt32.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(string value, int fromBase)
		{
			// 使用内置方法，会快一些。
			if (fromBase == 2 || fromBase == 8 || fromBase == 10 || fromBase == 16)
			{
				return System.Convert.ToUInt32(value, fromBase);
			}
			// 使用自己的算法。
			if (value == null)
			{
				return 0U;
			}
			CheckBaseConvert(value, fromBase);
			uint result = 0;
			uint uBase = (uint)fromBase;
			for (int i = 0; i < value.Length; i++)
			{
				int t = GetBaseValue(value[i], fromBase);
				if (t < 0)
				{
					if (i == 0)
					{
						throw CommonExceptions.NoParsibleDigits();
					}
					throw CommonExceptions.ExtraJunkAtEnd();
				}
				uint next = unchecked(result * uBase + (uint)t);
				// 判断是否超出 UInt32 的范围。
				if (next < result)
				{
					throw CommonExceptions.OverflowUInt32();
				}
				result = next;
			}
			return result;
		}

		#endregion // ToUInt64

		#region ToUInt64

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>64</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>64</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="UInt64.MinValue"/> 或大于 
		/// <see cref="UInt64.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(string value, int fromBase)
		{
			// 使用内置方法，会快一些。
			if (fromBase == 2 || fromBase == 8 || fromBase == 10 || fromBase == 16)
			{
				return System.Convert.ToUInt64(value, fromBase);
			}
			// 使用自己的算法。
			if (value == null)
			{
				return 0UL;
			}
			CheckBaseConvert(value, fromBase);
			ulong result = 0;
			ulong ulBase = (ulong)fromBase;
			for (int i = 0; i < value.Length; i++)
			{
				int t = GetBaseValue(value[i], fromBase);
				if (t < 0)
				{
					if (i == 0)
					{
						throw CommonExceptions.NoParsibleDigits();
					}
					throw CommonExceptions.ExtraJunkAtEnd();
				}
				ulong next = unchecked(result * ulBase + (ulong)t);
				// 判断是否超出 UInt64 的范围。
				if (next < result)
				{
					throw CommonExceptions.OverflowUInt64();
				}
				result = next;
			}
			return result;
		}

		#endregion // ToUInt64

		#region ToString

		/// <summary>
		/// 将 <c>8</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>8</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 
		/// <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <overloads>
		/// <summary>
		/// 将给定的整数值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static string ToString(this sbyte value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			bool neg = false;
			ulong ulValue;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				// 这里必须保证位数相同。
				ulValue = (byte)value;
			}
			char[] buffer = new char[8];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>16</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>16</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this short value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			bool neg = false;
			ulong ulValue;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				// 这里必须保证位数相同。
				ulValue = (ushort)value;
			}
			char[] buffer = new char[16];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>32</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>32</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this int value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			bool neg = false;
			ulong ulValue;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				// 这里必须保证位数相同。
				ulValue = (uint)value;
			}
			char[] buffer = new char[32];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>64</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>64</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this long value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			bool neg = false;
			ulong ulValue;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				ulValue = (ulong)value;
			}
			char[] buffer = new char[64];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>8</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>8</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this byte value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			char[] buffer = new char[8];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>16</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>16</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		[CLSCompliant(false)]
		public static string ToString(this ushort value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			char[] buffer = new char[16];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>32</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>32</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		[CLSCompliant(false)]
		public static string ToString(this uint value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			char[] buffer = new char[32];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>64</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>64</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		[CLSCompliant(false)]
		public static string ToString(this ulong value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			Contract.EndContractBlock();
			char[] buffer = new char[64];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>64</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="buffer">字符串的缓冲区。</param>
		/// <param name="value">要转换的 <c>64</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>转换后字符串的起始索引。</returns>
		private static int ConvertBase(char[] buffer, ulong value, int toBase)
		{
			Contract.Requires(buffer != null && toBase >= 2 && toBase <= 36);
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() < buffer.Length);
			// 从后向前转换，不必反转字符串。
			ulong ulBase = (ulong)toBase;
			int idx = buffer.Length - 1;
			do
			{
				ulong quot = value / ulBase;
				buffer[idx--] = CharExt.BaseDigits[value - quot * ulBase];
				value = quot;
			} while (value > 0);
			return idx + 1;
		}

		#endregion

		/// <summary>
		/// 对给定的基数和字符串进行检查。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="FormatException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		private static void CheckBaseConvert(string value, int fromBase)
		{
			// 基数检查。
			if (fromBase < 3 || fromBase > 36)
			{
				throw CommonExceptions.InvalidBase("fromBase", fromBase);
			}
			if (value.Length == 0)
			{
				throw CommonExceptions.NoParsibleDigits();
			}
			// 负号检查。
			if (value[0] == '-')
			{
				throw CommonExceptions.BaseConvertNegativeValue();
			}
		}
		/// <summary>
		/// 返回指定字符以指定的基表示的值。
		/// </summary>
		/// <param name="ch">要获取值的字符，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="ch"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>如果字符有效，则返回字符对应的值。否则返回 <c>-1</c>。</returns>
		private static int GetBaseValue(char ch, int fromBase)
		{
			Contract.Requires(fromBase >= 2 && fromBase <= 36);
			int value = -1;
			if (ch < 'A')
			{
				if (ch >= '0' && ch <= '9')
				{
					value = ch - '0';
				}
			}
			else if (ch < 'a')
			{
				if (ch <= 'Z')
				{
					value = ch - 'A' + 10;
				}
			}
			else if (ch <= 'z')
			{
				value = ch - 'a' + 10;
			}
			if (value < fromBase)
			{
				return value;
			}
			return -1;
		}

		#endregion // 进制转换

	}
}
