using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="MethodBase"/> 及其子类的扩展方法。
	/// </summary>
	public static class MethodExt
	{
		/// <summary>
		/// 隐式类型转换方法的名称。
		/// </summary>
		internal const string ImplicitMethodName = "op_Implicit";
		/// <summary>
		/// 显式类型转换方法的名称。
		/// </summary>
		internal const string ExplicitMethodName = "op_Explicit";

		#region 方法参数

		/// <summary>
		/// 获取参数列表而不复制的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Func<MethodBase, ParameterInfo[]> getParametersNoCopy = BuildGetParametersNoCopy();
		/// <summary>
		/// 构造获取参数列表而不复制的委托，兼容 Mono 运行时。
		/// </summary>
		/// <returns>获取参数列表而不复制的委托。</returns>
		private static Func<MethodBase, ParameterInfo[]> BuildGetParametersNoCopy()
		{
			if (!TypeExt.IsMonoRuntime)
			{
				// 为了防止 DelegateBuilder 里调用 GetParametersNoCopy 而导致死循环，这里必须使用 Delegate.CreateDelegate 方法。
				MethodInfo methodGetParametersNoCopy = typeof(MethodBase).GetMethod("GetParametersNoCopy", TypeExt.InstanceFlag);
				return (Func<MethodBase, ParameterInfo[]>)Delegate.CreateDelegate(typeof(Func<MethodBase, ParameterInfo[]>),
					methodGetParametersNoCopy);
			}
			Type monoMethodInfo = Type.GetType("System.Reflection.MonoMethodInfo");
			Contract.Assume(monoMethodInfo != null);
			MethodInfo getParamsInfoMethod = monoMethodInfo.GetMethod("GetParametersInfo", TypeExt.StaticFlag);
			Type monoMethod = Type.GetType("System.Reflection.MonoMethod");
			Contract.Assume(monoMethod != null);
			FieldInfo mhandleField = monoMethod.GetField("mhandle", TypeExt.InstanceFlag);
			Contract.Assume(mhandleField != null);
			DynamicMethod method = new DynamicMethod("GetParametersNoCopy", typeof(ParameterInfo[]),
				new[] { typeof(MethodBase) }, true);
			ILGenerator il = method.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, mhandleField);
			il.Emit(OpCodes.Ldarg_0);
			il.EmitCall(getParamsInfoMethod, true);
			il.Emit(OpCodes.Ret);
			return (Func<MethodBase, ParameterInfo[]>)method.CreateDelegate(typeof(Func<MethodBase, ParameterInfo[]>));
		}
		/// <summary>
		/// 返回当前方法的参数列表，而不会复制参数列表。
		/// </summary>
		/// <param name="method">要获取参数列表的方法。</param>
		/// <returns>方法的参数列表。</returns>
		[Pure]
		public static ParameterInfo[] GetParametersNoCopy(this MethodBase method)
		{
			return getParametersNoCopy(method);
		}
		/// <summary>
		/// 返回当前方法的参数类型列表。
		/// </summary>
		/// <param name="method">要获取参数类型列表的方法。</param>
		/// <returns>方法的参数类型列表。</returns>
		public static Type[] GetParameterTypes(this MethodBase method)
		{
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			if (parameters.Length == 0)
			{
				return Type.EmptyTypes;
			}
			Type[] types = new Type[parameters.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}
		/// <summary>
		/// 返回当前方法的参数类型列表，那么最后一个类型表示返回值类型。
		/// </summary>
		/// <param name="method">要获取参数类型列表的方法。</param>
		/// <returns>方法的参数类型列表。</returns>
		public static Type[] GetParameterTypesWithReturn(this MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			if (parameters.Length == 0)
			{
				return new[] { method.ReturnType };
			}
			Type[] types = new Type[parameters.Length + 1];
			int i = 0;
			for (; i < parameters.Length; i++)
			{
				types[i] = parameters[i].ParameterType;
			}
			types[i] = method.ReturnType;
			return types;
		}

		#endregion // 方法参数

		#region 参数顺序

		/// <summary>
		/// 默认的参数顺序数组。
		/// </summary>
		private static int[] defaultParamOrders = { 0 };
		/// <summary>
		/// 返回指定长度的参数顺序数组，其中的参数是按顺序排列的。
		/// </summary>
		/// <param name="len">要获取的参数顺序数组的长度。</param>
		/// <returns>获取的参数顺序数组。</returns>
		internal static int[] GetParamOrder(int len)
		{
			if (defaultParamOrders.Length < len)
			{
				int[] newOrder = new int[len];
				for (int i = 0; i < len; i++) { newOrder[i] = i; }
				int[] oldOrder = defaultParamOrders;
				while (oldOrder.Length < len)
				{
					oldOrder = Interlocked.CompareExchange(ref defaultParamOrders, newOrder, oldOrder);
				}
			}
			return defaultParamOrders;
		}

		#endregion // 参数顺序

		#region 泛型参数推断

		/// <summary>
		/// 根据实参参数类型推断当前泛型方法定义的类型参数，
		/// 并返回表示结果封闭构造方法的 <see cref="MethodInfo"/> 对象。
		/// </summary>
		/// <param name="method">要进行类型参数推断的泛型方法定义。</param>
		/// <param name="types">泛型方法的实参参数数组。</param>
		/// <returns>一个 <see cref="MethodInfo"/> 对象，表示通过将当前泛型方法定义的类型参数替换为根据 
		/// <paramref name="types"/> 推断得到的元素生成的封闭构造方法。</returns>
		/// <exception cref="InvalidOperationException">当前 <see cref="MethodInfo"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="MethodInfo.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException">不能从 <paramref name="types"/> 推断得到类型参数。</exception>
		/// <exception cref="ArgumentException">根据 <paramref name="types"/>
		/// 推断出来的类型参数中的某个元素不满足为当前泛型方法定义的相应类型参数指定的约束。</exception>
		public static MethodInfo MakeGenericMethodFromParamTypes(this MethodInfo method, params Type[] types)
		{
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			if (types == null)
			{
				throw CommonExceptions.ArgumentNull("types");
			}
			Contract.EndContractBlock();
			if (!method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.NeedGenericMethodDefinition("method");
			}
			var result = GenericArgumentsInferences(method, null, types, null,
				GenericArgInferenceOptions.OptionalParamBinding);
			if (result == null)
			{
				throw CommonExceptions.CannotInferenceGenericArguments("method");
			}
			return method.MakeGenericMethod(result.GenericArguments);
		}
		/// <summary>
		/// 根据实参参数类型推断当前泛型方法定义的类型参数，
		/// 并返回表示结果封闭构造方法的 <see cref="MethodInfo"/> 对象。
		/// </summary>
		/// <param name="method">要进行类型参数推断的泛型方法定义。</param>
		/// <param name="types">泛型方法的实参参数数组。</param>
		/// <param name="options">泛型类型推断的选项。</param>
		/// <returns>一个 <see cref="MethodInfo"/> 对象，表示通过将当前泛型方法定义的类型参数替换为根据 
		/// <paramref name="types"/> 推断得到的元素生成的封闭构造方法。</returns>
		/// <exception cref="InvalidOperationException">当前 <see cref="MethodInfo"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="MethodInfo.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException">不能从 <paramref name="types"/> 推断得到类型参数。</exception>
		/// <exception cref="ArgumentException">根据 <paramref name="types"/>
		/// 推断出来的类型参数中的某个元素不满足为当前泛型方法定义的相应类型参数指定的约束。</exception>
		public static MethodInfo MakeGenericMethodFromParamTypes(this MethodInfo method, Type[] types,
			GenericArgInferenceOptions options)
		{
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			if (types == null)
			{
				throw CommonExceptions.ArgumentNull("types");
			}
			Contract.EndContractBlock();
			if (!method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.NeedGenericMethodDefinition("method");
			}
			var result = GenericArgumentsInferences(method, null, types, null, options);
			if (result == null)
			{
				throw CommonExceptions.CannotInferenceGenericArguments("method");
			}
			return method.MakeGenericMethod(result.GenericArguments);
		}
		/// <summary>
		/// 根据给定的方法实参类型数组推断泛型方法的泛型类型。
		/// </summary>
		/// <param name="method">要推断泛型类型的泛型方法定义。</param>
		/// <param name="types">方法实参类型数组。</param>
		/// <returns>如果成功推断泛型方法的类型参数，则为推断结果；否则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidOperationException">当前 <see cref="MethodInfo"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="MethodInfo.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		public static Type[] GenericArgumentsInferences(this MethodBase method, params Type[] types)
		{
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			if (types == null)
			{
				throw CommonExceptions.ArgumentNull("types");
			}
			Contract.EndContractBlock();
			if (!method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.NeedGenericMethodDefinition("method");
			}
			var result = GenericArgumentsInferences(method, null, types, null,
				GenericArgInferenceOptions.OptionalParamBinding);
			return result == null ? null : result.GenericArguments;
		}
		/// <summary>
		/// 根据给定的方法实参类型数组推断泛型方法的泛型类型。
		/// </summary>
		/// <param name="method">要推断泛型类型的泛型方法定义。</param>
		/// <param name="types">方法实参类型数组。</param>
		/// <param name="options">泛型类型推断的选项。</param>
		/// <returns>如果成功推断泛型方法的类型参数，则为推断结果；否则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidOperationException">当前 <see cref="MethodInfo"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="MethodInfo.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		public static Type[] GenericArgumentsInferences(this MethodBase method, Type[] types,
			GenericArgInferenceOptions options)
		{
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			if (types == null)
			{
				throw CommonExceptions.ArgumentNull("types");
			}
			Contract.EndContractBlock();
			if (!method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.NeedGenericMethodDefinition("method");
			}
			var result = GenericArgumentsInferences(method, null, types, null, options);
			return result == null ? null : result.GenericArguments;
		}
		/// <summary>
		/// 根据给定的方法实参类型数组推断泛型方法的泛型类型。
		/// </summary>
		/// <param name="method">要推断泛型类型的泛型方法定义。</param>
		/// <param name="returnType">方法的实际返回类型，如果不存在则传入 <c>null</c>。</param>
		/// <param name="types">方法实参类型数组。</param>
		/// <param name="paramOrder">方法实参的顺序，它的长度必须大于等于形参个数和实参个数。
		/// 传入 <c>null</c> 表示使用默认顺序。</param>
		/// <param name="options">泛型类型推断的选项。</param>
		/// <returns>如果成功推断泛型方法的类型参数，则为推断结果；否则为 <c>null</c>。</returns>
		/// <remarks><c>paramOrder = { 1, 2, 0}</c> 表示，实参的顺序为 <c>types[1], types[2], types[0]</c>。</remarks>
		internal static GenericArgInferenceResult GenericArgumentsInferences(this MethodBase method,
			Type returnType, Type[] types, int[] paramOrder, GenericArgInferenceOptions options)
		{
			Contract.Requires(method != null && types != null);
			bool isExplicit = options.HasFlag(GenericArgInferenceOptions.Explicit);
			TypeBounds bounds = new TypeBounds(method.GetGenericArguments());
			if (!CheckReturnType(method, returnType, bounds, isExplicit))
			{
				return null;
			}
			// 对方法参数进行类型推断。
			GenericArgInferenceResult result = new GenericArgInferenceResult();
			int typeLen = types.Length;
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			int paramLen = parameters.Length;
			if (paramOrder == null)
			{
				paramOrder = GetParamOrder(Math.Max(paramLen, typeLen));
			}
			// 仅当 params 参数类型包含泛型参数时，才不为 null。
			Type paramArrayType = null;
			if (method.CallingConvention.HasFlag(CallingConventions.VarArgs))
			{
				// 方法具有可变参数。
				result.OptionalParameterTypes = GetExternalParamTypes(types, paramLen, paramOrder);
			}
			else if (paramLen > 0)
			{
				ParameterInfo lastParam = parameters[paramLen - 1];
				if (lastParam.IsParamArray())
				{
					// 方法具有 params 参数。
					paramLen--;
					result.ParamArrayTypes = GetExternalParamTypes(types, paramLen, paramOrder);
					if (lastParam.ParameterType.ContainsGenericParameters ||
						CheckParamArrayType(lastParam.ParameterType, result.ParamArrayTypes, isExplicit))
					{
						paramArrayType = lastParam.ParameterType;
					}
					else
					{
						return null;
					}
				}
				else if (typeLen > paramLen)
				{
					// 方法实参过多。
					return null;
				}
			}
			// 检查实参是否与形参对应，未对应的参数是否包含默认值。
			for (int i = 0; i < paramLen; i++)
			{
				ParameterInfo parameter = parameters[i];
				int idx = paramOrder[i];
				if (idx < typeLen)
				{
					// 对参数进行类型推断。
					if (!CheckParameter(parameter.ParameterType, types[idx], bounds, isExplicit))
					{
						return null;
					}
				}
				else if (!options.HasFlag(GenericArgInferenceOptions.OptionalParamBinding) ||
					parameter.DefaultValue == DBNull.Value)
				{
					// 参数不包含默认值。
					return null;
				}
			}
			Type[] paramTypes = result.ParamArrayTypes;
			if (paramArrayType == null || paramTypes.Length == 0)
			{
				Type[] args = bounds.FixTypeArguments();
				if (args == null)
				{
					return null;
				}
				result.GenericArguments = args;
				return result;
			}
			// 对 params 参数进行推断。
			Type paramElementType = paramArrayType.GetElementType();
			if (paramTypes.Length > 1)
			{
				// 多个实参对应一个形参，做多次类型推断。
				for (int i = 0; i < paramTypes.Length; i++)
				{
					if (!bounds.TypeInferences(paramElementType, paramTypes[i]))
					{
						return null;
					}
				}
				Type[] args = bounds.FixTypeArguments();
				if (args == null)
				{
					return null;
				}
				result.GenericArguments = args;
				return result;
			}
			// 一个实参对应一个形参，需要判断是否需要展开 params 参数。
			TypeBounds newBounds = new TypeBounds(bounds);
			Type type = paramTypes[0];
			// 首先尝试对 paramArrayType 进行推断。
			if (bounds.TypeInferences(paramArrayType, type))
			{
				Type[] args = bounds.FixTypeArguments();
				if (args != null)
				{
					// 推断成功的话，则无需展开 params 参数。
					result.ParamArrayTypes = null;
					result.GenericArguments = args;
					return result;
				}
			}
			// 然后尝试对 paramElementType 进行推断。
			if (newBounds.TypeInferences(paramElementType, type))
			{
				Type[] args = newBounds.FixTypeArguments();
				if (args == null)
				{
					return null;
				}
				result.GenericArguments = args;
				return result;
			}
			return null;
		}
		/// <summary>
		/// 检查方法的返回类型。
		/// </summary>
		/// <param name="method">要推断泛型类型的泛型方法定义。</param>
		/// <param name="returnType">方法的实际返回类型，如果不存在则传入 <c>null</c>。</param>
		/// <param name="bounds">界限集集合。</param>
		/// <param name="isExplicit">类型检查时，如果考虑显式类型转换，则为 <c>true</c>；
		/// 否则只考虑隐式类型转换。</param>
		/// <returns>如果成功检查返回类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool CheckReturnType(MethodBase method, Type returnType, TypeBounds bounds, bool isExplicit)
		{
			Contract.Requires(method != null && bounds != null);
			if (returnType == null)
			{
				return true;
			}
			MethodInfo methodInfo = method as MethodInfo;
			if (methodInfo == null)
			{
				return true;
			}
			Type type = methodInfo.ReturnType;
			if (type.ContainsGenericParameters)
			{
				// 对方法返回类型进行上限推断。
				return bounds.TypeInferences(type, returnType, true);
			}
			return returnType.IsConvertFrom(type, isExplicit);
		}
		/// <summary>
		/// 返回非固定参数的类型。
		/// </summary>
		/// <param name="types">所有参数类型。</param>
		/// <param name="index">非固定参数的起始索引。</param>
		/// <param name="paramOrder">方法实参的顺序。</param>
		/// <returns>非固定参数的类型。</returns>
		private static Type[] GetExternalParamTypes(Type[] types, int index, int[] paramOrder)
		{
			Contract.Requires(types != null && paramOrder != null);
			int len = types.Length - index;
			if (len <= 0)
			{
				return Type.EmptyTypes;
			}
			Type[] paramTypes = new Type[len];
			for (int i = 0, j = index; i < len; i++)
			{
				paramTypes[i] = types[paramOrder[j]];
			}
			return paramTypes;
		}
		/// <summary>
		/// 检查方法的参数类型。
		/// </summary>
		/// <param name="paramType">方法形参的类型。</param>
		/// <param name="type">相应实参的类型，允许使用 <c>null</c> 表示引用类型约束。</param>
		/// <param name="bounds">界限集集合。</param>
		/// <param name="isExplicit">类型检查时，如果考虑显式类型转换，则为 <c>true</c>；
		/// 否则只考虑隐式类型转换。</param>
		/// <returns>如果成功检查返回类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool CheckParameter(Type paramType, Type type, TypeBounds bounds, bool isExplicit)
		{
			Contract.Requires(paramType != null && bounds != null);
			if (paramType.ContainsGenericParameters)
			{
				return bounds.TypeInferences(paramType, type);
			}
			if (type == null)
			{
				return !paramType.IsValueType;
			}
			return paramType.IsConvertFrom(type, isExplicit);
		}
		/// <summary>
		/// 检查方法的 params 参数类型。
		/// </summary>
		/// <param name="paramArrayType">params 形参的类型。</param>
		/// <param name="types">params 实参的类型。</param>
		/// <param name="isExplicit">类型检查时，如果考虑显式类型转换，则为 <c>true</c>；
		/// 否则只考虑隐式类型转换。</param>
		/// <returns>如果成功检查 params 参数类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool CheckParamArrayType(Type paramArrayType, Type[] types, bool isExplicit)
		{
			Contract.Requires(paramArrayType != null && types != null);
			if (types.Length == 0)
			{
				return true;
			}
			Type paramElementType = paramArrayType.GetElementType();
			if (types.Length == 1)
			{
				// 只有一个实参，可能是数组或数组元素。
				Type type = types[0];
				return paramArrayType.IsConvertFrom(type, isExplicit) ||
					paramElementType.IsConvertFrom(type, isExplicit);
			}
			// 有多个实参，必须是数组元素。
			for (int i = 0; i < types.Length; i++)
			{
				if (!paramElementType.IsConvertFrom(types[i], isExplicit))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 获取当前参数是否是 params 参数。
		/// </summary>
		/// <param name="parameter">要判断的参数。</param>
		/// <returns>如果是 params 参数，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public static bool IsParamArray(this ParameterInfo parameter)
		{
			return parameter != null && parameter.ParameterType.IsArray &&
				parameter.IsDefined(typeof(ParamArrayAttribute), true);
		}

		#endregion // 泛型参数推断

	}

	/// <summary>
	/// 泛型参数推断的选项。
	/// </summary>
	[Flags]
	public enum GenericArgInferenceOptions
	{
		/// <summary>
		/// 无任何选项。
		/// </summary>
		None,
		/// <summary>
		/// 对可选参数进行绑定。
		/// </summary>
		OptionalParamBinding,
		/// <summary>
		/// 类型检查时，使用显式类型转换，而不是默认的隐式类型转换。
		/// </summary>
		Explicit,
		/// <summary>
		/// 设置全部选项。
		/// </summary>
		All,
	}
}
