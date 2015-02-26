using System;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示拆箱转换。
	/// </summary>
	internal class UnboxConversion : Conversion
	{
		/// <summary>
		/// <see cref="UnboxConversion"/> 的默认实例。
		/// </summary>
		public static readonly Conversion Default = new UnboxConversion();
		/// <summary>
		/// 初始化 <see cref="UnboxConversion"/> 类的新实例。
		/// </summary>
		private UnboxConversion() : base(ConversionType.Unbox) { }
		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要被转换的类型。</param>
		/// <param name="outputType">要转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			generator.Emit(OpCodes.Unbox_Any, outputType);
		}
	}
}
