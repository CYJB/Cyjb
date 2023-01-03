using System.Diagnostics;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 表示字符的只读有序集合。
/// </summary>
/// <remarks><see cref="ReadOnlyCharSet"/> 类采用类似位示图的树状位压缩数组存储字符，
/// 关于该数据结构的更多解释，请参见我的博文
/// <see href="http://www.cnblogs.com/cyjb/archive/p/BitCharSet.html">
/// 《基于树状位压缩数组的字符集合》</see>。</remarks>
/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/BitCharSet.html">
/// 《基于树状位压缩数组的字符集合》</seealso>
[Serializable, DebuggerDisplay("{ToString()} Count = {Count}")]
[DebuggerTypeProxy(typeof(CharSetDebugView))]
public sealed partial class ReadOnlyCharSet : ReadOnlySetBase<char>, ICharSet,
	IEquatable<ReadOnlyCharSet>, IRangeCollection<char>
{
	/// <summary>
	/// 字符集合的顶层数组。
	/// </summary>
	private readonly CharSetItem[] data;
	/// <summary>
	/// 集合中字符的个数。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly int count = 0;
	/// <summary>
	/// 集合中字符范围的个数。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Lazy<int> rangeCount;
	/// <summary>
	/// 集合的字符串表示。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? text = null;

	/// <summary>
	/// 从指定的范围字符串初始化 <see cref="ReadOnlyCharSet"/> 类的新实例。
	/// </summary>
	/// <param name="ranges">范围字符串，字符串依次包含了每个字符范围的起止位置。</param>
	/// <returns><see cref="ReadOnlyCharSet"/> 实例。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="ranges"/> 为 <c>null</c>。</exception>
	public static ReadOnlyCharSet FromRange(string ranges)
	{
		ArgumentNullException.ThrowIfNull(ranges);
		CharSet set = new();
		for (int i = 1; i < ranges.Length; i += 2)
		{
			set.Add(ranges[i - 1], ranges[i]);
		}
		return set.MoveReadOnly();
	}

	/// <summary>
	/// 初始化 <see cref="ReadOnlyCharSet"/> 类的新实例。
	/// </summary>
	public ReadOnlyCharSet() : base()
	{
		data = CharSetConfig.CreateData();
		rangeCount = new Lazy<int>(0);
	}
	/// <summary>
	/// 使用指定的元素初始化 <see cref="ReadOnlyCharSet"/> 类的新实例。
	/// </summary>
	/// <param name="collection">初始化的字符集合。</param>
	public ReadOnlyCharSet(IEnumerable<char> collection) : base()
	{
		CharSet set = new(collection);
		data = ((ICharSet)set).Data;
		count = set.Count;
		rangeCount = new Lazy<int>(() => CharSetConfig.RangeCount(data));
	}
	/// <summary>
	/// 使用指定的数据初始化 <see cref="ReadOnlyCharSet"/> 类的新实例。
	/// </summary>
	internal ReadOnlyCharSet(CharSetItem[] data, int count, string? text) : base()
	{
		this.data = data;
		this.count = count;
		this.text = text;
		rangeCount = new Lazy<int>(() => CharSetConfig.RangeCount(data));
	}

	/// <summary>
	/// 字符集合的顶层数组。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	CharSetItem[] ICharSet.Data => data;

	/// <summary>
	/// 返回当前字符集合包含的范围个数。
	/// </summary>
	/// <returns>当前字符集合包含的范围个数。</returns>
	/// <remarks>与 <c>ReadOnlyCharSet.Ranges().Count()</c> 等价，但效率更高。</remarks>
	public int RangeCount()
	{
		return rangeCount.Value;
	}

	/// <summary>
	/// 返回一个循环访问字符范围的枚举器。
	/// </summary>
	/// <returns>可用于循环访问字符范围的 <see cref="IEnumerable{Tuple}"/>。</returns>
	public IEnumerable<ValueRange<char>> Ranges()
	{
		return CharSetConfig.Ranges(data);
	}

	#region ReadOnlySetBase<char> 成员

	/// <summary>
	/// 获取当前集合包含的字符数。
	/// </summary>
	/// <value>当前集合中包含的字符数。</value>
	public override int Count => count;

	/// <summary>
	/// 确定当前集合是否包含指定字符。
	/// </summary>
	/// <param name="ch">要在当前集合中定位的字符。</param>
	/// <returns>如果在当前集合中找到 <paramref name="ch"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool Contains(char ch)
	{
		CharSetConfig.GetIndex(ch, out int topIndex, out int btmIndex, out ulong mask);
		return data[topIndex].Contains(btmIndex, mask);
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<char> GetEnumerator()
	{
		return CharSetConfig.GetEnumerator(data);
	}

	#endregion // ReadOnlySetBase<char> 成员

	#region ISet<char> 成员

	/// <summary>
	/// 确定当前集合是否为指定集合的真子集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的真子集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsProperSubsetOf(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is ICharSet otherSet)
		{
			// CharSet 可以批量操作。
			return count < otherSet.Count && CharSetConfig.ContainsAllElements(otherSet.Data, data);
		}
		else if (count == 0)
		{
			return other.Any();
		}
		else
		{
			var (sameCount, unfoundCount) = CountElements(other, false);
			return sameCount == Count && unfoundCount > 0;
		}
	}

	/// <summary>
	/// 确定当前集合是否为指定集合的真超集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的真超集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsProperSupersetOf(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (count == 0)
		{
			return false;
		}
		if (other is ICharSet otherSet)
		{
			return count > otherSet.Count && CharSetConfig.ContainsAllElements(data, otherSet.Data);
		}
		else if (other is IRangeCollection<char> otherRange)
		{
			return count > otherRange.Count && CharSetConfig.ContainsAllElements(data, otherRange);
		}
		else
		{
			var (sameCount, unfoundCount) = CountElements(other, true);
			return sameCount < Count && unfoundCount == 0;
		}
	}

	/// <summary>
	/// 确定当前集合是否为指定集合的子集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的子集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsSubsetOf(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (count == 0)
		{
			return true;
		}
		if (other is ICharSet otherSet)
		{
			return count <= otherSet.Count && CharSetConfig.ContainsAllElements(otherSet.Data, data);
		}
		else
		{
			var (sameCount, unfoundCount) = CountElements(other, false);
			return sameCount == Count && unfoundCount >= 0;
		}
	}

	/// <summary>
	/// 确定当前集合是否为指定集合的超集合。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合是 <paramref name="other"/> 的超集合，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool IsSupersetOf(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (other is ICharSet otherSet)
		{
			return count >= otherSet.Count && CharSetConfig.ContainsAllElements(data, otherSet.Data);
		}
		else if (other is IRangeCollection<char> otherRange)
		{
			return count >= otherRange.Count && CharSetConfig.ContainsAllElements(data, otherRange);
		}
		else if (count == 0)
		{
			return !other.Any();
		}
		else
		{
			return other.All(Contains);
		}
	}

	/// <summary>
	/// 确定当前集合是否与指定的集合重叠。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合与 <paramref name="other"/> 
	/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool Overlaps(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (count == 0)
		{
			return false;
		}
		if (other is ICharSet otherSet)
		{
			for (int i = 0; i < CharSetConfig.TopLen; i++)
			{
				if (data[i].Overlaps(otherSet.Data[i]))
				{
					return true;
				}
			}
			return false;
		}
		else if (other is IRangeCollection<char> otherRange)
		{
			foreach (var (start, end) in otherRange.Ranges())
			{
				if (CharSetConfig.ContainsAny(data, start, end))
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			return other.Any(Contains);
		}
	}

	/// <summary>
	/// 确定当前集合与指定的集合中是否包含相同的元素。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <returns>如果当前集合等于 <paramref name="other"/>，则为 <c>true</c>；
	/// 否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override bool SetEquals(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (count == 0)
		{
			return !other.Any();
		}
		if (other is ICharSet otherSet)
		{
			return count == otherSet.Count && CharSetConfig.ContainsAllElements(data, otherSet.Data);
		}
		else if (other is IRangeCollection<char> otherRange)
		{
			return count == otherRange.Count && CharSetConfig.ContainsAllElements(data, otherRange);
		}
		else
		{
			var (sameCount, unfoundCount) = CountElements(other, true);
			return (sameCount == Count && unfoundCount == 0);
		}
	}

	#endregion // ISet<char> 成员

	#region IEquatable<CharSet> 成员

	/// <summary>
	/// 指示当前对象是否等于同一类型的另一个对象。
	/// </summary>
	/// <param name="other">一个与此对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="NotImplementedException"></exception>
	public bool Equals(ReadOnlyCharSet? other)
	{
		if (other is null)
		{
			return false;
		}
		return count == other.count && CharSetConfig.ContainsAllElements(data, other.data);
	}

	/// <summary>
	/// 确定指定对象是否等于当前对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果指定的对象等于当前对象，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is ReadOnlyCharSet set)
		{
			return Equals(set);
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 返回当前对象的哈希代码。
	/// </summary>
	/// <returns>当前对象的哈希代码。</returns>
	public override int GetHashCode()
	{
		return CharSetConfig.GetHashCode(data);
	}

	#endregion // IEquatable<CharSet> 成员

	/// <summary>
	/// 返回当前集合的字符串表示。
	/// </summary>
	/// <returns>当前集合的字符串表示。</returns>
	public override string ToString()
	{
		text ??= CharSetConfig.ToString(this);
		return text;
	}
}
