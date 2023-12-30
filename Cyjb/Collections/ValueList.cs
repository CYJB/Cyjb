namespace Cyjb.Collections;

/// <summary>
/// 允许在栈上分配的列表。
/// </summary>
public static class ValueList
{
	/// <summary>
	/// 堆栈分配的 <see cref="char"/> 缓冲大小，值为 256，占用空间为 512B。
	/// </summary>
	public const int StackallocCharSizeLimit = 256;
	/// <summary>
	/// 堆栈分配的 <see cref="int"/> 缓冲大小，值为 128，占用空间为 512B。
	/// </summary>
	public const int StackallocIntSizeLimit = 128;
	/// <summary>
	/// 堆栈分配的 <see cref="long"/> 缓冲大小，值为 64，占用空间为 512B。
	/// </summary>
	public const int StackallocLongSizeLimit = 64;
}
