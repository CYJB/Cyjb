using System;
using System.Collections.Generic;

namespace Cyjb.Collections
{
	/// <summary>
	/// 提供对 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 的扩展方法。
	/// </summary>
	public static class ListExt
	{

		#region 二分查找

		/// <summary>
		/// 使用默认的比较器，在整个排序 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 
		/// 中搜索特定元素。
		/// </summary>
		/// <typeparam name="T">列表元素的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<T>(this IList<T> list, T value)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			return BinarySearch<T>(list, 0, list.Count, value, null);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，
		/// 在整个排序 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="T">列表元素的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<T>(this IList<T> list, T value, IComparer<T> comparer)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			return BinarySearch<T>(list, 0, list.Count, value, null);
		}
		/// <summary>
		/// 使用默认的比较器，在排序 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 
		/// 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="T">列表元素的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<T>(this IList<T> list, int index, int length, T value)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			return BinarySearch<T>(list, index, length, value, null);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，
		/// 在排序 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="T">列表元素的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<T>(this IList<T> list, int index, int length, T value, IComparer<T> comparer)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			try
			{
				if (comparer == null)
				{
					comparer = Comparer<T>.Default;
				}
				int low = index;
				int high = (index + length) - 1;
				while (low <= high)
				{
					int mid = low + ((high - low) >> 1);
					int cmp = comparer.Compare(list[mid], value);
					if (cmp == 0)
					{
						return mid;
					}
					else if (cmp < 0)
					{
						low = mid + 1;
					}
					else
					{
						high = mid - 1;
					}
				}
				return ~low;
			}
			catch (Exception ex)
			{
				throw ExceptionHelper.IComparerFailed(ex);
			}
		}
		/// <summary>
		/// 使用默认的比较器，在整个排序 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 中搜索特定元素。
		/// <typeparam name="TElement">列表元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// </summary>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this IList<TElement> list, TKey value,
			Func<TElement, TKey> keySelector)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			return BinarySearch(list, 0, list.Count, value, keySelector, null);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，
		/// 在整个排序 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="TElement">列表元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this IList<TElement> list, TKey value,
			Func<TElement, TKey> keySelector, IComparer<TKey> comparer)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			return BinarySearch(list, 0, list.Count, value, keySelector, comparer);
		}
		/// <summary>
		/// 使用默认的比较器，在排序 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 
		/// 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="TElement">列表元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this IList<TElement> list, int index, int length,
			TKey value, Func<TElement, TKey> keySelector)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			return BinarySearch(list, index, length, value, keySelector, null);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，在排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 的某个元素范围中搜索键值。
		/// </summary>
		/// <typeparam name="TElement">列表元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 
		/// <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this IList<TElement> list, int index, int length,
			TKey value, Func<TElement, TKey> keySelector, IComparer<TKey> comparer)
		{
			ExceptionHelper.CheckArgumentNull(list, "list");
			try
			{
				if (comparer == null)
				{
					comparer = Comparer<TKey>.Default;
				}
				int low = index;
				int high = (index + length) - 1;
				while (low <= high)
				{
					int mid = low + ((high - low) >> 1);
					int cmp = comparer.Compare(keySelector(list[mid]), value);
					if (cmp == 0)
					{
						return mid;
					}
					else if (cmp < 0)
					{
						low = mid + 1;
					}
					else
					{
						high = mid - 1;
					}
				}
				return ~low;
			}
			catch (Exception ex)
			{
				throw ExceptionHelper.IComparerFailed(ex);
			}
		}

		#endregion // 二分查找

	}
}
