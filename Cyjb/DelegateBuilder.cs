using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 表示通用方法的调用委托。
	/// </summary>
	/// <param name="instance">要调用方法的实例。</param>
	/// <param name="parameters">方法的参数。</param>
	/// <returns>方法的返回值。</returns>
	public delegate object MethodInvoker(object instance, params object[] parameters);
	/// <summary>
	/// 表示通用构造函数的调用委托。
	/// </summary>
	/// <param name="parameters">构造函数的参数。</param>
	/// <returns>新创建的实例。</returns>
	public delegate object InstanceCreator(params object[] parameters);
	/// <summary>
	/// 提供动态构造方法、属性或字段委托的方法。
	/// </summary>
	public static class DelegateBuilder
	{

		#region 从 MethodInfo 构造方法委托

		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TDelegate"/> 不继承 <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), method, true) as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TDelegate"/> 不继承 <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method, bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), method, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(Type type, MethodInfo method)
		{
			return CreateDelegate(type, method, true);
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(Type type, MethodInfo method, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(method, "method");
			CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			Delegate dlg = CreateOpenDelegate(type, invoke, invoke.GetParameters(), method, method.GetParameters());
			if (dlg == null && throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 创建表示指定的静态或实例方法的的委托。
		/// 如果是实例方法，需要将实例对象作为第一个参数；如果是静态方法，则第一个参数无效。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <returns>表示指定的静态或实例方法的委托。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static MethodInvoker CreateDelegate(this MethodInfo method)
		{
			ExceptionHelper.CheckArgumentNull(method, "method");
			if (method.IsGenericMethodDefinition)
			{
				// 不对开放的泛型方法执行绑定。
				throw ExceptionHelper.BindTargetMethod("method");
			}
			// 要执行方法的实例。
			ParameterExpression instanceParam = Expression.Parameter(typeof(object));
			// 方法的参数。
			ParameterExpression parametersParam = Expression.Parameter(typeof(object[]));
			// 构造参数列表。
			ParameterInfo[] methodParams = method.GetParameters();
			Expression[] paramExps = new Expression[methodParams.Length];
			for (int i = 0; i < methodParams.Length; i++)
			{
				// (Ti)parameters[i]
				paramExps[i] = ConvertType(
					Expression.ArrayIndex(parametersParam, Expression.Constant(i)),
					methodParams[i].ParameterType);
			}
			// 静态方法不需要实例，实例方法需要 (TInstance)instance
			Expression instanceCast = method.IsStatic ? null :
				ConvertType(instanceParam, method.DeclaringType);
			// 调用方法。
			Expression methodCall = Expression.Call(instanceCast, method, paramExps);
			// 添加参数数量检测。
			methodCall = Expression.Block(GetCheckParameterExp(parametersParam, methodParams.Length), methodCall);
			return Expression.Lambda<MethodInvoker>(GetReturn(methodCall, typeof(object)),
				instanceParam, parametersParam).Compile();
		}
		/// <summary>
		/// 创建指定的静态或实例方法的指定类型的开放方法委托。
		/// 如果是实例方法，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="invoke">委托的方法信息。</param>
		/// <param name="invokeParams">委托的方法参数列表。</param>
		/// <param name="method">目标方法的信息。</param>
		/// <param name="methodParams">目标方法的参数列表。</param>
		/// <returns>指定类型的委托，表示静态方法或实例方法。</returns>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		private static Delegate CreateOpenDelegate(Type type,
			MethodInfo invoke, ParameterInfo[] invokeParams,
			MethodInfo method, ParameterInfo[] methodParams)
		{
			// 要求参数数量匹配，其中实例方法的第一个参数用作传递实例对象。
			int skipIdx = method.IsStatic ? 0 : 1;
			if (invokeParams.Length == methodParams.Length + skipIdx)
			{
				if (method.IsGenericMethodDefinition)
				{
					// 构造泛型方法的封闭方法，对于实例方法要跳过第一个参数。
					Type[] paramTypes = GetParameterTypes(invokeParams, skipIdx, 0, 0);
					method = method.MakeGenericMethodFromParams(methodParams, paramTypes);
					if (method == null) { return null; }
					methodParams = method.GetParameters();
				}
				// 方法的参数列表。
				ParameterExpression[] paramList = GetParameters(invokeParams);
				// 构造调用参数列表。
				Expression[] paramExps = GetParameterExpressions(paramList, skipIdx, methodParams, 0);
				if (paramExps != null)
				{
					// 调用方法的实例对象。
					Expression instance = null;
					if (skipIdx == 1)
					{
						instance = ConvertType(paramList[0], method.DeclaringType);
						if (instance == null)
						{
							return null;
						}
					}
					Expression methodCall = Expression.Call(instance, method, paramExps);
					methodCall = GetReturn(methodCall, invoke.ReturnType);
					if (methodCall != null)
					{
						return Expression.Lambda(type, methodCall, paramList).Compile();
					}
				}
			}
			return null;
		}

		#endregion // 从 MethodInfo 构造方法委托

		#region 从 MethodInfo 构造带有第一个参数的方法委托

		/// <summary>
		/// 使用指定的第一个参数创建表示指定的静态或实例方法的指定类型的委托。 
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="method"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method, object firstArgument)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), method, firstArgument, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建表示指定的静态或实例方法的指定类型的委托。 
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="method"/> 视为 <c>static</c>。</param>
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method, object firstArgument,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), method, firstArgument, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数创建表示指定的静态或实例方法的指定类型的委托。 
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="method"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(Type type, MethodInfo method, object firstArgument)
		{
			return CreateDelegate(type, method, firstArgument, true);
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建表示指定的静态或实例方法的指定类型的委托。 
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="method">描述委托要表示的静态或实例方法的 
		/// <see cref="System.Reflection.MethodInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="method"/> 视为 <c>static</c>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(Type type, MethodInfo method, object firstArgument, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(method, "method");
			CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			ParameterInfo[] invokeParams = invoke.GetParameters();
			ParameterInfo[] methodParams = method.GetParameters();
			// 尝试创建带有第一个参数的方法委托。
			Delegate dlg = CreateDelegateWithArgument(type, firstArgument, invoke, invokeParams, method, methodParams);
			if (dlg == null && throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例方法的指定类型的带有第一个参数的方法委托。
		/// 如果是实例方法，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">委托的类型。</param>
		/// <param name="firstArgument">委托表示的方法的第一个参数。</param>
		/// <param name="invoke">委托的方法信息。</param>
		/// <param name="invokeParams">委托的参数列表。</param>
		/// <param name="method">目标方法的信息。</param>
		/// <param name="methodParams">目标方法的参数列表。</param>
		/// <returns>指定类型的委托，表示静态方法或实例方法。</returns>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		private static Delegate CreateDelegateWithArgument(Type type, object firstArgument,
			MethodInfo invoke, ParameterInfo[] invokeParams,
			MethodInfo method, ParameterInfo[] methodParams)
		{
			// 尝试创建开放的方法委托。
			Delegate dlg = CreateOpenDelegate(type, invoke, invokeParams, method, methodParams);
			if (dlg != null)
			{
				return dlg;
			}
			// 尝试创建封闭的方法委托。
			if (firstArgument == null && !method.IsStatic && invokeParams.Length == methodParams.Length)
			{
				return CreateNullCloseInstanceDelegate(type, invoke, invokeParams, method, methodParams);
			}
			else
			{
				return CreateCloseDelegate(type, firstArgument, invoke, invokeParams, method, methodParams);
			}
		}
		/// <summary>
		/// 创建指定的实例方法的通过空引用封闭的方法委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">委托的类型。</param>
		/// <param name="invoke">委托的方法信息。</param>
		/// <param name="invokeParams">委托的参数列表。</param>
		/// <param name="method">目标方法的信息。</param>
		/// <param name="methodParams">目标方法的参数列表。</param>
		/// <returns>指定类型的委托，表示通过空引用封闭的实例方法。</returns>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		private static Delegate CreateNullCloseInstanceDelegate(Type type,
			MethodInfo invoke, ParameterInfo[] invokeParams,
			MethodInfo method, ParameterInfo[] methodParams)
		{
			// 通过空引用封闭实例方法，类似于对空实例调用实例方法，无法自己产生代码，
			if (method.IsGenericMethodDefinition)
			{
				// 构造泛型方法的封闭方法，对于静态方法要在前面添加一个参数。
				Type[] paramTypes = GetParameterTypes(invokeParams, 0, 0, 0);
				method = method.MakeGenericMethodFromParams(methodParams, paramTypes);
				if (method == null) { return null; }
				methodParams = method.GetParameters();
			}
			// 尝试直接使用 CreateDelegate 进行创建。
			Delegate dlg = Delegate.CreateDelegate(type, null, method, false);
			if (dlg == null)
			{
				// 尝试包装强制类型转换的代码。
				// 生成与 method 完全匹配的委托类型。
				Type[] methodTypes = GetParameterTypes(methodParams, 0, 0, 1);
				methodTypes[methodParams.Length] = method.ReturnType;
				Type delType = Expression.GetDelegateType(methodTypes);
				dlg = Delegate.CreateDelegate(delType, null, method, false);
				if (dlg != null)
				{
					// 将由 Delegate 创建的委托进行参数的强制类型转换。
					ParameterExpression[] paramList = GetParameters(invokeParams);
					Expression[] paramExps = GetParameterExpressions(paramList, 0, methodParams, 0);
					if (paramExps != null)
					{
						Expression delInvoke = Expression.Invoke(Expression.Constant(dlg), paramExps);
						delInvoke = GetReturn(delInvoke, invoke.ReturnType);
						if (delInvoke != null)
						{
							dlg = Expression.Lambda(type, delInvoke, paramList).Compile();
						}
					}
				}
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例方法的指定类型的封闭方法委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">委托的类型。</param>
		/// <param name="firstArgument">委托表示的方法的第一个参数。</param>
		/// <param name="invoke">委托的方法信息。</param>
		/// <param name="invokeParams">委托的参数列表。</param>
		/// <param name="method">目标方法的信息。</param>
		/// <param name="methodParams">目标方法的参数列表。</param>
		/// <returns>指定类型的委托，表示静态方法或实例方法。</returns>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="method"/>。</exception>
		private static Delegate CreateCloseDelegate(Type type, object firstArgument,
			MethodInfo invoke, ParameterInfo[] invokeParams,
			MethodInfo method, ParameterInfo[] methodParams)
		{
			// 要求参数数量匹配，其中静态方法的第一个参数被封闭。
			int skipIdx = method.IsStatic ? 1 : 0;
			if (invokeParams.Length == methodParams.Length - skipIdx)
			{
				if (method.IsGenericMethodDefinition)
				{
					// 构造泛型方法的封闭方法，对于静态方法要在前面添加一个参数。
					Type[] paramTypes = GetParameterTypes(invokeParams, 0, skipIdx, 0);
					if (skipIdx == 1)
					{
						// 将第一个参数的类型填充到参数类型列表中。
						paramTypes[0] = firstArgument == null ? typeof(object) : firstArgument.GetType();
					}
					method = method.MakeGenericMethodFromParams(methodParams, paramTypes);
					if (method == null) { return null; }
					methodParams = method.GetParameters();
				}
				// 方法的参数列表。
				ParameterExpression[] paramList = GetParameters(invokeParams);
				// 构造调用参数列表。
				Expression[] paramExps = GetParameterExpressions(paramList, 0, methodParams, skipIdx);
				if (paramExps != null)
				{
					Expression instance = null;
					if (skipIdx == 1)
					{
						paramExps[0] = ConvertType(Expression.Constant(firstArgument), methodParams[0].ParameterType);
						if (paramExps[0] == null)
						{
							// 不允许进行强制类型转换。
							return null;
						}
					}
					else
					{
						instance = ConvertType(Expression.Constant(firstArgument), method.DeclaringType);
						if (instance == null)
						{
							// 不允许进行强制类型转换。
							return null;
						}
					}
					Expression methodCall = Expression.Call(instance, method, paramExps);
					methodCall = GetReturn(methodCall, invoke.ReturnType);
					if (methodCall != null)
					{
						return Expression.Lambda(type, methodCall, paramList).Compile();
					}
				}
			}
			return null;
		}

		#endregion // 从 MethodInfo 构造带有第一个参数的方法委托

		#region 从 ConstructorInfo 构造构造函数委托

		/// <summary>
		/// 创建用于表示指定构造函数的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="ctor">描述委托要表示的构造函数的 
		/// <see cref="System.Reflection.ConstructorInfo"/>。</param>
		/// <returns>指定类型的委托，表示指定的构造函数。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="ctor"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承 
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="ctor"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="ctor"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this ConstructorInfo ctor)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), ctor, true) as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定构造函数的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="ctor">描述委托要表示的构造函数的 
		/// <see cref="System.Reflection.ConstructorInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="ctor"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的构造函数。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="ctor"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承 
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="ctor"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="ctor"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this ConstructorInfo ctor, bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), ctor, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 创建用于表示指定构造函数的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="ctor">描述委托要表示的构造函数的 
		/// <see cref="System.Reflection.ConstructorInfo"/>。</param>
		/// <returns>指定类型的委托，表示指定的构造函数。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="ctor"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承 
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="ctor"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="ctor"/>。</exception>
		public static Delegate CreateDelegate(Type type, ConstructorInfo ctor)
		{
			return CreateDelegate(type, ctor, true);
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定构造函数的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="ctor">描述委托要表示的构造函数的 
		/// <see cref="System.Reflection.ConstructorInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="ctor"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的构造函数。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="ctor"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承 
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="ctor"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="ctor"/>。</exception>
		public static Delegate CreateDelegate(Type type, ConstructorInfo ctor, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(ctor, "ctor");
			CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			ParameterInfo[] invokeParams = invoke.GetParameters();
			ParameterInfo[] methodParams = ctor.GetParameters();
			// 要求参数数量匹配。
			if (invokeParams.Length == methodParams.Length)
			{
				// 构造函数的参数列表。
				ParameterExpression[] paramList = GetParameters(invokeParams);
				// 构造调用参数列表。
				Expression[] paramExps = GetParameterExpressions(paramList, 0, methodParams, 0);
				if (paramExps != null)
				{
					Expression methodCall = Expression.New(ctor, paramExps);
					methodCall = GetReturn(methodCall, invoke.ReturnType);
					if (methodCall != null)
					{
						return Expression.Lambda(type, methodCall, paramList).Compile();
					}
				}
			}
			if (throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetMethod("ctor");
			}
			return null;
		}
		/// <summary>
		/// 创建表示指定的构造函数的的委托。
		/// </summary>
		/// <param name="ctor">描述委托要表示的构造函数的 
		/// <see cref="System.Reflection.ConstructorInfo"/>。</param>
		/// <returns>表示指定的构造函数的委托。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="ctor"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="ctor"/>。</exception>
		public static InstanceCreator CreateDelegate(this ConstructorInfo ctor)
		{
			ExceptionHelper.CheckArgumentNull(ctor, "ctor");
			// 构造函数的参数。
			ParameterExpression parametersParam = Expression.Parameter(typeof(object[]));
			// 构造参数列表。
			ParameterInfo[] methodParams = ctor.GetParameters();
			Expression[] paramExps = new Expression[methodParams.Length];
			for (int i = 0; i < methodParams.Length; i++)
			{
				// (Ti)parameters[i]
				paramExps[i] = ConvertType(
					Expression.ArrayIndex(parametersParam, Expression.Constant(i)),
					methodParams[i].ParameterType);
			}
			// 新建实例。
			Expression methodCall = Expression.New(ctor, paramExps);
			// 添加参数数量检测。
			methodCall = Expression.Block(GetCheckParameterExp(parametersParam, methodParams.Length), methodCall);
			return Expression.Lambda<InstanceCreator>(GetReturn(methodCall, typeof(object)),
				parametersParam).Compile();
		}

		#endregion // 从 ConstructorInfo 构造构造函数委托

		#region 从 PropertyInfo 构造属性委托

		/// <summary>
		/// ，创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), property, true) as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property, bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), property, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property)
		{
			return CreateDelegate(type, property, true);
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(property, "property");
			CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			// 判断是获取属性还是设置属性。
			MethodInfo method = null;
			if (invoke.ReturnType == typeof(void))
			{
				method = property.GetSetMethod(true);
				if (method == null && throwOnBindFailure)
				{
					throw ExceptionHelper.BindTargetPropertyNoSet("property");
				}
			}
			else
			{
				method = property.GetGetMethod(true);
				if (method == null && throwOnBindFailure)
				{
					throw ExceptionHelper.BindTargetPropertyNoGet("property");
				}
			}
			Delegate dlg = null;
			if (method != null)
			{
				// 创建委托。
				dlg = CreateOpenDelegate(type, invoke, invoke.GetParameters(), method, method.GetParameters());
			}
			if (dlg == null && throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetProperty("property");
			}
			return dlg;
		}

		#endregion // 从 PropertyInfo 构造属性委托

		#region 从 PropertyInfo 构造带有第一个参数的属性委托

		/// <summary>
		/// 使用指定的第一个参数，创建表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="property"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property, object firstArgument)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), property, firstArgument, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="property"/> 视为 <c>static</c>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property,
			object firstArgument, bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), property, firstArgument, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数，创建表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="property"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property, object firstArgument)
		{
			return CreateDelegate(type, property, firstArgument, true);
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="property"/> 视为 <c>static</c>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property, object firstArgument,
			bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(property, "property");
			CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			// 判断是获取属性还是设置属性。
			MethodInfo method = null;
			if (invoke.ReturnType == typeof(void))
			{
				method = property.GetSetMethod(true);
				if (method == null && throwOnBindFailure)
				{
					throw ExceptionHelper.BindTargetPropertyNoSet("property");
				}
			}
			else
			{
				method = property.GetGetMethod(true);
				if (method == null && throwOnBindFailure)
				{
					throw ExceptionHelper.BindTargetPropertyNoGet("property");
				}
			}
			Delegate dlg = null;
			if (method != null)
			{
				// 创建委托。
				dlg = CreateDelegateWithArgument(type, firstArgument,
					invoke, invoke.GetParameters(), method, method.GetParameters());
			}
			if (dlg == null && throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetProperty("property");
			}
			return dlg;
		}

		#endregion // 从 PropertyInfo 构造带有第一个参数的属性委托

		#region 从 FieldInfo 构造字段委托

		/// <summary>
		/// 创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), field, true) as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field, bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), field, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(Type type, FieldInfo field)
		{
			return CreateDelegate(type, field, true);
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(Type type, FieldInfo field, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(field, "field");
			CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			Delegate dlg = CreateOpenDelegate(type, field, invoke, invoke.GetParameters());
			if (dlg == null && throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetField("field");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例字段的指定类型的开放字段委托。
		/// 如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="field">目标字段的信息。</param>
		/// <param name="invoke">委托的方法信息。</param>
		/// <param name="invokeParams">委托的方法参数列表。</param>
		/// <returns>指定类型的委托，表示静态方法或实例字段。</returns>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		private static Delegate CreateOpenDelegate(Type type, FieldInfo field,
			MethodInfo invoke, ParameterInfo[] invokeParams)
		{
			// 检查参数数量。
			bool setField = invoke.ReturnType == typeof(void);
			int paramLen = field.IsStatic ? 0 : 1;
			paramLen += setField ? 1 : 0;
			if (invokeParams.Length == paramLen)
			{
				// 委托的参数列表。
				ParameterExpression[] paramList = GetParameters(invokeParams);
				// 访问字段的实例对象。
				Expression instance = null;
				if (!field.IsStatic)
				{
					instance = ConvertType(paramList[0], field.DeclaringType);
					if (instance == null)
					{
						return null;
					}
				}
				Expression fieldExp = Expression.Field(instance, field);
				if (setField)
				{
					// 字段的设置值。
					Expression value = ConvertType(paramList[paramLen - 1], field.FieldType);
					if (value == null)
					{
						return null;
					}
					fieldExp = Expression.Assign(fieldExp, value);
				}
				fieldExp = GetReturn(fieldExp, invoke.ReturnType);
				if (fieldExp != null)
				{
					// 创建委托。
					return Expression.Lambda(type, fieldExp, paramList).Compile();
				}
			}
			return null;
		}

		#endregion // 从 FieldInfo 构造字段委托

		#region 从 FieldInfo 构造带有第一个参数的字段委托

		/// <summary>
		/// 使用指定的第一个参数创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="field"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field, object firstArgument)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), field, firstArgument, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="field"/> 视为 <c>static</c>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field, object firstArgument, bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), field, firstArgument, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="field"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(Type type, FieldInfo field, object firstArgument)
		{
			return CreateDelegate(type, field, firstArgument, true);
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="field">描述委托要表示的静态或实例字段的 
		/// <see cref="System.Reflection.PropertyInfo"/>。</param>
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="field"/> 视为 <c>static</c>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">不能绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(Type type, FieldInfo field, object firstArgument, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(field, "field");
			CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			ParameterInfo[] invokeParams = invoke.GetParameters();
			// 尝试创建开放的字段委托。
			Delegate dlg = CreateOpenDelegate(type, field, invoke, invokeParams);
			if (dlg != null)
			{
				return dlg;
			}
			// 尝试创建封闭的字段委托。
			bool setField = invoke.ReturnType == typeof(void);
			// 检查参数数量。
			if (invokeParams.Length == ((field.IsStatic || !setField) ? 0 : 1))
			{
				if (firstArgument == null && !field.IsStatic)
				{
					// 通过空引用封闭的实例字段委托。
					// 委托的参数列表。
					ParameterExpression[] paramList = GetParameters(invokeParams);
					Expression throwExp = Expression.Throw(Expression.New(typeof(NullReferenceException)));
					Expression fieldExp = Expression.Block(throwExp, Expression.Default(field.FieldType));
					dlg = Expression.Lambda(type, fieldExp, paramList).Compile();
				}
				else
				{
					// 通过第一个参数封闭的字段委托。
					dlg = CreateCloseDelegate(type, field, firstArgument, invoke, invokeParams);
				}
			}
			if (dlg == null && throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetField("field");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例字段的指定类型的封闭字段委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="field">目标字段的信息。</param>
		/// <param name="firstArgument">委托表示的字段的第一个参数。</param>
		/// <param name="invoke">委托的方法信息。</param>
		/// <param name="invokeParams">委托的方法参数列表。</param>
		/// <returns>指定类型的委托，表示静态方法或实例字段。</returns>
		/// <exception cref="System.MethodAccessException">调用方无权访问
		/// <paramref name="field"/>。</exception>
		private static Delegate CreateCloseDelegate(Type type, FieldInfo field, object firstArgument,
			MethodInfo invoke, ParameterInfo[] invokeParams)
		{
			// 委托的参数列表。
			ParameterExpression[] paramList = GetParameters(invokeParams);
			// 访问字段的实例对象。
			Expression instance = null;
			if (!field.IsStatic)
			{
				instance = ConvertType(Expression.Constant(firstArgument), field.DeclaringType);
				if (instance == null)
				{
					return null;
				}
			}
			Expression fieldExp = Expression.Field(instance, field);
			if (invoke.ReturnType == typeof(void))
			{
				// 字段的设置值。
				Expression value = null;
				if (field.IsStatic)
				{
					value = ConvertType(Expression.Constant(firstArgument), field.FieldType);
				}
				else
				{
					value = ConvertType(paramList[0], field.FieldType);
				}
				if (value == null)
				{
					return null;
				}
				fieldExp = Expression.Assign(fieldExp, value);
			}
			fieldExp = GetReturn(fieldExp, invoke.ReturnType);
			if (fieldExp != null)
			{
				// 创建委托。
				return Expression.Lambda(type, fieldExp, paramList).Compile();
			}
			return null;
		}

		#endregion // 从 FieldInfo 构造带有第一个参数的字段委托

		#region 从 Type 构造成员委托

		/// <summary>
		/// 构造函数的名称。
		/// </summary>
		private const string ConstructorName = ".ctor";
		/// <summary>
		/// 默认的绑定设置值。
		/// </summary>
		private const BindingFlags BinderDefault = BindingFlags.Static | BindingFlags.Instance |
			BindingFlags.Public | BindingFlags.NonPublic;
		/// <summary>
		/// 获取或设置字段的有效值掩码。
		/// </summary>
		private const BindingFlags BinderGetSetField = BindingFlags.GetField | BindingFlags.SetField;
		/// <summary>
		/// 获取或设置属性的有效值掩码。
		/// </summary>
		private const BindingFlags BinderGetSetProperty = BindingFlags.GetProperty | BindingFlags.SetProperty;
		/// <summary>
		/// 属性或方法的有效值掩码。
		/// </summary>
		private const BindingFlags BinderMethodOrProperty = BinderGetSetProperty | BindingFlags.InvokeMethod;
		/// <summary>
		/// 所有成员有效值掩码。
		/// </summary>
		private const BindingFlags BinderMemberMask = BinderGetSetField | BinderGetSetProperty |
			BindingFlags.InvokeMethod | BindingFlags.CreateInstance;
		/// <summary>
		/// 绑定设置值的有效值掩码。
		/// </summary>
		private const BindingFlags BinderMask = BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance |
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.ExactBinding;
		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type target, string memberName)
			where TDelegate : class
		{
			return new Lazy<TDelegate>(() =>
				CreateDelegate(typeof(TDelegate), target, memberName, BinderDefault, true) as TDelegate);
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type target, string memberName, BindingFlags bindingAttr)
			where TDelegate : class
		{
			return new Lazy<TDelegate>(() =>
				CreateDelegate(typeof(TDelegate), target, memberName, bindingAttr, true) as TDelegate);
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type target, string memberName, BindingFlags bindingAttr,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			return new Lazy<TDelegate>(() =>
				CreateDelegate(typeof(TDelegate), target, memberName, bindingAttr, throwOnBindFailure) as TDelegate);
		}
		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type target, string memberName)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), target, memberName, BinderDefault, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type target, string memberName, BindingFlags bindingAttr)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), target, memberName, bindingAttr, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type target, string memberName, BindingFlags bindingAttr,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), target, memberName, bindingAttr, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, Type target, string memberName)
		{
			return CreateDelegate(type, target, memberName, BinderDefault, true);
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, Type target, string memberName, BindingFlags bindingAttr)
		{
			return CreateDelegate(type, target, memberName, bindingAttr, true);
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, Type target, string memberName, BindingFlags bindingAttr,
			bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(memberName, "memberName");
			CheckDelegateType(type, "type");
			CheckTargetType(target, "target");
			MethodInfo invoke = type.GetMethod("Invoke");
			ParameterInfo[] paramInfos = invoke.GetParameters();
			Type[] types = GetParameterTypes(paramInfos, 0, 0, 0);
			// 未设置 MemberMask 的情况下，查找所有成员。
			if ((bindingAttr & BinderMemberMask) == BindingFlags.Default)
			{
				bindingAttr |= BinderMemberMask;
			}
			BindingFlags instancecBindingAttr = bindingAttr & ~BindingFlags.Static;
			Delegate dlg = null;
			if (memberName.Equals(ConstructorName, StringComparison.OrdinalIgnoreCase))
			{
				// 查找构造函数成员，此时注意只搜索实例方法。
				ConstructorInfo ctor = target.GetConstructor(instancecBindingAttr | BindingFlags.Instance,
					PowerBinder.CastBinder, types, null);
				if (ctor != null)
				{
					dlg = CreateDelegate(type, ctor, false);
				}
			}
			else
			{
				// 查找其它成员。
				Type[] instanceTypes = null;
				bool containsStaticMember = (bindingAttr & BindingFlags.Static) == BindingFlags.Static;
				bool containsInstnceMember = (bindingAttr & BindingFlags.Instance) == BindingFlags.Instance;
				if (containsInstnceMember)
				{
					// 构造查找实例方法用的参数列表。
					if (types.Length - 1 < 0 ||
						(bindingAttr & BinderMethodOrProperty) == BindingFlags.Default)
					{
						// 参数个数为 0，不能是实例成员。
						// 没有方法或属性调用，也不用区分实例成员。
						containsInstnceMember = false;
					}
					else
					{
						instanceTypes = new Type[types.Length - 1];
						for (int i = 0; i < instanceTypes.Length; i++)
						{
							instanceTypes[i] = types[i + 1];
						}
					}
				}
				BindingFlags staticBindingAttr = bindingAttr & ~BindingFlags.Instance;
				if ((bindingAttr & BindingFlags.InvokeMethod) == BindingFlags.InvokeMethod)
				{
					// 查找静态方法成员。
					if (containsStaticMember)
					{
						dlg = CreateMethodDelegate(type, target, memberName, null, staticBindingAttr, types);
					}
					// 查找实例方法成员。
					if (dlg == null && containsInstnceMember)
					{
						dlg = CreateMethodDelegate(type, target, memberName, null, instancecBindingAttr, instanceTypes);
					}
				}
				if ((bindingAttr & BinderGetSetProperty) != BindingFlags.Default)
				{
					// 查找静态属性成员。
					if (dlg == null && containsStaticMember)
					{
						dlg = CreatePropertyDelegate(type, target, memberName, null, staticBindingAttr,
							invoke.ReturnType, types);
					}
					// 查找实例属性成员。
					if (dlg == null && containsInstnceMember)
					{
						dlg = CreatePropertyDelegate(type, target, memberName, null, instancecBindingAttr,
							invoke.ReturnType, instanceTypes);
					}
				}
				// 查找字段成员。
				if (dlg == null && (bindingAttr & BinderGetSetField) != BindingFlags.Default)
				{
					FieldInfo field = target.GetField(memberName, bindingAttr);
					if (field != null)
					{
						dlg = CreateDelegate(type, field, false);
					}
				}
			}
			if (dlg != null)
			{
				return dlg;
			}
			if (throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetMethod("memberName");
			}
			return null;
		}
		/// <summary>
		/// 使用指定的搜索方式创建用于表示静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="methodName">方法的名称。</param>
		/// <param name="firstArgument">委托要绑定到的对象。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="types">方法的签名。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		private static Delegate CreateMethodDelegate(Type type, Type target, string methodName, object firstArgument,
			BindingFlags bindingAttr, Type[] types)
		{
			MethodInfo method = target.GetMethod(methodName, bindingAttr, PowerBinder.CastBinder, types, null);
			if (method != null)
			{
				if (firstArgument == null)
				{
					return CreateDelegate(type, method, false);
				}
				else
				{
					return CreateDelegate(type, method, firstArgument, false);
				}
			}
			return null;
		}
		/// <summary>
		/// 使用指定的搜索方式创建用于表示静态或实例属性的指定类型的委托。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="propertyName">方法的名称。</param>
		/// <param name="firstArgument">委托要绑定到的对象。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="propType">属性的类型。</param>
		/// <param name="types">方法的签名。</param>
		/// <returns>指定类型的委托，表示指定的静态或属性方法。</returns>
		private static Delegate CreatePropertyDelegate(Type type, Type target, string propertyName,
			object firstArgument, BindingFlags bindingAttr, Type propType, Type[] types)
		{
			PropertyInfo property = null;
			if (propType == typeof(void))
			{
				// 是设置属性，将第一个参数作为属性类型。
				propType = types[0];
				if (types.Length == 1)
				{
					types = Type.EmptyTypes;
				}
				else
				{
					Type[] newTypes = new Type[types.Length - 1];
					for (int i = 0; i < newTypes.Length; i++)
					{
						newTypes[i] = types[i + 1];
					}
					types = newTypes;
				}
				property = target.GetProperty(propertyName, bindingAttr, PowerBinder.CastBinder,
					propType, types, null);
			}
			else
			{
				// 是获取属性。
				property = target.GetProperty(propertyName, bindingAttr, PowerBinder.CastBinder,
					propType, types, null);
			}
			if (property != null)
			{
				if (firstArgument == null)
				{
					return CreateDelegate(type, property, false);
				}
				else
				{
					return CreateDelegate(type, property, firstArgument, false);
				}
			}
			return null;
		}

		#endregion // 从 Type 构造成员委托

		#region 从 Type 构造带有第一个参数的成员委托

		/// <summary>
		/// 使用指定的名称和第一个参数，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type target, string memberName, object firstArgument)
			where TDelegate : class
		{
			return new Lazy<TDelegate>(() =>
				CreateDelegate(typeof(TDelegate), target, memberName, firstArgument, BinderDefault, true) as TDelegate);
		}
		/// <summary>
		/// 使用指定的名称、第一个参数和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type target, string memberName,
			object firstArgument, BindingFlags bindingAttr)
			where TDelegate : class
		{
			return new Lazy<TDelegate>(() =>
				CreateDelegate(typeof(TDelegate), target, memberName, firstArgument, bindingAttr, true) as TDelegate);
		}
		/// <summary>
		/// 使用指定的名称、第一个参数、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type target, string memberName,
			object firstArgument, BindingFlags bindingAttr, bool throwOnBindFailure)
			where TDelegate : class
		{
			return new Lazy<TDelegate>(() =>
				CreateDelegate(typeof(TDelegate), target, memberName, firstArgument, bindingAttr, throwOnBindFailure) as TDelegate);
		}
		/// <summary>
		/// 使用指定的名称和第一个参数，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type target, string memberName, object firstArgument)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), target, memberName, firstArgument,
				BinderDefault, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称、第一个参数和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type target, string memberName,
			object firstArgument, BindingFlags bindingAttr)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), target, memberName, firstArgument,
				bindingAttr, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称、第一个参数、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type target, string memberName,
			object firstArgument, BindingFlags bindingAttr, bool throwOnBindFailure)
			where TDelegate : class
		{
			return CreateDelegate(typeof(TDelegate), target, memberName, firstArgument,
				bindingAttr, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称和第一个参数，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, Type target, string memberName, object firstArgument)
		{
			return CreateDelegate(type, target, memberName, firstArgument, BinderDefault, true);
		}
		/// <summary>
		/// 使用指定的名称、第一个参数和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, Type target, string memberName, object firstArgument,
			BindingFlags bindingAttr)
		{
			return CreateDelegate(type, target, memberName, firstArgument, bindingAttr, true);
		}
		/// <summary>
		/// 使用指定的名称、第一个参数、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// 如果 <paramref name="firstArgument"/> 不为 <c>null</c>，则搜索实例成员，
		/// 并将 <paramref name="firstArgument"/> 作为实例。如果为 <c>null</c>，则搜索静态成员。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的 <see cref="System.Type"/>。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">委托要绑定到的对象，或为 <c>null</c>，
		/// 后者表示将 <paramref name="memberName"/> 视为 <c>static</c>。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="target"/> 是一个开放式泛型类型。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, Type target, string memberName, object firstArgument,
			BindingFlags bindingAttr, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(memberName, "memberName");
			CheckDelegateType(type, "type");
			CheckTargetType(target, "target");
			MethodInfo invoke = type.GetMethod("Invoke");
			ParameterInfo[] paramInfos = invoke.GetParameters();
			Type[] types = GetParameterTypes(paramInfos, 0, 0, 0);
			// 未设置 MemberMask 的情况下，查找所有成员。
			if ((bindingAttr & BinderMemberMask) == BindingFlags.Default)
			{
				bindingAttr |= BinderMemberMask;
			}
			BindingFlags instancecBindingAttr = (bindingAttr & ~BindingFlags.Static) | BindingFlags.Instance;
			Delegate dlg = null;
			if (memberName.Equals(ConstructorName, StringComparison.OrdinalIgnoreCase))
			{
				// 查找构造函数成员，此时注意只搜索实例方法。
				ConstructorInfo ctor = target.GetConstructor(instancecBindingAttr, PowerBinder.CastBinder, types, null);
				if (ctor != null)
				{
					dlg = CreateDelegate(type, ctor, false);
				}
			}
			else
			{
				// 查找其它成员。
				if (firstArgument == null)
				{
					bindingAttr = (bindingAttr & ~BindingFlags.Instance) | BindingFlags.Static;
				}
				else
				{
					bindingAttr = instancecBindingAttr;
				}
				// 查找方法成员。
				if ((bindingAttr & BindingFlags.InvokeMethod) == BindingFlags.InvokeMethod)
				{
					dlg = CreateMethodDelegate(type, target, memberName, firstArgument, bindingAttr, types);
				}
				// 查找属性成员。
				if (dlg == null && (bindingAttr & BinderGetSetProperty) != BindingFlags.Default)
				{
					dlg = CreatePropertyDelegate(type, target, memberName, firstArgument, bindingAttr, invoke.ReturnType, types);
				}
				// 查找字段成员。
				if (dlg == null && (bindingAttr & BinderGetSetField) != BindingFlags.Default)
				{
					FieldInfo field = target.GetField(memberName, bindingAttr);
					if (field != null)
					{
						if (firstArgument == null)
						{
							dlg = CreateDelegate(type, field, throwOnBindFailure);
						}
						else
						{
							dlg = CreateDelegate(type, field, firstArgument, throwOnBindFailure);
						}
					}
				}
			}
			if (dlg != null)
			{
				return dlg;
			}
			if (throwOnBindFailure)
			{
				throw ExceptionHelper.BindTargetMethod("memberName");
			}
			return null;
		}

		#endregion // 从 Type 构造带有第一个参数的成员委托

		#region 从 Object 构造成员委托

		/// <summary>
		/// 使用指定的实例和名称，创建用于表示实例成员的指定类型的委托。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的实例。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的委托，表示指定的实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(object target, string memberName)
			where TDelegate : class
		{
			ExceptionHelper.CheckArgumentNull(target, "target");
			return CreateDelegate(typeof(TDelegate), target.GetType(), memberName, target,
				BinderDefault, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的实例、名称和搜索方式，创建用于表示实例成员的指定类型的委托。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的实例。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(object target, string memberName, BindingFlags bindingAttr)
			where TDelegate : class
		{
			ExceptionHelper.CheckArgumentNull(target, "target");
			return CreateDelegate(typeof(TDelegate), target.GetType(), memberName, target,
				bindingAttr, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的实例、名称、搜索方式和针对绑定失败的指定行为，创建用于表示实例成员的指定类型的委托。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">表示实现成员的类的实例。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <typeparamref name="TDelegate"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(object target, string memberName,
			BindingFlags bindingAttr, bool throwOnBindFailure)
			where TDelegate : class
		{
			ExceptionHelper.CheckArgumentNull(target, "target");
			return CreateDelegate(typeof(TDelegate), target.GetType(), memberName, target,
				bindingAttr, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用指定的实例和名称创建用于表示实例成员的指定类型的委托。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的实例。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的委托，表示指定的实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, object target, string memberName)
		{
			ExceptionHelper.CheckArgumentNull(target, "target");
			return CreateDelegate(type, target.GetType(), memberName, target, BinderDefault, true);
		}
		/// <summary>
		/// 使用指定的实例、名称和搜索方式，创建用于表示实例成员的指定类型的委托。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的实例。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, object target, string memberName, BindingFlags bindingAttr)
		{
			ExceptionHelper.CheckArgumentNull(target, "target");
			return CreateDelegate(type, target.GetType(), memberName, target, bindingAttr, true);
		}
		/// <summary>
		/// 使用指定的实例、名称、搜索方式和针对绑定失败的指定行为，创建用于表示实例成员的指定类型的委托。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 按照方法、属性、字段的顺序查找匹配的成员。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不相同。
		/// 可以通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 允许绑定到方法的默认参数和带变量参数。
		/// 可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="target">表示实现成员的类的实例。</param>
		/// <param name="memberName">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 
		/// <see cref="System.Reflection.BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="memberName"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的实例成员。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="memberName"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="type"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentException">无法绑定 <paramref name="memberName"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="System.MissingMethodException">未找到 <paramref name="type"/>
		/// 的 <c>Invoke</c> 方法。</exception>
		/// <exception cref="System.MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, object target, string memberName,
			BindingFlags bindingAttr, bool throwOnBindFailure)
		{
			ExceptionHelper.CheckArgumentNull(target, "target");
			return CreateDelegate(type, target.GetType(), memberName, target, bindingAttr, throwOnBindFailure);
		}

		#endregion // 从 Object 构造成员委托

		#region 参数检查

		/// <summary>
		/// 检查委托的类型是否合法。
		/// </summary>
		/// <param name="type">委托的类型。</param>
		/// <param name="paramName">参数的名称。</param>
		private static void CheckDelegateType(Type type, string paramName)
		{
			ExceptionHelper.CheckArgumentNull(type, paramName);
			Type baseType = type.BaseType;
			if (baseType == null || baseType != typeof(MulticastDelegate))
			{
				throw ExceptionHelper.MustBeDelegate(paramName);
			}
		}
		/// <summary>
		/// 检查目标类型是否合法。
		/// </summary>
		/// <param name="target">目标的类型。</param>
		/// <param name="paramName">参数的名称。</param>
		private static void CheckTargetType(Type target, string paramName)
		{
			ExceptionHelper.CheckArgumentNull(target, paramName);
			if (target.IsGenericType && target.ContainsGenericParameters)
			{
				throw ExceptionHelper.UnboundGenParam(paramName);
			}
		}

		#endregion // 参数检查

		#region Expression 辅助方法

		/// <summary>
		/// 返回检查参数数组长度的表达式。认为 count 大于 0。
		/// </summary>
		/// <param name="paramExp">要检查长度的参数数组的表达式。</param>
		/// <param name="count">预期的参数数目。</param>
		/// <returns>检查参数数组长度的表达式。</returns>
		private static Expression GetCheckParameterExp(Expression paramExp, int count)
		{
			return Expression.IfThen(
				Expression.OrElse(
				// paramExp == null
					Expression.Equal(paramExp, Expression.Constant(null)),
				// paramExp.Length != count
					Expression.NotEqual(Expression.PropertyOrField(paramExp, "Length"), Expression.Constant(count))),
				Expression.Throw(Expression.New(typeof(TargetParameterCountException))));
		}
		/// <summary>
		/// 根据给定的参数信息创建参数表达式列表。
		/// </summary>
		/// <param name="paramInfos">参数信息。</param>
		/// <returns>参数表达式列表。</returns>
		private static ParameterExpression[] GetParameters(ParameterInfo[] paramInfos)
		{
			ParameterExpression[] paramList = new ParameterExpression[paramInfos.Length];
			for (int i = 0; i < paramInfos.Length; i++)
			{
				paramList[i] = Expression.Parameter(paramInfos[i].ParameterType);
			}
			return paramList;
		}
		/// <summary>
		/// 返回给定的参数信息的参数类型列表。
		/// </summary>
		/// <param name="paramInfos">参数信息。</param>
		/// <param name="sourceIndex">要获取参数类型的起始索引。</param>
		/// <param name="destinationIndex">要保存到的参数类型列表的起始索引。</param>
		/// <param name="extLen">参数类型列表的额外长度。</param>
		/// <returns>参数类型列表。</returns>
		private static Type[] GetParameterTypes(ParameterInfo[] paramInfos,
			int sourceIndex, int destinationIndex, int extLen)
		{
			int dif = sourceIndex - destinationIndex;
			Type[] types = new Type[paramInfos.Length - dif + extLen];
			for (int i = sourceIndex; i < paramInfos.Length; i++)
			{
				types[i - dif] = paramInfos[i].ParameterType;
			}
			return types;
		}
		/// <summary>
		/// 根据给定的参数信息创建引用参数表达式列表。如果不能进行强制类型转换，则为 <c>null</c>。
		/// </summary>
		/// <param name="parameters">参数表达式列表。</param>
		/// <param name="invokeIndex">参数表达式列表的起始索引。</param>
		/// <param name="paramInfos">目标参数信息。</param>
		/// <param name="methodIndex">目标参数信息的起始索引。</param>
		/// <returns>得到的引用参数表达式列表。</returns>
		private static Expression[] GetParameterExpressions(ParameterExpression[] parameters,
			int invokeIndex, ParameterInfo[] paramInfos, int methodIndex)
		{
			Expression[] paramExps = new Expression[paramInfos.Length];
			for (int i = invokeIndex, j = methodIndex; j < paramExps.Length; i++, j++)
			{
				Expression exp = ConvertType(parameters[i], paramInfos[j].ParameterType);
				if (exp == null)
				{
					// 不能进行强制类型转换。
					return null;
				}
				paramExps[j] = exp;
			}
			return paramExps;
		}
		/// <summary>
		/// 返回对指定表达式到目标类型的强制类型转换的表达式。如果不能进行强制类型转换，则为 <c>null</c>。
		/// </summary>
		/// <param name="exp">要引用的表达式。</param>
		/// <param name="expType">要强制类型转换到的目标类型。</param>
		/// <returns>对指定表达式到目标类型的强制类型转换的表达式。</returns>
		private static Expression ConvertType(Expression exp, Type expType)
		{
			// 对于可隐式类型转换和 ref 参数，不进行类型转换。
			if (exp.Type == expType || expType.IsAssignableFrom(exp.Type) || expType.IsByRef)
			{
				return exp;
			}
			try
			{
				// 需要强制转换。
				return Expression.Convert(exp, expType);
			}
			catch (InvalidOperationException)
			{
				// 不允许进行强制类型转换。
				return null;
			}
		}
		/// <summary>
		/// 返回对指定返回值到目标类型的强制类型转换的表达式。
		/// </summary>
		/// <param name="returnExp">返回值定义表达式。</param>
		/// <param name="returnType">要强制类型转换到的目标类型。</param>
		/// <returns>指定返回值到目标类型的强制类型转换的表达式。</returns>
		private static Expression GetReturn(Expression returnExp, Type returnType)
		{
			if (returnType == typeof(void) || returnType.IsAssignableFrom(returnExp.Type))
			{
				return returnExp;
			}
			if (returnExp.Type == typeof(void))
			{
				// 添加默认返回值。
				return Expression.Block(returnExp, Expression.Default(returnType));
			}
			try
			{
				// 需要强制转换。
				return Expression.Convert(returnExp, returnType);
			}
			catch (InvalidOperationException)
			{
				// 不允许进行强制类型转换。
				return null;
			}
		}

		#endregion // Expression 辅助方法

	}
}