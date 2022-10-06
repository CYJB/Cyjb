using System.Diagnostics;
using System.Globalization;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 表示字符的有序集合。
/// </summary>
/// <remarks><see cref="CharSet"/> 类采用类似位示图的树状位压缩数组存储字符，
/// 关于该数据结构的更多解释，请参见我的博文
/// <see href="http://www.cnblogs.com/cyjb/archive/p/BitCharSet.html">
/// 《基于树状位压缩数组的字符集合》</see>。</remarks>
/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/BitCharSet.html">
/// 《基于树状位压缩数组的字符集合》</seealso>
[Serializable, DebuggerDisplay("{ToString()} Count = {Count}")]
[DebuggerTypeProxy(typeof(CharSetDebugView))]
public sealed partial class CharSet : SetBase<char>, ICharSet, IEquatable<CharSet>, IRangeCollection<char>
{
	/// <summary>
	/// 字符集合的顶层数组。
	/// </summary>
	private readonly CharSetItem[] data;
	/// <summary>
	/// 集合中字符的个数。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int count = 0;
	/// <summary>
	/// 集合的字符串表示。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string? text = null;

	/// <summary>
	/// 从指定的范围字符串初始化 <see cref="CharSet"/> 类的新实例。
	/// </summary>
	/// <param name="ranges">范围字符串，字符串依次包含了每个字符范围的起止位置。</param>
	/// <returns><see cref="CharSet"/> 实例。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="ranges"/> 为 <c>null</c>。</exception>
	public static CharSet FromRange(string ranges)
	{
		ArgumentNullException.ThrowIfNull(ranges);
		CharSet set = new();
		for (int i = 1; i < ranges.Length; i += 2)
		{
			set.Add(ranges[i - 1], ranges[i]);
		}
		return set;
	}

	/// <summary>
	/// 初始化 <see cref="CharSet"/> 类的新实例。
	/// </summary>
	public CharSet() : base()
	{
		data = CharSetConfig.CreateData();
	}
	/// <summary>
	/// 使用指定的元素初始化 <see cref="CharSet"/> 类的新实例。
	/// </summary>
	/// <param name="collection">初始化的字符集合。</param>
	public CharSet(IEnumerable<char> collection) : this()
	{
		UnionWith(collection);
	}

	/// <summary>
	/// 字符集合的顶层数组。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	CharSetItem[] ICharSet.Data => data;

	/// <summary>
	/// 将当前集合标记为已改变。
	/// </summary>
	private void MarkDirty()
	{
		text = null;
	}

	/// <summary>
	/// 返回当前集合的只读包装器。
	/// </summary>
	/// <returns>当前集合的只读包装器。</returns>
	public ReadOnlyCharSet AsReadOnly()
	{
		return new ReadOnlyCharSet(this);
	}

	/// <summary>
	/// 将当前集合转换为只读集合，当前集合的数据不再可用。
	/// </summary>
	/// <returns>转换后的只读集合。</returns>
	internal ReadOnlyCharSet MoveReadOnly()
	{
		return new ReadOnlyCharSet(data, count, text);
	}

	/// <summary>
	/// 将指定字符范围添加到当前集合中。
	/// </summary>
	/// <param name="start">要添加到当前集合的字符范围起始（包含）。</param>
	/// <param name="end">要添加到当前集合的字符范围结束（包含）。</param>
	/// <returns>如果该字符范围内的<b>任何</b>字符已添加到集合内，则为 <c>true</c>；
	/// 如果该字符范围已全部在集合内，则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentOutOfRangeException">字符范围的起始大于结束。</exception>
	public bool Add(char start, char end)
	{
		if (start > end)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
		}
		int oldCount = count;
		CharSetConfig.GetIndex(start, out int startTopIndex, out int startBtmIndex, out ulong startMask);
		CharSetConfig.GetIndex(end, out int endTopIndex, out int endBtmIndex, out ulong endMask);
		if (startTopIndex == endTopIndex && startBtmIndex == endBtmIndex)
		{
			// start 和 end 位于同一个底层数组项中，将 start ~ end 之间二进制置 1
			ulong mask = (endMask - startMask) + endMask;
			count += data[startTopIndex].Fill(startBtmIndex, mask);
			if (count > oldCount)
			{
				MarkDirty();
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			// 将 start ~ max 之间按位置 1。
			ulong mask = ~startMask + 1UL;
			count += data[startTopIndex].Fill(startBtmIndex, mask);
			// 将 0 ~ end 之间按位置 1。
			mask = (endMask - 1UL) + endMask;
			count += data[endTopIndex].Fill(endBtmIndex, mask);
		}
		if (startTopIndex == endTopIndex)
		{
			// 将 startBtmIndex ~ endBtmIndex 之间按位置 1。
			count += data[startTopIndex].FillRange(startBtmIndex + 1, endBtmIndex);
		}
		else
		{
			// 将 startBtmIndex ~ BtmLen 之间按位置 1。
			count += data[startTopIndex].FillRange(startBtmIndex + 1, CharSetConfig.BtmLen);
			// 将 0 ~ endBtmIndex 之间按位置 1。
			count += data[endTopIndex].FillRange(0, endBtmIndex);
			// 将 startTopIndex ~ startTopIndex 之间按位置 1。
			for (int i = startTopIndex + 1; i < endTopIndex; i++)
			{
				count += data[i].Fill();
			}
		}
		if (count > oldCount)
		{
			MarkDirty();
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 向当前集内添加字符及其相应的大/小写字母，并返回一个指示是否已成功添加字符的值。
	/// </summary>
	/// <param name="ch">要添加到 <see cref="CharSet"/> 的中的字符。</param>
	/// <param name="culture">大小写转换使用的区域信息。</param>
	/// <returns>如果该字符的任意大/小写已添加到集内，则为 <c>true</c>；
	/// 如果该字符的全部大小写都已在集内，则为 <c>false</c>。</returns>
	public bool AddIgnoreCase(char ch, CultureInfo? culture = null)
	{
		if (culture == null)
		{
			culture = CultureInfo.InvariantCulture;
		}
		int oldCount = count;
		Add(ch);
		TextInfo textInfo = culture.TextInfo;
		switch (char.GetUnicodeCategory(ch))
		{
			case UnicodeCategory.TitlecaseLetter:
				Add(textInfo.ToLower(ch));
				break;
			case UnicodeCategory.LowercaseLetter:
				Add(textInfo.ToUpper(ch));
				break;
			case UnicodeCategory.UppercaseLetter:
			// UppercaseLetter 存在 ToUpper 不是自身的情况
			// \u01C5 \u01C8 \u01CB \u01F2
			case UnicodeCategory.NonSpacingMark:
			case UnicodeCategory.LetterNumber:
			case UnicodeCategory.OtherSymbol:
				Add(textInfo.ToUpper(ch));
				Add(textInfo.ToLower(ch));
				break;
		}
		return count > oldCount;
	}

	/// <summary>
	/// 将指定字符范围及其相应的大/小写字母添加到当前集合中。
	/// </summary>
	/// <param name="start">要添加到当前集合的字符范围起始（包含）。</param>
	/// <param name="end">要添加到当前集合的字符范围结束（包含）。</param>
	/// <param name="culture">大小写转换使用的区域信息。</param>
	/// <returns>如果该字符范围内的<b>任何</b>大/小写字符已添加到集合内，则为 <c>true</c>；
	/// 如果该字符范围已全部在集合内，则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentOutOfRangeException">字符范围的起始大于结束。</exception>
	public bool AddIgnoreCase(char start, char end, CultureInfo? culture = null)
	{
		if (start > end)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
		}
		if (culture == null)
		{
			culture = CultureInfo.InvariantCulture;
		}
		int oldCount = count;
		Add(start, end);
		CaseConverter.GetLowercaseConverter(culture).ConvertRange(start, end, this);
		CaseConverter.GetUppercaseConverter(culture).ConvertRange(start, end, this);
		return count > oldCount;
	}

	/// <summary>
	/// 将当前集合内大写字母对应的小写字母添加到集合中。
	/// </summary>
	/// <param name="culture">大小写转换使用的区域信息。</param>
	public void AddLowercase(CultureInfo? culture = null)
	{
		if (culture == null)
		{
			culture = CultureInfo.InvariantCulture;
		}
		CaseConverter converter = CaseConverter.GetLowercaseConverter(culture);
		for (int i = 0; i < CharSetConfig.TopLen; i++)
		{
			foreach (var (start, end) in data[i])
			{
				if (start == end)
				{
					Add(converter.ConvertChar(start));
				}
				else
				{
					converter.ConvertRange(start, end, this);
				}
			}
		}
	}

	/// <summary>
	/// 将当前集合内小写字母对应的大写字母添加到集合中。
	/// </summary>
	/// <param name="culture">大小写转换使用的区域信息。</param>
	public void AddUppercase(CultureInfo? culture = null)
	{
		if (culture == null)
		{
			culture = CultureInfo.InvariantCulture;
		}
		CaseConverter converter = CaseConverter.GetUppercaseConverter(culture);
		for (int i = 0; i < CharSetConfig.TopLen; i++)
		{
			foreach (var (start, end) in data[i])
			{
				if (start == end)
				{
					Add(converter.ConvertChar(start));
				}
				else
				{
					converter.ConvertRange(start, end, this);
				}
			}
		}
	}

	/// <summary>
	/// 从当前集合中移除指定字符范围。
	/// </summary>
	/// <param name="start">要从当前集合中移除的字符范围起始（包含）。</param>
	/// <param name="end">要从当前集合中移除的字符范围结束（包含）。</param>
	/// <returns>如果已从当前集合中成功移除该字符范围内的<b>任何</b>字符，则为 <c>true</c>；否则为 <c>false</c>。
	/// 如果在原始当前集合中没有找到字符范围内的字符，该方法也会返回 <c>false</c>。</returns>
	/// <exception cref="ArgumentOutOfRangeException">字符范围的起始大于结束。</exception>
	public bool Remove(char start, char end)
	{
		if (start > end)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
		}
		int oldCount = count;
		CharSetConfig.GetIndex(start, out int startTopIndex, out int startBtmIndex, out ulong startMask);
		CharSetConfig.GetIndex(end, out int endTopIndex, out int endBtmIndex, out ulong endMask);
		if (startTopIndex == endTopIndex && startBtmIndex == endBtmIndex)
		{
			// start 和 end 位于同一个底层数组项中，将 start ~ end 之间按位置 0
			ulong mask = (endMask - startMask) + endMask;
			count += data[startTopIndex].Clear(startBtmIndex, mask);
			if (count < oldCount)
			{
				MarkDirty();
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			// 将 start ~ max 之间按位置 0。
			ulong mask = ~startMask + 1UL;
			count += data[startTopIndex].Clear(startBtmIndex, mask);
			// 将 0 ~ end 之间按位置 0。
			mask = (endMask - 1UL) + endMask;
			count += data[endTopIndex].Clear(endBtmIndex, mask);
		}
		if (startTopIndex == endTopIndex)
		{
			// 将 startBtmIndex ~ endBtmIndex 之间按位置 0。
			count += data[startTopIndex].ClearRange(startBtmIndex + 1, endBtmIndex);
		}
		else
		{
			// 将 startBtmIndex ~ BtmLen 之间二进制置 0。
			count += data[startTopIndex].ClearRange(startBtmIndex + 1, CharSetConfig.BtmLen);
			// 将 0 ~ endBtmIndex 之间按位置 0。
			count += data[endTopIndex].ClearRange(0, endBtmIndex);
			// 将 startTopIndex ~ startTopIndex 之间按位置 0。
			for (int i = startTopIndex + 1; i < endTopIndex; i++)
			{
				count += data[i].Clear();
			}
		}
		if (count < oldCount)
		{
			MarkDirty();
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// 反转当前集合，使集合内容变为之前未包含的字符。
	/// </summary>
	public void Negated()
	{
		for (int i = 0; i < CharSetConfig.TopLen; i++)
		{
			count += data[i].Negated();
		}
		MarkDirty();
	}

	/// <summary>
	/// 返回当前字符集合包含的范围个数。
	/// </summary>
	/// <returns>当前字符集合包含的范围个数。</returns>
	/// <remarks>与 <c>CharSet.Ranges().Count()</c> 等价，但效率更高。</remarks>
	public int RangeCount()
	{
		return CharSetConfig.RangeCount(data);
	}

	/// <summary>
	/// 返回一个循环访问字符范围的枚举器。
	/// </summary>
	/// <returns>可用于循环访问字符范围的 <see cref="IEnumerable{Tuple}"/>。</returns>
	public IEnumerable<ValueRange<char>> Ranges()
	{
		return CharSetConfig.Ranges(data);
	}

	#region SetBase<char> 成员

	/// <summary>
	/// 获取当前集合包含的字符数。
	/// </summary>
	/// <value>当前集合中包含的字符数。</value>
	public override int Count => count;

	/// <summary>
	/// 向当前集内添加字符，并返回一个指示是否已成功添加字符的值。
	/// </summary>
	/// <param name="ch">要添加到 <see cref="CharSet"/> 的中的字符。</param>
	/// <returns>如果该字符已添加到集内，则为 <c>true</c>；如果该字符已在集内，则为 <c>false</c>。</returns>
	public override bool Add(char ch)
	{
		CharSetConfig.GetIndex(ch, out int topIndex, out int btmIndex, out ulong mask);
		int modified = data[topIndex].FillSingle(btmIndex, mask);
		if (modified == 0)
		{
			return false;
		}
		else
		{
			MarkDirty();
			count += modified;
			return true;
		}
	}

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
	/// 从当前集合中移除指定字符。
	/// </summary>
	/// <param name="ch">要从当前集合中移除的字符。</param>
	/// <returns>如果已从当前集合中成功移除 <paramref name="ch"/>，则为 <c>true</c>；否则为 <c>false</c>。
	/// 如果在原始当前集合中没有找到 <paramref name="ch"/>，该方法也会返回 <c>false</c>。</returns>
	public override bool Remove(char ch)
	{
		CharSetConfig.GetIndex(ch, out int topIndex, out int btmIndex, out ulong mask);
		int modified = data[topIndex].ClearSingle(btmIndex, mask);
		if (modified == 0)
		{
			return false;
		}
		else
		{
			count += modified;
			MarkDirty();
			return false;
		}
	}

	/// <summary>
	/// 从当前集合中移除所有字符。
	/// </summary>
	public override void Clear()
	{
		if (count == 0)
		{
			return;
		}
		count = 0;
		for (int i = 0; i < CharSetConfig.TopLen; i++)
		{
			data[i].Clear();
		}
		MarkDirty();
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<char> GetEnumerator()
	{
		return CharSetConfig.GetEnumerator(data);
	}

	#endregion // SetBase<char> 成员

	#region ISet<char> 成员

	/// <summary>
	/// 从当前集合内移除指定集合中的所有元素。
	/// </summary>
	/// <param name="other">要从集合内移除的项的集合。</param>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override void ExceptWith(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (count <= 0)
		{
			return;
		}
		if (ReferenceEquals(this, other))
		{
			Clear();
			return;
		}
		if (other is ICharSet otherSet)
		{
			// ICharSet 可以批量操作。
			int oldCount = count;
			for (int i = 0; i < CharSetConfig.TopLen; i++)
			{
				count += data[i].ExceptWith(otherSet.Data[i]);
			}
			if (count != oldCount)
			{
				MarkDirty();
			}
		}
		else if (other is IRangeCollection<char> otherRange)
		{
			foreach (var (start, end) in otherRange.Ranges())
			{
				Remove(start, end);
			}
		}
		else
		{
			foreach (char ch in other)
			{
				Remove(ch);
			}
		}
	}

	/// <summary>
	/// 修改当前集合，使当前集合仅包含指定集合中也存在的元素。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>internal 
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override void IntersectWith(IEnumerable<char> other)
	{
		if (other is ICharSet otherSet)
		{
			// ICharSet 可以批量操作。
			int oldCount = count;
			for (int i = 0; i < CharSetConfig.TopLen; i++)
			{
				count += data[i].IntersectWith(otherSet.Data[i]);
			}
			if (count != oldCount)
			{
				MarkDirty();
			}
		}
		else if (other is IRangeCollection<char> otherRange)
		{
			// 移除不在 otherRange 里的字符范围。
			char begin = '\0';
			foreach (var (start, end) in otherRange.Ranges())
			{
				if (begin < start)
				{
					Remove(begin, (char)(start - 1));
				}
				begin = (char)(end + 1);
			}
			if (begin < char.MaxValue)
			{
				Remove(begin, char.MaxValue);
			}
		}
		else
		{
			base.IntersectWith(other);
		}
	}

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
			// ICharSet 可以批量操作。
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

	/// <summary>
	/// 修改当前集合，使该集合仅包含当前集合或指定集合中存在的元素（但不可包含两者共有的元素）。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override void SymmetricExceptWith(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (count == 0)
		{
			UnionWith(other);
			return;
		}
		else if (ReferenceEquals(this, other))
		{
			Clear();
			return;
		}
		if (other is not ICharSet otherSet)
		{
			otherSet = new CharSet(other);
		}
		for (int i = 0; i < CharSetConfig.TopLen; i++)
		{
			count += data[i].SymmetricExceptWith(otherSet.Data[i]);
		}
		// 这里不对比 count，因为字符可能增删。
		MarkDirty();
	}

	/// <summary>
	/// 修改当前集合，使该集合包含当前集合和指定集合中同时存在的所有元素。
	/// </summary>
	/// <param name="other">要与当前集合进行比较的集合。</param>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public override void UnionWith(IEnumerable<char> other)
	{
		ArgumentNullException.ThrowIfNull(other);
		if (ReferenceEquals(this, other))
		{
			return;
		}
		if (other is ICharSet otherSet)
		{
			// ICharSet 可以批量操作。
			int oldCount = count;
			for (int i = 0; i < CharSetConfig.TopLen; i++)
			{
				count += data[i].UnionWith(otherSet.Data[i]);
			}
			if (count != oldCount)
			{
				MarkDirty();
			}
		}
		else if (other is IRangeCollection<char> otherRange)
		{
			foreach (var (start, end) in otherRange.Ranges())
			{
				Add(start, end);
			}
		}
		else
		{
			foreach (char ch in other)
			{
				Add(ch);
			}
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
	public bool Equals(CharSet? other)
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
		if (obj is CharSet set)
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
		if (text == null)
		{
			text = CharSetConfig.ToString(this);
		}
		return text;
	}
}
