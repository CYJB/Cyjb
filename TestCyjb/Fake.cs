namespace TestCyjb
{
	/// <summary>
	/// 提供用于测试的各个类型的测试值。
	/// </summary>
	internal static class Fake
	{
		/// <summary>
		/// 返回 <typeparamref name="T"/> 类型的 <c>null</c> 值。
		/// </summary>
		public static T Null<T>()
		{
			return default!;
		}
	}
}
