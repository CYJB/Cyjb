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
		public static T[] Random<T>(this T[] array)
		{
			for (int i = array.Length - 1; i > 0; i--)
			{
				int j = RandomExt.Next(i);
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
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		public static T[,] Random<T>(this T[,] array)
		{
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = array.GetLength(1) - 1; j >= 0; j--)
				{
					int a = RandomExt.Next(i);
					int b = 0;
					if (a != i)
					{
						b = RandomExt.Next(array.GetLength(1));
					}
					else
					{
						b = RandomExt.Next(j);
					}
					T temp = array[i, j];
					array[i, j] = array[a, b];
					array[a, b] = temp;
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
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"), CLSCompliant(false)]
		public static T[][] Random<T>(this T[][] array)
		{
			for (int i = array.Length - 1; i >= 0; i--)
			{
				for (int j = array[i].Length - 1; j >= 0; j--)
				{
					int a = RandomExt.Next(i);
					int b = 0;
					if (a != i)
					{
						b = RandomExt.Next(array[a].Length);
					}
					else
					{
						b = RandomExt.Next(j);
					}
					T temp = array[i][j];
					array[i][j] = array[a][b];
					array[a][b] = temp;
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
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#"),]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Random<T>(this T[, ,] array)
		{
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = array.GetLength(1) - 1; j >= 0; j--)
				{
					for (int k = array.GetLength(2) - 1; k >= 0; k--)
					{
						int a = RandomExt.Next(i);
						int b = 0, c = 0;
						if (a != i)
						{
							b = RandomExt.Next(array.GetLength(1));
							c = RandomExt.Next(array.GetLength(2));
						}
						else
						{
							b = RandomExt.Next(j);
							if (b != j)
							{
								c = RandomExt.Next(array.GetLength(2));
							}
							else
							{
								c = RandomExt.Next(b);
							}
						}
						T temp = array[i, j, k];
						array[i, j, k] = array[a, b, c];
						array[a, b, c] = temp;
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
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"), CLSCompliant(false)]
		public static T[][][] Random<T>(this T[][][] array)
		{
			for (int i = array.Length - 1; i >= 0; i--)
			{
				for (int j = array[i].Length - 1; j >= 0; j--)
				{
					for (int k = array[i][j].Length - 1; k >= 0; k--)
					{
						int a = RandomExt.Next(i);
						int b = 0, c = 0;
						if (a != i)
						{
							b = RandomExt.Next(array[a].Length);
							c = RandomExt.Next(array[a][b].Length);
						}
						else
						{
							b = RandomExt.Next(j);
							if (b != j)
							{
								c = RandomExt.Next(array[a][b].Length);
							}
							else
							{
								c = RandomExt.Next(k);
							}
						}
						T temp = array[i][j][k];
						array[i][j][k] = array[a][b][c];
						array[a][b][c] = temp;
					}
				}
			}
			return array;
		}

		#endregion // 随机排序

	}
}
