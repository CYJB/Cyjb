using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="string"/> 类的扩展方法。
	/// </summary>
	public static class StringUtil
	{

		#region Escape

		/// <summary>
		/// 返回当前字符串的 Unicode 转义字符串。
		/// </summary>
		/// <param name="str">要进行转义的字符串。</param>
		/// <returns>转义后的字符串。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v），会替换为其转义形式。</para>
		/// <para>对于其它字符，会替换为其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		[return: NotNullIfNotNull("str")]
		public static string? UnicodeEscape(this string? str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			StringBuilder builder = new(str.Length * 2);
			for (int i = 0; i < str.Length; i++)
			{
				builder.Append(str[i].UnicodeEscape());
			}
			return builder.ToString();
		}

		/// <summary>
		/// 将字符串中的 \u,\U 和 \x 转义转换为对应的字符。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <returns>转换后的字符串。</returns>
		/// <remarks>
		/// <para>解码方法支持 \x00，\u0000 和 \U00000000 转义。使用 \U 转义时，大于 0x10FFFF 的值不被支持，会被舍弃。</para>
		/// <para>如果不满足上面的情况，则不会进行转义，也不会报错。</para></remarks>
		[return: NotNullIfNotNull("str")]
		public static string? UnicodeUnescape(this string? str)
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
			StringBuilder builder = new(len);
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
				if (idx >= len)
				{
					break;
				}
				// Unicode 转义需要的十六进制字符的长度。
				int hexLen = 0;
				bool parsed = true;
				char ch = str[idx];
				switch (ch)
				{
					case 'x':
						// \x 后面必须是 2 位。
						hexLen = 2;
						break;
					case 'u':
						// \u 后面必须是 4 位。
						hexLen = 4;
						break;
					case 'U':
						// \U 后面必须是 8 位。
						hexLen = 8;
						break;
					case '0':
						builder.Append('\0');
						break;
					case '\\':
						builder.Append('\\');
						break;
					case 'a':
						builder.Append('\a');
						break;
					case 'b':
						builder.Append('\b');
						break;
					case 'f':
						builder.Append('\f');
						break;
					case 'n':
						builder.Append('\n');
						break;
					case 'r':
						builder.Append('\r');
						break;
					case 't':
						builder.Append('\t');
						break;
					case 'v':
						builder.Append('\v');
						break;
					default:
						// 其它未支持的转义
						parsed = false;
						break;
				}
				idx++;
				if (hexLen > 0)
				{
					// Unicode 反转义。
					if (CheckHexLength(str, idx, hexLen))
					{
						int charNum = System.Convert.ToInt32(str.Substring(idx, hexLen), 16);
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
						idx += hexLen;
					}
					else
					{
						parsed = false;
					}
				}
				if (!parsed)
				{
					// 添加其他未被解析的字符。
					builder.Append('\\');
					builder.Append(ch);
				}
				start = idx;
				idx = str.IndexOf('\\', idx);
			}
			// 添加剩余的字符串。
			if (start < len)
			{
				builder.Append(str, start, len - start);
			}
			return builder.ToString();
		}

		/// <summary>
		/// 检查字符串指定索引位置之后是否包含指定个数的十六进制字符。
		/// </summary>
		/// <param name="str">要检查十六进制字符个数的字符串。</param>
		/// <param name="index">要开始计算十六进制字符个数的起始索引。</param>
		/// <param name="maxLength">需要的十六进制字符个数。</param>
		/// <returns>如果包含指定个数的十六进制字符，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool CheckHexLength(string str, int index, int maxLength)
		{
			if (index + maxLength > str.Length)
			{
				return false;
			}
			for (int i = 0; i < maxLength; i++, index++)
			{
				if (!Uri.IsHexDigit(str[index]))
				{
					return false;
				}
			}
			return true;
		}

		#endregion // Escape

		/// <summary>
		/// 返回指定字符串的字符顺序是相反的字符串。
		/// </summary>
		/// <param name="str">字符反转的字符串。</param>
		/// <returns>字符反转后的字符串。</returns>
		/// <remarks>参考了 Microsoft.VisualBasic.Strings.StrReverse 方法的实现。</remarks>
		[return: NotNullIfNotNull("str")]
		public static string? Reverse(this string? str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			int len = str.Length;
			int end = len - 1;
			int i = 0;
			char[] strArr = new char[len];
			while (end >= 0)
			{
				switch (CharUnicodeInfo.GetUnicodeCategory(str[i]))
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
						int start = i;
						TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(str, start);
						textElementEnumerator.MoveNext();
						int idx = start + textElementEnumerator.ElementIndex;
						while (end >= 0)
						{
							i = idx;
							idx = textElementEnumerator.MoveNext() ? start + textElementEnumerator.ElementIndex : len;
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

		#region 合并

		/// <summary>
		/// 合并字符串中的空白。
		/// </summary>
		/// <param name="text">要合并空白的字符串。</param>
		/// <returns>合并完成的字符串。</returns>
		/// <overloads>
		/// <summary>
		/// 合并字符串中的空白。
		/// </summary>
		/// </overloads>
		public static string CombineWhiteSpace(this string text)
		{
			return Combine(text, char.IsWhiteSpace, " ");
		}
		/// <summary>
		/// 合并字符串中的空白，并使用指定字符串替换空白字符串。
		/// </summary>
		/// <param name="text">要合并空白的字符串。</param>
		/// <param name="replace">要替换空白的字符串。</param>
		/// <returns>合并完成的字符串。</returns>
		public static string CombineWhiteSpace(this string text, string replace)
		{
			return Combine(text, char.IsWhiteSpace, replace);
		}
		/// <summary>
		/// 使用指定字符串，替换原字符串中连续的满足指定条件的字符。
		/// </summary>
		/// <param name="text">要合并连续字符的字符串。</param>
		/// <param name="predicate">要合并的字符满足的条件。</param>
		/// <param name="replace">要替换连续字符的字符串。</param>
		/// <returns>合并完成的字符串。</returns>
		public static string Combine(this string text, Predicate<char> predicate, string replace)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			StringBuilder builder = new StringBuilder(text.Length);
			// 0：起始位置。
			// 1：正常添加字符。
			// 2：需要添加替代字符。
			int state = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (predicate(text[i]))
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

		#endregion // 合并

		#region 字符串检查

		/// <summary>
		/// 判断指定的字符串是否是 <c>null</c> 或者 <see cref="string.Empty"/>。
		/// </summary>
		/// <param name="value">要测试的字符串。</param>
		/// <returns>如果 <paramref name="value"/> 参数是 <c>null</c> 或者 <see cref="string.Empty"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsNullOrEmpty(this string? value)
		{
			return string.IsNullOrEmpty(value);
		}

		/// <summary>
		/// 判断指定的字符串是否是 <c>null</c>、<see cref="string.Empty"/> 或者仅由空白字符组成。
		/// </summary>
		/// <param name="value">要测试的字符串。</param>
		/// <returns>如果 <paramref name="value"/> 参数是 <c>null</c>、<see cref="string.Empty"/> 
		/// 或者仅由空白字符组成，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsNullOrWhiteSpace(this string? value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		#endregion // 字符串检查

	}
}
