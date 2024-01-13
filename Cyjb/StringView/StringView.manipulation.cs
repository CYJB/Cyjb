using System.Globalization;
using System.Text;
using Cyjb.Collections;

namespace Cyjb;

public readonly partial struct StringView
{
	/// <summary>
	/// 返回从指定位置开始指定长度的子字符串视图。
	/// </summary>
	/// <param name="startIndex">子字符串视图的起始位置。</param>
	/// <param name="length">子字符串视图的长度。</param>
	/// <returns>子字符串视图。</returns>
	/// <remarks>该方法是为了支持 <see cref="Range"/> 操作。</remarks>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
	/// 小于零或大于当前字符串视图的长度。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><c>startIndex + length</c>
	/// 表示的位置不在当前字符串视图内。</exception>
	public StringView Slice(int startIndex, int length)
	{
		if (startIndex < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		if (length < 0 || startIndex + length > this.length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		return new StringView(text, start + startIndex, length);
	}

	/// <summary>
	/// 返回一个新的字符串视图，在当前字符串视图中的指定索引位置插入指定的字符串。
	/// </summary>
	/// <param name="startIndex">要插入的索引。</param>
	/// <param name="value">要插入的字符串。</param>
	/// <returns>插入 <paramref name="value"/> 后的新字符串视图。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
	/// 不表示当前字符串视图的有效位置。</exception>
	public StringView Insert(int startIndex, string value)
	{
		ArgumentNullException.ThrowIfNull(value);
		if (startIndex < 0 || startIndex > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		int oldLength = length;
		int insertLength = value.Length;
		if (oldLength == 0)
		{
			return value;
		}
		else if (insertLength == 0)
		{
			return this;
		}
		int newLength = oldLength + insertLength;
		return string.Create(newLength, this, (Span<char> span, StringView view) =>
		{
			using var list = new ValueList<char>(span);
			var text = view.text;
			var start = view.start;
			list.Add(text.AsSpan(start, startIndex));
			list.Add(value);
			list.Add(text.AsSpan(start + startIndex, view.length - startIndex));
		});
	}

	#region Remove

	/// <summary>
	/// 返回删除当前字符串视图中指定位置之后所有字符的新字符串视图。
	/// </summary>
	/// <param name="startIndex">要开始删除字符的索引。</param>
	/// <returns>删除指定字符之后的新字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 小于 <c>0</c>
	/// 或不是当前字符串视图中的有效位置。</exception>
	public StringView Remove(int startIndex)
	{
		if (startIndex < 0 || startIndex > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		return new StringView(text, start, startIndex);
	}

	/// <summary>
	/// 返回删除当前字符串视图中指定位置之后指定个数字符的新字符串视图。
	/// </summary>
	/// <param name="startIndex">要开始删除字符的索引。</param>
	/// <param name="count">要删除的字符串个数。</param>
	/// <returns>删除指定字符之后的新字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 小于 <c>0</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><c>startIndex + count</c>
	/// 不表示当前字符串视图中的有效位置。</exception>
	public StringView Remove(int startIndex, int count)
	{
		if (startIndex < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		int newLength = length - count;
		if (count < 0 || startIndex > newLength)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		if (count == 0)
		{
			return this;
		}
		if (newLength == 0)
		{
			return Empty;
		}
		if (startIndex == 0)
		{
			return new StringView(text, start + count, newLength);
		}
		if (startIndex == newLength)
		{
			return new StringView(text, start, newLength);
		}
		return string.Create(newLength, this, (Span<char> span, StringView view) =>
		{
			using var list = new ValueList<char>(span);
			var text = view.text;
			var start = view.start;
			list.Add(text.AsSpan(start, startIndex));
			list.Add(text.AsSpan(start + startIndex + count, newLength - startIndex));
		});
	}

	#endregion // Remove

	#region Replace

	/// <summary>
	/// 返回一个新的字符串视图，将当前字符串视图中的指定字符替换为其它字符。
	/// </summary>
	/// <param name="oldChar">要替换的字符。</param>
	/// <param name="newChar">要替换到的字符。</param>
	/// <returns>替换后的字符串视图。</returns>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public StringView Replace(char oldChar, char newChar)
	{
		if (oldChar == newChar)
		{
			return this;
		}
		int idx = AsSpan().IndexOf(oldChar);
		if (idx < 0)
		{
			return this;
		}
		return string.Create(length, this, (Span<char> span, StringView view) =>
		{
			ReadOnlySpan<char> originSpan = view.AsSpan();
			originSpan[0..idx].CopyTo(span);
			for (int i = idx; i < originSpan.Length; i++)
			{
				char ch = originSpan[i];
				span[i] = ch == oldChar ? newChar : ch;
			}
		});
	}

	/// <summary>
	/// 返回一个新的字符串视图，将当前字符串视图中的指定字符串替换为其它字符串。
	/// </summary>
	/// <param name="oldValue">要替换的字符串。</param>
	/// <param name="newValue">要替换到的字符串。</param>
	/// <returns>替换后的字符串视图。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="oldValue"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="oldValue"/> 为空字符串。</exception>
	/// <remarks>查找时使用区分大小写和不区分区域性的序列比较。</remarks>
	public StringView Replace(string oldValue, string? newValue)
	{
		ArgumentNullException.ThrowIfNull(oldValue);
		if (oldValue.Length == 0)
		{
			throw CommonExceptions.ArgumentEmptyString(nameof(oldValue));
		}
		newValue ??= string.Empty;
		if (oldValue.Length == 1 && newValue.Length == 1)
		{
			return Replace(oldValue[0], newValue[0]);
		}
		var indexes = SearchIndexes(oldValue);
		if (indexes == null)
		{
			// 字符串未出现过。
			return this;
		}
		string result = ReplaceWithIndexes(oldValue.Length, newValue, indexes);
		indexes.Dispose();
		return result;
	}

	/// <summary>
	/// 搜索指定字符串的所有出现索引（原始字符串索引）。
	/// </summary>
	/// <param name="value">要查找的字符串。</param>
	/// <returns><paramref name="value"/> 出现的索引，如果未出现过则返回 <c>null</c>。</returns>
	private PooledList<int>? SearchIndexes(string value)
	{
		int idx = text.IndexOf(value, start, length, StringComparison.Ordinal);
		if (idx < 0)
		{
			return null;
		}
		var indexes = new PooledList<int>();
		int end = start + length;
		int step = value.Length;
		while (idx >= 0)
		{
			indexes.Add(idx);
			idx += step;
			idx = text.IndexOf(value, idx, end - idx, StringComparison.Ordinal);
		}
		return indexes;
	}

	/// <summary>
	/// 替换指定索引的字符串。
	/// </summary>
	/// <param name="oldValueLength">要替换的字符串长度。</param>
	/// <param name="newValue">要替换到的字符串。</param>
	/// <param name="indexes">旧字符串的出现索引。</param>
	/// <returns>替换后的结果。</returns>
	private string ReplaceWithIndexes(int oldValueLength, string newValue, PooledList<int> indexes)
	{
		long finalLength = length + ((long)(newValue.Length - oldValueLength)) * indexes.Length;
		if (finalLength > int.MaxValue)
		{
			throw new OutOfMemoryException();
		}
		return string.Create((int)finalLength, this, (Span<char> span, StringView view) =>
		{
			string text = view.text;
			int startIdx = view.start;
			int step = newValue.Length;
			for (int i = 0; i < indexes.Length; i++)
			{
				int idx = indexes[i];
				int count = idx - startIdx;
				if (count != 0)
				{
					text.AsSpan(startIdx, count).CopyTo(span);
					span = span[count..];
				}
				startIdx = idx + oldValueLength;
				newValue.CopyTo(span);
				span = span[step..];
			}
			text.AsSpan(startIdx, view.start + view.length - startIdx).CopyTo(span);
			indexes.Dispose();
		});
	}

	/// <summary>
	/// 返回一个新的字符串视图，将当前字符串视图中的指定字符串替换为其它字符串。
	/// </summary>
	/// <param name="oldValue">要替换的字符串。</param>
	/// <param name="newValue">要替换到的字符串。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns>替换后的字符串视图。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="oldValue"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="oldValue"/> 为空字符串。</exception>
	public StringView Replace(string oldValue, string? newValue, StringComparison comparisonType)
	{
		return comparisonType switch
		{
			StringComparison.CurrentCulture or StringComparison.CurrentCultureIgnoreCase => Replace(oldValue, newValue, IsIgnoreCase(comparisonType), CultureInfo.CurrentCulture),
			StringComparison.InvariantCulture or StringComparison.InvariantCultureIgnoreCase => Replace(oldValue, newValue, IsIgnoreCase(comparisonType), CultureInfo.InvariantCulture),
			StringComparison.Ordinal => Replace(oldValue, newValue),
			StringComparison.OrdinalIgnoreCase => Replace(oldValue, newValue, true, CultureInfo.InvariantCulture),
			_ => throw new ArgumentException(Resources.NotSupportedStringComparison, nameof(comparisonType)),
		};
	}

	/// <summary>
	/// 检查 <see cref="StringComparison"/> 是否是不区分大小写的。
	/// </summary>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns>比较是否是不区分大小写的。</returns>
	private static bool IsIgnoreCase(StringComparison comparisonType)
	{
		return ((int)comparisonType & (int)CompareOptions.IgnoreCase) > 0;
	}

	/// <summary>
	/// 返回一个新的字符串视图，将当前字符串视图中的指定字符串替换为其它字符串。
	/// </summary>
	/// <param name="oldValue">要替换的字符串。</param>
	/// <param name="newValue">要替换到的字符串。</param>
	/// <param name="ignoreCase">比较时是否忽略大小写。</param>
	/// <param name="culture">比较中要使用的区域性。如果为 <c>null</c>，则使用当前区域性。</param>
	/// <returns>替换后的字符串视图。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="oldValue"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentException"><paramref name="oldValue"/> 为空字符串。</exception>
	public StringView Replace(string oldValue, string? newValue, bool ignoreCase, CultureInfo? culture)
	{
		ArgumentNullException.ThrowIfNull(oldValue);
		if (oldValue.Length == 0)
		{
			throw CommonExceptions.ArgumentEmptyString(nameof(oldValue));
		}
		culture ??= CultureInfo.CurrentCulture;
		CompareOptions options = ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None;
		string? result = Replace(AsSpan(), oldValue, newValue, culture.CompareInfo, options);
		if (result == null)
		{
			return this;
		}
		else
		{
			return result;
		}
	}

	/// <summary>
	/// 替换指定的文本。
	/// </summary>
	/// <param name="text">要替换的原始文本。</param>
	/// <param name="oldValue">要替换的字符串。</param>
	/// <param name="newValue">要替换到的字符串。</param>
	/// <param name="compareInfo">区域性的比较信息。</param>
	/// <param name="options">区域性的比较选项。</param>
	/// <returns>替换后的字符串，如果没有替换则返回 <c>false</c>。</returns>
	/// <remarks>由于会根据区域性进行比较，同一个字符串可能会匹配到不同长度的字串，因此只能使用依次拼接的方式，
	/// 不能计算好长度后统一替换。</remarks>
	private static string? Replace(ReadOnlySpan<char> text, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, CompareInfo compareInfo, CompareOptions options)
	{
		using var result = new ValueList<char>(stackalloc char[ValueList.StackallocCharSizeLimit]);
		result.EnsureCapacity(text.Length);
		bool hasReplacement = false;
		while (true)
		{
			int index = compareInfo.IndexOf(text, oldValue, options, out int matchLength);
			if (index < 0 || matchLength == 0)
			{
				break;
			}
			result.Add(text[..index]);
			result.Add(newValue);
			text = text[(index + matchLength)..];
			hasReplacement = true;
		}
		if (!hasReplacement)
		{
			// 没有替换的时候，直接返回 null。
			return null;
		}
		result.Add(text);
		return result.ToString();
	}

	/// <summary>
	/// 将当前字符串视图中的所有换行序列替换为 <see cref="Environment.NewLine"/>。
	/// </summary>
	/// <returns>替换换行符后的字符串视图。</returns>
	/// <remarks>识别与 <see cref="string.ReplaceLineEndings()"/> 相同的换行序列，具体为
	/// CR (U+000D)、LF (U+000A)、CRLF (U+000D U+000A)、NEL (U+0085)、LS (U+2028)、FF (U+000C)和 PS (U+2029)。</remarks>
	public StringView ReplaceLineEndings() => ReplaceLineEndings(Environment.NewLine);

	/// <summary>
	/// 将当前字符串视图中的所有换行序列替换为 <paramref name="replacementText"/>。
	/// </summary>
	/// <param name="replacementText">要替换到的字符串。</param>
	/// <returns>替换换行符后的字符串视图。</returns>
	/// <remarks>识别与 <see cref="string.ReplaceLineEndings()"/> 相同的换行序列，具体为
	/// CR (U+000D)、LF (U+000A)、CRLF (U+000D U+000A)、NEL (U+0085)、LS (U+2028)、FF (U+000C)和 PS (U+2029)。</remarks>
	public StringView ReplaceLineEndings(string replacementText)
	{
		replacementText ??= string.Empty;
		ReadOnlySpan<char> span = AsSpan();
		int idx = IndexOfNewlineChar(span, replacementText, out int stride);
		if (idx < 0)
		{
			return this;
		}
		using var result = new ValueList<char>(stackalloc char[ValueList.StackallocCharSizeLimit]);
		result.Add(span[..idx]);
		span = span[(idx + stride)..];
		while (true)
		{
			idx = IndexOfNewlineChar(span, replacementText, out stride);
			if (idx < 0)
			{
				break;
			}
			result.Add(replacementText);
			result.Add(span[..idx]);
			span = span[(idx + stride)..];
		}
		result.Add(replacementText);
		result.Add(span);
		return result.ToString();
	}

	/// <summary>
	/// Unicode 换行符。
	/// </summary>
	public static readonly string NewLineChars = "\r\n\f\u0085\u2028\u2029";

	/// <summary>
	/// 搜索换行符的索引。
	/// </summary>
	/// <param name="text">要搜索的文本。</param>
	/// <param name="replacementText">换行符要替换到的文本。</param>
	/// <param name="stride">搜索到的换行符长度。</param>
	/// <returns>换行符的索引。</returns>
	private static int IndexOfNewlineChar(ReadOnlySpan<char> text, string replacementText, out int stride)
	{
		stride = default;
		int offset = 0;
		while (true)
		{
			int idx = text.IndexOfAny(NewLineChars);
			if (idx < 0)
			{
				return -1;
			}
			offset += idx;
			stride = 1;
			// 检查 \r\n，此时需要将替换两个字符。
			char ch = text[idx];
			if (ch == '\r')
			{
				int nextIdx = idx + 1;
				if (nextIdx < text.Length && text[nextIdx] == '\n')
				{
					stride = 2;
					if (replacementText != "\r\n")
					{
						return offset;
					}
				}
				else if (replacementText != "\r")
				{
					return offset;
				}
			}
			else if (replacementText.Length != 1 || replacementText[0] != ch)
			{
				return offset;
			}
			offset += stride;
			text = text[(idx + stride)..];
		}
	}

	#endregion // Replace

	#region Split

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	public StringView[] Split(char separator, StringSplitOptions options = StringSplitOptions.None)
	{
		Span<char> span = stackalloc char[1];
		span[0] = separator;
		return SplitInternal(span, int.MaxValue, options);
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public StringView[] Split(char separator, int count, StringSplitOptions options = StringSplitOptions.None)
	{
		Span<char> span = stackalloc char[1];
		span[0] = separator;
		return SplitInternal(span, count, options);
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符数组。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	public StringView[] Split(params char[]? separator)
	{
		return SplitInternal(separator, int.MaxValue, StringSplitOptions.None);
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符数组。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	public StringView[] Split(char[]? separator, StringSplitOptions options)
	{
		return SplitInternal(separator, int.MaxValue, options);
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符数组。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public StringView[] Split(char[]? separator, int count, StringSplitOptions options = StringSplitOptions.None)
	{
		return SplitInternal(separator, count, options);
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separators">分割字符的字符列表。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	private StringView[] SplitInternal(ReadOnlySpan<char> separators, int count, StringSplitOptions options)
	{
		if (count < 0)
		{
			throw CommonExceptions.ArgumentNegative(count);
		}
		if (count <= 1 || length == 0)
		{
			return CreateSingleSplitArray(count, options);
		}
		if (separators.IsEmpty && count > length)
		{
			// 如果是根据空白字符分割的，那么不需要再做额外的 Trim。
			options &= ~StringSplitOptions.TrimEntries;
		}
		var indexes = new ValueList<int>(stackalloc int[ValueList.StackallocIntSizeLimit]);
		SearchSeparatorsAny(separators, ref indexes);
		if (indexes.Length == 0)
		{
			// 未找到分隔符，直接创建分割数组即可。
			return CreateSingleSplitArray(1, options);
		}
		StringView[] result = (options == StringSplitOptions.None)
			? SplitWithoutOptions(indexes.AsSpan(), default, 1, count)
			: SplitWithOptions(indexes.AsSpan(), default, 1, count, options);
		indexes.Dispose();
		return result;
	}

	/// <summary>
	/// 搜索任意分隔符首次出现的索引。
	/// </summary>
	/// <param name="separators">字符分隔符列表。</param>
	/// <param name="indexes">搜索到的分隔符索引。</param>
	internal void SearchSeparatorsAny(ReadOnlySpan<char> separators, ref ValueList<int> indexes)
	{
		int len = start + length;
		// 如果长度为 0，表示分隔符为空白字符。
		if (separators.Length == 0)
		{
			for (int i = start; i < len; i++)
			{
				if (char.IsWhiteSpace(text[i]))
				{
					indexes.Add(i);
				}
			}
		}
		else
		{
			for (int i = start; i < len; i++)
			{
				if (separators.Contains(text[i]))
				{
					indexes.Add(i);
				}
			}
		}
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符串。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	public StringView[] Split(string? separator, StringSplitOptions options = StringSplitOptions.None)
	{
		if (length == 0)
		{
			return CreateSingleSplitArray(int.MaxValue, options);
		}
		if (separator!.Length == 0)
		{
			return CreateSingleSplitArray(1, options);
		}
		else
		{
			return SplitInternal(separator, int.MaxValue, options);
		}
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符串。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public StringView[] Split(string? separator, int count, StringSplitOptions options = StringSplitOptions.None)
	{
		if (count < 0)
		{
			throw CommonExceptions.ArgumentNegative(count);
		}
		if (count <= 1 || length == 0)
		{
			return CreateSingleSplitArray(count, options);
		}
		if (separator!.Length == 0)
		{
			return CreateSingleSplitArray(1, options);
		}
		else
		{
			return SplitInternal(separator, count, options);
		}
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符串。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	private StringView[] SplitInternal(string separator, int count, StringSplitOptions options)
	{
		var indexes = new ValueList<int>(stackalloc int[ValueList.StackallocIntSizeLimit]);
		SearchSeparator(separator, ref indexes);
		if (indexes.Length == 0)
		{
			return CreateSingleSplitArray(count, options);
		}
		StringView[] result = (options == StringSplitOptions.None)
			? SplitWithoutOptions(indexes.AsSpan(), default, separator.Length, count)
			: SplitWithOptions(indexes.AsSpan(), default, separator.Length, count, options);
		indexes.Dispose();
		return result;
	}

	/// <summary>
	/// 搜索分隔符首次出现的索引。
	/// </summary>
	/// <param name="separator">字符串分隔符。</param>
	/// <param name="indexes">搜索到的分隔符索引。</param>
	private void SearchSeparator(string separator, ref ValueList<int> indexes)
	{
		int idx = start;
		int end = start + length;
		int step = separator.Length;
		while (idx < end)
		{
			idx = text.IndexOf(separator, idx, end - idx, StringComparison.Ordinal);
			if (idx < 0)
			{
				break;
			}
			indexes.Add(idx);
			idx += step;
		}
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符串数组。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	public StringView[] Split(string[]? separator, StringSplitOptions options)
	{
		return SplitInternal(separator, int.MaxValue, options);
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separator">分割字符的字符串数组。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public StringView[] Split(string[]? separator, int count, StringSplitOptions options)
	{
		return SplitInternal(separator, count, options);
	}

	/// <summary>
	/// 使用指定的分隔符将字符串视图拆分为子字符串视图。
	/// </summary>
	/// <param name="separators">分割字符的字符串列表。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	private StringView[] SplitInternal(string[]? separators, int count, StringSplitOptions options)
	{
		if (count < 0)
		{
			throw CommonExceptions.ArgumentNegative(count);
		}
		if (separators == null || separators.Length == 0)
		{
			// 按照空白字符分割。
			return SplitInternal(ReadOnlySpan<char>.Empty, count, options);
		}
		if (count <= 1 || length == 0)
		{
			return CreateSingleSplitArray(count, options);
		}

		var indexes = new ValueList<int>(stackalloc int[ValueList.StackallocIntSizeLimit]);
		var lengthes = new ValueList<int>(stackalloc int[ValueList.StackallocIntSizeLimit]);
		SearchSeparatorsAny(separators, ref indexes, ref lengthes);
		if (indexes.Length == 0)
		{
			return CreateSingleSplitArray(count, options);
		}
		StringView[] result = (options == StringSplitOptions.None)
			? SplitWithoutOptions(indexes.AsSpan(), lengthes.AsSpan(), 0, count)
			: SplitWithOptions(indexes.AsSpan(), lengthes.AsSpan(), 0, count, options);
		indexes.Dispose();
		lengthes.Dispose();
		return result;
	}

	/// <summary>
	/// 搜索任意分隔符首次出现的索引。
	/// </summary>
	/// <param name="separators">字符串分隔符列表。</param>
	/// <param name="indexes">搜索到的分隔符索引。</param>
	/// <param name="lengthes">搜索到的分隔符长度。</param>
	internal void SearchSeparatorsAny(ReadOnlySpan<string> separators, ref ValueList<int> indexes, ref ValueList<int> lengthes)
	{
		int len = start + length;
		ReadOnlySpan<char> span = text;
		for (int i = start; i < len; i++)
		{
			for (int j = 0; j < separators.Length; j++)
			{
				string? separator = separators[j];
				if (string.IsNullOrEmpty(separator))
				{
					continue;
				}
				int sepLen = separator.Length;
				if (span[i] == separator[0] && sepLen <= len - i)
				{
					if (sepLen == 1 || span.Slice(i, sepLen).SequenceEqual(separator))
					{
						indexes.Add(i);
						lengthes.Add(sepLen);
						i += sepLen - 1;
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// 从当前字符串视图创建拆分数组。
	/// </summary>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	private StringView[] CreateSingleSplitArray(int count, StringSplitOptions options)
	{
		if (count != 0)
		{
			StringView item = this;
			if ((options & StringSplitOptions.TrimEntries) != 0)
			{
				item = item.Trim();
			}
			if (item.length != 0 || (options & StringSplitOptions.RemoveEmptyEntries) == 0)
			{
				return new StringView[] { item };
			}
		}
		return Array.Empty<StringView>();
	}

	/// <summary>
	/// 根据索引拆分当前字符串，不需要处理任何拆分选项。
	/// </summary>
	/// <param name="indexes">分隔符的索引列表。</param>
	/// <param name="lengthes">分隔符的长度列表。</param>
	/// <param name="defaultLength">分隔符长度一致时的默认长度。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	private StringView[] SplitWithoutOptions(ReadOnlySpan<int> indexes, ReadOnlySpan<int> lengthes, int defaultLength, int count)
	{
		// 分隔符的个数是 indexes 的长度，结果个数是分隔符个数 +1。
		int sepCount = Math.Min(indexes.Length, count - 1);
		StringView[] result = new StringView[sepCount + 1];
		int idx = start;
		int arrIndex = 0;
		for (int i = 0; i < sepCount; i++, arrIndex++)
		{
			result[arrIndex] = new StringView(text, idx, indexes[i] - idx);
			idx = indexes[i] + (lengthes.IsEmpty ? defaultLength : lengthes[i]);
		}
		int lastLen = start + length - idx;
		if (lastLen > 0)
		{
			result[arrIndex] = new StringView(text, idx, lastLen);
		}
		else if (arrIndex == sepCount)
		{
			// 字符串视图的最后一个字符是分隔符，因此添加一个空的字符串视图。
			result[arrIndex] = Empty;
		}
		return result;
	}

	/// <summary>
	/// 根据索引拆分当前字符串，并处理任何选项。
	/// </summary>
	/// <param name="indexes">分隔符的索引列表。</param>
	/// <param name="lengthes">分隔符的长度列表。</param>
	/// <param name="defaultLength">分隔符长度一致时的默认长度。</param>
	/// <param name="count">要返回的子字符串视图的最大数量。</param>
	/// <param name="options">拆分字符串视图的选项。</param>
	/// <returns>拆分后的子字符串视图。</returns>
	private StringView[] SplitWithOptions(ReadOnlySpan<int> indexes, ReadOnlySpan<int> lengthes, int defaultLength, int count, StringSplitOptions options)
	{
		// 分配一个足够大的数组，数组可能会由于选项而被裁剪。
		int sepCount = indexes.Length;
		StringView[] result = new StringView[(sepCount < count) ? (sepCount + 1) : count];
		StringView view;
		int idx = start;
		int arrIndex = 0;
		bool trimEntries = (options & StringSplitOptions.TrimEntries) != 0;
		bool removeEmpty = (options & StringSplitOptions.RemoveEmptyEntries) != 0;
		for (int i = 0; i < sepCount; i++)
		{
			view = new StringView(text, idx, indexes[i] - idx);
			if (trimEntries)
			{
				view = view.Trim();
			}
			if (view.length != 0 || !removeEmpty)
			{
				result[arrIndex++] = view;
			}
			idx = indexes[i] + (lengthes.IsEmpty ? defaultLength : lengthes[i]);
			if (arrIndex == count - 1)
			{
				// 之后要处理拆分结果的最后一项，需要跳过目前剩余的空白项。
				if (removeEmpty)
				{
					while (++i < sepCount)
					{
						view = new StringView(text, idx, indexes[i] - idx);
						if (trimEntries)
						{
							view = view.Trim();
						}
						if (view.length != 0)
						{
							// 找到了非空空白项，退出。
							break;
						}
						idx = indexes[i] + (lengthes.IsEmpty ? defaultLength : lengthes[i]);
					}
				}
				break;
			}
		}
		view = new StringView(text, idx, start + length - idx);
		if (trimEntries)
		{
			view = view.Trim();
		}
		if (view.length != 0 || !removeEmpty)
		{
			result[arrIndex++] = view;
		}
		Array.Resize(ref result, arrIndex);
		return result;
	}

	#endregion // Split

	#region Substring

	/// <summary>
	/// 返回从指定位置开始的子字符串视图。
	/// </summary>
	/// <param name="startIndex">子字符串视图的起始位置。</param>
	/// <returns>子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
	/// 小于零或大于当前字符串视图的长度。</exception>
	public StringView Substring(int startIndex)
	{
		if (startIndex == 0)
		{
			return this;
		}
		int len = length - startIndex;
		if (len == 0)
		{
			return Empty;
		}
		if ((uint)startIndex > (uint)length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		return new StringView(text, start + startIndex, len);
	}

	/// <summary>
	/// 返回从指定位置开始指定长度的子字符串视图。
	/// </summary>
	/// <param name="startIndex">子字符串视图的起始位置。</param>
	/// <param name="length">子字符串视图的长度。</param>
	/// <returns>子字符串视图。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
	/// 小于零或大于当前字符串视图的长度。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><c>startIndex + length</c>
	/// 表示的位置不在当前字符串视图内。</exception>
	public StringView Substring(int startIndex, int length)
	{
		if (startIndex < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		if (length < 0 || startIndex + length > this.length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		return new StringView(text, start + startIndex, length);
	}

	#endregion // Substring

	#region ToLower

	/// <summary>
	/// 返回当前字符串视图的小写形式。
	/// </summary>
	/// <returns>小写的字符串视图。</returns>
	/// <remarks>使用当前区域性的大小写规则。</remarks>
	public StringView ToLower() => ToLower(null);

	/// <summary>
	/// 返回当前字符串视图的小写形式。
	/// </summary>
	/// <param name="culture">转换时使用的区域性。如果为 <c>null</c>，则使用当前区域性。</param>
	/// <returns>小写的字符串视图。</returns>
	public StringView ToLower(CultureInfo? culture)
	{
		if (length == 0)
		{
			return this;
		}
		culture ??= CultureInfo.CurrentCulture;
		// 这里先对每个字符分别 ToLower，如果后续遇到问题再做优化。
		ReadOnlySpan<char> source = AsSpan();
		int end = source.Length - 1;
		int i = 0;
		Span<char> surrogate = stackalloc char[2];
		for (; i < source.Length; i++)
		{
			if (char.IsHighSurrogate(source[i]) && i < end && char.IsLowSurrogate(source[i + 1]))
			{
				Rune rune = new(source[i], source[i + 1]);
				rune = Rune.ToLower(rune, culture);
				rune.EncodeToUtf16(surrogate);
				if (!surrogate.SequenceEqual(source.Slice(i, 2)))
				{
					break;
				}
				i++;
			}
			else if (char.ToLower(source[i], culture) != source[i])
			{
				break;
			}
		}
		// 未发生大小写改变。
		if (i >= source.Length)
		{
			return this;
		}
		// 发生了大小写改变。
		return string.Create(length, this, (Span<char> span, StringView view) =>
		{
			ReadOnlySpan<char> source = view.AsSpan();
			source[..i].CopyTo(span);
			source[i..].ToLower(span[i..], culture);
		});
	}

	/// <summary>
	/// 返回当前字符串视图的小写形式，使用固定区域性的大小写规则。
	/// </summary>
	/// <returns>小写的字符串视图。</returns>
	public StringView ToLowerInvariant() => ToLower(CultureInfo.InvariantCulture);

	#endregion // ToLower

	#region ToUpper

	/// <summary>
	/// 返回当前字符串视图的大写形式。
	/// </summary>
	/// <returns>大写的字符串视图。</returns>
	/// <remarks>使用当前区域性的大小写规则。</remarks>
	public StringView ToUpper() => ToUpper(null);

	/// <summary>
	/// 返回当前字符串视图的大写形式。
	/// </summary>
	/// <param name="culture">转换时使用的区域性。如果为 <c>null</c>，则使用当前区域性。</param>
	/// <returns>大写的字符串视图。</returns>
	public StringView ToUpper(CultureInfo? culture)
	{
		if (length == 0)
		{
			return this;
		}
		culture ??= CultureInfo.CurrentCulture;
		// 这里先对每个字符分别 ToLower，如果后续遇到问题再做优化。
		ReadOnlySpan<char> source = AsSpan();
		int end = source.Length - 1;
		int i = 0;
		Span<char> surrogate = stackalloc char[2];
		for (; i < source.Length; i++)
		{
			if (char.IsHighSurrogate(source[i]) && i < end && char.IsLowSurrogate(source[i + 1]))
			{
				Rune rune = new(source[i], source[i + 1]);
				rune = Rune.ToUpper(rune, culture);
				rune.EncodeToUtf16(surrogate);
				if (!surrogate.SequenceEqual(source.Slice(i, 2)))
				{
					break;
				}
				i++;
			}
			else if (char.ToUpper(source[i], culture) != source[i])
			{
				break;
			}
		}
		// 未发生大小写改变。
		if (i >= source.Length)
		{
			return this;
		}
		// 发生了大小写改变。
		return string.Create(length, this, (Span<char> span, StringView view) =>
		{
			ReadOnlySpan<char> source = view.AsSpan();
			source[..i].CopyTo(span);
			source[i..].ToUpper(span[i..], culture);
		});
	}

	/// <summary>
	/// 返回当前字符串视图的大写形式，使用固定区域性的大小写规则。
	/// </summary>
	/// <returns>大写的字符串视图。</returns>
	public StringView ToUpperInvariant() => ToUpper(CultureInfo.InvariantCulture);

	#endregion // ToUpper

	#region Trim

	/// <summary>
	/// 从当前字符串视图删除所有前导和尾随空白字符。
	/// </summary>
	/// <returns>删除前导和尾随空白字符后的剩余字符串视图。</returns>
	public StringView Trim()
	{
		if (length == 0 || (!char.IsWhiteSpace(text[start]) && !char.IsWhiteSpace(text[start + length - 1])))
		{
			return this;
		}
		return TrimWhiteSpace(true, true);
	}

	/// <summary>
	/// 从当前字符串视图删除指定字符数组的前导和尾随匹配项。
	/// </summary>
	/// <param name="trimChars">要删除的字符数组。</param>
	/// <returns>删除前导和尾随字符后的剩余字符串视图。</returns>
	/// <remarks>如果 <paramref name="trimChars"/> 为 <c>null</c> 或空数组，则删除所有空白字符。</remarks>
	public StringView Trim(params char[]? trimChars)
	{
		if (trimChars == null || trimChars.Length == 0)
		{
			return TrimWhiteSpace(true, true);
		}
		return Trim(trimChars, true, true);
	}

	/// <summary>
	/// 从当前字符串视图删除指定字符的前导和尾随匹配项。
	/// </summary>
	/// <param name="trimChar">要删除的字符。</param>
	/// <returns>删除前导和尾随字符的剩余字符串视图。</returns>
	public StringView Trim(char trimChar)
	{
		if (length == 0 || (text[start] != trimChar && text[start + length - 1] != trimChar))
		{
			return this;
		}
		Span<char> span = stackalloc char[1];
		span[0] = trimChar;
		return Trim(span, true, true);
	}

	/// <summary>
	/// 从当前字符串视图删除所有前导空白字符。
	/// </summary>
	/// <returns>删除前导空白字符后的剩余字符串视图。</returns>
	public StringView TrimStart()
	{
		if (length == 0 || !char.IsWhiteSpace(text[start]))
		{
			return this;
		}
		return TrimWhiteSpace(true, false);
	}

	/// <summary>
	/// 从当前字符串视图删除指定字符数组的前导匹配项。
	/// </summary>
	/// <param name="trimChars">要删除的字符数组。</param>
	/// <returns>删除前导字符后的剩余字符串视图。</returns>
	/// <remarks>如果 <paramref name="trimChars"/> 为 <c>null</c> 或空数组，则删除所有空白字符。</remarks>
	public StringView TrimStart(params char[]? trimChars)
	{
		if (trimChars == null || trimChars.Length == 0)
		{
			return TrimWhiteSpace(true, false);
		}
		return Trim(trimChars, true, false);
	}

	/// <summary>
	/// 从当前字符串视图删除指定字符的前导匹配项。
	/// </summary>
	/// <param name="trimChar">要删除的字符。</param>
	/// <returns>删除前导字符的剩余字符串视图。</returns>
	public StringView TrimStart(char trimChar)
	{
		if (length == 0 || text[start] != trimChar)
		{
			return this;
		}
		Span<char> span = stackalloc char[1];
		span[0] = trimChar;
		return Trim(span, true, false);
	}

	/// <summary>
	/// 从当前字符串视图删除所有尾随空白字符。
	/// </summary>
	/// <returns>删除尾随空白字符后的剩余字符串视图。</returns>
	public StringView TrimEnd()
	{
		if (length == 0 || !char.IsWhiteSpace(text[start + length - 1]))
		{
			return this;
		}
		return TrimWhiteSpace(false, true);
	}

	/// <summary>
	/// 从当前字符串视图删除指定字符数组的尾随匹配项。
	/// </summary>
	/// <param name="trimChars">要删除的字符数组。</param>
	/// <returns>删除尾随字符后的剩余字符串视图。</returns>
	/// <remarks>如果 <paramref name="trimChars"/> 为 <c>null</c> 或空数组，则删除所有空白字符。</remarks>
	public StringView TrimEnd(params char[]? trimChars)
	{
		if (trimChars == null || trimChars.Length == 0)
		{
			return TrimWhiteSpace(false, true);
		}
		return Trim(trimChars, false, true);
	}

	/// <summary>
	/// 从当前字符串视图删除指定字符的尾随匹配项。
	/// </summary>
	/// <param name="trimChar">要删除的字符。</param>
	/// <returns>删除尾随字符的剩余字符串视图。</returns>
	public StringView TrimEnd(char trimChar)
	{
		if (length == 0 || text[start + length - 1] != trimChar)
		{
			return this;
		}
		Span<char> span = stackalloc char[1];
		span[0] = trimChar;
		return Trim(span, false, true);
	}

	/// <summary>
	/// 从当前字符串视图删除所有前导和尾随空白字符。
	/// </summary>
	/// <param name="trimStart">是否删除前导空白字符。</param>
	/// <param name="trimEnd">是否删除尾随空白字符。</param>
	/// <returns>剩余字符串视图。</returns>
	private StringView TrimWhiteSpace(bool trimStart, bool trimEnd)
	{
		int endIdx = start + length - 1;
		int startIdx = start;
		if (trimStart)
		{
			for (; startIdx <= endIdx && char.IsWhiteSpace(text[startIdx]); startIdx++) ;
		}
		if (trimEnd)
		{
			for (; endIdx >= startIdx && char.IsWhiteSpace(text[endIdx]); endIdx--) ;
		}
		return new StringView(text, startIdx, endIdx - startIdx + 1);
	}

	/// <summary>
	/// 从当前字符串视图删除指定字符数组的前导和尾随匹配项。
	/// </summary>
	/// <param name="trimChars">要删除的字符数组。</param>
	/// <param name="trimStart">是否删除前导字符。</param>
	/// <param name="trimEnd">是否删除尾随字符。</param>
	/// <returns>剩余字符串视图。</returns>
	private StringView Trim(ReadOnlySpan<char> trimChars, bool trimStart, bool trimEnd)
	{
		int endIdx = start + length - 1;
		int startIdx = start;
		if (trimStart)
		{
			for (; startIdx <= endIdx && trimChars.Contains(text[startIdx]); startIdx++) ;
		}
		if (trimEnd)
		{
			for (; endIdx >= startIdx && trimChars.Contains(text[endIdx]); endIdx--) ;
		}
		return new StringView(text, startIdx, endIdx - startIdx + 1);
	}

	#endregion // Trim

	#region PadLeft/PadRight

	/// <summary>
	/// 返回一个新字符串，在当前字符串视图的开头填充空格来达到指定的总长度。
	/// </summary>
	/// <param name="totalWidth">结果字符串中的字符数。</param>
	/// <returns>填充后的结果。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="totalWidth"/> 小于 <c>0</c>。</exception>
	public StringView PadLeft(int totalWidth) => PadLeft(totalWidth, ' ');

	/// <summary>
	/// 返回一个新字符串，在当前字符串视图的开头填充指定字符来达到指定的总长度。
	/// </summary>
	/// <param name="totalWidth">结果字符串中的字符数。</param>
	/// <param name="paddingChar">要填充的字符。</param>
	/// <returns>填充后的结果。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="totalWidth"/> 小于 <c>0</c>。</exception>
	public StringView PadLeft(int totalWidth, char paddingChar)
	{
		if (totalWidth < 0)
		{
			throw CommonExceptions.ArgumentNegative(totalWidth);
		}
		int count = totalWidth - length;
		if (count <= 0)
		{
			return this;
		}
		return string.Create(totalWidth, this, (Span<char> span, StringView view) =>
		{
			using var list = new ValueList<char>(span);
			list.Add(paddingChar, count);
			list.Add(view.AsSpan());
		});
	}

	/// <summary>
	/// 返回一个新字符串，在当前字符串视图的末尾填充空格来达到指定的总长度。
	/// </summary>
	/// <param name="totalWidth">结果字符串中的字符数。</param>
	/// <returns>填充后的结果。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="totalWidth"/> 小于 <c>0</c>。</exception>
	public StringView PadRight(int totalWidth) => PadRight(totalWidth, ' ');

	/// <summary>
	/// 返回一个新字符串，在当前字符串视图的末尾填充指定字符来达到指定的总长度。
	/// </summary>
	/// <param name="totalWidth">结果字符串中的字符数。</param>
	/// <param name="paddingChar">要填充的字符。</param>
	/// <returns>填充后的结果。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="totalWidth"/> 小于 <c>0</c>。</exception>
	public StringView PadRight(int totalWidth, char paddingChar)
	{
		if (totalWidth < 0)
		{
			throw CommonExceptions.ArgumentNegative(totalWidth);
		}
		int count = totalWidth - length;
		if (count <= 0)
		{
			return this;
		}
		return string.Create(totalWidth, this, (Span<char> span, StringView view) =>
		{
			using var list = new ValueList<char>(span);
			list.Add(view.AsSpan());
			list.Add(paddingChar, count);
		});
	}

	#endregion // PadLeft/PadRight

	#region Concat

	/// <summary>
	/// 连接指定的两个字符串视图。
	/// </summary>
	/// <param name="text1">要连接的第一个字符串视图。</param>
	/// <param name="text2">要连接的第二个字符串视图。</param>
	/// <returns>两个字符串视图连接后的字符串。根据参数的不同可能会创建新的字符串。</returns>
	public static StringView Concat(StringView text1, StringView text2)
	{
		return string.Concat(text1.AsSpan(), text2.AsSpan());
	}

	/// <summary>
	/// 连接指定的三个字符串视图。
	/// </summary>
	/// <param name="text1">要连接的第一个字符串视图。</param>
	/// <param name="text2">要连接的第二个字符串视图。</param>
	/// <param name="text3">要连接的第三个字符串视图。</param>
	/// <returns>三个字符串视图连接后的字符串。根据参数的不同可能会创建新的字符串。</returns>
	public static StringView Concat(StringView text1, StringView text2, StringView text3)
	{
		return string.Concat(text1.AsSpan(), text2.AsSpan(), text3.AsSpan());
	}

	/// <summary>
	/// 连接指定的四个字符串视图。
	/// </summary>
	/// <param name="text1">要连接的第一个字符串视图。</param>
	/// <param name="text2">要连接的第二个字符串视图。</param>
	/// <param name="text3">要连接的第三个字符串视图。</param>
	/// <param name="text4">要连接的第四个字符串视图。</param>
	/// <returns>四个字符串视图连接后的字符串。根据参数的不同可能会创建新的字符串。</returns>
	public static StringView Concat(StringView text1, StringView text2, StringView text3, StringView text4)
	{
		return string.Concat(text1.AsSpan(), text2.AsSpan(), text3.AsSpan(), text4.AsSpan());
	}

	/// <summary>
	/// 连接指定字符串视图的数组。
	/// </summary>
	/// <param name="texts">要连接的符串视图数组。</param>
	/// <returns>多个字符串视图连接后的字符串。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="texts"/> 为 <c>null</c>。</exception>
	/// <exception cref="OutOfMemoryException">内存不足。</exception>
	public static StringView Concat(params StringView[] texts)
	{
		ArgumentNullException.ThrowIfNull(texts);
		if (texts.Length == 0)
		{
			return Empty;
		}
		else if (texts.Length == 1)
		{
			return texts[0];
		}
		int totalLength = 0;
		for (int i = 0; i < texts.Length; i++)
		{
			totalLength += texts[i].length;
			if (totalLength < 0)
			{
				throw new OutOfMemoryException();
			}
		}
		return string.Create(totalLength, 0, (Span<char> span, int state) =>
		{
			using var list = new ValueList<char>(span);
			for (int i = 0; i < texts.Length; i++)
			{
				list.Add(texts[i].AsSpan());
			}
		});
	}

	/// <summary>
	/// 连接指定字符串视图的数组。
	/// </summary>
	/// <param name="texts">要连接的符串视图数组。</param>
	/// <returns>多个字符串视图连接后的字符串。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="texts"/> 为 <c>null</c>。</exception>
	public static StringView Concat(IEnumerable<StringView> texts)
	{
		ArgumentNullException.ThrowIfNull(texts);
		using IEnumerator<StringView> enumerator = texts.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return Empty;
		}
		StringView first = enumerator.Current;
		if (!enumerator.MoveNext())
		{
			return first;
		}
		using var result = new ValueList<char>(stackalloc char[ValueList.StackallocCharSizeLimit]);
		result.Add(first.AsSpan());
		do
		{
			result.Add(enumerator.Current.AsSpan());
		}
		while (enumerator.MoveNext());
		return result.ToString();
	}

	#endregion // Concat

	#region Join

	/// <summary>
	/// 连接字符串视图数组，每个成员之间使用指定的分隔符。
	/// </summary>
	/// <param name="separator">用作分隔符的字符。</param>
	/// <param name="texts">要连接的字符串视图数组。</param>
	/// <returns>连接结果。根据参数的不同可能会创建新的字符串。</returns>
	public static StringView Join(char separator, params StringView[] texts)
	{
		ArgumentNullException.ThrowIfNull(texts);
		if (texts.Length == 0)
		{
			return Empty;
		}
		else if (texts.Length == 1)
		{
			return texts[0];
		}
		int totalLength = texts.Length - 1;
		for (int i = 0; i < texts.Length; i++)
		{
			totalLength += texts[i].length;
			if (totalLength < 0)
			{
				throw new OutOfMemoryException();
			}
		}
		return string.Create(totalLength, separator, (Span<char> span, char separator) =>
		{
			using var list = new ValueList<char>(span);
			list.Add(texts[0].AsSpan());
			for (int i = 1; i < texts.Length; i++)
			{
				list.Add(separator);
				list.Add(texts[i].AsSpan());
			}
		});
	}

	/// <summary>
	/// 连接字符串视图数组，每个成员之间使用指定的分隔符。
	/// </summary>
	/// <param name="separator">用作分隔符的字符。</param>
	/// <param name="texts">要连接的字符串视图数组。</param>
	/// <returns>连接结果。根据参数的不同可能会创建新的字符串。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="texts"/> 为 <c>null</c>。</exception>
	/// <exception cref="OutOfMemoryException">内存不足。</exception>
	public static StringView Join(string? separator, params StringView[] texts)
	{
		ArgumentNullException.ThrowIfNull(texts);
		if (texts.Length == 0)
		{
			return Empty;
		}
		else if (texts.Length == 1)
		{
			return texts[0];
		}
		int totalLength = 0;
		if (separator != null)
		{
			totalLength = separator.Length * (texts.Length - 1);
		}
		for (int i = 0; i < texts.Length; i++)
		{
			totalLength += texts[i].length;
			if (totalLength < 0)
			{
				throw new OutOfMemoryException();
			}
		}
		return string.Create(totalLength, separator, (Span<char> span, string? separator) =>
		{
			ReadOnlySpan<char> sep = separator.AsSpan();
			using var list = new ValueList<char>(span);
			list.Add(texts[0].AsSpan());
			for (int i = 1; i < texts.Length; i++)
			{
				list.Add(sep);
				list.Add(texts[i].AsSpan());
			}
		});
	}

	/// <summary>
	/// 连接字符串视图数组，每个成员之间使用指定的分隔符。
	/// </summary>
	/// <param name="separator">用作分隔符的字符。</param>
	/// <param name="texts">要连接的字符串视图数组。</param>
	/// <returns>连接结果。根据参数的不同可能会创建新的字符串。</returns>
	public static StringView Join(char separator, IEnumerable<StringView> texts)
	{
		Span<char> sep = stackalloc char[1] { separator };
		return Join(sep, texts);
	}

	/// <summary>
	/// 连接字符串视图数组，每个成员之间使用指定的分隔符。
	/// </summary>
	/// <param name="separator">用作分隔符的字符。</param>
	/// <param name="texts">要连接的字符串视图数组。</param>
	/// <returns>连接结果。根据参数的不同可能会创建新的字符串。</returns>
	public static StringView Join(ReadOnlySpan<char> separator, IEnumerable<StringView> texts)
	{
		ArgumentNullException.ThrowIfNull(texts);
		using IEnumerator<StringView> enumerator = texts.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return Empty;
		}
		StringView first = enumerator.Current;
		if (!enumerator.MoveNext())
		{
			return first;
		}
		using var result = new ValueList<char>(stackalloc char[ValueList.StackallocCharSizeLimit]);
		result.Add(first.AsSpan());
		do
		{
			result.Add(separator);
			result.Add(enumerator.Current.AsSpan());
		}
		while (enumerator.MoveNext());
		return result.ToString();
	}

	#endregion // Join

}
