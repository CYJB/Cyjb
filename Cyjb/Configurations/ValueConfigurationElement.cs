using System.Configuration;

namespace Cyjb.Configurations
{
	/// <summary>
	/// 表示包含值的配置元素。
	/// </summary>
	public sealed class ValueConfigurationElement : ConfigurationElement
	{
		/// <summary>
		/// 初始化 <see cref="ValueConfigurationElement"/> 类的新实例。
		/// </summary>
		internal ValueConfigurationElement() { }
		/// <summary>
		/// 使用指定的值初始化 <see cref="ValueConfigurationElement"/> 类的新实例。
		/// </summary>
		/// <param name="value">配置元素的值。</param>
		public ValueConfigurationElement(string value)
		{
			this.Value = value;
		}
		/// <summary>
		/// 获取或设置配置元素的值。
		/// </summary>
		/// <value>配置元素的值。</value>
		[ConfigurationProperty("value", IsRequired = true, IsKey = true)]
		public string Value
		{
			get { return (string)base["value"]; }
			set { base["value"] = value; }
		}
	}
}
