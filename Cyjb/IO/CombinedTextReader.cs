namespace Cyjb.IO;

/// <summary>
/// 表示多个字符读取器的组合。
/// </summary>
internal sealed class CombinedTextReader : TextReader
{
	/// <summary>
	/// 字符读取器的队列。
	/// </summary>
	private readonly Queue<TextReader> readers = new();
	/// <summary>
	/// 当前字符读取器。
	/// </summary>
	private TextReader? currentReader;

	/// <summary>
	/// 使用指定的字符读取器序列初始化 <see cref="CombinedTextReader"/> 类的新实例。
	/// </summary>
	/// <param name="readers">要组合的字符读取器序列。</param>
	public CombinedTextReader(IEnumerable<TextReader> readers)
	{
		foreach (TextReader reader in readers)
		{
			this.readers.Enqueue(reader);
		}
	}

	/// <summary>
	/// 读取下一个字符，而不更改读取器状态。
	/// </summary>
	/// <returns>一个表示下一个要读取的字符的整数；
	/// 如果没有更多可读取的字符或该读取器不支持查找，则为 -1。</returns>
	public override int Peek()
	{
		TextReader? reader = GetCurrentReader();
		while (reader != null)
		{
			int result = reader.Peek();
			if (result != -1)
			{
				return result;
			}
			reader = GetNextReader();
		}
		return -1;
	}

	/// <summary>
	/// 读取下一个字符，并使读取器的位置后移一个字符。
	/// </summary>
	/// <returns>一个表示下一个要读取的字符的整数；如果没有更多可读取的字符，则为 -1。</returns>
	public override int Read()
	{
		TextReader? reader = GetCurrentReader();
		while (reader != null)
		{
			int result = reader.Read();
			if (result != -1)
			{
				return result;
			}
			reader = GetNextReader();
		}
		return -1;
	}

	/// <summary>
	/// 从当前读取器中读取字符，并将数据写入指定的缓冲区。
	/// </summary>
	/// <param name="buffer">要写入的缓冲区。</param>
	/// <returns>已读取的字符数。该值小于等于 <paramref name="buffer"/> 的长度，
	/// 具体取决于读取器中是否有可用的数据。如果调用此方法时没有更多可供读取的字符，
	/// 则返回 <c>0</c>。</returns>
	public override int Read(Span<char> buffer)
	{
		TextReader? reader = GetCurrentReader();
		while (buffer.Length > 0 && reader != null)
		{
			int readCount = reader.Read(buffer);
			if (readCount > 0)
			{
				return readCount;
			}
			// 当前读取器没有有效字符，切换到下一个读取器。
			reader = GetNextReader();
		}
		return 0;
	}

	/// <summary>
	/// 从当前读取器中读取指定个数的字符，并将数据写入指定的缓冲区的指定索引。
	/// </summary>
	/// <param name="buffer">要写入的缓冲区。</param>
	/// <param name="index">要开始写入的索引。</param>
	/// <param name="count">最多读取的字符数。</param>
	/// <returns>已读取的字符数。该值小于等于 <paramref name="count"/>，
	/// 具体取决于读取器中是否有可用的数据。如果调用此方法时没有更多可供读取的字符，
	/// 则返回 <c>0</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="buffer"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于零或加上
	/// <paramref name="index"/> 超出了缓冲区的长度。</exception>
	public override int Read(char[] buffer, int index, int count)
	{
		ArgumentNullException.ThrowIfNull(buffer);
		if (index < 0)
		{
			throw CommonExceptions.ArgumentNegative(index);
		}
		if (count < 0 || index + count > buffer.Length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		TextReader? reader = GetCurrentReader();
		while (count > 0 && reader != null)
		{
			int readCount = reader.Read(buffer, index, count);
			if (readCount > 0)
			{
				return readCount;
			}
			// 当前读取器没有有效字符，切换到下一个读取器。
			reader = GetNextReader();
		}
		return 0;
	}

	/// <summary>
	/// 从当前读取器中读取字符，并将数据写入指定的缓冲区。
	/// </summary>
	/// <param name="buffer">要写入的缓冲区。</param>
	/// <returns>已读取的字符数。该值小于等于 <paramref name="buffer"/> 的长度，
	/// 具体取决于读取器中是否有可用的数据。如果调用此方法时没有更多可供读取的字符，
	/// 则返回 <c>0</c>。</returns>
	/// <remarks>该方法会持续阻塞直到完全填充 <paramref name="buffer"/> 或读取了所有字符。</remarks>
	public override int ReadBlock(Span<char> buffer)
	{
		TextReader? reader = GetCurrentReader();
		int sum = 0;
		while (buffer.Length > 0 && reader != null)
		{
			int readCount = reader.ReadBlock(buffer);
			sum += readCount;
			if (readCount > 0)
			{
				buffer = buffer[readCount..];
			}
			else
			{
				// 当前读取器没有有效字符，切换到下一个读取器。
				reader = GetNextReader();
			}
		}
		return sum;
	}

	/// <summary>
	/// 从当前读取器中读取指定个数的字符，并将数据写入指定的缓冲区的指定索引。
	/// </summary>
	/// <param name="buffer">要写入的缓冲区。</param>
	/// <param name="index">要开始写入的索引。</param>
	/// <param name="count">最多读取的字符数。</param>
	/// <returns>已读取的字符数。该值小于等于 <paramref name="count"/>，
	/// 具体取决于读取器中是否有可用的数据。如果调用此方法时没有更多可供读取的字符，
	/// 则返回 <c>0</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="buffer"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于零或加上
	/// <paramref name="index"/> 超出了缓冲区的长度。</exception>
	/// <remarks>该方法会持续阻塞直到读取 <paramref name="count"/> 个字符或读取了所有字符。</remarks>
	public override int ReadBlock(char[] buffer, int index, int count)
	{
		ArgumentNullException.ThrowIfNull(buffer);
		if (index < 0)
		{
			throw CommonExceptions.ArgumentNegative(index);
		}
		if (count < 0 || index + count > buffer.Length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		TextReader? reader = GetCurrentReader();
		int sum = 0;
		while (count > 0 && reader != null)
		{
			int readCount = reader.Read(buffer, index, count);
			sum += readCount;
			if (readCount > 0)
			{
				index += readCount;
				count -= readCount;
			}
			else
			{
				// 当前读取器没有有效字符，切换到下一个读取器。
				reader = GetNextReader();
			}
		}
		return sum;
	}

	/// <summary>
	/// 释放由 <see cref="CombinedTextReader"/> 占用的资源。
	/// </summary>
	/// <param name="disposing">若要释放托管资源和非托管资源，则为 <c>true</c>；
	/// 若仅释放非托管资源，则为 <c>false</c>。</param>
	protected override void Dispose(bool disposing)
	{
		currentReader?.Dispose();
		currentReader = null;
		foreach (TextReader reader in readers)
		{
			reader.Dispose();
		}
		readers.Clear();
		base.Dispose(disposing);
	}

	/// <summary>
	/// 返回当前字符读取器。
	/// </summary>
	/// <returns>当前字符读取器。</returns>
	private TextReader? GetCurrentReader()
	{
		if (currentReader == null && readers.Count > 0)
		{
			currentReader = readers.Dequeue();
		}
		return currentReader;
	}

	/// <summary>
	/// 返回下一个字符读取器。
	/// </summary>
	/// <returns>下一个字符读取器。</returns>
	private TextReader? GetNextReader()
	{
		currentReader?.Dispose();
		if (readers.Count > 0)
		{
			currentReader = readers.Dequeue();
		}
		else
		{
			currentReader = null;
		}
		return currentReader;
	}
}
