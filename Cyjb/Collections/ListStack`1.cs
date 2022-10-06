using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 表示同一任意类型的实例的大小可变的后进先出 (LIFO) 集合。
/// 该集合还允许使用索引访问堆栈中的元素。
/// </summary>
/// <typeparam name="T">指定堆栈中的元素的类型。</typeparam>
public class ListStack<T> : ReadOnlyCollectionBase<T>, IReadOnlyList<T>
{
	/// <summary>
	/// 集合中的元素。
	/// </summary>
	private T[] items;
	/// <summary>
	/// 集合中包含的元素个数。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int count = 0;
	/// <summary>
	/// 集合的版本，用于与枚举器同步。
	/// </summary>
	private int version = 0;

	/// <summary>
	/// 初始化 <see cref="ListStack{T}"/> 类的新实例。
	/// </summary>
	public ListStack()
	{
		items = Array.Empty<T>();
	}

	/// <summary>
	/// 使用指定集合的元素初始化 <see cref="ListStack{T}"/> 类的新实例。
	/// </summary>
	/// <param name="source">从中复制元素的集合。</param>
	/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
	public ListStack(IEnumerable<T> source) : base(false)
	{
		ArgumentNullException.ThrowIfNull(source);
		items = EnumerableUtil.ToArray(source, out count);
	}

	/// <summary>
	/// 使用指定的初始容量初始化 <see cref="ListStack{T}"/> 类的新实例。
	/// </summary>
	/// <param name="capacity">初始容量。</param>
	/// <exception cref="ArgumentException"><paramref name="capacity"/> 小于 <c>0</c>。</exception>
	public ListStack(int capacity) : base(false)
	{
		if (capacity < 0)
		{
			throw CommonExceptions.ArgumentNegative(capacity);
		}
		else if (capacity == 0)
		{
			items = Array.Empty<T>();
		}
		else
		{
			items = new T[capacity];
		}
	}

	/// <summary>
	/// 获取堆栈距离栈顶指定偏移处的元素。
	/// </summary>
	/// <param name="offset">要获取的元素距离栈顶从零开始的偏移。</param>
	/// <value>指定偏移处的元素。</value>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> 不是 
	/// <see cref="ListStack{T}"/> 中的有效偏移。</exception>
	public T this[int offset]
	{
		get
		{
			if (offset < 0)
			{
				throw CommonExceptions.ArgumentNegative(offset);
			}
			else if (offset >= count)
			{
				throw CommonExceptions.ArgumentOutOfRange(offset);
			}
			return items[count - offset - 1];
		}
	}

	/// <summary>
	/// 获取堆栈距离栈顶指定偏移处的元素。
	/// </summary>
	/// <param name="offset">要获取的元素距离栈顶从零开始的偏移。</param>
	/// <value>指定偏移处的元素。</value>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> 不是 
	/// <see cref="ListStack{T}"/> 中的有效偏移。</exception>
	public T this[Index offset]
	{
		get
		{
			int index = (offset.IsFromEnd ? offset.Value : count - offset.Value) - 1;
			if (index < 0 || index >= count)
			{
				throw CommonExceptions.ArgumentOutOfRange(offset);
			}
			return items[index];
		}
	}

	/// <summary>
	/// 确保堆栈至少具有指定容量。
	/// </summary>
	/// <param name="capacity">要确保的最小容量。</param>
	/// <returns>当前堆栈的新容量。</returns>
	/// <exception cref="ArgumentException"><paramref name="capacity"/> 小于 <c>0</c>。</exception>
	public int EnsureCapacity(int capacity)
	{
		if (capacity < 0)
		{
			throw CommonExceptions.ArgumentNegative(capacity);
		}
		if (items.Length < capacity)
		{
			Grow(capacity);
		}
		return items.Length;
	}

	/// <summary>
	/// 将指定元素推入堆栈的顶部。
	/// </summary>
	/// <param name="item">要推入的元素。</param>
	public void Push(T item)
	{
		if (count == items.Length)
		{
			Grow(count + 1);
		}
		items[count++] = item;
		version++;
	}

	/// <summary>
	/// 删除并返回堆栈顶部的元素。
	/// </summary>
	/// <returns>堆栈顶部的元素。</returns>
	/// <exception cref="InvalidOperationException">堆栈是空的。</exception>
	public T Pop()
	{
		if (count == 0)
		{
			throw new InvalidOperationException(Resources.EmptyStack);
		}
		T result = items[--count];
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			items[count] = default!;
		}
		version++;
		return result;
	}

	/// <summary>
	/// 删除堆栈顶部指定个数的元素。
	/// </summary>
	/// <param name="size">要删除的元素个数。</param>
	/// <exception cref="InvalidOperationException"><paramref name="size"/> 不是堆栈的有效个数。</exception>
	public void Pop(int size)
	{
		if (size < 0 || size > count)
		{
			throw CommonExceptions.ArgumentOutOfRange(size);
		}
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			Array.Clear(items, count - size, size);
		}
		count -= size;
		version++;
	}

	/// <summary>
	/// 返回堆栈顶部的元素，但不将其移除。
	/// </summary>
	/// <returns>堆栈顶部的元素。</returns>
	/// <exception cref="InvalidOperationException">堆栈是空的。</exception>
	public T Peek()
	{
		if (count == 0)
		{
			throw new InvalidOperationException(Resources.EmptyStack);
		}
		return items[count - 1];
	}

	/// <summary>
	/// 尝试删除并返回堆栈顶部的元素。
	/// </summary>
	/// <param name="result">堆栈顶部的元素，如果不存在则为 <typeparamref name="T"/> 的默认值。</param>
	/// <returns>如果堆栈不是空的，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryPop([NotNullWhen(true)] out T? result)
	{
		if (count == 0)
		{
			result = default;
			return false;
		}
		count--;
		result = items[count]!;
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			items[count] = default!;
		}
		version++;
		return true;
	}

	/// <summary>
	/// 尝试返回堆栈顶部的元素，但不将其移除。
	/// </summary>
	/// <param name="result">堆栈顶部的元素，如果不存在则为 <typeparamref name="T"/> 的默认值。</param>
	/// <returns>如果堆栈不是空的，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryPeek([NotNullWhen(true)] out T? result)
	{
		if (count == 0)
		{
			result = default;
			return false;
		}
		result = items[count - 1]!;
		return true;
	}

	/// <summary>
	/// 从当前集合中移除所有元素。
	/// </summary>
	public void Clear()
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			Array.Clear(items, 0, count);
		}
		count = 0;
		version++;
	}

	/// <summary>
	/// 将堆栈的元素复制到新数组中。
	/// </summary>
	/// <returns>复制到的新数组。</returns>
	public T[] ToArray()
	{
		if (count == 0)
		{
			return Array.Empty<T>();
		}
		T[] result = new T[count];
		for (int i = 0; i < count; i++)
		{
			result[i] = items[count - i - 1];
		}
		return result;
	}

	/// <summary>
	/// 如果元素数小于当前容量的 90%，将容量设置为堆栈中的实际元素数。
	/// </summary>
	public void TrimExcess()
	{
		int capacity = (int)(items.Length * 0.9);
		if (count < capacity)
		{
			Array.Resize(ref items, capacity);
		}
	}

	/// <summary>
	/// 增加堆栈的容量。
	/// </summary>
	/// <param name="capacity">需要的最小容量。</param>
	private void Grow(int capacity)
	{
		capacity = Math.Max(items.Length == 0 ? 4 : items.Length * 2, capacity);
		Array.Resize(ref items, capacity);
	}

	#region ReadOnlyCollectionBase<T> 成员

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public override int Count => count;

	/// <summary>
	/// 确定当前集合是否包含指定对象。
	/// </summary>
	/// <param name="item">要在当前集合中定位的对象。</param>
	/// <returns>如果在当前集合中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool Contains(T item)
	{
		if (count > 0)
		{
			return Array.LastIndexOf(items, item) >= 0;
		}
		return false;
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
	public override void CopyTo(T[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array);
		if (arrayIndex < 0)
		{
			throw CommonExceptions.ArgumentNegative(arrayIndex);
		}
		else if (array.Length - arrayIndex < count)
		{
			throw CommonExceptions.ArrayTooSmall(nameof(array));
		}
		int idx = arrayIndex + count - 1;
		for (int i = 0; i < count; i++, idx--)
		{
			array[idx] = items[i];
		}
	}

	/// <summary>
	/// 堆栈的枚举器。
	/// </summary>
	private class Enumerator : EnumeratorBase<T>
	{
		/// <summary>
		/// 要枚举的堆栈实例。
		/// </summary>
		private readonly ListStack<T> stack;
		/// <summary>
		/// 创建枚举器时的堆栈版本。
		/// </summary>
		private readonly int version;
		/// <summary>
		/// 当前正在枚举的元素索引。
		/// </summary>
		private int index;

		/// <summary>
		/// 使用指定的堆栈初始化 <see cref="Enumerator"/> 类的新实例。
		/// </summary>
		/// <param name="stack">要枚举的堆栈实例。</param>
		public Enumerator(ListStack<T> stack)
		{
			this.stack = stack;
			version = stack.version;
		}

		#region EnumeratorBase<T> 成员

		/// <summary>
		/// 检查容器版本是否发生了变化。
		/// </summary>
		/// <returns>如果容器版本发生了变化，则为 <c>true</c>；否则返回 <c>false</c>。</returns>
		protected override bool CheckVersionChanged()
		{
			return version != stack.version;
		}

		/// <summary>
		/// 将枚举数推进到集合的下一个元素。
		/// </summary>
		/// <param name="initial">当前是否是首次调用。</param>
		/// <param name="current">集合的下一个元素。</param>
		/// <returns>如果枚举数已成功地推进到下一个元素，则为 <c>true</c>；
		/// 如果枚举数传递到集合的末尾，则为 <c>false</c>。</returns>
		protected override bool MoveNext(bool initial, out T current)
		{
			if (initial)
			{
				index = stack.count - 1;
			}
			else
			{
				index--;
			}
			if (index >= 0)
			{
				current = stack.items[index];
				return true;
			}
			else
			{
				current = default!;
				return false;
			}
		}

		#endregion // EnumeratorBase<T> 成员

	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}

	#endregion // ReadOnlyCollectionBase<T> 成员

}
