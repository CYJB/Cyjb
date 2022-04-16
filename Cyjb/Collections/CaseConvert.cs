using System.Collections.Concurrent;
using System.Globalization;

namespace Cyjb.Collections
{
	/// <summary>
	/// 提供切换大小写的功能。
	/// </summary>
	internal static class CaseConvert
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
		/// 大小写转换的映射表。
		/// </summary>
		private static readonly ConcurrentDictionary<CultureInfo, CaseConvertMapping> mappings = new();

		/// <summary>
		/// 添加指定字符范围对应的大/小写字符。
		/// </summary>
		/// <param name="mapping">映射关系表。</param>
		/// <param name="min">字符范围的起始。</param>
		/// <param name="max">字符范围的结束。</param>
		/// <param name="ranges">要添加到的字符范围。</param>
		private static void AddCaseRange(string[] mapping, char min, char max, CharSet ranges)
		{
			int low, high, mid;
			// 二分查找匹配的映射关系。
			for (low = 0, high = mapping.Length; low < high;)
			{
				mid = (low + high) >> 1;
				if (mapping[mid][IndexMax] < min)
				{
					low = mid + 1;
				}
				else
				{
					high = mid;
				}
			}
			if (low >= mapping.Length)
			{
				return;
			}
			char curMin, curMax;
			string map;
			for (; low < mapping.Length && (map = mapping[low])[IndexMin] <= max; low++)
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
					case CaseConvertMapping.OperatorSet:
						curMin = map[IndexData];
						curMax = map[IndexData];
						break;
					case CaseConvertMapping.OperatorAdd:
						curMin += map[IndexData];
						curMax += map[IndexData];
						break;
					case CaseConvertMapping.OperatorOr:
						curMin |= (char)1;
						curMax |= (char)1;
						break;
					case CaseConvertMapping.OperatorAddAnd:
						curMin += (char)(curMin & 1u);
						curMax += (char)(curMax & 1u);
						break;
					case CaseConvertMapping.OperatorAnd:
						curMin &= (char)0xFFFE;
						curMax &= (char)0xFFFE;
						break;
					case CaseConvertMapping.OperatorSubAnd:
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
		/// 添加指定字符范围对应的大写字符。
		/// </summary>
		/// <param name="culture">小写字母转换用到的区域信息。</param>
		/// <param name="min">字符范围的起始。</param>
		/// <param name="max">字符范围的结束。</param>
		/// <param name="ranges">要添加到的字符范围。</param>
		public static void AddUppercaseRange(CultureInfo culture, char min, char max, CharSet ranges)
		{
			string[] mapping = mappings.GetOrAdd(culture, (culture) => new CaseConvertMapping(culture))
				.ToUpperMapping.Value;
			AddCaseRange(mapping, min, max, ranges);
		}

		/// <summary>
		/// 添加指定字符范围对应的小写字符。
		/// </summary>
		/// <param name="culture">大写字母转换用到的区域信息。</param>
		/// <param name="min">字符范围的起始。</param>
		/// <param name="max">字符范围的结束。</param>
		/// <param name="ranges">要添加到的字符范围。</param>
		public static void AddLowercaseRange(CultureInfo culture, char min, char max, CharSet ranges)
		{
			string[] mapping = mappings.GetOrAdd(culture, (culture) => new CaseConvertMapping(culture))
				.ToLowerMapping.Value;
			AddCaseRange(mapping, min, max, ranges);
		}
	}
}
