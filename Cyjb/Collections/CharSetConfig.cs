namespace Cyjb.Collections
{
	/// <summary>
	/// 字符集合的配置。
	/// </summary>
	internal static class CharSetConfig
	{
		/// <summary>
		/// <see cref="char"/> 的字节数。
		/// </summary>
		private const int CharBitLength = 16;
		/// <summary>
		/// 数组项可以存储的字节数，<see cref="ulong"/> 可以存储 64 中情况（2^6）。
		/// </summary>
		private const int ItemBitLength = 6;
		/// <summary>
		/// 顶层数组的比特长度。
		/// </summary>
		private const int TopBitLength = 6;
		/// <summary>
		/// 底层数组的比特长度。
		/// </summary>
		public const int BottomBitLength = CharBitLength - TopBitLength - ItemBitLength;
		/// <summary>
		/// 顶层数组的长度。
		/// </summary>
		public const int TopLen = 1 << TopBitLength;
		/// <summary>
		/// 顶层数组索引的位移。
		/// </summary>
		public const int TopShift = CharBitLength - TopBitLength;
		/// <summary>
		/// 底层数组的长度。
		/// </summary>
		public const int BtmLen = 1 << BottomBitLength;
		/// <summary>
		/// 每个底层数组包含的字符个数。
		/// </summary>
		public const int BtmCount = BtmLen << ItemBitLength;
		/// <summary>
		/// 底层数组索引的位移。
		/// </summary>
		public const int BtmShift = TopShift - BottomBitLength;
		/// <summary>
		/// 底层数组部分的掩码。
		/// </summary>
		public const int BtmMask = (1 << TopShift) - 1;
		/// <summary>
		/// 底层数组的索引的掩码。
		/// </summary>
		public const int IndexMask = (1 << BtmShift) - 1;

		/// <summary>
		/// 获取指定字符对应的索引和掩码。
		/// </summary>
		/// <param name="ch">要获取索引和掩码的字符。</param>
		/// <param name="topIndex">顶层数组的索引。</param>
		/// <param name="btmIndex">底层数组的索引。</param>
		/// <param name="binMask">数据的二进制掩码。</param>
		public static void GetIndex(char ch, out int topIndex, out int btmIndex, out ulong binMask)
		{
			topIndex = ch >> TopShift;
			btmIndex = (ch & BtmMask) >> BtmShift;
			binMask = 1UL << (ch & IndexMask);
		}
	}
}
