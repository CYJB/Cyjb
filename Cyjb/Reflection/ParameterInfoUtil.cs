using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="ParameterInfo"/> 的扩展方法。
	/// </summary>
	public static class ParameterInfoUtil
	{
		/// <summary>
		/// 获取当前参数是否是 params 参数。
		/// </summary>
		/// <param name="parameter">要判断的参数。</param>
		/// <returns>如果是 params 参数，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public static bool IsParamArray(this ParameterInfo? parameter)
		{
			return parameter != null && parameter.ParameterType.IsArray &&
				parameter.IsDefined(typeof(ParamArrayAttribute), true);
		}
	}
}
