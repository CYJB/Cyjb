using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示数组的一部分的列表。
	/// 与 ArraySegment&lt;T&gt; 的区别就是其访问方式与普通的列表相同。
	/// </summary>
	/// <typeparam name="T">元素的类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public sealed class ArrayAdapter<T> : IList<T>, IList
	{
		/// <summary>
		/// 被包装的数组。
		/// </summary>
		private T[] items;
		/// <summary>
		/// 获取由数组段分隔的范围中的第一个元素的位置（相对于原始数组的开始位置）。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int offset;
		/// <summary>
		/// 获取由数组段分隔的范围中的元素个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;
		/// <summary>
		/// 初始化 <see cref="Cyjb.Collections.ArrayAdapter&lt;T&gt;"/> 类的新实例。
		/// </summary>
		public ArrayAdapter()
		{
			items = new T[0];
		}
		/// <summary>
		/// 使用给定的数组初始化 <see cref="Cyjb.Collections.ArrayAdapter&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="array">初始化使用的数组。</param>
		public ArrayAdapter(T[] array)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			this.items = array;
			count = this.items.Length;
		}
		/// <summary>
		/// 使用给定的数组初始化 <see cref="Cyjb.Collections.ArrayAdapter&lt;T&gt;"/> 类的新实例。
		/// 它分割指定数组中指定的元素范围。
		/// </summary>
		/// <param name="array">初始化使用的数组。</param>
		/// <param name="offset">相应范围中第一个元素的从零开始的索引。</param>
		/// <param name="count">范围中的元素数。</param>
		public ArrayAdapter(T[] array, int offset, int count)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			if (offset < 0)
			{
				throw ExceptionHelper.ArgumentNegative("offset");
			}
			if (count < 0)
			{
				throw ExceptionHelper.ArgumentNegative("count");
			}
			if (offset + count > array.Length)
			{
				throw ExceptionHelper.InvalidOffsetLength();
			}
			this.items = array;
			this.offset = offset;
			this.count = count;
		}
		/// <summary>
		/// 获取由数组段分隔的范围中的第一个元素的位置（相对于原始数组的开始位置）。
		/// </summary>
		public int Offset { get { return offset; } }

		#region IList<T> 成员

		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		public T this[int index]
		{
			get { return items[index + offset]; }
			set { items[index + offset] = value; }
		}

		/// <summary>
		/// 确定 <see cref="ArrayAdapter&lt;T&gt;"/> 中特定项的索引。
		/// </summary>
		/// <param name="item">要在 <see cref="ArrayAdapter&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ArrayAdapter&lt;T&gt;"/> 中找到 <paramref name="item"/>，
		/// 则为该项的索引；否则为 <c>-1</c>。</returns>
		public int IndexOf(T item)
		{
			IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = offset; i < offset + count; i++)
			{
				if (comparer.Equals(items[i], item))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// 在 <see cref="ArrayAdapter&lt;T&gt;"/> 中的指定索引处插入项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入到 <see cref="ArrayAdapter&lt;T&gt;"/> 中的对象。</param>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void IList<T>.Insert(int index, T item)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		/// <summary>
		/// 移除指定索引处的 <see cref="ArrayAdapter&lt;T&gt;"/> 项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="index">从零开始的索引（属于要移除的项）。</param>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void IList<T>.RemoveAt(int index)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		#endregion // IList<T> 成员

		#region IList 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ArrayAdapter&lt;T&gt;"/> 是否具有固定大小。
		/// </summary>
		/// <value>如果 <see cref="ArrayAdapter&lt;T&gt;"/> 具有固定大小，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsFixedSize
		{
			get { return true; }
		}

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ArrayAdapter&lt;T&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ArrayAdapter&lt;T&gt;"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
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
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ArrayAdapter&lt;T&gt;"/> 中的有效索引。</exception>
		object IList.this[int index]
		{
			get { return items[index + offset]; }
			set
			{
				if (value is T)
				{
					items[index + offset] = (T)value;
				}
				else
				{
					throw ExceptionHelper.ArgumentWrongType("value", value, typeof(T));
				}
			}
		}

		/// <summary>
		/// 向 <see cref="ArrayAdapter&lt;T&gt;"/> 中添加项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="value">要添加到 <see cref="ArrayAdapter&lt;T&gt;"/> 的对象。</param>
		/// <returns>新元素所插入到的位置，或为 <c>-1</c> 以指示未将该项插入到集合中。</returns>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		int IList.Add(object value)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		/// <summary>
		/// 从 <see cref="ArrayAdapter&lt;T&gt;"/> 中移除所有项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void IList.Clear()
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		/// <summary>
		/// 确定 <see cref="ArrayAdapter&lt;T&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="value">要在 <see cref="ArrayAdapter&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ArrayAdapter&lt;T&gt;"/> 中找到 <paramref name="value"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		bool IList.Contains(object value)
		{
			if (value is T)
			{
				return this.Contains((T)value);
			}
			return false;
		}

		/// <summary>
		/// 确定 <see cref="ArrayAdapter&lt;T&gt;"/> 中特定项的索引。
		/// </summary>
		/// <param name="value">要在 <see cref="ArrayAdapter&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ArrayAdapter&lt;T&gt;"/> 中找到 <paramref name="value"/>，
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
		/// 在 <see cref="ArrayAdapter&lt;T&gt;"/> 中的指定索引处插入项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="index">应插入 <paramref name="value"/> 的位置的零始索引。</param>
		/// <param name="value">要插入 <see cref="ArrayAdapter&lt;T&gt;"/> 中的对象。</param>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void IList.Insert(int index, object value)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		/// <summary>
		/// 从 <see cref="ArrayAdapter&lt;T&gt;"/> 中移除特定对象的第一个匹配项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="value">要从 <see cref="ArrayAdapter&lt;T&gt;"/> 中移除的对象。</param>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void IList.Remove(object value)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		/// <summary>
		/// 移除指定索引处的 <see cref="ArrayAdapter&lt;T&gt;"/> 项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="index">要移除的项的从零开始的索引。</param>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void IList.RemoveAt(int index)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		#endregion // IList 成员

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="ArrayAdapter&lt;T&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="ArrayAdapter&lt;T&gt;"/> 中包含的元素数。</value>
		public int Count
		{
			get { return this.count; }
		}

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ArrayAdapter&lt;T&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ArrayAdapter&lt;T&gt;"/> 为只读，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// 将某项添加到 <see cref="ArrayAdapter&lt;T&gt;"/> 中。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要添加到 <see cref="ArrayAdapter&lt;T&gt;"/> 的对象。</param>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void ICollection<T>.Add(T item)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		/// <summary>
		/// 从 <see cref="ArrayAdapter&lt;T&gt;"/> 中移除所有项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		void ICollection<T>.Clear()
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		/// <summary>
		/// 确定 <see cref="ArrayAdapter&lt;T&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="ArrayAdapter&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果包含特定值，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(T item)
		{
			return this.IndexOf(item) >= 0;
		}
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，将 
		/// <see cref="ArrayAdapter&lt;T&gt;"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="ArrayAdapter&lt;T&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于 <paramref name="array"/> 的下限。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 
		/// <see cref="ArrayAdapter&lt;T&gt;"/> 中的元素数目大于从 
		/// <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 末尾之间的可用空间。</exception>
		/// <exception cref="System.InvalidCastException">源 
		/// <see cref="ArrayAdapter&lt;T&gt;"/> 的类型无法自动转换为目标 
		/// <paramref name="array"/> 的类型。</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this.items, offset, array, arrayIndex, count);
		}

		/// <summary>
		/// 从 <see cref="ArrayAdapter&lt;T&gt;"/> 中移除特定对象的第一个匹配项。
		/// 此实现总是引发 <see cref="System.NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要从 <see cref="ArrayAdapter&lt;T&gt;"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="ArrayAdapter&lt;T&gt;"/> 中成功移除 <paramref name="item"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。如果在原始 <see cref="ArrayAdapter&lt;T&gt;"/> 中没有找到 <paramref name="item"/>，
		/// 该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="System.NotSupportedException">总是引发。</exception>
		bool ICollection<T>.Remove(T item)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		#endregion // ICollection<T> 成员

		#region ICollection 成员

		/// <summary>
		/// 获取 <see cref="ArrayAdapter&lt;T&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="ArrayAdapter&lt;T&gt;"/> 中包含的元素数。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int ICollection.Count
		{
			get { return this.count; }
		}

		/// <summary>
		/// 获取一个值，该值指示是否同步对 <see cref="ArrayAdapter&lt;T&gt;"/> 的访问（线程安全）。
		/// </summary>
		/// <value>如果对 <see cref="ArrayAdapter&lt;T&gt;"/> 的访问是同步的（线程安全），则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized
		{
			get { return this.items.IsSynchronized; }
		}

		/// <summary>
		/// 获取一个可用于同步对 <see cref="ArrayAdapter&lt;T&gt;"/> 的访问的对象。
		/// </summary>
		/// <value>可用于同步对 <see cref="ArrayAdapter&lt;T&gt;"/> 的访问的对象。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot
		{
			get { return this.items.SyncRoot; }
		}
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，将 
		/// <see cref="ArrayAdapter&lt;T&gt;"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="ArrayAdapter&lt;T&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 小于 <paramref name="array"/> 的下限。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 
		/// <see cref="ArrayAdapter&lt;T&gt;"/> 中的元素数目大于从 
		/// <paramref name="index"/> 到目标 <paramref name="array"/> 末尾之间的可用空间。</exception>
		/// <exception cref="System.InvalidCastException">源 
		/// <see cref="ArrayAdapter&lt;T&gt;"/> 的类型无法自动转换为目标 
		/// <paramref name="array"/> 的类型。</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			Array.Copy(this.items, offset, array, index, count);
		}

		#endregion // ICollection 成员

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public IEnumerator<T> GetEnumerator()
		{
			for (int i = offset; i < offset + count; i++)
			{
				yield return this.items[i];
			}
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
