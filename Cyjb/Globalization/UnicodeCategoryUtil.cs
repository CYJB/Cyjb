using System.Globalization;
using Cyjb.Collections;

namespace Cyjb.Globalization;

/// <summary>
/// 提供 <see cref="UnicodeCategory"/> 的扩展方法。
/// </summary>
public static class UnicodeCategoryUtil
{
	/// <summary>
	/// Unicode 类别的名称。
	/// </summary>
	private static readonly string[] Names = new[] {
		"Lu", "Ll", "Lt", "Lm", "Lo", "Mn", "Mc", "Me", "Nd", "Nl", "No", "Zs", "Zl", "Zp", "Cc",
		"Cf", "Cs", "Co", "Pc", "Pd", "Ps", "Pe", "Pi", "Pf", "Po", "Sm", "Sc", "Sk", "So", "Cn",
	};

	/// <summary>
	/// Unicode 类别对应的字符集合。
	/// </summary>
	private static readonly Lazy<Dictionary<UnicodeCategory, ReadOnlyCharSet>> charSets = new(() =>
	{
		UnicodeCategory[] categories = Enum.GetValues<UnicodeCategory>();
		Dictionary<UnicodeCategory, CharSet> map = new(categories.Length);
		foreach (UnicodeCategory category in categories)
		{
			map[category] = new CharSet();
		}
		for (char ch = '\0'; ch < char.MaxValue; ch++)
		{
			map[char.GetUnicodeCategory(ch)].Add(ch);
		}
		map[char.GetUnicodeCategory(char.MaxValue)].Add(char.MaxValue);
		return new Dictionary<UnicodeCategory, ReadOnlyCharSet>(map.Select(pair =>
		{
			return new KeyValuePair<UnicodeCategory, ReadOnlyCharSet>(pair.Key, pair.Value.MoveReadOnly());
		}));
	});

	/// <summary>
	/// 返回当前 UnicodeCategory 的名称（Lu、Ll 等）。
	/// </summary>
	/// <param name="category">当前 UnicodeCategory。</param>
	/// <returns>当前 UnicodeCategory 的名称。</returns>
	public static string GetName(this UnicodeCategory category)
	{
		return Names[(int)category];
	}

	/// <summary>
	/// 返回指定名称对应的 UnicodeCategory。
	/// </summary>
	/// <param name="name">Unicode 类别名称。</param>
	/// <returns>指定名称的 UnicodeCategory。</returns>
	public static UnicodeCategory GetCategory(string name)
	{
		return name switch
		{
			"Lu" => UnicodeCategory.UppercaseLetter,
			"Ll" => UnicodeCategory.LowercaseLetter,
			"Lt" => UnicodeCategory.TitlecaseLetter,
			"Lm" => UnicodeCategory.ModifierLetter,
			"Lo" => UnicodeCategory.OtherLetter,
			"Mn" => UnicodeCategory.NonSpacingMark,
			"Mc" => UnicodeCategory.SpacingCombiningMark,
			"Me" => UnicodeCategory.EnclosingMark,
			"Nd" => UnicodeCategory.DecimalDigitNumber,
			"Nl" => UnicodeCategory.LetterNumber,
			"No" => UnicodeCategory.OtherNumber,
			"Zs" => UnicodeCategory.SpaceSeparator,
			"Zl" => UnicodeCategory.LineSeparator,
			"Zp" => UnicodeCategory.ParagraphSeparator,
			"Cc" => UnicodeCategory.Control,
			"Cf" => UnicodeCategory.Format,
			"Cs" => UnicodeCategory.Surrogate,
			"Co" => UnicodeCategory.PrivateUse,
			"Cn" => UnicodeCategory.OtherNotAssigned,
			"Pc" => UnicodeCategory.ConnectorPunctuation,
			"Pd" => UnicodeCategory.DashPunctuation,
			"Ps" => UnicodeCategory.OpenPunctuation,
			"Pe" => UnicodeCategory.ClosePunctuation,
			"Pi" => UnicodeCategory.InitialQuotePunctuation,
			"Pf" => UnicodeCategory.FinalQuotePunctuation,
			"Po" => UnicodeCategory.OtherPunctuation,
			"Sm" => UnicodeCategory.MathSymbol,
			"Sc" => UnicodeCategory.CurrencySymbol,
			"Sk" => UnicodeCategory.ModifierSymbol,
			"So" => UnicodeCategory.OtherSymbol,
			_ => throw new ArgumentException(Resources.InvalidUnicodeCategoryName, nameof(name)),
		};
	}

	/// <summary>
	/// 返回当前 UnicodeCategory 包含的全部字符。
	/// </summary>
	/// <param name="category">当前 UnicodeCategory。</param>
	/// <returns>当前 UnicodeCategory 包含的全部字符。</returns>
	public static ReadOnlyCharSet GetChars(this UnicodeCategory category)
	{
		return charSets.Value[category];
	}
}
