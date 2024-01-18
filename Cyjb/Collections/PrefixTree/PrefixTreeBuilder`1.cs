using System.Runtime.InteropServices;

namespace Cyjb.Collections;

/// <summary>
/// 前缀树的构造器。
/// </summary>
/// <typeparam name="TValue">前缀树的值的类型。</typeparam>
internal class PrefixTreeBuilder<TValue>
{
	/// <summary>
	/// 表示无效值索引的值。
	/// </summary>
	private const int InvalidValueIndex = -1;
	/// <summary>
	/// 表示无效基索引的值。
	/// </summary>
	private const int InvalidBaseIndex = int.MinValue;
	/// <summary>
	/// 表示无效节点的值。
	/// </summary>
	private const ulong InvalidNode = unchecked(((ulong)InvalidValueIndex << 32) | (uint)InvalidBaseIndex);
	/// <summary>
	/// 三数组压缩。
	/// </summary>
	private readonly TripleArrayCompress<int> arrayCompress = new(-1);
	/// <summary>
	/// 子节点字典。
	/// </summary>
	private readonly SortedDictionary<int, PrefixTreeBuildNode<TValue>> childDict = new(PrefixTreeNodeComparer.Instance);
	/// <summary>
	/// 内容数组。
	/// </summary>
	private readonly KeyValuePair<string, TValue>[] items;
	/// <summary>
	/// 内容的索引。
	/// </summary>
	private int itemIndex = 0;
	/// <summary>
	/// 前缀树节点列表。
	/// </summary>
	/// <remarks>使用 <see cref="ulong"/> 的低 32 位表示节点的基索引，高 32 位表示节点对应的值；
	/// 若没有对应的值，则高 32 位为 -1。</remarks>
	private readonly List<ulong> nodes = new();
	/// <summary>
	/// 节点的索引。
	/// </summary>
	private int nodeIndex = 0;
	/// <summary>
	/// 生成前缀树的状态堆栈。
	/// </summary>
	private readonly Stack<PrefixTreeBuildNode<TValue>> stack = new();

	/// <summary>
	/// 使用指定的内容初始化前缀树构造器。
	/// </summary>
	/// <param name="collection">要填充到前缀树中的内容。</param>
	/// <exception cref="ArgumentException"><paramref name="collection"/> 包含重复键。</exception>
	public PrefixTreeBuilder(IEnumerable<KeyValuePair<string, TValue>> collection)
	{
		PrefixTreeBuildNode<TValue> node = new(nodeIndex++, 0);
		foreach (var item in collection)
		{
			if (item.Key == null)
			{
				node.Add(new KeyValuePair<string, TValue>(string.Empty, item.Value));
			}
			else
			{
				node.Add(item);
			}
		}
		items = new KeyValuePair<string, TValue>[node.Count];
		stack.Push(node);
		while (stack.Count > 0)
		{
			AddNode();
		}
	}

	/// <summary>
	/// 获取内容的数组。
	/// </summary>
	public KeyValuePair<string, TValue>[] Items => items;

	/// <summary>
	/// 返回节点的数组。
	/// </summary>
	/// <returns>表示节点值的 <see cref="ulong"/> 数组。</returns>
	/// <remarks>使用 <see cref="ulong"/> 的低 32 位表示节点的基索引，高 32 位表示节点对应的值；
	/// 若没有对应的值，则高 32 位为 -1。</remarks>
	public ulong[] GetNodes() => nodes.ToArray();

	/// <summary>
	/// 返回转移的数组。
	/// </summary>
	/// <returns>表示节点转移的 <see cref="ulong"/> 数组。</returns>
	/// <remarks>使用 <see cref="ulong"/> 的低 32 位表示 check，高 32 位表示 next。</remarks>
	public ulong[] GetTrans()
	{
		ReadOnlySpan<int> next = CollectionsMarshal.AsSpan(arrayCompress.Next);
		ReadOnlySpan<int> check = CollectionsMarshal.AsSpan(arrayCompress.Check);
		ulong[] trans = new ulong[next.Length];
		for (int i = 0; i < next.Length; i++)
		{
			trans[i] = unchecked(((ulong)next[i] << 32) | (uint)check[i]);
		}
		return trans;
	}

	/// <summary>
	/// 添加新的前缀树节点。
	/// </summary>
	private void AddNode()
	{
		PrefixTreeBuildNode<TValue> node = stack.Pop();
		int charIndex = node.CharIndex;
		int nextCharIndex = charIndex + 1;
		KeyValuePair<string, TValue>? curValue = null;
		childDict.Clear();
		foreach (var pair in node)
		{
			string key = pair.Key;
			if (charIndex == key.Length)
			{
				if (curValue.HasValue)
				{
					throw new ArgumentException(Resources.CollectionDuplicateKey(key));
				}
				curValue = pair;
				continue;
			}
			char ch = key[charIndex];
			if (!childDict.TryGetValue(ch, out var childNode))
			{
				childNode = new PrefixTreeBuildNode<TValue>(nodeIndex++, nextCharIndex);
				childDict.Add(ch, childNode);
			}
			childNode.Add(pair);
		}
		EnsureNodesCount();
		int valueIndex = InvalidValueIndex;
		if (curValue.HasValue)
		{
			valueIndex = itemIndex;
			items[itemIndex++] = curValue.Value;
		}
		if (childDict.Count == 0)
		{
			// 是叶子节点。
			SetNode(node.Index, valueIndex, InvalidBaseIndex);
		}
		else
		{
			// 是父节点。
			// 三数组压缩。
			int baseIndex = arrayCompress.AddNode(node.Index,
				childDict.Select(pair => new KeyValuePair<int, int>(pair.Key, pair.Value.Index)));
			SetNode(node.Index, valueIndex, baseIndex);
			// 将子节点逆序（字符从大到小）添加到堆栈中。
			foreach (var pair in childDict)
			{
				stack.Push(pair.Value);
			}
		}
		node.Dispose();
	}

	/// <summary>
	/// 确保节点包含足够的内容。
	/// </summary>
	private void EnsureNodesCount()
	{
		if (nodes.Count >= nodeIndex)
		{
			return;
		}
		nodes.EnsureCapacity(nodeIndex);
		for (int i = nodes.Count; i < nodeIndex; i++)
		{
			nodes.Add(InvalidNode);
		}
	}

	/// <summary>
	/// 设置指定节点的值。
	/// </summary>
	/// <param name="index">节点的索引。</param>
	/// <param name="valueIndex">节点对应的值。</param>
	/// <param name="baseIndex">节点的基索引。</param>
	private void SetNode(int index, int valueIndex, int baseIndex)
	{
		nodes[index] = unchecked(((ulong)valueIndex << 32) | (uint)baseIndex);
	}
}
