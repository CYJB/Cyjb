using Cyjb.IO;

namespace Cyjb.Text
{
	/// <summary>
	/// 表示词法单元的读取器。
	/// </summary>
	public abstract class TokenReader
	{
		/// <summary>
		/// 要读取的下一个词法单元。
		/// </summary>
		private Token nextToken;
		/// <summary>
		/// 是否已读取下一个词法单元。
		/// </summary>
		private bool peekToken = false;
		/// <summary>
		/// 使用要扫描的源文件初始化 <see cref="TokenReader"/> 类的新实例。
		/// </summary>
		/// <param name="reader">要使用的源文件读取器。</param>
		protected TokenReader(SourceReader reader)
		{
			this.Source = reader;
		}
		/// <summary>
		/// 获取要扫描的源文件。
		/// </summary>
		/// <value>要扫描的源文件。</value>
		public SourceReader Source { get; private set; }
		/// <summary>
		/// 读取输入流中的下一个词法单元并提升输入流的字符位置。
		/// </summary>
		/// <returns>输入流中的下一个词法单元。</returns>
		public Token ReadToken()
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
		public Token PeekToken()
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
		protected abstract Token InternalReadToken();
	}
}
