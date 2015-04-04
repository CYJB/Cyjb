using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Cyjb.Collections
{
	/// <summary>
	/// 提供对 <see cref="ICollection{T}"/> 的扩展方法。
	/// </summary>
	public static class CollectionExt
	{
		/// <summary>
		/// 将指定集合的元素添加到当前集合中。
		/// </summary>
		/// <typeparam name="T">集合中元素的类型。</typeparam>
		/// <param name="collection">要添加到的集合。</param>
		/// <param name="other">要添加的元素集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
		{
			CommonExceptions.CheckArgumentNull(collection, "collection");
			CommonExceptions.CheckArgumentNull(other, "other");
			Contract.EndContractBlock();
			foreach (T item in other)
			{
				collection.Add(item);
			}
		}
	}
}
