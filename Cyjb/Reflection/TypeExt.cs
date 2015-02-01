using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
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
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
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
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (fromType == null)
			{
				throw CommonExceptions.ArgumentNull("otherType");
			}
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
		/// 下面是 <see cref="IsImplicitFrom"/> 方法的一些实例。
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
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			if (fromType == null)
			{
				throw CommonExceptions.ArgumentNull("otherType");
			}
			Contract.EndContractBlock();
			Conversion conversion = ConversionFactory.GetPreDefinedConversion(fromType, type);
			return conversion != null && conversion.ConversionType != ConversionType.None;
		}
		/// <summary>
		/// 返回 <paramref name="types"/> 中能够包含其它所有类型的类型。
		/// </summary>
		/// <param name="types">类型集合。</param>
		/// <returns><paramref name="types"/> 中能够包含其它所有类型的类型，如果不存在则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <remarks>若 A 类型可以隐式类型转换（指预定义的类型转换）为 B 类型，那么就称 A 被 B 包含，而 B 包含 A。</remarks>
		public static Type GetEncompassingType(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw CommonExceptions.ArgumentNull("types");
			}
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
		/// 返回 <paramref name="types"/> 中能够被其它所有类型包含的类型。
		/// </summary>
		/// <param name="types">类型集合。</param>
		/// <returns><paramref name="types"/> 中能够被其它所有类型包含的类型，如果不存在则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <remarks>若 A 类型可以隐式类型转换（指预定义的类型转换）为 B 类型，那么就称 A 被 B 包含，而 B 包含 A。</remarks>
		public static Type GetEncompassedType(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw CommonExceptions.ArgumentNull("types");
			}
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
			if (definition == null)
			{
				throw CommonExceptions.ArgumentNull("definition");
			}
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			Contract.EndContractBlock();
			if (!definition.IsGenericTypeDefinition)
			{
				return null;
			}
			if (definition.IsInterface == type.IsInterface)
			{
				// 沿继承链向上查找。
				while (type != null)
				{
					if (type.IsGenericType)
					{
						if (definition == type.GetGenericTypeDefinition())
						{
							return type;
						}
					}
					type = type.BaseType;
				}
				return null;
			}
			if (definition.IsInterface)
			{
				// 查找实现的接口。
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					if (interfaces[i].IsGenericType && definition == interfaces[i].GetGenericTypeDefinition())
					{
						return interfaces[i];
					}
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
			if (definition == null)
			{
				throw CommonExceptions.ArgumentNull("definition");
			}
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			Contract.EndContractBlock();
			if (!definition.IsGenericTypeDefinition)
			{
				return null;
			}
			if (definition.IsInterface == type.IsInterface)
			{
				// 沿继承链向上查找。
				while (type != null)
				{
					if (type.IsGenericType)
					{
						if (definition == type.GetGenericTypeDefinition())
						{
							return type;
						}
					}
					type = type.BaseType;
				}
				return null;
			}
			if (definition.IsInterface)
			{
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
				if (unique.IsUnique)
				{
					return unique.Value;
				}
			}
			return null;
		}

		#endregion // 返回泛型类型定义的封闭构造类型

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
			tempParamType = Nullable.GetUnderlyingType(paramType);
			if (tempParamType != null)
			{
				Type tempType = Nullable.GetUnderlyingType(type);
				if (tempType != null)
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
			tempParamType = Nullable.GetUnderlyingType(paramType);
			if (tempParamType != null)
			{
				tempType = Nullable.GetUnderlyingType(type);
				if (tempType != null)
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
				if ((tempType = tempParamType.UniqueCloseDefinitionFrom(type)) != null)
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
			tempParamType = Nullable.GetUnderlyingType(paramType);
			if (tempParamType != null)
			{
				tempType = Nullable.GetUnderlyingType(type);
				if (tempType != null)
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
				if ((tempParamType = tempType.UniqueCloseDefinitionFrom(paramType)) != null)
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
					if (!testType.IsImplicitFrom(type)) { return false; }
				}
				return true;
			}
		}

		#endregion // 泛型参数推断

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
			if (type == null)
			{
				throw CommonExceptions.ArgumentNull("type");
			}
			Contract.EndContractBlock();
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
		/// <summary>
		/// 返回获取类型的数组深度。
		/// </summary>
		/// <param name="type">要获取数组深度的类型。</param>
		/// <returns>如果给定类型是数组，则为数组的深度；否则为 <c>0</c>。</returns>
		internal static int GetArrayDepth(this Type type)
		{
			Type elementType;
			return GetArrayDepth(type, out elementType);
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
