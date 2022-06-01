using System.Collections;
using System.Diagnostics;

namespace Cyjb.Collections.ObjectModel;

/// <summary>
/// 为泛型列表提供基类。
/// </summary>
/// <typeparam name="T">列表中的元素类型。</typeparam>
public abstract class ListBase<T> : CollectionBase<T>, IList<T>, IReadOnlyList<T>, IList
{
	/// <summary>
	/// 初始化 <see cref="ListBase{T}"/> 类的新实例。
	/// </summary>
	protected ListBase() : base() { }

	#region ListBase<T> 成员

	/// <summary>
	/// 将元素插入 <see cref="ListBase{T}"/> 的指定索引处。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
	/// <param name="item">要插入的对象。</param>
	protected abstract void InsertItem(int index, T item);

	/// <summary>
	/// 移除 <see cref="ListBase{T}"/> 的指定索引处的元素。
	/// </summary>
	/// <param name="index">要移除的元素的从零开始的索引。</param>
	protected abstract void RemoveItem(int index);

	/// <summary>
	/// 返回指定索引处的元素。
	/// </summary>
	/// <param name="index">要返回元素的从零开始的索引。</param>
	/// <returns>位于指定索引处的元素。</returns>
	protected abstract T GetItemAt(int index);

	/// <summary>
	/// 替换指定索引处的元素。
	/// </summary>
	/// <param name="index">待替换元素的从零开始的索引。</param>
	/// <param name="item">位于指定索引处的元素的新值。</param>
	protected abstract void SetItemAt(int index, T item);

	#endregion // ListBase<T> 成员

	#region CollectionBase<T> 成员

	/// <summary>
	/// 将指定对象添加到当前集合中。
	/// </summary>
	/// <param name="item">要添加到当前集合的对象。</param>
	protected override void AddItem(T item)
	{
		InsertItem(Count, item);
	}

	#endregion // CollectionBase<T> 成员

	#region IList<T> 成员

	/// <summary>
	/// 获取或设置指定索引处的元素。
	/// </summary>
	/// <param name="index">要获取或设置的元素从零开始的索引。</param>
	/// <value>指定索引处的元素。</value>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表的有效索引。</exception>
	public T this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			return GetItemAt(index);
		}
		set
		{
			if (index < 0 || index >= Count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			SetItemAt(index, value);
		}
	}

	/// <summary>
	/// 确定当前列表中指定对象的索引。
	/// </summary>
	/// <param name="item">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="item"/>，则为该对象的索引；否则为 <c>-1</c>。</returns>
	public abstract int IndexOf(T item);

	/// <summary>
	/// 将元素插入当前列表的指定索引处。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
	/// <param name="item">要插入到当前列表中的对象。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表中的有效索引。</exception>
	public void Insert(int index, T item)
	{
		if (index < 0 || index > Count)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		InsertItem(index, item);
	}

	/// <summary>
	/// 移除当前列表中指定索引处的元素。
	/// </summary>
	/// <param name="index">要移除的元素的从零开始的索引。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表的有效索引。</exception>
	public void RemoveAt(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		RemoveItem(index);
	}

	#endregion // IList<T> 成员

	#region IList 成员

	/// <summary>
	/// 获取一个值，该值指示当前列表是否具有固定大小。
	/// </summary>
	/// <value>如果当前列表具有固定大小，则为 <c>true</c>；否则为 <c>false</c>。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool IList.IsFixedSize
	{
		get { return false; }
	}

	/// <summary>
	/// 获取一个值，该值指示当前列表是否为只读。
	/// </summary>
	/// <value>如果当前列表为只读，则为 <c>true</c>；否则为 <c>false</c>。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool IList.IsReadOnly
	{
		get { return false; }
	}

	/// <summary>
	/// 获取或设置指定索引处的元素。
	/// </summary>
	/// <param name="index">要获得或设置的元素从零开始的索引。</param>
	/// <value>指定索引处的元素。</value>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表中的有效索引。</exception>
	/// <exception cref="ArgumentException">设置属性，且 <paramref name="value"/> 不能赋值给 <typeparamref name="T"/> 类型。</exception>
	object? IList.this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			return GetItemAt(index);
		}
		set
		{
			if (index < 0 || index >= Count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			try
			{
				// 这里无法检查 T 是否是不可空的 null 类型，只能忽略 null 相关检查。
				SetItemAt(index, (T)value!);
			}
			catch (InvalidCastException)
			{
				throw CommonExceptions.ArgumentWrongType(value, typeof(T));
			}
		}
	}

	/// <summary>
	/// 将指定对象添加到当前列表的末尾。
	/// </summary>
	/// <param name="value">要添加到当前列表末尾的对象。</param>
	/// <returns>新元素所插入到的位置，或为 <c>-1</c> 以指示未将该项插入到集合中。</returns>
	/// <exception cref="ArgumentException"><paramref name="value"/> 不能赋值给 <typeparamref name="T"/> 类型。</exception>
	int IList.Add(object? value)
	{
		try
		{
			int idx = Count;
			// 这里无法检查 T 是否是不可空的 null 类型，只能忽略 null 相关检查。
			InsertItem(idx, (T)value!);
			return idx;
		}
		catch (InvalidCastException)
		{
			throw CommonExceptions.ArgumentWrongType(value, typeof(T));
		}
	}

	/// <summary>
	/// 确定当前列表是否包含指定对象。
	/// </summary>
	/// <param name="value">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="value"/>，
	/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
	bool IList.Contains(object? value)
	{
		// 这里无法检查 T 是否是不可空的 null 类型，只能忽略 null 相关检查。
		return CollectionHelper.IsCompatible<T>(value) && IndexOf((T)value!) >= 0;
	}

	/// <summary>
	/// 确定当前列表中指定对象的索引。
	/// </summary>
	/// <param name="value">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="value"/>，则为该对象的索引；否则为 <c>-1</c>。</returns>
	int IList.IndexOf(object? value)
	{
		if (CollectionHelper.IsCompatible<T>(value))
		{
			// 这里无法检查 T 是否是不可空的 null 类型，只能忽略 null 相关检查。
			return IndexOf((T)value!);
		}
		return -1;
	}

	/// <summary>
	/// 将对象插入当前列表的指定索引处。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="value"/>。</param>
	/// <param name="value">要插入到当前列表中的对象。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表中的有效索引。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 不能赋值给 <typeparamref name="T"/> 类型。</exception>
	void IList.Insert(int index, object? value)
	{
		if (index < 0 || index > Count)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		try
		{
			// 这里无法检查 T 是否是不可空的 null 类型，只能忽略 null 相关检查。
			InsertItem(index, (T)value!);
		}
		catch (InvalidCastException)
		{
			throw CommonExceptions.ArgumentWrongType(value, typeof(T));
		}
	}

	/// <summary>
	/// 从当前列表中移除特定对象的第一个匹配项。
	/// </summary>
	/// <param name="value">要从当前列表中移除的对象。</param>
	void IList.Remove(object? value)
	{
		if (CollectionHelper.IsCompatible<T>(value))
		{
			// 这里无法检查 T 是否是不可空的 null 类型，只能忽略 null 相关检查。
			int idx = IndexOf((T)value!);
			if (idx >= 0)
			{
				RemoveItem(idx);
			}
		}
	}

	#endregion // IList 成员

	#region ICollection<T> 成员

	/// <summary>
	/// 确定当前列表是否包含指定对象。
	/// </summary>
	/// <param name="item">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	/// <summary>
	/// 从当前列表中移除特定对象的第一个匹配项。
	/// </summary>
	/// <param name="item">要从当前列表中移除的对象。</param>
	/// <returns>如果已从当前列表中成功移除 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。
	/// 如果在原始列表中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
	public override bool Remove(T item)
	{
		int idx = IndexOf(item);
		if (idx < 0)
		{
			return false;
		}
		RemoveItem(idx);
		return true;
	}

	#endregion // ICollection<T> 成员

}
