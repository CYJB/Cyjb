using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb
{
	public static partial class DelegateBuilder
	{
		/// <summary>
		/// 构造函数的名称。
		/// </summary>
		private const string ConstructorName = ".ctor";
		/// <summary>
		/// 查找字段。
		/// </summary>
		private const BindingFlags FindFieldFlags = BindingFlags.GetField | BindingFlags.SetField;
		/// <summary>
		/// 查找属性。
		/// </summary>
		private const BindingFlags FindPropertyFlags = BindingFlags.GetProperty | BindingFlags.SetProperty;
		/// <summary>
		/// 查找属性或方法。
		/// </summary>
		private const BindingFlags FindMethodOrPropertyFlags = FindPropertyFlags | BindingFlags.InvokeMethod;
		/// <summary>
		/// 查找所有成员。
		/// </summary>
		private const BindingFlags FindAllMemberFlags = FindFieldFlags | FindMethodOrPropertyFlags |
			BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding;
		/// <summary>
		/// 查找范围。
		/// </summary>
		private const BindingFlags FindScopeFlags = BindingFlags.Static | BindingFlags.Instance;
		/// <summary>
		/// 查找访问性。
		/// </summary>
		private const BindingFlags FindAccessFlags = BindingFlags.Public | BindingFlags.NonPublic;

		#region 构造开放委托

		/// <summary>
		/// 使用指定的名称创建用于表示默认构造函数的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要创建默认构造函数委托的 <see cref="Type"/>。</param>
		/// <returns>指定类型的委托，表示指定类型的默认构造函数。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type type)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateDefaultConstructorDelegate(type, dlgType);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(".ctor");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type type, string name)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateOpenDelegate(type, name, dlgType, BindingFlags.Default);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type type, string name, BindingFlags bindingAttr)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateOpenDelegate(type, name, dlgType, bindingAttr);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type type, string name, BindingFlags bindingAttr,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.EndContractBlock();
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateOpenDelegate(type, name, dlgType, bindingAttr);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(this Type type, string name, Type delegateType)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			Delegate dlg = CreateOpenDelegate(type, name, delegateType, BindingFlags.Default);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, string name, Type delegateType, BindingFlags bindingAttr)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			Delegate dlg = CreateOpenDelegate(type, name, delegateType, bindingAttr);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, string name, Type delegateType, BindingFlags bindingAttr,
			bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.EndContractBlock();
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			Delegate dlg = CreateOpenDelegate(type, name, delegateType, bindingAttr);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		private static Delegate CreateOpenDelegate(Type type, string name, Type delegateType, BindingFlags bindingAttr)
		{
			Contract.Requires(type != null && delegateType != null && !string.IsNullOrEmpty(name));
			MethodInfo invoke = delegateType.GetInvokeMethod();
			Type[] paramTypes = invoke.GetParameterTypes();
			MemberFinder finder = new MemberFinder(type, name, FillDefaultFlags(bindingAttr), invoke.ReturnType, paramTypes);
			// 搜索构造函数。
			if (name.Equals(ConstructorName, StringComparison.Ordinal))
			{
				if (paramTypes.Length == 0)
				{
					return CreateDefaultConstructorDelegate(type, delegateType);
				}
				ConstructorInfo ctor = finder.FindConstructor();
				return ctor == null ? null : CreateOpenDelegate(ctor, delegateType);
			}
			// 查找方法。
			MethodInfo method = finder.FindMethod();
			if (method != null)
			{
				return CreateOpenDelegate(method, delegateType);
			}
			// 查找属性。
			PropertyInfo property = finder.FindProperty();
			if (property != null)
			{
				return CreateOpenDelegate(property, delegateType, false);
			}
			// 查找字段。
			FieldInfo field = finder.FindField();
			return field == null ? null : CreateOpenDelegate(field, delegateType);
		}
		/// <summary>
		/// 填充默认搜索标志。
		/// </summary>
		/// <param name="bindingAttr">要填充的搜索标志。</param>
		/// <returns>填充完毕的搜索标志。</returns>
		private static BindingFlags FillDefaultFlags(BindingFlags bindingAttr)
		{
			if ((bindingAttr & FindAllMemberFlags) == BindingFlags.Default)
			{
				bindingAttr |= FindAllMemberFlags;
			}
			if ((bindingAttr & FindAccessFlags) == BindingFlags.Default)
			{
				bindingAttr |= FindAccessFlags;
			}
			if ((bindingAttr & FindScopeFlags) == BindingFlags.Default)
			{
				bindingAttr |= FindScopeFlags;
			}
			return bindingAttr;
		}
		/// <summary>
		/// 创建表示指定类型的默认构造函数的委托。
		/// </summary>
		/// <param name="type">描述委托要创建的实例的类型。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns>表示指定类型的默认构造函数的委托。</returns>
		private static Delegate CreateDefaultConstructorDelegate(Type type, Type delegateType)
		{
			Contract.Requires(type != null && delegateType != null);
			// 检查委托类型。
			MethodInfo invoke = delegateType.GetInvokeMethod();
			if (invoke.GetParametersNoCopy().Length != 0)
			{
				return null;
			}
			Type returnType = invoke.ReturnType;
			if (!returnType.IsExplicitFrom(type))
			{
				return null;
			}
			// 构造动态委托。
			DynamicMethod dlgMethod = new DynamicMethod("DefaultConstructorDelegate", returnType, Type.EmptyTypes, true);
			ILGenerator il = dlgMethod.GetILGenerator();
			Contract.Assume(il != null);
			Converter converter = il.GetConversion(type, returnType, ConversionType.Explicit);
			il.EmitNew(type);
			converter.Emit(true);
			il.Emit(OpCodes.Ret);
			return dlgMethod.CreateDelegate(delegateType);
		}

		#endregion // 构造开放委托

		#region 延迟构造开放委托

		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的延迟初始化委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		/// <overloads>
		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// </overloads>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type type, string name)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<Lazy<TDelegate>>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			return new Lazy<TDelegate>(() =>
			{
				Delegate dlg = CreateOpenDelegate(type, name, dlgType, BindingFlags.Default);
				if (dlg == null)
				{
					throw CommonExceptions.TypeMemberNotFound("name");
				}
				return dlg as TDelegate;
			});
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的延迟初始化委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type type, string name, BindingFlags bindingAttr)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<Lazy<TDelegate>>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			return new Lazy<TDelegate>(() =>
			{
				Delegate dlg = CreateOpenDelegate(type, name, dlgType, bindingAttr);
				if (dlg == null)
				{
					throw CommonExceptions.TypeMemberNotFound("name");
				}
				return dlg as TDelegate;
			});
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的延迟初始化委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type type, string name, BindingFlags bindingAttr,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<Lazy<TDelegate>>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			return new Lazy<TDelegate>(() =>
			{
				Delegate dlg = CreateOpenDelegate(type, name, dlgType, bindingAttr);
				if (dlg == null && throwOnBindFailure)
				{
					throw CommonExceptions.TypeMemberNotFound("name");
				}
				return dlg as TDelegate;
			});
		}

		#endregion // 延迟构造开放委托

		#region 构造封闭委托

		/// <summary>
		/// 使用指定的名称和第一个参数，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type type, string name, object firstArgument)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateClosedDelegate(type, name, dlgType, firstArgument, BindingFlags.Default);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称、第一参数和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type type, string name, object firstArgument,
			BindingFlags bindingAttr)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateClosedDelegate(type, name, dlgType, firstArgument, bindingAttr);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称、第一参数、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this Type type, string name, object firstArgument,
			BindingFlags bindingAttr, bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.EndContractBlock();
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateClosedDelegate(type, name, dlgType, firstArgument, bindingAttr);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称和第一个参数，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, string name, Type delegateType, object firstArgument)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			Delegate dlg = CreateClosedDelegate(type, name, delegateType, firstArgument, BindingFlags.Default);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称、第一个参数和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, string name, Type delegateType, object firstArgument,
			BindingFlags bindingAttr)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			Delegate dlg = CreateClosedDelegate(type, name, delegateType, firstArgument, bindingAttr);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称、第一个参数、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(Type type, string name, Type delegateType, object firstArgument,
			BindingFlags bindingAttr, bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.EndContractBlock();
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			Delegate dlg = CreateClosedDelegate(type, name, delegateType, firstArgument, bindingAttr);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称、第一个参数和搜索方式，创建用于表示静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		private static Delegate CreateClosedDelegate(Type type, string name, Type delegateType, object firstArgument,
			BindingFlags bindingAttr)
		{
			Contract.Requires(type != null && delegateType != null && !string.IsNullOrEmpty(name));
			MethodInfo invoke = delegateType.GetInvokeMethod();
			Type[] paramTypes = invoke.GetParameterTypes();
			bindingAttr = FillDefaultFlags(bindingAttr);
			MemberFinder openFinder = new MemberFinder(type, name, bindingAttr, invoke.ReturnType, paramTypes);
			Type[] typesWithFirstArg = paramTypes.Insert(0, firstArgument == null ? typeof(object) : firstArgument.GetType());
			MemberFinder closedFinder = new MemberFinder(type, name, bindingAttr, invoke.ReturnType, typesWithFirstArg);
			// 搜索构造函数。
			if (name.Equals(ConstructorName, StringComparison.Ordinal))
			{
				ConstructorInfo ctor;
				if (firstArgument == null)
				{
					if (paramTypes.Length == 0)
					{
						Delegate dlg = CreateDefaultConstructorDelegate(type, delegateType);
						if (dlg != null)
						{
							return dlg;
						}
					}
					if ((ctor = openFinder.FindConstructor()) != null)
					{
						return CreateOpenDelegate(ctor, delegateType);
					}
				}
				ctor = closedFinder.FindConstructor();
				return ctor == null ? null : CreateClosedDelegate(ctor, delegateType, firstArgument, true);
			}
			// 查找方法。
			MethodInfo method;
			if (firstArgument == null && (method = openFinder.FindMethod()) != null)
			{
				return CreateOpenDelegate(method, delegateType);
			}
			if ((method = closedFinder.FindMethod()) != null)
			{
				return CreateClosedDelegate(method, delegateType, firstArgument, true);
			}
			// 查找属性。
			PropertyInfo property;
			if (firstArgument == null && (property = openFinder.FindProperty()) != null)
			{
				return CreateOpenDelegate(property, delegateType, false);
			}
			if ((property = closedFinder.FindProperty()) != null)
			{
				return CreateClosedDelegate(property, delegateType, firstArgument, false, true);
			}
			// 查找字段。
			FieldInfo field = openFinder.FindField();
			return field == null ? null : CreateClosedDelegate(field, delegateType, firstArgument);
		}

		#endregion // 构造封闭委托

		#region 延迟构造封闭委托

		/// <summary>
		/// 使用指定的名称和第一参数创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <returns>指定类型的延迟初始化委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		/// <overloads>
		/// <summary>
		/// 使用指定的名称创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// </overloads>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type type, string name, object firstArgument)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<Lazy<TDelegate>>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			return new Lazy<TDelegate>(() =>
			{
				Delegate dlg = CreateClosedDelegate(type, name, dlgType, firstArgument, BindingFlags.Default);
				if (dlg == null)
				{
					throw CommonExceptions.TypeMemberNotFound("name");
				}
				return dlg as TDelegate;
			});
		}
		/// <summary>
		/// 使用指定的名称、第一参数和搜索方式，创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的延迟初始化委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type type, string name, object firstArgument,
			BindingFlags bindingAttr)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<Lazy<TDelegate>>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			return new Lazy<TDelegate>(() =>
			{
				Delegate dlg = CreateClosedDelegate(type, name, dlgType, firstArgument, bindingAttr);
				if (dlg == null)
				{
					throw CommonExceptions.TypeMemberNotFound("name");
				}
				return dlg as TDelegate;
			});
		}
		/// <summary>
		/// 使用指定的名称、第一参数、搜索方式和针对绑定失败的指定行为，创建用于表示静态或实例成员的指定类型的延迟初始化委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="firstArgument">如果是实例方法（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的延迟初始化委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> 是一个开放泛型类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Lazy<TDelegate> CreateDelegateLazy<TDelegate>(this Type type, string name, object firstArgument,
			BindingFlags bindingAttr, bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<Lazy<TDelegate>>() != null);
			if (type.ContainsGenericParameters)
			{
				throw CommonExceptions.UnboundGenParam("type");
			}
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			return new Lazy<TDelegate>(() =>
			{
				Delegate dlg = CreateClosedDelegate(type, name, dlgType, firstArgument, bindingAttr);
				if (dlg == null && throwOnBindFailure)
				{
					throw CommonExceptions.TypeMemberNotFound("name");
				}
				return dlg as TDelegate;
			});
		}

		#endregion // 延迟构造封闭委托

		#region 从对象构造封闭委托

		/// <summary>
		/// 使用指定的名称，创建用于表示实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">要从中查找实例成员的对象。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(object target, string name)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			Delegate dlg = CreateClosedDelegate(target.GetType(), name, dlgType, target, BindingFlags.Instance);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">要从中查找实例成员的对象。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(object target, string name, BindingFlags bindingAttr)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			bindingAttr &= ~BindingFlags.Static;
			bindingAttr |= BindingFlags.Instance;
			Delegate dlg = CreateClosedDelegate(target.GetType(), name, dlgType, target, bindingAttr);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="target">要从中查找实例成员的对象。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static TDelegate CreateDelegate<TDelegate>(object target, string name, BindingFlags bindingAttr,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.EndContractBlock();
			Type dlgType = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(dlgType);
			bindingAttr &= ~BindingFlags.Static;
			bindingAttr |= BindingFlags.Instance;
			Delegate dlg = CreateClosedDelegate(target.GetType(), name, dlgType, target, bindingAttr);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的名称，创建用于表示实例成员的指定类型的委托。
		/// </summary>
		/// <param name="target">要从中查找实例成员的对象。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(object target, string name, Type delegateType)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			Delegate dlg = CreateClosedDelegate(target.GetType(), name, delegateType, target, BindingFlags.Instance);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称和搜索方式，创建用于表示实例成员的指定类型的委托。
		/// </summary>
		/// <param name="target">要从中查找实例成员的对象。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(object target, string name, Type delegateType, BindingFlags bindingAttr)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			bindingAttr &= ~BindingFlags.Static;
			bindingAttr |= BindingFlags.Instance;
			Delegate dlg = CreateClosedDelegate(target.GetType(), name, delegateType, target, bindingAttr);
			if (dlg == null)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的名称、搜索方式和针对绑定失败的指定行为，创建用于表示实例成员的指定类型的委托。
		/// </summary>
		/// <param name="target">要从中查找实例成员的对象。</param>
		/// <param name="name">委托要表示的成员的名称。</param> 
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="name"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示指定的静态或实例成员。</returns>
		/// <remarks>如果是实例成员，需要将实例对象作为委托的第一个参数。
		/// 对于属性和字段成员，如果委托具有返回值，则认为是获取属性或字段，否则认为是设置。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。按照方法、属性、字段的顺序查找匹配的成员，
		/// 也可以通过指定 <see cref="BindingFlags.InvokeMethod"/>，<see cref="BindingFlags.CreateInstance"/>，
		/// <see cref="BindingFlags.GetField"/>，<see cref="BindingFlags.SetField"/>，
		/// <see cref="BindingFlags.GetProperty"/>，<see cref="BindingFlags.SetProperty"/> 来选择要绑定到的成员类型。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">找不到名为 <paramref name="name"/> 的类型成员且 
		/// <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问成员。</exception>
		public static Delegate CreateDelegate(object target, string name, Type delegateType, BindingFlags bindingAttr,
			bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.EndContractBlock();
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			bindingAttr &= ~BindingFlags.Static;
			bindingAttr |= BindingFlags.Instance;
			Delegate dlg = CreateClosedDelegate(target.GetType(), name, delegateType, target, bindingAttr);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.TypeMemberNotFound(name);
			}
			return dlg;
		}

		#endregion // 从对象构造封闭委托

		#region 查找类型成员

		/// <summary>
		/// 寻找指定类型中指定名称的成员。
		/// </summary>
		private class MemberFinder
		{
			/// <summary>
			/// 要从中查找成员的 <see cref="Type"/>。
			/// </summary>
			private readonly Type type;
			/// <summary>
			/// 要查找的成员名称。
			/// </summary>
			private readonly string name;
			/// <summary>
			/// 搜索方式。
			/// </summary>
			private readonly BindingFlags bindingAttr;
			/// <summary>
			/// 静态成员的搜索方式。
			/// </summary>
			private readonly BindingFlags staticBindingAttr;
			/// <summary>
			/// 实例成员的搜索方式。
			/// </summary>
			private readonly BindingFlags instanceBindingAttr;
			/// <summary>
			/// 需要的返回值类型。
			/// </summary>
			private readonly Type returnType;
			/// <summary>
			/// 原始参数类型数组。
			/// </summary>
			private readonly Type[] paramTypes;
			/// <summary>
			/// 参数类型数组，用于搜索静态成员。
			/// </summary>
			private readonly Type[] staticParamTypes;
			/// <summary>
			/// 参数类型数组，用于搜索实例成员。
			/// </summary>
			private readonly Type[] instanceParamTypes;
			/// <summary>
			/// 绑定器。
			/// </summary>
			private readonly Binder binder;
			/// <summary>
			/// 使用指定的名称、搜索方式和参数类型，创建 <see cref="MemberFinder"/> 类的新实例。
			/// </summary>
			/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
			/// <param name="name">要查找的成员名称。</param> 
			/// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
			/// <param name="returnType">需要的返回值类型。</param>
			/// <param name="paramTypes">参数类型数组，用于搜索实例成员。</param>
			public MemberFinder(Type type, string name, BindingFlags bindingAttr, Type returnType, Type[] paramTypes)
			{
				Contract.Requires(type != null && name != null && returnType != null && paramTypes != null);
				this.type = type;
				this.name = name;
				this.bindingAttr = bindingAttr;
				this.returnType = returnType;
				this.paramTypes = paramTypes;
				bindingAttr &= ~FindScopeFlags;
				if (this.bindingAttr.HasFlag(BindingFlags.Static))
				{
					this.staticParamTypes = paramTypes;
					this.staticBindingAttr = bindingAttr | BindingFlags.Static;
				}
				if (this.bindingAttr.HasFlag(BindingFlags.Instance) && paramTypes.Length > 0 &&
					(paramTypes[0] == null || type.IsExplicitFrom(paramTypes[0])))
				{
					this.instanceParamTypes = paramTypes.Slice(1);
					this.instanceBindingAttr = bindingAttr | BindingFlags.Instance;
				}
				this.paramTypes = paramTypes;
				if (!this.bindingAttr.HasFlag(BindingFlags.ExactBinding))
				{
					this.binder = PowerBinder.Explicit;
				}
			}
			/// <summary>
			/// 搜索构造函数。
			/// </summary>
			/// <returns>搜索得到的构造函数，找不到则为 <c>null</c>。</returns>
			public ConstructorInfo FindConstructor()
			{
				return type.GetConstructor(bindingAttr, binder, paramTypes, null);
			}
			/// <summary>
			/// 搜索方法。
			/// </summary>
			/// <returns>搜索得到的方法，找不到则为 <c>null</c>。</returns>
			public MethodInfo FindMethod()
			{
				if (bindingAttr.HasFlag(BindingFlags.InvokeMethod))
				{
					// 查找静态方法。
					if (staticParamTypes != null)
					{
						MethodInfo method = type.GetMethod(name, staticBindingAttr, binder, staticParamTypes, null);
						if (method != null && CheckReturnType(method.ReturnType))
						{
							return method;
						}
					}
					// 查找实例方法。
					if (instanceParamTypes != null)
					{
						MethodInfo method = type.GetMethod(name, instanceBindingAttr, binder, instanceParamTypes, null);
						if (method != null && CheckReturnType(method.ReturnType))
						{
							return method;
						}
					}
				}
				return null;
			}
			/// <summary>
			/// 搜索属性。
			/// </summary>
			/// <returns>搜索得到的属性，找不到则为 <c>null</c>。</returns>
			public PropertyInfo FindProperty()
			{
				if ((bindingAttr & FindPropertyFlags) != BindingFlags.Default)
				{
					// 查找静态属性。
					if (staticParamTypes != null)
					{
						PropertyInfo property = GetProperty(staticBindingAttr, staticParamTypes);
						if (property != null)
						{
							return property;
						}
					}
					// 查找实例属性。
					if (instanceParamTypes != null)
					{
						PropertyInfo property = GetProperty(instanceBindingAttr, instanceParamTypes);
						if (property != null)
						{
							return property;
						}
					}
				}
				return null;
			}
			/// <summary>
			/// 搜索字段，只会根据名称搜索。
			/// </summary>
			/// <returns>搜索得到的字段，找不到则为 <c>null</c>。</returns>
			public FieldInfo FindField()
			{
				if ((bindingAttr & FindFieldFlags) != BindingFlags.Default)
				{
					FieldInfo field = type.GetField(name, bindingAttr);
					if (field != null)
					{
						return field;
					}
				}
				return null;
			}
			/// <summary>
			/// 检查指定的类型是否与返回类型兼容。
			/// </summary>
			/// <param name="checkType">要检查的类型。</param>
			/// <returns>如果与返回类型兼容，则为 <c>true</c>；否则为 <c>false</c>。</returns>
			private bool CheckReturnType(Type checkType)
			{
				Contract.Requires(checkType != null);
				if (returnType == typeof(void))
				{
					return true;
				}
				if (checkType == typeof(void))
				{
					return false;
				}
				return returnType.IsExplicitFrom(checkType);
			}
			/// <summary>
			/// 搜索与指定参数类型匹配的属性。
			/// </summary>
			/// <param name="propertyBindingAttr">搜索方式。</param>
			/// <param name="types">属性的索引参数。</param>
			/// <returns>指定类型的属性，如果无法找到则为 <c>null</c>。</returns>
			private PropertyInfo GetProperty(BindingFlags propertyBindingAttr, Type[] types)
			{
				if (returnType == typeof(void))
				{
					// 是设置属性，将最后一个参数作为属性类型。
					return type.GetProperty(name, propertyBindingAttr, binder, types[types.Length - 1], types.Slice(0, -1), null);
				}
				// 是获取属性。
				return type.GetProperty(name, propertyBindingAttr, binder, returnType, types, null);
			}
		}

		#endregion 查找类型成员

	}
}
