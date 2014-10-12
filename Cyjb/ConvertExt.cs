using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 提供将一个数据类型转换为另一个数据类型的方法。
	/// </summary>
	public static class ConvertExt
	{

		#region 隐式类型转换

		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。参数提供区域性特定的格式设置信息。
		/// 只对允许进行隐式类型转换。
		/// </summary>
		/// <param name="value">要转换的对象。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <param name="provider">一个提供区域性特定的格式设置信息的对象。</param>
		/// <returns>一个对象，其类型为 <paramref name="conversionType"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		internal static object ImplicitChangeType(object value, Type conversionType, IFormatProvider provider)
		{
			Type nonNullableType;
			if (BasicChangeType(ref value, conversionType, out nonNullableType))
			{
				return value;
			}
			Type type = value.GetType();
			// 尝试标准隐式类型转换。
			bool success;
			object result = StandardImplicitChangeType(value, type, nonNullableType, provider, out success);
			if (success)
			{
				return result;
			}
			// 对隐式类型转换运算符进行判断。
			ConversionMethod method = ConversionCache.GetImplicitConversion(type, conversionType);
			if (method != null)
			{
				value = MethodInfo.GetMethodFromHandle(method.Method).Invoke(null, new object[] { value });
				if (value != null)
				{
					type = value.GetType();
					if (type != nonNullableType)
					{
						// 处理用户定义隐式类型转换之后的标准隐式类型转换。
						value = StandardImplicitChangeType(value, type, nonNullableType, provider, out success);
					}
				}
				return value;
			}
			throw CommonExceptions.ConvertInvalidValue(value, conversionType);
		}
		/// <summary>
		/// 对指定类型执行基本的类型转换判断，包括转换为 object 和 null 转换。
		/// 基本类型转换失败时，保证 value != null，nonNullableType != null。
		/// </summary>
		/// <param name="value">要转换的对象。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <param name="nonNullableType"><paramref name="value"/> 对应的 non-nullable-type。</param>
		/// <returns>如果基本类型转换成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool BasicChangeType(ref object value, Type conversionType, out Type nonNullableType)
		{
			CommonExceptions.CheckArgumentNull(conversionType, "conversionType");
			if (conversionType.IsByRef)
			{
				conversionType = conversionType.GetElementType();
			}
			// 总是可以转换为 Object。
			if (conversionType == typeof(object))
			{
				nonNullableType = null;
				return true;
			}
			// value 为 null 的情况。
			bool nullableCType = TypeExt.IsNullableType(conversionType, out nonNullableType);
			if (value == null)
			{
				if (conversionType.IsValueType && !nullableCType)
				{
					throw CommonExceptions.CannotCastNullToValueType();
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。参数提供区域性特定的格式设置信息。
		/// 只对允许进行标准隐式类型转换。
		/// </summary>
		/// <remarks>在运行时，value.GetType() 永远不可能为 Nullalbe{T}，因此某些情况可以不考虑。</remarks>
		/// <param name="value">要转换的对象。</param>
		/// <param name="type">要转换的对象的类型。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <param name="provider">一个提供区域性特定的格式设置信息的对象。</param>
		/// <param name="success">如果标准隐式类型转换成功，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <returns>一个对象，其类型为 <paramref name="conversionType"/>，并且其值等效于 <paramref name="value"/>。</returns>
		private static object StandardImplicitChangeType(object value, Type type, Type conversionType,
			IFormatProvider provider, out bool success)
		{
			success = true;
			// 判断隐式数值转换。
			HashSet<TypeCode> typeSet;
			TypeCode cTypeCode = Type.GetTypeCode(conversionType);
			if (!conversionType.IsEnum && TypeExt.ImplicitNumericConversions.TryGetValue(cTypeCode, out typeSet))
			{
				TypeCode typeCode = Type.GetTypeCode(type);
				if (!type.IsEnum && typeSet.Contains(typeCode))
				{
					// char 类型的变量需要额外判断，
					// 因为 Convert 类并不支持到 Single，Double 和 Decimal 类型的转换。
					if (typeCode == TypeCode.Char)
					{
						switch (cTypeCode)
						{
							case TypeCode.Single: return ConvertCharToSingle(value);
							case TypeCode.Double: return ConvertCharToDouble(value);
							case TypeCode.Decimal: return ConvertCharToDecimal(value);
						}
					}
					return Convert.ChangeType(value, conversionType, provider);
				}
			}
			// 判断隐式引用转换和装箱转换。
			if (conversionType.IsAssignableFrom(type))
			{
				return value;
			}
			success = false;
			return null;
		}
		/// <summary>
		/// 将 <see cref="System.Char"/> 类型的值转换为 <see cref="System.Single"/> 类型的值。
		/// </summary>
		/// <param name="value">要转换的 <see cref="System.Char"/> 类型的值。</param>
		/// <returns>转换得到的 <see cref="Single"/> 类型的值。</returns>
		private static object ConvertCharToSingle(object value)
		{
			return (float)(char)value;
		}
		/// <summary>
		/// 将 <see cref="System.Char"/> 类型的值转换为 <see cref="System.Double"/> 类型的值。
		/// </summary>
		/// <param name="value">要转换的 <see cref="System.Char"/> 类型的值。</param>
		/// <returns>转换得到的 <see cref="Double"/> 类型的值。</returns>
		private static object ConvertCharToDouble(object value)
		{
			return (double)(char)value;
		}
		/// <summary>
		/// 将 <see cref="System.Char"/> 类型的值转换为 <see cref="System.Decimal"/> 类型的值。
		/// </summary>
		/// <param name="value">要转换的 <see cref="System.Char"/> 类型的值。</param>
		/// <returns>转换得到的 <see cref="Decimal"/> 类型的值。</returns>
		private static object ConvertCharToDecimal(object value)
		{
			return (decimal)(char)value;
		}

		#endregion // 隐式类型转换

		#region 类型转换

		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。支持可空类型、枚举和用户自定义类型转换。
		/// </summary>
		/// <typeparam name="T">要转换到的类型。</typeparam>
		/// <param name="value">要转换的对象。</param>
		/// <returns>一个对象，其类型为 <typeparamref name="T"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		public static T ChangeType<T>(object value)
		{
			return (T)ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。参数提供区域性特定的格式设置信息。
		/// 支持可空类型、枚举和用户自定义类型转换。
		/// </summary>
		/// <typeparam name="T">要转换到的类型。</typeparam>
		/// <param name="value">要转换的对象。</param>
		/// <param name="provider">一个提供区域性特定的格式设置信息的对象。</param>
		/// <returns>一个对象，其类型为 <typeparamref name="T"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		public static T ChangeType<T>(object value, IFormatProvider provider)
		{
			return (T)ChangeType(value, typeof(T), provider);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。 支持可空类型、枚举和用户自定义类型转换。
		/// </summary>
		/// <param name="value">要转换的对象。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <returns>一个对象，其类型为 <paramref name="conversionType"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		public static object ChangeType(object value, Type conversionType)
		{
			return ChangeType(value, conversionType, CultureInfo.CurrentCulture);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。参数提供区域性特定的格式设置信息。
		/// 支持可空类型、枚举和用户自定义类型转换。
		/// </summary>
		/// <param name="value">要转换的对象。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <param name="provider">一个提供区域性特定的格式设置信息的对象。</param>
		/// <returns>一个对象，其类型为 <paramref name="conversionType"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		/// <overloads>
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。支持可空类型、枚举和用户自定义类型转换。
		/// </summary>
		/// </overloads>
		public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
		{
			Type nonNullableType;
			if (BasicChangeType(ref value, conversionType, out nonNullableType))
			{
				return value;
			}
			Type type = value.GetType();
			// 尝试显示枚举转换。
			if (conversionType.IsEnumExplicitFrom(type))
			{
				if (conversionType.IsEnum)
				{
					// Enum.ToObject 不支持 char, float, double 和 decimal。
					switch (Type.GetTypeCode(type))
					{
						case TypeCode.Char:
							value = (ushort)(char)value;
							break;
						case TypeCode.Single:
							value = (long)(float)value;
							break;
						case TypeCode.Double:
							value = (long)(double)value;
							break;
						case TypeCode.Decimal:
							value = (long)(decimal)value;
							break;
					}
					return Enum.ToObject(conversionType, value);
				}
				return Convert.ChangeType(value, conversionType, provider);
			}
			// 尝试标准显式类型转换。
			bool success;
			object result = StandardExplicitChangeType(value, type, nonNullableType, provider, out success);
			if (success)
			{
				return result;
			}
			// 对显式类型转换运算符进行判断。
			ConversionMethod method = ConversionCache.GetExplicitConversion(type, conversionType);
			if (method != null)
			{
				value = MethodBase.GetMethodFromHandle(method.Method).Invoke(null, new [] { value });
				if (value != null)
				{
					type = value.GetType();
					if (type != nonNullableType)
					{
						// 处理用户定义显式类型转换之后的标准显式类型转换。
						value = StandardExplicitChangeType(value, type, nonNullableType, provider, out success);
					}
				}
				return value;
			}
			// 尝试其他支持的转换。
			return Convert.ChangeType(value, conversionType, provider);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。参数提供区域性特定的格式设置信息。
		/// 只对允许进行标准显式类型转换。
		/// </summary>
		/// <remarks>在运行时，value.GetType() 永远不可能为 Nullalbe{T}，因此某些情况可以不考虑。</remarks>
		/// <param name="value">要转换的对象。</param>
		/// <param name="type">要转换的对象的类型。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <param name="provider">一个提供区域性特定的格式设置信息的对象。</param>
		/// <param name="success">如果标准显式类型转换成功，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <returns>一个对象，其类型为 <paramref name="conversionType"/>，
		/// 并且其值等效于 <paramref name="value"/>。</returns>
		private static object StandardExplicitChangeType(object value, Type type, Type conversionType,
			IFormatProvider provider, out bool success)
		{
			success = true;
			// 判断显式数值转换。
			TypeCode cTypeCode = Type.GetTypeCode(conversionType);
			TypeCode typeCode = Type.GetTypeCode(type);
			if (!conversionType.IsEnum && TypeExt.ExplicitNumericConversions.Contains(cTypeCode) &&
				!type.IsEnum && TypeExt.ExplicitNumericConversions.Contains(typeCode))
			{
				// char 类型的变量需要额外判断，
				// 因为 Convert 类并不支持与 Single，Double 和 Decimal 类型之间的转换。
				if (typeCode == TypeCode.Char)
				{
					switch (cTypeCode)
					{
						case TypeCode.Single: return ConvertCharToSingle(value);
						case TypeCode.Double: return ConvertCharToDouble(value);
						case TypeCode.Decimal: return ConvertCharToDecimal(value);
					}
				}
				else if (cTypeCode == TypeCode.Char)
				{
					switch (typeCode)
					{
						case TypeCode.Single: return ConvertSingleToChar(value);
						case TypeCode.Double: return ConvertDoubleToChar(value);
						case TypeCode.Decimal: return ConvertDecimalToChar(value);
					}
				}
				return Convert.ChangeType(value, conversionType, provider);
			}
			// 要想显式转换成功，value 必须为 null 或者可以进行隐式转换。
			// 判断隐式引用转换和装箱转换。
			if (conversionType.IsAssignableFrom(type))
			{
				return value;
			}
			success = false;
			return null;
		}
		/// <summary>
		/// 将 <see cref="System.Single"/> 类型的值转换为 <see cref="System.Char"/> 类型的值。
		/// </summary>
		/// <param name="value">要转换的 <see cref="System.Single"/> 类型的值。</param>
		/// <returns>转换得到的 <see cref="Char"/> 类型的值。</returns>
		private static object ConvertSingleToChar(object value)
		{
			return (char)(float)value;
		}
		/// <summary>
		/// 将 <see cref="System.Double"/> 类型的值转换为 <see cref="System.Char"/> 类型的值。
		/// </summary>
		/// <param name="value">要转换的 <see cref="System.Double"/> 类型的值。</param>
		/// <returns>转换得到的 <see cref="Char"/> 类型的值。</returns>
		private static object ConvertDoubleToChar(object value)
		{
			return (char)(double)value;
		}
		/// <summary>
		/// 将 <see cref="System.Decimal"/> 类型的值转换为 <see cref="System.Char"/> 类型的值。
		/// </summary>
		/// <param name="value">要转换的 <see cref="System.Decimal"/> 类型的值。</param>
		/// <returns>转换得到的 <see cref="Char"/> 类型的值。</returns>
		private static object ConvertDecimalToChar(object value)
		{
			return (char)(decimal)value;
		}

		#endregion // 类型转换

		#region 进制转换

		#region ToSByte

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>8</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>8</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.SByte.MinValue"/> 或大于 
		/// <see cref="System.SByte.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(string value, int fromBase)
		{
			return unchecked((sbyte)ToByte(value, fromBase));
		}

		#endregion // ToInt16

		#region ToInt16

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>16</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>16</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.Int16.MinValue"/> 或大于 
		/// <see cref="System.Int16.MaxValue"/> 的数字。</exception>
		public static short ToInt16(string value, int fromBase)
		{
			return unchecked((short)ToUInt16(value, fromBase));
		}

		#endregion // ToInt16

		#region ToInt32

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>32</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>32</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.Int32.MinValue"/> 或大于 
		/// <see cref="System.Int32.MaxValue"/> 的数字。</exception>
		public static int ToInt32(string value, int fromBase)
		{
			return unchecked((int)ToUInt32(value, fromBase));
		}

		#endregion // ToInt32

		#region ToInt64

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>64</c> 位有符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>64</c> 位有符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.Int64.MinValue"/> 或大于 
		/// <see cref="System.Int64.MaxValue"/> 的数字。</exception>
		public static long ToInt64(string value, int fromBase)
		{
			return unchecked((long)ToUInt64(value, fromBase));
		}

		#endregion // ToInt64

		#region ToByte

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>8</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>8</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.Byte.MinValue"/> 或大于 
		/// <see cref="System.Byte.MaxValue"/> 的数字。</exception>
		public static byte ToByte(string value, int fromBase)
		{
			uint result = ToUInt32(value, fromBase);
			if (result > byte.MaxValue)
			{
				throw CommonExceptions.OverflowByte();
			}
			return unchecked((byte)result);
		}

		#endregion // ToInt16

		#region ToUInt16

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>16</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>16</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.UInt16.MinValue"/> 或大于 
		/// <see cref="System.UInt16.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(string value, int fromBase)
		{
			uint result = ToUInt32(value, fromBase);
			if (result > ushort.MaxValue)
			{
				throw CommonExceptions.OverflowUInt16();
			}
			return unchecked((ushort)result);
		}

		#endregion // ToUInt64

		#region ToUInt32

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>32</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>32</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.UInt32.MinValue"/> 或大于 
		/// <see cref="System.UInt32.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(string value, int fromBase)
		{
			// 使用内置方法，会快一些。
			if (fromBase == 2 || fromBase == 8 || fromBase == 10 || fromBase == 16)
			{
				return Convert.ToUInt32(value, fromBase);
			}
			// 使用自己的算法。
			if (value == null)
			{
				return 0U;
			}
			CheckBaseConvert(value, fromBase);
			uint result = 0;
			uint uBase = (uint)fromBase;
			for (int i = 0; i < value.Length; i++)
			{
				int t = GetBaseValue(value[i], fromBase);
				if (t < 0)
				{
					if (i == 0)
					{
						throw CommonExceptions.NoParsibleDigits();
					}
					else
					{
						throw CommonExceptions.ExtraJunkAtEnd();
					}
				}
				uint next = unchecked(result * uBase + (uint)t);
				// 判断是否超出 UInt32 的范围。
				if (next < result)
				{
					throw CommonExceptions.OverflowUInt32();
				}
				result = next;
			}
			return result;
		}

		#endregion // ToUInt64

		#region ToUInt64

		/// <summary>
		/// 将指定基的数字的字符串表示形式转换为等效的 <c>64</c> 位无符号整数。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>与 <paramref name="value"/> 中数字等效的 <c>64</c> 位无符号整数，
		/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> 
		/// 包含的一个字符不是 <paramref name="fromBase"/> 指定的基中的有效数字。
		/// 如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
		/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
		/// <exception cref="System.OverflowException"><paramref name="value"/> 
		/// 表示小于 <see cref="System.UInt64.MinValue"/> 或大于 
		/// <see cref="System.UInt64.MaxValue"/> 的数字。</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(string value, int fromBase)
		{
			// 使用内置方法，会快一些。
			if (fromBase == 2 || fromBase == 8 || fromBase == 10 || fromBase == 16)
			{
				return Convert.ToUInt64(value, fromBase);
			}
			// 使用自己的算法。
			if (value == null)
			{
				return 0UL;
			}
			CheckBaseConvert(value, fromBase);
			ulong result = 0;
			ulong ulBase = (ulong)fromBase;
			for (int i = 0; i < value.Length; i++)
			{
				int t = GetBaseValue(value[i], fromBase);
				if (t < 0)
				{
					if (i == 0)
					{
						throw CommonExceptions.NoParsibleDigits();
					}
					else
					{
						throw CommonExceptions.ExtraJunkAtEnd();
					}
				}
				ulong next = unchecked(result * ulBase + (ulong)t);
				// 判断是否超出 UInt64 的范围。
				if (next < result)
				{
					throw CommonExceptions.OverflowUInt64();
				}
				result = next;
			}
			return result;
		}

		#endregion // ToUInt64

		#region ToString

		/// <summary>
		/// 将 <c>8</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>8</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <overloads>
		/// <summary>
		/// 将给定的整数值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static string ToString(this sbyte value, int toBase)
		{
			bool neg = false;
			ulong ulValue = 0;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				// 这里必须保证位数相同。
				ulValue = (byte)value;
			}
			char[] buffer = new char[8];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>16</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>16</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this short value, int toBase)
		{
			bool neg = false;
			ulong ulValue = 0;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				// 这里必须保证位数相同。
				ulValue = (ushort)value;
			}
			char[] buffer = new char[16];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>32</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>32</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this int value, int toBase)
		{
			bool neg = false;
			ulong ulValue = 0;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				// 这里必须保证位数相同。
				ulValue = (uint)value;
			}
			char[] buffer = new char[32];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>64</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>64</c> 位有符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this long value, int toBase)
		{
			bool neg = false;
			ulong ulValue = 0;
			if (value < 0 && toBase == 10)
			{
				// 仅 10 进制支持负数。
				neg = true;
				ulValue = (ulong)-value;
			}
			else
			{
				ulValue = (ulong)value;
			}
			char[] buffer = new char[64];
			int idx = ConvertBase(buffer, ulValue, toBase);
			if (neg)
			{
				buffer[--idx] = '-';
			}
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>8</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>8</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		public static string ToString(this byte value, int toBase)
		{
			char[] buffer = new char[8];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>16</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>16</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		[CLSCompliant(false)]
		public static string ToString(this ushort value, int toBase)
		{
			char[] buffer = new char[16];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>32</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>32</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		[CLSCompliant(false)]
		public static string ToString(this uint value, int toBase)
		{
			char[] buffer = new char[32];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>64</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="value">要转换的 <c>64</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
		/// 的字符串表示形式。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		[CLSCompliant(false)]
		public static string ToString(this ulong value, int toBase)
		{
			char[] buffer = new char[64];
			int idx = ConvertBase(buffer, value, toBase);
			return new string(buffer, idx, buffer.Length - idx);
		}
		/// <summary>
		/// 将 <c>64</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
		/// </summary>
		/// <param name="buffer">字符串的缓冲区。</param>
		/// <param name="value">要转换的 <c>64</c> 位无符号整数。</param>
		/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>转换后字符串的起始索引。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		private static int ConvertBase(char[] buffer, ulong value, int toBase)
		{
			if (toBase < 2 || toBase > 36)
			{
				throw CommonExceptions.InvalidBase("toBase", toBase);
			}
			// 从后向前转换，不必反转字符串。
			ulong ulBase = (ulong)toBase;
			int idx = buffer.Length - 1;
			do
			{
				ulong quot = value / ulBase;
				buffer[idx--] = CharExt.BaseDigits[value - quot * ulBase];
				value = quot;
			} while (value > 0);
			return idx + 1;
		}

		#endregion

		/// <summary>
		/// 对给定的基数和字符串进行检查。
		/// </summary>
		/// <param name="value">包含要转换的数字的字符串，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="value"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.FormatException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		private static void CheckBaseConvert(string value, int fromBase)
		{
			// 基数检查。
			if (fromBase < 3 || fromBase > 36)
			{
				throw CommonExceptions.InvalidBase("fromBase", fromBase);
			}
			if (value.Length == 0)
			{
				throw CommonExceptions.NoParsibleDigits();
			}
			// 负号检查。
			if (value[0] == '-')
			{
				throw CommonExceptions.BaseConvertNegativeValue();
			}
		}
		/// <summary>
		/// 返回指定字符以指定的基表示的值。
		/// </summary>
		/// <param name="ch">要获取值的字符，
		/// 使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
		/// <param name="fromBase"><paramref name="ch"/> 中数字的基数，
		/// 它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
		/// <returns>如果字符有效，则返回字符对应的值。否则返回 <c>-1</c>。</returns>
		private static int GetBaseValue(char ch, int fromBase)
		{
			int value = -1;
			if (ch < 'A')
			{
				if (ch >= '0' && ch <= '9')
				{
					value = ch - '0';
				}
			}
			else if (ch < 'a')
			{
				if (ch <= 'Z')
				{
					value = ch - 'A' + 10;
				}
			}
			else if (ch <= 'z')
			{
				value = ch - 'a' + 10;
			}
			if (value < fromBase)
			{
				return value;
			}
			return -1;
		}

		#endregion // 进制转换

	}
}
