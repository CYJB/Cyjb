using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Emit;
using Cyjb.Cache;
using Cyjb.Conversions;
using Cyjb.Reflection;
using ObjectConverter = System.Converter<object?, object?>;

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
	public static class GenericConvert
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
			ArgumentNullException.ThrowIfNull(inputType);
			ArgumentNullException.ThrowIfNull(outputType);
			return typeof(Converter<,>).MakeGenericType(inputType, outputType);
		}

		#region 转换器

		/// <summary>
		/// 类型的转换器。
		/// </summary>
		private sealed class Converter
		{
			/// <summary>
			/// 类型转换。
			/// </summary>
			private readonly Conversion conversion;
			/// <summary>
			/// 泛型的类型转换器。
			/// </summary>
			private Delegate? genericConverter;
			/// <summary>
			/// 对象类型转换器。
			/// </summary>
			private ObjectConverter? objectConverter;
			/// <summary>
			/// 使用指定的转换初始化。
			/// </summary>
			/// <param name="conversion">类型转换。</param>
			public Converter(Conversion conversion)
			{
				this.conversion = conversion;
			}
			/// <summary>
			/// 使用指定的转换器初始化。
			/// </summary>
			/// <param name="converter">类型转换器。</param>
			public Converter(Delegate converter)
			{
				conversion = IdentityConversion.Default;
				genericConverter = converter;
				objectConverter = obj => converter.DynamicInvoke(obj);
			}

			/// <summary>
			/// 获取泛型的类型转换器。
			/// </summary>
			public Delegate GetGenericConverter(Type inputType, Type outputType)
			{
				genericConverter ??= BuildConverter(inputType, outputType, false, true);
				return genericConverter;
			}

			/// <summary>
			/// 获取对象的类型转换器。
			/// </summary>
			public ObjectConverter GetObjectConverter(Type inputType, Type outputType)
			{
				if (objectConverter == null)
				{
					if (conversion.ConversionType == ConversionType.Identity ||
						conversion.ConversionType == ConversionType.ImplicitReference ||
						conversion.ConversionType == ConversionType.Box ||
						conversion.ConversionType == ConversionType.Unbox)
					{
						objectConverter = defaultObjectConverter;
					}
					else if (conversion.ConversionType == ConversionType.ExplicitReference)
					{
						// 对于显式引用转换，只需要检查一下实际类型是否是 outputType 即可。
						objectConverter = obj =>
						{
							if (obj != null && !outputType.IsInstanceOfType(obj))
							{
								throw CommonExceptions.InvalidCast(obj.GetType(), outputType);
							}
							return obj;
						};
					}
					else
					{
						objectConverter = (ObjectConverter)BuildConverter(inputType, outputType, false, false);
					}
				}
				return objectConverter;
			}

			/// <summary>
			/// 构造类型转换器。
			/// </summary>
			/// <param name="inputType">要转换的对象的类型。</param>
			/// <param name="outputType">要将输入对象转换到的类型。</param>
			/// <param name="isChecked">是否执行溢出检查。</param>
			/// <param name="buildGeneric"><c>true</c> 表示需要生成泛型类型转换委托；<c>false</c> 
			/// 表示需要生成非泛型的类型转换委托。</param>
			/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。</returns>
			private Delegate BuildConverter(Type inputType, Type outputType, bool isChecked, bool buildGeneric)
			{
				DynamicMethod method = new("Converter", buildGeneric ? outputType : typeof(object),
					new[] { buildGeneric ? inputType : typeof(object) }, true);
				ILGenerator il = method.GetILGenerator();
				bool passByAddr = conversion.PassByAddr;
				if (passByAddr && (buildGeneric || !inputType.IsValueType))
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
					return method.CreateDelegate(GetConverterType(inputType, outputType));
				}
				// 从 object 拆箱得到值类型。
				ConversionFactory.GetPreDefinedConversion(typeof(object), inputType)!
					.Emit(il, typeof(object), inputType, isChecked);
				if (passByAddr)
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
				return method.CreateDelegate(typeof(ObjectConverter));
			}
		}

		/// <summary>
		/// 直接返回对象的类型转换器。
		/// </summary>
		/// <remarks>用于标识转换、隐式引用转换，不需要做任何操作；装箱转换，参数已被装箱；
		/// 拆箱转换，结果拆箱后又会被装箱为 object。</remarks>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ObjectConverter defaultObjectConverter = obj => obj;
		/// <summary>
		/// 类型转换器的存储字典。
		/// </summary>
		private static readonly SimplyCache<Tuple<Type, Type>, Converter?> converterCache = new();
		/// <summary>
		/// 类型转换器提供者的列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConcurrentDictionary<Type, IConverterProvider> providers = new(new[] {
				new KeyValuePair<Type, IConverterProvider>(StringConverterProvider.Default.OriginType,
					StringConverterProvider.Default)
			});

		/// <summary>
		/// 获取将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的转换器。
		/// 如果不存在则为 <c>null</c>。</returns>
		private static Converter? GetConverterInternal(Type inputType, Type outputType)
		{
			return converterCache.GetOrAdd(new Tuple<Type, Type>(inputType, outputType), types =>
			{
				Type inputType = types.Item1;
				Type outputType = types.Item2;
				Conversion? conversion = ConversionFactory.GetConversion(inputType, outputType);
				if (conversion != null)
				{
					return new Converter(conversion);
				}
				if (providers.TryGetValue(inputType, out IConverterProvider? provider))
				{
					Delegate? dlg = provider.GetConverterTo(outputType);
					if (ConverterProvider.IsValidConverter(dlg, inputType, outputType))
					{
						return new Converter(dlg);
					}
				}
				if (providers.TryGetValue(outputType, out provider))
				{
					Delegate? dlg = provider.GetConverterFrom(inputType);
					if (ConverterProvider.IsValidConverter(dlg, inputType, outputType))
					{
						return new Converter(dlg);
					}
				}
				return null;
			});
		}

		/// <summary>
		/// 获取将对象从 <typeparamref name="TInput"/> 类型转换为 <typeparamref name="TOutput"/> 类型的转换器。
		/// </summary>
		/// <typeparam name="TInput">输入对象的类型。</typeparam>
		/// <typeparam name="TOutput">输出对象的类型。</typeparam>
		/// <returns>将对象从 <typeparamref name="TInput"/> 类型转换为 <typeparamref name="TOutput"/> 类型的转换器。
		/// 如果不存在则为 <c>null</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 获取将对象从一种类型转换为另一种类型的转换器。
		/// </summary>
		/// </overloads>
		public static Converter<TInput, TOutput>? GetConverter<TInput, TOutput>()
		{
			Type inputType = typeof(TInput);
			Type outputType = typeof(TOutput);
			return (Converter<TInput, TOutput>?)GetConverterInternal(inputType, outputType)?.GetGenericConverter(inputType, outputType);
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
		/// <exception cref="ArgumentException"><paramref name="inputType"/> 包含泛型参数。</exception>
		/// <exception cref="ArgumentException"><paramref name="outputType"/> 包含泛型参数。</exception>
		public static ObjectConverter? GetConverter(Type inputType, Type outputType)
		{
			ArgumentNullException.ThrowIfNull(inputType);
			ArgumentNullException.ThrowIfNull(outputType);
			if (inputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(inputType);
			}
			if (outputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(outputType);
			}
			return GetConverterInternal(inputType, outputType)?.GetObjectConverter(inputType, outputType);
		}

		/// <summary>
		/// 返回对象能否从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>如果对象能够从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型，
		/// 则为 <c>true</c>。否则为 <c>false</c>。</returns>
		/// <remarks>与 <see cref="TypeUtil.IsImplicitFrom"/> 或 <see cref="TypeUtil.IsExplicitFrom"/> 不同，
		/// 这里会考虑预定义的隐式、显式类型转换，用户自定义的类型转换（包括 <c>implicit</c> 和 <c>explicit</c>），
		/// 以及通过 <see cref="AddConverter{TInput, TOutput}"/> 和 <see cref="AddConverterProvider"/> 
		/// 方法注册的类型转换方法。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="inputType"/> 包含泛型参数。</exception>
		/// <exception cref="ArgumentException"><paramref name="outputType"/> 包含泛型参数。</exception>
		public static bool CanChangeType(Type inputType, Type outputType)
		{
			ArgumentNullException.ThrowIfNull(inputType);
			ArgumentNullException.ThrowIfNull(outputType);
			if (inputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(inputType);
			}
			if (outputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(outputType);
			}
			if (ConversionFactory.GetConversion(inputType, outputType) != null)
			{
				return true;
			}
			return GetConverterInternal(inputType, outputType) != null;
		}

		/// <summary>
		/// 添加将对象从 <typeparamref name="TInput"/> 类型转换为 <typeparamref name="TOutput"/> 类型的转换器。
		/// </summary>
		/// <typeparam name="TInput">输入对象的类型。</typeparam>
		/// <typeparam name="TOutput">输出对象的类型。</typeparam>
		/// <param name="converter">将对象从 <typeparamref name="TInput"/> 类型转换为 
		/// <typeparamref name="TOutput"/> 类型的转换器。</param>
		/// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <c>null</c>。</exception>
		/// <remarks><paramref name="converter"/> 不会覆盖预定义的隐式、显式类型转换和用户自定义的类型转换。
		/// 对于相同输入/输出类型的 <paramref name="converter"/>，后设置的会覆盖先设置的，以及任何 
		/// <see cref="IConverterProvider"/> 提供的类型转换方法。</remarks>
		public static void AddConverter<TInput, TOutput>(Converter<TInput, TOutput> converter)
		{
			ArgumentNullException.ThrowIfNull(converter);
			ConverterProvider provider = new(converter, typeof(TInput), typeof(TOutput));
			providers.AddOrUpdate(provider.OriginType, provider, (type, old) => ConverterProvider.Combine(old, provider));
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
			ArgumentNullException.ThrowIfNull(provider);
			ArgumentNullException.ThrowIfNull(provider.OriginType);
			providers.AddOrUpdate(provider.OriginType, provider, (type, old) => ConverterProvider.Combine(old, provider));
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
		/// <overloads>
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。
		/// </summary>
		/// </overloads>
		public static TOutput ChangeType<TInput, TOutput>(TInput value)
		{
			Converter<TInput, TOutput>? converter = GetConverter<TInput, TOutput>();
			if (converter == null)
			{
				throw CommonExceptions.InvalidCast(typeof(TInput), typeof(TOutput));
			}
			return converter(value);
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
		public static object? ChangeType(object? value, Type outputType)
		{
			ArgumentNullException.ThrowIfNull(outputType);
			if (value == null)
			{
				if (outputType.IsValueType)
				{
					throw CommonExceptions.CannotCastNullToValueType();
				}
				return null;
			}
			ObjectConverter? converter = GetConverter(value.GetType(), outputType);
			if (converter == null)
			{
				throw CommonExceptions.InvalidCast(value.GetType(), outputType);
			}
			return converter(value);
		}

		#endregion // 类型转换

	}
}
