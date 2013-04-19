using System.Collections.Generic;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 提供列表键嵌入在值中的列表的抽象基类。
	/// </summary>
	/// <typeparam name="TKey">列表中键的类型。</typeparam>
	/// <typeparam name="TItem">列表中的项的类型。</typeparam>
	public abstract class KeyedListBase<TKey, TItem> : ListBase<TItem>
	{
		/// <summary>
		/// 默认的创建字典的阀值。
		/// </summary>
		private const int defaultThreshold = 0;
		/// <summary>
		/// 用于确定列表中的键是否相等的泛型相等比较器。
		/// </summary>
		private readonly IEqualityComparer<TKey> comparer;
		/// <summary>
		/// 用于保存键的字典。
		/// </summary>
		private Dictionary<TKey, TItem> dict;
		/// <summary>
		/// 键的数目。
		/// </summary>
		private int keyCount;
		/// <summary>
		/// 创建字典的阀值。
		/// </summary>
		private readonly int threshold;
		/// <summary>
		/// 初始化使用默认相等比较器的 <see cref="KeyedListBase&lt;TKey, TItem&gt;"/> 类的新实例。
		/// </summary>
		protected KeyedListBase()
			: this(null, 0)
		{ }
		/// <summary>
		/// 初始化使用指定相等比较器的 <see cref="KeyedListBase&lt;TKey, TItem&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 
		/// <see cref="System.Collections.Generic.IEqualityComparer&lt;T&gt;"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 
		/// <see cref="System.Collections.Generic.EqualityComparer&lt;T&gt;.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		protected KeyedListBase(IEqualityComparer<TKey> comparer)
			: this(comparer, 0)
		{ }
		/// <summary>
		/// 初始化 <see cref="KeyedListBase&lt;TKey, TItem&gt;"/> 类的新实例，
		/// 该新实例使用指定的相等比较器并在超过指定阈值时创建一个查找字典。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 
		/// <see cref="System.Collections.Generic.IEqualityComparer&lt;T&gt;"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 
		/// <see cref="System.Collections.Generic.EqualityComparer&lt;T&gt;.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		/// <param name="dictionaryCreationThreshold">在不创建查找字典的情况下列表可容纳的元素的数目
		/// （<c>0</c> 表示添加第一项时创建查找字典）；或者为 <c>-1</c>，表示指定永远不会创建查找字典。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="dictionaryCreationThreshold"/> 小于 <c>-1</c>。</exception>
		protected KeyedListBase(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			if (dictionaryCreationThreshold == -1)
			{
				dictionaryCreationThreshold = int.MaxValue;
			}
			if (dictionaryCreationThreshold < -1)
			{
				ExceptionHelper.InvalidDictionaryThreshold("dictionaryCreationThreshold");
			}
			this.comparer = comparer;
			this.threshold = dictionaryCreationThreshold;
		}
		/// <summary>
		/// 获取用于确定列表中的键是否相等的泛型相等比较器。
		/// </summary>
		public IEqualityComparer<TKey> Comparer
		{
			get { return this.comparer; }
		}
		/// <summary>
		/// 获取 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 的查找字典。
		/// </summary>
		protected IDictionary<TKey, TItem> Dictionary
		{
			get { return this.dict; }
		}
		/// <summary>
		/// 获取具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取的元素的键。</param>
		/// <returns>带有指定键的元素。如果未找到具有指定键的元素，则引发异常。</returns>
		public TItem this[TKey key]
		{
			get
			{
				if (this.dict != null)
				{
					return this.dict[key];
				}
				int cnt = base.Items.Count;
				for (int i = 0; i < cnt; i++)
				{
					TItem item = base.Items[i];
					if (this.comparer.Equals(this.GetKeyForItem(item), key))
					{
						return item;
					}
				}
				throw ExceptionHelper.KeyNotFound(key);
			}
		}
		/// <summary>
		/// 确定某元素的键是否在 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中。
		/// </summary>
		/// <param name="key">要定位的元素的键。</param>
		/// <returns>如果在 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 
		/// 中找到具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(TKey key)
		{
			if (this.dict != null)
			{
				return this.dict.ContainsKey(key);
			}
			int cnt = base.Items.Count;
			for (int i = 0; i < cnt; i++)
			{
				if (this.comparer.Equals(this.GetKeyForItem(base.Items[i]), key))
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 确定 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中特定元素的键的索引。
		/// </summary>
		/// <param name="key">要在 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中定位的元素的键。</param>
		/// <returns>如果在 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中找到 <paramref name="key"/>，
		/// 则为该项的索引；否则为 <c>-1</c>。</returns>
		public int IndexOf(TKey key)
		{
			if (this.dict != null)
			{
				TItem item;
				if (this.dict.TryGetValue(key, out item))
				{
					return this.IndexOf(item);
				}
				return -1;
			}
			int cnt = base.Items.Count;
			for (int i = 0; i < cnt; i++)
			{
				if (this.comparer.Equals(this.GetKeyForItem(base.Items[i]), key))
				{
					return i;
				}
			}
			return -1;
		}
		/// <summary>
		/// 从 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中移除具有指定键的元素。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <returns>如果已从 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中成功移除元素，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 
		/// 中没有找到指定的键，该方法也会返回 <c>false</c>。</returns>
		public bool Remove(TKey key)
		{
			int index = IndexOf(key);
			if (index >= 0)
			{
				this.RemoveItem(index);
				return true;
			}
			return false;
		}
		/// <summary>
		/// 获取具有指定的键的元素。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="item">当此方法返回值时，如果找到该键，便会返回与指定的键相关联的值；
		/// 否则，则会返回 <paramref name="item"/> 参数的类型默认值。 该参数未经初始化即被传递。</param>
		/// <returns>如果包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool TryGetItem(TKey key, out TItem item)
		{
			ExceptionHelper.CheckArgumentNull(key, "key");
			if (this.dict != null)
			{
				return this.dict.TryGetValue(key, out item);
			}
			int cnt = base.Items.Count;
			for (int i = 0; i < cnt; i++)
			{
				item = base.Items[i];
				if (this.comparer.Equals(this.GetKeyForItem(item), key))
				{
					return true;
				}
			}
			item = default(TItem);
			return false;
		}
		/// <summary>
		/// 更改与查找字典中指定元素相关联的键。
		/// </summary>
		/// <param name="item">要更改其键的元素。</param>
		/// <param name="newKey"><paramref name="item"/> 的新键。</param>
		/// <exception cref="System.ArgumentException"><paramref name="item"/> 未找到。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="newKey"/> 在列表中已存在。</exception>
		protected void ChangeItemKey(TItem item, TKey newKey)
		{
			if (!this.Contains(item))
			{
				throw ExceptionHelper.ItemNotExist("item");
			}
			TKey key = this.GetKeyForItem(item);
			if (!this.comparer.Equals(key, newKey))
			{
				this.RemoveKey(key);
				this.AddKey(newKey, item);
			}
		}
		/// <summary>
		/// 在派生类中实现时，将从指定元素提取键。
		/// </summary>
		/// <param name="item">从中提取键的元素。</param>
		/// <returns>指定元素的键。</returns>
		protected abstract TKey GetKeyForItem(TItem item);

		#region ListBase<TItem> 成员

		/// <summary>
		/// 从 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中移除所有元素。
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			if (this.dict != null)
			{
				this.dict.Clear();
			}
			this.keyCount = 0;
		}
		/// <summary>
		/// 将元素插入 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入的对象。对于引用类型，该值可以为 <c>null</c>。</param>
		protected override void InsertItem(int index, TItem item)
		{
			this.AddKey(this.GetKeyForItem(item), item);
			base.InsertItem(index, item);
		}
		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。对于引用类型，该值可以为 <c>null</c>。</param>
		protected override void SetItem(int index, TItem item)
		{
			TKey key = this.GetKeyForItem(item);
			TKey okdKey = this.GetKeyForItem(base.Items[index]);
			if (this.comparer.Equals(okdKey, key))
			{
				if (this.dict != null)
				{
					this.dict[key] = item;
				}
			}
			else
			{
				this.RemoveKey(okdKey);
				this.AddKey(key, item);
			}
			base.SetItem(index, item);
		}
		/// <summary>
		/// 移除 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			this.RemoveKey(this.GetKeyForItem(base.Items[index]));
			base.RemoveItem(index);
		}

		#endregion // ListBase<TItem> 成员

		#region ICollection<TItem> 成员

		/// <summary>
		/// 确定某元素是否在 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 中。
		/// </summary>
		/// <param name="item">要定位的元素。</param>
		/// <returns>如果在 <see cref="KeyedListBase&lt;TKey,TItem&gt;"/> 
		/// 中找到指定的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(TItem item)
		{
			if (this.dict == null)
			{
				return base.Items.Contains(item);
			}
			TItem newItem;
			if (this.dict.TryGetValue(this.GetKeyForItem(item), out newItem))
			{
				return EqualityComparer<TItem>.Default.Equals(newItem, item);
			}
			return false;
		}

		#endregion // ICollection<TItem> 成员

		/// <summary>
		/// 将元素的键和值添加到字典中。
		/// </summary>
		/// <param name="key">要添加的键。</param>
		/// <param name="item">要添加的值。</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="key"/> 在列表中已存在。</exception>
		private void AddKey(TKey key, TItem item)
		{
			if (this.dict != null)
			{
				this.dict.Add(key, item);
			}
			else if (this.keyCount == this.threshold)
			{
				this.CreateDictionary();
				this.dict.Add(key, item);
			}
			else
			{
				if (this.Contains(key))
				{
					throw ExceptionHelper.KeyDuplicate("key");
				}
				this.keyCount++;
			}
		}
		/// <summary>
		/// 从字典中移除指定的键。
		/// </summary>
		/// <param name="key">要移除的键。</param>
		private void RemoveKey(TKey key)
		{
			if (this.dict != null)
			{
				this.dict.Remove(key);
			}
			else
			{
				this.keyCount--;
			}
		}
		/// <summary>
		/// 创建字典。
		/// </summary>
		private void CreateDictionary()
		{
			this.dict = new Dictionary<TKey, TItem>(this.comparer);
			int cnt = base.Items.Count;
			for (int i = 0; i < cnt; i++)
			{
				TItem item = base.Items[i];
				this.dict.Add(this.GetKeyForItem(item), item);
			}
		}
	}
}
