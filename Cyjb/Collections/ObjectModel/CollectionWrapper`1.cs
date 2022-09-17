using System.Collections;

namespace Cyjb.Collections.ObjectModel;

/// <summary>
/// 支持将 <see cref="ICollection{T}"/> 包装为 <see cref="ICollection"/>。
/// </summary>
internal class CollectionWrapper<T> : CollectionBase<T>
{
	/// <summary>
	/// 被包装的集合。
	/// </summary>
	private readonly ICollection<T> collection;
	/// <summary>
	/// 使用指定的 <see cref="ICollection{T}"/> 初始化 <see cref="CollectionWrapper{T}"/> 类的新实例。
	/// </summary>
	/// <param name="collection">要被包装的集合。</param>
	public CollectionWrapper(ICollection<T> collection)
	{
		this.collection = collection;
	}

	#region CollectionBase<T> 成员

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
	/// 将指定对象添加到当前集合中。
	/// </summary>
	/// <param name="item">要添加到当前集合的对象。</param>
	protected override void AddItem(T item)
	{
		collection.Add(item);
		throw new NotImplementedException();
	}

	/// <summary>
	/// 从当前集合中移除特定对象的第一个匹配项。
	/// </summary>
	/// <param name="item">要从当前集合中移除的对象。</param>
	/// <returns>如果已从当前集合中成功移除 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。
	/// 如果在原始当前集合中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
	public override bool Remove(T item)
	{
		return collection.Remove(item);
	}

	/// <summary>
	/// 从当前集合中移除所有元素。
	/// </summary>
	public override void Clear()
	{
		collection.Clear();
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<T> GetEnumerator()
	{
		return collection.GetEnumerator();
	}

	#endregion // CollectionBase<T> 成员

}
