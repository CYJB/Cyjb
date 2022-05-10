using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Conversions;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供提交加载的 IL 指令的扩展方法。
	/// </summary>
	public static partial class ILGeneratorUtil
	{

		#region 加载参数

		/// <summary>
		/// 加载指定索引的参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要加载的参数索引。</param>
		/// <remarks>会根据 <paramref name="index"/> 的值，选择最合适的 IL 指令。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 加载指定索引的参数。
		/// </summary>
		/// </overloads>
		public static void EmitLoadArg(this ILGenerator il, int index)
		{
			ArgumentNullException.ThrowIfNull(il);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
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
			else if (index <= byte.MaxValue)
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
			ArgumentNullException.ThrowIfNull(il);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (index <= byte.MaxValue)
			{
				il.Emit(OpCodes.Ldarga_S, (byte)index);
			}
			else
			{
				il.Emit(OpCodes.Ldarga, index);
			}
		}

		/// <summary>
		/// 加载指定索引的参数，并转换为指定类型。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要加载的参数索引。</param>
		/// <param name="paramType">要加载的参数类型。</param>
		/// <param name="targetType">要转换到的类型。</param>
		/// <remarks>会根据 <paramref name="index"/> 的值，选择最合适的 IL 指令。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="paramType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		public static void EmitLoadArg(this ILGenerator il, int index, Type paramType, Type targetType)
		{
			ArgumentNullException.ThrowIfNull(il);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			ArgumentNullException.ThrowIfNull(paramType);
			ArgumentNullException.ThrowIfNull(targetType);
			EmitLoadArg(il, index, paramType, targetType, true);
		}

		/// <summary>
		/// 加载指定索引的参数，并转换为指定类型。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要加载的参数索引。</param>
		/// <param name="paramType">要加载的参数类型。</param>
		/// <param name="targetType">要转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		/// <remarks>会根据 <paramref name="index"/> 的值，选择最合适的 IL 指令。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="paramType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="paramType"/> 包含泛型参数。</exception>
		/// <exception cref="ArgumentException"><paramref name="targetType"/> 包含泛型参数。</exception>
		public static void EmitLoadArg(this ILGenerator il, int index, Type paramType, Type targetType, bool isChecked)
		{
			ArgumentNullException.ThrowIfNull(il);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			ArgumentNullException.ThrowIfNull(paramType);
			ArgumentNullException.ThrowIfNull(targetType);
			if (paramType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(paramType);
			}
			if (targetType.ContainsGenericParameters)
			{
				throw ReflectionExceptions.TypeContainsGenericParameters(targetType);
			}
			if (paramType == targetType)
			{
				il.EmitLoadArg(index);
				return;
			}
			Conversion? conversion = ConversionFactory.GetConversion(paramType, targetType);
			if (conversion == null)
			{
				throw CommonExceptions.InvalidCast(paramType, targetType);
			}
			if (conversion.PassByAddr)
			{
				il.EmitLoadArgAddress(index);
			}
			else
			{
				il.EmitLoadArg(index);
			}
			conversion.Emit(il, paramType, targetType, isChecked);
		}

		#endregion // 加载参数

		#region 加载字段

		/// <summary>
		/// 写入加载字段值的指令。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="field">要加载的字段信息。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		public static void EmitLoadField(this ILGenerator il, FieldInfo field)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(field);
			il.Emit(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
		}

		/// <summary>
		/// 写入加载字段地址的指令。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="field">要加载的字段信息。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		public static void EmitLoadFieldAddress(this ILGenerator il, FieldInfo field)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(field);
			il.Emit(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field);
		}

		/// <summary>
		/// 写入设置字段值的指令。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="field">要设置的字段信息。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		public static void EmitStoreField(this ILGenerator il, FieldInfo field)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(field);
			il.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
		}

		#endregion 加载字段

		#region 数组元素

		/// <summary>
		/// 加载数组元素，要求已将数组对象和元素索引压入堆栈。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="elementType">要加载的数组元素类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="elementType"/> 为 <c>null</c>。</exception>
		public static void EmitLoadElement(this ILGenerator il, Type elementType)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(elementType);
			if (!elementType.IsValueType)
			{
				il.Emit(OpCodes.Ldelem_Ref);
			}
			else if (elementType.IsEnum)
			{
				il.Emit(OpCodes.Ldelem, elementType);
			}
			else
			{
				switch (Type.GetTypeCode(elementType))
				{
					case TypeCode.Boolean:
					case TypeCode.SByte:
						il.Emit(OpCodes.Ldelem_I1);
						break;
					case TypeCode.Byte:
						il.Emit(OpCodes.Ldelem_U1);
						break;
					case TypeCode.Int16:
						il.Emit(OpCodes.Ldelem_I2);
						break;
					case TypeCode.Char:
					case TypeCode.UInt16:
						il.Emit(OpCodes.Ldelem_U2);
						break;
					case TypeCode.Int32:
						il.Emit(OpCodes.Ldelem_I4);
						break;
					case TypeCode.UInt32:
						il.Emit(OpCodes.Ldelem_U4);
						break;
					case TypeCode.Int64:
					case TypeCode.UInt64:
						il.Emit(OpCodes.Ldelem_I8);
						break;
					case TypeCode.Single:
						il.Emit(OpCodes.Ldelem_R4);
						break;
					case TypeCode.Double:
						il.Emit(OpCodes.Ldelem_R8);
						break;
					default:
						il.Emit(OpCodes.Ldelem, elementType);
						break;
				}
			}
		}

		/// <summary>
		/// 替换数组元素，要求已将数组对象、元素索引和元素值压入堆栈。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="elementType">要替换的数组元素类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="elementType"/> 为 <c>null</c>。</exception>
		public static void EmitStoreElement(this ILGenerator il, Type elementType)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(elementType);
			if (!elementType.IsValueType)
			{
				il.Emit(OpCodes.Stelem_Ref);
			}
			if (elementType.IsEnum)
			{
				il.Emit(OpCodes.Stelem, elementType);
				return;
			}
			switch (Type.GetTypeCode(elementType))
			{
				case TypeCode.Boolean:
				case TypeCode.SByte:
				case TypeCode.Byte:
					il.Emit(OpCodes.Stelem_I1);
					break;
				case TypeCode.Char:
				case TypeCode.Int16:
				case TypeCode.UInt16:
					il.Emit(OpCodes.Stelem_I2);
					break;
				case TypeCode.Int32:
				case TypeCode.UInt32:
					il.Emit(OpCodes.Stelem_I4);
					break;
				case TypeCode.Int64:
				case TypeCode.UInt64:
					il.Emit(OpCodes.Stelem_I8);
					break;
				case TypeCode.Single:
					il.Emit(OpCodes.Stelem_R4);
					break;
				case TypeCode.Double:
					il.Emit(OpCodes.Stelem_R8);
					break;
				default:
					il.Emit(OpCodes.Stelem, elementType);
					break;
			}
		}

		#endregion // 数组元素

		/// <summary>
		/// 加载栈顶地址指向的值。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="type">要间接加载的值的类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		public static void EmitLoadIndirect(this ILGenerator il, Type type)
		{
			ArgumentNullException.ThrowIfNull(il);
			ArgumentNullException.ThrowIfNull(type);
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
				case TypeCode.SByte:
					il.Emit(OpCodes.Ldind_I1);
					break;
				case TypeCode.Byte:
					il.Emit(OpCodes.Ldind_U1);
					break;
				case TypeCode.Char:
				case TypeCode.Int16:
					il.Emit(OpCodes.Ldind_I2);
					break;
				case TypeCode.UInt16:
					il.Emit(OpCodes.Ldind_U2);
					break;
				case TypeCode.Int32:
					il.Emit(OpCodes.Ldind_I4);
					break;
				case TypeCode.UInt32:
					il.Emit(OpCodes.Ldind_U4);
					break;
				case TypeCode.Int64:
				case TypeCode.UInt64:
					il.Emit(OpCodes.Ldind_I8);
					break;
				case TypeCode.Single:
					il.Emit(OpCodes.Ldind_R4);
					break;
				case TypeCode.Double:
					il.Emit(OpCodes.Ldind_R8);
					break;
				default:
					if (type.IsValueType)
					{
						il.Emit(OpCodes.Ldobj, type);
					}
					else
					{
						il.Emit(OpCodes.Ldind_Ref);
					}
					break;
			}
		}

		/// <summary>
		/// 加载第一个参数作为实例，调用保证类型可以转换。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="member">要为实例调用的成员。</param>
		/// <param name="instanceType">实例实参的类型。</param>
		/// <param name="needCheckNull">是否检查实例是否为 <c>null</c>。</param>
		internal static void EmitLoadInstance(this ILGenerator il, MemberInfo member, Type instanceType, bool needCheckNull)
		{
			if (needCheckNull)
			{
				il.EmitCheckArgumentNull(0, "instance");
			}
			il.Emit(OpCodes.Ldarg_0);
			Type declType = member.DeclaringType!;
			il.EmitConversion(instanceType, declType, true);
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
		/// <param name="paramType">方法形参的类型。</param>
		/// <param name="index">要加载的实参索引。</param>
		/// <param name="argumentType">要加载的实参类型。</param>
		/// <returns>能否对方法进行 tailcall 优化，如果可以则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		internal static bool EmitLoadParameter(this ILGenerator il, Type paramType, int index, Type argumentType)
		{
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
					il.EmitLoadIndirect(argumentType.GetElementType()!);
				}
				else if (paramType != argumentType)
				{
					il.EmitConversion(argumentType, paramType, true);
				}
			}
			return true;
		}
	}
}
