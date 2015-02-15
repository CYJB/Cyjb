using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示调用委托进行类型转换。
	/// </summary>
	internal class DelegateConversion : Conversion
	{
		/// <summary>
		/// 表示 <see cref="Convert.ChangeType{TInput, TOutput}"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo changeType = typeof(Convert).GetMethods().First(
			m => m.Name == "ChangeType" && m.IsGenericMethodDefinition);
		/// <summary>
		/// 要调用的类型转换委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Delegate converter;
		/// <summary>
		/// 使用要调用的委托初始化 <see cref="DelegateConversion"/> 类的新实例。
		/// </summary>
		/// <param name="converter">要调用的委托。</param>
		public DelegateConversion(Delegate converter)
			: base(ConversionType.UserDefinedConversion)
		{
			Contract.Requires(converter != null);
			this.converter = converter;
		}
		/// <summary>
		/// 获取要调用的类型转换委托。
		/// </summary>
		/// <value>要调用的类型转换委托。</value>
		public Delegate Converter
		{
			get { return this.converter; }
		}
		/// <summary>
		/// 写入类型转换的指令。
		/// </summary>
		/// <param name="generator">IL 的指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public override void Emit(ILGenerator generator, Type inputType, Type outputType, bool isChecked)
		{
			MethodInfo method = converter.Method;
			if (method.IsStatic && method.GetParametersNoCopy().Length == 1)
			{
				// 没有闭包的静态方法可以直接调用。
				generator.EmitCall(method);
			}
			else
			{
				// 有闭包的静态方法，或实例方法，其参数不能直接写入 IL，因此直接调用 Convert 的 ChangeType 方法。
				generator.EmitCall(changeType.MakeGenericMethod(inputType, outputType));
			}
		}
	}
}
