using System;
using System.IO;
using Cyjb.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.IO;

/// <summary>
/// <see cref="TextReaderUtil"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestTextReaderUtil
{
	/// <summary>
	/// 对 <see cref="TextReaderUtil.Combine"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestCombine()
	{
		TextReader reader = TextReaderUtil.Combine("abc", "123", new StringReader("456"));
		Assert.AreEqual('a', reader.Peek());
		Assert.AreEqual('a', reader.Peek());
		Assert.AreEqual('a', reader.Read());
		Assert.AreEqual('b', reader.Read());
		Assert.AreEqual('c', reader.Read());
		Assert.AreEqual('1', reader.Peek());
		Assert.AreEqual('1', reader.Read());
		Assert.AreEqual("23456", reader.ReadToEnd());
		Assert.AreEqual(-1, reader.Peek());
		Assert.AreEqual(-1, reader.Read());
		Assert.AreEqual(-1, reader.Peek());
		Assert.AreEqual(-1, reader.Read());

		reader.Dispose();

		reader = TextReaderUtil.Combine("abc", "123", new StringReader("456"));
		char[] buffer = new char[5];
		Assert.AreEqual(3, reader.Read(buffer, 0, 5));
		CollectionAssert.AreEqual("abc\0\0".ToCharArray(), buffer);
		Assert.AreEqual(4, reader.ReadBlock(buffer.AsSpan(1, 4)));
		CollectionAssert.AreEqual("a1234".ToCharArray(), buffer);
		Assert.AreEqual(2, reader.ReadBlock(buffer, 0, 5));
		CollectionAssert.AreEqual("56234".ToCharArray(), buffer);
		Assert.AreEqual(0, reader.ReadBlock(buffer, 0, 5));
		Assert.AreEqual(0, reader.ReadBlock(buffer.AsSpan(0, 5)));
	}
}
