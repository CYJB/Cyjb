using System.Runtime.InteropServices;

namespace Cyjb.Collections;

/// <summary>
/// 前缀树的构造器。
/// </summary>
/// <typeparam name="TValue">前缀树的值的类型。</typeparam>
internal class PrefixTreeBuilder<TValue>
{
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
	/// 前缀树节点列表。
	/// </summary>
	/// </remarks>
	private readonly List<int> nodes = new();
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
		PrefixTreeBuildNode<TValue> node = new(0);
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
		items = new KeyValuePair<string, TValue>[node.Count + (node.HasValue ? 1 : 0)];
		node.Index = NextNodeIndex(node);
		EnsureNodesCount();
		// 根节点的路径压缩一定对应 item 从小到大的顺序，可以直接添加。
		while (node.TryGetCommonPrefix(out var prefix))
		{
			// 存在公共前缀，添加公共前缀作为路径压缩。
			AddShortPath(node, prefix);
			node.Index = NextNodeIndex(node);
		}
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
	/// <returns>表示节点信息的 <see cref="int"/> 数组。</returns>
	/// <remarks>使用 <c>node[i]</c> 表示节点的子节点信息；最低的一位（<c>node[i] &amp; 1</c>）表示是否包含值，
	/// 若包含值则 <c>node[i+1]</c> 即为值的索引；最低的倒数第二位（<c>node[i] &amp; 2</c>）表示是否使用路径压缩，
	/// 若未使用路径压缩则 <c>node[i] >> 2</c> 表示子节点的基索引，若使用路径压缩则 <c>node[i] >> 2</c> 表示路径压缩的长度，
	/// 从 <c>node[i+1]</c>（如果有值则为 <c>node[i+2]</c>）之后开始指定长度 <c>char</c> 的内容即为路径压缩的字符串内容，
	/// 并四字节对齐后跟下一节点的信息。</remarks>
	public int[] GetNodes() => nodes.ToArray();

	/// <summary>
	/// 返回转移的数组。
	/// </summary>
	/// <returns>表示节点转移的 <see cref="int"/> 数组。</returns>
	/// <remarks>使用 <c>node[i]</c> 表示 <c>check</c>，<c>node[i+1]</c> 表示 <c>next</c>。</remarks>
	public int[] GetTrans()
	{
		ReadOnlySpan<int> next = CollectionsMarshal.AsSpan(arrayCompress.Next);
		ReadOnlySpan<int> check = CollectionsMarshal.AsSpan(arrayCompress.Check);
		int[] trans = new int[next.Length * 2];
		for (int i = 0, j = 0; i < next.Length; i++)
		{
			trans[j++] = check[i];
			trans[j++] = next[i];
		}
		return trans;
	}

	/// <summary>
	/// 返回下一个节点索引。
	/// </summary>
	/// <param name="node">当前节点。</param>
	/// <returns>下一个节点索引。</returns>
	private int NextNodeIndex(PrefixTreeBuildNode<TValue> node)
	{
		int index = nodeIndex++;
		if (node.HasValue)
		{
			int valueIndex = nodeIndex++;
			// 现在可以直接把值复制过去。
			EnsureNodesCount();
			int itemIndex = node.ItemIndex++;
			nodes[valueIndex] = itemIndex;
			items[itemIndex] = node.Value;
			// 标记为有值。
			nodes[index] |= 1;
		}
		return index;
	}

	/// <summary>
	/// 将指定路径压缩的内容添加到节点数组中。
	/// </summary>
	/// <param name="node">当前节点。</param>
	/// <param name="str">要添加的公共前缀。</param>
	private void AddShortPath(PrefixTreeBuildNode<TValue> node, ReadOnlySpan<char> str)
	{
		int len = (str.Length + 1) / 2;
		int index = nodeIndex;
		nodeIndex += len;
		// 确保足够容量。
		EnsureNodesCount();
		str.CopyTo(MemoryMarshal.Cast<int, char>(CollectionsMarshal.AsSpan(nodes)[index..]));
		nodes[node.Index] |= str.Length << 2;
	}

	/// <summary>
	/// 添加新的前缀树节点。
	/// </summary>
	private void AddNode()
	{
		PrefixTreeBuildNode<TValue> node = stack.Pop();
		int charIndex = node.CharIndex;
		int nextCharIndex = charIndex + 1;
		childDict.Clear();
		foreach (var pair in node)
		{
			char ch = pair.Key[charIndex];
			if (!childDict.TryGetValue(ch, out var childNode))
			{
				childNode = new PrefixTreeBuildNode<TValue>(nextCharIndex);
				childDict.Add(ch, childNode);
			}
			childNode.Add(pair);
		}
		int childCount = childDict.Count;
		if (childCount == 0)
		{
			// 是叶子节点，什么都不需要做。
			node.Dispose();
			return;
		}
		// 是父节点，将子节点逆序（字符从大到小）添加到堆栈中，这样后续遍历的时候可以确保 item 的顺序。
		foreach (var pair in childDict)
		{
			stack.Push(pair.Value);
		}
		// 按照字符从小到大检查路径压缩的场景。
		int itemIndex = node.ItemIndex;
		{
			foreach (var childNode in stack.Take(childCount))
			{
				childNode.ItemIndex = itemIndex;
				itemIndex += childNode.Count;
				// 注意节点本身的值也会占用 item 的个数。
				if (childNode.HasValue)
				{
					itemIndex++;
				}
				childNode.Index = NextNodeIndex(childNode);
				while (childNode.TryGetCommonPrefix(out var prefix))
				{
					// 存在公共前缀，添加公共前缀作为路径压缩。
					AddShortPath(childNode, prefix);
					childNode.Index = NextNodeIndex(childNode);
				}
			}
		}
		// 依次将每个子节点压缩到三数组中。
		int baseIndex = arrayCompress.AddNode(node.Index,
			childDict.Select(pair => new KeyValuePair<int, int>(pair.Key, pair.Value.FirstIndex)));
		EnsureNodesCount();
		nodes[node.Index] |= (baseIndex << 2) | 2;
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
			// 将节点初始化为无效节点：无值、路径压缩长度 0。
			nodes.Add(0);
		}
	}
}
