using System;
using System.Collections.Generic;
using System.Linq;

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
		private static readonly Dictionary<RuntimeTypeHandle, HashSet<RuntimeTypeHandle>> ConvertFromDict =
			new Dictionary<RuntimeTypeHandle, HashSet<RuntimeTypeHandle>>() { 
				{ typeof(short).TypeHandle, new HashSet<RuntimeTypeHandle>(){ typeof(sbyte).TypeHandle, typeof(byte).TypeHandle } },
				{ typeof(ushort).TypeHandle, new HashSet<RuntimeTypeHandle>(){ typeof(char).TypeHandle, typeof(byte).TypeHandle } },
				{ typeof(int).TypeHandle, new HashSet<RuntimeTypeHandle>(){ 
					typeof(char).TypeHandle, typeof(sbyte).TypeHandle, typeof(byte).TypeHandle, 
					typeof(short).TypeHandle, typeof(ushort).TypeHandle } },
				{ typeof(uint).TypeHandle, new HashSet<RuntimeTypeHandle>(){ 
					typeof(char).TypeHandle, typeof(byte).TypeHandle, typeof(ushort).TypeHandle } },
				{ typeof(long).TypeHandle, new HashSet<RuntimeTypeHandle>(){ 
					typeof(char).TypeHandle, typeof(sbyte).TypeHandle, typeof(byte).TypeHandle, 
					typeof(short).TypeHandle, typeof(ushort).TypeHandle, typeof(int).TypeHandle, typeof(uint).TypeHandle } },
				{ typeof(ulong).TypeHandle, new HashSet<RuntimeTypeHandle>(){ 
					typeof(char).TypeHandle, typeof(byte).TypeHandle, typeof(ushort).TypeHandle, typeof(uint).TypeHandle } },
				{ typeof(float).TypeHandle, new HashSet<RuntimeTypeHandle>(){ 
					typeof(char).TypeHandle, typeof(sbyte).TypeHandle, typeof(byte).TypeHandle, typeof(short).TypeHandle, 
					typeof(ushort).TypeHandle, typeof(int).TypeHandle, typeof(uint).TypeHandle, 
					typeof(long).TypeHandle, typeof(ulong).TypeHandle } },
				{ typeof(double).TypeHandle, new HashSet<RuntimeTypeHandle>(){ 
					typeof(char).TypeHandle, typeof(sbyte).TypeHandle, typeof(byte).TypeHandle, typeof(short).TypeHandle, 
					typeof(ushort).TypeHandle, typeof(int).TypeHandle, typeof(uint).TypeHandle, typeof(long).TypeHandle, 
					typeof(ulong).TypeHandle, typeof(float).TypeHandle } },
				{ typeof(decimal).TypeHandle, new HashSet<RuntimeTypeHandle>(){ 
					typeof(char).TypeHandle, typeof(sbyte).TypeHandle, typeof(byte).TypeHandle, typeof(short).TypeHandle, 
					typeof(ushort).TypeHandle, typeof(int).TypeHandle, typeof(uint).TypeHandle, 
					typeof(long).TypeHandle, typeof(ulong).TypeHandle } },
		};
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行内置隐式类型转换。
		/// </summary>
		/// <param name="type">要判断的实例。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例分配或进行内置隐式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool IsAssignableFromEx(Type type, Type fromType)
		{
			if (type.IsAssignableFrom(fromType))
			{
				return true;
			}
			HashSet<RuntimeTypeHandle> typeSet;
			if (ConvertFromDict.TryGetValue(type.TypeHandle, out typeSet))
			{
				return typeSet.Contains(fromType.TypeHandle);
			}
			return false;
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
			if (type.TypeHandle.Equals(typeof(object).TypeHandle))
			{
				return true;
			}
			// 对 Nullable<T> 的支持。
			if (NullableAssignableFrom(ref type))
			{
				NullableAssignableFrom(ref fromType);
			}
			// 判断是否可以从实例分配。
			if (IsAssignableFromEx(type, fromType))
			{
				return true;
			}
			// 对隐式类型转换运算符进行判断。
			if (ConversionCache.GetTypeOperators(type.TypeHandle).Any(
				pair => ((pair.Value.ConversionType & ConversionType.ImplicitFrom) == ConversionType.ImplicitFrom) &&
					IsAssignableFromEx(Type.GetTypeFromHandle(pair.Key), fromType)))
			{
				return true;
			}
			if (ConversionCache.GetTypeOperators(fromType.TypeHandle).Any(
				pair => ((pair.Value.ConversionType & ConversionType.ImplicitTo) == ConversionType.ImplicitTo) &&
					IsAssignableFromEx(type, Type.GetTypeFromHandle(pair.Key))))
			{
				return true;
			}
			return false;
		}

		#endregion // 是否可以进行隐式类型转换

		#region 是否可以进行强制类型转换

		/// <summary>
		/// 可以进行强制类型转换的类型集合。
		/// </summary>
		/// <remarks>内置的强制类型转换是 Char, SByte, Byte, Int16, UInt16, Int32, UInt32, 
		/// Int64, UInt64, Single, Double 和 Decimal 之间的相互转换，没必要也是用字典来表示，用集合同样可以。</remarks>
		private static readonly HashSet<RuntimeTypeHandle> CastFromSet = new HashSet<RuntimeTypeHandle>(){ 
			typeof(char).TypeHandle, typeof(sbyte).TypeHandle, typeof(byte).TypeHandle, typeof(short).TypeHandle, 
			typeof(ushort).TypeHandle, typeof(int).TypeHandle, typeof(uint).TypeHandle, typeof(long).TypeHandle, 
			typeof(ulong).TypeHandle, typeof(float).TypeHandle, typeof(double).TypeHandle, typeof(decimal).TypeHandle };
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行内置强制类型转换。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例分配或进行内置强制类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool IsAssignableFromCastEx(Type type, Type fromType)
		{
			if (type.IsAssignableFrom(fromType) || fromType.IsAssignableFrom(type))
			{
				return true;
			}
			return CastFromSet.Contains(type.TypeHandle) && CastFromSet.Contains(fromType.TypeHandle);
		}
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 是否属于合法的枚举基础类型。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 是合法的枚举基础类型，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		private static bool IsEnumUnderlyingType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
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
		public static bool IsCastableFrom(this Type type, Type fromType)
		{
			if (type == null || fromType == null)
			{
				return false;
			}
			// 对引用类型的支持。
			if (type.IsByRef) { type = type.GetElementType(); }
			if (fromType.IsByRef) { fromType = type.GetElementType(); }
			// 总是可以与 Object 进行强制类型转换。
			if (type.TypeHandle.Equals(typeof(object).TypeHandle) ||
				fromType.TypeHandle.Equals(typeof(object).TypeHandle))
			{
				return true;
			}
			// 对 Nullable<T> 的支持。
			NullableAssignableFrom(ref type);
			NullableAssignableFrom(ref fromType);
			// 对枚举的支持。
			if (type.IsEnum)
			{
				if (fromType.IsEnum || IsEnumUnderlyingType(fromType))
				{
					return true;
				}
			}
			else if (fromType.IsEnum && IsEnumUnderlyingType(type))
			{
				return true;
			}
			// 判断是否可以从实例分配，强制类型转换允许沿着继承链反向转换。
			if (IsAssignableFromCastEx(type, fromType))
			{
				return true;
			}
			else if (fromType.IsEnum)
			{
				if (type.TypeHandle.Equals(typeof(Enum).TypeHandle))
				{
					return true;
				}
				fromType = Enum.GetUnderlyingType(fromType);
			}
			// 对强制类型转换运算符进行判断。
			if (ConversionCache.GetTypeOperators(type.TypeHandle).Any(
				pair => ((pair.Value.ConversionType & ConversionType.From) != 0) &&
					IsAssignableFromCastEx(Type.GetTypeFromHandle(pair.Key), fromType)))
			{
				return true;
			}
			if (ConversionCache.GetTypeOperators(fromType.TypeHandle).Any(
				pair => ((pair.Value.ConversionType & ConversionType.To) != 0) &&
					IsAssignableFromCastEx(type, Type.GetTypeFromHandle(pair.Key))))
			{
				return true;
			}
			return false;
		}

		#endregion // 是否可以进行强制类型转换

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
			if (type != null && fromType != null && type.IsGenericType)
			{
				if (type.IsInterface == fromType.IsInterface)
				{
					if (InInheritanceChain(type, fromType, out genericArguments))
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
						if (InInheritanceChain(type, interfaces[i], out genericArguments))
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
		/// 确定指定 <see cref="System.Type"/> 是否是 Nullable&lt;&gt; 泛型，如果是则返回泛型的参数。
		/// </summary>
		/// <param name="type">要判断是否可空类型的类型。如果是可空类型，则返回泛型类型参数；否则不变。</param>
		/// <returns>如果 <paramref name="type"/> 是可空类型，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		internal static bool NullableAssignableFrom(ref Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition().TypeHandle.Equals(typeof(Nullable<>).TypeHandle))
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
		/// 确定当前的开放泛型类型是否在指定 <see cref="System.Type"/> 类型的继承链中，并返回泛型的参数。
		/// </summary>
		/// <param name="type">要判断的开放泛型类型。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <param name="genericArguments">如果在继承链中，则返回泛型类型参数；否则返回 <c>null</c>。</param>
		/// <returns>如果当前的开放泛型类型在 <paramref name="fromType"/> 的继承链中，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool InInheritanceChain(Type type, Type fromType, out Type[] genericArguments)
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
