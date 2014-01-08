using Cyjb.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb.IO
{
	/// <summary>
	/// <see cref="SourceRange"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestSourceRange
	{
		/// <summary>
		/// 对 <see cref="SourceRange.Merge"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestMerge()
		{
			SourceLocation loc1 = new SourceLocation(0, 1, 1);
			SourceLocation loc2 = new SourceLocation(2, 1, 3);
			SourceLocation loc3 = new SourceLocation(5, 1, 6);
			SourceLocation loc4 = new SourceLocation(6, 2, 1);
			SourceLocation loc5 = new SourceLocation(9, 2, 4);
			Assert.AreEqual(SourceRange.Unknown, SourceRange.Merge(SourceRange.Unknown));
			Assert.AreEqual(SourceRange.Unknown, SourceRange.Merge(SourceRange.Unknown, SourceRange.Unknown));
			Assert.AreEqual(new SourceRange(loc1), SourceRange.Merge(SourceRange.Unknown, new SourceRange(loc1)));
			Assert.AreEqual(new SourceRange(loc1),
				SourceRange.Merge(SourceRange.Unknown, new SourceRange(loc1), SourceRange.Unknown, new SourceRange(loc1)));
			Assert.AreEqual(new SourceRange(loc1, loc5),
				SourceRange.Merge(new SourceRange(loc3, loc5), SourceRange.Unknown, new SourceRange(loc1)));
			Assert.AreEqual(new SourceRange(loc1, loc5),
				SourceRange.Merge(new SourceRange(loc3, loc5), new SourceRange(loc1, loc5)));
			Assert.AreEqual(new SourceRange(loc1, loc5),
				SourceRange.Merge(new SourceRange(loc4, loc5), new SourceRange(loc1, loc2), new SourceRange(loc2, loc3)));
			Assert.AreEqual(new SourceRange(loc2, loc5),
				SourceRange.Merge(new SourceRange(loc3, loc5), new SourceRange(loc2, loc3)));
			Assert.AreEqual(new SourceRange(loc2, loc5),
				SourceRange.Merge(new SourceRange(loc4, loc5), new SourceRange(loc2, loc3)));
			Assert.AreEqual(new SourceRange(loc1, loc3),
				SourceRange.Merge(new SourceRange(loc1, loc2), new SourceRange(loc2, loc3)));
			Assert.AreEqual(SourceRange.Unknown, SourceRange.Unknown.MergeWith());
			Assert.AreEqual(SourceRange.Unknown, SourceRange.Unknown.MergeWith(SourceRange.Unknown));
			Assert.AreEqual(new SourceRange(loc1), SourceRange.Unknown.MergeWith(new SourceRange(loc1)));
			Assert.AreEqual(new SourceRange(loc1), new SourceRange(loc1).MergeWith(SourceRange.Unknown));
			Assert.AreEqual(new SourceRange(loc1),
				SourceRange.Unknown.MergeWith(new SourceRange(loc1), SourceRange.Unknown, new SourceRange(loc1)));
			Assert.AreEqual(new SourceRange(loc1, loc5),
				new SourceRange(loc3, loc5).MergeWith(SourceRange.Unknown, new SourceRange(loc1)));
			Assert.AreEqual(new SourceRange(loc1, loc5),
				new SourceRange(loc3, loc5).MergeWith(new SourceRange(loc1, loc5)));
			Assert.AreEqual(new SourceRange(loc1, loc5),
				new SourceRange(loc4, loc5).MergeWith(new SourceRange(loc1, loc2), new SourceRange(loc2, loc3)));
			Assert.AreEqual(new SourceRange(loc2, loc5),
				new SourceRange(loc3, loc5).MergeWith(new SourceRange(loc2, loc3)));
			Assert.AreEqual(new SourceRange(loc2, loc5),
				new SourceRange(loc4, loc5).MergeWith(new SourceRange(loc2, loc3)));
			Assert.AreEqual(new SourceRange(loc1, loc3),
				new SourceRange(loc1, loc2).MergeWith(new SourceRange(loc2, loc3)));
		}
	}
}
