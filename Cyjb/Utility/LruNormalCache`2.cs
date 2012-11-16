using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cyjb.Utility
{
	/// <summary>
	/// 表示使用普通的最近最少使用算法的对象缓冲池。并未包含在项目文件中。
	/// </summary>
	/// <typeparam name="TKey">对象缓存的键的类型。</typeparam>
	/// <typeparam name="TValue">被缓存的对象的类型。</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class LruNormalCache<TKey, TValue> : ICache<TKey, TValue>
	{
		/// <summary>
		/// <typeparamref name="TValue"/> 表示的类型是否可以被释放。
		/// </summary>
		private static readonly bool IsDisposable = typeof(TValue).GetInterface("IDisposable") != null;
		/// <summary>
		/// 用于多线程同步的锁。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object cacheLock = new object();
		/// <summary>
		/// 缓冲池中可以保存的最大对象数目。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int maxSize;
		/// <summary>
		/// 缓冲池中当前存储的对象数目。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;
		/// <summary>
		/// 缓存对象的字典。
		/// </summary>
		private Dictionary<TKey, LruNormalNode<TKey, TValue>> cacheDict = new Dictionary<TKey, LruNormalNode<TKey, TValue>>();
		/// <summary>
		/// 链表的头节点。
		/// </summary>
		private LruNormalNode<TKey, TValue> head;
		/// <summary>
		/// 使用指定的最大对象数目初始化 <see cref="LruNormalCache&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="maxSize">缓存中可以保存的最大对象数目。</param>
		public LruNormalCache(int maxSize)
		{
			if (maxSize <= 0)
			{
				throw ExceptionHelper.ArgumentMustBePositive("maxSize");
			}
			this.maxSize = maxSize;
		}
		/// <summary>
		/// 获取实际包含在缓存中得对象数目。
		/// </summary>
		public int Count
		{
			get { return count; }
		}
		/// <summary>
		/// 获取缓存中可以保存的最大对象数目。
		/// </summary>
		public int MaxSize { get { return maxSize; } }

		#region ICache<TKey, TValue> 成员

		/// <summary>
		/// 将指定的键和对象添加到缓存中，无论键是否存在。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public void Add(TKey key, TValue value)
		{
			ExceptionHelper.CheckArgumentNull(key, "key");
			this.AddInternal(key, value);
		}
		/// <summary>
		/// 清空缓存中的所有对象。
		/// </summary>
		public void Clear()
		{
			LruNormalNode<TKey, TValue> oldHead;
			lock (cacheLock)
			{
				cacheDict.Clear();
				oldHead = head;
				head = null;
				count = 0;
			}
			if (IsDisposable)
			{
				// 释放对象资源。
				LruNormalNode<TKey, TValue> node = oldHead;
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
		/// 确定缓存中是否包含指定的键。
		/// </summary>
		/// <param name="key">要在缓存中查找的键。</param>
		/// <returns>如果缓存中包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public bool Contains(TKey key)
		{
			ExceptionHelper.CheckArgumentNull(key, "key");
			lock (cacheLock)
			{
				return cacheDict.ContainsKey(key);
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
		public void Remove(TKey key)
		{
			LruNormalNode<TKey, TValue> node;
			lock (cacheLock)
			{
				if (cacheDict.TryGetValue(key, out node))
				{
					if (cacheDict.Remove(key))
					{
						Remove(node);
						count--;
					}
				}
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
		public bool TryGet(TKey key, out TValue value)
		{
			lock (cacheLock)
			{
				LruNormalNode<TKey, TValue> node;
				if (cacheDict.TryGetValue(key, out node))
				{
					MoveToFirst(node);
					value = node.Value;
					return true;
				}
				else
				{
					value = default(TValue);
					return false;
				}
			}
		}

		#endregion // ICache<TKey, TValue> 成员

		#region 链表操作

		/// <summary>
		/// 从链表中移除指定的节点。
		/// </summary>
		/// <param name="node">要移除的节点。</param>
		private void Remove(LruNormalNode<TKey, TValue> node)
		{
			if (node.Next == node)
			{
				this.head = null;
			}
			else
			{
				node.Next.Previous = node.Previous;
				node.Previous.Next = node.Next;
				if (this.head == node)
				{
					this.head = node.Next;
				}
			}
		}
		/// <summary>
		/// 将指定的节点添加到链表的头部。
		/// </summary>
		/// <param name="node">要添加的节点。</param>
		private void AddFirst(LruNormalNode<TKey, TValue> node)
		{
			if (this.head == null)
			{
				node.Next = node.Previous = node;
			}
			else
			{
				node.Next = this.head;
				node.Previous = this.head.Previous;
				this.head.Previous.Next = node;
				this.head.Previous = node;
			}
			this.head = node;
		}
		/// <summary>
		/// 将指定的链表节点移动到链表的头部。
		/// </summary>
		/// <param name="node">要移动的链表节点。</param>
		private void MoveToFirst(LruNormalNode<TKey, TValue> node)
		{
			if (node != this.head)
			{
				Remove(node);
				AddFirst(node);
			}
		}

		#endregion // 链表操作

		/// <summary>
		/// 将指定的键和对象添加到缓存中，并返回添加的节点。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		private void AddInternal(TKey key, TValue value)
		{
			LruNormalNode<TKey, TValue> node;
			IDisposable disposable = null;
			lock (cacheLock)
			{
				if (cacheDict.TryGetValue(key, out node))
				{
					// 移动旧节点。
					MoveToFirst(node);
					node.Value = value;
				}
				else
				{
					if (count >= maxSize)
					{
						// 淘汰旧节点。
						node = this.head.Previous;
						cacheDict.Remove(node.Key);
						disposable = node.Value as IDisposable;
						Remove(node);
						node.Key = key;
						node.Value = value;
					}
					else
					{
						node = new LruNormalNode<TKey, TValue>(key, value);
						count++;
					}
					AddFirst(node);
					cacheDict.Add(key, node);
				}
			}
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}
}
