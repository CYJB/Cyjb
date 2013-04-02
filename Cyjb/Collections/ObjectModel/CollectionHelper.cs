using System;
using System.Collections.Generic;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 提供集合的帮助类。
	/// </summary>
	internal static class CollectionHelper
	{
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将源集合的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="source">要复制元素的源集合。</param>
		/// <param name="array">作为从源集合复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源集合中的元素数目大于从 
		/// <paramref name="index"/> 到目标 
		/// <paramref name="array"/> 末尾之间的可用空间。</exception>
		/// <exception cref="System.ArgumentException">源集合的类型无法自动转换为目标 
		/// <paramref name="array"/> 的类型。</exception>
		public static void CopyTo<T>(ICollection<T> source, Array array, int index)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			ExceptionHelper.CheckFlatArray(array, "array");
			if (index < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("index");
			}
			if (array.Length - index < source.Count)
			{
				throw ExceptionHelper.ArrayTooSmall("array");
			}
			T[] arr = array as T[];
			if (arr != null)
			{
				source.CopyTo(arr, index);
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
					throw ExceptionHelper.ArrayTypeInvalid(ex);
				}
			}
		}
	}
}
