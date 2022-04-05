using System.Diagnostics;
using System.Text;
using Cyjb.Collections.ObjectModel;
using Node = Cyjb.Collections.AVLTree<char, char>.Node;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示字符的有序集合。
	/// </summary>
	[Serializable, DebuggerDisplay("{ToString()} Count = {Count}")]
	public sealed class CharSet : SetBase<char>, IEquatable<CharSet>
	{
		/// <summary>
		/// 字符集合内的字符范围列表（Key 为 Start，Value 为 End）。
		/// </summary>
		private readonly AVLTree<char, char> ranges = new();
		/// <summary>
		/// 集合中字符的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;

		/// <summary>
		/// 初始化 <see cref="CharSet"/> 类的新实例。
		/// </summary>
		public CharSet() : base() { }
		/// <summary>
		/// 使用指定的元素初始化 <see cref="CharSet"/> 类的新实例。
		/// </summary>
		/// <param name="collection">初始化的字符集合。</param>
		public CharSet(IEnumerable<char> collection) : base()
		{
			UnionWith(collection);
		}

		/// <summary>
		/// 将指定字符范围添加到当前集合中。
		/// </summary>
		/// <param name="start">要添加到当前集合的字符范围起始（包含）。</param>
		/// <param name="end">要添加到当前集合的字符范围结束（包含）。</param>
		/// <returns>如果该字符范围内的<b>任何</b>字符已添加到集合内，则为 <c>true</c>；如果该字符范围已全部在集合内，则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentOutOfRangeException">字符范围的起始大于结束。</exception>
		public bool Add(char start, char end)
		{
			if (start > end)
			{
				throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
			}
			Node? node = ranges.FindLE(start);
			if (node == null || node.Value + 1 < start)
			{
				// 之前的节点不能覆盖或连接 [start, end] 的范围。
				count += end - start + 1;
				node = ranges.Add(start, end);
			}
			else if (node.Value < end)
			{
				// 存在可以覆盖部分 [start, end] 的范围。
				count += end - node.Value;
				node.Value = end;
			}
			else
			{
				// 已存在可以覆盖 [start, end] 的范围。
				return false;
			}
			MergeRange(node);
			return true;
		}

		/// <summary>
		/// 从当前集合中移除指定字符范围。
		/// </summary>
		/// <param name="start">要从当前集合中移除的字符范围起始（包含）。</param>
		/// <param name="end">要从当前集合中移除的字符范围结束（包含）。</param>
		/// <returns>如果已从当前集合中成功移除该字符范围内的<b>任何</b>字符，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在原始当前集合中没有找到字符范围内的字符，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentOutOfRangeException">字符范围的起始大于结束。</exception>
		public bool Remove(char start, char end)
		{
			if (start > end)
			{
				throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
			}
			int oldCount = count;
			// 从 node 开始向后遍历，移除范围。
			Node? next;
			for (Node? node = ranges.FindLE(start) ?? ranges.First; node != null && node.Key <= end; node = next)
			{
				next = node.Next;
				if (node.Value < start)
				{
					// 未在要移除的范围内。
					continue;
				}
				else if (node.Key < start)
				{
					// 保留前面部分字符。
					char oldEnd = node.Value;
					count -= node.Value - start + 1;
					node.Value = (char)(start - 1);
					if (oldEnd > end)
					{
						// 同时需要保留后面部分字符。
						count += oldEnd - end;
						ranges.Add((char)(end + 1), oldEnd);
						break;
					}
				}
				else if (node.Value <= end)
				{
					// 全部移除。
					count -= node.Value - node.Key + 1;
					ranges.Remove(node);
				}
				else
				{
					// 保留后面部分字符。
					count -= end - node.Key + 1;
					ranges.Remove(node);
					ranges.Add((char)(end + 1), node.Value);
					break;
				}
			}
			return count < oldCount;
		}

		/// <summary>
		/// 返回一个循环访问字符范围的枚举器。
		/// </summary>
		/// <returns>可用于循环访问字符范围的 <see cref="IEnumerable{Tuple}"/>。</returns>
		public IEnumerable<(char start, char end)> Ranges()
		{
			foreach (var node in ranges)
			{
				yield return (node.Key, node.Value);
			}
		}

		#region 字符范围操作

		/// <summary>
		/// 从指定节点开始合并重叠的字符范围。
		/// </summary>
		/// <param name="node">合并的起始节点。</param>
		private void MergeRange(Node node)
		{
			Node? next;
			while ((next = node.Next) != null)
			{
				if (node.Value + 1 < next.Key)
				{
					// 无法覆盖下一范围。
					break;
				}
				ranges.Remove(next);
				count -= next.Value - next.Key + 1;
				if (node.Value < next.Value)
				{
					count += next.Value - node.Value;
					node.Value = next.Value;
					break;
				}
			}
		}

		/// <summary>
		/// 确定当前集合是否包含指定的集合中的所有字符。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果包含 <paramref name="other"/> 中的所有字符，则返回 
		/// <c>true</c>，否则返回 <c>false</c>。</returns>
		private bool ContainsAllElements(CharSet other)
		{
			foreach (var (start, end) in other.Ranges())
			{
				Node? node = ranges.FindLE(start);
				if (node == null || node.Value < end)
				{
					return false;
				}
			}
			return true;
		}

		#endregion // 字符范围操作

		#region ISet<char> 成员

		/// <summary>
		/// 向当前集内添加字符，并返回一个指示是否已成功添加字符的值。
		/// </summary>
		/// <param name="ch">要添加到 <see cref="CharSet"/> 的中的字符。</param>
		/// <returns>如果该字符已添加到集内，则为 <c>true</c>；如果该字符已在集内，则为 <c>false</c>。</returns>
		public override bool Add(char ch)
		{
			return Add(ch, ch);
		}

		/// <summary>
		/// 从当前集合内移除指定集合中的所有元素。
		/// </summary>
		/// <param name="other">要从集合内移除的项的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void ExceptWith(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (count <= 0)
			{
				return;
			}
			if (ReferenceEquals(this, other))
			{
				Clear();
				return;
			}
			if (other is CharSet otherSet)
			{
				// CharSet 可以范围操作。
				foreach (var (start, end) in otherSet.Ranges())
				{
					Remove(start, end);
				}
			}
			else
			{
				foreach (char ch in other)
				{
					Remove(ch);
				}
			}
		}

		/// <summary>
		/// 修改当前集合，使当前集合仅包含指定集合中也存在的元素。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void IntersectWith(IEnumerable<char> other)
		{
			if (other is CharSet otherSet)
			{
				// 移除不在 otherSet 里的字符范围。
				char begin = '\0';
				foreach (var (start, end) in otherSet.Ranges())
				{
					if (begin < start)
					{
						Remove(begin, (char)(start - 1));
					}
					begin = (char)(end + 1);
				}
				if (begin < char.MaxValue)
				{
					Remove(begin, char.MaxValue);
				}
			}
			else
			{
				base.IntersectWith(other);
			}
		}

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
			if (other is CharSet otherSet)
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
			if (other is CharSet otherSet)
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
			if (other is CharSet otherSet)
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
			if (other is CharSet otherSet)
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
			if (other is CharSet otherSet)
			{
				// CharSet 可以范围操作。
				foreach (var (start, end) in otherSet.Ranges())
				{
					Node? node = ranges.FindLE(start);
					if (node != null && node.Key <= end && node.Value >= start)
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
			if (other is CharSet otherSet)
			{
				return count == otherSet.Count && ContainsAllElements(otherSet);
			}
			else
			{
				var (sameCount, unfoundCount) = CountElements(other, true);
				return (sameCount == Count && unfoundCount == 0);
			}
		}

		/// <summary>
		/// 修改当前集合，使该集合仅包含当前集合或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void SymmetricExceptWith(IEnumerable<char> other)
		{
			if (other is CharSet otherSet)
			{
				CharSet newSet = new(otherSet);
				newSet.ExceptWith(this);
				ExceptWith(otherSet);
				UnionWith(newSet);
			}
			else
			{
				base.SymmetricExceptWith(other);
			}
		}

		/// <summary>
		/// 修改当前集合，使该集合包含当前集合和指定集合中同时存在的所有元素。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		public override void UnionWith(IEnumerable<char> other)
		{
			CommonExceptions.CheckArgumentNull(other);
			if (ReferenceEquals(this, other))
			{
				return;
			}
			if (other is CharSet otherSet)
			{
				// CharSet 可以范围操作。
				foreach (var (start, end) in otherSet.Ranges())
				{
					Add(start, end);
				}
			}
			else
			{
				foreach (char ch in other)
				{
					Add(ch, ch);
				}
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
		/// 从当前集合中移除所有字符。
		/// </summary>
		public override void Clear()
		{
			count = 0;
			ranges.Clear();
		}

		/// <summary>
		/// 确定当前集合是否包含指定字符。
		/// </summary>
		/// <param name="ch">要在当前集合中定位的字符。</param>
		/// <returns>如果在当前集合中找到 <paramref name="ch"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(char ch)
		{
			Node? node = ranges.FindLE(ch);
			return node != null && node.Value >= ch;
		}

		/// <summary>
		/// 从当前集合中移除指定字符。
		/// </summary>
		/// <param name="ch">要从当前集合中移除的字符。</param>
		/// <returns>如果已从当前集合中成功移除 <paramref name="ch"/>，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在原始当前集合中没有找到 <paramref name="ch"/>，该方法也会返回 <c>false</c>。</returns>
		public override bool Remove(char ch)
		{
			return Remove(ch, ch);
		}

		#endregion // ICollection<char> 成员

		#region IEnumerable<char> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{Char}"/>。</returns>
		public override IEnumerator<char> GetEnumerator()
		{
			foreach (var node in ranges)
			{
				char end = node.Value;
				// 避免 end 正好是 char.MaxValue 时导致死循环。
				for (char ch = node.Key; ch < end; ch++)
				{
					yield return ch;
				}
				yield return end;
			}
		}

		#endregion // IEnumerable<char> 成员

		#region IEquatable<CharSet> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">一个与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="NotImplementedException"></exception>
		public bool Equals(CharSet? other)
		{
			if (other == null)
			{
				return false;
			}
			return count == other.Count && ContainsAllElements(other);
		}

		#endregion // IEquatable<CharSet> 成员

		/// <summary>
		/// 返回当前集合的字符串表示。
		/// </summary>
		/// <returns>当前集合的字符串表示。</returns>
		public override string ToString()
		{
			StringBuilder builder = new();
			builder.Append('[');
			foreach (var node in ranges)
			{
				builder.Append(node.Key);
				if (node.Key + 1 < node.Value)
				{
					builder.Append('-');
				}
				if (node.Key != node.Value)
				{
					builder.Append(node.Value);
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
			else if (obj is CharSet set)
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
			HashCode hashCode = new();
			foreach (Node node in ranges)
			{
				hashCode.Add(node.Key);
				hashCode.Add(node.Value);
			}
			return hashCode.GetHashCode();
		}
	}
}