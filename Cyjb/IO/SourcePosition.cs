using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Cyjb.Utility;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件中的位置信息。
	/// </summary>
	/// <remarks>
	/// 不同源文件的位置之间没有可比性，进行大小比较是没有意义的。
	/// </remarks>
	[Serializable, StructLayout(LayoutKind.Auto)]
	public struct SourcePosition : IEquatable<SourcePosition>, IComparable<SourcePosition>
	{
		/// <summary>
		/// 表示未知的位置信息。
		/// </summary>
		public readonly static SourcePosition Unknown;
		/// <summary>
		/// 所在的行。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int line;
		/// <summary>
		/// 所在的列。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int col;
		/// <summary>
		/// 从零开始的索引。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int index;
		/// <summary>
		/// 使用索引、行和列初始化 <see cref="SourcePosition"/> 结构的新实例。
		/// </summary>
		/// <param name="idx">从零开始的索引。</param>
		/// <param name="line">所在的行。</param>
		/// <param name="col">所在的列。</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="idx"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="line"/> 或 <paramref name="col"/> 
		/// 小于 <c>1</c>。</exception>
		public SourcePosition(int idx, int line, int col)
		{
			if (idx < 0)
			{
				throw CommonExceptions.ArgumentNegative("idx", idx);
			}
			if (line < 1)
			{
				throw CommonExceptions.ArgumentOutOfRange("line", line);
			}
			if (col < 1)
			{
				throw CommonExceptions.ArgumentOutOfRange("col", col);
			}
			this.index = idx;
			this.line = line;
			this.col = col;
		}
		/// <summary>
		/// 获取所在的行。
		/// </summary>
		/// <value>表示当前位置所在行的整数，从 <c>1</c> 开始。</value>
		public int Line { get { return this.line; } }
		/// <summary>
		/// 获取所在的列。
		/// </summary>
		/// <value>表示当前位置所在列的整数，从 <c>1</c> 开始。</value>
		public int Col { get { return this.col; } }
		/// <summary>
		/// 获取索引。
		/// </summary>
		/// <value>表示当前位置从起始位置从零开始的索引。</value>
		public int Index { get { return this.index; } }
		/// <summary>
		/// 获取当前位置是否表示未知位置。
		/// </summary>
		/// <value>如果当前位置表示未知位置，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsUnknown
		{
			get { return this.line == 0 || this.col == 0; }
		}

		#region IComparable<SourcePosition> 成员

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
		/// <remarks>
		/// <para>这里首先较索引，然后是行，最后是列。</para>
		/// <para><see cref="Unknown"/> 总是小于其它任何位置。</para>
		/// </remarks>
		public int CompareTo(SourcePosition other)
		{
			int cmp = this.index - other.index;
			if (cmp != 0)
			{
				return cmp;
			}
			cmp = this.line - other.line;
			if (cmp != 0)
			{
				return cmp;
			}
			return this.col - other.col;
		}

		#endregion // IComparable<SourcePosition> 成员

		#region IEquatable<SourcePosition> 成员

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
		public bool Equals(SourcePosition other)
		{
			return this.index == other.index && this.line == other.line && this.col == other.col;
		}

		#endregion // IEquatable<SourcePosition> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="SourcePosition"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="SourcePosition"/> 进行比较的 <see cref="System.Object"/>。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="SourcePosition"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SourcePosition))
			{
				return false;
			}
			return this.Equals((SourcePosition)obj);
		}

		/// <summary>
		/// 用于 <see cref="SourcePosition"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="SourcePosition"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			return Hash.Combine(Hash.Combine(this.index, this.line), this.col);
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			if (this.IsUnknown)
			{
				return "?,?";
			}
			return string.Concat(this.line, ",", this.col);
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="SourcePosition"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourcePosition"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourcePosition"/> 对象。</param>
		/// <returns>如果两个 <see cref="SourcePosition"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(SourcePosition obj1, SourcePosition obj2)
		{
			return obj1.Equals(obj2);
		}

		/// <summary>
		/// 判断两个 <see cref="SourcePosition"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourcePosition"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourcePosition"/> 对象。</param>
		/// <returns>如果两个 <see cref="SourcePosition"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(SourcePosition obj1, SourcePosition obj2)
		{
			return !obj1.Equals(obj2);
		}
		/// <summary>
		/// 判断第一个 <see cref="SourcePosition"/> 是否大于第二个 <see cref="SourcePosition"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourcePosition"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourcePosition"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourcePosition"/> 对象大于第二个 <see cref="SourcePosition"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >(SourcePosition obj1, SourcePosition obj2)
		{
			return obj1.CompareTo(obj2) > 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourcePosition"/> 是否大于等于第二个 <see cref="SourcePosition"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourcePosition"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourcePosition"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourcePosition"/> 对象大于等于第二个 <see cref="SourcePosition"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >=(SourcePosition obj1, SourcePosition obj2)
		{
			return obj1.CompareTo(obj2) >= 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourcePosition"/> 是否小于第二个 <see cref="SourcePosition"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourcePosition"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourcePosition"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourcePosition"/> 对象小于第二个 <see cref="SourcePosition"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <(SourcePosition obj1, SourcePosition obj2)
		{
			return obj1.CompareTo(obj2) < 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourcePosition"/> 是否小于等于第二个 <see cref="SourcePosition"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourcePosition"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourcePosition"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourcePosition"/> 对象小于等于第二个 <see cref="SourcePosition"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <=(SourcePosition obj1, SourcePosition obj2)
		{
			return obj1.CompareTo(obj2) <= 0;
		}

		#endregion // 运算符重载

	}
}
