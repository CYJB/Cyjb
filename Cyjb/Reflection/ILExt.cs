using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Cyjb.Conversions;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供提交 IL 指令的扩展方法。
	/// </summary>
	public static partial class ILExt
	{

		#region ILManager

		/// <summary>
		/// IL 管理器的实例。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConditionalWeakTable<ILGenerator, ILManager> managers =
			new ConditionalWeakTable<ILGenerator, ILManager>();
		/// <summary>
		/// 获取与指定 IL 指令生成器相关的管理器。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <returns>IL 指令管理器。</returns>
		internal static ILManager GetManager(this ILGenerator il)
		{
			Contract.Requires(il != null);
			Contract.Ensures(Contract.Result<ILManager>() != null);
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
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			Contract.Ensures(Contract.Result<LocalBuilder>() != null);
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
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (local == null)
			{
				throw CommonExceptions.ArgumentNull("local");
			}
			Contract.EndContractBlock();
			il.GetManager().FreeLocal(local);
		}

		#endregion // ILManager

		#region 加载参数

		/// <summary>
		/// 加载指定索引的参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要加载的参数索引。</param>
		/// <remarks>会根据 <paramref name="index"/> 的值，选择最合适的 IL 指令。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		public static void EmitLoadArg(this ILGenerator il, int index)
		{
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			Contract.EndContractBlock();
			if (index == 0)
			{
				il.Emit(OpCodes.Ldarg_0);
			}
			else if (index == 1)
			{
				il.Emit(OpCodes.Ldarg_1);
			}
			else if (index == 2)
			{
				il.Emit(OpCodes.Ldarg_2);
			}
			else if (index == 3)
			{
				il.Emit(OpCodes.Ldarg_3);
			}
			else if (index <= Byte.MaxValue)
			{
				il.Emit(OpCodes.Ldarg_S, (byte)index);
			}
			else
			{
				il.Emit(OpCodes.Ldarg, index);
			}
		}
		/// <summary>
		/// 加载指定索引的参数地址。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要加载的参数索引。</param>
		/// <remarks>会根据 <paramref name="index"/> 的值，选择最合适的 IL 指令。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		public static void EmitLoadArgAddress(this ILGenerator il, int index)
		{
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			Contract.EndContractBlock();
			if (index <= Byte.MaxValue)
			{
				il.Emit(OpCodes.Ldarga_S, (byte)index);
			}
			else
			{
				il.Emit(OpCodes.Ldarga, index);
			}
		}
		/// <summary>
		/// 获取栈顶的值转换为相应的地址。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="valueType">栈顶值的类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="valueType"/> 为 <c>null</c>。</exception>
		public static void EmitGetAddress(this ILGenerator il, Type valueType)
		{
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (valueType == null)
			{
				throw CommonExceptions.ArgumentNull("valueType");
			}
			Contract.EndContractBlock();
			LocalBuilder locFrom = il.GetManager().GetLocal(valueType);
			il.Emit(OpCodes.Stloc, locFrom);
			il.Emit(OpCodes.Ldloca, locFrom);
			il.GetManager().FreeLocal(locFrom);
		}

		#endregion // 加载参数

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
		public static void EmitConversion(this ILGenerator il, Type inputType, Type outputType, bool isChecked)
		{
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (inputType == null)
			{
				throw CommonExceptions.ArgumentNull("inputType");
			}
			if (outputType == null)
			{
				throw CommonExceptions.ArgumentNull("outputType");
			}
			Contract.EndContractBlock();
			Conversion conversion = ConversionFactory.GetConversion(inputType, outputType);
			if (conversion == null)
			{
				throw CommonExceptions.InvalidCastFromTo(inputType, outputType);
			}
			if (conversion is FromNullableConversion)
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
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		public static Converter GetConversion(this ILGenerator il, Type inputType, Type outputType)
		{
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (inputType == null)
			{
				throw CommonExceptions.ArgumentNull("inputType");
			}
			if (outputType == null)
			{
				throw CommonExceptions.ArgumentNull("outputType");
			}
			Contract.EndContractBlock();
			Conversion conversion = ConversionFactory.GetConversion(inputType, outputType);
			if (conversion == null)
			{
				throw CommonExceptions.InvalidCastFromTo(inputType, outputType);
			}
			return new Converter(conversion, il, inputType, outputType);
		}

		#endregion // 类型转换

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
			if (il == null)
			{
				throw CommonExceptions.ArgumentNull("il");
			}
			if (method == null)
			{
				throw CommonExceptions.ArgumentNull("method");
			}
			Contract.EndContractBlock();
			if (method.MemberType == MemberTypes.Constructor)
			{
				il.Emit(opCode, (ConstructorInfo)method);
			}
			else
			{
				il.Emit(opCode, (MethodInfo)method);
			}
		}
	}
}
