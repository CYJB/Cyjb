using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Security;
using Cyjb.Reflection;

namespace Cyjb.Utility
{
	/// <summary>
	/// 允许根据配置文件使用不同的缓冲池配置的工厂类。
	/// </summary>
	/// <remarks>
	/// 配置文件示例：
	/// <code>
	/// <configSections>
	///		<!-- 这里的 Section 名称必须为 cyjb.cache -->
	///		<section name="cyjb.cache" type="Cyjb.Utility.CacheSection, Cyjb"/>
	/// </configSections>
	/// <cyjb.cache>
	///		<!-- key 是缓冲池的键，cacheType 是缓冲池的类型，option 是缓冲池构造函数参数的名称和相应的值。 -->
	///		<cache key="Cyjb.EnumDescriptionCache" cacheType="Cyjb.Utility.LruCacheNoSync`2, Cyjb" >
	///			<option name="maxSize" value="100" />
	///		</cache>
	/// </cyjb.cache>
	/// </code>
	/// </remarks>
	public static class CacheFactory
	{
		/// <summary>
		/// 缓冲池配置节的名称。
		/// </summary>
		public const string SectionName = "cyjb.cache";
		/// <summary>
		/// 当需要为指定键创建缓冲池时发生。
		/// </summary>
		public static event EventHandler<CacheResolveEventArgs> CacheResolve;
		/// <summary>
		/// 返回与指定的键关联的缓冲池。
		/// </summary>
		/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
		/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
		/// <param name="key">要获取的缓冲池的键。</param>
		/// <returns>与指定的键关联的缓冲池。如果配置无效且 <see cref="CacheResolve"/> 事件不能返回正确的缓冲池，
		/// 则返回 <c>null</c>。</returns>
		/// <remarks>如果配置信息不存在，会引发 <see cref="CacheResolve"/> 事件来创建缓冲池。如果
		/// <see cref="CacheResolve"/> 事件没有创建缓冲池，则返回 <c>null</c>。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public static ICache<TKey, TValue> Create<TKey, TValue>(string key)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			Contract.EndContractBlock();
			try
			{
				ICache<TKey, TValue> cache = CreateInternal<TKey, TValue>(key);
				return cache ?? OnCacheResolve<TKey, TValue>(key, null);
			}
			catch (ConfigurationErrorsException ex)
			{
				return OnCacheResolve<TKey, TValue>(key, ex);
			}
		}
		/// <summary>
		/// 返回与指定的键关联的缓冲池。
		/// </summary>
		/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
		/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
		/// <param name="key">要获取的缓冲池的键。</param>
		/// <returns>与指定的键关联的缓冲池，如果未能找到则返回 <c>null</c>。</returns>
		private static ICache<TKey, TValue> CreateInternal<TKey, TValue>(string key)
		{
			Contract.Requires(key != null);
			// 读取配置文件。
			CacheSection section = ConfigurationManager.GetSection(SectionName) as CacheSection;
			if (section == null)
			{
				return null;
			}
			CacheElement element = section.Caches[key];
			if (element == null)
			{
				return null;
			}
			Type cacheType = GetCacheType<TKey, TValue>(element);
			// 读取缓冲池设置。
			Dictionary<string, string> options = new Dictionary<string, string>(element.Options.Count,
				StringComparer.OrdinalIgnoreCase);
			foreach (NameValueConfigurationElement nv in element.Options)
			{
				options.Add(nv.Name, nv.Value);
			}
			return CreateCacheType(cacheType, options) as ICache<TKey, TValue> ?? OnCacheResolve<TKey, TValue>(key, null);
		}
		/// <summary>
		/// 获取缓冲池的类型。
		/// </summary>
		/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
		/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
		/// <param name="element">缓冲池的配置元素。</param>
		/// <returns>缓冲池的类型。</returns>
		private static Type GetCacheType<TKey, TValue>(CacheElement element)
		{
			Contract.Requires(element != null);
			string typeName = element.CacheType;
			// 读取缓冲池类型，类型总是开放泛型类型。
			if (typeName.IndexOf(',') == -1)
			{
				// 如果不是类型的限定名 AssemblyQualifiedName，
				// 则尝试自动补全类型名称最后的 `2。
				if (!typeName.EndsWith("`2", StringComparison.Ordinal))
				{
					typeName += "`2";
				}
			}
			Type cacheType = Type.GetType(typeName, false, true);
			if (cacheType == null)
			{
				throw CommonExceptions.InvalidCacheType(element);
			}
			// 构造封闭泛型类型。
			try
			{
				cacheType = cacheType.MakeGenericType(typeof(TKey), typeof(TValue));
			}
			catch (ArgumentException ex)
			{
				throw CommonExceptions.InvalidCacheType(element, ex);
			}
			// 缓冲池类型不是 ICache{TKey, TValue}。
			if (!typeof(ICache<TKey, TValue>).IsAssignableFrom(cacheType))
			{
				throw CommonExceptions.InvalidCacheType_ICache(element);
			}
			return cacheType;
		}
		/// <summary>
		/// 使用指定参数创建指定类型的实例。
		/// </summary>
		/// <param name="type">要创建的实例类型。</param>
		/// <param name="arguments">使用的参数。</param>
		/// <returns>创建的实例，如果失败则为 <c>null</c>。</returns>
		private static object CreateCacheType(Type type, Dictionary<string, string> arguments)
		{
			Contract.Requires(type != null && arguments != null);
			int argCnt = arguments.Count;
			object[] values = new object[argCnt];
			// 使用反射检索缓冲池类型，这里不能直接用 PowerBinder，是因为参数类型是未知的。
			// 找到与设置个数和名称匹配的构造函数。
			ConstructorInfo[] ctors = type.GetConstructors();
			for (int i = 0; i < ctors.Length; i++)
			{
				ParameterInfo[] parameters = ctors[i].GetParametersNoCopy();
				if (parameters.Length != argCnt) { continue; }
				// 测试参数名称是否全部能匹配上，并进行参数类型转换。
				int j = 0;
				for (; j < argCnt; j++)
				{
					string value;
					if (!arguments.TryGetValue(parameters[j].Name, out value))
					{
						break;
					}
					// 尝试进行类型转换。
					try
					{
						values[j] = Convert.ChangeType(value, parameters[j].ParameterType);
					}
					catch (InvalidCastException)
					{
						break;
					}
					catch (FormatException)
					{
						break;
					}
					catch (OverflowException)
					{
						break;
					}
				}
				if (j < argCnt) { continue; }
				// 找到了匹配的构造函数，构造实例。
				try
				{
					return ctors[i].Invoke(values);
				}
				catch (MemberAccessException) { }
				catch (TargetInvocationException) { }
				catch (SecurityException) { }
			}
			return null;
		}
		/// <summary>
		/// 引发 <see cref="CacheResolve"/> 事件。
		/// </summary>
		/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
		/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
		/// <param name="key">缓冲池的键。</param>
		/// <param name="innerException">发生的内部异常。</param>
		private static ICache<TKey, TValue> OnCacheResolve<TKey, TValue>(string key,
			ConfigurationErrorsException innerException)
		{
			Contract.Requires(key != null);
			EventHandler<CacheResolveEventArgs> events = CacheResolve;
			if (events != null)
			{
				CacheResolveEventArgs args = new CacheResolveEventArgs(
					key, typeof(TKey), typeof(TValue), innerException);
				events(null, args);
				return args.CacheObject as ICache<TKey, TValue>;
			}
			return null;
		}
	}
}
