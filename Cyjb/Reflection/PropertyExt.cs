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
		public static Type[] GetIndexParameterTypes(this PropertyInfo property)
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

		#region 基定义

		/// <summary>
		/// 返回在派生类中重写的属性，在其直接或间接的基类中首先声明的 <see cref="PropertyInfo"/>。
		/// </summary>
		/// <param name="property">要检查的属性。</param>
		/// <returns>当前属性的第一次实现。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <remarks>
		/// <para>若当前属性是在接口上声明的，则 <see cref="GetBaseDefinition"/> 返回当前属性。</para>
		/// <para>若当前属性是在基类中声明的，则 <see cref="GetBaseDefinition"/> 将以下列方式运行：
		/// <list type="bubble">
		/// <item><description>如果当前属性重写基类中的虚拟定义，则返回该虚拟定义。</description></item>
		/// <item><description>如果当前属性是用 <c>new</c> 关键字指定的，则返回当前属性。</description></item>
		/// <item><description>如果当前属性不是在调用 <see cref="GetBaseDefinition"/> 的对象中定义的，
		/// 则返回类结构层次中最高一级的属性定义。</description></item>
		/// </list></para></remarks>
		/// <seealso cref="MethodInfo.GetBaseDefinition"/>
		public static PropertyInfo GetBaseDefinition(this PropertyInfo property)
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.Ensures(Contract.Result<PropertyInfo>() != null);
			MethodInfo method;
			if (property.CanRead)
			{
				method = property.GetGetMethod(true);
			}
			else if (property.CanWrite)
			{
				method = property.GetSetMethod(true);
			}
			else
			{
				return property;
			}
			MethodInfo baseMethod = method.GetBaseDefinition();
			if (baseMethod == method)
			{
				return property;
			}
			// 找到方法对应的属性。
			Type baseType = method.DeclaringType;
			Contract.Assume(baseType != null);
			PropertyInfo baseProperty = baseType.GetProperty(property.Name, TypeExt.AllMemberFlag | BindingFlags.ExactBinding,
				null, property.PropertyType, property.GetIndexParameterTypes(), null);
			return baseProperty ?? property;
		}

		#endregion // 基定义

	}
}
