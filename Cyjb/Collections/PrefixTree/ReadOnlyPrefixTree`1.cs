using System.Diagnostics.CodeAnalysis;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 表示只读的前缀树。
/// </summary>
/// <remarks>使用双数组表示，占用内存与字符集大小有关。注意会认为 <c>null</c> 和空字符串是相同的。</remarks>
/// <typeparam name="TValue">前缀树的值的类型。</typeparam>
public class ReadOnlyPrefixTree<TValue> : ReadOnlyDictionaryBase<string, TValue>
{
	/// <summary>
	/// 值索引是否不存在的掩码。
	/// </summary>
	private const ulong ValueIndexMask = 0x8000000000000000;
	/// <summary>
	/// 当前前缀树包含的元素。
	/// </summary>
	private readonly KeyValuePair<string, TValue>[] items;
	/// <summary>
	/// 当前前缀树的节点列表。
	/// </summary>
	/// <remarks>使用 <see cref="ulong"/> 的低 32 位表示节点的基索引，高 32 位表示节点对应的值；
	/// 若没有对应的值，则高 32 位为 -1。</remarks>
	private readonly ulong[] nodes;
	/// <summary>
	/// 当前前缀树的转移列表。
	/// </summary>
	/// <remarks>使用 <see cref="ulong"/> 的低 32 位表示 check，高 32 位表示 next。</remarks>
	private readonly ulong[] trans;
	/// <summary>
	/// 转移列表的长度。
	/// </summary>
	private readonly int transLength;
	/// <summary>
	/// 键的集合。
	/// </summary>
	private KeyCollection? keys;
	/// <summary>
	/// 值的集合。
	/// </summary>
	private ValueCollection? values;

	/// <summary>
	/// 使用指定的内容初始化 <see cref="ReadOnlyPrefixTree{TValue}"/> 类的新实例。
	/// </summary>
	/// <param name="collection">要填充到前缀树中的内容。</param>
	/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="collection"/> 包含重复键。</exception>
	public ReadOnlyPrefixTree(IEnumerable<KeyValuePair<string, TValue>> collection)
	{
		ArgumentNullException.ThrowIfNull(collection);
		PrefixTreeBuilder<TValue> builder = new(collection);
		items = builder.Items;
		nodes = builder.GetNodes();
		trans = builder.GetTrans();
		transLength = trans.Length;
	}

	/// <summary>
	/// 使用指定的预构建前缀树数据初始化 <see cref="ReadOnlyPrefixTree{TValue}"/> 类的新实例。
	/// </summary>
	/// <param name="data">预构建的前缀树数据。</param>
	[CLSCompliant(false)]
	public ReadOnlyPrefixTree(ReadOnlyPrefixTreeData<TValue> data)
	{
		items = data.Items;
		nodes = data.Nodes;
		trans = data.Trans;
		transLength = trans.Length;
	}

	/// <summary>
	/// 获取当前前缀树包含的元素数。
	/// </summary>
	/// <value>当前前缀树中包含的元素数。</value>
	public override int Count => items.Length;

	/// <summary>
	/// 获取包含当前前缀树的键的 <see cref="ICollection{TKey}"/>。
	/// </summary>
	public override ICollection<string> Keys => keys ??= new KeyCollection(this);

	/// <summary>
	/// 获取包含当前前缀树的值的 <see cref="ICollection{TValue}"/>。
	/// </summary>
	public override ICollection<TValue> Values => values ??= new ValueCollection(this);

	/// <summary>
	/// 获取具有指定键的元素。
	/// </summary>
	/// <param name="key">要获取的元素的键。</param>
	/// <returns>指定键对应的值。</returns>
	protected override TValue GetItem(string key)
	{
		int valueIndex = GetValueIndex(key);
		if (valueIndex < 0)
		{
			throw new KeyNotFoundException(Resources.DictionaryKeyNotFound(key));
		}
		else
		{
			return items[valueIndex].Value;
		}
	}

	/// <summary>
	/// 确定当前前缀树是否包含带有指定键的元素。
	/// </summary>
	/// <param name="key">要在当前前缀树中定位的键。</param>
	/// <returns>如果当前前缀树包含包含具有键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	public override bool ContainsKey(string key)
	{
		return GetValueIndex(key) >= 0;
	}

	/// <summary>
	/// 获取与指定键关联的值。
	/// </summary>
	/// <param name="key">要获取其值的键。</param>
	/// <param name="value">如果找到指定键，则返回与该键相关联的值；
	/// 否则返回 <paramref name="value"/> 参数的类型的默认值。</param>
	/// <returns>如果当前前缀树包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
	public override bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
	{
		int valueIndex = GetValueIndex(key);
		if (valueIndex < 0)
		{
			value = default;
			return false;
		}
		else
		{
			value = items[valueIndex].Value;
			return true;
		}
	}

	/// <summary>
	/// 匹配最短的前缀。
	/// </summary>
	/// <param name="text">要匹配的文本。</param>
	/// <param name="pair">如果找到了需要的匹配，则返回相应的键值对；
	/// 否则返回 <paramref name="pair"/> 参数的类型的默认值。</param>
	/// <returns>如果当前前缀树找到了最短的匹配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryMatchShortest(ReadOnlySpan<char> text, out KeyValuePair<string, TValue> pair)
	{
		int nodeIdx = 0;
		// 支持空字符串作为 key。
		ulong value = nodes[nodeIdx];
		if ((value & ValueIndexMask) == 0)
		{
			pair = items[unchecked((int)(value >> 32))];
			return true;
		}
		foreach (char ch in text)
		{
			nodeIdx = Next(nodeIdx, ch);
			if (nodeIdx < 0)
			{
				break;
			}
			value = nodes[nodeIdx];
			if ((value & ValueIndexMask) == 0)
			{
				pair = items[unchecked((int)(value >> 32))];
				return true;
			}
		}
		pair = default;
		return false;
	}

	/// <summary>
	/// 匹配最长的前缀。
	/// </summary>
	/// <param name="text">要匹配的文本。</param>
	/// <param name="pair">如果找到了需要的匹配，则返回相应的键值对；
	/// 否则返回 <paramref name="pair"/> 参数的类型的默认值。</param>
	/// <returns>如果当前前缀树找到了最长的匹配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryMatchLongest(ReadOnlySpan<char> text, out KeyValuePair<string, TValue> pair)
	{
		int nodeIdx = 0;
		int lastMatch = -1;
		// 支持空字符串作为 key。
		ulong value = nodes[nodeIdx];
		if ((value & ValueIndexMask) == 0)
		{
			lastMatch = unchecked((int)(value >> 32));
		}
		foreach (char ch in text)
		{
			nodeIdx = Next(nodeIdx, ch);
			if (nodeIdx < 0)
			{
				break;
			}
			value = nodes[nodeIdx];
			if ((value & ValueIndexMask) == 0)
			{
				lastMatch = unchecked((int)(value >> 32));
			}
		}
		if (lastMatch == -1)
		{
			pair = default;
			return false;
		}
		else
		{
			pair = items[lastMatch];
			return true;
		}
	}

	/// <summary>
	/// 返回一个循环访问前缀树的枚举器。
	/// </summary>
	/// <returns>可用于循环访问前缀树的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
	{
		foreach (KeyValuePair<string, TValue> item in items)
		{
			yield return item;
		}
	}

	/// <summary>
	/// 返回当前前缀树的底层数据，用于序列化预构建的前缀树，减少运行时构建耗时。
	/// </summary>
	/// <returns>前缀树的底层数据。</returns>
	[CLSCompliant(false)]
	public ReadOnlyPrefixTreeData<TValue> GetData()
	{
		return new ReadOnlyPrefixTreeData<TValue>(items, nodes, trans);
	}

	/// <summary>
	/// 返回指定键对应的值索引。
	/// </summary>
	/// <param name="key">要检查的键。</param>
	/// <returns>值的索引，如果不存在则返回 <c>-1</c>。</returns>
	private int GetValueIndex(string key)
	{
		int nodeIdx = 0;
		foreach (char ch in key)
		{
			nodeIdx = Next(nodeIdx, ch);
			if (nodeIdx < 0)
			{
				return -1;
			}
		}
		return unchecked((int)(uint)(nodes[nodeIdx] >> 32));
	}

	/// <summary>
	/// 返回指定节点指定转移的子节点。
	/// </summary>
	/// <param name="node">要检查的节点。</param>
	/// <param name="idx">要检查的转移。</param>
	/// <returns>子节点，如果不存在则返回 <c>-1</c>。</returns>
	private int Next(int node, int idx)
	{
		idx += unchecked((int)(uint)nodes[node]);
		if (idx < 0 || idx >= transLength)
		{
			return -1;
		}
		ulong result = trans[idx];
		if ((uint)result == node)
		{
			return unchecked((int)(result >> 32));
		}
		return -1;
	}

	/// <summary>
	/// 键的集合。
	/// </summary>
	private class KeyCollection : ReadOnlyCollectionBase<string>
	{
		/// <summary>
		/// 前缀树。
		/// </summary>
		private readonly ReadOnlyPrefixTree<TValue> tree;

		/// <summary>
		/// 使用指定的前缀树初始化 <see cref="KeyCollection"/> 类的新实例。
		/// </summary>
		/// <param name="tree">所属前缀树。</param>
		public KeyCollection(ReadOnlyPrefixTree<TValue> tree)
		{
			this.tree = tree;
		}

		#region ReadOnlyCollectionBase<string> 成员

		/// <summary>
		/// 获取当前集合包含的元素数。
		/// </summary>
		/// <value>当前集合中包含的元素数。</value>
		public override int Count => tree.Count;

		/// <summary>
		/// 确定当前集合是否包含指定对象。
		/// </summary>
		/// <param name="item">要在当前集合中定位的对象。</param>
		/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(string item)
		{
			return tree.ContainsKey(item);
		}

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public override IEnumerator<string> GetEnumerator()
		{
			foreach (KeyValuePair<string, TValue> item in tree.items)
			{
				yield return item.Key;
			}
		}

		#endregion // ReadOnlyCollectionBase<string> 成员

	}

	/// <summary>
	/// 值的集合。
	/// </summary>
	private class ValueCollection : ReadOnlyCollectionBase<TValue>
	{
		/// <summary>
		/// 前缀树。
		/// </summary>
		private readonly ReadOnlyPrefixTree<TValue> tree;

		/// <summary>
		/// 使用指定的前缀树初始化 <see cref="KeyCollection"/> 类的新实例。
		/// </summary>
		/// <param name="tree">所属前缀树。</param>
		public ValueCollection(ReadOnlyPrefixTree<TValue> tree)
		{
			this.tree = tree;
		}

		#region ReadOnlyCollectionBase<TValue> 成员

		/// <summary>
		/// 获取当前集合包含的元素数。
		/// </summary>
		/// <value>当前集合中包含的元素数。</value>
		public override int Count => tree.Count;

		/// <summary>
		/// 确定当前集合是否包含指定对象。
		/// </summary>
		/// <param name="item">要在当前集合中定位的对象。</param>
		/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(TValue item)
		{
			EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
			foreach (var pair in tree.items)
			{
				if (comparer.Equals(pair.Value, item))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public override IEnumerator<TValue> GetEnumerator()
		{
			foreach (KeyValuePair<string, TValue> item in tree.items)
			{
				yield return item.Value;
			}
		}

		#endregion // ReadOnlyCollectionBase<TValue> 成员

	}

}
