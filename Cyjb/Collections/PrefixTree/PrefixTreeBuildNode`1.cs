using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cyjb.Collections;

/// <summary>
/// 前缀树的节点。
/// </summary>
/// <typeparam name="TValue">前缀树的值的类型。</typeparam>
internal sealed class PrefixTreeBuildNode<TValue> : IDisposable
{
	/// <summary>
	/// 节点的索引。
	/// </summary>
	private int index;
	/// <summary>
	/// 节点的首个索引。
	/// </summary>
	private int firstIndex = -1;
	/// <summary>
	/// 当前节点的字符索引。
	/// </summary>
	private int charIndex;
	/// <summary>
	/// 当前节点对应的项。
	/// </summary>
	private readonly PooledList<KeyValuePair<string, TValue>> items = new();
	/// <summary>
	/// 当前节点的值。
	/// </summary>
	private KeyValuePair<string, TValue> value;
	/// <summary>
	/// 当前节点是否有值。
	/// </summary>
	private bool hasValue = false;

	/// <summary>
	/// 使用指定的字符索引初始化 <see cref="PrefixTreeBuildNode{TValue}"/> 类的新实例。
	/// </summary>
	/// <param name="charIndex">字符索引。</param>
	public PrefixTreeBuildNode(int charIndex)
	{
		this.charIndex = charIndex;
	}

	/// <summary>
	/// 节点索引。
	/// </summary>
	public int Index
	{
		get => index;
		set
		{
			index = value;
			if (firstIndex < 0)
			{
				firstIndex = value;
			}
		}
	}

	/// <summary>
	/// 节点的首个索引。
	/// </summary>
	public int FirstIndex => firstIndex;

	/// <summary>
	/// 对应项的索引。
	/// </summary>
	public int ItemIndex { get; set; }

	/// <summary>
	/// 获取字符索引。
	/// </summary>
	public int CharIndex => charIndex;

	/// <summary>
	/// 获取节点是否有值。
	/// </summary>
	public bool HasValue => hasValue;

	/// <summary>
	/// 获取节点的值。
	/// </summary>
	public KeyValuePair<string, TValue> Value => value;

	/// <summary>
	/// 获取节点项的个数。
	/// </summary>
	public int Count => items.Length;

	/// <summary>
	/// 添加指定的项。
	/// </summary>
	public void Add(KeyValuePair<string, TValue> item)
	{
		if (charIndex == item.Key.Length)
		{
			if (hasValue)
			{
				throw new ArgumentException(Resources.CollectionDuplicateKey(item.Key));
			}
			hasValue = true;
			value = item;
		}
		else
		{
			items.Add(item);
		}
	}

	/// <summary>
	/// 尝试提取子节点的公共前缀。
	/// </summary>
	/// <param name="prefix">公共前缀。</param>
	/// <returns>是否成功提取到了公共前缀。</returns>
	public bool TryGetCommonPrefix(out ReadOnlySpan<char> prefix)
	{
		int length = items.Length;
		if (length == 0)
		{
			prefix = ReadOnlySpan<char>.Empty;
			return false;
		}
		else if (length == 1)
		{
			string key = items[0].Key;
			prefix = key.AsSpan(charIndex);
			charIndex = key.Length;
			// 重新调整当前项的值。
			hasValue = true;
			value = items[0];
			items.Clear();
			return true;
		}
		// 找到公共前缀。
		prefix = GetCommonPrefix();
		if (prefix.Length == 0)
		{
			return false;
		}
		charIndex += prefix.Length;
		// 重新调整当前项的值。
		hasValue = false;
		for (int i = 0; i < items.Length; i++)
		{
			var item = items[i];
			if (charIndex == item.Key.Length)
			{
				if (hasValue)
				{
					throw new ArgumentException(Resources.CollectionDuplicateKey(item.Key));
				}
				hasValue = true;
				value = item;
				items.RemoveAt(i);
				i--;
			}
		}
		return true;
	}

	/// <summary>
	/// 返回子节点的公共前缀，要求至少包含两个字符。
	/// </summary>
	/// <returns>子节点的公共前缀。</returns>
	private ReadOnlySpan<char> GetCommonPrefix()
	{
		ReadOnlySpan<char> prefix = items[0].Key.AsSpan(charIndex);
		int len = prefix.Length;
		for (int i = 1; i < items.Length && len > 0; i++)
		{
			ReadOnlySpan<char> other = items[i].Key.AsSpan(charIndex);
			len = Math.Min(len, other.Length);
			if (len == 0)
			{
				return ReadOnlySpan<char>.Empty;
			}
			unsafe
			{
				ref char x = ref MemoryMarshal.GetReference(prefix);
				ref char y = ref MemoryMarshal.GetReference(other);
				for (int j = 0; j < len; j++)
				{
					if (Unsafe.Add(ref x, j) != Unsafe.Add(ref y, j))
					{
						len = j;
						break;
					}
				}
			}
		}
		return prefix[..len];
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
