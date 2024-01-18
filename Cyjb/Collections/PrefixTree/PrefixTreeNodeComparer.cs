namespace Cyjb.Collections;

/// <summary>
/// 前缀树的节点比较器，使用逆序比较来令较大的字符排在前面。
/// </summary>
internal sealed class PrefixTreeNodeComparer : IComparer<int>
{
	/// <summary>
	/// 比较器实例。
	/// </summary>
	public static IComparer<int> Instance = new PrefixTreeNodeComparer();

	/// <summary>
	/// 比较指定的两个字符。
	/// </summary>
	/// <param name="x">要比较的第一个字符。</param>
	/// <param name="y">要比较的第二个字符。</param>
	/// <returns>两个对象的相对顺序，会将较大的字符排在前面。</returns>
	/// <exception cref="NotImplementedException"></exception>
	public int Compare(int x, int y)
	{
		return y - x;
	}
}
