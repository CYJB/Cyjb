using System.Globalization;
using System.Text;

namespace Cyjb.Collections
{
	/// <summary>
	/// 大小写转换的映射表。
	/// </summary>
	internal sealed class CaseConvertMapping
	{
		/// <summary>
		/// 直接设置相应的大/小写字符：Upper(ch)/Lower(ch) = constant。
		/// </summary>
		public const char OperatorSet = '\x00';
		/// <summary>
		/// 加上一个固定偏移的操作：ch + offset。
		/// </summary>
		public const char OperatorAdd = '\x01';
		/// <summary>
		/// 字符按位或 1 的操作：ch | 1。
		/// </summary>
		/// <remarks>对应于 lowerCase = upperCase + 1 且 upperCase 为偶数的场景，
		/// 此时 upperCase 或 lowerCase 都会被转换为 lowerCase。
		/// </remarks>
		public const char OperatorOr = '\x03';
		/// <summary>
		/// 加上按位与 1 的操作：ch + (ch &amp; 1)。
		/// </summary>
		/// <remarks>对应于 lowerCase = upperCase + 1 且 upperCase 为奇数的场景，
		/// 此时 upperCase 或 lowerCase 都会被转换为 lowerCase。
		/// </remarks>
		public const char OperatorAddAnd = '\x04';
		/// <summary>
		/// 字符按位与 -2 的操作：ch &amp; 0xFFFE。
		/// </summary>
		/// <remarks>对应于 lowerCase = upperCase + 1 且 lowerCase 为奇数的场景，
		/// 此时 upperCase 或 lowerCase 都会被转换为 upperCaseCase。
		/// </remarks>
		public const char OperatorAnd = '\x05';
		/// <summary>
		/// 减去上按位与 1 的操作：ch - (~ch &amp; 1)。
		/// </summary>
		/// <remarks>对应于 lowerCase = upperCase + 1 且 lowerCase 为偶数的场景，
		/// 此时 upperCase 或 lowerCase 都会被转换为 upperCaseCase。
		/// </remarks>
		public const char OperatorSubAnd = '\x06';
		/// <summary>
		/// 大小写转换使用的区域信息。
		/// </summary>
		private readonly TextInfo textInfo;
		/// <summary>
		/// 转换到小写字母的映射表。
		/// </summary>
		private readonly Lazy<string[]> toLowerMapping;
		/// <summary>
		/// 转换到大写字母的映射表。
		/// </summary>
		private readonly Lazy<string[]> toUpperMapping;

		/// <summary>
		/// 使用指定的区域信息初始化。
		/// </summary>
		/// <param name="culture">大小写转换使用的区域信息。</param>
		public CaseConvertMapping(CultureInfo culture)
		{
			textInfo = culture.TextInfo;
			toLowerMapping = new(BuildToLowerMapping);
			toUpperMapping = new(BuildToUpperMapping);
		}

		/// <summary>
		/// 获取转换到小写字母的映射表。
		/// </summary>
		public Lazy<string[]> ToLowerMapping => toLowerMapping;

		/// <summary>
		/// 获取转换到大写字母的映射表。
		/// </summary>
		public Lazy<string[]> ToUpperMapping => toUpperMapping;

		/// <summary>
		/// 生成从大写字母转换为小写字母的映射表。
		/// </summary>
		/// <returns>映射表。</returns>
		private string[] BuildToLowerMapping()
		{
			List<string> result = new(255);
			char min = '\0', max = '\0', op = '\0', data = '\0';
			StringBuilder builder = new("0000");
			for (int i = 0; i < char.MaxValue; i++)
			{
				char upperChar = (char)i;
				char lowerChar = textInfo.ToLower(upperChar);
				if (upperChar == lowerChar)
				{
					continue;
				}
				if (min == '\0')
				{
					min = max = upperChar;
					op = GetToLowerOperator(upperChar, lowerChar, out data);
					continue;
				}
				char curOp = GetToLowerOperator(upperChar, lowerChar, out char curData);
				char maxLower;
				if (upperChar == max + 1)
				{
					if (curOp == op && curData == data)
					{
						max = upperChar;
						continue;
					}
					else
					{
						maxLower = textInfo.ToLower(max);
						if (maxLower == lowerChar)
						{
							builder[0] = min;
							builder[1] = upperChar;
							builder[2] = OperatorSet;
							builder[3] = lowerChar;
							result.Add(builder.ToString());
							min = '\0';
							continue;
						}
					}
				}
				else
				{
					maxLower = textInfo.ToLower(max);
					if (maxLower == max + 1 && upperChar == maxLower + 1)
					{
						if (curOp == op && curData == data)
						{
							max = upperChar;
							continue;
						}
					}
				}
				if (min == max)
				{
					op = OperatorSet;
					data = maxLower;
				}
				builder[0] = min;
				builder[1] = max;
				builder[2] = op;
				builder[3] = data;
				result.Add(builder.ToString());
				min = max = upperChar;
				op = curOp;
				data = curData;
			}
			builder[0] = min;
			builder[1] = max;
			builder[2] = op;
			builder[3] = data;
			result.Add(builder.ToString());
			return result.ToArray();
		}

		/// <summary>
		/// 返回从大写字母转换为小写字母的操作符。
		/// </summary>
		/// <param name="upperChar">大写字母。</param>
		/// <param name="lowerChar">小写字母。</param>
		/// <param name="data">操作的数据。</param>
		/// <returns>对应的操作符。</returns>
		private static char GetToLowerOperator(char upperChar, char lowerChar, out char data)
		{
			if (lowerChar == upperChar + 1)
			{
				data = '\0';
				if ((upperChar & 1) == 0)
				{
					return OperatorOr;
				}
				else
				{
					return OperatorAddAnd;
				}
			}
			else
			{
				data = (char)(lowerChar - upperChar);
				return OperatorAdd;
			}
		}

		/// <summary>
		/// 生成从小写字母转换为大写字母的映射表。
		/// </summary>
		/// <returns>映射表。</returns>
		private string[] BuildToUpperMapping()
		{
			List<string> result = new(255);
			char min = '\0', max = '\0', op = '\0', data = '\0';
			StringBuilder builder = new("0000");
			for (int i = 0; i < char.MaxValue; i++)
			{
				char lowerChar = (char)i;
				char upperChar = textInfo.ToUpper(lowerChar);
				if (lowerChar == upperChar)
				{
					continue;
				}
				if (min == '\0')
				{
					min = max = lowerChar;
					op = GetToUpperOperator(lowerChar, upperChar, out data);
					continue;
				}
				char curOp = GetToUpperOperator(lowerChar, upperChar, out char curData);
				char maxUpper;
				if (lowerChar == max + 1)
				{
					if (curOp == op && curData == data)
					{
						max = lowerChar;
						continue;
					}
					else
					{
						maxUpper = textInfo.ToUpper(max);
						if (maxUpper == lowerChar)
						{
							builder[0] = min;
							builder[1] = lowerChar;
							builder[2] = OperatorSet;
							builder[3] = upperChar;
							result.Add(builder.ToString());
							min = '\0';
							continue;
						}
					}
				}
				else
				{
					if (upperChar == max + 1 && lowerChar == upperChar + 1)
					{
						if (curOp == op && curData == data)
						{
							max = lowerChar;
							continue;
						}
					}
					maxUpper = textInfo.ToUpper(max);
				}
				if (min == max)
				{
					op = OperatorSet;
					data = maxUpper;
				}
				builder[0] = min;
				builder[1] = max;
				builder[2] = op;
				builder[3] = data;
				result.Add(builder.ToString());
				min = max = lowerChar;
				op = curOp;
				data = curData;
			}
			builder[0] = min;
			builder[1] = max;
			builder[2] = op;
			builder[3] = data;
			result.Add(builder.ToString());
			return result.ToArray();
		}

		/// <summary>
		/// 返回从小写字母转换为大写字母的操作符。
		/// </summary>
		/// <param name="lowerChar">小写字母。</param>
		/// <param name="upperChar">大写字母。</param>
		/// <param name="data">操作的数据。</param>
		/// <returns>对应的操作符。</returns>
		private static char GetToUpperOperator(char lowerChar, char upperChar, out char data)
		{
			if (lowerChar == upperChar + 1)
			{
				data = '\0';
				if ((lowerChar & 1) == 0)
				{
					return OperatorSubAnd;
				}
				else
				{
					return OperatorAnd;
				}
			}
			else
			{
				data = (char)(upperChar - lowerChar);
				return OperatorAdd;
			}
		}
	}
}
