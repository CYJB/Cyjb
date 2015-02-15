using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;

namespace Cyjb.Reflection
{
	public static partial class ILExt
	{

		#region 成员反射信息常量

		/// <summary>
		/// 表示 <see cref="ArgumentNullException"/> 的构造函数。
		/// </summary>
		private static readonly ConstructorInfo argumentNullExceptionCtor =
			typeof(ArgumentNullException).GetConstructor(new[] { typeof(string) });
		/// <summary>
		/// 表示 <see cref="TargetParameterCountException"/> 的构造函数。
		/// </summary>
		private static readonly ConstructorInfo targetParameterCountExceptionCtor =
			typeof (TargetParameterCountException).GetConstructor(Type.EmptyTypes);

		#endregion // 成员反射信息常量

		#region 参数异常

		/// <summary>
		/// 检查指定参数是否为 <c>null</c>。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要检查的参数索引。</param>
		/// <param name="paramName">为 <c>null</c> 的参数名。</param>
		internal static void EmitCheckArgumentNull(this ILGenerator il, int index, string paramName)
		{
			Contract.Requires(il != null);
			Label trueLabel = il.DefineLabel();
			il.EmitLoadArg(index);
			il.Emit(OpCodes.Brtrue, trueLabel);
			il.EmitConstant(paramName);
			il.Emit(OpCodes.Newobj, argumentNullExceptionCtor);
			il.Emit(OpCodes.Throw);
			il.MarkLabel(trueLabel);
		}
		/// <summary>
		/// 检查目标参数数组的长度。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="length">数组的期望长度。</param>
		internal static void EmitCheckTargetParameterCount(this ILGenerator il, int length)
		{
			Contract.Requires(il != null);
			Label trueLabel = il.DefineLabel();
			il.Emit(OpCodes.Ldlen);
			il.EmitConstant(length);
			il.Emit(OpCodes.Ceq);
			il.Emit(OpCodes.Brtrue, trueLabel);
			il.Emit(OpCodes.Newobj, targetParameterCountExceptionCtor);
			il.Emit(OpCodes.Throw);
			il.MarkLabel(trueLabel);
		}

		#endregion // 参数异常

	}
}
