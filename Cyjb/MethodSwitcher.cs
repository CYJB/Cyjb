using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Cyjb.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 方法调用的切换器，对于使用同一个基类的不同子类作为参数的多个方法，能够自动推断关键参数
	/// （使用不同子类的参数，必须是唯一的），并根据关键参数的实际类型调用相应的方法。
	/// </summary>
	/// <remarks><see cref="MethodSwitcher"/> 可以手动添加委托，也可以自动从类型中寻找处理器。
	/// 关于方法切换器的更多信息，可以参加我的博文<see href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">
	/// 《C# 方法调用的切换器》</see></remarks>
	/// <seealso cref="ProcessorAttribute"/>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">《C# 方法调用的切换器》</seealso>
	/// <example>
	/// 下面演示了手动添加委托的简单用法。
	/// <code>
	/// class Program {
	/// 	static void A(int m) { Console.WriteLine("int"); }
	/// 	static void B(string m) { Console.WriteLine("string"); }
	/// 	static void C(Array m) { Console.WriteLine("Array"); }
	/// 	static void D(int[] m) { Console.WriteLine("int[]"); }
	/// 	static void E(object m) { Console.WriteLine("object"); }
	/// 	static void Main(string[] args) {
	/// 		Action&lt;object&gt; invoke = MethodSwitcher.Create&lt;Action&lt;object&gt;&gt;((Action&lt;int&gt;)A, 
	/// 			(Action&lt;string&gt;)B, (Action&lt;Array&gt;)C, (Action&lt;int[]&gt;)D, (Action&lt;object&gt;)E);
	/// 		invoke(10);
	/// 		invoke("10");
	/// 		invoke(new int[0]);
	/// 		invoke(new string[0]);
	/// 		invoke(10L);
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// 下面演示了自动寻找处理器的简单用法。
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
	/// 		Action&lt;object&gt; invoke = MethodSwitcher.Create&lt;Action&lt;object&gt;&gt;(typeof(Program));
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
		/// 表示 object.GetType 方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo methodGetType = typeof(object).GetMethod("GetType");
		/// <summary>
		/// 表示 MethodSwitcher.GetMethod 方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo methodGetMethod = typeof(MethodSwitcher).GetMethod("GetMethod", TypeExt.StaticFlag);
		/// <summary>
		/// 特定类型中的所有标记为处理器的方法，及其元数据。
		/// </summary>
		private static readonly ConcurrentDictionary<string, ProcessorData> methodDict =
			new ConcurrentDictionary<string, ProcessorData>();

		#region 返回方法切换器

		/// <summary>
		/// 创建与指定委托列表相关的方法切换器，会自动推断关键参数（使用不同子类的参数，必须是唯一的）。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托类型。</typeparam>
		/// <param name="delegates">使用不同子类作为参数的委托列表。</param>
		/// <returns>与 <paramref name="delegates"/> 相关的方法切换器。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="delegates"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegates"/> 中存在为 <c>null</c> 的委托。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">委托类型与处理器不匹配。</exception>
		/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
		/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
		/// <overloads>
		/// <summary>
		/// 创建方法切换器，会自动推断关键参数（使用不同子类的参数，必须是唯一的）。
		/// </summary>
		/// </overloads>
		public static TDelegate Create<TDelegate>(params Delegate[] delegates)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(delegates, "delegates");
			if (delegates.Any(d => d == null))
			{
				throw CommonExceptions.CollectionItemNull("delegates");
			}
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			CommonExceptions.CheckDelegateType(typeof(TDelegate));
			ProcessorData data = new ProcessorData(delegates);
			CheckDelegateType<TDelegate>(data);
			return CreateSwitcher<TDelegate>(data, null, null);
		}
		/// <summary>
		/// 创建指定类型中与默认标识相关的静态方法切换器，会自动推断关键参数（使用不同子类的参数，必须是唯一的）。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="type">在其中查找静态方法的类型。</param>
		/// <returns><paramref name="type"/> 中与默认标识相关的静态方法切换器。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">委托类型与处理器不匹配。</exception>
		/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
		/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
		public static TDelegate Create<TDelegate>(Type type)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			CommonExceptions.CheckDelegateType(typeof(TDelegate));
			ProcessorData data = GetMethods<TDelegate>(type, ProcessorAttribute.DefaultId, true);
			return CreateSwitcher<TDelegate>(data, ProcessorAttribute.DefaultId, null);
		}
		/// <summary>
		/// 创建指定类型中与标识指定相关的静态方法切换器，会自动推断关键参数（使用不同子类的参数，必须是唯一的）。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="type">在其中查找静态方法的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns><paramref name="type"/> 中与指定标识相关的静态方法切换器。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="id"/> 为空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">委托类型与处理器不匹配。</exception>
		/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
		/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
		public static TDelegate Create<TDelegate>(Type type, string id)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(type, "type");
			CommonExceptions.CheckStringEmpty(id, "id");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			CommonExceptions.CheckDelegateType(typeof(TDelegate));
			ProcessorData data = GetMethods<TDelegate>(type, id, true);
			return CreateSwitcher<TDelegate>(data, id, null);
		}
		/// <summary>
		/// 创建指定对象中与默认指定相关的实例方法切换器，会自动推断关键参数（使用不同子类的参数，必须是唯一的）。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="target">在其中查找实例方法的对象。</param>
		/// <returns><paramref name="target"/> 中与默认标识相关的实例方法切换器。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">委托类型与处理器不匹配。</exception>
		/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
		/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
		public static TDelegate Create<TDelegate>(object target)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			CommonExceptions.CheckDelegateType(typeof(TDelegate));
			ProcessorData data = GetMethods<TDelegate>(target.GetType(), ProcessorAttribute.DefaultId, false);
			return CreateSwitcher<TDelegate>(data, ProcessorAttribute.DefaultId, target);
		}
		/// <summary>
		/// 创建指定对象中与标识指定相关的实例方法切换器，会自动推断关键参数（使用不同子类的参数，必须是唯一的）。
		/// </summary>
		/// <typeparam name="TDelegate">使用基类型调用方法的委托。</typeparam>
		/// <param name="target">在其中查找实例方法的对象。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns><paramref name="target"/> 中与指定标识相关的实例方法切换器。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="id"/> 为空字符串。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">委托类型与处理器不匹配。</exception>
		/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
		/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
		public static TDelegate Create<TDelegate>(object target, string id)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(id, "id");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			CommonExceptions.CheckDelegateType(typeof(TDelegate));
			ProcessorData data = GetMethods<TDelegate>(target.GetType(), id, false);
			return CreateSwitcher<TDelegate>(data, id, target);
		}
		/// <summary>
		/// 返回与指定处理器数据相关的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">委托的类型。</typeparam>
		/// <param name="data">处理器数据。</param>
		/// <param name="id">处理器标识。</param>
		/// <param name="instance">处理器要绑定到的实例。</param>
		/// <returns>与 <paramref name="data"/> 相关的 <typeparamref name="TDelegate"/> 类型的委托。</returns>
		private static TDelegate CreateSwitcher<TDelegate>(ProcessorData data, string id, object instance)
			where TDelegate : class
		{
			Contract.Requires(data != null);
			MethodInfo invoke = typeof(TDelegate).GetInvokeMethod();
			Type[] paramTypes = invoke.GetParameterTypes();
			object closure;
			if (data.IsStatic)
			{
				closure = data.Processors;
			}
			else
			{
				closure = new Closure(new[] { instance, data.Processors }, null);
			}
			DynamicMethod method = new DynamicMethod("MethodSwitcher", invoke.ReturnType,
				paramTypes.Insert(0, closure.GetType()), true);
			ILGenerator il = method.GetILGenerator();
			// 静态方法中，arg_0 用作存储处理器委托字典。
			if (data.IsStatic)
			{
				il.Emit(OpCodes.Ldarg_0);
			}
			else
			{
				// 实例方法中，Closure.Constants[1] 用作存储处理器委托字典。
				il.EmitLoadClosureConstant(1, typeof(Dictionary<Type, Delegate>));
			}
			// 判断关键参数是否为 null。
			il.EmitLoadArg(data.KeyIndex + 1);
			Label keyNullCase = il.DefineLabel();
			il.Emit(OpCodes.Brtrue, keyNullCase);
			// 关键参数为 null，将 object 作为查找类型。
			il.EmitConstant(typeof(object));
			Label endKeyNull = il.DefineLabel();
			il.Emit(OpCodes.Br, endKeyNull);
			// 关键参数不为 null，将参数类型作为查找类型。
			il.MarkLabel(keyNullCase);
			il.EmitLoadArg(data.KeyIndex + 1);
			il.EmitCall(methodGetType);
			il.MarkLabel(endKeyNull);
			// 调用 GetMethod 方法，取得方法委托。
			il.EmitConstant(id);
			il.EmitCall(methodGetMethod);
			il.Emit(OpCodes.Castclass, data.DelegateType);
			//// 载入参数，调用委托。
			Type[] originParamTypes = data.DelegateParamTypes;
			if (!data.IsStatic)
			{
				// 载入实例，Closure.Constants[0]。
				il.EmitLoadClosureConstant(0, instance.GetType());
			}
			int offset = data.IsStatic ? 0 : 1;
			for (int i = 0; i < paramTypes.Length; i++)
			{
				Type paramType = paramTypes[i];
				Type targetType = originParamTypes[i + offset];
				Contract.Assume(paramType != null && targetType != null);
				il.EmitLoadArg(i + 1, paramType, targetType);
			}
			il.Emit(OpCodes.Callvirt, data.DelegateType.GetInvokeMethod());
			Type returnType = originParamTypes[originParamTypes.Length - 1];
			Type targetReturnType = invoke.ReturnType;
			// 转换返回类型。
			if (returnType == typeof(void))
			{
				if (targetReturnType != typeof(void))
				{
					il.EmitConstant(null, targetReturnType);
				}
			}
			else
			{
				if (targetReturnType == typeof(void))
				{
					il.Emit(OpCodes.Pop);
				}
				else if (returnType != targetReturnType)
				{
					il.EmitConversion(returnType, targetReturnType, true, ConversionType.Explicit);
				}
			}
			il.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof(TDelegate), closure) as TDelegate;
		}

		#endregion // 返回方法切换器

		#region 查找方法

		/// <summary>
		/// 返回与指定类型和标识相关的处理器数据。
		/// </summary>
		/// <typeparam name="TDelegate">调用委托的类型。</typeparam>
		/// <param name="type">处理器所属的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <param name="needStatic">需要的是否是静态方法。</param>
		/// <returns>与指定类型和标识相关的处理器数据。</returns>
		/// <exception cref="ArgumentException">委托类型与处理器不匹配。</exception>
		private static ProcessorData GetMethods<TDelegate>(Type type, string id, bool needStatic)
		{
			Contract.Requires(type != null && id != null);
			Contract.Ensures(Contract.Result<ProcessorData>() != null);
			ProcessorData data = methodDict.GetOrAdd(string.Concat(type.FullName, "_", id),
				key => new ProcessorData(type, id));
			if (data.IsStatic != needStatic)
			{
				throw CommonExceptions.ProcessorMismatch(type, id);
			}
			CheckDelegateType<TDelegate>(data);
			return data;
		}
		/// <summary>
		/// 检查委托类型是否与处理器兼容。
		/// </summary>
		/// <typeparam name="TDelegate">调用委托的类型。</typeparam>
		/// <param name="data">处理器的数据。</param>
		/// <exception cref="ArgumentException">委托类型与处理器不匹配。</exception>
		private static void CheckDelegateType<TDelegate>(ProcessorData data)
		{
			Contract.Requires(data != null);
			Type dlgType = typeof(TDelegate);
			if (data.IsStatic)
			{
				if (data.DelegateType != dlgType)
				{
					// 检查静态委托参数。
					ParameterInfo[] paramInfos = data.DelegateType.GetMethod("Invoke").GetParametersNoCopy();
					ParameterInfo[] dlgParamInfos = dlgType.GetMethod("Invoke").GetParametersNoCopy();
					if (paramInfos.Length != dlgParamInfos.Length)
					{
						throw CommonExceptions.DelegateCompatible(data.DelegateType, dlgType);
					}
					if (paramInfos.Where((param, idx) => !param.ParameterType.IsExplicitFrom(dlgParamInfos[idx].ParameterType))
						.Any())
					{
						throw CommonExceptions.DelegateCompatible(data.DelegateType, dlgType);
					}
				}
			}
			else
			{
				// 检查实例委托参数，要考虑实例对应的参数。
				ParameterInfo[] paramInfos = data.DelegateType.GetMethod("Invoke").GetParametersNoCopy();
				ParameterInfo[] dlgParamInfos = dlgType.GetMethod("Invoke").GetParametersNoCopy();
				if (paramInfos.Length != dlgParamInfos.Length + 1)
				{
					throw CommonExceptions.DelegateCompatible(data.DelegateType, dlgType);
				}
				for (int i = 1; i < paramInfos.Length; i++)
				{
					if (!paramInfos[i].ParameterType.IsExplicitFrom(dlgParamInfos[i - 1].ParameterType))
					{
						throw CommonExceptions.DelegateCompatible(data.DelegateType, dlgType);
					}
				}
			}
		}
		/// <summary>
		/// 返回与指定类型相关的处理器委托。
		/// </summary>
		/// <param name="dict">处理器的委托字典。</param>
		/// <param name="type">要获取处理器委托的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns>与指定类型相关的处理器委托。</returns>
		private static Delegate GetMethod(Dictionary<Type, Delegate> dict, Type type, string id)
		{
			Contract.Requires(dict != null && type != null);
			Contract.Ensures(Contract.Result<Delegate>() != null);
			Delegate dlg = GetMethodUnderlying(dict, type);
			if (dlg == null)
			{
				throw CommonExceptions.ProcessorNotFound(type, id);
			}
			return dlg;
		}
		/// <summary>
		/// 返回与指定类型相关的处理器委托。
		/// </summary>
		/// <param name="dict">处理器的委托字典。</param>
		/// <param name="type">要获取处理器委托的类型。</param>
		/// <returns>与指定类型相关的处理器委托。</returns>
		private static Delegate GetMethodUnderlying(IDictionary<Type, Delegate> dict, Type type)
		{
			Contract.Requires(dict != null && type != null);
			Delegate dlg;
			if (dict.TryGetValue(type, out dlg))
			{
				return dlg;
			}
			if (type.BaseType == null)
			{
				return null;
			}
			dlg = GetMethodUnderlying(dict, type.BaseType);
			dict.Add(type, dlg);
			return dlg;
		}

		#endregion // 查找方法

		#region 处理器

		/// <summary>
		/// 与特定类型相关的处理器数据。
		/// </summary>
		private class ProcessorData
		{
			/// <summary>
			/// 委托方法是否是静态方法（无需传入实例）。
			/// </summary>
			public readonly bool IsStatic;
			/// <summary>
			/// 关键参数的索引。
			/// </summary>
			public readonly int KeyIndex;
			/// <summary>
			/// 字典中委托的类型。
			/// </summary>
			public readonly Type DelegateType;
			/// <summary>
			/// 委托的参数类型，最后一个参数表示返回值类型。
			/// </summary>
			public readonly Type[] DelegateParamTypes;
			/// <summary>
			/// 与类型相关的委托，委托的实际类型是 <see cref="DelegateType"/>。
			/// </summary>
			public readonly Dictionary<Type, Delegate> Processors;
			/// <summary>
			/// 使用处理器的委托列表初始化 <see cref="ProcessorData"/> 类的新实例。 
			/// </summary>
			/// <param name="delegates">使用不同子类作为参数的委托列表。</param>
			/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
			/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
			public ProcessorData(Delegate[] delegates)
			{
				Contract.Requires(delegates != null && delegates.All(d => d != null));
				this.IsStatic = true;
				List<MethodInfo> list = new List<MethodInfo>(delegates.Length);
				list.AddRange(delegates.Select(d => d.GetInvokeMethod()));
				int cnt = list.Count;
				// 发现关键参数，所有处理器各不相同的参数就认为是关键参数。
				List<Type[]> paramTypes = new List<Type[]>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					paramTypes.Add(list[i].GetParameterTypesWithReturn());
				}
				this.KeyIndex = FindKeyIndex(null, null, paramTypes);
				// 构造委托类型。
				this.DelegateParamTypes = FindDelegateType(paramTypes, null);
				this.DelegateType = Expression.GetDelegateType(this.DelegateParamTypes);
				this.Processors = new Dictionary<Type, Delegate>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					Type keyType = list[i].GetParameters()[this.KeyIndex].ParameterType;
					// 经过前面的类型检查，包装委托时应当总是会成功。
					this.Processors.Add(keyType, DelegateBuilder.Wrap(this.DelegateType, delegates[i]));
				}
			}
			/// <summary>
			/// 使用处理器所属的类型和标识初始化 <see cref="ProcessorData"/> 类的新实例。
			/// </summary>
			/// <param name="type">处理器所属的类型。</param>
			/// <param name="id">处理器的标识。</param>
			/// <exception cref="ArgumentException">没有找到处理器。</exception>
			/// <exception cref="ArgumentException">处理器中混杂着静态和动态方法。</exception>
			/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
			/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
			public ProcessorData(Type type, string id)
			{
				Contract.Requires(type != null && id != null);
				// 寻找处理器。
				MethodInfo[] methods = type.GetMethods(TypeExt.AllMemberFlag);
				List<MethodInfo> list = new List<MethodInfo>(methods.Length);
				list.AddRange(methods.Where(m => m.GetCustomAttributes(typeof(ProcessorAttribute), true)
					.Cast<ProcessorAttribute>().Any(p => p.Id == id)));
				int cnt = list.Count;
				if (cnt == 0)
				{
					throw CommonExceptions.ProcessorNotFound(type, id);
				}
				// 判断是静态方法还是动态方法。
				this.IsStatic = list[0].IsStatic;
				if (list.Any(m => m.IsStatic != IsStatic))
				{
					throw CommonExceptions.ProcessorMixed(type, id);
				}
				// 发现关键参数，所有处理器各不相同的参数就认为是关键参数。
				List<Type[]> paramTypes = new List<Type[]>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					paramTypes.Add(list[i].GetParameterTypesWithReturn());
				}
				this.KeyIndex = FindKeyIndex(type, id, paramTypes);
				// 构造委托类型。
				this.DelegateParamTypes = FindDelegateType(paramTypes, this.IsStatic ? null : type);
				this.DelegateType = Expression.GetDelegateType(this.DelegateParamTypes);
				this.Processors = new Dictionary<Type, Delegate>(cnt);
				for (int i = 0; i < cnt; i++)
				{
					Type keyType = list[i].GetParameters()[this.KeyIndex].ParameterType;
					// 经过前面的类型检查，创建委托时应当总是会成功。
					this.Processors.Add(keyType, DelegateBuilder.CreateDelegate(list[i], this.DelegateType));
				}
			}
			/// <summary>
			/// 找到方法参数中的关键参数。
			/// </summary>
			/// <param name="type">处理器所属的类型。</param>
			/// <param name="id">处理器的标识。</param>
			/// <param name="paramTypes">处理器参数类型列表。</param>
			/// <returns>关键参数的索引。</returns>
			/// <exception cref="ArgumentException">处理器的参数不匹配。</exception>
			/// <exception cref="ArgumentException">没有找到唯一的关键参数。</exception>
			private static int FindKeyIndex(Type type, string id, List<Type[]> paramTypes)
			{
				Contract.Requires(paramTypes != null && paramTypes.Count > 0);
				int cnt = paramTypes.Count;
				int paramCnt = -1;
				for (int i = 0; i < cnt; i++)
				{
					if (paramCnt == -1)
					{
						paramCnt = paramTypes[i].Length;
					}
					else if (paramCnt != paramTypes[i].Length)
					{
						throw CommonExceptions.ProcessorParameterMismatch(type, id);
					}
				}
				UniqueValue<int> keyIdx = new UniqueValue<int>();
				// 不考虑最后的返回值类型。
				for (int i = paramCnt - 2; i >= 0; i--)
				{
					var idx = i;
					if (!paramTypes.Select(types => types[idx]).Iterative().Any())
					{
						keyIdx.Value = idx;
					}
				}
				if (keyIdx.IsAmbig)
				{
					throw CommonExceptions.ProcessorKeyAmbigus(type, id);
				}
				if (keyIdx.IsEmpty)
				{
					throw CommonExceptions.ProcessorKeyNotFound(type, id);
				}
				return keyIdx.Value;
			}
			/// <summary>
			/// 找到委托的参数列表，最后一个参数总是表示返回类型。
			/// </summary>
			/// <param name="paramTypes">处理器参数类型列表。</param>
			/// <param name="instanceType">实例的类型，如果不需要传入实例则为 <c>null</c>。</param>
			/// <returns>委托的参数列表。</returns>
			private static Type[] FindDelegateType(List<Type[]> paramTypes, Type instanceType)
			{
				Contract.Requires(paramTypes != null && paramTypes.Count > 0);
				int len = paramTypes[0].Length;
				Type[] types;
				if (instanceType == null)
				{
					types = new Type[len];
				}
				else
				{
					len++;
					types = new Type[len];
					types[0] = instanceType;
				}
				int paramsIdx = paramTypes[0].Length - 1, typesIdx = len - 1;
				Contract.Assume(paramsIdx >= 0);
				int returnIdx = paramsIdx;
				types[typesIdx] = TypeExt.GetEncompassedType(paramTypes.Select(t => t[returnIdx])) ?? typeof(object);
				for (; paramsIdx >= 0; paramsIdx--, typesIdx--)
				{
					int idx = paramsIdx;
					types[typesIdx] = TypeExt.GetEncompassingType(paramTypes.Select(t => t[idx])) ?? typeof(object);
				}
				return types;
			}
		}

		#endregion // 处理器

	}
}
