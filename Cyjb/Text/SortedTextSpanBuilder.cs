namespace Cyjb.Text;

/// <summary>
/// 从有序索引序列中构造可以覆盖所有序列值的 <see cref="TextSpan"/>。
/// </summary>
/// <remarks>假设索引序列会按照从小到大的顺序添加。</remarks>
public sealed class SortedTextSpanBuilder
{
	/// <summary>
	/// 起始索引。
	/// </summary>
	private int start = -1;
	/// <summary>
	/// 结束索引。
	/// </summary>
	private int end;

	/// <summary>
	/// 添加指定的索引。
	/// </summary>
	/// <param name="index">要添加的索引。</param>
	public void Add(int index)
	{
		if (start < 0)
		{
			start = index;
		}
		end = index;
	}

	/// <summary>
	/// 添加指定的文本范围。
	/// </summary>
	/// <param name="span">要添加的文本范围。</param>
	public void Add(TextSpan span)
	{
		if (start < 0)
		{
			start = span.Start;
		}
		end = span.End;
	}

	/// <summary>
	/// 返回当前已覆盖的文本范围。
	/// </summary>
	/// <returns>当前已覆盖的文本范围。</returns>
	public TextSpan GetSpan()
	{
		if (start < 0)
		{
			return new TextSpan();
		}
		return new TextSpan(start, end);
	}
}
