using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.Enum"/> 类的扩展方法。
	/// </summary>
	/// <remarks>
	/// <para>关于枚举类型的 <see cref="System.ComponentModel.DescriptionAttribute"/> 
	/// 获取与解析，可以参考我的博文 
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/EnumDescription.html">
	/// C# 获取与解析枚举类型的 DescriptionAttribute</see></para>
	/// <para>内部使用 <see cref="Cyjb.Utility.ICache{TKey,TValue}"/> 
	/// 接口缓存枚举的描述，使用的键为 Cyjb.EnumDescriptionCache。
	/// 关于如何设置缓存，可以参见 <see cref="CacheFactory"/>。</para>
	/// </remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/EnumDescription.html">
	/// C# 获取与解析枚举类型的 DescriptionAttribute</seealso>
	public static class EnumExt
	{

		#region 枚举字符串

		/// <summary>
		/// 将指定具有整数值的对象转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		/// <overloads>
		/// <summary>
		/// 将指定具有整数值的对象转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// </overloads>
		public static string GetString(Type enumType, object value)
		{
			return GetString(enumType, ToUInt64(value), false);
		}
		/// <summary>
		/// 将指定 8 位有符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		[CLSCompliant(false)]
		public static string GetString(Type enumType, sbyte value)
		{
			return GetString(enumType, (ulong)value, false);
		}
		/// <summary>
		/// 将指定 16 位有符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		public static string GetString(Type enumType, short value)
		{
			return GetString(enumType, (ulong)value, false);
		}
		/// <summary>
		/// 将指定 32 位有符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		public static string GetString(Type enumType, int value)
		{
			return GetString(enumType, (ulong)value, false);
		}
		/// <summary>
		/// 将指定 64 位有符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		public static string GetString(Type enumType, long value)
		{
			return GetString(enumType, (ulong)value, false);
		}
		/// <summary>
		/// 将指定 8 位无符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		public static string GetString(Type enumType, byte value)
		{
			return GetString(enumType, value, false);
		}
		/// <summary>
		/// 将指定 16 位无符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		[CLSCompliant(false)]
		public static string GetString(Type enumType, ushort value)
		{
			return GetString(enumType, value, false);
		}
		/// <summary>
		/// 将指定 32 位无符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		[CLSCompliant(false)]
		public static string GetString(Type enumType, uint value)
		{
			return GetString(enumType, value, false);
		}
		/// <summary>
		/// 将指定 64 位无符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		[CLSCompliant(false)]
		public static string GetString(Type enumType, ulong value)
		{
			return GetString(enumType, value, false);
		}
		/// <summary>
		/// 返回指定枚举值的描述（通过 
		/// <see cref="System.ComponentModel.DescriptionAttribute"/> 指定）。
		/// 如果没有指定描述，则返回枚举常数的名称，没有找到枚举常数则返回枚举值。
		/// </summary>
		/// <param name="value">要获取描述的枚举值。</param>
		/// <returns>指定枚举值的描述。</returns>
		public static string GetDescription(this Enum value)
		{
			return GetString(value.GetType(), ToUInt64(value), true);
		}
		/// <summary>
		/// 将指定 64 位有符号整数转换为与其枚举值等效的字符串表示形式。
		/// </summary>
		/// <param name="enumType">枚举的类型</param>
		/// <param name="value">要转换为与其枚举值等效的字符串的值。</param>
		/// <param name="useDescription">是否获得枚举的描述。</param>
		/// <returns>与给定值的枚举值等效的字符串表示形式。</returns>
		private static string GetString(Type enumType, ulong value, bool useDescription)
		{
			// 寻找枚举值的组合。
			EnumCache cache = GetEnumCache(enumType);
			int idx = Array.BinarySearch(cache.Values, value);
			string[] names = useDescription ? cache.Descriptions : cache.Names;
			if (idx >= 0)
			{
				// 枚举值已定义，直接返回相应的名称。
				return names[idx];
			}
			// 不是可组合的枚举，直接返回枚举值得字符串形式。
			if (!cache.HasFlagsAttribute)
			{
				return GetStringValue(enumType, value);
			}
			List<string> list = new List<string>();
			// 从后向前寻找匹配的二进制。
			for (int i = cache.Values.Length - 1; i >= 0 && value != 0UL; i--)
			{
				ulong enumValue = cache.Values[i];
				if (enumValue == 0UL)
				{
					continue;
				}
				if ((value & enumValue) == enumValue)
				{
					value -= enumValue;
					list.Add(names[i]);
				}
			}
			list.Reverse();
			// 添加最后剩余的未定义值。
			if (list.Count == 0 || value != 0UL)
			{
				list.Add(GetStringValue(enumType, value));
			}
			return string.Join(", ", list);
		}
		/// <summary>
		/// 返回指定枚举值的字符串形式。一般是由于枚举值没有对应的定义。
		/// </summary>
		/// <param name="enumType">要获取描述的枚举值类型。</param>
		/// <param name="value">要获取描述的枚举值。</param>
		/// <returns>指定枚举值的字符串形式。</returns>
		private static string GetStringValue(Type enumType, ulong value)
		{
			if ((value & 0x8000000000000000UL) > 0)
			{
				// 最高位不为 0，需要根据原先的类型是否是有符号数字，决定是否输出负号。
				switch (Type.GetTypeCode(enumType))
				{
					case TypeCode.SByte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
						long longValue = unchecked((long)value);
						return longValue.ToString(CultureInfo.CurrentCulture);
				}
			}
			return value.ToString(CultureInfo.CurrentCulture);
		}

		#endregion // 枚举描述

		#region 枚举值

		/// <summary>
		/// 检索指定枚举类型中常数值的数组。
		/// </summary>
		/// <typeparam name="TEnum">要获取常数值的枚举类型。</typeparam>
		/// <returns>一个数组，其中包含 <typeparamref name="TEnum"/> 中实例的值。
		/// 该数组的元素按枚举常数的二进制值排序。</returns>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		public static TEnum[] GetValues<TEnum>()
		{
			return Enum.GetValues(typeof(TEnum)) as TEnum[];
		}
		/// <summary>
		/// 检索指定枚举类型中常数文本和值的集合。会检索描述信息作为文本。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <returns>一个数组，其中包含 <paramref name="enumType"/> 中实例的文本和值。
		/// 该集合的元素按枚举阐述的二进制值排序。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="enumType"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 检索指定枚举类型中常数文本和值的集合。
		/// </summary>
		/// </overloads>
		public static TextValuePairCollection GetTextValues(Type enumType)
		{
			return GetTextValues(enumType, true);
		}
		/// <summary>
		/// 检索指定枚举类型中常数文本和值的集合。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="useDescription">是否读取枚举值的描述信息作为文本。</param>
		/// <returns>一个数组，其中包含 <paramref name="enumType"/> 中实例的文本和值。
		/// 该集合的元素按枚举阐述的二进制值排序。</returns>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="enumType"/> 不是 <see cref="System.Enum"/>。</exception>
		public static TextValuePairCollection GetTextValues(Type enumType, bool useDescription)
		{
			TextValuePairCollection enumList = new TextValuePairCollection();
			// 这里使用自己的缓存。
			EnumCache cache = GetEnumCache(enumType);
			ulong[] values = cache.Values;
			string[] names = useDescription ? cache.Descriptions : cache.Names;
			for (int i = 0; i < values.Length; i++)
			{
				enumList.Add(names[i], Enum.ToObject(enumType, values[i]));
			}
			return enumList;
		}
		/// <summary>
		/// 检索指定枚举类型中常数文本和值的集合。
		/// </summary>
		/// <typeparam name="TEnum">要获取常数文本和值的枚举类型。</typeparam>
		/// <returns>一个数组，其中包含 <typeparamref name="TEnum"/> 中实例的文本和值。
		/// 该集合的元素按枚举阐述的二进制值排序。</returns>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		public static TextValuePairCollection<TEnum> GetTextValues<TEnum>()
		{
			return GetTextValues<TEnum>(true);
		}
		/// <summary>
		/// 检索指定枚举类型中常数文本和值的集合。
		/// </summary>
		/// <typeparam name="TEnum">要获取常数文本和值的枚举类型。</typeparam>
		/// <param name="useDescription">是否读取枚举值的描述信息作为文本。</param>
		/// <returns>一个数组，其中包含 <typeparamref name="TEnum"/> 中实例的文本和值。
		/// 该集合的元素按枚举阐述的二进制值排序。</returns>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		public static TextValuePairCollection<TEnum> GetTextValues<TEnum>(bool useDescription)
		{
			Type enumType = typeof(TEnum);
			TextValuePairCollection<TEnum> enumList = new TextValuePairCollection<TEnum>();
			// 这里使用自己的缓存。
			EnumCache cache = GetEnumCache(enumType);
			ulong[] values = cache.Values;
			string[] names = useDescription ? cache.Descriptions : cache.Names;
			for (int i = 0; i < values.Length; i++)
			{
				enumList.Add(names[i], (TEnum)Enum.ToObject(enumType, values[i]));
			}
			return enumList;
		}

		#endregion // 枚举值

		#region 字符串分析

		/// <summary>
		/// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// <typeparam name="TEnum">要获取枚举对象的枚举类型。</typeparam>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <returns><typeparamref name="TEnum"/> 类型的对象，
		/// 其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。</exception>
		/// <exception cref="System.OverflowException">
		/// <paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		/// <overloads>
		/// <summary>
		/// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// </overloads>
		public static TEnum Parse<TEnum>(string value)
		{
			return (TEnum)Enum.Parse(typeof(TEnum), value);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
		/// 一个参数指定该操作是否区分大小写。
		/// </summary>
		/// <typeparam name="TEnum">要获取枚举对象的枚举类型。</typeparam>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <param name="ignoreCase"><c>true</c> 为忽略大小写；<c>false</c> 为考虑大小写。</param>
		/// <returns><typeparamref name="TEnum"/> 类型的对象，
		/// 其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。</exception>
		/// <exception cref="System.OverflowException">
		/// <paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		public static TEnum Parse<TEnum>(string value, bool ignoreCase)
		{
			return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <returns><paramref name="enumType"/> 类型的对象，其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="enumType"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。</exception>
		/// <exception cref="System.OverflowException">
		/// <paramref name="value"/> 超出 <paramref name="enumType"/> 
		/// 基础类型的范围。</exception>
		/// <overloads>
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// </overloads>
		public static object ParseEx(Type enumType, string value)
		{
			return ParseEx(enumType, value, false);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// 一个参数指定该操作是否区分大小写。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <param name="ignoreCase">若要忽略大小写则为 <c>true</c>；
		/// 否则为 <c>false</c>。</param>
		/// <returns><paramref name="enumType"/> 类型的对象，其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="enumType"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。</exception>
		/// <exception cref="System.OverflowException">
		/// <paramref name="value"/> 超出 <paramref name="enumType"/> 
		/// 基础类型的范围。</exception>
		public static object ParseEx(Type enumType, string value, bool ignoreCase)
		{
			CommonExceptions.CheckArgumentNull(enumType, "enumType");
			CommonExceptions.CheckArgumentNull(value, "value");
			if (!enumType.IsEnum)
			{
				throw CommonExceptions.MustBeEnum("enumType", enumType);
			}
			value = value.Trim();
			if (value.Length == 0)
			{
				throw CommonExceptions.MustContainEnumInfo("value");
			}
			// 尝试对数字进行解析，这样可避免之后的字符串比较。
			ulong tmpValue;
			if (ParseString(value, out tmpValue))
			{
				return Enum.ToObject(enumType, tmpValue);
			}
			// 尝试对描述信息进行解析。
			EnumCache cache = GetEnumCache(enumType);
			StringComparison comparison = ignoreCase ?
				StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			ulong valueUL = 0;
			int start = 0;
			do
			{
				// 去除前导空白。
				while (char.IsWhiteSpace(value, start)) { start++; }
				int idx = value.IndexOf(',', start);
				if (idx < 0) { idx = value.Length; }
				int nIdx = idx - 1;
				// 去除后面的空白。
				while (char.IsWhiteSpace(value, nIdx)) { nIdx--; }
				if (nIdx >= start)
				{
					string str = value.Substring(start, nIdx - start + 1);
					int j = 0;
					// 比较常数值的名称和描述信息，先比较名称，后比较描述信息。
					for (; j < cache.Names.Length; j++)
					{
						if (string.Equals(str, cache.Names[j], comparison))
						{
							// 与常数值匹配。
							valueUL |= cache.Values[j];
							break;
						}
					}
					if (j == cache.Names.Length && cache.HasDescription)
					{
						// 比较描述信息。
						for (j = 0; j < cache.Descriptions.Length; j++)
						{
							if (string.Equals(str, cache.Descriptions[j], comparison))
							{
								// 与描述信息匹配。
								valueUL |= cache.Values[j];
								break;
							}
						}
					}
					// 未识别的枚举值。
					if (j == cache.Descriptions.Length)
					{
						// 尝试识别为数字。
						if (ParseString(str, out tmpValue))
						{
							valueUL |= tmpValue;
						}
						else
						{
							// 不能识别为数字。
							throw CommonExceptions.EnumValueNotFound(enumType, str);
						}
					}
				}
				start = idx + 1;
			} while (start < value.Length);
			return Enum.ToObject(enumType, valueUL);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// <typeparam name="TEnum">要获取枚举对象的枚举类型。</typeparam>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <returns><typeparamref name="TEnum"/> 类型的对象，
		/// 其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。</exception>
		/// <exception cref="System.OverflowException">
		/// <paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		public static TEnum ParseEx<TEnum>(string value)
		{
			return (TEnum)ParseEx(typeof(TEnum), value, false);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// 一个参数指定该操作是否区分大小写。
		/// </summary>
		/// <typeparam name="TEnum">要获取枚举对象的枚举类型。</typeparam>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <param name="ignoreCase"><c>true</c> 为忽略大小写；<c>false</c> 为考虑大小写。</param>
		/// <returns><typeparamref name="TEnum"/> 类型的对象，
		/// 其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。</exception>
		/// <exception cref="System.OverflowException">
		/// <paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		public static TEnum ParseEx<TEnum>(string value, bool ignoreCase)
		{
			return (TEnum)ParseEx(typeof(TEnum), value, ignoreCase);
		}
		/// <summary>
		/// 解析给定的字符串，并返回对应的值。
		/// </summary>
		/// <param name="str">要解析的字符串。</param>
		/// <param name="value">字符串对应的值。</param>
		/// <returns>字符串的解析是否成功。</returns>
		private static bool ParseString(string str, out ulong value)
		{
			char firstChar = str[0];
			if (char.IsDigit(firstChar) || firstChar == '+')
			{
				return ulong.TryParse(str, out value);
			}
			else if (firstChar == '-')
			{
				long valueL;
				if (long.TryParse(str, out valueL))
				{
					value = unchecked((ulong)valueL);
					return true;
				}
			}
			value = 0;
			return false;
		}

		#endregion // 字符串分析

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
				throw CommonExceptions.EnumTypeDoesNotMatch("value", value.GetType(), baseEnum.GetType());
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
					return unchecked((ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture));
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			}
			Debug.Fail("无效的枚举类型");
			return 0;
		}

		#region 枚举缓存

		/// <summary>
		/// 枚举类型的缓存。
		/// </summary>
		private static readonly ICache<Type, EnumCache> EnumCaches =
			CacheFactory.CreateCache<Type, EnumCache>("Cyjb.EnumDescriptionCache") ??
			new LruCache<Type, EnumCache>(100);
		/// <summary>
		/// 返回枚举的缓存。
		/// </summary>
		/// <param name="enumType">要获取缓存的枚举类型。</param>
		/// <value>枚举的缓存。</value>
		private static EnumCache GetEnumCache(Type enumType)
		{
			return EnumCaches.GetOrAdd(enumType, type =>
			{
				// 返回枚举类型的值、常数名称和相应描述的列表。
				// 直接使用反射获取数据，而不是 Enum 类的相关方法。
				FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				int len = fields.Length;
				ulong[] values = new ulong[len];
				for (int i = 0; i < len; i++)
				{
					values[i] = ToUInt64(fields[i].GetRawConstantValue());
				}
				// 按照二进制大小排序。
				Array.Sort(values, fields);
				// 获取常数名称和相应描述。
				string[] names = new string[len];
				string[] descs = new string[len];
				bool hasDesc = false;
				for (int i = 0; i < len; i++)
				{
					FieldInfo fieldInfo = fields[i];
					// 获取常数名称。
					names[i] = descs[i] = fieldInfo.Name;
					// 尝试获取描述。
					CustomAttributeData attr = fieldInfo.GetCustomAttributesData().FirstOrDefault(
						data => data.Constructor.ReflectedType.FullName
							.Equals("System.ComponentModel.DescriptionAttribute", StringComparison.Ordinal));
					if (attr != null)
					{
						descs[i] = attr.ConstructorArguments[0].Value as string;
						hasDesc = true;
					}
				}
				// 如果不包含描述信息，直接将描述信息指向常数名称。
				if (!hasDesc)
				{
					descs = names;
				}
				// 将是否包含 FlagsAttribute 一并缓存，能有效的提高性能。
				bool hasFlags = type.GetCustomAttributesData().Any(
					data => data.Constructor.ReflectedType.FullName
						.Equals("System.FlagsAttribute", StringComparison.Ordinal));
				return new EnumCache(hasFlags, hasDesc, values, names, descs);
			});
		}
		/// <summary>
		/// 表示枚举缓存。
		/// </summary>
		[StructLayout(LayoutKind.Auto)]
		private struct EnumCache
		{
			/// <summary>
			/// 枚举值的列表，从小到大排序。
			/// </summary>
			public ulong[] Values;
			/// <summary>
			/// 枚举描述的列表，与枚举值列表一一对应。
			/// </summary>
			public string[] Descriptions;
			/// <summary>
			/// 枚举名称的列表，与枚举值列表一一对应。
			/// </summary>
			public string[] Names;
			/// <summary>
			/// 枚举类是否含有 FlagsAttribute。
			/// </summary>
			public bool HasFlagsAttribute;
			/// <summary>
			/// 枚举类是否定义了 DescriptionAttribute。
			/// </summary>
			public bool HasDescription;
			/// <summary>
			/// 使用给定的值初始化 <see cref="EnumCache"/> 结构的新实例。
			/// </summary>
			/// <param name="hasFlags">枚举类是否含有 FlagsAttribute。</param>
			/// <param name="hasDesc">枚举类是否定义了 DescriptionAttribute。</param>
			/// <param name="values">枚举值的列表，从小到大排序。</param>
			/// <param name="names">枚举名称的列表，与枚举值列表一一对应。</param>
			/// <param name="descs">枚举描述的列表，与枚举值列表一一对应。</param>
			public EnumCache(bool hasFlags, bool hasDesc, ulong[] values, string[] names, string[] descs)
			{
				this.HasFlagsAttribute = hasFlags;
				this.HasDescription = hasDesc;
				this.Values = values;
				this.Names = names;
				this.Descriptions = descs;
			}
		}

		#endregion // 枚举缓存

	}
}
