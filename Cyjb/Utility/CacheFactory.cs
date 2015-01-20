using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Security;

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
		/// 当创建缓冲池出现异常时发生。
		/// </summary>
		public static event EventHandler<CacheResolveEventArgs> CacheResolve;
		/// <summary>
		/// 当创建默认缓冲池时发生。
		/// </summary>
		public static event EventHandler<CacheDefaultEventArgs> CacheDefault;
		/// <summary>
		/// 返回与指定的键关联的缓冲池。如果配置信息不存在，则返回 <c>null</c>。
		/// 如果配置文件出现错误，同样返回 <c>null</c>，这时候会发生 <see cref="CacheResolve"/> 事件。
		/// </summary>
		/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
		/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
		/// <param name="key">要获取的缓冲池的键。</param>
		/// <returns>与指定的键关联的缓冲池。如果配置不存在或非法，则返回 <c>null</c>。</returns>
		/// <exception cref="System.Configuration.ConfigurationErrorsException">配置文件错误。</exception>
		public static ICache<TKey, TValue> CreateCache<TKey, TValue>(string key)
		{
			// 读取配置文件。
			CacheSection section = null;
			try
			{
				section = ConfigurationManager.GetSection(SectionName) as CacheSection;
			}
			catch (ConfigurationErrorsException ex)
			{
				return OnCacheResolve<TKey, TValue>(key, ex);
			}
			if (section == null)
			{
				return OnCacheDefault<TKey, TValue>(key);
			}
			CacheElement element = section.Caches[key];
			if (element == null)
			{
				return OnCacheDefault<TKey, TValue>(key);
			}
			// 读取缓冲池类型。
			Type cacheType = null;
			try
			{
				cacheType = GetCacheType<TKey, TValue>(element);
			}
			catch (ConfigurationErrorsException ex)
			{
				return OnCacheResolve<TKey, TValue>(key, ex);
			}
			// 读取缓冲池设置。
			int cnt = element.Options.Count;
			Dictionary<string, NameValueConfigurationElement> options =
				new Dictionary<string, NameValueConfigurationElement>(cnt, StringComparer.OrdinalIgnoreCase);
			foreach (NameValueConfigurationElement nv in element.Options)
			{
				options.Add(nv.Name, nv);
			}
			// 使用反射检索缓冲池类型，这里不能直接用 PowerBinder，是因为参数类型是未知的。
			// 找到与设置个数和名称匹配的构造函数。
			ConstructorInfo[] ctors = cacheType.GetConstructors();
			object[] values = new object[cnt];
			for (int i = 0; i < ctors.Length; i++)
			{
				ParameterInfo[] parameters = ctors[i].GetParameters();
				if (parameters.Length != cnt) { continue; }
				// 测试参数名称是否全部能匹配上，并进行参数类型转换。
				int j = 0;
				for (; j < cnt; j++)
				{
					NameValueConfigurationElement value;
					if (!options.TryGetValue(parameters[j].Name, out value))
					{
						break;
					}
					// 尝试进行类型转换。
					try
					{
						values[j] = Convert.ChangeType(value.Value,
							parameters[j].ParameterType);
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
				if (j < cnt) { continue; }
				// 找到了匹配的构造函数，构造实例。
				try
				{
					return ctors[i].Invoke(values) as ICache<TKey, TValue>;
				}
				catch (MemberAccessException) { }
				catch (TargetInvocationException) { }
				catch (SecurityException) { }
			}
			return OnCacheResolve<TKey, TValue>(key, CommonExceptions.InvalidCacheOptions(element));
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
			// 构造闭合泛型类型。
			try
			{
				cacheType = cacheType.MakeGenericType(typeof(TKey), typeof(TValue));
			}
			catch (ArgumentException ex)
			{
				// 闭合泛型类型构造失败。
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
		/// 引发 <see cref="CacheResolve"/> 事件。
		/// </summary>
		/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
		/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
		/// <param name="key">缓冲池的键。</param>
		/// <param name="exception">配置错误异常。</param>
		private static ICache<TKey, TValue> OnCacheResolve<TKey, TValue>(string key,
			ConfigurationErrorsException exception)
		{
			EventHandler<CacheResolveEventArgs> events = CacheResolve;
			if (events != null)
			{
				CacheResolveEventArgs args = new CacheResolveEventArgs(
					key, typeof(TKey), typeof(TValue), exception);
				events(null, args);
				return args.CacheObject as ICache<TKey, TValue>;
			}
			return null;
		}
		/// <summary>
		/// 引发 <see cref="CacheDefault"/> 事件。
		/// </summary>
		/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
		/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
		/// <param name="key">缓冲池的键。</param>
		private static ICache<TKey, TValue> OnCacheDefault<TKey, TValue>(string key)
		{
			EventHandler<CacheDefaultEventArgs> events = CacheDefault;
			if (events != null)
			{
				CacheDefaultEventArgs args = new CacheDefaultEventArgs(
					key, typeof(TKey), typeof(TValue));
				events(null, args);
				return args.CacheObject as ICache<TKey, TValue>;
			}
			return null;
		}
	}
}
