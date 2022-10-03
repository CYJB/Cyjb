using System.Collections;

namespace Cyjb.Collections.ObjectModel;

/// <summary>
/// 提供 <see cref="IDictionaryEnumerator"/> 的包装。
/// </summary>
internal class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator
{
	/// <summary>
	/// 被包装的迭代器。
	/// </summary>
	private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

	/// <summary>
	/// 使用指定的迭代器初始化 <see cref="DictionaryEnumerator{TKey, TValue}"/> 类的新实例。
	/// </summary>
	/// <param name="enumerator">被包装的迭代器。</param>
	public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
	{
		this.enumerator = enumerator;
	}

	/// <summary>
	/// 同时获取当前字典项的键和值。
	/// </summary>
	public DictionaryEntry Entry => new(enumerator.Current.Key!, enumerator.Current.Value);

	/// <summary>
	/// 获取当前字典项的键。
	/// </summary>
	public object Key => enumerator.Current.Key!;

	/// <summary>
	/// 获取当前字典项的值。
	/// </summary>
	public object? Value => enumerator.Current.Value;

	/// <summary>
	/// 获取集合中位于枚举数当前位置的元素。
	/// </summary>
	public object Current => enumerator.Current;

	/// <summary>
	/// 将枚举数推进到集合的下一个元素。
	/// </summary>
	/// <returns>如果枚举数已成功地推进到下一个元素，则为 <c>true</c>；
	/// 如果枚举数传递到集合的末尾，则为 <c>false</c>。</returns>
	public bool MoveNext()
	{
		return enumerator.MoveNext();
	}

	/// <summary>
	/// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
	/// </summary>
	public void Reset()
	{
		enumerator.Reset();
	}
}
