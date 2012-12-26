using System.Configuration;

namespace Cyjb.Utility
{
	/// <summary>
	/// 表示 cyjb.cache 缓冲池配置节。
	/// </summary>
	public sealed class CacheSection : ConfigurationSection
	{
		/// <summary>
		/// 获取缓存配置。
		/// </summary>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(CacheElementCollection), AddItemName = "cache")]
		public CacheElementCollection Caches
		{
			get { return ((CacheElementCollection)this[""]); }
		}
	}
}
