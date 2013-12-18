using System;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件中的位置信息。
	/// </summary>
	[Serializable]
	public struct SourceLocation : IEquatable<SourceLocation>, IComparable<SourceLocation>
	{
		/// <summary>
		/// 表示未知的位置信息。
		/// </summary>
		public readonly static SourceLocation Unknown;
		/// <summary>
		/// 所在的行。
		/// </summary>
		private int line;
		/// <summary>
		/// 所在的列。
		/// </summary>
		private int col;
		/// <summary>
		/// 从零开始的索引。
		/// </summary>
		private int index;
		/// <summary>
		/// 使用索引、行和列初始化 <see cref="SourceLocation"/> 结构的新实例。
		/// </summary>
		/// <param name="idx">从零开始的索引。</param>
		/// <param name="line">所在的行。</param>
		/// <param name="col">所在的列。</param>
		public SourceLocation(int idx, int line, int col)
		{
			if (idx < 0)
			{
				throw ExceptionHelper.ArgumentNegative("idx");
			}
			if (line < 1)
			{
				throw ExceptionHelper.ArgumentOutOfRange("line");
			}
			if (col < 1)
			{
				throw ExceptionHelper.ArgumentOutOfRange("col");
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

		#region IComparable<SourceLocation> 成员

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
		/// <remarks>首先按行比较，其次按列比较，最后考虑索引的值。</remarks>
		public int CompareTo(SourceLocation other)
		{
			int cmp = this.line - other.line;
			if (cmp != 0)
			{
				return cmp;
			}
			cmp = this.col - other.col;
			if (cmp != 0)
			{
				return cmp;
			}
			return this.index - other.index;
		}

		#endregion // IComparable<SourceLocation> 成员

		#region IEquatable<SourceLocation> 成员

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
		public bool Equals(SourceLocation other)
		{
			if (index != other.index)
			{
				return false;
			}
			if (line != other.line)
			{
				return false;
			}
			return col == other.col;
		}

		#endregion // IEquatable<SourceLocation> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="SourceLocation"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="SourceLocation"/> 进行比较的 <see cref="System.Object"/>。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="SourceLocation"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SourceLocation))
			{
				return false;
			}
			return this.Equals((SourceLocation)obj);
		}

		/// <summary>
		/// 用于 <see cref="SourceLocation"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="SourceLocation"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			int hashCode = 345421;
			hashCode ^= Index;
			hashCode ^= Line << 16 + col;
			return hashCode;
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			if (this == Unknown)
			{
				return "Unknown";
			}
			return string.Concat(index, "(Line ", line, ", Col ", col, ")");
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="SourceLocation"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceLocation"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceLocation"/> 对象。</param>
		/// <returns>如果两个 <see cref="SourceLocation"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(SourceLocation obj1, SourceLocation obj2)
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
		/// 判断两个 <see cref="SourceLocation"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceLocation"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceLocation"/> 对象。</param>
		/// <returns>如果两个 <see cref="SourceLocation"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(SourceLocation obj1, SourceLocation obj2)
		{
			return !(obj1 == obj2);
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceLocation"/> 是否大于第二个 <see cref="SourceLocation"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceLocation"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceLocation"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceLocation"/> 对象大于第二个 <see cref="SourceLocation"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >(SourceLocation obj1, SourceLocation obj2)
		{
			return obj1.CompareTo(obj2) > 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceLocation"/> 是否大于等于第二个 <see cref="SourceLocation"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceLocation"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceLocation"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceLocation"/> 对象大于等于第二个 <see cref="SourceLocation"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator >=(SourceLocation obj1, SourceLocation obj2)
		{
			return obj1.CompareTo(obj2) >= 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceLocation"/> 是否小于第二个 <see cref="SourceLocation"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceLocation"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceLocation"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceLocation"/> 对象小于第二个 <see cref="SourceLocation"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <(SourceLocation obj1, SourceLocation obj2)
		{
			return obj1.CompareTo(obj2) < 0;
		}
		/// <summary>
		/// 判断第一个 <see cref="SourceLocation"/> 是否小于等于第二个 <see cref="SourceLocation"/>。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="SourceLocation"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="SourceLocation"/> 对象。</param>
		/// <returns>如果第一个 <see cref="SourceLocation"/> 对象小于等于第二个 <see cref="SourceLocation"/> 对象，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator <=(SourceLocation obj1, SourceLocation obj2)
		{
			return obj1.CompareTo(obj2) <= 0;
		}

		#endregion // 运算符重载

	}
}
