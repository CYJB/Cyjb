using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Cyjb
{
	public static partial class ArrayExt
	{

		#region Array

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// </overloads>
		public static Array Fill(this Array array, object value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static Array Fill(this Array array, object value, int startIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static Array Fill(this Array array, object value, int startIndex, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (length < 0 || startIndex + length > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, startIndex, length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		private static Array FillInternal(this Array array, object value, int startIndex, int length)
		{
			Contract.Requires(array != null);
			Contract.Requires(startIndex >= array.GetLowerBound(0));
			Contract.Requires(length >= 0);
			Contract.Requires(startIndex + length <= array.Length);
			Contract.Ensures(Contract.Result<Array>() != null);
			for (int i = startIndex + length - 1; i >= startIndex; i--)
			{
				array.SetValue(value, i);
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static Array Fill(this Array array, Func<object> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static Array Fill(this Array array, Func<object> value, int startIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static Array Fill(this Array array, Func<object> value, int startIndex, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (length < 0 || startIndex + length > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, startIndex, length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		private static Array FillInternal(this Array array, Func<object> value, int startIndex, int length)
		{
			Contract.Requires(array != null);
			Contract.Requires(value != null);
			Contract.Requires(startIndex >= 0 && startIndex + length <= array.Length);
			Contract.Ensures(Contract.Result<Array>() != null);
			int len = startIndex + length;
			for (int i = startIndex; i < len; i++)
			{
				array.SetValue(value(), i);
			}
			return array;
		}
		/// <summary>
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static Array Fill(this Array array, Func<int, object> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static Array Fill(this Array array, Func<int, object> value, int startIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, startIndex, array.Length - startIndex);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static Array Fill(this Array array, Func<int, object> value, int startIndex, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (length < 0 || startIndex + length > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<Array>() != null);
			return FillInternal(array, value, startIndex, length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="length">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		private static Array FillInternal(this Array array, Func<int, object> value, int startIndex, int length)
		{
			Contract.Requires(array != null);
			Contract.Requires(value != null);
			Contract.Requires(startIndex >= 0 && startIndex + length <= array.Length);
			Contract.Ensures(Contract.Result<Array>() != null);
			int len = startIndex + length;
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		public static T[] Fill<T>(this T[] array, T value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static T[] Fill<T>(this T[] array, T value, int startIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, startIndex, array.Length - startIndex);
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static T[] Fill<T>(this T[] array, T value, int startIndex, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (length < 0 || startIndex + length > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, startIndex, array.Length - startIndex);
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
		private static T[] FillInternal<T>(this T[] array, T value, int startIndex, int length)
		{
			Contract.Requires(array != null);
			Contract.Requires(startIndex >= 0 && startIndex + length <= array.Length);
			Contract.Ensures(Contract.Result<T[]>() != null);
			for (int i = startIndex + length - 1; i >= startIndex; i--)
			{
				array[i] = value;
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static T[] Fill<T>(this T[] array, Func<T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, 0, array.Length);
		}
		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 小于零或大于等于数组的长度。</exception>
		public static T[] Fill<T>(this T[] array, Func<T> value, int startIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, startIndex, array.Length - startIndex);
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static T[] Fill<T>(this T[] array, Func<T> value, int startIndex, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (length < 0 || startIndex + length > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, startIndex, length);
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
		private static T[] FillInternal<T>(this T[] array, Func<T> value, int startIndex, int length)
		{
			Contract.Requires(array != null);
			Contract.Requires(value != null);
			Contract.Requires(startIndex >= 0 && startIndex + length <= array.Length);
			Contract.Ensures(Contract.Result<T[]>() != null);
			int len = startIndex + length;
			for (int i = startIndex; i < len; i++)
			{
				array[i] = value();
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static T[] Fill<T>(this T[] array, Func<int, T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, 0, array.Length);
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
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0 || startIndex >= array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, startIndex, array.Length - startIndex);
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		public static T[] Fill<T>(this T[] array, Func<int, T> value, int startIndex, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			if (startIndex < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (length < 0 || startIndex + length > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			return FillInternal(array, value, startIndex, length);
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
		private static T[] FillInternal<T>(this T[] array, Func<int, T> value, int startIndex, int length)
		{
			Contract.Requires(array != null);
			Contract.Requires(value != null);
			Contract.Requires(startIndex >= 0 && startIndex + length <= array.Length);
			Contract.Ensures(Contract.Result<T[]>() != null);
			int len = startIndex + length;
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[,] Fill<T>(this T[,] array, T value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<T[,]>() != null);
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = array.GetLength(1) - 1; j >= 0; j--)
				{
					array[i, j] = value;
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[,] Fill<T>(this T[,] array, Func<T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[,]>() != null);
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					array[i, j] = value();
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[,] Fill<T>(this T[,] array, Func<int, int, T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[,]>() != null);
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					array[i, j] = value(i, j);
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		public static T[][] Fill<T>(this T[][] array, T value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<T[][]>() != null);
			for (int i = array.Length - 1; i >= 0; i--)
			{
				for (int j = array[i].Length - 1; j >= 0; j--)
				{
					array[i][j] = value;
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static T[][] Fill<T>(this T[][] array, Func<T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[][]>() != null);
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[i].Length; j++)
				{
					array[i][j] = value();
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static T[][] Fill<T>(this T[][] array, Func<int, int, T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[][]>() != null);
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[i].Length; j++)
				{
					array[i][j] = value(i, j);
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Fill<T>(this T[, ,] array, T value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<T[, ,]>() != null);
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
			return array;
		}
		/// <summary>
		/// 将数组使用指定函数的返回的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组的值的函数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Fill<T>(this T[, ,] array, Func<T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[, ,]>() != null);
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
			return array;
		}
		/// <summary>
		/// 将数组使用指定函数的返回的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		public static T[, ,] Fill<T>(this T[, ,] array, Func<int, int, int, T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[, ,]>() != null);
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		public static T[][][] Fill<T>(this T[][][] array, T value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<T[][][]>() != null);
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = array[i].Length - 1; j >= 0; j--)
				{
					for (int k = array[i][j].Length - 1; k >= 0; k--)
					{
						array[i][j][k] = value;
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static T[][][] Fill<T>(this T[][][] array, Func<T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[][][]>() != null);
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[i].Length; j++)
				{
					for (int k = 0; k < array[i][j].Length; k++)
					{
						array[i][j][k] = value();
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
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static T[][][] Fill<T>(this T[][][] array, Func<int, int, int, T> value)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<T[][][]>() != null);
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[i].Length; j++)
				{
					for (int k = 0; k < array[i][j].Length; k++)
					{
						array[i][j][k] = value(i, j, k);
					}
				}
			}
			return array;
		}

		#endregion // 三维交错数组

	}
}
