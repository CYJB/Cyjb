using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Cyjb.IO;
using Cyjb.Utility;

namespace Cyjb.Text
{
	/// <summary>
	/// 表示一个词法单元。在比较时不考虑 <see cref="Token{T}.Value"/> 属性。
	/// </summary>
	/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
	/// <remarks><typeparamref name="T"/> 必须的枚举类型，使用该类型的特殊值 
	/// <c>-1</c> 用于表示文件结束，<c>-2</c> 表示语法产生式的错误。</remarks>
	public class Token<T> : ISourceLocatable, IEquatable<Token<T>>
		where T : struct
	{

		#region 静态成员

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
		public static Token<T> GetEndOfFile(SourcePosition loc)
		{
			return new Token<T>(EndOfFile, string.Empty, loc);
		}
		/// <summary>
		/// 返回表示文件结束的词法单元。
		/// </summary>
		/// <param name="loc">文件结束的位置。</param>
		/// <param name="value">词法单元的值。</param>
		/// <returns>表示文件结束的词法单元。</returns>
		public static Token<T> GetEndOfFile(SourcePosition loc, object value)
		{
			return new Token<T>(EndOfFile, string.Empty, loc, loc, value);
		}

		#endregion // 静态成员

		/// <summary>
		/// 词法单元的标识符。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly T id;
		/// <summary>
		/// 词法单元的文本。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string text;
		/// <summary>
		/// 词法单元的起始位置。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourcePosition start;
		/// <summary>
		/// 词法单元的结束位置。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourcePosition end;
		/// <summary>
		/// 词法单元的值。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly object value;

		#region 构造函数

		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token{T}"/> 类的新实例。
		/// 起始位置和结束位置都会被设置为 <paramref name="loc"/>。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="loc">词法单元的位置。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="Token{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public Token(T id, string text, SourcePosition loc)
		{
			this.id = id;
			this.text = text;
			this.start = this.end = loc;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token{T}"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="start">起始位置。</param>
		/// <param name="end">结束位置。</param>
		public Token(T id, string text, SourcePosition start, SourcePosition end)
		{
			CommonExceptions.CheckSourceRange(start, end);
			Contract.EndContractBlock();
			this.id = id;
			this.text = text;
			this.start = start;
			this.end = end;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token{T}"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="range">位置范围。</param>
		public Token(T id, string text, ISourceLocatable range)
		{
			CommonExceptions.CheckSourceLocatable(range, "range");
			Contract.EndContractBlock();
			if (range != null)
			{
				this.start = range.Start;
				this.end = range.End;
			}
			this.id = id;
			this.text = text;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token{T}"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="start">起始位置。</param>
		/// <param name="end">结束位置。</param>
		/// <param name="value">词法单元的值。</param>
		public Token(T id, string text, SourcePosition start, SourcePosition end, object value)
		{
			CommonExceptions.CheckSourceRange(start, end);
			Contract.EndContractBlock();
			this.id = id;
			this.text = text;
			this.start = start;
			this.end = end;
			this.value = value;
		}
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token{T}"/> 类的新实例。
		/// </summary>
		/// <param name="id">标识符。</param>
		/// <param name="text">文本。</param>
		/// <param name="range">位置范围。</param>
		/// <param name="value">词法单元的值。</param>
		public Token(T id, string text, ISourceLocatable range, object value)
		{
			CommonExceptions.CheckSourceLocatable(range, "range");
			Contract.EndContractBlock();
			if (range != null)
			{
				this.start = range.Start;
				this.end = range.End;
			}
			this.id = id;
			this.text = text;
			this.value = value;
		}

		#endregion // 构造函数

		/// <summary>
		/// 获取词法单元的标识符。
		/// </summary>
		/// <value>词法单元的标识符。</value>
		public T Id { get { return this.id; } }
		/// <summary>
		/// 获取词法单元的文本。
		/// </summary>
		/// <value>词法单元的文本。</value>
		public string Text { get { return this.text; } }
		/// <summary>
		/// 获取词法单元的起始位置。
		/// </summary>
		/// <value>词法单元的起始位置。</value>
		public SourcePosition Start { get { return this.start; } }
		/// <summary>
		/// 获取词法单元的结束位置。
		/// </summary>
		/// <value>词法单元的结束位置。</value>
		public SourcePosition End { get { return this.end; } }
		/// <summary>
		/// 获取词法单元的值。
		/// </summary>
		/// <value>词法单元的值。</value>
		public object Value { get { return this.value; } }
		/// <summary>
		/// 获取当前词法单元是否表示文件的结束。
		/// </summary>
		/// <value>如果表示文件的结束，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsEndOfFile
		{
			get { return EqualityComparer<T>.Default.Equals(this.id, EndOfFile); }
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
			if (ReferenceEquals(other, null))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return EqualityComparer<T>.Default.Equals(this.id, other.id) && this.text == other.text &&
			this.start == other.start && this.end == other.end;
		}

		#endregion // IEquatable<Token> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="Token{T}"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="Token{T}"/> 进行比较的 <see cref="System.Object"/>。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="Token{T}"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			Token<T> token = obj as Token<T>;
			return token != null && this.Equals(token);
		}
		/// <summary>
		/// 用于 <see cref="Token{T}"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="Token{T}"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			return Hash.Combine(Hash.Combine(id.GetHashCode(), text), start, end);
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.IsNullOrEmpty(text) ? id.ToString() : string.Concat(id, " \"", text, "\"");
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="Token{T}"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Token{T}"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Token{T}"/> 对象。</param>
		/// <returns>如果两个 <see cref="Token{T}"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(Token<T> obj1, Token<T> obj2)
		{
			if (ReferenceEquals(obj1, null))
			{
				return ReferenceEquals(obj2, null);
			}
			return obj1.Equals(obj2);
		}

		/// <summary>
		/// 判断两个 <see cref="Token{T}"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Token{T}"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Token{T}"/> 对象。</param>
		/// <returns>如果两个 <see cref="Token{T}"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(Token<T> obj1, Token<T> obj2)
		{
			return !(obj1 == obj2);
		}

		#endregion // 运算符重载

	}
}
