namespace Cyjb.Collections
{
	/// <summary>
	/// 表示使用范围表示的集合。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	public interface IRangeCollection<T> : IEnumerable<T>
		where T : IComparable<T>
	{
		/// <summary>
		/// 获取当前集合包含的元素数。
		/// </summary>
		/// <value>当前集合中包含的元素数。</value>
		int Count { get; }

		/// <summary>
		/// 返回一个循环访问元素范围的枚举器。
		/// </summary>
		/// <returns>可用于循环访问元素范围的 <see cref="IEnumerable{Tuple}"/>。</returns>
		IEnumerable<(T start, T end)> Ranges();
	}
}
