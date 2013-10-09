using System;

namespace Cyjb
{
	/// <summary>
	/// 表示一个全局的伪随机数生成器，并提供关于随机数的扩展方法。
	/// </summary>
	public static class RandomExt
	{
		/// <summary>
		/// 全局伪随机数生成器。
		/// </summary>
		private static Random random = new Random();
		/// <summary>
		/// 更新种子值为与时间相关的值。
		/// </summary>
		public static void UpdateSeed()
		{
			random = new Random();
		}
		/// <summary>
		/// 更新种子值为与指定的值。
		/// </summary>
		/// <param name="seed">用来计算伪随机数序列起始值的数字。
		/// 如果指定的是负数，则使用其绝对值。</param>
		public static void UpdateSeed(int seed)
		{
			random = new Random(seed);
		}
		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.Int32.MaxValue"/> 
		/// 的 <c>32</c> 位带符号整数。</returns>
		public static int Next()
		{
			return random.Next();
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>32</c> 位带符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="maxValue"/> 小于零。</exception>
		public static int Next(int maxValue)
		{
			return random.Next(maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>32</c> 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		public static int Next(int minValue, int maxValue)
		{
			return random.Next(minValue, maxValue);
		}
		/// <summary>
		/// 返回随机布尔值。
		/// </summary>
		/// <returns>随机布尔值。</returns>
		public static bool NextBoolean()
		{
			return random.Next(2) == 1;
		}
		/// <summary>
		/// 用随机数填充指定字节数组的元素。
		/// </summary>
		/// <param name="buffer">包含随机数的字节数组。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="buffer"/> 为 <c>null</c>。</exception>
		public static void NextBytes(byte[] buffer)
		{
			random.NextBytes(buffer);
		}
		/// <summary>
		/// 返回一个介于 <c>0.0</c> 和 <c>1.0</c> 之间的随机数。
		/// </summary>
		/// <returns>大于等于 <c>0.0</c> 并且小于 <c>1.0</c> 的单精度浮点数。</returns>
		public static float NextSingle()
		{
			return (float)random.NextDouble();
		}
		/// <summary>
		/// 返回一个介于 <c>0.0</c> 和 <c>1.0</c> 之间的随机数。
		/// </summary>
		/// <returns>大于等于 <c>0.0</c> 并且小于 <c>1.0</c> 的双精度浮点数。</returns>
		public static double NextDouble()
		{
			return random.NextDouble();
		}
	}
}
