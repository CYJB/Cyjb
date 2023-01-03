using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="SortedTextSpanBuilder"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestSortedTextSpanBuilder
{
	/// <summary>
	/// 对 <see cref="SortedTextSpanBuilder.Add"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestAdd()
	{
		SortedTextSpanBuilder builder = new();
		Assert.AreEqual(TextSpan.Empty, builder.GetSpan());
		builder.Add(10);
		Assert.AreEqual(new TextSpan(10, 10), builder.GetSpan());
		builder.Add(100);
		Assert.AreEqual(new TextSpan(10, 100), builder.GetSpan());
		builder.Add(new TextSpan(100, 101));
		Assert.AreEqual(new TextSpan(10, 101), builder.GetSpan());
	}
}
