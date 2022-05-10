using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Cyjb.Conversions;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供提交 IL 指令的扩展方法。
	/// </summary>
	public static partial class ILGeneratorUtil
	{

		#region ILManager

		/// <summary>
		/// IL 管理器的实例。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConditionalWeakTable<ILGenerator, ILManager> managers = new();

		/// <summary>
		/// 获取与指定 IL 指令生成器相关的管理器。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <returns>IL 指令管理器。</returns>
		internal static ILManager GetManager(this ILGenerator il)
		{
			return managers.GetValue(il, key => new ILManager(key));
		}

		/// <summary>
		/// 返回可用的指定类型的局部变量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="type">局部变量的类型。</param>
		/// <returns>可用的局部变量。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		public static LocalBuilder GetLocal(this ILGenerator il, Type type)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(type);
			return il.GetManager().GetLocal(type);
		}

		/// <summary>
		/// 释放指定的局部变量，该局部变量之后可以被重复利用。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="local">要释放的局部变量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="local"/> 为 <c>null</c>。</exception>
		public static void FreeLocal(this ILGenerator il, LocalBuilder local)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(local);
			il.GetManager().FreeLocal(local);
		}

		/// <summary>
		/// 返回指定闭包值的索引。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要使用的闭包值。</param>
		/// <returns>指定闭包值的索引。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static int GetClosure(this ILGenerator il, object value)
		{
			ArgumentNullException.ThrowIfNull(il);
			return il.GetManager().GetClosure(value);
		}

		/// <summary>
		/// 返回当前的闭包。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <returns>回当前的闭包，如果不需要则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static object? GetClosure(this ILGenerator il)
		{
			ArgumentNullException.ThrowIfNull(il);
			return il.GetManager().GetClosure();
		}

		#endregion // ILManager

		#region 类型转换

		/// <summary>
		/// 将栈顶的对象从 <paramref name="inputType"/> 转换为 <paramref name="outputType"/>。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="inputType"/> 包含泛型参数。</exception>
		/// <exception cref="ArgumentException"><paramref name="outputType"/> 包含泛型参数。</exception>
		public static void EmitConversion(this ILGenerator il, Type inputType, Type outputType, bool isChecked)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(inputType);
			ArgumentNullException.ThrowIfNull(outputType);
			if (inputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(inputType);
			}
			if (outputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(outputType);
			}
			Conversion? conversion = ConversionFactory.GetConversion(inputType, outputType);
			if (conversion == null)
			{
				throw CommonExceptions.InvalidCast(inputType, outputType);
			}
			if (conversion.PassByAddr)
			{
				il.EmitGetAddress(inputType);
			}
			conversion.Emit(il, inputType, outputType, isChecked);
		}

		/// <summary>
		/// 获取转换类型的指令生成器，能够将栈顶的对象从 <paramref name="inputType"/> 转换为 
		/// <paramref name="outputType"/>。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>类型转换的指令生成器，如果不能进行类型转换则返回 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="inputType"/> 包含泛型参数。</exception>
		/// <exception cref="ArgumentException"><paramref name="outputType"/> 包含泛型参数。</exception>
		public static Converter? GetConversion(this ILGenerator il, Type inputType, Type outputType)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(inputType);
			ArgumentNullException.ThrowIfNull(outputType);
			if (inputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(inputType);
			}
			if (outputType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(outputType);
			}
			Conversion? conversion = ConversionFactory.GetConversion(inputType, outputType);
			if (conversion == null)
			{
				return null;
			}
			return new Converter(conversion, il, inputType, outputType);
		}

		#endregion // 类型转换

		#region 方法

		/// <summary>
		/// 将指定的方法或构造函数的指定指令和元数据标记放到 Microsoft 中间语言 (MSIL) 指令流上。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="opCode">要发到流中的 MSIL 指令。</param>
		/// <param name="method">方法或构造函数。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void Emit(this ILGenerator il, OpCode opCode, MethodBase method)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			if (method.MemberType == MemberTypes.Constructor)
			{
				il.Emit(opCode, (ConstructorInfo)method);
			}
			else
			{
				il.Emit(opCode, (MethodInfo)method);
			}
		}

		/// <summary>
		/// 调用指定的方法（call）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 调用指定的方法。
		/// </summary>
		/// </overloads>
		public static void EmitCall(this ILGenerator il, MethodInfo method)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(method, false, Type.EmptyTypes);
		}

		/// <summary>
		/// 调用指定的方法（call）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="tailCall">是否执行后缀的方法调用。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, MethodInfo method, bool tailCall)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(method, tailCall, Type.EmptyTypes);
		}

		/// <summary>
		/// 调用指定的方法（call）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="parameterTypes">参数的类型；如果 <paramref name="method"/> 不是可变参数方法，
		/// 则忽略该参数。</param>>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, MethodInfo method, params Type[] parameterTypes)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(method, false, parameterTypes);
		}

		/// <summary>
		/// 调用指定的方法（call）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="tailCall">是否执行后缀的方法调用。</param>
		/// <param name="parameterTypes">参数的类型；如果 <paramref name="method"/> 不是可变参数方法，
		/// 则忽略该参数。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, MethodInfo method, bool tailCall,
			params Type[] parameterTypes)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(method, tailCall, parameterTypes);
		}

		/// <summary>
		/// 调用指定的方法（call）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="tailCall">是否执行后缀的方法调用。</param>
		/// <param name="parameterTypes">参数的类型；如果 <paramref name="method"/> 不是可变参数方法，
		/// 则忽略该参数。</param>
		private static void EmitCallInternal(this ILGenerator il, MethodInfo method, bool tailCall,
			params Type[] parameterTypes)
		{
			method = CheckDynamicMethod(method);
			if (tailCall)
			{
				il.Emit(OpCodes.Tailcall);
			}
			if (method.CallingConvention == CallingConventions.VarArgs)
			{
				il.EmitCall(OpCodes.Call, method, parameterTypes);
			}
			else
			{
				il.Emit(OpCodes.Call, method);
			}
		}

		/// <summary>
		/// 调用指定的方法（自动决定 call/callvirt）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="instanceType">要调用方法的实例类型；如果 <paramref name="method"/> 是静态方法，
		/// 则忽略该参数。</param>
		/// <param name="method">要调用的方法。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, Type instanceType, MethodInfo method)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(instanceType, method, false, Type.EmptyTypes);
		}

		/// <summary>
		/// 调用指定的方法（自动决定 call/callvirt）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="instanceType">要调用方法的实例类型；如果 <paramref name="method"/> 是静态方法，
		/// 则忽略该参数。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="tailCall">是否执行后缀的方法调用。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, Type instanceType, MethodInfo method, bool tailCall)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(instanceType, method, tailCall, Type.EmptyTypes);
		}

		/// <summary>
		/// 调用指定的方法（自动决定 call/callvirt）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="instanceType">要调用方法的实例类型；如果 <paramref name="method"/> 是静态方法，
		/// 则忽略该参数。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="parameterTypes">参数的类型；如果 <paramref name="method"/> 不是可变参数方法，
		/// 则忽略该参数。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, Type instanceType, MethodInfo method,
			params Type[]? parameterTypes)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(instanceType, method, false, parameterTypes);
		}

		/// <summary>
		/// 调用指定的方法（自动决定 call/callvirt）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="instanceType">要调用方法的实例类型；如果 <paramref name="method"/> 是静态方法，
		/// 则忽略该参数。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="tailCall">是否执行后缀的方法调用。</param>
		/// <param name="parameterTypes">参数的类型；如果 <paramref name="method"/> 不是可变参数方法，
		/// 则忽略该参数。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, Type instanceType, MethodInfo method, bool tailCall,
			params Type[]? parameterTypes)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(method);
			il.EmitCallInternal(instanceType, method, tailCall, parameterTypes);
		}

		/// <summary>
		/// 调用指定的方法（自动决定 call/callvirt）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="instanceType">要调用方法的实例类型；如果 <paramref name="method"/> 是静态方法，
		/// 则忽略该参数。</param>
		/// <param name="method">要调用的方法。</param>
		/// <param name="tailCall">是否执行后缀的方法调用。</param>
		/// <param name="parameterTypes">参数的类型；如果 <paramref name="method"/> 不是可变参数方法，
		/// 则忽略该参数。</param>
		private static void EmitCallInternal(this ILGenerator il, Type instanceType, MethodInfo method, bool tailCall,
			params Type[]? parameterTypes)
		{
			method = CheckDynamicMethod(method);
			OpCode callOp;
			if (UseVirtual(method))
			{
				callOp = OpCodes.Callvirt;
				if (instanceType != null && instanceType.IsValueType)
				{
					il.Emit(OpCodes.Constrained, instanceType);
				}
			}
			else
			{
				callOp = OpCodes.Call;
			}
			if (tailCall)
			{
				il.Emit(OpCodes.Tailcall);
			}
			if (method.CallingConvention.HasFlag(CallingConventions.VarArgs))
			{
				il.EmitCall(callOp, method, parameterTypes);
			}
			else
			{
				il.Emit(callOp, method);
			}
		}

		/// <summary>
		/// 获取指定方法是否需要使用 <c>callvirt</c> 指令调用。
		/// </summary>
		/// <param name="method">要判断的方法。</param>
		/// <returns>如果 <paramref name="method"/> 需要使用 <c>callvirt</c> 指令调用，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		private static bool UseVirtual(MethodInfo method)
		{
			if (method.IsStatic)
			{
				return false;
			}
			// 非静态方法，应当总是存在 DeclaringType。
			Type declType = method.DeclaringType!;
			if (declType.IsValueType)
			{
				// 值类型上声明的方法。
				return false;
			}
			if (!method.IsVirtual || method.IsFinal)
			{
				// 不可重写的方法。
				return false;
			}
			return true;
		}

		/// <summary>
		/// 表示 <c>System.Reflection.Emit.DynamicMethod+RTDynamicMethod</c> 类。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Type? rtDynamicMethodType = Type.GetType("System.Reflection.Emit.DynamicMethod+RTDynamicMethod");
		/// <summary>
		/// 获取 <c>System.Reflection.Emit.DynamicMethod+RTDynamicMethod</c> 类对应的 <see cref="DynamicMethod"/> 的方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Func<MethodInfo, DynamicMethod>? getDynamicMethod;

		/// <summary>
		/// 检查方法是否是 <see cref="DynamicMethod"/>。
		/// </summary>
		/// <param name="method">要调用的方法。</param>
		/// <returns>如果是 <see cref="DynamicMethod"/>，则为转换后的方法。</returns>
		private static MethodInfo CheckDynamicMethod(MethodInfo method)
		{
			if (rtDynamicMethodType != null && rtDynamicMethodType.IsInstanceOfType(method))
			{
				// RTDynamicMethod 不能直接调用，需要取得相应的 DynamicMethod 才可以。
				if (getDynamicMethod == null)
				{
					Type[] types = { typeof(MethodInfo) };
					FieldInfo field = rtDynamicMethodType.GetField("m_owner", BindingFlagsUtil.Instance)!;
					DynamicMethod dlgMethod = new("RTDynamicMethodOwnerDelegate", typeof(DynamicMethod),
						types, field.Module, true);
					ILGenerator il = dlgMethod.GetILGenerator();
					il.EmitLoadInstance(field, types[0], true);
					il.EmitLoadField(field);
					il.Emit(OpCodes.Ret);
					getDynamicMethod = (Func<MethodInfo, DynamicMethod>)dlgMethod.CreateDelegate(typeof(Func<MethodInfo, DynamicMethod>));
				}
				return getDynamicMethod(method);
			}
			return method;
		}

		#endregion // 方法

		/// <summary>
		/// 将栈顶的值转换为相应的地址。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="valueType">栈顶值的类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="valueType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="valueType"/> 包含泛型参数。</exception>
		public static void EmitGetAddress(this ILGenerator il, Type valueType)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(valueType);
			LocalBuilder locFrom = il.DeclareLocal(valueType);
			il.Emit(OpCodes.Stloc, locFrom);
			il.Emit(OpCodes.Ldloca, locFrom);
		}

		/// <summary>
		/// 使用指定类型的无参数构造函数创建实例。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="type">要创建实例的类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 包含泛型参数。</exception>
		public static void EmitNew(this ILGenerator il, Type type)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(type);
			if (type.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(type);
			}
			if (type.IsValueType)
			{
				// 值类型使用 initobj 指令。
				ILManager manager = il.GetManager();
				LocalBuilder local = manager.GetLocal(type);
				il.Emit(OpCodes.Ldloca, local);
				il.Emit(OpCodes.Initobj, type);
				il.Emit(OpCodes.Ldloc, local);
				manager.FreeLocal(local);
			}
			else
			{
				// 引用类型调用构造函数。
				ConstructorInfo? ctor = type.GetConstructor(BindingFlagsUtil.Instance, null, Type.EmptyTypes, null);
				if (ctor == null)
				{
					throw ReflectionExceptions.TypeMissingDefaultConstructor(type);
				}
				il.Emit(OpCodes.Newobj, ctor);
			}
		}
	}
}
