using System.Diagnostics;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 提供 <see cref="ISet{T}"/> 的只读包装。
/// </summary>
/// <typeparam name="T">集合中元素的类型。</typeparam>
internal sealed class ReadOnlySetWrap<T> : ReadOnlySetBase<T>
{
	/// <summary>
	/// 被包装的集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly ISet<T> set;

	/// <summary>
	/// 使用指定的集合初始化 <see cref="ReadOnlySetWrap{T}"/> 类的新实例。
	/// </summary>
	/// <param name="set">要被包装的集合。</param>
	public ReadOnlySetWrap(ISet<T> set)
	{
		this.set = set;
	}

	#region ReadOnlySetBase<T> 成员

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public override int Count => set.Count;

	/// <summary>
	/// 确定当前集合是否包含指定对象。
	/// </summary>
	/// <param name="item">要在当前集合中定位的对象。</param>
	/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool Contains(T item)
	{
		return set.Contains(item);
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<T> GetEnumerator()
	{
		return set.GetEnumerator();
	}

	#endregion // ReadOnlySetBase<T> 成员

	#region ISet<T> 成员

	/// <summary>
	/// 确定当前集合是否为指定集合的真子集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的真子集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return set.IsProperSubsetOf(other);
	}

	/// <summary>
	/// 确定当前集合是否为指定集合的真超集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的真超集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return set.IsProperSupersetOf(other);
	}

	/// <summary>
	/// 确定当前集合是否为指定集合的子集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的子集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsSubsetOf(IEnumerable<T> other)
	{
		return set.IsSubsetOf(other);
	}

	/// <summary>
	/// 确定当前集合是否为指定集合的超集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的超集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsSupersetOf(IEnumerable<T> other)
	{
		return set.IsSupersetOf(other);
	}

	/// <summary>
	/// 确定当前集合是否与指定的集合重叠。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合与 <paramref name="other"/> 
	/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool Overlaps(IEnumerable<T> other)
	{
		return set.Overlaps(other);
	}

	/// <summary>
	/// 确定当前集合与指定的集合中是否包含相同的元素。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合等于 <paramref name="other"/>，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool SetEquals(IEnumerable<T> other)
	{
		return set.SetEquals(other);
	}

	#endregion

}
