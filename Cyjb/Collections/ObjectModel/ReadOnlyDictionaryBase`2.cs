using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cyjb.Collections.ObjectModel;

/// <summary>
/// 为泛型只读字典提供基类。
/// </summary>
/// <typeparam name="TKey">字典中的键的类型。</typeparam>
/// <typeparam name="TValue">字典中的值的类型。</typeparam>
[Serializable, DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
public abstract class ReadOnlyDictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>,
	IReadOnlyDictionary<TKey, TValue>, IDictionary, ICollection<KeyValuePair<TKey, TValue>>
	where TKey : notnull
{
	/// <summary>
	/// 用于同步集合访问的对象。
	/// </summary>
	[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object? syncRoot;
	/// <summary>
	/// 键的集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ICollection? keyCollection;
	/// <summary>
	/// 值的集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ICollection? valueCollection;

	/// <summary>
	/// 初始化 <see cref="ReadOnlyDictionaryBase{TKey, TValue}"/> 类的新实例。
	/// </summary>
	protected ReadOnlyDictionaryBase()
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
	/// 获取具有指定键的元素。
	/// </summary>
	/// <param name="key">要获取的元素的键。</param>
	/// <returns>指定键对应的值。</returns>
	protected abstract TValue GetItem(TKey key);

	#region IDictionary<TKey, TValue> 成员

	/// <summary>
	/// 获取或设置具有指定键的元素。
	/// </summary>
	/// <param name="key">要获取或设置的元素的键。</param>
	/// <returns>指定键对应的值。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	/// <exception cref="KeyNotFoundException">未找到指定的 <paramref name="key"/>。</exception>
	/// <exception cref="NotSupportedException">设置时总是引发。</exception>
	public TValue this[TKey key] { get => GetItem(key); set => throw CommonExceptions.MethodNotSupported(); }

	/// <summary>
	/// 获取包含当前字典的键的 <see cref="ICollection{TKey}"/>。
	/// </summary>
	public abstract ICollection<TKey> Keys { get; }

	/// <summary>
	/// 获取包含当前字典的值的 <see cref="ICollection{TValue}"/>。
	/// </summary>
	public abstract ICollection<TValue> Values { get; }

	/// <summary>
	/// 向当前字典添加一个带有所提供的键和值的元素。
	/// </summary>
	/// <param name="key">要添加的元素的键。</param>
	/// <param name="value">要添加的元素的值。</param>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
	{
		throw CommonExceptions.MethodNotSupported();
	}

	/// <summary>
	/// 确定当前字典是否包含带有指定键的元素。
	/// </summary>
	/// <param name="key">要在当前字典中定位的键。</param>
	/// <returns>如果当前字典包含包含具有键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	public abstract bool ContainsKey(TKey key);

	/// <summary>
	/// 从当字典中移除包含指定键的元素。
	/// </summary>
	/// <param name="key">要移除的元素的键。</param>
	/// <returns>如果成功移除该元素，则为 <c>true</c>；否则为 <c>false</c>。
	/// 如果在原始字典中没有找到指定键，也会返回 <c>false</c>。</returns>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	bool IDictionary<TKey, TValue>.Remove(TKey key)
	{
		throw CommonExceptions.MethodNotSupported();
	}

	/// <summary>
	/// 获取与指定键关联的值。
	/// </summary>
	/// <param name="key">要获取其值的键。</param>
	/// <param name="value">如果找到指定键，则返回与该键相关联的值；
	/// 否则返回 <paramref name="value"/> 参数的类型的默认值。</param>
	/// <returns>如果当前字典包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	public abstract bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

	#endregion // IDictionary<TKey, TValue> 成员

	#region IReadOnlyDictionary<TKey, TValue>

	/// <summary>
	/// 获取包含当前字典中的键的可枚举集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	/// <summary>
	/// 获取包含当前字典中的值的可枚举集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

	#endregion // IReadOnlyDictionary<TKey, TValue>

	#region IDictionary 成员

	/// <summary>
	/// 获取当前字典是否具有固定大小。
	/// </summary>
	/// <value>如果当前字典具有固定大小，则为 <c>true</c>；否则为 <c>false</c>。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool IDictionary.IsFixedSize => false;

	/// <summary>
	/// 获取一个值，该值指示当前字典是否为只读。
	/// </summary>
	/// <value>如果当前字典是只读的，则为 <c>true</c>；否则为 <c>false</c>。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool IDictionary.IsReadOnly => true;

	/// <summary>
	/// 获取包含当前字典的键的 <see cref="ICollection"/>。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	ICollection IDictionary.Keys
	{
		get
		{
			if (keyCollection == null)
			{
				if (Keys is ICollection collection)
				{
					keyCollection = collection;
				}
				else
				{
					keyCollection = new CollectionWrapper<TKey>(Keys);
				}
			}
			return keyCollection;
		}
	}

	/// <summary>
	/// 获取包含当前字典的值的 <see cref="ICollection"/>。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	ICollection IDictionary.Values
	{
		get
		{
			if (valueCollection == null)
			{
				if (Values is ICollection collection)
				{
					valueCollection = collection;
				}
				else
				{
					valueCollection = new CollectionWrapper<TValue>(Values);
				}
			}
			return valueCollection;
		}
	}

	/// <summary>
	/// 获取或设置具有指定键的元素。
	/// </summary>
	/// <param name="key">要获取或设置的元素的键。</param>
	/// <returns>指定键对应的值。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	/// <exception cref="KeyNotFoundException">未找到指定的 <paramref name="key"/>。</exception>
	/// <exception cref="NotSupportedException">设置时总是引发。</exception>
	object? IDictionary.this[object key]
	{
		get
		{
			if (CollectionHelper.IsCompatible<TKey>(key))
			{
				return GetItem((TKey)key);
			}
			return null;
		}
		set
		{
			throw CommonExceptions.MethodNotSupported();
		}
	}

	/// <summary>
	/// 向当前字典添加一个带有所提供的键和值的元素。
	/// </summary>
	/// <param name="key">要添加的元素的键。</param>
	/// <param name="value">要添加的元素的值。</param>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	void IDictionary.Add(object key, object? value)
	{
		throw CommonExceptions.MethodNotSupported();
	}

	/// <summary>
	/// 从当前字典中移除所有元素。
	/// </summary>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	void IDictionary.Clear()
	{
		throw CommonExceptions.MethodNotSupported();
	}

	/// <summary>
	/// 确定当前字典是否包含带有指定键的元素。
	/// </summary>
	/// <param name="key">要在当前字典中定位的键。</param>
	/// <returns>如果当前字典包含包含具有键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	bool IDictionary.Contains(object key)
	{
		if (CollectionHelper.IsCompatible<TKey>(key))
		{
			return ContainsKey((TKey)key);
		}
		return false;
	}

	/// <summary>
	/// 返回当前字典的 <see cref="IDictionaryEnumerator"/> 对象。
	/// </summary>
	/// <returns>当前字典的 <see cref="IDictionaryEnumerator"/> 对象。</returns>
	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return new DictionaryEnumerator<TKey, TValue>(GetEnumerator());
	}

	/// <summary>
	/// 从当字典中移除包含指定键的元素。
	/// </summary>
	/// <param name="key">要移除的元素的键。</param>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	void IDictionary.Remove(object key)
	{
		throw CommonExceptions.MethodNotSupported();
	}

	#endregion // IDictionary 成员

	#region ICollection<KeyValuePair<TKey, TValue>> 成员

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
	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

	/// <summary>
	/// 将指定对象添加到当前字典。
	/// </summary>
	/// <param name="keyValuePair">要添加到当前字典的键值对。</param>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
	{
		throw CommonExceptions.MethodNotSupported();
	}

	/// <summary>
	/// 从当前集合中移除所有元素。
	/// </summary>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		throw CommonExceptions.MethodNotSupported();
	}

	/// <summary>
	/// 确定当前字典是否包含指定的键值对。
	/// </summary>
	/// <param name="keyValuePair">要在当前字典中定位的键值对。</param>
	/// <returns>如果在当前字典中找到 <paramref name="keyValuePair"/>，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
	{
		if (TryGetValue(keyValuePair.Key, out TValue? value))
		{
			return EqualityComparer<TValue>.Default.Equals(keyValuePair.Value, value);
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 从特定的 <see cref="Array"/> 索引处开始，将当前字典
	/// 的元素复制到一个 <see cref="Array"/> 中。
	/// </summary>
	/// <param name="array">从当前字典复制的元素的目标位置的一维 
	/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
	/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
	/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 小于零。</exception>
	/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
	/// <exception cref="ArgumentException"><see cref="CollectionBase{T}"/> 
	/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
	/// 末尾之间的可用空间。</exception>
	/// <exception cref="ArgumentException">当前字典
	/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		CollectionHelper.CopyTo(this, array, arrayIndex);
	}

	/// <summary>
	/// 从当前字典中移除指定的键值对。
	/// </summary>
	/// <param name="keyValuePair">要从当前字典中移除的键值对。</param>
	/// <returns>如果已从当前字典中成功移除 <paramref name="keyValuePair"/>，则为 <c>true</c>；
	/// 否则为 <c>false</c>。
	/// 如果在当前字典中没有找到 <paramref name="keyValuePair"/>，该方法也会返回 <c>false</c>。</returns>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
	{
		throw CommonExceptions.MethodNotSupported();
	}

	#endregion // ICollection<KeyValuePair<TKey, TValue>> 成员

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
	public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

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
