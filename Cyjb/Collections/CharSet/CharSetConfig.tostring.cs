using System.Text;
using System.Globalization;
using Cyjb.Globalization;

namespace Cyjb.Collections;

internal static partial class CharSetConfig
{
	/// <summary>
	/// 简化字符集合字符串表示用到的 Unicode 类别。
	/// </summary>
	private sealed class UnicodeCategoryInfo
	{
		/// <summary>
		/// 使用指定的 Unicode 类别和相应字符集合初始化 <see cref="UnicodeCategoryInfo"/> 类的新实例。
		/// </summary>
		/// <param name="chars">Unicode 类别对应的字符集合。</param>
		/// <param name="categories">Unicode 类别。</param>
		public UnicodeCategoryInfo(ReadOnlyCharSet chars, params UnicodeCategory[] categories)
		{
			Chars = chars;
			Categories = categories;
		}

		/// <summary>
		/// Unicode 类别对应的字符集合。
		/// </summary>
		public ReadOnlyCharSet Chars { get; }
		/// <summary>
		/// Unicode 类别。
		/// </summary>
		public UnicodeCategory[] Categories { get; }
	}

	/// <summary>
	/// Unicode 类别信息。
	/// </summary>
	private static readonly Lazy<UnicodeCategoryInfo[]> UnicodeCategoryInfos = new(() =>
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
		ReadOnlyCharSet lull = UnicodeCategory.UppercaseLetter.GetChars() +
			UnicodeCategory.LowercaseLetter.GetChars();
		// UppercaseLetter & LowercaseLetter
		infos[i++] = new UnicodeCategoryInfo(lull,
			UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter
			);
		// UppercaseLetter & LowercaseLetter & TitlecaseLetter
		infos[i++] = new UnicodeCategoryInfo(lull + UnicodeCategory.TitlecaseLetter.GetChars(),
			UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter, UnicodeCategory.TitlecaseLetter
			);
		ReadOnlyCharSet mn = UnicodeCategory.NonSpacingMark.GetChars();
		ReadOnlyCharSet mnmc = mn + UnicodeCategory.SpacingCombiningMark.GetChars();
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
		infos[i++] = new UnicodeCategoryInfo(UnicodeCategory.OpenPunctuation.GetChars() +
			UnicodeCategory.ClosePunctuation.GetChars(),
			UnicodeCategory.OpenPunctuation, UnicodeCategory.ClosePunctuation
			);
		// 按字符个数从大到小排序。
		Array.Sort(infos, (left, right) => right.Chars.Count - left.Chars.Count);
		return infos;
	});

	/// <summary>
	/// 返回指定字符集合数据的字符串表示。
	/// </summary>
	/// <param name="charSet">要检查的字符集合。</param>
	/// <returns><paramref name="charSet"/> 的字符串表示。</returns>
	public static string ToString(ICharSet charSet)
	{
		// 允许通过 UnicodeCategory 压缩字符串表示形式。
		using ValueList<UnicodeCategory> categories = new(stackalloc UnicodeCategory[30]);
		List<UnicodeCategoryInfo> infos = new(UnicodeCategoryInfos.Value);
		CharSet positive = new(charSet);
		CharSet negated = new();
		bool changed = true;
		while (changed)
		{
			changed = false;
			for (int i = infos.Count - 1; i >= 0; i--)
			{
				UnicodeCategoryInfo info = infos[i];
				IReadOnlySet<char> catChars = info.Chars;
				// 允许最多尝试 10% 的排除字符。
				if (1.1 * positive.Count < catChars.Count)
				{
					// 字符个数不足，后面 Unicode 类别的字符个数更多，可以全部跳过。
					infos.RemoveRange(0, i);
					break;
				}
				if (!positive.Overlaps(catChars))
				{
					infos.RemoveAt(i);
					continue;
				}
				// 确保应用 Unicode 类别后能够减少范围和字符个数。
				// 不满足要求的需要在后面重复检测，避免遗漏。
				CharSet applied = positive ^ catChars;
				if (applied.RangeCount() > positive.RangeCount() || applied.Count > positive.Count)
				{
					continue;
				}
				negated.UnionWith(applied);
				positive.ExceptWith(catChars);
				for (int j = 0; j < info.Categories.Length; j++)
				{
					categories.Add(info.Categories[j]);
				}
				changed = true;
				infos.RemoveAt(i);
			}
		}
		negated.ExceptWith(charSet);

		ValueList<char> builder = new(stackalloc char[ValueList.StackallocCharSizeLimit]);
		builder.Add('[');
		// 输出剩余字符
		PrintChars(positive, ref builder);
		// 输出 Unicode 类别
		if (categories.Length > 0)
		{
			Span<UnicodeCategory> span = categories.AsSpan();
			span.Sort();
			for (int i = 0; i < span.Length; i++)
			{
				builder.Add(@"\p{");
				builder.Add(span[i].GetName());
				builder.Add('}');
			}
		}
		// 输出排除字符
		if (negated.Count > 0)
		{
			builder.Add("-[");
			PrintChars(negated, ref builder);
			builder.Add(']');
		}
		builder.Add(']');
		string result = builder.ToString();
		builder.Dispose();
		return result;
	}

	/// <summary>
	/// 打印指定的字符集合。
	/// </summary>
	/// <param name="set">要打印的字符集合。</param>
	/// <param name="builder">字符串输出。</param>
	private static void PrintChars(CharSet set, ref ValueList<char> builder)
	{
		foreach (var (start, end) in set.Ranges())
		{
			builder.Add(start.UnicodeEscape(false));
			if (start < end)
			{
				if (start + 1 < end)
				{
					builder.Add('-');
				}
				builder.Add(end.UnicodeEscape(false));
			}
		}
	}
}
