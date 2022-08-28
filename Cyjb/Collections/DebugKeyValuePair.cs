using System.Diagnostics;

namespace Cyjb.Collections;

/// <summary>
/// 用于调试视图的键值对。
/// </summary>
[DebuggerDisplay("{Value}", Name = "{Key}")]
internal class DebugKeyValuePair
{
	public DebugKeyValuePair(object key, object value)
	{
		Key = key;
		Value = value;
	}
	public object Key { get; }
	public object Value { get; }
}
