using System.Configuration;
using System.Diagnostics.Contracts;

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
		/// 初始化 <see cref="ConfigurationElementCollection{TKey,TElement}"/> 类的新实例。
		/// </summary>
		protected ConfigurationElementCollection() { }
		/// <summary>
		/// 基于所提供的键，从集合中移除 <see cref="ConfigurationElement"/> 对象。
		/// </summary>
		/// <param name="key">要移除的 <see cref="ConfigurationElement"/> 
		/// 对象的键。</param>
		/// <overloads>
		/// <summary>
		/// 从集合中移除 <see cref="ConfigurationElement"/> 对象。
		/// </summary>
		/// </overloads>
		public void Remove(TKey key)
		{
			BaseRemove(key);
		}
		/// <summary>
		/// 基于所提供的参数，获取或设置子配置元素。
		/// </summary>
		/// <param name="key">集合中包含的子配置元素的键。</param>
		/// <returns>子配置元素，如果不存在则返回 <c>null</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 获取或设置此 <see cref="ConfigurationElementCollection{T}"/> 对象的属性、特性或子元素。
		/// </summary>
		/// </overloads>
		public TElement this[TKey key]
		{
			get { return BaseGet(key) as TElement; }
			set
			{
				if (value == null)
				{
					throw CommonExceptions.ArgumentNull("value");
				}
				Contract.EndContractBlock();
				ConfigurationElement item = BaseGet(key);
				if (item != null)
				{
					BaseRemove(key);
				}
				try
				{
					this.BaseAdd(value);
				}
				catch
				{
					// 出错了，撤销删除。
					if (item != null)
					{
						this.BaseAdd(item);
					}
					throw;
				}
			}
		}
	}
}
