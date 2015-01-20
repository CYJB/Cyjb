using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示可空类型到非可空值类型的转换，栈顶必须是值的地址。
	/// </summary>
	internal class FromNullableConversion : Conversion
	{
		/// <summary>
		/// <see cref="FromNullableConversion"/> 的默认实例。
		/// </summary>
		public static readonly Conversion Default = new FromNullableConversion(ConversionType.ExplicitNullableConversion);
		/// <summary>
		/// <see cref="FromNullableConversion"/> 的用户自定义类型转换实例。
		/// </summary>
		public static readonly Conversion UserDefined = new FromNullableConversion(ConversionType.UserDefinedConversion);
		/// <summary>
		/// 使用指定的转换类型初始化 <see cref="FromNullableConversion"/> 类的新实例。
		/// </summary>
		/// <param name="conversionType">当前转换的类型。</param>
		private FromNullableConversion(ConversionType conversionType) : base(conversionType) { }
		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			Contract.Requires(inputType.IsNullable());
			Type inputUnderlyingType = Nullable.GetUnderlyingType(inputType);
			MethodInfo getValueMethod = inputType.GetMethod("get_Value");
			generator.Emit(OpCodes.Call, getValueMethod);
			if (inputUnderlyingType != outputType)
			{
				ConversionFactory.GetConversion(inputUnderlyingType, outputType)
					.Emit(generator, inputUnderlyingType, outputType, isChecked);
			}
		}
	}
}
