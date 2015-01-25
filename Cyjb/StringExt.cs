using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.String"/> 类的扩展方法。
	/// </summary>
	public static class StringExt
	{

		#region Escape

		/// <summary>
		/// 返回当前字符串的转义字符串。
		/// </summary>
		/// <param name="str">要进行转义的字符串。</param>
		/// <returns>转义后的字符串。</returns>
		/// <overloads>
		/// <summary>
		/// 返回当前字符串的转义字符串。
		/// </summary>
		/// </overloads>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v），会替换为其转义形式。</para>
		/// <para>对于其它字符，会替换为其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string Escape(this string str)
		{
			return Escape(str, null);
		}
		/// <summary>
		/// 返回当前字符串的转义字符串。
		/// </summary>
		/// <param name="str">要进行转义的字符串。</param>
		/// <param name="customEscape">自定义的需要转义的字符，会在前面加上 \。</param>
		/// <returns>转义后的字符串。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v）以及自定义的字符，会替换为其转义形式。</para>
		/// <para>对于其它字符，会替换为其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string Escape(this string str, ISet<char> customEscape)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			StringBuilder builder = new StringBuilder(str.Length * 2);
			for (int i = 0; i < str.Length; i++)
			{
				builder.Append(str[i].Escape(customEscape));
			}
			return builder.ToString();
		}
		/// <summary>
		/// 返回当前字符串的 Unicode 转义字符串。
		/// </summary>
		/// <param name="str">要进行转义的字符串。</param>
		/// <returns>转义后的字符串。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
		/// <para>对于其它字符，会替换为其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string EscapeUnicode(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			StringBuilder builder = new StringBuilder(str.Length * 2);
			for (int i = 0; i < str.Length; i++)
			{
				builder.Append(str[i].EscapeUnicode());
			}
			return builder.ToString();
		}
		/// <summary>
		/// 将字符串中的转义字符转换为原始的字符。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <returns>转换后的字符串。</returns>
		/// <overloads>
		/// <summary>
		/// 将字符串中的转义字符转换为原始的字符。
		/// </summary>
		/// </overloads>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
		/// <para>对于特殊字符的转义（\0, \, \a, \b, \f, \n, \r, \t, \v），会替换为其原始字符。</para>
		/// <para>对于 Unicode 转义（\u, \U 和 \x），会替换为相应字符。</para>
		/// </remarks>
		public static string Unescape(this string str)
		{
			return Unescape(str, false, null);
		}
		/// <summary>
		/// 将字符串中的转义字符转换为原始的字符。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <param name="customEscape">自定义的转义字符，会在前面加上 \。</param>
		/// <returns>转换后的字符串。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
		/// <para>对于特殊字符的转义（\0, \, \a, \b, \f, \n, \r, \t, \v）以及自定义的字符，会替换为其原始字符。</para>
		/// <para>对于 Unicode 转义（\u, \U 和 \x），会替换为相应字符。</para>
		/// </remarks>
		public static string Unescape(this string str, ISet<char> customEscape)
		{
			return Unescape(str, false, customEscape);
		}
		/// <summary>
		/// 将字符串中的 \u,\U 和 \x 转义转换为对应的字符。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <returns>转换后的字符串。</returns>
		/// <remarks>
		/// <para>解码方法支持 \x，\u 和 \U 转义，其中 \x 后面是 2 个十六进制字符，
		/// \u 后面是 4 个十六进制字符，\U 后面则是 8 个十六进制字符。
		/// 使用 \U 转义时，大于 0x10FFFF 的值不被支持，会被舍弃。</para>
		/// <para>如果不满足上面的情况，则不会进行转义，也不会报错。</para></remarks>
		public static string UnescapeUnicode(this string str)
		{
			return Unescape(str, true, null);
		}
		/// <summary>
		/// 将字符串中的转义字符转换为原始的字符。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <param name="unicodeOnly">是否只处理 Unicode 转义。</param>
		/// <param name="customEscape">自定义的转义字符，会在前面加上 \。</param>
		/// <returns>转换后的字符串。</returns>
		private static string Unescape(this string str, bool unicodeOnly, ISet<char> customEscape)
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
				if (idx >= len)
				{
					break;
				}
				// Unicode 转义需要的十六进制字符的长度。
				int hexLen = 0;
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
				}
				if (hexLen > 0)
				{
					// Unicode 反转义。
					if (CheckHexLength(str, idx + 1, hexLen))
					{
						idx++;
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
						idx = start = idx + hexLen;
					}
				}
				else if (!unicodeOnly)
				{
					// 其它反转义。
					char result;
					if (TryUnescape(str[idx], customEscape, out result))
					{
						builder.Append(result);
						idx = start = idx + 1;
					}
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
				if (!CharExt.IsHex(str, index))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 尝试返回与转义字符 <paramref name="ch"/> 对应的原始字符。
		/// </summary>
		/// <param name="ch">转义字符。</param>
		/// <param name="customEscape">自定义的转义字符。</param>
		/// <param name="result">原始字符。</param>
		/// <returns>如果存在对应的原始字符，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool TryUnescape(char ch, ISet<char> customEscape, out char result)
		{
			switch (ch)
			{
				case '0':
					result = '\0';
					return true;
				case '\\':
					result = '\\';
					return true;
				case 'a':
					result = '\a';
					return true;
				case 'b':
					result = '\b';
					return true;
				case 'f':
					result = '\f';
					return true;
				case 'n':
					result = '\n';
					return true;
				case 'r':
					result = '\r';
					return true;
				case 't':
					result = '\t';
					return true;
				case 'v':
					result = '\v';
					return true;
				default:
					if (customEscape != null && customEscape.Contains(ch))
					{
						result = ch;
						return true;
					}
					break;
			}
			result = '\0';
			return false;
		}

		#endregion // Escape

		#region 截取

		/// <summary>
		/// 返回一个字符串，其中包含从指定字符串左端开始的指定数量的字符。
		/// </summary>
		/// <param name="str">从该字符串返回最左端的字符。</param>
		/// <param name="length">指示要返回的字符数的数值表达式。
		/// 如果为 <c>0</c>，则返回零长度字符串 ("")。
		/// 如果大于或等于 <paramref name="str"/> 的长度，则返回整个字符串。</param>
		/// <returns>从指定字符串左端开始的指定数量的字符。</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		public static string Left(this string str, int length)
		{
			if (length < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<string>() != null);
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
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		public static string Right(this string str, int length)
		{
			if (length < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<string>() != null);
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
		/// 如果 <paramref name="startIndex"/> 等于此实例的长度，则为 <see cref="String.Empty"/>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 大于此实例的长度。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 小于负的此实例的长度。</exception>
		/// <seealso cref="String.Substring(int)"/>
		public static string SubstringEx(this string str, int startIndex)
		{
			if (str == null)
			{
				throw CommonExceptions.ArgumentNull("str");
			}
			if (startIndex < -str.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<string>() != null);
			if (startIndex < 0)
			{
				startIndex += str.Length;
			}
			return str.Substring(startIndex);
		}
		/// <summary>
		/// 从此实例检索子字符串。子字符串从指定的字符位置开始且具有指定的长度。如果 
		/// <paramref name="startIndex"/> 小于 <c>0</c>，那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <param name="str">要检索子字符串的字符串实例。</param>
		/// <param name="startIndex">此示例中子字符串的起始字符位置（从零开始）。</param>
		/// <param name="length">子字符串中的字符数。</param>
		/// <returns>与此实例中在 <paramref name="startIndex"/> 处开头、长度为 <paramref name="length"/> 
		/// 的子字符串等效的一个字符串，如果 <paramref name="startIndex"/> 等于此实例的长度或 
		/// <paramref name="length"/> 为零，则为空字符串（""）。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 加 <paramref name="length"/>
		/// 之和指示的位置不在此实例中。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 小于负的此实例的长度。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于零。</exception>
		/// <seealso cref="String.Substring(int,int)"/>
		/// <overloads>
		/// <summary>
		/// 从此实例检索子字符串。
		/// </summary>
		/// </overloads>
		public static string SubstringEx(this string str, int startIndex, int length)
		{
			if (str == null)
			{
				throw CommonExceptions.ArgumentNull("str");
			}
			if (length < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			if (startIndex < -str.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<string>() != null);
			if (startIndex < 0)
			{
				startIndex += str.Length;
			}
			return str.Substring(startIndex, length);
		}
		/// <summary>
		/// 从此实例检索子字符串。子字符串从指定的字符位置开始到字符串的结尾。
		/// 如果 <paramref name="startIndex"/> 小于 <c>0</c>，那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <param name="str">要检索子字符串的字符串实例。</param>
		/// <param name="startIndex">此示例中子字符串的起始字符位置（从零开始）。</param>
		/// <returns>与此实例中在 <paramref name="startIndex"/> 处开头到字符串结尾的子字符串等效的一个字符串，
		/// 如果 <paramref name="startIndex"/> 等于此实例的长度或大于等于字符串的长度，则为空字符串（""）。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 指示的位置不在此实例中。</exception>
		public static string Slice(this string str, int startIndex)
		{
			if (str == null)
			{
				throw CommonExceptions.ArgumentNull("str");
			}
			if (startIndex < -str.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<string>() != null);
			if (startIndex < 0)
			{
				startIndex += str.Length;
			}
			if (startIndex < str.Length)
			{
				return str.Substring(startIndex);
			}
			return string.Empty;
		}
		/// <summary>
		/// 从此实例检索子字符串。子字符串从指定的字符位置开始，从指定的字符位置结束。
		/// 如果 <paramref name="startIndex"/> 或 <paramref name="endIndex"/>
		/// 小于 <c>0</c>，那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <param name="str">要检索子字符串的字符串实例。</param>
		/// <param name="startIndex">此示例中子字符串的起始字符位置（从零开始）。</param>
		/// <param name="endIndex">此示例中子字符串的结束字符位置（从零开始），但不包括该位置的字符。</param>
		/// <returns>与此实例中在 <paramref name="startIndex"/> 处开头、在 <paramref name="endIndex"/> 
		/// 处结束的子字符串等效的一个字符串，如果 <paramref name="startIndex"/> 等于此实例的长度或大于等于 
		/// <paramref name="endIndex"/> ，则为 <see cref="String.Empty"/>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 或 <paramref name="endIndex"/>
		/// 指示的位置不在此实例中。</exception>
		/// <overloads>
		/// <summary>
		/// 从此实例检索子字符串。
		/// </summary>
		/// </overloads>
		public static string Slice(this string str, int startIndex, int endIndex)
		{
			if (str == null)
			{
				throw CommonExceptions.ArgumentNull("str");
			}
			if (startIndex < -str.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (endIndex < -str.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("endIndex", endIndex);
			}
			Contract.Ensures(Contract.Result<string>() != null);
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
			return string.Empty;
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
						TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(str, i);
						textElementEnumerator.MoveNext();
						int idx = textElementEnumerator.ElementIndex;
						while (end >= 0)
						{
							i = idx;
							idx = textElementEnumerator.MoveNext() ? textElementEnumerator.ElementIndex : len;
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
		/// 判断指定的字符串是否是 <c>null</c> 或者 <see cref="String.Empty"/>。
		/// </summary>
		/// <param name="value">要测试的字符串。</param>
		/// <returns>如果 <paramref name="value"/> 参数是 <c>null</c> 或者 <see cref="String.Empty"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}
		/// <summary>
		/// 判断指定的字符串是否是 <c>null</c>、<see cref="String.Empty"/> 或者仅由空白字符组成。
		/// </summary>
		/// <param name="value">要测试的字符串。</param>
		/// <returns>如果 <paramref name="value"/> 参数是 <c>null</c>、<see cref="String.Empty"/> 
		/// 或者仅由空白字符组成，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		#endregion // 字符串检查

	}
}
