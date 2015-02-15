using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示与 <see cref="decimal"/> 相关的类型转换方法。
	/// </summary>
	internal class DecimalConversion : Conversion
	{
		/// <summary>
		/// 隐式数值转换的实例。
		/// </summary>
		public static readonly Conversion ImplicitNumeric = new DecimalConversion(ConversionType.ImplicitNumericConversion);
		/// <summary>
		/// 显式数值转换的实例。
		/// </summary>
		public static readonly Conversion ExplicitNumeric = new DecimalConversion(ConversionType.ExplicitNumericConversion);
		/// <summary>
		/// 显式枚举转换的实例。
		/// </summary>
		public static readonly Conversion ExplicitEnum = new DecimalConversion(ConversionType.EnumConversion);
		/// <summary>
		/// 使用指定的转换类型初始化 <see cref="DecimalConversion"/> 类的新实例。
		/// </summary>
		/// <param name="conversionType">当前转换的类型。</param>
		private DecimalConversion(ConversionType conversionType)
			: base(conversionType)
		{ }
		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			Contract.Assume((inputType == typeof(decimal) && outputType.IsNumeric()) ||
				(outputType == typeof(decimal) && inputType.IsNumeric()));
			MethodInfo method;
			if (inputType == typeof(decimal))
			{
				if (outputType.IsEnum)
				{
					outputType = Enum.GetUnderlyingType(outputType);
				}
				method = UserConversionCache.GetConversionTo(inputType, outputType);
			}
			else
			{
				if (inputType.IsEnum)
				{
					inputType = Enum.GetUnderlyingType(inputType);
				}
				method = UserConversionCache.GetConversionFrom(outputType, inputType);
			}
			generator.EmitCall(method);
		}
	}
}
