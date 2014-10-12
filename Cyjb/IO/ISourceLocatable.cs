using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示可在源文件中定位的对象。
	/// </summary>
	/// <remarks>
	/// <para>表示从 <see cref="Start"/> 开始，到 <see cref="End"/> （包含）结束的范围，
	/// 要求 <see cref="Start"/> 属性的值小于等于 <see cref="End"/> 属性的值。</para>
	/// <para>若 <see cref="Start"/> 属性和 <see cref="End"/> 属性的值都为 <see cref="SourcePosition.Unknown"/>，
	/// 则表示未知的位置。</para>
	/// </remarks>
	[ContractClass(typeof(ContractsForISourceLocatable))]
	public interface ISourceLocatable
	{
		/// <summary>
		/// 获取在源文件中的起始位置。
		/// </summary>
		/// <value>源文件中的起始位置，表示第一个字符的位置。</value>
		SourcePosition Start { get; }
		/// <summary>
		/// 获取在源文件中的结束位置。
		/// </summary>
		/// <value>源文件中的结束位置，表示最后一个字符的位置。</value>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "End")]
		SourcePosition End { get; }
	}
	/// <summary>
	/// 表示 <see cref="ISourceLocatable"/> 接口的协定。
	/// </summary>
	[ContractClassFor(typeof(ISourceLocatable))]
	internal abstract class ContractsForISourceLocatable : ISourceLocatable
	{
		/// <summary>
		/// 初始化 <see cref="ContractsForISourceLocatable"/> 类的新实例。
		/// </summary>
		private ContractsForISourceLocatable() { }
		/// <summary>
		/// 获取在源文件中的起始位置。
		/// </summary>
		/// <value>源文件中的起始位置，表示第一个字符的位置。</value>
		SourcePosition ISourceLocatable.Start
		{
			get { return SourcePosition.Unknown; }
		}
		/// <summary>
		/// 获取在源文件中的结束位置。
		/// </summary>
		/// <value>源文件中的结束位置，表示最后一个字符的位置。</value>
		SourcePosition ISourceLocatable.End
		{
			get { return SourcePosition.Unknown; }
		}
		/// <summary>
		/// 对象不变量。
		/// </summary>
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			ISourceLocatable locatable = this;
			Contract.Invariant(
				(locatable.Start.IsUnknown && locatable.End == SourcePosition.Unknown) ||
				((!locatable.Start.IsUnknown) && locatable.Start < locatable.End));
		}
	}
}
