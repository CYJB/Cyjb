using System.Diagnostics.CodeAnalysis;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 提供对 <see cref="IDictionary{TKey, TValue}"/> 的扩展方法。
/// </summary>
public static class DictionaryUtil
{
	/// <summary>
	/// 返回当前字典的只读包装。
	/// </summary>
	/// <typeparam name="TKey">字典中键的类型。</typeparam>
	/// <typeparam name="TValue">字典中值的类型。</typeparam>
	/// <param name="dict">要被包装的字典。</param>
	/// <returns>当前字典的只读包装。</returns>
	public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dict)
	{
		return new ReadOnlyDictionaryWrap<TKey, TValue>(dict);
	}

	#region Empty

	/// <summary>
	/// 返回一个空的只读字典。
	/// </summary>
	/// <typeparam name="TKey">字典中键的类型。</typeparam>
	/// <typeparam name="TValue">字典中值的类型。</typeparam>
	/// <returns>空的只读字典。</returns>
	public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>()
	{
		return EmptyDictionary<TKey, TValue>.Instance;
	}

	/// <summary>
	/// 空的只读字典。
	/// </summary>
	/// <typeparam name="TKey">字典中键的类型。</typeparam>
	/// <typeparam name="TValue">字典中值的类型。</typeparam>
	private class EmptyDictionary<TKey, TValue> : ReadOnlyDictionaryBase<TKey, TValue>
	{
		/// <summary>
		/// 空的只读字典实例。
		/// </summary>
		public static readonly EmptyDictionary<TKey, TValue> Instance = new();

		/// <summary>
		/// 获取当前字典包含的元素数。
		/// </summary>
		/// <value>当前字典中包含的元素数。</value>
		public override int Count => 0;

		/// <summary>
		/// 获取包含当前字典的键的 <see cref="ICollection{TKey}"/>。
		/// </summary>
		public override ICollection<TKey> Keys => EmptyList<TKey>.Instance;

		/// <summary>
		/// 获取包含当前字典的值的 <see cref="ICollection{TValue}"/>。
		/// </summary>
		public override ICollection<TValue> Values => EmptyList<TValue>.Instance;

		/// <summary>
		/// 获取具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取的元素的键。</param>
		/// <returns>指定键对应的值。</returns>
		protected override TValue GetItem(TKey key)
		{
			return default!;
		}

		/// <summary>
		/// 确定当前字典是否包含带有指定键的元素。
		/// </summary>
		/// <param name="key">要在当前字典中定位的键。</param>
		/// <returns>如果当前字典包含包含具有键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public override bool ContainsKey(TKey key)
		{
			return false;
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
			value = default;
			return false;
		}

		/// <summary>
		/// 返回一个循环访问字典的枚举器。
		/// </summary>
		/// <returns>可用于循环访问字典的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			yield break;
		}
	}

	#endregion // Empty

}
