namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 为泛型集合提供基类。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	public abstract class ReadOnlySetBase<T> : ReadOnlyCollectionBase<T>, ISet<T>, IReadOnlySet<T>
	{
		/// <summary>
		/// 初始化 <see cref="ReadOnlySetBase{T}"/> 类的新实例。
		/// </summary>
		protected ReadOnlySetBase() : base() { }

		/// <summary>
		/// 确定当前集与指定集合相比，相同的和未包含的元素数目。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <param name="returnIfUnfound">是否遇到未包含的元素就返回。</param>
		/// <returns>当前集合中相同元素和为包含的元素数目。</returns>
		protected virtual (int sameCount, int unfoundCount) CountElements(IEnumerable<T> other,
			bool returnIfUnfound)
		{
			int sameCount = 0, unfoundCount = 0;
			HashSet<T> uniqueSet = new();
			foreach (T item in other)
			{
				if (Contains(item))
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

		#region ISet<T> 成员

		/// <summary>
		/// 向当前集合内添加元素，并返回一个指示是否已成功添加元素的值。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要添加到集合内的元素。</param>
		/// <returns>如果该元素已添加到集合内，则为 <c>true</c>；如果该元素已在集合内，则为 <c>false</c>。</returns>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		bool ISet<T>.Add(T item)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 从当前集合内移除指定集合中的所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要从集合内移除的项的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.ExceptWith(IEnumerable<T> other)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 修改当前集合，使当前集合仅包含指定集合中也存在的元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.IntersectWith(IEnumerable<T> other)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 确定当前集合是否为指定集合的真子集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的真子集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsProperSubsetOf(IEnumerable<T> other)
		{
			ArgumentNullException.ThrowIfNull(other);
			if (Count == 0)
			{
				return other.Any();
			}
			var (sameCount, unfoundCount) = CountElements(other, false);
			return sameCount == Count && unfoundCount > 0;
		}

		/// <summary>
		/// 确定当前集合是否为指定集合的真超集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的真超集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsProperSupersetOf(IEnumerable<T> other)
		{
			ArgumentNullException.ThrowIfNull(other);
			if (Count == 0)
			{
				return false;
			}
			var (sameCount, unfoundCount) = CountElements(other, true);
			return sameCount < Count && unfoundCount == 0;
		}

		/// <summary>
		/// 确定当前集合是否为指定集合的子集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的子集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsSubsetOf(IEnumerable<T> other)
		{
			ArgumentNullException.ThrowIfNull(other);
			if (Count == 0)
			{
				return true;
			}
			var (sameCount, unfoundCount) = CountElements(other, false);
			return sameCount == Count && unfoundCount >= 0;
		}

		/// <summary>
		/// 确定当前集合是否为指定集合的超集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的超集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool IsSupersetOf(IEnumerable<T> other)
		{
			ArgumentNullException.ThrowIfNull(other);
			if (Count == 0)
			{
				return !other.Any();
			}
			return other.All(Contains);
		}

		/// <summary>
		/// 确定当前集合是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool Overlaps(IEnumerable<T> other)
		{
			ArgumentNullException.ThrowIfNull(other);
			if (Count == 0)
			{
				return false;
			}
			return other.Any(Contains);
		}

		/// <summary>
		/// 确定当前集合与指定的集合中是否包含相同的元素。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合等于 <paramref name="other"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public virtual bool SetEquals(IEnumerable<T> other)
		{
			ArgumentNullException.ThrowIfNull(other);
			if (Count == 0)
			{
				return !other.Any();
			}
			var (sameCount, unfoundCount) = CountElements(other, true);
			return (sameCount == Count && unfoundCount == 0);
		}

		/// <summary>
		/// 修改当前集合，使该集合仅包含当前集合或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 修改当前集合，使该集合包含当前集合和指定集合中同时存在的所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.UnionWith(IEnumerable<T> other)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		#endregion

	}
}
