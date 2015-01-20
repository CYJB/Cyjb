using System.Diagnostics.CodeAnalysis;

namespace Cyjb
{
	/// <summary>
	/// 表示类型转换的类型。
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	public enum ConversionType : byte
	{
		/// <summary>
		/// 表示不存在预定义转换。
		/// </summary>
		None,
		/// <summary>
		/// 表示标识转换。
		/// </summary>
		IdentityConversion,
		/// <summary>
		/// 表示隐式数值转换。
		/// </summary>
		ImplicitNumericConversion,
		/// <summary>
		/// 表示可空类型的隐式转换。
		/// </summary>
		ImplicitNullableConversion,
		/// <summary>
		/// 表示装箱转换。
		/// </summary>
		BoxConversion,
		/// <summary>
		/// 表示隐式引用转换。
		/// </summary>
		ImplicitReferenceConversion,
		/// <summary>
		/// 表示显式数值转换。
		/// </summary>
		ExplicitNumericConversion,
		/// <summary>
		/// 表示显式枚举转换。
		/// </summary>
		EnumConversion,
		/// <summary>
		/// 表示可空类型的显式转换。
		/// </summary>
		ExplicitNullableConversion,
		/// <summary>
		/// 表示拆箱转换。
		/// </summary>
		UnboxConversion,
		/// <summary>
		/// 表示显式引用转换。
		/// </summary>
		ExplicitReferenceConversion,
		/// <summary>
		/// 表示用户定义的转换。
		/// </summary>
		UserDefinedConversion,
	}
	/// <summary>
	/// 提供 <see cref="ConversionType"/> 类的扩展方法。
	/// </summary>
	public static class ConversionTypeExt
	{
		/// <summary>
		/// 获取当前类型转换是否是隐式类型转换。
		/// </summary>
		/// <param name="conversionType">要判断的类型转换。</param>
		/// <returns>如果当前类型转换是隐式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsImplicit(this ConversionType conversionType)
		{
			return conversionType > ConversionType.None &&
				conversionType <= ConversionType.ImplicitReferenceConversion;
		}
		/// <summary>
		/// 获取当前类型转换是否是显式类型转换。
		/// </summary>
		/// <param name="conversionType">要判断的类型转换。</param>
		/// <returns>如果当前类型转换是显式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsExplicit(this ConversionType conversionType)
		{
			return conversionType >= ConversionType.ExplicitNumericConversion;
		}
		/// <summary>
		/// 获取当前类型转换是否是标识转换或引用类型转换。
		/// </summary>
		/// <param name="conversionType">要判断的类型转换。</param>
		/// <returns>如果当前类型转换是标识转换或引用类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsReference(this ConversionType conversionType)
		{
			return conversionType == ConversionType.IdentityConversion ||
				conversionType == ConversionType.ImplicitReferenceConversion ||
					conversionType == ConversionType.ExplicitReferenceConversion;
		}
	}
}
