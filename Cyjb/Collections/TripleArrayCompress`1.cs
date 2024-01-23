namespace Cyjb.Collections;

/// <summary>
/// 提供三数组压缩能力。
/// </summary>
/// <remarks><para>三数组压缩一般用于压缩存储树/图的子节点，通过将到子节点的转移压缩到
/// base + next + check 三个数组内，在保持性能基本不变的前提下充分利用转移间的空格来减少内存使用。
/// 假设使用 <c>node[id]</c> 来表示 <c>id</c> 对应的子节点，使用三数组表示时，若
/// <c>base[node]</c> 在 <c>check</c> 范围内，且 <c>check[base[node] + id] == node</c>，那么
/// <c>node[id] = next[base[node] + id]</c>；否则不存在 <c>node[id]</c>。</para>
/// <para>要求子节点的 key 不能重复，且子节点的索引必须大于等于零。</para></remarks>
/// <typeparam name="T">节点的类型。</typeparam>
public class TripleArrayCompress<T> : TripleArrayCompress<T, T>
{
	/// <summary>
	/// 使用表示无效的节点初始化 <see cref="TripleArrayCompress{T}"/> 类的新实例。
	/// </summary>
	/// <param name="invalidNode">表示无效的节点。</param>
	public TripleArrayCompress(T invalidNode) : base(invalidNode, invalidNode) { }
}

