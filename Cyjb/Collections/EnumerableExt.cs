using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cyjb.Collections
{
	/// <summary>
	/// 提供对 <see cref="IEnumerable{T}"/> 接口的扩展方法。
	/// </summary>
	public static class EnumerableExt
	{
		/// <summary>
		/// 随机打乱转序列中元素的顺序。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。
		/// </typeparam>
		/// <param name="source">要随机打乱顺序的值序列。</param>
		/// <returns>一个序列，其元素以随机顺序对应于输入序列的元素。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
		public static IEnumerable<TSource> RandomOrder<TSource>(this IEnumerable<TSource> source)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			Contract.EndContractBlock();
			return source.ToArray().Random();
		}
		/// <summary>
		/// 通过使用默认的相等比较器对值进行比较返回序列中的重复元素。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。
		/// </typeparam>
		/// <param name="source">要从中得到重复元素的序列。</param>
		/// <returns>一个序列，包含源序列中的重复元素。</returns>
		/// <overloads>
		/// <summary>
		/// 返回序列中的重复元素。
		/// </summary>
		/// </overloads>
		public static IEnumerable<TSource> Iterative<TSource>(
			this IEnumerable<TSource> source)
		{
			return Iterative(source, null);
		}
		/// <summary>
		/// 通过使用指定的 <see cref="IEqualityComparer{T}"/> 对值进行比较返回序列中的重复元素。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">要从中得到重复元素的序列。</param>
		/// <param name="comparer">用于比较值的 <see cref="IEqualityComparer{T}"/>。</param>
		/// <returns>一个序列，包含源序列中的重复元素。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
		public static IEnumerable<TSource> Iterative<TSource>(this IEnumerable<TSource> source,
			IEqualityComparer<TSource> comparer)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			Contract.EndContractBlock();
			Dictionary<TSource, bool> dict = new Dictionary<TSource, bool>(comparer);
			foreach (TSource item in source)
			{
				bool flag;
				if (dict.TryGetValue(item, out flag))
				{
					if (flag)
					{
						yield return item;
						dict[item] = false;
					}
				}
				else
				{
					dict.Add(item, true);
				}
			}
		}
		/// <summary>
		/// 通过使用默认的相等比较器判断序列中是否不包含重复元素。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。
		/// </typeparam>
		/// <param name="source">要判断是否不包含重复元素的序列。</param>
		/// <returns>如果序列中不包含重复元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 判断序列中是否不包含重复元素。
		/// </summary>
		/// </overloads>
		public static bool IsDistinct<TSource>(this IEnumerable<TSource> source)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			Contract.EndContractBlock();
			HashSet<TSource> set = new HashSet<TSource>();
			return source.All(set.Add);
		}
		/// <summary>
		/// 通过使用指定的 <see cref="IEqualityComparer{T}"/> 判断序列中是否不包含重复元素。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">要判断是否不包含重复元素的序列。</param>
		/// <param name="comparer">用于比较值的 <see cref="IEqualityComparer{T}"/>。</param>
		/// <returns>如果序列中不包含重复元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
		public static bool IsDistinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			Contract.EndContractBlock();
			HashSet<TSource> set = new HashSet<TSource>(comparer);
			return source.All(set.Add);
		}
		/// <summary>
		/// 返回序列中满足指定条件的第一个元素的索引。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">要从中返回元素索引的 <see cref="IEnumerable{T}"/>。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <returns>序列中通过指定谓词函数中的测试的第一个元素的索引。如果没有这样的元素，则返回 <c>-1</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		public static int FirstIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
			Contract.EndContractBlock();
			int idx = 0;
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					return idx;
				}
				idx++;
			}
			return -1;
		}
		/// <summary>
		/// 返回序列中满足指定条件的最后一个元素的索引。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">要从中返回元素索引的 <see cref="IEnumerable{T}"/>。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <returns>序列中通过指定谓词函数中的测试的最后一个元素的索引。如果没有这样的元素，则返回 <c>-1</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		public static int LastIndex<TSource>(this IEnumerable<TSource> source,
			Func<TSource, bool> predicate)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
			Contract.EndContractBlock();
			int lastIdx = -1;
			int idx = 0;
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					lastIdx = idx;
				}
				idx++;
			}
			return lastIdx;
		}
	}
}
