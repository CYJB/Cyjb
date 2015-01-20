using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cyjb
{
	/// <summary>
	/// 提供数组的扩展方法。
	/// </summary>
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
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
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
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<T[,]>() != null);
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
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
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
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		public static T[] Left<T>(this T[] array, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (length < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (length == 0)
			{
				return Empty<T>();
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
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		public static T[] Right<T>(this T[] array, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (length < 0)
			{
				throw CommonExceptions.ArgumentOutOfRange("length", length);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (length == 0)
			{
				return Empty<T>();
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
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 小于负的数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 大于数组的长度。</exception>
		/// <overloads>
		/// <summary>
		/// 从当前数组的指定索引开始截取一部分。
		/// </summary>
		/// </overloads>
		public static T[] Subarray<T>(this T[] array, int startIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < -array.Length || startIndex > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (startIndex == array.Length)
			{
				return Empty<T>();
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
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 小于负的此数组的长度。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 加 <paramref name="length"/>
		/// 之和指示的位置不在此数组中。</exception>
		public static T[] Subarray<T>(this T[] array, int startIndex, int length)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < -array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (length < 0 || startIndex + length > array.Length)
			{
				throw CommonExceptions.InvalidOffsetLength();
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (length == 0)
			{
				return Empty<T>();
			}
			if (startIndex < 0)
			{
				startIndex += array.Length;
			}
			return SubarrayInternal(array, startIndex, startIndex + length);
		}
		/// <summary>
		/// 从当前数组的指定索引开始截取一部分。
		/// 如果 <paramref name="startIndex"/> 小于 <c>0</c>，那么表示从数组末尾向前计算的位置。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要截取的数组。</param>
		/// <param name="startIndex">要截取的起始索引。</param>
		/// <returns>截取得到的数组。如果 <paramref name="startIndex"/> 
		/// 等于数组的长度，则为空数组。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 指示的位置不在此数组中。</exception>
		/// <overloads>
		/// <summary>
		/// 从当前数组的指定索引开始截取一部分。
		/// </summary>
		/// </overloads>
		public static T[] Slice<T>(this T[] array, int startIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < -array.Length || startIndex >= array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (startIndex == array.Length)
			{
				return Empty<T>();
			}
			if (startIndex < 0)
			{
				startIndex += array.Length;
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
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> 或 <paramref name="endIndex"/>
		/// 指示的位置不在此数组中。</exception>
		public static T[] Slice<T>(this T[] array, int startIndex, int endIndex)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (startIndex < -array.Length || startIndex > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("startIndex", startIndex);
			}
			if (endIndex < -array.Length || endIndex > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("endIndex", endIndex);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (startIndex == endIndex)
			{
				return Empty<T>();
			}
			if (startIndex < 0)
			{
				startIndex += array.Length;
			}
			if (endIndex < 0)
			{
				endIndex += array.Length;
			}
			if (startIndex < endIndex)
			{
				return SubarrayInternal(array, startIndex, endIndex);
			}
			return Empty<T>();
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
			Contract.Requires(array != null);
			Contract.Requires(startIndex >= 0 && startIndex <= array.Length);
			Contract.Requires(endIndex >= 0 && endIndex <= array.Length);
			Contract.Requires(startIndex <= endIndex);
			Contract.Ensures(Contract.Result<T[]>() != null);
			T[] re = new T[endIndex - startIndex];
			for (int idx = 0, i = startIndex; i < endIndex; idx++, i++)
			{
				re[idx] = array[i];
			}
			return re;
		}

		#endregion // 截取

		#region 零长数组

		/// <summary>
		/// 返回长度为 <c>0</c> 的数组。
		/// </summary>
		/// <typeparam name="T">数组元素的类型。</typeparam>
		/// <returns><typeparamref name="T"/> 类型的长度为 <c>0</c> 的数组。</returns>
		public static T[] Empty<T>()
		{
			Contract.Ensures(Contract.Result<T[]>() != null);
			return EmptyArray<T>.Array;
		}
		/// <summary>
		/// 保存长度为 <c>0</c> 的数组的辅助类。
		/// </summary>
		/// <typeparam name="T">数组元素的类型。</typeparam>
		private static class EmptyArray<T>
		{
			/// <summary>
			/// 长度为 <c>0</c> 的数组。
			/// </summary>
			public static readonly T[] Array = new T[0];
		}

		#endregion // 零长数组

		#region 合并

		/// <summary>
		/// 将多个数组合并为一个数组。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="arrays">要合并的数组。</param>
		/// <returns>数组的合并结果。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="arrays"/> 为 <c>null</c>。</exception>
		public static T[] Combine<T>(params T[][] arrays)
		{
			if (arrays == null)
			{
				throw CommonExceptions.ArgumentNull("arrays");
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			int len = arrays.Sum(arr => arr == null ? 0 : arr.Length);
			if (len == 0)
			{
				return Empty<T>();
			}
			T[] result = new T[len];
			int idx = 0;
			for (int i = 0; i < arrays.Length; i++)
			{
				if (arrays[i] != null)
				{
					arrays[i].CopyTo(result, idx);
					idx += arrays[i].Length;
				}
			}
			return result;
		}

		#endregion // 合并

		/// <summary>
		/// 使用默认的类型转换方法将当前数组转换为另一种类型的数组。
		/// </summary>
		/// <typeparam name="TInput">源数组元素的类型。</typeparam>
		/// <typeparam name="TOutput">目标数组元素的类型。</typeparam>
		/// <param name="array">要转换为目标类型的一维数组。</param>
		/// <returns>目标类型的数组，包含从源数组转换而来的元素。</returns>
		/// <exception cref="InvalidCastException"><typeparamref name="TInput"/> 类型不能转换到 
		/// <typeparamref name="TOutput"/> 类型。</exception>
		public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array)
		{
			Converter<TInput, TOutput> converter = Convert.GetConverter<TInput, TOutput>();
			if (converter == null)
			{
				throw CommonExceptions.InvalidCastFromTo(typeof (TInput), typeof (TOutput));
			}
			return Array.ConvertAll(array, converter);
		}
	}
}
