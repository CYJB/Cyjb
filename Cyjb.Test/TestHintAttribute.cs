namespace Cyjb.Test;

/// <summary>
/// 表示用于测试的提示特性。
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class TestHintAttribute : Attribute
{
	/// <summary>
	/// 提示的内容。
	/// </summary>
	public readonly string Hint;

	/// <summary>
	/// 使用指定的提示初始化。
	/// </summary>
	/// <param name="hint">提示的内容。</param>
	public TestHintAttribute(string hint)
	{
		Hint = hint;
	}
}
