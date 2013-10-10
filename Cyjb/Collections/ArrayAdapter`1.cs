using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示数组的一部分的列表。与 <see cref="System.ArraySegment&lt;T&gt;"/> 
	/// 的区别就是 <see cref="ArrayAdapter&lt;T&gt;"/> 实现了 <see cref="IList&lt;T&gt;"/> 接口，
	/// 访问方式与普通的列表相同。
	/// </summary>
	/// <typeparam name="T">元素的类型。</typeparam>
	public sealed class ArrayAdapter<T> : ListBase<T>, IList
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
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="Cyjb.Collections.ArrayAdapter&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public ArrayAdapter()
		{
			items = new T[0];
		}
		/// <summary>
		/// 使用给定的数组初始化 <see cref="Cyjb.Collections.ArrayAdapter&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="array">初始化使用的数组。</param>
		public ArrayAdapter(params T[] array)
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
		/// <value>由数组段分隔的范围中的第一个元素的位置（相对于原始数组的开始位置）。</value>
		public int Offset { get { return offset; } }

		#region ListBase<T> 成员

		/// <summary>
		/// 从 <see cref="ListBase&lt;T&gt;"/> 中移除所有元素。
		/// </summary>
		protected override void ClearItems()
		{
			throw ExceptionHelper.FixedSizeCollection();
		}
		/// <summary>
		/// 将元素插入 <see cref="ListBase&lt;T&gt;"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入的对象。对于引用类型，该值可以为 <c>null</c>。</param>
		protected override void InsertItem(int index, T item)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}
		/// <summary>
		/// 移除 <see cref="ListBase&lt;T&gt;"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}
		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。对于引用类型，该值可以为 <c>null</c>。</param>
		protected override void SetItem(int index, T item)
		{
			this.items[index] = item;
		}
		/// <summary>
		/// 返回指定索引处的元素。
		/// </summary>
		/// <param name="index">要返回元素的从零开始的索引。</param>
		/// <returns>位于指定索引处的元素。</returns>
		protected override T GetItem(int index)
		{
			return this.items[index + offset];
		}

		#endregion // ListBase<T> 成员

		#region IList<T> 成员

		/// <summary>
		/// 确定 <see cref="ArrayAdapter&lt;T&gt;"/> 中特定项的索引。
		/// </summary>
		/// <param name="item">要在 <see cref="ArrayAdapter&lt;T&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ArrayAdapter&lt;T&gt;"/> 中找到 <paramref name="item"/>，
		/// 则为该项的索引；否则为 <c>-1</c>。</returns>
		public override int IndexOf(T item)
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

		#endregion // IList 成员

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="ArrayAdapter&lt;T&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="ArrayAdapter&lt;T&gt;"/> 中包含的元素数。</value>
		public override int Count
		{
			get { return this.count; }
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
		public override void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this.items, offset, array, arrayIndex, count);
		}

		#endregion // ICollection<T> 成员

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public override IEnumerator<T> GetEnumerator()
		{
			for (int i = offset; i < offset + count; i++)
			{
				yield return this.items[i];
			}
		}

		#endregion // IEnumerable<T> 成员

	}
}
