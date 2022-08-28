using System.Diagnostics;

namespace Cyjb.Collections;

/// <summary>
/// 为 <see cref="CharSet"/> 类提供调试视图。
/// </summary>
internal sealed class CharSetDebugView
{
	/// <summary>
	/// 调试视图的源集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly CharSet source;

	/// <summary>
	/// 使用指定的源集合初始化 <see cref="CharSet"/> 类的实例。
	/// </summary>
	/// <param name="sourceCollection">使用调试视图的源集合。</param>
	public CharSetDebugView(CharSet sourceCollection)
	{
		source = sourceCollection;
	}

	/// <summary>
	/// 获取源集合中的所有项。
	/// </summary>
	/// <value>包含了源集合中的所有项的数组。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public DebugKeyValuePair[] Keys
	{
		get
		{
			List<DebugKeyValuePair> result = new();
			int index = 0;
			foreach (ValueRange<char> range in source.Ranges())
			{
				int count = range.End - range.Start;
				string key = count == 0 ? $"[{index}]" : $"[{index}..{index + count}]";
				result.Add(new DebugKeyValuePair(key, range.ToString()));
				index += count + 1;
			}
			return result.ToArray();
		}
	}
}

