using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cyjb
{
	/// <summary>
	/// 提供数组的扩展方法。
	/// </summary>
	public static partial class ArrayExt
	{

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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
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
			T[] result = new T[length];
			Array.Copy(array, result, length);
			return result;
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
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
			T[] result = new T[length];
			Array.Copy(array, array.Length - length, result, 0, length);
			return result;
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 小于负的数组的长度。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 大于数组的长度。</exception>
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
			T[] result = new T[array.Length - startIndex];
			Array.Copy(array, startIndex, result, 0, result.Length);
			return result;
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 小于负的此数组的长度。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> 小于 <c>0</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 加 <paramref name="length"/>
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
			T[] result = new T[length];
			Array.Copy(array, startIndex, result, 0, length);
			return result;
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 指示的位置不在此数组中。</exception>
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
			if (startIndex < -array.Length || startIndex > array.Length)
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
			T[] result = new T[array.Length - startIndex];
			Array.Copy(array, startIndex, result, 0, result.Length);
			return result;
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 或 <paramref name="endIndex"/>
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
			if (startIndex >= endIndex)
			{
				return Empty<T>();
			}
			T[] result = new T[endIndex - startIndex];
			Array.Copy(array, startIndex, result, 0, result.Length);
			return result;
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
		/// 向当前数组的末尾添加指定的项，并返回新数组。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">当前数组。</param>
		/// <param name="items">要添加的项。</param>
		/// <returns>数组的添加项后的结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		public static T[] Add<T>(this T[] array, params T[] items)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (items == null || items.Length == 0)
			{
				return array;
			}
			int len = array.Length + items.Length;
			T[] result = new T[len];
			array.CopyTo(result, 0);
			items.CopyTo(result, array.Length);
			return result;
		}
		/// <summary>
		/// 向当前数组的指定索引插入指定的项，并返回新数组。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">当前数组。</param>
		/// <param name="index">新项要插入的索引。</param>
		/// <param name="items">要插入的项。</param>
		/// <returns>数组插入项后的结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c> 或大于数组的长度。</exception>
		public static T[] Insert<T>(this T[] array, int index, params T[] items)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative("index", index);
			}
			if (index > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange("index", index);
			}
			Contract.Ensures(Contract.Result<T[]>() != null);
			if (items == null || items.Length == 0)
			{
				return array;
			}
			int len = array.Length + items.Length;
			T[] result = new T[len];
			if (index > 0)
			{
				Array.Copy(array, 0, result, 0, index);
			}
			items.CopyTo(result, index);
			Array.Copy(array, index, result, index + items.Length, array.Length - index);
			return result;
		}
		/// <summary>
		/// 将多个数组合并为一个数组。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="arrays">要合并的数组。</param>
		/// <returns>数组的合并结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="arrays"/> 为 <c>null</c>。</exception>
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="InvalidCastException"><typeparamref name="TInput"/> 类型不能转换到 
		/// <typeparamref name="TOutput"/> 类型。</exception>
		public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array)
		{
			if (array == null)
			{
				throw CommonExceptions.ArgumentNull("array");
			}
			Contract.EndContractBlock();
			Converter<TInput, TOutput> converter = Convert.GetConverter<TInput, TOutput>();
			if (converter == null)
			{
				throw CommonExceptions.InvalidCast(typeof(TInput), typeof(TOutput));
			}
			return Array.ConvertAll(array, converter);
		}
	}
}
