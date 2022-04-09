using System.Collections;
using System.Diagnostics;
using System.Text;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示字符的只读有序集合。
	/// </summary>
	[Serializable, DebuggerDisplay("{ToString()} Count = {Count}")]
	public struct ReadOnlyCharSet : ISet<char>, IReadOnlySet<char>, IRangeCollection<char>,
		ICollection<char>, IReadOnlyCollection<char>, IEnumerable<char>, IEnumerable
	{
		/// <summary>
		/// 字符集合数据，索引 0 表示集合内的字符个数，之后为字符范围列表。
		/// </summary>
		private readonly string data = "\0";

		/// <summary>
		/// 初始化 <see cref="ReadOnlyCharSet"/> 类的新实例。
		/// </summary>
		/// <param name="data">字符集合数据。</param>
		internal ReadOnlyCharSet(string data)
		{
			this.data = data;
		}

		/// <summary>
		/// 返回一个循环访问字符范围的枚举器。
		/// </summary>
		/// <returns>可用于循环访问字符范围的 <see cref="IEnumerable{Tuple}"/>。</returns>
		public IEnumerable<(char start, char end)> Ranges()
		{
			for (int i = 1; i < data.Length; i += 2)
			{
				yield return (data[i], data[i + 1]);
			}
		}

		/// <summary>
		/// 找到包含指定字符的范围起始索引。
		/// </summary>
		/// <param name="ch">要搜索的字符。</param>
		/// <returns>包含指定字符的范围起始索引，如果不存在则返回 <c>-1</c>。</returns>
		private int FindRange(char ch)
		{
			int low = 1;
			int high = data.Length - 1;
			while (low < high)
			{
				int mid = (low + high + 1) >> 1;
				if (ch < data[mid])
				{
					high = mid - 1;
				}
				else
				{
					low = mid;
				}
			}
			if ((low & 1) == 0)
			{
				if (ch <= data[low])
				{
					return low - 1;
				}
				else
				{
					// 大于当前范围的结束。
					return -1;
				}
			}
			else if (low == 1 && ch < data[1])
			{
				// 小于首个范围的起始。
				return -1;
			}
			else
			{
				return low;
			}
		}

		/// <summary>
		/// 确定当前集合是否包含指定的集合中的所有字符。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果包含 <paramref name="other"/> 中的所有字符，则返回 
		/// <c>true</c>，否则返回 <c>false</c>。</returns>
		internal bool ContainsAllElements(IRangeCollection<char> other)
		{
			foreach (var (start, end) in other.Ranges())
			{
				int idx = FindRange(start);
				if (idx < 0 || data[idx + 1] < end)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 确定当前集与指定集合相比，相同的和未包含的元素数目。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <param name="returnIfUnfound">是否遇到未包含的元素就返回。</param>
		/// <returns>当前集合中相同元素和为包含的元素数目。</returns>
		private (int sameCount, int unfoundCount) CountElements(IEnumerable<char> other, bool returnIfUnfound)
		{
			int sameCount = 0, unfoundCount = 0;
			HashSet<char> uniqueSet = new();
			foreach (char item in other)
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

		#region ISet<char> 成员

		/// <summary>
		/// 向当前集合内添加元素，并返回一个指示是否已成功添加元素的值。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要添加到集合内的元素。</param>
		/// <returns>如果该元素已添加到集合内，则为 <c>true</c>；如果该元素已在集合内，则为 <c>false</c>。</returns>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		bool ISet<char>.Add(char item)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 从当前集合内移除指定集合中的所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要从集合内移除的项的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<char>.ExceptWith(IEnumerable<char> other)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 修改当前集合，使当前集合仅包含指定集合中也存在的元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<char>.IntersectWith(IEnumerable<char> other)
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
		public bool IsProperSubsetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (Count == 0)
			{
				return other.Any();
			}
			if (other is CharSet otherSet)
			{
				return Count < otherSet.Count && otherSet.ContainsAllElements(this);
			}
			else if (other is ReadOnlyCharSet otherRSet)
			{
				return Count < otherRSet.Count && otherRSet.ContainsAllElements(this);
			}
			else
			{
				var (sameCount, unfoundCount) = CountElements(other, false);
				return sameCount == Count && unfoundCount > 0;
			}
		}

		/// <summary>
		/// 确定当前集合是否为指定集合的真超集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的真超集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public bool IsProperSupersetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (Count == 0)
			{
				return false;
			}
			if (other is IRangeCollection<char> otherRange)
			{
				return Count > otherRange.Count && ContainsAllElements(otherRange);
			}
			else
			{
				var (sameCount, unfoundCount) = CountElements(other, true);
				return sameCount < Count && unfoundCount == 0;
			}
		}

		/// <summary>
		/// 确定当前集合是否为指定集合的子集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的子集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public bool IsSubsetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (Count == 0)
			{
				return true;
			}
			if (other is CharSet otherSet)
			{
				return Count <= otherSet.Count && otherSet.ContainsAllElements(this);
			}
			else if (other is ReadOnlyCharSet otherRSet)
			{
				return Count <= otherRSet.Count && otherRSet.ContainsAllElements(this);
			}
			else
			{
				var (sameCount, unfoundCount) = CountElements(other, false);
				return sameCount == Count && unfoundCount >= 0;
			}
		}

		/// <summary>
		/// 确定当前集合是否为指定集合的超集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的超集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public bool IsSupersetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (Count == 0)
			{
				return !other.Any();
			}
			if (other is IRangeCollection<char> otherRange)
			{
				return Count >= otherRange.Count && ContainsAllElements(otherRange);
			}
			else
			{
				return other.All(Contains);
			}
		}

		/// <summary>
		/// 确定当前集合是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public bool Overlaps(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (Count == 0)
			{
				return false;
			}
			if (other is IRangeCollection<char> otherRange)
			{
				foreach (var (start, end) in otherRange.Ranges())
				{
					int idx = FindRange(start);
					if (idx >= 0 && data[idx + 1] >= start)
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				return other.Any(Contains);
			}
		}

		/// <summary>
		/// 确定当前集合与指定的集合中是否包含相同的元素。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合等于 <paramref name="other"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public bool SetEquals(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (Count == 0)
			{
				return !other.Any();
			}
			if (other is IRangeCollection<char> otherRange)
			{
				return Count == otherRange.Count && ContainsAllElements(otherRange);
			}
			else
			{
				var (sameCount, unfoundCount) = CountElements(other, true);
				return (sameCount == Count && unfoundCount == 0);
			}
		}

		/// <summary>
		/// 修改当前集合，使该集合仅包含当前集合或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<char>.SymmetricExceptWith(IEnumerable<char> other)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 修改当前集合，使该集合包含当前集合和指定集合中同时存在的所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<char>.UnionWith(IEnumerable<char> other)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		#endregion

		#region ICollection<char> 成员

		/// <summary>
		/// 获取当前集合包含的元素数。
		/// </summary>
		/// <value>当前集合中包含的元素数。</value>
		public int Count => data[0];

		/// <summary>
		/// 获取一个值，该值指示当前集合是否为只读。
		/// </summary>
		/// <value>如果当前集合是只读的，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<char>.IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// 将指定对象添加到当前集合中。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要添加到当前集合的对象。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ICollection<char>.Add(char item)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 从当前集合中移除所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ICollection<char>.Clear()
		{
			throw CommonExceptions.MethodNotSupported();
		}

		/// <summary>
		/// 确定当前集合是否包含指定对象。
		/// </summary>
		/// <param name="item">要在当前集合中定位的对象。</param>
		/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(char item)
		{
			int idx = FindRange(item);
			return idx >= 0 && data[idx + 1] >= item;
		}

		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将当前集合
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从当前集合复制的元素的目标位置的一维 
		/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><see cref="CollectionBase{T}"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException">源当前集合
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		public void CopyTo(char[] array, int arrayIndex)
		{
			CollectionHelper.CopyTo(this, array, arrayIndex);
		}

		/// <summary>
		/// 从当前集合中移除特定对象的第一个匹配项。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要从当前集合中移除的对象。</param>
		/// <returns>如果已从当前集合中成功移除 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在原始当前集合中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		bool ICollection<char>.Remove(char item)
		{
			throw CommonExceptions.MethodNotSupported();
		}

		#endregion // ICollection<char> 成员

		#region IEnumerable<char> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public IEnumerator<char> GetEnumerator()
		{
			for (int i = 1; i < data.Length; i += 2)
			{
				char end = data[i + 1];
				// 避免 end 正好是 char.MaxValue 时导致死循环。
				for (char ch = data[i]; ch < end; ch++)
				{
					yield return ch;
				}
				yield return end;
			}
		}

		#endregion // IEnumerable<char> 成员

		#region IEnumerable 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion // IEnumerable 成员

		/// <summary>
		/// 返回当前集合的字符串表示。
		/// </summary>
		/// <returns>当前集合的字符串表示。</returns>
		public override string ToString()
		{
			StringBuilder builder = new();
			builder.Append('[');
			for (int i = 1; i < data.Length; i += 2)
			{
				char start = data[i];
				char end = data[i + 1];
				builder.Append(start);
				if (start + 1 < end)
				{
					builder.Append('-');
				}
				if (start != end)
				{
					builder.Append(end);
				}
			}
			builder.Append(']');
			return builder.ToString();
		}
	}
}
