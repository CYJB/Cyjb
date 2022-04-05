using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cyjb.Cache
{
	/// <summary>
	/// 表示使用改进的 LRU（最近最少使用）算法的对象缓冲池。
	/// </summary>
	/// <typeparam name="TKey">缓存对象的键的类型。</typeparam>
	/// <typeparam name="TValue">缓存对象的类型。</typeparam>
	/// <remarks>
	/// <para>该类不包含多线程同步，请仅在单线程应用中使用。</para>
	/// <para>关于改进的 LRU（最近最少使用）算法，可以参考我的博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/LruCache.html">
	/// 《一个改进 LRU 算法的缓冲池》</see>。</para></remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/LruCache.html">
	/// 《一个改进 LRU 算法的缓冲池》</seealso>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class LruCache<TKey, TValue> : ICache<TKey, TValue>
		 where TKey : notnull
	{
		/// <summary>
		/// 缓冲池中可以保存的最大对象数目。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int maxSize;
		/// <summary>
		/// 缓冲池中热端的最大对象数目。
		/// </summary>
		private readonly int hotSize;
		/// <summary>
		/// 缓冲池中当前存储的对象数目。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;
		/// <summary>
		/// 缓存对象的字典。
		/// </summary>
		private readonly Dictionary<TKey, LruNode<TKey, TValue>> cacheDict = new();
		/// <summary>
		/// 链表的头节点，也是热端的头。
		/// </summary>
		private LruNode<TKey, TValue>? head;
		/// <summary>
		/// 链表冷端的头节点。
		/// </summary>
		private LruNode<TKey, TValue>? codeHead;

		/// <summary>
		/// 使用指定的最大对象数目初始化 <see cref="LruCache{TKey,TValue}"/> 类的新实例。
		/// </summary>
		/// <param name="maxSize">缓存中可以保存的最大对象数目，必须大于等于 2。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="LruCache{TKey,TValue}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public LruCache(int maxSize)
			: this(maxSize, 0.5)
		{ }
		/// <summary>
		/// 使用指定的最大对象数目和热对象所占的百分比初始化 
		/// <see cref="LruCache{TKey,TValue}"/> 类的新实例。
		/// </summary>
		/// <param name="maxSize">缓存中可以保存的最大对象数目，必须大于等于 2。</param>
		/// <param name="hotPrecent">热对象所占的百分比。</param>
		public LruCache(int maxSize, double hotPrecent)
		{
			if (maxSize < 2)
			{
				throw CommonExceptions.ArgumentOutOfRange(maxSize);
			}
			this.maxSize = maxSize;
			hotSize = (int)(maxSize * hotPrecent);
			if (hotSize < 1)
			{
				hotSize = 1;
			}
			else if (maxSize - hotSize < 1)
			{
				hotSize = this.maxSize - 1;
			}
		}

		/// <summary>
		/// 获取实际包含在缓存中的对象数目。
		/// </summary>
		/// <value>实际包含在缓存中的对象数目。</value>
		public int Count { get { return count; } }

		/// <summary>
		/// 获取缓存中可以保存的最大对象数目。
		/// </summary>
		/// <value>缓存中可以保存的最大对象数目。</value>
		public int MaxSize { get { return maxSize; } }

		#region ICache<TKey, TValue> 成员

		/// <summary>
		/// 将指定的键和对象添加到缓存中，无论键是否存在。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		public void Add(TKey key, TValue value)
		{
			if (cacheDict.TryGetValue(key, out LruNode<TKey, TValue>? node))
			{
				// 更新节点。
				node.Value = value;
				node.VisitCount++;
			}
			else
			{
				AddInternal(key, value);
			}
		}

		/// <summary>
		/// 清空缓存中的所有对象。
		/// </summary>
		public void Clear()
		{
			cacheDict.Clear();
			head = codeHead = null;
			count = 0;
		}

		/// <summary>
		/// 确定缓存中是否包含指定的键。
		/// </summary>
		/// <param name="key">要在缓存中查找的键。</param>
		/// <returns>如果缓存中包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(TKey key)
		{
			return cacheDict.ContainsKey(key);
		}

		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将新对象添加到缓存中。
		/// </summary>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="valueFactory">用于根据键生成新对象的函数。</param>
		/// <returns>如果在缓存中找到该键，则为对应的对象；否则为 <paramref name="valueFactory"/> 返回的新对象。</returns>
		/// <overloads>
		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将新对象添加到缓存中。
		/// </summary>
		/// </overloads>
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			if (TryGet(key, out TValue? value))
			{
				return value;
			}
			value = valueFactory(key);
			AddInternal(key, value);
			return value;
		}

		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将新对象添加到缓存中。
		/// </summary>
		/// <typeparam name="TArg">参数的类型。</typeparam>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="arg">用于生成新对象的参数。</param>
		/// <param name="valueFactory">用于根据键和参数生成新对象的函数。</param>
		/// <returns>如果在缓存中找到该键，则为对应的对象；否则为 <paramref name="valueFactory"/> 
		/// 返回的新对象。</returns>
		public TValue GetOrAdd<TArg>(TKey key, TArg arg, Func<TKey, TArg, TValue> valueFactory)
		{
			if (TryGet(key, out TValue? value))
			{
				return value;
			}
			value = valueFactory(key, arg);
			AddInternal(key, value);
			return value;
		}

		/// <summary>
		/// 从缓存中移除具有指定键的对象。
		/// </summary>
		/// <param name="key">要移除的对象的键。</param>
		public void Remove(TKey key)
		{
			if (cacheDict.TryGetValue(key, out LruNode<TKey, TValue>? node))
			{
				cacheDict.Remove(key);
				Remove(node);
				count--;
			}
		}

		/// <summary>
		/// 尝试从缓存中获取与指定的键关联的对象。
		/// </summary>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="value">此方法返回时，<paramref name="value"/> 包含缓存中具有指定键的对象；
		/// 如果操作失败，则包含默认值。</param>
		/// <returns>如果在缓存中找到该键，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value)
		{
			if (cacheDict.TryGetValue(key, out LruNode<TKey, TValue>? node))
			{
				value = node.Value;
				node.VisitCount++;
				return true;
			}
			value = default;
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
				head = null;
			}
			else
			{
				node.Next.Prev = node.Prev;
				node.Prev.Next = node.Next;
				if (head == node)
				{
					head = node.Next;
				}
				else if (codeHead == node)
				{
					codeHead = node.Next;
				}
			}
		}

		/// <summary>
		/// 将指定的节点添加到链表热端的头部。
		/// </summary>
		/// <param name="node">要添加的节点。</param>
		private void AddHotFirst(LruNode<TKey, TValue> node)
		{
			if (head == null)
			{
				node.Next = node.Prev = node;
			}
			else
			{
				head.AddBefore(node);
				// 热端长度增加，将冷端头节点像前移动一个位置。
				if (codeHead != null)
				{
					codeHead = codeHead.Prev;
				}
			}
			head = node;
		}

		/// <summary>
		/// 将指定的节点添加到链表冷端的头部。
		/// </summary>
		/// <param name="node">要添加的节点。</param>
		private void AddCodeFirst(LruNode<TKey, TValue> node)
		{
			// 这里 codeHead != null，在调用的时候已经保证了这一点。
			codeHead!.AddBefore(node);
			codeHead = node;
		}

		#endregion // 链表操作

		/// <summary>
		/// 将指定的键和对象添加到缓存中，并返回添加的节点。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		private void AddInternal(TKey key, TValue value)
		{
			LruNode<TKey, TValue> node;
			if (count < maxSize)
			{
				// 将节点添加到热端的头。
				node = new LruNode<TKey, TValue>(key, value);
				AddHotFirst(node);
				count++;
				if (count == hotSize + 1)
				{
					codeHead = head!.Prev;
				}
			}
			else
			{
				// 从冷端末尾尝试淘汰旧节点，将访问次数大于 1 的移动到热端的头。
				// 由于双向链表是环形存储的，就相当于将 head 前移。
				while (head!.Prev.VisitCount >= 2)
				{
					// 清零访问计数。
					head.Prev.VisitCount = 0;
					head = head.Prev;
					codeHead = codeHead!.Prev;
				}
				// 将 node 移除，并添加到冷端的头。
				node = head.Prev;
				cacheDict.Remove(node.Key);
				Remove(node);
				// 这里直接重用旧节点。
				node.Key = key;
				node.Value = value;
				node.VisitCount = 1;
				AddCodeFirst(node);
			}
			cacheDict.Add(key, node);
		}
	}
}
