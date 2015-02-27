using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb
{
	public static partial class DelegateBuilder
	{

		#region 构造开放方法委托

		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于表示指定静态或实例方法、字段或属性的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static TDelegate CreateDelegate<TDelegate>(this MethodBase method)
			where TDelegate : class
		{
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
			{
				throw CommonExceptions.MustBeDelegate("TDelegate", typeof(TDelegate));
			}
			Type type = typeof(TDelegate);
			Delegate dlg = CreateOpenDelegate(type, type.GetInvokeMethod(), method);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this MethodBase method, bool throwOnBindFailure)
			where TDelegate : class
		{
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			Contract.EndContractBlock();
			if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
			{
				throw CommonExceptions.MustBeDelegate("TDelegate", typeof(TDelegate));
			}
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Type type = typeof(TDelegate);
			Delegate dlg = CreateOpenDelegate(type, type.GetInvokeMethod(), method);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(Type type, MethodBase method)
		{
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			Contract.Ensures(Contract.Result<Delegate>() != null);
			if (!type.IsSubclassOf(typeof(Delegate)))
			{
				throw CommonExceptions.MustBeDelegate("type", type);
			}
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateOpenDelegate(type, type.GetInvokeMethod(), method);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// 如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(Type type, MethodBase method, bool throwOnBindFailure)
		{
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			Contract.EndContractBlock();
			if (!type.IsSubclassOf(typeof(Delegate)))
			{
				throw CommonExceptions.MustBeDelegate("type", type);
			}
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateOpenDelegate(type, type.GetInvokeMethod(), method);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例方法的指定类型的开放方法委托。
		/// 如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。方法不能是包含泛型参数的非泛型定义。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="invoke">委托的 invoke 方法。</param>
		/// <param name="method">要调用的方法。</param>
		/// <returns><paramref name="type"/> 类型的委托，表示静态或实例方法的委托。</returns>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		private static Delegate CreateOpenDelegate(Type type, MethodInfo invoke, MethodBase method)
		{
			Contract.Requires(type != null && invoke != null && method != null);
			Type[] invokeParamTypes = invoke.GetParameterTypes();
			Type[] types = invokeParamTypes;
			MethodArgumentsOption options = MethodArgumentsOption.OptionalParamBinding | MethodArgumentsOption.Explicit;
			int index = 0;
			if (!method.IsStatic && !(method is ConstructorInfo))
			{
				options |= MethodArgumentsOption.ContainsInstance;
				index++;
			}
			// 提取方法参数信息。
			int paramLen = method.GetParametersNoCopy().Length;
			if (invokeParamTypes.Length < paramLen + index)
			{
				types = new Type[paramLen + index];
				invokeParamTypes.CopyTo(types, 0);
			}
			MethodArgumentsInfo argumentsInfo;
			if (method.IsGenericMethodDefinition)
			{
				// 对泛型方法定义进行类型推断。
				argumentsInfo = method.GenericArgumentsInferences(invoke.ReturnType, types, options);
				if (argumentsInfo == null)
				{
					return null;
				}
				method = ((MethodInfo)method).MakeGenericMethod(argumentsInfo.GenericArguments);
			}
			else
			{
				// 调用时保证 !method.ContainsGenericParameters。
				argumentsInfo = MethodArgumentsInfo.GetInfo(method, types, options);
				if (argumentsInfo == null)
				{
					return null;
				}
			}
			// 构造动态委托。
			DynamicMethod dlgMethod = new DynamicMethod("MethodDelegate", invoke.ReturnType, invokeParamTypes,
				method.Module, true);
			ILGenerator il = dlgMethod.GetILGenerator();
			Contract.Assume(il != null);
			// 实例方法的第一个参数用作传递实例对象。
			if (argumentsInfo.InstanceType != null)
			{
				EmitLoadInstance(il, method, argumentsInfo.InstanceType);
			}
			// 加载方法参数。
			bool optimizeTailcall = EmitLoadParameters(il, method, argumentsInfo, index);
			// 调用方法。
			Type[] optionalArgumentTypes = null;
			if (argumentsInfo.OptionalArgumentTypes != null)
			{
				optionalArgumentTypes = argumentsInfo.OptionalArgumentTypes.ToArray();
			}
			if (EmitInvokeMethod(il, method, optionalArgumentTypes, invoke.ReturnType, optimizeTailcall))
			{
				return dlgMethod.CreateDelegate(type);
			}
			return null;
		}
		/// <summary>
		/// 加载第一个参数作为实例，调用保证类型可以转换。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="instanceType">实例实参的类型。</param>
		private static void EmitLoadInstance(ILGenerator il, MethodBase method, Type instanceType)
		{
			Contract.Requires(il != null && method != null && instanceType != null);
			il.EmitCheckArgumentNull(0, "instance");
			il.Emit(OpCodes.Ldarg_0);
			Type declType = method.DeclaringType;
			Contract.Assume(declType != null);
			il.EmitConversion(instanceType, declType, true, ConversionType.Explicit);
			if (declType.IsValueType)
			{
				// 值类型要转换为相应的指针。
				il.EmitGetAddress(declType);
			}
		}
		/// <summary>
		/// 加载方法参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要加载参数的方法。</param>
		/// <param name="argumentsInfo">方法实参信息。</param>
		/// <param name="index">实参的起始索引。</param>
		/// <returns>能否对方法进行 tailcall 优化，如果可以则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private static bool EmitLoadParameters(ILGenerator il, MethodBase method, MethodArgumentsInfo argumentsInfo,
			int index)
		{
			Contract.Requires(il != null && method != null && argumentsInfo != null && index >= 0);
			bool optimizeTailcall = true;
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			IList<Type> args = argumentsInfo.FixedArguments;
			int argCnt = args.Count;
			for (int i = 0; i < argCnt; i++, index++)
			{
				Type type = args[i];
				if (type == null)
				{
					ParameterInfo param = parameters[i];
					if (param.IsParamArray())
					{
						// params 参数。
						il.EmitConstant(0);
						il.Emit(OpCodes.Newarr, param.ParameterType.GetElementType());
					}
					else
					{
						// MethodArgumentsInfo 保证包含默认值。
						il.EmitConstant(param.DefaultValue);
					}
				}
				else
				{
					Type paramType = parameters[i].ParameterType;
					if (paramType.IsByRef)
					{
						if (type.IsByRef)
						{
							il.EmitLoadArg(index);
						}
						else
						{
							il.EmitLoadArgAddress(index);
							optimizeTailcall = false;
						}
					}
					else
					{
						EmitLoadParameter(il, paramType, index, type);
					}
				}
			}
			if ((args = argumentsInfo.ParamArgumentTypes) != null)
			{
				// 加载 params 参数。
				argCnt = args.Count;
				Type elementType = argumentsInfo.ParamArrayType.GetElementType();
				Contract.Assume(elementType != null);
				il.EmitConstant(argCnt);
				il.Emit(OpCodes.Newarr, elementType);
				LocalBuilder local = il.GetLocal(argumentsInfo.ParamArrayType);
				il.Emit(OpCodes.Stloc, local);
				for (int i = 0; i < argCnt; i++, index++)
				{
					il.Emit(OpCodes.Ldloc, local);
					il.EmitConstant(i);
					EmitLoadParameter(il, elementType, index, args[i]);
					il.EmitStoreElement(elementType);
				}
				il.Emit(OpCodes.Ldloc, local);
				il.FreeLocal(local);
				optimizeTailcall = false;
			}
			else if ((args = argumentsInfo.OptionalArgumentTypes) != null)
			{
				// 加载可变参数。
				argCnt = args.Count;
				for (int i = 0; i < argCnt; i++, index++)
				{
					EmitLoadParameter(il, args[i], index, args[i]);
				}
			}
			return optimizeTailcall;
		}
		/// <summary>
		/// 加载方法参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="paramType">方法形参的类型。</param>
		/// <param name="index">要加载的实参索引。</param>
		/// <param name="argumentType">要加载的实参类型。</param>
		private static void EmitLoadParameter(ILGenerator il, Type paramType, int index, Type argumentType)
		{
			Contract.Requires(il != null && paramType != null && index >= 0 && argumentType != null);
			if (argumentType.IsByRef)
			{
				il.EmitLoadArg(index);
				il.EmitLoadIndirect(argumentType.GetElementType());
			}
			else
			{
				il.EmitLoadArg(index, argumentType, paramType, true);
			}
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
		private static bool EmitInvokeMethod(ILGenerator il, MethodBase method, Type[] optionalArgumentTypes,
			Type returnType, bool optimizeTailcall)
		{
			Contract.Requires(il != null && method != null && returnType != null);
			MethodInfo methodInfo = method as MethodInfo;
			if (methodInfo == null)
			{
				// 调用构造函数。
				Converter converter = il.GetConversion(method.DeclaringType, returnType, ConversionType.Explicit);
				if (converter == null)
				{
					return false;
				}
				il.Emit(OpCodes.Newobj, method);
				converter.Emit(true);
			}
			else if (returnType == typeof(void))
			{
				if (methodInfo.ReturnType == typeof(void))
				{
					// 无返回值。
					il.EmitCall(method.DeclaringType, methodInfo, optimizeTailcall, optionalArgumentTypes);
				}
				else
				{
					// 忽略返回值。
					il.EmitCall(method.DeclaringType, methodInfo, optionalArgumentTypes);
					il.Emit(OpCodes.Pop);
				}
			}
			else if (methodInfo.ReturnType == typeof(void))
			{
				// 返回默认值。
				il.EmitCall(method.DeclaringType, methodInfo, optionalArgumentTypes);
				il.EmitConstant(null, returnType);
			}
			else
			{
				// 对返回值进行类型转换。
				Converter converter = il.GetConversion(methodInfo.ReturnType, returnType, ConversionType.Explicit);
				if (converter == null)
				{
					return false;
				}
				il.EmitCall(method.DeclaringType, methodInfo, optimizeTailcall && !converter.NeedEmit, optionalArgumentTypes);
				converter.Emit(true);
			}
			il.Emit(OpCodes.Ret);
			return true;
		}

		#endregion // 构造开放方法委托

		#region 构造封闭方法委托

		/// <summary>
		/// 使用指定的第一个参数创建表示指定的静态或实例方法的指定类型的委托。 
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
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
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
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
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
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
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
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
			CommonExceptions.CheckArgumentNull(method, "method");
			CommonExceptions.CheckDelegateType(type, "type");
			MethodInfo invoke = type.GetMethod("Invoke");
			ParameterInfo[] invokeParams = invoke.GetParameters();
			ParameterInfo[] methodParams = method.GetParameters();
			// 尝试创建带有第一个参数的方法委托。
			Delegate dlg = CreateDelegateWithArgument(type, firstArgument, invoke, invokeParams, method, methodParams);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例方法的指定类型的带有第一个参数的方法委托。
		/// 如果是实例方法，需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
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
			Delegate dlg = CreateOpenDelegate(type, invoke, method);
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
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
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
				method = method.MakeGenericMethodFromParamTypes(paramTypes, MethodArgumentsOption.None);
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
					ParameterExpression[] paramList = invokeParams.ToExpressions();
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
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
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
					method = method.MakeGenericMethodFromParamTypes(paramTypes, MethodArgumentsOption.None);
					if (method == null) { return null; }
					methodParams = method.GetParameters();
				}
				// 方法的参数列表。
				ParameterExpression[] paramList = invokeParams.ToExpressions();
				// 构造调用参数列表。
				Expression[] paramExps = GetParameterExpressions(paramList, 0, methodParams, skipIdx);
				if (paramExps != null)
				{
					Expression instance = null;
					if (skipIdx == 1)
					{
						paramExps[0] = Expression.Constant(firstArgument).ConvertType(methodParams[0].ParameterType);
						if (paramExps[0] == null)
						{
							// 不允许进行强制类型转换。
							return null;
						}
					}
					else
					{
						instance = Expression.Constant(firstArgument).ConvertType(method.DeclaringType);
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

		#endregion // 构造封闭方法委托

	}
}
