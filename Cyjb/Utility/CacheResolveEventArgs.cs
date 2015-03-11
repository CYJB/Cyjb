using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb.Utility
{
	/// <summary>
	/// 为创建指定键的缓冲池事件提供数据。
	/// </summary>
	public sealed class CacheResolveEventArgs : EventArgs
	{
		/// <summary>
		/// 缓冲池的键。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string key;
		/// <summary>
		/// 缓冲对象的键的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type keyType;
		/// <summary>
		/// 缓冲对象的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type valueType;
		/// <summary>
		/// 发生的内部异常。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ConfigurationErrorsException innerException;
		/// <summary>
		/// 使用缓冲池的键、缓冲对象的类型和发生的内部异常初始化 <see cref="CacheResolveEventArgs"/> 类的新实例。
		/// </summary>
		/// <param name="key">缓冲池的键。</param>
		/// <param name="keyType">缓冲对象的键的类型。</param>
		/// <param name="valueType">缓冲对象的类型。</param>
		/// <param name="innerException">发生的内部异常。</param>
		public CacheResolveEventArgs(string key, Type keyType, Type valueType, ConfigurationErrorsException innerException)
		{
			Contract.Requires(key != null && keyType != null && valueType != null);
			this.key = key;
			this.keyType = keyType;
			this.valueType = valueType;
			this.innerException = innerException;
		}
		/// <summary>
		/// 获取缓冲池的键。
		/// </summary>
		/// <value>缓冲池的键。</value>
		public string Key
		{
			get
			{
				Contract.Ensures(Contract.Result<string>() != null);
				return this.key;
			}
		}
		/// <summary>
		/// 缓冲对象的键的类型。
		/// </summary>
		/// <value>缓冲对象的键的类型。</value>
		public Type CacheKeyType
		{
			get
			{
				Contract.Ensures(Contract.Result<Type>() != null);
				return this.keyType;
			}
		}
		/// <summary>
		/// 缓冲对象的类型。
		/// </summary>
		/// <value>缓冲对象的类型。</value>
		public Type CacheValueType
		{
			get
			{
				Contract.Ensures(Contract.Result<Type>() != null);
				return this.valueType;
			}
		}
		/// <summary>
		/// 获取创建缓冲池时已发生的 <see cref="ConfigurationErrorsException"/>。
		/// </summary>
		/// <value>已发生的 <see cref="ConfigurationErrorsException"/>。</value>
		public ConfigurationErrorsException InnerException { get { return this.innerException; } }
		/// <summary>
		/// 获取或设置默认的缓冲池对象。
		/// </summary>
		/// <value>默认的缓冲池对象。</value>
		public object CacheObject { get; set; }
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Concat(key, " ICache<", keyType.FullName, ", ", valueType.FullName, ">");
		}
	}
}
