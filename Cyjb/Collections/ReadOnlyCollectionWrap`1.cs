using System.Diagnostics;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 提供 <see cref="ICollection{T}"/> 的只读包装。
/// </summary>
/// <typeparam name="T">列表中元素的类型。</typeparam>
internal sealed class ReadOnlyCollectionWrap<T> : ReadOnlyCollectionBase<T>
{
	/// <summary>
	/// 被包装的集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly ICollection<T> collection;

	/// <summary>
	/// 使用指定的集合初始化 <see cref="ReadOnlyListWrap{T}"/> 类的新实例。
	/// </summary>
	/// <param name="collection">要被包装的集合。</param>
	public ReadOnlyCollectionWrap(ICollection<T> collection)
	{
		this.collection = collection;
	}

	#region ReadOnlyCollectionBase<T> 成员

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public override int Count => collection.Count;

	/// <summary>
	/// 确定当前集合是否包含指定对象。
	/// </summary>
	/// <param name="item">要在当前集合中定位的对象。</param>
	/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool Contains(T item)
	{
		return collection.Contains(item);
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<T> GetEnumerator()
	{
		return collection.GetEnumerator();
	}

	#endregion // ReadOnlyCollectionBase<T> 成员

}
