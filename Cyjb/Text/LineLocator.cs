using System.Diagnostics;
using Cyjb.Collections;

namespace Cyjb.Text;

/// <summary>
/// 提供可以用于定位行位置的功能，支持 \n（Unix） 和 \r\n（Windows） 作为换行符。
/// </summary>
/// <remarks>
/// 对行位置定位方法的详细解释，请参见我的 C# 词法分析器系列博文
/// <see href="http://www.cnblogs.com/cyjb/archive/p/LexerInputBuffer.html#SourceLocation">
/// 《C# 词法分析器（二）输入缓冲和代码定位》</see>。</remarks>
/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/LexerInputBuffer.html#SourceLocation">
/// 《C# 词法分析器（二）输入缓冲和代码定位》</seealso>
public sealed class LineLocator
{
	/// <summary>
	/// 表示 Tab 的宽度标记。
	/// </summary>
	public const int TabSizeMark = 3;

	/// <summary>
	/// Tab 的宽度。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly int tabSize;
	/// <summary>
	/// 行起始的索引。
	/// </summary>
	private readonly List<int> lineStarts = new();
	/// <summary>
	/// 每行的宽度信息，[0..^1] 总是 ColumnInfo[]，[^1] 总是指向 lastLine。
	/// </summary>
	private readonly List<LineInfo> lineInfoList = new();
	/// <summary>
	/// 最后一行的宽度信息。
	/// </summary>
	private LineInfo lastLineInfo = new();
	/// <summary>
	/// 最后一列的信息。
	/// </summary>
	private ColumnInfo lastColumnInfo = ColumnInfo.Default;
	/// <summary>
	/// 当前字符索引。
	/// </summary>
	private int index = 0;
	/// <summary>
	/// 最后一个字符是否是 <c>'\r'</c>。
	/// </summary>
	private bool lastWasCR = false;
	/// <summary>
	/// 最近一次读取的行号。
	/// </summary>
	private int lastLineNumber = 0;

	/// <summary>
	/// 使用默认的 Tab 宽度(4)初始化 <see cref="LineLocator"/> 类的新实例。
	/// </summary>
	/// <overloads>
	/// <summary>
	/// 初始化 <see cref="LineLocator"/> 类的新实例。
	/// </summary>
	/// </overloads>
	public LineLocator() : this(4) { }

	/// <summary>
	/// 使用指定的 Tab 宽度初始化 <see cref="LineLocator"/> 类的新实例。
	/// </summary>
	/// <param name="tabSize">Tab 的宽度。</param>
	public LineLocator(int tabSize)
	{
		this.tabSize = tabSize;
		// 首行索引总是 0
		lineStarts.Add(0);
		lineInfoList.Add(lastLineInfo);
	}

	/// <summary>
	/// 获取 Tab 的宽度。
	/// </summary>
	/// <value>Tab 字符的宽度。</value>
	public int TabSize => tabSize;

	/// <summary>
	/// 返回指定字符索引的行位置。
	/// </summary>
	/// <param name="index">要检查的字符索引。</param>
	/// <returns>指定索引对应的行位置。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
	public LinePosition GetPosition(int index)
	{
		if (index < 0)
		{
			throw CommonExceptions.ArgumentNegative(index);
		}
		int line = GetLine(index);
		int character = index - lineStarts[line];
		ColumnInfo columnInfo = lineInfoList[line].FindColumnInfo(character);
		// 先检查最后一个列号。
		return new LinePosition(line + 1, character, columnInfo.GetColumn(character, tabSize));
	}

	/// <summary>
	/// 返回指定字符索引的行索引。
	/// </summary>
	/// <param name="index">要检查的字符索引。</param>
	/// <returns>指定索引对应的行索引。</returns>
	private int GetLine(int index)
	{
		// 一般都会顺序读取位置，因此优先检查最近一次读取的行号。
		if (index >= lineStarts[lastLineNumber])
		{
			// 至多顺序检查最近 4 行。
			int limit = Math.Min(lineStarts.Count, lastLineNumber + 4);
			for (int i = lastLineNumber; i < limit; i++)
			{
				if (index < lineStarts[i])
				{
					lastLineNumber = i - 1;
					return lastLineNumber;
				}
			}
		}
		// 二分查找合适的行索引。
		int line = lineStarts.BinarySearch(index);
		if (line < 0)
		{
			line = (~line) - 1;
		}
		lastLineNumber = line;
		return line;
	}

	/// <summary>
	/// 读取指定的字符范围。
	/// </summary>
	/// <param name="chars">要读取的字符范围。</param>
	public void Read(ReadOnlySpan<char> chars)
	{
		if (chars.IsEmpty)
		{
			return;
		}
		int idx = 0;
		// 检查 \r\n
		if (lastWasCR)
		{
			if (chars[0] == '\n')
			{
				idx++;
			}
			AddLine(index + idx);
			lastWasCR = false;
		}
		int lineStart = lineStarts[^1];
		int len = chars.Length;
		for (; idx < len; idx++)
		{
			char ch = chars[idx];
			if (ch.IsAnyLineSeparator())
			{
				if (lastColumnInfo.Width != 0)
				{
					int character = index + idx - lineStart;
					int column = lastColumnInfo.GetColumn(character, tabSize);
					lastColumnInfo = new ColumnInfo(character, column, 0);
					lastLineInfo.Add(lastColumnInfo);
				}
				if (ch == '\r')
				{
					int nextIdx = idx + 1;
					if (nextIdx >= len)
					{
						lastWasCR = true;
						break;
					}
					else if (chars[nextIdx] == '\n')
					{
						idx = nextIdx;
					}
				}
				lineStart = index + idx + 1;
				AddLine(lineStart);
			}
			else
			{
				int width = ch == '\t' ? TabSizeMark : ch.Width();
				if (lastColumnInfo.Width == width)
				{
					continue;
				}
				int character = index + idx - lineStart;
				int column = lastColumnInfo.GetColumn(character, tabSize);
				lastColumnInfo = new ColumnInfo(character, column, width);
				lastLineInfo.Add(lastColumnInfo);
			}
		}
		index += len;
		lineInfoList[^1] = lastLineInfo;
	}

	/// <summary>
	/// 添加新的行。
	/// </summary>
	/// <param name="index">行的起始索引。</param>
	private void AddLine(int index)
	{
		lineStarts.Add(index);
		int end = lineInfoList.Count - 1;
		lineInfoList[end] = lastLineInfo;
		lastLineInfo.Clear();
		lineInfoList.Add(lastLineInfo);
		lastColumnInfo = ColumnInfo.Default;
	}

	/// <summary>
	/// 行的信息。
	/// </summary>
	private struct LineInfo
	{
		/// <summary>
		/// 最后一列的信息。
		/// </summary>
		private ColumnInfo lastColumn;
		/// <summary>
		/// 其它列的信息。
		/// </summary>
		private List<ColumnInfo>? columns;

		/// <summary>
		/// 初始化 <see cref="LineInfo"/> 结构的新实例。
		/// </summary>
		public LineInfo()
		{
			lastColumn = ColumnInfo.Default;
			columns = null;
		}

		/// <summary>
		/// 寻找指定字符位置对应的列信息。
		/// </summary>
		/// <param name="character">字符位置。</param>
		/// <returns>列信息。</returns>
		public readonly ColumnInfo FindColumnInfo(int character)
		{
			// 先检查最后一个列号。
			if (character >= lastColumn.Character)
			{
				return lastColumn;
			}
			else if (columns == null)
			{
				return ColumnInfo.Default;
			}
			else if (columns.Count == 1)
			{
				ColumnInfo columnInfo = columns[0];
				if (character >= columnInfo.Character)
				{
					return columnInfo;
				}
				else
				{
					return ColumnInfo.Default;
				}
			}
			int colIndex = columns.BinarySearch(character, (columnInfo) => columnInfo.Character);
			if (colIndex < 0)
			{
				colIndex = ~colIndex;
				// 需要插入到 0 表示 ColumnInfo.Default。
				if (colIndex > 0)
				{
					return columns[colIndex - 1];
				}
				else
				{
					return ColumnInfo.Default;
				}
			}
			else
			{
				return columns[colIndex];
			}
		}

		/// <summary>
		/// 添加指定的列信息。
		/// </summary>
		/// <param name="columnInfo">要添加的列信息。</param>
		public void Add(ColumnInfo columnInfo)
		{
			if (lastColumn == ColumnInfo.Default)
			{
				lastColumn = columnInfo;
			}
			else
			{
				columns ??= new List<ColumnInfo>();
				columns.Add(lastColumn);
				lastColumn = columnInfo;
			}
		}

		/// <summary>
		/// 重置当前列信息。
		/// </summary>
		public void Clear()
		{
			lastColumn = ColumnInfo.Default;
			columns = null;
		}
	}

	/// <summary>
	/// 列信息。
	/// </summary>
	/// <param name="Character">字符索引。</param>
	/// <param name="Column">列号。</param>
	/// <param name="Width">宽度，<c>0</c> ~ <c>2</c> 表示字符宽度，<c>3</c> 表示 Tab 字符。</param>
	private record struct ColumnInfo(int Character, int Column, int Width)
	{
		/// <summary>
		/// 默认的列信息。
		/// </summary>
		public static readonly ColumnInfo Default = new(0, 1, 1);
		/// <summary>
		/// 返回指定字符索引的列号。
		/// </summary>
		/// <param name="character">要检查的字符索引。</param>
		/// <param name="tabSize">Tab 的宽度。</param>
		/// <returns>指定字符索引的列号。</returns>
		public readonly int GetColumn(int character, int tabSize)
		{
			int cnt = character - Character;
			if (cnt == 0)
			{
				return Column;
			}
			if (Width == TabSizeMark)
			{
				int result = Column + cnt * tabSize;
				// Tab 需要向下取整到 tabSize 的整数倍。
				result = tabSize * ((result - 1) / tabSize) + 1;
				return result;
			}
			else
			{
				return Column + cnt * Width;
			}
		}
	}
}
