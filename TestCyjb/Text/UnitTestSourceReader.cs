using System;
using System.IO;
using System.Text;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text
{
	/// <summary>
	/// <see cref="SourceReader"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestSourceReader
	{
		/// <summary>
		/// 对 <see cref="SourceReader"/> 读取短文本进行测试。
		/// </summary>
		[TestMethod]
		public void TestShortText()
		{
			SourceReader reader = new(new StringReader("1234567890"));
			Assert.AreEqual('1', reader.Peek());
			Assert.AreEqual('1', reader.Peek());
			Assert.AreEqual('1', reader.Read());
			Assert.AreEqual('2', reader.Peek());
			Assert.AreEqual('2', reader.Read());
			Assert.AreEqual("12", reader.ReadedText());
			Assert.IsTrue(reader.Unget());
			Assert.IsTrue(reader.Unget());
			Assert.IsFalse(reader.Unget());

			Assert.AreEqual('2', reader.Read(1));
			Assert.AreEqual("12", reader.ReadedText());
			Assert.AreEqual(2, reader.Unget(5));

			Assert.AreEqual("", reader.ReadedText());
			Assert.AreEqual('3', reader.Read(2));
			Assert.AreEqual("123", reader.ReadedText());
			Assert.AreEqual("123", reader.Accept());

			Assert.AreEqual("", reader.ReadedText());
			Assert.AreEqual("", reader.Accept());
			Assert.IsFalse(reader.Unget());
			Assert.AreEqual(0, reader.Unget(5));

			Assert.AreEqual('6', reader.Read(2));
			Assert.AreEqual("456", reader.ReadedText());
			reader.Drop();

			Assert.AreEqual(-1, reader.Read(10));
			Assert.AreEqual("7890", reader.ReadedText());
			Token<int> token = reader.AcceptToken(11);
			Assert.AreEqual(11, token.Kind);
			Assert.AreEqual("7890", token.Text);
			Assert.AreEqual(new TextSpan(6, 10), token.Span);
		}

		/// <summary>
		/// 对 <see cref="SourceReader"/> 读取长文本进行测试。
		/// </summary>
		[TestMethod]
		public void TestLongText()
		{
			StringBuilder builder = new(2813);
			for (int i = 0; i < 2813; i++)
			{
				builder.Append((char)Random.Shared.Next(char.MaxValue));
			}
			string text = builder.ToString();
			SourceReader reader = new(new StringReader(text));

			Assert.AreEqual(text[0], reader.Peek());
			Assert.AreEqual(text[0], reader.Peek());
			Assert.AreEqual(text[0], reader.Read());
			Assert.AreEqual(text[1], reader.Peek());
			Assert.AreEqual(text[1], reader.Read());
			Assert.AreEqual(text[..2], reader.ReadedText());
			Assert.IsTrue(reader.Unget());
			Assert.IsTrue(reader.Unget());
			Assert.IsFalse(reader.Unget());

			Assert.AreEqual(text[521], reader.Read(521));
			Assert.AreEqual(text[..522], reader.ReadedText());
			Assert.AreEqual(522, reader.Unget(530));

			Assert.AreEqual("", reader.ReadedText());
			Assert.AreEqual(text[1482], reader.Read(1482));
			Assert.AreEqual(text[..1483], reader.ReadedText());
			Assert.AreEqual(text[..1483], reader.Accept());

			Assert.AreEqual("", reader.ReadedText());
			Assert.AreEqual("", reader.Accept());
			Assert.IsFalse(reader.Unget());
			Assert.AreEqual(0, reader.Unget(5));

			Assert.AreEqual(text[1483 + 561], reader.Read(561));
			Assert.AreEqual(text[1483..(1483 + 561 + 1)], reader.ReadedText());
			reader.Drop();

			Assert.AreEqual(-1, reader.Read(4000));
			Assert.AreEqual(text[(1483 + 561 + 1)..], reader.ReadedText());
			Token<int> token = reader.AcceptToken(11);
			Assert.AreEqual(11, token.Kind);
			Assert.AreEqual(text[(1483 + 561 + 1)..], token.Text);
			Assert.AreEqual(new TextSpan(1483 + 561 + 1, text.Length), token.Span);
		}
	}
}
