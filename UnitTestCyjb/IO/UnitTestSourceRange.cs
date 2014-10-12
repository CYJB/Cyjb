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
		/// 对 <see cref="SourceRange"/> 的属进行测试。
		/// </summary>
		[TestMethod]
		public void TestProperties()
		{
			SourcePosition loc1 = new SourcePosition(0, 1, 1);
			SourcePosition loc2 = new SourcePosition(2, 1, 3);
			Assert.AreEqual(true, SourceRange.Unknown.IsUnknown);
			Assert.AreEqual(true, new SourceRange(SourcePosition.Unknown).IsUnknown);
			Assert.AreEqual(true, new SourceRange(SourcePosition.Unknown, SourcePosition.Unknown).IsUnknown);
			Assert.AreEqual(true, new SourceRange(new SourceRange(SourcePosition.Unknown, SourcePosition.Unknown)).IsUnknown);
			Assert.AreEqual(false, new SourceRange(loc1).IsUnknown);
			Assert.AreEqual(false, SourceRange.Merge(SourceRange.Unknown, new SourceRange(loc1)).IsUnknown);
			Assert.AreEqual(0, SourceRange.Unknown.Length);
			Assert.AreEqual(0, new SourceRange(new SourceRange(SourcePosition.Unknown, SourcePosition.Unknown)).Length);
			Assert.AreEqual(1, new SourceRange(loc1).Length);
			Assert.AreEqual(3, new SourceRange(loc1, loc2).Length);
		}
		/// <summary>
		/// 对 <see cref="SourceRange"/> 的 Contains 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestContains()
		{
			SourcePosition loc1 = new SourcePosition(0, 1, 1);
			SourcePosition loc2 = new SourcePosition(2, 1, 3);
			SourcePosition loc3 = new SourcePosition(5, 1, 6);
			SourcePosition loc4 = new SourcePosition(6, 1, 7);
			SourcePosition loc5 = new SourcePosition(9, 2, 14);
			SourceRange range1 = new SourceRange(loc1, loc2);
			SourceRange range2 = new SourceRange(loc2, loc4);
			SourceRange range3 = new SourceRange(loc1, loc4);
			SourceRange range4 = new SourceRange(loc1, loc5);
			// Contains(ISourceLocatable)
			Assert.AreEqual(false, SourceRange.Unknown.Contains(SourceRange.Unknown));
			Assert.AreEqual(false, SourceRange.Unknown.Contains(range1));
			Assert.AreEqual(true, range1.Contains(range1));
			Assert.AreEqual(false, range1.Contains(range2));
			Assert.AreEqual(false, range1.Contains(SourceRange.Unknown));
			Assert.AreEqual(false, range2.Contains(range3));
			Assert.AreEqual(false, range2.Contains(range4));
			Assert.AreEqual(true, range3.Contains(range2));
			Assert.AreEqual(true, range4.Contains(range2));
			// Contains(SourcePosition)
			Assert.AreEqual(false, range2.Contains(SourcePosition.Unknown));
			Assert.AreEqual(false, range2.Contains(loc1));
			Assert.AreEqual(true, range2.Contains(loc2));
			Assert.AreEqual(true, range2.Contains(loc3));
			Assert.AreEqual(true, range2.Contains(loc4));
			Assert.AreEqual(false, range2.Contains(loc5));
			// Contains(int)
			Assert.AreEqual(false, range2.Contains(loc1.Index));
			Assert.AreEqual(true, range2.Contains(loc2.Index));
			Assert.AreEqual(true, range2.Contains(loc3.Index));
			Assert.AreEqual(true, range2.Contains(loc4.Index));
			Assert.AreEqual(false, range2.Contains(loc5.Index));
			// Contains(int,int)
			Assert.AreEqual(false, range2.Contains(loc1.Line, loc1.Col));
			Assert.AreEqual(true, range2.Contains(loc2.Line, loc2.Col));
			Assert.AreEqual(true, range2.Contains(loc3.Line, loc3.Col));
			Assert.AreEqual(true, range2.Contains(loc4.Line, loc4.Col));
			Assert.AreEqual(false, range2.Contains(loc5.Line, loc5.Col));
		}
		/// <summary>
		/// 对 <see cref="SourceRange"/> 的 Overlaps 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestOverlaps()
		{
			SourcePosition loc1 = new SourcePosition(0, 1, 1);
			SourcePosition loc2 = new SourcePosition(2, 1, 3);
			SourcePosition loc3 = new SourcePosition(5, 1, 6);
			SourcePosition loc4 = new SourcePosition(6, 1, 7);
			SourcePosition loc5 = new SourcePosition(9, 2, 14);
			SourceRange range1 = new SourceRange(loc2, loc4);
			SourceRange range2 = new SourceRange(loc1, loc1);
			SourceRange range3 = new SourceRange(loc1, loc2);
			SourceRange range4 = new SourceRange(loc1, loc3);
			SourceRange range5 = new SourceRange(loc1, loc4);
			SourceRange range6 = new SourceRange(loc1, loc5);
			SourceRange range7 = new SourceRange(loc2, loc2);
			SourceRange range8 = new SourceRange(loc3, loc3);
			Assert.AreEqual(false, SourceRange.Unknown.OverlapsWith(SourceRange.Unknown));
			Assert.AreEqual(SourceRange.Unknown, SourceRange.Unknown.Overlap(SourceRange.Unknown));
			Assert.AreEqual(false, range1.OverlapsWith(SourceRange.Unknown));
			Assert.AreEqual(SourceRange.Unknown, range1.Overlap(SourceRange.Unknown));
			Assert.AreEqual(false, range1.OverlapsWith(range2));
			Assert.AreEqual(SourceRange.Unknown, range1.Overlap(range2));
			Assert.AreEqual(true, range1.OverlapsWith(range3));
			Assert.AreEqual(new SourceRange(loc2), range1.Overlap(range3));
			Assert.AreEqual(true, range1.OverlapsWith(range4));
			Assert.AreEqual(new SourceRange(loc2, loc3), range1.Overlap(range4));
			Assert.AreEqual(true, range1.OverlapsWith(range5));
			Assert.AreEqual(range1, range1.Overlap(range5));
			Assert.AreEqual(true, range1.OverlapsWith(range6));
			Assert.AreEqual(range1, range1.Overlap(range6));
			Assert.AreEqual(true, range1.OverlapsWith(range7));
			Assert.AreEqual(range7, range1.Overlap(range7));
			Assert.AreEqual(true, range1.OverlapsWith(range8));
			Assert.AreEqual(range8, range1.Overlap(range8));
		}
		/// <summary>
		/// 对 <see cref="SourceRange.Merge(ISourceLocatable[])"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestMerge()
		{
			SourcePosition loc1 = new SourcePosition(0, 1, 1);
			SourcePosition loc2 = new SourcePosition(2, 1, 3);
			SourcePosition loc3 = new SourcePosition(5, 2, 6);
			SourcePosition loc4 = new SourcePosition(6, 1, 7);
			SourcePosition loc5 = new SourcePosition(9, 2, 14);
			Assert.AreEqual(SourceRange.Unknown, SourceRange.Merge());
			Assert.AreEqual(SourceRange.Unknown, SourceRange.Merge(null));
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
		}
	}
}
