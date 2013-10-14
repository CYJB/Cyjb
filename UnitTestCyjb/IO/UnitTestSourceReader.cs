using System.IO;
using Cyjb.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb.IO
{
	/// <summary>
	/// 表示 <see cref="SourceReader"/> 类的相关扩展方法。
	/// </summary>
	[TestClass]
	public class UnitTestSourceReader
	{
		/// <summary>
		/// 测试 <see cref="SourceReader"/> 的相关方法。
		/// </summary>
		[TestMethod]
		public void TestSourceReader()
		{
			SourceReader reader = new SourceReader(new StringReader("1234567890"));
			Assert.AreEqual((int)'1', reader.Read());
			Assert.AreEqual((int)'2', reader.Read());
			Assert.AreEqual("12", reader.ReadedBlock());
			Assert.AreEqual(true, reader.Unget());
			Assert.AreEqual(true, reader.Unget());
			Assert.AreEqual(false, reader.Unget());
			Assert.AreEqual((int)'2', reader.Read(1));
			Assert.AreEqual("12", reader.ReadedBlock());
			Assert.AreEqual(true, reader.Unget());
			Assert.AreEqual("1", reader.ReadedBlock());
			Assert.AreEqual((int)'3', reader.Read(1));
			Assert.AreEqual("123", reader.ReadedBlock());
			Assert.AreEqual("123", reader.Accept());
			Assert.AreEqual("", reader.ReadedBlock());
			Assert.AreEqual("", reader.Accept());
			Assert.AreEqual(false, reader.Unget());
			Assert.AreEqual((int)'6', reader.Read(2));
			Assert.AreEqual("456", reader.ReadedBlock());
		}
	}
}
