using System.Text;
using System.Globalization;
using Cyjb.Globalization;

namespace Cyjb.Collections;

public sealed partial class CharSet
{
	/// <summary>
	/// 简化字符集合字符串表示用到的 Unicode 类别。
	/// </summary>
	private class UnicodeCategoryInfo
	{
		/// <summary>
		/// 使用指定的 Unicode 类别和相应字符集合初始化 <see cref="UnicodeCategoryInfo"/> 类的新实例。
		/// </summary>
		/// <param name="chars">Unicode 类别对应的字符集合。</param>
		/// <param name="categories">Unicode 类别。</param>
		public UnicodeCategoryInfo(IReadOnlySet<char> chars, params UnicodeCategory[] categories)
		{
			Chars = chars;
			Categories = categories;
		}

		/// <summary>
		/// Unicode 类别对应的字符集合。
		/// </summary>
		public IReadOnlySet<char> Chars { get; }
		/// <summary>
		/// Unicode 类别。
		/// </summary>
		public UnicodeCategory[] Categories { get; }
	}

	/// <summary>
	/// Unicode 类别信息。
	/// </summary>
	private static Lazy<UnicodeCategoryInfo[]> UnicodeCategoryInfos = new(() =>
	{
		UnicodeCategory[] categories = Enum.GetValues<UnicodeCategory>();
		UnicodeCategoryInfo[] infos = new UnicodeCategoryInfo[categories.Length + 6];
		int i = 0;
		for (; i < categories.Length; i++)
		{
			UnicodeCategory category = categories[i];
			infos[i] = new UnicodeCategoryInfo(category.GetChars(), category);
		}
		// 定制的特殊 Unicode 类别
		CharSet lull = (CharSet)UnicodeCategory.UppercaseLetter.GetChars() +
			UnicodeCategory.LowercaseLetter.GetChars();
		// UppercaseLetter & LowercaseLetter
		infos[i++] = new UnicodeCategoryInfo(lull,
			UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter
			);
		// UppercaseLetter & LowercaseLetter & TitlecaseLetter
		infos[i++] = new UnicodeCategoryInfo(lull + UnicodeCategory.TitlecaseLetter.GetChars(),
			UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter, UnicodeCategory.TitlecaseLetter
			);
		CharSet mn = (CharSet)UnicodeCategory.NonSpacingMark.GetChars();
		CharSet mnmc = mn + UnicodeCategory.SpacingCombiningMark.GetChars();
		// NonSpacingMark & SpacingCombiningMark
		infos[i++] = new UnicodeCategoryInfo(mnmc,
			UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark
			);
		// NonSpacingMark & SpacingCombiningMark & EnclosingMark
		infos[i++] = new UnicodeCategoryInfo(mnmc + UnicodeCategory.EnclosingMark.GetChars(),
			UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark, UnicodeCategory.EnclosingMark
			);
		// NonSpacingMark & EnclosingMark
		infos[i++] = new UnicodeCategoryInfo(mn + UnicodeCategory.EnclosingMark.GetChars(),
			UnicodeCategory.NonSpacingMark, UnicodeCategory.EnclosingMark
			);
		// OpenPunctuation & ClosePunctuation
		infos[i++] = new UnicodeCategoryInfo((CharSet)UnicodeCategory.OpenPunctuation.GetChars() +
			UnicodeCategory.ClosePunctuation.GetChars(),
			UnicodeCategory.OpenPunctuation, UnicodeCategory.ClosePunctuation
			);
		// 按字符个数从小到大排序。
		Array.Sort(infos, (left, right) => left.Chars.Count - right.Chars.Count);
		return infos;
	});

	/// <summary>
	/// 返回当前集合的字符串表示。
	/// </summary>
	/// <returns>当前集合的字符串表示。</returns>
	public override string ToString()
	{
		if (text == null)
		{
			StringBuilder builder = new();
			builder.Append('[');
			// 允许通过 UnicodeCategory 压缩字符串表示形式。
			List<UnicodeCategoryInfo> infos = new(UnicodeCategoryInfos.Value);
			List<UnicodeCategoryInfo> nextInfos = new();
			List<UnicodeCategory> categories = new();
			CharSet chars = new(this);
			CharSet negate = new();
			bool changed = true;
			while (changed)
			{
				changed = false;
				for (int i = 0; i < infos.Count; i++)
				{
					UnicodeCategoryInfo info = infos[i];
					IReadOnlySet<char> catChars = info.Chars;
					// 允许最多尝试 10% 的排除字符。
					if (1.1 * chars.Count < catChars.Count)
					{
						// 字符个数不足，后面 Unicode 类别的字符个数更多，可以全部跳过。
						break;
					}
					if (!chars.Overlaps(catChars))
					{
						continue;
					}
					// 确保应用 Unicode 类别后能够减少范围和字符个数。
					// 不满足要求的需要在后面重复检测，避免遗漏。
					CharSet applied = chars ^ catChars;
					if (applied.RangeCount() > chars.RangeCount() || applied.Count > chars.Count)
					{
						nextInfos.Add(info);
						continue;
					}
					negate.UnionWith(applied);
					chars.ExceptWith(catChars);
					categories.AddRange(info.Categories);
					changed = true;
				}
				var temp = infos;
				infos = nextInfos;
				nextInfos = temp;
				nextInfos.Clear();
			}
			negate.ExceptWith(this);
			// 输出剩余字符
			PrintChars(chars, builder);
			// 输出 Unicode 类别
			if (categories.Count > 0)
			{
				categories.Sort();
				foreach (UnicodeCategory category in categories)
				{
					builder.Append(@"\p{");
					builder.Append(category.GetName());
					builder.Append('}');
				}
			}
			// 输出排除字符
			if (negate.Count > 0)
			{
				builder.Append("-[");
				PrintChars(negate, builder);
				builder.Append(']');
			}
			builder.Append(']');
			text = builder.ToString();
		}
		return text;
	}

	/// <summary>
	/// 打印指定的字符集合。
	/// </summary>
	/// <param name="set">要打印的字符集合。</param>
	/// <param name="builder">字符串输出。</param>
	private static void PrintChars(CharSet set, StringBuilder builder)
	{
		foreach (var (start, end) in set.Ranges())
		{
			builder.Append(start.UnicodeEscape(false));
			if (start < end)
			{
				if (start + 1 < end)
				{
					builder.Append('-');
				}
				builder.Append(end.UnicodeEscape(false));
			}
		}
	}
}
