namespace Cyjb.Collections;

/// <summary>
/// 表示只读前缀树的数据。
/// </summary>
/// <typeparam name="TValue">前缀树的值的类型。</typeparam>
[CLSCompliant(false)]
public readonly struct ReadOnlyPrefixTreeData<TValue>
{
	/// <summary>
	/// 前缀树包含的元素。
	/// </summary>
	public readonly KeyValuePair<string, TValue>[] Items;
	/// <summary>
	/// 前缀树的节点列表。
	/// </summary>
	public readonly int[] Nodes;
	/// <summary>
	/// 前缀树的转移列表。
	/// </summary>
	public readonly int[] Trans;

	/// <summary>
	/// 初始化只读前缀的数据。
	/// </summary>
	/// <param name="items">前缀树包含的元素。</param>
	/// <param name="nodes">前缀树的节点列表。</param>
	/// <param name="trans">前缀树的转移列表。</param>
	public ReadOnlyPrefixTreeData(KeyValuePair<string, TValue>[] items, int[] nodes, int[] trans)
	{
		Items = items;
		Nodes = nodes;
		Trans = trans;
	}
}
