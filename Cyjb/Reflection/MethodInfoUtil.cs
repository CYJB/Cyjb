using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="MethodInfo"/> 及其子类的扩展方法。
	/// </summary>
	public static class MethodInfoUtil
	{
		/// <summary>
		/// 返回当前方法的参数类型列表，使用最后一个类型表示返回值类型。
		/// </summary>
		/// <param name="method">要获取参数类型列表的方法。</param>
		/// <returns>方法的参数类型列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static Type[] GetParameterTypesWithReturn(this MethodInfo method)
		{
			ArgumentNullException.ThrowIfNull(method);
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

		#region 泛型参数推断

		/// <summary>
		/// 根据实参参数类型推断当前泛型方法定义的类型参数，并返回表示结果封闭构造方法的 <see cref="MethodInfo"/> 对象。
		/// </summary>
		/// <param name="method">要进行类型参数推断的泛型方法定义。</param>
		/// <param name="types">泛型方法的实参参数数组。</param>
		/// <returns>一个 <see cref="MethodInfo"/> 对象，表示通过将当前泛型方法定义的类型参数替换为根据 
		/// <paramref name="types"/> 推断得到的元素生成的封闭构造方法。</returns>
		/// <exception cref="InvalidOperationException">当前 <see cref="MethodInfo"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="MethodBase.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException">不能从 <paramref name="types"/> 推断得到类型参数。</exception>
		/// <exception cref="ArgumentException">根据 <paramref name="types"/>
		/// 推断出来的类型参数中的某个元素不满足为当前泛型方法定义的相应类型参数指定的约束。</exception>
		/// <overloads>
		/// <summary>
		/// 根据实参参数类型推断当前泛型方法定义的类型参数，
		/// 并返回表示结果封闭构造方法的 <see cref="MethodInfo"/> 对象。
		/// </summary>
		/// </overloads>
		public static MethodInfo MakeGenericMethodFromArgumentTypes(this MethodInfo method, params Type[] types)
		{
			ArgumentNullException.ThrowIfNull(method);
			ArgumentNullException.ThrowIfNull(types);
			if (!method.IsGenericMethodDefinition)
			{
				throw ReflectionExceptions.NeedGenericMethodDefinition(nameof(method));
			}
			MethodArgumentsOption options = MethodArgumentsOption.OptionalParamBinding;
			Type[]? genericArguments = MethodArgumentsInfo.GetInfo(method, types, options)?.GetGenericArguments(null, options);
			if (genericArguments == null)
			{
				throw ReflectionExceptions.CannotInferenceGenericArguments(nameof(method));
			}
			return method.MakeGenericMethod(genericArguments);
		}

		/// <summary>
		/// 根据实参参数类型推断当前泛型方法定义的类型参数，并返回表示结果封闭构造方法的 <see cref="MethodInfo"/> 对象。
		/// </summary>
		/// <param name="method">要进行类型参数推断的泛型方法定义。</param>
		/// <param name="types">泛型方法的实参参数数组。</param>
		/// <param name="options">泛型类型推断的选项。</param>
		/// <returns>一个 <see cref="MethodInfo"/> 对象，表示通过将当前泛型方法定义的类型参数替换为根据 
		/// <paramref name="types"/> 推断得到的元素生成的封闭构造方法。</returns>
		/// <exception cref="InvalidOperationException">当前 <see cref="MethodInfo"/> 不表示泛型方法定义。
		/// 也就是说，<see cref="MethodBase.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException">不能从 <paramref name="types"/> 推断得到类型参数。</exception>
		/// <exception cref="ArgumentException">根据 <paramref name="types"/>
		/// 推断出来的类型参数中的某个元素不满足为当前泛型方法定义的相应类型参数指定的约束。</exception>
		public static MethodInfo MakeGenericMethodFromArgumentTypes(this MethodInfo method, Type[] types,
			MethodArgumentsOption options)
		{
			ArgumentNullException.ThrowIfNull(method);
			ArgumentNullException.ThrowIfNull(types);
			if (!method.IsGenericMethodDefinition)
			{
				throw ReflectionExceptions.NeedGenericMethodDefinition(nameof(method));
			}
			Type[]? genericArguments = MethodArgumentsInfo.GetInfo(method, types, options)?.GetGenericArguments(null, options);
			if (genericArguments == null)
			{
				throw ReflectionExceptions.CannotInferenceGenericArguments(nameof(method));
			}
			return method.MakeGenericMethod(genericArguments);
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
		/// 也就是说，<see cref="MethodBase.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 根据给定的方法实参类型数组推断泛型方法的泛型类型。
		/// </summary>
		/// </overloads>
		public static Type[]? GenericArgumentsInferences(this MethodBase method, params Type[] types)
		{
			ArgumentNullException.ThrowIfNull(method);
			ArgumentNullException.ThrowIfNull(types);
			if (method.IsGenericMethodDefinition)
			{
				MethodArgumentsOption options = MethodArgumentsOption.OptionalParamBinding;
				return MethodArgumentsInfo.GetInfo(method, types, options)?.GetGenericArguments(null, options);
			}
			else
			{
				throw ReflectionExceptions.NeedGenericMethodDefinition(nameof(method));
			}
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
		/// 也就是说，<see cref="MethodBase.IsGenericMethodDefinition "/> 返回 <c>false</c>。</exception>
		public static Type[]? GenericArgumentsInferences(this MethodBase method, Type[] types,
			MethodArgumentsOption options)
		{
			ArgumentNullException.ThrowIfNull(method);
			ArgumentNullException.ThrowIfNull(types);
			if (method.IsGenericMethodDefinition)
			{
				return MethodArgumentsInfo.GetInfo(method, types, options)?.GetGenericArguments(null, options);
			}
			else
			{
				throw ReflectionExceptions.NeedGenericMethodDefinition(nameof(method));
			}
		}

		#endregion // 泛型参数推断

	}
}
