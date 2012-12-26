using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.Enum"/> 类的扩展方法。
	/// </summary>
	public static class EnumExt
	{
		/// <summary>
		/// 确定当前实例中是否设置了一个或多个位域中的任何一个。
		/// </summary>
		/// <param name="baseEnum">当前枚举实例。</param>
		/// <param name="value">一个枚举值。</param>
		/// <returns>如果在 <paramref name="value"/> 中设置的位域的任意一个也在当前实例中进行了设置，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		public static bool AnyFlag(this Enum baseEnum, Enum value)
		{
			if (!Type.GetTypeHandle(baseEnum).Equals(Type.GetTypeHandle(value)))
			{
				throw ExceptionHelper.EnumTypeDoesNotMatch(value.GetType(), baseEnum.GetType());
			}
			return ((ToUInt64(baseEnum) & ToUInt64(value)) != 0);
		}
		/// <summary>
		/// 获取 <see cref="System.UInt64"/> 类型的枚举值。
		/// </summary>
		/// <param name="value">要获取的枚举值。</param>
		/// <returns><see cref="System.UInt64"/> 类型的枚举值。</returns>
		private static ulong ToUInt64(object value)
		{
			switch (Convert.GetTypeCode(value))
			{
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			}
			throw ExceptionHelper.UnknownEnumType();
		}
	}
}
