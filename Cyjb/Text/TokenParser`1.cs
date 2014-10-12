using System.Collections.Generic;

namespace Cyjb.Text
{
	/// <summary>
	/// 表示词法单元的分析器。
	/// </summary>
	/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
	public abstract class TokenParser<T>
		where T : struct
	{
		/// <summary>
		/// 初始化 <see cref="TokenParser{T}"/> 类的新实例。
		/// </summary>
		protected TokenParser() { }
		/// <summary>
		/// 获取词法单元的分析结果。
		/// </summary>
		/// <value>词法单元的分析结果。其值储存在 <see cref="Token{T}.Value"/> 属性中。</value>
		public abstract Token<T> Result { get; }
		/// <summary>
		/// 分析当前的词法单元。
		/// </summary>
		/// <param name="token">要分析的词法单元。</param>
		/// <overloads>
		/// <summary>
		/// 分析词法单元。
		/// </summary>
		/// </overloads>
		public abstract void Parse(Token<T> token);
		/// <summary>
		/// 分析指定的词法单元序列。
		/// </summary>
		/// <param name="tokens">要分析的词法单元序列。</param>
		public void Parse(IEnumerable<Token<T>> tokens)
		{
			foreach (Token<T> token in tokens)
			{
				Parse(token);
			}
		}
	}
}
