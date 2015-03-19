using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="EventInfo"/> 及其子类的扩展方法。
	/// </summary>
	public static class EventExt
	{

		#region 基定义

		/// <summary>
		/// 返回在派生类中重写的事件，在其直接或间接的基类中首先声明的 <see cref="EventInfo"/>。
		/// </summary>
		/// <param name="evn">要检查的事件。</param>
		/// <returns>当前事件的第一次实现。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="evn"/> 为 <c>null</c>。</exception>
		/// <remarks>
		/// <para>若当前事件是在接口上声明的，则 <see cref="GetBaseDefinition"/> 返回当前事件。</para>
		/// <para>若当前事件是在基类中声明的，则 <see cref="GetBaseDefinition"/> 将以下列方式运行：
		/// <list type="bubble">
		/// <item><description>如果当前事件重写基类中的虚拟定义，则返回该虚拟定义。</description></item>
		/// <item><description>如果当前事件是用 <c>new</c> 关键字指定的，则返回当前事件。</description></item>
		/// <item><description>如果当前事件不是在调用 <see cref="GetBaseDefinition"/> 的对象中定义的，
		/// 则返回类结构层次中最高一级的事件定义。</description></item>
		/// </list></para></remarks>
		/// <seealso cref="MethodInfo.GetBaseDefinition"/>
		public static EventInfo GetBaseDefinition(this EventInfo evn)
		{
			CommonExceptions.CheckArgumentNull(evn, "evn");
			Contract.Ensures(Contract.Result<EventInfo>() != null);
			MethodInfo method = evn.GetAddMethod(true);
			MethodInfo baseMethod = method.GetBaseDefinition();
			if (baseMethod == method)
			{
				return evn;
			}
			// 找到方法对应的事件。
			Type baseType = method.DeclaringType;
			Contract.Assume(baseType != null);
			EventInfo baseEvent = baseType.GetEvent(evn.Name, TypeExt.AllMemberFlag | BindingFlags.ExactBinding);
			return baseEvent ?? evn;
		}

		#endregion // 基定义

	}
}
