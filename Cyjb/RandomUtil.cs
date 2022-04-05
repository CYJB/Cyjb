namespace Cyjb
{
	/// <summary>
	/// 提供关于随机数的扩展方法。
	/// </summary>
	public static class RandomUtil
	{
		/// <summary>
		/// 返回随机布尔值。
		/// </summary>
		/// <param name="random">随机数发生器。</param>
		/// <returns>随机布尔值。</returns>
		public static bool NextBoolean(this Random random)
		{
			return (random.Next() & 1) == 1;
		}
	}
}
