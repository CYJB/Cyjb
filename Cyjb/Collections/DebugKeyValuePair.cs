using System.Diagnostics;

namespace Cyjb.Collections;

/// <summary>
/// 用于调试视图的键值对。
/// </summary>
[DebuggerDisplay("{Value}", Name = "{Key}")]
internal class DebugKeyValuePair
{
	/// <summary>
	/// 使用指定的键值初始化 <see cref="DebugKeyValuePair"/> 类的新实例。
	/// </summary>
	/// <param name="key">键。</param>
	/// <param name="value">值。</param>
	public DebugKeyValuePair(object key, object value)
	{
		Key = key;
		Value = value;
	}
	/// <summary>
	/// 获取键。
	/// </summary>
	public object Key { get; }
	/// <summary>
	/// 获取值。
	/// </summary>
	public object Value { get; }
}
