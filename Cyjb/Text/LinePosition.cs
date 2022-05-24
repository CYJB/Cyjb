namespace Cyjb.Text;

/// <summary>
/// 表示行位置。
/// </summary>
public struct LinePosition : IComparable<LinePosition>, IEquatable<LinePosition>
{
	/// <summary>
	/// 行号。
	/// </summary>
	private readonly int line;
	/// <summary>
	/// 行内的字符位置。
	/// </summary>
	private readonly int character;
	/// <summary>
	/// 列号。
	/// </summary>
	private readonly int column;

	/// <summary>
	/// 使用的行号的字符位置初始化。
	/// </summary>
	/// <param name="line">行号。</param>
	/// <param name="character">行内的字符位置。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="line"/> 小于 <c>0</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="character"/> 小于 <c>0</c>。</exception>
	public LinePosition(int line, int character) : this(line, character, character) { }

	/// <summary>
	/// 使用的行号、字符位置和列号初始化。
	/// </summary>
	/// <param name="line">行号。</param>
	/// <param name="character">行内的字符位置。</param>
	/// <param name="column">行号。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="line"/> 小于 <c>0</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="character"/> 小于 <c>0</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="column"/> 小于 <c>0</c>。</exception>
	public LinePosition(int line, int character, int column)
	{
		if (line < 0)
		{
			throw CommonExceptions.ArgumentNegative(line);
		}
		if (character < 0)
		{
			throw CommonExceptions.ArgumentNegative(character);
		}
		if (column < 0)
		{
			throw CommonExceptions.ArgumentNegative(column);
		}
		this.line = line;
		this.character = character;
		this.column = column;
	}

	/// <summary>
	/// 行号。
	/// </summary>
	public int Line => line;
	/// <summary>
	/// 列号。
	/// </summary>
	public int Column => column;
	/// <summary>
	/// 行内的字符位置，不参与比较。
	/// </summary>
	public int Character => character;

	#region IComparable<LinePosition> 成员

	/// <summary>
	/// 将当前对象与同一类型的另一个对象进行比较。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
	public int CompareTo(LinePosition other)
	{
		int cmp = line - other.line;
		if (cmp != 0)
		{
			return cmp;
		}
		return character - other.character;
	}

	/// <summary>
	/// 返回一个 <see cref="LinePosition"/> 对象是否小于另一个 <see cref="LinePosition"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 小于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator <(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) < 0;
	}

	/// <summary>
	/// 返回一个 <see cref="LinePosition"/> 对象是否小于等于另一个 <see cref="LinePosition"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 小于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator <=(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) <= 0;
	}

	/// <summary>
	/// 返回一个 <see cref="LinePosition"/> 对象是否大于另一个 <see cref="LinePosition"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 大于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator >(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) > 0;
	}

	/// <summary>
	/// 返回一个 <see cref="LinePosition"/> 对象是否大于等于另一个 <see cref="LinePosition"/> 对象。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 大于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator >=(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) >= 0;
	}

	#endregion // IComparable<LinePosition> 成员

	#region IEquatable<LinePosition> 成员

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(LinePosition other)
	{
		return line == other.line && character == other.character;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is LinePosition other)
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
		return HashCode.Combine(line, character);
	}

	/// <summary>
	/// 返回指定的 <see cref="LinePosition"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(LinePosition left, LinePosition right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="LinePosition"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(LinePosition left, LinePosition right)
	{
		return !left.Equals(right);
	}

	#endregion // IEquatable<LinePosition> 成员

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return line + "," + character;
	}
}
