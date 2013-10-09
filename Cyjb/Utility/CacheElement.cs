using System.Configuration;

namespace Cyjb.Utility
{
	/// <summary>
	/// 表示缓冲池的配置元素。
	/// </summary>
	public sealed class CacheElement : ConfigurationElement
	{
		/// <summary>
		/// 初始化 <see cref="CacheElement"/> 类的新实例。
		/// </summary>
		public CacheElement() { }
		/// <summary>
		/// 获取或设置缓冲池配置的键。
		/// </summary>
		/// <value>缓冲池配置的键。</value>
		[ConfigurationProperty("key", IsRequired = true, IsKey = true)]
		public string Key
		{
			get { return (string)base["key"]; }
			set { base["key"] = value; }
		}
		/// <summary>
		/// 获取或设置的缓冲池的类名。
		/// </summary>
		/// <value>缓冲池的类名。</value>
		[ConfigurationProperty("cacheType", IsRequired = true)]
		public string CacheType
		{
			get { return (string)base["cacheType"]; }
			set { base["cacheType"] = value; }
		}
		/// <summary>
		/// 获取缓冲池的选项列表。
		/// </summary>
		/// <value>缓冲池的选项列表。</value>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(NameValueConfigurationCollection), AddItemName = "option")]
		public NameValueConfigurationCollection Options
		{
			get { return (NameValueConfigurationCollection)base[""]; }
		}
	}
}
