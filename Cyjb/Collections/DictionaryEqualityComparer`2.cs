namespace Cyjb.Collections;

/// <summary>
/// 表示根据内容比较 <see cref="IReadOnlyDictionary{TKey, TValue}"/> 字典的比较器。
/// </summary>
/// <typeparam name="TKey">要比较的字典键的类型。</typeparam>
/// <typeparam name="TValue">要比较的字典值的类型。</typeparam>
public sealed class DictionaryEqualityComparer<TKey, TValue> : EqualityComparer<IReadOnlyDictionary<TKey, TValue>>
{
	/// <summary>
	/// 获取默认的相等比较器。
	/// </summary>
	/// <value>一个默认的 <see cref="DictionaryEqualityComparer{TKey, TValue}"/> 比较器。</value>
	public new static readonly IEqualityComparer<IReadOnlyDictionary<TKey, TValue>> Default =
		new DictionaryEqualityComparer<TKey, TValue>();

	/// <summary>
	/// 初始化 <see cref="DictionaryEqualityComparer{TKey, TValue}"/> 类的新实例。
	/// </summary>
	private DictionaryEqualityComparer() { }

	#region EqualityComparer<IReadOnlyDictionary<TKey, TValue>> 成员

	/// <summary>
	/// 确定指定的对象是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个 <see cref="IReadOnlyDictionary{TKey, TValue}"/> 的对象。</param>
	/// <param name="right">要比较的第二个 <see cref="IReadOnlyDictionary{TKey, TValue}"/> 的对象。</param>
	/// <returns>如果指定的对象相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <overloads>
	/// <summary>
	/// 确定指定的对象是否相等。
	/// </summary>
	/// </overloads>
	public override bool Equals(IReadOnlyDictionary<TKey, TValue>? left, IReadOnlyDictionary<TKey, TValue>? right)
	{
		if (ReferenceEquals(left, right))
		{
			return true;
		}
		if (left is null)
		{
			return right is null;
		}
		if (right is null)
		{
			return false;
		}
		if (left.Count != right.Count)
		{
			return false;
		}
		EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
		foreach (var (key, value) in left)
		{
			if (!right.TryGetValue(key, out TValue? rightValue) || !comparer.Equals(value, rightValue))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 返回指定对象的哈希代码，使用 MurmurHash3 的无序版本。
	/// </summary>
	/// <param name="dict">将为其返回哈希代码的集合。</param>
	/// <returns>指定集合的哈希代码。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="dict"/> 为 <c>null</c>。</exception>
	/// <overloads>
	/// <summary>
	/// 返回指定对象的哈希代码。
	/// </summary>
	/// </overloads>
	public override int GetHashCode(IReadOnlyDictionary<TKey, TValue> dict)
	{
		UnorderedHashCode hashCode = new();
		foreach (var (key, value) in dict)
		{
			hashCode.Add(HashCode.Combine(key, value));
		}
		return hashCode.ToHashCode();
	}

	#endregion // EqualityComparer<IReadOnlyDictionary<TKey, TValue>> 成员

}
