using System.Collections.Concurrent;
using System.Globalization;

namespace Cyjb.Collections;

/// <summary>
/// 大小写转换器。
/// </summary>
internal sealed class CaseConverter
{
	/// <summary>
	/// 最小值的索引。
	/// </summary>
	private const int IndexMin = 0;
	/// <summary>
	/// 最大值的索引。
	/// </summary>
	private const int IndexMax = 1;
	/// <summary>
	/// 操作符的索引。
	/// </summary>
	private const int IndexOperator = 2;
	/// <summary>
	/// 数据的索引。
	/// </summary>
	private const int IndexData = 3;

	/// <summary>
	/// 直接设置相应的大/小写字符：Upper(ch)/Lower(ch) = constant。
	/// </summary>
	private const char OperatorSet = '\x00';
	/// <summary>
	/// 加上一个固定偏移的操作：ch + offset。
	/// </summary>
	private const char OperatorAdd = '\x01';
	/// <summary>
	/// 字符按位或 1 的操作：ch | 1。
	/// </summary>
	/// <remarks>对应于 lowerCase = upperCase + 1 且 upperCase 为偶数的场景，
	/// 此时 upperCase 或 lowerCase 都会被转换为 lowerCase。
	/// </remarks>
	private const char OperatorOr = '\x03';
	/// <summary>
	/// 加上按位与 1 的操作：ch + (ch &amp; 1)。
	/// </summary>
	/// <remarks>对应于 lowerCase = upperCase + 1 且 upperCase 为奇数的场景，
	/// 此时 upperCase 或 lowerCase 都会被转换为 lowerCase。
	/// </remarks>
	private const char OperatorAddAnd = '\x04';
	/// <summary>
	/// 字符按位与 -2 的操作：ch &amp; 0xFFFE。
	/// </summary>
	/// <remarks>对应于 lowerCase = upperCase + 1 且 lowerCase 为奇数的场景，
	/// 此时 upperCase 或 lowerCase 都会被转换为 upperCaseCase。
	/// </remarks>
	private const char OperatorAnd = '\x05';
	/// <summary>
	/// 减去上按位与 1 的操作：ch - (~ch &amp; 1)。
	/// </summary>
	/// <remarks>对应于 lowerCase = upperCase + 1 且 lowerCase 为偶数的场景，
	/// 此时 upperCase 或 lowerCase 都会被转换为 upperCaseCase。
	/// </remarks>
	private const char OperatorSubAnd = '\x06';

	/// <summary>
	/// 大写字母到小写字母的转换器。
	/// </summary>
	private static readonly ConcurrentDictionary<CultureInfo, CaseConverter> toLowerConverter = new();
	/// <summary>
	/// 小写字母到大写字母的转换器。
	/// </summary>
	private static readonly ConcurrentDictionary<CultureInfo, CaseConverter> toUpperConverter = new();

	/// <summary>
	/// 返回大写字母到小写字母的转换器。
	/// </summary>
	/// <param name="culture">转换用到的区域信息。</param>
	public static CaseConverter GetLowercaseConverter(CultureInfo culture)
	{
		return toLowerConverter.GetOrAdd(culture, (culture) => new CaseConverter(culture, true));
	}

	/// <summary>
	/// 返回小写字母到小写字母的转换器。
	/// </summary>
	/// <param name="culture">转换用到的区域信息。</param>
	public static CaseConverter GetUppercaseConverter(CultureInfo culture)
	{
		return toUpperConverter.GetOrAdd(culture, (culture) => new CaseConverter(culture, false));
	}

	/// <summary>
	/// 大小写转换的映射表。
	/// </summary>
	private readonly string[] mapping;

	/// <summary>
	/// 使用指定的区域信息初始化。
	/// </summary>
	/// <param name="culture">大小写转换使用的区域信息。</param>
	/// <param name="toLower">是否是转换为小写字母。</param>
	private CaseConverter(CultureInfo culture, bool toLower)
	{
		if (toLower)
		{
			mapping = BuildToLowerMapping(culture.TextInfo);
		}
		else
		{
			mapping = BuildToUpperMapping(culture.TextInfo);
		}
	}

	/// <summary>
	/// 添加指定字符范围对应的大/小写字符。
	/// </summary>
	/// <param name="min">字符范围的起始。</param>
	/// <param name="max">字符范围的结束。</param>
	/// <param name="ranges">要添加到的字符范围。</param>
	public void ConvertRange(char min, char max, CharSet ranges)
	{
		int index = GetLowerBound(min);
		if (index >= mapping.Length)
		{
			return;
		}
		char curMin, curMax;
		string map;
		for (; index < mapping.Length && (map = mapping[index])[IndexMin] <= max; index++)
		{
			if ((curMin = map[IndexMin]) < min)
			{
				curMin = min;
			}
			if ((curMax = map[IndexMax]) > max)
			{
				curMax = max;
			}
			switch (map[IndexOperator])
			{
				case OperatorSet:
					curMin = map[IndexData];
					curMax = map[IndexData];
					break;
				case OperatorAdd:
					curMin += map[IndexData];
					curMax += map[IndexData];
					break;
				case OperatorOr:
					curMin |= (char)1;
					curMax |= (char)1;
					break;
				case OperatorAddAnd:
					curMin += (char)(curMin & 1u);
					curMax += (char)(curMax & 1u);
					break;
				case OperatorAnd:
					curMin &= (char)0xFFFE;
					curMax &= (char)0xFFFE;
					break;
				case OperatorSubAnd:
					curMin -= (char)(~curMin & 1u);
					curMax -= (char)(~curMax & 1u);
					break;
			}
			if (curMin < min || curMax > max)
			{
				ranges.Add(curMin, curMax);
			}
		}
	}

	/// <summary>
	/// 返回指定字符对应的大/小写字符。
	/// </summary>
	/// <param name="ch">要转换的字符。</param>
	/// <returns>转换后的字符。</returns>
	public char ConvertChar(char ch)
	{
		int index = GetLowerBound(ch);
		if (index >= mapping.Length)
		{
			return ch;
		}
		string map = mapping[index];
		if (map[IndexMin] > ch)
		{
			return ch;
		}
		return map[IndexOperator] switch
		{
			OperatorSet => map[IndexData],
			OperatorAdd => (char)(ch + map[IndexData]),
			OperatorOr => (char)(ch | 1u),
			OperatorAddAnd => (char)(ch + (ch & 1u)),
			OperatorAnd => (char)(ch & 0xFFFE),
			OperatorSubAnd => (char)(ch - (~ch & 1u)),
			_ => ch,
		};
	}

	/// <summary>
	/// 返回指定字符在映射表中的索引下界。
	/// </summary>
	/// <param name="ch">要查找的字符。</param>
	/// <returns>字符的索引下界。</returns>
	private int GetLowerBound(char ch)
	{
		int low, high, mid;
		// 二分查找匹配的映射关系。
		for (low = 0, high = mapping.Length; low < high;)
		{
			mid = (low + high) >> 1;
			if (mapping[mid][IndexMax] < ch)
			{
				low = mid + 1;
			}
			else
			{
				high = mid;
			}
		}
		return low;
	}

	/// <summary>
	/// 生成从大写字母转换为小写字母的映射表。
	/// </summary>
	/// <param name="textInfo">生成映射时使用的文本信息</param>
	/// <returns>映射表。</returns>
	private static string[] BuildToLowerMapping(TextInfo textInfo)
	{
		using ValueList<string> result = new(200);
		char min = '\0', max = '\0', op = '\0', data = '\0';
		Span<char> builder = stackalloc char[4];
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
	/// <param name="textInfo">生成映射时使用的文本信息</param>
	/// <returns>映射表。</returns>
	private static string[] BuildToUpperMapping(TextInfo textInfo)
	{
		using ValueList<string> result = new(200);
		char min = '\0', max = '\0', op = '\0', data = '\0';
		Span<char> builder = stackalloc char[4];
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
