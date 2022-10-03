namespace Cyjb.Collections;

/// <summary>
/// 提供对 <see cref="IDictionary{TKey, TValue}"/> 的扩展方法。
/// </summary>
public static class IDictionaryUtil
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
}
