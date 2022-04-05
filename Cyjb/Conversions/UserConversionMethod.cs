using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示用户定义的类型转换方法。
	/// </summary>
	internal class UserConversionMethod
	{
		/// <summary>
		/// 类型转换方法。
		/// </summary>
		public readonly MethodInfo Method;
		/// <summary>
		/// 是否是隐式类型转换。
		/// </summary>
		public bool IsImplicit;
		/// <summary>
		/// 类型转换方法的输入类型。
		/// </summary>
		public readonly Type InputType;
		/// <summary>
		/// 获取类型转换方法的目标类型，即方法的返回值类型。
		/// </summary>
		/// <value>类型转换方法的目标类型，即方法的返回值类型。</value>
		public Type OutputType => Method.ReturnType;

		/// <summary>
		/// 使用指定的类型转换方法初始化 <see cref="UserConversionMethod"/> 类的新实例。
		/// </summary>
		/// <param name="method">类型转换方法。</param>
		/// <param name="isImplicit">是否是隐式类型转换方法。</param>
		public UserConversionMethod(MethodInfo method, bool isImplicit)
		{
			Method = method;
			IsImplicit = isImplicit;
			InputType = method.GetParametersNoCopy()[0].ParameterType;
		}

		/// <summary>
		/// 返回表示当前对象的字符串。
		/// </summary>
		/// <returns>表示当前对象的字符串。</returns>
		public override string ToString()
		{
			string tag = IsImplicit ? "implicit" : "explicit";
			return $"[{tag}] {InputType} -> {OutputType}";
		}
	}
}
