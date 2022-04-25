namespace Cyjb
{
	/// <summary>
	/// 表示值的范围。
	/// </summary>
	/// <typeparam name="T">值的类型。</typeparam>
	/// <param name="Start">起始值（包含）</param>
	/// <param name="End">结束值（包含）</param>
	public readonly record struct ItemRange<T>(T Start, T End);
}
