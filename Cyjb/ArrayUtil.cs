using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 提供数组的扩展方法。
	/// </summary>
	public static partial class ArrayUtil
	{
		/// <summary>
		/// <see cref="Array.Empty{T}"/> 方法。
		/// </summary>
		private static readonly MethodInfo ArrayEmptyMethod = typeof(Array).GetMethod("Empty")!;

		/// <summary>
		/// 返回指定类型的空数组。
		/// </summary>
		/// <param name="type">空数组的元素类型。</param>
		/// <returns>指定类型的空数组。</returns>
		public static Array Empty(Type type)
		{
			ArgumentNullException.ThrowIfNull(type);
			return (Array)ArrayEmptyMethod.MakeGenericMethod(type).Invoke(null, Array.Empty<object>())!;
		}

		/// <summary>
		/// 返回当前数组是否与指定数组包含同样内容。
		/// </summary>
		/// <typeparam name="T">数组的元素类型。</typeparam>
		/// <param name="array">要比较的当前数组。</param>
		/// <param name="other">要比较的另一数组。</param>
		/// <returns>数组内容是否一致。</returns>
		public static bool ContentEquals<T>(this T[]? array, T[]? other)
		{
			if (ReferenceEquals(array, other))
			{
				return true;
			}
			if (array == null || other == null || array.Length != other.Length)
			{
				return false;
			}
			IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < array.Length; i++)
			{
				if (!comparer.Equals(array[i], other[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 返回当前数组内容的哈希值。
		/// </summary>
		/// <typeparam name="T">数组的元素类型。</typeparam>
		/// <param name="array">要获取哈希值的数组。</param>
		/// <returns>数组内容的哈希值。</returns>
		public static int GetContentHashCode<T>(this T[]? array)
		{
			HashCode hashCode = new();
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					hashCode.Add(array[i]);
				}
			}
			return hashCode.ToHashCode();
		}

		#region 填充一维数组

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// </overloads>
		public static T[] Fill<T>(this T[] array, T value)
		{
			ArgumentNullException.ThrowIfNull(array);
			Array.Fill(array, value);
			return array;
		}

		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的值填充指定的长度。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="count">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="count"/> 小于零。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="count"/> 之和大于数组的长度。</exception>
		public static T[] Fill<T>(this T[] array, T value, int startIndex, int count)
		{
			ArgumentNullException.ThrowIfNull(array);
			Array.Fill(array, value, startIndex, count);
			return array;
		}

		/// <summary>
		/// 将数组使用指定的函数的返回值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[] Fill<T>(this T[] array, Func<int, T> value)
		{
			ArgumentNullException.ThrowIfNull(array);
			return FillInternal(array, value, 0, array.Length);
		}

		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="count">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 加 <paramref name="count"/> 之和大于数组的长度。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/>
		/// 或 <paramref name="count"/> 小于零。</exception>
		public static T[] Fill<T>(this T[] array, Func<int, T> value, int startIndex, int count)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (startIndex < 0 || startIndex > array.Length)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
			}
			else if (count < 0 || startIndex + count > array.Length)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(count);
			}
			else
			{
				return FillInternal(array, value, startIndex, count);
			}
		}

		/// <summary>
		/// 将数组从指定的索引位置开始使用指定的函数的返回值填充指定的长度。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">返回要填充数组指定索引的值的函数，其参数是当前填充到的索引。</param>
		/// <param name="startIndex">要开始填充的起始索引。</param>
		/// <param name="count">要填充的数组元素个数。</param>
		/// <returns>填充完毕的数组。</returns>
		private static T[] FillInternal<T>(this T[] array, Func<int, T> value, int startIndex, int count)
		{
			int len = startIndex + count;
			for (int i = startIndex; i < len; i++)
			{
				array[i] = value(i);
			}
			return array;
		}

		#endregion // 填充一维数组

		#region 填充二维数组

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[,] Fill<T>(this T[,] array, T value)
		{
			ArgumentNullException.ThrowIfNull(array);
			int len1 = array.GetLength(0);
			int len2 = array.GetLength(1);
			for (int i = 0; i < len1; i++)
			{
				for (int j = 0; j < len2; j++)
				{
					array[i, j] = value;
				}
			}
			return array;
		}

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[][] Fill<T>(this T[][] array, T value)
		{
			ArgumentNullException.ThrowIfNull(array);
			for (int i = 0; i < array.Length; i++)
			{
				T[] arr = array[i];
				for (int j = 0; j < arr.Length; j++)
				{
					arr[j] = value;
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[,] Fill<T>(this T[,] array, Func<int, int, T> value)
		{
			ArgumentNullException.ThrowIfNull(array);
			int len1 = array.GetLength(0);
			int len2 = array.GetLength(1);
			for (int i = 0; i < len1; i++)
			{
				for (int j = 0; j < len2; j++)
				{
					array[i, j] = value(i, j);
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[][] Fill<T>(this T[][] array, Func<int, int, T> value)
		{
			ArgumentNullException.ThrowIfNull(array);
			for (int i = 0; i < array.Length; i++)
			{
				T[] arr = array[i];
				for (int j = 0; j < arr.Length; j++)
				{
					arr[j] = value(i, j);
				}
			}
			return array;
		}

		#endregion // 填充二维数组

		#region 填充三维数组

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[,,] Fill<T>(this T[,,] array, T value)
		{
			ArgumentNullException.ThrowIfNull(array);
			int len1 = array.GetLength(0);
			int len2 = array.GetLength(1);
			int len3 = array.GetLength(2);
			for (int i = 0; i < len1; i++)
			{
				for (int j = 0; j < len2; j++)
				{
					for (int k = 0; k < len3; k++)
					{
						array[i, j, k] = value;
					}
				}
			}
			return array;
		}

		/// <summary>
		/// 将数组使用指定的值填充。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要填充的数组。</param>
		/// <param name="value">要填充数组的值。</param>
		/// <returns>填充完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[][][] Fill<T>(this T[][][] array, T value)
		{
			ArgumentNullException.ThrowIfNull(array);
			for (int i = 0; i < array.Length; i++)
			{
				T[][] arr1 = array[i];
				for (int j = 0; j < arr1.Length; j++)
				{
					T[] arr2 = arr1[j];
					for (int k = 0; k < arr2.Length; k++)
					{
						arr2[k] = value;
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[,,] Fill<T>(this T[,,] array, Func<int, int, int, T> value)
		{
			ArgumentNullException.ThrowIfNull(array);
			int len1 = array.GetLength(0);
			int len2 = array.GetLength(1);
			int len3 = array.GetLength(2);
			for (int i = 0; i < len1; i++)
			{
				ArgumentNullException.ThrowIfNull(array);
				for (int j = 0; j < len2; j++)
				{
					for (int k = 0; k < len3; k++)
					{
						array[i, j, k] = value(i, j, k);
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[][][] Fill<T>(this T[][][] array, Func<int, int, int, T> value)
		{
			ArgumentNullException.ThrowIfNull(array);
			for (int i = 0; i < array.Length; i++)
			{
				T[][] arr1 = array[i];
				for (int j = 0; j < arr1.Length; j++)
				{
					T[] arr2 = arr1[j];
					for (int k = 0; k < arr2.Length; k++)
					{
						arr2[k] = value(i, j, k);
					}
				}
			}
			return array;
		}

		#endregion // 填充三维数组

		#region 插入

		/// <summary>
		/// 向当前数组的末尾添加指定的项，并返回新数组。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">当前数组。</param>
		/// <param name="items">要添加的项。</param>
		/// <returns>数组的添加项后的结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[] Add<T>(this T[] array, params T[] items)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (items.Length == 0)
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 <c>0</c> 或大于数组的长度。</exception>
		public static T[] Insert<T>(this T[] array, int index, params T[] items)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (index < 0)
			{
				throw CommonExceptions.ArgumentNegative(index);
			}
			else if (index > array.Length)
			{
				throw CommonExceptions.ArgumentOutOfRange(index);
			}
			if (items.Length == 0)
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
		/// 将当前数组与其它数组合并。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">当前数组。</param>
		/// <param name="others">要合并的数组。</param>
		/// <returns>数组的合并结果。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[] Concat<T>(this T[] array, params T[][] others)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (others.Length == 0)
			{
				return array;
			}
			int idx = array.Length;
			int len = idx + others.Sum(arr => arr == null ? 0 : arr.Length);
			if (len == 0)
			{
				return Array.Empty<T>();
			}
			T[] result = new T[len];
			array.CopyTo(result, 0);
			for (int i = 0; i < others.Length; i++)
			{
				if (others[i] != null)
				{
					others[i].CopyTo(result, idx);
					idx += others[i].Length;
				}
			}
			return result;
		}

		/// <summary>
		/// 将当前数组更改为指定的新大小，并返回新数组。使用 <typeparamref name="T"/> 的默认值填充额外位置。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要调整大小的数组。</param>
		/// <param name="newSize">要调整到的长度，如果小于等于当前数组的长度，则忽略其余元素。</param>
		/// <returns>调整得到的新数组；如果 <paramref name="newSize"/> 小于原数组的长度，则忽略其余元素。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 将当前数组更改为指定的新大小，并返回新数组。
		/// </summary>
		/// </overloads>
		public static T[] Resize<T>(this T[] array, int newSize)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (newSize == array.Length)
			{
				return array;
			}
			T[] result = new T[newSize];
			int len = Math.Min(newSize, array.Length);
			Array.Copy(array, result, len);
			return result;
		}

		/// <summary>
		/// 将当前数组更改为指定的新大小，并返回新数组。使用 <paramref name="value"/> 填充额外位置。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要调整大小的数组。</param>
		/// <param name="newSize">要调整到的长度，如果小于等于当前数组的长度，则忽略其余元素。</param>
		/// <param name="value">要用于填充额外位置的数据。</param>
		/// <returns>调整得到的新数组；如果 <paramref name="newSize"/> 小于原数组的长度，则忽略其余元素。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		public static T[] Resize<T>(this T[] array, int newSize, T value)
		{
			ArgumentNullException.ThrowIfNull(array);
			if (newSize == array.Length)
			{
				return array;
			}
			T[] result = new T[newSize];
			int len = Math.Min(newSize, array.Length);
			Array.Copy(array, result, len);
			// 填充剩余部分。
			if (len < newSize)
			{
				Array.Fill(result, value, len, newSize - len);
			}
			return result;
		}

		#endregion // 插入

		#region 随机排序

		/// <summary>
		/// 将数组进行随机排序。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要进行随机排序的数组。</param>
		/// <returns>已完成随机排序的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <remarks>应保证每个元素出现在每个位置的概率基本相同。
		/// 采用下面的代码进行测试：
		/// <code>int size = 10;
		/// int[] arr = new int[size];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i &lt; 200; i++)
		/// {
		/// 	arr.Fill(n => n).Suffle();
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
		public static T[] Suffle<T>(this T[] array)
		{
			ArgumentNullException.ThrowIfNull(array);
			for (int i = array.Length - 1; i > 0; i--)
			{
				int j = Random.Shared.Next(i + 1);
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <remarks>应保证每个元素出现在每个位置的概率基本相同。
		/// 采用下面的代码进行测试：
		/// <code>int w = 4;
		/// int h = 3;
		/// int size = w * h;
		/// int[,] arr = new int[h, w];
		/// int[,] cnt = new int[size, size];
		/// for (int i = 0; i &lt; 320; i++)
		/// {
		/// 	arr.Fill((y, x) => y * w + x).Suffle();
		/// 	for (int j = 0; j &lt; size; j++) cnt[j, arr[j / w, j % w]]++;
		/// }
		/// for (int i = 0; i &lt; size; i++)
		/// {
		/// 	for (int j = 0; j &lt; size; j++) Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		public static T[,] Suffle<T>(this T[,] array)
		{
			ArgumentNullException.ThrowIfNull(array);
			int w = array.GetLength(1);
			int idx = array.Length;
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = w - 1; j >= 0; j--)
				{
					int r = Random.Shared.Next(idx--);
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
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
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
		/// 	arr.Fill((z, y, x) => z * w * h + y * w + x).Suffle();
		/// 	for (int j = 0; j &lt; size; j++) cnt[j, arr[j / (w * h), j / w % h, j % w]]++;
		/// }
		/// for (int i = 0; i &lt; size; i++)
		/// {
		/// 	for (int j = 0; j &lt; size; j++) Console.Write("{0} ", cnt[i, j]);
		/// 	Console.WriteLine();
		/// }</code>
		/// </remarks>
		public static T[,,] Suffle<T>(this T[,,] array)
		{
			ArgumentNullException.ThrowIfNull(array);
			int h = array.GetLength(1);
			int w = array.GetLength(2);
			int idx = array.Length;
			for (int i = array.GetLength(0) - 1; i >= 0; i--)
			{
				for (int j = h - 1; j >= 0; j--)
				{
					for (int k = w - 1; k >= 0; k--)
					{
						int r = Random.Shared.Next(idx--);
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

		#region 翻转

		/// <summary>
		/// 翻转指定的数组。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要翻转的数组。</param>
		/// <returns>翻转完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 翻转指定的数组。
		/// </summary>
		/// </overloads>
		public static T[] Reverse<T>(this T[] array)
		{
			ArgumentNullException.ThrowIfNull(array);
			Array.Reverse(array);
			return array;
		}

		/// <summary>
		/// 翻转指定数组的指定部分。
		/// </summary>
		/// <typeparam name="T">数组中元素的类型。</typeparam>
		/// <param name="array">要翻转的数组。</param>
		/// <param name="index">要翻转部分的起始索引。</param>
		/// <param name="length">要翻转部分的长度。</param>
		/// <returns>翻转完毕的数组。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为<c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>
		/// 或 <paramref name="length"/> 小于零。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>
		/// 加 <paramref name="length"/> 之和大于数组的长度。</exception>
		public static T[] Reverse<T>(this T[] array, int index, int length)
		{
			ArgumentNullException.ThrowIfNull(array);
			Array.Reverse(array, index, length);
			return array;
		}

		#endregion // 翻转

	}
}
