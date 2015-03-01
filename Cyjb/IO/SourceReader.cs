using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
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
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int BufferSize = 0x200;
		/// <summary>
		/// 当前存有数据的缓冲区的指针。
		/// </summary>
		private SourceBuffer current;
		/// <summary>
		/// 最后一个存有数据的缓冲区的指针。
		/// </summary>
		private SourceBuffer last;
		/// <summary>
		/// 第一个存有数据的缓冲区的指针。
		/// </summary>
		private SourceBuffer first;
		/// <summary>
		/// 文本的读取器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TextReader reader;
		/// <summary>
		/// 字符的占位符。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Queue<Tuple<int, string>> placeHolders = new Queue<Tuple<int, string>>();
		/// <summary>
		/// 当前的字符索引。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int index;
		/// <summary>
		/// 全局字符索引。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int globalIndex;
		/// <summary>
		/// 与 <see cref="StartPosition"/> 对应的全局字符索引，
		/// 因为 placeHolder 可能导致 <see cref="StartPosition"/> 的索引不正确。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int startIndex;
		/// <summary>
		/// 当前缓冲区的字符长度。
		/// </summary>
		private int length;
		/// <summary>
		/// 第一块缓冲区的字符索引。
		/// </summary>
		private int firstIndex;
		/// <summary>
		/// 最后一块缓冲区的字符长度。
		/// </summary>
		private int lastLength;
		/// <summary>
		/// 用于构造字符串的 <see cref="StringBuilder"/> 实例。
		/// </summary>
		private StringBuilder builder;
		/// <summary>
		/// 构造字符串时的起始索引。
		/// </summary>
		private int builderIndex = -1;
		/// <summary>
		/// 已向 <see cref="builder"/> 中复制了的字符串长度。
		/// </summary>
		private int builderCopiedLen;
		/// <summary>
		/// 源代码位置计数器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourceLocator locator;
		/// <summary>
		/// 使用指定的字符读取器初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// <param name="reader">用于读取源文件的字符读取器。</param>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public SourceReader(TextReader reader)
			: this(reader, SourcePosition.Unknown, SourceLocator.DefaultTabSize)
		{ }
		/// <summary>
		/// 使用指定的字符读取器和 Tab 宽度初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// <param name="reader">用于读取源文件的字符读取器。</param>
		/// <param name="tabSize">Tab 的宽度。</param>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="tabSize"/> 小于等于 <c>0</c>。</exception>
		public SourceReader(TextReader reader, int tabSize)
			: this(reader, SourcePosition.Unknown, tabSize)
		{ }
		/// <summary>
		/// 使用指定的字符读取器和起始位置初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// <param name="reader">用于读取源文件的字符读取器。</param>
		/// <param name="initPosition">起始位置。</param>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> 为 <c>null</c>。</exception>
		public SourceReader(TextReader reader, SourcePosition initPosition)
			: this(reader, initPosition, SourceLocator.DefaultTabSize)
		{ }
		/// <summary>
		/// 使用指定的字符读取器、起始位置和 Tab 宽度初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// <param name="reader">用于读取源文件的字符读取器。</param>
		/// <param name="initPosition">起始位置。</param>
		/// <param name="tabSize">Tab 的宽度。</param>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="tabSize"/> 小于等于 <c>0</c>。</exception>
		public SourceReader(TextReader reader, SourcePosition initPosition, int tabSize)
			: this(initPosition, tabSize)
		{
			CommonExceptions.CheckArgumentNull(reader, "reader");
			if (tabSize <= 0)
			{
				throw CommonExceptions.ArgumentMustBePositive("tabSize", tabSize);
			}
			Contract.EndContractBlock();
			this.reader = reader;
		}
		/// <summary>
		/// 使用指定的起始位置和 Tab 宽度初始化 <see cref="SourceReader"/> 类的新实例。
		/// </summary>
		/// <param name="initPosition">起始位置。</param>
		/// <param name="tabSize">Tab 的宽度。</param>
		private SourceReader(SourcePosition initPosition, int tabSize)
		{
			locator = new SourceLocator(initPosition, tabSize);
			current = first = last = new SourceBuffer();
			firstIndex = lastLength = 0;
			current.Next = current;
			current.StartIndex = 0;
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
		/// <remarks>由于允许通过 <see cref="PlaceHolders"/> 属性更改字符所占用的范围，因此 
		/// <see cref="Index"/> 属性可能与 <see cref="StartPosition"/> 和 <see cref="BeforeStartPosition"/> 
		/// 的 <c>Index</c> 属性不同。</remarks>
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
		public SourcePosition BeforeStartPosition
		{
			get { return locator.Position; }
		}
		/// <summary>
		/// 获取或设置起始索引的源代码位置（不包括被丢弃的字符）。
		/// </summary>
		/// <value>起始索引的源代码位置。起始索引会从上一次丢弃或接受字符开始算起。</value>
		/// <remarks>不会改变 <see cref="BeforeStartPosition"/> 的位置。</remarks>
		public SourcePosition StartPosition
		{
			get { return locator.NextPosition; }
			set { locator.NextPosition = value; }
		}
		/// <summary>
		/// 获取字符的占位符队列，可以修改特定索引字符所占的长度。
		/// </summary>
		/// <value>字符的占位符，特定索引字符所占的长度由占位符指定。</value>
		/// <remarks>
		/// <para>该队列的每一项，表示了特定索引的字符索引及其所占的长度，
		/// 字符的索引必须按从小到大排序，且只有在该字符计算位置（<see cref="Drop"/> 或 <see cref="Accept"/>）
		/// 之前设置占位符才有效。</para>
		/// <para>字符的计算位置之后，相应的占位符设置会被清除。</para>
		/// </remarks>
		public Queue<Tuple<int, string>> PlaceHolders
		{
			get { return this.placeHolders; }
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
			if (this.reader == null)
			{
				return;
			}
			this.reader.Dispose();
			this.reader = null;
			this.current = this.last = this.first = null;
			this.builder = null;
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
			CheckDisposed();
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
		/// <exception cref="ObjectDisposedException">当前 <see cref="SourceReader"/> 已关闭。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="idx"/> 小于 <c>0</c>。</exception>
		public int Peek(int idx)
		{
			if (idx < 0)
			{
				throw CommonExceptions.ArgumentNegative("idx", idx);
			}
			Contract.EndContractBlock();
			CheckDisposed();
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
			CheckDisposed();
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
		/// <exception cref="ObjectDisposedException">当前 <see cref="SourceReader"/> 已关闭。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="idx"/> 小于 <c>0</c>。</exception>
		public int Read(int idx)
		{
			if (idx < 0)
			{
				throw CommonExceptions.ArgumentNegative("idx", idx);
			}
			Contract.EndContractBlock();
			CheckDisposed();
			if (idx == 0)
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
			CheckDisposed();
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
			if (index > firstIndex)
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
		/// <exception cref="ObjectDisposedException">当前 <see cref="SourceReader"/> 已关闭。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 <c>0</c>。</exception>
		public int Unget(int count)
		{
			if (count < 0)
			{
				throw CommonExceptions.ArgumentNegative("count", count);
			}
			Contract.EndContractBlock();
			CheckDisposed();
			if (count == 0)
			{
				return 0;
			}
			if (count == 1)
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
						index = firstIndex;
					}
					else
					{
						backCount += count;
						index -= count;
					}
					break;
				}
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
			globalIndex -= backCount;
			return backCount;
		}
		/// <summary>
		/// 返回当前位置之前的数据。
		/// </summary>
		/// <returns>当前位置之前的数据。</returns>
		public string ReadedBlock()
		{
			CheckDisposed();
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
			CheckDisposed();
			while (first != current)
			{
				this.Forward(first, firstIndex, BufferSize - firstIndex);
				startIndex += BufferSize - firstIndex;
				firstIndex = 0;
				first = first.Next;
			}
			this.Forward(current, firstIndex, index - firstIndex);
			startIndex += index - firstIndex;
			firstIndex = index;
		}
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// <returns>当前位置之前的数据。</returns>
		public string Accept()
		{
			CheckDisposed();
			InitBuilder();
			// 将字符串复制到 StringBuilder 中。
			while (first != current)
			{
				CopyToBuilder(first, firstIndex, BufferSize - firstIndex);
				this.Forward(first, firstIndex, BufferSize - firstIndex);
				startIndex += BufferSize - firstIndex;
				firstIndex = 0;
				first = first.Next;
			}
			CopyToBuilder(first, firstIndex, index - firstIndex);
			this.Forward(current, firstIndex, index - firstIndex);
			startIndex += index - firstIndex;
			firstIndex = index;
			builder.Length = builderCopiedLen;
			return builder.ToString();
		}
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并以 <see cref="Cyjb.Text.Token{T}"/> 
		/// 的形式返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
		/// <param name="id">返回的 <see cref="Cyjb.Text.Token{T}"/> 的标识符。</param>
		/// <returns>当前位置之前的数据。</returns>
		/// <overloads>
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并以 <see cref="Cyjb.Text.Token{T}"/> 
		/// 的形式返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// </overloads>
		public Token<T> AcceptToken<T>(T id)
			where T : struct
		{
			return AcceptToken(id, null);
		}
		/// <summary>
		/// 将当前位置之前的数据全部丢弃，并以 <see cref="Cyjb.Text.Token{T}"/> 
		/// 的形式返回被丢弃的数据。
		/// 之后的 <see cref="Unget()"/> 操作至多回退到当前位置。
		/// </summary>
		/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
		/// <param name="id">返回的 <see cref="Cyjb.Text.Token{T}"/> 的标识符。</param>
		/// <param name="value"><see cref="Cyjb.Text.Token{T}"/> 的值。</param>
		/// <returns>当前位置之前的数据。</returns>
		public Token<T> AcceptToken<T>(T id, object value)
			where T : struct
		{
			SourcePosition start = locator.NextPosition;
			return new Token<T>(id, Accept(), start, locator.Position, value);
		}
		/// <summary>
		/// 检查是否已释放当前读取器。
		/// </summary>
		private void CheckDisposed()
		{
			if (this.reader == null)
			{
				throw CommonExceptions.StreamClosed(typeof(SourceReader));
			}
		}
		/// <summary>
		/// 令源代码位置计数器前进指定缓冲的部分。
		/// </summary>
		/// <param name="buffer">要前进的字符的缓冲区。</param>
		/// <param name="start">要前进的字符的起始长度。</param>
		/// <param name="len">要前进的字符的长度。</param>
		private void Forward(SourceBuffer buffer, int start, int len)
		{
			Tuple<int, string> placeholder = CurrentPlaceHolder();
			if (placeholder == null)
			{
				locator.Forward(buffer.Buffer, start, len);
			}
			else
			{
				int end = start + len;
				int local = placeholder.Item1 - buffer.StartIndex;
				while (local < end)
				{
					this.placeHolders.Dequeue();
					locator.Forward(buffer.Buffer, start, local - start);
					locator.Forward(placeholder.Item2);
					start = local + 1;
					if ((placeholder = CurrentPlaceHolder()) == null)
					{
						break;
					}
					local = placeholder.Item1 - buffer.StartIndex;
				}
				if (start < end)
				{
					locator.Forward(buffer.Buffer, start, end - start);
				}
			}
		}
		/// <summary>
		/// 获取当前的占位符。
		/// </summary>
		/// <returns>当前的占位符，如果不存在则为 <c>null</c>。</returns>
		private Tuple<int, string> CurrentPlaceHolder()
		{
			Tuple<int, string> tuple = null;
			while (this.placeHolders.Count > 0 && (tuple = this.placeHolders.Peek()) == null)
			{
				this.placeHolders.Dequeue();
			}
			return tuple;
		}
		/// <summary>
		/// 初始化构造字符串的 <see cref="StringBuilder"/>。
		/// </summary>
		private void InitBuilder()
		{
			if (builder == null)
			{
				builder = new StringBuilder(BufferSize);
				builderIndex = startIndex;
			}
			else if (builderIndex != startIndex)
			{
				builder.Clear();
				builderIndex = startIndex;
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
			Contract.Requires(start >= 0);
			Contract.Requires(start + len <= BufferSize);
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
			Contract.Requires(index == length);
			Contract.Requires(reader != null);
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
					SourceBuffer buffer = new SourceBuffer
					{
						Next = last.Next,
						Prev = current
					};
					last.Next.Prev = buffer;
					last.Next = buffer;
				}
				last = last.Next;
				last.StartIndex = last.Prev.StartIndex + BufferSize;
			}
			else
			{
				// len 为 0 应仅当 last == current 时。
				Contract.Assert(last == current);
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
			Contract.Requires(current != first);
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
			public readonly char[] Buffer = new char[BufferSize];
			/// <summary>
			/// 字符缓冲区起始位置的字符索引。
			/// </summary>
			public int StartIndex;
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
				return string.Concat("{", new string(Buffer.Where(ch => ch != '\0').ToArray()), "}");
			}
		}

		#endregion // 缓冲区操作

	}
}
