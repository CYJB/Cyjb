using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using Cyjb.Reflection;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="Enum"/> 类的扩展方法。
	/// </summary>
	/// <remarks>
	/// <para>关于枚举类型的 <see cref="DescriptionAttribute"/> 获取与解析，可以参考我的博文 
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/EnumDescription.html">
	/// C# 获取与解析枚举类型的 DescriptionAttribute</see></para>
	/// <para>枚举缓存的键为 Cyjb.EnumDescriptionCache，默认使用上限为 <c>256</c> 的 
	/// <see cref="LruCache{TKey, TValue}"/>。关于如何设置缓存，可以参见 <see cref="CacheFactory"/>。</para>
	/// </remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/EnumDescription.html">
	/// C# 获取与解析枚举类型的 DescriptionAttribute</seealso>
	public static class EnumExt
	{
		/// <summary>
		/// 在指定枚举中检索具有指定值的常数的名称。
		/// </summary>
		/// <typeparam name="TValue">常数的类型。</typeparam>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">特定枚举常数的值（与 <paramref name="enumType"/> 兼容）。</param>
		/// <returns>一个字符串，其中包含 <paramref name="enumType"/> 中值为 <paramref name="value"/> 
		/// 的枚举常数的名称；如果没有找到这样的常数，则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="enumType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="enumType"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidCastException"><paramref name="value"/> 不能转换为 <paramref name="enumType"/>。</exception>
		/// <remarks>如果多个枚举成员具有相同的基础值，则 <see cref="GetName{TValue}"/> 
		/// 方法可保证它将返回其中一个枚举成员的名称。但是，它并不保证它将始终返回相同枚举成员的名称。
		/// 因此，如果多个枚举成员具有相同的值，应用程序代码决不应依赖于返回特定成员名称的方法。</remarks>
		public static string GetName<TValue>(Type enumType, TValue value)
		{
			if (enumType == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (!enumType.IsValueType)
			{
				throw CommonExceptions.MustBeEnum("type", enumType);
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			ulong ulValue = ToUInt64(value);
			EnumCache cache = GetEnumCache(enumType);
			int idx = Array.BinarySearch(cache.Values, ulValue);
			return idx >= 0 ? cache.Names[idx] : null;
		}

		#region 枚举描述

		/// <summary>
		/// 在指定枚举中检索具有指定值的常数的描述。如果不存在描述，则使用其名称。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">特定枚举常数的值（与 <paramref name="enumType"/> 兼容）。 </param>
		/// <returns>一个字符串，其中包含 <paramref name="enumType"/> 中值为 <paramref name="value"/> 
		/// 的枚举常数的描述；如果没有找到这样的常数，则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="enumType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="enumType"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidCastException"><paramref name="value"/> 不能转换为 <paramref name="enumType"/>。</exception>
		/// <remarks>如果多个枚举成员具有相同的基础值，则 <see cref="GetName{TValue}"/> 
		/// 方法可保证它将返回其中一个枚举成员的描述。但是，它并不保证它将始终返回相同枚举成员的描述。
		/// 因此，如果多个枚举成员具有相同的值，应用程序代码决不应依赖于返回特定成员描述的方法。</remarks>
		public static string GetDescription(Type enumType, object value)
		{
			if (enumType == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (!enumType.IsValueType)
			{
				throw CommonExceptions.MustBeEnum("type", enumType);
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			ulong ulValue = ToUInt64(value);
			EnumCache cache = GetEnumCache(enumType);
			int idx = Array.BinarySearch(cache.Values, ulValue);
			return idx >= 0 ? cache.Descriptions[idx] : null;
		}
		/// <summary>
		/// 在指定枚举中检索具有指定值的常数的描述。如果不存在描述，则使用其名称。
		/// </summary>
		/// <typeparam name="TValue">常数的类型。</typeparam>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">特定枚举常数的值（与 <paramref name="enumType"/> 兼容）。 </param>
		/// <returns>一个字符串，其中包含 <paramref name="enumType"/> 中值为 <paramref name="value"/> 
		/// 的枚举常数的描述；如果没有找到这样的常数，则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="enumType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="enumType"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidCastException"><paramref name="value"/> 不能转换为 <paramref name="enumType"/>。</exception>
		/// <remarks>如果多个枚举成员具有相同的基础值，则 <see cref="GetName{TValue}"/> 
		/// 方法可保证它将返回其中一个枚举成员的描述。但是，它并不保证它将始终返回相同枚举成员的描述。
		/// 因此，如果多个枚举成员具有相同的值，应用程序代码决不应依赖于返回特定成员描述的方法。</remarks>
		public static string GetDescription<TValue>(Type enumType, TValue value)
		{
			if (enumType == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (!enumType.IsValueType)
			{
				throw CommonExceptions.MustBeEnum("type", enumType);
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			ulong ulValue = ToUInt64(value);
			EnumCache cache = GetEnumCache(enumType);
			int idx = Array.BinarySearch(cache.Values, ulValue);
			return idx >= 0 ? cache.Descriptions[idx] : null;
		}
		/// <summary>
		/// 返回指定枚举值的描述（通过 <see cref="DescriptionAttribute"/> 指定）。
		/// 如果没有指定描述，则返回枚举常数的名称，没有找到枚举常数则返回枚举值。
		/// </summary>
		/// <param name="value">要获取描述的枚举值。</param>
		/// <returns>指定枚举值的描述。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static string ToDescription(this Enum value)
		{
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<string>() != null);
			Type enumType = value.GetType();
			ulong ulongValue = ToUInt64((object)value);
			// 寻找枚举值的组合。
			EnumCache cache = GetEnumCache(enumType);
			int idx = Array.BinarySearch(cache.Values, ulongValue);
			string[] names = cache.Descriptions;
			if (idx >= 0)
			{
				// 枚举值已定义，直接返回相应的名称。
				return names[idx];
			}
			// 不是可组合的枚举，直接返回枚举值的字符串形式。
			if (!cache.HasFlags)
			{
				return ToStringValue(enumType, ulongValue);
			}
			List<string> list = new List<string>();
			// 从后向前寻找匹配的二进制。
			for (int i = cache.Values.Length - 1; i >= 0 && ulongValue != 0UL; i--)
			{
				ulong enumValue = cache.Values[i];
				if (enumValue == 0UL)
				{
					continue;
				}
				if ((ulongValue & enumValue) == enumValue)
				{
					ulongValue -= enumValue;
					list.Add(names[i]);
				}
			}
			list.Reverse();
			// 添加最后剩余的未定义值。
			if (list.Count == 0 || ulongValue != 0UL)
			{
				list.Add(ToStringValue(enumType, ulongValue));
			}
			return string.Join(", ", list);
		}
		/// <summary>
		/// 返回指定枚举值的字符串形式，一般是由于枚举值没有对应的定义。
		/// </summary>
		/// <param name="enumType">要获取描述的枚举值类型。</param>
		/// <param name="value">要获取描述的枚举值。</param>
		/// <returns>指定枚举值的字符串形式。</returns>
		private static string ToStringValue(Type enumType, ulong value)
		{
			Contract.Requires(enumType != null);
			Contract.Ensures(Contract.Result<string>() != null);
			// 最高位不为 0，需要根据原先的类型是否是有符号数字，决定是否输出负号。
			if (value <= 0x8000000000000000UL || enumType.IsUnsigned())
			{
				return value.ToString(CultureInfo.InvariantCulture);
			}
			long longValue = unchecked((long)value);
			return longValue.ToString(CultureInfo.InvariantCulture);
		}

		#endregion // 枚举描述

		#region 枚举值

		/// <summary>
		/// 检索指定枚举类型中常数值的数组。
		/// </summary>
		/// <typeparam name="TEnum">要获取常数值的枚举类型。</typeparam>
		/// <returns>一个数组，其中包含 <typeparamref name="TEnum"/> 中实例的值。
		/// 该数组的元素按枚举常数的二进制值排序。</returns>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> 不是 <see cref="Enum"/>。</exception>
		public static TEnum[] GetValues<TEnum>()
		{
			return Enum.GetValues(typeof(TEnum)) as TEnum[];
		}
		/// <summary>
		/// 检索指定枚举类型中常数名称和值的集合。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <returns>一个 <see cref="TextValuePairCollection"/> 实例，其中包含 
		/// <paramref name="enumType"/> 中实例的名称和值。该集合的元素按枚举的二进制值排序。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="enumType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="enumType"/> 不是 <see cref="Enum"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 检索指定枚举类型中常数名称和值的集合。
		/// </summary>
		/// </overloads>
		public static TextValuePairCollection GetNameValues(Type enumType)
		{
			if (enumType == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (!enumType.IsEnum)
			{
				throw CommonExceptions.MustBeEnum("type", enumType);
			}
			Contract.Ensures(Contract.Result<TextValuePairCollection>() != null);
			return GetTextValues(enumType, false);
		}
		/// <summary>
		/// 检索指定枚举类型中常数名称和值的集合。
		/// </summary>
		/// <typeparam name="TEnum">枚举类型。</typeparam>
		/// <returns>一个 <see cref="TextValuePairCollection"/> 实例，其中包含 
		/// <typeparamref name="TEnum"/> 中实例的名称和值。该集合的元素按枚举的二进制值排序。</returns>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> 不是 <see cref="Enum"/>。</exception>
		public static TextValuePairCollection<TEnum> GetNameValues<TEnum>()
		{
			if (!typeof(TEnum).IsEnum)
			{
				throw CommonExceptions.MustBeEnum("TEnum", typeof(TEnum));
			}
			Contract.Ensures(Contract.Result<TextValuePairCollection<TEnum>>() != null);
			return GetTextValues<TEnum>(false);
		}
		/// <summary>
		/// 检索指定枚举类型中常数描述和值的集合。如果不存在描述，则使用其名称。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <returns>一个 <see cref="TextValuePairCollection"/> 实例，其中包含 
		/// <paramref name="enumType"/> 中实例的描述和值。该集合的元素按枚举的二进制值排序。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="enumType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="enumType"/> 不是 <see cref="Enum"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 检索指定枚举类型中常数描述和值的集合。
		/// </summary>
		/// </overloads>
		public static TextValuePairCollection GetDescValues(Type enumType)
		{
			if (enumType == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (!enumType.IsEnum)
			{
				throw CommonExceptions.MustBeEnum("type", enumType);
			}
			Contract.Ensures(Contract.Result<TextValuePairCollection>() != null);
			return GetTextValues(enumType, true);
		}
		/// <summary>
		/// 检索指定枚举类型中常数描述和值的集合。如果不存在描述，则使用其名称。
		/// </summary>
		/// <typeparam name="TEnum">枚举类型。</typeparam>
		/// <returns>一个 <see cref="TextValuePairCollection"/> 实例，其中包含 
		/// <typeparamref name="TEnum"/> 中实例的名称和值。该集合的元素按枚举的二进制值排序。</returns>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> 不是 <see cref="Enum"/>。</exception>
		public static TextValuePairCollection<TEnum> GetDescValues<TEnum>()
		{
			if (!typeof(TEnum).IsEnum)
			{
				throw CommonExceptions.MustBeEnum("TEnum", typeof(TEnum));
			}
			Contract.Ensures(Contract.Result<TextValuePairCollection<TEnum>>() != null);
			return GetTextValues<TEnum>(true);
		}
		/// <summary>
		/// 检索指定枚举类型中常数文本和值的集合。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="useDescription">是否读取枚举值的描述信息作为文本。</param>
		/// <returns>一个 <see cref="TextValuePairCollection"/> 实例，其中包含 
		/// <paramref name="enumType"/> 中实例的文本和值。该集合的元素按枚举的二进制值排序。</returns>
		private static TextValuePairCollection GetTextValues(Type enumType, bool useDescription)
		{
			Contract.Requires(enumType != null && enumType.IsEnum);
			Contract.Ensures(Contract.Result<TextValuePairCollection>() != null);
			TextValuePairCollection list = new TextValuePairCollection();
			EnumCache cache = GetEnumCache(enumType);
			ulong[] values = cache.Values;
			string[] names = useDescription ? cache.Descriptions : cache.Names;
			Contract.Assume(names != null);
			Contract.Assume(names.Length == values.Length);
			Converter<object, object> converter = Convert.GetConverter(typeof(ulong), enumType);
			for (int i = 0; i < values.Length; i++)
			{
				list.Add(names[i], converter(values[i]));
			}
			return list;
		}
		/// <summary>
		/// 检索指定枚举类型中常数文本和值的集合。
		/// </summary>
		/// <param name="useDescription">是否读取枚举值的描述信息作为文本。</param>
		/// <returns>一个 <see cref="TextValuePairCollection"/> 实例，其中包含 
		/// <typeparamref name="TEnum"/> 中实例的文本和值。该集合的元素按枚举的二进制值排序。</returns>
		private static TextValuePairCollection<TEnum> GetTextValues<TEnum>(bool useDescription)
		{
			Contract.Requires(typeof(TEnum).IsEnum);
			Contract.Ensures(Contract.Result<TextValuePairCollection<TEnum>>() != null);
			TextValuePairCollection<TEnum> enumList = new TextValuePairCollection<TEnum>();
			EnumCache cache = GetEnumCache(typeof(TEnum));
			ulong[] values = cache.Values;
			string[] names = useDescription ? cache.Descriptions : cache.Names;
			Contract.Assume(names != null);
			Contract.Assume(names.Length == values.Length);
			Converter<ulong, TEnum> converter = Convert.GetConverter<ulong, TEnum>();
			for (int i = 0; i < values.Length; i++)
			{
				enumList.Add(names[i], converter(values[i]));
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
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。
		/// </exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		/// <overloads>
		/// <summary>
		/// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// </overloads>
		public static TEnum Parse<TEnum>(string value)
		{
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			return (TEnum)Enum.Parse(typeof(TEnum), value);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象。
		/// 一个参数指定该操作是否区分大小写。
		/// </summary>
		/// <typeparam name="TEnum">要获取枚举对象的枚举类型。</typeparam>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <param name="ignoreCase"><c>true</c> 为忽略大小写；<c>false</c> 为考虑大小写。</param>
		/// <returns><typeparamref name="TEnum"/> 类型的对象，其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> 不是 <see cref="System.Enum"/>。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。
		/// </exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		public static TEnum Parse<TEnum>(string value, bool ignoreCase)
		{
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <returns><paramref name="enumType"/> 类型的对象，其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="enumType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="enumType"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是一个名称或描述，但不是为该枚举定义的命名常量之一。
		/// </exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 超出 <paramref name="enumType"/>
		/// 基础类型的范围。</exception>
		/// <overloads>
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// </overloads>
		public static object ParseEx(Type enumType, string value)
		{
			if (enumType == null)
			{
				throw CommonExceptions.ArgumentNull("enumType");
			}
			if (!enumType.IsEnum)
			{
				throw CommonExceptions.MustBeEnum("enumType", enumType);
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			ulong ulValue = ParseToULong(enumType, value, false);
			if (enumType.IsUnsigned())
			{
				return Convert.ChangeType(ulValue, enumType);
			}
			return Convert.ChangeType((long)ulValue, enumType);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// 一个参数指定该操作是否区分大小写。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <param name="ignoreCase"><c>true</c> 为忽略大小写；<c>false</c> 为考虑大小写。</param>
		/// <returns><paramref name="enumType"/> 类型的对象，其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="enumType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="enumType"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是一个名称或描述，但不是为该枚举定义的命名常量之一。
		/// </exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 超出 <paramref name="enumType"/>
		/// 基础类型的范围。</exception>
		public static object ParseEx(Type enumType, string value, bool ignoreCase)
		{
			if (enumType == null)
			{
				throw CommonExceptions.ArgumentNull("enumType");
			}
			if (!enumType.IsEnum)
			{
				throw CommonExceptions.MustBeEnum("enumType", enumType);
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			ulong ulValue = ParseToULong(enumType, value, ignoreCase);
			if (enumType.IsUnsigned())
			{
				return Convert.ChangeType(ulValue, enumType);
			}
			return Convert.ChangeType((long)ulValue, enumType);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// </summary>
		/// <typeparam name="TEnum">要获取枚举对象的枚举类型。</typeparam>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <returns><typeparamref name="TEnum"/> 类型的对象，其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是一个名称或描述，但不是为该枚举定义的命名常量之一。
		/// </exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		public static TEnum ParseEx<TEnum>(string value)
		{
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (!typeof(TEnum).IsEnum)
			{
				throw CommonExceptions.MustBeEnum("TEnum", typeof(TEnum));
			}
			Contract.EndContractBlock();
			ulong ulValue = ParseToULong(typeof(TEnum), value, false);
			if (typeof(TEnum).IsUnsigned())
			{
				return Convert.ChangeType<ulong, TEnum>(ulValue);
			}
			return Convert.ChangeType<long, TEnum>((long)ulValue);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举对象。
		/// 一个参数指定该操作是否区分大小写。
		/// </summary>
		/// <typeparam name="TEnum">要获取枚举对象的枚举类型。</typeparam>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <param name="ignoreCase"><c>true</c> 为忽略大小写；<c>false</c> 为考虑大小写。</param>
		/// <returns><typeparamref name="TEnum"/> 类型的对象，其值由 <paramref name="value"/> 表示。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TEnum"/> 不是 <see cref="Enum"/>。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是一个名称或描述，但不是为该枚举定义的命名常量之一。
		/// </exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 超出 <typeparamref name="TEnum"/> 
		/// 基础类型的范围。</exception>
		public static TEnum ParseEx<TEnum>(string value, bool ignoreCase)
		{
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (!typeof(TEnum).IsEnum)
			{
				throw CommonExceptions.MustBeEnum("TEnum", typeof(TEnum));
			}
			Contract.EndContractBlock();
			ulong ulValue = ParseToULong(typeof(TEnum), value, ignoreCase);
			if (typeof(TEnum).IsUnsigned())
			{
				return Convert.ChangeType<ulong, TEnum>(ulValue);
			}
			return Convert.ChangeType<long, TEnum>((long)ulValue);
		}
		/// <summary>
		/// 将一个或多个枚举常数的名称、描述或数字值的字符串表示转换成等效的枚举值。
		/// 一个参数指定该操作是否区分大小写。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">包含要转换的值或名称的字符串。</param>
		/// <param name="ignoreCase">若要忽略大小写则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <returns><see cref="ulong"/> 表示的枚举值。</returns>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是空字符串 ("") 或只包含空白。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 是一个名称，但不是为该枚举定义的命名常量之一。
		/// </exception>
		/// <exception cref="OverflowException"><paramref name="value"/> 超出 <paramref name="enumType"/> 
		/// 基础类型的范围。</exception>
		private static ulong ParseToULong(Type enumType, string value, bool ignoreCase)
		{
			Contract.Requires(enumType != null && enumType.IsEnum);
			Contract.Requires(value != null);
			value = value.Trim();
			if (value.Length == 0)
			{
				throw CommonExceptions.MustContainEnumInfo("value");
			}
			// 尝试对数字进行解析，这样可避免之后的字符串比较。
			ulong tmpValue;
			if (TryParseString(value, out tmpValue))
			{
				return tmpValue;
			}
			// 尝试对描述信息进行解析。
			EnumCache cache = GetEnumCache(enumType);
			StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			ulong ulValue = 0UL;
			int start = 0;
			do
			{
				// 去除前导空白。
				while (char.IsWhiteSpace(value, start)) { start++; }
				int idx = value.IndexOf(',', start);
				if (idx < 0) { idx = value.Length; }
				if (idx == start)
				{
					start = idx + 1;
					continue;
				}
				int nIdx = idx - 1;
				// 去除后面的空白。
				while (char.IsWhiteSpace(value, nIdx)) { nIdx--; }
				string str = value.Substring(start, nIdx - start + 1);
				start = idx + 1;
				// 尝试识别为名称、描述或数字。
				if (!TryParseString(str, cache, comparison, ref tmpValue) &&
					!TryParseString(str, out tmpValue))
				{
					throw CommonExceptions.EnumValueNotFound(enumType, str);
				}
				ulValue |= tmpValue;
			} while (start < value.Length);
			return ulValue;
		}
		/// <summary>
		/// 解析给定的数字字符串，并返回对应的值。
		/// </summary>
		/// <param name="str">要解析的数字字符串。</param>
		/// <param name="value">字符串对应的值。</param>
		/// <returns>数字字符串的解析是否成功。</returns>
		private static bool TryParseString(string str, out ulong value)
		{
			Contract.Requires(str != null & str.Length > 0);
			char firstChar = str[0];
			if (char.IsDigit(firstChar) || firstChar == '+')
			{
				return ulong.TryParse(str, out value);
			}
			if (firstChar == '-')
			{
				long valueL;
				if (long.TryParse(str, out valueL))
				{
					value = unchecked((ulong)valueL);
					return true;
				}
			}
			value = 0UL;
			return false;
		}
		/// <summary>
		/// 解析给定的数字字符串，并返回对应的值。
		/// </summary>
		/// <param name="str">要解析的数字字符串。</param>
		/// <param name="cache">枚举的缓存。</param>
		/// <param name="comparison">字符串比较。</param>
		/// <param name="value">字符串对应的值。</param>
		/// <returns>数字字符串的解析是否成功。</returns>
		private static bool TryParseString(string str, EnumCache cache, StringComparison comparison, ref ulong value)
		{
			Contract.Requires(str != null && cache != null);
			// 比较常数值的名称。
			for (int i = 0; i < cache.Names.Length; i++)
			{
				if (string.Equals(str, cache.Names[i], comparison))
				{
					value = cache.Values[i];
					return true;
				}
			}
			if (!cache.HasDescription)
			{
				return false;
			}
			// 比较常数值的描述信息。
			for (int i = 0; i < cache.Descriptions.Length; i++)
			{
				if (string.Equals(str, cache.Descriptions[i], comparison))
				{
					value = cache.Values[i];
					return true;
				}
			}
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
		/// <exception cref="ArgumentNullException"><paramref name="baseEnum"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		public static bool AnyFlag(this Enum baseEnum, Enum value)
		{
			if (baseEnum == null)
			{
				throw CommonExceptions.ArgumentNull("baseEnum");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.EndContractBlock();
			if (baseEnum.GetType() != value.GetType())
			{
				throw CommonExceptions.EnumTypeDoesNotMatch("value", value.GetType(), baseEnum.GetType());
			}
			return ((ToUInt64(baseEnum) & ToUInt64(value)) != 0);
		}
		/// <summary>
		/// 获取 <see cref="ulong"/> 类型的枚举值，<see cref="long"/> 会隐式转换为 <see cref="ulong"/>。
		/// </summary>
		/// <param name="value">要获取的枚举值。</param>
		/// <returns><see cref="ulong"/> 类型的枚举值。</returns>
		/// <exception cref="InvalidCastException"><paramref name="value"/> 不能转换为与 <see cref="ulong"/>
		/// 兼容的类型。</exception>
		private static ulong ToUInt64(object value)
		{
			Contract.Requires(value != null);
			// 除了 ulong，其它整数类型都可以隐式转换为 long。
			if (Type.GetTypeCode(value.GetType()) == TypeCode.UInt64)
			{
				return Convert.ChangeType<ulong>(value);
			}
			return unchecked((ulong)Convert.ChangeType<long>(value));
		}
		/// <summary>
		/// 获取 <see cref="ulong"/> 类型的枚举值，<see cref="long"/> 会隐式转换为 <see cref="ulong"/>。
		/// </summary>
		/// <typeparam name="TValue">值的类型。</typeparam>
		/// <param name="value">要获取的枚举值。</param>
		/// <returns><see cref="ulong"/> 类型的枚举值。</returns>
		/// <exception cref="InvalidCastException"><paramref name="value"/> 不能转换为与 <see cref="ulong"/>
		/// 兼容的类型。</exception>
		private static ulong ToUInt64<TValue>(TValue value)
		{
			// 除了 ulong，其它整数类型都可以隐式转换为 long。
			if (Type.GetTypeCode(typeof(TValue)) == TypeCode.UInt64)
			{
				return Convert.ChangeType<TValue, ulong>(value);
			}
			return unchecked((ulong)Convert.ChangeType<TValue, long>(value));
		}

		#region 枚举缓存

		/// <summary>
		/// 枚举类型的缓存。
		/// </summary>
		private static readonly ICache<Type, EnumCache> enumCaches =
			CacheFactory.Create<Type, EnumCache>("Cyjb.EnumDescriptionCache") ??
			new LruCache<Type, EnumCache>(256);
		/// <summary>
		/// 返回枚举的缓存。
		/// </summary>
		/// <param name="enumType">要获取缓存的枚举类型。</param>
		/// <value>枚举的缓存。</value>
		private static EnumCache GetEnumCache(Type enumType)
		{
			Contract.Requires(enumType != null);
			Contract.Ensures(Contract.Result<EnumCache>() != null);
			return enumCaches.GetOrAdd(enumType, type =>
			{
				// 返回枚举类型的值、常数名称和相应描述的列表。
				// 直接使用反射获取数据，而不是 Enum 类的相关方法。
				FieldInfo[] fields = type.GetFields(TypeExt.StaticFlag);
				int length = fields.Length;
				ulong[] values = new ulong[length];
				for (int i = 0; i < length; i++)
				{
					values[i] = ToUInt64(fields[i].GetRawConstantValue());
				}
				// 按照二进制大小排序。
				Array.Sort(values, fields);
				// 获取常数名称和相应描述。
				string[] names = new string[length];
				string[] descs = new string[length];
				bool hasDesc = false;
				for (int i = 0; i < length; i++)
				{
					FieldInfo fieldInfo = fields[i];
					// 获取常数名称。
					names[i] = descs[i] = fieldInfo.Name;
					// 尝试获取描述。
					object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
					for (int j = 0; j < attrs.Length; j++)
					{
						DescriptionAttribute desc = attrs[j] as DescriptionAttribute;
						if (desc != null)
						{
							descs[i] = desc.Description;
							hasDesc = true;
							break;
						}
					}
				}
				// 如果不包含描述信息，直接将描述信息指向常数名称。
				if (!hasDesc)
				{
					descs = names;
				}
				// 将是否包含 FlagsAttribute 一并缓存，能有效的提高性能。
				bool hasFlags = type.IsDefined(typeof(FlagsAttribute), false);
				return new EnumCache(hasFlags, hasDesc, values, names, descs);
			});
		}
		/// <summary>
		/// 表示枚举缓存。
		/// </summary>
		private sealed class EnumCache
		{
			/// <summary>
			/// 枚举值的列表，从小到大排序。
			/// </summary>
			public readonly ulong[] Values;
			/// <summary>
			/// 枚举描述的列表，与枚举值列表一一对应。
			/// </summary>
			public readonly string[] Descriptions;
			/// <summary>
			/// 枚举名称的列表，与枚举值列表一一对应。
			/// </summary>
			public readonly string[] Names;
			/// <summary>
			/// 枚举类是否含有 <see cref="FlagsAttribute"/>。
			/// </summary>
			public readonly bool HasFlags;
			/// <summary>
			/// 枚举类是否定义了 <see cref="DescriptionAttribute"/>。
			/// </summary>
			public readonly bool HasDescription;
			/// <summary>
			/// 使用给定的值初始化 <see cref="EnumCache"/> 结构的新实例。
			/// </summary>
			/// <param name="hasFlags">枚举类是否含有 <see cref="FlagsAttribute"/>。</param>
			/// <param name="hasDesc">枚举类是否定义了 <see cref="DescriptionAttribute"/>。</param>
			/// <param name="values">枚举值的列表，从小到大排序。</param>
			/// <param name="names">枚举名称的列表，与枚举值列表一一对应。</param>
			/// <param name="descs">枚举描述的列表，与枚举值列表一一对应。</param>
			public EnumCache(bool hasFlags, bool hasDesc, ulong[] values, string[] names, string[] descs)
			{
				Contract.Requires(values != null && names != null && descs != null &&
					values.Length == names.Length && values.Length == descs.Length);
				this.HasFlags = hasFlags;
				this.HasDescription = hasDesc;
				this.Values = values;
				this.Names = names;
				this.Descriptions = descs;
			}
		}

		#endregion // 枚举缓存

	}
}
