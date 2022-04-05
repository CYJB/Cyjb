using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示非可空类型转换到可空类型的转换。
	/// </summary>
	internal class ToNullableConversion : Conversion
	{
		/// <summary>
		/// 隐式可空类型转换的实例。
		/// </summary>
		public static readonly Conversion Implicit = new ToNullableConversion(ConversionType.ImplicitNullable);
		/// <summary>
		/// 显式可空类型转换的实例。
		/// </summary>
		public static readonly Conversion Explicit = new ToNullableConversion(ConversionType.ExplicitNullable);
		/// <summary>
		/// 使用指定的转换类型初始化 <see cref="ToNullableConversion"/> 类的新实例。
		/// </summary>
		/// <param name="conversionType">当前转换的类型。</param>
		private ToNullableConversion(ConversionType conversionType) : base(conversionType) { }
		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			Contract.Assume(outputType.IsNullable());
			Type outputUnderlyingType = Nullable.GetUnderlyingType(outputType)!;
			if (inputType != outputUnderlyingType)
			{
				Conversion conversion = ConversionFactory.GetConversion(inputType, outputUnderlyingType)!;
				Contract.Assume(conversion != null);
				conversion.Emit(generator, inputType, outputUnderlyingType, isChecked);
			}
			ConstructorInfo ctor = outputType.GetConstructor(new[] { outputUnderlyingType })!;
			Contract.Assume(ctor != null);
			generator.Emit(OpCodes.Newobj, ctor);
		}
	}
}
