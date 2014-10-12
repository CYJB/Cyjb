using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
			// 这里加入 IsEnum 的判断，是因为枚举的 TypeCode 是其基类型的 TypeCode，会导致判断失误。
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
		/// <remarks>
		/// 判断类型间能否进行隐式类型转换的算法来自于 《CSharp Language Specification》v5.0 
		/// 的第 6.1 节，更多信息可以参考我的博客文章 
		/// <see href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</see>。</remarks>
		/// <example>
		/// 下面是 <see cref="IsImplicitFrom"/> 方法与 <see cref="System.Type.IsAssignableFrom"/> 方法的一些对比。
		/// <code>
		/// Console.WriteLine(typeof(object).IsAssignableFrom(typeof(uint))); // True 
		/// Console.WriteLine(typeof(object).IsImplicitFrom(typeof(uint))); // True
		/// Console.WriteLine(typeof(int).IsAssignableFrom(typeof(short))); // False
		/// Console.WriteLine(typeof(int).IsImplicitFrom(typeof(short))); // True
		/// Console.WriteLine(typeof(long?).IsAssignableFrom(typeof(int?))); // False
		/// Console.WriteLine(typeof(long?).IsImplicitFrom(typeof(int?))); // True
		/// Console.WriteLine(typeof(long).IsAssignableFrom(typeof(TestClass))); // False
		/// Console.WriteLine(typeof(long).IsImplicitFrom(typeof(TestClass))); // True
		/// class TestClass {
		///     public static implicit operator int(TestClass t) { return 1; }
		/// }
		/// </code>
		/// </example>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		public static bool IsImplicitFrom(this Type type, Type fromType)
		{
			if (type == null || fromType == null)
			{
				return false;
			}
			// 标识转换。
			if (type == fromType) { return true; }
			// 对引用类型的支持。
			if (type.IsByRef) { type = type.GetElementType(); }
			if (fromType.IsByRef) { fromType = type.GetElementType(); }
			// 总是可以隐式类型转换为 Object。
			if (type.Equals(typeof(object))) { return true; }
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
			// 这里加入 IsEnum 的判断，是因为枚举的 TypeCode 是其基类型的 TypeCode，会导致判断失误。
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
		/// <remarks>
		/// 判断类型间能否进行显式类型转换的算法来自于 《CSharp Language Specification》v5.0 
		/// 的第 6.2 节，更多信息可以参考我的博客文章 
		/// <see href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</see>。</remarks>
		/// <example>
		/// 下面是 <see cref="IsImplicitFrom"/> 方法的一些实例。
		/// <code>
		/// Console.WriteLine(typeof(uint).IsExplicitFrom(typeof(object))); // True
		/// Console.WriteLine(typeof(short).IsExplicitFrom(typeof(int))); // True
		/// Console.WriteLine(typeof(int).IsExplicitFrom(typeof(long?))); // True
		/// Console.WriteLine(typeof(long).IsExplicitFrom(typeof(TestClass))); // True
		/// class TestClass {
		///     public static explicit operator int(TestClass t) { return 1; }
		/// }
		/// </code>
		/// </example>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		public static bool IsExplicitFrom(this Type type, Type fromType)
		{
			if (type == null || fromType == null)
			{
				return false;
			}
			// 标识转换。
			if (type == fromType) { return true; }
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
		/// <example>
		/// 下面是 <see cref="OpenGenericIsAssignableFrom(Type,Type)"/> 方法的简单示例：
		/// <code>
		/// Console.WriteLine(typeof(IEnumerable{}).UniqueOpenGenericIsAssignableFrom(
		///		typeof(List}int}))); // True
		/// </code>
		/// </example>
		/// <remarks>
		/// 关于判断是否可以分配到开放泛型类型的算法可以参考我的博客文章 
		/// <see href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</see>。</remarks>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		public static bool OpenGenericIsAssignableFrom(this Type type, Type fromType)
		{
			Type closedType;
			return OpenGenericIsAssignableFrom(type, fromType, out closedType);
		}
		/// <summary>
		/// 确定当前的开放泛型类型的实例是否可以从指定 <see cref="System.Type"/> 的实例分配，
		/// 并返回相应的封闭泛型类型。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <param name="closedType">如果可以分配到开放泛型类型，则返回相应的封闭泛型类型；
		/// 否则返回 <c>null</c>。</param>
		/// <returns>如果当前的开放泛型类型可以从 <paramref name="fromType"/> 的实例分配，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <example>
		/// 下面是 <see cref="OpenGenericIsAssignableFrom(Type,Type,out Type)"/> 方法的简单示例：
		/// <code>
		/// Type type;
		/// Console.WriteLine(typeof(IEnumerable{}).UniqueOpenGenericIsAssignableFrom(
		///		typeof(List}int}), out type));
		/// Console.WriteLine(type);
		/// // 示例输出：
		/// // True
		/// // System.Collections.Generic.IEnumerable`1[System.Int32]
		/// </code>
		/// </example>
		/// <remarks>
		/// <para>如果一个类将一个泛型接口实现了多次，则只会返回其中的某一个实现。</para>
		/// <para>关于判断是否可以分配到开放泛型类型的算法可以参考我的博客文章 
		/// <see href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</see>。</para></remarks>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		/// <overloads>
		/// <summary>
		/// 确定当前的开放泛型类型的实例是否可以从指定 <see cref="System.Type"/> 的实例分配。
		/// </summary>
		/// </overloads>
		public static bool OpenGenericIsAssignableFrom(this Type type, Type fromType, out Type closedType)
		{
			if (type != null && fromType != null && type.IsGenericTypeDefinition)
			{
				if (type.IsInterface == fromType.IsInterface)
				{
					if (type.InInheritanceChain(fromType, out closedType))
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
						if (type.InInheritanceChain(interfaces[i], out closedType))
						{
							return true;
						}
					}
				}
			}
			closedType = null;
			return false;
		}
		/// <summary>
		/// 确定当前的开放泛型类型的实例是否可以从指定 <see cref="System.Type"/> 的实例唯一分配。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前的开放泛型类型可以从 <paramref name="fromType"/>
		/// 的实例唯一分配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <example>
		/// 下面是 <see cref="UniqueOpenGenericIsAssignableFrom(Type,Type)"/> 方法的简单示例：
		/// <code>
		/// Type type;
		/// Console.WriteLine(typeof(IEnumerable{}).UniqueOpenGenericIsAssignableFrom(
		///		typeof(List}int}))); // True
		/// Console.WriteLine(typeof(IEnumerable{}).UniqueOpenGenericIsAssignableFrom(
		///		typeof(TestClass))); // False
		/// class TestClass : IEnumerable{int}, IEnumerable{long} { }
		/// </code>
		/// </example>
		/// <remarks>
		/// <para>如果一个类将一个泛型接口实现了多次，则不满足唯一实现，会返回 <c>false</c>。
		/// 仅当实现了一次时，才会返回 <c>true</c>，以及相应的封闭泛型类型。</para>
		/// <para>关于判断是否可以分配到开放泛型类型的算法可以参考我的博客文章 
		/// <see href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</see>。</para></remarks>
		public static bool UniqueOpenGenericIsAssignableFrom(this Type type, Type fromType)
		{
			Type closedType;
			return UniqueOpenGenericIsAssignableFrom(type, fromType, out closedType);
		}
		/// <summary>
		/// 确定当前的开放泛型类型的实例是否可以从指定 <see cref="System.Type"/> 的实例唯一分配，
		/// 并返回唯一相应的封闭泛型类型。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <param name="closedType">如果可以唯一分配到开放泛型类型，则返回相应的封闭泛型类型；
		/// 否则返回 <c>null</c>。</param>
		/// <returns>如果当前的开放泛型类型可以从 <paramref name="fromType"/> 的实例唯一分配，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <example>
		/// 下面是 <see cref="UniqueOpenGenericIsAssignableFrom(Type,Type,out Type)"/> 方法的简单示例：
		/// <code>
		/// Type type;
		/// Console.WriteLine(typeof(IEnumerable{}).UniqueOpenGenericIsAssignableFrom(
		///		typeof(List}int}), out type));
		/// Console.WriteLine(type);
		/// Console.WriteLine(typeof(IEnumerable{}).UniqueOpenGenericIsAssignableFrom(
		///		typeof(TestClass)));
		/// class TestClass : IEnumerable{int}, IEnumerable{long} { }
		/// // 示例输出：
		/// // True
		/// // System.Collections.Generic.IEnumerable`1[System.Int32]
		/// // False
		/// </code>
		/// </example>
		/// <remarks>
		/// <para>如果一个类将一个泛型接口实现了多次，则不满足唯一实现，会返回 <c>false</c>。
		/// 仅当实现了一次时，才会返回 <c>true</c>，以及相应的封闭泛型类型。</para>
		/// <para>关于判断是否可以分配到开放泛型类型的算法可以参考我的博客文章 
		/// <see href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</see>。</para></remarks>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		/// <overloads>
		/// <summary>
		/// 确定当前的开放泛型类型的实例是否可以从指定 <see cref="System.Type"/> 的实例唯一分配。
		/// </summary>
		/// </overloads>
		public static bool UniqueOpenGenericIsAssignableFrom(this Type type, Type fromType, out Type closedType)
		{
			if (type != null && fromType != null && type.IsGenericTypeDefinition)
			{
				if (type.IsInterface == fromType.IsInterface)
				{
					if (type.InInheritanceChain(fromType, out closedType))
					{
						return true;
					}
				}
				if (type.IsInterface)
				{
					// 查找唯一实现的接口。
					Type[] interfaces = fromType.GetInterfaces();
					UniqueValue<Type> unique = new UniqueValue<Type>();
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (type.InInheritanceChain(interfaces[i], out closedType))
						{
							unique.Value = closedType;
							if (unique.IsAmbig)
							{
								return false;
							}
						}
					}
					if (unique.IsUnique)
					{
						closedType = unique.Value;
						return true;
					}
				}
			}
			closedType = null;
			return false;
		}
		/// <summary>
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable{} 泛型。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool IsNullableType(this Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}
		/// <summary>
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable{} 泛型，如果是则返回泛型的参数。
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
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable{} 泛型，如果是则返回泛型的参数。
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
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable{} 泛型，如果是则返回泛型的参数。
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
		/// 确定当前的开放泛型类型是否在指定 <see cref="System.Type"/> 类型的继承链中，
		/// 并返回相应的封闭泛型类型。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <param name="closedType">如果在继承链中，则返回相应的封闭泛型类型；否则返回 <c>null</c>。</param>
		/// <returns>如果当前的开放泛型类型在 <paramref name="fromType"/> 的继承链中，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool InInheritanceChain(this Type type, Type fromType, out Type closedType)
		{
			// 沿着 fromType 的继承链向上查找。
			while (fromType != null)
			{
				if (fromType.IsGenericType)
				{
					if (type == fromType.GetGenericTypeDefinition())
					{
						closedType = fromType;
						return true;
					}
				}
				fromType = fromType.BaseType;
			}
			closedType = null;
			return false;
		}

		#endregion // 是否可分配到开放泛型类型

		#region 泛型参数推断

		/// <summary>
		/// 根据给定的类型数组推断泛型参数组的类型参数。
		/// 允许通过为 <paramref name="types"/> 的某一项为 <c>null</c> 
		/// 来指定对应参数是引用类型，但不会进行其它的推断。
		/// </summary>
		/// <param name="genericArgs">泛型类型参数数组。</param>
		/// <param name="paramTypes">形参参数数组。</param>
		/// <param name="paramArrayType">如果形参参数数组的最后一项是 prams 参数，
		/// 则为相应的数组元素类型；否则为 <c>null</c>。</param>
		/// <param name="types">实参参数数组。</param>
		/// <param name="paramOrder">实参参数顺序，这是为了提供对 PowerBinder 的支持。
		/// 它的长度必须大于等于形参个数和实参个数。</param>
		/// <returns>如果成功推断泛型参数组的类型参数，则为类型参数数组；
		/// 如果推断失败，则为 <c>null</c>。</returns>
		internal static Type[] GenericArgumentsInferences(Type[] genericArgs, Type[] paramTypes,
			ref Type paramArrayType, Type[] types, int[] paramOrder)
		{
			Debug.Assert(genericArgs.Length > 0);
			Debug.Assert(paramTypes.Length > 0);
			Debug.Assert(types.Length > 0);
			Debug.Assert(paramOrder.Length >= paramTypes.Length);
			Debug.Assert(paramOrder.Length >= types.Length);
			Dictionary<Type, BoundSet> boundSets = new Dictionary<Type, BoundSet>();
			for (int i = 0; i < genericArgs.Length; i++)
			{
				boundSets.Add(genericArgs[i], new BoundSet());
			}
			int len = paramTypes.Length;
			// params T 数组参数。
			Type paramElementType = null;
			if (paramArrayType != null)
			{
				// 最后一个参数需要对 params 参数进行特殊处理。
				len--;
				if (paramTypes[len].ContainsGenericParameters && paramOrder[len] < types.Length)
				{
					if (len + 1 == types.Length)
					{
						// 如果实参形参数量相等，需要对 params T 数组进行特殊判断。
						paramArrayType.GetArrayDepth(out paramElementType);
						if (!paramElementType.IsGenericParameter)
						{
							paramElementType = null;
						}
					}
					if (paramElementType == null)
					{
						// params 参数需要从数组元素类型到多个实参做类型推断。
						for (int i = len; i < types.Length; i++)
						{
							if (!TypeInferences(paramArrayType, types[paramOrder[len]], boundSets))
							{
								// 类型推断失败。
								return null;
							}
						}
					}
				}
			}
			for (len--; len >= 0; len--)
			{
				if (paramTypes[len].ContainsGenericParameters && paramOrder[len] < types.Length)
				{
					if (!TypeInferences(paramTypes[len], types[paramOrder[len]], boundSets))
					{
						// 类型推断失败。
						return null;
					}
				}
			}
			Type[] args = null;
			if (paramElementType == null)
			{
				// 没有 params T 数组参数，直接进行固定。
				args = FixTypeArguments(genericArgs, boundSets);
			}
			else
			{
				// 需要复制界限集。
				Dictionary<Type, BoundSet> boundSetsClone = new Dictionary<Type, BoundSet>();
				foreach (KeyValuePair<Type, BoundSet> pair in boundSets)
				{
					boundSetsClone.Add(pair.Key, pair.Value.Clone());
				}
				len = paramTypes.Length - 1;
				// 首先尝试对 paramArrayType[] 进行推断。
				if (TypeInferences(paramTypes[len], types[paramOrder[len]], boundSets))
				{
					args = FixTypeArguments(genericArgs, boundSets);
				}
				if (args != null)
				{
					// 推断成功的话，则需要将 paramArrayType 置为 null 以表示无需展开 params 参数。
					paramArrayType = null;
				}
				else
				{
					// 失败的话则尝试对 paramArrayType 进行推断。
					if (TypeInferences(paramArrayType, types[paramOrder[len]], boundSetsClone))
					{
						args = FixTypeArguments(genericArgs, boundSetsClone);
					}
				}
			}
			return args;
		}
		/// <summary>
		/// 根据给定的界限集固定类型参数。
		/// </summary>
		/// <param name="genericArgs">泛型类型参数数组。</param>
		/// <param name="boundSets">界限集。</param>
		/// <returns>如果成功推断泛型参数组的类型参数，则为类型参数数组；
		/// 如果推断失败，则为 <c>null</c>。</returns>
		private static Type[] FixTypeArguments(Type[] genericArgs, IDictionary<Type, BoundSet> boundSets)
		{
			Type[] result = new Type[genericArgs.Length];
			for (int i = 0; i < genericArgs.Length; i++)
			{
				result[i] = boundSets[genericArgs[i]].FixTypeArg();
				if (result[i] == null)
				{
					return null;
				}
			}
			return result;
		}
		/// <summary>
		/// 对类型进行推断。
		/// </summary>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <param name="boundSets">泛型形参的界限集。</param>
		/// <returns>如果类型推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool TypeInferences(Type paramType, Type type, Dictionary<Type, BoundSet> boundSets)
		{
			if (type == null)
			{
				if (paramType.IsByRef)
				{
					paramType = paramType.GetElementType();
				}
				if (paramType.IsGenericParameter)
				{
					boundSets[paramType].ReferenceType = true;
				}
				return true;
			}
			else if (paramType.IsByRef)
			{
				return ExactInferences(paramType.GetElementType(), type, boundSets);
			}
			else
			{
				return LowerBoundInferences(paramType, type, boundSets);
			}
		}
		/// <summary>
		/// 对类型进行精确推断。
		/// </summary>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <param name="boundSets">泛型形参的界限集。</param>
		/// <returns>如果精确推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool ExactInferences(Type paramType, Type type, Dictionary<Type, BoundSet> boundSets)
		{
			Type tempParamType;
			if (paramType.IsGenericParameter)
			{
				return boundSets[paramType].AddExactBound(type);
			}
			else if (paramType.IsNullableType(out tempParamType))
			{
				Type tempType;
				if (type.IsNullableType(out tempType))
				{
					return ExactInferences(tempParamType, tempType, boundSets);
				}
			}
			else if (paramType.IsArray)
			{
				if (type.IsArray && paramType.GetArrayRank() == type.GetArrayRank())
				{
					return LowerBoundInferences(paramType.GetElementType(), type.GetElementType(), boundSets);
				}
			}
			else if (paramType.GetGenericTypeDefinition() == type.GetGenericTypeDefinition())
			{
				Type[] paramTypeArgs = paramType.GetGenericArguments();
				Type[] typeArgs = type.GetGenericArguments();
				for (int i = 0; i < paramTypeArgs.Length; i++)
				{
					if (!ExactInferences(paramTypeArgs[i], typeArgs[i], boundSets))
					{
						return false;
					}
				}
			}
			return true;
		}
		/// <summary>
		/// 对类型进行下限推断。
		/// </summary>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <param name="boundSets">泛型形参的界限集。</param>
		/// <returns>如果下限推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool LowerBoundInferences(Type paramType, Type type, Dictionary<Type, BoundSet> boundSets)
		{
			Type tempParamType = null, tempType = null;
			if (paramType.IsGenericParameter)
			{
				return boundSets[paramType].AddLowerBound(type);
			}
			else if (paramType.IsNullableType(out tempParamType))
			{
				if (type.IsNullableType(out tempType))
				{
					return LowerBoundInferences(tempParamType, tempType, boundSets);
				}
			}
			else if (type.IsArray)
			{
				tempType = type.GetElementType();
				if (paramType.IsArray)
				{
					if (paramType.GetArrayRank() == type.GetArrayRank())
					{
						return LowerBoundInferences(paramType.GetElementType(), tempType, boundSets);
					}
				}
				else if (paramType.IsGenericType)
				{
					tempParamType = paramType.GetGenericTypeDefinition();
					if (tempParamType == typeof(IList<>) ||
						tempParamType == typeof(ICollection<>) ||
						tempParamType == typeof(IEnumerable<>))
					{
						return LowerBoundInferences(paramType.GetGenericArguments()[0], tempType, boundSets);
					}
				}
			}
			else if (paramType.IsGenericType)
			{
				tempParamType = paramType.GetGenericTypeDefinition();
				if (tempParamType.UniqueOpenGenericIsAssignableFrom(type, out tempType))
				{
					Type[] originArgs = tempParamType.GetGenericArguments();
					Type[] paramTypeArgs = paramType.GetGenericArguments();
					Type[] typeArgs = tempType.GetGenericArguments();
					for (int i = 0; i < originArgs.Length; i++)
					{
						bool result = true;
						switch (originArgs[i].GenericParameterAttributes & GenericParameterAttributes.VarianceMask)
						{
							case GenericParameterAttributes.None:
								result = ExactInferences(paramTypeArgs[i], typeArgs[i], boundSets);
								break;
							case GenericParameterAttributes.Covariant:
								result = LowerBoundInferences(paramTypeArgs[i], typeArgs[i], boundSets);
								break;
							case GenericParameterAttributes.Contravariant:
								result = UpperBoundInferences(paramTypeArgs[i], typeArgs[i], boundSets);
								break;
						}
						if (!result)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		/// <summary>
		/// 对类型进行上限推断。
		/// </summary>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <param name="boundSets">泛型形参的界限集。</param>
		/// <returns>如果上限推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool UpperBoundInferences(Type paramType, Type type, Dictionary<Type, BoundSet> boundSets)
		{
			Type tempParamType = null, tempType = null;
			if (paramType.IsGenericParameter)
			{
				return boundSets[paramType].AddUpperBound(type);
			}
			else if (paramType.IsNullableType(out tempParamType))
			{
				if (type.IsNullableType(out tempType))
				{
					return UpperBoundInferences(tempParamType, tempType, boundSets);
				}
			}
			else if (paramType.IsArray)
			{
				tempParamType = paramType.GetElementType();
				if (type.IsArray)
				{
					if (paramType.GetArrayRank() == type.GetArrayRank())
					{
						return UpperBoundInferences(tempParamType, type.GetElementType(), boundSets);
					}
				}
				else if (type.IsGenericType)
				{
					tempType = type.GetGenericTypeDefinition();
					if (tempType == typeof(IList<>) ||
						tempType == typeof(ICollection<>) ||
						tempType == typeof(IEnumerable<>))
					{
						return UpperBoundInferences(tempParamType, tempType.GetGenericArguments()[0], boundSets);
					}
				}
			}
			else if (type.IsGenericType)
			{
				tempType = type.GetGenericTypeDefinition();
				if (tempType.UniqueOpenGenericIsAssignableFrom(paramType, out tempParamType))
				{
					Type[] originArgs = tempType.GetGenericArguments();
					Type[] paramTypeArgs = tempParamType.GetGenericArguments();
					Type[] typeArgs = type.GetGenericArguments();
					for (int i = 0; i < originArgs.Length; i++)
					{
						bool result = true;
						switch (originArgs[i].GenericParameterAttributes & GenericParameterAttributes.VarianceMask)
						{
							case GenericParameterAttributes.None:
								result = ExactInferences(paramTypeArgs[i], typeArgs[i], boundSets);
								break;
							case GenericParameterAttributes.Covariant:
								result = UpperBoundInferences(paramTypeArgs[i], typeArgs[i], boundSets);
								break;
							case GenericParameterAttributes.Contravariant:
								result = LowerBoundInferences(paramTypeArgs[i], typeArgs[i], boundSets);
								break;
						}
						if (!result)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		/// <summary>
		/// 泛型形参的界限集。
		/// </summary>
		private class BoundSet
		{
			/// <summary>
			/// 类型推断的下限界限集。
			/// </summary>
			private HashSet<Type> lowerBounds = new HashSet<Type>();
			/// <summary>
			/// 类型推断的上限界限集。
			/// </summary>
			private HashSet<Type> upperBounds = new HashSet<Type>();
			/// <summary>
			/// 类型推断的精确界限（这个需要是唯一的）。
			/// </summary>
			private Type exactBound = null;
			/// <summary>
			/// 是否要求类型形参必须是泛型类型。
			/// </summary>
			public bool ReferenceType = false;
			/// <summary>
			/// 向类型推断的精确界限集中添加指定的类型。
			/// </summary>
			/// <param name="type">要添加的类型。</param>
			/// <returns>如果添加成功，则为 <c>true</c>；如果产生了冲突，则为 <c>false</c>。</returns>
			public bool AddExactBound(Type type)
			{
				if (exactBound == null)
				{
					if (!CanFixed(type))
					{
						// 与现有的界限冲突。
						return false;
					}
					lowerBounds.Clear();
					upperBounds.Clear();
					exactBound = type;
				}
				else if (exactBound != type)
				{
					return false;
				}
				return true;
			}
			/// <summary>
			/// 向类型推断的下限界限集中添加指定的类型。
			/// </summary>
			/// <param name="type">要添加的类型。</param>
			/// <returns>如果添加成功，则为 <c>true</c>；如果产生了冲突，则为 <c>false</c>。</returns>
			public bool AddLowerBound(Type type)
			{
				if (exactBound == null)
				{
					lowerBounds.Add(type);
				}
				else if (!exactBound.IsImplicitFrom(type))
				{
					// 与精确界限冲突。
					return false;
				}
				return true;
			}
			/// <summary>
			/// 向类型推断的上限界限集中添加指定的类型。
			/// </summary>
			/// <param name="type">要添加的类型。</param>
			/// <returns>如果添加成功，则为 <c>true</c>；如果产生了冲突，则为 <c>false</c>。</returns>
			public bool AddUpperBound(Type type)
			{
				if (exactBound == null)
				{
					upperBounds.Add(type);
				}
				else if (!type.IsImplicitFrom(exactBound))
				{
					// 与精确界限冲突。
					return false;
				}
				return true;
			}
			/// <summary>
			/// 返回当前类型的复制。
			/// </summary>
			/// <returns>当前类型的复制。</returns>
			public BoundSet Clone()
			{
				BoundSet newSet = new BoundSet();
				if (this.exactBound == null)
				{
					newSet.lowerBounds.UnionWith(this.lowerBounds);
					newSet.upperBounds.UnionWith(this.upperBounds);
				}
				else
				{
					newSet.exactBound = this.exactBound;
				}
				newSet.ReferenceType = this.ReferenceType;
				return newSet;
			}
			/// <summary>
			/// 固定当前界限集所限定的类型参数。
			/// </summary>
			/// <returns>如果成功固定当前界限集的的类型参数，则为类型参数；
			/// 如果固定失败，则为 <c>null</c>。</returns>
			public Type FixTypeArg()
			{
				Type result = null;
				if (exactBound == null)
				{
					List<Type> list = new List<Type>(new HashSet<Type>(lowerBounds.Concat(upperBounds))
						.Where(type => this.CanFixed(type)));
					if (list.Count == 0)
					{
						// 没有找到合适的推断结果。
						return null;
					}
					else if (list.Count == 1)
					{
						// 找到唯一的推断结果。
						result = list[0];
					}
					else
					{
						// 进一步进行推断。
						UniqueValue<Type> uType = new UniqueValue<Type>();
						int cnt = list.Count;
						for (int j = 0; j < cnt; j++)
						{
							int k = 0;
							for (; k < cnt; k++)
							{
								if (k == j) { continue; }
								if (!list[j].IsImplicitFrom(list[k]))
								{
									break;
								}
							}
							if (k == cnt) { uType.Value = list[j]; }
						}
						if (uType.IsUnique)
						{
							result = uType.Value;
						}
						else
						{
							// 推断失败。
							return null;
						}
					}
				}
				else
				{
					result = exactBound;
				}
				// 判断引用类型约束。
				if (ReferenceType && result.IsValueType)
				{
					return null;
				}
				return result;
			}
			/// <summary>
			/// 判断指定的类型能否根据给定的上限集和下限集中被固定。
			/// </summary>
			/// <param name="testType">要判断能否固定的类型。</param>
			/// <returns>如果给定的类型可以固定，则为 <c>true</c>；否则为 <c>false</c>。</returns>
			private bool CanFixed(Type testType)
			{
				foreach (Type type in lowerBounds)
				{
					if (!testType.IsImplicitFrom(type)) { return false; }
				}
				foreach (Type type in upperBounds)
				{
					if (!type.IsImplicitFrom(testType)) { return false; }
				}
				return true;
			}
		}

		#endregion // 泛型参数推断

		/// <summary>
		/// 返回获取类型的数组深度。
		/// </summary>
		/// <param name="type">要获取数组深度的类型。</param>
		/// <returns>如果给定类型是数组，则为数组的深度；否则为 <c>0</c>。</returns>
		internal static int GetArrayDepth(this Type type)
		{
			int depth = 0;
			while (type.IsArray)
			{
				depth++;
				type = type.GetElementType();
			}
			return depth;
		}
		/// <summary>
		/// 返回获取类型的数组深度和数组元素类型。
		/// </summary>
		/// <param name="type">要获取数组深度的类型。</param>
		/// <param name="elementType">数组元素的类型。</param>
		/// <returns>如果给定类型是数组，则为数组的深度；否则为 <c>0</c>。</returns>
		internal static int GetArrayDepth(this Type type, out Type elementType)
		{
			int depth = 0;
			while (type.IsArray)
			{
				depth++;
				type = type.GetElementType();
			}
			elementType = type;
			return depth;
		}
		/// <summary>
		/// 返回类型的定义层级深度。
		/// </summary>
		/// <param name="type">要获取定义层级深度的类型。</param>
		/// <returns>指定类型的定义层级深度。</returns>
		internal static int GetHierarchyDepth(this Type type)
		{
			int depth = -1;
			while (type != null)
			{
				type = type.BaseType;
				depth++;
			}
			return depth;
		}
	}
}
