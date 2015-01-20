using System;
using System.Reflection.Emit;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示显式引用转换。
	/// </summary>
	internal class CastClassConversion : Conversion
	{
		/// <summary>
		/// <see cref="CastClassConversion"/> 的默认实例。
		/// </summary>
		public static readonly Conversion Default = new CastClassConversion();
		/// <summary>
		/// 初始化 <see cref="CastClassConversion"/> 类的新实例。
		/// </summary>
		private CastClassConversion() : base(ConversionType.ExplicitReferenceConversion) { }
		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			generator.Emit(OpCodes.Castclass, outputType);
		}
	}
}
