namespace Cyjb.Text;

/// <summary>
/// 表示行位置的范围。
/// </summary>
public struct LinePositionSpan : IComparable<LinePositionSpan>, IEquatable<LinePositionSpan>
{
	/// <summary>
	/// 起始行位置。
	/// </summary>
	private readonly LinePosition start;
	/// <summary>
	/// 结束起始行位置（不含）。
	/// </summary>
	private readonly LinePosition end;

	/// <summary>
	/// 使用指定的起始和结束位置初始化。
	/// </summary>
	/// <param name="start">起始行位置。</param>
	/// <param name="end">结束起始行位置（不含）。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> 小于 <paramref name="start"/>。</exception>
	public LinePositionSpan(LinePosition start, LinePosition end)
	{
		if (end < start)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
		}
		this.start = start;
		this.end = end;
	}

	/// <summary>
	/// 起始行位置。
	/// </summary>
	public LinePosition Start => start;
	/// <summary>
	/// 结束起始行位置（不含）。
	/// </summary>
	public LinePosition End => end;

	#region IComparable<LinePositionSpan> 成员

	/// <summary>
	/// 将当前对象与同一类型的另一个对象进行比较。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
	public int CompareTo(LinePositionSpan other)
	{
		int cmp = start.CompareTo(other.start);
		if (cmp != 0)
		{
			return cmp;
		}
		return end.CompareTo(other.end);
	}

	/// <summary>
	/// 返回一个 <see cref="LinePositionSpan"/> 对象是否小于另一个 <see cref="LinePositionSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 小于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator <(LinePositionSpan left, LinePositionSpan right)
	{
		return left.CompareTo(right) < 0;
	}

	/// <summary>
	/// 返回一个 <see cref="LinePositionSpan"/> 对象是否小于等于另一个 <see cref="LinePositionSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 小于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator <=(LinePositionSpan left, LinePositionSpan right)
	{
		return left.CompareTo(right) <= 0;
	}

	/// <summary>
	/// 返回一个 <see cref="LinePositionSpan"/> 对象是否大于另一个 <see cref="LinePositionSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 大于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator >(LinePositionSpan left, LinePositionSpan right)
	{
		return left.CompareTo(right) > 0;
	}

	/// <summary>
	/// 返回一个 <see cref="LinePositionSpan"/> 对象是否大于等于另一个 <see cref="LinePositionSpan"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 大于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator >=(LinePositionSpan left, LinePositionSpan right)
	{
		return left.CompareTo(right) >= 0;
	}

	#endregion // IComparable<LinePositionSpan> 成员

	#region IEquatable<LinePositionSpan> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(LinePositionSpan other)
	{
		return start.Equals(other) && end.Equals(other);
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is LinePositionSpan other)
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
	/// 返回指定的 <see cref="LinePositionSpan"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(LinePositionSpan left, LinePositionSpan right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="LinePositionSpan"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(LinePositionSpan left, LinePositionSpan right)
	{
		return !left.Equals(right);
	}

	#endregion // IEquatable<LinePositionSpan> 成员

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return $"[{start}-{end})";
	}

}
