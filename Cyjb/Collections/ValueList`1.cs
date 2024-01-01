using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cyjb.Collections;

/// <summary>
/// 允许在栈上分配的列表，要求在不使用后主动调用 <see cref="Dispose"/> 方法回收空间。
/// </summary>
/// <typeparam name="T">列表中的元素类型。</typeparam>
/// <remarks>适合临时使用列表的场景，使用栈上分配的数组来避免内存回收；也支持利用
/// <see cref="ArrayPool{T}"/> 自动扩容。</remarks>
public ref struct ValueList<T>
{
	/// <summary>
	/// 需要归还到 ArrayPool 中的数组。
	/// </summary>
	private T[]? arrayFromPool;
	/// <summary>
	/// 当前列表切片。
	/// </summary>
	private Span<T> span;
	/// <summary>
	/// 当前列表包含的长度。
	/// </summary>
	private int length = 0;

	/// <summary>
	/// 使用指定的初始缓冲区初始化 <see cref="ValueList{T}"/> 结构的新实例。
	/// </summary>
	/// <param name="initialBuffer">初始缓冲区。</param>
	public ValueList(Span<T> initialBuffer)
	{
		arrayFromPool = null;
		span = initialBuffer;
	}

	/// <summary>
	/// 使用指定的初始容量初始化 <see cref="ValueList{T}"/> 结构的新实例。
	/// </summary>
	/// <param name="initialCapacity">初始容量。</param>
	public ValueList(int initialCapacity)
	{
		arrayFromPool = ArrayPool<T>.Shared.Rent(initialCapacity);
		span = arrayFromPool;
	}

	/// <summary>
	/// 获取当前列表的长度。
	/// </summary>
	/// <value>当前列表的长度。</value>
	public readonly int Length => length;

	/// <summary>
	/// 获取当前列表的容量。
	/// </summary>
	public readonly int Capacity => span.Length;

	/// <summary>
	/// 获取指定索引处元素的引用。
	/// </summary>
	/// <param name="index">要获取的元素从零开始的索引。</param>
	/// <value>指定索引处的元素。</value>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表的有效索引。</exception>
	public ref T this[int index]
	{
		get
		{
			if (index < 0 || index >= length)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(index);
			}
			return ref span[index];
		}
	}

	/// <summary>
	/// 返回当前列表的切片。
	/// </summary>
	/// <returns>当前列表的切片。</returns>
	public readonly Span<T> AsSpan() => span[..length];

	/// <summary>
	/// 返回当前列表的切片。
	/// </summary>
	/// <param name="start">切片的起始索引。</param>
	/// <returns>当前列表的切片。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 不是当前列表的有效索引。</exception>
	public readonly Span<T> AsSpan(int start)
	{
		if (start < 0 || start > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		return span[start..length];
	}

	/// <summary>
	/// 返回当前列表的切片。
	/// </summary>
	/// <param name="start">切片的起始索引。</param>
	/// <param name="length">切片的长度。</param>
	/// <returns>当前列表的切片。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> 或 <c>start + length</c>
	/// 不是当前列表的有效索引。</exception>
	public readonly Span<T> AsSpan(int start, int length)
	{
		if (start < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(start);
		}
		int end = start + length;
		if (length < 0 || end > this.length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(length);
		}
		return span[start..end];
	}

	/// <summary>
	/// 将指定元素添加到当前列表中。
	/// </summary>
	/// <param name="item">要添加到当前列表的元素。</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T item)
	{
		int idx = length;
		// 避免额外的索引检查 https://github.com/dotnet/runtime/issues/72004
		Span<T> data = span;
		if (idx < data.Length)
		{
			data[idx] = item;
			length = idx + 1;
		}
		else
		{
			GrowAndAdd(item);
		}
	}

	/// <summary>
	/// 将指定的多个元素添加到当前列表中。
	/// </summary>
	/// <param name="items">要添加到当前列表的多个元素。</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(ReadOnlySpan<T> items)
	{
		int idx = length;
		Span<T> data = span;
		if (items.Length == 1 && idx < data.Length)
		{
			data[idx] = items[0];
			length = idx + 1;
		}
		else
		{
			AddMulti(ref items);
		}
	}

	/// <summary>
	/// 将指定的多个元素添加到当前列表中。
	/// </summary>
	/// <param name="items">要添加到当前列表的多个元素。</param>
	private void AddMulti(ref ReadOnlySpan<T> items)
	{
		int finalLength = length + items.Length;
		if (finalLength > span.Length)
		{
			Grow(items.Length);
		}
		items.CopyTo(span.Slice(length));
		length = finalLength;
	}

	/// <summary>
	/// 将指定元素重复多次添加到当前列表中。
	/// </summary>
	/// <param name="item">要添加到当前列表的元素。</param>
	/// <param name="count">要重复的次数。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public void Add(T item, int count)
	{
		if (count > 0)
		{
			int finalLength = length + count;
			if (finalLength > span.Length)
			{
				Grow(count);
			}
			span.Slice(length, count).Fill(item);
			length = finalLength;
		}
		else if (count < 0)
		{
			throw CommonExceptions.ArgumentNegative(count);
		}
	}

	/// <summary>
	/// 添加指定长度的元素，并返回相应的切片用于初始化。
	/// </summary>
	/// <param name="count">要添加元素的个数。</param>
	/// <returns>被添加元素的切片。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public Span<T> AddSpan(int count)
	{
		if (count > 0)
		{
			int idx = length;
			int finalLength = length + count;
			if (finalLength > span.Length)
			{
				Grow(count);
			}
			length = finalLength;
			return span.Slice(idx, count);
		}
		else if (count == 0)
		{
			return Span<T>.Empty;
		}
		else
		{
			throw CommonExceptions.ArgumentOutOfRange(count);
		}
	}

	/// <summary>
	/// 将元素插入当前列表的指定索引处。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
	/// <param name="item">要插入到当前列表中的元素。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表中的有效索引。</exception>
	public void Insert(int index, T item)
	{
		if (index < 0 || index > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		int finalLength = length + 1;
		if (finalLength > span.Length)
		{
			Grow(1);
		}
		if (index < length)
		{
			span[index..length].CopyTo(span[(index + 1)..]);
		}
		span[index] = item;
		length = finalLength;
	}

	/// <summary>
	/// 将多个元素插入当前列表的指定索引处。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="items"/>。</param>
	/// <param name="items">要插入到当前列表中的多个元素。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表中的有效索引。</exception>
	public void Insert(int index, ReadOnlySpan<T> items)
	{
		if (index < 0 || index > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		int count = items.Length;
		int finalLength = length + count;
		if (finalLength > span.Length)
		{
			Grow(count);
		}
		if (index < length)
		{
			span[index..length].CopyTo(span[(index + count)..]);
		}
		items.CopyTo(span[index..]);
		length = finalLength;
	}

	/// <summary>
	/// 将指定元素重复多次插入到当前列表的指定索引处。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
	/// <param name="item">要插入到当前列表的元素。</param>
	/// <param name="count">要重复的次数。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表中的有效索引。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public void Insert(int index, T item, int count)
	{
		if (index < 0 || index > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		if (count > 0)
		{
			int finalLength = length + count;
			if (finalLength > span.Length)
			{
				Grow(count);
			}
			if (index < length)
			{
				span[index..length].CopyTo(span[(index + count)..]);
			}
			span.Slice(index, count).Fill(item);
			length = finalLength;
		}
		else if (count < 0)
		{
			throw CommonExceptions.ArgumentNegative(count);
		}
	}

	/// <summary>
	/// 在当前列表的指定索引处插入指定长度的元素，并返回相应的切片用于初始化。
	/// </summary>
	/// <param name="index">从零开始的索引，应在该位置插入元素。</param>
	/// <param name="count">要插入的元素个数。</param>
	/// <returns>被插入元素的切片。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表中的有效索引。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 小于 0。</exception>
	public Span<T> InsertSpan(int index, int count)
	{
		if (index < 0 || index > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		if (count > 0)
		{
			int finalLength = length + count;
			if (finalLength > span.Length)
			{
				Grow(count);
			}
			if (index < length)
			{
				span[index..length].CopyTo(span[(index + count)..]);
			}
			length = finalLength;
			return span.Slice(index, count);
		}
		else if (count == 0)
		{
			return Span<T>.Empty;
		}
		else
		{
			throw CommonExceptions.ArgumentNegative(count);
		}
	}

	/// <summary>
	/// 移除当前列表中指定索引处的元素。
	/// </summary>
	/// <param name="index">要移除的元素的从零开始的索引。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表的有效索引。</exception>
	public void RemoveAt(int index)
	{
		int end = index + 1;
		if (index < 0 || end > length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		if (end < length)
		{
			span[end..length].CopyTo(span[index..]);
		}
		length--;
	}

	/// <summary>
	/// 移除当前列表中指定索引处的多个元素。
	/// </summary>
	/// <param name="index">要移除的元素的从零开始的索引。</param>
	/// <param name="count">要移除的元素个数。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是当前列表的有效索引。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><c>index + count</c> 不是当前当前列表的有效索引。</exception>
	public void RemoveRange(int index, int count)
	{
		if (index < 0)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(index);
		}
		int end = index + count;
		if (count < 0 || end > length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		if (end < length)
		{
			span[end..length].CopyTo(span[index..]);
		}
		length -= count;
	}

	/// <summary>
	/// 从当前集合中移除所有元素。
	/// </summary>
	public void Clear()
	{
		length = 0;
	}

	/// <summary>
	/// 将当前列表中的元素复制到目标 <see cref="Span{T}"/>。
	/// </summary>
	/// <param name="destination">要复制到的目标 <see cref="Span{T}"/>。</param>
	/// <exception cref="ArgumentException"><paramref name="destination"/> 短于当前列表。</exception>
	public readonly void CopyTo(Span<T> destination)
	{
		span[..length].CopyTo(destination);
	}

	/// <summary>
	/// 返回一个循环访问当前列表的枚举器。
	/// </summary>
	/// <returns>可用于循环访问当前列表的枚举器。</returns>
	public readonly Span<T>.Enumerator GetEnumerator()
	{
		return span[..length].GetEnumerator();
	}

	/// <summary>
	/// 确保当前实例至少可以容纳指定容量的字符。
	/// </summary>
	/// <param name="capacity">要确保的最小容量。</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 0。</exception>
	public void EnsureCapacity(int capacity)
	{
		if (capacity < 0)
		{
			throw CommonExceptions.ArgumentOutOfRange(capacity);
		}
		else if (capacity > span.Length)
		{
			Grow(capacity - length);
		}
	}

	/// <summary>
	/// 返回对于可用于固定的 <typeparamref name="T"/> 类型的对象的引用。
	/// </summary>
	/// <returns>返回对索引 <c>0</c> 处元素的引用，如果列表为空，则为 <c>null</c>。</returns>
	public readonly ref T GetPinnableReference()
	{
		return ref span[..length].GetPinnableReference();
	}

	/// <summary>
	/// 尝试将当前列表中的元素复制到目标 <see cref="Span{T}"/>，并返回已复制的元素个数。
	/// </summary>
	/// <param name="destination">要复制到的目标 <see cref="Span{T}"/>。</param>
	/// <param name="itemsWritten">已复制的元素个数；如果无法复制，则返回 0。</param>
	/// <returns>是否成功复制元素。</returns>
	public readonly bool TryCopyTo(Span<T> destination, out int itemsWritten)
	{
		if (span[..length].TryCopyTo(destination))
		{
			itemsWritten = length;
			return true;
		}
		else
		{
			itemsWritten = 0;
			return false;
		}
	}

	/// <summary>
	/// 将当前列表的内容复制到新数组中。
	/// </summary>
	/// <returns>包含当前列表中数据的数组。</returns>
	public readonly T[] ToArray()
	{
		return span[..length].ToArray();
	}

	/// <summary>
	/// 返回当前元素的字符串表示形式。
	/// </summary>
	/// <returns>当前元素的字符串表示形式。</returns>
	public override readonly string ToString()
	{
		return span[..length].ToString();
	}

	/// <summary>
	/// 释放需要回收的空间。
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		T[]? toReturn = arrayFromPool;
		// 避免后续被误用。
		this = default;
		if (toReturn != null)
		{
			arrayFromPool = null;
			ArrayPool<T>.Shared.Return(toReturn);
		}
	}

	/// <summary>
	/// 扩容并添加指定的项。
	/// </summary>
	/// <param name="item">要添加的项。</param>
	private void GrowAndAdd(T item)
	{
		int idx = length;
		Grow(1);
		span[idx] = item;
		length = idx + 1;
	}

	/// <summary>
	/// 扩容当前列表，至少会扩容指定长度。
	/// </summary>
	/// <param name="count">要扩容的长度</param>
	private void Grow(int count)
	{
		const uint ArrayMaxLength = 0x7FFFFFC7; // 等于 Array.MaxLength
		uint requiredLength = (uint)(span.Length + count);
		uint doubleLength = span.Length == 0 ? 4 : Math.Min((uint)span.Length * 2, ArrayMaxLength);
		int newCapacity = (int)Math.Max(requiredLength, doubleLength);

		T[] poolArray = ArrayPool<T>.Shared.Rent(newCapacity);
		span[..length].CopyTo(poolArray);
		T[]? toReturn = arrayFromPool;
		span = arrayFromPool = poolArray;
		if (toReturn != null)
		{
			ArrayPool<T>.Shared.Return(toReturn);
		}
	}
}

