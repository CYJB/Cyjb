using System;
using System.Collections.Generic;
using Cyjb.IO;

namespace Cyjb.Text
{
	/// <summary>
	/// 表示一个词法单元。在比较时不考虑 <see cref="Token&lt;T&gt;.Value"/> 属性。
	/// </summary>
	/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
	/// <remarks><typeparamref name="T"/> 必须的枚举类型，
	/// 该类型的特殊值 <c>-1</c> 将用于表示文件结束。</remarks>
	public class Token<T> : ISourceLocatable, IEquatable<Token<T>>
		where T : struct
	{
		/// <summary>
		/// 表示文件结束的词法单元标识符。
		/// </summary>
		/// <remarks>其值为 <c>-1</c>。</remarks>
		public static readonly T EndOfFile = (T)Enum.ToObject(typeof(T), -1);
		/// <summary>
		/// 表示语法产生式的错误的特殊标识符。
		/// </summary>
		/// <remarks>该标识符用于定义语法产生式，其值为 <c>-2</c>。</remarks>
		public static readonly T Error = (T)Enum.ToObject(typeof(T), -2);
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
		public static Token<T> GetEndOfFile(SourceLocation loc)
		{
			return new Token<T>(EndOfFile, string.Empty, loc);
		}
		/// <summary>
		/// 返回表示文件结束的词法单元。
		/// </summary>
		/// <param name="loc">文件结束的位置。</param>
		/// <param name="value">词法单元的值。</param>
		/// <returns>表示文件结束的词法单元。</returns>
		public static Token<T> GetEndOfFile(SourceLocation loc, object value)
		{
			return new Token<T>(EndOfFile, string.Empty, loc, loc, value);
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token&lt;T&gt;"/> 类的新实例。
		/// 结束位置会被置为无效位置。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="loc">词法单元的位置。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="Token&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public Token(T id, string text, SourceLocation loc)
		{
			this.Id = id;
			this.Text = text;
			this.Start = this.End = loc;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="start">起始位置。</param>
		/// <param name="end">结束位置。</param>
		public Token(T id, string text, SourceLocation start, SourceLocation end)
		{
			if (start > end)
			{
				throw ExceptionHelper.ReversedArgument("start", "end");
			}
			this.Id = id;
			this.Text = text;
			this.Start = start;
			this.End = end;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="range">位置范围。</param>
		public Token(T id, string text, ISourceLocatable range)
		{
			if (range.Start > range.End)
			{
				throw ExceptionHelper.ReversedArgument("range.Start", "range.End");
			}
			this.Id = id;
			this.Text = text;
			this.Start = range.Start;
			this.End = range.End;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="start">起始位置。</param>
		/// <param name="end">结束位置。</param>
		/// <param name="value">词法单元的值。</param>
		public Token(T id, string text, SourceLocation start, SourceLocation end, object value)
		{
			if (start > end)
			{
				throw ExceptionHelper.ReversedArgument("start", "end");
			}
			this.Id = id;
			this.Text = text;
			this.Start = start;
			this.End = end;
			this.Value = value;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="range">位置范围。</param>
		/// <param name="value">词法单元的值。</param>
		public Token(T id, string text, ISourceLocatable range, object value)
		{
			if (range.Start > range.End)
			{
				throw ExceptionHelper.ReversedArgument("range.Start", "range.End");
			}
			this.Id = id;
			this.Text = text;
			this.Start = range.Start;
			this.End = range.End;
			this.Value = value;
		}
		/// <summary>
		/// 获取词法单元的标识符。
		/// </summary>
		/// <value>词法单元的标识符。</value>
		public T Id { get; private set; }
		/// <summary>
		/// 获取词法单元的文本。
		/// </summary>
		/// <value>词法单元的文本。</value>
		public string Text { get; private set; }
		/// <summary>
		/// 获取词法单元的起始位置。
		/// </summary>
		/// <value>词法单元的起始位置。</value>
		public SourceLocation Start { get; private set; }
		/// <summary>
		/// 获取词法单元的结束位置。
		/// </summary>
		/// <value>词法单元的结束位置。</value>
		public SourceLocation End { get; private set; }
		/// <summary>
		/// 获取词法单元的值。
		/// </summary>
		/// <value>词法单元的值。</value>
		public object Value { get; private set; }
		/// <summary>
		/// 获取当前词法单元是否表示文件的结束。
		/// </summary>
		/// <value>如果表示文件的结束，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsEndOfFile
		{
			get { return EqualityComparer<T>.Default.Equals(this.Id, EndOfFile); }
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
		public bool Equals(Token<T> other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			if (!EqualityComparer<T>.Default.Equals(this.Id, other.Id))
			{
				return false;
			}
			if (this.Text != other.Text)
			{
				return false;
			}
			if (this.Start != other.Start)
			{
				return false;
			}
			return this.End == other.End;
		}

		#endregion // IEquatable<Token> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="Token&lt;T&gt;"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="Token&lt;T&gt;"/> 进行比较的 <see cref="System.Object"/>。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="Token&lt;T&gt;"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			Token<T> token = obj as Token<T>;
			if (token == null)
			{
				return false;
			}
			return this.Equals(token);
		}

		/// <summary>
		/// 用于 <see cref="Token&lt;T&gt;"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="Token&lt;T&gt;"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			int hashCode = 3321;
			hashCode ^= Id.GetHashCode();
			if (Text != null)
			{
				hashCode ^= Text.GetHashCode();
			}
			hashCode ^= Start.GetHashCode() << 4;
			hashCode ^= End.GetHashCode() << 8;
			return hashCode;
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Text))
			{
				return Id.ToString();
			}
			else
			{
				return string.Concat(Id, " \"", Text, "\"");
			}
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="Token&lt;T&gt;"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Token&lt;T&gt;"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Token&lt;T&gt;"/> 对象。</param>
		/// <returns>如果两个 <see cref="Token&lt;T&gt;"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(Token<T> obj1, Token<T> obj2)
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
		/// 判断两个 <see cref="Token&lt;T&gt;"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Token&lt;T&gt;"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Token&lt;T&gt;"/> 对象。</param>
		/// <returns>如果两个 <see cref="Token&lt;T&gt;"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(Token<T> obj1, Token<T> obj2)
		{
			return !(obj1 == obj2);
		}

		#endregion // 运算符重载

	}
}
