namespace Cyjb
{
	/// <summary>
	/// 表示值的范围。
	/// </summary>
	/// <typeparam name="T">值的类型。</typeparam>
	public readonly record struct ValueRange<T> : IComparable<ValueRange<T>>
		where T : notnull, IComparable<T>
	{
		/// <summary>
		/// 范围的起始值（包含）。
		/// </summary>
		private readonly T start;
		/// <summary>
		/// 范围的结束值（包含）。
		/// </summary>
		private readonly T end;

		/// <summary>
		/// 使用指定的起始值和结束值初始化。
		/// </summary>
		/// <param name="start">起始值（包含）</param>
		/// <param name="end">结束值（包含）</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> 小于 <paramref name="start"/>。</exception>
		public ValueRange(T start, T end)
		{
			if (start.CompareTo(end) > 0)
			{
				throw CommonExceptions.ArgumentMinMaxValue(nameof(start), nameof(end));
			}
			this.start = start;
			this.end = end;
		}

		/// <summary>
		/// 范围的起始值（包含）。
		/// </summary>
		public T Start => start;

		/// <summary>
		/// 范围的结束值（包含）。
		/// </summary>
		public T End => end;

		/// <summary>
		/// 解构当前范围。
		/// </summary>
		/// <param name="start">范围的起始值（包含）。</param>
		/// <param name="end">范围的结束值（包含）。</param>
		public void Deconstruct(out T start, out T end)
		{
			start = this.start;
			end = this.end;
		}

		/// <summary>
		/// 返回二者间的最大值。
		/// </summary>
		/// <param name="left">要比较的第一个值。</param>
		/// <param name="right">要比较的第二个值。</param>
		/// <returns>最大值。</returns>
		private static T Max(T left, T right) => left.CompareTo(right) >= 0 ? left : right;

		/// <summary>
		/// 返回二者间的最小值。
		/// </summary>
		/// <param name="left">要比较的第一个值。</param>
		/// <param name="right">要比较的第二个值。</param>
		/// <returns>最大值。</returns>
		private static T Min(T left, T right) => left.CompareTo(right) <= 0 ? left : right;

		#region 范围操作

		/// <summary>
		/// 返回指定的位置是否包含在当前范围中。
		/// </summary>
		/// <param name="position">要检查的位置。</param>
		/// <returns>如果指定的位置包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 返回指定的位置或范围是否包含在当前范围中。
		/// </summary>
		/// </overloads>
		public bool Contains(T position) => position.CompareTo(start) >= 0 && position.CompareTo(end) <= 0;

		/// <summary>
		/// 返回指定的 <see cref="ValueRange{T}"/> 是否完全包含在当前范围中。
		/// </summary>
		/// <param name="range">要检查的范围。</param>
		/// <returns>如果指定的范围完全包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(ValueRange<T> range) => range.start.CompareTo(start) >= 0 && range.end.CompareTo(end) <= 0;

		/// <summary>
		/// 返回指定的 <see cref="ValueRange{T}"/> 是否与当前范围存在重叠。
		/// </summary>
		/// <param name="range">要检查的范围。</param>
		/// <returns>如果指定的范围与当前范围存在重叠，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于空的范围，也会返回 <c>false</c>。</returns>
		public bool OverlapsWith(ValueRange<T> range)
		{
			T curStart = Max(start, range.start);
			T curEnd = Min(end, range.end);
			return curStart.CompareTo(curEnd) <= 0;
		}

		/// <summary>
		/// 返回当前范围与指定 <see cref="ValueRange{T}"/> 的重叠范围，如果不存在则为 <c>null</c>。
		/// </summary>
		/// <param name="range">要检查的范围。</param>
		/// <returns>当前范围与指定范围重叠的部分，如果不存在则为 <c>null</c>。</returns>
		public ValueRange<T>? Overlap(ValueRange<T> range)
		{
			T curStart = Max(start, range.start);
			T curEnd = Min(end, range.end);
			if (curStart.CompareTo(curEnd) <= 0)
			{
				return new ValueRange<T>(curStart, curEnd);
			}
			else
			{
				return null;
			}
		}

		#endregion // 范围操作

		#region IComparable<ValueRange<T>> 成员

		/// <summary>
		/// 将当前对象与同一类型的另一个对象进行比较。
		/// </summary>
		/// <param name="other">要比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
		public int CompareTo(ValueRange<T> other)
		{
			int cmp = start.CompareTo(other.start);
			if (cmp != 0)
			{
				return cmp;
			}
			return end.CompareTo(other.end);
		}

		/// <summary>
		/// 返回一个 <see cref="ValueRange{T}"/> 对象是否小于另一个 <see cref="ValueRange{T}"/> 对象。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 小于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <(ValueRange<T> left, ValueRange<T> right)
		{
			return left.CompareTo(right) < 0;
		}

		/// <summary>
		/// 返回一个 <see cref="ValueRange{T}"/> 对象是否小于等于另一个 <see cref="ValueRange{T}"/> 对象。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 小于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <=(ValueRange<T> left, ValueRange<T> right)
		{
			return left.CompareTo(right) <= 0;
		}

		/// <summary>
		/// 返回一个 <see cref="ValueRange{T}"/> 对象是否大于另一个 <see cref="ValueRange{T}"/> 对象。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 大于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >(ValueRange<T> left, ValueRange<T> right)
		{
			return left.CompareTo(right) > 0;
		}

		/// <summary>
		/// 返回一个 <see cref="ValueRange{T}"/> 对象是否大于等于另一个 <see cref="ValueRange{T}"/> 对象。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 大于等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >=(ValueRange<T> left, ValueRange<T> right)
		{
			return left.CompareTo(right) >= 0;
		}

		#endregion // IComparable<ValueRange<T>> 成员

		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return $"[{start}..{end}]";
		}
	}
}
