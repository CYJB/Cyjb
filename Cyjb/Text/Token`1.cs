namespace Cyjb.Text
{
	/// <summary>
	/// 表示一个词法单元。
	/// </summary>
	/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
	/// <remarks><typeparamref name="T"/> 必须的枚举类型，使用该类型的特殊值 
	/// <c>-1</c> 用于表示文件结束，<c>-2</c> 表示语法产生式的错误。</remarks>
	public struct Token<T> : IEquatable<Token<T>>
		where T : struct
	{
		/// <summary>
		/// 使用词法单元的相关信息初始化 <see cref="Token{T}"/> 类的新实例。
		/// </summary>
		/// <param name="kind">词法单元的类型。</param>
		/// <param name="text">词法单元的文本。</param>
		/// <param name="span">词法单元的范围。</param>
		/// <param name="value">词法单元的值。</param>
		public Token(T kind, string text, TextSpan span, object? value = null)
		{
			Kind = kind;
			Text = text;
			Span = span;
			Value = value;
		}

		/// <summary>
		/// 获取词法单元的类型。
		/// </summary>
		/// <value>词法单元的类型。</value>
		public T Kind { get; }

		/// <summary>
		/// 获取词法单元的文本。
		/// </summary>
		/// <value>词法单元的文本。</value>
		public string Text { get; }

		/// <summary>
		/// 获取词法单元的范围。
		/// </summary>
		/// <value>词法单元的范围。</value>
		public TextSpan Span { get; }

		/// <summary>
		/// 获取词法单元的值。
		/// </summary>
		/// <value>词法单元的值。</value>
		public object? Value { get; }

		#region IEquatable<Token> 成员

		/// <summary>
		/// 返回当前对象是否等于同一类型的另一对象。
		/// </summary>
		/// <param name="other">要比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Equals(Token<T> other)
		{
			return EqualityComparer<T>.Default.Equals(Kind, other.Kind) && Text == other.Text && Span == other.Span;
		}

		/// <summary>
		/// 返回当前对象是否等于另一对象。
		/// </summary>
		/// <param name="obj">要与当前对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="obj"/>，则为 true；否则为 false。</returns>
		public override bool Equals(object? obj)
		{
			if (obj is Token<T> other)
			{
				return Equals(other);
			}
			return false;
		}

		/// <summary>
		/// 返回当前对象的哈希值。
		/// </summary>
		/// <returns>当前对象的哈希值。</returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(Kind, Text, Span);
		}

		/// <summary>
		/// 返回指定的 <see cref="Token{T}"/> 是否相等。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator ==(Token<T> left, Token<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// 返回指定的 <see cref="Token{T}"/> 是否不相等。
		/// </summary>
		/// <param name="left">要比较的第一个对象。</param>
		/// <param name="right">要比较的第二个对象。</param>
		/// <returns>如果 <paramref name="left"/> 等于 <paramref name="right"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator !=(Token<T> left, Token<T> right)
		{
			return !left.Equals(right);
		}

		#endregion // IEquatable<Token> 成员

		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			string result = Kind.ToString()!;
			if (Text.Length > 0)
			{
				result += " \"" + Text + "\"";
			}
			return result;
		}
	}
}