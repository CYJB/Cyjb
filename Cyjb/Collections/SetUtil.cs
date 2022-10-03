namespace Cyjb.Collections;

/// <summary>
/// 提供对 <see cref="ISet{T}"/> 的扩展方法。
/// </summary>
public static class SetUtil
{
	/// <summary>
	/// 返回当前集合的只读包装。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	/// <param name="set">要被包装的集合。</param>
	/// <returns>当前集合的只读包装。</returns>
	public static IReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
	{
		return new ReadOnlySetWrap<T>(set);
	}
}
