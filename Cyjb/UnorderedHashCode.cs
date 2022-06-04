using System.Diagnostics.CodeAnalysis;

namespace Cyjb;

/// <summary>
/// 表示无序的哈希代码。
/// </summary>
/// <remarks>
/// 与 <see cref="HashCode"/> 类似，此请勿存储序列化 <see cref="UnorderedHashCode"/> 生成的哈希代码。
/// </remarks>
[SuppressMessage("Usage", "CA2231:重写值类型的 Equals 方法时应重载相等运算符", Justification = "<挂起>")]
public struct UnorderedHashCode
{
	private uint count;
	private int sum;
	private int xor;
	private int multiply;

	/// <summary>
	/// 初始化 <see cref="UnorderedHashCode"/> 结构的新实例。
	/// </summary>
	public UnorderedHashCode()
	{
		count = 0;
		sum = 0;
		xor = 0;
		multiply = 1;
	}

	/// <summary>
	/// 将指定值添加到无序哈希代码。
	/// </summary>
	/// <typeparam name="T">要添加到无序哈希代码的值的类型。</typeparam>
	/// <param name="value">要添加到哈希代码的值。</param>
	public void Add<T>(T value)
	{
		if (value is null)
		{
			count++;
		}
		else
		{
			Add(value.GetHashCode());
		}
	}

	/// <summary>
	/// 将指定值添加到无序哈希代码，指定提供哈希代码的方法。
	/// </summary>
	/// <typeparam name="T">要添加到无序哈希代码的值的类型。</typeparam>
	/// <param name="value">要添加到哈希代码的值。</param>
	/// <param name="comparer">使用 <see cref="IEqualityComparer{T}"/> 计算哈希代码。</param>
	public void Add<T>(T value, IEqualityComparer<T>? comparer)
	{
		if (value is null)
		{
			count++;
		}
		else if (comparer is null)
		{
			Add(value.GetHashCode());
		}
		else
		{
			Add(comparer.GetHashCode(value));
		}
	}

	/// <summary>
	/// 将指定哈希代码添加到无序哈希代码。
	/// </summary>
	/// <param name="hashCode">要添加的哈希代码。</param>
	private void Add(int hashCode)
	{
		sum += hashCode;
		xor ^= hashCode;
		if (hashCode != 0)
		{
			multiply *= hashCode;
		}
		count++;
	}

	/// <summary>
	/// 返回计算得到的最终哈希代码。
	/// </summary>
	/// <returns>计算的哈希代码。</returns>
	public int ToHashCode()
	{
		return HashCode.Combine(sum, xor, multiply, count);
	}

#pragma warning disable CS0809 // 过时成员重写未过时成员

	/// <summary>
	/// 此方法不受支持，因此不应调用。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>此方法将始终引发 <see cref="NotSupportedException"/>。</returns>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	[Obsolete("UnorderedHashCode 是一个可变的结构体，不应与其它  UnorderedHashCode 比较。", true)]
	public override bool Equals(object? obj)
	{
		throw new NotSupportedException(Resources.UnorderedHashCodeEqualityNotSupported);
	}

	/// <summary>
	/// 此方法不受支持，因此不应调用。
	/// </summary>
	/// <returns>此方法将始终引发 <see cref="NotSupportedException"/>。</returns>
	/// <exception cref="NotSupportedException">总是引发。</exception>
	[Obsolete("UnorderedHashCode 是一个可变的结构体，不应与其它  UnorderedHashCode 比较。使用 ToHashCode 来获取计算的哈希代码。", true)]
	public override int GetHashCode()
	{
		throw new NotSupportedException(Resources.UnorderedHashCodeHashCodeNotSupported);
	}

#pragma warning restore CS0809 // 过时成员重写未过时成员

}
