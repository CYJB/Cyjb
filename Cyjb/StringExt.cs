using System.Globalization;
using System.Text;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.String"/> 类的扩展方法。
	/// </summary>
	public static class StringExt
	{

		#region Unicode 操作

		/// <summary>
		/// 将字符串中的 \u,\U 和 \x 转义转换为对应的字符。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <returns>转换后的字符串。</returns>
		public static string DecodeUnicode(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			int idx = str.IndexOf('\\');
			if (idx < 0)
			{
				return str;
			}
			int len = str.Length, start = 0;
			StringBuilder builder = new StringBuilder(len);
			while (idx >= 0)
			{
				// 添加当前 '\' 之前的字符串。
				if (idx > start)
				{
					builder.Append(str, start, idx - start);
					start = idx;
				}
				// 跳过 '\' 字符。
				idx++;
				// '\' 字符后的字符数小于 2，不可能是转义字符，直接返回。
				if (idx + 1 >= len)
				{
					break;
				}
				// 十六进制字符的长度。
				int hexLen = 0;
				// 处理 Unicode 转义。
				switch (str[idx])
				{
					case 'x':
						// \x 后面可以是 1 至 4 位。
						hexLen = GetHexLength(str, idx + 1, 4);
						break;
					case 'u':
						// \u 后面必须是 4 位。
						if (idx + 4 < len && GetHexLength(str, idx + 1, 4) == 4)
						{
							hexLen = 4;
						}
						else
						{
							hexLen = 0;
						}
						break;
					case 'U':
						// \U 后面必须是 8 位。
						if (idx + 8 < len && GetHexLength(str, idx + 1, 8) == 8)
						{
							hexLen = 8;
						}
						else
						{
							hexLen = 0;
						}
						break;
				}
				if (hexLen > 0)
				{
					idx++;
					int charNum = int.Parse(str.Substring(idx, hexLen), NumberStyles.HexNumber,
						CultureInfo.InvariantCulture);
					if (charNum < 0xFFFF)
					{
						// 单个字符。
						builder.Append((char)charNum);
					}
					else
					{
						// 代理项对的字符。
						builder.Append(char.ConvertFromUtf32(charNum & 0x1FFFFF));
					}
					idx = start = idx + hexLen;
				}
				idx = str.IndexOf('\\', idx);
			}
			// 添加剩余的字符串。
			if (start < len)
			{
				builder.Append(str.Substring(start));
			}
			return builder.ToString();
		}
		/// <summary>
		/// 返回字符串指定索引位置之后的十六进制字符的个数。
		/// </summary>
		/// <param name="str">要获取十六进制字符个数的字符串。</param>
		/// <param name="index">要开始计算十六进制字符个数的其实索引。</param>
		/// <param name="maxLength">需要的最长的十六进制字符个数。</param>
		/// <returns>实际的十六进制字符的个数。</returns>
		internal static int GetHexLength(string str, int index, int maxLength)
		{
			if (index + maxLength > str.Length)
			{
				maxLength = str.Length - index;
			}
			for (int i = 0; i < maxLength; i++, index++)
			{
				if (!CharExt.IsHex(str, index))
				{
					return i;
				}
			}
			return maxLength;
		}
		/// <summary>
		/// 将字符串中不可显示字符（0x00~0x1F,0x7F之后）转义为 \\u 形式，其中十六进制以大写字母形式输出。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <returns>转换后的字符串。</returns>
		public static string EncodeUnicode(this string str)
		{
			return EncodeUnicode(str, true);
		}
		/// <summary>
		/// 将字符串中不可显示字符（0x00~0x1F,0x7F之后）转义为 \\u 形式。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <param name="upperCase">是否以大写字母形式输出十六进制。如果为 <c>true</c> 则是以大写字母形式输出十六进制，
		/// 否则以小写字母形式输出。</param>
		/// <returns>转换后的字符串。</returns>
		public static string EncodeUnicode(this string str, bool upperCase)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			string format = upperCase ? "X4" : "x4";
			StringBuilder builder = new StringBuilder(str.Length * 2);
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c >= ' ' && c <= '~')
				{
					// 可显示字符。
					builder.Append(c);
				}
				else
				{
					builder.Append("\\u");
					builder.Append(((int)c).ToString(format, CultureInfo.InvariantCulture));
				}
			}
			return builder.ToString();
		}

		#endregion

		#region 截取

		/// <summary>
		/// 返回一个字符串，其中包含从指定字符串左端开始的指定数量的字符。
		/// </summary>
		/// <param name="str">从该字符串返回最左端的字符。</param>
		/// <param name="length">指示要返回的字符数的数值表达式。
		/// 如果为 <c>0</c>，则返回零长度字符串 ("")。
		/// 如果大于或等于 <paramref name="str"/> 的长度，则返回整个字符串。</param>
		/// <returns>从指定字符串左端开始的指定数量的字符。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		public static string Left(this string str, int length)
		{
			if (length < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("length");
			}
			if (length == 0 || string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (length >= str.Length)
			{
				return str;
			}
			return str.Substring(0, length);
		}
		/// <summary>
		/// 返回一个字符串，其中包含从指定字符串右端开始的指定数量的字符。
		/// </summary>
		/// <param name="str">从该字符串返回最右端的字符。</param>
		/// <param name="length">指示要返回的字符数的数值表达式。
		/// 如果为 <c>0</c>，则返回零长度字符串 ("")。
		/// 如果大于或等于 <paramref name="str"/> 的长度，则返回整个字符串。</param>
		/// <returns>从指定字符串右端开始的指定数量的字符。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		public static string Right(this string str, int length)
		{
			if (length < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("length");
			}
			if (length == 0 || string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (length >= str.Length)
			{
				return str;
			}
			return str.Substring(str.Length - length);
		}
		/// <summary>
		/// 从此实例检索子字符串。如果 <paramref name="startIndex"/> 小于 <c>0</c>，
		/// 那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <param name="str">要检索子字符串的字符串实例。</param>
		/// <param name="startIndex">此示例中子字符串的起始字符位置（从零开始）。</param>
		/// <returns>与此实例中在 <paramref name="startIndex"/> 处开头的子字符串等效的一个字符串，
		/// 如果 <paramref name="startIndex"/> 等于此实例的长度，
		/// 则为 <see cref="System.String.Empty"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 大于此实例的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 小于负的此实例的长度。</exception>
		public static string SubstringEx(this string str, int startIndex)
		{
			ExceptionHelper.CheckArgumentNull(str, "str");
			if (startIndex < 0)
			{
				startIndex += str.Length;
			}
			return str.Substring(startIndex);
		}
		/// <summary>
		/// 从此实例检索子字符串。子字符串从指定的字符位置开始且具有指定的长度。
		/// 如果 <paramref name="startIndex"/> 小于 <c>0</c>，
		/// 那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <param name="str">要检索子字符串的字符串实例。</param>
		/// <param name="startIndex">此示例中子字符串的起始字符位置（从零开始）。</param>
		/// <param name="length">子字符串中的字符数。</param>
		/// <returns>与此实例中在 <paramref name="startIndex"/> 处开头、
		/// 长度为 <paramref name="length"/> 的子字符串等效的一个字符串，
		/// 如果 <paramref name="startIndex"/> 等于此实例的长度或 
		/// <paramref name="length"/> 为零，则为空字符串（""）。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 加 <paramref name="length"/>
		/// 之和指示的位置不在此实例中。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 小于负的此实例的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于零。</exception>
		public static string SubstringEx(this string str, int startIndex, int length)
		{
			ExceptionHelper.CheckArgumentNull(str, "str");
			if (startIndex < 0)
			{
				startIndex += str.Length;
			}
			return str.Substring(startIndex, length);
		}
		/// <summary>
		/// 从此实例检索子字符串。子字符串从指定的字符位置开始到字符串的结尾。
		/// 如果 <paramref name="startIndex"/> 小于 <c>0</c>，
		/// 那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <param name="str">要检索子字符串的字符串实例。</param>
		/// <param name="startIndex">此示例中子字符串的起始字符位置（从零开始）。</param>
		/// <returns>与此实例中在 <paramref name="startIndex"/> 
		/// 处开头到字符串结尾的子字符串等效的一个字符串，
		/// 如果 <paramref name="startIndex"/> 等于此实例的长度或大于等于字符串的长度，
		/// 则为空字符串（""）。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 指示的位置不在此实例中。</exception>
		public static string Slice(this string str, int startIndex)
		{
			ExceptionHelper.CheckArgumentNull(str, "str");
			if (startIndex < 0)
			{
				startIndex += str.Length;
			}
			if (startIndex < str.Length)
			{
				return str.Substring(startIndex);
			}
			else
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// 从此实例检索子字符串。子字符串从指定的字符位置开始，从指定的字符位置结束。
		/// 如果 <paramref name="startIndex"/> 或 <paramref name="endIndex"/>
		/// 小于 <c>0</c>，那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <param name="str">要检索子字符串的字符串实例。</param>
		/// <param name="startIndex">此示例中子字符串的起始字符位置（从零开始）。</param>
		/// <param name="endIndex">此示例中子字符串的结束字符位置（从零开始），
		/// 但不包括该位置的字符。</param>
		/// <returns>与此实例中在 <paramref name="startIndex"/> 处开头、
		/// 在 <paramref name="endIndex"/> 处结束的子字符串等效的一个字符串，
		/// 如果 <paramref name="startIndex"/> 等于此实例的长度或大于等于 
		/// <paramref name="endIndex"/> ，则为 <see cref="System.String.Empty"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 或 <paramref name="endIndex"/>
		/// 指示的位置不在此实例中。</exception>
		public static string Slice(this string str, int startIndex, int endIndex)
		{
			ExceptionHelper.CheckArgumentNull(str, "str");
			if (startIndex < 0)
			{
				startIndex += str.Length;
			}
			if (endIndex < 0)
			{
				endIndex += str.Length;
			}
			if (startIndex < endIndex)
			{
				return str.Substring(startIndex, endIndex - startIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		#endregion // 截取

		/// <summary>
		/// 返回指定字符串的字符顺序是相反的字符串。
		/// </summary>
		/// <param name="str">字符反转的字符串。</param>
		/// <returns>字符反转后的字符串。</returns>
		/// <remarks>参考了 Microsoft.VisualBasic.Strings.StrReverse 方法的实现。</remarks>
		public static string Reverse(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			int len = str.Length;
			int end = len - 1;
			int i = 0;
			char[] strArr = new char[len];
			while (end >= 0)
			{
				switch (char.GetUnicodeCategory(str[i]))
				{
					case UnicodeCategory.Surrogate:
					case UnicodeCategory.NonSpacingMark:
					case UnicodeCategory.SpacingCombiningMark:
					case UnicodeCategory.EnclosingMark:
						// 字符串中包含组合字符，翻转时需要保证组合字符的顺序。
						// 为了能够包含基字符，回退一位。
						if (i > 0)
						{
							i--;
							end++;
						}
						TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(str, i);
						textElementEnumerator.MoveNext();
						int idx = textElementEnumerator.ElementIndex;
						while (end >= 0)
						{
							i = idx;
							if (textElementEnumerator.MoveNext())
							{
								idx = textElementEnumerator.ElementIndex;
							}
							else
							{
								idx = len;
							}
							for (int j = idx - 1; j >= i; strArr[end--] = str[j--]) ;
						}
						goto EndReverse;
				}
				// 直接复制。
				strArr[end--] = str[i++];
			}
		EndReverse:
			return new string(strArr);
		}
		/// <summary>
		/// 合并字符串中的空白。
		/// </summary>
		/// <param name="text">要合并空白的字符串。</param>
		/// <returns>合并完成的字符串。</returns>
		public static string CombineWhiteSpace(this string text)
		{
			return CombineWhiteSpace(text, " ");
		}
		/// <summary>
		/// 合并字符串中的空白。
		/// </summary>
		/// <param name="text">要合并空白的字符串。</param>
		/// <param name="replace">要替换空白的字符串。</param>
		/// <returns>合并完成的字符串。</returns>
		public static string CombineWhiteSpace(this string text, string replace)
		{
			StringBuilder builder = new StringBuilder(text.Length);
			// 0：起始位置。
			// 1：正常添加字符。
			// 2：需要添加空白。
			int state = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (char.IsWhiteSpace(text, i))
				{
					if (state == 1)
					{
						state = 2;
					}
				}
				else
				{
					if (state == 2)
					{
						builder.Append(replace);
					}
					builder.Append(text[i]);
					state = 1;
				}
			}
			return builder.ToString();
		}
	}
}
