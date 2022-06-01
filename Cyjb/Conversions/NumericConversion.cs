using System.Reflection.Emit;

namespace Cyjb.Conversions;

/// <summary>
/// 表示数值类型的转换。
/// </summary>
internal class NumericConversion : Conversion
{

	#region 静态属性

	/// <summary>
	/// <see cref="sbyte"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion SByte = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I1 }, new[] { OpCodes.Conv_Ovf_I1 });
	/// <summary>
	/// <see cref="sbyte"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion SByteUn = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I1 }, new[] { OpCodes.Conv_Ovf_I1_Un });

	/// <summary>
	/// <see cref="byte"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Byte = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_U1 }, new[] { OpCodes.Conv_Ovf_U1 });
	/// <summary>
	/// <see cref="byte"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion ByteUn = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_U1 }, new[] { OpCodes.Conv_Ovf_U1_Un });

	/// <summary>
	/// <see cref="short"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Int16 = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I2 }, new[] { OpCodes.Conv_Ovf_I2 });
	/// <summary>
	/// <see cref="short"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Int16Un = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I2 }, new[] { OpCodes.Conv_Ovf_I2_Un });

	/// <summary>
	/// <see cref="ushort"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt16 = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_U2 }, new[] { OpCodes.Conv_Ovf_U2 });
	/// <summary>
	/// <see cref="ushort"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt16Un = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_U2 }, new[] { OpCodes.Conv_Ovf_U2_Un });

	/// <summary>
	/// <see cref="int"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Int32 = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I4 }, new[] { OpCodes.Conv_Ovf_I4 });
	/// <summary>
	/// <see cref="int"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Int32Un = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I4 }, new[] { OpCodes.Conv_Ovf_I4_Un });

	/// <summary>
	/// <see cref="uint"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt32 = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_U4 }, new[] { OpCodes.Conv_Ovf_U4 });
	/// <summary>
	/// <see cref="uint"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt32Empty = new NumericConversion(ConversionType.ExplicitNumeric,
		Array.Empty<OpCode>(), new[] { OpCodes.Conv_Ovf_U4 });
	/// <summary>
	/// <see cref="uint"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt32Un = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_U4 }, new[] { OpCodes.Conv_Ovf_U4_Un });

	/// <summary>
	/// <see cref="long"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Int64Implicit = new NumericConversion(ConversionType.ImplicitNumeric,
		OpCodes.Conv_I8);
	/// <summary>
	/// <see cref="long"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Int64Explicit = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I8 }, new[] { OpCodes.Conv_Ovf_I8 });
	/// <summary>
	/// <see cref="long"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Int64UnExplicit = new NumericConversion(ConversionType.ExplicitNumeric,
		Array.Empty<OpCode>(), new[] { OpCodes.Conv_Ovf_I8_Un });

	/// <summary>
	/// <see cref="ulong"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt64Implicit = new NumericConversion(ConversionType.ImplicitNumeric,
		OpCodes.Conv_U8);
	/// <summary>
	/// <see cref="ulong"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt64Explicit = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_I8 }, new[] { OpCodes.Conv_Ovf_U8 });
	/// <summary>
	/// <see cref="ulong"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion UInt64UnExplicit = new NumericConversion(ConversionType.ExplicitNumeric,
		new[] { OpCodes.Conv_U8 }, new[] { OpCodes.Conv_Ovf_U8 });

	/// <summary>
	/// <see cref="float"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion SingleImplicit = new NumericConversion(ConversionType.ImplicitNumeric,
		OpCodes.Conv_R4);
	/// <summary>
	/// <see cref="float"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion SingleExplicit = new NumericConversion(ConversionType.ExplicitNumeric,
		OpCodes.Conv_R4);
	/// <summary>
	/// <see cref="float"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion SingleUnImplicit = new NumericConversion(ConversionType.ImplicitNumeric,
		OpCodes.Conv_R_Un, OpCodes.Conv_R4);

	/// <summary>
	/// <see cref="double"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion Double = new NumericConversion(ConversionType.ImplicitNumeric,
		OpCodes.Conv_R8);
	/// <summary>
	/// <see cref="double"/> 类型转换的实例。
	/// </summary>
	public static readonly Conversion DoubleUn = new NumericConversion(ConversionType.ImplicitNumeric,
		OpCodes.Conv_R_Un, OpCodes.Conv_R8);

	#endregion // 静态属性

	/// <summary>
	/// 返回将对象从数值类型 <paramref name="inputTypeCode"/> 转换为数值类型 <paramref name="outputTypeCode"/> 
	/// 的类型转换。要求类型不能是 <see cref="TypeCode.Decimal"/>。
	/// </summary>
	/// <param name="inputTypeCode">要转换的对象的类型。</param>
	/// <param name="outputTypeCode">要将输入对象转换到的类型。</param>
	/// <returns>将对象从 <paramref name="inputTypeCode"/> 类型转换为 <paramref name="outputTypeCode"/> 
	/// 类型的类型转换。</returns>
	public static Conversion GetConversion(TypeCode inputTypeCode, TypeCode outputTypeCode)
	{
		bool fromUnsigned = inputTypeCode.IsUnsigned();
		switch (outputTypeCode)
		{
			case TypeCode.Char:
				if (inputTypeCode == TypeCode.Byte || inputTypeCode == TypeCode.UInt16)
				{
					return IdentityConversion.ExplicitNumeric;
				}
				else
				{
					return fromUnsigned ? UInt16Un : UInt16;
				}
			case TypeCode.SByte:
				return fromUnsigned ? SByteUn : SByte;
			case TypeCode.Byte:
				return fromUnsigned ? ByteUn : Byte;
			case TypeCode.Int16:
				if (inputTypeCode == TypeCode.SByte || inputTypeCode == TypeCode.Byte)
				{
					return IdentityConversion.ImplicitNumeric;
				}
				else
				{
					return fromUnsigned ? Int16Un : Int16;
				}
			case TypeCode.UInt16:
				if (inputTypeCode == TypeCode.Byte || inputTypeCode == TypeCode.Char)
				{
					return IdentityConversion.ImplicitNumeric;
				}
				else
				{
					return fromUnsigned ? UInt16Un : UInt16;
				}
			case TypeCode.Int32:
				if (inputTypeCode < outputTypeCode || inputTypeCode == TypeCode.Char)
				{
					return IdentityConversion.ImplicitNumeric;
				}
				else
				{
					return fromUnsigned ? Int32Un : Int32;
				}
			case TypeCode.UInt32:
				if (fromUnsigned)
				{
					if (inputTypeCode == TypeCode.UInt64)
					{
						return UInt32Un;
					}
					return IdentityConversion.ImplicitNumeric;
				}
				else
				{

					return inputTypeCode < outputTypeCode ? UInt32Empty : UInt32;
				}
			case TypeCode.Int64:
				if (inputTypeCode < outputTypeCode || inputTypeCode == TypeCode.Char)
				{
					return fromUnsigned ? UInt64Implicit : Int64Implicit;
				}
				else
				{
					return fromUnsigned ? Int64UnExplicit : Int64Explicit;
				}
			case TypeCode.UInt64:
				if (fromUnsigned)
				{
					return UInt64Implicit;
				}
				else if (inputTypeCode < outputTypeCode)
				{
					return UInt64Explicit;
				}
				else
				{
					return UInt64UnExplicit;
				}
			case TypeCode.Single:
				if (inputTypeCode == TypeCode.Double)
				{
					return SingleExplicit;
				}
				else if (inputTypeCode == TypeCode.UInt32 || inputTypeCode == TypeCode.UInt64)
				{
					return SingleUnImplicit;
				}
				else
				{
					return SingleImplicit;
				}
			case TypeCode.Double:
				if (inputTypeCode == TypeCode.UInt32 || inputTypeCode == TypeCode.UInt64)
				{
					return DoubleUn;
				}
				else
				{
					return Double;
				}
		}
		throw CommonExceptions.Unreachable();
	}

	/// <summary>
	/// 要写入的无溢出检查的 IL 指令。
	/// </summary>
	private readonly OpCode[] uncheckedOpCodes;
	/// <summary>
	/// 要写入的有溢出检查的 IL 指令。
	/// </summary>
	private readonly OpCode[] checkedOpCodes;

	/// <summary>
	/// 使用指定的转换类型和要写入的 IL 指令初始化 <see cref="NumericConversion"/> 类的新实例。
	/// </summary>
	/// <param name="conversionType">当前转换的类型。</param>
	/// <param name="opCodes">要写入的 IL 指令。</param>
	private NumericConversion(ConversionType conversionType, params OpCode[] opCodes)
		: base(conversionType)
	{
		uncheckedOpCodes = checkedOpCodes = opCodes;
	}

	/// <summary>
	/// 使用指定的转换类型和要写入的 IL 指令初始化 <see cref="NumericConversion"/> 类的新实例。
	/// </summary>
	/// <param name="conversionType">当前转换的类型。</param>
	/// <param name="uncheckedOpCodes">要写入的无溢出检查的 IL 指令。</param>
	/// <param name="checkedOpCodes">要写入的有溢出检查的 IL 指令。</param>
	private NumericConversion(ConversionType conversionType, OpCode[] uncheckedOpCodes, OpCode[] checkedOpCodes)
		: base(conversionType)
	{
		this.uncheckedOpCodes = uncheckedOpCodes;
		this.checkedOpCodes = checkedOpCodes;
	}

	/// <summary>
	/// 使用指定的转换类型和要复制 IL 指令的数值类型转换初始化 <see cref="NumericConversion"/> 类的新实例。
	/// </summary>
	/// <param name="conversionType">当前转换的类型。</param>
	/// <param name="conversion">要复制的数值类型转换。</param>
	public NumericConversion(ConversionType conversionType, NumericConversion conversion)
		: base(conversionType)
	{
		uncheckedOpCodes = conversion.uncheckedOpCodes;
		checkedOpCodes = conversion.checkedOpCodes;
	}

	/// <summary>
	/// 写入类型转换的 IL 指令。
	/// </summary>
	/// <param name="generator">IL 的指令生成器。</param>
	/// <param name="inputType">要转换的对象的类型。</param>
	/// <param name="outputType">要将输入对象转换到的类型。</param>
	/// <param name="isChecked">是否执行溢出检查。</param>
	public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
	{
		OpCode[] opCodes = isChecked ? checkedOpCodes : uncheckedOpCodes;
		foreach (OpCode opCode in opCodes)
		{
			generator.Emit(opCode);
		}
	}
}
