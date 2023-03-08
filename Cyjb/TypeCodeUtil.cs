namespace Cyjb;

/// <summary>
/// 提供 <see cref="TypeCode"/> 类的扩展方法。
/// </summary>
public static class TypeCodeUtil
{
	/// <summary>
	/// 返回当前 <see cref="TypeCode"/> 是否表示数字类型。
	/// </summary>
	/// <param name="typeCode">要判断的类型。</param>
	/// <returns>如果当前 <see cref="TypeCode"/> 表示数字类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>数字类型为 <see cref="TypeCode.Char"/>、<see cref="TypeCode.SByte"/>、<see cref="TypeCode.Byte"/>、 
	/// <see cref="TypeCode.Int16"/>、<see cref="TypeCode.UInt16"/>、<see cref="TypeCode.Int32"/>、
	/// <see cref="TypeCode.UInt32"/>、<see cref="TypeCode.Int64"/>、<see cref="TypeCode.UInt64"/>、
	/// <see cref="TypeCode.Single"/>、<see cref="TypeCode.Double"/> 和 <see cref="TypeCode.Decimal"/>，
	/// 或者基础类型为其中之一的枚举类型。</remarks>
	/// <overloads>
	/// <summary>
	/// 返回当前类型是否表示数字类型。
	/// </summary>
	/// </overloads>
	public static bool IsNumeric(this TypeCode typeCode)
	{
		return typeCode >= TypeCode.Char && typeCode <= TypeCode.Decimal;
	}

	/// <summary>
	/// 返回当前 <see cref="TypeCode"/> 是否表示无符号整数类型。
	/// </summary>
	/// <param name="typeCode">要判断的类型。</param>
	/// <returns>如果当前 <see cref="TypeCode"/> 表示无符号整数类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>无符号整数类型为 <see cref="TypeCode.Char"/>、<see cref="TypeCode.Byte"/>、
	/// <see cref="TypeCode.UInt16"/>、<see cref="TypeCode.UInt32"/> 和 <see cref="TypeCode.UInt64"/>，
	/// 或者基础类型为其中之一的枚举类型。</remarks>
	/// <overloads>
	/// <summary>
	/// 返回当前类型是否表示无符号整数类型。
	/// </summary>
	/// </overloads>
	public static bool IsUnsigned(this TypeCode typeCode)
	{
		return typeCode == TypeCode.Char || typeCode == TypeCode.Byte || typeCode == TypeCode.UInt16 ||
			typeCode == TypeCode.UInt32 || typeCode == TypeCode.UInt64;
	}

	/// <summary>
	/// 返回当前 <see cref="TypeCode"/> 是否表示有符号整数类型。
	/// </summary>
	/// <param name="typeCode">要判断的类型。</param>
	/// <returns>如果当前 <see cref="TypeCode"/> 表示有符号整数类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>有符号整数类型为 <see cref="TypeCode.SByte"/>、<see cref="TypeCode.Int16"/>、
	/// <see cref="TypeCode.Int32"/> 和 <see cref="TypeCode.Int64"/>，或者基础类型为其中之一的枚举类型。</remarks>
	/// <overloads>
	/// <summary>
	/// 返回当前类型是否表示有符号整数类型。
	/// </summary>
	/// </overloads>
	public static bool IsSigned(this TypeCode typeCode)
	{
		return typeCode == TypeCode.SByte || typeCode == TypeCode.Int16 ||
			typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64;
	}
}
