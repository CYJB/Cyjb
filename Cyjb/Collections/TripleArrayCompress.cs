namespace Cyjb.Collections;

/// <summary>
/// 提供三数组压缩能力。
/// </summary>
/// <remarks><para>三数组压缩一般用于压缩存储树/图的子节点，通过将到子节点的转移压缩到
/// base + next + check 三个数组内，在保持性能基本不变的前提下充分利用转移间的空格来减少内存使用。
/// 假设使用 <c>node[id]</c> 来表示 <c>id</c> 对应的子节点，使用三数组表示时，若
/// <c>base[node]</c> 在 <c>check</c> 范围内，且 <c>check[base[node] + id] == node</c>，那么
/// <c>node[id] = next[base[node] + id]</c>；否则不存在 <c>node[id]</c>。</para>
/// <para>要求子节点的 key 不能重复，且子节点的索引必须大于等于零。</para>
/// </remarks>
/// <typeparam name="T">节点的类型。</typeparam>
public sealed class TripleArrayCompress<T>
{
	/// <summary>
	/// 表示无效的节点。
	/// </summary>
	private readonly T invalidNode;
	/// <summary>
	/// 子节点列表。
	/// </summary>
	private readonly List<T> next = new();
	/// <summary>
	/// 状态检查。
	/// </summary>
	private readonly List<T> check = new();
	/// <summary>
	/// 子节点索引的匹配模式。
	/// </summary>
	private readonly BitList pattern = new();
	/// <summary>
	/// 下一状态列表的可用空当列表。
	/// </summary>
	private readonly BitList spaces = new();
	/// <summary>
	/// 下一个可用的 next 空当。
	/// </summary>
	private int nextSpaceIndex = 0;

	/// <summary>
	/// 使用表示无效的节点初始化 <see cref="TripleArrayCompress{T}"/> 类的新实例。
	/// </summary>
	/// <param name="invalidNode">表示无效的节点。</param>
	public TripleArrayCompress(T invalidNode)
	{
		this.invalidNode = invalidNode;
	}

	/// <summary>
	/// 获取子节点列表。
	/// </summary>
	public List<T> Next => next;

	/// <summary>
	/// 获取状态检查列表。
	/// </summary>
	public List<T> Check => check;

	/// <summary>
	/// 添加指定的节点。
	/// </summary>
	/// <param name="currentNode">当前节点。</param>
	/// <param name="children">子节点列表，会被多次遍历因此请视情况使用缓存。</param>
	/// <returns>指定节点的基线索引。</returns>
	public int AddNode(T currentNode, IEnumerable<KeyValuePair<int, T>> children)
	{
		pattern.Clear();
		// 找到最小的子节点索引。
		int minIndex = int.MaxValue;
		foreach (var pair in children)
		{
			int idx = pair.Key;
			if (idx < minIndex)
			{
				minIndex = idx;
			}
			if (pattern.Count <= idx)
			{
				pattern.Resize(idx + 1);
			}
			pattern[idx] = true;
		}
		if (minIndex == int.MaxValue)
		{
			// 没有子节点。
			return int.MinValue;
		}
		if (minIndex > 0)
		{
			pattern.RemoveRange(0, minIndex);
		}
		int baseIndex = FindSpace() - minIndex;
		foreach (var (idx, childNode) in children)
		{
			int index = idx + baseIndex;
			spaces[index] = true;
			next[index] = childNode;
			check[index] = currentNode;
		}
		// 更新 nextSpaceIndex 和 lastFilledIndex
		nextSpaceIndex = spaces.IndexOf(false, nextSpaceIndex);
		if (nextSpaceIndex < 0)
		{
			nextSpaceIndex = spaces.Count;
		}
		return baseIndex;
	}

	/// <summary>
	/// 找到可用于插入转移的索引。
	/// </summary>
	/// <returns>可用于插入转移的索引。</returns>
	private int FindSpace()
	{
		int spaceIndex = spaces.FindSpace(pattern, nextSpaceIndex);
		int count = pattern.Count;
		// 确保空白记录包含足够的空间
		if (spaces.Count < spaceIndex + count)
		{
			spaces.Resize(spaceIndex + count);
		}
		int addedCount = spaceIndex + count - next.Count;
		if (addedCount > 0)
		{
			next.AddRange(Enumerable.Repeat(invalidNode, addedCount));
			check.AddRange(Enumerable.Repeat(invalidNode, addedCount));
		}
		return spaceIndex;
	}
}

