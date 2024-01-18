namespace Cyjb.Collections;

/// <summary>
/// 前缀树的节点。
/// </summary>
/// <typeparam name="TValue">前缀树的值的类型。</typeparam>
internal sealed class PrefixTreeBuildNode<TValue> : IDisposable
{
	/// <summary>
	/// 当前节点对应的项。
	/// </summary>
	private readonly PooledList<KeyValuePair<string, TValue>> items = new();

	/// <summary>
	/// 使用指定的节点索引和字符索引初始化 <see cref="PrefixTreeBuildNode{TValue}"/> 类的新实例。
	/// </summary>
	/// <param name="index">节点索引。</param>
	/// <param name="charIndex">字符索引。</param>
	public PrefixTreeBuildNode(int index, int charIndex)
	{
		Index = index;
		CharIndex = charIndex;
	}

	/// <summary>
	/// 获取节点索引。
	/// </summary>
	public int Index { get; init; }

	/// <summary>
	/// 获取字符索引。
	/// </summary>
	public int CharIndex { get; init; }

	/// <summary>
	/// 获取节点项的个数。
	/// </summary>
	public int Count => items.Length;

	/// <summary>
	/// 添加指定的项。
	/// </summary>
	public void Add(KeyValuePair<string, TValue> item)
	{
		items.Add(item);
	}

	/// <summary>
	/// 释放当前实例的非托管资源。
	/// </summary>
	public void Dispose()
	{
		items.Dispose();
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的枚举器。</returns>
	public Span<KeyValuePair<string, TValue>>.Enumerator GetEnumerator()
	{
		return items.GetEnumerator();
	}
}
