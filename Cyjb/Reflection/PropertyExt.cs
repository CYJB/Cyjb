using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="PropertyInfo"/> 及其子类的扩展方法。
	/// </summary>
	public static class PropertyExt
	{

		#region 属性参数

		/// <summary>
		/// 返回当前属性的索引参数类型列表。
		/// </summary>
		/// <param name="property">要获取索引参数类型列表的属性。</param>
		/// <returns>属性的参数类型列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public static Type[] GetParameterTypes(this PropertyInfo property)
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.Ensures(Contract.Result<Type[]>() != null);
			ParameterInfo[] parameters = property.GetIndexParameters();
			if (parameters.Length == 0)
			{
				return Type.EmptyTypes;
			}
			Type[] types = new Type[parameters.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}

		#endregion // 属性参数

	}
}
