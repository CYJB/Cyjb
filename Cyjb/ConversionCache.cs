using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 表示用户定义类型转换的缓存。
	/// </summary>
	internal static class ConversionCache
	{
		/// <summary>
		/// 隐式类型转换方法的名称。
		/// </summary>
		private const string ImplicitConversionName = "op_Implicit";
		/// <summary>
		/// 显式类型转换方法的名称。
		/// </summary>
		private const string ExplicitConviersionName = "op_Explicit";
		/// <summary>
		/// 类型的隐式和显式类型转换可以转换到的类型。
		/// </summary>
		private static readonly ICache<Type, Conversion> Conversions =
			CacheFactory.CreateCache<Type, Conversion>("Cyjb.ConversionCache") ?? new LruCache<Type, Conversion>(100);
		/// <summary>
		/// 返回指定类型中用户定义的转换方法。
		/// </summary>
		/// <param name="type">要获取类型转换方法的类型。</param>
		/// <returns>指定类型中用户定义的转换方法。</returns>
		private static Conversion GetTypeConversions(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			if (typeCode != TypeCode.Object && typeCode != TypeCode.Decimal)
			{
				// 其余内置类型都不包含类型转换运算符。
				return Conversion.Empty;
			}
			return Conversions.GetOrAdd(type, t =>
			{
				Conversion conv = new Conversion();
				List<ConversionMethod> cList = new List<ConversionMethod>();
				MethodInfo[] methods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				for (int i = 0; i < methods.Length; i++)
				{
					MethodInfo m = methods[i];
					bool opImplicit = m.Name.Equals(ImplicitConversionName, StringComparison.Ordinal);
					// 如果 opImplicit 已经为 true，则不需要再次进行方法名称的比较。
					bool opExplicit = opImplicit ? false : m.Name.Equals(ExplicitConviersionName, StringComparison.Ordinal);
					if (opImplicit || opExplicit)
					{
						if (m.ReturnType == type)
						{
							// 转换自其它类型。
							int index = conv.ImplicitConvertFromIndex;
							if (opExplicit)
							{
								index = Conversion.ConvertFromIndex;
								conv.ImplicitConvertFromIndex++;
							}
							conv.ConvertToIndex++;
							conv.ImplicitConvertToIndex++;
							cList.Insert(index, new ConversionMethod(m.GetParameters()[0].ParameterType, t, m));
						}
						else
						{
							// 转换到其它类型。
							int index = conv.ImplicitConvertToIndex;
							if (opExplicit)
							{
								index = conv.ConvertToIndex;
								conv.ImplicitConvertToIndex++;
							}
							cList.Insert(index, new ConversionMethod(t, m.ReturnType, m));
						}
					}
				}
				// 如果子类的类型转换运算符与基类完全相同，则为 true；否则为 false。
				bool sameWithBase = (cList.Count == 0);
				// 基类转换为其它类型的运算符是可以由子类继承的。
				if (type.IsClass)
				{
					Conversion baseConv = GetTypeConversions(type.BaseType);
					for (int i = baseConv.ConvertToIndex; i < baseConv.Methods.Length; i++)
					{
						ConversionMethod m = baseConv.Methods[i];
						bool contains = false;
						for (int j = conv.ConvertToIndex; j < cList.Count; j++)
						{
							if (cList[j].TargetType == m.TargetType)
							{
								sameWithBase = false;
								contains = true;
								break;
							}
						}
						if (!contains)
						{
							if (i >= baseConv.ImplicitConvertToIndex)
							{
								cList.Insert(conv.ImplicitConvertToIndex, m);
							}
							else
							{
								cList.Insert(conv.ConvertToIndex, m);
								conv.ImplicitConvertToIndex++;
							}
						}
					}
					if (sameWithBase)
					{
						// 这时候可以略微节约内存。
						return baseConv;
					}
				}
				if (sameWithBase)
				{
					// 这时候可以略微节约内存。
					return Conversion.Empty;
				}
				conv.Methods = cList.ToArray();
				return conv;
			});
		}
		/// <summary>
		/// 返回从源类型到目标类型的用户定义的隐式转换方法。
		/// 该转换方法的参数与源类型和目标类型并不一定完全相同，但保证存在标准隐式转换。
		/// 不支持提升转换运算符，不允许两个类同时为 nullable 类型。
		/// </summary>
		/// <param name="sourceType">要获取用户定义的转换方法的源类型。</param>
		/// <param name="targetType">要获取用户定义的转换方法的目标类型。</param>
		/// <returns>如果存在从源类型到目标类型的用户定义的隐式转换方法，则返回该方法；
		/// 否则返回 <c>null</c>。</returns>
		public static ConversionMethod GetImplicitConversion(Type sourceType, Type targetType)
		{
			Debug.Assert(!(sourceType.IsNullableType() && targetType.IsNullableType()));
			Type exactSource = null, exactTarget = null;
			UniqueValue<ConversionMethod> method = new UniqueValue<ConversionMethod>();
			Conversion conv = GetTypeConversions(TypeExt.GetNonNullableType(targetType));
			for (int i = conv.ImplicitConvertFromIndex; i < conv.ConvertToIndex; i++)
			{
				ConversionMethod m = conv.Methods[i];
				if (m.SourceType.IsStandardImplicitFrom(sourceType))
				{
					GetBestConversion(m, ref exactSource, ref exactTarget, method);
				}
			}
			if (!sourceType.IsNullableType())
			{
				conv = GetTypeConversions(sourceType);
				for (int i = conv.ImplicitConvertToIndex; i < conv.Methods.Length; i++)
				{
					ConversionMethod m = conv.Methods[i];
					if (targetType.IsStandardImplicitFrom(m.TargetType))
					{
						GetBestConversion(m, ref exactSource, ref exactTarget, method);
					}
				}
			}
			if (method.IsUnique)
			{
				return method.Value;
			}
			return null;
		}
		/// <summary>
		/// 返回从源类型到目标类型的用户定义的显式转换方法。
		/// 该转换方法的参数与源类型和目标类型并不一定完全相同，但保证存在标准显式转换。
		/// </summary>
		/// <param name="sourceType">要获取用户定义的转换方法的源类型。</param>
		/// <param name="targetType">要获取用户定义的转换方法的目标类型。</param>
		/// <returns>如果存在从源类型到目标类型的用户定义的显式转换方法，则返回该方法；
		/// 否则返回 <c>null</c>。</returns>
		public static ConversionMethod GetExplicitConversion(Type sourceType, Type targetType)
		{
			Type exactSource = null, exactTarget = null;
			UniqueValue<ConversionMethod> method = new UniqueValue<ConversionMethod>();
			Conversion conv = GetTypeConversions(sourceType.GetNonNullableType());
			for (int i = conv.ConvertToIndex; i < conv.Methods.Length; i++)
			{
				ConversionMethod m = conv.Methods[i];
				if (targetType.IsStandardExplicitFrom(m.TargetType))
				{
					GetBestConversion(m, ref exactSource, ref exactTarget, method);
				}
			}
			conv = GetTypeConversions(targetType.GetNonNullableType());
			for (int i = Conversion.ConvertFromIndex; i < conv.ConvertToIndex; i++)
			{
				ConversionMethod m = conv.Methods[i];
				if (m.SourceType.IsStandardExplicitFrom(sourceType))
				{
					GetBestConversion(m, ref exactSource, ref exactTarget, method);
				}
			}
			if (method.IsUnique)
			{
				return method.Value;
			}
			return null;
		}
		/// <summary>
		/// 根据给定的用户定义转换获取最合适的转换。
		/// </summary>
		/// <param name="method">要测试的用户定义转换。</param>
		/// <param name="exactSource">当前最合适的源类型。</param>
		/// <param name="exactTarget">当前最合适的目标类型。</param>
		/// <param name="uniqueMethod">最合适的转换。</param>
		private static void GetBestConversion(ConversionMethod method, ref Type exactSource, ref Type exactTarget,
			UniqueValue<ConversionMethod> uniqueMethod)
		{
			if (exactSource != method.SourceType)
			{
				if (exactSource != null && method.SourceType.IsAssignableFrom(exactSource))
				{
					return;
				}
				else
				{
					exactSource = method.SourceType;
					uniqueMethod.Reset();
				}
			}
			if (exactTarget != method.TargetType)
			{
				if (exactTarget != null && exactTarget.IsAssignableFrom(method.TargetType))
				{
					return;
				}
				else
				{
					exactTarget = method.TargetType;
					uniqueMethod.Reset();
				}
			}
			uniqueMethod.Value = method;
		}
		/// <summary>
		/// 表示某个类中的用户定义的类型转换方法。
		/// </summary>
		[DebuggerDisplay("Count = {Methods.Length}")]
		private class Conversion
		{
			/// <summary>
			/// 表示空的用户定义的类型转换方法。
			/// </summary>
			public static readonly Conversion Empty = new Conversion() { Methods = new ConversionMethod[0] };
			/// <summary>
			/// 类型转换方法列表。方法在其中的存储顺序为 ExplicitFrom, ImplicitFrom, ExplicitTo, ImplicitTo。
			/// </summary>
			public ConversionMethod[] Methods;
			/// <summary>
			/// 转换自方法在列表中的起始索引。
			/// </summary>
			public const int ConvertFromIndex = 0;
			/// <summary>
			/// 隐式转换自方法在列表中的起始索引。
			/// </summary>
			public int ImplicitConvertFromIndex = 0;
			/// <summary>
			/// 转换到方法在列表中的起始索引。
			/// </summary>
			public int ConvertToIndex = 0;
			/// <summary>
			/// 隐式转换到方法在列表中的起始索引。
			/// </summary>
			public int ImplicitConvertToIndex = 0;
		}
	}
}
