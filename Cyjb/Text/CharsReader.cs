using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace Cyjb.Text
{
	/// <summary>
	/// 实现从字符序列进行读取的 <see cref="TextReader"/>。
	/// </summary>
	public class CharsReader : TextReader
	{
		/// <summary>
		/// 字符序列的枚举器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IEnumerator<char> chars;
		/// <summary>
		/// 要读取的下一个字符，一般等于 chars.Current。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int nextChar;
		/// <summary>
		/// 使用指定的字符序列初始化 <see cref="CharsReader"/> 类的新实例。
		/// </summary>
		/// <param name="chars">要读取的字符序列。</param>
		/// <exception cref="ArgumentNullException"><paramref name="chars"/> 为 <c>null</c>。</exception>
		public CharsReader(IEnumerable<char> chars)
		{
			if (chars == null)
			{
				throw CommonExceptions.ArgumentNull("chars");
			}
			Contract.EndContractBlock();
			this.chars = chars.GetEnumerator();
			this.nextChar = this.NextChar();
		}

		#region IDisposable 成员

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		/// <param name="disposing">是否释放托管资源。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.chars != null)
			{
				this.chars.Dispose();
				this.chars = null;
			}
			base.Dispose(disposing);
		}

		#endregion // IDisposable 成员

		/// <summary>
		/// 返回下一个可用的字符，但不使用它。
		/// </summary>
		/// <returns>一个表示下一个要读取的字符的整数；如果没有更多可读取的字符，则为 <c>-1</c>。</returns>
		[Pure]
		public override int Peek()
		{
			CheckDisposed();
			return this.nextChar;
		}
		/// <summary>
		/// 读取文本读取器中的下一个字符并使该字符的位置提升一个字符。
		/// </summary>
		/// <returns>文本读取器中的下一个字符，或为 <c>-1</c>（如果没有更多的可用字符）。</returns>
		public override int Read()
		{
			CheckDisposed();
			int result = this.nextChar;
			if (result != -1)
			{
				this.nextChar = this.NextChar();
			}
			return result;
		}
		/// <summary>
		/// 读取输入字符序列中的字符块，并将字符位置提升 <paramref name="count"/>。
		/// </summary>
		/// <param name="buffer">读取字符的缓冲区。</param>
		/// <param name="index">缓存区中的起始索引。</param>
		/// <param name="count">要读取的字符数。</param>
		/// <returns>读入缓冲区的总字符数。如果当前没有足够多的可用字符，则总字符数可能会少于所请求的字符数；
		/// 若已到达字符序列的结尾，则总字符数为零。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 或 
		/// <paramref name="count"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="index"/> 和 <paramref name="count"/> 
		/// 不指定 <paramref name="buffer"/> 中的有效范围。</exception>
		public override int Read(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw CommonExceptions.ArgumentNull("buffer");
			}
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			if (count < 0)
			{
				throw CommonExceptions.ArgumentNegative("count", count);
			}
			if (index + count > buffer.Length)
			{
				throw CommonExceptions.InvalidOffsetLength();
			}
			Contract.EndContractBlock();
			CheckDisposed();
			if (count == 0 || this.nextChar == -1)
			{
				return count;
			}
			int end = index + count;
			for (int i = index; i < end; i++)
			{
				buffer[i] = this.chars.Current;
				if (!this.chars.MoveNext())
				{
					this.nextChar = -1;
					return i - index;
				}
			}
			this.nextChar = this.chars.Current;
			return count;
		}
		/// <summary>
		/// 读取当前位置的任何字符，直到字符序列的末尾并将其返回作为一个字符串。
		/// </summary>
		/// <returns>从当前位置到字符序列的结尾之间的内容。</returns>
		public override String ReadToEnd()
		{
			CheckDisposed();
			if (this.nextChar == -1)
			{
				return string.Empty;
			}
			StringBuilder text = new StringBuilder(2);
			text.Append(this.chars.Current);
			while (this.chars.MoveNext())
			{
				text.Append(this.chars.Current);
			}
			this.nextChar = -1;
			return text.ToString();
		}
		/// <summary>
		/// 从字符序列中读取一行字符串。
		/// </summary>
		/// <returns>字符序列中的下一行，若到达了字符序列的末尾则为 <c>null</c>。</returns>
		public override String ReadLine()
		{
			CheckDisposed();
			if (this.nextChar == -1)
			{
				return null;
			}
			StringBuilder text = new StringBuilder(2);
			while (true)
			{
				if (this.chars.Current == '\n')
				{
					this.nextChar = this.NextChar();
					break;
				}
				if (this.chars.Current == '\r')
				{
					if ((this.nextChar = this.NextChar()) == '\n')
					{
						this.nextChar = this.NextChar();
					}
					break;
				}
				text.Append(this.chars.Current);
				if (!this.chars.MoveNext())
				{
					this.nextChar = -1;
					break;
				}
			}
			return text.ToString();
		}
		/// <summary>
		/// 获取字符序列中的下一个字符。
		/// </summary>
		/// <returns>字符序列中的下一个字符。</returns>
		private int NextChar()
		{
			Contract.Requires(this.chars != null);
			if (this.chars.MoveNext())
			{
				return this.chars.Current;
			}
			return -1;
		}
		/// <summary>
		/// 检查是否已释放当前读取器。
		/// </summary>
		private void CheckDisposed()
		{
			if (this.chars == null)
			{
				throw CommonExceptions.TextReaderClosed();
			}
		}
	}
}
