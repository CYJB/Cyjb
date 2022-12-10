using Cyjb.Collections.ObjectModel;

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

	#region Empty

	/// <summary>
	/// 返回一个空的只读集合。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	/// <returns>空的只读集合。</returns>
	public static IReadOnlySet<T> Empty<T>()
	{
		return EmptySet<T>.Instance;
	}

	/// <summary>
	/// 空的只读集合。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	private class EmptySet<T> : ReadOnlySetBase<T>
	{
		/// <summary>
		/// 空的只读集合实例。
		/// </summary>
		public static readonly EmptySet<T> Instance = new();
		/// <summary>
		/// 获取当前集合包含的元素数。
		/// </summary>
		/// <value>当前集合中包含的元素数。</value>
		public override int Count => 0;

		/// <summary>
		/// 确定当前集合是否包含指定对象。
		/// </summary>
		/// <param name="item">要在当前集合中定位的对象。</param>
		/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(T item)
		{
			return false;
		}

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public override IEnumerator<T> GetEnumerator()
		{
			yield break;
		}
	}

	#endregion // Empty

}
