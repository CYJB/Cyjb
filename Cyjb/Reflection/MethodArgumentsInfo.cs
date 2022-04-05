using System.Diagnostics;
using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 方法的实参列表信息。
	/// </summary>
	internal class MethodArgumentsInfo
	{
		/// <summary>
		/// 返回方法的参数信息。
		/// </summary>
		/// <param name="method">要获取参数信息的方法。</param>
		/// <param name="arguments">方法实参类型数组，其长度必须大于等于方法的必选参数个数。
		/// 使用 <c>null</c> 表示无需对指定位置进行类型检查。</param>
		/// <param name="options">方法参数信息的选项。</param>
		/// <returns>方法的参数信息。</returns>
		public static MethodArgumentsInfo? GetInfo(MethodBase method, Type?[] arguments, MethodArgumentsOption options)
		{
			MethodArgumentsInfo result = new(method, arguments);
			bool isExplicit = options.HasFlag(MethodArgumentsOption.Explicit);
			bool convertRefType = options.HasFlag(MethodArgumentsOption.ConvertRefType);
			if (options.HasFlag(MethodArgumentsOption.ContainsInstance))
			{
				if (!result.DetectInstanceType(isExplicit, convertRefType))
				{
					return null;
				}
			}
			// 填充 params 参数和可变参数。
			if (!result.DetectParamArray(isExplicit))
			{
				return null;
			}
			// 检查固定实参类型。
			bool optionalParamBinding = options.HasFlag(MethodArgumentsOption.OptionalParamBinding);
			if (!result.DetectFixedArguments(optionalParamBinding, isExplicit, convertRefType))
			{
				return null;
			}
			return result;
		}

		/// <summary>
		/// 方法信息。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly MethodBase method;
		/// <summary>
		/// 方法实参类型列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type?[] arguments;
		/// <summary>
		/// 方法实例实参类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type? instanceType;
		/// <summary>
		/// 方法的固定实参列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ArraySegment<Type?> fixedArguments = new();
		/// <summary>
		/// params 形参的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type? paramArrayType;
		/// <summary>
		/// params 实参的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ArraySegment<Type?>? paramArgumentTypes;
		/// <summary>
		/// 可变参数的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ArraySegment<Type?>? optionalArgumentTypes;

		/// <summary>
		/// 使用指定的方法信息和实参类型列表初始化 <see cref="MethodArgumentsInfo"/> 类的新实例。
		/// </summary>
		/// <param name="method">方法信息。</param>
		/// <param name="arguments">方法实参类型列表。</param>
		private MethodArgumentsInfo(MethodBase method, Type?[] arguments)
		{
			this.method = method;
			this.arguments = arguments;
		}

		/// <summary>
		/// 获取方法实例实参类型。
		/// </summary>
		/// <value>方法实例实参类型。<c>null</c> 表示不是实例方法。</value>
		public Type? InstanceType => instanceType;
		/// <summary>
		/// 获取方法的固定实参列表。
		/// </summary>
		/// <value>方法的固定实参列表。如果 <see cref="ParamArrayType"/> 不为 <c>null</c>，
		/// 则不包含最后的 params 参数。</value>
		public IList<Type?> FixedArguments => fixedArguments;
		/// <summary>
		/// 获取 params 形参的类型。
		/// </summary>
		/// <value>params 形参的类型，如果为 <c>null</c> 表示无需特殊处理 params 参数。</value>
		public Type? ParamArrayType => paramArrayType;
		/// <summary>
		/// 获取 params 实参的类型列表。
		/// </summary>
		/// <value>params 实参的类型列表，如果为 <c>null</c> 表示无需特殊处理 params 参数。</value>
		public IList<Type?>? ParamArgumentTypes => paramArgumentTypes;
		/// <summary>
		/// 获取可变参数的类型。
		/// </summary>
		/// <value>可变参数的类型，如果为 <c>null</c> 表示没有可变参数。</value>
		public IList<Type?>? OptionalArgumentTypes => optionalArgumentTypes;

		#region 获取方法参数类型

		/// <summary>
		/// 将实参中的第一个参数作为方法的实例。
		/// </summary>
		/// <param name="isExplicit">类型检查时，使用显式类型转换，而不是默认的隐式类型转换。</param>
		/// <param name="convertRefType">是否允许对按引用传递的类型进行类型转换。</param>
		/// <returns>如果第一个参数可以作为方法的实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool DetectInstanceType(bool isExplicit, bool convertRefType)
		{
			// 静态方法没有实例入参，也需要至少有一个实参。
			if (method.IsStatic || arguments.Length == 0)
			{
				return false;
			}
			instanceType = arguments[0];
			Type declaringType = method.DeclaringType!;
			if (instanceType == null)
			{
				instanceType = declaringType;
				return true;
			}
			if (instanceType.IsByRef)
			{
				instanceType = instanceType.GetElementType()!;
				if (!convertRefType)
				{
					// 按引用传递时不支持逆变和协变。
					return instanceType == declaringType;
				}
			}
			return declaringType.IsConvertFrom(instanceType, isExplicit);
		}

		/// <summary>
		/// 检测 params 参数和可变参数，params 参数和可变参数均不支持按引用传递。
		/// </summary>
		/// <param name="isExplicit">类型检查时，使用显式类型转换，而不是默认的隐式类型转换。</param>
		/// <returns>如果检测参数成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool DetectParamArray(bool isExplicit)
		{
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			int argLen = arguments.Length;
			int offset = parameters.Length + (instanceType == null ? 0 : 1);
			if (method.CallingConvention.HasFlag(CallingConventions.VarArgs))
			{
				// 未传入可变参数。
				if (offset >= argLen) { return true; }
				optionalArgumentTypes = new ArraySegment<Type?>(arguments, offset, argLen - offset);
				return true;
			}
			if (parameters.Length > 0)
			{
				ParameterInfo lastParam = parameters[^1];
				if (lastParam.IsParamArray())
				{
					// 未传入 params 参数。
					if (offset - 1 >= argLen) { return true; }
					Type paramArrayType = lastParam.ParameterType;
					if (offset == argLen)
					{
						// 只有一个 params 实参，可能是数组或者数组元素。
						Type? type = arguments[^1];
						// 是数组，不需要特殊处理 params 参数
						if (type == null || paramArrayType.IsConvertFrom(type, isExplicit))
						{
							return true;
						}
						// 包含泛型参数时，无法确认类型兼容，认为实参是数组元素。
						if (paramArrayType.ContainsGenericParameters ||
							paramArrayType.GetElementType()!.IsConvertFrom(type, isExplicit))
						{
							this.paramArrayType = paramArrayType;
							paramArgumentTypes = new ArraySegment<Type?>(arguments, offset - 1, 1);
							return true;
						}
						return false;
					}
					// 有多个 params 实参，只能是数组元素。
					this.paramArrayType = paramArrayType;
					paramArgumentTypes = new(arguments, offset - 1, argLen + 1 - offset);
					Type paramElementType = paramArrayType.GetElementType()!;
					if (paramElementType.ContainsGenericParameters)
					{
						// 包含泛型参数时，无法确认类型兼容。
						return true;
					}
					foreach (Type? type in paramArgumentTypes)
					{
						if (type != null && !paramElementType.IsConvertFrom(type, isExplicit))
						{
							return false;
						}
					}
					return true;
				}
			}
			return true;
		}

		/// <summary>
		/// 检测固定实参类型。
		/// </summary>
		/// <param name="optionalParamBinding">是否对可选参数进行绑定。</param>
		/// <param name="isExplicit">类型检查时，使用显式类型转换，而不是默认的隐式类型转换。</param>
		/// <param name="convertRefType">是否允许对按引用传递的类型进行类型转换。</param>
		/// <returns>如果成功检测固定实参类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool DetectFixedArguments(bool optionalParamBinding, bool isExplicit, bool convertRefType)
		{
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			int paramLen = parameters.Length;
			if (paramArrayType != null)
			{
				paramLen--;
			}
			int offset = (instanceType == null ? 0 : 1);
			fixedArguments = new ArraySegment<Type?>(arguments, offset, Math.Min(paramLen, arguments.Length - offset));
			for (int i = 0; i < paramLen; i++)
			{
				ParameterInfo parameter = parameters[i];
				if (i >= fixedArguments.Count)
				{
					// 未指定实参，检查可选参数（有默认值）和 params 参数（可指定为空数组）。
					if (!parameter.IsParamArray() && !(optionalParamBinding && parameter.HasDefaultValue))
					{
						return false;
					}
					continue;
				}
				Type? type = fixedArguments[i];
				if (type == null)
				{
					continue;
				}
				Type paramType = parameter.ParameterType;
				if (paramType.ContainsGenericParameters)
				{
					// 泛型参数无法检查类型。
					continue;
				}
				bool isByRef = false;
				if (paramType.IsByRef)
				{
					paramType = paramType.GetElementType()!;
					isByRef = true;
				}
				if (type.IsByRef)
				{
					type = type.GetElementType()!;
				}
				if (isByRef && !convertRefType && type != paramType)
				{
					return false;
				}
				if (!paramType.IsConvertFrom(type, isExplicit))
				{
					return false;
				}
			}
			return true;
		}

		#endregion // 获取方法参数类型

		/// <summary>
		/// 根据给定的方法实参类型数组推断泛型方法的泛型类型。
		/// </summary>
		/// <param name="returnType">方法的实际返回类型，如果不存在则传入 <c>null</c>。</param>
		/// <param name="options">方法参数信息的选项。</param>
		/// <returns>如果成功推断泛型方法的类型参数，则为推断结果；否则为 <c>null</c>。</returns>
		public Type[]? GetGenericArguments(Type? returnType, MethodArgumentsOption options)
		{
			// 对方法返回值进行推断。
			bool isExplicit = options.HasFlag(MethodArgumentsOption.Explicit);
			TypeBounds bounds = new(method.GetGenericArguments());
			if (!CheckReturnType(returnType, bounds, isExplicit))
			{
				return null;
			}
			// 对方法固定参数进行推断。
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			int paramLen = fixedArguments.Count;
			for (int i = 0; i < paramLen; i++)
			{
				Type paramType = parameters[i].ParameterType;
				if (i < fixedArguments.Count && paramType.ContainsGenericParameters)
				{
					Type? argType = fixedArguments[i];
					if (argType != null && !bounds.TypeInferences(paramType, argType))
					{
						return null;
					}
				}
			}
			IList<Type>? paramArgTypes = paramArgumentTypes;
			if (paramArrayType == null || paramArgTypes == null || paramArgTypes.Count == 0)
			{
				return bounds.FixTypeArguments();
			}
			// 对 params 参数进行推断。
			Type paramElementType = paramArrayType.GetElementType()!;
			int paramArgCnt = paramArgTypes.Count;
			if (paramArgCnt > 1)
			{
				// 多个实参对应一个形参，做多次类型推断。
				for (int i = 0; i < paramArgCnt; i++)
				{
					if (!bounds.TypeInferences(paramElementType, paramArgTypes[i]))
					{
						return null;
					}
				}
				return bounds.FixTypeArguments();
			}
			// 一个实参对应一个形参，需要判断是否需要展开 params 参数。
			TypeBounds newBounds = new(bounds);
			Type type = paramArgTypes[0];
			// 首先尝试对 paramArrayType 进行推断。
			if (bounds.TypeInferences(paramArrayType, type))
			{
				Type[]? args = bounds.FixTypeArguments();
				if (args != null)
				{
					// 推断成功的话，则无需展开 params 参数。
					if (paramArrayType != null)
					{
						paramArrayType = null;
						paramArgumentTypes = null;
						fixedArguments = new ArraySegment<Type?>(arguments, fixedArguments.Offset, fixedArguments.Count + 1);
					}
					return args;
				}
			}
			// 然后尝试对 paramElementType 进行推断。
			if (newBounds.TypeInferences(paramElementType, type))
			{
				return newBounds.FixTypeArguments();
			}
			return null;
		}

		/// <summary>
		/// 检查方法的返回类型。
		/// </summary>
		/// <param name="returnType">方法的实际返回类型，如果不存在则传入 <c>null</c>。</param>
		/// <param name="bounds">界限集集合。</param>
		/// <param name="isExplicit">类型检查时，如果考虑显式类型转换，则为 <c>true</c>；
		/// 否则只考虑隐式类型转换。</param>
		/// <returns>如果成功检查返回类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckReturnType(Type? returnType, TypeBounds bounds, bool isExplicit)
		{
			if (returnType == null)
			{
				return true;
			}
			if (method is not MethodInfo methodInfo)
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
	}

	/// <summary>
	/// 方法实参的选项。
	/// </summary>
	[Flags]
	public enum MethodArgumentsOption
	{
		/// <summary>
		/// 无任何选项。
		/// </summary>
		None = 0,
		/// <summary>
		/// 方法的第一个实参表示方法实例。
		/// </summary>
		ContainsInstance = 1 << 0,
		/// <summary>
		/// 对可选参数进行绑定。
		/// </summary>
		OptionalParamBinding = 1 << 1,
		/// <summary>
		/// 类型检查时，使用显式类型转换，而不是默认的隐式类型转换。
		/// </summary>
		Explicit = 1 << 2,
		/// <summary>
		/// 允许对按引用传递的类型进行类型转换。
		/// </summary>
		ConvertRefType = 1 << 3,
		/// <summary>
		/// 对可选参数进行绑定，且使用显式类型转换。
		/// </summary>
		OptionalAndExplicit = OptionalParamBinding | Explicit,
		/// <summary>
		/// 对可变参数（VarArgs）进行绑定，注意 <see cref="Type.InvokeMember(string, BindingFlags, Binder, object, object[])"/> 
		/// 并不支持可变参数。
		/// </summary>
		VarArgsParamBinding = 1 << 4,
	}
}