using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyjb
{
	/// <summary>
	/// 提供对 <see cref="System.Collections.Generic.IEnumerable{T}"/> 
	/// 接口的扩展方法。
	/// </summary>
	public static class EnumerableExt
	{
		/// <summary>
		/// 对序列中的所有元素依次执行操作。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">包含要应用操作的元素的 
		/// <see cref="System.Collections.Generic.IEnumerable{T}"/>。
		/// </param>
		/// <param name="action">用于对每个元素执行的操作的函数。</param>
		/// <returns>类型为 <see cref="System.Collections.Generic.IEnumerable{T}"/> 
		/// 的输入序列。</returns>
		/// <overloads>
		/// <summary>
		/// 对序列中的所有元素依次执行操作。
		/// </summary>
		/// </overloads>
		public static IEnumerable<TSource> Each<TSource>(
			this IEnumerable<TSource> source, Action<TSource> action)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			CommonExceptions.CheckArgumentNull(action, "action");
			foreach (TSource obj in source)
			{
				action(obj);
			}
			return source;
		}
		/// <summary>
		/// 对序列中的所有元素依次执行操作，并可以随时停止。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。
		/// </typeparam>
		/// <param name="source">包含要应用操作的元素的 
		/// <see cref="System.Collections.Generic.IEnumerable{T}"/>。
		/// </param>
		/// <param name="func">用于对每个元素执行的操作的函数，返回 <c>true</c>
		/// 则继续执行操作，返回 <c>false</c> 则停止操作。</param>
		/// <returns>类型为 <see cref="System.Collections.Generic.IEnumerable{T}"/> 
		/// 的输入序列。</returns>
		public static IEnumerable<TSource> Each<TSource>(
			this IEnumerable<TSource> source, Func<TSource, bool> func)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			CommonExceptions.CheckArgumentNull(func, "func");
			foreach (TSource obj in source)
			{
				if (!func(obj))
				{
					break;
				}
			}
			return source;
		}
		/// <summary>
		/// 随机打乱转序列中元素的顺序。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。
		/// </typeparam>
		/// <param name="source">要随机打乱顺序的值序列。</param>
		/// <returns>一个序列，其元素以随机顺序对应于输入序列的元素。</returns>
		public static IEnumerable<TSource> RandomOrder<TSource>(
			this IEnumerable<TSource> source)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
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
		/// 通过使用指定的 
		/// <see cref="System.Collections.Generic.IEqualityComparer{T}"/> 
		/// 对值进行比较返回序列中的重复元素。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。
		/// </typeparam>
		/// <param name="source">要从中得到重复元素的序列。</param>
		/// <param name="comparer">用于比较值的 
		/// <see cref="System.Collections.Generic.IEqualityComparer{T}"/>。</param>
		/// <returns>一个序列，包含源序列中的重复元素。</returns>
		public static IEnumerable<TSource> Iterative<TSource>(
			this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
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
		/// 返回序列中满足指定条件的第一个元素的索引。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">要从中返回元素索引的 
		/// <see cref="System.Collections.Generic.IEnumerable{T}"/>。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <returns>序列中通过指定谓词函数中的测试的第一个元素的索引。
		/// 如果没有这样的元素，则返回 <c>-1</c>。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="source"/> 
		/// 或 <paramref name="predicate"/> 为 <c>null</c>。</exception>
		public static int FirstIndex<TSource>(this IEnumerable<TSource> source,
			Func<TSource, bool> predicate)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
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
		/// <param name="source">要从中返回元素索引的 
		/// <see cref="System.Collections.Generic.IEnumerable{T}"/>。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <returns>序列中通过指定谓词函数中的测试的最后一个元素的索引。
		/// 如果没有这样的元素，则返回 <c>-1</c>。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="source"/> 
		/// 或 <paramref name="predicate"/> 为 <c>null</c>。</exception>
		public static int LastIndex<TSource>(this IEnumerable<TSource> source,
			Func<TSource, bool> predicate)
		{
			CommonExceptions.CheckArgumentNull(source, "source");
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
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
