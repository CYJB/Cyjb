using System.Collections.Concurrent;
using System.Diagnostics;
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
		/// 已创建的自定义类型转换器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConcurrentDictionary<Tuple<Type, Type>, Conversion> userConversions = new();

		/// <summary>
		/// 返回将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 类型的类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换，如果不存在则为 <c>null</c>。</returns>
		public static Conversion? GetConversion(Type inputType, Type outputType)
		{
			// 不能转换到或转换自 void 类型。
			if (inputType == typeof(void) || outputType == typeof(void))
			{
				return null;
			}
			// 检索已创建的类型转换器。
			// 检查预定义类型转换。
			Conversion? conversion = GetPreDefinedConversion(inputType, outputType);
			if (conversion != null)
			{
				return conversion;
			}
			// 检查自定义类型转换。
			return GetUserDefinedConversion(inputType, outputType);
		}

		/// <summary>
		/// 返回的将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的预定义类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的预定义类型转换，如果不存在则为 <c>null</c>。</returns>
		public static Conversion? GetPreDefinedConversion(Type inputType, Type outputType)
		{
			if (inputType == typeof(void) || outputType == typeof(void))
			{
				return null;
			}
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
				Type? inputUnderlyingType = Nullable.GetUnderlyingType(inputType);
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
				Type? outputUnderlyingType = Nullable.GetUnderlyingType(outputType);
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

		/// <summary>
		/// 返回从值类型转换为值类型的类型转换。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>从值类型转换为值类型的类型转换，如果不存在则为 <c>null</c>。</returns>
		private static Conversion? GetBetweenValueTypeConversion(Type inputType, Type outputType)
		{
			TypeCode inputTypeCode = Type.GetTypeCode(inputType);
			TypeCode outputTypeCode = Type.GetTypeCode(outputType);
			// 数值或枚举转换。
			if (inputTypeCode.IsNumeric() && outputTypeCode.IsNumeric())
			{
				return GetNumericOrEnumConversion(inputType, inputTypeCode, outputType, outputTypeCode);
			}
			// 可空类型转换。
			Type? inputUnderlyingType = Nullable.GetUnderlyingType(inputType);
			Type? outputUnderlyingType = Nullable.GetUnderlyingType(outputType);
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
					if (inputUnderlyingType == outputType || (inputTypeCode.IsNumeric() && outputTypeCode.IsNumeric()))
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
		private static Conversion GetNumericOrEnumConversion(Type inputType, TypeCode inputTypeCode,
			Type outputType, TypeCode outputTypeCode)
		{
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
			Conversion conversion = NumericConversion.GetConversion(inputTypeCode, outputTypeCode);
			if (inputType.IsEnum || outputType.IsEnum)
			{
				// 将类型转换的类型修正为 Enum。
				if (conversion is NumericConversion numericConv)
				{
					return new NumericConversion(ConversionType.Enum, numericConv);
				}
				else
				{
					return IdentityConversion.ExplicitEnum;
				}
			}
			return conversion;
		}

		#region 显式引用类型转换或拆箱转换

		/// <summary>
		/// 测试是否是显式引用类型转换或拆箱转换。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>
		/// <returns>如果是显式引用类型转换或拆箱转换，则为相应的 IL 生成器；否则为 <c>null</c>。</returns>
		private static Conversion? GetExplicitRefConversion(Type inputType, Type outputType)
		{
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
					CanReferenceConvert(inputType.GetGenericArguments()[0], outputType.GetElementType()!))
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
					CanReferenceConvert(inputType.GetElementType()!, outputType.GetGenericArguments()[0]))
				{
					return CastClassConversion.Default;
				}
				return null;
			}
			if (inputType.IsArray)
			{
				// 元素类型可以显式引用转换的等秩数组间的转换。
				if (outputType.IsArray && inputType.GetArrayRank() == outputType.GetArrayRank() &&
					CanReferenceConvert(inputType.GetElementType()!, outputType.GetElementType()!))
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
			if (inputType.IsValueType || outputType.IsValueType)
			{
				return false;
			}
			Conversion? conversion = GetPreDefinedConversion(inputType, outputType);
			return conversion != null && conversion.ConversionType.IsReference();
		}

		/// <summary>
		/// 返回泛型委托间的协变和逆变类型转换。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>
		/// <returns>如果是泛型委托间的协变和逆变类型转换，则为相应的 IL 生成器；否则为 <c>null</c>。</returns>>
		private static Conversion? GetDelegateConversion(Type inputType, Type outputType)
		{
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
			Conversion? conversion = GetConversion(inputArg, outputArg);
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
		private static Conversion? GetUserDefinedConversion(Type inputType, Type outputType)
		{
			Tuple<Type, Type> key = new(inputType, outputType);
			if (userConversions.TryGetValue(key, out Conversion? conversion))
			{
				return conversion;
			}
			// 判断可空类型。
			Type inputUnderlyingType = inputType.GetNonNullableType();
			Type outputUnderlyingType = outputType.GetNonNullableType();
			UserConversion? userConversion = UserConversionFinder.GetConversion(inputUnderlyingType, outputUnderlyingType);
			if (userConversion == null)
			{
				return null;
			}
			// 存入缓存。
			conversion = userConversion;
			MethodInfo method = userConversion.Method;
			Type methodInputType = method.GetParametersNoCopy()[0].ParameterType;
			if (inputType != methodInputType || outputType != method.ReturnType)
			{
				conversion = userConversions.GetOrAdd(new Tuple<Type, Type>(methodInputType, method.ReturnType), conversion);
			}
			if (inputUnderlyingType != inputType || outputUnderlyingType != outputType)
			{
				userConversions.TryAdd(new Tuple<Type, Type>(inputUnderlyingType, outputUnderlyingType), conversion);
			}
			if (inputUnderlyingType == inputType || !methodInputType.IsValueType || methodInputType.IsNullable())
			{
				return userConversions.GetOrAdd(key, conversion);
			}
			// 需要将输入的 Nullable<T> 解包。
			if (outputUnderlyingType != outputType || !outputType.IsValueType)
			{
				// outputType 可以为 null（引用类型或 Nullable<T>）。
				Conversion nullableConv = userConversion.ConversionType == ConversionType.ImplicitUserDefined ?
					BetweenNullableConversion.ImplicitUserDefined : BetweenNullableConversion.ExplicitUserDefined;
				return userConversions.GetOrAdd(key, nullableConv);
			}
			else
			{
				return userConversions.GetOrAdd(key, FromNullableConversion.UserDefined);
			}
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
			Conversion? conversion = GetPreDefinedConversion(inputType, outputType);
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
				case ConversionType.Enum:
					// 不包含显式枚举转换。
					return ConversionType.None;
				case ConversionType.Unbox:
					// 完整包含拆箱转换。
					return ConversionType.Unbox;
				case ConversionType.ExplicitNumeric:
				case ConversionType.ExplicitNullable:
					// 包含部分转换，需要判断是否存在反向隐式转换。
					// 此时 inputType 和 outputType 一定都是值类型。
					Conversion reversedConversion = GetBetweenValueTypeConversion(outputType, inputType)!;
					return reversedConversion.ConversionType.IsImplicit() ?
						conversion.ConversionType : ConversionType.None;
				case ConversionType.ExplicitReference:
					// 包含部分转换，需要判断是否存在反向转换。
					if (inputType.IsAssignableFrom(outputType))
					{
						return ConversionType.ExplicitReference;
					}
					return ConversionType.None;
			}
			return ConversionType.None;
		}
	}
}
