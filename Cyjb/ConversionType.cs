using System;

namespace Cyjb
{
	/// <summary>
	/// 表示用户定义类型转换的类型。
	/// </summary>
	[Flags]
	internal enum ConversionType
	{
		/// <summary>
		/// 可以从指定类型隐式转换。
		/// </summary>
		ImplicitFrom = 1,
		/// <summary>
		/// 可以隐式转换为指定类型。
		/// </summary>
		ImplicitTo = 2,
		/// <summary>
		/// 可以从指定类型强制转换。
		/// </summary>
		ExplicitFrom = 4,
		/// <summary>
		/// 可以强制转换为指定类型。
		/// </summary>
		ExplicitTo = 8,
		/// <summary>
		/// 可以从指定类型转换。
		/// </summary>
		From = 5,
		/// <summary>
		/// 可以转换为指定类型。
		/// </summary>
		To = 0xA
	}
}
