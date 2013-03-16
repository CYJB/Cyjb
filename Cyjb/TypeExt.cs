using System;
using System.Collections.Generic;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.Type"/> 类的扩展方法。
	/// </summary>
	public static class TypeExt
	{

		#region 是否可以进行隐式类型转换

		/// <summary>
		/// 可以进行隐式类型转换的类型字典。
		/// </summary>
		internal static readonly Dictionary<TypeCode, HashSet<TypeCode>> ImplicitNumericConversions =
			new Dictionary<TypeCode, HashSet<TypeCode>>() { 
				{ TypeCode.Int16, new HashSet<TypeCode>(){ TypeCode.SByte, TypeCode.Byte } },
				{ TypeCode.UInt16, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.Byte } },
				{ TypeCode.Int32, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.SByte, TypeCode.Byte,
					TypeCode.Int16, TypeCode.UInt16 } },
				{ TypeCode.UInt32, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.Byte, TypeCode.UInt16 } },
				{ TypeCode.Int64, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.SByte, TypeCode.Byte, 
					TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32 } },
				{ TypeCode.UInt64, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.Byte, TypeCode.UInt16, TypeCode.UInt32 } },
				{ TypeCode.Single, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.SByte, TypeCode.Byte, 
					TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64 } },
				{ TypeCode.Double, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.SByte, TypeCode.Byte, TypeCode.Int16,
					TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single } },
				{ TypeCode.Decimal, new HashSet<TypeCode>(){ TypeCode.Char, TypeCode.SByte, TypeCode.Byte,
					TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64 } },
		};
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行标准隐式类型转换。
		/// </summary>
		/// <param name="type">要判断的实例。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例进行标准隐式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool IsStandardImplicitFrom(this Type type, Type fromType)
		{
			// 对 Nullable<T> 的支持。
			if (!type.IsValueType || IsNullableType(ref type))
			{
				fromType = GetNonNullableType(fromType);
			}
			// 判断隐式数值转换。
			HashSet<TypeCode> typeSet;
			if (!type.IsEnum && ImplicitNumericConversions.TryGetValue(Type.GetTypeCode(type), out typeSet))
			{
				if (!fromType.IsEnum && typeSet.Contains(Type.GetTypeCode(fromType)))
				{
					return true;
				}
			}
			// 判断隐式引用转换和装箱转换。
			return type.IsAssignableFrom(fromType);
		}
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行隐式类型转换。
		/// </summary>
		/// <param name="type">要判断的实例。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例分配或进行隐式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsImplicitFrom(this Type type, Type fromType)
		{
			if (type == null || fromType == null)
			{
				return false;
			}
			// 对引用类型的支持。
			if (type.IsByRef) { type = type.GetElementType(); }
			if (fromType.IsByRef) { fromType = type.GetElementType(); }
			// 总是可以隐式类型转换为 Object。
			if (type.Equals(typeof(object)))
			{
				return true;
			}
			// 判断是否可以进行标准隐式转换。
			if (IsStandardImplicitFrom(type, fromType))
			{
				return true;
			}
			// 对隐式类型转换运算符进行判断。
			// 处理提升转换运算符。
			Type nonNullalbeType, nonNullableFromType;
			if (IsNullableType(type, out nonNullalbeType) &&
				IsNullableType(fromType, out nonNullableFromType))
			{
				type = nonNullalbeType;
				fromType = nonNullableFromType;
			}
			return ConversionCache.GetImplicitConversion(fromType, type) != null;
		}

		#endregion // 是否可以进行隐式类型转换

		#region 是否可以进行显式类型转换

		/// <summary>
		/// 可以进行显式数值转换或枚举转换的类型集合。
		/// </summary>
		internal static readonly HashSet<TypeCode> ExplicitNumericConversions = new HashSet<TypeCode>(){ 
				TypeCode.Char, TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32,
				TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal };
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行标准显式类型转换。
		/// </summary>
		/// <param name="type">要判断的实例。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例进行标准显式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool IsStandardExplicitFrom(this Type type, Type fromType)
		{
			// 判断显式数值转换。
			if (!type.IsEnum && ExplicitNumericConversions.Contains(Type.GetTypeCode(type)) &&
				!fromType.IsEnum && ExplicitNumericConversions.Contains(Type.GetTypeCode(fromType)))
			{
				return true;
			}
			// 判断正向和反向的隐式引用转换和装箱转换。
			return (type.IsAssignableFrom(fromType) || fromType.IsAssignableFrom(type));
		}
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行显式枚举转换。
		/// </summary>
		/// <param name="type">要判断的实例。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例进行显式枚举转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool IsEnumExplicitFrom(this Type type, Type fromType)
		{
			if (type.IsEnum)
			{
				if (fromType.IsEnum || ExplicitNumericConversions.Contains(Type.GetTypeCode(fromType)))
				{
					return true;
				}
			}
			else if (fromType.IsEnum && ExplicitNumericConversions.Contains(Type.GetTypeCode(type)))
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行强制类型转换。
		/// </summary>
		/// <param name="type">要判断的实例。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例分配或进行强制类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsExplicitFrom(this Type type, Type fromType)
		{
			if (type == null || fromType == null)
			{
				return false;
			}
			// 对引用类型的支持。
			if (type.IsByRef) { type = type.GetElementType(); }
			if (fromType.IsByRef) { fromType = type.GetElementType(); }
			// 总是可以与 Object 进行显示类型转换。
			if (type.Equals(typeof(object)) || fromType.Equals(typeof(object)))
			{
				return true;
			}
			// 对 Nullable<T> 的支持。
			IsNullableType(ref type);
			IsNullableType(ref fromType);
			// 显式枚举转换。
			if (type.IsEnumExplicitFrom(fromType))
			{
				return true;
			}
			// 判断是否可以进行标准显式转换。
			if (IsStandardExplicitFrom(type, fromType))
			{
				return true;
			}
			// 对显式类型转换运算符进行判断。
			return ConversionCache.GetExplicitConversion(fromType, type) != null;
		}

		#endregion // 是否可以进行显式类型转换

		#region 是否可分配到开放泛型类型

		/// <summary>
		/// 确定当前的开放泛型类型的实例是否可以从指定 <see cref="System.Type"/> 的实例分配。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前的开放泛型类型可以从 <paramref name="fromType"/>
		/// 的实例分配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool OpenGenericIsAssignableFrom(this Type type, Type fromType)
		{
			Type[] genericArguments;
			return OpenGenericIsAssignableFrom(type, fromType, out genericArguments);
		}
		/// <summary>
		/// 确定当前的开放泛型类型的实例是否可以从指定 <see cref="System.Type"/> 的实例分配，并返回泛型的参数。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <param name="genericArguments">如果可以分配到开放泛型类型，则返回泛型类型参数；否则返回 <c>null</c>。</param>
		/// <returns>如果当前的开放泛型类型可以从 <paramref name="fromType"/> 的实例分配，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool OpenGenericIsAssignableFrom(this Type type, Type fromType, out Type[] genericArguments)
		{
			if (type != null && fromType != null && type.IsGenericTypeDefinition)
			{
				if (type.IsInterface == fromType.IsInterface)
				{
					if (type.InInheritanceChain(fromType, out genericArguments))
					{
						return true;
					}
				}
				if (type.IsInterface)
				{
					// 查找实现的接口。
					Type[] interfaces = fromType.GetInterfaces();
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (type.InInheritanceChain(interfaces[i], out genericArguments))
						{
							return true;
						}
					}
				}
			}
			genericArguments = null;
			return false;
		}
		/// <summary>
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable&lt;&gt; 泛型。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool IsNullableType(this Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}
		/// <summary>
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable&lt;&gt; 泛型，如果是则返回泛型的参数。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。</param>
		/// <param name="nonNullalbeType">如果 <paramref name="type"/> 是可空类型，则返回泛型类型参数。
		/// 否则为 <paramref name="type"/>。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool IsNullableType(this Type type, out Type nonNullalbeType)
		{
			if (IsNullableType(type))
			{
				nonNullalbeType = type.GetGenericArguments()[0];
				return true;
			}
			else
			{
				nonNullalbeType = type;
				return false;
			}
		}
		/// <summary>
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable&lt;&gt; 泛型，如果是则返回泛型的参数。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。如果是可空类型，则返回泛型类型参数；否则不变。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool IsNullableType(ref Type type)
		{
			if (IsNullableType(type))
			{
				type = type.GetGenericArguments()[0];
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable&lt;&gt; 泛型，如果是则返回泛型的参数。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为泛型的参数；否则为原类型。</returns>
		internal static Type GetNonNullableType(this Type type)
		{
			if (IsNullableType(type))
			{
				return type.GetGenericArguments()[0];
			}
			return type;
		}
		/// <summary>
		/// 确定当前的开放泛型类型是否在指定 <see cref="System.Type"/> 类型的继承链中，并返回泛型的参数。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <param name="genericArguments">如果在继承链中，则返回泛型类型参数；否则返回 <c>null</c>。</param>
		/// <returns>如果当前的开放泛型类型在 <paramref name="fromType"/> 的继承链中，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool InInheritanceChain(this Type type, Type fromType, out Type[] genericArguments)
		{
			// 沿着 fromType 的继承链向上查找。
			while (fromType != null)
			{
				if (fromType.IsGenericType)
				{
					genericArguments = fromType.GetGenericArguments();
					if (genericArguments.Length == type.GetGenericArguments().Length)
					{
						try
						{
							Type closedType = type.MakeGenericType(genericArguments);
							if (closedType.IsAssignableFrom(fromType))
							{
								return true;
							}
						}
						catch (ArgumentException)
						{
							// 不满足参数的约束。
						}
					}
				}
				fromType = fromType.BaseType;
			}
			genericArguments = null;
			return false;
		}

		#endregion // 是否可分配到开放泛型类型

	}
}
