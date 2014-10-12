using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb.IO
{
	/// <summary>
	/// 提供可以用于在要定位的方法。
	/// 支持 \n（Unix） 和 \r\n（Windows） 作为换行符。
	/// </summary>
	/// <remarks>
	/// 对代码定位方法的详细解释，请参见我的 C# 词法分析器系列博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/LexerInputBuffer.html#SourceLocation">
	/// 《C# 词法分析器（二）输入缓冲和代码定位》</see>。</remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/LexerInputBuffer.html#SourceLocation">
	/// 《C# 词法分析器（二）输入缓冲和代码定位》</seealso>
	[DebuggerDisplay("{Position}")]
	public sealed class SourceLocator
	{
		/// <summary>
		/// 默认的 Tab 长度。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const int DefaultTabSize = 4;
		/// <summary>
		/// 不存在的代理项。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const char NoSurrogate = '\0';
		/// <summary>
		/// Tab 的宽度。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int tabSize;
		/// <summary>
		/// 代理项对的待匹配代理项。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private char surrogate = NoSurrogate;
		/// <summary>
		/// 当前字符所在的行。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int curLine = 1;
		/// <summary>
		/// 当前字符所在的列，若为 <c>0</c> 则表示当前位置无效。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int curCol;
		/// <summary>
		/// 下一个字符所在的行。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int nextLine = 1;
		/// <summary>
		/// 下一个字符所在的列。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int nextCol = 1;
		/// <summary>
		/// 当前字符从零开始的索引。
		/// </summary>
		private int curIndex = -1;
		/// <summary>
		/// 被 TAB 字符的字符串宽度列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<int> widths = new List<int>();
		/// <summary>
		/// 使用默认的 Tab 宽度初始化 <see cref="SourceLocator"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="SourceLocator"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public SourceLocator() : this(DefaultTabSize) { }
		/// <summary>
		/// 使用指定的 Tab 宽度初始化 <see cref="SourceLocator"/> 类的新实例。
		/// </summary>
		/// <param name="tabSize">Tab 的宽度。</param>
		public SourceLocator(int tabSize)
		{
			this.tabSize = tabSize;
		}
		/// <summary>
		/// 使用指定的起始位置和 Tab 宽度初始化 <see cref="SourceLocator"/> 类的新实例。
		/// </summary>
		/// <param name="initPosition">起始位置。</param>
		/// <param name="tabSize">Tab 的宽度。</param>
		public SourceLocator(SourcePosition initPosition, int tabSize)
		{
			this.tabSize = tabSize;
			this.NextPosition = initPosition;
		}
		/// <summary>
		/// 获取 Tab 的宽度。
		/// </summary>
		/// <value>Tab 字符的宽度。</value>
		public int TabSize
		{
			get { return tabSize; }
		}
		/// <summary>
		/// 获取当前读进字符的的位置。
		/// </summary>
		/// <value>当前读进字符的位置。</value>
		public SourcePosition Position
		{
			get
			{
				if (curCol == 0)
				{
					return SourcePosition.Unknown;
				}
				return new SourcePosition(curIndex, curLine, curCol);
			}
		}
		/// <summary>
		/// 获取或设置下一个字符的位置。
		/// </summary>
		/// <value>下一个未读进字符的位置。</value>
		/// <remarks>不会改变 <see cref="Position"/>，因为难以根据 <see cref="NextPosition"/> 确定 
		/// <see cref="Position"/> 的准确值。</remarks>
		public SourcePosition NextPosition
		{
			get { return new SourcePosition(curIndex + 1, nextLine, nextCol); }
			set
			{
				if (value.IsUnknown)
				{
					this.Reset();
				}
				else
				{
					curIndex = value.Index - 1;
					if (curIndex == -1)
					{
						// curIndex 为 -1，会导致当前位置无效。
						curCol = 0;
					}
					nextLine = value.Line;
					nextCol = value.Col;
				}
			}
		}
		/// <summary>
		/// 重置当前的定位信息。
		/// </summary>
		public void Reset()
		{
			curIndex = -1;
			curLine = nextLine = 1;
			curCol = 0;
			nextCol = 1;
		}
		/// <summary>
		/// 令位置前进一个字符。
		/// </summary>
		/// <param name="ch">要前进的字符。</param>
		/// <overloads>
		/// <summary>
		/// 令位置前进一个或多个字符。
		/// </summary>
		/// </overloads>
		public void Forward(char ch)
		{
			curIndex++;
			curLine = nextLine;
			curCol = nextCol;
			switch (ch)
			{
				case '\n':
					nextLine++;
					nextCol = 1;
					break;
				case '\t':
					nextCol = ForwardTab(nextCol);
					break;
				default:
					nextCol += GetWidth(ch);
					break;
			}
		}
		/// <summary>
		/// 令位置前进指定的字符数组。
		/// </summary>
		/// <param name="chars">要前进的字符数组。</param>
		/// <exception cref="ArgumentNullException"><paramref name="chars"/> 为 <c>null</c>。</exception>
		public void Forward(char[] chars)
		{
			if (chars == null)
			{
				throw CommonExceptions.ArgumentNull("chars");
			}
			Contract.EndContractBlock();
			if (chars.Length == 1)
			{
				Forward(chars[0]);
			}
			else if (chars.Length > 1)
			{
				ForwardInternal(chars, 0, chars.Length);
			}
		}
		/// <summary>
		/// 令位置前进指定字符数组的一部分。
		/// </summary>
		/// <param name="chars">要前进的字符数组。</param>
		/// <param name="index">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度。</param>
		/// <exception cref="ArgumentNullException"><paramref name="chars"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 或 
		/// <paramref name="count"/> 小于零。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 和 <paramref name="count"/> 
		/// 不表示 <paramref name="chars"/> 中的有效范围。</exception>
		public void Forward(char[] chars, int index, int count)
		{
			if (chars == null)
			{
				throw CommonExceptions.ArgumentNull("chars");
			}
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			if (count < 0)
			{
				throw CommonExceptions.ArgumentNegative("count", count);
			}
			if (index + count > chars.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("count", count);
			}
			Contract.EndContractBlock();
			if (count == 1)
			{
				Forward(chars[index]);
			}
			else if (count > 1)
			{
				ForwardInternal(chars, index, count);
			}
		}
		/// <summary>
		/// 令位置前进指定字符串。
		/// </summary>
		/// <param name="str">要前进的字符串。</param>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		public void Forward(string str)
		{
			if (str == null)
			{
				throw CommonExceptions.ArgumentNull("str");
			}
			Contract.EndContractBlock();
			if (str.Length == 1)
			{
				Forward(str[0]);
			}
			else if (str.Length > 1)
			{
				ForwardInternal(str, 0, str.Length);
			}
		}
		/// <summary>
		/// 令位置前进指定字符串的一部分。
		/// </summary>
		/// <param name="str">要前进的字符串。</param>
		/// <param name="index">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度。</param>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 或 
		/// <paramref name="count"/> 小于零。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 和 <paramref name="count"/> 
		/// 不表示 <paramref name="str"/> 中的有效范围。</exception>
		public void Forward(string str, int index, int count)
		{
			if (str == null)
			{
				throw CommonExceptions.ArgumentNull("str");
			}
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			if (count < 0)
			{
				throw CommonExceptions.ArgumentNegative("count", count);
			}
			if (index + count > str.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("count", count);
			}
			Contract.EndContractBlock();
			if (count == 1)
			{
				Forward(str[index]);
			}
			else if (count > 1)
			{
				ForwardInternal(str, index, count);
			}
		}
		/// <summary>
		/// 令位置前进指定字符数组的一部分。
		/// </summary>
		/// <param name="chars">要前进的字符数组。</param>
		/// <param name="idx">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度，必须大于 <c>1</c>。</param>
		private void ForwardInternal(char[] chars, int idx, int count)
		{
			Contract.Requires(chars != null && chars.Length > 1);
			Contract.Requires(idx >= 0 && count > 1 && idx + count <= chars.Length);
			unsafe
			{
				fixed (char* start = &chars[idx])
				{
					ForwardInternal(start, start + count - 1);
				}
			}
			curIndex += count;
		}
		/// <summary>
		/// 令位置前进指定字符串的一部分。
		/// </summary>
		/// <param name="str">要前进的字符串。</param>
		/// <param name="idx">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度，必须大于 <c>1</c>。</param>
		private void ForwardInternal(string str, int idx, int count)
		{
			Contract.Requires(str != null && str.Length > 1);
			Contract.Requires(idx >= 0 && count > 1 && idx + count <= str.Length);
			unsafe
			{
				fixed (char* start = str)
				{
					ForwardInternal(start + idx, start + idx + count - 1);
				}
			}
			curIndex += count;
		}
		/// <summary>
		/// 令位置前进指定字符指针所指的部分。
		/// </summary>
		/// <param name="start">要前进的起始字符指针（包含）。</param>
		/// <param name="end">要前进的结束字符指针（包含）。</param>
		/// <remarks>不会修改 <c>curIndex</c> 属性。</remarks>
		private unsafe void ForwardInternal(char* start, char* end)
		{
			Contract.Requires(end > start);
			Contract.Requires(widths.Count == 0);
			char oldHighSurrogate = this.surrogate;
			this.surrogate = NoSurrogate;
			// 最后一个字符需要特殊考虑，否则无法得到当前位置。
			int lastWidth = 0;
			char lastChar = *end--;
			if (!char.IsHighSurrogate(lastChar))
			{
				if (lastChar == '\n')
				{
					lastWidth = -1;
				}
				else if (lastChar == '\t')
				{
					lastWidth = -2;
				}
				else if (char.IsLowSurrogate(lastChar))
				{
					if (char.IsHighSurrogate(*end))
					{
						// 匹配的代理项对。
						lastWidth = CharExt.Width(char.ConvertToUtf32(*end, lastChar));
						end--;
					}
				}
				else
				{
					lastWidth = lastChar.Width();
				}
				lastChar = NoSurrogate;
			}
			this.curCol = this.nextCol;
			this.curLine = this.nextLine;
			int width = 0;
			for (; end >= start; end--)
			{
				if (*end == '\n')
				{
					widths.Add(width);
					oldHighSurrogate = NoSurrogate;
					// 统计之前的行数。
					this.curCol = 1;
					this.curLine++;
					for (; end >= start; end--)
					{
						if (*end == '\n')
						{
							this.curLine++;
						}
					}
					break;
				}
				if (*end == '\t')
				{
					widths.Add(width);
					width = 0;
				}
				else
				{
					width += GetWidthReversed(*end);
				}
			}
			if (oldHighSurrogate != NoSurrogate)
			{
				width += GetWidthReversed(oldHighSurrogate);
			}
			if (width > 0)
			{
				widths.Add(width);
			}
			// 统计列数。
			for (int j = widths.Count - 1; j > 0; j--)
			{
				this.curCol += widths[j];
				this.curCol = ForwardTab(this.curCol);
			}
			this.surrogate = lastChar;
			this.curCol += widths[0];
			widths.Clear();
			this.nextLine = this.curLine;
			if (lastWidth >= 0)
			{
				this.nextCol = this.curCol + lastWidth;
			}
			else if (lastWidth == -1)
			{
				this.nextCol = 1;
				this.nextLine++;
			}
			else
			{
				this.nextCol = this.ForwardTab(this.curCol);
			}
		}
		/// <summary>
		/// 令列前进一个 Tab 字符。
		/// </summary>
		/// <param name="col">当前所在列。</param>
		/// <returns>前进一个 Tab 字符后的列。</returns>
		private int ForwardTab(int col)
		{
			return tabSize * (1 + (col - 1) / tabSize) + 1;
		}
		/// <summary>
		/// 返回指定字符的宽度。
		/// </summary>
		/// <param name="ch">要获取宽度的字符。</param>
		/// <returns><paramref name="ch"/> 的宽度。</returns>
		private int GetWidth(char ch)
		{
			if (!char.IsSurrogate(ch))
			{
				return ch.Width();
			}
			if (char.IsHighSurrogate(ch))
			{
				this.surrogate = ch;
				return 0;
			}
			// ch 是低代理项
			if (this.surrogate == NoSurrogate)
			{
				// 缺失相应高代理项，忽略无效数据。
				return 0;
			}
			int chValue = char.ConvertToUtf32(this.surrogate, ch);
			this.surrogate = NoSurrogate;
			return CharExt.Width(chValue);
		}
		/// <summary>
		/// 返回指定字符的宽度，字符串遍历的顺序是从后至前的。
		/// </summary>
		/// <param name="ch">要获取宽度的字符。</param>
		/// <returns><paramref name="ch"/> 的宽度。</returns>
		private int GetWidthReversed(char ch)
		{
			if (!char.IsSurrogate(ch))
			{
				return ch.Width();
			}
			if (char.IsLowSurrogate(ch))
			{
				this.surrogate = ch;
				return 0;
			}
			// ch 是高代理项
			if (this.surrogate == NoSurrogate)
			{
				// 缺失相应低代理项，忽略无效数据。
				return 0;
			}
			int chValue = char.ConvertToUtf32(ch, this.surrogate);
			this.surrogate = NoSurrogate;
			return CharExt.Width(chValue);
		}
	}
}
