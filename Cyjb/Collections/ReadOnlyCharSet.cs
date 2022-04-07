using System.Diagnostics;
using System.Text;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示字符的只读有序集合。
	/// </summary>
	[Serializable, DebuggerDisplay("{ToString()} Count = {Count}")]
	public sealed class ReadOnlyCharSet : ReadOnlySetBase<char>, IEquatable<ReadOnlyCharSet>
	{
		/// <summary>
		/// 字符集合内的字符范围列表。
		/// </summary>
		private readonly string ranges;
		/// <summary>
		/// 集合中字符的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int count;

		/// <summary>
		/// 初始化 <see cref="ReadOnlyCharSet"/> 类的新实例。
		/// </summary>
		internal ReadOnlyCharSet(string ranges, int count) : base()
		{
			this.ranges = ranges;
			this.count = count;
		}

		/// <summary>
		/// 返回一个循环访问字符范围的枚举器。
		/// </summary>
		/// <returns>可用于循环访问字符范围的 <see cref="IEnumerable{Tuple}"/>。</returns>
		public IEnumerable<(char start, char end)> Ranges()
		{
			for (int i = 0; i < ranges.Length; i += 2)
			{
				yield return (ranges[i], ranges[i + 1]);
			}
		}

		/// <summary>
		/// 找到包含指定字符的范围起始索引。
		/// </summary>
		/// <param name="ch">要搜索的字符。</param>
		/// <returns>包含指定字符的范围起始索引，如果不存在则返回 <c>-1</c>。</returns>
		private int FindRange(char ch)
		{
			int low = 0;
			int high = ranges.Length - 1;
			while (low < high)
			{
				int mid = (low + high + 1) >> 1;
				if (ch < ranges[mid])
				{
					high = mid - 1;
				}
				else
				{
					low = mid;
				}
			}
			if ((low & 1) == 1)
			{
				if (ch <= ranges[low])
				{
					return low - 1;
				}
				else
				{
					// 大于当前范围的结束。
					return -1;
				}
			}
			else if (low == 0 && ch < ranges[0])
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
		private bool ContainsAllElements(ReadOnlyCharSet other)
		{
			string otherRange = other.ranges;
			for (int i = 0; i < otherRange.Length; i += 2)
			{
				int idx = FindRange(otherRange[i]);
				if (idx < 0 || ranges[idx + 1] < otherRange[i + 1])
				{
					return false;
				}
			}
			return true;
		}

		#region ISet<char> 成员

		/// <summary>
		/// 确定当前集合是否为指定集合的真子集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的真子集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsProperSubsetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (count == 0)
			{
				return other.Any();
			}
			if (other is ReadOnlyCharSet otherSet)
			{
				return count < otherSet.Count && otherSet.ContainsAllElements(this);
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
		public override bool IsProperSupersetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (count == 0)
			{
				return false;
			}
			if (other is ReadOnlyCharSet otherSet)
			{
				return count > otherSet.Count && ContainsAllElements(otherSet);
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
		public override bool IsSubsetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (count == 0)
			{
				return true;
			}
			if (other is ReadOnlyCharSet otherSet)
			{
				return count <= otherSet.Count && otherSet.ContainsAllElements(this);
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
		public override bool IsSupersetOf(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (count == 0)
			{
				return !other.Any();
			}
			if (other is ReadOnlyCharSet otherSet)
			{
				return count >= otherSet.Count && ContainsAllElements(otherSet);
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
		public override bool Overlaps(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (count == 0)
			{
				return false;
			}
			if (other is ReadOnlyCharSet otherSet)
			{
				// ReadOnlyCharSet 可以范围操作。
				string otherRange = otherSet.ranges;
				for (int i = 0; i < otherRange.Length; i += 2)
				{
					char start = otherRange[i];
					int idx = FindRange(start);
					if (idx >= 0 && ranges[idx + 1] >= start)
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
		public override bool SetEquals(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (count == 0)
			{
				return !other.Any();
			}
			if (other is ReadOnlyCharSet otherSet)
			{
				return count == otherSet.Count && ContainsAllElements(otherSet);
			}
			else
			{
				var (sameCount, unfoundCount) = CountElements(other, true);
				return (sameCount == Count && unfoundCount == 0);
			}
		}

		#endregion // ISet<char> 成员

		#region ICollection<char> 成员

		/// <summary>
		/// 获取当前集合包含的字符数。
		/// </summary>
		/// <value>当前集合中包含的字符数。</value>
		public override int Count => count;

		/// <summary>
		/// 确定当前集合是否包含指定字符。
		/// </summary>
		/// <param name="ch">要在当前集合中定位的字符。</param>
		/// <returns>如果在当前集合中找到 <paramref name="ch"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(char ch)
		{
			int idx = FindRange(ch);
			return idx >= 0 && ranges[idx + 1] >= ch;
		}

		#endregion // ICollection<char> 成员

		#region IEnumerable<char> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{Char}"/>。</returns>
		public override IEnumerator<char> GetEnumerator()
		{
			for (int i = 0; i < ranges.Length; i += 2)
			{
				char end = ranges[i + 1];
				// 避免 end 正好是 char.MaxValue 时导致死循环。
				for (char ch = ranges[i]; ch < end; ch++)
				{
					yield return ch;
				}
				yield return end;
			}
		}

		#endregion // IEnumerable<char> 成员

		#region IEquatable<ReadOnlyCharSet> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">一个与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="NotImplementedException"></exception>
		public bool Equals(ReadOnlyCharSet? other)
		{
			if (other == null)
			{
				return false;
			}
			return count == other.count && ranges == other.ranges;
		}

		#endregion // IEquatable<ReadOnlyCharSet> 成员

		/// <summary>
		/// 返回当前集合的字符串表示。
		/// </summary>
		/// <returns>当前集合的字符串表示。</returns>
		public override string ToString()
		{
			StringBuilder builder = new();
			builder.Append('[');
			for (int i = 0; i < ranges.Length; i += 2)
			{
				char start = ranges[i];
				char end = ranges[i + 1];
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

		/// <summary>
		/// 确定指定对象是否等于当前对象。
		/// </summary>
		/// <param name="obj">要与当前对象进行比较的对象。</param>
		/// <returns>如果指定的对象等于当前对象，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			else if (obj is ReadOnlyCharSet set)
			{
				return Equals(set);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 返回当前对象的哈希代码。
		/// </summary>
		/// <returns>当前对象的哈希代码。</returns>
		public override int GetHashCode()
		{
			return ranges.GetHashCode();
		}
	}
}
