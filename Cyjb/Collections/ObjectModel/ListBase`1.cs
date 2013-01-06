using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 为泛型列表提供基类。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class ListBase<T> : IList<T>, IList
	{
		/// <summary>
		/// 提供同步的对象。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object syncRoot;
		/// <summary>
		/// 集合是否是只读的。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool isReadOnly;
		/// <summary>
		/// 周围的 <see cref="IList&lt;T&gt;"/> 包装。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IList<T> items;
		/// <summary>
		/// 初始化 <see cref="ListBase&lt;T&gt;"/> 类的新实例。
		/// </summary>
		protected ListBase()
		{
			this.items = new List<T>();
		}
		/// <summary>
		/// 初始化 <see cref="ListBase&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="isReadOnly">集合是否是只读的。</param>
		protected ListBase(bool isReadOnly)
		{
			this.items = new List<T>();
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 将 <see cref="ListBase&lt;T&gt;"/> 类的新实例初始化为指定集合的包装。
		/// </summary>
		/// <param name="list">由新的集合包装的集合。</param>
		protected ListBase(IList<T> list)
		{
			this.items = list;
		}
		/// <summary>
		/// 将 <see cref="ListBase&lt;T&gt;"/> 类的新实例初始化为指定集合的包装。
		/// </summary>
		/// <param name="list">由新的集合包装的集合。</param>
		/// <param name="isReadOnly">集合的包装是否是只读的。</param>
		protected ListBase(IList<T> list, bool isReadOnly)
		{
			this.items = list;
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 获取 <see cref="ListBase&lt;T&gt;"/> 周围的 <see cref="IList&lt;T&gt;"/> 包装。
		/// </summary>
		protected IList<T> Items
		{
			get { return items; }
		}
		/// <summary>
		/// 从 <see cref="ListBase&lt;T&gt;"/> 中移除所有元素。
		/// </summary>
		protected virtual void ClearItems()
		{
			this.items.Clear();
		}
		/// <summary>
		/// 将元素插入 <see cref="ListBase&lt;T&gt;"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入的对象。对于引用类型，该值可以为 <c>null</c>。</param>
		protected virtual void InsertItem(int index, T item)
		{
			this.items.Insert(index, item);
		}
		/// <summary>
		/// 移除 <see cref="ListBase&lt;T&gt;"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected virtual void RemoveItem(int index)
		{
			this.items.RemoveAt(index);
		}
		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。对于引用类型，该值可以为 <c>null</c>。</param>
		protected virtual void SetItem(int index, T item)
		{
			this.items[index] = item;
		}

		#region IList<T> 成员

		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="System.ArgumentOutOfRangeException">设置该属性，
		/// 而且 <see cref="ListBase&lt;T&gt;"/> 为只读。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		public virtual T this[int index]
		{
			get
			{
				return this.items[index];
			}
			set
			{
				if (this.isReadOnly)
				{
					throw ExceptionHelper.ReadOnlyCollection();
				}
				if (index < 0 || index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index");
				}
				this.SetItem(index, value);
			}
		}

		/// <summary>
		/// 确定 <see cref="ListBase&lt;T&gt;"/> 中特定项的索引。
		/// </summary>
		/// <param name="item">要在 <see cref="ListBase&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ListBase&lt;T&gt;"/> 中找到 <paramref name="item"/>，
		/// 则为该项的索引；否则为 <c>-1</c>。</returns>
		public virtual int IndexOf(T item)
		{
			return items.IndexOf(item);
		}

		/// <summary>
		/// 在 <see cref="ListBase&lt;T&gt;"/> 中的指定索引处插入项。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入到 <see cref="ListBase&lt;T&gt;"/> 中的对象。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase&lt;T&gt;"/> 中的有效索引。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		public void Insert(int index, T item)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			if (index < 0 || index > this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			this.InsertItem(index, item);
		}

		/// <summary>
		/// 移除指定索引处的 <see cref="ListBase&lt;T&gt;"/> 项。
		/// </summary>
		/// <param name="index">从零开始的索引（属于要移除的项）。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase&lt;T&gt;"/> 中的有效索引。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		public void RemoveAt(int index)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			if (index < 0 || index >= this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			this.RemoveItem(index);
		}

		#endregion // IList<T> 成员

		#region IList 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ListBase&lt;T&gt;"/> 是否具有固定大小。
		/// </summary>
		/// <value>如果 <see cref="ListBase&lt;T&gt;"/> 具有固定大小，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsFixedSize
		{
			get { return false; }
		}

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ListBase&lt;T&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ListBase&lt;T&gt;"/> 为只读，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsReadOnly
		{
			get { return this.isReadOnly; }
		}

		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获得或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase&lt;T&gt;"/> 中的有效索引。</exception>
		/// <exception cref="System.NotSupportedException">设置该属性，
		/// 而且 <see cref="ListBase&lt;T&gt;"/> 为只读。</exception>
		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (this.isReadOnly)
				{
					ExceptionHelper.ReadOnlyCollection();
				}
				if (index < 0 || index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index");
				}
				if (value is T)
				{
					this.SetItem(index, (T)value);
				}
				else
				{
					throw ExceptionHelper.ArgumentWrongType("value", value, typeof(T));
				}
			}
		}

		/// <summary>
		/// 向 <see cref="ListBase&lt;T&gt;"/> 中添加项。
		/// </summary>
		/// <param name="value">要添加到 <see cref="ListBase&lt;T&gt;"/> 的对象。</param>
		/// <returns>新元素所插入到的位置，或为 <c>-1</c> 以指示未将该项插入到集合中。</returns>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		int IList.Add(object value)
		{
			if (this.isReadOnly)
			{
				ExceptionHelper.ReadOnlyCollection();
			}
			int idx = this.Count;
			if (value is T)
			{
				this.Insert(idx, (T)value);
			}
			else
			{
				throw ExceptionHelper.ArgumentWrongType("value", value, typeof(T));
			}
			return idx;
		}

		/// <summary>
		/// 从 <see cref="ListBase&lt;T&gt;"/> 中移除所有项。
		/// </summary>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		void IList.Clear()
		{
			if (this.isReadOnly)
			{
				ExceptionHelper.ReadOnlyCollection();
			}
			this.ClearItems();
		}

		/// <summary>
		/// 确定 <see cref="List&lt;T&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="value">要在 <see cref="List&lt;T&gt;"/> 中查找的对象。</param>
		/// <returns>如果在 <see cref="List&lt;T&gt;"/> 中找到 <paramref name="value"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		bool IList.Contains(object value)
		{
			return value is T && this.IndexOf((T)value) >= 0;
		}

		/// <summary>
		/// 确定 <see cref="ListBase&lt;T&gt;"/> 中特定项的索引。
		/// </summary>
		/// <param name="value">要在 <see cref="ListBase&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ListBase&lt;T&gt;"/> 中找到 <paramref name="value"/>，
		/// 则为该项的索引；否则为 <c>-1</c>。</returns>
		int IList.IndexOf(object value)
		{
			if (value is T)
			{
				return this.IndexOf((T)value);
			}
			return -1;
		}

		/// <summary>
		/// 在 <see cref="ListBase&lt;T&gt;"/> 中的指定索引处插入项。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="value"/>。</param>
		/// <param name="value">要插入到 <see cref="ListBase&lt;T&gt;"/> 中的对象。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase&lt;T&gt;"/> 中的有效索引。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		void IList.Insert(int index, object value)
		{
			if (this.isReadOnly)
			{
				ExceptionHelper.ReadOnlyCollection();
			}
			if (index < 0 || index >= this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			if (value is T)
			{
				this.InsertItem(index, (T)value);
			}
			else
			{
				throw ExceptionHelper.ArgumentWrongType("value", value, typeof(T));
			}
		}

		/// <summary>
		/// 从 <see cref="ListBase&lt;T&gt;"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="value">要从 <see cref="ListBase&lt;T&gt;"/> 中移除的对象。</param>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		void IList.Remove(object value)
		{
			if (this.isReadOnly)
			{
				ExceptionHelper.ReadOnlyCollection();
			}
			if (value is T)
			{
				int index = this.IndexOf((T)value);
				if (index >= 0)
				{
					this.RemoveItem(index);
				}
			}
		}

		/// <summary>
		/// 移除指定索引处的 <see cref="ListBase&lt;T&gt;"/> 项。
		/// </summary>
		/// <param name="index">从零开始的索引（属于要移除的项）。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase&lt;T&gt;"/> 中的有效索引。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="ListBase&lt;T&gt;"/> 是只读的。</exception>
		void IList.RemoveAt(int index)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			if (index < 0 || index >= this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			this.RemoveItem(index);
		}

		#endregion // IList 成员

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="CollectionBase&lt;T&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="CollectionBase&lt;T&gt;"/> 中包含的元素数。</value>
		public virtual int Count
		{
			get { return this.items.Count; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="CollectionBase&lt;T&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="CollectionBase&lt;T&gt;"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		public bool IsReadOnly
		{
			get { return this.isReadOnly; }
		}
		/// <summary>
		/// 将某元素添加到 <see cref="CollectionBase&lt;T&gt;"/> 中。
		/// </summary>
		/// <param name="item">要添加到 <see cref="CollectionBase&lt;T&gt;"/> 的元素。</param>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="CollectionBase&lt;T&gt;"/> 是只读的。</exception>
		public void Add(T item)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.Insert(this.Count, item);
		}
		/// <summary>
		/// 从 <see cref="CollectionBase&lt;T&gt;"/> 中移除所有元素。
		/// </summary>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="CollectionBase&lt;T&gt;"/> 是只读的。</exception>
		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.ClearItems();
		}

		/// <summary>
		/// 确定 <see cref="CollectionBase&lt;T&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="CollectionBase&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="CollectionBase&lt;T&gt;"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(T item)
		{
			return this.IndexOf(item) >= 0;
		}

		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将 <see cref="CollectionBase&lt;T&gt;"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="CollectionBase&lt;T&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="CollectionBase&lt;T&gt;"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			ExceptionHelper.CheckFlatArray(array, "array");
			if (arrayIndex < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("arrayIndex");
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw ExceptionHelper.ArrayTooSmall("array");
			}
			int cnt = this.Count;
			for (int i = 0; i < cnt; i++)
			{
				array[arrayIndex++] = this[i];
			}
		}

		/// <summary>
		/// 从 <see cref="CollectionBase&lt;T&gt;"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="CollectionBase&lt;T&gt;"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="CollectionBase&lt;T&gt;"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="CollectionBase&lt;T&gt;"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="CollectionBase&lt;T&gt;"/> 是只读的。</exception>
		public bool Remove(T item)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			int idx = this.IndexOf(item);
			if (idx >= 0)
			{
				this.RemoveItem(idx);
				return true;
			}
			return false;
		}

		#endregion // ICollection<T> 成员

		#region ICollection 成员

		/// <summary>
		/// 获取 <see cref="CollectionBase&lt;T&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="CollectionBase&lt;T&gt;"/> 中包含的元素数。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int ICollection.Count
		{
			get { return this.Count; }
		}

		/// <summary>
		/// 获取一个值，该值指示是否同步对 <see cref="CollectionBase&lt;T&gt;"/> 的访问（线程安全）。
		/// </summary>
		/// <value>如果对 <see cref="CollectionBase&lt;T&gt;"/> 的访问是同步的（线程安全），
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// 获取一个可用于同步对 <see cref="CollectionBase&lt;T&gt;"/> 的访问的对象。
		/// </summary>
		/// <value>可用于同步对 <see cref="CollectionBase&lt;T&gt;"/> 的访问的对象。</value>
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
		/// 从特定的 <see cref="System.Array"/> 索引处开始，将 
		/// <see cref="CollectionBase&lt;T&gt;"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="CollectionBase&lt;T&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="CollectionBase&lt;T&gt;"/> 
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 
		/// <paramref name="array"/> 末尾之间的可用空间。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="CollectionBase&lt;T&gt;"/> 
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			ExceptionHelper.CheckFlatArray(array, "array");
			if (index < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			if (array.Length - index < this.Count)
			{
				throw ExceptionHelper.ArrayTooSmall("array");
			}
			T[] arr = array as T[];
			if (arr != null)
			{
				this.CopyTo(arr, index);
			}
			else
			{
				try
				{
					foreach (T obj in this)
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

		#endregion // ICollection 成员

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public virtual IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		#endregion // IEnumerable<T> 成员

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

	}

}