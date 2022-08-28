using System;
using System.Globalization;
using Cyjb.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Globalization;

/// <summary>
/// <see cref="UnicodeCategoryUtil"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestUnicodeCategoryUtil
{
	/// <summary>
	/// 对 <see cref="UnicodeCategoryUtil.GetName"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestGetName()
	{
		Assert.AreEqual("Pe", UnicodeCategory.ClosePunctuation.GetName());
		Assert.AreEqual("Pc", UnicodeCategory.ConnectorPunctuation.GetName());
		Assert.AreEqual("Cc", UnicodeCategory.Control.GetName());
		Assert.AreEqual("Sc", UnicodeCategory.CurrencySymbol.GetName());
		Assert.AreEqual("Pd", UnicodeCategory.DashPunctuation.GetName());
		Assert.AreEqual("Nd", UnicodeCategory.DecimalDigitNumber.GetName());
		Assert.AreEqual("Me", UnicodeCategory.EnclosingMark.GetName());
		Assert.AreEqual("Pf", UnicodeCategory.FinalQuotePunctuation.GetName());
		Assert.AreEqual("Cf", UnicodeCategory.Format.GetName());
		Assert.AreEqual("Pi", UnicodeCategory.InitialQuotePunctuation.GetName());
		Assert.AreEqual("Nl", UnicodeCategory.LetterNumber.GetName());
		Assert.AreEqual("Zl", UnicodeCategory.LineSeparator.GetName());
		Assert.AreEqual("Ll", UnicodeCategory.LowercaseLetter.GetName());
		Assert.AreEqual("Sm", UnicodeCategory.MathSymbol.GetName());
		Assert.AreEqual("Lm", UnicodeCategory.ModifierLetter.GetName());
		Assert.AreEqual("Sk", UnicodeCategory.ModifierSymbol.GetName());
		Assert.AreEqual("Mn", UnicodeCategory.NonSpacingMark.GetName());
		Assert.AreEqual("Ps", UnicodeCategory.OpenPunctuation.GetName());
		Assert.AreEqual("Lo", UnicodeCategory.OtherLetter.GetName());
		Assert.AreEqual("Cn", UnicodeCategory.OtherNotAssigned.GetName());
		Assert.AreEqual("No", UnicodeCategory.OtherNumber.GetName());
		Assert.AreEqual("Po", UnicodeCategory.OtherPunctuation.GetName());
		Assert.AreEqual("So", UnicodeCategory.OtherSymbol.GetName());
		Assert.AreEqual("Zp", UnicodeCategory.ParagraphSeparator.GetName());
		Assert.AreEqual("Co", UnicodeCategory.PrivateUse.GetName());
		Assert.AreEqual("Zs", UnicodeCategory.SpaceSeparator.GetName());
		Assert.AreEqual("Mc", UnicodeCategory.SpacingCombiningMark.GetName());
		Assert.AreEqual("Cs", UnicodeCategory.Surrogate.GetName());
		Assert.AreEqual("Lt", UnicodeCategory.TitlecaseLetter.GetName());
		Assert.AreEqual("Lu", UnicodeCategory.UppercaseLetter.GetName());
	}

	/// <summary>
	/// 对 <see cref="UnicodeCategoryUtil.GetCategory"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestGetCategory()
	{
		Assert.AreEqual(UnicodeCategory.ClosePunctuation, UnicodeCategoryUtil.GetCategory("Pe"));
		Assert.AreEqual(UnicodeCategory.ConnectorPunctuation, UnicodeCategoryUtil.GetCategory("Pc"));
		Assert.AreEqual(UnicodeCategory.Control, UnicodeCategoryUtil.GetCategory("Cc"));
		Assert.AreEqual(UnicodeCategory.CurrencySymbol, UnicodeCategoryUtil.GetCategory("Sc"));
		Assert.AreEqual(UnicodeCategory.DashPunctuation, UnicodeCategoryUtil.GetCategory("Pd"));
		Assert.AreEqual(UnicodeCategory.DecimalDigitNumber, UnicodeCategoryUtil.GetCategory("Nd"));
		Assert.AreEqual(UnicodeCategory.EnclosingMark, UnicodeCategoryUtil.GetCategory("Me"));
		Assert.AreEqual(UnicodeCategory.FinalQuotePunctuation, UnicodeCategoryUtil.GetCategory("Pf"));
		Assert.AreEqual(UnicodeCategory.Format, UnicodeCategoryUtil.GetCategory("Cf"));
		Assert.AreEqual(UnicodeCategory.InitialQuotePunctuation, UnicodeCategoryUtil.GetCategory("Pi"));
		Assert.AreEqual(UnicodeCategory.LetterNumber, UnicodeCategoryUtil.GetCategory("Nl"));
		Assert.AreEqual(UnicodeCategory.LineSeparator, UnicodeCategoryUtil.GetCategory("Zl"));
		Assert.AreEqual(UnicodeCategory.LowercaseLetter, UnicodeCategoryUtil.GetCategory("Ll"));
		Assert.AreEqual(UnicodeCategory.MathSymbol, UnicodeCategoryUtil.GetCategory("Sm"));
		Assert.AreEqual(UnicodeCategory.ModifierLetter, UnicodeCategoryUtil.GetCategory("Lm"));
		Assert.AreEqual(UnicodeCategory.ModifierSymbol, UnicodeCategoryUtil.GetCategory("Sk"));
		Assert.AreEqual(UnicodeCategory.NonSpacingMark, UnicodeCategoryUtil.GetCategory("Mn"));
		Assert.AreEqual(UnicodeCategory.OpenPunctuation, UnicodeCategoryUtil.GetCategory("Ps"));
		Assert.AreEqual(UnicodeCategory.OtherLetter, UnicodeCategoryUtil.GetCategory("Lo"));
		Assert.AreEqual(UnicodeCategory.OtherNotAssigned, UnicodeCategoryUtil.GetCategory("Cn"));
		Assert.AreEqual(UnicodeCategory.OtherNumber, UnicodeCategoryUtil.GetCategory("No"));
		Assert.AreEqual(UnicodeCategory.OtherPunctuation, UnicodeCategoryUtil.GetCategory("Po"));
		Assert.AreEqual(UnicodeCategory.OtherSymbol, UnicodeCategoryUtil.GetCategory("So"));
		Assert.AreEqual(UnicodeCategory.ParagraphSeparator, UnicodeCategoryUtil.GetCategory("Zp"));
		Assert.AreEqual(UnicodeCategory.PrivateUse, UnicodeCategoryUtil.GetCategory("Co"));
		Assert.AreEqual(UnicodeCategory.SpaceSeparator, UnicodeCategoryUtil.GetCategory("Zs"));
		Assert.AreEqual(UnicodeCategory.SpacingCombiningMark, UnicodeCategoryUtil.GetCategory("Mc"));
		Assert.AreEqual(UnicodeCategory.Surrogate, UnicodeCategoryUtil.GetCategory("Cs"));
		Assert.AreEqual(UnicodeCategory.TitlecaseLetter, UnicodeCategoryUtil.GetCategory("Lt"));
		Assert.AreEqual(UnicodeCategory.UppercaseLetter, UnicodeCategoryUtil.GetCategory("Lu"));
		Assert.ThrowsException<ArgumentException>(() => UnicodeCategoryUtil.GetCategory("Other"));
	}

	/// <summary>
	/// 对 <see cref="UnicodeCategoryUtil.GetChars"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestGetChars()
	{
		int count = 0;
		foreach (UnicodeCategory category in Enum.GetValues<UnicodeCategory>())
		{
			foreach (char ch in category.GetChars())
			{
				Assert.AreEqual(category, char.GetUnicodeCategory(ch));
				count++;
			}
		}
		Assert.AreEqual(65536, count);
	}
}
