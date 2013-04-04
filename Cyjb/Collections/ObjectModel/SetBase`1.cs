using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{

	/// <summary>
	/// 为泛型集提供基类。
	/// </summary>
	/// <typeparam name="T">集中的元素类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class SetBase<T> : ISet<T>, ICollection
	{
		/// <summary>
		/// 提供同步的对象。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object syncRoot;
		/// <summary>
		/// 集是否是只读的。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool isReadOnly;
		/// <summary>
		/// 周围的 <see cref="ISet&lt;T&gt;"/> 包装。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ISet<T> items;
		/// <summary>
		/// 初始化 <see cref="SetBase&lt;T&gt;"/> 类的新实例。
		/// </summary>
		protected SetBase()
		{
			this.items = new HashSet<T>();
		}
		/// <summary>
		/// 初始化 <see cref="SetBase&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="isReadOnly">集是否是只读的。</param>
		protected SetBase(bool isReadOnly)
		{
			this.items = new HashSet<T>();
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 将 <see cref="SetBase&lt;T&gt;"/> 类的新实例初始化为指定集的包装。
		/// </summary>
		/// <param name="set">由新的集包装的集。</param>
		protected SetBase(ISet<T> set)
		{
			this.items = set;
			if (this.items != null)
			{
				this.isReadOnly = this.items.IsReadOnly;
			}
		}
		/// <summary>
		/// 将 <see cref="SetBase&lt;T&gt;"/> 类的新实例初始化为指定集的包装。
		/// </summary>
		/// <param name="set">由新的集包装的集。</param>
		/// <param name="isReadOnly">集的包装是否是只读的。</param>
		protected SetBase(ISet<T> set, bool isReadOnly)
		{
			this.items = set;
			this.isReadOnly = isReadOnly;
		}
		/// <summary>
		/// 获取 <see cref="SetBase&lt;T&gt;"/> 周围的 <see cref="ISet&lt;T&gt;"/> 包装。
		/// </summary>
		protected ISet<T> Items
		{
			get { return items; }
		}
		/// <summary>
		/// 从 <see cref="SetBase&lt;T&gt;"/> 中移除所有元素。
		/// </summary>
		protected virtual void ClearItems()
		{
			this.items.Clear();
		}
		/// <summary>
		/// 向当前集内添加元素，并返回一个指示是否已成功添加元素的值。
		/// </summary>
		/// <param name="item">要添加到 <see cref="SetBase&lt;T&gt;"/> 的中的对象。
		/// 对于引用类型，该值可以为 <c>null</c>。</param>
		/// <returns>如果该元素已添加到集内，则为 <c>true</c>；
		/// 如果该元素已在集内，则为 <c>false</c>。</returns>
		protected virtual bool AddItem(T item)
		{
			return this.items.Add(item);
		}
		/// <summary>
		/// 移除 <see cref="SetBase&lt;T&gt;"/> 的指定元素。
		/// </summary>
		/// <param name="item">要移除的元素。</param>
		/// <returns>如果已从 <see cref="SetBase&lt;T&gt;"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="SetBase&lt;T&gt;"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		protected virtual bool RemoveItem(T item)
		{
			return this.items.Remove(item);
		}

		#region ISet<T> 成员

		/// <summary>
		/// 向当前集内添加元素，并返回一个指示是否已成功添加元素的值。
		/// </summary>
		/// <param name="item">要添加到集内的元素。</param>
		/// <returns>如果该元素已添加到集内，则为 <c>true</c>；
		/// 如果该元素已在集内，则为 <c>false</c>。</returns>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
		public bool Add(T item)
		{
			if (this.IsReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			return this.AddItem(item);
		}
		/// <summary>
		/// 从当前集内移除指定集合中的所有元素。
		/// </summary>
		/// <param name="other">要从集内移除的项的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
		public virtual void ExceptWith(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.IsReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.items.ExceptWith(other);
		}
		/// <summary>
		/// 修改当前集，使该集仅包含指定集合中也存在的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
		public virtual void IntersectWith(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.IsReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.items.IntersectWith(other);
		}
		/// <summary>
		/// 确定当前集是否为指定集合的真子集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的真子集，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsProperSubsetOf(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			return this.items.IsProperSubsetOf(other);
		}
		/// <summary>
		/// 确定当前集是否为指定集合的真超集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的真超集，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsProperSupersetOf(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			return this.items.IsProperSubsetOf(other);
		}
		/// <summary>
		/// 确定当前集是否为指定集合的子集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的子集，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsSubsetOf(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			return this.items.IsSubsetOf(other);
		}
		/// <summary>
		/// 确定当前集是否为指定集合的超集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的超集，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsSupersetOf(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			return this.items.IsSupersetOf(other);
		}
		/// <summary>
		/// 确定当前集是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool Overlaps(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			return this.items.Overlaps(other);
		}
		/// <summary>
		/// 确定当前集与指定的集合中是否包含相同的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集等于 <paramref name="other"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool SetEquals(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			return this.items.SetEquals(other);
		}
		/// <summary>
		/// 修改当前集，使该集仅包含当前集或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
		public virtual void SymmetricExceptWith(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.IsReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.items.SymmetricExceptWith(other);
		}
		/// <summary>
		/// 修改当前集，使该集包含当前集和指定集合中同时存在的所有元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
		public virtual void UnionWith(IEnumerable<T> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.items.UnionWith(other);
		}

		#endregion

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="SetBase&lt;T&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="SetBase&lt;T&gt;"/> 中包含的元素数。</value>
		public virtual int Count
		{
			get { return this.items.Count; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="SetBase&lt;T&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="SetBase&lt;T&gt;"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		public bool IsReadOnly
		{
			get { return this.isReadOnly; }
		}
		/// <summary>
		/// 将某元素添加到 <see cref="SetBase&lt;T&gt;"/> 中。
		/// </summary>
		/// <param name="item">要添加到 <see cref="SetBase&lt;T&gt;"/> 的元素。</param>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
		void ICollection<T>.Add(T item)
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.AddItem(item);
		}
		/// <summary>
		/// 从 <see cref="SetBase&lt;T&gt;"/> 中移除所有元素。
		/// </summary>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw ExceptionHelper.ReadOnlyCollection();
			}
			this.ClearItems();
		}

		/// <summary>
		/// 确定 <see cref="SetBase&lt;T&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="SetBase&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="SetBase&lt;T&gt;"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public virtual bool Contains(T item)
		{
			return this.items.Contains(item);
		}

		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将 <see cref="SetBase&lt;T&gt;"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="SetBase&lt;T&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="SetBase&lt;T&gt;"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			this.items.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// 从 <see cref="SetBase&lt;T&gt;"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="SetBase&lt;T&gt;"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="SetBase&lt;T&gt;"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="SetBase&lt;T&gt;"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="System.NotSupportedException">
		/// <see cref="SetBase&lt;T&gt;"/> 是只读的。</exception>
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
		/// 获取 <see cref="SetBase&lt;T&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="SetBase&lt;T&gt;"/> 中包含的元素数。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int ICollection.Count
		{
			get { return this.Count; }
		}

		/// <summary>
		/// 获取一个值，该值指示是否同步对 <see cref="SetBase&lt;T&gt;"/> 的访问（线程安全）。
		/// </summary>
		/// <value>如果对 <see cref="SetBase&lt;T&gt;"/> 的访问是同步的（线程安全），
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// 获取一个可用于同步对 <see cref="SetBase&lt;T&gt;"/> 的访问的对象。
		/// </summary>
		/// <value>可用于同步对 <see cref="SetBase&lt;T&gt;"/> 的访问的对象。</value>
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
		/// <see cref="SetBase&lt;T&gt;"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="SetBase&lt;T&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="SetBase&lt;T&gt;"/> 
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 
		/// <paramref name="array"/> 末尾之间的可用空间。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="SetBase&lt;T&gt;"/> 
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			CollectionHelper.CopyTo(this, array, index);
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
