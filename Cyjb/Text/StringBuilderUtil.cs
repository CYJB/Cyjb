using System.Text;

namespace Cyjb.Text;

/// <summary>
/// 提供 <see cref="StringBuilder"/> 类的扩展方法。
/// </summary>
public static class StringBuilderUtil
{

	#region IndexOf

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符首次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符。</param>
	/// <returns>指定字符首次出现的索引。</returns>
	public static int IndexOf(this StringBuilder text, char value)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符从指定位置开始首次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <returns>指定字符从指定位置开始首次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 大于 
	/// <paramref name="text"/> 的长度。</exception>
	public static int IndexOf(this StringBuilder text, char value, int startIndex)
	{
		if (startIndex < 0 || startIndex > text.Length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		for (int i = startIndex; i < text.Length; i++)
		{
			if (text[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符从在指定范围内首次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <param name="count">要检查的字符长度。</param>
	/// <returns>指定字符在指定范围内首次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 大于 
	/// <paramref name="text"/> 的长度。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 加
	/// <paramref name="count"/> 大于 <paramref name="text"/> 的长度。</exception>
	public static int IndexOf(this StringBuilder text, char value, int startIndex, int count)
	{
		if (startIndex < 0 || startIndex > text.Length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		int end = startIndex + count;
		if (count < 0 || end > text.Length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		for (int i = startIndex; i < end; i++)
		{
			if (text[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符串首次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符串。</param>
	/// <returns>指定字符首次出现的索引。</returns>
	public static int IndexOf(this StringBuilder text, string value)
	{
		return IndexOf(text, value, 0, text.Length);
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符串从指定位置开始首次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符串。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <returns>指定字符从指定位置开始首次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 大于 
	/// <paramref name="text"/> 的长度。</exception>
	public static int IndexOf(this StringBuilder text, string value, int startIndex)
	{
		return IndexOf(text, value, startIndex, text.Length - startIndex);
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符串从在指定范围内首次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符串。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <param name="count">要检查的字符长度。</param>
	/// <returns>指定字符在指定范围内首次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 大于 
	/// <paramref name="text"/> 的长度。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> 加
	/// <paramref name="count"/> 大于 <paramref name="text"/> 的长度。</exception>
	public static int IndexOf(this StringBuilder text, string value, int startIndex, int count)
	{
		if (startIndex < 0 || startIndex > text.Length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		int end = startIndex + count;
		if (count < 0 || end > text.Length)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		end -= value.Length;
		if (startIndex > end)
		{
			return -1;
		}
		else if (startIndex == end)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (text[startIndex + i] != value[i])
				{
					return -1;
				}
			}
			return startIndex;
		}
		// 使用 Horspool 算法
		// 预处理
		Dictionary<char, int> distance = new();
		int valueEnd = value.Length - 1;
		for (int i = 0; i < valueEnd; i++)
		{
			distance[value[i]] = valueEnd - i;
		}
		// 查找
		for (int i = startIndex; i <= end;)
		{
			int j = valueEnd;
			for (; j >= 0 && text[i + j] == value[j]; j--) ;
			if (j < 0)
			{
				return i;
			}
			if (distance.TryGetValue(text[i + valueEnd], out int offset))
			{
				i += offset;
			}
			else
			{
				i += valueEnd + 1;
			}
		}
		return -1;
	}

	#endregion // IndexOf

	#region LastIndexOf

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符最后一次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符。</param>
	/// <returns>指定字符最后一次出现的索引。</returns>
	public static int LastIndexOf(this StringBuilder text, char value)
	{
		for (int i = text.Length - 1; i >= 0; i--)
		{
			if (text[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符从指定位置开始最后一次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <returns>指定字符从指定位置开始最后一次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="text"/> 非空且
	/// <paramref name="startIndex"/> 小于零或大于 <paramref name="text"/> 的长度；或者
	/// <paramref name="text"/> 为空且 <paramref name="startIndex"/> 小于 <c>-1</c> 或大于零。</exception>
	public static int LastIndexOf(this StringBuilder text, char value, int startIndex)
	{
		if (text.Length == 0)
		{
			if (startIndex < -1 || startIndex > 0)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
			}
			return -1;
		}
		if (startIndex < 0 || startIndex > text.Length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		if (startIndex == text.Length)
		{
			startIndex--;
		}
		for (int i = startIndex; i >= 0; i--)
		{
			if (text[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符从在指定范围内最后一次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <param name="count">要检查的字符长度。</param>
	/// <returns>指定字符在指定范围内最后一次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="text"/> 非空且
	/// <paramref name="startIndex"/> 小于零或大于等于 <paramref name="text"/> 的长度；或者
	/// <paramref name="text"/> 为空且 <paramref name="startIndex"/> 小于 <c>-1</c> 或大于零。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 大于
	/// <paramref name="startIndex"/> + 1。</exception>
	public static int LastIndexOf(this StringBuilder text, char value, int startIndex, int count)
	{
		if (text.Length == 0)
		{
			if (startIndex < -1 || startIndex > 0)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
			}
			if (count < 0 || count > startIndex + 1)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(count);
			}
			return -1;
		}
		if (startIndex < 0 || startIndex > text.Length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		int start = startIndex - count + 1;
		if (count < 0 || start < 0)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		if (startIndex == text.Length)
		{
			startIndex--;
		}
		for (int i = startIndex; i >= start; i--)
		{
			if (text[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符串最后一次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符串。</param>
	/// <returns>指定字符最后一次出现的索引。</returns>
	public static int LastIndexOf(this StringBuilder text, string value)
	{
		return LastIndexOf(text, value, text.Length - 1, text.Length);
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符串从指定位置开始最后一次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符串。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <returns>指定字符从指定位置开始最后一次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="text"/> 非空且
	/// <paramref name="startIndex"/> 小于零或大于 <paramref name="text"/> 的长度；或者
	/// <paramref name="text"/> 为空且 <paramref name="startIndex"/> 小于 <c>-1</c> 或大于零。</exception>
	public static int LastIndexOf(this StringBuilder text, string value, int startIndex)
	{
		return LastIndexOf(text, value, startIndex, startIndex + 1);
	}

	/// <summary>
	/// 返回指定 <see cref="StringBuilder"/> 中指定字符串从在指定范围内最后一次出现的索引。
	/// </summary>
	/// <param name="text">要查找的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要查找的字符串。</param>
	/// <param name="startIndex">要查找的起始位置。</param>
	/// <param name="count">要检查的字符长度。</param>
	/// <returns>指定字符在指定范围内最后一次出现的索引。</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="text"/> 非空且
	/// <paramref name="startIndex"/> 小于零或大于等于 <paramref name="text"/> 的长度；或者
	/// <paramref name="text"/> 为空且 <paramref name="startIndex"/> 小于 <c>-1</c> 或大于零。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 为负数。</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> 大于
	/// <paramref name="startIndex"/> + 1。</exception>
	public static int LastIndexOf(this StringBuilder text, string value, int startIndex, int count)
	{
		if (text.Length == 0)
		{
			if (startIndex < -1 || startIndex > 0)
			{
				throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
			}
			if (count < 0 || count > startIndex + 1)
			{
				throw CommonExceptions.ArgumentCountOutOfRange(count);
			}
			return -1;
		}
		if (startIndex < 0 || startIndex > text.Length)
		{
			throw CommonExceptions.ArgumentIndexOutOfRange(startIndex);
		}
		int start = startIndex - count + 1;
		if (count < 0 || start < 0)
		{
			throw CommonExceptions.ArgumentCountOutOfRange(count);
		}
		int end = startIndex;
		if (end == text.Length)
		{
			end--;
		}
		end -= value.Length - 1;
		if (end < start)
		{
			return -1;
		}
		else if (end == start)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (text[start + i] != value[i])
				{
					return -1;
				}
			}
			return start;
		}
		// 使用 Horspool 算法
		// 预处理
		Dictionary<char, int> distance = new();
		int valueLen = value.Length;
		for (int i = valueLen - 1; i > 0; i--)
		{
			distance[value[i]] = i;
		}
		// 查找
		for (int i = end; i >= start;)
		{
			int j = 0;
			for (; j < valueLen && text[i + j] == value[j]; j++) ;
			if (j >= valueLen)
			{
				return i;
			}
			if (distance.TryGetValue(text[i], out int offset))
			{
				i -= offset;
			}
			else
			{
				i -= valueLen;
			}
		}
		return -1;
	}

	#endregion // IndexOf

	/// <summary>
	/// 向当前 <see cref="StringBuilder"/> 中追加指定的字符串视图。
	/// </summary>
	/// <param name="text">要修改的 <see cref="StringBuilder"/> 实例。</param>
	/// <param name="value">要追加的字符串视图。</param>
	/// <returns>完成追加操作后的 <paramref name="text"/>。</returns>
	public static StringBuilder Append(this StringBuilder text, StringView value)
	{
		text.Append(value.AsSpan());
		return text;
	}
}
