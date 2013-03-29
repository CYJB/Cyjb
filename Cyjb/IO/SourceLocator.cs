using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cyjb.IO
{
	/// <summary>
	/// 提供可以用于在源文件中定位的方法。
	/// 支持 \n（Unix） 和 \r\n（Windows） 作为换行符。
	/// </summary>
	[DebuggerDisplay("Location")]
	public sealed class SourceLocator
	{
		/// <summary>
		/// 默认的 Tab 长度。
		/// </summary>
		public const int DefaultTabSize = 4;
		/// <summary>
		/// Tab 的宽度。
		/// </summary>
		private readonly int tabSize;
		/// <summary>
		/// 当前字符所在的行。
		/// </summary>
		private int curLine = 1;
		/// <summary>
		/// 下一个字符所在的行。
		/// </summary>
		private int nextLine = 1;
		/// <summary>
		/// 当前字符所在的列。
		/// </summary>
		private int curCol = 0;
		/// <summary>
		/// 下一个字符所在的列。
		/// </summary>
		private int nextCol = 1;
		/// <summary>
		/// 当前字符从零开始的索引。
		/// </summary>
		private int curIndex = -1;
		/// <summary>
		/// 单一的字符数组。
		/// </summary>
		private char[] charArr = new char[1];
		/// <summary>
		/// 储存特殊位置的列表。
		/// </summary>
		private List<int> idxList;
		/// <summary>
		/// 计算字节数使用的编码器。
		/// </summary>
		private Encoder encoder = Encoding.Default.GetEncoder();
		/// <summary>
		/// 使用默认的 Tab 宽度初始化 <see cref="SourceLocator"/> 类的新实例。
		/// </summary>
		public SourceLocator() : this(DefaultTabSize) { }
		/// <summary>
		/// 使用默认的 Tab 宽度初始化 <see cref="SourceLocator"/> 类的新实例。
		/// </summary>
		/// <param name="tabSize">Tab 的宽度。</param>
		public SourceLocator(int tabSize)
		{
			this.tabSize = tabSize;
			Reset();
		}
		/// <summary>
		/// 获取 Tab 的宽度。
		/// </summary>
		public int TabSize
		{
			get { return tabSize; }
		}
		/// <summary>
		/// 获取当前读进字符的的位置。
		/// </summary>
		public SourceLocation Location
		{
			get { return new SourceLocation(curIndex, curLine, curCol); }
		}
		/// <summary>
		/// 获取下一个字符的位置。
		/// </summary>
		public SourceLocation NextLocation
		{
			get { return new SourceLocation(curIndex + 1, nextLine, nextCol); }
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
		/// 在源文件中前进一个字符。
		/// </summary>
		/// <param name="ch">源文件中前进的字符。</param>
		public void Forward(char ch)
		{
			curIndex++;
			curLine = nextLine;
			curCol = nextCol;
			if (ch == '\n')
			{
				// 换到新行。
				nextLine++;
				nextCol = 1;
			}
			else if (ch == '\t')
			{
				ForwardTab();
			}
			else
			{
				charArr[0] = ch;
				nextCol += encoder.GetByteCount(charArr, 0, 1, false);
			}
		}
		/// <summary>
		/// 在源文件中前进一个字符数组。
		/// </summary>
		/// <param name="arr">源文件中前进的字符数组。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="arr"/> 为 <c>null</c>。</exception>
		public void Forward(char[] arr)
		{
			ExceptionHelper.CheckArgumentNull(arr, "arr");
			if (arr.Length == 1)
			{
				Forward(arr[0]);
			}
			else if (arr.Length > 1)
			{
				ForwardInternal(arr, 0, arr.Length);
			}
		}
		/// <summary>
		/// 在源文件中前进一个字符数组的一部分。
		/// </summary>
		/// <param name="arr">源文件中前进的字符数组。</param>
		/// <param name="index">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="arr"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 小于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="count"/> 小于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 和 <paramref name="count"/> 不表示
		/// <paramref name="arr"/> 中的有效范围。</exception>
		public void Forward(char[] arr, int index, int count)
		{
			ExceptionHelper.CheckArgumentNull(arr, "arr");
			if (index < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			if (count < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("count");
			}
			if (index + count > arr.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("count");
			}
			if (count == 1)
			{
				Forward(arr[index]);
			}
			else if (count > 1)
			{
				ForwardInternal(arr, index, count);
			}
		}
		/// <summary>
		/// 在源文件中前进一个字符串。
		/// </summary>
		/// <param name="str">源文件中前进的字符串。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="str"/> 为 <c>null</c>。</exception>
		public void Forward(string str)
		{
			ExceptionHelper.CheckArgumentNull(str, "str");
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
		/// 在源文件中前进一个字符串的一部分。
		/// </summary>
		/// <param name="str">源文件中前进的字符串。</param>
		/// <param name="index">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 小于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="count"/> 小于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 和 <paramref name="count"/> 不表示
		/// <paramref name="str"/> 中的有效范围。</exception>
		public void Forward(string str, int index, int count)
		{
			ExceptionHelper.CheckArgumentNull(str, "chars");
			if (index < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			if (count < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("count");
			}
			if (index + count > str.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("count");
			}
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
		/// 在源文件中前进一个字符数组的一部分。
		/// </summary>
		/// <param name="chars">源文件中前进的字符数组。</param>
		/// <param name="idx">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度，必须大于 <c>1</c>。</param>
		private void ForwardInternal(char[] chars, int idx, int count)
		{
			Debug.Assert(count > 1);
			if (idxList == null)
			{
				idxList = new List<int>();
			}
			else
			{
				idxList.Clear();
			}
			// 余下最后一个字符单独考虑。
			count--;
			int end = idx + count;
			int i = end - 1;
			for (; i >= idx; i--)
			{
				if (chars[i] == '\t')
				{
					if (end > i)
					{
						idxList.Add(encoder.GetByteCount(chars, i + 1, end - i - 1, false));
						end = i;
					}
					else
					{
						idxList.Add(0);
					}
				}
				else if (chars[i] == '\n')
				{
					// 统计之前的行数。
					nextCol = 1;
					nextLine++;
					if (end > i)
					{
						idxList.Add(encoder.GetByteCount(chars, i + 1, end - i - 1, false));
						end = i;
					}
					for (int j = i - 1; j >= idx; j--)
					{
						if (chars[j] == '\n') { nextLine++; }
					}
					break;
				}
			}
			if (end > i)
			{
				idxList.Add(encoder.GetByteCount(chars, i + 1, end - i - 1, false));
			}
			// 统计列数。
			for (int j = idxList.Count - 1; j > 0; j--)
			{
				nextCol += idxList[j];
				ForwardTab();
			}
			nextCol += idxList[0];
			curIndex += count;
			Forward(chars[idx + count]);
		}
		/// <summary>
		/// 在源文件中前进一个字符串的一部分。
		/// </summary>
		/// <param name="str">源文件中前进的字符串。</param>
		/// <param name="idx">要前进的字符的索引。</param>
		/// <param name="count">要前进的字符的长度，必须大于 <c>1</c>。</param>
		private void ForwardInternal(string str, int idx, int count)
		{
			Debug.Assert(count > 1);
			if (idxList == null)
			{
				idxList = new List<int>();
			}
			else
			{
				idxList.Clear();
			}
			// 余下最后一个字符单独考虑。
			count--;
			unsafe
			{
				fixed (char* start = str)
				{
					char* end = start + count;
					char* i = end - 1;
					for (; i >= start; i--)
					{
						if (*i == '\t')
						{
							if (end > i)
							{
								idxList.Add(encoder.GetByteCount(i + 1, (int)(end - i) - 1, false));
								end = i;
							}
							else
							{
								idxList.Add(0);
							}
						}
						else if (*i == '\n')
						{
							// 统计之前的行数。
							nextCol = 1;
							nextLine++;
							if (end > i)
							{
								idxList.Add(encoder.GetByteCount(i + 1, (int)(end - i) - 1, false));
								end = i;
							}
							for (char* j = i - 1; j >= start; j--)
							{
								if (*j == '\n') { nextLine++; }
							}
							break;
						}
					}
					if (end > i)
					{
						idxList.Add(encoder.GetByteCount(i + 1, (int)(end - i) - 1, false));
					}
				}
			}
			// 统计列数。
			for (int j = idxList.Count - 1; j > 0; j--)
			{
				nextCol += idxList[j];
				ForwardTab();
			}
			nextCol += idxList[0];
			curIndex += count;
			Forward(str[idx + count]);
		}
		/// <summary>
		/// 向前前进一个 Tab 字符。
		/// </summary>
		private void ForwardTab()
		{
			nextCol = tabSize * (1 + (nextCol - 1) / tabSize) + 1;
		}
	}
}
