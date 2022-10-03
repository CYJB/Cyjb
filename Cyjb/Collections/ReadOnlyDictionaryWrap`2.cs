using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 提供 <see cref="IDictionary{TKey, TValue}"/> 的只读包装。
/// </summary>
/// <typeparam name="TKey">字典中键的类型。</typeparam>
/// <typeparam name="TValue">字典中值的类型。</typeparam>
internal sealed class ReadOnlyDictionaryWrap<TKey, TValue> : ReadOnlyDictionaryBase<TKey, TValue>
{
	/// <summary>
	/// 被包装的字典。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly IDictionary<TKey, TValue> dict;

	/// <summary>
	/// 使用指定的字典初始化 <see cref="ReadOnlyListWrap{T}"/> 类的新实例。
	/// </summary>
	/// <param name="dict">要被包装的字典。</param>
	public ReadOnlyDictionaryWrap(IDictionary<TKey, TValue> dict)
	{
		this.dict = dict;
	}

	#region ReadOnlyDictionaryWrap<TKey, TValue> 成员

	/// <summary>
	/// 获取当前字典包含的元素数。
	/// </summary>
	/// <value>当前字典中包含的元素数。</value>
	public override int Count => dict.Count;

	/// <summary>
	/// 获取包含当前字典的键的 <see cref="ICollection{TKey}"/>。
	/// </summary>
	public override ICollection<TKey> Keys => dict.Keys;

	/// <summary>
	/// 获取包含当前字典的值的 <see cref="ICollection{TValue}"/>。
	/// </summary>
	public override ICollection<TValue> Values => dict.Values;

	/// <summary>
	/// 获取具有指定键的元素。
	/// </summary>
	/// <param name="key">要获取的元素的键。</param>
	/// <returns>指定键对应的值。</returns>
	protected override TValue GetItem(TKey key)
	{
		return dict[key];
	}

	/// <summary>
	/// 确定当前字典是否包含带有指定键的元素。
	/// </summary>
	/// <param name="key">要在当前字典中定位的键。</param>
	/// <returns>如果当前字典包含包含具有键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	public override bool ContainsKey(TKey key)
	{
		return dict.ContainsKey(key);
	}

	/// <summary>
	/// 获取与指定键关联的值。
	/// </summary>
	/// <param name="key">要获取其值的键。</param>
	/// <param name="value">如果找到指定键，则返回与该键相关联的值；
	/// 否则返回 <paramref name="value"/> 参数的类型的默认值。</param>
	/// <returns>如果当前字典包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		return dict.TryGetValue(key, out value);
	}

	/// <summary>
	/// 返回一个循环访问字典的枚举器。
	/// </summary>
	/// <returns>可用于循环访问字典的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return dict.GetEnumerator();
	}

	#endregion // ReadOnlyDictionaryWrap<TKey, TValue> 成员

}
