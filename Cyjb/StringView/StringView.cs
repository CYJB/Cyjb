using System.Collections;

namespace Cyjb;

/// <summary>
/// 表示字符串视图，其行为类似 <see cref="ReadOnlyMemory{T}"/>，但支持类似 <see cref="string"/> 的接口。
/// </summary>
public readonly partial struct StringView : IEnumerable<char>
{
	/// <summary>
	/// 表示空字符串视图，此字段为只读。
	/// </summary>
	public static readonly StringView Empty = new(string.Empty);

	/// <summary>
	/// 原始字符串。
	/// </summary>
	private readonly string text;
	/// <summary>
	/// 视图的起始索引。
	/// </summary>
	private readonly int start;
	/// <summary>
	/// 视图的长度。
	/// </summary>
	private readonly int length;

	/// <summary>
	/// 创建指定字符串的视图。
	/// </summary>
	/// <param name="text">要创建视图的字符串。</param>
	/// <remarks>如果 <paramref name="text"/> 为 <c>null</c>，会返回空视图。</remarks>
	public StringView(string? text)
	{
		start = 0;
		if (text == null)
		{
			this.text = string.Empty;
			length = 0;
		}
		else
		{
			this.text = text;
			length = text.Length;
		}
	}

	/// <summary>
	/// 创建指定字符串从指定索引开始的视图。
	/// </summary>
	/// <param name="text">要创建视图的字符串。</param>
	/// <param name="start">视图的起始索引。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于 <c>0</c> 或大于 <c>text.Length</c>。</exception>
	/// <remarks>如果 <paramref name="text"/> 为 <c>null</c>，会返回空视图。</remarks>
	public StringView(string? text, int start)
	{
		if (text == null)
		{
			this.text = string.Empty;
			this.start = 0;
			length = 0;
			return;
		}
		length = text.Length - start;
		if (start < 0 || length < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		if (length > 0)
		{
			this.text = text;
			this.start = start;
		}
		else
		{
			// 长度为 0 的时候，就不持有 text 的引用，及时释放内存。
			this.text = string.Empty;
			this.start = 0;
		}
	}

	/// <summary>
	/// 创建指定字符串从指定索引开始的视图。
	/// </summary>
	/// <param name="text">要创建视图的字符串。</param>
	/// <param name="start">视图的起始索引。</param>
	/// <param name="length">视图的长度。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于 <c>0</c> 或大于 <c>text.Length</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><c>start + length</c> 表示的位置不在 <paramref name="text"/> 范围内。</exception>
	/// <remarks>如果 <paramref name="text"/> 为 <c>null</c>，会返回空视图。</remarks>
	public StringView(string? text, int start, int length)
	{
		if (text == null)
		{
			this.text = string.Empty;
			this.start = 0;
			this.length = 0;
			return;
		}
		if (start < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		if (length < 0 || start + length > text.Length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		this.length = length;
		if (length == 0)
		{
			// 长度为 0 的时候，就不持有 text 的引用，及时释放内存。
			this.text = string.Empty;
			this.start = 0;
		}
		else
		{
			this.text = text;
			this.start = start;
		}
	}

	/// <summary>
	/// 获取当前字符串视图中位于指定位置的字符。
	/// </summary>
	/// <param name="index">要返回字符的索引。</param>
	/// <returns>位于 <paramref name="index"/> 位置的字符。</returns>
	public char this[int index]
	{
		get
		{
			if (index < 0 || index >= length)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			return text[start + index];
		}
	}

	/// <summary>
	/// 获取当前字符串视图中的字符数。
	/// </summary>
	public int Length => length;

	/// <summary>
	/// 获取当前字符串视图是否是空的。
	/// </summary>
	public bool IsEmpty => length == 0;

	/// <summary>
	/// 返回当前字符串视图是否仅由空白字符组成。
	/// </summary>
	/// <returns>如果当前字符串视图是否仅由空白字符组成，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	public bool IsWhiteSpace()
	{
		return AsSpan().IsWhiteSpace();
	}

	/// <summary>
	/// 将当前字符串视图中的字符复制到字符数组。
	/// </summary>
	/// <returns>复制到的字符数组。</returns>
	public char[] ToCharArray()
	{
		if (length == 0)
		{
			return Array.Empty<char>();
		}
		return text.ToCharArray(start, length);
	}

	/// <summary>
	/// 将当前字符串视图的内容复制到目标范围。
	/// </summary>
	/// <param name="destination">要将内容复制到的目标。</param>
	/// <returns>如果成功复制数据，则为 <c>true</c>；如果目标太短无法容纳字符串视图的内容，则为 <c>false</c>。</returns>
	public bool TryCopyTo(Span<char> destination)
	{
		return AsSpan().TryCopyTo(destination);
	}

	/// <summary>
	/// 尝试连接指定的字符串视图。
	/// </summary>
	/// <param name="other">要连接的字符串视图。</param>
	/// <param name="result">连接的结果字符串视图。</param>
	/// <returns>如果当前字符串视图与 <paramref name="other"/> 是从相同字符串创建的，
	/// 且首尾相接时表示可以连接，返回 <c>true</c>，并将连接结果通过 <paramref name="result"/> 返回；
	/// 或者两个字符串视图其中之一为空，不需要连接时也会返回 <c>true</c>；
	/// 否则返回 <c>false</c>，<paramref name="result"/> 会返回空。</returns>
	public bool TryConcat(StringView other, out StringView result)
	{
		if (length == 0)
		{
			result = other;
			return true;
		}
		else if (other.length == 0)
		{
			result = this;
			return true;
		}
		else if (text == other.text && start + length == other.start)
		{
			result = new StringView(text, start, length + other.length);
			return true;
		}
		result = Empty;
		return false;
	}

	/// <summary>
	/// 允许从 <see cref="string"/> 隐式转换为 <see cref="StringView"/>。
	/// </summary>
	/// <param name="text">要转换的字符串。</param>
	/// <returns>转换得到的 <see cref="StringView"/>。</returns>
	public static implicit operator StringView(string? text)
	{
		return new StringView(text);
	}

	/// <summary>
	/// 允许从 <see cref="StringView"/> 隐式转换为 <see cref="ReadOnlySpan{T}"/>。
	/// </summary>
	/// <param name="value">要转换的字符串视图。</param>
	/// <returns>转换得到的 <see cref="ReadOnlySpan{T}"/>。</returns>
	public static implicit operator ReadOnlySpan<char>(StringView value)
	{
		return value.AsSpan();
	}

	/// <summary>
	/// 返回当前对象的字符串表示形式。
	/// </summary>
	/// <returns>当前对象的字符串表示形式。</returns>
	public override string ToString()
	{
		return text.Substring(start, length);
	}

	#region Span

	/// <summary>
	/// 返回当前字符串视图的 <see cref="ReadOnlySpan{T}"/>。
	/// </summary>
	/// <returns>字符串视图的 <see cref="ReadOnlySpan{T}"/> 表示形式。</returns>
	public ReadOnlySpan<char> AsSpan()
	{
		return text.AsSpan(start, length);
	}

	/// <summary>
	/// 返回当前字符串视图的 <see cref="ReadOnlySpan{T}"/>。
	/// </summary>
	/// <param name="start">开始切片处的索引。</param>
	/// <returns>字符串视图的 <see cref="ReadOnlySpan{T}"/> 表示形式。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于 <c>0</c> 或大于 <see cref="Length"/>。</exception>
	public ReadOnlySpan<char> AsSpan(int start)
	{
		if (start < 0 || start > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		return text.AsSpan(this.start + start, length - start);
	}

	/// <summary>
	/// 返回当前字符串视图的 <see cref="ReadOnlySpan{T}"/>。
	/// </summary>
	/// <param name="start">开始切片处的索引。</param>
	/// <param name="length">切片所需的长度。</param>
	/// <returns>字符串视图的 <see cref="ReadOnlySpan{T}"/> 表示形式。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于 <c>0</c> 或大于 <see cref="Length"/>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><c>start + length</c> 表示的位置不在当前字符串视图范围内。</exception>
	public ReadOnlySpan<char> AsSpan(int start, int length)
	{
		if (start < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		if (length < 0 || start + length > this.length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		return text.AsSpan(this.start + start, length);
	}

	#endregion // Span

	#region Memory

	/// <summary>
	/// 返回当前字符串视图的 <see cref="ReadOnlyMemory{T}"/>。
	/// </summary>
	/// <returns>字符串视图的 <see cref="ReadOnlyMemory{T}"/> 表示形式。</returns>
	public ReadOnlyMemory<char> AsMemory()
	{
		return text.AsMemory(start, length);
	}

	/// <summary>
	/// 返回当前字符串视图的 <see cref="ReadOnlyMemory{T}"/>。
	/// </summary>
	/// <param name="start">开始切片处的索引。</param>
	/// <returns>字符串视图的 <see cref="Memory{T}"/> 表示形式。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于 <c>0</c> 或大于 <see cref="Length"/>。</exception>
	public ReadOnlyMemory<char> AsMemory(int start)
	{
		if (start < 0 || start > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		return text.AsMemory(this.start + start, length - start);
	}

	/// <summary>
	/// 返回当前字符串视图的 <see cref="ReadOnlyMemory{T}"/>。
	/// </summary>
	/// <param name="start">开始切片处的索引。</param>
	/// <param name="length">切片所需的长度。</param>
	/// <returns>字符串视图的 <see cref="ReadOnlyMemory{T}"/> 表示形式。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 小于 <c>0</c> 或大于 <see cref="Length"/>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><c>start + length</c> 表示的位置不在当前字符串视图范围内。</exception>
	public ReadOnlyMemory<char> AsMemory(int start, int length)
	{
		if (start < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		if (length < 0 || start + length > this.length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		return text.AsMemory(this.start + start, length);
	}

	#endregion // Memory

	#region IEnumerable<char> 成员

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public IEnumerator<char> GetEnumerator()
	{
		for (int i = 0, j = start; i < length; i++, j++)
		{
			yield return text[j];
		}
	}

	#endregion // IEnumerable<T> 成员

	#region IEnumerable 成员

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	#endregion // IEnumerable 成员

}
