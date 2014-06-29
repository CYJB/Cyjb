using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 提供集合键嵌入在值中的集合的抽象基类。
	/// </summary>
	/// <typeparam name="TKey">集合中键的类型。</typeparam>
	/// <typeparam name="TItem">集合中的元素的类型。</typeparam>
	/// <remarks>集合中存储的元素都不能为 <c>null</c>，相应的键也不能为 <c>null</c>。</remarks>
	[Serializable]
	public abstract class KeyedCollectionBase<TKey, TItem> : CollectionBase<TItem>
	{
		/// <summary>
		/// 用于保存元素的字典。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IDictionary<TKey, TItem> dict;
		/// <summary>
		/// 初始化使用默认相等比较器的 <see cref="KeyedCollectionBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="KeyedCollectionBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		protected KeyedCollectionBase()
			: this(new Dictionary<TKey, TItem>())
		{ }
		/// <summary>
		/// 使用指定的相等比较器初始化 <see cref="KeyedCollectionBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 <see cref="EqualityComparer{T}.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		protected KeyedCollectionBase(IEqualityComparer<TKey> comparer) :
			this(new Dictionary<TKey, TItem>(comparer))
		{ }
		/// <summary>
		/// 使用指定的字典初始化 <see cref="KeyedCollectionBase{TKey, TItem}"/> 类的新实例。
		/// </summary>
		/// <param name="dict">用于保存项的字典。</param>
		/// <exception cref="ArgumentNullException"><paramref name="dict"/> 为 <c>null</c>。</exception>
		protected KeyedCollectionBase(IDictionary<TKey, TItem> dict)
			: base(CollectionHelper.GetDictValues(dict))
		{
			Contract.Requires(dict != null);
			this.dict = dict;
		}
		/// <summary>
		/// 获取 <see cref="KeyedCollectionBase{TKey,TItem}"/> 的查找字典。
		/// </summary>
		/// <value><see cref="KeyedCollectionBase{TKey,TItem}"/> 的查找字典。</value>
		/// <remarks>集合元素直接储存在查找字典中，<see cref="CollectionBase{T}.Items"/> 
		/// 是值的只读集合。</remarks>
		protected IDictionary<TKey, TItem> Dictionary
		{
			get
			{
				Contract.Ensures(Contract.Result<IDictionary<TKey, TItem>>() != null);
				return this.dict;
			}
		}

		#region 键操作

		/// <summary>
		/// 获取具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取的元素的键。</param>
		/// <returns>带有指定键的元素。如果未找到具有指定键的元素，则引发异常。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		/// <exception cref="KeyNotFoundException">在集合中不存在 <paramref name="key"/>。</exception>
		[Pure]
		public TItem this[TKey key]
		{
			get
			{
				if (key == null)
				{
					throw ExceptionHelper.ArgumentNull("key");
				}
				Contract.Ensures(Contract.Result<TItem>() != null);
				return this.dict[key];
			}
		}
		/// <summary>
		/// 确定某元素的键是否在 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中。
		/// </summary>
		/// <param name="key">要定位的元素的键。</param>
		/// <returns>如果在 <see cref="KeyedCollectionBase{TKey,TItem}"/> 
		/// 中找到具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		[Pure]
		public bool ContainsKey(TKey key)
		{
			return key != null && this.dict.ContainsKey(key);
		}
		/// <summary>
		/// 从 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中移除具有指定键的元素。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <returns>如果已从 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中成功移除元素，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="KeyedCollectionBase{TKey,TItem}"/> 
		/// 中没有找到指定的键，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public virtual bool Remove(TKey key)
		{
			if (key == null)
			{
				throw ExceptionHelper.ArgumentNull("key");
			}
			Contract.EndContractBlock();
			return this.dict.Remove(key);
		}
		/// <summary>
		/// 获取具有指定的键的元素。
		/// </summary>
		/// <param name="key">要获取的值的键。</param>
		/// <param name="item">当此方法返回值时，如果找到该键，便会返回与指定的键相关联的值；
		/// 否则，则会返回 <paramref name="item"/> 参数的类型默认值。 该参数未经初始化即被传递。</param>
		/// <returns>如果包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		[Pure]
		public bool TryGetValue(TKey key, out TItem item)
		{
			if (key == null)
			{
				throw ExceptionHelper.ArgumentNull("key");
			}
			Contract.EndContractBlock();
			return this.dict.TryGetValue(key, out item);
		}
		/// <summary>
		/// 在派生类中实现时，将从指定元素提取键。
		/// </summary>
		/// <param name="item">从中提取键的元素。</param>
		/// <returns>指定元素的键。</returns>
		[Pure]
		protected abstract TKey GetKeyForItem(TItem item);
		/// <summary>
		/// 更改与指定元素相关联的键。
		/// </summary>
		/// <param name="item">要更改其键的元素。</param>
		/// <param name="newKey"><paramref name="item"/> 的新键。</param>
		/// <exception cref="ArgumentException"><paramref name="newKey"/> 在字典中已存在。</exception>
		protected void ChangeItemKey(TItem item, TKey newKey)
		{
			Contract.Requires(this.Contains(item));
			TKey key = this.GetKeyForItem(item);
			Contract.Assert(key != null);
			if (EqualityComparer<TKey>.Default.Equals(key, newKey))
			{
				return;
			}
			this.dict.Remove(key);
			this.dict.Add(newKey, item);
		}

		#endregion // 键操作

		#region ICollection<TItem> 成员

		/// <summary>
		/// 将指定对象添加到 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中。
		/// </summary>
		/// <param name="item">要添加到 <see cref="KeyedCollectionBase{TKey,TItem}"/> 的对象。</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException">集合中已存在具有相同键的元素。</exception>
		public override void Add(TItem item)
		{
			if (item == null)
			{
				throw ExceptionHelper.ArgumentNull("item");
			}
			Contract.EndContractBlock();
			TKey key = this.GetKeyForItem(item);
			Contract.Assert(key != null);
			this.dict.Add(key, item);
		}
		/// <summary>
		/// 从 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中移除所有元素。
		/// </summary>
		public override void Clear()
		{
			this.dict.Clear();
		}
		/// <summary>
		/// 确定 <see cref="KeyedCollectionBase{TKey,TItem}"/> 是否包含指定对象。
		/// </summary>
		/// <param name="item">要在 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(TItem item)
		{
			if (item == null)
			{
				return false;
			}
			TKey key = this.GetKeyForItem(item);
			Contract.Assert(key != null);
			TItem newItem;
			return this.dict.TryGetValue(key, out newItem) &&
				EqualityComparer<TItem>.Default.Equals(newItem, item);
		}
		/// <summary>
		/// 从 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="KeyedCollectionBase{TKey,TItem}"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="KeyedCollectionBase{TKey,TItem}"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> 为 <c>null</c>。</exception>
		public override bool Remove(TItem item)
		{
			if (item == null)
			{
				throw ExceptionHelper.ArgumentNull("item");
			}
			Contract.EndContractBlock();
			TKey key = this.GetKeyForItem(item);
			Contract.Assert(key != null);
			TItem oldItem;
			if (this.dict.TryGetValue(key, out oldItem) &&
				EqualityComparer<TItem>.Default.Equals(item, oldItem))
			{
				return this.dict.Remove(key);
			}
			return false;
		}

		#endregion // ICollection<TItem> 成员

	}
}
