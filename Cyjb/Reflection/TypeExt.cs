using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text;
using Cyjb.Conversions;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="Type"/> 类的扩展方法。
	/// </summary>
	public static class TypeExt
	{
		/// <summary>
		/// 搜索公共静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags PublicStaticFlag = BindingFlags.Public | BindingFlags.Static;
		/// <summary>
		/// 搜索公共或私有静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags StaticFlag = BindingFlags.NonPublic | PublicStaticFlag;
		/// <summary>
		/// 搜索公共实例成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags PublicInstanceFlag = BindingFlags.Public | BindingFlags.Instance;
		/// <summary>
		/// 搜索公共或私有实例成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags InstanceFlag = BindingFlags.NonPublic | PublicInstanceFlag;
		/// <summary>
		/// 搜索公共实例或静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags PublicFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		/// <summary>
		/// 搜索公共或私有实例或静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags AllMemberFlag = BindingFlags.NonPublic | PublicFlag;
		/// <summary>
		/// 是否运行在 Mono 运行时中。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static readonly bool IsMonoRuntime = Type.GetType("Mono.Runtime") != null;

		#region 类型判断

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
		[Pure]
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
		[Pure]
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
		[Pure]
		public static bool IsSigned(this TypeCode typeCode)
		{
			return typeCode == TypeCode.SByte || typeCode == TypeCode.Int16 ||
				typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64;
		}
		/// <summary>
		/// 返回当前 <see cref="Type"/> 是否表示数字类型。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 表示数字类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>数字类型为 <see cref="char"/>、<see cref="sbyte"/>、<see cref="byte"/>、 
		/// <see cref="short"/>、<see cref="ushort"/>、<see cref="int"/>、<see cref="uint"/>、 
		/// <see cref="long"/>、<see cref="ulong"/>、<see cref="float"/>、<see cref="double"/> 
		/// 和 <see cref="decimal"/>，或者基础类型为其中之一的枚举类型。</remarks>
		[Pure]
		public static bool IsNumeric(this Type type)
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
		[Pure]
		public static bool IsUnsigned(this Type type)
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
		[Pure]
		public static bool IsSigned(this Type type)
		{
			return Type.GetTypeCode(type).IsSigned();
		}
		/// <summary>
		/// 确定当前 <see cref="Type"/> 是否是可空类型。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		[Pure]
		public static bool IsNullable(this Type type)
		{
			return type != null && Nullable.GetUnderlyingType(type) != null;
		}
		/// <summary>
		/// 返回与当前 <see cref="Type"/> 对应的非可空类型。
		/// </summary>
		/// <param name="type">要获取非可空类型的类型。</param>
		/// <returns>如果当前类型是可空类型，则为相应的非可空类型；否则为当前类型。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		[Pure]
		public static Type GetNonNullableType(this Type type)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.Ensures(Contract.Result<Type>() != null);
			return Nullable.GetUnderlyingType(type) ?? type;
		}

		#endregion // 类型判断

		#region 类型转换

		/// <summary>
		/// 确定当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 的实例隐式类型转换得到。
		/// 这里仅考虑预定义的隐式类型转换，不考虑由 <c>implicit</c> 运算符指定的转换。
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
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckArgumentNull(fromType, "fromType");
			Contract.EndContractBlock();
			Conversion conversion = ConversionFactory.GetPreDefinedConversion(fromType, type);
			return conversion != null && conversion.ConversionType.IsImplicit();
		}
		/// <summary>
		/// 确定当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 的实例显式类型转换得到。
		/// 这里仅考虑预定义的显式类型转换，不考虑由 <c>implicit</c> 运算符和 <c>explicit</c> 运算符指定的转换。
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
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckArgumentNull(fromType, "fromType");
			Contract.EndContractBlock();
			Conversion conversion = ConversionFactory.GetPreDefinedConversion(fromType, type);
			return conversion != null && conversion.ConversionType != ConversionType.None;
		}
		/// <summary>
		/// 确定当前 <see cref="Type"/> 的实例是否可以从 <paramref name="fromType"/> 的实例类型转换得到。
		/// 这里仅考虑预定义的类型转换，不考虑由 <c>implicit</c> 运算符和 <c>explicit</c> 运算符指定的转换。
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
		public static bool IsConvertFrom(this Type type, Type fromType, bool isExplicit)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckArgumentNull(fromType, "fromType");
			Contract.EndContractBlock();
			Conversion conversion = ConversionFactory.GetPreDefinedConversion(fromType, type);
			return conversion != null && conversion.ConversionType != ConversionType.None &&
				(isExplicit || conversion.ConversionType.IsImplicit());
		}
		/// <summary>
		/// 返回 <paramref name="inputType"/> 类型和 <paramref name="outputType"/> 类型之间的标准转换类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns><paramref name="inputType"/> 类型和 <paramref name="outputType"/> 类型之间的标准转换类型。</returns>
		/// <remarks><para>这里标准转换指标准显式转换，包含了所有预定义隐式转换和预定义显式转换的子集，
		/// 该子集是预定义隐式转换反向的转换。也就是说，如果存在从 A 类型到 B 类型的预定义隐式转换，
		/// 那么 A 类型和 B 类型之间存在标准转换（A 到 B 或 B 到 A）。</para>
		/// <para>如果不存在标准类型转换，则总是返回 <see cref="ConversionType.None"/>，即使存在其它的类型转换。</para>
		/// </remarks>
		internal static ConversionType GetStandardConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null);
			if (inputType == typeof(void) || outputType == typeof(void))
			{
				return ConversionType.None;
			}
			return ConversionFactory.GetStandardConversion(inputType, outputType);
		}
		/// <summary>
		/// 返回 <paramref name="types"/> 中能够包含其它所有类型的类型（可以从其它所有类型隐式转换而来）。
		/// </summary>
		/// <param name="types">类型集合。</param>
		/// <returns><paramref name="types"/> 中能够包含其它所有类型的类型，如果不存在则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <remarks>若 A 类型可以隐式类型转换（指预定义的类型转换）为 B 类型，那么就称 A 被 B 包含，而 B 包含 A。</remarks>
		public static Type GetEncompassingType(IEnumerable<Type> types)
		{
			CommonExceptions.CheckArgumentNull(types, "types");
			Contract.EndContractBlock();
			Type encompassingType = null;
			foreach (Type type in types)
			{
				if (type == null)
				{
					continue;
				}
				if (encompassingType == null)
				{
					encompassingType = type;
				}
				else if (encompassingType != type)
				{
					// 这里剔除 void 类型，但若 types 全部是 void，能够使得结果是 void。
					if (encompassingType == typeof(void))
					{
						encompassingType = type;
					}
					else if (type != typeof(void))
					{
						ConversionType convType = ConversionFactory.GetStandardConversion(type, encompassingType);
						if (convType == ConversionType.None)
						{
							return null;
						}
						if (convType.IsExplicit())
						{
							encompassingType = type;
						}
					}
				}
			}
			return encompassingType;
		}
		/// <summary>
		/// 返回 <paramref name="types"/> 中能够被其它所有类型包含的类型（可以隐式转换为其它所有类型）。
		/// </summary>
		/// <param name="types">类型集合。</param>
		/// <returns><paramref name="types"/> 中能够被其它所有类型包含的类型，如果不存在则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <remarks>若 A 类型可以隐式类型转换（指预定义的类型转换）为 B 类型，那么就称 A 被 B 包含，而 B 包含 A。</remarks>
		public static Type GetEncompassedType(IEnumerable<Type> types)
		{
			CommonExceptions.CheckArgumentNull(types, "types");
			Contract.EndContractBlock();
			Type encompassedType = null;
			foreach (Type type in types)
			{
				if (type == null)
				{
					continue;
				}
				if (encompassedType == null)
				{
					encompassedType = type;
				}
				else if (encompassedType != type)
				{
					// 这里剔除 void 类型，但若 types 全部是 void，能够使得结果是 void。
					if (encompassedType == typeof(void))
					{
						encompassedType = type;
					}
					else if (type != typeof(void))
					{
						ConversionType convType = ConversionFactory.GetStandardConversion(encompassedType, type);
						if (convType == ConversionType.None)
						{
							return null;
						}
						if (convType.IsExplicit())
						{
							encompassedType = type;
						}
					}
				}
			}
			return encompassedType;
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
		public static Type CloseDefinitionFrom(this Type definition, Type type)
		{
			CommonExceptions.CheckArgumentNull(definition, "definition");
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.EndContractBlock();
			if (!definition.IsGenericTypeDefinition)
			{
				return null;
			}
			if (!definition.IsInterface && !type.IsInterface)
			{
				// 沿继承链向上查找。
				while (type != null)
				{
					if (type.IsGenericType && definition == type.GetGenericTypeDefinition())
					{
						return type;
					}
					type = type.BaseType;
				}
				return null;
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
		public static Type UniqueCloseDefinitionFrom(this Type definition, Type type)
		{
			CommonExceptions.CheckArgumentNull(definition, "definition");
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.EndContractBlock();
			if (!definition.IsGenericTypeDefinition)
			{
				return null;
			}
			if (!definition.IsInterface && !type.IsInterface)
			{
				// 沿继承链向上查找。
				while (type != null)
				{
					if (type.IsGenericType && definition == type.GetGenericTypeDefinition())
					{
						return type;
					}
					type = type.BaseType;
				}
				return null;
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
			UniqueValue<Type> unique = new UniqueValue<Type>();
			for (int i = 0; i < interfaces.Length && !unique.IsAmbig; i++)
			{
				if (interfaces[i].IsGenericType && definition == interfaces[i].GetGenericTypeDefinition())
				{
					unique.Value = interfaces[i];
				}
			}
			return unique.ValueOrDefault;
		}

		#endregion // 返回泛型类型定义的封闭构造类型

		#region Invoke 方法

		/// <summary>
		/// 获取委托类型的 Invoke 方法。
		/// </summary>
		/// <param name="dlg">委托。</param>
		/// <returns>委托类型的 Invoke 方法。</returns>
		internal static MethodInfo GetInvokeMethod(this Delegate dlg)
		{
			Contract.Requires(dlg != null);
			return dlg.GetType().GetMethod("Invoke");
		}
		/// <summary>
		/// 获取委托类型的 Invoke 方法。
		/// </summary>
		/// <param name="type">委托类型。</param>
		/// <returns>委托类型的 Invoke 方法。</returns>
		internal static MethodInfo GetInvokeMethod(this Type type)
		{
			Contract.Requires(type != null && type.IsSubclassOf(typeof(Delegate)));
			return type.GetMethod("Invoke");
		}

		#endregion // Invoke 方法

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
		/// <summary>
		/// 返回指定类型是否是 <see cref="IList{T}"/> 或其类型的泛型方法定义。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果是 <see cref="IList{T}"/> 或其类型的泛型方法定义，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		internal static bool IsIListOrBase(this Type type)
		{
			Contract.Requires(type != null);
			return type == typeof(IList<>) || type == typeof(ICollection<>) || type == typeof(IEnumerable<>);
		}
		/// <summary>
		/// 检查当前 <see cref="Type"/> 是否表示委托。
		/// </summary>
		/// <param name="type">要检查的类型。</param>
		/// <returns>如果当前 <see cref="Type"/> 表示委托，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsDelegate(this Type type)
		{
			return type != null && type.IsSubclassOf(typeof(Delegate));
		}
		/// <summary>
		/// 获取 <see cref="Type"/> 的完全限定名，包括 <see cref="Type"/> 的命名空间，但不包括程序集。
		/// </summary>
		/// <param name="type">要获取完全限定名的类型。</param>
		/// <returns>获取 <see cref="Type"/> 的完全限定名，包括 <see cref="Type"/> 的命名空间，但不包括程序集；
		/// 如果当前实例表示泛型类型参数、数组类型、指针类型或基于类型参数的 byref 类型，
		/// 或表示不属于泛型类型定义但包含无法解析的类型参数的泛型类型，则返回 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <remarks>与 <see cref="Type.FullName"/> 不同，即使如果当前 <see cref="Type"/> 表示泛型类型，
		/// <see cref="FullName"/> 返回的字符串中的类型实参也不会包含程序集、版本等信息。</remarks>
		public static string FullName(this Type type)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.Ensures(Contract.Result<string>() != null);
			if (!type.IsGenericType || type.ContainsGenericParameters)
			{
				return type.FullName;
			}
			StringBuilder text = new StringBuilder(type.GetGenericTypeDefinition().FullName);
			text.Append('[');
			Type[] args = type.GetGenericArguments();
			for (int i = 0; i < args.Length; i++)
			{
				if (i > 0)
				{
					text.Append(',');
				}
				Contract.Assume(args[i] != null);
				text.Append(args[i].FullName());
			}
			text.Append(']');
			return text.ToString();
		}
	}
}
