using System.Diagnostics.CodeAnalysis;

namespace Cyjb;

/// <summary>
/// 提供保有可选类型之一的值的能力。
/// </summary>
/// <typeparam name="T1">第一个可选类型。</typeparam>
/// <typeparam name="T2">第二个可选类型。</typeparam>
public class Variant<T1, T2>
{
	/// <summary>
	/// 值的索引。
	/// </summary>
	protected int index;
	/// <summary>
	/// 第一个类型的值。
	/// </summary>
	private T1? value1;
	/// <summary>
	/// 第二个类型的值。
	/// </summary>
	private T2? value2;

	/// <summary>
	/// 使用指定的值索引初始化 <see cref="Variant{T1, T2}"/> 结构的新实例。
	/// </summary>
	/// <param name="index">值的索引。</param>
	protected Variant(int index)
	{
		this.index = index;
	}

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T1 value)
	{
		index = 0;
		value1 = value;
	}

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T2 value)
	{
		index = 1;
		value2 = value;
	}

	/// <summary>
	/// 获取值的类型。
	/// </summary>
	public virtual Type ValueType => index switch
	{
		0 => typeof(T1),
		_ => typeof(T2),
	};

	/// <summary>
	/// 获取或设置当前实例保存的值。
	/// </summary>
	public virtual object Value
	{
		get
		{
			return index switch
			{
				0 => value1!,
				_ => value2!,
			};
		}
		set
		{
			if (value is T1 v1)
			{
				SetValue(v1);
			}
			else if (value is T2 v2)
			{
				SetValue(v2);
			}
		}
	}

	/// <summary>
	/// 设置当前对象的值。
	/// </summary>
	/// <param name="value">要设置的值。</param>
	public void SetValue(T1 value)
	{
		ResetValue(0);
		value1 = value;
	}

	/// <summary>
	/// 设置当前对象的值。
	/// </summary>
	/// <param name="value">要设置的值。</param>
	public void SetValue(T2 value)
	{
		ResetValue(1);
		value2 = value;
	}

	/// <summary>
	/// 检查当前对象的值是否是 <typeparamref name="T1"/>，如果是则返回其值。
	/// </summary>
	/// <param name="value">要获取的值。</param>
	/// <returns>如果当前对象的值是 <typeparamref name="T1"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryGetValue([NotNullWhen(true)] out T1? value)
	{
		if (index == 0)
		{
			value = value1!;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	/// <summary>
	/// 检查当前对象的值是否是 <typeparamref name="T2"/>，如果是则返回其值。
	/// </summary>
	/// <param name="value">要获取的值。</param>
	/// <returns>如果当前对象的值是 <typeparamref name="T2"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryGetValue([NotNullWhen(true)] out T2? value)
	{
		if (index == 1)
		{
			value = value2!;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	/// <summary>
	/// 重置所有值。
	/// </summary>
	/// <param name="index">要重置到的索引。</param>
	protected virtual void ResetValue(int index)
	{
		this.index = index;
		value1 = default;
		value2 = default;
	}

	/// <summary>
	/// 允许从 <typeparamref name="T1"/> 隐式转换为 <see cref="Variant{T1, T2}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static implicit operator Variant<T1, T2>(T1 value)
	{
		return new Variant<T1, T2>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2}"/> 显式转换为 <typeparamref name="T1"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static explicit operator T1(Variant<T1, T2> value)
	{
		if (value.TryGetValue(out T1? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(typeof(T2), typeof(T1));
	}

	/// <summary>
	/// 允许从 <typeparamref name="T2"/> 隐式转换为 <see cref="Variant{T1, T2}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static implicit operator Variant<T1, T2>(T2 value)
	{
		return new Variant<T1, T2>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2}"/> 显式转换为 <typeparamref name="T2"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static explicit operator T2(Variant<T1, T2> value)
	{
		if (value.TryGetValue(out T2? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(typeof(T1), typeof(T2));
	}
}
