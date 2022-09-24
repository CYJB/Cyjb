using System.Diagnostics.CodeAnalysis;

namespace Cyjb;

/// <summary>
/// 提供保有可选类型之一的值的能力。
/// </summary>
/// <typeparam name="T1">第一个可选类型。</typeparam>
/// <typeparam name="T2">第二个可选类型。</typeparam>
/// <typeparam name="T3">第三个可选类型。</typeparam>
public class Variant<T1, T2, T3> : Variant<T1, T2>
{
	/// <summary>
	/// 第三个类型的值。
	/// </summary>
	private T3? value3;

	/// <summary>
	/// 使用指定的值索引初始化 <see cref="Variant{T1, T2, T3}"/> 结构的新实例。
	/// </summary>
	/// <param name="index">值的索引。</param>
	protected Variant(int index) : base(index) { }

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T1 value) : base(value) { }

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T2 value) : base(value) { }

	/// <summary>
	/// 使用指定的值初始化 <see cref="Variant{T1, T2, T3}"/> 结构的新实例。
	/// </summary>
	/// <param name="value">初始化的值。</param>
	public Variant(T3 value) : base(2) { value3 = value; }

	/// <summary>
	/// 获取值的类型。
	/// </summary>
	public override Type ValueType => index switch
	{
		0 => typeof(T1),
		1 => typeof(T2),
		_ => typeof(T3),
	};

	/// <summary>
	/// 获取或设置当前实例保存的值。
	/// </summary>
	public override object Value
	{
		get
		{
			if (index == 2)
			{
				return value3!;
			}
			else
			{
				return base.Value;
			}
		}
		set
		{
			if (value is T3 v)
			{
				SetValue(v);
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
	public void SetValue(T3 value)
	{
		ResetValue(2);
		value3 = value;
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
	/// 重置所有值。
	/// </summary>
	/// <param name="index">要重置到的索引。</param>
	protected override void ResetValue(int index)
	{
		base.ResetValue(index);
		value3 = default;
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
		if (value.TryGetValue(out T1? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(value.ValueType, typeof(T1));
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
		if (value.TryGetValue(out T2? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(value.ValueType, typeof(T2));
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
		if (value.TryGetValue(out T3? result))
		{
			return result;
		}
		throw CommonExceptions.InvalidCast(value.ValueType, typeof(T3));
	}
}
