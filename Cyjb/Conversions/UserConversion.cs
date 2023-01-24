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
		public readonly MethodInfo Method;

		/// <summary>
		/// 使用指定的类型转换方法初始化 <see cref="UserConversion"/> 类的新实例。
		/// </summary>
		/// <param name="method">类型转换方法。</param>
		/// <param name="isImplicit">是否是隐式类型转换。</param>
		public UserConversion(MethodInfo method, bool isImplicit)
			: base(isImplicit ? ConversionType.ImplicitUserDefined : ConversionType.ExplicitUserDefined)
		{
			Method = method;
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
			Type methodType = Method.GetParametersNoCopy()[0].ParameterType;
			// 额外的入参转换
			Conversion? conv = ConversionFactory.GetPreDefinedConversion(inputType, methodType);
			conv?.Emit(generator, inputType, methodType, isChecked);
			generator.EmitCall(Method);
			methodType = Method.ReturnType;
			// 额外的出参转换
			conv = ConversionFactory.GetPreDefinedConversion(methodType, outputType);
			if (conv != null)
			{
				if (conv is FromNullableConversion)
				{
					generator.EmitGetAddress(methodType);
				}
				conv.Emit(generator, methodType, outputType, isChecked);
			}
		}
	}
}
