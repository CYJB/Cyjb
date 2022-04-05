using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text
{
	/// <summary>
	/// <see cref="Token{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestToken
	{
		/// <summary>
		/// 对 <see cref="Token{T}"/> 的构造函数进行测试。
		/// </summary>
		[TestMethod]
		public void TestToken()
		{
			Token<int> token = new(10, "test", new TextSpan(10, 30));
			Assert.AreEqual(10, token.Kind);
			Assert.AreEqual("test", token.Text);
			Assert.AreEqual(new TextSpan(10, 30), token.Span);
			Assert.IsNull(token.Value);

			token = new(11, "test2", new TextSpan(20, 40), "test2");
			Assert.AreEqual(11, token.Kind);
			Assert.AreEqual("test2", token.Text);
			Assert.AreEqual(new TextSpan(20, 40), token.Span);
			Assert.AreEqual("test2", token.Value);
		}
	}
}
