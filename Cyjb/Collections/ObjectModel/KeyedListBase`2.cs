using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 提供列表键嵌入在值中的列表的抽象基类。
	/// </summary>
	/// <typeparam name="TKey">列表中键的类型。</typeparam>
	/// <typeparam name="TItem">列表中的项的类型。</typeparam>
	/// <remarks>列表中存储的元素都不能为 <c>null</c>，相应的键也不能为 <c>null</c>。</remarks>
	[Serializable]
	[ContractClass(typeof(ContractsForKeyedListBase<,>))]
	public abstract class KeyedListBase<TKey, TItem> : ListBase<TItem>
	{
		/// <summary>
		/// 默认的创建字典的阀值。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int DefaultThreshold = 3;
		/// <summary>
		/// 用于确定列表中的键是否相等的泛型相等比较器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IEqualityComparer<TKey> comparer;
		/// <summary>
		/// 用于保存键的字典。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Dictionary<TKey, TItem> dict;
		/// <summary>
		/// 创建字典的阀值。
		/// </summary>
		private readonly int threshold;

		#region 构造函数

		/// <summary>
		/// 初始化使用默认相等比较器的 <see cref="KeyedListBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="KeyedListBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		protected KeyedListBase()
			: this(null, DefaultThreshold, new List<TItem>())
		{ }
		/// <summary>
		/// 使用指定的相等比较器初始化 <see cref="KeyedListBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 <see cref="EqualityComparer{T}.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		protected KeyedListBase(IEqualityComparer<TKey> comparer)
			: this(comparer, DefaultThreshold, new List<TItem>())
		{ }
		/// <summary>
		/// 使用指定的相等比较器和被包装的列表初始化 
		/// <see cref="KeyedListBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 <see cref="EqualityComparer{T}.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		/// <param name="list">由新的列表包装的集合。</param>
		/// <remarks>如果 <paramref name="list"/> 实现了 <see cref="IList{T}"/>
		/// 接口，则使用 <paramref name="list"/> 作为内部集合；否则使用 
		/// <see cref="List{T}"/> 作为内部集合。</remarks>
		protected KeyedListBase(IEqualityComparer<TKey> comparer, IEnumerable<TItem> list)
			: this(comparer, DefaultThreshold, list)
		{ }
		/// <summary>
		/// 使用指定的相等比较器和创建查找字典的阈值初始化 
		/// <see cref="KeyedListBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 <see cref="EqualityComparer{T}.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		/// <param name="dictionaryCreationThreshold">在不创建查找字典的情况下列表可容纳的元素的数目
		/// （<c>0</c> 表示添加第一项时创建查找字典）；或者为 <c>-1</c>，表示指定永远不会创建查找字典。</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="dictionaryCreationThreshold"/> 小于 <c>-1</c>。</exception>
		protected KeyedListBase(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
			: this(comparer, dictionaryCreationThreshold, new List<TItem>())
		{ }
		/// <summary>
		/// 使用指定的相等比较器、创建查找字典的阈值和被包装的列表，
		/// 初始化 <see cref="KeyedListBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 <see cref="EqualityComparer{T}.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		/// <param name="dictionaryCreationThreshold">在不创建查找字典的情况下列表可容纳的元素的数目
		/// （<c>0</c> 表示添加第一项时创建查找字典）；或者为 <c>-1</c>，表示指定永远不会创建查找字典。</param>
		/// <param name="list">由新的列表包装的集合。</param>
		/// <remarks>如果 <paramref name="list"/> 实现了 <see cref="IList{T}"/>
		/// 接口，则使用 <paramref name="list"/> 作为内部集合；否则使用 
		/// <see cref="List{T}"/> 作为内部集合。</remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="dictionaryCreationThreshold"/> 小于 <c>-1</c>。</exception>
		protected KeyedListBase(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold,
			IEnumerable<TItem> list)
			: base(list)
		{
			if (dictionaryCreationThreshold < -1)
			{
				throw CommonExceptions.InvalidDictionaryThreshold("dictionaryCreationThreshold", dictionaryCreationThreshold);
			}
			this.comparer = comparer ?? EqualityComparer<TKey>.Default;
			this.threshold = dictionaryCreationThreshold == -1 ? int.MaxValue : dictionaryCreationThreshold;
			int cnt = this.Items.Count;
			for (int i = 0; i < cnt; i++)
			{
				TItem item = this.Items[i];
				if (item == null)
				{
					throw CommonExceptions.CollectionItemNull("list");
				}
				TKey key = this.GetKeyForItem(this.Items[i]);
				this.AddKey(key, this.Items[i]);
			}
		}

		#endregion // 构造函数

		/// <summary>
		/// 获取用于确定列表中的键是否相等的泛型相等比较器。
		/// </summary>
		/// <value>用于确定列表中的键是否相等的泛型相等比较器。</value>
		public IEqualityComparer<TKey> Comparer
		{
			get { return this.comparer; }
		}
		/// <summary>
		/// 获取 <see cref="KeyedListBase{TKey,TItem}"/> 的查找字典。
		/// </summary>
		/// <value><see cref="KeyedListBase{TKey,TItem}"/> 的查找字典。</value>
		protected IDictionary<TKey, TItem> Dictionary
		{
			get { return this.dict; }
		}

		#region 键操作

		/// <summary>
		/// 获取具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取的元素的键。</param>
		/// <returns>带有指定键的元素。如果未找到具有指定键的元素，则引发异常。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		/// <exception cref="KeyNotFoundException">在列表中不存在 <paramref name="key"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 获取指定的元素。
		/// </summary>
		/// </overloads>
		[Pure]
		public TItem this[TKey key]
		{
			get
			{
				if (key == null)
				{
					throw CommonExceptions.ArgumentNull("key");
				}
				Contract.Ensures(Contract.Result<TItem>() != null);
				TItem item;
				if (this.TryGetValue(key, out item))
				{
					Contract.Assume(item != null);
					return item;
				}
				throw CommonExceptions.KeyNotFound(key.ToString());
			}
		}
		/// <summary>
		/// 确定某元素的键是否在 <see cref="KeyedListBase{TKey,TItem}"/> 中。
		/// </summary>
		/// <param name="key">要定位的元素的键。</param>
		/// <returns>如果在 <see cref="KeyedListBase{TKey,TItem}"/> 
		/// 中找到具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		[Pure]
		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				return false;
			}
			TItem item;
			return this.TryGetValue(key, out item);
		}
		/// <summary>
		/// 确定 <see cref="KeyedListBase{TKey,TItem}"/> 中具有特定键的元素的索引。
		/// </summary>
		/// <param name="key">要在 <see cref="KeyedListBase{TKey,TItem}"/> 中定位的元素的键。</param>
		/// <returns>如果在 <see cref="KeyedListBase{TKey,TItem}"/> 中找到 <paramref name="key"/>，
		/// 则为该项的索引；否则为 <c>-1</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 确定 <see cref="KeyedListBase{TKey,TItem}"/> 中特定元素的索引。
		/// </summary>
		/// </overloads>
		[Pure]
		public int IndexOf(TKey key)
		{
			if (key == null)
			{
				return -1;
			}
			if (this.dict != null)
			{
				TItem item;
				if (this.dict.TryGetValue(key, out item))
				{
					return this.IndexOf(item);
				}
				return -1;
			}
			int cnt = this.Count;
			for (int i = 0; i < cnt; i++)
			{
				TItem item = this.GetItemAt(i);
				Contract.Assume(item != null);
				TKey itemKey = this.GetKeyForItem(item);
				if (this.comparer.Equals(itemKey, key))
				{
					return i;
				}
			}
			return -1;
		}
		/// <summary>
		/// 从 <see cref="KeyedListBase{TKey,TItem}"/> 中移除具有指定键的元素。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <returns>如果已从 <see cref="KeyedListBase{TKey,TItem}"/> 中成功移除元素，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="KeyedListBase{TKey,TItem}"/> 
		/// 中没有找到指定的键，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 从 <see cref="KeyedListBase{TKey,TItem}"/> 中移除特定元素。
		/// </summary>
		/// </overloads>
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw CommonExceptions.ArgumentNull("key");
			}
			Contract.EndContractBlock();
			int index = this.IndexOf(key);
			if (index < 0)
			{
				return false;
			}
			this.RemoveItem(index);
			return true;
		}
		/// <summary>
		/// 获取具有指定的键的元素。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="item">当此方法返回值时，如果找到该键，便会返回与指定的键相关联的值；
		/// 否则，则会返回 <paramref name="item"/> 参数的类型默认值。</param>
		/// <returns>如果包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> 为 <c>null</c>。</exception>
		[Pure]
		public bool TryGetValue(TKey key, out TItem item)
		{
			if (key == null)
			{
				throw CommonExceptions.ArgumentNull("key");
			}
			Contract.EndContractBlock();
			if (this.dict != null)
			{
				return this.dict.TryGetValue(key, out item);
			}
			int cnt = this.Count;
			for (int i = 0; i < cnt; i++)
			{
				item = this.GetItemAt(i);
				Contract.Assume(item != null);
				TKey itemKey = this.GetKeyForItem(item);
				if (this.comparer.Equals(itemKey, key))
				{
					return true;
				}
			}
			item = default(TItem);
			return false;
		}
		/// <summary>
		/// 在派生类中实现时，将从指定元素提取键。
		/// </summary>
		/// <param name="item">从中提取键的元素。</param>
		/// <returns>指定元素的键。</returns>
		[Pure]
		protected abstract TKey GetKeyForItem(TItem item);
		/// <summary>
		/// 更改与查找字典中指定元素相关联的键。
		/// </summary>
		/// <param name="item">要更改其键的元素。</param>
		/// <param name="newKey"><paramref name="item"/> 的新键。</param>
		/// <exception cref="ArgumentException"><paramref name="newKey"/> 在字典中已存在。</exception>
		protected void ChangeItemKey(TItem item, TKey newKey)
		{
			Contract.Requires(item != null && this.Contains(item));
			if (this.dict == null)
			{
				return;
			}
			TKey key = this.GetKeyForItem(item);
			if (this.comparer.Equals(key, newKey))
			{
				return;
			}
			this.dict.Remove(key);
			this.dict.Add(newKey, item);
		}

		#endregion // 键操作

		#region ListBase<TItem> 成员

		/// <summary>
		/// 将元素插入 <see cref="KeyedListBase{TKey,TItem}"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入的对象。对于引用类型，该值可以为 <c>null</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> 为 <c>null</c>。</exception>
		protected override void InsertItem(int index, TItem item)
		{
			if (item == null)
			{
				throw CommonExceptions.ArgumentNull("item");
			}
			Contract.EndContractBlock();
			TKey key = this.GetKeyForItem(item);
			this.AddKey(key, item);
			base.InsertItem(index, item);
		}
		/// <summary>
		/// 移除 <see cref="KeyedListBase{TKey,TItem}"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			TItem item = this.GetItemAt(index);
			Contract.Assume(item != null);
			TKey key = this.GetKeyForItem(item);
			this.RemoveKey(key);
			base.RemoveItem(index);
		}
		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> 为 <c>null</c>。</exception>
		protected override void SetItemAt(int index, TItem item)
		{
			if (item == null)
			{
				throw CommonExceptions.ArgumentNull("item");
			}
			Contract.EndContractBlock();
			TItem oldItem = this.GetItemAt(index);
			Contract.Assume(oldItem != null);
			TKey oldKey = this.GetKeyForItem(oldItem);
			TKey key = this.GetKeyForItem(item);
			if (this.dict != null)
			{
				if (this.comparer.Equals(oldKey, key))
				{
					this.dict[key] = item;
				}
				else
				{
					this.dict.Remove(oldKey);
					this.dict.Add(key, item);
				}
			}
			base.SetItemAt(index, item);
		}

		#endregion // ListBase<TItem> 成员

		#region ICollection<TItem> 成员

		/// <summary>
		/// 从 <see cref="KeyedListBase{TKey,TItem}"/> 中移除所有元素。
		/// </summary>
		public override void Clear()
		{
			base.Clear();
			if (this.dict != null)
			{
				this.dict.Clear();
			}
		}
		/// <summary>
		/// 确定 <see cref="KeyedListBase{TKey,TItem}"/> 是否包含指定对象。
		/// </summary>
		/// <param name="item">要在 <see cref="KeyedListBase{TKey,TItem}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="KeyedListBase{TKey,TItem}"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> 为 <c>null</c>。</exception>
		public override bool Contains(TItem item)
		{
			if (item == null)
			{
				return false;
			}
			if (this.dict == null)
			{
				return this.IndexOf(item) >= 0;
			}
			TKey key = this.GetKeyForItem(item);
			TItem newItem;
			return this.dict.TryGetValue(key, out newItem) &&
				EqualityComparer<TItem>.Default.Equals(newItem, item);
		}

		#endregion // ICollection<TItem> 成员

		#region 字典操作

		/// <summary>
		/// 将指定的键和值添加到字典中。
		/// </summary>
		/// <param name="key">要添加的键。</param>
		/// <param name="item">要添加的值。</param>
		/// <exception cref="ArgumentException"><paramref name="key"/> 在列表中已存在。</exception>
		protected void AddKey(TKey key, TItem item)
		{
			if (this.dict != null)
			{
				this.dict.Add(key, item);
			}
			else if (this.Count >= this.threshold)
			{
				this.CreateDictionary();
				this.dict.Add(key, item);
			}
			else if (this.ContainsKey(key))
			{
				throw CommonExceptions.KeyDuplicate("key");
			}
		}
		/// <summary>
		/// 从字典中移除指定的键。
		/// </summary>
		/// <param name="key">要移除的键。</param>
		protected void RemoveKey(TKey key)
		{
			if (this.dict != null)
			{
				this.dict.Remove(key);
			}
		}
		/// <summary>
		/// 创建字典。
		/// </summary>
		private void CreateDictionary()
		{
			this.dict = new Dictionary<TKey, TItem>(this.comparer);
			int cnt = this.Count;
			for (int i = 0; i < cnt; i++)
			{
				TItem item = this.GetItemAt(i);
				Contract.Assume(item != null);
				TKey key = this.GetKeyForItem(item);
				this.dict.Add(key, item);
			}
		}

		#endregion // 字典操作

	}
	/// <summary>
	/// 表示 <see cref="KeyedListBase{TKey,TItem}"/> 接口的协定。
	/// </summary>
	[ContractClassFor(typeof(KeyedListBase<,>))]
	internal abstract class ContractsForKeyedListBase<TKey, TItem> : KeyedListBase<TKey, TItem>
	{
		/// <summary>
		/// 初始化 <see cref="ContractsForKeyedListBase{TKey,TItem}"/> 类的新实例。
		/// </summary>
		private ContractsForKeyedListBase() { }
		/// <summary>
		/// 在派生类中实现时，将从指定元素提取键。
		/// </summary>
		/// <param name="item">从中提取键的元素。</param>
		/// <returns>指定元素的键。</returns>
		[Pure]
		protected override TKey GetKeyForItem(TItem item)
		{
			Contract.Requires(item != null);
			Contract.Ensures(Contract.Result<TKey>() != null);
			return default(TKey);
		}
	}
}
