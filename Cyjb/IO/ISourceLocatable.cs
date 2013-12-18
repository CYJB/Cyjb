using System.Diagnostics.CodeAnalysis;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示可在源文件中定位的对象。
	/// </summary>
	/// <remarks>要求 <see cref="Start"/> 属性小于等于 <see cref="End"/> 属性。
	/// <see cref="Start"/> 属性和 <see cref="End"/> 属性可以同时为 <see cref="SourceLocation.Unknown"/>，
	/// 表示未知的位置。</remarks>
	public interface ISourceLocatable
	{
		/// <summary>
		/// 获取在源文件中的起始位置。
		/// </summary>
		/// <value>源文件中的起始位置，表示第一个字符的位置。</value>
		SourceLocation Start { get; }
		/// <summary>
		/// 获取在源文件中的结束位置。
		/// </summary>
		/// <value>源文件中的结束位置，表示最后一个字符的位置。</value>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "End")]
		SourceLocation End { get; }
	}
}
