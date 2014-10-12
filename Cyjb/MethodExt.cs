using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.Reflection.MethodBase"/> 及其子类的扩展方法。
	/// </summary>
	public static class MethodExt
	{

		#region 参数顺序

		/// <summary>
		/// 默认的参数顺序数组。
		/// </summary>
		private static int[] defaultParamOrders = new int[] { 0 };
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
		/// 并返回表示结果构造方法的 <see cref="System.Reflection.MethodInfo"/> 对象。
		/// </summary>
		/// <param name="method">要进行类型参数推断的泛型方法。</param>
		/// <param name="types">泛型方法的实参参数数组。</param>
		/// <returns>一个 <see cref="System.Reflection.MethodInfo"/> 对象，
		/// 表示通过将当前泛型方法定义的类型参数替换为根据 <paramref name="types"/> 
		/// 推断得到的元素生成的构造方法。</returns>
		/// <exception cref="System.InvalidOperationException">
		/// 当前 <see cref="System.Reflection.MethodBase"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="System.Reflection.MethodBase.IsGenericMethodDefinition "/> 
		/// 返回 <c>false</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">不能从 <paramref name="types"/> 推断得到类型参数。</exception>
		/// <exception cref="System.ArgumentException">根据 <paramref name="types"/>
		/// 推断出来的类型参数中的某个元素不满足为当前泛型方法定义的相应类型参数指定的约束。</exception>
		public static MethodInfo MakeGenericMethodFromParams(this MethodInfo method, params Type[] types)
		{
			Type[] args = GenericArgumentsInferences(method, types);
			if (args == null)
			{
				throw CommonExceptions.CannotInferGenericArguments("types", method);
			}
			return method.MakeGenericMethod(args);
		}
		/// <summary>
		/// 根据实参参数类型推断当前泛型方法定义的类型参数，
		/// 并返回表示结果构造方法的 <see cref="System.Reflection.MethodInfo"/> 对象。
		/// </summary>
		/// <param name="method">要进行类型参数推断的泛型方法。</param>
		/// <param name="parameters">泛型方法的形参参数数组。</param>
		/// <param name="types">泛型方法的实参参数数组。</param>
		/// <returns>一个 <see cref="System.Reflection.MethodInfo"/> 对象，
		/// 表示通过将当前泛型方法定义的类型参数替换为根据 <paramref name="types"/> 
		/// 推断得到的元素生成的构造方法。如果构造失败，则为 <c>null</c>。</returns>
		internal static MethodInfo MakeGenericMethodFromParams(this MethodInfo method,
			ParameterInfo[] parameters, Type[] types)
		{
			Debug.Assert(method.IsGenericMethodDefinition);
			Debug.Assert(parameters != null);
			Debug.Assert(types != null);
			Type[] args = GenericArgumentsInferences(method, parameters, types);
			if (args != null)
			{
				try
				{
					return method.MakeGenericMethod(args);
				}
				catch (ArgumentException)
				{
					// 不满足方法的约束。
				}
			}
			return null;
		}
		/// <summary>
		/// 根据给定的类型数组推断泛型方法的类型参数。
		/// </summary>
		/// <param name="method">要推断类型参数的泛型方法。</param>
		/// <param name="types">实参参数数组。</param>
		/// <returns>如果成功推断泛型方法的类型参数，则为类型参数数组；
		/// 如果推断失败，则为 <c>null</c>。</returns>
		/// <exception cref="System.InvalidOperationException">
		/// 当前 <see cref="System.Reflection.MethodBase"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="System.Reflection.MethodBase.IsGenericMethodDefinition "/> 
		/// 返回 <c>false</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		public static Type[] GenericArgumentsInferences(this MethodBase method, params Type[] types)
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			CommonExceptions.CheckArgumentNull(types, "types");
			if (method.IsGenericMethodDefinition)
			{
				ParameterInfo[] parameters = method.GetParameters();
				return GenericArgumentsInferences(method, parameters, types);
			}
			else
			{
				throw CommonExceptions.NotGenericMethodDefinition(method, "GenericArgumentsInferences");
			}
		}
		/// <summary>
		/// 根据给定的类型数组推断泛型方法的类型参数。
		/// </summary>
		/// <param name="method">要推断类型参数的泛型方法。</param>
		/// <param name="parameters">泛型方法的形参参数数组。</param>
		/// <param name="types">实参参数数组。</param>
		/// <returns>如果成功推断泛型方法的类型参数，则为类型参数数组；
		/// 如果推断失败或给定的方法不是泛型方法定义，则为 <c>null</c>。</returns>
		internal static Type[] GenericArgumentsInferences(this MethodBase method,
			ParameterInfo[] parameters, Type[] types)
		{
			Debug.Assert(method.IsGenericMethodDefinition);
			Debug.Assert(parameters != null);
			Debug.Assert(types != null);
			int len = parameters.Length > types.Length ? parameters.Length : types.Length;
			int[] paramOrder = GetParamOrder(len);
			Type paramArrayType;
			if (CheckParameterCount(parameters, types, paramOrder, true, out paramArrayType))
			{
				Type[] paramTypes = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					paramTypes[i] = parameters[i].ParameterType;
				}
				return TypeExt.GenericArgumentsInferences(method.GetGenericArguments(), paramTypes,
					ref paramArrayType, types, paramOrder);
			}
			return null;
		}
		/// <summary>
		/// 检测方法的参数数量是否与给定的类型匹配。
		/// </summary>
		/// <param name="parameters">方法的形参数组。</param>
		/// <param name="types">实参数组。</param>
		/// <param name="paramOrder">实参参数顺序。</param>
		/// <param name="optionalParamBinding">是否绑定默认值。</param>
		/// <param name="paramArrayType">方法的 params 参数元素类型。</param>
		/// <returns>参数数量匹配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool CheckParameterCount(ParameterInfo[] parameters, Type[] types,
			int[] paramOrder, bool optionalParamBinding, out Type paramArrayType)
		{
			paramArrayType = null;
			int len = parameters.Length;
			if (len == 0)
			{
				// 判断包含变量参数的方法。
				return (types.Length == 0);
			}
			else if (len > types.Length)
			{
				// 方法形参过多，要求指定可选参数绑定。
				if (!optionalParamBinding)
				{
					return false;
				}
				// 没有实参可匹配的参数必须有默认值。
				// 检查最后一个参数是否有默认值，或者是 params 参数。
				len--;
				if (paramOrder[len] >= types.Length && parameters[len].DefaultValue == DBNull.Value)
				{
					if ((paramArrayType = GetParamArrayType(parameters[len])) == null)
					{
						return false;
					}
				}
				for (len--; len >= 0; len--)
				{
					if (paramOrder[len] >= types.Length && parameters[len].DefaultValue == DBNull.Value)
					{
						return false;
					}
				}
				return true;
			}
			else if (len < types.Length)
			{
				// 方法形参过多，要求具有 params 参数。
				return ((paramArrayType = GetParamArrayType(parameters[len - 1])) != null);
			}
			else
			{
				// 参数数量相等。
				len--;
				if ((paramArrayType = GetParamArrayType(parameters[len])) != null)
				{
					// 判断是否需要展开 params 参数。
					Type paramElementType;
					int paramDepth = paramArrayType.GetArrayDepth(out paramElementType);
					int typeDepth = types[paramOrder[len]].GetArrayDepth();
					if (paramElementType.IsGenericParameter)
					{
						// 如果是 params T 数组，那么不能判定 T 本身的数组层数。
						if (typeDepth < paramDepth)
						{
							// 如果实参数组层数少，那么肯定无法正确匹配。
							return false;
						}
					}
					else
					{
						if (paramDepth + 1 == typeDepth)
						{
							// 数组深度相差 1，不需要展开 params 参数。
							paramArrayType = null;
						}
						else if (paramDepth != typeDepth)
						{
							// 数组深度不等。
							return false;
						}
					}
				}
				return true;
			}
		}
		/// <summary>
		/// 如果给定的参数信息是 params 参数，则返回对应的数组元素类型；否则为 <c>null</c>。
		/// </summary>
		/// <param name="parameter">参数信息。</param>
		/// <returns>params 参数的元素类型。</returns>
		internal static Type GetParamArrayType(ParameterInfo parameter)
		{
			if (parameter.ParameterType.IsArray && parameter.IsDefined(typeof(ParamArrayAttribute), true))
			{
				return parameter.ParameterType.GetElementType();
			}
			return null;
		}

		#endregion // 泛型参数推断

	}
}
