using System;
using System.Configuration;

namespace Cyjb.Utility
{
	/// <summary>
	/// 为创建缓冲池事件提供数据。
	/// </summary>
	public sealed class CacheResolveEventArgs : EventArgs
	{
		/// <summary>
		/// 使用缓冲池的键、缓冲对象的类型和已发生的 
		/// <see cref="System.Configuration.ConfigurationErrorsException"/> 初始化 
		/// <see cref="CacheResolveEventArgs"/> 类的新实例。
		/// </summary>
		/// <param name="key">缓冲池的键。</param>
		/// <param name="keyType">缓冲对象的键的类型。</param>
		/// <param name="valueType">缓冲对象的类型。</param>
		/// <param name="exception">已发生的 <see cref="System.Exception"/>。</param>
		public CacheResolveEventArgs(string key, Type keyType, Type valueType,
			ConfigurationErrorsException exception)
		{
			this.Key = key;
			this.CacheKeyType = keyType;
			this.CacheValueType = valueType;
			this.Exception = exception;
		}
		/// <summary>
		/// 获取或设置新的缓冲池对象。
		/// </summary>
		/// <value>缓冲池对象。</value>
		public object CacheObject { get; set; }
		/// <summary>
		/// 获取缓冲池的键。
		/// </summary>
		/// <value>缓冲池的键。</value>
		public string Key { get; private set; }
		/// <summary>
		/// 缓冲对象的键的类型。
		/// </summary>
		/// <value>缓冲对象的键的类型。</value>
		public Type CacheKeyType { get; private set; }
		/// <summary>
		/// 缓冲对象的类型。
		/// </summary>
		/// <value>缓冲对象的类型。</value>
		public Type CacheValueType { get; private set; }
		/// <summary>
		/// 获取创建缓冲池时已发生的 <see cref="System.Configuration.ConfigurationErrorsException"/>。
		/// </summary>
		/// <value>已发生的 <see cref="System.Configuration.ConfigurationErrorsException"/>。</value>
		public ConfigurationErrorsException Exception { get; private set; }
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Concat(Key, " ICache<", CacheKeyType, ", ", CacheValueType, ">");
		}
	}
}
