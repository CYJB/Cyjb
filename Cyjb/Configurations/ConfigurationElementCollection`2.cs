using System.Configuration;

namespace Cyjb.Configurations
{
	/// <summary>
	/// 表示强类型的包含一个子元素集合的配置元素。
	/// </summary>
	/// <typeparam name="TKey">子配置元素的键的类型。</typeparam>
	/// <typeparam name="TElement">子配置元素的类型。</typeparam>
	public abstract class ConfigurationElementCollection<TKey, TElement> : ConfigurationElementCollection<TElement>
		where TElement : ConfigurationElement
	{
		/// <summary>
		/// 基于所提供的键，从集合中移除 <see cref="System.Configuration.ConfigurationElement"/> 对象。
		/// </summary>
		/// <param name="key">要移除的 <see cref="System.Configuration.ConfigurationElement"/> 
		/// 对象的键。</param>
		public void Remove(TKey key)
		{
			base.BaseRemove(key);
		}
		/// <summary>
		/// 基于所提供的参数，获取或设置子配置元素。
		/// </summary>
		/// <param name="key">集合中包含的子配置元素的键。</param>
		/// <returns>子配置元素，如果不存在则返回 <c>null</c>。</returns>
		public TElement this[TKey key]
		{
			get { return base.BaseGet(key as object) as TElement; }
			set
			{
				object objKey = key;
				ConfigurationElement item = base.BaseGet(objKey);
				if (item != null)
				{
					base.BaseRemove(objKey);
				}
				try
				{
					this.BaseAdd(value);
				}
				finally
				{
					// 出错了，撤销删除。
					if (item != null)
					{
						this.BaseAdd(item);
					}
				}
			}
		}

	}
}
