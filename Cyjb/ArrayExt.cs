using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cyjb
{
	/// <summary>
	/// 提供数组的扩展方法。
	/// </summary>
	public static class ArrayExt
	{

		#region 填充

		#region Array

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <overloads>
		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// </overloads>
		public static Array Fill(this Array array, object value)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static Array Fill(this Array array, object value, int startIndex)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static Array Fill(this Array array, object value, int startIndex, int length)
		{
			if (array != null)
			{
				if (startIndex < 0 || startIndex >= array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("startIndex");
				}
				int len = startIndex + length;
				if (length < 0 || len > array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("length");
				}
				for (int i = startIndex; i < len; i++)
				{
					array.SetValue(value, i);
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
		public static Array Fill(this Array array, Func<object> value)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static Array Fill(this Array array, Func<object> value, int startIndex)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static Array Fill(this Array array, Func<object> value, int startIndex, int length)
		{
			if (array != null)
			{
				if (startIndex < 0 || startIndex >= array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("startIndex");
				}
				int len = startIndex + length;
				if (length < 0 || len > array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("length");
				}
				for (int i = startIndex; i < len; i++)
				{
					array.SetValue(value(), i);
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
		public static Array Fill(this Array array, Func<int, object> value)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static Array Fill(this Array array, Func<int, object> value, int startIndex)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static Array Fill(this Array array, Func<int, object> value, int startIndex, int length)
		{
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("startIndex");
			}
			int len = startIndex + length;
			if (length < 0 || len > array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("length");
			}
			for (int i = startIndex; i < len; i++)
			{
				array.SetValue(value(i), i);
			}
			return array;
		}

		#endregion // Array

		#region 一维数组

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		public static T[] Fill<T>(this T[] array, T value)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static T[] Fill<T>(this T[] array, T value, int startIndex)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充指定的长度。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static T[] Fill<T>(this T[] array, T value, int startIndex, int length)
		{
			if (array != null)
			{
				if (startIndex < 0 || startIndex >= array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("startIndex");
				}
				int len = startIndex + length;
				if (length < 0 || len > array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("length");
				}
				for (int i = startIndex; i < len; i++)
				{
					array[i] = value;
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
		public static T[] Fill<T>(this T[] array, Func<T> value)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static T[] Fill<T>(this T[] array, Func<T> value, int startIndex)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static T[] Fill<T>(this T[] array, Func<T> value, int startIndex, int length)
		{
			if (array != null)
			{
				if (startIndex < 0 || startIndex >= array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("startIndex");
				}
				int len = startIndex + length;
				if (length < 0 || len > array.Length)
				{
					throw ExceptionHelper.ArgumentOutOfRange("length");
				}
				for (int i = startIndex; i < len; i++)
				{
					array[i] = value();
				}
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
		public static T[] Fill<T>(this T[] array, Func<int, T> value)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static T[] Fill<T>(this T[] array, Func<int, T> value, int startIndex)
		{
			if (array == null)
			{
				return array;
			}
			return Fill(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static T[] Fill<T>(this T[] array, Func<int, T> value, int startIndex, int length)
		{
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("startIndex");
			}
			int len = startIndex + length;
			if (length < 0 || len > array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("length");
			}
			for (int i = startIndex; i < len; i++)
			{
				array[i] = value(i);
			}
			return array;
		}

		#endregion // 一维数组

		#region 二维数组

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定函数的返回的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定函数的返回的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定函数的返回的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// 将数组使用指定函数的返回的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
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
		/// <param name="array">要进行随机排序的数组。</param>
		/// <returns>已完成随机排序的数组。</returns>
		/// <remarks>应保证每个元素出现在每个位置的概率基本相同。
		/// 采用下面的代码进行测试：
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
		/// <overloads>
		/// <summary>
		/// 将数组进行随机排序。
		/// </summary>
		/// </overloads>
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
		/// <param name="array">要进行随机排序的数组。</param>
		/// <returns>已完成随机排序的数组。</returns>
		/// <remarks>应保证每个元素出现在每个位置的概率基本相同。
		/// 采用下面的代码进行测试：
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

		#region 截取

		/// <summary>
		/// 从当前数组的左端截取一部分。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">从该数组返回其最左端截取的部分。</param>
		/// <param name="length">要截取的元素个数。
		/// 如果为 <c>0</c>，则返回空数组。如果大于或等于 <paramref name="array"/> 的长度，
		/// 则返回整个数组的一个浅拷贝。</param>
		/// <returns>从指定数组的左端截取的部分。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		public static T[] Left<T>(this T[] array, int length)
		{
			if (length < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("length");
			}
			if (array == null || length == 0)
			{
				return new T[0];
			}
			if (length > array.Length)
			{
				length = array.Length;
			}
			return SubarrayInternal(array, 0, length);
		}
		/// <summary>
		/// 从当前数组的右端截取一部分。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">从该数组返回其最右端截取的部分。</param>
		/// <param name="length">要截取的元素个数。
		/// 如果为 <c>0</c>，则返回空数组。如果大于或等于 <paramref name="array"/> 的长度，
		/// 则返回整个数组的一个浅拷贝。</param>
		/// <returns>从指定数组的右端截取的部分。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		public static T[] Right<T>(this T[] array, int length)
		{
			if (length < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("length");
			}
			if (array == null || length == 0)
			{
				return new T[0];
			}
			if (length > array.Length)
			{
				length = array.Length;
			}
			return SubarrayInternal(array, array.Length - length, length);
		}
		/// <summary>
		/// 从当前数组的指定索引开始截取一部分。
		/// 如果 <paramref name="startIndex"/> 小于 <c>0</c>，
		/// 那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="startIndex">要截取的起始索引。</param>
		/// <param name="array">要截取的数组。</param>
		/// <returns>截取得到的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 小于负的数组的长度。</exception>
		/// <overloads>
		/// <summary>
		/// 从当前数组的指定索引开始截取一部分。
		/// </summary>
		/// </overloads>
		public static T[] Subarray<T>(this T[] array, int startIndex)
		{
			if (array == null || startIndex == array.Length)
			{
				return new T[0];
			}
			return Subarray(array, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 从当前数组的指定索引开始截取指定长度的一部分。
		/// 如果 <paramref name="startIndex"/> 小于 <c>0</c>，
		/// 那么表示从字符串结束位置向前计算的位置。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要截取的数组。</param>
		/// <param name="startIndex">要截取的起始索引。</param>
		/// <param name="length">要截取的数组元素个数。</param>
		/// <returns>截取得到的数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 加 <paramref name="length"/>
		/// 之和指示的位置不在此数组中。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 小于负的此数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于零。</exception>
		public static T[] Subarray<T>(this T[] array, int startIndex, int length)
		{
			if (array == null || length == 0)
			{
				return new T[0];
			}
			if (startIndex < 0)
			{
				startIndex += array.Length;
			}
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("startIndex");
			}
			int len = startIndex + length;
			if (length < 0 || len > array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("length");
			}
			return SubarrayInternal(array, startIndex, len);
		}
		/// <summary>
		/// 从当前数组的指定索引开始截取一部分。
		/// 如果 <paramref name="startIndex"/> 小于 <c>0</c>，那么表示从数组末尾向前计算的位置。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要截取的数组。</param>
		/// <param name="startIndex">要截取的起始索引。</param>
		/// <returns>截取得到的数组。如果 <paramref name="startIndex"/> 等于数组的长度，则为空数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 指示的位置不在此数组中。</exception>
		/// <overloads>
		/// <summary>
		/// 从当前数组的指定索引开始截取一部分。
		/// </summary>
		/// </overloads>
		public static T[] Slice<T>(this T[] array, int startIndex)
		{
			if (array == null || startIndex == array.Length)
			{
				return new T[0];
			}
			if (startIndex < 0)
			{
				startIndex += array.Length;
			}
			if (startIndex < 0 || startIndex > array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("startIndex");
			}
			return SubarrayInternal(array, startIndex, array.Length);
		}
		/// <summary>
		/// 从当前数组的指定索引开始截取到指定索引结束的一部分。
		/// 如果 <paramref name="startIndex"/> 或 <paramref name="endIndex"/>
		/// 小于 <c>0</c>，那么表示从数组末尾向前计算的位置。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要截取的数组。</param>
		/// <param name="startIndex">要截取的起始索引。</param>
		/// <param name="endIndex">要截取的结束索引，但不包括该位置的元素。</param>
		/// <returns>截取得到的数组。如果 <paramref name="startIndex"/> 等于数组的长度或大于等于 
		/// <paramref name="endIndex"/> ，则为空数组。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 或 <paramref name="endIndex"/>
		/// 指示的位置不在此数组中。</exception>
		public static T[] Slice<T>(this T[] array, int startIndex, int endIndex)
		{
			if (array == null || startIndex == endIndex)
			{
				return new T[0];
			}
			if (startIndex < 0)
			{
				startIndex += array.Length;
			}
			if (endIndex < 0)
			{
				endIndex += array.Length;
			}
			if (startIndex < 0 || startIndex > array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("startIndex");
			}
			if (endIndex < 0 || endIndex > array.Length)
			{
				throw ExceptionHelper.ArgumentOutOfRange("endIndex");
			}
			if (startIndex < endIndex)
			{
				return SubarrayInternal(array, startIndex, endIndex);
			}
			else
			{
				return new T[0];
			}
		}
		/// <summary>
		/// 从当前数组的指定索引开始截取指定长度的一部分。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要截取的数组。</param>
		/// <param name="startIndex">要截取的起始索引。</param>
		/// <param name="endIndex">要截取的结束位置之后的索引。</param>
		/// <returns>截取得到的数组。</returns>
		private static T[] SubarrayInternal<T>(T[] array, int startIndex, int endIndex)
		{
			T[] re = new T[endIndex - startIndex];
			for (int idx = 0, i = startIndex; i < endIndex; idx++, i++)
			{
				re[idx] = array[i];
			}
			return re;
		}

		#endregion // 截取

		#region 二分查找

		#region Array 方法包装

		/// <summary>
		/// 使用由 <see cref="System.Array"/> 中每个元素和指定的对象实现的 
		/// <see cref="System.IComparable&lt;T&gt;"/> 泛型接口，在整个一维排序 
		/// <see cref="System.Array"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="T">数组元素的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		/// <overloads>
		/// <summary>
		/// 使用二分搜索算法在一维的排序 <see cref="System.Array"/> 中搜索特定值。
		/// </summary>
		/// </overloads>
		public static int BinarySearch<T>(this T[] array, T value)
		{
			return Array.BinarySearch(array, value);
		}
		/// <summary>
		/// 使用由 <see cref="System.Array"/> 中每个元素和指定的对象实现的 
		/// <see cref="System.IComparable"/> 接口，在整个一维排序 
		/// <see cref="System.Array"/> 中搜索特定元素。
		/// </summary>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch(this Array array, object value)
		{
			return Array.BinarySearch(array, value);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，在整个一维排序 
		/// <see cref="System.Array"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="T">数组元素的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<T>(this T[] array, T value, IComparer<T> comparer)
		{
			return Array.BinarySearch(array, value, comparer);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.IComparer"/> 接口，在整个一维排序 
		/// <see cref="System.Array"/> 中搜索特定元素。
		/// </summary>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 <see cref="System.Collections.IComparer"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch(this Array array, object value, IComparer comparer)
		{
			return Array.BinarySearch(array, value, comparer);
		}
		/// <summary>
		/// 使用由 <see cref="System.Array"/> 中每个元素和指定的对象实现的 
		/// <see cref="System.IComparable&lt;T&gt;"/> 泛型接口，在一维排序 
		/// <see cref="System.Array"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="T">数组元素的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<T>(this T[] array, int index, int length, T value)
		{
			return Array.BinarySearch(array, index, length, value);
		}
		/// <summary>
		/// 使用由 <see cref="System.Array"/> 中每个元素和指定的对象实现的 
		/// <see cref="System.IComparable"/> 接口，在一维排序 
		/// <see cref="System.Array"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch(this Array array, int index, int length, object value)
		{
			return Array.BinarySearch(array, index, length, value);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，在一维排序 
		/// <see cref="System.Array"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="T">数组元素的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<T>(this T[] array, int index, int length, T value, IComparer<T> comparer)
		{
			return Array.BinarySearch(array, index, length, value, comparer);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.IComparer"/> 接口，在一维排序 
		/// <see cref="System.Array"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的对象。</param>
		/// <param name="comparer">比较元素时要使用的 <see cref="System.Collections.IComparer"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch(this Array array, int index, int length, object value, IComparer comparer)
		{
			return Array.BinarySearch(array, index, length, value, comparer);
		}

		#endregion // Array 方法包装

		#region 键值搜索

		/// <summary>
		/// 使用由 <see cref="System.Array"/> 中每个元素和指定的对象实现的 
		/// <see cref="System.IComparable&lt;T&gt;"/> 泛型接口，在整个一维排序 
		/// <see cref="System.Array"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="TElement">数组元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this TElement[] array, TKey value,
			Func<TElement, TKey> keySelector)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			return BinarySearch(array, 0, array.Length, value, keySelector, null);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，在整个一维排序 
		/// <see cref="System.Array"/> 中搜索特定元素。
		/// </summary>
		/// <typeparam name="TElement">数组元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this TElement[] array, TKey value,
			Func<TElement, TKey> keySelector, IComparer<TKey> comparer)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			return BinarySearch(array, 0, array.Length, value, keySelector, comparer);
		}
		/// <summary>
		/// 使用由 <see cref="System.Array"/> 中每个元素和指定的对象实现的 
		/// <see cref="System.IComparable&lt;T&gt;"/> 泛型接口，在一维排序 
		/// <see cref="System.Array"/> 的某个元素范围中搜索值。
		/// </summary>
		/// <typeparam name="TElement">数组元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this TElement[] array, int index, int length,
			TKey value, Func<TElement, TKey> keySelector)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			return BinarySearch(array, index, length, value, keySelector, null);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 泛型接口，在一维排序 
		/// <see cref="System.Array"/> 的某个元素范围中搜索键值。
		/// </summary>
		/// <typeparam name="TElement">数组元素的类型。</typeparam>
		/// <typeparam name="TKey">要查找的键的类型。</typeparam>
		/// <param name="array">要搜索的从零开始的一维排序 <see cref="System.Array"/>。</param>
		/// <param name="index">要搜索的范围的起始索引。</param>
		/// <param name="length">要搜索的范围的长度。</param>
		/// <param name="value">要搜索的键值。</param>
		/// <param name="keySelector">用于从元素中提取键的函数。</param>
		/// <param name="comparer">比较元素时要使用的 
		/// <see cref="System.Collections.Generic.IComparer&lt;T&gt;"/> 实现。</param>
		/// <returns>如果找到 <paramref name="value"/>，则为指定 <paramref name="array"/> 
		/// 中的指定值的索引。如果找不到值
		/// 且值小于 <paramref name="array"/> 中的一个或多个元素，
		/// 则为一个负数，该负数是大于值的第一个元素的索引的按位求补。
		/// 如果找不到值且值大于 <paramref name="array"/> 
		/// 中的任何元素，则为一个负数，该负数是（最后一个元素的索引加 1）的按位求补。</returns>
		public static int BinarySearch<TElement, TKey>(this TElement[] array, int index, int length,
			TKey value, Func<TElement, TKey> keySelector, IComparer<TKey> comparer)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			try
			{
				if (comparer == null)
				{
					comparer = Comparer<TKey>.Default;
				}
				int low = index;
				int high = (index + length) - 1;
				while (low <= high)
				{
					int mid = low + ((high - low) >> 1);
					int cmp = comparer.Compare(keySelector(array[mid]), value);
					if (cmp == 0)
					{
						return mid;
					}
					else if (cmp < 0)
					{
						low = mid + 1;
					}
					else
					{
						high = mid - 1;
					}
				}
				return ~low;
			}
			catch (Exception ex)
			{
				throw ExceptionHelper.IComparerFailed(ex);
			}
		}

		#endregion // 键值搜索

		#endregion // 二分查找

	}
}
