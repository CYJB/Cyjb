using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 方法调用的切换器，对于使用同一个基类的不同子类作为参数的多个方法，
	/// 可以根据关键参数的实际类型调用相应的方法。
	/// </summary>
	/// <remarks><see cref="MethodSwitcher"/> 可以自动从类型中寻找方法处理器，
	/// 无需手动配置。
	/// 关于方法切换器的更多信息，可以参加我的博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">
	/// 《C# 方法调用的切换器》</see>
	/// </remarks>
	/// <seealso cref="MethodSwitcherManual&lt;TDelegate&gt;"/>
	/// <seealso cref="ProcessorAttribute"/>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">
	/// 《C# 方法调用的切换器》</seealso>
	/// <example>
	/// 下面演示了方法切换器的简单用法。
	/// <code>
	/// class Program {
	///		[Processor]
	/// 	static void A(int m) { Console.WriteLine("int"); }
	///		[Processor]
	/// 	static void B(string m) { Console.WriteLine("string"); }
	///		[Processor]
	/// 	static void C(Array m) { Console.WriteLine("Array"); }
	///		[Processor]
	/// 	static void D(int[] m) { Console.WriteLine("int[]"); }
	///		[Processor]
	/// 	static void E(object m) { Console.WriteLine("object"); }
	/// 	static void Main(string[] args) {
	/// 		Action&lt;object&gt; invoke = MethodSwitcher.GetSwitcher&lt;Action&lt;object&gt;&gt;(typeof(Program), 0);
	/// 		invoke(10);
	/// 		invoke("10");
	/// 		invoke(new int[0]);
	/// 		invoke(new string[0]);
	/// 		invoke(10L);
	/// 		return;
	/// 	}
	/// }
	/// </code>
	/// </example>
	public static class MethodSwitcher
	{
		/// <summary>
		/// 扫描所有公共和非公共的实例或静态方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const BindingFlags MethodFlags = BindingFlags.Static | BindingFlags.Instance |
			BindingFlags.Public | BindingFlags.NonPublic;
		/// <summary>
		/// 特定类型中的所有标记为处理器的方法，及其元数据。
		/// </summary>
		private static Dictionary<string, Tuple<bool, Type, Dictionary<Type, Delegate>>> methodDict =
			new Dictionary<string, Tuple<bool, Type, Dictionary<Type, Delegate>>>();

		#region 返回方法切换器

		/// <summary>
		/// 返回 <paramref name="type"/> 中与默认标识符相关的静态方法切换器。
		/// 多次获取特定类型中同一标识符的方法切换器，必须使用相同的 
		/// <typeparamref name="TDelegate"/> 和 <paramref name="index"/>。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="type">在其中查找静态方法的类型。</param>
		/// <param name="index">方法的关键参数索引。</param>
		/// <returns><paramref name="type"/> 中与默认标识符相关的静态方法切换器。</returns>
		/// <overloads>
		/// <summary>
		/// 返回特定类型或对象中与特定标识符相关的静态或实例方法切换器。
		/// 多次获取特定类型中同一标识符的方法切换器，必须使用相同的 
		/// <typeparamref name="TDelegate"/> 和 <paramref name="index"/>。
		/// </summary>
		/// </overloads>
		public static TDelegate GetSwitcher<TDelegate>(Type type, int index)
			where TDelegate : class
		{
			return GetSwitcher<TDelegate>(type, index, ProcessorAttribute.DefaultId);
		}
		/// <summary>
		/// 返回 <paramref name="type"/> 中与指定标识符相关的静态方法切换器。
		/// 多次获取特定类型中同一标识符的方法切换器，必须使用相同的 
		/// <typeparamref name="TDelegate"/> 和 <paramref name="index"/>。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="type">在其中查找静态方法的类型。</param>
		/// <param name="index">方法的关键参数索引。</param>
		/// <param name="id">方法切换器的标识符。</param>
		/// <returns><paramref name="type"/> 中与指定标识符相关的静态方法切换器。</returns>
		public static TDelegate GetSwitcher<TDelegate>(Type type, int index, string id)
			where TDelegate : class
		{
			ExceptionHelper.CheckArgumentNull(type, "type");
			Type dlgType = typeof(TDelegate);
			ExceptionHelper.CheckDelegateType(dlgType, "TDelegate");
			Dictionary<Type, Delegate> methods = GetMethods<TDelegate>(type, id, index, true);
			MethodInfo invoke = dlgType.GetMethod("Invoke");
			// 构造委托。
			ParameterExpression[] paramList = invoke.GetParameters().ToExpressions();
			// 取得关键类型。
			Expression getType = Expression.Call(paramList[index], typeof(object).GetMethod("GetType"));
			// 从字典取得相应委托。
			Expression getDlg = Expression.Invoke(
				Expression.Constant((Func<Dictionary<Type, Delegate>, Type, Delegate>)GetMethod),
				Expression.Constant(methods), getType);
			getDlg = Expression.Convert(getDlg, dlgType);
			// 调用委托。
			Expression invokeDlg = Expression.Invoke(getDlg, paramList);
			return Expression.Lambda<TDelegate>(invokeDlg, paramList).Compile() as TDelegate;
		}
		/// <summary>
		/// 返回指定对象中与默认标识符相关的实例方法切换器。
		/// 多次获取特定类型中同一标识符的方法切换器，必须使用相同的 
		/// <typeparamref name="TDelegate"/> 和 <paramref name="index"/>。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="target">实例方法的目标对象。</param>
		/// <param name="index">方法的关键参数索引。</param>
		/// <returns>指定对象中与默认标识符相关的实例方法切换器。</returns>
		public static TDelegate GetSwitcher<TDelegate>(object target, int index)
			where TDelegate : class
		{
			return GetSwitcher<TDelegate>(target, index, ProcessorAttribute.DefaultId);
		}
		/// <summary>
		/// 返回指定对象中与指定标识符相关的实例方法切换器。
		/// 多次获取特定类型中同一标识符的方法切换器，必须使用相同的 
		/// <typeparamref name="TDelegate"/> 和 <paramref name="index"/>。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="target">实例方法的目标对象。</param>
		/// <param name="index">方法的关键参数索引。</param>
		/// <param name="id">方法切换器的标识符。</param>
		/// <returns>指定对象中与指定标识符相关的实例方法切换器。</returns>
		public static TDelegate GetSwitcher<TDelegate>(object target, int index, string id)
			where TDelegate : class
		{
			ExceptionHelper.CheckArgumentNull(target, "target");
			Type dlgType = typeof(TDelegate);
			ExceptionHelper.CheckDelegateType(dlgType, "TDelegate");
			Dictionary<Type, Delegate> methods = GetMethods<TDelegate>(target.GetType(), id, index, false);
			MethodInfo invoke = dlgType.GetMethod("Invoke");
			// 构造委托。
			ParameterExpression[] paramList = invoke.GetParameters().ToExpressions();
			// 取得关键类型。
			Expression getType = Expression.Call(paramList[index], typeof(object).GetMethod("GetType"));
			// 从字典取得相应委托。
			Expression getDlg = Expression.Invoke(
				Expression.Constant((Func<Dictionary<Type, Delegate>, Type, Delegate>)GetMethod),
				Expression.Constant(methods), getType);
			// 调用实例方法委托。
			Type insDlgType = GetInstanceDlgType(dlgType);
			getDlg = Expression.Convert(getDlg, insDlgType);
			Expression[] invokeArgs = new Expression[paramList.Length + 1];
			invokeArgs[0] = Expression.Constant(target);
			for (int i = 0; i < paramList.Length; i++)
			{
				invokeArgs[i + 1] = paramList[i];
			}
			// 调用委托。
			Expression invokeDlg = Expression.Invoke(getDlg, invokeArgs);
			return Expression.Lambda<TDelegate>(invokeDlg, paramList).Compile() as TDelegate;
		}

		#endregion // 返回方法切换器

		#region 查找方法

		/// <summary>
		/// 返回与指定标识符相关的处理器方法集合。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="type">在其中查找静态或实例方法的类型。</param>
		/// <param name="id">方法切换器的标识符。</param>
		/// <param name="index">方法的关键参数索引。</param>
		/// <param name="queryStatic">是否请求的是静态方法。</param>
		/// <returns>与指定标识符相关的处理器方法集合。</returns>
		private static Dictionary<Type, Delegate> GetMethods<TDelegate>(Type type, string id, int index, bool queryStatic)
		{
			Type dlgType = typeof(TDelegate);
			Tuple<bool, Type, Dictionary<Type, Delegate>> data;
			string key = string.Concat(type.FullName, "_", id);
			if (!methodDict.TryGetValue(key, out data))
			{
				MethodInfo[] methods = type.GetMethods(MethodFlags);
				List<MethodInfo> list = new List<MethodInfo>();
				for (int i = 0; i < methods.Length; i++)
				{
					if (methods[i].GetCustomAttributes(typeof(ProcessorAttribute), true)
						.Cast<ProcessorAttribute>().Any(s => s.Id == id))
					{
						list.Add(methods[i]);
					}
				}
				int cnt = list.Count;
				if (cnt == 0)
				{
					throw ExceptionHelper.ProcessorNotFound("type", type, id);
				}
				bool isStatic = list[0].IsStatic;
				for (int i = 1; i < cnt; i++)
				{
					if (list[i].IsStatic != isStatic)
					{
						throw ExceptionHelper.ProcessorMixed("type", type, id);
					}
				}
				Dictionary<Type, Delegate> dict = new Dictionary<Type, Delegate>();
				Type newDlgType = dlgType;
				if (!isStatic)
				{
					newDlgType = GetInstanceDlgType(newDlgType);
				}
				for (int i = 0; i < cnt; i++)
				{
					Type keyType = list[i].GetParameters()[index].ParameterType;
					Delegate dlg = DelegateBuilder.CreateDelegate(newDlgType, list[i], false);
					if (dlg == null)
					{
						throw ExceptionHelper.DelegateCompatible(list[i].ToString(), dlgType);
					}
					dict.Add(keyType, dlg);
				}
				data = new Tuple<bool, Type, Dictionary<Type, Delegate>>(isStatic, dlgType, dict);
				methodDict.Add(key, data);
			}
			if (data.Item1 != queryStatic)
			{
				throw ExceptionHelper.ProcessorMismatch("type", type, id);
			}
			if (data.Item2 != dlgType)
			{
				// 检查委托参数。
				ParameterInfo[] paramInfos = data.Item2.GetMethod("Invoke").GetParameters();
				ParameterInfo[] dlgParamInfos = dlgType.GetMethod("Invoke").GetParameters();
				if (paramInfos.Length != dlgParamInfos.Length)
				{
					throw ExceptionHelper.DelegateCompatible("TDelegate", dlgType);
				}
				for (int i = 0; i < paramInfos.Length; i++)
				{
					if (paramInfos[i].ParameterType != dlgParamInfos[i].ParameterType)
					{
						throw ExceptionHelper.DelegateCompatible("TDelegate", dlgType);
					}
				}
			}
			return data.Item3;
		}
		/// <summary>
		/// 返回实例方法对应的委托类型，需要将实例对象作为第一个参数。
		/// </summary>
		/// <param name="type">原始的委托类型。</param>
		/// <returns>实例方法对应的委托类型。</returns>
		private static Type GetInstanceDlgType(Type type)
		{
			MethodInfo invoke = type.GetMethod("Invoke");
			ParameterInfo[] paramInfos = invoke.GetParameters();
			Type[] types = new Type[paramInfos.Length + 2];
			types[0] = typeof(object);
			for (int i = 0; i < paramInfos.Length; i++)
			{
				types[i + 1] = paramInfos[i].ParameterType;
			}
			types[types.Length - 1] = invoke.ReturnType;
			return Expression.GetDelegateType(types);
		}
		/// <summary>
		/// 返回与指定类型相关的处理器方法委托。
		/// </summary>
		/// <param name="dict">要获取处理器方法的字典。</param>
		/// <param name="type">要获取处理器方法的类型。</param>
		/// <returns>与指定类型相关的处理器方法委托。</returns>
		private static Delegate GetMethod(Dictionary<Type, Delegate> dict, Type type)
		{
			Delegate dlg = GetMethodUnderlying(dict, type);
			if (dlg == null)
			{
				throw ExceptionHelper.ProcessorNotFound("type", type);
			}
			return dlg;
		}
		/// <summary>
		/// 返回与指定类型相关的处理器方法委托。
		/// </summary>
		/// <param name="dict">要获取处理器方法的字典。</param>
		/// <param name="type">要获取处理器方法的类型。</param>
		/// <returns>与指定类型相关的处理器方法委托。</returns>
		private static Delegate GetMethodUnderlying(Dictionary<Type, Delegate> dict, Type type)
		{
			Delegate dlg;
			if (dict.TryGetValue(type, out dlg))
			{
				return dlg;
			}
			else if (type.BaseType == null)
			{
				return null;
			}
			else
			{
				dlg = GetMethodUnderlying(dict, type.BaseType);
				dict.Add(type, dlg);
				return dlg;
			}
		}

		#endregion // 查找方法

	}
}
