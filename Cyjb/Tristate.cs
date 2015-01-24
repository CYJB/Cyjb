namespace Cyjb
{
	/// <summary>
	/// 表示一个三态枚举。
	/// </summary>
	public enum Tristate
	{
		/// <summary>
		/// 表示否。
		/// </summary>
		[ResDescription("Tristate_False")]
		False = 0,
		/// <summary>
		/// 表示是。
		/// </summary>
		[ResDescription("Tristate_True")]
		True = 1,
		/// <summary>
		/// 表示未知。
		/// </summary>
		[ResDescription("Tristate_NotSure")]
		NotSure = 2
	}
}