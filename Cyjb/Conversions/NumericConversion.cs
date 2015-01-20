using System;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示数值类型的转换。
	/// </summary>
	internal class NumericConversion : Conversion
	{

		#region 静态属性

		/// <summary>
		/// <see cref="sbyte"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion SByte = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I1 }, new[] { OpCodes.Conv_Ovf_I1 });
		/// <summary>
		/// <see cref="sbyte"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion SByteUn = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I1 }, new[] { OpCodes.Conv_Ovf_I1_Un });

		/// <summary>
		/// <see cref="byte"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Byte = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_U1 }, new[] { OpCodes.Conv_Ovf_U1 });
		/// <summary>
		/// <see cref="byte"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion ByteUn = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_U1 }, new[] { OpCodes.Conv_Ovf_U1_Un });

		/// <summary>
		/// <see cref="short"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Int16 = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I2 }, new[] { OpCodes.Conv_Ovf_I2 });
		/// <summary>
		/// <see cref="short"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Int16Un = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I2 }, new[] { OpCodes.Conv_Ovf_I2_Un });

		/// <summary>
		/// <see cref="ushort"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt16 = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_U2 }, new[] { OpCodes.Conv_Ovf_U2 });
		/// <summary>
		/// <see cref="ushort"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt16Un = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_U2 }, new[] { OpCodes.Conv_Ovf_U2_Un });

		/// <summary>
		/// <see cref="int"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Int32 = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I4 }, new[] { OpCodes.Conv_Ovf_I4 });
		/// <summary>
		/// <see cref="int"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Int32Un = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I4 }, new[] { OpCodes.Conv_Ovf_I4_Un });

		/// <summary>
		/// <see cref="uint"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt32 = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_U4 }, new[] { OpCodes.Conv_Ovf_U4 });
		/// <summary>
		/// <see cref="uint"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt32Empty = new NumericConversion(ConversionType.ExplicitNumericConversion,
			ArrayExt.Empty<OpCode>(), new[] { OpCodes.Conv_Ovf_U4 });
		/// <summary>
		/// <see cref="uint"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt32Un = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_U4 }, new[] { OpCodes.Conv_Ovf_U4_Un });

		/// <summary>
		/// <see cref="long"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Int64Implicit = new NumericConversion(ConversionType.ImplicitNumericConversion,
			OpCodes.Conv_I8);
		/// <summary>
		/// <see cref="long"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Int64Explicit = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I8 }, new[] { OpCodes.Conv_Ovf_I8 });
		/// <summary>
		/// <see cref="long"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Int64UnExplicit = new NumericConversion(ConversionType.ExplicitNumericConversion,
			ArrayExt.Empty<OpCode>(), new[] { OpCodes.Conv_Ovf_I8_Un });

		/// <summary>
		/// <see cref="ulong"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt64Implicit = new NumericConversion(ConversionType.ImplicitNumericConversion,
			OpCodes.Conv_U8);
		/// <summary>
		/// <see cref="ulong"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt64Explicit = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_I8 }, new[] { OpCodes.Conv_Ovf_U8 });
		/// <summary>
		/// <see cref="ulong"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion UInt64UnExplicit = new NumericConversion(ConversionType.ExplicitNumericConversion,
			new[] { OpCodes.Conv_U8 }, new[] { OpCodes.Conv_Ovf_U8 });

		/// <summary>
		/// <see cref="float"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion SingleImplicit = new NumericConversion(ConversionType.ImplicitNumericConversion,
			OpCodes.Conv_R4);
		/// <summary>
		/// <see cref="float"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion SingleExplicit = new NumericConversion(ConversionType.ExplicitNumericConversion,
			OpCodes.Conv_R4);
		/// <summary>
		/// <see cref="float"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion SingleUnImplicit = new NumericConversion(ConversionType.ImplicitNumericConversion,
			OpCodes.Conv_R_Un, OpCodes.Conv_R4);

		/// <summary>
		/// <see cref="double"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion Double = new NumericConversion(ConversionType.ImplicitNumericConversion,
			OpCodes.Conv_R8);
		/// <summary>
		/// <see cref="double"/> 类型转换的实例。
		/// </summary>
		public static readonly Conversion DoubleUn = new NumericConversion(ConversionType.ImplicitNumericConversion,
			OpCodes.Conv_R_Un, OpCodes.Conv_R8);

		#endregion // 静态属性

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
			Contract.Requires(opCodes != null && opCodes.Length >= 1);
			this.uncheckedOpCodes = this.checkedOpCodes = opCodes;
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
			Contract.Requires(uncheckedOpCodes != null && uncheckedOpCodes.Length >= 0);
			Contract.Requires(checkedOpCodes != null && checkedOpCodes.Length >= 0);
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
			Contract.Requires(conversion != null);
			this.uncheckedOpCodes = conversion.uncheckedOpCodes;
			this.checkedOpCodes = conversion.checkedOpCodes;
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
			for (int i = 0; i < opCodes.Length; i++)
			{
				generator.Emit(opCodes[i]);
			}
		}
	}
}
