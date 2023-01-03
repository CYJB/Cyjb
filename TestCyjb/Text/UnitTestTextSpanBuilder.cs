using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="TextSpanBuilder"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestTextSpanBuilder
{
	/// <summary>
	/// 对 <see cref="TextSpanBuilder.Add"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestAdd()
	{
		TextSpanBuilder builder = new();
		Assert.AreEqual(TextSpan.Empty, builder.GetSpan());
		builder.Add(10);
		Assert.AreEqual(new TextSpan(10, 10), builder.GetSpan());
		builder.Add(100);
		Assert.AreEqual(new TextSpan(10, 100), builder.GetSpan());
		builder.Add(2);
		Assert.AreEqual(new TextSpan(2, 100), builder.GetSpan());
		builder.Add(new TextSpan(3, 10));
		Assert.AreEqual(new TextSpan(2, 100), builder.GetSpan());
		builder.Add(new TextSpan(1, 101));
		Assert.AreEqual(new TextSpan(1, 101), builder.GetSpan());
	}
}
