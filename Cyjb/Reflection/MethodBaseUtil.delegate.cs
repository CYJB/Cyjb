using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Conversions;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="MethodBase"/> 创建委托的扩展方法。
	/// </summary>
	public static partial class MethodBaseUtil
	{
		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例方法。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例方法的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static Delegate? PowerDelegate(this MethodBase method, Type delegateType)
		{
			ArgumentNullException.ThrowIfNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ReflectionExceptions.CheckUnboundGenParam(method);
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(method));
			}
			return method.PowerDelegate(delegateType, false, null);
		}

		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodInfo"/>。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例方法。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例方法的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static TDelegate? PowerDelegate<TDelegate>(this MethodBase method)
			where TDelegate : Delegate
		{
			ReflectionExceptions.CheckUnboundGenParam(method);
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(method));
			}
			Type delegateType = typeof(TDelegate);
			return method.PowerDelegate(delegateType, false, null) as TDelegate;
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例方法。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static Delegate? PowerDelegate(this MethodBase method, Type delegateType, object? firstArgument)
		{
			ArgumentNullException.ThrowIfNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ReflectionExceptions.CheckUnboundGenParam(method);
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(method));
			}
			return method.PowerDelegate(delegateType, true, firstArgument);
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodInfo"/>。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例方法。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例方法的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static TDelegate? PowerDelegate<TDelegate>(this MethodBase method, object? firstArgument)
			where TDelegate : Delegate
		{
			ReflectionExceptions.CheckUnboundGenParam(method);
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(method));
			}
			Type delegateType = typeof(TDelegate);
			return method.PowerDelegate(delegateType, true, firstArgument) as TDelegate;
		}

		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="hasFirstArg">是否指定了首个参数的类型。</param>
		/// <param name="firstArg">首个参数的值。</param>
		/// <returns>指定类型的动态方法，表示访问指定的静态或实例方法。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法，需要将实例对象作为委托的第一个参数。支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		internal static Delegate? PowerDelegate(this MethodBase method, Type delegateType, bool hasFirstArg, object? firstArg)
		{
			// 判断是否需要作为实例的形参。
			int index = 0;
			if (!method.IsStatic && method is not ConstructorInfo)
			{
				index++;
			}
			MethodInfo invoke = delegateType.GetDelegateInvoke();
			Type[] types = invoke.GetParameterTypes();
			bool needReplaceFirstArgument = false;
			if (hasFirstArg)
			{
				Type? firstParamType = GetFirstParameterType(method, firstArg);
				if (firstParamType == null)
				{
					return null;
				}
				// 如果首个参数是值类型，由于 CreateDelegate 只能传递 object，需要提前做类型转换，确保与 firstParamType 匹配。
				if (firstArg != null && firstArg.GetType().IsValueType)
				{
					firstArg = GenericConvert.ChangeType(firstArg, firstParamType);
					// 后面泛型类型推断时需要使用正确的类型，推断结束后再替换为 object 类型。
					needReplaceFirstArgument = true;
				}
				types = types.Insert(0, firstParamType);
			}
			Type returnType = invoke.ReturnType;
			// 提取方法参数信息。
			MethodArgumentsInfo? argInfo = GetArgumentsInfo(ref method, types, returnType);
			if (argInfo == null)
			{
				return null;
			}
			if (needReplaceFirstArgument)
			{
				types[0] = typeof(object);
			}
			// 构造动态委托。
			DynamicMethod dlgMethod;
			if (method.DeclaringType == null)
			{
				dlgMethod = new("MethodDelegate", returnType, types, method.Module, true);
			}
			else
			{
				dlgMethod = new("MethodDelegate", returnType, types, method.DeclaringType, true);
			}
			ILGenerator il = dlgMethod.GetILGenerator();
			// 实例方法的第一个参数用作传递实例对象。
			if (argInfo.InstanceType != null)
			{
				il.EmitLoadInstance(method, argInfo.InstanceType, true);
			}
			// 加载方法参数。
			bool optimizeTailcall = il.EmitLoadParameters(method, argInfo, index);
			// 调用方法。
			Type[]? optionalArgumentTypes = null;
			if (argInfo.OptionalArgumentTypes != null)
			{
				optionalArgumentTypes = argInfo.OptionalArgumentTypes.ToArray()!;
			}
			if (!il.EmitInvokeMethod(method, optionalArgumentTypes, returnType, optimizeTailcall))
			{
				return null;
			}
			if (hasFirstArg)
			{
				return dlgMethod.CreateDelegate(delegateType, firstArg);
			}
			else
			{
				return dlgMethod.CreateDelegate(delegateType);
			}
		}

		/// <summary>
		/// 获取首个参数对应的形参类型。
		/// </summary>
		/// <param name="method">方法信息。</param>
		/// <param name="firstArgument">首个参数的值。</param>
		/// <returns>首个参数对应的形参类型。</returns>
		private static Type? GetFirstParameterType(MethodBase method, object? firstArgument)
		{
			Type? type = null;
			if (method.IsStatic || method is ConstructorInfo)
			{
				ParameterInfo[] parameters = method.GetParametersNoCopy();
				if (parameters.Length > 0)
				{
					type = parameters[0].ParameterType;
				}
				else if (!method.CallingConvention.HasFlag(CallingConventions.VarArgs))
				{
					// 无入参又没有可变参数，无法满足封闭委托。
					return null;
				}
			}
			else
			{
				type = method.DeclaringType;
			}
			// 未找到入参或者包含泛型参数，只能使用首个参数的类型。
			if (type == null || type.ContainsGenericParameters)
			{
				return firstArgument == null ? typeof(object) : firstArgument.GetType();
			}
			return type;
		}

		/// <summary>
		/// 获取指定方法的参数信息。
		/// </summary>
		/// <param name="method">要获取参数信息的方法。</param>
		/// <param name="types">方法的实参类型列表。</param>
		/// <param name="returnType">方法的返回值类型。</param>
		/// <returns>方法的参数信息。</returns>
		private static MethodArgumentsInfo? GetArgumentsInfo(ref MethodBase method, Type[] types, Type returnType)
		{
			MethodArgumentsOption options = MethodArgumentsOption.OptionalAndExplicit | MethodArgumentsOption.ConvertRefType;
			if (!method.IsStatic && method is not ConstructorInfo)
			{
				options |= MethodArgumentsOption.ContainsInstance;
			}
			MethodArgumentsInfo? argInfo = MethodArgumentsInfo.GetInfo(method, types, options);
			// 对泛型方法定义进行类型推断。
			if (argInfo != null && method.IsGenericMethodDefinition)
			{
				Type[]? genericArguments = argInfo.GetGenericArguments(returnType, options);
				if (genericArguments == null)
				{
					return null;
				}
				method = ((MethodInfo)method).MakeGenericMethod(genericArguments);
			}
			return argInfo;
		}

		/// <summary>
		/// 加载方法参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要加载参数的方法。</param>
		/// <param name="argumentsInfo">方法实参信息。</param>
		/// <param name="index">实参的起始索引。</param>
		/// <returns>能否对方法进行 tailcall 优化，如果可以则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private static bool EmitLoadParameters(this ILGenerator il, MethodBase method, MethodArgumentsInfo argumentsInfo,
			int index)
		{
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			IList<Type?> fixedArgs = argumentsInfo.FixedArguments;
			bool optimizeTailcall = true;
			for (int i = 0; i < parameters.Length; i++, index++)
			{
				ParameterInfo paramInfo = parameters[i];
				if (i < fixedArgs.Count)
				{
					if (!il.EmitLoadParameter(paramInfo.ParameterType, index, fixedArgs[i]!))
					{
						optimizeTailcall = false;
					}
				}
				else if (!paramInfo.IsParamArray())
				{
					// MethodArgumentsInfo 保证包含默认值。
					il.EmitConstant(paramInfo.DefaultValue);
				}
				else if (argumentsInfo.ParamArrayType == null)
				{
					// params 参数。
					il.EmitEmptyArray(paramInfo.ParameterType.GetElementType()!);
				}
				else
				{
					break;
				}
			}
			IList<Type?>? paramArgs = argumentsInfo.ParamArgumentTypes;
			if (paramArgs != null)
			{
				// 加载 params 参数。
				int argCnt = paramArgs.Count;
				Type elementType = argumentsInfo.ParamArrayType!.GetElementType()!;
				il.EmitConstant(argCnt);
				il.Emit(OpCodes.Newarr, elementType);
				for (int i = 0; i < argCnt; i++, index++)
				{
					// 直接将栈顶元素（params 数组）复制用于设置数组元素。
					il.Emit(OpCodes.Dup);
					il.EmitConstant(i);
					// 前面保证了不存在 null 类型。
					il.EmitLoadParameter(elementType, index, paramArgs[i]!);
					il.EmitStoreElement(elementType);
				}
				optimizeTailcall = false;
			}
			IList<Type?>? optionalArgs = argumentsInfo.OptionalArgumentTypes;
			if (optionalArgs != null)
			{
				// 加载可变参数。
				for (int i = 0; i < optionalArgs.Count; i++, index++)
				{
					// 前面保证了不存在 null 类型。
					il.EmitLoadParameter(optionalArgs[i]!, index, optionalArgs[i]!);
				}
				// 带有可变参数时，不优化 tailCall，避免参数信息丢失。
				optimizeTailcall = false;
			}
			return optimizeTailcall;
		}

		/// <summary>
		/// 写入调用方法的指令。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="optionalArgumentTypes">可变参数的类型。</param>
		/// <param name="returnType">方法调用的返回值类型。</param>
		/// <param name="optimizeTailcall">是否对方法调用进行 tailcall 优化。</param>
		/// <returns>如果成功写入调用方法的指令，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool EmitInvokeMethod(this ILGenerator il, MethodBase method, Type[]? optionalArgumentTypes,
			Type returnType, bool optimizeTailcall)
		{
			Type declaringType = method.DeclaringType!;
			if (method is not MethodInfo methodInfo)
			{
				// 调用构造函数。
				Conversion? conversion = ConversionFactory.GetConversion(declaringType, returnType);
				if (conversion == null)
				{
					return false;
				}
				il.Emit(OpCodes.Newobj, method);
				conversion.Emit(il, declaringType, returnType, true);
			}
			else if (returnType == typeof(void))
			{
				if (methodInfo.ReturnType == typeof(void))
				{
					// 无返回值。
					il.EmitCall(declaringType, methodInfo, optimizeTailcall, optionalArgumentTypes);
				}
				else
				{
					// 忽略返回值。
					il.EmitCall(declaringType, methodInfo, optionalArgumentTypes);
					il.Emit(OpCodes.Pop);
				}
			}
			else if (methodInfo.ReturnType == typeof(void))
			{
				// 返回默认值。
				il.EmitCall(declaringType, methodInfo, optionalArgumentTypes);
				il.EmitDefault(returnType);
			}
			else
			{
				// 对返回值进行类型转换。
				Type methodReturnType = methodInfo.ReturnType;
				Conversion? conversion = ConversionFactory.GetConversion(methodReturnType, returnType);
				if (conversion == null)
				{
					return false;
				}
				il.EmitCall(declaringType, methodInfo, optimizeTailcall && !conversion.NeedEmit, optionalArgumentTypes);
				if (conversion.PassByAddr)
				{
					il.EmitGetAddress(methodReturnType);
				}
				conversion.Emit(il, methodReturnType, returnType, true);
			}
			il.Emit(OpCodes.Ret);
			return true;
		}
	}
}
