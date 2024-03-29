using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cyjb.Collections;

/// <summary>
/// 表示字符集合中的底层数组项。
/// </summary>
internal sealed class CharSetItem : IEnumerable<ValueRange<char>>, IEquatable<CharSetItem>
{
	/// <summary>
	/// 全部为 <c>0</c> 的数组，用于简化访问。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static readonly ulong[] EmptyData = new ulong[CharSetConfig.BtmLen] {
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
	};
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
	/// 字符集合的底层数组项,使用 <see cref="EmptyData"/> 表示全为 <c>0</c>，
	/// 使用 <see cref="FullFilledData"/> 表示全为 <c>1</c>。
	/// </summary>
	private ulong[] data = EmptyData;
	/// <summary>
	/// 集合中字符的个数。
	/// </summary>
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
	/// 获取当前项是否为空。
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsEmpty() => count == 0;

	/// <summary>
	/// 获取当前项是否已填满。
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsFullFilled() => count == CharSetConfig.BtmCount;

	/// <summary>
	/// 返回指定的索引是否包含指定的值。
	/// </summary>
	/// <param name="index">要检查的索引。</param>
	/// <param name="mask">要检查的值。</param>
	/// <returns>如果包含指定的值，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool Contains(int index, ulong mask)
	{
		return (data[index] & mask) == mask;
	}

	/// <summary>
	/// 返回指定的索引是否全部为 1。
	/// </summary>
	/// <param name="startIndex">要检查的起始索引（含）。</param>
	/// <param name="endIndex">要检查的结束索引（不含）。</param>
	/// <returns>如果指定的范围全部为 <c>1</c>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool ContainsRange(int startIndex, int endIndex)
	{
		if (startIndex >= endIndex || IsFullFilled())
		{
			return true;
		}
		if (IsEmpty())
		{
			return false;
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

	/// <summary>
	/// 返回指定的索引是否包含指定的值。
	/// </summary>
	/// <param name="index">要检查的索引。</param>
	/// <param name="mask">要检查的值。</param>
	/// <returns>如果包含指定的值，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool ContainsAny(int index, ulong mask)
	{
		return (data[index] & mask) != 0UL;
	}

	/// <summary>
	/// 返回指定的索引是否不全部为 0。
	/// </summary>
	/// <param name="startIndex">要检查的起始索引（含）。</param>
	/// <param name="endIndex">要检查的结束索引（不含）。</param>
	/// <returns>如果指定的范围不全部为 <c>0</c>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool ContainsAnyRange(int startIndex, int endIndex)
	{
		if (startIndex >= endIndex || IsFullFilled())
		{
			return true;
		}
		if (IsEmpty())
		{
			return false;
		}
		for (int i = startIndex; i < endIndex; i++)
		{
			if (data[i] != 0UL)
			{
				return true;
			}
		}
		return false;
	}

	#region 按位操作

	/// <summary>
	/// 将指定索引的指定单个比特位填充为 <c>1</c>。
	/// </summary>
	/// <param name="index">要填充的索引。</param>
	/// <param name="mask">要填充的位置。</param>
	/// <returns>受影响的字符个数。</returns>
	public int FillSingle(int index, ulong mask)
	{
		ulong value = data[index];
		if ((value & mask) > 0UL)
		{
			return 0;
		}
		EnsureData();
		value |= mask;
		data[index] = value;
		count++;
		if (IsFullFilled())
		{
			OptimizeFullFilled();
		}
		return 1;
	}

	/// <summary>
	/// 将指定索引的指定位置填充为 <c>1</c>。
	/// </summary>
	/// <param name="index">要填充的索引。</param>
	/// <param name="mask">要填充的位置。</param>
	/// <returns>受影响的字符个数。</returns>
	public int Fill(int index, ulong mask)
	{
		if (IsFullFilled())
		{
			return 0;
		}
		ulong value = data[index];
		int modifiedCount = mask.CountBits() - (value & mask).CountBits();
		if (modifiedCount == 0)
		{
			return 0;
		}
		EnsureData();
		value |= mask;
		data[index] = value;
		count += modifiedCount;
		if (IsFullFilled())
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
		if (IsFullFilled())
		{
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
		if (startIndex >= endIndex || IsFullFilled())
		{
			return 0;
		}
		if (startIndex == 0 && endIndex == CharSetConfig.BtmLen)
		{
			return Fill();
		}
		EnsureData();
		int modifiedCount = 0;
		for (int i = startIndex; i < endIndex; i++)
		{
			modifiedCount += 64 - data[i].CountBits();
			data[i] = ulong.MaxValue;
		}
		count += modifiedCount;
		if (IsFullFilled())
		{
			OptimizeFullFilled();
		}
		return modifiedCount;
	}

	/// <summary>
	/// 将指定索引的指定单个位填充为 <c>0</c>。
	/// </summary>
	/// <param name="index">要填充的索引。</param>
	/// <param name="mask">要填充的位置。</param>
	/// <returns>受影响的字符个数。</returns>
	public int ClearSingle(int index, ulong mask)
	{
		ulong value = data[index];
		if ((value & mask) == 0UL)
		{
			return 0;
		}
		ExpandData();
		value &= ~mask;
		data[index] = value;
		count -= 1;
		if (IsEmpty())
		{
			OptimizeCleared();
		}
		return -1;
	}

	/// <summary>
	/// 将指定索引的指定位置填充为 <c>0</c>。
	/// </summary>
	/// <param name="index">要填充的索引。</param>
	/// <param name="mask">要填充的位置。</param>
	/// <returns>受影响的字符个数。</returns>
	public int Clear(int index, ulong mask)
	{
		if (IsEmpty())
		{
			return 0;
		}
		ulong value = data[index];
		int modifiedCount = -(value & mask).CountBits();
		if (modifiedCount == 0)
		{
			return 0;
		}
		ExpandData();
		value &= ~mask;
		data[index] = value;
		count += modifiedCount;
		if (IsEmpty())
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
		if (IsEmpty())
		{
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
		if (startIndex >= endIndex || IsEmpty())
		{
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
		if (IsEmpty())
		{
			OptimizeCleared();
		}
		return modifiedCount;
	}

	/// <summary>
	/// 反转当前集合，使集合内容变为之前未包含的字符。
	/// </summary>
	/// <returns>受影响的字符个数。</returns>
	public int Negated()
	{
		if (IsEmpty())
		{
			data = FullFilledData;
			count = CharSetConfig.BtmCount;
			return count;
		}
		else if (IsFullFilled())
		{
			data = EmptyData;
			count = 0;
			return -CharSetConfig.BtmCount;
		}
		else
		{
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				data[i] = ~data[i];
			}
			int oldCount = count;
			count = CharSetConfig.BtmCount - count;
			return count - oldCount;
		}
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
		if (count < other.count)
		{
			return false;
		}
		if (other.IsEmpty() || IsFullFilled())
		{
			return true;
		}
		ulong[] otherData = other.data;
		for (int i = 0; i < CharSetConfig.BtmLen; i++)
		{
			ulong value = data[i];
			ulong otherValue = otherData[i];
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
		if (IsEmpty() || other.IsEmpty())
		{
			return 0;
		}
		if (other.IsFullFilled())
		{
			// 全部排除。
			return Clear();
		}
		ExpandData();
		int modifiedCount = 0;
		ulong[] otherData = other.data;
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
		if (IsEmpty() || other.IsFullFilled())
		{
			return 0;
		}
		if (other.IsEmpty())
		{
			// 全部排除。
			return Clear();
		}
		ExpandData();
		ulong[] otherData = other.data;
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
		if (IsEmpty() || other.IsEmpty())
		{
			return false;
		}
		if (IsFullFilled() || other.IsFullFilled())
		{
			return true;
		}
		ulong[] otherData = other.data;
		for (int i = 0; i < CharSetConfig.BtmLen; i++)
		{
			if ((data[i] & otherData[i]) > 0UL)
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
		if (other.IsEmpty())
		{
			return 0;
		}
		if (IsFullFilled())
		{
			return ExceptWith(other);
		}
		ulong[] otherData = other.data;
		if (IsEmpty())
		{
			// 复制数据。
			count = other.count;
			CopyData(otherData);
			return other.count;
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
		if (IsFullFilled() || other.IsEmpty())
		{
			return 0;
		}
		if (other.IsFullFilled())
		{
			count = CharSetConfig.BtmCount;
			OptimizeFullFilled();
			return count;
		}
		ulong[] otherData = other.data;
		if (IsEmpty())
		{
			count = other.count;
			CopyData(otherData);
			return other.count;
		}

		int modifiedCount = 0;
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
		if (IsFullFilled())
		{
			OptimizeFullFilled();
		}
		return modifiedCount;
	}

	#endregion // 集合操作

	/// <summary>
	/// 确保 data 不为 <see cref="FullFilledData"/>。
	/// </summary>
	private void ExpandData()
	{
		if (data == FullFilledData)
		{
			data = ArrayPool<ulong>.Shared.Rent(CharSetConfig.BtmLen);
			// 从 ArrayPool 获取的数组需要自行初始化。
			Array.Fill(data, ulong.MaxValue, 0, CharSetConfig.BtmLen);
		}
	}

	/// <summary>
	/// 确保 data 不为 <see cref="EmptyData"/>。
	/// </summary>
	private void EnsureData()
	{
		if (data == EmptyData)
		{
			data = ArrayPool<ulong>.Shared.Rent(CharSetConfig.BtmLen);
			// 从 ArrayPool 获取的数组需要自行初始化。
			Array.Fill(data, 0UL, 0, CharSetConfig.BtmLen);
		}
	}

	/// <summary>
	/// 从指定的数组复制数据。
	/// </summary>
	/// <param name="other">要复制的数组。</param>
	private void CopyData(ulong[] other)
	{
		// 可以直接复制数据。
		if (other == FullFilledData)
		{
			data = FullFilledData;
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
		if (data != EmptyData)
		{
			ArrayPool<ulong>.Shared.Return(data);
		}
		data = FullFilledData;
	}

	/// <summary>
	/// 数组已全部清空，优化内存。
	/// </summary>
	private void OptimizeCleared()
	{
		if (data != EmptyData && data != FullFilledData)
		{
			ArrayPool<ulong>.Shared.Return(data!);
		}
		data = EmptyData;
	}

	/// <summary>
	/// 计算范围的端点个数。
	/// </summary>
	/// <param name="lastBit">前一 bit 的值。</param>
	/// <returns>范围的端点个数。</returns>
	public int PointCount(ref ulong lastBit)
	{
		int pCount;
		if (count == 0)
		{
			pCount = (int)lastBit;
			lastBit = 0UL;
			return pCount;
		}
		if (count == CharSetConfig.BtmCount)
		{
			pCount = 1 - (int)lastBit;
			lastBit = 1UL;
			return pCount;
		}
		pCount = 0;
		for (int i = 0; i < CharSetConfig.BtmLen; i++)
		{
			ulong value = data[i];
			if (value == ulong.MaxValue)
			{
				if (lastBit == 0UL)
				{
					pCount++;
					lastBit = 1UL;
				}
			}
			else
			{
				pCount += (value ^ (value << 1 | lastBit)).CountBits();
				lastBit = (value & 0x8000000000000000UL) > 0 ? 1UL : 0UL;
			}
		}
		return pCount;
	}

	#region IEnumerable<ItemRange<char>> 成员

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{Char}"/>。</returns>
	public IEnumerator<ValueRange<char>> GetEnumerator()
	{
		if (count == 0)
		{
			yield break;
		}
		if (count == CharSetConfig.BtmCount)
		{
			yield return new ValueRange<char>((char)highBits, (char)(highBits | CharSetConfig.BtmMask));
			yield break;
		}
		char start = '\0', end = '\0';
		bool hasRange = false;
		for (int i = 0; i < CharSetConfig.BtmLen; i++)
		{
			int midBits = highBits | (i << CharSetConfig.BtmShift);
			ulong value = data[i];
			if (value == ulong.MaxValue)
			{
				char ch = (char)midBits;
				if (hasRange)
				{
					if (end < ch - 1)
					{
						yield return new ValueRange<char>(start, end);
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
							yield return new ValueRange<char>(start, end);
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
			yield return new ValueRange<char>(start, end);
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
		if (other is null)
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
		if (!IsEmpty())
		{
			for (int i = 0; i < CharSetConfig.BtmLen; i++)
			{
				hashCode.Add(data[i]);
			}
		}
		return hashCode.ToHashCode();
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
