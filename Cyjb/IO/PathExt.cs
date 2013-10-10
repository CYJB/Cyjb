using System.Collections.Generic;
using System.IO;
using System.Text;
using Cyjb.Collections;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.IO
{
	/// <summary>
	/// 对包含文件或目录路径信息的 <see cref="System.String"/> 实例执行操作。
	/// </summary>
	public static class PathExt
	{
		/// <summary>
		/// 在路径中无效的字符集合。
		/// </summary>
		private readonly static ISet<char> invalidPathChars = new ReadOnlySet<char>(
			new CharSet(Path.GetInvalidPathChars()));
		/// <summary>
		/// 在文件名中无效的字符集合。
		/// </summary>
		private readonly static ISet<char> invalidFileNameChars = new ReadOnlySet<char>(
			new CharSet(Path.GetInvalidFileNameChars()));
		/// <summary>
		/// 获取在路径中无效的字符集合。
		/// </summary>
		/// <value>在路径中无效的字符集合。</value>
		public static ISet<char> InvalidPathChars
		{
			get { return invalidPathChars; }
		}
		/// <summary>
		/// 获取在文件名中无效的字符集合。
		/// </summary>
		/// <value>在文件名中无效的字符集合。</value>
		public static ISet<char> InvalidFileNameChars
		{
			get { return invalidFileNameChars; }
		}
		/// <summary>
		/// 返回有效的路径，路径中的无效字符会被直接删去。
		/// </summary>
		/// <param name="path">要获取的路径。</param>
		/// <returns>得到的有效路径。</returns>
		/// <overloads>
		/// <summary>
		/// 返回有效的路径，路径中的无效字符会被删去或替换。
		/// </summary>
		/// </overloads>
		public static string GetValidPath(string path)
		{
			return GetValidPath(path, string.Empty);
		}
		/// <summary>
		/// 返回有效的路径，路径中的无效字符会被替换成给定的字符串。
		/// </summary>
		/// <param name="path">要获取的路径。</param>
		/// <param name="replaceStr">要替换路径中的无效字符的字符串。</param>
		/// <returns>得到的有效路径。</returns>
		public static string GetValidPath(string path, string replaceStr)
		{
			StringBuilder builder = new StringBuilder();
			int len = path.Length;
			for (int i = 0; i < len; i++)
			{
				if (invalidPathChars.Contains(path[i]))
				{
					builder.Append(replaceStr);
				}
				else
				{
					builder.Append(path[i]);
				}
			}
			return builder.ToString();
		}
		/// <summary>
		/// 返回有效的文件名，文件名中的无效字符会被直接删去。
		/// </summary>
		/// <param name="fileName">要获取的文件名。</param>
		/// <returns>得到的有效文件名。</returns>
		/// <overloads>
		/// <summary>
		/// 返回有效的文件名，文件名中的无效字符会被删去或替换。
		/// </summary>
		/// </overloads>
		public static string GetValidFileName(string fileName)
		{
			return GetValidFileName(fileName, string.Empty);
		}
		/// <summary>
		/// 返回有效的文件名，文件名中的无效字符会被替换成给定的字符串。
		/// </summary>
		/// <param name="fileName">要获取的文件名。</param>
		/// <param name="replaceStr">要替换文件名中的无效字符的字符串。</param>
		/// <returns>得到的有效文件名。</returns>
		public static string GetValidFileName(string fileName, string replaceStr)
		{
			StringBuilder builder = new StringBuilder();
			int len = fileName.Length;
			for (int i = 0; i < len; i++)
			{
				if (invalidFileNameChars.Contains(fileName[i]))
				{
					builder.Append(replaceStr);
				}
				else
				{
					builder.Append(fileName[i]);
				}
			}
			return builder.ToString();
		}
	}
}
