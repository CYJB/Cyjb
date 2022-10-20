namespace Cyjb.Text;

/// <summary>
/// 提供对 <see cref="LineLocator"/> 的扩展方法。
/// </summary>
public static class LineLocatorUtil
{
	/// <summary>
	/// 返回指定文本范围的行位置范围。
	/// </summary>
	/// <param name="locator">行定位器。</param>
	/// <param name="span">要转换的文本范围。</param>
	/// <returns>指定文本范围对应的行位置范围。</returns>
	public static LinePositionSpan GetSpan(this LineLocator? locator, TextSpan span)
	{
		int start = span.Start;
		int end = span.End;
		if (locator == null)
		{
			return new LinePositionSpan(new LinePosition(1, start), new LinePosition(1, end));
		}
		else
		{
			return new LinePositionSpan(locator.GetPosition(start), locator.GetPosition(end));
		}
	}
}
