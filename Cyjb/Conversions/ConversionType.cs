using System.Diagnostics.CodeAnalysis;

namespace Cyjb
{
	/// <summary>
	/// 表示类型转换的类型。
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	internal enum ConversionType : byte
	{
		/// <summary>
		/// 表示不存在类型转换。
		/// </summary>
		None,
		/// <summary>
		/// 表示标识转换。
		/// </summary>
		Identity,
		/// <summary>
		/// 表示隐式数值转换。
		/// </summary>
		ImplicitNumeric,
		/// <summary>
		/// 表示可空类型的隐式转换。
		/// </summary>
		ImplicitNullable,
		/// <summary>
		/// 表示装箱转换。
		/// </summary>
		Box,
		/// <summary>
		/// 表示隐式引用转换。
		/// </summary>
		ImplicitReference,
		/// <summary>
		/// 表示任意隐式类型转换。
		/// </summary>
		Implicit = ImplicitReference,
		/// <summary>
		/// 表示显式数值转换。
		/// </summary>
		ExplicitNumeric,
		/// <summary>
		/// 表示显式枚举转换。
		/// </summary>
		Enum,
		/// <summary>
		/// 表示可空类型的显式转换。
		/// </summary>
		ExplicitNullable,
		/// <summary>
		/// 表示拆箱转换。
		/// </summary>
		Unbox,
		/// <summary>
		/// 表示显式引用转换。
		/// </summary>
		ExplicitReference,
		/// <summary>
		/// 表示任意显式类型转换。
		/// </summary>
		Explicit = ExplicitReference,
		/// <summary>
		/// 表示用户定义的转换。
		/// </summary>
		UserDefined,
	}
	/// <summary>
	/// 提供 <see cref="ConversionType"/> 枚举的扩展方法。
	/// </summary>
	internal static class ConversionTypeExt
	{
		/// <summary>
		/// 获取当前类型转换是否是隐式类型转换。
		/// </summary>
		/// <param name="conversionType">要判断的类型转换。</param>
		/// <returns>如果当前类型转换是隐式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsImplicit(this ConversionType conversionType)
		{
			return conversionType > ConversionType.None && conversionType <= ConversionType.Implicit;
		}
		/// <summary>
		/// 获取当前类型转换是否是显式类型转换。
		/// </summary>
		/// <param name="conversionType">要判断的类型转换。</param>
		/// <returns>如果当前类型转换是显式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsExplicit(this ConversionType conversionType)
		{
			return conversionType >= ConversionType.ExplicitNumeric;
		}
		/// <summary>
		/// 获取当前类型转换是否是标识转换或引用类型转换。
		/// </summary>
		/// <param name="conversionType">要判断的类型转换。</param>
		/// <returns>如果当前类型转换是标识转换或引用类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsReference(this ConversionType conversionType)
		{
			return conversionType == ConversionType.Identity ||
				conversionType == ConversionType.ImplicitReference ||
				conversionType == ConversionType.ExplicitReference;
		}
	}
}
