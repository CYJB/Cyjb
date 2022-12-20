using System.Data.Common;

namespace Cyjb.Text;

/// <summary>
/// 表示文本的范围。
/// </summary>
public readonly struct TextSpan : IComparable<TextSpan>, IEquatable<TextSpan>
{
	/// <summary>
	/// 返回恰好可以包含指定两个文本范围的最小范围。
	/// </summary>
	/// <param name="first">要组合的第一个文本范围。</param>
	/// <param name="second">要组合的第二个文本范围。</param>
	/// <returns>恰好可以包含 <paramref name="first"/> 和 <paramref name="second"/> 的文本范围。</returns>
	public static TextSpan Combine(TextSpan first, TextSpan second)
	{
		return new TextSpan(Math.Min(first.start, second.start), Math.Max(first.end, second.end));
	}

	/// <summary>
	/// 返回恰好可以包含指定多个文本范围的最小范围。
	/// </summary>
	/// <param name="spans">要组合的文本范围。</param>
	/// <returns>恰好可以包含 <paramref name="spans"/> 的文本范围。</returns>
	public static TextSpan Combine(params TextSpan[] spans)
	{
		if (spans.Length == 0)
		{
			return new TextSpan();
		}
		else if (spans.Length == 1)
		{
			return spans[0];
		}
		var (start, end) = spans[0];
		for (int i = 1; i < spans.Length; i++)
		{
			var (curStart, curEnd) = spans[i];
			if (curStart < start)
			{
				start = curStart;
			}
			if (curEnd > end)
			{
				end = curEnd;
			}
		}
		return new TextSpan(start, end);
	}

	/// <summary>
	/// 范围的起始位置。
	/// </summary>
	private readonly int start;
	/// <summary>
	/// 范围的结束位置。
	/// </summary>
	private readonly int end;

	/// <summary>
	/// 使用指定的起始和结束位置初始化。
	/// </summary>
	/// <param name="start">范围的起始位置。</param>
	/// <param name="end">范围的结束位置（不含）。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于 0。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> 小于 <paramref name="start"/>。</exception>
	public TextSpan(int start, int end)
	{
		if (start < 0)
		{
			throw CommonExceptions.ArgumentNegative(start);
		}
		if (end < start)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
		}
		this.start = start;
		this.end = end;
	}

	/// <summary>
	/// 范围的起始位置。
	/// </summary>
	public int Start
	{
		get => start;
		init => start = value;
	}
	/// <summary>
	/// 范围的结束位置（不含）。
	/// </summary>
	public int End
	{
		get => end;
		init => end = value;
	}
	/// <summary>
	/// 范围的长度。
	/// </summary>
	public int Length => end - start;
	/// <summary>
	/// 范围是否是空的。
	/// </summary>
	public bool IsEmpty => start == end;

	/// <summary>
	/// 解构当前文本范围。
	/// </summary>
	/// <param name="start">文本范围的起始位置（包含）。</param>
	/// <param name="end">文本范围的结束位置（不含）。</param>
	public void Deconstruct(out int start, out int end)
	{
		start = this.start;
		end = this.end;
	}

	/// <summary>
	/// 允许从 <see cref="Range"/> 隐式转换为 <see cref="TextSpan"/>。
	/// </summary>
	/// <param name="range">要转换的范围。</param>
	/// <returns>转换得到的 <see cref="TextSpan"/>。</returns>
	/// <exception cref="ArgumentException"><paramref name="range"/>
	/// 包含从结尾进行的索引。</exception>
	public static implicit operator TextSpan(Range range)
	{
		if(range.Start.IsFromEnd ||range.End.IsFromEnd)
		{
			throw new ArgumentException(Resources.RangeIndexFromEnd, nameof(range));
		}
		return new TextSpan(range.Start.Value, range.End.Value);
	}

	#region 范围操作

	/// <summary>
	/// 返回指定的位置是否包含在当前范围中。
	/// </summary>
	/// <param name="position">要检查的位置。</param>
	/// <returns>如果指定的位置包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <overloads>
	/// <summary>
	/// 返回指定的位置或范围是否包含在当前范围中。
	/// </summary>
	/// </overloads>
	public bool Contains(int position)
	{
		return position >= start && position < end;
	}

	/// <summary>
	/// 返回指定的 <see cref="TextSpan"/> 是否完全包含在当前范围中。
	/// </summary>
	/// <param name="span">要检查的范围。</param>
	/// <returns>如果指定的范围完全包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Contains(TextSpan span)
	{
		if (span.start >= start)
		{
			if (span.IsEmpty)
			{
				return span.end < end;
			}
			else
			{
				return span.end <= end;
			}
		}
		return false;
	}

	/// <summary>
	/// 返回指定的 <see cref="TextSpan"/> 是否与当前范围存在重叠。
	/// </summary>
	/// <param name="span">要检查的范围。</param>
	/// <returns>如果指定的范围与当前范围存在重叠，则为 <c>true</c>；否则为 <c>false</c>。
	/// 对于空的范围，也会返回 <c>false</c>。</returns>
	public bool OverlapsWith(TextSpan span)
	{
		int start = Math.Max(this.start, span.start);
		int end = Math.Min(this.end, span.end);
		return start < end;
	}

	/// <summary>
	/// 返回当前范围与指定 <see cref="TextSpan"/> 的重叠范围，如果不存在则为 <c>null</c>。
	/// </summary>
	/// <param name="span">要检查的范围。</param>
	/// <returns>当前范围与指定范围重叠的部分，如果不存在则为 <c>null</c>。</returns>
	public TextSpan? Overlap(TextSpan span)
	{
		int start = Math.Max(this.start, span.start);
		int end = Math.Min(this.end, span.end);
		if (start < end)
		{
			return new TextSpan(start, end);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// 返回指定的位置是否与当前范围相交。相交指的位置在当前范围内或者位于结束位置。
	/// </summary>
	/// <param name="position">要检查的位置。</param>
	/// <returns>如果指定的位置与当前范围相交，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <overloads>
	/// <summary>
	/// 返回指定的位置或范围是否与当前范围相交。
	/// </summary>
	/// </overloads>
	public bool IntersectsWith(int position)
	{
		return position >= start && position <= end;
	}

	/// <summary>
	/// 返回指定的 <see cref="TextSpan"/> 是否与当前范围相交。相交指的是两个范围存在重叠，
	/// 或者某个范围的结束位置与另一范围的起始位置相同。
	/// </summary>
	/// <param name="span">要检查的范围。</param>
	/// <returns>如果指定的范围与当前范围相交，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool IntersectsWith(TextSpan span)
	{
		return start <= span.end && span.start <= end;
	}

	/// <summary>
	/// 返回当前范围与指定 <see cref="TextSpan"/> 的相交范围，如果不存在则为 <c>null</c>。
	/// </summary>
	/// <param name="span">要检查的范围。</param>
	/// <returns>当前范围与指定范围相交的部分，如果不存在则为 <c>null</c>。</returns>
	public TextSpan? Intersection(TextSpan span)
	{
		int start = Math.Max(this.start, span.start);
		int end = Math.Min(this.end, span.end);
		if (start <= end)
		{
			return new TextSpan(start, end);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// 返回当前范围与指定 <see cref="TextSpan"/> 的合并范围，如果无法合并则为 <c>null</c>。
	/// </summary>
	/// <param name="span">要检查的范围。</param>
	/// <returns>当前范围与指定范围相交的合并，如果无法合并则为 <c>null</c>。</returns>
	public TextSpan? Union(TextSpan span)
	{
		int maxStart, maxEnd, minStart, minEnd;
		if (start < span.start)
		{
			maxStart = span.start;
			minStart = start;
		}
		else
		{
			maxStart = start;
			minStart = span.start;
		}
		if (end < span.end)
		{
			maxEnd = span.end;
			minEnd = end;
		}
		else
		{
			maxEnd = end;
			minEnd = span.end;
		}
		if (maxStart <= minEnd)
		{
			return new TextSpan(minStart, maxEnd);
		}
		else
		{
			return null;
		}
	}

	#endregion // 范围操作

	#region IComparable<TextSpan> 成员

	/// <summary>
	/// 将当前对象与同一类型的另一个对象进行比较。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
	public int CompareTo(TextSpan other)
	{
		int cmp = start - other.start;
		if (cmp != 0)
		{
			return cmp;
		}
		return end - other.end;
	}

	/// <summary>
	/// 返回一个 <see cref="TextSpan"/> 对象是否小于另一个 <see cref="TextSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 小于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator <(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) < 0;
	}

	/// <summary>
	/// 返回一个 <see cref="TextSpan"/> 对象是否小于等于另一个 <see cref="TextSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 小于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator <=(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) <= 0;
	}

	/// <summary>
	/// 返回一个 <see cref="TextSpan"/> 对象是否大于另一个 <see cref="TextSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 大于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator >(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) > 0;
	}

	/// <summary>
	/// 返回一个 <see cref="TextSpan"/> 对象是否大于等于另一个 <see cref="TextSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 大于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator >=(TextSpan left, TextSpan right)
	{
		return left.CompareTo(right) >= 0;
	}

	#endregion // IComparable<TextSpan> 成员

	#region IEquatable<TextSpan> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(TextSpan other)
	{
		return start == other.start && end == other.end;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is TextSpan other)
		{
			return Equals(other);
		}
		return false;
	}

	/// <summary>
	/// 返回当前对象的哈希值。
	/// </summary>
	/// <returns>当前对象的哈希值。</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(start, end);
	}

	/// <summary>
	/// 返回指定的 <see cref="TextSpan"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(TextSpan left, TextSpan right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="TextSpan"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(TextSpan left, TextSpan right)
	{
		return !left.Equals(right);
	}

	#endregion // IEquatable<TextSpan> 成员

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"[{start}..{end})";
	}
}
