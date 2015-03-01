using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Cyjb.Collections;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.IO
{
	/// <summary>
	/// 对包含文件或目录路径信息的 <see cref="string"/> 实例执行操作。
	/// </summary>
	public static class PathExt
	{
		/// <summary>
		/// 在路径中无效的字符集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly static ISet<char> invalidPathChars = new ReadOnlySet<char>(
			new CharSet(Path.GetInvalidPathChars()));
		/// <summary>
		/// 在文件名中无效的字符集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
		/// 检查指定路径是否包含无效的字符。
		/// </summary>
		/// <param name="path">要检查的路径。</param>
		/// <returns>如果指定路径中包含的字符全部是有效的，则为 <c>true</c>；
		/// 否则若包含无效的字符，则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> 为 <c>null</c>。</exception>
		public static bool IsValidPath(string path)
		{
			CommonExceptions.CheckArgumentNull(path, "path");
			Contract.EndContractBlock();
			return path.All(t => !invalidPathChars.Contains(t));
		}
		/// <summary>
		/// 返回有效的路径，路径中的无效字符会被直接删去。
		/// </summary>
		/// <param name="path">要获取的路径。</param>
		/// <returns>得到的有效路径。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 返回有效的路径，路径中的无效字符会被删去或替换。
		/// </summary>
		/// </overloads>
		public static string GetValidPath(string path)
		{
			return GetValidPath(path, null);
		}
		/// <summary>
		/// 返回有效的路径，路径中的无效字符会被替换成给定的字符串。
		/// </summary>
		/// <param name="path">要获取的路径。</param>
		/// <param name="replaceStr">要替换路径中的无效字符的字符串。</param>
		/// <returns>得到的有效路径。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> 为 <c>null</c>。</exception>
		public static string GetValidPath(string path, string replaceStr)
		{
			CommonExceptions.CheckArgumentNull(path, "path");
			Contract.EndContractBlock();
			StringBuilder builder = new StringBuilder(path.Length);
			for (int i = 0; i < path.Length; i++)
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
		/// 检查指定文件名是否包含无效的字符。
		/// </summary>
		/// <param name="fileName">要检查的文件名。</param>
		/// <returns>如果指定文件名中包含的字符全部是有效的，则为 <c>true</c>；
		/// 否则若包含无效的字符，则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="fileName"/> 为 <c>null</c>。</exception>
		public static bool IsValidFileName(string fileName)
		{
			CommonExceptions.CheckArgumentNull(fileName, "fileName");
			Contract.EndContractBlock();
			return fileName.All(t => !invalidFileNameChars.Contains(t));
		}
		/// <summary>
		/// 返回有效的文件名，文件名中的无效字符会被直接删去。
		/// </summary>
		/// <param name="fileName">要获取的文件名。</param>
		/// <returns>得到的有效文件名。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="fileName"/> 为 <c>null</c>。</exception>
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
		/// <exception cref="ArgumentNullException"><paramref name="fileName"/> 为 <c>null</c>。</exception>
		public static string GetValidFileName(string fileName, string replaceStr)
		{
			CommonExceptions.CheckArgumentNull(fileName, "fileName");
			StringBuilder builder = new StringBuilder(fileName.Length);
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
