using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Cyjb.Utility;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件中的指定范围。
	/// </summary>
	/// <remarks>
	/// <para>表示从 <see cref="Start"/> 开始，到 <see cref="End"/> （包含）结束的范围，
	/// 要求 <see cref="Start"/> 属性的值小于等于 <see cref="End"/> 属性的值。</para>
	/// <para>若 <see cref="Start"/> 属性和 <see cref="End"/> 属性的值都为 <see cref="SourcePosition.Unknown"/>，
	/// 则表示未知的位置。</para>
	/// </remarks>
	[Serializable]
	public class SourceRange : ISourceLocatable, IEquatable<SourceRange>, IComparable<SourceRange>
	{
		/// <summary>
		/// 表示未知的范围信息。
		/// </summary>
		/// <remarks>
		/// <see cref="Start"/> 属性和 <see cref="End"/> 属性都为 <see cref="SourcePosition.Unknown"/>。
		/// </remarks>
		public readonly static SourceRange Unknown = new SourceRange(SourcePosition.Unknown);
		/// <summary>
		/// 起始位置。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourcePosition start;
		/// <summary>
		/// 结束位置。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourcePosition end;
		/// <summary>
		/// 使用指定的位置初始化 <see cref="SourceRange"/> 类的新实例。
		/// 结束位置与起始位置相同。
		/// </summary>
		/// <param name="loc">范围的位置。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="SourceRange"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public SourceRange(SourcePosition loc)
		{
			this.start = this.end = loc;
		}
		/// <summary>
		/// 使用指定的范围初始化 <see cref="SourceRange"/> 类的新实例。
		/// </summary>
		/// <param name="start">范围的起始位置。</param>
		/// <param name="end">范围的结束位置。</param>
		/// <exception cref="ArgumentException"><paramref name="start"/> 和 <paramref name="end"/> 
		/// 表的不是有效的范围。</exception>
		public SourceRange(SourcePosition start, SourcePosition end)
		{
			if (start.IsUnknown != end.IsUnknown)
			{
				throw CommonExceptions.InvalidSourceRange(start, end);
			}
			if (!start.IsUnknown && start > end)
			{
				throw CommonExceptions.ReversedArgument("start", "end");
			}
			Contract.EndContractBlock();
			this.start = start;
			this.end = end;
		}
		/// <summary>
		/// 使用指定的范围初始化 <see cref="SourceRange"/> 类的新实例。
		/// </summary>
		/// <param name="range">要设置的范围。</param>
		/// <exception cref="ArgumentException"><paramref name="range"/> 表的不是有效的范围。</exception>
		public SourceRange(ISourceLocatable range)
		{
			CommonExceptions.CheckArgumentNull(range, "range");
			if (range.Start.IsUnknown != range.End.IsUnknown)
			{
				throw CommonExceptions.InvalidSourceRange(range.Start, range.End);
			}
			if (!range.Start.IsUnknown && range.Start > range.End)
			{
				throw CommonExceptions.ReversedArgument("locatable.Start", "locatable.End");
			}
			Contract.EndContractBlock();
			this.start = range.Start;
			this.end = range.End;
		}
		/// <summary>
		/// 获取在源文件中的起始位置。
		/// </summary>
		/// <value>源文件中的起始位置。</value>
		public SourcePosition Start { get { return this.start; } }
		/// <summary>
		/// 获取在源文件中的结束位置。
		/// </summary>
		/// <value>源文件中的结束位置。</value>
		public SourcePosition End { get { return this.end; } }
		/// <summary>
		/// 获取当前范围在源文件中的字符长度。
		/// </summary>
		/// <value>当前范围在源文件中的字符长度。</value>
		public int Length
		{
			get { return this.IsUnknown ? 0 : this.end.Index - this.start.Index + 1; }
		}
		/// <summary>
		/// 获取当前范围是否表示未知范围。
		/// </summary>
		/// <value>如果当前范围表示未知范围，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsUnknown
		{
			get { return this.start.IsUnknown; }
		}
		/// <summary>
		/// 为当前源文件位置附加源文件名。
		/// </summary>
		/// <param name="fileName">源文件的名称。</param>
		/// <returns>表示指定源文件中的指定范围的实例。</returns>
		public SourceFileRange WithFile(string fileName)
		{
			CommonExceptions.CheckArgumentNull(fileName, "fileName");
			Contract.EndContractBlock();
			return new SourceFileRange(fileName, this.start, this.end);
		}

		#region 范围操作

		/// <summary>
		/// 返回指定的 <see cref="ISourceLocatable"/> 是否完全包含在当前范围中。
		/// </summary>
		/// <param name="locatable">要检查的范围。</param>
		/// <returns>如果指定的范围完全包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="locatable"/> 为 <c>null</c>。</exception>
		/// <override>
		/// <summary>
		/// 返回指定的范围或位置是否完全包含在当前范围中。
		/// </summary>
		/// </override>
		public bool Contains(ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			return (!this.IsUnknown) && this.start <= locatable.Start && this.end >= locatable.End;
		}
		/// <summary>
		/// 返回指定的位置是否完全包含在当前范围中。
		/// </summary>
		/// <param name="location">要检查的位置。</param>
		/// <returns>如果指定的位置包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		public bool Contains(SourcePosition location)
		{
			if (location.IsUnknown || this.IsUnknown)
			{
				return false;
			}
			return this.end.Index >= location.Index && this.start.Index <= location.Index;
		}
		/// <summary>
		/// 返回指定的索引是否完全包含在当前范围中。
		/// </summary>
		/// <param name="index">要检查的索引。</param>
		/// <returns>如果指定的索引包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		public bool Contains(int index)
		{
			if (index < 0 || this.IsUnknown)
			{
				return false;
			}
			return this.end.Index >= index && this.start.Index <= index;
		}
		/// <summary>
		/// 返回指定的行列位置是否完全包含在当前范围中。
		/// </summary>
		/// <param name="line">要检查的行。</param>
		/// <param name="col">要检查的列。</param>
		/// <returns>如果指定的行列位置包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="line"/> 或 <paramref name="col"/> 
		/// 小于 <c>0</c>。</exception>
		public bool Contains(int line, int col)
		{
			if (line < 1 || col < 1 || this.IsUnknown)
			{
				return false;
			}
			if (this.start.Line == this.end.Line)
			{
				return this.start.Line == line && this.start.Col <= col && this.end.Col >= col;
			}
			if (this.start.Line == line)
			{
				return this.start.Col <= col;
			}
			if (this.end.Line == line)
			{
				return this.end.Col >= col;
			}
			return true;
		}
		/// <summary>
		/// 返回指定的 <see cref="ISourceLocatable"/> 是否与当前范围存在重叠。
		/// </summary>
		/// <param name="locatable">要检查的范围。</param>
		/// <returns>如果指定的范围与当前范围存在重叠，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="locatable"/> 为 <c>null</c>。</exception>
		public bool OverlapsWith(ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			return (!this.IsUnknown) && this.start <= locatable.End && this.end >= locatable.Start;
		}
		/// <summary>
		/// 返回当前范围与指定 <see cref="ISourceLocatable"/> 的重叠范围，如果不存在则为 
		/// <see cref="SourceRange.Unknown"/>。
		/// </summary>
		/// <param name="locatable">要检查的范围。</param>
		/// <returns>当前范围与指定范围重叠的部分，如果不存在则为 
		/// <see cref="SourceRange.Unknown"/>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="locatable"/> 为 <c>null</c>。</exception>
		public SourceRange Overlap(ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			SourcePosition maxStart = this.start > locatable.Start ? this.start : locatable.Start;
			SourcePosition minEnd = this.end < locatable.End ? this.end : locatable.End;
			if (maxStart == SourcePosition.Unknown || maxStart > minEnd)
			{
				return Unknown;
			}
			return new SourceRange(maxStart, minEnd);
		}

		#endregion // 范围操作

		#region 合并

		/// <summary>
		/// 返回将指定的一个或多个范围合并的结果，忽略无效范围。
		/// </summary>
		/// <param name="ranges">要进行合并的范围集合。</param>
		/// <returns>合并后的结果。</returns>
		/// <override>
		/// <summary>
		/// 返回将指定的一个或多个范围合并的结果，忽略无效范围。
		/// </summary>
		/// </override>
		public static SourceRange Merge(params ISourceLocatable[] ranges)
		{
			return Merge(ranges as IEnumerable<ISourceLocatable>);
		}
		/// <summary>
		/// 返回将指定的一个或多个范围合并的结果，忽略无效范围。
		/// </summary>
		/// <param name="ranges">要进行合并的范围集合。</param>
		/// <returns>合并后的结果。</returns>
		public static SourceRange Merge(IEnumerable<ISourceLocatable> ranges)
		{
			if (ranges == null)
			{
				return Unknown;
			}
			SourcePosition finalStart = SourcePosition.Unknown;
			SourcePosition finalEnd = SourcePosition.Unknown;
			foreach (ISourceLocatable loc in ranges)
			{
				if (loc == null)
				{
					continue;
				}
				SourcePosition start = loc.Start;
				SourcePosition end = loc.End;
				if (start.IsUnknown || end.IsUnknown)
				{
					continue;
				}
				// 防止 ranges 中范围的 Start 和 End 颠倒。
				if (start > end)
				{
					start = loc.End;
					end = loc.Start;
				}
				if (finalStart.IsUnknown)
				{
					finalStart = start;
					finalEnd = end;
				}
				else
				{
					if (finalStart > start)
					{
						finalStart = start;
					}
					if (finalEnd < end)
					{
						finalEnd = end;
					}
				}
			}
			return finalStart.IsUnknown ? Unknown : new SourceRange(finalStart, finalEnd);
		}

		#endregion // 合并

		#region IComparable<SourceRange> 成员

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
		public int CompareTo(SourceRange other)
		{
			if (ReferenceEquals(other, null))
			{
				return 1;
			}
			int cmp = this.start.CompareTo(other.start);
			if (cmp != 0)
			{
				return cmp;
			}
			return this.end.CompareTo(other.end);
		}

		#endregion // IComparable<SourceRange> 成员

		#region IEquatable<SourceRange> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/> 参数，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 指示当前对象是否等于另一个对象。
		/// </summary>
		/// </overloads>
		public virtual bool Equals(SourceRange other)
		{
			if (ReferenceEquals(other, this))
			{
				return true;
			}
			if (ReferenceEquals(other, null))
			{
				return false;
			}
			return this.start == other.start && this.end == other.end;
		}

		#endregion // IEquatable<SourceRange> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="Object"/> 是否等于当前的 <see cref="SourceRange"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="SourceRange"/> 进行比较的 object。</param>
		/// <returns>如果指定的 <see cref="Object"/> 等于当前的 <see cref="SourceRange"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			SourceRange range = obj as SourceRange;
			if (ReferenceEquals(range, null))
			{
				return false;
			}
			return this.Equals(range);
		}
		/// <summary>
		/// 用于 <see cref="SourceRange"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="SourceRange"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			return Hash.Combine(this.start.GetHashCode(), this.end);
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Concat("(", this.start, ")-(", this.end, ")");
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="SourceRange"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceRange"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceRange"/> 对象。</param>
		/// <returns>如果两个 <see cref="SourceRange"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(SourceRange obj1, SourceRange obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return ReferenceEquals(obj2, null);
			}
			return obj1.Equals(obj2);
		}
		/// <summary>
		/// 判断两个 <see cref="SourceRange"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceRange"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceRange"/> 对象。</param>
		/// <returns>如果两个 <see cref="SourceRange"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(SourceRange obj1, SourceRange obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return !ReferenceEquals(obj2, null);
			}
			return !obj1.Equals(obj2);
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceRange"/> 是否大于第二个 <see cref="SourceRange"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceRange"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceRange"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceRange"/> 对象大于第二个 <see cref="SourceRange"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >(SourceRange obj1, SourceRange obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return false;
			}
			return obj1.CompareTo(obj2) > 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceRange"/> 是否大于等于第二个 <see cref="SourceRange"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceRange"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceRange"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceRange"/> 对象大于等于第二个 <see cref="SourceRange"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >=(SourceRange obj1, SourceRange obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return ReferenceEquals(obj2, null);
			}
			return obj1.CompareTo(obj2) >= 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceRange"/> 是否小于第二个 <see cref="SourceRange"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceRange"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceRange"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceRange"/> 对象小于第二个 <see cref="SourceRange"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <(SourceRange obj1, SourceRange obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return !ReferenceEquals(obj2, null);
			}
			return obj1.CompareTo(obj2) < 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceRange"/> 是否小于等于第二个 <see cref="SourceRange"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceRange"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceRange"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceRange"/> 对象小于等于第二个 <see cref="SourceRange"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <=(SourceRange obj1, SourceRange obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return ReferenceEquals(obj2, null);
			}
			return obj1.CompareTo(obj2) <= 0;
		}

		#endregion // 运算符重载

	}
}
