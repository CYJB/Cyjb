using System;
using System.Globalization;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 将一个基本数据类型转换为另一个基本数据类型的方法。
	/// </summary>
	public static class ConvertExt
	{

		#region 类型转换

		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。
		/// 对可空类型、枚举和用户自定义隐式类型转换提供支持。
		/// </summary>
		/// <typeparam name="T">要转换到的类型。</typeparam>
		/// <param name="value">要转换的对象。</param>
		/// <returns>一个对象，其类型为 <typeparamref name="T"/>，并且其值等效于 <paramref name="value"/>。</returns>
		public static T ChangeType<T>(object value)
		{
			return (T)ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。参数提供区域性特定的格式设置信息。
		/// 对可空类型、枚举和用户自定义隐式类型转换提供支持。
		/// </summary>
		/// <typeparam name="T">要转换到的类型。</typeparam>
		/// <param name="value">要转换的对象。</param>
		/// <param name="provider">一个提供区域性特定的格式设置信息的对象。</param>
		/// <returns>一个对象，其类型为 <typeparamref name="T"/>，并且其值等效于 <paramref name="value"/>。</returns>
		public static T ChangeType<T>(object value, IFormatProvider provider)
		{
			return (T)ChangeType(value, typeof(T), provider);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。 
		/// 对可空类型、枚举和用户自定义隐式类型转换提供支持。
		/// </summary>
		/// <param name="value">要转换的对象。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <returns>一个对象，其类型为 <paramref name="conversionType"/>，并且其值等效于 <paramref name="value"/>。</returns>
		public static object ChangeType(object value, Type conversionType)
		{
			return ChangeType(value, conversionType, CultureInfo.CurrentCulture);
		}
		/// <summary>
		/// 返回指定类型的对象，其值等效于指定对象。参数提供区域性特定的格式设置信息。
		/// 对可空类型、枚举和用户自定义类型转换提供支持。
		/// </summary>
		/// <param name="value">要转换的对象。</param>
		/// <param name="conversionType">要返回的对象的类型。</param>
		/// <param name="provider">一个提供区域性特定的格式设置信息的对象。</param>
		/// <returns>一个对象，其类型为 <paramref name="conversionType"/>，并且其值等效于 <paramref name="value"/>。</returns>
		public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
		{
			ExceptionHelper.CheckArgumentNull(conversionType, "conversionType");
			if (conversionType.TypeHandle.Equals(typeof(object).TypeHandle))
			{
				return value;
			}
			Type type = value.GetType();
			if (conversionType.IsByRef)
			{
				conversionType = conversionType.GetElementType();
			}
			// 对 Nullable<T> 的支持。
			bool nullalbe = TypeExt.NullableAssignableFrom(ref conversionType);
			if (value == null)
			{
				if (!nullalbe && conversionType.IsValueType)
				{
					throw ExceptionHelper.CannotCastNullToValueType();
				}
				return null;
			}
			// 对枚举的支持。
			if (conversionType.IsEnum)
			{
				conversionType = Enum.GetUnderlyingType(conversionType);
			}
			RuntimeTypeHandle conversionHandle = conversionType.TypeHandle;
			if (conversionType.IsInstanceOfType(type) || conversionHandle.Equals(typeof(object).TypeHandle))
			{
				return value;
			}
			RuntimeTypeHandle typeHandle = type.TypeHandle;
			// 检测用户定义类型转换。
			ConversionMethod method;
			if (ConversionCache.GetTypeOperators(typeHandle).TryGetValue(conversionHandle, out method) &&
				method.ConversionType.AnyFlag(ConversionType.To))
			{
				return MethodInfo.GetMethodFromHandle(method.ToMethod).Invoke(null, new object[] { value });
			}
			if (ConversionCache.GetTypeOperators(conversionHandle).TryGetValue(typeHandle, out method) &&
				method.ConversionType.AnyFlag(ConversionType.From))
			{
				return MethodInfo.GetMethodFromHandle(method.FromMethod).Invoke(null, new object[] { value });
			}
			return Convert.ChangeType(value, conversionType, provider);
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
				throw ExceptionHelper.OverflowByte();
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
				throw ExceptionHelper.OverflowUInt16();
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
						throw ExceptionHelper.NoParsibleDigits();
					}
					else
					{
						throw ExceptionHelper.ExtraJunkAtEnd();
					}
				}
				uint next = unchecked(result * uBase + (uint)t);
				// 判断是否超出 UInt32 的范围。
				if (next < result)
				{
					throw ExceptionHelper.OverflowUInt32();
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
						throw ExceptionHelper.NoParsibleDigits();
					}
					else
					{
						throw ExceptionHelper.ExtraJunkAtEnd();
					}
				}
				ulong next = unchecked(result * ulBase + (ulong)t);
				// 判断是否超出 UInt64 的范围。
				if (next < result)
				{
					throw ExceptionHelper.OverflowUInt64();
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
				throw ExceptionHelper.InvalidBase();
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
		/// <exception cref="System.ArgumentException">
		/// <paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
		/// 但前面带一个负号。</exception>
		private static void CheckBaseConvert(string value, int fromBase)
		{
			// 基数检查。
			if (fromBase < 3 || fromBase > 36)
			{
				throw ExceptionHelper.InvalidBase();
			}
			if (value.Length == 0)
			{
				throw ExceptionHelper.NoParsibleDigits();
			}
			// 负号检查。
			if (value[0] == '-')
			{
				throw ExceptionHelper.BaseConvertNegativeValue();
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
