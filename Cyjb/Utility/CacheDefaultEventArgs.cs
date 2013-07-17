using System;

namespace Cyjb.Utility
{
	/// <summary>
	/// 为创建默认缓冲池事件提供数据。
	/// </summary>
	public sealed class CacheDefaultEventArgs : EventArgs
	{
		/// <summary>
		/// 使用缓冲池的键和缓冲对象的类型初始化 <see cref="CacheDefaultEventArgs"/> 类的新实例。
		/// </summary>
		/// <param name="key">缓冲池的键。</param>
		/// <param name="keyType">缓冲对象的键的类型。</param>
		/// <param name="valueType">缓冲对象的类型。</param>
		public CacheDefaultEventArgs(string key, Type keyType, Type valueType)
		{
			this.Key = key;
			this.CacheKeyType = keyType;
			this.CacheValueType = valueType;
		}
		/// <summary>
		/// 获取或设置默认的缓冲池对象。
		/// </summary>
		public object CacheObject { get; set; }
		/// <summary>
		/// 获取缓冲池的键。
		/// </summary>
		public string Key { get; private set; }
		/// <summary>
		/// 缓冲对象的键的类型。
		/// </summary>
		public Type CacheKeyType { get; private set; }
		/// <summary>
		/// 缓冲对象的类型。
		/// </summary>
		public Type CacheValueType { get; private set; }
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
