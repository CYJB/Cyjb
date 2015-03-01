using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Cyjb
{
	public static partial class ArrayExt
	{

		#region 随机排序

		/// <summary>
		/// 将数组进行随机排序。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要进行随机排序的数组。</param>
		/// <returns>已完成随机排序的数组。</returns>
		/// <remarks>应保证每个元素出现在每个位置的概率基本相同。
		/// 采用下面的代码进行测试：
		/// <code>int size = 10;
		/// int[] arr = new int[size];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i { 200; i++)
		/// {
		/// 	arr.Fill(n => n).Random();
		/// 	for (int j = 0; j { size; j++) cnt[j, arr[j]]++;
		/// }
		/// for (int i = 0; i { size; i++)
		/// {
		/// 	for (int j = 0; j { size; j++)
		/// 		Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		/// <overloads>
		/// <summary>
		/// 将数组进行随机排序。
		/// </summary>
		/// </overloads>
		public static T[] Random<T>(this T[] array)
		{
			CommonExceptions.CheckArgumentNull(array, "array");
			Contract.Ensures(Contract.Result<T[]>() != null);
			for (int i = array.Length - 1; i > 0; i--)
			{
				int j = RandomExt.Next(i + 1);
				if (j != i)
				{
					T temp = array[i];
					array[i] = array[j];
					array[j] = temp;
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组进行随机排序。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要进行随机排序的数组。</param>
		/// <returns>已完成随机排序的数组。</returns>
		/// <remarks>应保证每个元素出现在每个位置的概率基本相同。
		/// 采用下面的代码进行测试：
		/// <code>int w = 4;
		/// int h = 3;
		/// int size = w * h;
		/// int[,] arr = new int[h, w];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i { 320; i++)
		/// {
		/// 	arr.Fill((y, x) => y * w + x).Random();
		/// 	for (int j = 0; j { size; j++) cnt[j, arr[j / w, j % w]]++;
		/// }
		/// for (int i = 0; i { size; i++)
		/// {
		/// 	for (int j = 0; j { size; j++) Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		public static T[,] Random<T>(this T[,] array)
		{
			CommonExceptions.CheckArgumentNull(array, "array");
			Contract.Ensures(Contract.Result<T[,]>() != null);
			int w = array.GetLength(1);
			int idx = array.Length;
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = w - 1; j >= 0; j--)
				{
					Contract.Assume(idx >= 0);
					int r = RandomExt.Next(idx--);
					int y = r / w;
					int x = r - y * w; // r % w
					if (y != i || x != j)
					{
						T temp = array[i, j];
						array[i, j] = array[y, x];
						array[y, x] = temp;
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组进行随机排序。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要进行随机排序的数组。</param>
		/// <returns>已完成随机排序的数组。</returns>
		/// <remarks>应保证每个元素出现在每个位置的概率基本相同。
		/// 采用下面的代码进行测试：
		/// <code>int w = 2;
		/// int h = 2;
		/// int d = 3;
		/// int size = w * h * d;
		/// int[, ,] arr = new int[d, h, w];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i { 240; i++)
		/// {
		/// 	arr.Fill((z, y, x) => z * w * h + y * w + x);
		/// 	arr.Random();
		/// 	for (int j = 0; j { size; j++) cnt[j, arr[j / (w * h), j / w % h, j % w]]++;
		/// }
		/// for (int i = 0; i { size; i++)
		/// {
		/// 	for (int j = 0; j { size; j++) Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#"),]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Random<T>(this T[, ,] array)
		{
			CommonExceptions.CheckArgumentNull(array, "array");
			Contract.Ensures(Contract.Result<T[, ,]>() != null);
			int h = array.GetLength(1);
			int w = array.GetLength(2);
			int idx = array.Length;
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = h - 1; j >= 0; j--)
				{
					for (int k = w - 1; k >= 0; k--)
					{
						Contract.Assume(idx >= 0);
						int r = RandomExt.Next(idx--);
						int t = r / w;
						int x = r - t * w; // r % w
						int z = t / h;
						int y = t - z * h; // t % h
						if (z != i || y != j || x != k)
						{
							T temp = array[i, j, k];
							array[i, j, k] = array[z, y, x];
							array[z, y, x] = temp;
						}
					}
				}
			}
			return array;
		}

		#endregion // 随机排序

	}
}
