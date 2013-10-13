using System;
using System.Collections.Generic;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 提供集合键嵌入在值中的集合的抽象基类。
	/// </summary>
	/// <typeparam name="TKey">集合中键的类型。</typeparam>
	/// <typeparam name="TItem">集合中的项的类型。</typeparam>
	[Serializable]
	public abstract class KeyedCollectionBase<TKey, TItem> : CollectionBase<TItem>
	{
		/// <summary>
		/// 用于确定集合中的键是否相等的泛型相等比较器。
		/// </summary>
		private readonly IEqualityComparer<TKey> comparer;
		/// <summary>
		/// 用于保存键的字典。
		/// </summary>
		private Dictionary<TKey, TItem> dict;
		/// <summary>
		/// 初始化使用默认相等比较器的 <see cref="KeyedCollectionBase&lt;TKey, TItem&gt;"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="KeyedCollectionBase&lt;TKey, TItem&gt;"/> 类的新实例。
		/// </summary>
		/// </overloads>
		protected KeyedCollectionBase()
			: this(null, false)
		{ }
		/// <summary>
		/// 初始化使用默认相等比较器的 <see cref="KeyedCollectionBase&lt;TKey, TItem&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="isReadOnly">集合是否是只读的。</param>
		protected KeyedCollectionBase(bool isReadOnly)
			: this(null, isReadOnly)
		{ }
		/// <summary>
		/// 初始化使用指定相等比较器的 <see cref="KeyedCollectionBase&lt;TKey, TItem&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 
		/// <see cref="System.Collections.Generic.IEqualityComparer&lt;T&gt;"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 
		/// <see cref="System.Collections.Generic.EqualityComparer&lt;T&gt;.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		protected KeyedCollectionBase(IEqualityComparer<TKey> comparer)
			: this(comparer, false)
		{ }
		/// <summary>
		/// 初始化 <see cref="KeyedCollectionBase&lt;TKey, TItem&gt;"/> 类的新实例，
		/// 该新实例使用指定的相等比较器并在超过指定阈值时创建一个查找字典。
		/// </summary>
		/// <param name="comparer">比较键时要使用的 
		/// <see cref="System.Collections.Generic.IEqualityComparer&lt;T&gt;"/> 泛型接口的实现，
		/// 如果为 <c>null</c>，则使用从 
		/// <see cref="System.Collections.Generic.EqualityComparer&lt;T&gt;.Default"/> 
		/// 获取的该类型的键的默认相等比较器。</param>
		/// <param name="isReadOnly">集合是否是只读的。</param>
		protected KeyedCollectionBase(IEqualityComparer<TKey> comparer, bool isReadOnly)
			: base(null, isReadOnly)
		{
			dict = new Dictionary<TKey, TItem>(comparer);
			this.comparer = dict.Comparer;
		}
		/// <summary>
		/// 获取用于确定集合中的键是否相等的泛型相等比较器。
		/// </summary>
		/// <value>用于确定集合中的键是否相等的泛型相等比较器。</value>
		public IEqualityComparer<TKey> Comparer
		{
			get { return this.comparer; }
		}
		/// <summary>
		/// 获取 <see cref="CollectionBase&lt;T&gt;"/> 周围的 <see cref="ICollection&lt;T&gt;"/> 包装。
		/// </summary>
		/// <value><see cref="CollectionBase&lt;T&gt;"/> 周围的 <see cref="ICollection&lt;T&gt;"/> 包装。</value>
		protected override ICollection<TItem> Items
		{
			get { return dict.Values; }
		}
		/// <summary>
		/// 获取 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 的查找字典。
		/// </summary>
		/// <value><see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 的查找字典，
		/// 集合中的所有值都保存在字典中，而不是 <see cref="CollectionBase&lt;T&gt;.Items"/> 中。</value>
		protected IDictionary<TKey, TItem> Dictionary
		{
			get { return this.dict; }
		}
		/// <summary>
		/// 获取具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取的元素的键。</param>
		/// <returns>带有指定键的元素。如果未找到具有指定键的元素，则引发异常。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="key"/> 为 <c>null</c>。</exception>
		public TItem this[TKey key]
		{
			get { return this.dict[key]; }
		}
		/// <summary>
		/// 确定某元素的键是否在 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中。
		/// </summary>
		/// <param name="key">要定位的元素的键。</param>
		/// <returns>如果在 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 
		/// 中找到具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(TKey key)
		{
			return this.dict.ContainsKey(key);
		}
		/// <summary>
		/// 从 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中移除具有指定键的元素。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <returns>如果已从 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中成功移除元素，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 
		/// 中没有找到指定的键，该方法也会返回 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 从 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中移除特定元素。
		/// </summary>
		/// </overloads>
		public bool Remove(TKey key)
		{
			if (this.dict.ContainsKey(key))
			{
				this.RemoveItem(this.dict[key]);
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
			return this.dict.TryGetValue(key, out item);
		}
		/// <summary>
		/// 在派生类中实现时，将从指定元素提取键。
		/// </summary>
		/// <param name="item">从中提取键的元素。</param>
		/// <returns>指定元素的键。</returns>
		protected abstract TKey GetKeyForItem(TItem item);

		#region CollectionBase<TItem> 成员

		/// <summary>
		/// 从 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中移除所有元素。
		/// </summary>
		protected override void ClearItems()
		{
			this.dict.Clear();
		}
		/// <summary>
		/// 将元素添加到 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中。
		/// </summary>
		/// <param name="item">要添加到 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 的对象。</param>
		protected override void AddItem(TItem item)
		{
			this.dict.Add(this.GetKeyForItem(item), item);
		}
		/// <summary>
		/// 移除 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 的指定元素。
		/// </summary>
		/// <param name="item">要移除的元素。</param>
		/// <returns>如果成功移除了元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		protected override bool RemoveItem(TItem item)
		{
			return this.dict.Remove(this.GetKeyForItem(item));
		}

		#endregion // CollectionBase<TItem> 成员

		#region ICollection<TItem> 成员

		/// <summary>
		/// 确定 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 确定 <see cref="KeyedCollectionBase&lt;TKey,TItem&gt;"/> 是否包含特定值。
		/// </summary>
		/// </overloads>
		public override bool Contains(TItem item)
		{
			ExceptionHelper.CheckArgumentNull(item, "item");
			TItem newItem;
			if (this.dict.TryGetValue(this.GetKeyForItem(item), out newItem))
			{
				return EqualityComparer<TItem>.Default.Equals(newItem, item);
			}
			return false;
		}

		#endregion // ICollection<TItem> 成员

	}
}
