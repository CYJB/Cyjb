using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 提供集合的辅助类。
	/// </summary>
	internal static class CollectionHelper
	{
		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将指定集合
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="source">要复制元素的集合。</param>
		/// <param name="array">从 <paramref name="source"/> 复制的元素的目标位置的一维 
		/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/>
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/>
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		public static void CopyTo<T>(ICollection<T> source, Array array, int index)
		{
			Contract.Requires(source != null);
			if (array == null)
			{
				throw ExceptionHelper.ArgumentNull("source");
			}
			if (array.Rank != 1)
			{
				throw ExceptionHelper.ArrayRankMultiDimNotSupported();
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw ExceptionHelper.ArrayNonZeroLowerBound("array");
			}
			if (index < 0)
			{
				throw ExceptionHelper.ArgumentNegative("arrayIndex");
			}
			if (array.Length - index < source.Count)
			{
				throw ExceptionHelper.ArrayTooSmall("array");
			}
			Contract.EndContractBlock();
			T[] arr = array as T[];
			if (arr != null)
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
					throw ExceptionHelper.ArrayTypeInvalid(ex);
				}
			}
		}
		/// <summary>
		/// 返回指定的对象是否与 <typeparamref name="T"/> 类型兼容。
		/// </summary>
		/// <typeparam name="T">要测试是否兼容的类型。</typeparam>
		/// <param name="value">要测试是否兼容的对象。</param>
		/// <returns>如果指定的对象与 <typeparamref name="T"/> 类型兼容，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool IsCompatible<T>(object value)
		{
			return (value is T) || (value == null && default(T) == null);
		}
		/// <summary>
		/// 获取指定字典的值集合。
		/// </summary>
		/// <param name="dict">要获取值集合的字典。</param>
		/// <returns>指定字典的值集合。</returns>
		public static IEnumerable<TItem> GetDictValues<TKey, TItem>(IDictionary<TKey, TItem> dict)
		{
			if (dict == null)
			{
				throw ExceptionHelper.ArgumentNull("dict");
			}
			Contract.Ensures(Contract.Result<IEnumerable<TItem>>() != null);
			return dict.Values;
		}
	}
}
