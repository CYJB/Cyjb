using System;
using System.Globalization;
using System.Reflection;
using Cyjb;
using Cyjb.Reflection;

namespace TestCyjb
{
	/// <summary>
	/// 表示私有类的类型，该私有类提供对私有静态实现的访问。
	/// </summary>
	/// <remarks>MSTest 并未在 .NET 下提供 PrivateType，这里提供一个单独的实现。</remarks>
	internal class PrivateType
	{
		/// <summary>
		/// 绑定标志位。
		/// </summary>
		private const BindingFlags BindFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
			| BindingFlags.FlattenHierarchy;

		/// <summary>
		/// 被包装的类型。
		/// </summary>
		private readonly Type type;

		/// <summary>
		/// 创建包含指定私有类型的 <see cref="PrivateType"/> 的新实例。
		/// </summary>
		/// <param name="assemblyName">程序集名称。</param>
		/// <param name="typeName">类型的完全限定名。</param>
		public PrivateType(string assemblyName, string typeName)
		{
			CommonExceptions.CheckArgumentNull(assemblyName);
			CommonExceptions.CheckArgumentNull(typeName);
			type = Assembly.Load(assemblyName).GetType(typeName, true)!;
		}

		/// <summary>
		/// 使用指定的类初始化 <see cref="PrivateType"/> 的新实例。
		/// </summary>
		/// <param name="type">要创建包装的类型。</param>
		public PrivateType(Type? type)
		{
			this.type = type ?? throw new ArgumentNullException(nameof(type));
		}

		/// <summary>
		/// 获取被包装的类型。
		/// </summary>
		public Type ReferencedType => type;

		/// <summary>
		/// 调用静态方法。
		/// </summary>
		/// <param name="name">要调用的静态方法名称。</param>
		/// <param name="args">要传递的方法参数。</param>
		/// <returns>静态方法的返回值。</returns>
		public object? InvokeStatic(string name, params object?[] args)
		{
			return InvokeStatic(name, BindingFlags.InvokeMethod, args, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 获取静态数组的元素。
		/// </summary>
		/// <param name="name">静态数组的名称。</param>
		/// <param name="indices">要获取的索引。</param>
		/// <returns>指定索引的数组元素。</returns>
		public object? GetStaticArrayElement(string name, params int[] indices)
		{
			CommonExceptions.CheckArgumentNull(name);
			Array? arr = (Array?)InvokeStatic(name, BindingFlags.GetField | BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
			return arr?.GetValue(indices);
		}

		/// <summary>
		/// 设置静态数组的元素。
		/// </summary>
		/// <param name="name">静态数组的名称。</param>
		/// <param name="value">要设置的数组元素的值。</param>
		/// <param name="indices">要设置的索引。</param>
		public void SetStaticArrayElement(string name, object? value, params int[] indices)
		{
			CommonExceptions.CheckArgumentNull(name);
			Array? arr = (Array?)InvokeStatic(name, BindingFlags.GetField | BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
			if (arr != null)
			{
				arr.SetValue(value, indices);
			}
		}

		/// <summary>
		/// 获取静态字段的值。
		/// </summary>
		/// <param name="name">静态字段的名称。</param>
		/// <returns>静态字段的值。</returns>
		public object? GetStaticField(string name)
		{
			CommonExceptions.CheckArgumentNull(name);
			return InvokeStatic(name, BindingFlags.GetField, null, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 设置静态字段的值。
		/// </summary>
		/// <param name="name">静态字段的名称。</param>
		/// <param name="value">要设置的静态字段的值。</param>
		public void SetStaticField(string name, object? value)
		{
			CommonExceptions.CheckArgumentNull(name);
			InvokeStatic(name, BindingFlags.SetField, new[] { value }, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 获取静态字段或属性的值。
		/// </summary>
		/// <param name="name">静态字段或属性的名称。</param>
		/// <returns>静态字段或属性的值。</returns>
		public object? GetStaticFieldOrProperty(string name)
		{
			CommonExceptions.CheckArgumentNull(name);
			return InvokeStatic(name, BindingFlags.GetField | BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 设置静态字段或属性的值。
		/// </summary>
		/// <param name="name">静态字段或属性的名称。</param>
		/// <param name="value">要设置的静态字段或属性的值。</param>
		public void SetStaticFieldOrProperty(string name, object value)
		{
			CommonExceptions.CheckArgumentNull(name);
			InvokeStatic(name, BindingFlags.SetField | BindingFlags.SetProperty, new[] { value }, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 获取静态属性的值。
		/// </summary>
		/// <param name="name">静态属性的名称。</param>
		/// <returns>静态属性的值。</returns>
		public object? GetStaticProperty(string name)
		{
			CommonExceptions.CheckArgumentNull(name);
			return InvokeStatic(name, BindingFlags.GetProperty, null, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 设置静态属性的值。
		/// </summary>
		/// <param name="name">静态属性的名称。</param>
		/// <param name="value">要设置的静态属性的值。</param>
		public void SetStaticProperty(string name, object value)
		{
			CommonExceptions.CheckArgumentNull(name);
			InvokeStatic(name, BindingFlags.SetProperty, new[] { value }, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 调用指定的静态成员。
		/// </summary>
		/// <param name="name">要调用的静态成员名称。</param>
		/// <param name="bindingFlags">绑定标记。</param>
		/// <param name="args">调用的参数列表。</param>
		/// <param name="culture">调用使用的全球化区域设置。</param>
		/// <returns>调用结果。</returns>
		private object? InvokeStatic(string name, BindingFlags bindingFlags, object?[]? args, CultureInfo? culture)
		{
			CommonExceptions.CheckArgumentNull(name);
			try
			{
				return type.InvokeMember(name, bindingFlags | BindFlags, PowerBinder.Default, null, args, culture);
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
}
