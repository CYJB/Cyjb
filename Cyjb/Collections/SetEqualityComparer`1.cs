namespace Cyjb.Collections;

/// <summary>
/// 表示根据内容比较 <see cref="IReadOnlySet{T}"/> 集合的比较器。
/// </summary>
/// <typeparam name="T">要比较的集合元素的类型。</typeparam>
public sealed class SetEqualityComparer<T> : EqualityComparer<IReadOnlySet<T>>
{
	/// <summary>
	/// 获取默认的相等比较器。
	/// </summary>
	/// <value>一个默认的 <see cref="SetEqualityComparer{T}"/> 比较器。</value>
	public new static readonly IEqualityComparer<IReadOnlySet<T>> Default = new SetEqualityComparer<T>();

	/// <summary>
	/// 初始化 <see cref="SetEqualityComparer{T}"/> 类的新实例。
	/// </summary>
	private SetEqualityComparer() { }

	#region EqualityComparer<IReadOnlySet<T>> 成员

	/// <summary>
	/// 确定指定的对象是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个 <see cref="IReadOnlySet{T}"/> 的对象。</param>
	/// <param name="right">要比较的第二个 <see cref="IReadOnlySet{T}"/> 的对象。</param>
	/// <returns>如果指定的对象相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <overloads>
	/// <summary>
	/// 确定指定的对象是否相等。
	/// </summary>
	/// </overloads>
	public override bool Equals(IReadOnlySet<T>? left, IReadOnlySet<T>? right)
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
		return left.SetEquals(right);
	}

	/// <summary>
	/// 返回指定对象的哈希代码，使用 MurmurHash3 的无序版本。
	/// </summary>
	/// <param name="set">将为其返回哈希代码的集合。</param>
	/// <returns>指定集合的哈希代码。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="set"/> 为 <c>null</c>。</exception>
	/// <overloads>
	/// <summary>
	/// 返回指定对象的哈希代码。
	/// </summary>
	/// </overloads>
	public override int GetHashCode(IReadOnlySet<T> set)
	{
		return UnorderedHashCode.Combine(set);
	}

	#endregion // EqualityComparer<IReadOnlySet<T>> 成员

}
