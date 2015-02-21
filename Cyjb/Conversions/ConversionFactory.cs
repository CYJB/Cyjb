using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 类型转换的工厂。
	/// </summary>
	internal static class ConversionFactory
	{
		/// <summary>
		/// 类型转换器提供者的列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConcurrentDictionary<Type, IConverterProvider> providers =
			new ConcurrentDictionary<Type, IConverterProvider>(new[] {
				new KeyValuePair<Type, IConverterProvider>(StringConverterProvider.Default.OriginType,
					StringConverterProvider.Default)
			});
		/// <summary>
		/// 用户自定义的类型转换器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConcurrentDictionary<Tuple<Type, Type>, Conversion> userDefinedConverers =
			new ConcurrentDictionary<Tuple<Type, Type>, Conversion>();
		/// <summary>
		/// 返回将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换，如果不存在则为 <c>null</c>。</returns>
		public static Conversion GetConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null);
			// 不能转换到或转换自 void 类型。
			if (inputType == typeof(void) || outputType == typeof(void))
			{
				return null;
			}
			// 检索已创建的用户自定义类型转换器。
			Tuple<Type, Type> key = new Tuple<Type, Type>(inputType, outputType);
			Conversion conversion;
			if (userDefinedConverers.TryGetValue(key, out conversion))
			{
				return conversion;
			}
			// 判断预定义类型转换。
			conversion = GetPreDefinedConversion(inputType, outputType);
			if (conversion != null)
			{
				return conversion;
			}
			// 判断用户自定义类型转换。
			conversion = GetUserDefinedConversion(inputType, outputType);
			if (conversion != null)
			{
				return conversion;
			}
			// 类型转换提供者。
			IConverterProvider provider;
			Delegate converterDelegate = null;
			if (providers.TryGetValue(inputType, out provider))
			{
				Delegate dlg = provider.GetConverterTo(outputType);
				if (!provider.IsValidConverterTo(dlg, outputType))
				{
					converterDelegate = dlg;
				}
			}
			if (converterDelegate == null && providers.TryGetValue(outputType, out provider))
			{
				Delegate dlg = provider.GetConverterFrom(inputType);
				if (provider.IsValidConverterFrom(dlg, inputType))
				{
					converterDelegate = dlg;
				}
			}
			if (converterDelegate != null)
			{
				return userDefinedConverers.GetOrAdd(key, new DelegateConversion(converterDelegate));
			}
			return null;
		}
		/// <summary>
		/// 添加指定的类型转换器提供者。
		/// </summary>
		/// <param name="provider">要添加的类型转换器提供者。</param>
		public static void AddConverterProvider(IConverterProvider provider)
		{
			Contract.Requires(provider != null && provider.OriginType != null);
			providers.AddOrUpdate(provider.OriginType, provider, (type, old) => ConverterProvider.Combine(old, provider));
		}
		/// <summary>
		/// 返回的将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的预定义类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的预定义类型转换，如果不存在则为 <c>null</c>。</returns>
		public static Conversion GetPreDefinedConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null &&
				inputType != typeof(void) && outputType != typeof(void));
			// 测试相等转换。
			if (inputType.IsEquivalentTo(outputType))
			{
				return IdentityConversion.Default;
			}
			if (inputType.IsValueType)
			{
				if (outputType.IsValueType)
				{
					// 值类型间转换。
					return GetBetweenValueTypeConversion(inputType, outputType);
				}
				// 装箱转换。
				if (outputType.IsAssignableFrom(inputType))
				{
					return BoxConversion.Default;
				}
				Type inputUnderlyingType = Nullable.GetUnderlyingType(inputType);
				if (inputUnderlyingType != null && outputType.IsAssignableFrom(inputUnderlyingType))
				{
					// 装箱为可空类型的内部类型实现的接口。
					return BoxConversion.Default;
				}
				return null;
			}
			if (outputType.IsValueType)
			{
				// 拆箱转换。
				if (inputType.IsAssignableFrom(outputType))
				{
					return UnboxConversion.Default;
				}
				Type outputUnderlyingType = Nullable.GetUnderlyingType(outputType);
				if (outputUnderlyingType != null && inputType.IsAssignableFrom(outputUnderlyingType))
				{
					return UnboxConversion.Default;
				}
				return null;
			}
			// 隐式引用转换。
			if (outputType.IsAssignableFrom(inputType))
			{
				return IdentityConversion.ImplicitReference;
			}
			// 显式引用转换。
			return GetExplicitRefConversion(inputType, outputType);
		}

		#region 值类型间转换

		/// <summary>
		/// 返回从值类型转换为值类型的类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>从值类型转换为值类型的类型转换，如果不存在则为 <c>null</c>。</returns>
		private static Conversion GetBetweenValueTypeConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null);
			Contract.Requires(inputType.IsValueType && outputType.IsValueType);
			TypeCode inputTypeCode = Type.GetTypeCode(inputType);
			TypeCode outputTypeCode = Type.GetTypeCode(outputType);
			// 数值或枚举转换。
			if (inputTypeCode.IsNumeric() && outputTypeCode.IsNumeric())
			{
				return GetNumericOrEnumConversion(inputType, inputTypeCode, outputType, outputTypeCode);
			}
			// 可空类型转换。
			Type inputUnderlyingType = Nullable.GetUnderlyingType(inputType);
			Type outputUnderlyingType = Nullable.GetUnderlyingType(outputType);
			if (inputUnderlyingType != null)
			{
				inputTypeCode = Type.GetTypeCode(inputUnderlyingType);
				if (outputUnderlyingType == null)
				{
					// 可空类型 S? 到非可空值类型 T 的转换。
					// 1. 可空类型为 null，引发异常。
					// 2. 将可空类型解包
					// 3. 执行从 S 到 T 的预定义类型转换。
					// 这里 S 和 T 都是值类型，可能的预定义类型转换只有标识转换和数值转换。
					if (inputUnderlyingType == outputType ||
						(inputTypeCode.IsNumeric() && outputTypeCode.IsNumeric()))
					{
						return FromNullableConversion.Default;
					}
					return null;
				}
				outputTypeCode = Type.GetTypeCode(outputUnderlyingType);
				// 可空类型间转换，从 S? 到 T?，与上面同理，但此时不可能有 S==T。
				if (inputTypeCode.IsNumeric() && outputTypeCode.IsNumeric())
				{
					Conversion conversion = GetNumericOrEnumConversion(inputUnderlyingType, inputTypeCode,
						outputUnderlyingType, outputTypeCode);
					return conversion.ConversionType.IsImplicit()
						? BetweenNullableConversion.Implicit : BetweenNullableConversion.Explicit;
				}
				return null;
			}
			if (outputUnderlyingType != null)
			{
				// 非可空类型到可空类型转换，与上面同理。
				if (inputType == outputUnderlyingType)
				{
					return ToNullableConversion.Implicit;
				}
				outputTypeCode = Type.GetTypeCode(outputUnderlyingType);
				if (inputType.IsNumeric() && outputTypeCode.IsNumeric())
				{
					Conversion conversion = GetNumericOrEnumConversion(inputType, inputTypeCode,
						outputUnderlyingType, outputTypeCode);
					return conversion.ConversionType.IsImplicit() ?
						ToNullableConversion.Implicit : ToNullableConversion.Explicit;
				}
			}
			return null;
		}

		#endregion // 值类型间转换

		#region 数值或枚举转换

		/// <summary>
		/// 返回将对象从数值类型 <paramref name="inputType"/> 转换为数值类型 <paramref name="outputType"/> 
		/// 的类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="inputTypeCode">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="outputTypeCode">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换，如果不存在则为 <c>null</c>。</returns>
		public static Conversion GetNumericOrEnumConversion(Type inputType, TypeCode inputTypeCode,
			Type outputType, TypeCode outputTypeCode)
		{
			Contract.Requires(inputType != null && outputType != null);
			Contract.Requires(Type.GetTypeCode(inputType) == inputTypeCode &&
				Type.GetTypeCode(outputType) == outputTypeCode);
			Contract.Requires(inputTypeCode.IsNumeric() && outputTypeCode.IsNumeric());
			// 处理输入 decimal 类型。
			if (inputTypeCode == TypeCode.Decimal)
			{
				return outputType.IsEnum ? DecimalConversion.ExplicitEnum : DecimalConversion.ExplicitNumeric;
			}
			// 处理输出 decimal 类型。
			if (outputTypeCode == TypeCode.Decimal)
			{
				if (inputType.IsEnum)
				{
					return DecimalConversion.ExplicitEnum;
				}
				if (inputType == typeof(float) || inputType == typeof(double))
				{
					return DecimalConversion.ExplicitNumeric;
				}
				return DecimalConversion.ImplicitNumeric;
			}
			Conversion conversion = GetNumericConversion(inputTypeCode, outputTypeCode);
			if (inputType.IsEnum || outputType.IsEnum)
			{
				// 将类型转换的类型修正为 EnumConversion。
				NumericConversion numericConv = conversion as NumericConversion;
				return numericConv == null ? IdentityConversion.ExplicitEnum :
					new NumericConversion(ConversionType.EnumConversion, numericConv);
			}
			return conversion;
		}
		/// <summary>
		/// 返回将对象从数值类型 <paramref name="inputTypeCode"/> 转换为数值类型 <paramref name="outputTypeCode"/> 
		/// 的类型转换。要求类型不能是 <see cref="TypeCode.Decimal"/>。
		/// </summary>
		/// <param name="inputTypeCode">要转换的对象的类型。</param>
		/// <param name="outputTypeCode">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputTypeCode"/> 类型转换为 <paramref name="outputTypeCode"/> 
		/// 类型的类型转换。</returns>
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private static Conversion GetNumericConversion(TypeCode inputTypeCode, TypeCode outputTypeCode)
		{
			Contract.Requires(inputTypeCode.IsNumeric() && inputTypeCode != TypeCode.Decimal);
			Contract.Requires(outputTypeCode.IsNumeric() && outputTypeCode != TypeCode.Decimal);
			Contract.Ensures(Contract.Result<Conversion>() != null);
			bool fromUnsigned = inputTypeCode.IsUnsigned();
			switch (outputTypeCode)
			{
				case TypeCode.Char:
					if (inputTypeCode == TypeCode.Byte || inputTypeCode == TypeCode.UInt16)
					{
						return IdentityConversion.ExplicitNumeric;
					}
					return fromUnsigned ? NumericConversion.UInt16Un : NumericConversion.UInt16;
				case TypeCode.SByte:
					return fromUnsigned ? NumericConversion.SByteUn : NumericConversion.SByte;
				case TypeCode.Byte:
					return fromUnsigned ? NumericConversion.ByteUn : NumericConversion.Byte;
				case TypeCode.Int16:
					if (inputTypeCode == TypeCode.SByte || inputTypeCode == TypeCode.Byte)
					{
						return IdentityConversion.ImplicitNumeric;
					}
					return fromUnsigned ? NumericConversion.Int16Un : NumericConversion.Int16;
				case TypeCode.UInt16:
					if (inputTypeCode == TypeCode.Byte || inputTypeCode == TypeCode.Char)
					{
						return IdentityConversion.ImplicitNumeric;
					}
					return fromUnsigned ? NumericConversion.UInt16Un : NumericConversion.UInt16;
				case TypeCode.Int32:
					if (inputTypeCode < outputTypeCode || inputTypeCode == TypeCode.Char)
					{
						return IdentityConversion.ImplicitNumeric;
					}
					return fromUnsigned ? NumericConversion.Int32Un : NumericConversion.Int32;
				case TypeCode.UInt32:
					if (fromUnsigned)
					{
						if (inputTypeCode == TypeCode.UInt64)
						{
							return NumericConversion.UInt32Un;
						}
						return IdentityConversion.ImplicitNumeric;
					}
					return inputTypeCode < outputTypeCode ? NumericConversion.UInt32Empty : NumericConversion.UInt32;
				case TypeCode.Int64:
					if (inputTypeCode < outputTypeCode || inputTypeCode == TypeCode.Char)
					{
						return fromUnsigned ? NumericConversion.UInt64Implicit : NumericConversion.Int64Implicit;
					}
					return fromUnsigned ? NumericConversion.Int64UnExplicit : NumericConversion.Int64Explicit;
				case TypeCode.UInt64:
					if (fromUnsigned)
					{
						return NumericConversion.UInt64Implicit;
					}
					if (inputTypeCode < outputTypeCode)
					{
						return NumericConversion.UInt64Explicit;
					}
					return NumericConversion.UInt64UnExplicit;
				case TypeCode.Single:
					if (inputTypeCode == TypeCode.Double)
					{
						return NumericConversion.SingleExplicit;
					}
					if (inputTypeCode == TypeCode.UInt32 || inputTypeCode == TypeCode.UInt64)
					{
						return NumericConversion.SingleUnImplicit;
					}
					return NumericConversion.SingleImplicit;
				case TypeCode.Double:
					if (inputTypeCode == TypeCode.UInt32 || inputTypeCode == TypeCode.UInt64)
					{
						return NumericConversion.DoubleUn;
					}
					return NumericConversion.Double;
			}
			throw CommonExceptions.Unreachable();
		}

		#endregion // 数值或枚举转换

		#region 显式引用类型转换或拆箱转换

		/// <summary>
		/// 测试是否是显式引用类型转换或拆箱转换。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>
		/// <returns>如果是显式引用类型转换或拆箱转换，则为相应的 IL 生成器；否则为 <c>null</c>。</returns>
		private static Conversion GetExplicitRefConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null);
			Contract.Requires(inputType != typeof(void) && outputType != typeof(void));
			if (inputType.IsAssignableFrom(outputType))
			{
				return CastClassConversion.Default;
			}
			if (inputType.IsInterface)
			{
				if (outputType.IsInterface)
				{
					// 任意接口间的转换。
					return CastClassConversion.Default;
				}
				if (outputType.IsClass && !outputType.IsSealed)
				{
					// 任意接口到非密封类型的转换。
					return CastClassConversion.Default;
				}
				// T[] 实现的接口转换到 T[]。
				if (outputType.IsArray && IsIListOrBase(inputType) &&
					CanReferenceConvert(inputType.GetGenericArguments()[0], outputType.GetElementType()))
				{
					return CastClassConversion.Default;
				}
				return null;
			}
			if (outputType.IsInterface)
			{
				if (inputType.IsClass && !inputType.IsSealed)
				{
					// 任意非密封类型到接口的转换。
					return CastClassConversion.Default;
				}
				// T[] 转换到实现的接口。
				if (inputType.IsArray && IsIListOrBase(outputType) &&
					CanReferenceConvert(inputType.GetElementType(), outputType.GetGenericArguments()[0]))
				{
					return CastClassConversion.Default;
				}
				return null;
			}
			if (inputType.IsArray)
			{
				// 元素类型可以显式引用转换的等秩数组间的转换。
				if (outputType.IsArray && inputType.GetArrayRank() == outputType.GetArrayRank() &&
					CanReferenceConvert(inputType.GetElementType(), outputType.GetElementType()))
				{
					return CastClassConversion.Default;
				}
				return null;
			}
			// 泛型委托间的协变和逆变。
			return GetDelegateConversion(inputType, outputType);
		}
		/// <summary>
		/// 返回指定类型是否是 <see cref="IList{T}"/> 或者其父接口。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果指定类型是 <see cref="IList{T}"/> 或者其父接口，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		private static bool IsIListOrBase(Type type)
		{
			Contract.Requires(type != null);
			if (!type.IsInterface || !type.IsGenericType)
			{
				return false;
			}
			return type.GetGenericTypeDefinition().IsIListOrBase();
		}
		/// <summary>
		/// 获取是否存在从 <paramref name="inputType"/> 到 <paramref name="outputType"/> 的预定义引用转换。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>>
		/// <returns>如果存在引用转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool CanReferenceConvert(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null);
			if (inputType.IsValueType || outputType.IsValueType)
			{
				return false;
			}
			Conversion conversion = GetPreDefinedConversion(inputType, outputType);
			return conversion != null && conversion.ConversionType.IsReference();
		}
		/// <summary>
		/// 返回泛型委托间的协变和逆变类型转换。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>
		/// <returns>如果是泛型委托间的协变和逆变类型转换，则为相应的 IL 生成器；否则为 <c>null</c>。</returns>>
		private static Conversion GetDelegateConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null);
			if (!inputType.IsGenericType || !outputType.IsGenericType ||
				!inputType.IsSubclassOf(typeof(Delegate)) || !outputType.IsSubclassOf(typeof(Delegate)))
			{
				return null;
			}
			Type typeDefinition = inputType.GetGenericTypeDefinition();
			if (typeDefinition != outputType.GetGenericTypeDefinition())
			{
				return null;
			}
			Type[] args = typeDefinition.GetGenericArguments();
			Type[] inputArgs = inputType.GetGenericArguments();
			Type[] outputArgs = outputType.GetGenericArguments();
			for (int i = 0; i < args.Length; i++)
			{
				if (!CheckGenericArguments(args[i], inputArgs[i], outputArgs[i]))
				{
					return null;
				}
			}
			return CastClassConversion.Default;
		}
		/// <summary>
		/// 检查泛型类型参数是否满足泛型委托类型的显示类型转换。
		/// </summary>
		/// <param name="arg">类型参数定义。</param>
		/// <param name="inputArg">输入泛型类型参数。</param>
		/// <param name="outputArg">输出泛型类型参数。</param>
		/// <returns>如果满足泛型委托类型的显示类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool CheckGenericArguments(Type arg, Type inputArg, Type outputArg)
		{
			Contract.Requires(arg != null && inputArg != null && outputArg != null);
			if (inputArg == outputArg)
			{
				return true;
			}
			GenericParameterAttributes attrs = arg.GenericParameterAttributes & GenericParameterAttributes.VarianceMask;
			if (attrs == GenericParameterAttributes.None || inputArg.IsValueType || outputArg.IsValueType)
			{
				return false;
			}
			if (attrs == GenericParameterAttributes.Contravariant)
			{
				return true;
			}
			Conversion conversion = GetConversion(inputArg, outputArg);
			return conversion != null && conversion.ConversionType.IsReference();
		}

		#endregion // 显式引用类型转换或拆箱转换

		/// <summary>
		/// 返回的将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的用户自定义类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的用户自定义类型转换，如果不存在则为 <c>null</c>。</returns>
		private static Conversion GetUserDefinedConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null &&
				inputType != typeof(void) && outputType != typeof(void));
			// 判断可空类型。
			Type inputUnderlyingType = inputType.GetNonNullableType();
			Type outputUnderlyingType = outputType.GetNonNullableType();
			MethodInfo method = UserConversionCache.GetConversion(inputUnderlyingType, outputUnderlyingType);
			if (method == null)
			{
				return null;
			}
			Conversion conversion = new UserConversion(method);
			// 存入缓存。
			Type methodInputType = method.GetParametersNoCopy()[0].ParameterType;
			Tuple<Type, Type> key = new Tuple<Type, Type>(inputType, outputType);
			if (inputType != methodInputType || outputType != method.ReturnType)
			{
				conversion = userDefinedConverers.GetOrAdd(new Tuple<Type, Type>(methodInputType, method.ReturnType),
					conversion);
			}
			if (inputUnderlyingType != inputType || outputUnderlyingType != outputType)
			{
				userDefinedConverers.TryAdd(new Tuple<Type, Type>(inputUnderlyingType, outputUnderlyingType), conversion);
			}
			if (inputUnderlyingType == inputType || !methodInputType.IsValueType || methodInputType.IsNullable())
			{
				return userDefinedConverers.GetOrAdd(key, conversion);
			}
			// 需要将输入的 Nullable<T> 解包。
			if (outputUnderlyingType != outputType || !outputType.IsValueType)
			{
				// outputType 可以为 null（引用类型或 Nullable<T>）。
				return userDefinedConverers.GetOrAdd(key, BetweenNullableConversion.UserDefined);
			}
			return userDefinedConverers.GetOrAdd(key, FromNullableConversion.UserDefined);
		}
		/// <summary>
		/// 返回 <paramref name="inputType"/> 类型和 <paramref name="outputType"/> 类型之间的标准转换类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns><paramref name="inputType"/> 类型和 <paramref name="outputType"/> 类型之间的标准转换类型。</returns>
		/// <remarks><para>这里标准转换指标准显式转换，包含了所有预定义隐式转换和预定义显式转换的子集，
		/// 该子集是预定义隐式转换反向的转换。也就是说，如果存在从 A 类型到 B 类型的预定义隐式转换，
		/// 那么 A 类型和 B 类型之间存在标准转换（A 到 B 或 B 到 A）。</para>
		/// <para>如果不存在标准类型转换，则总是返回 <see cref="ConversionType.None"/>，即使存在其它的类型转换。</para>
		/// </remarks>
		public static ConversionType GetStandardConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null &&
				inputType != typeof(void) && outputType != typeof(void));
			Conversion conversion = GetPreDefinedConversion(inputType, outputType);
			// 不存在预定义类型转换。
			if (conversion == null)
			{
				return ConversionType.None;
			}
			// 隐式类型转换。
			if (conversion.ConversionType.IsImplicit())
			{
				return conversion.ConversionType;
			}
			switch (conversion.ConversionType)
			{
				case ConversionType.EnumConversion:
					// 不包含显式枚举转换。
					return ConversionType.None;
				case ConversionType.UnboxConversion:
					// 完整包含拆箱转换。
					return ConversionType.None;
				case ConversionType.ExplicitNumericConversion:
				case ConversionType.ExplicitNullableConversion:
					// 包含部分转换，需要判断是否存在反向隐式转换。
					// 此时 inputType 和 outputType 一定都是值类型。
					Conversion reversedConversion = GetBetweenValueTypeConversion(outputType, inputType);
					Contract.Assume(reversedConversion != null);
					return reversedConversion.ConversionType.IsImplicit() ?
						conversion.ConversionType : ConversionType.None;
				case ConversionType.ExplicitReferenceConversion:
					// 包含部分转换，需要判断是否存在反向转换。
					if (inputType.IsAssignableFrom(outputType))
					{
						return ConversionType.ExplicitReferenceConversion;
					}
					return ConversionType.None;
			}
			return ConversionType.None;
		}
	}
}
