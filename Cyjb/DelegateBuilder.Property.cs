using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb
{
	public static partial class DelegateBuilder
	{

		#region 构造开放属性委托

		/// <summary>
		/// 创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/> 。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			return CreateOpenDelegate(type, property, true) as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property, bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			return CreateOpenDelegate(type, property, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(type, "type");
			return CreateOpenDelegate(type, property, true);
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例属性的指定类型的委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property, bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			CommonExceptions.CheckDelegateType(type, "type");
			return CreateOpenDelegate(type, property, throwOnBindFailure);
		}
		/// <summary>
		/// 创建指定的静态或实例属性的指定类型的开放属性委托。
		/// 如果是实例属性，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">要获取或设置的属性。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns><paramref name="type"/> 类型的委托，表示静态或实例属性的委托。</returns>
		private static Delegate CreateOpenDelegate(Type type, PropertyInfo property, bool throwOnBindFailure)
		{
			Contract.Requires(type != null && property != null);
			MethodInfo invoke = type.GetInvokeMethod();
			// 判断是获取属性还是设置属性。
			MethodInfo method;
			if (invoke.ReturnType == typeof(void))
			{
				method = property.GetSetMethod(true);
				if (method == null)
				{
					if (throwOnBindFailure)
					{
						throw CommonExceptions.BindTargetPropertyNoSet("property");
					}
					return null;
				}
			}
			else
			{
				method = property.GetGetMethod(true);
				if (method == null)
				{
					if (throwOnBindFailure)
					{
						throw CommonExceptions.BindTargetPropertyNoGet("property");
					}
					return null;
				}
			}
			// 检查方法是否包含泛型参数。
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				if (throwOnBindFailure)
				{
					throw CommonExceptions.UnboundGenParam("property");
				}
				return null;
			}
			// 创建委托。
			Delegate dlg = CreateOpenDelegate(type, method);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetProperty("property");
			}
			return dlg;
		}

		#endregion // 构造开放属性委托

		#region 构造封闭属性委托

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="firstArgument">如果是实例属性，则作为委托要绑定到的对象；否则将作为属性的第一个参数。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property, object firstArgument)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			return CreateClosedDelegate(type, property, firstArgument, true) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建用于表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="firstArgument">如果是实例属性，则作为委托要绑定到的对象；否则将作为属性的第一个参数。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this PropertyInfo property, object firstArgument,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			return CreateClosedDelegate(type, property, firstArgument, throwOnBindFailure) as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数，创建用于表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="firstArgument">如果是实例属性，则作为委托要绑定到的对象；否则将作为属性的第一个参数。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property, object firstArgument)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(type, "type");
			return CreateClosedDelegate(type, property, firstArgument, true);
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建用于表示获取或设置指定的静态或实例属性的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">描述委托要表示的静态或实例属性的 <see cref="PropertyInfo"/>。</param>
		/// <param name="firstArgument">如果是实例属性，则作为委托要绑定到的对象；否则将作为属性的第一个参数。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例属性。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="property"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="property"/>。</exception>
		public static Delegate CreateDelegate(Type type, PropertyInfo property, object firstArgument,
			bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			CommonExceptions.CheckDelegateType(type, "type");
			return CreateClosedDelegate(type, property, firstArgument, throwOnBindFailure);
		}
		/// <summary>
		/// 创建指定的静态或实例属性的指定类型的封闭属性委托。
		/// 如果委托具有返回值，则认为是获取属性，否则认为是设置属性。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="type">要创建的委托的类型。</param>
		/// <param name="property">要获取或设置的属性。</param>
		/// <param name="firstArgument">如果是实例属性，则作为委托要绑定到的对象；否则将作为属性的第一个参数。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="property"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns><paramref name="type"/> 类型的委托，表示静态或实例属性的委托。</returns>
		private static Delegate CreateClosedDelegate(Type type, PropertyInfo property, object firstArgument,
			bool throwOnBindFailure)
		{
			Contract.Requires(type != null && property != null);
			MethodInfo invoke = type.GetInvokeMethod();
			// 判断是获取属性还是设置属性。
			MethodInfo method;
			if (invoke.ReturnType == typeof(void))
			{
				method = property.GetSetMethod(true);
				if (method == null)
				{
					if (throwOnBindFailure)
					{
						throw CommonExceptions.BindTargetPropertyNoSet("property");
					}
					return null;
				}
			}
			else
			{
				method = property.GetGetMethod(true);
				if (method == null)
				{
					if (throwOnBindFailure)
					{
						throw CommonExceptions.BindTargetPropertyNoGet("property");
					}
					return null;
				}
			}
			// 检查方法是否包含泛型参数。
			if (method.ContainsGenericParameters && !method.IsGenericMethodDefinition)
			{
				if (throwOnBindFailure)
				{
					throw CommonExceptions.UnboundGenParam("property");
				}
				return null;
			}
			// 创建委托。
			Delegate dlg = CreateClosedDelegate(type, method, firstArgument);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetProperty("property");
			}
			return dlg;
		}

		#endregion // 构造封闭属性委托

	}
}
