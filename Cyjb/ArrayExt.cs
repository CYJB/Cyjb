using System;
using System.Diagnostics.CodeAnalysis;

namespace Cyjb
{
	/// <summary>
	/// 提供数组的扩展方法。
	/// </summary>
	public static class ArrayExt
	{

		#region 填充

		#region 一维数组

		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>要填充的数组。</returns>
		public static T[] Fill<T>(this T[] array, T value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = value;
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		public static T[] Fill<T>(this T[] array, Func<T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = value();
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		public static T[] Fill<T>(this T[] array, Func<int, T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = value(i);
				}
			}
			return array;
		}

		#endregion // 一维数组

		#region 二维数组

		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>要填充的数组。</returns>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[,] Fill<T>(this T[,] array, T value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.GetLength(0); i++)
				{
					for (int j = 0; j < array.GetLength(1); j++)
					{
						array[i, j] = value;
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[,] Fill<T>(this T[,] array, Func<T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.GetLength(0); i++)
				{
					for (int j = 0; j < array.GetLength(1); j++)
					{
						array[i, j] = value();
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[,] Fill<T>(this T[,] array, Func<int, int, T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.GetLength(0); i++)
				{
					for (int j = 0; j < array.GetLength(1); j++)
					{
						array[i, j] = value(i, j);
					}
				}
			}
			return array;
		}

		#endregion // 二维数组

		#region 二维交错数组

		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>要填充的数组。</returns>
		public static T[][] Fill<T>(this T[][] array, T value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[i].Length; j++)
					{
						array[i][j] = value;
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		public static T[][] Fill<T>(this T[][] array, Func<T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[i].Length; j++)
					{
						array[i][j] = value();
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		public static T[][] Fill<T>(this T[][] array, Func<int, int, T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[i].Length; j++)
					{
						array[i][j] = value(i, j);
					}
				}
			}
			return array;
		}

		#endregion // 二维交错数组

		#region 三维数组

		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>要填充的数组。</returns>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Fill<T>(this T[, ,] array, T value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.GetLength(0); i++)
				{
					for (int j = 0; j < array.GetLength(1); j++)
					{
						for (int k = 0; k < array.GetLength(2); k++)
						{
							array[i, j, k] = value;
						}
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Fill<T>(this T[, ,] array, Func<T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.GetLength(0); i++)
				{
					for (int j = 0; j < array.GetLength(1); j++)
					{
						for (int k = 0; k < array.GetLength(2); k++)
						{
							array[i, j, k] = value();
						}
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Fill<T>(this T[, ,] array, Func<int, int, int, T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.GetLength(0); i++)
				{
					for (int j = 0; j < array.GetLength(1); j++)
					{
						for (int k = 0; k < array.GetLength(2); k++)
						{
							array[i, j, k] = value(i, j, k);
						}
					}
				}
			}
			return array;
		}

		#endregion // 三维数组

		#region 三维交错数组

		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>要填充的数组。</returns>
		public static T[][][] Fill<T>(this T[][][] array, T value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[j].Length; j++)
					{
						for (int k = 0; k < array[j][k].Length; k++)
						{
							array[i][j][k] = value;
						}
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		public static T[][][] Fill<T>(this T[][][] array, Func<T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[j].Length; j++)
					{
						for (int k = 0; k < array[j][k].Length; k++)
						{
							array[i][j][k] = value();
						}
					}
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的 <paramref name="value"/> 填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数。</param>
		/// <returns>要填充的数组。</returns>
		public static T[][][] Fill<T>(this T[][][] array, Func<int, int, int, T> value)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[j].Length; j++)
					{
						for (int k = 0; k < array[j][k].Length; k++)
						{
							array[i][j][k] = value(i, j, k);
						}
					}
				}
			}
			return array;
		}

		#endregion // 三维交错数组

		#endregion // 填充

		#region 随机排序

		/// <summary>
		/// 将数组进行随机排序。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要随机排序的数组。</param>
		/// <returns>要随机排序数组。</returns>
		/// <remarks>采用下面的代码进行测试：
		/// <code>int size = 10;
		/// int[] arr = new int[size];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i &lt; 200; i++)
		/// {
		/// 	arr.Fill(n => n).Random();
		/// 	for (int j = 0; j &lt; size; j++) cnt[j, arr[j]]++;
		/// }
		/// for (int i = 0; i &lt; size; i++)
		/// {
		/// 	for (int j = 0; j &lt; size; j++)
		/// 		Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		public static T[] Random<T>(this T[] array)
		{
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
		/// <param name="array">要随机排序的数组。</param>
		/// <returns>要随机排序数组。</returns>
		/// <remarks>采用下面的代码进行测试：
		/// <code>int w = 4;
		/// int h = 3;
		/// int size = w * h;
		/// int[,] arr = new int[h, w];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i &lt; 320; i++)
		/// {
		/// 	arr.Fill((y, x) => y * w + x).Random();
		/// 	for (int j = 0; j &lt; size; j++) cnt[j, arr[j / w, j % w]]++;
		/// }
		/// for (int i = 0; i &lt; size; i++)
		/// {
		/// 	for (int j = 0; j &lt; size; j++) Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		public static T[,] Random<T>(this T[,] array)
		{
			int w = array.GetLength(1);
			int idx = array.Length;
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = w - 1; j >= 0; j--)
				{
					int r = RandomExt.Next(idx--);
					int x = r % w;
					int y = r / w;
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
		/// <param name="array">要随机排序的数组。</param>
		/// <returns>要随机排序数组。</returns>
		/// <remarks>采用下面的代码进行测试：
		/// <code>int w = 2;
		/// int h = 2;
		/// int d = 3;
		/// int size = w * h * d;
		/// int[, ,] arr = new int[d, h, w];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i &lt; 240; i++)
		/// {
		/// 	arr.Fill((z, y, x) => z * w * h + y * w + x);
		/// 	arr.Random();
		/// 	for (int j = 0; j &lt; size; j++) cnt[j, arr[j / (w * h), j / w % h, j % w]]++;
		/// }
		/// for (int i = 0; i &lt; size; i++)
		/// {
		/// 	for (int j = 0; j &lt; size; j++) Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#"),]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Random<T>(this T[, ,] array)
		{
			int h = array.GetLength(1);
			int w = array.GetLength(2);
			int idx = array.Length;
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = h - 1; j >= 0; j--)
				{
					for (int k = w - 1; k >= 0; k--)
					{
						int r = RandomExt.Next(idx--);
						int x = r % w;
						int z = r / w;
						int y = z % h;
						z /= h;
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
