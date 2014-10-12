namespace Cyjb.IO
{
	/// <summary>
	/// 表示可在指定源文件中定位的对象。
	/// </summary>
	public interface ISourceFileLocatable : ISourceLocatable
	{
		/// <summary>
		/// 获取源文件的名称。
		/// </summary>
		/// <value>源文件的名称。</value>
		string FileName { get; }
	}
}
