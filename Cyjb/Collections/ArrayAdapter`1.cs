using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示数组的一部分的列表。与 <see cref="ArraySegment{T}"/> 
	/// 的区别就是 <see cref="ArrayAdapter{T}"/> 实现了 <see cref="IList{T}"/> 接口，
	/// 访问方式与普通的列表相同。
	/// </summary>
	/// <typeparam name="T">元素的类型。</typeparam>
	public sealed class ArrayAdapter<T> : ListBase<T>, IList
	{
		/// <summary>
		/// 原数组。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly T[] array;
		/// <summary>
		/// 范围中的第一个元素的位置。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int offset;
		/// <summary>
		/// 范围中的元素个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int count;
		/// <summary>
		/// 使用给定的数组初始化 <see cref="Cyjb.Collections.ArrayAdapter{T}"/> 类的新实例。
		/// </summary>
		/// <param name="array">初始化使用的数组。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="Cyjb.Collections.ArrayAdapter{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public ArrayAdapter(params T[] array)
			: base(array)
		{
			if (array == null)
			{
				throw ExceptionHelper.ArgumentNull("array");
			}
			Contract.EndContractBlock();
			this.array = array;
			this.count = array.Length;
		}
		/// <summary>
		/// 使用给定的数组初始化 <see cref="Cyjb.Collections.ArrayAdapter{T}"/> 类的新实例。
		/// 它分割指定数组中指定的元素范围。
		/// </summary>
		/// <param name="array">初始化使用的数组。</param>
		/// <param name="offset">相应范围中第一个元素的从零开始的索引。</param>
		/// <param name="count">范围中的元素数。</param>
		public ArrayAdapter(T[] array, int offset, int count)
			: base(array)
		{
			if (array == null)
			{
				throw ExceptionHelper.ArgumentNull("array");
			}
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
			Contract.EndContractBlock();
			this.array = array;
			this.offset = offset;
			this.count = count;
		}
		/// <summary>
		/// 获取由数组段分隔的范围中的第一个元素的位置（相对于原始数组的开始位置）。
		/// </summary>
		/// <value>由数组段分隔的范围中的第一个元素的位置（相对于原始数组的开始位置）。</value>
		public int Offset
		{
			get
			{
				Contract.Ensures(Contract.Result<int>() >= 0);
				return offset;
			}
		}

		#region ListBase<T> 成员

		/// <summary>
		/// 将元素插入 <see cref="ArrayAdapter{T}"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, T item)
		{
			throw ExceptionHelper.FixedSizeCollection();
		}
		/// <summary>
		/// 移除 <see cref="ArrayAdapter{T}"/> 的指定索引处的元素。
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
		protected override void SetItemAt(int index, T item)
		{
			this.array[index] = item;
		}
		/// <summary>
		/// 返回指定索引处的元素。
		/// </summary>
		/// <param name="index">要返回元素的从零开始的索引。</param>
		/// <returns>位于指定索引处的元素。</returns>
		protected override T GetItemAt(int index)
		{
			return this.array[index + offset];
		}

		#endregion // ListBase<T> 成员

		#region IList<T> 成员

		/// <summary>
		/// 确定 <see cref="ArrayAdapter{T}"/> 中指定对象的索引。
		/// </summary>
		/// <param name="item">要在 <see cref="ArrayAdapter{T}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ArrayAdapter{T}"/> 中找到 <paramref name="item"/>，
		/// 则为该对象的索引；否则为 <c>-1</c>。</returns>
		public override int IndexOf(T item)
		{
			return Array.IndexOf(this.array, item, this.offset, this.count);
		}

		#endregion // IList<T> 成员

		#region IList 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ArrayAdapter{T}"/> 是否具有固定大小。
		/// </summary>
		/// <value>如果 <see cref="ArrayAdapter{T}"/> 具有固定大小，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsFixedSize
		{
			get { return true; }
		}

		#endregion // IList 成员

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="ArrayAdapter{T}"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="ArrayAdapter{T}"/> 中包含的元素数。</value>
		public override int Count
		{
			get { return this.count; }
		}
		/// <summary>
		/// 从 <see cref="ArrayAdapter{T}"/> 中移除所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		public override void Clear()
		{
			throw ExceptionHelper.FixedSizeCollection();
		}

		#endregion // ICollection<T> 成员

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator{T}"/>。</returns>
		public override IEnumerator<T> GetEnumerator()
		{
			int len = offset + count;
			for (int i = offset; i < len; i++)
			{
				yield return this.array[i];
			}
		}

		#endregion // IEnumerable<T> 成员

	}
}
