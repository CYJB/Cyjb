using System;
using System.Diagnostics;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件中的指定范围。
	/// </summary>
	/// <remarks><see cref="Start"/> 属性小于等于 <see cref="End"/> 属性。
	/// <see cref="Start"/> 属性和 <see cref="End"/> 属性可以同时为 <see cref="SourceLocation.Unknown"/>，
	/// 表示未知的位置。</remarks>
	public class SourceRange : ISourceLocatable, IEquatable<SourceRange>
	{
		/// <summary>
		/// 表示未知的范围信息。
		/// </summary>
		/// <remarks>
		/// <see cref="Start"/> 属性和 <see cref="End"/> 属性都为 <see cref="SourceLocation.Unknown"/>。
		/// </remarks>
		public readonly static SourceRange Unknown = new SourceRange(SourceLocation.Unknown);
		/// <summary>
		/// 使用指定的范围初始化 <see cref="SourceRange"/> 类的新实例。
		/// </summary>
		/// <param name="start">范围的起始位置。</param>
		/// <param name="end">范围的结束位置。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="SourceRange"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public SourceRange(SourceLocation start, SourceLocation end)
		{
			if (start > end)
			{
				throw ExceptionHelper.ReversedArgument("start", "end");
			}
			this.Start = start;
			this.End = end;
		}
		/// <summary>
		/// 使用指定的范围初始化 <see cref="SourceRange"/> 类的新实例。
		/// 结束位置认为与起始位置相同。
		/// </summary>
		/// <param name="loc">范围的位置。</param>
		public SourceRange(SourceLocation loc)
		{
			this.Start = this.End = loc;
		}
		/// <summary>
		/// 使用指定的范围初始化 <see cref="SourceRange"/> 类的新实例。
		/// </summary>
		/// <param name="range">要设置的范围。</param>
		public SourceRange(ISourceLocatable range)
		{
			if (range.Start > range.Start)
			{
				throw ExceptionHelper.ReversedArgument("range.Start", "range.End");
			}
			this.Start = range.Start;
			this.End = range.End;
		}
		/// <summary>
		/// 获取在源文件中的起始位置。
		/// </summary>
		/// <value>源文件中的起始位置。</value>
		public SourceLocation Start { get; private set; }
		/// <summary>
		/// 获取在源文件中的结束位置。
		/// </summary>
		/// <value>源文件中的结束位置。</value>
		public SourceLocation End { get; private set; }
		/// <summary>
		/// 获取当前范围是否是有效的范围。
		/// </summary>
		/// <value>如果起始和结束位置都不是 <see cref="SourceLocation.Unknown"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		public bool IsValid
		{
			get { return this.Start != SourceLocation.Unknown; }
		}
		/// <summary>
		/// 将当前范围与指定范围合并，并返回结果范围。
		/// </summary>
		/// <param name="ranges">要合并的范围。</param>
		/// <returns>合并后的结果。</returns>
		public SourceRange MergeWith(params ISourceLocatable[] ranges)
		{
			if (ranges == null || ranges.Length == 0)
			{
				return this;
			}
			SourceRange baseRange = new SourceRange(this);
			int idx = -1;
			if (!this.IsValid)
			{
				idx = ranges.FirstIndex(range => range != null &&
					range.Start != SourceLocation.Unknown &&
					range.End != SourceLocation.Unknown);
				if (idx == -1)
				{
					return Unknown;
				}
				baseRange = new SourceRange(ranges[idx]);
			}
			return Merge(baseRange, ranges, idx + 1);
		}
		/// <summary>
		/// 返回将指定范围合并的结果范围。
		/// </summary>
		/// <param name="ranges">要合并的范围。</param>
		/// <returns>合并后的结果。</returns>
		public static SourceRange Merge(params ISourceLocatable[] ranges)
		{
			if (ranges == null || ranges.Length == 0)
			{
				return Unknown;
			}
			int idx = ranges.FirstIndex(range => range != null &&
				range.Start != SourceLocation.Unknown &&
				range.End != SourceLocation.Unknown);
			if (idx == -1)
			{
				return Unknown;
			}
			return Merge(new SourceRange(ranges[idx]), ranges, idx + 1);
		}
		/// <summary>
		/// 返回将指定范围合并的结果范围。
		/// </summary>
		/// <param name="range">合并的基础范围。</param>
		/// <param name="ranges">要合并的范围。</param>
		/// <param name="idx">要合并的起始索引。</param>
		/// <returns>合并后的结果。</returns>
		private static SourceRange Merge(SourceRange range, ISourceLocatable[] ranges, int idx)
		{
			Debug.Assert(range.IsValid);
			for (int i = idx; i < ranges.Length; i++)
			{
				if (ranges[i] == null)
				{
					continue;
				}
				SourceLocation start = ranges[i].Start;
				SourceLocation end = ranges[i].End;
				// 防止 ranges 中范围的 Start 和 End 颠倒。
				if (start > end)
				{
					start = ranges[i].End;
					end = ranges[i].Start;
				}
				if (start == SourceLocation.Unknown)
				{
					continue;
				}
				if (range.Start > start)
				{
					range.Start = start;
				}
				else if (range.End < end)
				{
					range.End = end;
				}
			}
			return range;
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			if (this.Start == SourceLocation.Unknown || this.End == this.Start)
			{
				return string.Concat("[", this.Start, "]");
			}
			else
			{
				return string.Concat("[", this.Start, " - ", this.End, "]");
			}
		}

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
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (this.Start != other.Start)
			{
				return false;
			}
			return this.End == other.End;
		}

		#endregion // IEquatable<SourceRange> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="SourceRange"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="SourceRange"/> 进行比较的 object。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="SourceRange"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			SourceRange thisObj = obj as SourceRange;
			if (object.ReferenceEquals(thisObj, null))
			{
				return false;
			}
			return this.Equals(thisObj);
		}
		/// <summary>
		/// 用于 <see cref="SourceRange"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="SourceRange"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			return this.Start.GetHashCode() ^ this.End.GetHashCode();
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
			if (object.ReferenceEquals(obj1, obj2))
			{
				return true;
			}
			if (object.ReferenceEquals(obj1, null))
			{
				return object.ReferenceEquals(obj2, null);
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
			return !(obj1 == obj2);
		}

		#endregion // 运算符重载

	}
}
