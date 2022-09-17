using System.Collections;
using System.Diagnostics;

namespace Cyjb.Collections.ObjectModel;

/// <summary>
/// 为泛型集合提供基类。
/// </summary>
/// <typeparam name="T">集合中元素的类型。</typeparam>
[Serializable, DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(EnumerableDebugView<>))]
public abstract class CollectionBase<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
{
	/// <summary>
	/// 用于同步集合访问的对象。
	/// </summary>
	[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object? syncRoot;
	/// <summary>
	/// 当前集合是否是只读的。
	/// </summary>
	private bool isReadOnly = false;

	/// <summary>
	/// 初始化 <see cref="CollectionBase{T}"/> 类的新实例。
	/// </summary>
	protected CollectionBase()
	{ }

	/// <summary>
	/// 获取一个可用于同步对当前集合的访问的对象。
	/// </summary>
	/// <returns>可用于同步对当前集合的访问的对象。</returns>
	protected virtual object GetSyncRoot()
	{
		if (syncRoot == null)
		{
			Interlocked.CompareExchange(ref syncRoot, new object(), null);
		}
		return syncRoot;
	}

	/// <summary>
	/// 将当前集合设置为只读的。
	/// </summary>
	protected void SetCollectionReadOnly()
	{
		isReadOnly = true;
	}

	/// <summary>
	/// 检查当前集合是否是只读的。
	/// </summary>
	protected void CheckIsReadOnly()
	{
		if (isReadOnly)
		{
			throw CommonExceptions.CollectionReadOnly();
		}
	}

	#region CollectionBase<T> 成员

	/// <summary>
	/// 将指定对象添加到当前集合中。
	/// </summary>
	/// <param name="item">要添加到当前集合的对象。</param>
	protected abstract void AddItem(T item);

	#endregion // CollectionBase<T> 成员

	#region ICollection<T> 成员

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public abstract int Count { get; }

	/// <summary>
	/// 获取一个值，该值指示当前集合是否为只读。
	/// </summary>
	/// <value>如果当前集合是只读的，则为 <c>true</c>；否则为 <c>false</c>。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool ICollection<T>.IsReadOnly => isReadOnly;

	/// <summary>
	/// 将指定对象添加到当前集合中。
	/// </summary>
	/// <param name="item">要添加到当前集合的对象。</param>
	public void Add(T item)
	{
		AddItem(item);
	}

	/// <summary>
	/// 从当前集合中移除所有元素。
	/// </summary>
	public abstract void Clear();

	/// <summary>
	/// 确定当前集合是否包含指定对象。
	/// </summary>
	/// <param name="item">要在当前集合中定位的对象。</param>
	/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public abstract bool Contains(T item);

	/// <summary>
	/// 从特定的 <see cref="Array"/> 索引处开始，将当前集合
	/// 的元素复制到一个 <see cref="Array"/> 中。
	/// </summary>
	/// <param name="array">从当前集合复制的元素的目标位置的一维 
	/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
	/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
	/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 小于零。</exception>
	/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
	/// <exception cref="ArgumentException"><see cref="CollectionBase{T}"/> 
	/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
	/// 末尾之间的可用空间。</exception>
	/// <exception cref="ArgumentException">当前集合
	/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
	public void CopyTo(T[] array, int arrayIndex)
	{
		CollectionHelper.CopyTo(this, array, arrayIndex);
	}

	/// <summary>
	/// 从当前集合中移除特定对象的第一个匹配项。
	/// </summary>
	/// <param name="item">要从当前集合中移除的对象。</param>
	/// <returns>如果已从当前集合中成功移除 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。
	/// 如果在当前集合中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
	public abstract bool Remove(T item);

	#endregion // ICollection<T> 成员

	#region ICollection 成员

	/// <summary>
	/// 获取一个值，该值指示是否同步对当前集合的访问（线程安全）。
	/// </summary>
	/// <value>如果对当前集合的访问是同步的（线程安全），则为 <c>true</c>；否则为 <c>false</c>。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool ICollection.IsSynchronized => false;

	/// <summary>
	/// 获取一个可用于同步对当前集合的访问的对象。
	/// </summary>
	/// <value>可用于同步对当前集合的访问的对象。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	object ICollection.SyncRoot => GetSyncRoot();

	/// <summary>
	/// 从特定的 <see cref="Array"/> 索引处开始，将当前集合的元素复制到一个 <see cref="Array"/> 中。
	/// </summary>
	/// <param name="array">从当前集合复制的元素的目标位置的一维 <see cref="Array"/>。
	/// <paramref name="array"/> 必须具有从零开始的索引。</param>
	/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
	/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
	/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
	/// <exception cref="ArgumentException">当前集合中的元素数目大于从 <paramref name="index"/>
	/// 到目标 <paramref name="array"/> 末尾之间的可用空间。</exception>
	/// <exception cref="ArgumentException">当前集合的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
	void ICollection.CopyTo(Array array, int index)
	{
		CollectionHelper.CopyTo(this, array, index);
	}

	#endregion // ICollection 成员

	#region IEnumerable<T> 成员

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public abstract IEnumerator<T> GetEnumerator();

	#endregion // IEnumerable<T> 成员

	#region IEnumerable 成员

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	#endregion // IEnumerable 成员

}
