using System.Globalization;
using System.IO;

namespace Cyjb.IO
{
	/// <summary>
	/// 提供对 <see cref="System.IO.File"/> 类的扩展方法。
	/// </summary>
	public static class FileExt
	{
		/// <summary>
		/// 返回与当前区域性相关的文件名。
		/// </summary>
		/// <param name="path">文件的路径。</param>
		/// <param name="fileName">文件的名称。</param>
		/// <returns>与当前区域性相关的文件名，如果不存在则为 <c>null</c>。</returns>
		public static string GetCultureSpecificFile(string path, string fileName)
		{
			return GetCultureSpecificFile(path, fileName, CultureInfo.CurrentCulture);
		}
		/// <summary>
		/// 返回与特定区域性相关的文件名。
		/// </summary>
		/// <param name="path">文件的路径。</param>
		/// <param name="fileName">文件的名称。</param>
		/// <param name="culture">要获取的文件的区域性信息。</param>
		/// <returns>与特定区域性相关的文件名，如果不存在则为 <c>null</c>。</returns>
		public static string GetCultureSpecificFile(string path, string fileName, CultureInfo culture)
		{
			while (true)
			{
				string filePath = Path.Combine(path, culture.Name, fileName);
				if (File.Exists(filePath))
				{
					return filePath;
				}
				if (culture == CultureInfo.InvariantCulture)
				{
					break;
				}
				culture = culture.Parent;
			}
			return null;
		}
	}
}
