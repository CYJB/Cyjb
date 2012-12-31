using System;
using System.Reflection;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.EnumExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestEnumExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.EnumExt.GetDescription"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestGetDescription()
		{
			for (int i = 0; i < 129; i++)
			{
				BindingFlags value = (BindingFlags)i;
				Assert.AreEqual(value.ToString(), value.GetDescription());
			}
			Assert.AreEqual("0", ((TestEnum)0).GetDescription());
			Assert.AreEqual("A Des", TestEnum.A.GetDescription());
			Assert.AreEqual("B Des", TestEnum.B.GetDescription());
			Assert.AreEqual("C Des", TestEnum.C.GetDescription());
			Assert.AreEqual("D Des", TestEnum.D.GetDescription());
			Assert.AreEqual("E", TestEnum.E.GetDescription());
			Assert.AreEqual("0", ((TestEnum2)0).GetDescription());
			Assert.AreEqual("A Des", TestEnum2.A.GetDescription());
			Assert.AreEqual("B Des", TestEnum2.B.GetDescription());
			Assert.AreEqual("C Des", TestEnum2.C.GetDescription());
			Assert.AreEqual("D", TestEnum2.D.GetDescription());
			Assert.AreEqual("AB Des", TestEnum2.AB.GetDescription());
			Assert.AreEqual("BC Des", TestEnum2.BC.GetDescription());
			Assert.AreEqual("A Des, BC Des", (TestEnum2.BC | TestEnum2.A).GetDescription());
			Assert.AreEqual("BC Des, D", (TestEnum2.BC | TestEnum2.D).GetDescription());
			Assert.AreEqual("All Des", TestEnum2.All.GetDescription());
			Assert.AreEqual("All Des, 128", (TestEnum2.All | (TestEnum2)128).GetDescription());
		}
		/// <summary>
		/// 对 <see cref="Cyjb.EnumExt.ParseEx"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestParseEx()
		{
			for (int i = 0; i < 150; i++)
			{
				BindingFlags value = (BindingFlags)i;
				Assert.AreEqual(value, EnumExt.ParseEx<BindingFlags>(value.ToString()));
				Assert.AreEqual(value, EnumExt.ParseEx<BindingFlags>(value.GetDescription()));
			}
			Assert.AreEqual((TestEnum)0, EnumExt.ParseEx<TestEnum>("  0"));
			Assert.AreEqual(TestEnum.A, EnumExt.ParseEx<TestEnum>("A "));
			Assert.AreEqual(TestEnum.A, EnumExt.ParseEx<TestEnum>("-1 "));
			Assert.AreEqual(TestEnum.A, EnumExt.ParseEx<TestEnum>("A Des"));
			Assert.AreEqual(TestEnum.B, EnumExt.ParseEx<TestEnum>(" B "));
			Assert.AreEqual(TestEnum.B, EnumExt.ParseEx<TestEnum>(" -2 "));
			Assert.AreEqual(TestEnum.B, EnumExt.ParseEx<TestEnum>("B Des"));
			Assert.AreEqual(TestEnum.C, EnumExt.ParseEx<TestEnum>("C"));
			Assert.AreEqual(TestEnum.C, EnumExt.ParseEx<TestEnum>("-3"));
			Assert.AreEqual(TestEnum.C, EnumExt.ParseEx<TestEnum>("C Des"));
			Assert.AreEqual(TestEnum.D, EnumExt.ParseEx<TestEnum>("D"));
			Assert.AreEqual(TestEnum.D, EnumExt.ParseEx<TestEnum>("-4"));
			Assert.AreEqual(TestEnum.D, EnumExt.ParseEx<TestEnum>("D Des"));
			Assert.AreEqual(TestEnum.E, EnumExt.ParseEx<TestEnum>("-5"));
			Assert.AreEqual(TestEnum.E, EnumExt.ParseEx<TestEnum>("E"));
			Assert.AreEqual((TestEnum2)0, EnumExt.ParseEx<TestEnum2>("0"));
			Assert.AreEqual(TestEnum2.A, EnumExt.ParseEx<TestEnum2>("  A "));
			Assert.AreEqual(TestEnum2.A, EnumExt.ParseEx<TestEnum2>("  1 "));
			Assert.AreEqual(TestEnum2.A, EnumExt.ParseEx<TestEnum2>("A Des"));
			Assert.AreEqual(TestEnum2.B, EnumExt.ParseEx<TestEnum2>("B "));
			Assert.AreEqual(TestEnum2.B, EnumExt.ParseEx<TestEnum2>(" +2 "));
			Assert.AreEqual(TestEnum2.B, EnumExt.ParseEx<TestEnum2>("B Des"));
			Assert.AreEqual(TestEnum2.C, EnumExt.ParseEx<TestEnum2>("C "));
			Assert.AreEqual(TestEnum2.C, EnumExt.ParseEx<TestEnum2>("   +4   "));
			Assert.AreEqual(TestEnum2.C, EnumExt.ParseEx<TestEnum2>("C Des"));
			Assert.AreEqual(TestEnum2.D, EnumExt.ParseEx<TestEnum2>("D "));
			Assert.AreEqual(TestEnum2.D, EnumExt.ParseEx<TestEnum2>("0008"));
			Assert.AreEqual(TestEnum2.D, EnumExt.ParseEx<TestEnum2>("D"));
			Assert.AreEqual(TestEnum2.AB, EnumExt.ParseEx<TestEnum2>("AB Des"));
			Assert.AreEqual(TestEnum2.AB, EnumExt.ParseEx<TestEnum2>("A,B Des"));
			Assert.AreEqual(TestEnum2.AB, EnumExt.ParseEx<TestEnum2>("1,2"));
			Assert.AreEqual(TestEnum2.AB, EnumExt.ParseEx<TestEnum2>("3"));
			Assert.AreEqual(TestEnum2.AB, EnumExt.ParseEx<TestEnum2>("A Des    ,   B Des"));
			Assert.AreEqual(TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("BC Des    "));
			Assert.AreEqual(TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("2, 4, 2, 4,C Des    "));
			Assert.AreEqual(TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("C Des    ,   B Des"));
			Assert.AreEqual(TestEnum2.A | TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("C Des    ,   B Des, A Des"));
			Assert.AreEqual(TestEnum2.A | TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("4    ,   B Des, A Des, 3"));
			Assert.AreEqual(TestEnum2.A | TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("7"));
			Assert.AreEqual(TestEnum2.A | TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("BC Des    ,   , A Des"));
			Assert.AreEqual(TestEnum2.A | TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("AB Des    ,    C Des"));
			Assert.AreEqual(TestEnum2.D | TestEnum2.BC, EnumExt.ParseEx<TestEnum2>("BC Des    ,    D"));
			Assert.AreEqual(TestEnum2.All, EnumExt.ParseEx<TestEnum2>("BC Des,A Des,D"));
			Assert.AreEqual(TestEnum2.All, EnumExt.ParseEx<TestEnum2>("15"));
			Assert.AreEqual(TestEnum2.All, EnumExt.ParseEx<TestEnum2>("+1,+2,  , 4, 8, "));
			Assert.AreEqual(TestEnum2.All, EnumExt.ParseEx<TestEnum2>("All Des"));
			Assert.AreEqual(TestEnum2.All, EnumExt.ParseEx<TestEnum2>("BC Des,AB Des,D"));
			Assert.AreEqual(TestEnum2.All | (TestEnum2)128, EnumExt.ParseEx<TestEnum2>("BC Des,AB Des,D, 128"));
			Assert.AreEqual(TestEnum2.All | (TestEnum2)128, EnumExt.ParseEx<TestEnum2>("15, 128"));
			Assert.AreEqual(TestEnum2.All | (TestEnum2)128, EnumExt.ParseEx<TestEnum2>("143"));
			Assert.AreEqual(TestEnum2.All | (TestEnum2)128, EnumExt.ParseEx<TestEnum2>("BC Des,AB Des,D,+128"));
		}
		private enum TestEnum
		{
			[System.ComponentModel.Description("A Des")]
			A = -1,
			[System.ComponentModel.Description("B Des")]
			B = -2,
			[System.ComponentModel.Description("C Des")]
			C = -3,
			[System.ComponentModel.Description("D Des")]
			D = -4,
			E = -5
		}
		[Flags]
		private enum TestEnum2
		{
			[System.ComponentModel.Description("A Des")]
			A = 1,
			[System.ComponentModel.Description("B Des")]
			B = 2,
			[System.ComponentModel.Description("C Des")]
			C = 4,
			D = 8,
			[System.ComponentModel.Description("AB Des")]
			AB = A | B,
			[System.ComponentModel.Description("BC Des")]
			BC = C | B,
			[System.ComponentModel.Description("All Des")]
			All = A | B | C | D
		}
	}
}
