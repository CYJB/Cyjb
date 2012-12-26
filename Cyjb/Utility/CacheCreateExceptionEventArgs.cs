using System;
using System.Configuration;

namespace Cyjb.Utility
{
	/// <summary>
	/// 为 <see cref="CacheFactory.CacheCreateException"/> 事件提供数据。
	/// </summary>
	public sealed class CacheCreateExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// 使用已发生的 <see cref="System.Configuration.ConfigurationErrorsException"/> 
		/// 初始化 <see cref="CacheCreateExceptionEventArgs"/> 类的新实例。
		/// </summary>
		/// <param name="exception">已发生的 <see cref="System.Exception"/>。</param>
		public CacheCreateExceptionEventArgs(ConfigurationErrorsException exception)
		{
			this.Exception = exception;
		}
		/// <summary>
		/// 获取已发生的 <see cref="System.Exception"/>。
		/// </summary>
		/// <value>已发生的 <see cref="System.Exception"/>。</value>
		public ConfigurationErrorsException Exception { get; private set; }
	}
}
