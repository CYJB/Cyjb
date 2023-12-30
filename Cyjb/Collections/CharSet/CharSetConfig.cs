namespace Cyjb.Collections;

/// <summary>
/// 字符集合的配置。
/// </summary>
internal static partial class CharSetConfig
{
	/// <summary>
	/// <see cref="char"/> 的字节数。
	/// </summary>
	private const int CharBitLength = 16;
	/// <summary>
	/// 数组项可以存储的字节数，<see cref="ulong"/> 可以存储 64 种情况（2^6）。
	/// </summary>
	private const int ItemBitLength = 6;
	/// <summary>
	/// 顶层数组的比特长度。
	/// </summary>
	private const int TopBitLength = 6;
	/// <summary>
	/// 底层数组的比特长度。
	/// </summary>
	public const int BottomBitLength = CharBitLength - TopBitLength - ItemBitLength;
	/// <summary>
	/// 顶层数组的长度。
	/// </summary>
	public const int TopLen = 1 << TopBitLength;
	/// <summary>
	/// 顶层数组索引的位移。
	/// </summary>
	public const int TopShift = CharBitLength - TopBitLength;
	/// <summary>
	/// 底层数组的长度。
	/// </summary>
	public const int BtmLen = 1 << BottomBitLength;
	/// <summary>
	/// 每个底层数组包含的字符个数。
	/// </summary>
	public const int BtmCount = BtmLen << ItemBitLength;
	/// <summary>
	/// 底层数组索引的位移。
	/// </summary>
	public const int BtmShift = TopShift - BottomBitLength;
	/// <summary>
	/// 底层数组部分的掩码。
	/// </summary>
	public const int BtmMask = (1 << TopShift) - 1;
	/// <summary>
	/// 底层数组的索引的掩码。
	/// </summary>
	public const int IndexMask = (1 << BtmShift) - 1;

	/// <summary>
	/// 创建新的字符集合数据。
	/// </summary>
	/// <returns>新的字符集合数据。</returns>
	public static CharSetItem[] CreateData()
	{
		return new CharSetItem[TopLen].Fill((index) => new CharSetItem(index << TopShift));
	}

	/// <summary>
	/// 获取指定字符对应的索引和掩码。
	/// </summary>
	/// <param name="ch">要获取索引和掩码的字符。</param>
	/// <param name="topIndex">顶层数组的索引。</param>
	/// <param name="btmIndex">底层数组的索引。</param>
	/// <param name="binMask">数据的二进制掩码。</param>
	public static void GetIndex(char ch, out int topIndex, out int btmIndex, out ulong binMask)
	{
		topIndex = ch >> TopShift;
		btmIndex = (ch & BtmMask) >> BtmShift;
		binMask = 1UL << (ch & IndexMask);
	}

	/// <summary>
	/// 返回指定字符集数据合包含的范围个数。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <returns><paramref name="data"/> 合包含的范围个数。</returns>
	/// <remarks>与 <c>Ranges().Count()</c> 等价，但效率更高。</remarks>
	public static int RangeCount(CharSetItem[] data)
	{
		int pointCount = 0;
		ulong lastBit = 0UL;
		for (int i = 0; i < TopLen; i++)
		{
			pointCount += data[i].PointCount(ref lastBit);
		}
		return (pointCount + 1) >> 1;
	}

	/// <summary>
	/// 返回一个循环访问字符范围的枚举器。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <returns>可用于循环访问字符范围的 <see cref="IEnumerable{Tuple}"/>。</returns>
	public static IEnumerable<ValueRange<char>> Ranges(CharSetItem[] data)
	{
		bool hasRange = false;
		char start = '\0', end = '\0';
		for (int i = 0; i < TopLen; i++)
		{
			foreach (var (curStart, curEnd) in data[i])
			{
				if (hasRange)
				{
					if (end < curStart - 1)
					{
						yield return new ValueRange<char>(start, end);
						start = curStart;
					}
				}
				else
				{
					start = curStart;
				}
				end = curEnd;
				hasRange = true;
			}
		}
		if (hasRange)
		{
			yield return new ValueRange<char>(start, end);
		}
	}

	/// <summary>
	/// 确定指定字符集合数据是否包含指定范围内的全部字符。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <param name="start">要在 <paramref name="data"/> 中定位的字符起始范围。</param>
	/// <param name="end">要在 <paramref name="data"/> 中定位的字符结束范围。</param>
	/// <returns>如果在 <paramref name="data"/> 中找到范围内的全部字符，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentOutOfRangeException">字符范围的起始大于结束。</exception>
	public static bool Contains(CharSetItem[] data, char start, char end)
	{
		if (start > end)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
		}
		GetIndex(start, out int startTopIndex, out int startBtmIndex, out ulong startMask);
		GetIndex(end, out int endTopIndex, out int endBtmIndex, out ulong endMask);
		if (startTopIndex == endTopIndex && startBtmIndex == endBtmIndex)
		{
			// start 和 end 位于同一个底层数组项中，检查 start ~ end 范围。
			ulong mask = (endMask - startMask) + endMask;
			return data[startTopIndex].Contains(startBtmIndex, mask);
		}
		else
		{
			// 检查 start ~ max 范围。
			ulong mask = ~startMask + 1UL;
			if (!data[startTopIndex].Contains(startBtmIndex, mask))
			{
				return false;
			}
			// 检查 0 ~ end 范围。
			mask = (endMask - 1UL) + endMask;
			if (!data[endTopIndex].Contains(endBtmIndex, mask))
			{
				return false;
			}
		}
		if (startTopIndex == endTopIndex)
		{
			// 检查 startBtmIndex ~ endBtmIndex 范围。
			if (!data[startTopIndex].ContainsRange(startBtmIndex + 1, endBtmIndex))
			{
				return false;
			}
		}
		else
		{
			// 检查 startBtmIndex ~ BtmLen 范围。
			if (!data[startTopIndex].ContainsRange(startBtmIndex + 1, BtmLen))
			{
				return false;
			}
			// 将 0 ~ endBtmIndex 之间按位置 1。
			if (!data[endTopIndex].ContainsRange(0, endBtmIndex))
			{
				return false;
			}
			// 检查 startTopIndex ~ startTopIndex 范围。
			for (int i = startTopIndex + 1; i < endTopIndex; i++)
			{
				if (!data[i].ContainsRange(0, BtmLen))
				{
					return false;
				}
			}
		}
		return true;
	}

	/// <summary>
	/// 确定指定字符集合数据是否包含指定范围内的任意字符。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <param name="start">要在 <paramref name="data"/> 中定位的字符起始范围。</param>
	/// <param name="end">要在 <paramref name="data"/> 中定位的字符结束范围。</param>
	/// <returns>如果在 <paramref name="data"/> 中找到范围内的任意字符，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentOutOfRangeException">字符范围的起始大于结束。</exception>
	public static bool ContainsAny(CharSetItem[] data, char start, char end)
	{
		if (start > end)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
		}
		GetIndex(start, out int startTopIndex, out int startBtmIndex, out ulong startMask);
		GetIndex(end, out int endTopIndex, out int endBtmIndex, out ulong endMask);
		if (startTopIndex == endTopIndex && startBtmIndex == endBtmIndex)
		{
			// start 和 end 位于同一个底层数组项中，检查 start ~ end 范围。
			ulong mask = (endMask - startMask) + endMask;
			return data[startTopIndex].ContainsAny(startBtmIndex, mask);
		}
		else
		{
			// 检查 start ~ max 范围。
			ulong mask = ~startMask + 1UL;
			if (data[startTopIndex].ContainsAny(startBtmIndex, mask))
			{
				return true;
			}
			// 检查 0 ~ end 范围。
			mask = (endMask - 1UL) + endMask;
			if (data[endTopIndex].ContainsAny(endBtmIndex, mask))
			{
				return true;
			}
		}
		if (startTopIndex == endTopIndex)
		{
			// 检查 startBtmIndex ~ endBtmIndex 范围。
			if (data[startTopIndex].ContainsAnyRange(startBtmIndex + 1, endBtmIndex))
			{
				return true;
			}
		}
		else
		{
			// 检查 startBtmIndex ~ BtmLen 范围。
			if (data[startTopIndex].ContainsAnyRange(startBtmIndex + 1, BtmLen))
			{
				return true;
			}
			// 将 0 ~ endBtmIndex 之间按位置 1。
			if (data[endTopIndex].ContainsAnyRange(0, endBtmIndex))
			{
				return true;
			}
			// 检查 startTopIndex ~ startTopIndex 范围。
			for (int i = startTopIndex + 1; i < endTopIndex; i++)
			{
				if (data[i].ContainsAnyRange(0, BtmLen))
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 确定当前字符集合数据是否包含指定的字符集合数据中的所有字符。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <param name="other">要进行比较的字符集合的数据。</param>
	/// <returns>如果 <paramref name="data"/> 包含 <paramref name="other"/> 中的所有字符，则返回 
	/// <c>true</c>，否则返回 <c>false</c>。</returns>
	public static bool ContainsAllElements(CharSetItem[] data, CharSetItem[] other)
	{
		for (int i = 0; i < TopLen; i++)
		{
			if (!data[i].ContainsAllElements(other[i]))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 确定指定字符集合数据是否包含指定的字符范围中的所有字符。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <param name="other">要与字符集合数据进行比较的字符范围。</param>
	/// <returns>如果 <paramref name="data"/> 包含 <paramref name="other"/> 中的所有字符，则返回 
	/// <c>true</c>，否则返回 <c>false</c>。</returns>
	public static bool ContainsAllElements(CharSetItem[] data, IRangeCollection<char> other)
	{
		foreach (var (start, end) in other.Ranges())
		{
			if (!Contains(data, start, end))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 返回指定字符集合数据的枚举器。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <returns>可用于循环访问集合数据的 <see cref="IEnumerator{Char}"/>。</returns>
	public static IEnumerator<char> GetEnumerator(CharSetItem[] data)
	{
		for (int i = 0; i < TopLen; i++)
		{
			foreach (var (start, end) in data[i])
			{
				// 避免 end 正好是 char.MaxValue 时导致死循环。
				for (char ch = start; ch < end; ch++)
				{
					yield return ch;
				}
				yield return end;
			}
		}
	}

	/// <summary>
	/// 返回指定字符集合数据的哈希代码。
	/// </summary>
	/// <param name="data">字符集合的数据。</param>
	/// <returns>指定字符集合数据的哈希代码。</returns>
	public static int GetHashCode(CharSetItem[] data)
	{
		HashCode hashCode = new();
		for (int i = 0; i < TopLen; i++)
		{
			hashCode.Add(data[i]);
		}
		return hashCode.ToHashCode();
	}
}
