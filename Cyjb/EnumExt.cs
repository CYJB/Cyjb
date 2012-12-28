using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.Enum"/> 类的扩展方法。
	/// </summary>
	public static class EnumExt
	{

		#region 枚举描述

		/// <summary>
		/// 返回指定枚举值的描述（通过 
		/// <see cref="System.ComponentModel.DescriptionAttribute"/> 指定）。
		/// 如果没有指定描述，则返回枚举常数的名称，没有找到枚举常数则返回枚举值。
		/// </summary>
		/// <param name="value">要获取描述的枚举值。</param>
		/// <returns>指定枚举值的描述。</returns>
		public static string GetDescription(this Enum value)
		{
			Type enumType = value.GetType();
			// 寻找枚举值的组合。
			EnumCache cache = GetEnumCache(enumType.TypeHandle);
			ulong valueUL = ToUInt64(value);
			int idx = Array.BinarySearch(cache.Values, valueUL);
			if (idx >= 0)
			{
				// 枚举值已定义，直接返回相应的描述。
				return cache.Descriptions[idx];
			}
			// 不是可组合的枚举，直接返回枚举值得字符串形式。
			if (!cache.HasFlagsAttribute)
			{
				return GetStringValue(enumType, valueUL);
			}
			List<string> list = new List<string>();
			// 从后向前寻找匹配的二进制。
			for (int i = cache.Values.Length - 1; i >= 0 && valueUL != 0UL; i--)
			{
				ulong enumValue = cache.Values[i];
				if (enumValue == 0UL)
				{
					continue;
				}
				if ((valueUL & enumValue) == enumValue)
				{
					valueUL -= enumValue;
					list.Add(cache.Descriptions[i]);
				}
			}
			list.Reverse();
			// 添加最后剩余的未定义值。
			if (list.Count == 0 || valueUL != 0UL)
			{
				list.Add(GetStringValue(enumType, valueUL));
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
			Array array = Enum.GetValues(enumType);
			string[] names = null;
			if (useDescription)
			{
				names = GetEnumCache(enumType.TypeHandle).Descriptions;
			}
			else
			{
				names = Enum.GetNames(enumType);
			}
			for (int i = 0; i < array.Length; i++)
			{
				enumList.Add(names[i], array.GetValue(i));
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
			Array array = Enum.GetValues(enumType);
			string[] names = null;
			if (useDescription)
			{
				names = GetEnumCache(enumType.TypeHandle).Descriptions;
			}
			else
			{
				names = Enum.GetNames(enumType);
			}
			for (int i = 0; i < array.Length; i++)
			{
				enumList.Add(names[i], (TEnum)array.GetValue(i));
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
			ExceptionHelper.CheckArgumentNull(enumType, "enumType");
			ExceptionHelper.CheckArgumentNull(value, "value");
			if (!enumType.IsEnum)
			{
				throw ExceptionHelper.MustBeEnum(enumType);
			}
			if (value.Length == 0)
			{
				throw ExceptionHelper.MustContainEnumInfo();
			}
			// 尝试对数字进行解析，这样可避免之后的字符串比较。
			char firstChar = value[0];
			long valueL;
			if (char.IsDigit(firstChar) || firstChar == '+' || firstChar == '-')
			{
				if (long.TryParse(value, out valueL))
				{
					return Enum.ToObject(enumType, valueL);
				}
			}
			// 尝试对描述信息进行解析。
			EnumCache cache = GetEnumCache(enumType.TypeHandle);
			// 描述信息可能是语言相关的，这里采用当前区域文化信息比较字符串。
			StringComparison comparison = ignoreCase ?
				StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
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
				// 比较描述信息。
				if (nIdx > start)
				{
					string str = value.Substring(start, nIdx - start + 1);
					int j = 0;
					for (; j < cache.Descriptions.Length; j++)
					{
						if (string.Equals(str, cache.Descriptions[j], comparison))
						{
							// 与描述信息匹配。
							valueUL |= cache.Values[j];
							break;
						}
					}
					// 未识别的枚举值。
					if (j == cache.Descriptions.Length)
					{
						// 尝试识别为数字。
						if (long.TryParse(str, out valueL))
						{
							valueUL |= unchecked((ulong)valueL);
						}
						else
						{
							// 不能识别为数字。
							throw ExceptionHelper.EnumValueNotFound(enumType, str);
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
				throw ExceptionHelper.EnumTypeDoesNotMatch(value.GetType(), baseEnum.GetType());
			}
			return ((ToUInt64(baseEnum) & ToUInt64(value)) != 0);
		}
		/// <summary>
		/// 获取 <see cref="System.UInt64"/> 类型的枚举值。
		/// </summary>
		/// <param name="value">要获取的枚举值。</param>
		/// <returns><see cref="System.UInt64"/> 类型的枚举值。</returns>
		private static ulong ToUInt64(Enum value)
		{
			return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
		}

		#region 枚举缓存

		/// <summary>
		/// 枚举类型的缓存。
		/// </summary>
		private static readonly ICache<RuntimeTypeHandle, EnumCache> EnumCaches =
			CacheFactory.CreateCache<RuntimeTypeHandle, EnumCache>("Cyjb.EnumDescriptionCache") ??
			new LruCache<RuntimeTypeHandle, EnumCache>(100);
		/// <summary>
		/// 返回枚举的缓存。
		/// </summary>
		/// <param name="typeHandle">要获取缓存的枚举类型。</param>
		/// <value>枚举的缓存。</value>
		private static EnumCache GetEnumCache(RuntimeTypeHandle typeHandle)
		{
			return EnumCaches.GetOrAdd(typeHandle, type =>
			{
				// 返回枚举类型的值和相应描述的列表。
				Type enumType = Type.GetTypeFromHandle(type);
				Array array = Enum.GetValues(enumType);
				string[] names = Enum.GetNames(enumType);
				int len = array.Length;
				ulong[] values = new ulong[len];
				for (int i = 0; i < len; i++)
				{
					// 获取 ulong 类型的值。
					values[i] = ToUInt64((Enum)array.GetValue(i));
					// 获取对应的描述。
					FieldInfo fieldInfo = enumType.GetField(names[i]);
					// 没有描述的直接使用枚举名称。
					if (fieldInfo != null)
					{
						DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
							typeof(DescriptionAttribute), false) as DescriptionAttribute;
						if (attr != null)
						{
							names[i] = attr.Description;
						}
					}
				}
				// 将是否包含 FlagsAttribute 一并缓存，能有效的提高性能。
				return new EnumCache(enumType.IsDefined(typeof(FlagsAttribute), false), values, names);
			});
		}
		/// <summary>
		/// 表示枚举缓存。
		/// </summary>
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
			/// 枚举类是否含有 FlagsAttribute。
			/// </summary>
			public bool HasFlagsAttribute;
			/// <summary>
			/// 使用给定的值初始化 <see cref="EnumCache"/> 结构的新实例。
			/// </summary>
			/// <param name="hasFlags">枚举类是否含有 FlagsAttribute。</param>
			/// <param name="values">枚举值的列表，从小到大排序。</param>
			/// <param name="descs">枚举描述的列表，与枚举值列表一一对应。</param>
			public EnumCache(bool hasFlags, ulong[] values, string[] descs)
			{
				this.HasFlagsAttribute = hasFlags;
				this.Values = values;
				this.Descriptions = descs;
			}
		}

		#endregion // 枚举缓存

	}
}
