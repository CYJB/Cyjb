
using System.Collections.Generic;
namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.Char"/> 类的扩展方法。
	/// </summary>
	public static class CharExt
	{
		/// <summary>
		/// 所有用于不同进制的字符。
		/// </summary>
		internal static readonly char[] BaseDigits = new char[] {
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
			'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

		#region IsHex

		/// <summary>
		/// 指示指定的 Unicode 字符是否属于十六进制数字类别。
		/// </summary>
		/// <param name="ch">要计算的 Unicode 字符。</param>
		/// <returns>如果 <paramref name="ch"/> 是十进制数字，则为 <c>true</c>；
		/// 否则，为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 指示指定的 Unicode 字符是否属于十六进制数字类别。
		/// </summary>
		/// </overloads>
		public static bool IsHex(this char ch)
		{
			if (ch <= 'f')
			{
				if (ch >= 'A')
				{
					return ch <= 'F' || ch >= 'a';
				}
				else
				{
					return ch >= '0' && ch <= '9';
				}
			}
			return false;
		}
		/// <summary>
		/// 指示指定字符串中位于指定位置处的字符是否属于十六进制数字类别。
		/// </summary>
		/// <param name="str">一个字符串。</param>
		/// <param name="index">要计算的字符在 <paramref name="str"/> 中的位置。</param>
		/// <returns>如果 <paramref name="str"/> 中位于 <paramref name="index"/> 处的字符是十进制数字，
		/// 则为 <c>true</c>；否则，为 <c>false</c>。</returns>
		/// <exception cref="System.IndexOutOfRangeException"><paramref name="index"/> 大于等于字符串的长度或小于零。</exception>
		public static bool IsHex(string str, int index)
		{
			return IsHex(str[index]);
		}

		#endregion // IsHex

		#region Escape

		/// <summary>
		/// 返回当前字符的转义字符串。
		/// </summary>
		/// <param name="ch">要获取转义字符串的字符。</param>
		/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
		/// <overloads>
		/// <summary>
		/// 返回当前字符的转义字符串。
		/// </summary>
		/// </overloads>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v），会返回其转义形式。</para>
		/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string Escape(this char ch)
		{
			return Escape(ch, null);
		}
		/// <summary>
		/// 返回当前字符的转义字符串。
		/// </summary>
		/// <param name="ch">要获取转义字符串的字符。</param>
		/// <param name="customEscape">自定义的需要转义的字符。</param>
		/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v）以及自定义的字符，会返回其转义形式。</para>
		/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string Escape(this char ch, ISet<char> customEscape)
		{
			// 转换字符转义。
			switch (ch)
			{
				case '\0': return "\\0";
				case '\\': return "\\\\";
				case '\a': return "\\a";
				case '\b': return "\\b";
				case '\f': return "\\f";
				case '\n': return "\\n";
				case '\r': return "\\r";
				case '\t': return "\\t";
				case '\v': return "\\v";
			}
			if (customEscape != null && customEscape.Contains(ch))
			{
				return "\\" + ch;
			}
			return EscapeUnicode(ch);
		}
		/// <summary>
		/// 返回当前字符的 Unicode 转义字符串。
		/// </summary>
		/// <param name="ch">要获取转义字符串的字符。</param>
		/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
		/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string EscapeUnicode(this char ch)
		{
			if (ch >= ' ' && ch <= '~')
			{
				return ch.ToString();
			}
			return string.Concat("\\u", BaseDigits[ch >> 12], BaseDigits[(ch >> 8) & 0xF],
				BaseDigits[(ch >> 4) & 0xF], BaseDigits[ch & 0xF]);
		}

		#endregion // Escape

	}
}
