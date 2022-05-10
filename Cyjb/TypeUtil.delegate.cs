using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="Type"/> 类创建委托的扩展方法。
	/// </summary>
	/// <example>
	/// 一下是一些简单的示例，很容易构造出需要的委托。
	/// <code>
	/// class Program {
	/// 	public delegate void MyDelegate(params int[] args);
	/// 	public static void TestMethod(int value) { }
	/// 	public void TestMethod(uint value) { }
	/// 	public static void TestMethod{T}(params T[] arg) { }
	/// 	static void Main(string[] args) {
	/// 		Type type = typeof(Program);
	/// 		Action&lt;int&gt; m1 = type.PowerDelegate&lt;Action&lt;int&gt;&gt;("TestMethod");
	/// 		m1(10);
	/// 		Program p = new Program();
	/// 		Action&lt;Program, uint&gt; m2 = type.PowerDelegate&lt;Action&lt;Program, uint&gt;&gt;("TestMethod");
	/// 		m2(p, 10);
	/// 		Action&lt;object, uint} m3 = type.PowerDelegate&lt;Action&lt;object, uint&gt;&gt;("TestMethod");
	/// 		m3(p, 10);
	/// 		Action&lt;uint} m4 = type.PowerDelegate&lt;Action&lt;uint&gt;&gt;("TestMethod", p);
	/// 		m4(10);
	/// 		MyDelegate m5 = type.PowerDelegate&lt;MyDelegate&gt;("TestMethod");
	/// 		m5(0, 1, 2);
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// <para><see cref="TypeUtil"/> 类提供的 <c>PowerDelegate</c> 方法，其的用法与 <c>Delegate.CreateDelegate</c> 类似，
	/// 功能却大大丰富，几乎可以只依靠委托类型、反射类型和成员名称构造出任何需要的委托。</para>
	/// <para>关于对反射创建委托的效率问题，以及该类的实现原理，可以参见我的博文 
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/DelegateBuilder.html">《C# 反射的委托创建器》</see>。</para>
	/// </remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/DelegateBuilder.html">《C# 反射的委托创建器》</seealso>
	public static partial class TypeUtil
	{
		/// <summary>
		/// 创建用于表示指定静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要创建委托的类型。</param>
		/// <param name="name">要创建委托的构造函数、方法、属性或字段成员的名称。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="flags">指示如何搜索成员的枚举值，默认会搜索全部公共成员。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例成员。如果无法绑定 <paramref name="name"/>，
		/// 则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="name"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例方法、字段或属性的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static Delegate? PowerDelegate(this Type type, string name, Type delegateType,
			BindingFlags flags = BindingFlags.Default)
		{
			ArgumentNullException.ThrowIfNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(name);
			if (type.ContainsGenericParameters)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(type));
			}
			return PowerDelegate(type, name, delegateType, flags, false, null);
		}

		/// <summary>
		/// 创建用于表示指定静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要创建委托的类型。</param>
		/// <param name="name">要创建委托的构造函数、方法、属性或字段成员的名称。</param>
		/// <param name="flags">指示如何搜索成员的枚举值，默认会搜索全部公共成员。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例成员。如果无法绑定 <paramref name="name"/>，
		/// 则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="name"/>。</exception>
		public static TDelegate? PowerDelegate<TDelegate>(this Type type, string name,
			BindingFlags flags = BindingFlags.Default)
			where TDelegate : Delegate
		{
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(name);
			if (type.ContainsGenericParameters)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(type));
			}
			return PowerDelegate(type, name, typeof(TDelegate), flags, false, null) as TDelegate;
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要创建委托的类型。</param>
		/// <param name="name">要创建委托的构造函数、方法、属性或字段成员的名称。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="firstArgument">如果是实例成员（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="flags">指示如何搜索成员的枚举值，默认会搜索全部公共成员。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例成员。如果无法绑定 <paramref name="name"/>，
		/// 则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="name"/>。</exception>
		public static Delegate? PowerDelegate(this Type type, string name, Type delegateType, object? firstArgument,
			BindingFlags flags = BindingFlags.Default)
		{
			ArgumentNullException.ThrowIfNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(name);
			if (type.ContainsGenericParameters)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(type));
			}
			return PowerDelegate(type, name, delegateType, flags, true, firstArgument);
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例成员的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="type">要创建委托的类型。</param>
		/// <param name="name">要创建委托的构造函数、方法、属性或字段成员的名称。</param>
		/// <param name="firstArgument">如果是实例成员（非构造函数），则作为委托要绑定到的对象；
		/// 否则将作为方法的第一个参数。</param>
		/// <param name="flags">指示如何搜索成员的枚举值，默认会搜索全部公共成员。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例成员。如果无法绑定 <paramref name="name"/>，
		/// 则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法（非构造函数），需要将实例对象作为委托的第一个参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="name"/>。</exception>
		public static TDelegate? PowerDelegate<TDelegate>(this Type type, string name, object? firstArgument,
			BindingFlags flags = BindingFlags.Default)
			where TDelegate : Delegate
		{
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(name);
			if (type.ContainsGenericParameters)
			{
				throw ReflectionExceptions.UnboundGenParam(nameof(type));
			}
			return PowerDelegate(type, name, typeof(TDelegate), flags, true, firstArgument) as TDelegate;
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例成员的指定类型的委托。
		/// </summary>
		/// <param name="type">要创建委托的类型。</param>
		/// <param name="name">要创建委托的构造函数、方法、属性或字段成员的名称。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="flags">指示如何搜索成员的枚举值，默认会搜索全部公共成员。</param>
		/// <param name="hasFirstArg">是否指定了首个参数的类型。</param>
		/// <param name="firstArg">首个参数的值。</param>
		/// <returns>指定类型的动态方法，表示访问指定的静态或实例方法。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例方法，需要将实例对象作为委托的第一个参数。支持参数的强制类型转换，
		/// 参数声明可以与实际类型不同。</remarks>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="name"/>。</exception>
		private static Delegate? PowerDelegate(Type type, string name, Type delegateType, BindingFlags flags,
			bool hasFirstArg, object? firstArg)
		{
			MethodInfo invoke = delegateType.GetDelegateInvoke();
			Type[] types = invoke.GetParameterTypes();
			if (hasFirstArg)
			{
				types = types.Insert(0, firstArg == null ? typeof(object) : firstArg.GetType());
			}
			MemberFinder finder = new(type, name, flags, invoke.ReturnType, types);
			// 搜索构造函数。
			Delegate? result = finder.FindConstructor()?.PowerDelegate(delegateType, hasFirstArg, firstArg);
			if (result != null)
			{
				return result;
			}
			else if (finder.IsCtor)
			{
				// 明确搜索构造函数，不需要继续搜索
				return null;
			}
			// 搜索方法。
			result = finder.FindMethod()?.PowerDelegate(delegateType, hasFirstArg, firstArg);
			if (result != null)
			{
				return result;
			}
			// 搜索属性。
			result = finder.FindProperty()?.PowerDelegate(delegateType, hasFirstArg, firstArg);
			if (result != null)
			{
				return result;
			}
			// 搜索字段。
			return finder.FindField()?.PowerDelegate(delegateType, hasFirstArg, firstArg);
		}
	}
}
