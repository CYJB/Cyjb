using System;
using System.Collections;
using System.Collections.Generic;
using Cyjb.IO;

namespace Cyjb.Text
{
	/// <summary>
	/// 表示词法单元的读取器。
	/// </summary>
	/// <seealso cref="Token{T}"/>
	/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
	public abstract class TokenReader<T> : IDisposable, IEnumerable<Token<T>>
		where T : struct
	{
		/// <summary>
		/// 要读取的下一个词法单元。
		/// </summary>
		private Token<T> nextToken;
		/// <summary>
		/// 是否已读取下一个词法单元。
		/// </summary>
		private bool peekToken = false;
		/// <summary>
		/// 使用要扫描的源文件初始化 <see cref="TokenReader{T}"/> 类的新实例。
		/// </summary>
		/// <param name="reader">要使用的源文件读取器。</param>
		protected TokenReader(SourceReader reader)
		{
			CommonExceptions.CheckArgumentNull(reader, "reader");
			this.Source = reader;
		}

		#region IDisposable 成员

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		/// </overloads>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		/// <param name="disposing">是否释放托管资源。</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Source.Dispose();
			}
		}

		#endregion // IDisposable 成员

		/// <summary>
		/// 获取要扫描的源文件。
		/// </summary>
		/// <value>要扫描的源文件。</value>
		public SourceReader Source { get; private set; }
		/// <summary>
		/// 读取输入流中的下一个词法单元并提升输入流的字符位置。
		/// </summary>
		/// <returns>输入流中的下一个词法单元。</returns>
		public Token<T> ReadToken()
		{
			if (this.peekToken)
			{
				this.peekToken = false;
				return this.nextToken;
			}
			else
			{
				return InternalReadToken();
			}
		}
		/// <summary>
		/// 读取输入流中的下一个词法单元，但是并不更改读取器的状态。
		/// </summary>
		/// <returns>输入流中的下一个词法单元。</returns>
		public Token<T> PeekToken()
		{
			if (!this.peekToken)
			{
				this.peekToken = true;
				this.nextToken = InternalReadToken();
			}
			return this.nextToken;
		}
		/// <summary>
		/// 读取输入流中的下一个词法单元并提升输入流的字符位置。
		/// </summary>
		/// <returns>输入流中的下一个词法单元。</returns>
		protected abstract Token<T> InternalReadToken();

		#region IEnumerable<Token<T>> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator{T}"/>。</returns>
		/// <remarks>在枚举的时候，<see cref="TokenReader{T}"/> 会不断的读出词法单元，
		/// 应当只使用一个枚举器。在使用多个枚举器时，他们之间会相互干扰，导致枚举值与期望的不同。
		/// 要解决这一问题，需要将词法单元缓存到数组中，再进行枚举。</remarks>
		public IEnumerator<Token<T>> GetEnumerator()
		{
			while (true)
			{
				Token<T> token = this.ReadToken();
				yield return token;
				if (token.IsEndOfFile)
				{
					break;
				}
			}
		}

		#endregion // IEnumerable<Token<T>> 成员

		#region IEnumerable 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.IEnumerator"/>。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion // IEnumerable 成员

	}
}
