using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.CharExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestCharExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.CharExt.IsHex"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsHex()
		{
			for (char c = '\x00'; c < '\xFF'; c++)
			{
				switch (c)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						Assert.IsTrue(c.IsHex(), "{0} 应当不是 Hex。", c);
						break;
					default:
						Assert.IsFalse(c.IsHex(), "{0} 应当是 Hex。", c);
						break;
				}
			}
		}
	}
}
