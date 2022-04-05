using System.Reflection;
using System.Text;
using Cyjb.Conversions;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="Type"/> 类的扩展方法。
	/// </summary>
	public static partial class TypeUtil
	{
		/// <summary>
		/// 类型的构造函数方法名称。
		/// </summary>
		public const string ConstructorName = ".ctor";

		#region 类型判断

		/// <summary>
		/// 返回当前 <see cref="Type"/> 是否表示数字类型。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 表示数字类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>数字类型为 <see cref="char"/>、<see cref="sbyte"/>、<see cref="byte"/>、 
		/// <see cref="short"/>、<see cref="ushort"/>、<see cref="int"/>、<see cref="uint"/>、 
		/// <see cref="long"/>、<see cref="ulong"/>、<see cref="float"/>、<see cref="double"/> 
		/// 和 <see cref="decimal"/>，或者基础类型为其中之一的枚举类型。</remarks>
		public static bool IsNumeric(this Type? type)
		{
			return Type.GetTypeCode(type).IsNumeric();
		}

		/// <summary>
		/// 返回当前 <see cref="Type"/> 是否表示无符号整数类型。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 表示无符号整数类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>无符号整数类型为 <see cref="char"/>、<see cref="byte"/>、<see cref="ushort"/>、<see cref="uint"/> 和 
		/// <see cref="ulong"/>，或者基础类型为其中之一的枚举类型。</remarks>
		public static bool IsUnsigned(this Type? type)
		{
			return Type.GetTypeCode(type).IsUnsigned();
		}

		/// <summary>
		/// 返回当前 <see cref="Type"/> 是否表示有符号整数类型。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 表示有符号整数类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>有符号整数类型为 <see cref="sbyte"/>、<see cref="short"/>、<see cref="int"/> 和 
		/// <see cref="long"/>，或者基础类型为其中之一的枚举类型。</remarks>
		public static bool IsSigned(this Type? type)
		{
			return Type.GetTypeCode(type).IsSigned();
		}

		/// <summary>
		/// 确定当前 <see cref="Type"/> 是否是可空类型。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsNullable(this Type? type)
		{
			return type != null && Nullable.GetUnderlyingType(type) != null;
		}

		/// <summary>
		/// 返回与当前 <see cref="Type"/> 对应的非可空类型。
		/// </summary>
		/// <param name="type">要获取非可空类型的类型。</param>
		/// <returns>如果当前类型是可空类型，则为相应的非可空类型；否则为当前类型。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		public static Type GetNonNullableType(this Type type)
		{
			CommonExceptions.CheckArgumentNull(type);
			return Nullable.GetUnderlyingType(type) ?? type;
		}

		/// <summary>
		/// 检查当前 <see cref="Type"/> 是否表示委托。
		/// </summary>
		/// <param name="type">要检查的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 表示委托，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsDelegate(this Type? type)
		{
			return type != null && type.IsSubclassOf(typeof(Delegate)) &&
				type != typeof(Delegate) && type != typeof(MulticastDelegate);
		}

		/// <summary>
		/// 返回指定类型是否是 <see cref="IList{T}"/> 或其类型的泛型方法定义。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果是 <see cref="IList{T}"/> 或其类型的泛型方法定义，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		internal static bool IsIListOrBase(this Type type)
		{
			return type == typeof(IList<>) || type == typeof(ICollection<>) || type == typeof(IEnumerable<>);
		}

		#endregion // 类型判断

		#region 类型转换

		/// <summary>
		/// 确定当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 的实例隐式类型转换得到。
		/// </summary>
		/// <param name="type">当前类型。</param>
		/// <param name="fromType">要判断能否隐式类型转换得到的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 
		/// 的实例隐式类型转换得到，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="fromType"/> 为 <c>null</c>。</exception>
		/// <remarks>
		/// 判断类型间能否进行隐式类型转换的算法来自于 《CSharp Language Specification》v5.0 
		/// 的第 6.1 节。</remarks>
		/// <example>
		/// 下面是 <see cref="IsImplicitFrom"/> 方法与 <see cref="Type.IsAssignableFrom"/> 方法的一些对比。
		/// <code>
		/// Console.WriteLine(typeof(object).IsAssignableFrom(typeof(uint))); // True 
		/// Console.WriteLine(typeof(object).IsImplicitFrom(typeof(uint))); // True
		/// Console.WriteLine(typeof(int).IsAssignableFrom(typeof(short))); // False
		/// Console.WriteLine(typeof(int).IsImplicitFrom(typeof(short))); // True
		/// Console.WriteLine(typeof(long?).IsAssignableFrom(typeof(int?))); // False
		/// Console.WriteLine(typeof(long?).IsImplicitFrom(typeof(int?))); // True
		/// </code>
		/// </example>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		public static bool IsImplicitFrom(this Type type, Type fromType)
		{
			CommonExceptions.CheckArgumentNull(type);
			CommonExceptions.CheckArgumentNull(fromType);
			Conversion? conversion = ConversionFactory.GetConversion(fromType, type);
			return conversion != null && conversion.ConversionType.IsImplicit();
		}

		/// <summary>
		/// 确定当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 的实例显式类型转换得到。
		/// </summary>
		/// <param name="type">当前类型。</param>
		/// <param name="fromType">要判断能否显式类型转换得到的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 
		/// 的实例显式类型转换得到，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="fromType"/> 为 <c>null</c>。</exception>
		/// <remarks>
		/// 判断类型间能否进行显式类型转换的算法来自于 《CSharp Language Specification》v5.0 
		/// 的第 6.1 节。</remarks>
		/// <example>
		/// 下面是 <see cref="IsExplicitFrom"/> 方法的一些实例。
		/// <code>
		/// Console.WriteLine(typeof(uint).IsExplicitFrom(typeof(object))); // True
		/// Console.WriteLine(typeof(short).IsExplicitFrom(typeof(int))); // True
		/// Console.WriteLine(typeof(int).IsExplicitFrom(typeof(long?))); // True
		/// </code>
		/// </example>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		public static bool IsExplicitFrom(this Type type, Type fromType)
		{
			CommonExceptions.CheckArgumentNull(type);
			CommonExceptions.CheckArgumentNull(fromType);
			return ConversionFactory.GetConversion(fromType, type) != null;
		}

		/// <summary>
		/// 确定当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 的实例类型转换得到。
		/// </summary>
		/// <param name="type">当前类型。</param>
		/// <param name="fromType">要判断能否类型转换得到的类型。</param>
		/// <param name="isExplicit">如果考虑显式类型转换，则为 <c>true</c>；否则只考虑隐式类型转换。</param>
		/// <returns>如果当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 
		/// 的实例类型转换得到，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="fromType"/> 为 <c>null</c>。</exception>
		/// <remarks>
		/// 判断类型间能否进行类型转换的算法来自于 《CSharp Language Specification》v5.0 
		/// 的第 6.1 节。</remarks>
		/// <example>
		/// 下面是 <see cref="IsConvertFrom"/> 方法的一些实例。
		/// <code>
		/// Console.WriteLine(typeof(object).IsExplicitFrom(typeof(uint), false)); // True
		/// Console.WriteLine(typeof(int).IsExplicitFrom(typeof(short), false)); // True
		/// Console.WriteLine(typeof(long?).IsExplicitFrom(typeof(int?), false)); // True
		/// Console.WriteLine(typeof(uint).IsConvertFrom(typeof(object), true)); // True
		/// Console.WriteLine(typeof(short).IsConvertFrom(typeof(int), true)); // True
		/// Console.WriteLine(typeof(int).IsConvertFrom(typeof(long?), true)); // True
		/// </code>
		/// </example>
		/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/TypeAssignableFrom.html">
		/// 《C# 判断类型间能否隐式或强制类型转换，以及开放泛型类型转换》</seealso>
		public static bool IsConvertFrom(this Type type, Type fromType, bool isExplicit = false)
		{
			CommonExceptions.CheckArgumentNull(type);
			CommonExceptions.CheckArgumentNull(fromType);
			Conversion? conversion = ConversionFactory.GetConversion(fromType, type);
			return conversion != null && (isExplicit || conversion.ConversionType.IsImplicit());
		}

		/// <summary>
		/// 返回 <paramref name="types"/> 中能够包含其它所有类型的类型（可以从其它所有类型隐式转换而来）。
		/// </summary>
		/// <param name="types">类型集合。</param>
		/// <returns><paramref name="types"/> 中能够包含其它所有类型的类型，如果不存在则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <remarks>若 A 类型可以隐式类型转换（指预定义的类型转换）为 B 类型，那么就称 A 被 B 包含，而 B 包含 A。</remarks>
		public static Type? GetEncompassingType(IEnumerable<Type> types)
		{
			CommonExceptions.CheckArgumentNull(types);
			List<Type> candidates = new();
			foreach (Type? type in types)
			{
				if (type == null)
				{
					// 忽略 null 类型。
					continue;
				}
				bool canConvert = false;
				for (int i = 0; i < candidates.Count; i++)
				{
					if (candidates[i] == type)
					{
						// 相同类型，不需要继续处理。
						canConvert = true;
						break;
					}
					ConversionType convType = ConversionFactory.GetStandardConversion(candidates[i], type);
					if (convType != ConversionType.None)
					{
						if (convType.IsExplicit())
						{
							// type 可转换到 candidates[i]，不需要继续处理。
							canConvert = true;
							break;
						}
						else if (canConvert)
						{
							candidates.RemoveAt(i);
							i--;
						}
						else
						{
							canConvert = true;
							candidates[i] = type;
						}
					}
				}
				if (!canConvert)
				{
					candidates.Add(type);
				}
			}
			if (candidates.Count == 1)
			{
				return candidates[0];
			}
			// 未找到候选类型，或者找到了多个无法互相覆盖的候选类型
			return null;
		}

		/// <summary>
		/// 返回 <paramref name="types"/> 中能够被其它所有类型包含的类型（可以隐式转换为其它所有类型）。
		/// </summary>
		/// <param name="types">类型集合。</param>
		/// <returns><paramref name="types"/> 中能够被其它所有类型包含的类型，如果不存在则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <remarks>若 A 类型可以隐式类型转换（指预定义的类型转换）为 B 类型，那么就称 A 被 B 包含，而 B 包含 A。</remarks>
		public static Type? GetEncompassedType(IEnumerable<Type> types)
		{
			CommonExceptions.CheckArgumentNull(types);
			List<Type> candidates = new();
			foreach (Type? type in types)
			{
				if (type == null)
				{
					// 忽略 null 类型。
					continue;
				}
				bool canConvert = false;
				for (int i = 0; i < candidates.Count; i++)
				{
					if (candidates[i] == type)
					{
						// 相同类型，不需要继续处理。
						canConvert = true;
						break;
					}
					ConversionType convType = ConversionFactory.GetStandardConversion(type, candidates[i]);
					if (convType != ConversionType.None)
					{
						if (convType.IsExplicit())
						{
							// candidates[i] 可转换到 type，不需要继续处理。
							canConvert = true;
							break;
						}
						else if (canConvert)
						{
							candidates.RemoveAt(i);
							i--;
						}
						else
						{
							canConvert = true;
							candidates[i] = type;
						}
					}
				}
				if (!canConvert)
				{
					candidates.Add(type);
				}
			}
			if (candidates.Count == 1)
			{
				return candidates[0];
			}
			// 未找到候选类型，或者找到了多个无法互相覆盖的候选类型
			return null;
		}

		#endregion // 类型转换

		#region 返回泛型类型定义的封闭构造类型

		/// <summary>
		/// 沿着指定 <see cref="Type"/> 的继承链向上查找，直到找到当前泛型类型定义的封闭构造类型。
		/// </summary>
		/// <param name="definition">要获取封闭构造类型的泛型类型定义。</param>
		/// <param name="type">要查找继承链的类型。</param>
		/// <returns>如果当前的泛型类型定义是 <paramref name="type"/> 继承链中类型的泛型定义，
		/// 或者是 <paramref name="type"/> 实现的接口的泛型定义，
		/// 或者 <paramref name="type"/> 是泛型类型参数且当前泛型类型定义是 <paramref name="type"/> 的约束之一，
		/// 则为相应的封闭构造类型。否则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="definition"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <remarks>如果 <paramref name="definition"/> 可以找到多个封闭构造类型，则只会返回其中的任意一个。</remarks>
		/// <example>
		/// 下面是 <see cref="CloseDefinitionFrom"/> 方法的简单示例：
		/// <code>
		/// Console.WriteLine(typeof(IEnumerable&lt;&gt;).CloseDefinitionFrom(typeof(List&lt;int&gt;)));
		/// // 输出：System.Collections.Generi.IEnumerable`1[System.Int32]
		/// </code>
		/// </example>
		public static Type? CloseDefinitionFrom(this Type definition, Type type)
		{
			CommonExceptions.CheckArgumentNull(definition);
			CommonExceptions.CheckArgumentNull(type);
			if (!definition.IsGenericTypeDefinition)
			{
				return null;
			}
			if (!definition.IsInterface && !type.IsInterface)
			{
				// 沿继承链向上查找。
				while (true)
				{
					if (type.IsGenericType && definition == type.GetGenericTypeDefinition())
					{
						return type;
					}
					else if (type.BaseType == null)
					{
						return null;
					}
					else
					{
						type = type.BaseType;
					}
				}
			}
			if (!definition.IsInterface)
			{
				return null;
			}
			if (type.IsGenericType && definition == type.GetGenericTypeDefinition())
			{
				return type;
			}
			// 查找实现的接口。
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (interfaces[i].IsGenericType && definition == interfaces[i].GetGenericTypeDefinition())
				{
					return interfaces[i];
				}
			}
			return null;
		}

		/// <summary>
		/// 沿着指定 <see cref="Type"/> 的继承链向上查找，直到找到当前泛型类型定义的封闭构造类型。
		/// </summary>
		/// <param name="definition">要获取封闭构造类型的泛型类型定义。</param>
		/// <param name="type">要查找继承链的类型。</param>
		/// <returns>如果当前的泛型类型定义是 <paramref name="type"/> 继承链中类型的泛型定义，
		/// 或者是 <paramref name="type"/> 实现的接口的泛型定义，
		/// 或者 <paramref name="type"/> 是泛型类型参数且当前泛型类型定义是 <paramref name="type"/> 的约束之一，
		/// 则为相应的封闭构造类型。否则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="definition"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <remarks>如果一个类将同一个接口实现了多次，那么该接口对应的封闭构造类型就不是唯一的。</remarks>
		/// <example>
		/// 下面是 <see cref="UniqueCloseDefinitionFrom"/> 方法的简单示例：
		/// <code>
		/// Console.WriteLine(typeof(IEnumerable&lt;&gt;).UniqueCloseDefinitionFrom(typeof(List&lt;int&gt;)));
		/// // 输出：System.Collections.Generic.IEnumerable`1[System.Int32]
		/// </code>
		/// </example>
		public static Type? UniqueCloseDefinitionFrom(this Type definition, Type type)
		{
			CommonExceptions.CheckArgumentNull(definition);
			CommonExceptions.CheckArgumentNull(type);
			if (!definition.IsGenericTypeDefinition)
			{
				return null;
			}
			if (!definition.IsInterface && !type.IsInterface)
			{
				// 沿继承链向上查找。
				while (true)
				{
					if (type.IsGenericType && definition == type.GetGenericTypeDefinition())
					{
						return type;
					}
					else if (type.BaseType == null)
					{
						return null;
					}
					else
					{
						type = type.BaseType;
					}
				}
			}
			if (!definition.IsInterface)
			{
				return null;
			}
			if (type.IsGenericType && definition == type.GetGenericTypeDefinition())
			{
				return type;
			}
			// 查找实现的接口。
			Type[] interfaces = type.GetInterfaces();
			UniqueValue<Type> unique = new();
			for (int i = 0; i < interfaces.Length && !unique.IsAmbig; i++)
			{
				if (interfaces[i].IsGenericType && definition == interfaces[i].GetGenericTypeDefinition())
				{
					unique.Value = interfaces[i];
				}
			}
			return unique.Value;
		}

		#endregion // 返回泛型类型定义的封闭构造类型

		/// <summary>
		/// 获取委托类型的 Invoke 方法。
		/// </summary>
		/// <param name="type">委托类型。</param>
		/// <returns>委托类型的 Invoke 方法。</returns>
		internal static MethodInfo GetDelegateInvoke(this Type type)
		{
			return type.GetMethod("Invoke")!;
		}

		/// <summary>
		/// 返回类型的定义层级深度。
		/// </summary>
		/// <param name="type">要获取定义层级深度的类型。</param>
		/// <returns>指定类型的定义层级深度。</returns>
		internal static int GetHierarchyDepth(this Type type)
		{
			int depth = -1;
			while (true)
			{
				depth++;
				if (type.BaseType == null)
				{
					return depth;
				}
				else
				{
					type = type.BaseType;
				}
			}
		}

		/// <summary>
		/// 获取 <see cref="Type"/> 的完全限定名，包括 <see cref="Type"/> 的命名空间，但不包括程序集。
		/// </summary>
		/// <param name="type">要获取完全限定名的类型。</param>
		/// <returns>获取 <see cref="Type"/> 的完全限定名，包括 <see cref="Type"/> 的命名空间，但不包括程序集；
		/// 如果当前实例表示泛型类型参数、数组类型、指针类型或基于类型参数的 byref 类型，
		/// 或表示不属于泛型类型定义但包含无法解析的类型参数的泛型类型，则返回 <c>null</c>。</returns>
		/// <remarks>与 <see cref="Type.FullName"/> 不同，即使如果当前 <see cref="Type"/> 表示泛型类型，
		/// <see cref="FullName"/> 返回的字符串中的类型实参也不会包含程序集、版本等信息。</remarks>
		public static string FullName(this Type type)
		{
			if (!type.IsGenericType || type.ContainsGenericParameters)
			{
				return type.FullName!;
			}
			StringBuilder text = new(type.GetGenericTypeDefinition().FullName);
			text.Append('[');
			Type[] args = type.GetGenericArguments();
			for (int i = 0; i < args.Length; i++)
			{
				if (i > 0)
				{
					text.Append(',');
				}
				text.Append(args[i].FullName());
			}
			text.Append(']');
			return text.ToString();
		}
	}
}
