namespace Cyjb.Reflection
{
	/// <summary>
	/// 表示方法的闭包。
	/// </summary>
	public sealed class Closure
	{
		/// <summary>
		/// 闭包中的常量。
		/// </summary>
		public readonly object[] Constants;

		/// <summary>
		/// 使用指定的常量初始化闭包。
		/// </summary>
		/// <param name="constants">闭包中的常量。</param>
		public Closure(object[] constants)
		{
			Constants = constants;
		}
	}
}
