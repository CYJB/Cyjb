using System;
using Cyjb.IO;

namespace Cyjb.Text
{
	/// <summary>
	/// 表示一个词法单元。在比较时不考虑 <see cref="Token.Value"/> 属性。
	/// </summary>
	public struct Token : IEquatable<Token>
	{
		/// <summary>
		/// 表示文件结束的词法单元标识符。
		/// </summary>
		public const string EndOfFile = "EOF";
		/// <summary>
		/// 返回表示文件结束的词法单元。
		/// </summary>
		/// <param name="loc">文件结束的位置。</param>
		/// <returns>表示文件结束的词法单元。</returns>
		/// <overloads>
		/// <summary>
		/// 返回表示文件结束的词法单元。
		/// </summary>
		/// </overloads>
		public static Token GetEndOfFile(SourceLocation loc)
		{
			return new Token(EndOfFile, string.Empty, loc);
		}
		/// <summary>
		/// 返回表示文件结束的词法单元。
		/// </summary>
		/// <param name="loc">文件结束的位置。</param>
		/// <param name="value">词法单元的值。</param>
		/// <returns>表示文件结束的词法单元。</returns>
		public static Token GetEndOfFile(SourceLocation loc, object value)
		{
			return new Token(EndOfFile, string.Empty, loc, SourceLocation.Invalid, value);
		}
		/// <summary>
		/// 词法单元的标识符。
		/// </summary>
		private string id;
		/// <summary>
		/// 词法单元的文本。
		/// </summary>
		private string text;
		/// <summary>
		/// 词法单元的起始位置。
		/// </summary>
		private SourceLocation start;
		/// <summary>
		/// 词法单元的结束位置。
		/// </summary>
		private SourceLocation end;
		/// <summary>
		/// 词法单元的值。
		/// </summary>
		private object value;
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token"/> 结构的新实例。
		/// 结束位置会被置为无效位置。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="start">起始位置。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="Token"/> 结构的新实例。
		/// </summary>
		/// </overloads>
		public Token(string id, string text, SourceLocation start)
		{
			this.id = id;
			this.text = text;
			this.start = start;
			this.end = SourceLocation.Invalid;
			this.value = null;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token"/> 结构的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="start">起始位置。</param>
		/// <param name="end">结束位置。</param>
		public Token(string id, string text, SourceLocation start, SourceLocation end)
		{
			if (start > end && end != SourceLocation.Invalid)
			{
				throw ExceptionHelper.ReversedArgument("start", "end");
			}
			this.id = id;
			this.text = text;
			this.start = start;
			this.end = end;
			this.value = null;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token"/> 结构的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="start">起始位置。</param>
		/// <param name="end">结束位置。</param>
		/// <param name="value">词法单元的值。</param>
		public Token(string id, string text, SourceLocation start, SourceLocation end, object value)
		{
			if (start > end && end != SourceLocation.Invalid)
			{
				throw ExceptionHelper.ReversedArgument("start", "end");
			}
			this.id = id;
			this.text = text;
			this.start = start;
			this.end = end;
			this.value = value;
		}
		/// <summary>
		/// 获取词法单元的标识符。
		/// </summary>
		/// <value>词法单元的标识符。</value>
		public string Id { get { return id; } }
		/// <summary>
		/// 获取词法单元的文本。
		/// </summary>
		/// <value>词法单元的文本。</value>
		public string Text { get { return text; } }
		/// <summary>
		/// 获取词法单元的起始位置。
		/// </summary>
		/// <value>词法单元的起始位置。</value>
		public SourceLocation Start { get { return start; } }
		/// <summary>
		/// 获取词法单元的结束位置。
		/// </summary>
		/// <value>词法单元的结束位置。</value>
		public SourceLocation End { get { return end; } }
		/// <summary>
		/// 获取词法单元的值。
		/// </summary>
		/// <value>词法单元的值。</value>
		public object Value { get { return value; } }
		/// <summary>
		/// 获取当前词法单元是否表示文件的结束。
		/// </summary>
		/// <value>如果表示文件的结束，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsEndOfFile
		{
			get { return this.id == EndOfFile; }
		}

		#region IEquatable<Token> 成员

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
		public bool Equals(Token other)
		{
			if (this.id != other.id)
			{
				return false;
			}
			if (this.text != other.text)
			{
				return false;
			}
			if (this.start != other.start)
			{
				return false;
			}
			return this.end == other.end;
		}

		#endregion // IEquatable<Token> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="Token"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="Token"/> 进行比较的 <see cref="System.Object"/>。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="Token"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Token))
			{
				return false;
			}
			return this.Equals((Token)obj);
		}

		/// <summary>
		/// 用于 <see cref="Token"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="Token"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			int hashCode = 3321;
			if (id != null)
			{
				hashCode ^= id.GetHashCode();
			}
			hashCode ^= text.GetHashCode();
			hashCode ^= start.GetHashCode() << 4;
			hashCode ^= end.GetHashCode() << 8;
			return hashCode;
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Concat("[", id, "]");
			}
			else
			{
				return string.Concat("[", id, "] ", text);
			}
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="Token"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Token"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Token"/> 对象。</param>
		/// <returns>如果两个 <see cref="Token"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(Token obj1, Token obj2)
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
		/// 判断两个 <see cref="Token"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Token"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Token"/> 对象。</param>
		/// <returns>如果两个 <see cref="Token"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(Token obj1, Token obj2)
		{
			return !(obj1 == obj2);
		}

		#endregion // 运算符重载

	}
}
