using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示可空类型之间的类型转换，输出类型可能是引用类型。
	/// </summary>
	internal class BetweenNullableConversion : Conversion
	{
		/// <summary>
		/// 隐式可空类型转换的实例。
		/// </summary>
		public static readonly Conversion Implicit = new BetweenNullableConversion(ConversionType.ImplicitNullable);
		/// <summary>
		/// 显式可空类型转换的实例。
		/// </summary>
		public static readonly Conversion Explicit = new BetweenNullableConversion(ConversionType.ExplicitNullable);
		/// <summary>
		/// 用户自定义可空类型隐式转换的实例。
		/// </summary>
		public static readonly Conversion ImplicitUserDefined = new BetweenNullableConversion(ConversionType.ImplicitUserDefined);
		/// <summary>
		/// 用户自定义可空类型显示转换的实例。
		/// </summary>
		public static readonly Conversion ExplicitUserDefined = new BetweenNullableConversion(ConversionType.ExplicitUserDefined);

		/// <summary>
		/// 使用指定的转换类型初始化 <see cref="BetweenNullableConversion"/> 类的新实例。
		/// </summary>
		/// <param name="conversionType">当前转换的类型。</param>
		private BetweenNullableConversion(ConversionType conversionType) : base(conversionType) { }

		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要被转换的类型。</param>
		/// <param name="outputType">要转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			// 调用方已保证 inputType 和 outputType 已是 Nullable<T>
			Type inputUnderlyingType = Nullable.GetUnderlyingType(inputType)!;
			Type outputUnderlyingType = Nullable.GetUnderlyingType(outputType)!;
			// 定义变量和标签
			LocalBuilder inputLocal = generator.GetLocal(inputType);
			Label trueCase = generator.DefineLabel();
			Label endConvert = generator.DefineLabel();
			// > inputLocal = value;
			generator.Emit(OpCodes.Stloc, inputLocal);
			// > if (!input.HasValue)
			generator.Emit(OpCodes.Ldloca, inputLocal);
			generator.EmitCall(inputType.GetMethod("get_HasValue")!);
			generator.Emit(OpCodes.Brtrue, trueCase);
			// >     return null
			if (outputUnderlyingType != null)
			{
				// default(Nullable<T>);
				generator.EmitDefault(outputType);
			}
			else
			{
				// null; 引用类型。
				generator.Emit(OpCodes.Ldnull);
			}
			generator.Emit(OpCodes.Br, endConvert);
			// else
			generator.MarkLabel(trueCase);
			// (outputType)input.GetValueOrDefault();
			generator.Emit(OpCodes.Ldloca, inputLocal);
			generator.FreeLocal(inputLocal);
			generator.EmitCall(inputType.GetMethod("GetValueOrDefault", Type.EmptyTypes)!);
			Conversion conversion = ConversionFactory.GetConversion(inputUnderlyingType, outputUnderlyingType ?? outputType)!;
			conversion.Emit(generator, inputUnderlyingType, outputUnderlyingType ?? outputType, isChecked);
			if (outputUnderlyingType != null)
			{
				ConstructorInfo ctor = outputType.GetConstructor(new[] { outputUnderlyingType })!;
				generator.Emit(OpCodes.Newobj, ctor);
			}
			generator.MarkLabel(endConvert);
		}
	}
}
