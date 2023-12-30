using System.Globalization;

namespace Cyjb;

public readonly partial struct StringView : IEquatable<StringView>, IEquatable<string>, IComparable, IComparable<StringView>, IComparable<string>
{

	#region IEquatable

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>比较时使用区分大小写和不区分区域性的序列比较。</remarks>
	public bool Equals(StringView other)
	{
		if (length != other.length)
		{
			return false;
		}
		return string.Compare(text, start, other.text, other.start, length, StringComparison.Ordinal) == 0;
	}

	/// <summary>
	/// 返回当前对象是否等于同一类型的另一对象。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <param name="comparisonType">比较时使用的规则。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(StringView other, StringComparison comparisonType)
	{
		if (length != other.length)
		{
			return false;
		}
		return string.Compare(text, start, other.text, other.start, length, comparisonType) == 0;
	}

	/// <summary>
	/// 返回当前对象是否等于另一字符串。
	/// </summary>
	/// <param name="other">要比较的字符串。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>比较时使用区分大小写和不区分区域性的序列比较。</remarks>
	public bool Equals(string? other)
	{
		if (other is null)
		{
			return false;
		}
		if (length != other.Length)
		{
			return false;
		}
		return string.Compare(text, start, other, 0, length, StringComparison.Ordinal) == 0;
	}

	/// <summary>
	/// 返回当前对象是否等于另一字符串。
	/// </summary>
	/// <param name="other">要比较的字符串。</param>
	/// <param name="comparisonType">比较时使用的规则。</param>
	/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Equals(string? other, StringComparison comparisonType)
	{
		if (other is null)
		{
			return false;
		}
		if (length != other.Length)
		{
			return false;
		}
		return string.Compare(text, start, other, 0, length, comparisonType) == 0;
	}

	/// <summary>
	/// 返回当前对象是否等于另一对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
	public override bool Equals(object? obj)
	{
		if (obj is StringView other)
		{
			return Equals(other);
		}
		else if (obj is string text)
		{
			return Equals(text);
		}
		return false;
	}

	/// <summary>
	/// 返回当前对象的哈希值。
	/// </summary>
	/// <returns>当前对象的哈希值。</returns>
	public override int GetHashCode()
	{
		return string.GetHashCode(AsSpan());
	}

	/// <summary>
	/// 返回指定的 <see cref="StringView"/> 是否相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator ==(StringView left, StringView right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// 返回指定的 <see cref="StringView"/> 是否不相等。
	/// </summary>
	/// <param name="left">要比较的第一个对象。</param>
	/// <param name="right">要比较的第二个对象。</param>
	/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool operator !=(StringView left, StringView right)
	{
		return !left.Equals(right);
	}

	#endregion // IEquatable

	#region IComparable

	/// <summary>
	/// 比较两个字符串视图。
	/// </summary>
	/// <param name="viewA">要比较的第一个字符串视图。</param>
	/// <param name="viewB">要比较的第二个字符串视图。</param>
	/// <param name="culture">比较时使用的区域特性。</param>
	/// <param name="options">比较时使用的选项。</param>
	/// <returns>一个值，指示要比较的字符串视图的相对顺序。</returns>
	public static int Compare(StringView viewA, StringView viewB, CultureInfo? culture, CompareOptions options)
	{
		int ret = string.Compare(viewA.text, viewA.start, viewB.text, viewB.start, Math.Min(viewA.length, viewB.length), culture, options);
		if (ret == 0)
		{
			return viewA.length - viewB.length;
		}
		else
		{
			return ret;
		}
	}

	/// <summary>
	/// 比较两个字符串视图。
	/// </summary>
	/// <param name="viewA">要比较的第一个字符串视图。</param>
	/// <param name="viewB">要比较的第二个字符串视图。</param>
	/// <param name="comparisonType">比较时使用的规则。</param>
	/// <returns>一个值，指示要比较的字符串视图的相对顺序。</returns>
	public static int Compare(StringView viewA, StringView viewB, StringComparison comparisonType)
	{
		int ret = string.Compare(viewA.text, viewA.start, viewB.text, viewB.start, Math.Min(viewA.length, viewB.length), comparisonType);
		if (ret == 0)
		{
			return viewA.length - viewB.length;
		}
		else
		{
			return ret;
		}
	}

	/// <summary>
	/// 通过依次比较字符来比较两个字符串视图。
	/// </summary>
	/// <param name="viewA">要比较的第一个字符串视图。</param>
	/// <param name="viewB">要比较的第二个字符串视图。</param>
	/// <returns>一个值，指示要比较的字符串视图的相对顺序。</returns>
	public static int CompareOrdinal(StringView viewA, StringView viewB)
	{
		int ret = string.CompareOrdinal(viewA.text, viewA.start, viewB.text, viewB.start, Math.Min(viewA.length, viewB.length));
		if (ret == 0)
		{
			return viewA.length - viewB.length;
		}
		else
		{
			return ret;
		}
	}

	/// <summary>
	/// 将当前对象与另一个字符串进行比较。
	/// </summary>
	/// <param name="other">要比较的字符串。</param>
	/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
	public int CompareTo(string? other)
	{
		if (other is null)
		{
			return 1;
		}
		int ret = string.Compare(text, start, other, 0, Math.Min(length, other.Length));
		if (ret == 0)
		{
			return length - other.Length;
		}
		else
		{
			return ret;
		}
	}

	/// <summary>
	/// 将当前对象与同一类型的另一个对象进行比较。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
	public int CompareTo(StringView other)
	{
		int ret = string.Compare(text, start, other.text, other.start, Math.Min(length, other.length));
		if (ret == 0)
		{
			return length - other.length;
		}
		else
		{
			return ret;
		}
	}

	/// <summary>
	/// 将当前对象与另一个对象进行比较。
	/// </summary>
	/// <param name="other">要比较的对象。</param>
	/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
	public int CompareTo(object? other)
	{
		if (other is null)
		{
			return 1;
		}
		else if (other is string text)
		{
			return CompareTo(text);
		}
		else if (other is StringView view)
		{
			return CompareTo(view);
		}
		else
		{
			return 1;
		}
	}

	#endregion // IComparable

}
