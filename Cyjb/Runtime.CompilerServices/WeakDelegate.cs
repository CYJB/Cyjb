using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Cyjb.Reflection;

namespace Cyjb.Runtime.CompilerServices
{
	/// <summary>
	/// 表示生命周期与指定对象绑定的弱引用委托。
	/// </summary>
	public static class WeakDelegate
	{
		/// <summary>
		/// 创建生命周期与委托的类实例绑定的委托。
		/// </summary>
		/// <typeparam name="TDelegate">委托的类型。</typeparam>
		/// <param name="dlg">要与类实例的生命周期绑定的委托。</param>
		/// <returns>生命周期与委托的类实例绑定的 <paramref name="dlg"/>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="dlg"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 是带有闭包的匿名委托。</exception>
		/// <remarks><para>如果 <see cref="Delegate.Target"/> 未被回收，则 <paramref name="dlg"/> 也不会被回收。
		/// 如果 <see cref="Delegate.Target"/> 已被回收，那么 <paramref name="dlg"/> 也可以被回收
		/// （如果外部没有对 <paramref name="dlg"/> 的强引用）。</para>
		/// <para>与 <see cref="ConditionalWeakTable{TKey,TValue}"/> 相同，如果外部没有对 <see cref="Delegate.Target"/>
		/// 的强引用，<paramref name="dlg"/> 中包含的对 <see cref="Delegate.Target"/> 的强引用不会影响类实例的回收。</para>
		/// <para>如果 <paramref name="dlg"/> 是带有闭包的匿名方法委托，那么 <see cref="Delegate.Target"/> 
		/// 会是一个编译器生成的嵌套类实例，无法找到生命周期应该绑定到的对象，会抛出 <see cref="ArgumentException"/> 
		/// 异常。此时请使用 <see cref="Create{TDelegate,TTarget}"/> 方法来指定生命周期绑定到的对象。</para>
		/// </remarks>
		/// <seealso cref="ConditionalWeakTable{TKey,TValue}"/>
		/// <seealso cref="DependentHandle{TKey,TValue}"/>
		public static TDelegate Create<TDelegate>(TDelegate dlg)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(dlg, "dlg");
			Contract.EndContractBlock();
			Delegate commonDlg = dlg as Delegate;
			if (commonDlg == null)
			{
				throw CommonExceptions.MustBeDelegate(typeof(TDelegate));
			}
			if (commonDlg.Target == null)
			{
				// 不包含强引用的委托。
				return dlg;
			}
			if (commonDlg.Target.GetType().IsDefined(typeof(CompilerGeneratedAttribute), false))
			{
				throw CommonExceptions.WeakDelegateForMethodWithClosure();
			}
			return WeakDelegateCreator<TDelegate>.Create(commonDlg.Target, dlg);
		}
		/// <summary>
		/// 创建生命周期与 <paramref name="target"/> 绑定的委托。
		/// </summary>
		/// <typeparam name="TDelegate">委托的类型。</typeparam>
		/// <typeparam name="TTarget">目标对象的类型。</typeparam>
		/// <param name="dlg">要绑定生命周期的委托。</param>
		/// <param name="target">生命周期绑定到的目标对象。</param>
		/// <returns>生命周期与 <paramref name="target"/> 绑定的 <paramref name="dlg"/>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="dlg"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <remarks><para>如果 <paramref name="target"/> 未被回收，则 <paramref name="dlg"/> 也不会被回收。
		/// 如果 <paramref name="target"/> 已被回收，那么 <paramref name="dlg"/> 也可以被回收
		/// （如果外部没有对 <paramref name="dlg"/> 的强引用）。</para>
		/// <para>与 <see cref="ConditionalWeakTable{TKey,TValue}"/> 相同，如果外部没有对 <paramref name="target"/> 
		/// 的强引用，即使 <paramref name="dlg"/> 中包含对 <paramref name="target"/> 的强引用，也不会影响 
		/// <paramref name="target"/> 的回收。</para></remarks>
		/// <seealso cref="ConditionalWeakTable{TKey,TValue}"/>
		/// <seealso cref="DependentHandle{TKey,TValue}"/>
		public static TDelegate Create<TDelegate, TTarget>(TDelegate dlg, TTarget target)
			where TDelegate : class
			where TTarget : class
		{
			CommonExceptions.CheckArgumentNull(dlg, "dlg");
			CommonExceptions.CheckArgumentNull(target, "target");
			Contract.EndContractBlock();
			Delegate commonDlg = dlg as Delegate;
			if (commonDlg == null)
			{
				throw CommonExceptions.MustBeDelegate(typeof(TDelegate));
			}
			if (commonDlg.Target == null)
			{
				// 不包含强引用的委托。
				return dlg;
			}
			return WeakDelegateCreator<TDelegate>.Create(target, dlg);
		}
		/// <summary>
		/// 弱引用委托的构造器。
		/// </summary>
		/// <typeparam name="TDelegate">委托的类型。</typeparam>
		private static class WeakDelegateCreator<TDelegate>
			where TDelegate : class
		{
			/// <summary>
			/// <see cref="DependentHandle{TKey,TValue}"/> 的类型。
			/// </summary>
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly Type dependentHandleType = typeof(DependentHandle<object, TDelegate>);
			/// <summary>
			/// <see cref="DependentHandle{TKey,TValue}"/> 的 <c>Value</c> 属性。
			/// </summary>
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly MethodInfo dependentHandleGetValue = dependentHandleType.GetMethod("get_Value");
			/// <summary>
			/// 构造弱引用委托的动态方法。
			/// </summary>
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private static readonly DynamicMethod invoker = MakeInvoker();
			/// <summary>
			/// 构造 <typeparamref name="TDelegate"/> 委托的弱引用。
			/// </summary>
			/// <param name="target">生命周期绑定到的目标对象。</param>
			/// <param name="dlg">要绑定生命周期的委托。</param>
			/// <returns>生命周期与 <paramref name="target"/> 绑定的 <paramref name="dlg"/>。</returns>
			public static TDelegate Create(object target, TDelegate dlg)
			{
				Contract.Requires(target != null && dlg != null);
				DependentHandle<object, TDelegate> handle = new DependentHandle<object, TDelegate>(target, dlg);
				return invoker.CreateDelegate(typeof(TDelegate), handle) as TDelegate;
			}
			/// <summary>
			/// 返回构造弱引用委托的动态方法。
			/// </summary>
			/// <returns>构造弱引用委托的动态方法。</returns>
			private static DynamicMethod MakeInvoker()
			{
				Type dlgType = typeof(TDelegate);
				MethodInfo invoke = dlgType.GetInvokeMethod();
				Type returnType = invoke.ReturnType;
				DynamicMethod method = new DynamicMethod("WeakDelegateInvoker", returnType,
					invoke.GetParameterTypes().Insert(0, dependentHandleType), true);
				ILGenerator il = method.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.EmitCall(dependentHandleGetValue);
				LocalBuilder local = il.GetLocal(typeof(TDelegate));
				il.Emit(OpCodes.Stloc, local);
				il.Emit(OpCodes.Ldloc, local);
				Label label = il.DefineLabel();
				il.Emit(OpCodes.Brfalse, label);
				il.Emit(OpCodes.Ldloc, local);
				il.EmitCall(invoke);
				if (returnType != typeof(void))
				{
					il.Emit(OpCodes.Ret);
				}
				il.MarkLabel(label);
				if (returnType != typeof(void))
				{
					il.EmitDefault(returnType);
				}
				il.Emit(OpCodes.Ret);
				return method;
			}
		}
	}
}
