using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供提交常量 IL 指令的扩展方法。
	/// </summary>
	public static partial class ILGeneratorUtil
	{

		#region 成员反射信息常量

		/// <summary>
		/// 表示 <see cref="decimal(int)"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConstructorInfo decimalCtorInt = typeof(decimal).GetConstructor(new[] { typeof(int) })!;
		/// <summary>
		/// 表示 <see cref="decimal(long)"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConstructorInfo decimalCtorLong = typeof(decimal).GetConstructor(new[] { typeof(long) })!;
		/// <summary>
		/// 表示 <see cref="decimal(int, int, int, bool, byte)"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ConstructorInfo decimalCtorFull = typeof(decimal).GetConstructor(
			new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) })!;
		/// <summary>
		/// 表示 <see cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle)"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo getMethodFromHandle = typeof(MethodBase).GetMethod(
			"GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) })!;
		/// <summary>
		/// 表示 <see cref="MethodBase.GetMethodFromHandle(RuntimeMethodHandle, RuntimeTypeHandle)"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo getMethodFromHandleWithType = typeof(MethodBase).GetMethod(
			"GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) })!;
		/// <summary>
		/// 表示 <see cref="Type.GetTypeFromHandle"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo getTypeFromHandler = typeof(Type).GetMethod("GetTypeFromHandle")!;
		/// <summary>
		/// 表示 <see cref="Closure.Constants"/> 字段。
		/// </summary>
		private static readonly FieldInfo closureConstants = typeof(Closure).GetField("Constants")!;

		#endregion // 成员反射信息常量

		#region 基本类型常量

		/// <summary>
		/// 写入指定的 <see cref="bool"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, bool value)
		{
			CommonExceptions.CheckArgumentNull(il);
			il.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
		}

		/// <summary>
		/// 写入指定的 <see cref="char"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, char value)
		{
			il.EmitInt(value);
			il.Emit(OpCodes.Conv_U2);
		}

		/// <summary>
		/// 写入指定的 <see cref="sbyte"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static void EmitConstant(this ILGenerator il, sbyte value)
		{
			il.EmitInt(value);
			il.Emit(OpCodes.Conv_I1);
		}

		/// <summary>
		/// 写入指定的 <see cref="byte"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, byte value)
		{
			il.EmitInt(value);
			il.Emit(OpCodes.Conv_U1);
		}

		/// <summary>
		/// 写入指定的 <see cref="short"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, short value)
		{
			il.EmitInt(value);
			il.Emit(OpCodes.Conv_I2);
		}

		/// <summary>
		/// 写入指定的 <see cref="ushort"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static void EmitConstant(this ILGenerator il, ushort value)
		{
			il.EmitInt(value);
			il.Emit(OpCodes.Conv_U2);
		}

		/// <summary>
		/// 写入指定的 <see cref="int"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, int value)
		{
			il.EmitInt(value);
		}

		/// <summary>
		/// 写入指定的 <see cref="uint"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static void EmitConstant(this ILGenerator il, uint value)
		{
			il.EmitInt((int)value);
		}

		/// <summary>
		/// 写入指定的 <see cref="long"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, long value)
		{
			CommonExceptions.CheckArgumentNull(il);
			il.Emit(OpCodes.Ldc_I8, value);
		}

		/// <summary>
		/// 写入指定的 <see cref="ulong"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static void EmitConstant(this ILGenerator il, ulong value)
		{
			CommonExceptions.CheckArgumentNull(il);
			il.Emit(OpCodes.Ldc_I8, (long)value);
		}

		/// <summary>
		/// 写入指定的 <see cref="float"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, float value)
		{
			CommonExceptions.CheckArgumentNull(il);
			il.Emit(OpCodes.Ldc_R4, value);
		}

		/// <summary>
		/// 写入指定的 <see cref="double"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, double value)
		{
			CommonExceptions.CheckArgumentNull(il);
			il.Emit(OpCodes.Ldc_R8, value);
		}

		/// <summary>
		/// 写入指定的 <see cref="decimal"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, decimal value)
		{
			CommonExceptions.CheckArgumentNull(il);
			if (decimal.Truncate(value) == value)
			{
				if (int.MinValue <= value && value <= int.MaxValue)
				{
					int intValue = decimal.ToInt32(value);
					il.EmitConstant(intValue);
					il.Emit(OpCodes.Newobj, decimalCtorInt);
					return;
				}
				if (long.MinValue <= value && value <= long.MaxValue)
				{
					long longValue = decimal.ToInt64(value);
					il.EmitConstant(longValue);
					il.Emit(OpCodes.Newobj, decimalCtorLong);
					return;
				}
			}
			int[] bits = decimal.GetBits(value);
			il.EmitConstant(bits[0]);
			il.EmitConstant(bits[1]);
			il.EmitConstant(bits[2]);
			il.EmitConstant((bits[3] & 0x80000000) != 0);
			il.EmitConstant((byte)(bits[3] >> 16));
			il.Emit(OpCodes.Newobj, decimalCtorFull);
		}

		/// <summary>
		/// 写入指定的 <see cref="string"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitConstant(this ILGenerator il, string value)
		{
			CommonExceptions.CheckArgumentNull(il);
			if (value == null)
			{
				il.Emit(OpCodes.Ldnull);
			}
			else
			{
				il.Emit(OpCodes.Ldstr, value);
			}
		}

		#endregion // 基本类型常量

		/// <summary>
		/// 将指定的常量写入 IL 中。不能写入到 IL 中的常量（如自定义类）会使用闭包来实现。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <returns>如果常量可以直接写入 IL，则为 <c>true</c>。否则用到了闭包，返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static bool EmitConstant(this ILGenerator il, object? value)
		{
			CommonExceptions.CheckArgumentNull(il);
			if (value == null)
			{
				il.Emit(OpCodes.Ldnull);
				return true;
			}
			Type type = value.GetType();
			if (il.EmitPrimitiveConstant(value, type))
			{
				return true;
			}
			if (value is Type typeValue)
			{
				if (typeValue.IsGenericParameter || typeValue.IsVisible)
				{
					il.Emit(OpCodes.Ldtoken, type);
					il.EmitCall(getTypeFromHandler);
					if (type != typeof(Type))
					{
						il.Emit(OpCodes.Castclass, type);
					}
					return true;
				}
			}
			else if (value is MethodBase method)
			{
				Type? declaringType = method.DeclaringType;
				if (declaringType == null || declaringType.IsGenericParameter || declaringType.IsVisible)
				{
					il.Emit(OpCodes.Ldtoken, method);
					if (declaringType == null || !declaringType.IsGenericType)
					{
						il.EmitCall(getMethodFromHandle);
					}
					else
					{
						il.Emit(OpCodes.Ldtoken, declaringType);
						il.EmitCall(getMethodFromHandleWithType);
					}
					if (type != typeof(MethodBase))
					{
						il.Emit(OpCodes.Castclass, type);
					}
					return true;
				}
			}
			// IL 不支持的常量，使用闭包。
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, closureConstants);
			int idx = il.GetManager().GetClosure(value);
			il.EmitInt(idx);
			il.Emit(OpCodes.Ldelem_Ref);
			il.EmitConversion(typeof(object), type, false);
			return false;
		}

		/// <summary>
		/// 将指定的基元类型常量写入 IL 中。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的基元类型常量。</param>
		/// <param name="type">要写入的基元类型常量的类型。</param>
		/// <returns>如果是基元类型常量，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool EmitPrimitiveConstant(this ILGenerator il, object value, Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					il.EmitConstant((bool)value);
					return true;
				case TypeCode.Char:
					il.EmitConstant((char)value);
					return true;
				case TypeCode.SByte:
					il.EmitConstant((sbyte)value);
					return true;
				case TypeCode.Byte:
					il.EmitConstant((byte)value);
					return true;
				case TypeCode.Int16:
					il.EmitConstant((short)value);
					return true;
				case TypeCode.UInt16:
					il.EmitConstant((ushort)value);
					return true;
				case TypeCode.Int32:
					il.EmitInt((int)value);
					return true;
				case TypeCode.UInt32:
					il.EmitConstant((uint)value);
					return true;
				case TypeCode.Int64:
					il.EmitConstant((long)value);
					return true;
				case TypeCode.UInt64:
					il.EmitConstant((ulong)value);
					return true;
				case TypeCode.Single:
					il.EmitConstant((float)value);
					return true;
				case TypeCode.Double:
					il.EmitConstant((double)value);
					return true;
				case TypeCode.Decimal:
					il.EmitConstant((decimal)value);
					return true;
				case TypeCode.String:
					il.EmitConstant((string)value);
					return true;
			}
			return false;
		}

		/// <summary>
		/// 将写入指定类型默认值的指令放到指令流上。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="type">要写入默认值的类型。</param>
		internal static void EmitDefault(this ILGenerator il, Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Object:
				case TypeCode.DateTime:
					if (type.IsValueType)
					{
						// 需要每次使用新的变量。
						LocalBuilder lb = il.DeclareLocal(type);
						il.Emit(OpCodes.Ldloca, lb);
						il.Emit(OpCodes.Initobj, type);
						il.Emit(OpCodes.Ldloc, lb);
					}
					else
					{
						il.Emit(OpCodes.Ldnull);
					}
					break;
				case TypeCode.Empty:
				case TypeCode.String:
				case TypeCode.DBNull:
					il.Emit(OpCodes.Ldnull);
					break;
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
					il.Emit(OpCodes.Ldc_I4_0);
					break;
				case TypeCode.Int64:
				case TypeCode.UInt64:
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Conv_I8);
					break;
				case TypeCode.Single:
					il.Emit(OpCodes.Ldc_R4, 0f);
					break;
				case TypeCode.Double:
					il.Emit(OpCodes.Ldc_R8, 0d);
					break;
				case TypeCode.Decimal:
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Newobj, decimalCtorInt);
					break;
				default:
					throw ReflectionExceptions.Unreachable();
			}
		}

		/// <summary>
		/// 写入指定的 <see cref="int"/> 常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="value">要写入的常量。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		public static void EmitInt(this ILGenerator il, int value)
		{
			CommonExceptions.CheckArgumentNull(il);
			OpCode code;
			switch (value)
			{
				case -1:
					code = OpCodes.Ldc_I4_M1;
					break;
				case 0:
					code = OpCodes.Ldc_I4_0;
					break;
				case 1:
					code = OpCodes.Ldc_I4_1;
					break;
				case 2:
					code = OpCodes.Ldc_I4_2;
					break;
				case 3:
					code = OpCodes.Ldc_I4_3;
					break;
				case 4:
					code = OpCodes.Ldc_I4_4;
					break;
				case 5:
					code = OpCodes.Ldc_I4_5;
					break;
				case 6:
					code = OpCodes.Ldc_I4_6;
					break;
				case 7:
					code = OpCodes.Ldc_I4_7;
					break;
				case 8:
					code = OpCodes.Ldc_I4_8;
					break;
				default:
					if (value >= -128 && value <= 127)
					{
						il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
					}
					else
					{
						il.Emit(OpCodes.Ldc_I4, value);
					}
					return;
			}
			il.Emit(code);
		}

		/// <summary>
		/// <see cref="Array.Empty"/> 方法的信息。
		/// </summary>
		private static readonly MethodInfo ArrayEmptyMethod = typeof(Array).GetMethod("Empty")!;

		/// <summary>
		/// 写入指定类型的空数组常量。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="type">要写入的空数组类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		public static void EmitEmptyArray(this ILGenerator il, Type type)
		{
			CommonExceptions.CheckArgumentNull(il);
			CommonExceptions.CheckArgumentNull(type);
			il.EmitCall(ArrayEmptyMethod.MakeGenericMethod(type));
		}
	}
}
