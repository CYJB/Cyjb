using System;
using System.Collections.Generic;
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
		private static readonly ICache<RuntimeTypeHandle, IDictionary<RuntimeTypeHandle, ConversionMethod>> Conversions =
			CacheFactory.CreateCache<RuntimeTypeHandle, IDictionary<RuntimeTypeHandle, ConversionMethod>>("Cyjb.ConversionCache") ??
			new LruCache<RuntimeTypeHandle, IDictionary<RuntimeTypeHandle, ConversionMethod>>(100);
		/// <summary>
		/// 返回指定类型可以转换到的类型。
		/// </summary>
		/// <param name="type">要获取类型转换的类型。</param>
		/// <returns>指定类型可以转换到的类型。</returns>
		public static IDictionary<RuntimeTypeHandle, ConversionMethod> GetTypeOperators(RuntimeTypeHandle type)
		{
			return Conversions.GetOrAdd(type, t =>
			{
				IDictionary<RuntimeTypeHandle, ConversionMethod> dict = new Dictionary<RuntimeTypeHandle, ConversionMethod>();
				MethodInfo[] methods = Type.GetTypeFromHandle(t).GetMethods(BindingFlags.Public | BindingFlags.Static);
				for (int i = 0; i < methods.Length; i++)
				{
					MethodInfo m = methods[i];
					bool opImplicit = m.Name.Equals(ImplicitConversionName, StringComparison.Ordinal);
					// 如果 opImplicit 已经为 true，则不需要再次进行方法名称的比较。
					bool opExplicit = opImplicit ? false : m.Name.Equals(ExplicitConviersionName, StringComparison.Ordinal);
					if (opImplicit || opExplicit)
					{
						ConversionType cType = ConversionType.ExplicitTo;
						RuntimeTypeHandle opType;
						if (m.ReturnType == type)
						{
							// 转换自其它类型。
							opType = m.GetParameters()[0].ParameterType.TypeHandle;
							cType = opImplicit ? ConversionType.ImplicitFrom : ConversionType.ExplicitFrom;
							ConversionMethod cMethod;
							if (dict.TryGetValue(opType, out cMethod))
							{
								cMethod.ConversionType |= cType;
								cMethod.FromMethod = m.MethodHandle;
								dict[opType] = cMethod;
							}
							else
							{
								dict.Add(opType, new ConversionMethod() { ConversionType = cType, FromMethod = m.MethodHandle });
							}
						}
						else
						{
							// 转换到其它类型。
							opType = m.ReturnType.TypeHandle;
							cType = opImplicit ? ConversionType.ImplicitTo : ConversionType.ExplicitTo;
							ConversionMethod cMethod;
							if (dict.TryGetValue(opType, out cMethod))
							{
								cMethod.ConversionType |= cType;
								cMethod.ToMethod = m.MethodHandle;
								dict[opType] = cMethod;
							}
							else
							{
								dict.Add(opType, new ConversionMethod() { ConversionType = cType, ToMethod = m.MethodHandle });
							}
						}
					}
				}
				return dict;
			});
		}
	}
}
