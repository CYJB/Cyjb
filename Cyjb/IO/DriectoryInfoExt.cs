using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Cyjb.IO
{
	/// <summary>
	/// 提供对 <see cref="System.IO.DirectoryInfo"/> 类的扩展方法。
	/// </summary>
	public static class DirectoryInfoExt
	{
		/// <summary>
		/// 获取带有分隔符的完整目录。
		/// </summary>
		/// <param name="info">要获取的目录信息。</param>
		/// <returns>带有分隔符的完整目录。</returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static string FullNameWithSeparator(this DirectoryInfo info)
		{
			return DirectoryExt.AppendSeparator(info.FullName);
		}
	}
}
