using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb.Test;

/// <summary>
/// 允许测试代码调用被测代码的方法和属性，这些方法和属性由于不是 public 的而不可访问。
/// </summary>
/// <remarks>MSTest 并未在 .NET 下提供 PrivateObject，这里提供一个单独的实现。</remarks>
/// <seealso href="https://github.com/microsoft/testfx/blob/main/src/TestFramework/Extension.Desktop/PrivateObject.cs"/>
public class PrivateObject
{
	/// <summary>
	/// 绑定标志位。
	/// </summary>
	private const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
		| BindingFlags.FlattenHierarchy;

	/// <summary>
	/// 构造函数的绑定标志位。
	/// </summary>
	private const BindingFlags ConstructorFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
		| BindingFlags.CreateInstance;

	/// <summary>
	/// 目标实例。
	/// </summary>
	private object target;
	/// <summary>
	/// 目标类型。
	/// </summary>
	private Type type;

	/// <summary>
	/// 创建指定的类型的新实例来初始化 <see cref="PrivateObject"/> 的新实例。
	/// </summary>
	/// <param name="assemblyName">程序集名称。</param>
	/// <param name="typeName">类型的完全限定名。</param>
	/// <param name="args">构造函数的参数。</param>
	public PrivateObject(string assemblyName, string typeName, params object?[] args)
		: this(Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}, {1}", typeName, assemblyName), false)!, args)
	{ }

	/// <summary>
	/// 创建指定的类型的新实例来初始化 <see cref="PrivateObject"/> 的新实例。
	/// </summary>
	/// <param name="type">要创建实例的类型。</param>
	/// <param name="args">构造函数的参数。</param>
	public PrivateObject(Type type, params object?[] args)
	{
		ArgumentNullException.ThrowIfNull(type);
		ConstructFrom(Activator.CreateInstance(type, ConstructorFlags, PowerBinder.Default, args, null)!);
	}

	/// <summary>
	/// 使用指定的对象初始化 <see cref="PrivateObject"/> 的新实例。
	/// </summary>
	/// <param name="obj">要包装的实例。</param>
	public PrivateObject(object obj)
	{
		ConstructFrom(obj);
	}

	/// <summary>
	/// 使用指定的对象初始化。
	/// </summary>
	/// <param name="obj">要包装的实例。</param>
	[MemberNotNull("target")]
	[MemberNotNull("type")]
	private void ConstructFrom(object obj)
	{
		ArgumentNullException.ThrowIfNull(obj);
		target = obj;
		type = obj.GetType();
	}

	/// <summary>
	/// 获取被包装的实例。
	/// </summary>
	public object Target => target;

	/// <summary>
	/// 获取实例的类型。
	/// </summary>
	public Type TargetType => type;

	/// <summary>
	/// 返回当前对象的哈希代码。
	/// </summary>
	/// <returns>当前对象的哈希代码。</returns>
	public override int GetHashCode()
	{
		return target.GetHashCode();
	}

	/// <summary>
	/// 确定指定对象是否等于当前对象。
	/// </summary>
	/// <param name="obj">要与当前对象进行比较的对象。</param>
	/// <returns>如果指定的对象等于当前对象，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public override bool Equals(object? obj)
	{
		if (this == obj || target == obj)
		{
			return true;
		}
		if (obj is PrivateObject pobj)
		{
			return target.Equals(pobj.target);
		}
		return false;
	}

	/// <summary>
	/// 调用方法。
	/// </summary>
	/// <param name="name">要调用的方法名称。</param>
	/// <param name="args">要传递的方法参数。</param>
	/// <returns>方法的返回值。</returns>
	public object? Invoke(string name, params object?[] args)
	{
		ArgumentNullException.ThrowIfNull(name);
		return Invoke(name, BindingFlags.InvokeMethod, args, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 获取数组的元素。
	/// </summary>
	/// <param name="name">数组的名称。</param>
	/// <param name="indices">要获取的索引。</param>
	/// <returns>指定索引的数组元素。</returns>
	public object? GetArrayElement(string name, params int[] indices)
	{
		ArgumentNullException.ThrowIfNull(name);
		Array? arr = (Array?)Invoke(name, BindingFlags.GetField | BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
		return arr?.GetValue(indices);
	}

	/// <summary>
	/// 设置数组的元素。
	/// </summary>
	/// <param name="name">数组的名称。</param>
	/// <param name="value">要设置的数组元素的值。</param>
	/// <param name="indices">要设置的索引。</param>
	public void SetArrayElement(string name, object? value, params int[] indices)
	{
		ArgumentNullException.ThrowIfNull(name);
		Array? arr = (Array?)Invoke(name, BindingFlags.GetField | BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
		arr?.SetValue(value, indices);
	}

	/// <summary>
	/// 获取字段的值。
	/// </summary>
	/// <param name="name">字段的名称。</param>
	/// <returns>字段的值。</returns>
	public object? GetField(string name)
	{
		ArgumentNullException.ThrowIfNull(name);
		return Invoke(name, BindingFlags.GetField, null, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 设置字段的值。
	/// </summary>
	/// <param name="name">字段的名称。</param>
	/// <param name="value">要设置的字段的值。</param>
	public void SetField(string name, object? value)
	{
		ArgumentNullException.ThrowIfNull(name);
		Invoke(name, BindingFlags.SetField, new[] { value }, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 获取字段或属性的值。
	/// </summary>
	/// <param name="name">字段或属性的名称。</param>
	/// <returns>字段或属性的值。</returns>
	public object? GetFieldOrProperty(string name)
	{
		ArgumentNullException.ThrowIfNull(name);
		return Invoke(name, BindingFlags.GetField | BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 设置字段或属性的值。
	/// </summary>
	/// <param name="name">字段或属性的名称。</param>
	/// <param name="value">要设置的字段或属性的值。</param>
	public void SetFieldOrProperty(string name, object value)
	{
		ArgumentNullException.ThrowIfNull(name);
		Invoke(name, BindingFlags.SetField | BindingFlags.SetProperty, new[] { value }, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 获取属性的值。
	/// </summary>
	/// <param name="name">属性的名称。</param>
	/// <returns>属性的值。</returns>
	public object? GetProperty(string name)
	{
		ArgumentNullException.ThrowIfNull(name);
		return Invoke(name, BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 设置属性的值。
	/// </summary>
	/// <param name="name">属性的名称。</param>
	/// <param name="value">要设置的属性的值。</param>
	public void SetProperty(string name, object value)
	{
		ArgumentNullException.ThrowIfNull(name);
		Invoke(name, BindingFlags.SetProperty, new[] { value }, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 调用指定的实例成员。
	/// </summary>
	/// <param name="name">要调用的实例成员名称。</param>
	/// <param name="bindingFlags">绑定标记。</param>
	/// <param name="args">调用的参数列表。</param>
	/// <param name="culture">调用使用的全球化区域设置。</param>
	/// <returns>调用结果。</returns>
	private object? Invoke(string name, BindingFlags bindingFlags, object?[]? args, CultureInfo? culture)
	{
		ArgumentNullException.ThrowIfNull(name);
		try
		{
			return type.InvokeMember(name, bindingFlags | BindFlags, PowerBinder.Default, target, args, culture);
		}
		catch (TargetInvocationException e)
		{
			if (e.InnerException != null)
			{
				throw e.InnerException;
			}
			throw;
		}
	}
}
