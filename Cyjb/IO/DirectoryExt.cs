using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Cyjb.IO
{
	/// <summary>
	/// 提供对 <see cref="Directory"/> 类和 <see cref="DirectoryInfo"/> 类的扩展方法。
	/// </summary>
	public static class DirectoryExt
	{
		/// <summary>
		/// 返回带有分隔符的目录。
		/// </summary>
		/// <param name="path">要添加分隔符的目录。</param>
		/// <returns>带有分隔符的目录。</returns>
		public static string AppendSeparator(string path)
		{
			if (path.IsNullOrWhiteSpace())
			{
				return path;
			}
			char lastChar = path[path.Length - 1];
			if (lastChar == Path.VolumeSeparatorChar ||
				lastChar == Path.DirectorySeparatorChar ||
				lastChar == Path.AltDirectorySeparatorChar)
			{
				return path;
			}
			return string.Concat(path, Path.DirectorySeparatorChar);
		}
		/// <summary>
		/// 获取带有分隔符的完整目录。
		/// </summary>
		/// <param name="info">要获取的目录信息。</param>
		/// <returns>带有分隔符的完整目录。</returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static string FullNameWithSeparator(this DirectoryInfo info)
		{
			return AppendSeparator(info.FullName);
		}
	}
}
