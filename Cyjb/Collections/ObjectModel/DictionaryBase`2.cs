using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{

	/// <summary>
	/// 为泛型字典提供基类。
	/// </summary>
	/// <typeparam name="TKey">字典的键的类型。</typeparam>
	/// <typeparam name="TValue">字典的值的类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(DictionaryBase<,>.DebugView))]
	public class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
	{
		/// <summary>
		/// 用于同步的对象。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object syncRoot;
		/// <summary>
		/// 集合是否是只读的。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool isReadOnly;
		/// <summary>
		/// 键集合。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private DictionaryKeyCollection<TKey, TValue> keys;
		/// <summary>
		/// 值集合。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private DictionaryValueCollection<TKey, TValue> values;
		/// <summary>
		/// 周围的 <see cref="IDictionary&lt;TKey,TValue&gt;"/> 包装。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IDictionary<TKey, TValue> items;
		/// <summary>
		/// 初始化 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// </overloads>
		protected DictionaryBase()
		{
			this.items = new Dictionary<TKey, TValue>();
		}
		/// <summary>
		/// 初始化 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="isReadOnly">集合是否是只读的。</param>
		protected DictionaryBase(bool isReadOnly)
		{
			this.items = new Dictionary<TKey, TValue>();
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 将 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 类的新实例初始化为指定字典的包装。
		/// </summary>
		/// <param name="dict">由新的字典包装的字典。</param>
		protected DictionaryBase(IDictionary<TKey, TValue> dict)
		{
			this.items = dict;
			if (this.items != null)
			{
				this.isReadOnly = this.items.IsReadOnly;
			}
		}
		/// <summary>
		/// 将 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 类的新实例初始化为指定字典的包装。
		/// </summary>
		/// <param name="dict">由新的字典包装的字典。</param>
		/// <param name="isReadOnly">集合的包装是否是只读的。</param>
		protected DictionaryBase(IDictionary<TKey, TValue> dict, bool isReadOnly)
		{
			this.items = dict;
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 确定 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="value">要在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中定位的值。</param>
		/// <returns>如果 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 包含具有指定值的元素，
		/// 则为 <c>true</c>；否则，为 <c>false</c>。</returns>
		public virtual bool ContainsValue(TValue value)
		{
			EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
			foreach (KeyValuePair<TKey, TValue> pair in this)
			{
				if (comparer.Equals(pair.Value, value))
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中移除所有元素。
		/// </summary>
		protected virtual void ClearItems()
		{
			this.items.Clear();
		}
		/// <summary>
		/// 在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中添加一个带有所提供的键和值的元素。
		/// </summary>
		/// <param name="key">用作要添加的元素的键的对象。</param>
		/// <param name="value">用作要添加的元素的值的对象。</param>
		protected virtual void AddItem(TKey key, TValue value)
		{
			this.items.Add(key, value);
		}
		/// <summary>
		/// 从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中移除所指定的键的值。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <returns>如果成功找到并移除该元素，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中没有找到 <paramref name="key"/>，
		/// 此方法则返回 <c>false</c>。</returns>
		protected virtual bool RemoveItem(TKey key)
		{
			return this.items.Remove(key);
		}
		/// <summary>
		/// 替换指定键对应的值。
		/// </summary>
		/// <param name="key">待替换的元素的键。</param>
		/// <param name="value">要替换的新值。</param>
		protected virtual void SetItem(TKey key, TValue value)
		{
			this.items[key] = value;
		}
		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取其值的键。</param>
		/// <param name="queryValue">是否需要获取值。如果为 <c>true</c>，则需要获取值；
		/// 否则只需要返回是否存在的布尔值。</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；
		/// 否则，将返回 <paramref name="key"/> 参数的类型的默认值。该参数未经初始化即被传递。</param>
		/// <returns>如果 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象包含具有指定键的元素，
		/// 则为 <c>true</c>；否则，为 <c>false</c>。</returns>
		protected virtual bool TryGetValue(TKey key, bool queryValue, out TValue value)
		{
			return this.items.TryGetValue(key, out value);
		}
		/// <summary>
		/// 获取 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 周围的 
		/// <see cref="IDictionary&lt;TKey,TValue&gt;"/> 包装。
		/// </summary>
		/// <value><see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 周围的 
		/// <see cref="IDictionary&lt;TKey,TValue&gt;"/> 包装。</value>
		protected IDictionary<TKey, TValue> Items
		{
			get { return items; }
		}

		#region IDictionary<TKey, TValue> 成员

		/// <summary>
		/// 获取包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的键的
		/// <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/>。
		/// </summary>
		/// <value>一个 <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/>，
		/// 它包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的键。</value>
		public ICollection<TKey> Keys
		{
			get
			{
				if (this.keys == null)
				{
					this.keys = new DictionaryKeyCollection<TKey, TValue>(this);
				}
				return this.keys;
			}
		}

		/// <summary>
		/// 获取包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中的值的 
		/// <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/>。
		/// </summary>
		/// <value>一个 <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/>，
		/// 它包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中的值。</value>
		public ICollection<TValue> Values
		{
			get
			{
				if (this.values == null)
				{
					this.values = new DictionaryValueCollection<TKey, TValue>(this);
				}
				return this.values;
			}
		}

		/// <summary>
		/// 获取或设置具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取或设置的元素的键。</param>
		/// <value>带有指定键的元素。</value>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">
		/// 检索了属性但没有找到 <paramref name="key"/>。</exception>
		/// <exception cref="System.NotSupportedException">设置该属性，而且
		/// <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 为只读。</exception>
		public TValue this[TKey key]
		{
			get
			{
				TValue value;
				if (!this.TryGetValue(key, out value))
				{
					throw ExceptionHelper.KeyNotFound("key");
				}
				return value;
			}
			set
			{
				if (this.isReadOnly)
				{
					throw ExceptionHelper.ReadOnlyCollection();
				}
				this.SetItem(key, value);
			}
		}

		/// <summary>
		/// 在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中添加一个带有所提供的键和值的元素。
		/// </summary>
		/// <param name="key">用作要添加的元素的键的对象。</param>
		/// <param name="value">用作要添加的元素的值的对象。</param>
		/// <exception cref="System.ArgumentException"><see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 对象中已存在具有相同键的元素。</exception>
		/// <exception cref="System.NotSupportedException"><see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 是只读的。</exception>
		public void Add(TKey key, TValue value)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.AddItem(key, value);
		}

		/// <summary>
		/// 确定 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是否包含具有指定键的元素。
		/// </summary>
		/// <param name="key">要在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中定位的键。</param>
		/// <returns>如果 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 包含带有该键的元素，
		/// 则为 <c>true</c>；否则，为 <c>false</c>。</returns>
		public bool ContainsKey(TKey key)
		{
			TValue value;
			return this.TryGetValue(key, false, out value);
		}

		/// <summary>
		/// 从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中移除所指定的键的值。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <returns>如果成功找到并移除该元素，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中没有找到 <paramref name="key"/>，
		/// 此方法则返回 <c>false</c>。</returns>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是只读的。</exception>
		public bool Remove(TKey key)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			return this.RemoveItem(key);
		}

		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <param name="key">要获取其值的键。</param>
		/// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；
		/// 否则，将返回 <paramref name="key"/> 参数的类型的默认值。该参数未经初始化即被传递。</param>
		/// <returns>如果 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象包含具有指定键的元素，
		/// 则为 <c>true</c>；否则，为 <c>false</c>。</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.TryGetValue(key, true, out value);
		}

		#endregion // IDictionary<TKey, TValue> 成员

		#region ICollection<KeyValuePair<TKey, TValue>> 成员

		/// <summary>
		/// 获取 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中包含的元素数。</value>
		public virtual int Count
		{
			get { return this.items.Count; }
		}

		/// <summary>
		/// 获取一个值，该值指示 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		public bool IsReadOnly
		{
			get { return this.isReadOnly; }
		}

		/// <summary>
		/// 将某项添加到 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中。
		/// </summary>
		/// <param name="item">要添加到 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的对象。</param>
		/// <exception cref="System.NotSupportedException"><see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是只读的。</exception>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.AddItem(item.Key, item.Value);
		}

		/// <summary>
		/// 从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中移除所有项。
		/// </summary>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是只读的。</exception>
		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.ClearItems();
		}

		/// <summary>
		/// 确定 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中定位的对象。</param>
		/// <returns>如果包含特定值，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			TValue value;
			if (this.TryGetValue(item.Key, out value))
			{
				return EqualityComparer<TValue>.Default.Equals(item.Value, value);
			}
			return false;
		}
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.items.CopyTo(array, arrayIndex);
		}
		/// <summary>
		/// 从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中成功移除
		/// <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在原始 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中没有找到 <paramref name="item"/>，
		/// 该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是只读的。</exception>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			TValue value;
			if (this.TryGetValue(item.Key, out value))
			{
				if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
				{
					this.RemoveItem(item.Key);
					return true;
				}
			}
			return false;
		}

		#endregion // ICollection<KeyValuePair<TKey, TValue>> 成员

		#region IDictionary 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是否具有固定大小。
		/// </summary>
		/// <value>如果 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 具有固定大小，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IDictionary.IsFixedSize
		{
			get { return false; }
		}

		/// <summary>
		/// 获取一个值，该值指示 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 为只读，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IDictionary.IsReadOnly
		{
			get { return this.isReadOnly; }
		}

		/// <summary>
		/// 获取 <see cref="System.Collections.ICollection"/> 对象，
		/// 它包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象的键。
		/// </summary>
		/// <value><see cref="System.Collections.ICollection"/> 对象，
		/// 它包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象的键。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ICollection IDictionary.Keys
		{
			get
			{
				if (this.keys == null)
				{
					this.keys = new DictionaryKeyCollection<TKey, TValue>(this);
				}
				return this.keys;
			}
		}

		/// <summary>
		/// 获取 <see cref="System.Collections.ICollection"/> 对象，
		/// 它包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象中的值。
		/// </summary>
		/// <value><see cref="System.Collections.ICollection"/> 对象，
		/// 它包含 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象中的值。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		ICollection IDictionary.Values
		{
			get
			{
				if (this.values == null)
				{
					this.values = new DictionaryValueCollection<TKey, TValue>(this);
				}
				return this.values;
			}
		}

		/// <summary>
		/// 获取或设置具有指定键的元素。
		/// </summary>
		/// <param name="key">要获取或设置的元素的键。</param>
		/// <value>带有指定键的元素。</value>
		/// <exception cref="System.NotSupportedException">设置该属性，
		/// 而且 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 为只读。</exception>
		object IDictionary.this[object key]
		{
			get
			{
				TValue value = default(TValue);
				if (key is TKey)
				{
					this.TryGetValue((TKey)key, out value);
				}
				return value;
			}
			set
			{
				if (this.isReadOnly)
				{
					throw ExceptionHelper.ReadOnlyCollection();
				}
				if (!(key is TKey))
				{
					throw ExceptionHelper.ArgumentWrongType("key", key, typeof(TKey));
				}
				if (!(value is TValue))
				{
					throw ExceptionHelper.ArgumentWrongType("value", value, typeof(TValue));
				}
				this.SetItem((TKey)key, (TValue)value);
			}
		}

		/// <summary>
		/// 在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象中添加一个带有所提供的键和值的元素。
		/// </summary>
		/// <param name="key">用作要添加的元素的键的 <see cref="System.Object"/>。</param>
		/// <param name="value">用作要添加的元素的值的 <see cref="System.Object"/>。</param>
		/// <exception cref="System.ArgumentNullException"><see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 对象中已存在具有相同键的元素。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是只读的。</exception>
		void IDictionary.Add(object key, object value)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			if (!(key is TKey))
			{
				throw ExceptionHelper.ArgumentWrongType("key", key, typeof(TKey));
			}
			if (!(value is TValue))
			{
				throw ExceptionHelper.ArgumentWrongType("value", value, typeof(TValue));
			}
			this.AddItem((TKey)key, (TValue)value);
		}

		/// <summary>
		/// 从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象中移除所有元素。
		/// </summary>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是只读的。</exception>
		void IDictionary.Clear()
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.Clear();
		}

		/// <summary>
		/// 确定 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象是否包含具有指定键的元素。
		/// </summary>
		/// <param name="key">要在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象中定位的键。</param>
		/// <returns>如果在 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 包含带有该键的元素，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		bool IDictionary.Contains(object key)
		{
			if (key is TKey)
			{
				return this.ContainsKey((TKey)key);
			}
			return false;
		}

		/// <summary>
		/// 从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象中移除带有指定键的元素。
		/// </summary>
		/// <param name="key">要移除的元素的键。</param>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 是只读的。</exception>
		void IDictionary.Remove(object key)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			if (key is TKey)
			{
				this.Remove((TKey)key);
			}
		}

		/// <summary>
		/// 返回一个用于 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象的
		/// <see cref="System.Collections.IDictionaryEnumerator"/> 对象。
		/// </summary>
		/// <returns>一个用于 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 对象的
		/// <see cref="System.Collections.IDictionaryEnumerator"/> 对象。</returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new DictionaryEnumerator<TKey, TValue>(this.GetEnumerator());
		}

		#endregion // IDictionary 成员

		#region ICollection 成员

		/// <summary>
		/// 获取 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 中包含的元素数。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int ICollection.Count
		{
			get { return this.Count; }
		}

		/// <summary>
		/// 获取一个值，该值指示是否同步对 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的访问（线程安全）。
		/// </summary>
		/// <value>如果对 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的访问是同步的（线程安全），
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// 获取一个可用于同步对 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的访问的对象。
		/// </summary>
		/// <value>可用于同步对 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 的访问的对象。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot
		{
			get
			{
				if (this.syncRoot == null)
				{
					ICollection colItems = this.items as ICollection;
					Interlocked.CompareExchange(ref this.syncRoot,
						colItems == null ? new object() : colItems.SyncRoot, null);
				}
				return this.syncRoot;
			}
		}

		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，将 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，
		/// 在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 <paramref name="array"/> 末尾之间的可用空间。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="DictionaryBase&lt;TKey,TValue&gt;"/> 
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			ExceptionHelper.CheckFlatArray(array, "array");
			if (index < 0)
			{
				throw ExceptionHelper.ArgumentNegative("index");
			}
			if (array.Length - index < this.Count)
			{
				throw ExceptionHelper.ArrayTooSmall("array");
			}
			KeyValuePair<TKey, TValue>[] arr = array as KeyValuePair<TKey, TValue>[];
			if (arr != null)
			{
				this.CopyTo(arr, index);
			}
			else
			{
				DictionaryEntry[] entryArr = array as DictionaryEntry[];
				if (entryArr != null)
				{
					foreach (KeyValuePair<TKey, TValue> obj in this)
					{
						entryArr[index++] = new DictionaryEntry(obj.Key, obj.Value);
					}
				}
				else
				{
					try
					{
						foreach (KeyValuePair<TKey, TValue> obj in this)
						{
							array.SetValue(obj, index++);
						}
					}
					catch (InvalidCastException ex)
					{
						throw ExceptionHelper.ArrayTypeInvalid(ex);
					}
				}
			}
		}

		#endregion // ICollection 成员

		#region IEnumerable<KeyValuePair<TKey, TValue>> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		#endregion // IEnumerable<KeyValuePair<TKey, TValue>> 成员

		#region IEnumerable 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.IEnumerator"/>。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion // IEnumerable 成员

		#region 调试视图

		/// <summary>
		/// 表示字符串键/值集合的调试视图。
		/// </summary>
		private class DebugView
		{
			/// <summary>
			/// 字符串键/值集合。
			/// </summary>
			private readonly DictionaryBase<TKey, TValue> dict;
			/// <summary>
			/// 使用指定的字符串键/值集合初始化 <see cref="DebugView"/>
			/// 类的实例。
			/// </summary>
			/// <param name="dict">使用视图的字符串键/值集合。</param>
			public DebugView(DictionaryBase<TKey, TValue> dict)
			{
				this.dict = dict;
			}
			/// <summary>
			/// 获取字符串键/值集合中的字符串。
			/// </summary>
			/// <value>包含了字符串键/值集合中所有字符串的数组。</value>
			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePair<TKey, TValue>[] Items
			{
				get { return this.dict.ToArray(); }
			}
		}

		#endregion

	}

}
