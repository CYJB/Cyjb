using System.Diagnostics;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 提供 <see cref="IList{T}"/> 的只读包装。
/// </summary>
/// <typeparam name="T">列表中元素的类型。</typeparam>
internal sealed class ReadOnlyListWrap<T> : ReadOnlyListBase<T>
{
	/// <summary>
	/// 被包装的列表。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly IList<T> list;

	/// <summary>
	/// 使用指定的列表初始化 <see cref="ReadOnlyListWrap{T}"/> 类的新实例。
	/// </summary>
	/// <param name="list">要被包装的列表。</param>
	public ReadOnlyListWrap(IList<T> list)
	{
		this.list = list;
	}

	#region ReadOnlyListBase<T> 成员

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public override int Count => list.Count;

	/// <summary>
	/// 返回指定索引处的元素。
	/// </summary>
	/// <param name="index">要返回元素的从零开始的索引。</param>
	/// <returns>位于指定索引处的元素。</returns>
	protected override T GetItemAt(int index)
	{
		return list[index];
	}

	/// <summary>
	/// 确定当前列表中指定对象的索引。
	/// </summary>
	/// <param name="item">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="item"/>，则为该对象的索引；否则为 <c>-1</c>。</returns>
	public override int IndexOf(T item)
	{
		return list.IndexOf(item);
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<T> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	#endregion // ReadOnlyListBase<T> 成员

}
