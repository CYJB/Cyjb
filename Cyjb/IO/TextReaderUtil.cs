namespace Cyjb.IO;

/// <summary>
/// 提供 <see cref="TextReader"/> 类的扩展方法。
/// </summary>
public static class TextReaderUtil
{
	/// <summary>
	/// 组合指定的多个文本读取器，返回按顺序依次读取的新文本读取器。
	/// </summary>
	/// <param name="sources">要组合的源文本读取器或字符串。</param>
	/// <returns>组合后的文本读取器。</returns>
	public static TextReader Combine(params Variant<TextReader, string>[] sources)
	{
		if (sources == null || sources.Length == 0)
		{
			return TextReader.Null;
		}
		if (sources.Length == 1)
		{
			return GetTextReader(sources[0]);
		}
		return new CombinedTextReader(sources.Select(GetTextReader));
	}

	/// <summary>
	/// 返回指定源相关的文本读取器。
	/// </summary>
	/// <param name="source">文本读取器或字符串。</param>
	/// <returns>文本读取器。</returns>
	private static TextReader GetTextReader(Variant<TextReader, string> source)
	{
		if (source.TryGetValue(out TextReader? reader))
		{
			return reader;
		}
		else
		{
			return new StringReader((string)source);
		}
	}
}
