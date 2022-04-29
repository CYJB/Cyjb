namespace Cyjb
{
	/// <summary>
	/// 提供关于随机数的扩展方法。
	/// </summary>
	public static class RandomUtil
	{
		/// <summary>
		/// 返回平均概率的随机布尔值。
		/// </summary>
		/// <param name="random">随机数发生器。</param>
		/// <returns>随机布尔值。</returns>
		public static bool NextBoolean(this Random random)
		{
			return (random.Next() & 1) == 1;
		}

		/// <summary>
		/// 返回指定概率的随机布尔值。
		/// </summary>
		/// <param name="random">随机数发生器。</param>
		/// <param name="probability"><c>true</c> 值的概率。</param>
		/// <returns>随机布尔值。</returns>
		public static bool NextBoolean(this Random random, double probability)
		{
			return random.NextDouble() > probability;
		}

		/// <summary>
		/// 随机返回数组的某一项。
		/// </summary>
		/// <typeparam name="T">数组的元素类型。</typeparam>
		/// <param name="random">随机数发生器。</param>
		/// <param name="items">随机选择的数组。</param>
		/// <returns>数组的随机项。</returns>
		/// <exception cref="ArgumentException"><paramref name="items"/> 是空数组。</exception>
		public static T Choose<T>(this Random random, params T[] items)
		{
			CommonExceptions.CheckCollectionEmpty(items);
			return items[random.Next(items.Length)];
		}
	}
}
