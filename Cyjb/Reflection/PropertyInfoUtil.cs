using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="PropertyInfo"/> 的扩展方法。
	/// </summary>
	public static partial class PropertyInfoUtil
	{
		/// <summary>
		/// 返回当前属性的索引参数类型列表。
		/// </summary>
		/// <param name="property">要获取索引参数类型列表的属性。</param>
		/// <returns>属性的参数类型列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public static Type[] GetIndexParameterTypes(this PropertyInfo property)
		{
			ArgumentNullException.ThrowIfNull(property);
			ParameterInfo[] paramInfos = property.GetIndexParameters();
			if (paramInfos.Length == 0)
			{
				return Type.EmptyTypes;
			}
			Type[] types = new Type[paramInfos.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = paramInfos[i].ParameterType;
			}
			return types;
		}

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
			ArgumentNullException.ThrowIfNull(property);
			MethodInfo? method = property.GetGetMethod(true) ?? property.GetSetMethod(true);
			if (method == null)
			{
				// 不存在 get/set 方法。
				return property;
			}
			MethodInfo baseMethod = method.GetBaseDefinition();
			if (baseMethod == method)
			{
				return property;
			}
			// 找到方法对应的属性，这时候 method 应当不是 global member。
			Type baseType = baseMethod.DeclaringType!;
			PropertyInfo? baseProperty = baseType.GetProperty(property.Name,
				BindingFlagsUtil.AllMember | BindingFlags.ExactBinding,
				null, property.PropertyType, property.GetIndexParameterTypes(), null);
			return baseProperty ?? property;
		}
	}
}
