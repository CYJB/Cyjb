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
	public static class CacheFactory
	{
		/// <summary>
		/// 当创建缓冲池出现异常时发生。
		/// </summary>
		public static event EventHandler<CacheCreateExceptionEventArgs> CacheCreateException;
		/// <summary>
		/// 返回与指定的键关联的缓冲池。如果配置信息不存在，则返回 <c>null</c>。
		/// 如果配置文件出现错误，同样返回 <c>null</c>，这时候会发生 <see cref="CacheCreateException"/> 事件。
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
				section = ConfigurationManager.GetSection("cyjb.cache") as CacheSection;
			}
			catch (ConfigurationErrorsException ex)
			{
				OnCacheCreateException(ex);
				return null;
			}
			if (section == null) { return null; }
			CacheElement element = section.Caches[key];
			if (element == null) { return null; }
			// 读取缓冲池类型。
			Type cacheType = GetCacheType<TKey, TValue>(element);
			if (cacheType == null) { return null; }
			// 读取缓冲池设置。
			int cnt = element.Options.Count;
			Dictionary<string, NameValueConfigurationElement> options =
				new Dictionary<string, NameValueConfigurationElement>(cnt, StringComparer.OrdinalIgnoreCase);
			foreach (NameValueConfigurationElement nv in element.Options)
			{
				options.Add(nv.Name, nv);
			}
			// 使用反射检索缓冲池类型。
			// 找到与设置个数和名称匹配的构造函数。
			ConstructorInfo[] ctors = cacheType.GetConstructors();
			for (int i = 0; i < ctors.Length; i++)
			{
				ParameterInfo[] parameters = ctors[i].GetParameters();
				if (parameters.Length == cnt)
				{
					// 测试参数名称是否全部能匹配上。
					int j = 0;
					while (j < cnt)
					{
						if (!options.ContainsKey(parameters[j++].Name))
						{
							break;
						}
					}
					if (j < cnt) { continue; }
					// 仅当参数名称能够完全匹配上时，才进行参数类型转换。
					object[] values = new object[cnt];
					for (j = 0; j < cnt; j++)
					{
						NameValueConfigurationElement value = options[parameters[j].Name];
						try
						{
							// 尝试进行类型转换。
							values[j] = Convert.ChangeType(value.Value,
								parameters[j].ParameterType, CultureInfo.InvariantCulture);
							continue;
						}
						catch (InvalidCastException icex)
						{
							OnCacheCreateException(ExceptionHelper.InvalidCacheOption(value, parameters[j].ParameterType, icex));
						}
						catch (FormatException fex)
						{
							OnCacheCreateException(ExceptionHelper.InvalidCacheOption(value, parameters[j].ParameterType, fex));
						}
						catch (OverflowException oex)
						{
							OnCacheCreateException(ExceptionHelper.InvalidCacheOption(value, parameters[j].ParameterType, oex));
						}
						return null;
					}
					// 找到了匹配的构造函数，构造实例。
					try
					{
						return ctors[i].Invoke(values) as ICache<TKey, TValue>;
					}
					catch (MemberAccessException maex)
					{
						OnCacheCreateException(ExceptionHelper.InvalidCacheType_CreateInstance(element, maex));
						return null;
					}
					catch (TargetInvocationException tiex)
					{
						OnCacheCreateException(ExceptionHelper.InvalidCacheType_CreateInstance(element, tiex));
						return null;
					}
					catch (SecurityException sex)
					{
						OnCacheCreateException(ExceptionHelper.InvalidCacheType_CreateInstance(element, sex));
						return null;
					}
				}
			}
			OnCacheCreateException(ExceptionHelper.InvalidCacheOptions(element));
			return null;
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
			// 自动补全类型名称最后的 `2。
			if (!typeName.EndsWith("`2", StringComparison.Ordinal))
			{
				typeName += "`2";
			}
			Type cacheType = Type.GetType(typeName, false, false);
			if (cacheType == null)
			{
				OnCacheCreateException(ExceptionHelper.InvalidCacheType(element));
				return null;
			}
			// 构造闭合泛型类型。
			try
			{
				cacheType = cacheType.MakeGenericType(typeof(TKey), typeof(TValue));
			}
			catch (ArgumentException ex)
			{
				// 闭合泛型类型构造失败。
				OnCacheCreateException(ExceptionHelper.InvalidCacheType(element, ex));
				return null;
			}
			// 缓冲池类型不是 ICache&lt;TKey, TValue&gt;。
			if (!typeof(ICache<TKey, TValue>).IsAssignableFrom(cacheType))
			{
				OnCacheCreateException(ExceptionHelper.InvalidCacheType_ICache(element));
				return null;
			}
			return cacheType;
		}
		/// <summary>
		/// 引发 <see cref="CacheCreateException"/> 事件。
		/// </summary>
		/// <param name="exception">配置错误异常。</param>
		private static void OnCacheCreateException(ConfigurationErrorsException exception)
		{
			EventHandler<CacheCreateExceptionEventArgs> events = CacheCreateException;
			if (events != null)
			{
				events(null, new CacheCreateExceptionEventArgs(exception));
			}
		}
	}
}
