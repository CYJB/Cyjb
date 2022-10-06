namespace Cyjb.Collections;

/// <summary>
/// 基于树状位压缩数组的字符的有序集合。
/// </summary>
internal interface ICharSet : ICollection<char>
{
	/// <summary>
	/// 字符集合的顶层数组。
	/// </summary>
	CharSetItem[] Data { get; }
}
