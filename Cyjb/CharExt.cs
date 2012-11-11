using System.Text;

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
		/// <param name="c">要计算的 Unicode 字符。</param>
		/// <returns>如果 <paramref name="c"/> 是十进制数字，则为 <c>true</c>；
		/// 否则，为 <c>false</c>。</returns>
		public static bool IsHex(this char c)
		{
			if (c <= 'f')
			{
				if (c >= 'A')
				{
					return c <= 'F' || c >= 'a';
				}
				else
				{
					return c >= '0' && c <= '9';
				}
			}
			return false;
		}
		/// <summary>
		/// 指示指定字符串中位于指定位置处的字符是否属于十六进制数字类别。
		/// </summary>
		/// <param name="s">一个字符串。</param>
		/// <param name="index">要计算的字符在 <paramref name="s"/> 中的位置。</param>
		/// <returns>如果 <paramref name="s"/> 中位于 <paramref name="index"/> 处的字符是十进制数字，
		/// 则为 <c>true</c>；否则，为 <c>false</c>。</returns>
		/// <exception cref="System.IndexOutOfRangeException"><paramref name="index"/> 大于等于字符串的长度或小于零。</exception>
		public static bool IsHex(string s, int index)
		{
			return IsHex(s[index]);
		}

		#endregion // IsHex

		/// <summary>
		/// 返回当前字符的可打印字符串形式。不可打印的字符将返回相应的转义字符串。
		/// </summary>
		/// <param name="ch">要获取可打印字符串形式的字符。</param>
		/// <returns>字符的可打印字符串形式。</returns>
		public static string ToPrintableString(this char ch)
		{
			// 转换字符转义。
			switch (ch)
			{
				case '\\': return "\\\\";
				case '\a': return "\\a";
				case '\b': return "\\b";
				case '\u001B': return "\\e";
				case '\f': return "\\f";
				case '\n': return "\\n";
				case '\r': return "\\r";
				case '\t': return "\\t";
				case '\v': return "\\v";
			}
			// ASCII 可见字符。
			if (ch >= ' ' && ch <= '~')
			{
				return ch.ToString();
			}
			StringBuilder builder = new StringBuilder();
			int shift;
			// 十六进制字符，较短的使用 \x 表示，较长的使用 \u 表示。
			if (ch < 256)
			{
				builder.Append("\\x");
				shift = 8;
			}
			else
			{
				builder.Append("\\u");
				shift = 16;
			}
			while (shift > 0)
			{
				shift -= 4;
				builder.Append(BaseDigits[(ch >> shift) & 0xF]);
			}
			return builder.ToString();
		}
	}
}
