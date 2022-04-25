using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示字符集合中的底层数组项。
	/// </summary>
	internal sealed class CharSetItem : IEnumerable<ItemRange<char>>, IEquatable<CharSetItem>
	{
		/// <summary>
		/// 全部为 <c>1</c> 的数组，用于简化访问。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ulong[] FullFilledData = new ulong[CharSetConfig.BtmLen] {
			ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
			ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
			ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
			ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
		};

		/// <summary>
		/// 集合中字符的高位比特。
		/// </summary>
		private readonly int highBits;
		/// <summary>
		/// 字符集合的底层数组项,使用 <c>null</c> 表示全为 <c>0</c>，使用 <c>[]</c> 表示全为 <c>1</c>。
		/// </summary>
		private ulong[]? data;
		/// <summary>
		/// 集合中字符的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count = 0;

		/// <summary>
		/// 使用指定的高位比特初始化。
		/// </summary>
		/// <param name="highBits">高位比特。</param>
		public CharSetItem(int highBits)
		{
			this.highBits = highBits;
		}

		/// <summary>
		/// 获取当前项中包含的字符个数。
		/// </summary>
		public int Count => count;

		/// <summary>
		/// 返回指定的索引是否包含指定的值。
		/// </summary>
		/// <param name="index">要检查的索引。</param>
		/// <param name="mask">要检查的值。</param>
		/// <returns>如果包含指定的值，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(int index, ulong mask)
		{
			if (data == null)
			{
				return false;
			}
			if (data.Length == 0)
			{
				return true;
			}
			return (data[index] & mask) != 0U;
		}

		/// <summary>
		/// 返回指定的索引是否全部为 1。
		/// </summary>
		/// <param name="startIndex">要检查的起始索引（含）。</param>
		/// <param name="endIndex">要检查的结束索引（不含）。</param>
		/// <returns>如果指定的范围全部为 <c>1</c>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool ContainsRange(int startIndex, int endIndex)
		{
			if (startIndex >= endIndex)
			{
				return true;
			}
			if (data == null)
			{
				return false;
			}
			if (data.Length == 0)
			{
				return true;
			}
			for (int i = startIndex; i < endIndex; i++)
			{
				if (data[i] != ulong.MaxValue)
				{
					return false;
				}
			}
			return true;
		}

		#region 按位操作

		/// <summary>
		/// 将指定索引的指定位置填充为 <c>1</c>。
		/// </summary>
		/// <param name="index">要填充的索引。</param>
		/// <param name="mask">要填充的位置。</param>
		/// <returns>受影响的字符个数。</returns>
		public int Fill(int index, ulong mask)
		{
			EnsureData();
			if (data.Length == 0)
			{
				// 已经全部是 1，不需要再处理。
				return 0;
			}
			ulong value = data[index];
			int modifiedCount = mask.CountBits() - (value & mask).CountBits();
			value |= mask;
			data[index] = value;
			count += modifiedCount;
			if (count == CharSetConfig.BtmCount)
			{
				OptimizeFullFilled();
			}
			return modifiedCount;
		}

		/// <summary>
		/// 将当前项全部填充为 <c>1</c>。
		/// </summary>
		/// <returns>受影响的字符个数。</returns>
		public int Fill()
		{
			if (data == null)
			{
				data = Array.Empty<ulong>();
				count = CharSetConfig.BtmCount;
				return count;
			}
			else if (data.Length == 0)
			{
				// 已经全部是 1，不需要再处理。
				return 0;
			}
			int modifiedCount = CharSetConfig.BtmCount - count;
			count = CharSetConfig.BtmCount;
			OptimizeFullFilled();
			return modifiedCount;
		}

		/// <summary>
		/// 将指定的索引按位填充为 <c>1</c>。
		/// </summary>
		/// <param name="startIndex">要填充的起始索引（含）。</param>
		/// <param name="endIndex">要填充的结束索引（不含）。</param>
		/// <returns>受影响的字符个数。</returns>
		public int FillRange(int startIndex, int endIndex)
		{
			if (startIndex >= endIndex)
			{
				return 0;
			}
			if (startIndex == 0 && endIndex == CharSetConfig.BtmLen)
			{
				return Fill();
			}
			EnsureData();
			if (data.Length == 0)
			{
				// 已经全部是 1，不需要再处理。
				return 0;
			}
			int modifiedCount = 0;
			for (int i = startIndex; i < endIndex; i++)
			{
				modifiedCount += 64 - data[i].CountBits();
				data[i] = ulong.MaxValue;
			}
			count += modifiedCount;
			if (count == CharSetConfig.BtmCount)
			{
				OptimizeFullFilled();
			}
			return modifiedCount;
		}

		/// <summary>
		/// 将指定索引的指定位置填充为 <c>0</c>。
		/// </summary>
		/// <param name="index">要填充的索引。</param>
		/// <param name="mask">要填充的位置。</param>
		/// <returns>受影响的字符个数。</returns>
		public int Clear(int index, ulong mask)
		{
			if (data == null)
			{
				// 已经全部是 0，不需要再处理。
				return 0;
			}
			ExpandData();
			ulong value = data[index];
			int modifiedCount = -(value & mask).CountBits();
			value &= ~mask;
			data[index] = value;
			count += modifiedCount;
			if (count == 0)
			{
				OptimizeCleared();
			}
			return modifiedCount;
		}

		/// <summary>
		/// 将当前项填充为 <c>0</c>。
		/// </summary>
		/// <returns>受影响的字符个数。</returns>
		public int Clear()
		{
			if (data == null)
			{
				// 已经全部是 0，不需要再处理。
				return 0;
			}
			int modifiedCount = -count;
			count = 0;
			OptimizeCleared();
			return modifiedCount;
		}

		/// <summary>
		/// 将指定的索引按位填充为 <c>0</c>。
		/// </summary>
		/// <param name="startIndex">要填充的起始索引（含）。</param>
		/// <param name="endIndex">要填充的结束索引（不含）。</param>
		/// <returns>受影响的字符个数。</returns>
		public int ClearRange(int startIndex, int endIndex)
		{
			if (startIndex >= endIndex)
			{
				return 0;
			}
			if (data == null)
			{
				// 已经全部是 0，不需要再处理。
				return 0;
			}
			if (startIndex == 0 && endIndex == CharSetConfig.BtmLen)
			{
				return Clear();
			}
			ExpandData();
			int modifiedCount = 0;
			for (int i = startIndex; i < endIndex; i++)
			{
				modifiedCount -= data[i].CountBits();
				data[i] = 0;
			}
			count += modifiedCount;
			if (count == 0)
			{
				OptimizeCleared();
			}
			return modifiedCount;
		}

		#endregion // 按位操作

		#region 集合操作

		/// <summary>
		/// 确定当前项是否包含指定项中的所有字符。
		/// </summary>
		/// <param name="other">要与当前项进行比较的项。</param>
		/// <returns>如果包含 <paramref name="other"/> 中的所有字符，则返回 
		/// <c>true</c>，否则返回 <c>false</c>。</returns>
		public bool ContainsAllElements(CharSetItem other)
		{
			ulong[]? otherData = other.data;
			if (otherData == null)
			{
				return true;
			}
			if (count < other.count)
			{
				return false;
			}
			// 已经全部是 1，一定能够包含。
			if (data!.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				ulong value = data![i];
				ulong otherValue = otherData![i];
				if ((value | otherValue) != value)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 从当前项内移除指定项中的所有元素。
		/// </summary>
		/// <param name="other">要移除字符的项。</param>
		/// <returns>受影响的字符个数。</returns>
		public int ExceptWith(CharSetItem other)
		{
			ulong[]? otherData = other.data;
			// 已经全部是 0，不需要再处理。
			if (data == null || otherData == null)
			{
				return 0;
			}
			if (otherData.Length == 0)
			{
				// 全部排除。
				return Clear();
			}
			ExpandData();
			int modifiedCount = 0;
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				ulong value = data[i];
				ulong removed = value & otherData[i];
				if (removed > 0UL)
				{
					modifiedCount -= removed.CountBits();
					value &= ~removed;
					data[i] = value;
				}
			}
			count += modifiedCount;
			if (count == 0)
			{
				OptimizeCleared();
			}
			return modifiedCount;
		}

		/// <summary>
		/// 修改当前项，使当前项仅包含指定项中也存在的元素。
		/// </summary>
		/// <param name="other">要与当前项进行比较的项。</param>
		/// <returns>受影响的字符个数。</returns>
		public int IntersectWith(CharSetItem other)
		{
			if (data == null)
			{
				return 0;
			}
			ulong[]? otherData = other.data;
			if (otherData == null)
			{
				// 全部排除。
				return Clear();
			}
			else if (otherData.Length == 0)
			{
				return 0;
			}
			ExpandData();
			int modifiedCount = 0;
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				ulong value = data[i];
				ulong removed = value & ~otherData[i];
				if (removed > 0UL)
				{
					modifiedCount -= removed.CountBits();
					value &= ~removed;
					data[i] = value;
				}
			}
			count += modifiedCount;
			if (count == 0)
			{
				OptimizeCleared();
			}
			return modifiedCount;
		}

		/// <summary>
		/// 确定当前项是否与指定的项重叠。
		/// </summary>
		/// <param name="other">要与当前项进行比较的项。</param>
		/// <returns>如果当前项与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Overlaps(CharSetItem other)
		{
			ulong[]? otherData = other.data;
			// 已经全部是 0，不需要再处理。
			if (data == null || otherData == null)
			{
				return false;
			}
			// 已经全部是 1，不需要再处理。
			if (data.Length == 0 || otherData.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				if ((data[i] & otherData[i]) > 0U)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 修改当前项，使该项仅包含当前项或指定项中存在的元素（但不可包含两者共有的元素）。
		/// </summary>
		/// <param name="other">要与当前项进行比较的项。</param>
		public int SymmetricExceptWith(CharSetItem other)
		{
			ulong[]? otherData = other.data;
			if (otherData == null)
			{
				return 0;
			}
			else if (otherData.Length == 0)
			{
				return Clear();
			}
			else if (data == null)
			{
				// 复制数据。
				count = other.count;
				CopyData(otherData);
				return other.count;
			}
			else if (data.Length == 0)
			{
				return ExceptWith(other);
			}
			int modifiedCount = 0;
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				ulong value = data[i];
				if (value > 0)
				{
					modifiedCount -= value.CountBits();
				}
				value ^= otherData[i];
				data[i] = value;
				if (value > 0)
				{
					modifiedCount += value.CountBits();
				}
			}
			count += modifiedCount;
			return modifiedCount;
		}

		/// <summary>
		/// 修改当前项，使该项包含当前项和指定项中同时存在的所有元素。
		/// </summary>
		/// <param name="other">要与当前项进行比较的项。</param>
		/// <returns>受影响的字符个数。</returns>
		public int UnionWith(CharSetItem other)
		{
			ulong[]? otherData = other.data;
			if (otherData == null)
			{
				return 0;
			}
			if (data == null)
			{
				count = other.count;
				CopyData(otherData);
				return other.count;
			}
			else if (data.Length == 0)
			{
				// 已经全部为 1，不需要再处理。
				return 0;
			}
			int modifiedCount = 0;
			if (otherData.Length == 0)
			{
				modifiedCount = CharSetConfig.BtmCount - count;
				count = modifiedCount;
				OptimizeFullFilled();
			}
			else
			{
				for (int i = 0; i < CharSetConfig.BtmLen; i++)
				{
					// 新增的字符。
					ulong value = data[i];
					ulong mask = (~value) & otherData[i];
					if (mask > 0)
					{
						modifiedCount += mask.CountBits();
						value |= mask;
						data[i] = value;
					}
				}
				count += modifiedCount;
				if (count == CharSetConfig.BtmCount)
				{
					OptimizeFullFilled();
				}
			}
			return modifiedCount;
		}

		#endregion // 集合操作

		/// <summary>
		/// 确保 data 不为空数组（表示全部为 <c>1</c>）。
		/// </summary>
		private void ExpandData()
		{
			if (data != null && data.Length == 0)
			{
				data = ArrayPool<ulong>.Shared.Rent(CharSetConfig.BtmLen);
				// 从 ArrayPool 获取的数组需要自行清零。
				data.Fill(ulong.MaxValue, 0, CharSetConfig.BtmLen);
			}
		}

		/// <summary>
		/// 确保 data 不为 <c>null</c>。
		/// </summary>
		[MemberNotNull("data")]
		private void EnsureData()
		{
			if (data == null)
			{
				data = ArrayPool<ulong>.Shared.Rent(CharSetConfig.BtmLen);
				// 从 ArrayPool 获取的数组需要自行清零。
				data.Fill(0UL, 0, CharSetConfig.BtmLen);
			}
		}

		/// <summary>
		/// 从指定的数组复制数据。
		/// </summary>
		/// <param name="other">要复制的数组。</param>
		private void CopyData(ulong[] other)
		{
			// 可以直接复制数据。
			if (other.Length == 0)
			{
				data = Array.Empty<ulong>();
			}
			else
			{
				data = ArrayPool<ulong>.Shared.Rent(CharSetConfig.BtmLen);
				other.CopyTo(data, 0);
			}
		}

		/// <summary>
		/// 数组已全部填满，优化内存。
		/// </summary>
		private void OptimizeFullFilled()
		{
			ArrayPool<ulong>.Shared.Return(data!);
			data = Array.Empty<ulong>();
		}

		/// <summary>
		/// 数组已全部清空，优化内存。
		/// </summary>
		private void OptimizeCleared()
		{
			// 调用方确保 data 不为 null。
			if (data!.Length > 0)
			{
				ArrayPool<ulong>.Shared.Return(data!);
			}
			data = null;
		}

		#region IEnumerable<ItemRange<char>> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{Char}"/>。</returns>
		public IEnumerator<ItemRange<char>> GetEnumerator()
		{
			if (data == null)
			{
				yield break;
			}
			if (data.Length == 0)
			{
				yield return new ItemRange<char>((char)highBits, (char)(highBits | CharSetConfig.BtmMask));
				yield break;
			}
			char start = '\0', end = '\0';
			bool hasRange = false;
			ulong[] items = data.Length == 0 ? FullFilledData : data;
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				int midBits = highBits | (i << CharSetConfig.BtmShift);
				ulong value = items[i];
				if (value == ulong.MaxValue)
				{
					char ch = (char)midBits;
					if (hasRange)
					{
						if (end < ch - 1)
						{
							yield return new ItemRange<char>(start, end);
							start = ch;
						}
					}
					else
					{
						start = ch;
					}
					end = (char)(midBits | CharSetConfig.IndexMask);
					hasRange = true;
				}
				else
				{
					for (int n = -1; value > 0UL;)
					{
						int oneIdx = (value & 1UL) == 1UL ? 1 : value.CountTrailingZeroBits() + 1;
						if (oneIdx == 64)
						{
							// C# 中 ulong 右移 64 位会不变。
							value = 0U;
						}
						else
						{
							value >>= oneIdx;
						}
						n += oneIdx;
						char ch = (char)(midBits | n);
						if (hasRange)
						{
							if (end < ch - 1)
							{
								yield return new ItemRange<char>(start, end);
								start = ch;
							}
						}
						else
						{
							start = ch;
						}
						end = ch;
						hasRange = true;
					}
				}
			}
			if (hasRange)
			{
				yield return new ItemRange<char>(start, end);
			}
		}

		#endregion // IEnumerable<ItemRange<char>> 成员

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

		#region IEquatable<CharSetItem> 成员

		/// <summary>
		/// 返回当前对象是否等于同一类型的另一对象。
		/// </summary>
		/// <param name="other">要比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Equals(CharSetItem? other)
		{
			if (other == null)
			{
				return false;
			}
			return highBits == other.highBits && count == other.count && data.ContentEquals(other.data);
		}

		/// <summary>
		/// 返回当前对象是否等于另一对象。
		/// </summary>
		/// <param name="obj">要与当前对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
		public override bool Equals(object? obj)
		{
			if (obj is CharSetItem other)
			{
				return Equals(other);
			}
			return false;
		}

		/// <summary>
		/// 返回当前对象的哈希值。
		/// </summary>
		/// <returns>当前对象的哈希值。</returns>
		public override int GetHashCode()
		{
			HashCode hashCode = new();
			hashCode.Add(highBits);
			hashCode.Add(count);
			if (data != null)
			{
				for (int i = 0; i < CharSetConfig.BtmLen; i++)
				{
					hashCode.Add(data[i]);
				}
			}
			return hashCode.GetHashCode();
		}

		/// <summary>
		/// 返回指定的 <see cref="CharSetItem"/> 是否相等。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator ==(CharSetItem? left, CharSetItem? right)
		{
			if (ReferenceEquals(left, right))
			{
				return true;
			}
			if (left is null)
			{
				return false;
			}
			return left.Equals(right);
		}

		/// <summary>
		/// 返回指定的 <see cref="CharSetItem"/> 是否不相等。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator !=(CharSetItem? left, CharSetItem? right)
		{
			if (ReferenceEquals(left, right))
			{
				return false;
			}
			if (left is null)
			{
				return true;
			}
			return !left.Equals(right);
		}

		#endregion // IEquatable<CharSetItem> 成员

	}
}
