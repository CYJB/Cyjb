using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 为泛型列表提供基类。
	/// </summary>
	/// <typeparam name="T">列表中的元素类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class ListBase<T> : IList<T>, IList
	{
		/// <summary>
		/// 用于同步列表访问的对象。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object syncRoot;
		/// <summary>
		/// 被包装的内部 <see cref="IList{T}"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IList<T> items;
		/// <summary>
		/// 初始化 <see cref="ListBase{T}"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="ListBase{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		protected ListBase()
			: this(new List<T>())
		{ }
		/// <summary>
		/// 将 <see cref="ListBase{T}"/> 类的新实例初始化为指定数据的包装。
		/// </summary>
		/// <param name="list">由新的列表包装的集合。</param>
		/// <remarks>如果 <paramref name="list"/> 实现了 <see cref="IList{T}"/>
		/// 接口，则使用 <paramref name="list"/> 作为内部集合；否则使用 
		/// <see cref="List{T}"/> 作为内部集合。</remarks>
		protected ListBase(IEnumerable<T> list)
		{
			if (list != null)
			{
				this.items = list as IList<T> ?? new List<T>(list);
			}
		}
		/// <summary>
		/// 获取被 <see cref="ListBase{T}"/> 包装的内部 <see cref="IList{T}"/>。
		/// </summary>
		/// <value>被 <see cref="ListBase{T}"/> 包装的内部 <see cref="IList{T}"/>。</value>
		protected IList<T> Items
		{
			get { return items; }
		}

		#region ListBase<T> 成员

		/// <summary>
		/// 将元素插入 <see cref="ListBase{T}"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入的对象。</param>
		protected virtual void InsertItem(int index, T item)
		{
			Contract.Requires(index >= 0 && index <= this.Count);
			this.items.Insert(index, item);
		}
		/// <summary>
		/// 移除 <see cref="ListBase{T}"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected virtual void RemoveItem(int index)
		{
			Contract.Requires(index >= 0 && index < this.Count);
			this.items.RemoveAt(index);
		}
		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。</param>
		protected virtual void SetItemAt(int index, T item)
		{
			Contract.Requires(index >= 0 && index < this.Count);
			this.items[index] = item;
		}
		/// <summary>
		/// 返回指定索引处的元素。
		/// </summary>
		/// <param name="index">要返回元素的从零开始的索引。</param>
		/// <returns>位于指定索引处的元素。</returns>
		[Pure]
		protected virtual T GetItemAt(int index)
		{
			Contract.Requires(index >= 0 && index < this.Count);
			return this.items[index];
		}

		#endregion // ListBase<T> 成员

		#region IList<T> 成员

		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase{T}"/> 中的有效索引。</exception>
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index");
				}
				Contract.EndContractBlock();
				return GetItemAt(index);
			}
			set
			{
				if (index < 0 || index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index");
				}
				Contract.EndContractBlock();
				this.SetItemAt(index, value);
			}
		}
		/// <summary>
		/// 确定 <see cref="ListBase{T}"/> 中指定对象的索引。
		/// </summary>
		/// <param name="item">要在 <see cref="ListBase{T}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ListBase{T}"/> 中找到 <paramref name="item"/>，
		/// 则为该对象的索引；否则为 <c>-1</c>。</returns>
		public virtual int IndexOf(T item)
		{
			return this.items.IndexOf(item);
		}
		/// <summary>
		/// 将元素插入 <see cref="ListBase{T}"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入到 <see cref="ListBase{T}"/> 中的对象。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase{T}"/> 中的有效索引。</exception>
		public void Insert(int index, T item)
		{
			if (index < 0 || index > this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			Contract.EndContractBlock();
			this.InsertItem(index, item);
		}
		/// <summary>
		/// 移除 <see cref="ListBase{T}"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase{T}"/> 中的有效索引。</exception>
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			Contract.EndContractBlock();
			this.RemoveItem(index);
		}

		#endregion // IList<T> 成员

		#region IList 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ListBase{T}"/> 是否具有固定大小。
		/// </summary>
		/// <value>如果 <see cref="ListBase{T}"/> 具有固定大小，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsFixedSize
		{
			get { return false; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="ListBase{T}"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ListBase{T}"/> 为只读，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsReadOnly
		{
			get { return false; }
		}
		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获得或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase{T}"/> 中的有效索引。</exception>
		/// <exception cref="ArgumentException">设置属性，且 <paramref name="value"/> 不能赋值给
		/// <typeparamref name="T"/> 类型。</exception>
		object IList.this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index");
				}
				Contract.EndContractBlock();
				return this.GetItemAt(index);
			}
			set
			{
				if (index < 0 || index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index");
				}
				Contract.EndContractBlock();
				try
				{
					this.SetItemAt(index, (T)value);
				}
				catch (InvalidCastException)
				{
					throw ExceptionHelper.ArgumentWrongType("value", value, typeof(T));
				}
			}
		}
		/// <summary>
		/// 将指定对象添加到 <see cref="ListBase{T}"/> 的末尾。
		/// </summary>
		/// <param name="value">要添加到 <see cref="ListBase{T}"/> 的末尾的对象。</param>
		/// <returns>新元素所插入到的位置，或为 <c>-1</c> 以指示未将该项插入到集合中。</returns>
		/// <exception cref="ArgumentException"><paramref name="value"/> 不能赋值给
		/// <typeparamref name="T"/> 类型。</exception>
		int IList.Add(object value)
		{
			Contract.Ensures(Contract.Result<int>() >= -1 &&
				Contract.Result<int>() <= Contract.OldValue(Count));
			try
			{
				int idx = this.Count;
				this.InsertItem(idx, (T)value);
				return idx;
			}
			catch (InvalidCastException)
			{
				throw ExceptionHelper.ArgumentWrongType("value", value, typeof(T));
			}
		}
		/// <summary>
		/// 确定 <see cref="ListBase{T}"/> 是否包含指定对象。
		/// </summary>
		/// <param name="value">要在 <see cref="ListBase{T}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ListBase{T}"/> 中找到 <paramref name="value"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		bool IList.Contains(object value)
		{
			return CollectionHelper.IsCompatible<T>(value) && this.Contains((T)value);
		}
		/// <summary>
		/// 确定 <see cref="ListBase{T}"/> 中指定对象的索引。
		/// </summary>
		/// <param name="value">要在 <see cref="ListBase{T}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ListBase{T}"/> 中找到 <paramref name="value"/>，
		/// 则为该对象的索引；否则为 <c>-1</c>。</returns>
		int IList.IndexOf(object value)
		{
			if (CollectionHelper.IsCompatible<T>(value))
			{
				return this.IndexOf((T)value);
			}
			return -1;
		}
		/// <summary>
		/// 将对象插入 <see cref="ListBase{T}"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="value"/>。</param>
		/// <param name="value">要插入到 <see cref="ListBase{T}"/> 中的对象。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase{T}"/> 中的有效索引。</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> 不能赋值给
		/// <typeparamref name="T"/> 类型。</exception>
		void IList.Insert(int index, object value)
		{
			if (index < 0 || index >= this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			Contract.EndContractBlock();
			try
			{
				this.InsertItem(index, (T)value);
			}
			catch (InvalidCastException)
			{
				throw ExceptionHelper.ArgumentWrongType("value", value, typeof(T));
			}
		}
		/// <summary>
		/// 从 <see cref="ListBase{T}"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="value">要从 <see cref="ListBase{T}"/> 中移除的对象。</param>
		void IList.Remove(object value)
		{
			if (CollectionHelper.IsCompatible<T>(value))
			{
				this.Remove((T)value);
			}
		}
		/// <summary>
		/// 移除 <see cref="ListBase{T}"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 不是 <see cref="ListBase{T}"/> 中的有效索引。</exception>
		void IList.RemoveAt(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			Contract.EndContractBlock();
			this.RemoveItem(index);
		}

		#endregion // IList 成员

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="ListBase{T}"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="ListBase{T}"/> 中包含的元素数。</value>
		public virtual int Count
		{
			get { return this.items.Count; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="ListBase{T}"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ListBase{T}"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}
		/// <summary>
		/// 将指定对象添加到 <see cref="ListBase{T}"/> 的末尾。
		/// </summary>
		/// <param name="item">要添加到 <see cref="ListBase{T}"/> 的末尾的对象。</param>
		public void Add(T item)
		{
			this.InsertItem(this.Count, item);
		}
		/// <summary>
		/// 从 <see cref="ListBase{T}"/> 中移除所有元素。
		/// </summary>
		public virtual void Clear()
		{
			this.items.Clear();
		}
		/// <summary>
		/// 确定 <see cref="ListBase{T}"/> 是否包含指定对象。
		/// </summary>
		/// <param name="item">要在 <see cref="ListBase{T}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ListBase{T}"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public virtual bool Contains(T item)
		{
			return this.IndexOf(item) >= 0;
		}
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将 <see cref="ListBase{T}"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ListBase{T}"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException"><see cref="ListBase{T}"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			CollectionHelper.CopyTo(this, array, arrayIndex);
		}
		/// <summary>
		/// 从 <see cref="ListBase{T}"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="ListBase{T}"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="ListBase{T}"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="ListBase{T}"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		public virtual bool Remove(T item)
		{
			int idx = this.IndexOf(item);
			if (idx < 0)
			{
				return false;
			}
			this.RemoveItem(idx);
			return true;
		}

		#endregion // ICollection<T> 成员

		#region ICollection 成员

		/// <summary>
		/// 获取一个值，该值指示是否同步对 <see cref="ListBase{T}"/> 的访问（线程安全）。
		/// </summary>
		/// <value>如果对 <see cref="ListBase{T}"/> 的访问是同步的（线程安全），
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}
		/// <summary>
		/// 获取一个可用于同步对 <see cref="ListBase{T}"/> 的访问的对象。
		/// </summary>
		/// <value>可用于同步对 <see cref="ListBase{T}"/> 的访问的对象。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot
		{
			get
			{
				if (this.syncRoot == null)
				{
					ICollection collection = this.items as ICollection;
					object syncObj = collection == null ? new object() : collection.SyncRoot;
					Interlocked.CompareExchange(ref this.syncRoot, syncObj, null);
				}
				return this.syncRoot;
			}
		}
		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将 <see cref="ListBase{T}"/> 
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ListBase{T}"/> 复制的元素的目标位置的一维 
		/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><see cref="ListBase{T}"/> 
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException">源 <see cref="ListBase{T}"/> 
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
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public virtual IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		#endregion // IEnumerable<T> 成员

		#region IEnumerable 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion // IEnumerable 成员

	}
}