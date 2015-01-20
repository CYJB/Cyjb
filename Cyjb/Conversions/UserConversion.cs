using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示用户自定义类型转换方法。
	/// </summary>
	internal class UserConversion : Conversion
	{
		/// <summary>
		/// 用户自定义类型转换方法。
		/// </summary>
		private readonly MethodInfo method;
		/// <summary>
		/// 使用指定的类型转换方法初始化 <see cref="UserConversion"/> 类的新实例。
		/// </summary>
		/// <param name="method">类型转换方法。</param>
		public UserConversion(MethodInfo method)
			: base(ConversionType.UserDefinedConversion)
		{
			Contract.Requires(method != null);
			this.method = method;
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
			Type methodType = method.GetParametersNoCopy()[0].ParameterType;
			Conversion conv = ConversionFactory.GetPreDefinedConversion(inputType, methodType);
			conv.Emit(generator, inputType, methodType, isChecked);
			generator.Emit(OpCodes.Call, method);
			methodType = method.ReturnType;
			conv = ConversionFactory.GetPreDefinedConversion(methodType, outputType);
			if (conv is FromNullableConversion)
			{
				generator.EmitGetAddress(methodType);
			}
			conv.Emit(generator, methodType, outputType, isChecked);
		}
	}
}
