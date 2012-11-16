using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb.Utility
{
	/// <summary>
	/// 表示使用改进的最近最少使用算法的对象缓冲池。
	/// </summary>
	/// <typeparam name="TKey">缓冲对象的键的类型。</typeparam>
	/// <typeparam name="TValue">缓冲对象的类型。</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class LruCache<TKey, TValue> : ICache<TKey, TValue>, IDisposable
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
		/// 缓冲池中可以保存的最大对象数目。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int maxSize;
		/// <summary>
		/// 缓冲池中热端的最大对象数目。
		/// </summary>
		private int hotSize;
		/// <summary>
		/// 缓冲池中当前存储的对象数目。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;
		/// <summary>
		/// 缓存对象的字典。
		/// </summary>
		private Dictionary<TKey, LruNode<TKey, TValue>> cacheDict = new Dictionary<TKey, LruNode<TKey, TValue>>();
		/// <summary>
		/// 链表的头节点，也是热端的头。
		/// </summary>
		private LruNode<TKey, TValue> head;
		/// <summary>
		/// 链表冷端的头节点。
		/// </summary>
		private LruNode<TKey, TValue> codeHead;
		/// <summary>
		/// 当前对象是否已释放资源。
		/// </summary>
		private bool isDisposed = false;
		/// <summary>
		/// 使用指定的最大对象数目初始化 <see cref="LruCache&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="maxSize">缓存中可以保存的最大对象数目，必须大于等于 2。</param>
		public LruCache(int maxSize)
			: this(maxSize, 0.5)
		{ }
		/// <summary>
		/// 使用指定的最大对象数目和热对象所占的百分比初始化 
		/// <see cref="LruCache&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="maxSize">缓存中可以保存的最大对象数目，必须大于等于 2。</param>
		/// <param name="hotPrecent">热对象所占的百分比，必须保证热对象和冷对象的实际值能够大于等于 1。</param>
		public LruCache(int maxSize, double hotPrecent)
		{
			if (maxSize < 2)
			{
				throw ExceptionHelper.ArgumentOutOfRange("maxSize");
			}
			this.maxSize = maxSize;
			this.hotSize = (int)(maxSize * hotPrecent);
			if (this.hotSize < 1 || this.maxSize - this.hotSize < 1)
			{
				throw ExceptionHelper.ArgumentOutOfRange("hotPrecent");
			}
		}
		/// <summary>
		/// 获取实际包含在缓存中的对象数目。
		/// </summary>
		public int Count { get { return count; } }
		/// <summary>
		/// 获取缓存中可以保存的最大对象数目。
		/// </summary>
		public int MaxSize { get { return maxSize; } }

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
			LruNode<TKey, TValue> node;
			cacheLock.EnterUpgradeableReadLock();
			try
			{
				if (cacheDict.TryGetValue(key, out node))
				{
					cacheLock.EnterWriteLock();
					try
					{
						cacheDict.Remove(key);
						Remove(node);
						// 写锁互斥，这里不用 Interlocked。
						count--;
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
				IDisposable disposable = node.Value as IDisposable;
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
				LruNode<TKey, TValue> node;
				if (cacheDict.TryGetValue(key, out node))
				{
					value = node.Value;
					Interlocked.Increment(ref node.VisitCount);
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

		#region 链表操作

		/// <summary>
		/// 从链表中移除指定的节点。
		/// </summary>
		/// <param name="node">要移除的节点。</param>
		private void Remove(LruNode<TKey, TValue> node)
		{
			if (node.Next == node)
			{
				this.head = null;
			}
			else
			{
				node.Next.Prev = node.Prev;
				node.Prev.Next = node.Next;
				if (this.head == node)
				{
					this.head = node.Next;
				}
				else if (this.codeHead == node)
				{
					this.codeHead = node.Next;
				}
			}
		}
		/// <summary>
		/// 将指定的节点添加到链表热端的头部。
		/// </summary>
		/// <param name="node">要添加的节点。</param>
		private void AddHotFirst(LruNode<TKey, TValue> node)
		{
			if (this.head == null)
			{
				node.Next = node.Prev = node;
			}
			else
			{
				this.head.AddBefore(node);
				// 热端长度增加，将冷端头节点像前移动一个位置。
				if (this.codeHead != null)
				{
					this.codeHead = this.codeHead.Prev;
				}
			}
			this.head = node;
		}
		/// <summary>
		/// 将指定的节点添加到链表冷端的头部。
		/// </summary>
		/// <param name="node">要添加的节点。</param>
		private void AddCodeFirst(LruNode<TKey, TValue> node)
		{
			// 这里 codeHead != null，在调用的时候已经保证了这一点。
			this.codeHead.AddBefore(node);
			this.codeHead = node;
		}

		#endregion // 链表操作

		/// <summary>
		/// 清空缓存中的所有对象。
		/// </summary>
		private void ClearInternal()
		{
			LruNode<TKey, TValue> oldHead;
			cacheLock.EnterWriteLock();
			try
			{
				cacheDict.Clear();
				oldHead = head;
				head = codeHead = null;
				count = 0;
			}
			finally
			{
				cacheLock.ExitWriteLock();
			}
			if (IsDisposable)
			{
				// 释放对象资源。
				LruNode<TKey, TValue> node = oldHead;
				do
				{
					IDisposable disposable = node.Value as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					node = node.Next;
				} while (node != oldHead);
			}
		}
		/// <summary>
		/// 将指定的键和对象添加到缓存中，并返回添加的节点。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		private void AddInternal(TKey key, TValue value)
		{
			LruNode<TKey, TValue> node;
			IDisposable disposable = null;
			cacheLock.EnterWriteLock();
			try
			{
				if (cacheDict.TryGetValue(key, out node))
				{
					// 更新节点。
					node.Value = value;
					// 写锁互斥，这里不用 Interlocked。
					node.VisitCount++;
					return;
				}
				else
				{
					if (count < maxSize)
					{
						// 将节点添加到热端起始。
						node = new LruNode<TKey, TValue>(key, value);
						AddHotFirst(node);
						// 写锁互斥，这里不用 Interlocked。
						count++;
						if (count == hotSize + 1)
						{
							codeHead = head.Prev;
						}
						cacheDict.Add(key, node);
						return;
					}
					else
					{
						// 从冷端末尾尝试淘汰旧节点，将访问次数大于 1 的移动到热端的头。
						// 由于双向链表是环形存储的，就相当于将 head 前移。
						while (head.Prev.VisitCount >= 2)
						{
							// 清零访问计数。
							head.Prev.VisitCount = 0;
							head = head.Prev;
							codeHead = codeHead.Prev;
						}
						// 将 node 移除，并添加到冷端的头。
						node = head.Prev;
						disposable = node.Value as IDisposable;
						this.cacheDict.Remove(node.Key);
						this.Remove(node);
						// 这里直接重用旧节点。
						node.Key = key;
						node.Value = value;
						node.VisitCount = 1;
						this.AddCodeFirst(node);
						cacheDict.Add(key, node);
					}
				}
			}
			finally
			{
				cacheLock.ExitWriteLock();
			}
			if (disposable != null)
			{
				disposable.Dispose();
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
