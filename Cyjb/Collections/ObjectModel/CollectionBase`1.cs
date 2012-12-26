using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{

	/// <summary>
	/// 为泛型集合提供基类。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class CollectionBase<T> : ICollection<T>, ICollection
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
		/// 周围的 <see cref="ICollection&lt;T&gt;"/> 包装。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ICollection<T> items;
		/// <summary>
		/// 初始化 <see cref="CollectionBase&lt;T&gt;"/> 类的新实例。
		/// </summary>
		protected CollectionBase()
		{
			this.items = new List<T>();
		}
		/// <summary>
		/// 初始化 <see cref="CollectionBase&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="isReadOnly">集合是否是只读的。</param>
		protected CollectionBase(bool isReadOnly)
		{
			this.items = new List<T>();
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 将 <see cref="CollectionBase&lt;T&gt;"/> 类的新实例初始化为指定集合的包装。
		/// </summary>
		/// <param name="collection">由新的集合包装的集合。</param>
		protected CollectionBase(ICollection<T> collection)
		{
			this.items = collection;
			this.isReadOnly = items.IsReadOnly;
		}
		/// <summary>
		/// 将 <see cref="CollectionBase&lt;T&gt;"/> 类的新实例初始化为指定集合的包装。
		/// </summary>
		/// <param name="collection">由新的集合包装的集合。</param>
		/// <param name="isReadOnly">集合的包装是否是只读的。</param>
		protected CollectionBase(ICollection<T> collection, bool isReadOnly)
		{
			this.items = collection;
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 获取 <see cref="CollectionBase&lt;T&gt;"/> 周围的 <see cref="ICollection&lt;T&gt;"/> 包装。
		/// </summary>
		protected ICollection<T> Items
		{
			get { return items; }
		}
		/// <summary>
		/// 从 <see cref="CollectionBase&lt;T&gt;"/> 中移除所有元素。
		/// </summary>
		protected virtual void ClearItems()
		{
			this.items.Clear();
		}
		/// <summary>
		/// 将元素添加到 <see cref="CollectionBase&lt;T&gt;"/> 中。
		/// </summary>
		/// <param name="item">要添加到 <see cref="CollectionBase&lt;T&gt;"/> 的中的对象。
		/// 对于引用类型，该值可以为 <c>null</c>。</param>
		protected virtual void AddItem(T item)
		{
			this.items.Add(item);
		}
		/// <summary>
		/// 移除 <see cref="CollectionBase&lt;T&gt;"/> 的指定元素。
		/// </summary>
		/// <param name="item">要移除的元素。</param>
		/// <returns>如果已从 <see cref="CollectionBase&lt;T&gt;"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="CollectionBase&lt;T&gt;"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		protected virtual bool RemoveItem(T item)
		{
			return this.items.Remove(item);
		}

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
			this.AddItem(item);
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
		public virtual bool Contains(T item)
		{
			return this.items.Contains(item);
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
			ExceptionHelper.CheckFlatArray(array);
			if (arrayIndex < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("arrayIndex");
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw ExceptionHelper.ArrayTooSmall();
			}
			foreach (T obj in this)
			{
				array.SetValue(obj, arrayIndex++);
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
			return this.RemoveItem(item);
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
			ExceptionHelper.CheckFlatArray(array);
			if (index < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			if (array.Length - index < this.Count)
			{
				throw ExceptionHelper.ArrayTooSmall();
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
				catch (ArrayTypeMismatchException ex)
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
