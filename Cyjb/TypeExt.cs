using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="System.Type"/> 类的扩展方法。
	/// </summary>
	public static class TypeExt
	{

		#region 类型转换运算符

		/// <summary>
		/// 类型的隐式和显式类型转换可以转换到的类型。
		/// </summary>
		private static readonly ICache<Type, Dictionary<Type, OperatorType>> TypeOperators =
			new LruCache<Type, Dictionary<Type, OperatorType>>(100);
		/// <summary>
		/// 类型转换运算符的类型。
		/// </summary>
		[Flags]
		private enum OperatorType
		{
			/// <summary>
			/// 可以从指定类型隐式转换。
			/// </summary>
			ImplicitFrom = 1,
			/// <summary>
			/// 可以隐式转换为指定类型。
			/// </summary>
			ImplicitTo = 2,
			/// <summary>
			/// 可以从指定类型强制转换。
			/// </summary>
			ExplicitFrom = 4,
			/// <summary>
			/// 可以强制转换为指定类型。
			/// </summary>
			ExplicitTo = 8,
			/// <summary>
			/// 可以从指定类型转换。
			/// </summary>
			From = 5,
			/// <summary>
			/// 可以转换为指定类型。
			/// </summary>
			To = 0xA
		}
		/// <summary>
		/// 返回指定类型可以转换到的类型。
		/// </summary>
		/// <param name="type">要获取类型转换的类型。</param>
		/// <returns>指定类型可以转换到的类型。</returns>
		private static Dictionary<Type, OperatorType> GetTypeOperators(Type type)
		{
			return TypeOperators.GetOrAdd(type, t =>
			{
				Dictionary<Type, OperatorType> dict = new Dictionary<Type, OperatorType>();
				MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
				for (int i = 0; i < methods.Length; i++)
				{
					MethodInfo m = methods[i];
					OperatorType op = OperatorType.ExplicitTo;
					Type opType = null;
					if (m.ReturnType == type)
					{
						opType = m.GetParameters()[0].ParameterType;
						if (m.Name == "op_Implicit")
						{
							op = OperatorType.ImplicitFrom;
						}
						else if (m.Name == "op_Explicit")
						{
							op = OperatorType.ExplicitFrom;
						}
					}
					else
					{
						opType = m.ReturnType;
						if (m.Name == "op_Implicit")
						{
							op = OperatorType.ImplicitTo;
						}
						else if (m.Name == "op_Explicit")
						{
							op = OperatorType.ExplicitTo;
						}
					}
					OperatorType oldOp;
					if (dict.TryGetValue(opType, out oldOp))
					{
						dict[opType] = oldOp | op;
					}
					else
					{
						dict.Add(opType, op);
					}
				}
				return dict;
			});
		}

		#endregion // 类型转换运算符

		#region 是否可以进行隐式类型转换

		/// <summary>
		/// 可以进行隐式类型转换的类型字典。
		/// </summary>
		private static readonly Dictionary<Type, HashSet<Type>> ConvertFromDict = new Dictionary<Type, HashSet<Type>>() { 
			{ typeof(short), new HashSet<Type>(){ typeof(sbyte), typeof(byte) } },
			{ typeof(ushort), new HashSet<Type>(){ typeof(char), typeof(byte) } },
			{ typeof(int), new HashSet<Type>(){ typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort) } },
			{ typeof(uint), new HashSet<Type>(){ typeof(char), typeof(byte), typeof(ushort) } },
			{ typeof(long), new HashSet<Type>(){ typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort),
				typeof(int), typeof(uint) } },
			{ typeof(ulong), new HashSet<Type>(){ typeof(char), typeof(byte), typeof(ushort), typeof(uint) } },
			{ typeof(float), new HashSet<Type>(){ typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort),
				typeof(int), typeof(uint), typeof(long), typeof(ulong) } },
			{ typeof(double), new HashSet<Type>(){ typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort),
				typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float) } },
			{ typeof(decimal), new HashSet<Type>(){ typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort),
				typeof(int), typeof(uint), typeof(long), typeof(ulong) } },
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
			HashSet<Type> typeSet;
			if (ConvertFromDict.TryGetValue(type, out typeSet))
			{
				return typeSet.Contains(fromType);
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
			// 总是可以隐式类型转换为 Object。
			if (type == typeof(object))
			{
				return true;
			}
			// 对 Nullable<T> 的支持。
			Type[] genericArguments;
			if (InInheritanceChain(typeof(Nullable<>), type, out genericArguments))
			{
				type = genericArguments[0];
				if (InInheritanceChain(typeof(Nullable<>), fromType, out genericArguments))
				{
					fromType = genericArguments[0];
				}
			}
			// 判断是否可以从实例分配。
			if (IsAssignableFromEx(type, fromType))
			{
				return true;
			}
			// 对隐式类型转换运算符进行判断。
			if (GetTypeOperators(type).Any(pair => pair.Value.HasFlag(OperatorType.ImplicitFrom) &&
				IsAssignableFromEx(pair.Key, fromType)))
			{
				return true;
			}
			if (GetTypeOperators(fromType).Any(pair => pair.Value.HasFlag(OperatorType.ImplicitTo) &&
				IsAssignableFromEx(type, pair.Key)))
			{
				return true;
			}
			return false;
		}

		#endregion // 是否可以进行隐式类型转换

		#region 是否可以进行强制类型转换

		/// <summary>
		/// 可以进行强制类型转换的类型字典。
		/// </summary>
		private static readonly Dictionary<Type, HashSet<Type>> CastFromDict = CreateCastFromDict();
		/// <summary>
		/// 构造可以进行强制类型转换的类型字典。
		/// </summary>
		private static Dictionary<Type, HashSet<Type>> CreateCastFromDict()
		{
			Dictionary<Type, HashSet<Type>> dict = new Dictionary<Type, HashSet<Type>>();
			// 互相可以进行强制类型转换的类型。
			HashSet<Type> set = new HashSet<Type>(){ typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort),
				typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };
			foreach (Type type in set)
			{
				dict.Add(type, set);
			}
			return dict;
		}
		/// <summary>
		/// 确定当前的 <see cref="System.Type"/> 的实例是否可以从指定 <see cref="System.Type"/> 
		/// 的实例进行内置强制类型转换。
		/// </summary>
		/// <param name="type">要判断的实例。</param>
		/// <param name="fromType">要与当前类型进行比较的类型。</param>
		/// <returns>如果当前 <see cref="System.Type"/> 可以从 <paramref name="fromType"/>
		/// 的实例分配或进行内置强制类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool IsAssignableFromCastEx(Type type, Type fromType)
		{
			if (type.IsAssignableFrom(fromType))
			{
				return true;
			}
			HashSet<Type> typeSet;
			if (CastFromDict.TryGetValue(type, out typeSet))
			{
				return typeSet.Contains(fromType);
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
			// 总是可以与 Object 进行强制类型转换。
			if (type == typeof(object) || fromType == typeof(object))
			{
				return true;
			}
			// 对 Nullable<T> 的支持。
			Type[] genericArguments;
			if (InInheritanceChain(typeof(Nullable<>), type, out genericArguments))
			{
				type = genericArguments[0];
			}
			if (InInheritanceChain(typeof(Nullable<>), fromType, out genericArguments))
			{
				fromType = genericArguments[0];
			}
			// 判断是否可以从实例分配，强制类型转换允许沿着继承链反向转换。
			if (IsAssignableFromCastEx(type, fromType) || IsAssignableFromCastEx(fromType, type))
			{
				return true;
			}
			// 对强制类型转换运算符进行判断。
			if (GetTypeOperators(type).Any(pair => pair.Value.AnyFlag(OperatorType.From) &&
				IsAssignableFromCastEx(pair.Key, fromType)))
			{
				return true;
			}
			if (GetTypeOperators(fromType).Any(pair => pair.Value.AnyFlag(OperatorType.To) &&
				IsAssignableFromCastEx(type, pair.Key)))
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
				if (!type.IsInterface || fromType.IsInterface)
				{
					// 如果 type 是接口而 fromType 是类型，则无需查找继承链。
					if (InInheritanceChain(type, fromType, out genericArguments))
					{
						return true;
					}
				}
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
			genericArguments = null;
			return false;
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
