using System.IO;

namespace Cyjb.IO
{
	/// <summary>
	/// 提供对 <see cref="System.IO.Directory"/> 类的扩展方法。
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
			if (string.IsNullOrWhiteSpace(path))
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
	}
}
