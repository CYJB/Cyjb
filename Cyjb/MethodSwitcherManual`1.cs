using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 方法调用的切换器，对于使用同一个基类的不同子类作为参数的多个方法，
	/// 可以根据关键参数的实际类型调用相应的方法。
	/// </summary>
	/// <typeparam name="TDelegate">使用基类调用方法的委托。</typeparam>
	/// <remarks>与 <see cref="MethodSwitcher"/> 不同，
	/// <see cref="MethodSwitcherManual&lt;TDelegate&gt;"/> 需要手动添加方法委托。
	/// 关于方法切换器的更多信息，可以参加我的博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">
	/// 《C# 方法调用的切换器》</see>
	/// </remarks>
	/// <seealso cref="MethodSwitcher"/>
	/// <example>
	/// 下面演示了方法切换器的简单用法。
	/// <code>
	/// class Program {
	/// 	static void A(int m) { Console.WriteLine("int"); }
	/// 	static void B(string m) { Console.WriteLine("string"); }
	/// 	static void C(Array m) { Console.WriteLine("Array"); }
	/// 	static void D(int[] m) { Console.WriteLine("int[]"); }
	/// 	static void E(object m) { Console.WriteLine("object"); }
	/// 	static void Main(string[] args) {
	/// 		MethodSwitcherManual&lt;Action&lt;object&gt;&gt; switcher = 
	/// 			new MethodSwitcherManual&lt;Action&lt;object&gt;&gt;(0, (Action&lt;int&gt;)A, 
	/// 			(Action&lt;string&gt;)B, (Action&lt;Array&gt;)C, (Action&lt;int[]&gt;)D, (Action&lt;object&gt;)E);
	/// 		switcher.Invoke(10);
	/// 		switcher.Invoke("10");
	/// 		switcher.Invoke(new int[0]);
	/// 		switcher.Invoke(new string[0]);
	/// 		switcher.Invoke(10L);
	/// 		return;
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">
	/// 《C# 方法调用的切换器》</seealso>
	public sealed class MethodSwitcherManual<TDelegate>
		where TDelegate : class
	{
		/// <summary>
		/// 关键参数的索引。
		/// </summary>
		private int keyIndex;
		/// <summary>
		/// 方法字典。
		/// </summary>
		private Dictionary<Type, TDelegate> methodDict;
		/// <summary>
		/// 调用方法的委托。
		/// </summary>
		private TDelegate invoke;
		/// <summary>
		/// 使用指定的关键参数索引和方法委托列表初始化 
		/// <see cref="MethodSwitcherManual&lt;TDelegate&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="idx">关键参数的索引。</param>
		/// <param name="methods">使用不同子类作为参数的方法列表。</param>
		/// <exception cref="System.ArgumentException"><typeparamref name="TDelegate"/> 不继承
		/// <see cref="System.MulticastDelegate"/>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="idx"/> 
		/// 小于零或者大于等于 <typeparamref name="TDelegate"/> 的参数个数。</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="methods"/> 
		/// 中存在为 <c>null</c> 的方法。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="methods"/> 
		/// 中存在不与 <typeparamref name="TDelegate"/> 兼容的方法。</exception>
		public MethodSwitcherManual(int idx, params Delegate[] methods)
		{
			ExceptionHelper.CheckArgumentNull(methods, "methods");
			Type dlgType = typeof(TDelegate);
			ExceptionHelper.CheckDelegateType(dlgType, "TDelegate");
			ParameterInfo[] paramInfos = dlgType.GetMethod("Invoke").GetParameters();
			int len = paramInfos.Length;
			if (idx < 0 || idx >= len)
			{
				throw ExceptionHelper.ArgumentOutOfRange("idx");
			}
			keyIndex = idx;
			InitMethods(methods);
			BuildInvoke(paramInfos);
		}
		/// <summary>
		/// 获取调用方法的委托。
		/// </summary>
		/// <value>用于调用方法的委托。</value>
		public TDelegate Invoke
		{
			get { return this.invoke; }
		}
		/// <summary>
		/// 初始化方法字典。
		/// </summary>
		/// <param name="methods">方法列表。</param>
		private void InitMethods(Delegate[] methods)
		{
			methodDict = new Dictionary<Type, TDelegate>(methods.Length);
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i] == null)
				{
					throw ExceptionHelper.ArgumentNull("methods[" + i + "]");
				}
				TDelegate dlg = methods[i].Wrap<TDelegate>();
				if (dlg == null)
				{
					throw ExceptionHelper.DelegateCompatible("methods[" + i + "]", typeof(TDelegate));
				}
				methodDict.Add(methods[i].GetType().GetMethod("Invoke").GetParameters()[keyIndex].ParameterType, dlg);
			}
		}
		/// <summary>
		/// 生成 invoke 方法。
		/// </summary>
		/// <param name="paramInfos">委托的参数列表。</param>
		private void BuildInvoke(ParameterInfo[] paramInfos)
		{
			ParameterExpression[] paramList = paramInfos.ToExpressions();
			// 取得关键类型。
			Expression getType = Expression.Call(paramList[keyIndex], typeof(object).GetMethod("GetType"));
			// 从字典取得相应委托。
			Expression getDlg = Expression.Invoke(Expression.Constant((Func<Type, TDelegate>)GetMethod), getType);
			// 调用委托。
			Expression invokeDlg = Expression.Invoke(getDlg, paramList);
			this.invoke = Expression.Lambda<TDelegate>(invokeDlg, paramList).Compile();
		}
		/// <summary>
		/// 返回与指定类型相关的方法委托。
		/// </summary>
		/// <param name="type">要获取方法的类型。</param>
		/// <returns>与指定类型相关的方法委托。</returns>
		private TDelegate GetMethod(Type type)
		{
			TDelegate dlg = GetMethodUnderlying(type);
			if (dlg == null)
			{
				throw ExceptionHelper.ProcessorNotFound("type", type);
			}
			return dlg;
		}
		/// <summary>
		/// 返回与指定类型相关的方法委托。
		/// </summary>
		/// <param name="type">要获取方法的类型。</param>
		/// <returns>与指定类型相关的方法委托。</returns>
		private TDelegate GetMethodUnderlying(Type type)
		{
			TDelegate dlg;
			if (methodDict.TryGetValue(type, out dlg))
			{
				return dlg;
			}
			else if (type.BaseType == null)
			{
				return null;
			}
			else
			{
				dlg = GetMethodUnderlying(type.BaseType);
				methodDict.Add(type, dlg);
				return dlg;
			}
		}
	}
}
