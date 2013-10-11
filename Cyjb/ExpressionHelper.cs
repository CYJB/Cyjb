using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// <see cref="Expression"/> 类的辅助方法。
	/// </summary>
	internal static class ExpressionHelper
	{
		/// <summary>
		/// 根据给定的参数信息创建参数表达式列表。
		/// </summary>
		/// <param name="paramInfos">参数信息。</param>
		/// <returns>参数表达式列表。</returns>
		public static ParameterExpression[] ToExpressions(this ParameterInfo[] paramInfos)
		{
			ParameterExpression[] paramList = new ParameterExpression[paramInfos.Length];
			for (int i = 0; i < paramInfos.Length; i++)
			{
				paramList[i] = Expression.Parameter(paramInfos[i].ParameterType);
			}
			return paramList;
		}
		/// <summary>
		/// 返回对指定表达式到目标类型的强制类型转换的表达式。如果不能进行强制类型转换，则为 <c>null</c>。
		/// </summary>
		/// <param name="exp">要引用的表达式。</param>
		/// <param name="expType">要强制类型转换到的目标类型。</param>
		/// <returns>对指定表达式到目标类型的强制类型转换的表达式。</returns>
		public static Expression ConvertType(this Expression exp, Type expType)
		{
			// 对于可隐式类型转换和 ref 参数，不进行类型转换。
			if (exp.Type == expType || expType.IsAssignableFrom(exp.Type) || expType.IsByRef)
			{
				return exp;
			}
			try
			{
				// 需要强制转换。
				return Expression.Convert(exp, expType);
			}
			catch (InvalidOperationException)
			{
				// 不允许进行强制类型转换。
				return null;
			}
		}
	}
}
