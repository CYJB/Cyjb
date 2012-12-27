using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb.Utility
{
	/// <summary>
	/// 表示缓存个数不受限制的对象缓冲池。
	/// </summary>
	/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
	/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class SimplyCache<TKey, TValue> : ICache<TKey, TValue>, IDisposable
	{
		/// <summary>
		/// <typeparamref name="TValue"/> 表示的类型是否可以被释放。
		/// </summary>
		private static readonly bool IsDisposable = typeof(TValue).GetInterface("IDisposable") != null;
		/// <summary>
		/// 用于多线程同步的锁。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
		/// <summary>
		/// 缓存对象的字典。
		/// </summary>
		private Dictionary<TKey, TValue> cacheDict = new Dictionary<TKey, TValue>();
		/// <summary>
		/// 当前对象是否已释放资源。
		/// </summary>
		private bool isDisposed = false;
		/// <summary>
		/// 初始化 <see cref="SimplyCache&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		public SimplyCache()
		{ }
		/// <summary>
		/// 获取实际包含在缓存中的对象数目。
		/// </summary>
		public int Count { get { return cacheDict.Count; } }

		#region IDisposable 成员

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.ClearInternal();
				cacheLock.Dispose();
				GC.SuppressFinalize(this);
			}
		}

		#endregion // IDisposable 成员

		#region ICache<TKey, TValue> 成员

		/// <summary>
		/// 将指定的键和对象添加到缓存中，无论键是否存在。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public void Add(TKey key, TValue value)
		{
			this.CheckDisposed();
			ExceptionHelper.CheckArgumentNull(key, "key");
			this.AddInternal(key, value);
		}
		/// <summary>
		/// 清空缓存中的所有对象。
		/// </summary>
		public void Clear()
		{
			CheckDisposed();
			ClearInternal();
		}
		/// <summary>
		/// 确定缓存中是否包含指定的键。
		/// </summary>
		/// <param name="key">要在缓存中查找的键。</param>
		/// <returns>如果缓存中包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public bool Contains(TKey key)
		{
			this.CheckDisposed();
			ExceptionHelper.CheckArgumentNull(key, "key");
			cacheLock.EnterReadLock();
			try
			{
				return cacheDict.ContainsKey(key);
			}
			finally
			{
				cacheLock.ExitReadLock();
			}
		}
		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将对象添加到缓存中。
		/// </summary>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="valueFactory">用于为键生成对象的函数。</param>
		/// <returns>如果在缓存中找到该键，则为对应的对象；否则为 <paramref name="valueFactory"/> 返回的新对象。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			ExceptionHelper.CheckArgumentNull(key, "key");
			ExceptionHelper.CheckArgumentNull(valueFactory, "valueFactory");
			TValue value;
			if (this.TryGet(key, out value))
			{
				return value;
			}
			value = valueFactory(key);
			this.AddInternal(key, value);
			return value;
		}
		/// <summary>
		/// 从缓存中移除并返回具有指定键的对象。
		/// </summary>
		/// <param name="key">要移除并返回的对象的键。</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public void Remove(TKey key)
		{
			this.CheckDisposed();
			ExceptionHelper.CheckArgumentNull(key, "key");
			TValue value;
			cacheLock.EnterUpgradeableReadLock();
			try
			{
				if (cacheDict.TryGetValue(key, out value))
				{
					cacheLock.EnterWriteLock();
					try
					{
						cacheDict.Remove(key);
					}
					finally
					{
						cacheLock.ExitWriteLock();
					}
				}
			}
			finally
			{
				cacheLock.ExitUpgradeableReadLock();
			}
			if (IsDisposable)
			{
				IDisposable disposable = value as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		/// <summary>
		/// 尝试从缓存中获取与指定的键关联的对象。
		/// </summary>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="value">此方法返回时，<paramref name="value"/> 包含缓存中具有指定键的对象；
		/// 如果操作失败，则包含默认值。</param>
		/// <returns>如果在缓存中找到该键，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public bool TryGet(TKey key, out TValue value)
		{
			this.CheckDisposed();
			ExceptionHelper.CheckArgumentNull(key, "key");
			cacheLock.EnterReadLock();
			try
			{
				if (cacheDict.TryGetValue(key, out value))
				{
					return true;
				}
			}
			finally
			{
				cacheLock.ExitReadLock();
			}
			value = default(TValue);
			return false;
		}

		#endregion // ICache<TKey, TValue> 成员

		/// <summary>
		/// 清空缓存中的所有对象。
		/// </summary>
		private void ClearInternal()
		{
			Dictionary<TKey, TValue> oldDict = null;
			cacheLock.EnterWriteLock();
			try
			{
				if (IsDisposable)
				{
					oldDict = cacheDict;
				}
				cacheDict = new Dictionary<TKey, TValue>();
			}
			finally
			{
				cacheLock.ExitWriteLock();
			}
			if (IsDisposable)
			{
				// 释放对象资源。
				foreach (TValue value in oldDict.Values)
				{
					IDisposable disposable = value as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
		/// <summary>
		/// 将指定的键和对象添加到缓存中，并返回添加的节点。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		private void AddInternal(TKey key, TValue value)
		{
			cacheLock.EnterWriteLock();
			try
			{
				// 更新节点。
				cacheDict[key] = value;
			}
			finally
			{
				cacheLock.ExitWriteLock();
			}
		}
		/// <summary>
		/// 检查当前对象是否已释放资源。
		/// </summary>
		private void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw ExceptionHelper.ObjectDisposed();
			}
		}
	}
}
