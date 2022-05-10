using System.Collections;
using System.Diagnostics;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections
{
	/// <summary>
	/// 管理位值的压缩列表，该值表示为布尔值，其中 <c>true</c> 表示位是打开的 (1)，<c>false</c> 表示位是关闭的 (0)。
	/// </summary>
	/// <remarks>
	/// <para><see cref="BitList"/> 类采用位示图来保存布尔值，关于该数据结构的更多解释，请参见我的博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/BitList.html">《C# 位压缩列表》</see>。</para>
	/// <para>由于位操作的复杂性，<see cref="BitList"/> 类的一些方法效率并不高，实际使用时需要做好相应的测试。</para>
	/// </remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/BitList.html">《C# 位压缩列表》</seealso>
	public sealed class BitList : ListBase<bool>, IEquatable<BitList>, ICollection
	{
		/// <summary>
		/// UInt32 索引的掩码。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IndexMask = 0x1F;
		/// <summary>
		/// UInt32 索引的位移量。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int IndexShift = 5;
		/// <summary>
		/// 每个存储单元的大小。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int UnitSize = 32;

		/// <summary>
		/// 当前列表的容量。
		/// </summary>
		private int capacity;
		/// <summary>
		/// 保存位值的数组。
		/// </summary>
		private uint[] items;
		/// <summary>
		/// 当前包含的位值个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;

		#region 构造函数

		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public BitList()
		{
			items = new uint[1];
			capacity = 1 << IndexShift;
		}

		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例，该实例可拥有指定的初始容量。
		/// </summary>
		/// <param name="capacity">新 <see cref="BitList"/> 最初可以存储的元素数。</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 <c>0</c>。</exception>
		public BitList(int capacity) : base()
		{
			if (capacity < 0)
			{
				throw CommonExceptions.ArgumentNegative(capacity);
			}
			items = new uint[(capacity >> IndexShift) + 1];
			this.capacity = items.Length << IndexShift;
		}

		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例，该实例包含从指定集合复制的元素。
		/// </summary>
		/// <param name="collection">一个集合，其元素被复制到新列表中，其中每个整数表示 32 个连续位。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public BitList(IEnumerable<uint> collection) : this()
		{
			ArgumentNullException.ThrowIfNull(collection);
			AddRange(collection);
		}

		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例，该实例包含从指定集合复制的元素。
		/// </summary>
		/// <param name="collection">一个集合，其元素被复制到新列表中，其中每个整数表示 32 个连续位。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public BitList(IEnumerable<int> collection) : this()
		{
			ArgumentNullException.ThrowIfNull(collection);
			AddRange(collection);
		}

		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例，该实例包含从指定集合复制的元素。
		/// </summary>
		/// <param name="collection">一个集合，其元素被复制到新列表中，其中每个字节表示 8 个连续位。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public BitList(IEnumerable<byte> collection) : this()
		{
			ArgumentNullException.ThrowIfNull(collection);
			AddRange(collection);
		}

		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例，该实例包含从指定集合复制的元素。
		/// </summary>
		/// <param name="collection">一个集合，其元素被复制到新列表中。</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public BitList(IEnumerable<bool> collection) : this()
		{
			ArgumentNullException.ThrowIfNull(collection);
			AddRange(collection);
		}

		/// <summary>
		/// 初始化 <see cref="BitList"/> 类的新实例，该实例包含指定数目的位值，并设定为指定的初始值。
		/// </summary>
		/// <param name="length">新 <see cref="BitList"/> 中的位值数目。</param>
		/// <param name="defaultValue">要分配给每个位值的布尔值。</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		public BitList(int length, bool defaultValue) : this(length)
		{
			if (length < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange(length);
			}
			count = length;
			if (defaultValue)
			{
				Fill(0, length, true);
			}
		}

		#endregion // 构造函数

		/// <summary>
		/// 获取或设置 <see cref="BitList"/> 在不调整大小的情况下能够容纳的元素总数。
		/// </summary>
		/// <value>在需要调整大小之前 <see cref="BitList"/> 能够容纳的元素的数目。</value>
		/// <exception cref="ArgumentOutOfRangeException"><c>Capacity</c> 设置为小于 <see cref="Count"/> 的值。</exception>
		public int Capacity
		{
			get { return capacity; }
			set
			{
				if (value < count)
				{
					throw CommonExceptions.ArgumentOutOfRange(value);
				}
				int newLength = (value >> IndexShift) + 1;
				if (newLength == items.Length)
				{
					return;
				}
				uint[] newData = new uint[newLength];
				Array.Copy(items, newData, Math.Min(newLength, items.Length));
				items = newData;
				capacity = newLength << IndexShift;
			}
		}

		/// <summary>
		/// 将当前列表调整为指定的新大小。
		/// </summary>
		/// <param name="newCount">要调整到的大小。</param>
		/// <returns>调整后的当前列表。</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="newCount"/> 小于零。</exception>
		public BitList Resize(int newCount)
		{
			if (newCount < 0)
			{
				throw CommonExceptions.ArgumentNegative(newCount);
			}
			if (newCount < count)
			{
				RemoveRange(newCount, count - newCount);
			}
			else if (newCount > count)
			{
				AddRange(newCount - count, false);
			}
			return this;
		}

		#region AddRange 操作

		/// <summary>
		/// 将指定集合的元素添加到 <see cref="BitList"/> 的末尾。
		/// </summary>
		/// <param name="collection">一个集合，其元素应被添加到 <see cref="BitList"/> 的末尾，
		/// 其中每个整数表示 32 个连续位。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 将指定集合的元素添加到 <see cref="BitList"/> 的末尾。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public BitList AddRange(IEnumerable<uint> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(count, length, list);
		}

		/// <summary>
		/// 将指定集合的元素添加到 <see cref="BitList"/> 的末尾。
		/// </summary>
		/// <param name="collection">一个集合，其元素应被添加到 <see cref="BitList"/> 的末尾，
		/// 其中每个整数表示 32 个连续位。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public BitList AddRange(IEnumerable<int> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(count, length, list);
		}

		/// <summary>
		/// 将指定集合的元素添加到 <see cref="BitList"/> 的末尾。
		/// </summary>
		/// <param name="collection">一个集合，其元素应被添加到 <see cref="BitList"/> 的末尾，
		/// 其中每个字节表示 8 个连续位。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public BitList AddRange(IEnumerable<byte> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(count, length, list);
		}

		/// <summary>
		/// 将指定集合的元素添加到 <see cref="BitList"/> 的末尾。
		/// </summary>
		/// <param name="collection">一个集合，其元素应被添加到 <see cref="BitList"/> 的末尾。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		public BitList AddRange(IEnumerable<bool> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(count, length, list);
		}

		/// <summary>
		/// 将指定长度的值添加到 <see cref="BitList"/> 的末尾。
		/// </summary>
		/// <param name="length">要添加的值的长度。</param>
		/// <param name="value">要添加的值。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		public BitList AddRange(int length, bool value)
		{
			if (length < 0)
			{
				throw CommonExceptions.ArgumentNegative(length);
			}
			int cnt = count;
			count += length;
			EnsureCapacity(count + UnitSize);
			FillInternal(cnt, length, value);
			return this;
		}

		/// <summary>
		/// 将指定集合转换为 <see cref="IList{UInt32}"/>。
		/// </summary>
		/// <param name="collection">要转换的集合。</param>
		/// <returns>转换后的 <see cref="IList{UInt32}"/></returns>
		private static (IList<uint> list, int length) ConvertToUIntList(IEnumerable<uint> collection)
		{
			IList<uint> list = collection as IList<uint> ?? new List<uint>(collection);
			return (list, list.Count << IndexShift);
		}

		/// <summary>
		/// 将指定集合转换为 <see cref="IList{UInt32}"/>。
		/// </summary>
		/// <param name="collection">要转换的集合。</param>
		/// <returns>转换后的 <see cref="IList{UInt32}"/></returns>
		private static (IList<uint> list, int length) ConvertToUIntList(IEnumerable<int> collection)
		{
			IList<uint> list = collection.Select(i => unchecked((uint)i)).ToList();
			return (list, list.Count << IndexShift);
		}

		/// <summary>
		/// 将指定集合转换为 <see cref="IList{UInt32}"/>。
		/// </summary>
		/// <param name="collection">要转换的集合。</param>
		/// <returns>转换后的 <see cref="IList{UInt32}"/></returns>
		private static (IList<uint> list, int length) ConvertToUIntList(IEnumerable<byte> collection)
		{
			List<uint> list = new();
			int length = 0;
			uint value = 0U;
			int j = 0;
			foreach (byte b in collection)
			{
				value |= unchecked((uint)(b << j));
				j += 8;
				length += 8;
				if (j == UnitSize)
				{
					list.Add(value);
					value = 0U;
					j = 0;
				}
			}
			if (j > 0)
			{
				list.Add(value);
			}
			return (list, length);
		}

		/// <summary>
		/// 将指定集合转换为 <see cref="IList{UInt32}"/>。
		/// </summary>
		/// <param name="collection">要转换的集合。</param>
		/// <returns>转换后的 <see cref="IList{UInt32}"/></returns>
		private static (IList<uint> list, int length) ConvertToUIntList(IEnumerable<bool> collection)
		{
			IList<uint> list;
			int length = 0;
			if (collection is BitList bList)
			{
				length = bList.count;
				list = bList.items;
			}
			else
			{
				list = new List<uint>();
				uint value = 0U;
				int j = 0;
				foreach (bool b in collection)
				{
					if (b)
					{
						value |= 1U << j;
					}
					j++;
					length++;
					if (j == UnitSize)
					{
						list.Add(value);
						value = 0U;
						j = 0;
					}
				}
				if (j > 0)
				{
					list.Add(value);
				}
			}
			return (list, length);
		}

		#endregion // AddRange 操作

		#region InsertRange 操作

		/// <summary>
		/// 将指定集合中的元素插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// <param name="index">应在此处插入新元素的从零开始的索引。</param>
		/// <param name="collection">一个集合，应将其元素插入到 
		/// <see cref="BitList"/> 中，其中每个整数表示 32 个连续位。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 大于 <see cref="Count"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 将指定集合中的元素插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// </overloads>
		public BitList InsertRange(int index, IEnumerable<int> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			if (index < 0 || index > count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(index, length, list);
		}

		/// <summary>
		/// 将指定集合中的元素插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// <param name="index">应在此处插入新元素的从零开始的索引。</param>
		/// <param name="collection">一个集合，应将其元素插入到 
		/// <see cref="BitList"/> 中，其中每个整数表示 32 个连续位。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 大于 <see cref="Count"/>。</exception>
		[CLSCompliant(false)]
		public BitList InsertRange(int index, IEnumerable<uint> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			if (index < 0 || index > count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(index, length, list);
		}

		/// <summary>
		/// 将指定集合中的元素插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// <param name="index">应在此处插入新元素的从零开始的索引。</param>
		/// <param name="collection">一个集合，应将其元素插入到 
		/// <see cref="BitList"/> 中，其中每个整数表示 8 个连续位。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 大于 <see cref="Count"/>。</exception>
		public BitList InsertRange(int index, IEnumerable<byte> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			if (index < 0 || index > count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(index, length, list);
		}

		/// <summary>
		/// 将指定集合中的元素插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// <param name="index">应在此处插入新元素的从零开始的索引。</param>
		/// <param name="collection">一个集合，应将其元素插入到 <see cref="BitList"/> 中。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 大于 <see cref="Count"/>。</exception>
		public BitList InsertRange(int index, IEnumerable<bool> collection)
		{
			ArgumentNullException.ThrowIfNull(collection);
			if (index < 0 || index > count)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			var (list, length) = ConvertToUIntList(collection);
			return InsertRange(index, length, list);
		}

		/// <summary>
		/// 将指定长度的值插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// <param name="index">应在此处插入值的从零开始的索引。</param>
		/// <param name="length">要插入的值的长度。</param>
		/// <param name="value">要插入的值。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 大于 <see cref="Count"/>。</exception>
		public BitList InsertRange(int index, int length, bool value)
		{
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (length < 0 || index + length > count)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(length);
			}
			int cnt = count + length;
			EnsureCapacity(cnt + UnitSize);
			if (index < count)
			{
				LeftShift(index, length);
			}
			FillInternal(index, length, value);
			count = cnt;
			return this;
		}

		/// <summary>
		/// 将指定集合中的元素插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// <param name="index">应在此处插入新元素的从零开始的索引。</param>
		/// <param name="length">要插入的位数。</param>
		/// <param name="uintList">一个集合，应将其元素插入到 <see cref="BitList"/> 中。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		private BitList InsertRange(int index, int length, IList<uint> uintList)
		{
			int cnt = count + length;
			EnsureCapacity(cnt + UnitSize);
			if (index < count)
			{
				LeftShift(index, length);
			}
			int lowSize = index & IndexMask;
			CopyItems(uintList, 0, index >> IndexShift, lowSize, length + lowSize);
			count += length;
			return this;
		}

		/// <summary>
		/// 将指定索引之后数据左移指定长度（向着索引增大的方向）。
		/// </summary>
		/// <param name="index">要左移的起始索引。</param>
		/// <param name="length">要左移的长度。</param>
		private void LeftShift(int index, int length)
		{
			int sourceIdx = (count - 1) >> IndexShift;
			int cnt = ((sourceIdx + 1) << IndexShift) + length;
			int lowSize = cnt & IndexMask;
			if (lowSize == 0)
			{
				lowSize = UnitSize;
			}
			CopyItemsBackward(sourceIdx, (cnt - 1) >> IndexShift, lowSize, cnt - length - index - lowSize + UnitSize);
		}

		#endregion // InsertRange 操作

		/// <summary>
		/// 从 <see cref="BitList"/> 中移除指定范围的元素。
		/// </summary>
		/// <param name="index">要移除的元素的范围从零开始的起始索引。</param>
		/// <param name="length">要移除的元素数。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 和 <paramref name="length"/> 不表示
		/// <see cref="BitList"/> 中元素的有效范围。</exception>
		public BitList RemoveRange(int index, int length)
		{
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (length < 0 || index + length > count)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(length);
			}
			if (length == 0)
			{
				return this;
			}
			if (length == 1)
			{
				RemoveItem(index);
				return this;
			}
			int valueIdx = (index + length) >> IndexShift;
			int idx = index >> IndexShift;
			int tailSize = index & IndexMask;
			int rSize = (index + length) & IndexMask;
			if (rSize > 0)
			{
				uint value = items[valueIdx];
				int highSize = tailSize == 0 ? 0 : UnitSize - tailSize;
				int restSize = UnitSize - rSize;
				if (highSize > restSize)
				{
					highSize = restSize;
				}
				if (highSize > 0)
				{
					uint tailMask = GetMask(tailSize);
					items[idx] = (items[idx] & tailMask) | ((value >> rSize) << tailSize);
					tailSize += highSize;
					if (tailSize == UnitSize)
					{
						idx++;
						tailSize = 0;
					}
				}
				if (restSize > highSize)
				{
					tailSize = restSize - highSize;
					items[idx] = value >> (UnitSize - tailSize);
				}
				valueIdx++;
			}
			// 计算要复制的长度。
			int len = count - (valueIdx << IndexShift) + tailSize;
			if (len > 0)
			{
				CopyItems(items, valueIdx, idx, tailSize, len);
			}
			count -= length;
			return this;
		}

		/// <summary>
		/// 将所有位填充为指定的值。
		/// </summary>
		/// <param name="value">要填充的值。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		public BitList FillAll(bool value)
		{
			if (count == 0)
			{
				return this;
			}
			FillInternal(0, count, value);
			return this;
		}

		/// <summary>
		/// 填充指定范围中的元素。
		/// </summary>
		/// <param name="index">要填充的范围的从零开始的起始索引。</param>
		/// <param name="length">要填充的范围内的元素数。</param>
		/// <param name="value">要填充的值。</param>
		/// <returns>当前 <see cref="BitList"/> 集合。</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="index"/> 和 <paramref name="length"/>
		/// 不表示 <see cref="BitList"/> 中元素的有效范围。</exception>
		public BitList Fill(int index, int length, bool value)
		{
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (length < 0 || index + length > count)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(length);
			}
			if (length == 0)
			{
				return this;
			}
			FillInternal(index, length, value);
			return this;
		}

		/// <summary>
		/// 填充指定范围中的元素。
		/// </summary>
		/// <param name="index">要填充的范围的从零开始的起始索引。</param>
		/// <param name="length">要填充的范围内的元素数。</param>
		/// <param name="value">要填充的值。</param>
		private void FillInternal(int index, int length, bool value)
		{
			uint uv = value ? uint.MaxValue : 0U;
			int idx = index >> IndexShift;
			int tailSize = index & IndexMask;
			if (tailSize > 0)
			{
				uint mask = GetMask(tailSize);
				if (length < UnitSize)
				{
					mask |= ~GetMask(length) << tailSize;
				}
				items[idx] = (items[idx] & mask) | (uv & ~mask);
				length -= UnitSize - tailSize;
				idx++;
			}
			while (length > 0)
			{
				if (length >= UnitSize)
				{
					items[idx] = uv;
					length -= UnitSize;
					idx++;
				}
				else
				{
					uint mask = GetMask(length);
					items[idx] = (items[idx] & ~mask) | (uv & mask);
					break;
				}
			}
		}

		/// <summary>
		/// 判断当前列表中的值是否全为 <c>true</c>。
		/// </summary>
		/// <returns>如果当前列表为空或其中的值全为 <c>true</c>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public bool AllTrue()
		{
			if (count == 0)
			{
				return true;
			}
			int end = count >> IndexShift;
			for (int i = 0; i < end; i++)
			{
				if (items[i] != uint.MaxValue)
				{
					return false;
				}
			}
			uint value = items[end];
			end = count - (end << IndexShift);
			return (value | ~GetMask(end)) == uint.MaxValue;
		}

		/// <summary>
		/// 判断当前列表中的值是否全为 <c>false</c>。
		/// </summary>
		/// <returns>如果当前列表为空或其中的值全为 <c>false</c>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public bool AllFalse()
		{
			if (count == 0)
			{
				return true;
			}
			int end = count >> IndexShift;
			for (int i = 0; i < end; i++)
			{
				if (items[i] != 0U)
				{
					return false;
				}
			}
			uint value = items[end];
			end = count - (end << IndexShift);
			return (value & GetMask(end)) == 0U;
		}

		#region 二进制操作

		/// <summary>
		/// 对当前 <see cref="BitList"/> 中的元素和指定 <see cref="BitList"/> 中的相应元素执行按位“与”运算。
		/// </summary>
		/// <param name="list">要对其执行按位“与”运算的 <see cref="BitList"/>。</param>
		/// <returns>当前实例，包含对当前 <see cref="BitList"/> 中的元素和指定的 <see cref="BitList"/> 
		/// 中的相应元素执行按位“与”运算的结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		public BitList And(BitList list)
		{
			ArgumentNullException.ThrowIfNull(list);
			int cnt = count >> IndexShift;
			for (int i = 0; i < cnt; i++)
			{
				items[i] &= list.items[i];
			}
			uint mask = GetMask(count - (cnt << IndexShift));
			items[cnt] &= (list.items[cnt] | ~mask);
			return this;
		}

		/// <summary>
		/// 对当前 <see cref="BitList"/> 中的元素和指定 <see cref="BitList"/> 中的相应元素执行按位“或”运算。
		/// </summary>
		/// <param name="list">要对其执行按位“或”运算的 <see cref="BitList"/>。</param>
		/// <returns>当前实例，包含对当前 <see cref="BitList"/> 中的元素和指定的 <see cref="BitList"/> 
		/// 中的相应元素执行按位“或”运算的结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		public BitList Or(BitList list)
		{
			ArgumentNullException.ThrowIfNull(list);
			int cnt = count >> IndexShift;
			for (int i = 0; i < cnt; i++)
			{
				items[i] |= list.items[i];
			}
			uint mask = GetMask(count - (cnt << IndexShift));
			items[cnt] |= (list.items[cnt] & mask);
			return this;
		}

		/// <summary>
		/// 对当前 <see cref="BitList"/> 中的元素和指定 <see cref="BitList"/> 中的相应元素执行按位“异或”运算。
		/// </summary>
		/// <param name="list">要对其执行按位“异或”运算的 <see cref="BitList"/>。</param>
		/// <returns>当前实例，包含对当前 <see cref="BitList"/> 中的元素和指定的 <see cref="BitList"/> 
		/// 中的相应元素执行按位“异或”运算的结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="list"/> 为 <c>null</c>。</exception>
		public BitList Xor(BitList list)
		{
			ArgumentNullException.ThrowIfNull(list);
			int cnt = count >> IndexShift;
			for (int i = 0; i < cnt; i++)
			{
				items[i] ^= list.items[i];
			}
			uint mask = GetMask(count - (cnt << IndexShift));
			items[cnt] ^= (list.items[cnt] & mask);
			return this;
		}

		/// <summary>
		/// 反转当前 <see cref="BitList"/> 中的所有位值，以便将设置为 <c>true</c> 的元素更改为 <c>false</c>；
		/// 将设置为 <c>false</c> 的元素更改为 <c>true</c>。
		/// </summary>
		/// <returns>当前 <see cref="BitList"/> 对象。</returns>
		public BitList Not()
		{
			int cnt = count >> IndexShift;
			for (int i = 0; i <= cnt; i++)
			{
				items[i] = ~items[i];
			}
			return this;
		}

		/// <summary>
		/// 将当前 <see cref="BitList"/> 中的所有位值左移 <paramref name="offset"/> 位。
		/// 这里的左移是向着索引增大的方向移动。
		/// </summary>
		/// <param name="offset">要左移的位数，实际的移位数会对 <see cref="Count"/> 取模。</param>
		/// <returns>当前 <see cref="BitList"/> 对象。</returns>
		public BitList LeftShift(int offset)
		{
			if (count <= 0)
			{
				return this;
			}
			int b = count.LogBase2();
			offset &= (1 << (b + 1)) - 1;
			LeftShift(0, offset);
			Fill(0, offset, false);
			return this;
		}

		/// <summary>
		/// 将当前 <see cref="BitList"/> 中的所有位值右移 <paramref name="offset"/> 位。
		/// 这里的右移是向着索引减小的方向移动。
		/// </summary>
		/// <param name="offset">要右移的位数，实际的移位数会对 <see cref="Count"/> 取模。</param>
		/// <returns>当前 <see cref="BitList"/> 对象。</returns>
		public BitList RightShift(int offset)
		{
			if (count <= 0)
			{
				return this;
			}
			int b = count.LogBase2();
			offset &= (1 << (b + 1)) - 1;
			int cnt = count;
			RemoveRange(0, offset);
			FillInternal(count, cnt - count, false);
			count = cnt;
			return this;
		}

		#endregion // 二进制操作

		#region ListBase<bool> 成员

		/// <summary>
		/// 从 <see cref="BitList"/> 中移除所有元素。
		/// </summary>
		public override void Clear()
		{
			count = 0;
		}

		/// <summary>
		/// 将元素插入 <see cref="BitList"/> 的指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, bool item)
		{
			if (count + 1 > capacity)
			{
				EnsureCapacity(count + 1);
			}
			int cnt = count >> IndexShift;
			int idx = index >> IndexShift;
			for (int i = cnt; i > idx; i--)
			{
				items[i] <<= 1;
				items[i] |= items[i - 1] >> 31;
			}
			uint value = items[idx];
			uint spliter = (1U << (index & IndexMask));
			uint lowMask = spliter - 1;
			uint lowBits = value & lowMask;
			uint highBits = value & (uint.MaxValue - lowMask);
			if (!item)
			{
				spliter = 0U;
			}
			items[idx] = (highBits << 1) | spliter | lowBits;
			count++;
		}

		/// <summary>
		/// 移除 <see cref="BitList"/> 的指定索引处的元素。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			int idx = index >> IndexShift;
			uint value = items[idx];
			uint spliter = 1U << (index & IndexMask);
			uint highBits = value & ~((spliter << 1) - 1U);
			uint lowBits = value & (spliter - 1U);
			items[idx] = (highBits >> 1) | lowBits;
			int end = count >> IndexShift;
			for (idx++; idx <= end; idx++)
			{
				items[idx - 1] |= items[idx] << 31;
				items[idx] >>= 1;
			}
			count--;
		}

		/// <summary>
		/// 返回指定索引处的元素。
		/// </summary>
		/// <param name="index">要返回元素的从零开始的索引。</param>
		/// <returns>位于指定索引处的元素。</returns>
		protected override bool GetItemAt(int index)
		{
			if (index < 0 || index >= count)
			{
				throw CommonExceptions.ArgumentOutOfRange(index);
			}
			return (items[index >> IndexShift] & (1 << (index & IndexMask))) != 0U;
		}

		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。对于引用类型，该值可以为 <c>null</c>。</param>
		protected override void SetItemAt(int index, bool item)
		{
			if (index < 0 || index >= count)
			{
				throw CommonExceptions.ArgumentOutOfRange(index);
			}
			if (item)
			{
				items[index >> IndexShift] |= (1U << (index & IndexMask));
			}
			else
			{
				items[index >> IndexShift] &= ~(1U << (index & IndexMask));
			}
		}

		#endregion // ListBase<bool> 成员

		#region IList<T> 成员

		/// <summary>
		/// 确定 <see cref="BitList"/> 中特定项的索引。
		/// </summary>
		/// <param name="item">要在 <see cref="BitList"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="BitList"/> 中找到 <paramref name="item"/>，
		/// 则为该项的索引；否则为 <c>-1</c>。</returns>
		public override int IndexOf(bool item)
		{
			int end = count >> IndexShift;
			int idx = -1;
			if (item)
			{
				for (int i = 0; i <= end; i++)
				{
					uint value = items[i];
					if (value > 0U)
					{
						idx = (i << IndexShift) + value.CountTrailingZeroBits();
						break;
					}
				}
			}
			else
			{
				for (int i = 0; i <= end; i++)
				{
					uint value = items[i];
					if (value < uint.MaxValue)
					{
						idx = (i << IndexShift) + value.CountTrailingBits();
						break;
					}
				}
			}
			return idx > count ? -1 : idx;
		}

		#endregion // IList<T> 成员

		#region ICollection<bool> 成员

		/// <summary>
		/// 获取 <see cref="BitList"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="BitList"/> 中包含的元素数。</value>
		public override int Count => count;

		#endregion // ICollection<bool> 成员

		#region ICollection 成员

		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将当前集合的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从当前集合复制的元素的目标位置的一维 <see cref="Array"/>。
		/// <paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException">当前集合中的元素数目大于从 <paramref name="index"/>
		/// 到目标 <paramref name="array"/> 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException">当前集合的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			CommonExceptions.CheckSimplyArray(array);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			if (array is uint[])
			{
				int end = 0;
				if (count > 0)
				{
					end = (count - 1) >> IndexShift;
				}
				Array.Copy(items, 0, array, index, end + 1);
			}
			else if (array is int[] iarr)
			{
				int cnt = 1;
				if (count > 0)
				{
					cnt = ((count - 1) >> IndexShift) + 1;
				}
				if (array.Length - index < cnt)
				{
					throw CommonExceptions.ArrayTooSmall(nameof(array));
				}
				for (int i = 0; i < cnt; i++)
				{
					iarr[index++] = unchecked((int)items[i]);
				}
			}
			else if (array is byte[] barr)
			{
				int cnt = 1;
				if (count > 0)
				{
					cnt = ((count - 1) / 8) + 1;
				}
				if (array.Length - index < cnt)
				{
					throw CommonExceptions.ArrayTooSmall(nameof(array));
				}
				for (int i = 0; i < cnt; i++)
				{
					barr[index++] = (byte)((items[i / 4] >> ((i % 4) * 8)) & 0xFF);
				}
			}
			else if (array is bool[] boarr)
			{
				if (array.Length - index < count)
				{
					throw CommonExceptions.ArrayTooSmall(nameof(array));
				}
				for (int i = 0; i < count; i++)
				{
					boarr[index++] = (items[i >> IndexShift] & (1U << (i & IndexMask))) > 0;
				}
			}
			else
			{
				throw CommonExceptions.InvalidElementType();
			}
		}

		#endregion // ICollection 成员

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 
		/// <see cref="System.Collections.Generic.IEnumerator{T}"/>。</returns>
		public override IEnumerator<bool> GetEnumerator()
		{
			int end = count >> IndexShift;
			uint value;
			for (int i = 0; i < end; i++)
			{
				value = items[i];
				for (int j = 0; j < UnitSize; j++, value >>= 1)
				{
					yield return (value & 1U) == 1U;
				}
			}
			value = items[end];
			end = count - (end << IndexShift);
			for (int i = 0; i < end; i++, value >>= 1)
			{
				yield return (value & 1U) == 1U;
			}
		}

		#endregion // IEnumerable<T> 成员

		#region IEquatable<BitList> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">一个与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="NotImplementedException"></exception>
		public bool Equals(BitList? other)
		{
			if (other == null || count != other.count)
			{
				return false;
			}
			int end = count >> IndexShift;
			for (int i = 0; i < end; i++)
			{
				if (items[i] != other.items[i])
				{
					return false;
				}
			}
			uint value = items[end];
			uint value2 = other.items[end];
			end = count - (end << IndexShift);
			uint mask = GetMask(end);
			return (value & mask) == (value2 & mask);
		}

		#endregion // IEquatable<BitList> 成员

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
			else if (obj is BitList list)
			{
				return Equals(list);
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
			int end = count >> IndexShift;
			for (int i = 0; i < end; i++)
			{
				hashCode.Add(items[i]);
			}
			uint value = items[end];
			end = count - (end << IndexShift);
			hashCode.Add(value & GetMask(end));
			return hashCode.ToHashCode();
		}

		#region 辅助方法

		/// <summary>
		/// 确保当前列表的长度至少是给定的值。
		/// </summary>
		/// <param name="min">列表最少的长度。</param>
		private void EnsureCapacity(int min)
		{
			if (capacity >= min)
			{
				return;
			}
			int newCapacity = capacity << 1;
			if (newCapacity < min)
			{
				newCapacity = min;
			}
			Capacity = newCapacity;
		}

		/// <summary>
		/// 获取指定长度的掩码。
		/// </summary>
		/// <param name="maskSize">要获取的掩码长度。</param>
		/// <returns>掩码。</returns>
		private static uint GetMask(int maskSize)
		{
			return (1U << maskSize) - 1U;
		}

		/// <summary>
		/// 将指定 uint 列表中的数据复制到当前 <see cref="BitList"/> 从指定索引开始的位置。
		/// </summary>
		/// <param name="source">要复制到当前 <see cref="BitList"/> 的 uint 列表。</param>
		/// <param name="sourceIdx">源列表的起始索引。</param>
		/// <param name="itemIdx">要复制到的索引。</param>
		/// <param name="lowSize">要跳过的位数。</param>
		/// <param name="length">最多可以复制的位数，<paramref name="lowSize"/> 也要算在内。</param>
		private void CopyItems(IList<uint> source, int sourceIdx, int itemIdx, int lowSize, int length)
		{
			int highSize = UnitSize - lowSize;
			int cnt = source.Count;
			// 特殊处理第一次循环。
			if (length >= UnitSize)
			{
				if (lowSize == 0)
				{
					items[itemIdx] = source[sourceIdx];
				}
				else
				{
					uint lowMask = GetMask(lowSize);
					items[itemIdx] = (source[sourceIdx] << lowSize) | (items[itemIdx] & lowMask);
				}
				length -= UnitSize;
				sourceIdx++;
				itemIdx++;
			}
			else
			{
				uint value = 0U;
				if (lowSize == 0)
				{
					value = source[sourceIdx];
				}
				else
				{
					if (sourceIdx > 0)
					{
						value = source[sourceIdx - 1] >> highSize;
					}
					if (sourceIdx < cnt)
					{
						value |= source[sourceIdx] << lowSize;
					}
				}
				uint lenMask = GetMask(length - lowSize) << lowSize;
				items[itemIdx] = (items[itemIdx] & ~lenMask) | (value & lenMask);
				return;
			}
			while (length > 0)
			{
				if (length >= UnitSize)
				{
					if (lowSize == 0)
					{
						items[itemIdx] = source[sourceIdx];
					}
					else
					{
						items[itemIdx] = (source[sourceIdx] << lowSize) | (source[sourceIdx - 1] >> highSize);
					}
					length -= UnitSize;
					sourceIdx++;
					itemIdx++;
				}
				else
				{
					uint value = 0U;
					if (lowSize == 0)
					{
						value = source[sourceIdx];
					}
					else
					{
						if (sourceIdx > 0)
						{
							value = source[sourceIdx - 1] >> highSize;
						}
						if (sourceIdx < cnt)
						{
							value |= source[sourceIdx] << lowSize;
						}
					}
					uint lenMask = GetMask(length);
					items[itemIdx] = (items[itemIdx] & ~lenMask) | (value & lenMask);
					break;
				}
			}
		}

		/// <summary>
		/// 将从 <paramref name="sourceIdx"/> 开始的数据逆向复制到从 <paramref name="itemIdx"/> 开始的位置。
		/// </summary>
		/// <param name="sourceIdx">要复制数据的起始索引。</param>
		/// <param name="itemIdx">要复制到的索引。</param>
		/// <param name="lowSize">起始的位数。</param>
		/// <param name="length">最多可以复制的位数，<paramref name="lowSize"/> 也要算在内。</param>
		private void CopyItemsBackward(int sourceIdx, int itemIdx, int lowSize, int length)
		{
			int highSize = UnitSize - lowSize;
			while (length > 0)
			{
				if (length >= UnitSize)
				{
					if (highSize == 0)
					{
						items[itemIdx] = items[sourceIdx];
					}
					else
					{
						items[itemIdx] = (items[sourceIdx + 1] << lowSize) | (items[sourceIdx] >> highSize);
					}
					length -= UnitSize;
					sourceIdx--;
					itemIdx--;
				}
				else
				{
					uint value;
					if (highSize == 0)
					{
						value = items[sourceIdx];
					}
					else
					{
						value = items[sourceIdx + 1] << lowSize;
						if (sourceIdx >= 0)
						{
							value |= items[sourceIdx] >> highSize;
						}
					}
					uint lenMask = GetMask(UnitSize - length);
					items[itemIdx] = (items[itemIdx] & lenMask) | (value & ~lenMask);
					break;
				}
			}
		}

		#endregion // 辅助方法

	}
}
