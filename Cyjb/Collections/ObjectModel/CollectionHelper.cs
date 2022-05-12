namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 提供集合的辅助类。
	/// </summary>
	internal static class CollectionHelper
	{
		/// <summary>
		/// 确定当前集与指定集合相比，相同的和未包含的元素数目。
		/// </summary>
		/// <param name="set">当前集。</param>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <param name="returnIfUnfound">是否遇到未包含的元素就返回。</param>
		/// <returns>当前集合中相同元素和为包含的元素数目。</returns>
		public static (int sameCount, int unfoundCount) CountElements<T>(ISet<T> set, IEnumerable<T> other,
			bool returnIfUnfound)
		{
			int sameCount = 0, unfoundCount = 0;
			HashSet<T> uniqueSet = new();
			foreach (T item in other)
			{
				if (set.Contains(item))
				{
					if (uniqueSet.Add(item))
					{
						sameCount++;
					}
				}
				else
				{
					unfoundCount++;
					if (returnIfUnfound)
					{
						break;
					}
				}
			}
			return (sameCount, unfoundCount);
		}

		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将指定集合的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="source">要复制元素的集合。</param>
		/// <param name="array">从 <paramref name="source"/> 复制的元素的目标位置的一维 
		/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/>
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/>
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		public static void CopyTo<T>(ICollection<T> source, Array array, int index)
		{
			CommonExceptions.CheckSimplyArray(array);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			else if (array.Length - index < source.Count)
			{
				throw CommonExceptions.ArrayTooSmall(nameof(array));
			}
			if (array is T[] arr)
			{
				foreach (T obj in source)
				{
					arr[index++] = obj;
				}
			}
			else
			{
				try
				{
					foreach (T obj in source)
					{
						array.SetValue(obj, index++);
					}
				}
				catch (InvalidCastException ex)
				{
					throw CommonExceptions.InvalidElementType(ex);
				}
			}
		}

		/// <summary>
		/// 返回指定的对象是否与 <typeparamref name="T"/> 类型兼容。
		/// </summary>
		/// <typeparam name="T">要测试是否兼容的类型。</typeparam>
		/// <param name="value">要测试是否兼容的对象。</param>
		/// <returns>如果指定的对象与 <typeparamref name="T"/> 类型兼容，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsCompatible<T>(object? value)
		{
			return (value is T) || (value == null && default(T) == null);
		}
	}
}
