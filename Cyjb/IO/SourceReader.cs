using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Cyjb.Text;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示支持行列计数的源文件读取器。
	/// </summary>
	/// <remarks><see cref="SourceReader"/> 类中，包含一个环形字符缓冲区，
	/// 关于环形字符缓冲区的详细解释，请参见我的 C# 词法分析器系列博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/LexerInputBuffer.html">
	/// 《C# 词法分析器（二）输入缓冲和代码定位》</see>。</remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/LexerInputBuffer.html">
	/// 《C# 词法分析器（二）输入缓冲和代码定位》</seealso>
	public sealed class SourceReader : IDisposable
	{
		/// <summary>
		/// 缓冲区的大小。
		/// </summary>
		private const int BufferSize = 0x200;
		/// <summary>
		/// 当前存有数据的缓冲区的指针。
		/// </summary>
		private SourceBuffer current = null;
		/// <summary>
		/// 最后一个存有数据的缓冲区的指针。
		/// </summary>
		private SourceBuffer last = null;
		/// <summary>
		/// 第一个存有数据的缓冲区的指针。
		/// </summary>
		private SourceBuffer first = null;
		/// <summary>
		/// 文本的读取器。
		/// </summary>
		private TextReader reader = null;
		/// <summary>
		/// 当前的字符索引。
		/// </summary>
		private int index = 0;
		/// <summary>
		/// 全局字符索引。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int globalIndex = 0;
		/// <summary>
		/// 当前缓冲区的字符长度。
		/// </summary>
		private int length = 0;
		/// <summary>
		/// 第一块缓冲区的字符索引。
		/// </summary>
		private int firstIndex = 0;
		/// <summary>
		/// 最后一块缓冲区的字符长度。
		/// </summary>
		private int lastLength = 0;
		/// <summary>
		/// 用于构造字符串的 <see cref="StringBuilder"/> 实例。
		/// </summary>
		private StringBuilder builder = null;
		/// <summary>
		/// 构造字符串时的起始索引。
		/// </summary>
		private int builderIndex = -1;
		/// <summary>
		/// 已向 <see cref="builder"/> 中复制了的字符串长度。
		/// </summary>
		private int builderCopiedLen = 0;
		/// <summary>
		/// 源代码位置计数器。
		/// </summary>
		private SourceLocator locator;
		/// <summary>
		/// 使用指定的字符读取器初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// <param name="reader">用于读取源文件的字符读取器。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public SourceReader(TextReader reader) : this(reader, 4) { }
		/// <summary>
		/// 使用指定的字符读取器和 Tab 宽度初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// <param name="reader">用于读取源文件的字符读取器。</param>
		/// <param name="tabSize">Tab 的宽度。</param>
		public SourceReader(TextReader reader, int tabSize)
		{
			ExceptionHelper.CheckArgumentNull(reader, "reader");
			locator = new SourceLocator(tabSize);
			this.reader = reader;
			current = first = last = new SourceBuffer();
			firstIndex = lastLength = 0;
			current.Buffer = new char[BufferSize];
			current.Next = current;
		}
		/// <summary>
		/// 获取基础的字符读取器。
		/// </summary>
		/// <value>基础的字符读取器。</value>
		public TextReader BaseReader
		{
			get { return reader; }
		}
		/// <summary>
		/// 获取或设置当前的字符索引。
		/// </summary>
		/// <value>当前的字符索引，该索引从零开始。设置索引时，不能达到被丢弃的字符，或者超出文件结尾。</value>
		public int Index
		{
			get { return globalIndex; }
			set
			{
				if (value > this.globalIndex)
				{
					this.Read(value - this.globalIndex - 1);
				}
				else if (value < this.globalIndex)
				{
					this.Unget(this.globalIndex - value);
				}
			}
		}
		/// <summary>
		/// 获取起始索引之前的源代码位置（不包括被丢弃的字符）。
		/// </summary>
		/// <value>起始索引之前的源代码位置。起始索引会从上一次丢弃或接受字符开始算起。</value>
		public SourceLocation BeforeStartLocation
		{
			get { return locator.Location; }
		}
		/// <summary>
		/// 起始索引的源代码位置（不包括被丢弃的字符）。
		/// </summary>
		/// <value>起始索引的源代码位置。起始索引会从上一次丢弃或接受字符开始算起。</value>
		public SourceLocation StartLocation
		{
			get { return locator.NextLocation; }
		}
		/// <summary>
		/// 关闭 <see cref="SourceReader"/> 对象和基础字符读取器，并释放与读取器关联的所有系统资源。
		/// </summary>
		public void Close()
		{
			this.Dispose();
		}

		#region IDisposable 成员

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		public void Dispose()
		{
			if (reader != null)
			{
				reader.Dispose();
				reader = null;
			}
			GC.SuppressFinalize(this);
		}

		#endregion

		#region 读取字符

		/// <summary>
		/// 返回下一个可用的字符，但不使用它。
		/// </summary>
		/// <returns>表示下一个要读取的字符的整数，或者如果没有要读取的字符，则为 <c>-1</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 返回之后可用的字符，但不使用它。
		/// </summary>
		/// </overloads>
		public int Peek()
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			if (index == length)
			{
				if (!SwitchNextBuffer())
				{
					return -1;
				}
			}
			return current.Buffer[index];
		}
		/// <summary>
		/// 返回文本读取器中之后的 <paramref name="idx"/> 索引的字符，但不使用它。
		/// Peek(0) 就相当于 Peek()，但效率不如 Peek()。
		/// </summary>
		/// <returns>文本读取器中之后的 <paramref name="idx"/> 索引的字符，
		/// 或为 <c>-1</c>（如果没有更多的可用字符）。</returns>
		public int Peek(int idx)
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			if (idx < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("count");
			}
			SourceBuffer temp = current;
			int tempLen = length;
			idx += index;
			while (true)
			{
				if (idx >= tempLen)
				{
					idx -= tempLen;
					if (temp == last && (tempLen = PrepareBuffer()) == 0)
					{
						// 没有可读数据了，返回。
						return -1;
					}
					temp = temp.Next;
				}
				else
				{
					return temp.Buffer[idx];
				}
			}
		}
		/// <summary>
		/// 读取文本读取器中的下一个字符并使该字符的位置提升一个字符。
		/// </summary>
		/// <returns>文本读取器中的下一个字符，或为 <c>-1</c>（如果没有更多的可用字符）。</returns>
		/// <overloads>
		/// <summary>
		/// 返回之后可用的字符，并使该字符的位置提升。
		/// </summary>
		/// </overloads>
		public int Read()
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			if (index == length)
			{
				if (!SwitchNextBuffer())
				{
					return -1;
				}
			}
			globalIndex++;
			return current.Buffer[index++];
		}
		/// <summary>
		/// 读取文本读取器中之后的 <paramref name="idx"/> 索引的字符，并使该字符的位置提升。
		/// <c>Read(0)</c> 与 <see cref="Read()"/> 等价。
		/// </summary>
		/// <returns>文本读取器中之后的 <paramref name="idx"/> 索引的字符，
		/// 或为 <c>-1</c>（如果没有更多的可用字符）。</returns>
		public int Read(int idx)
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			if (idx < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("idx");
			}
			else if (idx == 0)
			{
				return Read();
			}
			while (true)
			{
				if (idx >= length - index)
				{
					globalIndex += length - index;
					idx -= length - index;
					index = length;
					if (!SwitchNextBuffer())
					{
						// 没有数据了，返回。
						index = length;
						return -1;
					}
				}
				else
				{
					globalIndex += idx + 1;
					index += idx;
					return current.Buffer[index++];
				}
			}
		}
		/// <summary>
		/// 回退最后被读取的字符，只有之前的数据未被丢弃时才可以进行回退。
		/// </summary>
		/// <returns>如果回退成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 回退最后被读取的字符，只有之前的数据未被丢弃时才可以进行回退。
		/// </summary>
		/// </overloads>
		public bool Unget()
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			if (current != first)
			{
				if (index <= 0)
				{
					SwitchPrevBuffer();
				}
				globalIndex--;
				index--;
				return true;
			}
			else if (index > firstIndex)
			{
				globalIndex--;
				index--;
				return true;
			}
			return false;
		}
		/// <summary>
		/// 回退 <paramref name="count"/> 个字符，只有之前的数据未被丢弃时才可以进行回退。
		/// <c>Unget(1)</c> 与 <see cref="Unget()"/> 等价。
		/// </summary>
		/// <param name="count">要回退的字符个数。</param>
		/// <returns>实际回退的字符个数，小于等于 <paramref name="count"/>。</returns>
		public int Unget(int count)
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			if (count < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("count");
			}
			else if (count == 0)
			{
				return 0;
			}
			else if (count == 1)
			{
				return this.Unget() ? 1 : 0;
			}
			int backCount = 0;
			while (true)
			{
				if (current == first)
				{
					int charCnt = index - firstIndex;
					if (count > charCnt)
					{
						backCount += charCnt;
						count -= charCnt;
						index = firstIndex;
					}
					else
					{
						backCount += count;
						index -= count;
					}
					break;
				}
				else
				{
					if (count > index)
					{
						backCount += index;
						count -= index;
						SwitchPrevBuffer();
					}
					else
					{
						backCount += count;
						index -= count;
						break;
					}
				}
			}
			globalIndex -= backCount;
			return backCount;
		}
		/// <summary>
		/// 返回当前位置之前的数据。
		/// </summary>
		/// <returns>当前位置之前的数据。</returns>
		public string ReadedBlock()
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			InitBuilder();
			// 将字符串复制到 StringBuilder 中。
			SourceBuffer buf = first;
			int fIndex = firstIndex;
			while (buf != current)
			{
				CopyToBuilder(buf, fIndex, BufferSize - fIndex);
				fIndex = 0;
				buf = buf.Next;
			}
			CopyToBuilder(buf, fIndex, index - fIndex);
			builder.Length = builderCopiedLen;
			return builder.ToString();
		}
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		public void Drop()
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			while (first != current)
			{
				locator.Forward(first.Buffer, firstIndex, BufferSize - firstIndex);
				firstIndex = 0;
				first = first.Next;
			}
			locator.Forward(current.Buffer, firstIndex, index - firstIndex);
			firstIndex = index;
		}
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// <returns>当前位置之前的数据。</returns>
		public string Accept()
		{
			if (reader == null)
			{
				throw ExceptionHelper.SourceReaderClosed();
			}
			InitBuilder();
			// 将字符串复制到 StringBuilder 中。
			while (first != current)
			{
				CopyToBuilder(first, firstIndex, BufferSize - firstIndex);
				locator.Forward(first.Buffer, firstIndex, BufferSize - firstIndex);
				firstIndex = 0;
				first = first.Next;
			}
			CopyToBuilder(first, firstIndex, index - firstIndex);
			locator.Forward(current.Buffer, firstIndex, index - firstIndex);
			firstIndex = index;
			builder.Length = builderCopiedLen;
			return builder.ToString();
		}
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并以 <see cref="Cyjb.Text.Token"/> 
		/// 的形式返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// <param name="id">返回的 <see cref="Cyjb.Text.Token"/> 的标识符。</param>
		/// <returns>当前位置之前的数据。</returns>
		/// <overloads>
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并以 <see cref="Cyjb.Text.Token"/> 
		/// 的形式返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// </overloads>
		public Token AcceptToken(string id)
		{
			return AcceptToken(id, null);
		}
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并以 <see cref="Cyjb.Text.Token"/> 
		/// 的形式返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// <param name="id">返回的 <see cref="Cyjb.Text.Token"/> 的标识符。</param>
		/// <param name="value"><see cref="Cyjb.Text.Token"/> 的值。</param>
		/// <returns>当前位置之前的数据。</returns>
		public Token AcceptToken(string id, object value)
		{
			SourceLocation start = locator.NextLocation;
			return new Token(id, Accept(), start, locator.Location, value);
		}
		/// <summary>
		/// 初始化构造字符串的 <see cref="StringBuilder"/>。
		/// </summary>
		private void InitBuilder()
		{
			if (builder == null)
			{
				builder = new StringBuilder(BufferSize);
				builderIndex = StartLocation.Index;
			}
			else if (builderIndex != StartLocation.Index)
			{
				builder.Clear();
				builderIndex = StartLocation.Index;
			}
			builderCopiedLen = 0;
		}
		/// <summary>
		/// 将指定缓冲区中从指定索引开始，指定长度的字符串复制到 <see cref="builder"/> 中。
		/// </summary>
		/// <param name="buffer">要复制字符串的缓冲区。</param>
		/// <param name="start">要复制的起始长度。</param>
		/// <param name="len">要复制的长度。</param>
		private void CopyToBuilder(SourceBuffer buffer, int start, int len)
		{
			Debug.Assert(start >= 0);
			Debug.Assert(start + len <= BufferSize);
			if (builderCopiedLen == builder.Length)
			{
				builder.Append(buffer.Buffer, start, len);
				builderCopiedLen += len;
			}
			else if ((builderCopiedLen += len) > builder.Length)
			{
				int l = builderCopiedLen - builder.Length;
				builder.Append(buffer.Buffer, len - l, l);
			}
		}

		#endregion // 读取字符

		#region 缓冲区操作

		/// <summary>
		/// 切换到下一块缓冲区。如果没有有效的数据，则从基础字符读取器中读取字符，并填充到缓冲区中。
		/// </summary>
		/// <returns>如果切换成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool SwitchNextBuffer()
		{
			Debug.Assert(index == length);
			Debug.Assert(reader != null);
			if (current == last)
			{
				// 下一块缓冲区没有数据，需要从基础字符读取器中读取。
				if (PrepareBuffer() == 0)
				{
					return false;
				}
				length = lastLength;
				current = last;
			}
			else
			{
				// 下一块缓冲区有数据，直接后移。
				current = current.Next;
				if (current == last)
				{
					length = lastLength;
				}
			}
			index = 0;
			return length > 0;
		}
		/// <summary>
		/// 从基础字符读取器中读取字符，并填充到新的缓冲区中。
		/// </summary>
		/// <returns>从基础字符读取器中读取的字符数量。</returns>
		private int PrepareBuffer()
		{
			if (reader.Peek() == -1)
			{
				return 0;
			}
			if (length > 0)
			{
				if (last.Next == first)
				{
					// 没有可用的空缓冲区，则需要新建立一块。
					SourceBuffer buffer = new SourceBuffer();
					buffer.Buffer = new char[BufferSize];
					buffer.Next = last.Next;
					buffer.Prev = current;
					last.Next.Prev = buffer;
					last.Next = buffer;
				}
				last = last.Next;
			}
			else
			{
				// len 为 0 应仅当 last == current 时。
				Debug.Assert(last == current);
			}
			lastLength = reader.ReadBlock(last.Buffer, 0, BufferSize);
			if (length == 0)
			{
				length = lastLength;
			}
			return lastLength;
		}
		/// <summary>
		/// 切换到上一块缓冲区。
		/// </summary>
		private void SwitchPrevBuffer()
		{
			Debug.Assert(current != first);
			current = current.Prev;
			index = length = BufferSize;
		}
		/// <summary>
		/// 表示 <see cref="Cyjb.IO.SourceReader"/> 的字符缓冲区。
		/// </summary>
		private sealed class SourceBuffer
		{
			/// <summary>
			/// 字符缓冲区。
			/// </summary>
			public char[] Buffer;
			/// <summary>
			/// 下一个字符缓冲区。
			/// </summary>
			public SourceBuffer Next;
			/// <summary>
			/// 上一个字符缓冲区。
			/// </summary>
			public SourceBuffer Prev;
			/// <summary>
			/// 返回当前对象的字符串表示形式。
			/// </summary>
			/// <returns>当前对象的字符串表示形式。</returns>
			public override string ToString()
			{
				return string.Concat("{", new string(Buffer), "}");
			}
		}

		#endregion // 缓冲区操作

	}
}
