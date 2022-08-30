using System.Diagnostics.CodeAnalysis;

namespace Cyjb;

/// <summary>
/// 提供保有可选类型之一的值的能力。
/// </summary>
/// <typeparam name="T1">第一个可选类型。</typeparam>
/// <typeparam name="T2">第二个可选类型。</typeparam>
/// <typeparam name="T3">第三个可选类型。</typeparam>
public sealed class Variant<T1, T2, T3>
{
	/// <summary>
	/// 值的索引。
	/// </summary>
	private int index;
	/// <summary>
	/// 第一个类型的值。
	/// </summary>
	private T1? value1;
	/// <summary>
	/// 第二个类型的值。
	/// </summary>
	private T2? value2;
	/// <summary>
	/// 第三个类型的值。
	/// </summary>
	private T3? value3;

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T1 value)
	{
		index = 0;
		value1 = value;
	}

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T2 value)
	{
		index = 1;
		value2 = value;
	}

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T3 value)
	{
		index = 2;
		value3 = value;
	}

	/// <summary>
	/// 获取值的类型。
	/// </summary>
	public Type ValueType => index switch
	{
		0 => typeof(T1),
		1 => typeof(T2),
		_ => typeof(T3),
	};

	/// <summary>
	/// 获取或设置当前实例保存的值。
	/// </summary>
	public object Value
	{
		get
		{
			return index switch
			{
				0 => value1!,
				1 => value2!,
				_ => value3!,
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
			else if (value is T3 v3)
			{
				SetValue(v3);
			}
		}
	}

	/// <summary>
	/// 设置当前对象的值。
	/// </summary>
	/// <param name="value">要设置的值。</param>
	public void SetValue(T1 value)
	{
		index = 0;
		value1 = value;
		value2 = default;
		value3 = default;
	}

	/// <summary>
	/// 设置当前对象的值。
	/// </summary>
	/// <param name="value">要设置的值。</param>
	public void SetValue(T2 value)
	{
		index = 1;
		value1 = default;
		value2 = value;
		value3 = default;
	}

	/// <summary>
	/// 设置当前对象的值。
	/// </summary>
	/// <param name="value">要设置的值。</param>
	public void SetValue(T3 value)
	{
		index = 2;
		value1 = default;
		value2 = default;
		value3 = value;
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
	/// 检查当前对象的值是否是 <typeparamref name="T3"/>，如果是则返回其值。
	/// </summary>
	/// <param name="value">要获取的值。</param>
	/// <returns>如果当前对象的值是 <typeparamref name="T3"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryGetValue([NotNullWhen(true)] out T3? value)
	{
		if (index == 2)
		{
			value = value3!;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	/// <summary>
	/// 允许从 <typeparamref name="T1"/> 隐式转换为 <see cref="Variant{T1, T2, T3}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static implicit operator Variant<T1, T2, T3>(T1 value)
	{
		return new Variant<T1, T2, T3>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2, T3}"/> 显式转换为 <typeparamref name="T1"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static explicit operator T1(Variant<T1, T2, T3> value)
	{
		if (value.index == 0)
		{
			return value.value1!;
		}
		throw CommonExceptions.InvalidCast(value.index == 1 ? typeof(T2) : typeof(T3), typeof(T1));
	}

	/// <summary>
	/// 允许从 <typeparamref name="T2"/> 隐式转换为 <see cref="Variant{T1, T2, T3}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static implicit operator Variant<T1, T2, T3>(T2 value)
	{
		return new Variant<T1, T2, T3>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2, T3}"/> 显式转换为 <typeparamref name="T2"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static explicit operator T2(Variant<T1, T2, T3> value)
	{
		if (value.index == 1)
		{
			return value.value2!;
		}
		throw CommonExceptions.InvalidCast(value.index == 0 ? typeof(T1) : typeof(T3), typeof(T2));
	}

	/// <summary>
	/// 允许从 <typeparamref name="T3"/> 隐式转换为 <see cref="Variant{T1, T2, T3}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static implicit operator Variant<T1, T2, T3>(T3 value)
	{
		return new Variant<T1, T2, T3>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2, T3}"/> 显式转换为 <typeparamref name="T3"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	public static explicit operator T3(Variant<T1, T2, T3> value)
	{
		if (value.index == 2)
		{
			return value.value3!;
		}
		throw CommonExceptions.InvalidCast(value.index == 0 ? typeof(T1) : typeof(T2), typeof(T3));
	}
}
