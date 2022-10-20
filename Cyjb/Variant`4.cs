using System.Diagnostics.CodeAnalysis;

namespace Cyjb;

/// <summary>
/// 提供保有可选类型之一的值的能力。
/// </summary>
/// <typeparam name="T1">第一个可选类型。</typeparam>
/// <typeparam name="T2">第二个可选类型。</typeparam>
/// <typeparam name="T3">第三个可选类型。</typeparam>
/// <typeparam name="T4">第四个可选类型。</typeparam>
public class Variant<T1, T2, T3, T4> : Variant<T1, T2, T3>
{
	/// <summary>
	/// 第四个类型的值。
	/// </summary>
	private T4? value4;

	/// <summary>
	/// 使用指定的值索引初始化 <see cref="Variant{T1, T2, T3, T4}"/> 结构的新实例。
	/// </summary>
	/// <param name="index">值的索引。</param>
	protected Variant(int index) : base(index) { }

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3, T4}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T1 value) : base(value) { }

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3, T4}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T2 value) : base(value) { }

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3, T4}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T3 value) : base(value) { }

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3, T4}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T4 value) : base(3)
	{
		value4 = value;
	}

	/// <summary>
	/// 获取值的类型。
	/// </summary>
	public override Type ValueType => index switch
	{
		0 => typeof(T1),
		1 => typeof(T2),
		2 => typeof(T3),
		_ => typeof(T4),
	};

	/// <summary>
	/// 获取或设置当前实例保存的值。
	/// </summary>
	public override object Value
	{
		get
		{
			if (index == 3)
			{
				return value4!;
			}
			return base.Value;
		}
		set
		{
			if (value is T4 v4)
			{
				SetValue(v4);
			}
			else
			{
				base.Value = value;
			}
		}
	}

	/// <summary>
	/// 设置当前对象的值。
	/// </summary>
	/// <param name="value">要设置的值。</param>
	public void SetValue(T4 value)
	{
		ResetValue(3);
		value4 = value;
	}

	/// <summary>
	/// 检查当前对象的值是否是 <typeparamref name="T4"/>，如果是则返回其值。
	/// </summary>
	/// <param name="value">要获取的值。</param>
	/// <returns>如果当前对象的值是 <typeparamref name="T4"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public bool TryGetValue([NotNullWhen(true)] out T4? value)
	{
		if (index == 3)
		{
			value = value4!;
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
	protected override void ResetValue(int index)
	{
		base.ResetValue(index);
		value4 = default;
	}

	/// <summary>
	/// 允许从 <typeparamref name="T1"/> 隐式转换为 <see cref="Variant{T1, T2, T3, T4}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <see cref="Variant{T1, T2, T3, T4}"/>。</returns>
	public static implicit operator Variant<T1, T2, T3, T4>(T1 value)
	{
		return new Variant<T1, T2, T3, T4>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2, T3, T4}"/> 显式转换为 <typeparamref name="T1"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <typeparamref name="T1"/>。</returns>
	public static explicit operator T1(Variant<T1, T2, T3, T4> value)
	{
		if (value.TryGetValue(out T1? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(value.ValueType, typeof(T1));
	}

	/// <summary>
	/// 允许从 <typeparamref name="T2"/> 隐式转换为 <see cref="Variant{T1, T2, T3, T4}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <see cref="Variant{T1, T2, T3, T4}"/>。</returns>
	public static implicit operator Variant<T1, T2, T3, T4>(T2 value)
	{
		return new Variant<T1, T2, T3, T4>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2, T3, T4}"/> 显式转换为 <typeparamref name="T2"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <typeparamref name="T2"/>。</returns>
	public static explicit operator T2(Variant<T1, T2, T3, T4> value)
	{
		if (value.TryGetValue(out T2? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(value.ValueType, typeof(T2));
	}

	/// <summary>
	/// 允许从 <typeparamref name="T3"/> 隐式转换为 <see cref="Variant{T1, T2, T3, T4}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <see cref="Variant{T1, T2, T3, T4}"/>。</returns>
	public static implicit operator Variant<T1, T2, T3, T4>(T3 value)
	{
		return new Variant<T1, T2, T3, T4>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2, T3, T4}"/> 显式转换为 <typeparamref name="T3"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <typeparamref name="T3"/>。</returns>
	public static explicit operator T3(Variant<T1, T2, T3, T4> value)
	{
		if (value.TryGetValue(out T3? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(value.ValueType, typeof(T3));
	}

	/// <summary>
	/// 允许从 <typeparamref name="T4"/> 隐式转换为 <see cref="Variant{T1, T2, T3, T4}"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <see cref="Variant{T1, T2, T3, T4}"/>。</returns>
	public static implicit operator Variant<T1, T2, T3, T4>(T4 value)
	{
		return new Variant<T1, T2, T3, T4>(value);
	}

	/// <summary>
	/// 允许从 <see cref="Variant{T1, T2, T3, T4}"/> 显式转换为 <typeparamref name="T4"/>。
	/// </summary>
	/// <param name="value">要转换的值。</param>
	/// <returns>转换得到的 <typeparamref name="T4"/>。</returns>
	public static explicit operator T4(Variant<T1, T2, T3, T4> value)
	{
		if (value.TryGetValue(out T4? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(value.ValueType, typeof(T4));
	}
}
