using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb
{
	/// <summary>
	/// 允许跨域的进度更新的提供程序。
	/// </summary>
	/// <typeparam name="T">进度更新值的类型。</typeparam>
	public sealed class RemoteProgress<T> : MarshalByRefObject, IProgress<T>
	{
		/// <summary>
		/// 被包装的进度更新提供程序。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IProgress<T> progress;
		/// <summary>
		/// 使用被包装的进度更新提供程序初始化 <see cref="RemoteProgress{T}"/> 类的新实例。
		/// </summary>
		/// <param name="progress">被包装的进度更新提供程序。</param>
		public RemoteProgress(IProgress<T> progress)
		{
			CommonExceptions.CheckArgumentNull(progress, "progress");
			Contract.EndContractBlock();
			this.progress = progress;
		}

		#region IProgress<T> 成员

		/// <summary>
		/// 报告进度更新。
		/// </summary>
		/// <param name="value">更新进度的值。</param>
		public void Report(T value)
		{
			progress.Report(value);
		}

		#endregion // IProgress<T> 成员

	}
}
