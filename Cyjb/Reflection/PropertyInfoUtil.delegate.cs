using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="PropertyInfo"/> 创建委托的扩展方法。
	/// </summary>
	public static partial class PropertyInfoUtil
	{
		/// <summary>
		/// 创建用于表示指定静态或实例属性的指定类型的委托。
		/// </summary>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例属性。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例属性的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static Delegate? PowerDelegate(this PropertyInfo property, Type delegateType)
		{
			ArgumentNullException.ThrowIfNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ReflectionExceptions.CheckUnboundGenParam(property);
			return property.PowerDelegate(delegateType, false, null);
		}

		/// <summary>
		/// 创建用于表示指定静态或实例属性的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例属性。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例属性的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static TDelegate? PowerDelegate<TDelegate>(this PropertyInfo property)
			where TDelegate : Delegate
		{
			ReflectionExceptions.CheckUnboundGenParam(property);
			Type delegateType = typeof(TDelegate);
			return property.PowerDelegate(delegateType, false, null) as TDelegate;
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例属性的指定类型的委托。
		/// </summary>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="firstArgument">如果是实例属性，则作为委托要绑定到的对象；否则将作为属性的第一个参数。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例属性。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static Delegate? PowerDelegate(this PropertyInfo property, Type delegateType, object? firstArgument)
		{
			ArgumentNullException.ThrowIfNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ReflectionExceptions.CheckUnboundGenParam(property);
			return property.PowerDelegate(delegateType, true, firstArgument);
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例属性的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="firstArgument">如果是实例属性，则作为委托要绑定到的对象；否则将作为属性的第一个参数。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例属性。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例属性的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static TDelegate? PowerDelegate<TDelegate>(this PropertyInfo property, object? firstArgument)
			where TDelegate : Delegate
		{
			ReflectionExceptions.CheckUnboundGenParam(property);
			Type delegateType = typeof(TDelegate);
			return property.PowerDelegate(delegateType, true, firstArgument) as TDelegate;
		}

		/// <summary>
		/// 创建用于表示指定静态或实例属性的指定类型的委托。
		/// </summary>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="hasFirstArg">是否指定了首个参数的类型。</param>
		/// <param name="firstArg">首个参数的值。</param>
		/// <returns>指定类型的动态属性，表示访问指定的静态或实例属性。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		internal static Delegate? PowerDelegate(this PropertyInfo property, Type delegateType, bool hasFirstArg, object? firstArg)
		{
			MethodInfo invoke = delegateType.GetDelegateInvoke();
			// 判断是获取属性还是设置属性。
			MethodInfo? method = (invoke.ReturnType == typeof(void)) ? property.GetSetMethod(true) : property.GetGetMethod(true);
			if (method == null)
			{
				return null;
			}
			// 创建委托。
			if (hasFirstArg)
			{
				return method.PowerDelegate(delegateType, firstArg);
			}
			else
			{
				return method.PowerDelegate(delegateType);
			}
		}
	}
}
