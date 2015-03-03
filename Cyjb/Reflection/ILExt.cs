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

		#region 成员反射信息常量

		/// <summary>
		/// 表示 <see cref="Closure.Constants"/> 字段。
		/// </summary>
		private static readonly FieldInfo closureConstants = typeof(Closure).GetField("Constants");
		/// <summary>
		/// 表示 <c>System.Reflection.Emit.DynamicMethod+RTDynamicMethod</c> 类。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Type rtDynamicMethodType = Type.GetType("System.Reflection.Emit.DynamicMethod+RTDynamicMethod");
		/// <summary>
		/// 获取 <c>System.Reflection.Emit.DynamicMethod+RTDynamicMethod</c> 类对应的 <see cref="DynamicMethod"/> 的方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Func<object, DynamicMethod> getDynamicMethod;
		/// <summary>
		/// 获取 <c>System.Reflection.Emit.DynamicMethod+RTDynamicMethod</c> 类对应的 <see cref="DynamicMethod"/> 的方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Func<object, DynamicMethod> GetDynamicMethod
		{
			get
			{
				if (getDynamicMethod == null)
				{
					getDynamicMethod = rtDynamicMethodType.GetField("m_owner", TypeExt.InstanceFlag)
						.CreateDelegate<Func<object, DynamicMethod>>();
				}
				return getDynamicMethod;
			}
		}

		#endregion // 成员反射信息常量

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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(type, "type");
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(local, "local");
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
		/// <overloads>
		/// <summary>
		/// 加载指定索引的参数。
		/// </summary>
		/// </overloads>
		public static void EmitLoadArg(this ILGenerator il, int index)
		{
			CommonExceptions.CheckArgumentNull(il, "il");
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
			CommonExceptions.CheckArgumentNull(il, "il");
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
			CommonExceptions.CheckArgumentNull(il, "il");
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			CommonExceptions.CheckArgumentNull(paramType, "paramType");
			CommonExceptions.CheckArgumentNull(targetType, "targetType");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			CommonExceptions.CheckArgumentNull(paramType, "paramType");
			CommonExceptions.CheckArgumentNull(targetType, "targetType");
			Contract.EndContractBlock();
			if (paramType.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(paramType);
			}
			if (targetType.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(targetType);
			}
			if (paramType == targetType)
			{
				il.EmitLoadArg(index);
				return;
			}
			Converter converter = il.GetConversion(paramType, targetType, ConversionType.UserDefined);
			if (converter == null)
			{
				throw CommonExceptions.InvalidCast(paramType, targetType);
			}
			if (converter.PassByAddr)
			{
				il.EmitLoadArgAddress(index);
			}
			else
			{
				il.EmitLoadArg(index);
			}
			converter.Emit(isChecked);
		}

		#endregion // 加载参数

		#region 加载值

		/// <summary>
		/// 加载栈顶地址指向的值。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="type">要间接加载的值的类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		public static void EmitLoadIndirect(this ILGenerator il, Type type)
		{
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.EndContractBlock();
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

		#endregion // 加载值

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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.EndContractBlock();
			il.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
		}

		#endregion 加载字段

		#region 数组元素

		/// <summary>
		/// 加载闭包中指定索引的常量，要求闭包总是作为第一个参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要加载的常量索引。</param>
		/// <param name="constantType">要加载的常量类型。</param>
		internal static void EmitLoadClosureConstant(this ILGenerator il, int index, Type constantType)
		{
			Contract.Requires(il != null && index >= 0 && constantType != null);
			il.EmitLoadClosureConstant(index, constantType, true);
		}
		/// <summary>
		/// 加载闭包中指定索引的常量，要求闭包总是作为第一个参数。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="index">要加载的常量索引。</param>
		/// <param name="constantType">要加载的常量类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		internal static void EmitLoadClosureConstant(this ILGenerator il, int index, Type constantType, bool isChecked)
		{
			Contract.Requires(il != null && index >= 0 && constantType != null);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, closureConstants);
			il.EmitInt(index);
			il.Emit(OpCodes.Ldelem_Ref);
			il.EmitConversion(typeof(object), constantType, isChecked, ConversionType.Explicit);
		}
		/// <summary>
		/// 加载数组元素，要求已将数组对象和元素索引压入堆栈。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="elementType">要加载的数组元素类型。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="elementType"/> 为 <c>null</c>。</exception>
		public static void EmitLoadElement(this ILGenerator il, Type elementType)
		{
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(elementType, "elementType");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(elementType, "elementType");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(inputType, "inputType");
			CommonExceptions.CheckArgumentNull(outputType, "outputType");
			Contract.EndContractBlock();
			if (inputType.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(inputType);
			}
			if (outputType.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(outputType);
			}
			EmitConversion(il, inputType, outputType, isChecked, ConversionType.UserDefined);
		}
		/// <summary>
		/// 将栈顶的对象从 <paramref name="inputType"/> 转换为 <paramref name="outputType"/>。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		/// <param name="conversionType">类型转换类型的限制。</param>
		internal static void EmitConversion(this ILGenerator il, Type inputType, Type outputType, bool isChecked,
			ConversionType conversionType)
		{
			Contract.Requires(il != null && inputType != null && outputType != null);
			Contract.Requires(conversionType == ConversionType.Implicit ||
				conversionType == ConversionType.Explicit ||
				conversionType == ConversionType.UserDefined);
			Conversion conversion = conversionType == ConversionType.UserDefined ?
				ConversionFactory.GetConversion(inputType, outputType) :
				ConversionFactory.GetPreDefinedConversion(inputType, outputType);
			if (conversion == null || conversion.ConversionType > conversionType)
			{
				throw CommonExceptions.InvalidCast(inputType, outputType);
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
		/// <returns>类型转换的指令生成器，如果不能进行类型转换则返回 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="inputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="outputType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="inputType"/> 包含泛型参数。</exception>
		/// <exception cref="ArgumentException"><paramref name="outputType"/> 包含泛型参数。</exception>
		public static Converter GetConversion(this ILGenerator il, Type inputType, Type outputType)
		{
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(inputType, "inputType");
			CommonExceptions.CheckArgumentNull(outputType, "outputType");
			Contract.EndContractBlock();
			if (inputType.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(inputType);
			}
			if (outputType.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(outputType);
			}
			return GetConversion(il, inputType, outputType, ConversionType.UserDefined);
		}
		/// <summary>
		/// 获取转换类型的指令生成器，能够将栈顶的对象从 <paramref name="inputType"/> 转换为 
		/// <paramref name="outputType"/>。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="conversionType">类型转换类型的限制。</param>
		/// <returns>类型转换的指令生成器，如果不能进行类型转换则返回 <c>null</c>。</returns>
		internal static Converter GetConversion(this ILGenerator il, Type inputType, Type outputType,
			ConversionType conversionType)
		{
			Contract.Requires(il != null && inputType != null && outputType != null);
			Contract.Requires(conversionType == ConversionType.Implicit ||
				conversionType == ConversionType.Explicit ||
				conversionType == ConversionType.UserDefined);
			Conversion conversion = conversionType == ConversionType.UserDefined ?
				ConversionFactory.GetConversion(inputType, outputType) :
				ConversionFactory.GetPreDefinedConversion(inputType, outputType);
			if (conversion == null || conversion.ConversionType > conversionType)
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
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
		/// <summary>
		/// 调用指定的方法（call）。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="method">要调用的方法。</param>
		/// <exception cref="ArgumentNullException"><paramref name="il"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static void EmitCall(this ILGenerator il, MethodInfo method)
		{
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			Contract.Requires(il != null && method != null);
			if (rtDynamicMethodType.IsInstanceOfType(method))
			{
				// RTDynamicMethod 不能直接调用，需要取得相应的 DynamicMethod 才可以。
				method = GetDynamicMethod(method);
			}
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			params Type[] parameterTypes)
		{
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			params Type[] parameterTypes)
		{
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(method, "method");
			Contract.EndContractBlock();
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
			params Type[] parameterTypes)
		{
			Contract.Requires(il != null && method != null);
			if (rtDynamicMethodType.IsInstanceOfType(method))
			{
				// RTDynamicMethod 不能直接调用，需要取得相应的 DynamicMethod 才可以。
				method = GetDynamicMethod(method);
			}
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
			Contract.Requires(method != null);
			if (method.IsStatic)
			{
				return false;
			}
			Type declType = method.DeclaringType;
			Contract.Assume(declType != null);
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(valueType, "valueType");
			Contract.EndContractBlock();
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
			CommonExceptions.CheckArgumentNull(il, "il");
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.EndContractBlock();
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.TypeContainsGenericParameters(type);
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
				ConstructorInfo ctor = type.GetConstructor(TypeExt.InstanceFlag, null, Type.EmptyTypes, null);
				if (ctor == null)
				{
					throw CommonExceptions.TypeMissingDefaultConstructor(type);
				}
				il.Emit(OpCodes.Newobj, ctor);
			}
		}
	}
}
