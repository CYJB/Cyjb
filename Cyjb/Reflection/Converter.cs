using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;
using Cyjb.Conversions;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 表示类型转换的 IL 指令。
	/// </summary>
	public class Converter
	{
		/// <summary>
		/// 类型转换器。
		/// </summary>
		private readonly Conversion conversion;
		/// <summary>
		/// IL 的指令生成器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ILGenerator il;
		/// <summary>
		/// 要转换的对象的类型。
		/// </summary>
		private readonly Type inputType;
		/// <summary>
		/// 要将输入对象转换到的类型。
		/// </summary>
		private readonly Type outputType;
		/// <summary>
		/// 使用指定的类型转换器、IL 指令生成器和类型初始化 <see cref="Converter"/> 类的新实例。
		/// </summary>
		/// <param name="conversion">类型转换器。</param>
		/// <param name="il">IL 的指令生成器。</param>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		internal Converter(Conversion conversion, ILGenerator il, Type inputType, Type outputType)
		{
			Contract.Requires(conversion != null && il != null && inputType != null && outputType != null);
			this.conversion = conversion;
			this.il = il;
			this.inputType = inputType;
			this.outputType = outputType;
		}
		/// <summary>
		/// 获取要转换的参数是否需要从值转换为其地址。
		/// </summary>
		/// <value>要转换的参数是否需要从值转换为其地址。</value>
		/// <remarks>一般将参数从值转换为地址的方法，是使用 <c>stloc</c> 指令和 <c>ldloca</c> 指令。</remarks>
		public bool PassByAddr
		{
			get { return conversion is FromNullableConversion; }
		}
		/// <summary>
		/// 获取是否需要写入 IL（一些类型转换并不需要写入 IL）。
		/// </summary>
		/// <value>当前转换是否需要写入 IL。</value>
		public bool NeedEmit
		{
			get { return !(conversion is IdentityConversion); }
		}
		/// <summary>
		/// 写入类型转换的 IL 指令。
		/// </summary>
		/// <param name="isChecked">是否执行溢出检查。</param>
		public void Emit(bool isChecked)
		{
			conversion.Emit(il, inputType, outputType, isChecked);
		}
	}
}
