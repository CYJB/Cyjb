using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
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
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateOpenDelegate(method, type);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this MethodBase method, bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateOpenDelegate(method, type);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(this MethodBase method, Type delegateType)
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateOpenDelegate(method, delegateType);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		public static Delegate CreateDelegate(this MethodBase method, Type delegateType, bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.EndContractBlock();
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateOpenDelegate(method, delegateType);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例方法的指定类型的开放方法委托。
		/// </summary>
		/// <param name="method">要调用的方法。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns><paramref name="delegateType"/> 类型的委托，表示静态或实例方法的委托。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。方法不能是包含泛型参数的非泛型定义。</remarks>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		private static Delegate CreateOpenDelegate(MethodBase method, Type delegateType)
		{
			Contract.Requires(method != null && delegateType != null);
			// 判断是否需要作为实例的形参。
			int index = 0;
			if (!method.IsStatic && !(method is ConstructorInfo))
			{
				index++;
			}
			MethodInfo invoke = delegateType.GetInvokeMethod();
			Type[] paramTypes = invoke.GetParameterTypes();
			Type[] types = paramTypes.Extend(method.GetParametersNoCopy().Length + index, typeof(Missing));
			Type returnType = invoke.ReturnType;
			// 提取方法参数信息。
			MethodArgumentsInfo argumentsInfo = GetArgumentsInfo(ref method, types, returnType);
			if (argumentsInfo == null)
			{
				return null;
			}
			// 构造动态委托。
			DynamicMethod dlgMethod = new DynamicMethod("MethodDelegate", returnType, paramTypes,
				method.Module, true);
			ILGenerator il = dlgMethod.GetILGenerator();
			Contract.Assume(il != null);
			// 实例方法的第一个参数用作传递实例对象。
			if (argumentsInfo.InstanceType != null)
			{
				il.EmitLoadInstance(method, argumentsInfo.InstanceType, true);
			}
			// 加载方法参数。
			bool optimizeTailcall = il.EmitLoadParameters(method, argumentsInfo, index);
			// 调用方法。
			Type[] optionalArgumentTypes = null;
			if (argumentsInfo.OptionalArgumentTypes != null)
			{
				optionalArgumentTypes = argumentsInfo.OptionalArgumentTypes.ToArray();
			}
			if (!il.EmitInvokeMethod(method, optionalArgumentTypes, returnType, optimizeTailcall))
			{
				return null;
			}
			return dlgMethod.CreateDelegate(delegateType);
		}
		/// <summary>
		/// 获取指定方法的参数信息。
		/// </summary>
		/// <param name="method">要获取参数信息的方法。</param>
		/// <param name="types">方法的实参类型列表。</param>
		/// <param name="returnType">方法的返回值类型。</param>
		/// <returns>方法的参数信息。</returns>
		private static MethodArgumentsInfo GetArgumentsInfo(ref MethodBase method, Type[] types, Type returnType)
		{
			MethodArgumentsOption options = MethodArgumentsOption.OptionalAndExplicit | MethodArgumentsOption.ConvertRefType;
			if (!method.IsStatic && !(method is ConstructorInfo))
			{
				options |= MethodArgumentsOption.ContainsInstance;
			}
			if (method.IsGenericMethodDefinition)
			{
				// 对泛型方法定义进行类型推断。
				MethodArgumentsInfo argumentsInfo = method.GenericArgumentsInferences(returnType, types, options);
				if (argumentsInfo == null)
				{
					return null;
				}
				method = ((MethodInfo)method).MakeGenericMethod(argumentsInfo.GenericArguments);
				argumentsInfo.UpdateParamArrayType(method);
				return argumentsInfo;
			}
			// 调用时保证 !member.ContainsGenericParameters。
			return MethodArgumentsInfo.GetInfo(method, types, options);
		}
		/// <summary>
		/// 加载第一个参数作为实例，调用保证类型可以转换。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="member">要为实例调用的成员。</param>
		/// <param name="instanceType">实例实参的类型。</param>
		/// <param name="isCheckNull">是否检查实例是否为 <c>null</c>。</param>
		private static void EmitLoadInstance(this ILGenerator il, MemberInfo member, Type instanceType, bool isCheckNull)
		{
			Contract.Requires(il != null && member != null && instanceType != null);
			if (isCheckNull)
			{
				il.EmitCheckArgumentNull(0, "instance");
			}
			il.Emit(OpCodes.Ldarg_0);
			Type declType = member.DeclaringType;
			Contract.Assume(declType != null);
			il.EmitConversion(instanceType, declType, true, ConversionType.Explicit);
			if (declType.IsValueType)
			{
				// 值类型要转换为相应的地址。
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
		private static bool EmitLoadParameters(this ILGenerator il, MethodBase method, MethodArgumentsInfo argumentsInfo,
			int index)
		{
			Contract.Requires(il != null && method != null && argumentsInfo != null && index >= 0);
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			IList<Type> args = argumentsInfo.FixedArguments;
			int argCnt = args.Count;
			bool optimizeTailcall = true;
			for (int i = 0; i < argCnt; i++, index++)
			{
				if (!il.EmitLoadParameter(parameters[i], index, args[i]))
				{
					optimizeTailcall = false;
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
					il.EmitLoadParameter(elementType, index, args[i]);
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
					il.EmitLoadParameter(args[i], index, args[i]);
				}
			}
			return optimizeTailcall;
		}
		/// <summary>
		/// 加载方法参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="parameter">方法形参信息。</param>
		/// <param name="index">要加载的实参索引。</param>
		/// <param name="argumentType">要加载的实参类型。</param>
		/// <returns>能否对方法进行 tailcall 优化，如果可以则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private static bool EmitLoadParameter(this ILGenerator il, ParameterInfo parameter, int index, Type argumentType)
		{
			Contract.Requires(il != null && parameter != null && index >= 0 && argumentType != null);
			if (argumentType != typeof(Missing))
			{
				return il.EmitLoadParameter(parameter.ParameterType, index, argumentType);
			}
			if (parameter.IsParamArray())
			{
				// params 参数。
				il.EmitConstant(0);
				il.Emit(OpCodes.Newarr, parameter.ParameterType.GetElementType());
			}
			else
			{
				// MethodArgumentsInfo 保证包含默认值。
				il.EmitConstant(parameter.DefaultValue);
			}
			return true;
		}
		/// <summary>
		/// 加载方法参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="paramType">方法形参的类型。</param>
		/// <param name="index">要加载的实参索引。</param>
		/// <param name="argumentType">要加载的实参类型。</param>
		/// <returns>能否对方法进行 tailcall 优化，如果可以则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private static bool EmitLoadParameter(this ILGenerator il, Type paramType, int index, Type argumentType)
		{
			Contract.Requires(il != null && paramType != null && index >= 0 && argumentType != null);
			if (paramType.IsByRef)
			{
				if (argumentType.IsByRef)
				{
					il.EmitLoadArg(index);
				}
				else
				{
					il.EmitLoadArgAddress(index);
					return false;
				}
			}
			else
			{
				il.EmitLoadArg(index);
				if (argumentType.IsByRef)
				{
					il.EmitLoadIndirect(argumentType.GetElementType());
				}
				else if (paramType != argumentType)
				{
					il.EmitConversion(argumentType, paramType, true, ConversionType.Explicit);
				}
			}
			return true;
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
		private static bool EmitInvokeMethod(this ILGenerator il, MethodBase method, Type[] optionalArgumentTypes,
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
		/// 使用指定的第一个参数，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <seealso cref="Delegate.CreateDelegate(Type, object, MethodInfo)"/>
		public static TDelegate CreateDelegate<TDelegate>(this MethodBase method, object firstArgument)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateClosedDelegate(method, type, firstArgument);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <seealso cref="Delegate.CreateDelegate(Type, object, MethodInfo, bool)"/>
		public static TDelegate CreateDelegate<TDelegate>(this MethodBase method, object firstArgument,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateClosedDelegate(method, type, firstArgument);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <seealso cref="Delegate.CreateDelegate(Type, object, MethodInfo)"/>
		public static Delegate CreateDelegate(this MethodBase method, Type delegateType, object firstArgument)
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateClosedDelegate(method, delegateType, firstArgument);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建用于表示指定静态或实例方法的指定类型的委托。
		/// </summary>
		/// <param name="method">描述委托要表示的静态或实例方法的 <see cref="MethodBase"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		///     否则将作为方法的第一个参数。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="method"/> 
		///     时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例方法。</returns>
		/// <remarks>支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="method"/>
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		/// <seealso cref="Delegate.CreateDelegate(Type, object, MethodInfo, bool)"/>
		public static Delegate CreateDelegate(this MethodBase method, Type delegateType, object firstArgument,
			bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(method, "method");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.EndContractBlock();
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(method, "method");
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				throw CommonExceptions.UnboundGenParam("method");
			}
			Delegate dlg = CreateClosedDelegate(method, delegateType, firstArgument);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetMethod("method");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例方法的指定类型的封闭方法委托。
		/// </summary>
		/// <param name="method">要调用的方法。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		///     否则将作为方法的第一个参数。</param>
		/// <returns><paramref name="delegateType"/> 类型的委托，表示静态或实例方法的委托。</returns>
		/// <remarks>支持参数的强制类型转换，参数声明可以与实际类型不同。方法不能是包含泛型参数的非泛型定义。</remarks>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="method"/>。</exception>
		private static Delegate CreateClosedDelegate(MethodBase method, Type delegateType, object firstArgument)
		{
			Contract.Requires(method != null && delegateType != null);
			if (firstArgument == null)
			{
				// 开放方法。
				Delegate dlg = CreateOpenDelegate(method, delegateType);
				if (dlg != null)
				{
					return dlg;
				}
			}
			// 封闭方法。
			MethodInfo invoke = delegateType.GetInvokeMethod();
			Type[] paramTypes = invoke.GetParameterTypes();
			Type[] paramTypesWithFirstArg = paramTypes.Insert(0,
				// 这里使用 firstArgument 的实际类型，因为需要做类型检查和泛型类型推断。
				firstArgument == null ? null : firstArgument.GetType());
			// 判断是否需要作为实例的形参。
			int index = 0;
			if (!method.IsStatic && !(method is ConstructorInfo))
			{
				index++;
			}
			Type[] types = paramTypesWithFirstArg.Extend(method.GetParametersNoCopy().Length + index, typeof(Missing));
			Type returnType = invoke.ReturnType;
			MethodArgumentsInfo argumentsInfo = GetArgumentsInfo(ref method, types, returnType);
			if (argumentsInfo == null)
			{
				return null;
			}
			bool needLoadFirstArg = false;
			if (!ILExt.CanEmitConstant(firstArgument))
			{
				needLoadFirstArg = true;
				// 修正额外参数的类型。
				Type argType = GetFirstArgParamType(method);
				// 提前进行类型转换。
				if (firstArgument != null && firstArgument.GetType() != argType)
				{
					firstArgument = Convert.ChangeType(firstArgument, argType);
				}
				paramTypesWithFirstArg[0] = argType;
				paramTypes = paramTypesWithFirstArg;
			}
			// 构造动态委托。
			DynamicMethod dlgMethod = new DynamicMethod("MethodDelegate", returnType, paramTypes,
				method.Module, true);
			ILGenerator il = dlgMethod.GetILGenerator();
			Contract.Assume(il != null);
			bool optimizeTailcall;
			if (needLoadFirstArg)
			{
				if (argumentsInfo.InstanceType != null)
				{
					il.EmitLoadInstance(method, argumentsInfo.InstanceType, true);
				}
				optimizeTailcall = il.EmitLoadParameters(method, argumentsInfo, index);
			}
			else if (argumentsInfo.InstanceType != null)
			{
				il.EmitLoadInstance(method, firstArgument);
				optimizeTailcall = il.EmitLoadParameters(method, argumentsInfo, 0);
			}
			else
			{
				optimizeTailcall = il.EmitLoadParameters(method, argumentsInfo, firstArgument);
			}
			// 调用方法。
			Type[] optionalArgumentTypes = null;
			if (argumentsInfo.OptionalArgumentTypes != null)
			{
				optionalArgumentTypes = argumentsInfo.OptionalArgumentTypes.ToArray();
			}
			if (!il.EmitInvokeMethod(method, optionalArgumentTypes, returnType, optimizeTailcall))
			{
				return null;
			}
			return needLoadFirstArg ? dlgMethod.CreateDelegate(delegateType, firstArgument) : dlgMethod.CreateDelegate(delegateType);
		}
		/// <summary>
		/// 获取 firstArgument 对应的形参类型。
		/// </summary>
		/// <param name="method">方法信息。</param>
		/// <returns>firstArgument 对应的形参类型。</returns>
		private static Type GetFirstArgParamType(MethodBase method)
		{
			Contract.Requires(method != null);
			Type type;
			if (method.IsStatic || method is ConstructorInfo)
			{
				type = method.GetParametersNoCopy()[0].ParameterType;
			}
			else
			{
				type = method.DeclaringType;
			}
			Contract.Assume(type != null);
			if (type.IsValueType)
			{
				// CreateDelegate 方法传入的 firstArgument 不能为值类型，除非形参类型是 object。
				type = typeof(object);
			}
			return type;
		}
		/// <summary>
		/// 加载指定值作为实例，调用值可写入 IL 且保证类型可以转换。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="member">要调用的成员。</param>
		/// <param name="value">作为实例加载的值。</param>
		private static void EmitLoadInstance(this ILGenerator il, MemberInfo member, object value)
		{
			Contract.Requires(il != null && member != null);
			Type declType = member.DeclaringType;
			Contract.Assume(declType != null);
			if (value == null)
			{
				il.EmitConstant(null, declType);
				return;
			}
			// 提前完成类型转换。
			value = Convert.ChangeType(value, declType);
			il.EmitConstant(value);
			if (declType.IsValueType)
			{
				// 值类型要转换为相应的指针。
				il.EmitGetAddress(declType);
			}
			else
			{
				// 需要对值类型进行装箱。
				il.Emit(OpCodes.Box, value.GetType());
			}
		}
		/// <summary>
		/// 加载方法参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要加载参数的方法。</param>
		/// <param name="argumentsInfo">方法实参信息。</param>
		/// <param name="firstArgument">第一个参数的值。</param>
		/// <returns>能否对方法进行 tailcall 优化，如果可以则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private static bool EmitLoadParameters(this ILGenerator il, MethodBase method, MethodArgumentsInfo argumentsInfo,
			object firstArgument)
		{
			Contract.Requires(il != null && method != null && argumentsInfo != null);
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			IList<Type> args = argumentsInfo.FixedArguments;
			int argCnt = args.Count, index = 0;
			bool optimizeTailcall = true, firstArg = true;
			if (argCnt > 0)
			{
				firstArg = false;
				Type paramType = parameters[0].ParameterType;
				if (paramType.IsByRef)
				{
					il.EmitConstant(Convert.ChangeType(firstArgument, paramType));
					il.EmitGetAddress(paramType);
					optimizeTailcall = false;
				}
				else
				{
					il.EmitConstant(Convert.ChangeType(firstArgument, paramType));
				}
			}
			for (int i = 1; i < argCnt; i++, index++)
			{
				if (!il.EmitLoadParameter(parameters[i], index, args[i]))
				{
					optimizeTailcall = false;
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
				int i = 0;
				if (firstArg)
				{
					i++;
					il.EmitConstant(Convert.ChangeType(firstArgument, elementType));
				}
				for (; i < argCnt; i++, index++)
				{
					il.Emit(OpCodes.Ldloc, local);
					il.EmitConstant(i);
					il.EmitLoadParameter(elementType, index, args[i]);
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
				int i = 0;
				if (firstArg)
				{
					i++;
					il.EmitConstant(firstArgument);
				}
				for (; i < argCnt; i++, index++)
				{
					il.EmitLoadParameter(args[i], index, args[i]);
				}
			}
			return optimizeTailcall;
		}

		#endregion // 构造封闭方法委托

	}
}
