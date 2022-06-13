using Cyjb.Reflection;

namespace Cyjb;

/// <summary>
/// 提供 <see cref="Delegate"/> 类的扩展方法。
/// </summary>
public static class DelegateUtil
{
	/// <summary>
	/// 将指定的委托包装为指定类型，支持对参数进行强制类型转换。
	/// </summary>
	/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
	/// <param name="dlg">要被包装的委托。</param>
	/// <returns>指定类型的委托，其包装了 <paramref name="dlg"/>。
	/// 如果参数个数不同，或者参数间不能执行强制类型转换，则为 <c>null</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="dlg"/> 为 <c>null</c>。</exception>
	/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
	/// <overloads>
	/// <summary>
	/// 将指定的委托包装为指定类型，支持对参数进行强制类型转换。
	/// </summary>
	/// </overloads>
	public static TDelegate? Wrap<TDelegate>(this Delegate dlg)
		where TDelegate : Delegate
	{
		ArgumentNullException.ThrowIfNull(dlg);
		if (dlg.GetType() == typeof(TDelegate))
		{
			return dlg as TDelegate;
		}
		if (dlg.Target == null)
		{
			return dlg.Method.PowerDelegate<TDelegate>();
		}
		else
		{
			return dlg.Method.PowerDelegate<TDelegate>(dlg.Target);
		}
	}

	/// <summary>
	/// 将指定的委托包装为指定类型，支持对参数进行强制类型转换。
	/// </summary>
	/// <param name="dlg">要被包装的委托。</param>
	/// <param name="delegateType">要包装为的委托的类型。</param>
	/// <returns>指定类型的委托，其包装了 <paramref name="dlg"/>。
	/// 如果参数个数不同，或者参数间不能执行强制类型转换，则为 <c>null</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="dlg"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
	/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
	public static Delegate? Wrap(this Delegate dlg, Type delegateType)
	{
		ArgumentNullException.ThrowIfNull(dlg);
		ArgumentNullException.ThrowIfNull(delegateType);
		CommonExceptions.CheckDelegateType(delegateType);
		if (dlg.GetType() == delegateType)
		{
			return dlg;
		}
		if (dlg.Target == null)
		{
			return dlg.Method.PowerDelegate(delegateType);
		}
		else
		{
			return dlg.Method.PowerDelegate(delegateType, dlg.Target);
		}
	}
}
