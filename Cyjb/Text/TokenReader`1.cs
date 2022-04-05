using System.Collections;

namespace Cyjb.Text
{
	/// <summary>
	/// 表示词法单元的读取器。
	/// </summary>
	/// <seealso cref="Token{T}"/>
	/// <typeparam name="T">词法单元标识符的类型，必须是一个枚举类型。</typeparam>
	public abstract class TokenReader<T> : IDisposable
		where T : struct
	{
		/// <summary>
		/// 要读取的下一个词法单元。
		/// </summary>
		private Token<T>? nextToken;
		/// <summary>
		/// 是否已读取下一个词法单元。
		/// </summary>
		private bool peekToken;

		/// <summary>
		/// 使用要扫描的源文件初始化 <see cref="TokenReader{T}"/> 类的新实例。
		/// </summary>
		/// <param name="source">要使用的源文件读取器。</param>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
		protected TokenReader(SourceReader source)
		{
			CommonExceptions.CheckArgumentNull(source);
			Source = source;
		}

		/// <summary>
		/// 要扫描的源文件。
		/// </summary>
		protected SourceReader Source { get; }

		/// <summary>
		/// 读取输入流中的下一个词法单元并提升输入流的字符位置。
		/// </summary>
		/// <returns>输入流中的下一个词法单元。</returns>
		public Token<T>? Read()
		{
			if (peekToken)
			{
				peekToken = false;
				return nextToken;
			}
			return InternalRead();
		}

		/// <summary>
		/// 读取输入流中的下一个词法单元，但是并不更改读取器的状态。
		/// </summary>
		/// <returns>输入流中的下一个词法单元。</returns>
		public Token<T>? Peek()
		{
			if (!peekToken)
			{
				peekToken = true;
				nextToken = InternalRead();
			}
			return nextToken;
		}

		/// <summary>
		/// 读取输入流中的下一个词法单元并提升输入流的字符位置。
		/// </summary>
		/// <returns>输入流中的下一个词法单元。</returns>
		protected abstract Token<T>? InternalRead();

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
			Dispose(true);
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
				Source.Dispose();
			}
		}

		#endregion // IDisposable 成员

	}
}