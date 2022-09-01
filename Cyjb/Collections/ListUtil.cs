namespace Cyjb.Collections
{
	/// <summary>
	/// 提供对 <see cref="IList{T}"/> 的扩展方法。
	/// </summary>
	public static class ListUtil
	{

		/// <summary>
		/// 将指定集合的元素插入到当前列表的指定索引处。
		/// </summary>
		/// <typeparam name="T">列表中元素的类型。</typeparam>
		/// <param name="list">要插入到的列表。</param>
		/// <param name="index">从零开始的索引，在此处开始插入新元素。</param>
		/// <param name="collection">要插入的元素集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是 
		/// <paramref name="list"/> 中的有效索引。</exception>
		public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> collection)
		{
			ArgumentNullException.ThrowIfNull(list);
			ArgumentNullException.ThrowIfNull(collection);
			if (index < 0 || index > list.Count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			foreach (T item in collection)
			{
				list.Insert(index++, item);
			}
		}

		/// <summary>
		/// 随机打乱当前列表中元素的顺序。
		/// </summary>
		/// <typeparam name="T"><paramref name="list"/> 中的元素的类型。</typeparam>
		/// <param name="list">要随机打乱顺序的列表。</param>
		/// <returns>已完成随机排序的列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		public static IList<T> Suffle<T>(this IList<T> list)
		{
			ArgumentNullException.ThrowIfNull(list);
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = Random.Shared.Next(i + 1);
				if (j != i)
				{
					T temp = list[i];
					list[i] = list[j];
					list[j] = temp;
				}
			}
			return list;
		}

		/// <summary>
		/// 将当前列表中的元素变换为下一排列。
		/// </summary>
		/// <typeparam name="T"><paramref name="list"/> 中的元素的类型。</typeparam>
		/// <param name="list">要变换为下一排列的列表。</param>
		/// <param name="comparer">列表元素的比较器。</param>
		/// <returns>如果排列存在，则为 <c>true</c>；否则变换为首个排列并返回 <c>false</c>。</returns>
		public static bool NextPermutation<T>(this IList<T> list, Comparer<T>? comparer = null)
		{
			int count = list.Count;
			if (count <= 1)
			{
				return false;
			}
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			int cur = count - 1;
			while (true)
			{
				int idx1 = cur;
				cur--;
				if (comparer.Compare(list[cur], list[idx1]) < 0)
				{
					int idx2 = count - 1;
					for (; comparer.Compare(list[cur], list[idx2]) >= 0; idx2--) ;
					T temp = list[cur];
					list[cur] = list[idx2];
					list[idx2] = temp;
					Reverse(list, idx1, count - idx1);
					return true;
				}
				if (cur == 0)
				{
					Reverse(list, 0, count);
					return false;
				}
			}
		}

		/// <summary>
		/// 将当前列表中的元素变换为上一排列。
		/// </summary>
		/// <typeparam name="T"><paramref name="list"/> 中的元素的类型。</typeparam>
		/// <param name="list">要变换为上一排列的列表。</param>
		/// <param name="comparer">列表元素的比较器。</param>
		/// <returns>如果排列存在，则为 <c>true</c>；否则变换为末排列并返回 <c>false</c>。</returns>
		public static bool PrevPermutation<T>(this IList<T> list, Comparer<T>? comparer = null)
		{
			int count = list.Count;
			if (count <= 1)
			{
				return false;
			}
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			int cur = count - 1;
			while (true)
			{
				int idx1 = cur;
				cur--;
				if (comparer.Compare(list[idx1], list[cur]) < 0)
				{
					int idx2 = count - 1;
					for (; comparer.Compare(list[idx2], list[cur]) >= 0; idx2--) ;
					T temp = list[cur];
					list[cur] = list[idx2];
					list[idx2] = temp;
					Reverse(list, idx1, count - idx1);
					return true;
				}
				if (cur == 0)
				{
					Reverse(list, 0, count);
					return false;
				}
			}
		}

		#region 翻转

		/// <summary>
		/// 翻转指定的列表。
		/// </summary>
		/// <typeparam name="T">列表中元素的类型。</typeparam>
		/// <param name="list">要翻转的列表。</param>
		/// <returns>翻转完毕的列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为<c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 翻转指定的列表。
		/// </summary>
		/// </overloads>
		public static IList<T> Reverse<T>(this IList<T> list)
		{
			ArgumentNullException.ThrowIfNull(list);
			ReverseInterval(list, 0, list.Count);
			return list;
		}

		/// <summary>
		/// 翻转指定列表的指定部分。
		/// </summary>
		/// <typeparam name="T">列表中元素的类型。</typeparam>
		/// <param name="list">要翻转的列表。</param>
		/// <param name="index">要翻转部分的起始索引。</param>
		/// <param name="count">要翻转部分的长度。</param>
		/// <returns>翻转完毕的列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为<c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>
		/// 或 <paramref name="count"/> 小于零。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>
		/// 加 <paramref name="count"/> 之和大于列表的长度。</exception>
		public static IList<T> Reverse<T>(this IList<T> list, int index, int count)
		{
			ArgumentNullException.ThrowIfNull(list);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (count < 0 || index + count > list.Count)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(count);
			}
			ReverseInterval(list, index, count);
			return list;
		}

		/// <summary>
		/// 翻转指定列表的指定部分。
		/// </summary>
		/// <typeparam name="T">列表中元素的类型。</typeparam>
		/// <param name="list">要翻转的列表。</param>
		/// <param name="index">要翻转部分的起始索引。</param>
		/// <param name="count">要翻转部分的长度。</param>
		private static void ReverseInterval<T>(this IList<T> list, int index, int count)
		{
			for (int end = index + count - 1; index < end; index++, end--)
			{
				T temp = list[index];
				list[index] = list[end];
				list[end] = temp;
			}
		}

		#endregion // 翻转

		#region 二分查找元素

		/// <summary>
		/// 在指定排序 <see cref="IList{T}"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="T">列表元素的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 <see cref="IList{T}"/>。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 <see cref="IComparer{T}"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidOperationException"><paramref name="comparer"/> 为 <c>null</c>，
		/// 且 <typeparamref name="T"/> 类型没有实现 <see cref="IComparable{T}"/> 泛型接口。</exception>
		public static int BinarySearch<T>(this IList<T> list, T value, IComparer<T>? comparer = null)
		{
			ArgumentNullException.ThrowIfNull(list);
			return BinarySearchInternal(list, 0, list.Count, value, comparer);
		}

		/// <summary>
		/// 在排序 <see cref="IList{T}"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="T">列表元素的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 <see cref="IList{T}"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 <see cref="IComparer{T}"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 或 
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="index"/> 和 <paramref name="length"/> 
		/// 不指定 <paramref name="list"/> 中的有效范围。</exception>
		/// <exception cref="InvalidOperationException"><paramref name="comparer"/> 为 <c>null</c>，
		/// 且 <typeparamref name="T"/> 类型没有实现 <see cref="IComparable{T}"/> 泛型接口。</exception>
		public static int BinarySearch<T>(this IList<T> list, int index, int length, T value, IComparer<T>? comparer = null)
		{
			ArgumentNullException.ThrowIfNull(list);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (length < 0 || index + length > list.Count)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(length);
			}
			return BinarySearchInternal(list, index, length, value, comparer);
		}

		/// <summary>
		/// 使用指定的 <see cref="IComparer{T}"/> 泛型接口，在排序 <see cref="IList{T}"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="T">列表元素的类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 <see cref="IList{T}"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 <see cref="IComparer{T}"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		/// <exception cref="InvalidOperationException"><paramref name="comparer"/> 为 <c>null</c>，
		/// 且 <typeparamref name="T"/> 类型没有实现 <see cref="IComparable{T}"/> 泛型接口。</exception>
		private static int BinarySearchInternal<T>(this IList<T> list, int index, int length,
			T value, IComparer<T>? comparer)
		{
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			try
			{
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
					if (cmp < 0)
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
				throw CommonExceptions.CollectionItemCompareFailed(ex);
			}
		}

		/// <summary>
		/// 在整个排序 <see cref="IList{T}"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="TSource">列表元素的类型。</typeparam>
		/// <typeparam name="TResult">要投影到的新类型。</typeparam>
		/// <param name="list">要搜索的从零开始的排序 <see cref="IList{T}"/>。</param>
		/// <param name="value">要搜索的元素。</param>
		/// <param name="selector">用于从元素投影的函数。</param>
		/// <param name="comparer">比较投影结果时要使用的 <see cref="IComparer{T}"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="selector"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidOperationException"><paramref name="comparer"/> 为 <c>null</c>，
		/// 且 <typeparamref name="TResult"/> 类型没有实现 <see cref="IComparable{T}"/> 泛型接口。</exception>
		public static int BinarySearch<TSource, TResult>(this IList<TSource> list, TResult value,
			Func<TSource, TResult> selector, IComparer<TResult>? comparer = null)
		{
			ArgumentNullException.ThrowIfNull(list);
			ArgumentNullException.ThrowIfNull(selector);
			return BinarySearchInternal(list, 0, list.Count, value, selector, comparer);
		}

		/// <summary>
		/// 在排序 <see cref="IList{T}"/> 的某个范围中搜索特定元素。
		/// </summary>
		/// <typeparam name="TSource">列表元素的类型。</typeparam>
		/// <typeparam name="TResult">要投影到的新类型。</typeparam>
		/// <param name="list">要搜索的排序 <see cref="IList{T}"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的元素。</param>
		/// <param name="selector">用于从元素投影的函数。</param>
		/// <param name="comparer">比较投影结果时要使用的 <see cref="IComparer{T}"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="selector"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 或 
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="index"/> 和 <paramref name="length"/> 
		/// 不指定 <paramref name="list"/> 中的有效范围。</exception>
		/// <exception cref="InvalidOperationException"><paramref name="comparer"/> 为 <c>null</c>，
		/// 且 <typeparamref name="TResult"/> 类型没有实现 <see cref="IComparable{T}"/> 泛型接口。</exception>
		public static int BinarySearch<TSource, TResult>(this IList<TSource> list, int index, int length,
			TResult value, Func<TSource, TResult> selector, IComparer<TResult>? comparer = null)
		{
			ArgumentNullException.ThrowIfNull(list);
			ArgumentNullException.ThrowIfNull(selector);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (length < 0 || index + length > list.Count)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(length);
			}
			return BinarySearchInternal(list, index, length, value, selector, comparer);
		}

		/// <summary>
		/// 在排序 <see cref="IList{T}"/> 的某个范围中搜索特定元素。
		/// </summary>
		/// <typeparam name="TSource">列表元素的类型。</typeparam>
		/// <typeparam name="TResult">要投影到的新类型。</typeparam>
		/// <param name="list">要搜索的排序 <see cref="IList{T}"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的元素。</param>
		/// <param name="selector">用于从元素投影的函数。</param>
		/// <param name="comparer">比较投影结果时要使用的 <see cref="IComparer{T}"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="list"/> 
		/// 中的指定 <paramref name="value"/> 的索引。如果找不到 <paramref name="value"/> 
		/// 且 <paramref name="value"/> 小于 <paramref name="list"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于 <paramref name="value"/> 的第一个元素的索引的按位求补。
		/// 如果找不到 <paramref name="value"/> 且 <paramref name="value"/> 大于 <paramref name="list"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="selector"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 或 
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="index"/> 和 <paramref name="length"/> 
		/// 不指定 <paramref name="list"/> 中的有效范围。</exception>
		/// <exception cref="InvalidOperationException"><paramref name="comparer"/> 为 <c>null</c>，
		/// 且 <typeparamref name="TResult"/> 类型没有实现 <see cref="IComparable{T}"/> 泛型接口。</exception>
		private static int BinarySearchInternal<TSource, TResult>(this IList<TSource> list, int index, int length,
			TResult value, Func<TSource, TResult> selector, IComparer<TResult>? comparer)
		{
			if (comparer == null)
			{
				comparer = Comparer<TResult>.Default;
			}
			try
			{
				int low = index;
				int high = (index + length) - 1;
				while (low <= high)
				{
					int mid = low + ((high - low) >> 1);
					int cmp = comparer.Compare(selector(list[mid]), value);
					if (cmp == 0)
					{
						return mid;
					}
					if (cmp < 0)
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
				throw CommonExceptions.CollectionItemCompareFailed(ex);
			}
		}

		#endregion // 二分查找键

	}
}
