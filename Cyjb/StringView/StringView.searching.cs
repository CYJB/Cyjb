namespace Cyjb;

public readonly partial struct StringView
{
	/// <summary>
	/// 返回当前字符串视图是否包含指定字符。
	/// </summary>
	/// <param name="value">要检查的字符。</param>
	/// <returns>当前字符串视图是否包含指定字符。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public bool Contains(char value)
	{
		return AsSpan().Contains(value);
	}

	/// <summary>
	/// 返回当前字符串视图是否包含指定字符串。
	/// </summary>
	/// <param name="value">要检查的字符串。</param>
	/// <returns>当前字符串视图是否包含指定字符串。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public bool Contains(ReadOnlySpan<char> value)
	{
		return AsSpan().Contains(value, StringComparison.Ordinal);
	}

	/// <summary>
	/// 返回当前字符串视图是否包含指定字符。
	/// </summary>
	/// <param name="value">要检查的字符。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns>当前字符串视图是否包含指定字符。</returns>
	public bool Contains(char value, StringComparison comparisonType)
	{
		Span<char> text = stackalloc char[1] { value };
		return AsSpan().IndexOf(text, comparisonType) >= 0;
	}

	/// <summary>
	/// 返回当前字符串视图是否包含指定字符串。
	/// </summary>
	/// <param name="value">要检查的字符串。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns>当前字符串视图是否包含指定字符串。</returns>
	public bool Contains(ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		return AsSpan().Contains(value, comparisonType);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符首次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符。</param>
	/// <returns><paramref name="value"/> 首次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public int IndexOf(char value)
	{
		return AsSpan().IndexOf(value);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符串首次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符串。</param>
	/// <returns><paramref name="value"/> 首次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	/// <remarks>查找时使用区分大小写的当前区域性的比较。</remarks>
	public int IndexOf(ReadOnlySpan<char> value)
	{
		return AsSpan().IndexOf(value, StringComparison.CurrentCulture);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符首次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns><paramref name="value"/> 首次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	public int IndexOf(char value, StringComparison comparisonType)
	{
		Span<char> text = stackalloc char[1] { value };
		return AsSpan().IndexOf(text, comparisonType);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符串首次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符串。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns><paramref name="value"/> 首次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	public int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		return AsSpan().IndexOf(value, comparisonType);
	}

	/// <summary>
	/// 返回指定字符数组中任意字符在当前字符串视图中第一个匹配项的索引。
	/// </summary>
	/// <param name="anyOf">要检查的字符数组。</param>
	/// <returns>当前字符串视图中第一次找到 <paramref name="anyOf"/> 中任意字符的索引；如果未找到
	/// <paramref name="anyOf"/> 中的字符则返回 <c>-1</c>。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public int IndexOfAny(char[] anyOf)
	{
		return AsSpan().IndexOfAny(anyOf);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符最后一次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符。</param>
	/// <returns><paramref name="value"/> 最后一次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public int LastIndexOf(char value)
	{
		return AsSpan().LastIndexOf(value);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符串最后一次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符串。</param>
	/// <returns><paramref name="value"/> 最后一次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	/// <remarks>查找时使用区分大小写的当前区域性的比较。</remarks>
	public int LastIndexOf(ReadOnlySpan<char> value)
	{
		return AsSpan().LastIndexOf(value, StringComparison.CurrentCulture);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符最后一次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns><paramref name="value"/> 最后一次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	public int LastIndexOf(char value, StringComparison comparisonType)
	{
		Span<char> text = stackalloc char[1] { value };
		return AsSpan().LastIndexOf(text, comparisonType);
	}

	/// <summary>
	/// 返回当前字符串视图中指定字符串最后一次出现的索引。
	/// </summary>
	/// <param name="value">要检查的字符串。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns><paramref name="value"/> 最后一次出现的索引，如果不存在则返回 <c>-1</c>。</returns>
	public int LastIndexOf(ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		return AsSpan().LastIndexOf(value, comparisonType);
	}

	/// <summary>
	/// 返回指定字符数组中任意字符在当前字符串视图中最后一个匹配项的索引。
	/// </summary>
	/// <param name="anyOf">要检查的字符数组。</param>
	/// <returns>当前字符串视图中最后一次找到 <paramref name="anyOf"/> 中任意字符的索引；如果未找到
	/// <paramref name="anyOf"/> 中的字符则返回 <c>-1</c>。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public int LastIndexOfAny(ReadOnlySpan<char> anyOf)
	{
		return AsSpan().LastIndexOfAny(anyOf);
	}

	/// <summary>
	/// 返回当前字符串视图的开头是否与指定的字符匹配。
	/// </summary>
	/// <param name="value">要比较的字符。</param>
	/// <returns>如果 <paramref name="value"/> 与当前字符串视图的开头匹配，则返回 <c>true</c>；
	/// 否则返回 <c>false</c>。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public bool StartsWith(char value)
	{
		return length > 0 && text[start] == value;
	}

	/// <summary>
	/// 返回当前字符串视图的开头是否与指定的字符串匹配。
	/// </summary>
	/// <param name="value">要比较的字符串。</param>
	/// <returns>如果 <paramref name="value"/> 与当前字符串视图的开头匹配，则返回 <c>true</c>；
	/// 否则返回 <c>false</c>。</returns>
	/// <remarks>查找时使用区分大小写的当前区域性的比较。</remarks>
	public bool StartsWith(ReadOnlySpan<char> value)
	{
		return AsSpan().StartsWith(value);
	}

	/// <summary>
	/// 返回当前字符串视图的开头是否与指定的字符串匹配。
	/// </summary>
	/// <param name="value">要比较的字符串。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns>如果 <paramref name="value"/> 与当前字符串视图的开头匹配，则返回 <c>true</c>；
	/// 否则返回 <c>false</c>。</returns>
	/// <remarks>查找时使用区分大小写的当前区域性的比较。</remarks>
	public bool StartsWith(ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		return AsSpan().StartsWith(value, comparisonType);
	}

	/// <summary>
	/// 返回当前字符串视图的结尾是否与指定的字符匹配。
	/// </summary>
	/// <param name="value">要比较的字符。</param>
	/// <returns>如果 <paramref name="value"/> 与当前字符串视图的结尾匹配，则返回 <c>true</c>；
	/// 否则返回 <c>false</c>。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public bool EndsWith(char value)
	{
		return length > 0 && text[start + length - 1] == value;
	}

	/// <summary>
	/// 返回当前字符串视图的结尾是否与指定的字符串匹配。
	/// </summary>
	/// <param name="value">要比较的字符串。</param>
	/// <returns>如果 <paramref name="value"/> 与当前字符串视图的结尾匹配，则返回 <c>true</c>；
	/// 否则返回 <c>false</c>。</returns>
	/// <remarks>查找时使用区分大小写的当前区域性的比较。</remarks>
	public bool EndsWith(ReadOnlySpan<char> value)
	{
		return AsSpan().EndsWith(value);
	}

	/// <summary>
	/// 返回当前字符串视图的结尾是否与指定的字符串匹配。
	/// </summary>
	/// <param name="value">要比较的字符串。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns>如果 <paramref name="value"/> 与当前字符串视图的结尾匹配，则返回 <c>true</c>；
	/// 否则返回 <c>false</c>。</returns>
	/// <remarks>查找时使用区分大小写的当前区域性的比较。</remarks>
	public bool EndsWith(ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		return AsSpan().EndsWith(value, comparisonType);
	}
}

