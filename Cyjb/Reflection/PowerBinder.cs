using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 从候选者列表中选择一个成员，并执行实参类型到形参类型的类型转换。
	/// 在选择时，支持对泛型方法进行选择，并允许进行强制类型转换。
	/// </summary>
	/// <example>
	/// 下面的例子演示了 <see cref="PowerBinder"/> 对泛型方法和强制类型转换的支持。
	/// <code>
	/// class TestClass {
	/// 	public static void TestMethod(int value) { }
	/// 	public static void TestMethod2{T}(T value) { }
	/// }
	/// Type type = typeof(TestClass);
	/// Console.WriteLine(type.GetMethod("TestMethod", new Type[] { typeof(long) }));
	/// Console.WriteLine(type.GetMethod("TestMethod", BindingFlags.Static | BindingFlags.Public, 
	///		PowerBinder.CastBinder, new Type[] { typeof(long) }, null));
	/// Console.WriteLine(type.GetMethod("TestMethod2", new Type[] { typeof(string) }));
	/// Console.WriteLine(type.GetMethod("TestMethod2", BindingFlags.Static | BindingFlags.Public, 
	///		PowerBinder.DefaultBinder, new Type[] { typeof(string) }, null));
	/// </code>
	/// </example>
	/// <remarks>
	/// <para><see cref="PowerBinder"/> 类是对 <see cref="Binder"/> 类的扩展，
	/// 支持泛型方法和强制类型转换，可以有效的扩展反射得到类型成员的过程。</para>
	/// <para>关于 <see cref="Binder"/> 类的原理，以及 <see cref="PowerBinder"/> 类的实现，
	/// 可以参见我的博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/PowerBinder.html">
	/// 《C# 使用 Binder 类自定义反射》</see>。</para>
	/// <para>关于进行泛型类型推断的原理，可以参考我的博文
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/GenericArgumentsInferences.html">
	/// 《C# 泛型方法的类型推断》</see>。</para>
	/// </remarks>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/PowerBinder.html">
	/// 《C# 使用 Binder 类自定义反射》</seealso>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/GenericArgumentsInferences.html">
	/// 《C# 泛型方法的类型推断》</seealso>
	[Serializable]
	public sealed class PowerBinder : Binder
	{
		/// <summary>
		/// 默认的 <see cref="Cyjb.PowerBinder"/> 实例。
		/// </summary>
		private static PowerBinder defaultInstance;
		/// <summary>
		/// 允许强制类型转换的 <see cref="Cyjb.PowerBinder"/> 实例。
		/// </summary>
		private static PowerBinder castInstance;
		/// <summary>
		/// 获取默认的 <see cref="Cyjb.PowerBinder"/> 实例。
		/// </summary>
		/// <value>默认的 <see cref="Cyjb.PowerBinder"/> 实例，只对泛型方法提供支持。</value>
		public static PowerBinder DefaultBinder
		{
			get
			{
				if (defaultInstance == null)
				{
					Interlocked.CompareExchange(ref defaultInstance, new PowerBinder(false), null);
				}
				return defaultInstance;
			}
		}
		/// <summary>
		/// 获取允许强制类型转换的 <see cref="Cyjb.PowerBinder"/> 实例。
		/// </summary>
		/// <value>允许强制类型转换的 <see cref="Cyjb.PowerBinder"/> 实例，
		/// 对强制类型转换和泛型方法提供支持。</value>
		public static PowerBinder CastBinder
		{
			get
			{
				if (castInstance == null)
				{
					Interlocked.CompareExchange(ref castInstance, new PowerBinder(true), null);
				}
				return castInstance;
			}
		}
		/// <summary>
		/// 是否允许强制类型转换。
		/// </summary>
		private bool allowCast = false;
		/// <summary>
		/// 初始化 <see cref="Cyjb.PowerBinder"/> 类的新实例。
		/// </summary>
		private PowerBinder(bool allowCast)
		{
			this.allowCast = allowCast;
		}

		#region Binder 成员

		/// <summary>
		/// 基于指定的判据，从给定的字段集中选择一个字段。
		/// </summary>
		/// <param name="bindingAttr"><see cref="System.Reflection.BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选字段集。</param>
		/// <param name="value">用于定位匹配字段的字段值。</param>
		/// <param name="culture">一个 <see cref="System.Globalization.CultureInfo"/> 实例，
		/// 用于在强制类型的联编程序实现中控制数据类型强制。</param>
		/// <returns>匹配的字段。</returns>
		public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value,
			CultureInfo culture)
		{
			int idx = 0;
			Type valueType = null;
			if (value != null) { valueType = value.GetType(); }
			bool setField = (bindingAttr & BindingFlags.SetField) != 0;
			if (setField)
			{
				// 在设置 SetField 标志时，根据 value 的类型进行选择。
				for (int i = 0; i < match.Length; i++)
				{
					if (CanChangeType(match[i].FieldType, valueType))
					{
						match[idx++] = match[i];
					}
				}
				if (idx == 0)
				{
					// 没有可匹配的字段。
					return null;
				}
				else if (idx > 1 && valueType != null)
				{
					// 多个可匹配字段，尝试寻找类型匹配的最好的字段。
					int len = idx;
					idx = 1;
					for (int i = 1; i < len; i++)
					{
						// 尝试进一步匹配字段类型。
						int cmp = FindMostSpecificType(match[0].FieldType, match[i].FieldType, valueType);
						if (cmp == 0)
						{
							match[idx++] = match[i];
						}
						else if (cmp == 2)
						{
							match[0] = match[i];
							idx = 1;
						}
					}
				}
			}
			else
			{
				idx = match.Length;
			}
			// 多个可匹配字段，寻找定义深度最深的字段。
			int min = 0;
			bool ambig = false;
			for (int i = 1; i < idx; i++)
			{
				// 比较定义的层级深度。
				int cmp = CompareHierarchyDepth(match[min], match[i]);
				if (cmp == 0)
				{
					ambig = true;
				}
				else if (cmp == 2)
				{
					min = i;
					ambig = false;
				}
			}
			if (ambig)
			{
				throw CommonExceptions.AmbiguousMatchField();
			}
			return match[min];
		}
		/// <summary>
		/// 基于提供的参数，从给定的方法集中选择要调用的方法。
		/// </summary>
		/// <param name="bindingAttr"><see cref="System.Reflection.BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选方法集。</param>
		/// <param name="args">传入的参数。</param>
		/// <param name="modifiers">使绑定能够处理在其中修改了类型的参数签名的参数修饰符数组。</param>
		/// <param name="culture">一个 <see cref="System.Globalization.CultureInfo"/> 实例，
		/// 用于在强制类型的联编程序实现中控制数据类型强制。</param>
		/// <param name="names">参数名（如果匹配时要考虑参数名）或 <c>null</c>（如果要将变量视为纯位置）。</param>
		/// <param name="state">方法返回之后，<paramref name="state"/> 包含一个联编程序提供的对象，
		/// 用于跟踪参数的重新排序。</param>
		/// <returns>匹配的方法。</returns>
		public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args,
			ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
		{
			if (match == null)
			{
				throw CommonExceptions.ArgumentNull("match");
			}
			if (match.Length == 0)
			{
				throw CommonExceptions.CollectionEmpty("match");
			}
			// 检查参数名称数组，不能出现名称相同的两个参数。
			if (names != null)
			{
				CheckNames(names);
			}
			// 构造方法信息数组。
			MatchInfo[] infos = new MatchInfo[match.Length];
			MatchInfo info = null;
			int idx = 0;
			for (int i = 0; i < match.Length; i++)
			{
				if (match[i] != null)
				{
					info = new MatchInfo(match[i]);
					int len = info.Parameters.Length > args.Length ? info.Parameters.Length : args.Length;
					if (names == null)
					{
						info.ParamOrder = info.ParamOrderRev = MethodExt.GetParamOrder(len);
					}
					else
					{
						info.ParamOrder = new int[len];
						// 根据 names 创建参数顺序。
						if (!CreateParamOrder(info.ParamOrder, info.Parameters, names))
						{
							continue;
						}
					}
					infos[idx++] = info;
				}
			}
			if (idx == 0)
			{
				// 没有可匹配的方法。
				state = null;
				return null;
			}
			Type[] types = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				// types[i] 为 null 表示可以匹配任何引用类型。
				if (args[i] != null)
				{
					types[i] = args[i].GetType();
				}
			}
			info = SelectMethod(bindingAttr, infos, idx, types);
			if (info == null)
			{
				// 没有可匹配的方法。
				state = null;
				return null;
			}
			UpdateArgs(info, ref args, names != null, out state);
			return info.Method;
		}
		/// <summary>
		/// 将给定 <see cref="Object"/> 的类型更改为给定 <see cref="Type"/>。
		/// </summary>
		/// <param name="value">要更改为新 <see cref="Type"/> 的对象。</param>
		/// <param name="type"><paramref name="value"/> 将变成的新 <see cref="Type"/>。</param>
		/// <param name="culture">一个 <see cref="CultureInfo"/> 实例，
		/// 用于控制数据类型的强制转换。</param>
		/// <returns>一个包含作为新类型的给定值的对象。</returns>
		public override object ChangeType(object value, Type type, CultureInfo culture)
		{
			return Convert.ChangeType(value, type);
		}
		/// <summary>
		/// 从 <see cref="BindToMethod"/> 返回后，将 <paramref name="args"/> 参数还原为从 
		/// <see cref="BindToMethod"/> 传入时的状态。
		/// </summary>
		/// <param name="args">传入的实参。参数的类型和值都可更改。</param>
		/// <param name="state">联编程序提供的对象，用于跟踪参数的重新排序。</param>
		public override void ReorderArgumentArray(ref object[] args, object state)
		{
			object[] oldArgs = state as object[];
			if (oldArgs != null)
			{
				args = oldArgs;
			}
		}
		/// <summary>
		/// 基于参数类型，从给定的方法集中选择一个方法。
		/// 允许通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 来匹配可选参数。
		/// </summary>
		/// <param name="bindingAttr"><see cref="System.Reflection.BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选方法集。</param>
		/// <param name="types">用于定位匹配方法的参数类型。</param>
		/// <param name="modifiers">使绑定能够处理在其中修改了类型的参数签名的参数修饰符数组。</param>
		/// <returns>如果找到，则为匹配的方法；否则为 <c>null</c>。</returns>
		public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types,
			ParameterModifier[] modifiers)
		{
			if (match == null)
			{
				throw CommonExceptions.ArgumentNull("match");
			}
			if (match.Length == 0)
			{
				throw CommonExceptions.CollectionEmpty("match");
			}
			// 构造方法信息数组。
			MatchInfo[] infos = new MatchInfo[match.Length];
			MatchInfo info = null;
			int idx = 0;
			for (int i = 0; i < match.Length; i++)
			{
				if (match[i] != null)
				{
					info = new MatchInfo(match[i]);
					int len = info.Parameters.Length > types.Length ? info.Parameters.Length : types.Length;
					info.ParamOrder = info.ParamOrderRev = MethodExt.GetParamOrder(len);
					infos[idx++] = info;
				}
			}
			info = SelectMethod(bindingAttr, infos, idx, types);
			if (info == null)
			{
				return null;
			}
			return info.Method;
		}
		/// <summary>
		/// 基于指定的判据，从给定的属性集中选择一个属性。
		/// </summary>
		/// <param name="bindingAttr"><see cref="System.Reflection.BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选属性集。</param>
		/// <param name="returnType">匹配属性必须具有的返回值。</param>
		/// <param name="indexes">所搜索的属性的索引类型。</param>
		/// <param name="modifiers">使绑定能够处理在其中修改了类型的参数签名的参数修饰符数组。</param>
		/// <returns>如果找到，则为匹配的属性；否则为 <c>null</c>。</returns>
		public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType,
			Type[] indexes, ParameterModifier[] modifiers)
		{
			CommonExceptions.CheckArgumentNull(match, "match");
			if (match.Length == 0)
			{
				return null;
			}
			int idxLen = indexes == null ? 0 : indexes.Length;
			// 构造属性信息数组。
			MatchInfo[] infos = new MatchInfo[match.Length];
			int idx = 0;
			for (int i = 0; i < match.Length; i++)
			{
				if (match[i] != null)
				{
					ParameterInfo[] parameters = match[i].GetIndexParameters();
					// 匹配属性类型与索引参数。
					if (parameters.Length == idxLen && CheckParameters(infos[i], indexes, idxLen) &&
						CanChangeType(match[i].PropertyType, returnType))
					{
						infos[idx] = new MatchInfo(parameters, MethodExt.GetParamOrder(idxLen));
						match[idx] = match[i];
						idx++;
					}
				}
			}
			if (idx == 0)
			{
				return null;
			}
			if (idx == 1)
			{
				return match[0];
			}
			// 多个可匹配属性，寻找匹配的最好的属性。
			int min = 0;
			bool ambig = false;
			for (int i = 1; i < idx; i++)
			{
				// 先比较属性类型。
				int cmp = FindMostSpecificType(match[min].PropertyType, match[i].PropertyType, returnType);
				if (cmp == 0 && indexes != null)
				{
					// 再比较属性参数。
					cmp = FindMostSpecific(infos[min], infos[i], indexes);
				}
				if (cmp == 0)
				{
					// 最后比较定义的层级深度。
					cmp = CompareHierarchyDepth(match[min], match[i]);
					if (cmp == 0)
					{
						ambig = true;
					}
				}
				if (cmp == 2)
				{
					ambig = false;
					min = i;
				}
			}
			if (ambig)
			{
				throw CommonExceptions.AmbiguousMatchProperty();
			}
			return match[min];
		}

		#endregion // Binder 成员

		#region 匹配信息类

		/// <summary>
		/// 表示一个匹配及其相关信息。
		/// </summary>
		private class MatchInfo
		{
			/// <summary>
			/// 方法信息。
			/// </summary>
			public MethodBase Method;
			/// <summary>
			/// 参数数组。
			/// </summary>
			public ParameterInfo[] Parameters;
			/// <summary>
			/// 参数顺序，不要依赖与该顺序的长度。
			/// </summary>
			public int[] ParamOrder;
			/// <summary>
			/// 逆序的参数顺序，不要依赖与该顺序的长度。
			/// </summary>
			public int[] ParamOrderRev;
			/// <summary>
			/// 方法的 params 参数类型。
			/// </summary>
			public Type ParamArrayType;
			/// <summary>
			/// 方法是否是开放泛型方法。
			/// </summary>
			public bool IsGeneric;
			/// <summary>
			/// 泛型方法的类型参数个数。
			/// </summary>
			public int GenericArgumentCount = 0;
			/// <summary>
			/// 使用指定的方法信息初始化 <see cref="PowerBinder.MatchInfo"/> 结构的新实例。
			/// </summary>
			/// <param name="method">方法信息。</param>
			public MatchInfo(MethodBase method)
			{
				this.Method = method;
				this.IsGeneric = method.IsGenericMethodDefinition;
				this.Parameters = method.GetParameters();
			}
			/// <summary>
			/// 使用指定的参数信息初始化 <see cref="PowerBinder.MatchInfo"/> 结构的新实例。
			/// </summary>
			/// <param name="parameters">参数信息。</param>
			/// <param name="paramOrder">参数顺序。</param>
			public MatchInfo(ParameterInfo[] parameters, int[] paramOrder)
			{
				this.Parameters = parameters;
				this.ParamOrder = this.ParamOrderRev = paramOrder;
			}
		}

		#endregion // 匹配信息类

		#region 辅助函数

		#region 选择方法

		/// <summary>
		/// 基于参数类型，从给定的方法集中选择一个方法。
		/// 允许通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 来匹配可选参数。
		/// </summary>
		/// <param name="bindingAttr"><see cref="System.Reflection.BindingFlags"/> 值的按位组合。</param>
		/// <param name="infos">用于匹配的候选方法信息集。</param>
		/// <param name="len">方法信息集合的长度。</param>
		/// <param name="types">用于定位匹配方法的参数类型。</param>
		/// <returns>如果找到，则为匹配的方法信息；否则为 <c>null</c>。</returns>
		private MatchInfo SelectMethod(BindingFlags bindingAttr, MatchInfo[] infos, int len, Type[] types)
		{
			bool optionalParamBinding = (bindingAttr & BindingFlags.OptionalParamBinding) != 0;
			int idx = 0;
			for (int i = 0; i < len; i++)
			{
				MatchInfo info = infos[i];
				Type paramArrayType;
				if (MethodExt.CheckParameterCount(info.Parameters, types, info.ParamOrder,
					optionalParamBinding, out paramArrayType))
				{
					info.ParamArrayType = paramArrayType;
					if (MakeGenericMethod(info, types) && CheckParameterType(info, types, optionalParamBinding))
					{
						infos[idx++] = info;
					}
				}
			}
			if (idx == 0)
			{
				// 没有可匹配的方法。
				return null;
			}
			else if (idx == 1)
			{
				// 只有一个可匹配的方法。
				return infos[0];
			}
			// 多个可匹配方法，寻找匹配的最好的方法。
			int min = 0;
			bool ambig = false;
			for (int i = 1; i < idx; i++)
			{
				int cmp = FindMostSpecificMethod(infos[min], infos[i], types);
				if (cmp == 0)
				{
					ambig = true;
				}
				else if (cmp == 2)
				{
					min = i;
					ambig = false;
				}
			}
			if (ambig)
			{
				throw CommonExceptions.AmbiguousMatchMethod();
			}
			return infos[min];
		}

		#endregion // 选择方法

		#region 参数顺序

		/// <summary>
		/// 检查参数名称数组是否有效。
		/// </summary>
		/// <param name="names">要检查的参数名称数组。</param>
		/// <exception cref="System.ArgumentException">参数名称数组包含重复名称。</exception>
		private static void CheckNames(string[] names)
		{
			for (int i = 0; i < names.Length; i++)
			{
				for (int j = i + 1; j < names.Length; j++)
				{
					if (string.Equals(names[i], names[j], StringComparison.Ordinal))
					{
						throw CommonExceptions.SameParameterName("names");
					}
				}
			}
		}
		/// <summary>
		/// 通过给定的参数和参数名称数组构造参数顺序映射。
		/// </summary>
		/// <param name="paramOrder">参数顺序映射。</param>
		/// <param name="parameters">参数列表。</param>
		/// <param name="names">参数名称列表。</param>
		/// <returns>参数顺序映射成功，则为 <c>true</c>；如果映射失败，则为 <c>null</c>。</returns>
		private static bool CreateParamOrder(int[] paramOrder, ParameterInfo[] parameters, string[] names)
		{
			if (names.Length > parameters.Length)
			{
				return false;
			}
			for (int i = 0; i < paramOrder.Length; i++)
			{
				paramOrder[i] = -1;
			}
			// 找到与参数名称对应的参数索引，names.Length <= parameters.Length。
			for (int i = 0; i < names.Length; i++)
			{
				int j;
				for (j = 0; j < parameters.Length; j++)
				{
					if (string.Equals(parameters[j].Name, names[i], StringComparison.Ordinal))
					{
						paramOrder[j] = i;
						break;
					}
				}
				// 未找到的参数名称，匹配失败。
				if (j == parameters.Length)
				{
					return false;
				}
			}
			// 依次填充剩余的 args 的参数顺序。
			int idx = names.Length;
			for (int i = 0; i < paramOrder.Length; i++)
			{
				if (paramOrder[i] == -1)
				{
					paramOrder[i] = idx++;
				}
			}
			return true;
		}
		/// <summary>
		/// 更新匹配信息的逆序的参数顺序。
		/// </summary>
		/// <param name="info">要更新的匹配信息。</param>
		private static void UpdateParamOrderRev(MatchInfo info)
		{
			if (info.ParamOrderRev == null)
			{
				int[] paramOrderRev = new int[info.ParamOrder.Length];
				for (int i = 0; i < paramOrderRev.Length; i++)
				{
					paramOrderRev[info.ParamOrder[i]] = i;
				}
				info.ParamOrderRev = paramOrderRev;
			}
		}

		#endregion // 参数顺序

		#region 构造封闭的泛型方法

		/// <summary>
		/// 使用类型数组的元素替代泛型方法定义的类型参数，并返回是否成功的标志。
		/// </summary>
		/// <param name="match">泛型方法的信息。</param>
		/// <param name="types">要替换泛型方法定义的类型参数的类型数组。</param>
		/// <returns>如果构造封闭的泛型方法，或者不是泛型方法则为 <c>true</c>；
		/// 如果没能成功构造封闭的泛型方法，则为 <c>false</c>。</returns>
		private static bool MakeGenericMethod(MatchInfo match, Type[] types)
		{
			MethodInfo method = match.Method as MethodInfo;
			if (method == null || !match.IsGeneric)
			{
				return true;
			}
			Type[] paramTypes = new Type[match.Parameters.Length];
			for (int i = 0; i < match.Parameters.Length; i++)
			{
				paramTypes[i] = match.Parameters[i].ParameterType;
			}
			Type paramArrayType = match.ParamArrayType;
			Type[] args = TypeExt.GenericArgumentsInferences(method.GetGenericArguments(),
				paramTypes, ref paramArrayType, types, match.ParamOrder);
			match.ParamArrayType = paramArrayType;
			try
			{
				method = method.MakeGenericMethod(args);
			}
			catch (ArgumentException)
			{
				// 不满足方法的约束。
				return false;
			}
			// 更新方法信息。
			match.Method = method;
			match.Parameters = method.GetParameters();
			match.GenericArgumentCount = args.Length;
			if (match.ParamArrayType != null)
			{
				match.ParamArrayType = MethodExt.GetParamArrayType(match.Parameters[match.Parameters.Length - 1]);
			}
			return true;
		}

		#endregion // 构造封闭的泛型方法

		#region 匹配参数类型

		/// <summary>
		/// 检测方法是否与给定的类型匹配。
		/// </summary>
		/// <param name="method">要检验的方法信息。</param>
		/// <param name="types">给定的参数类型。</param>
		/// <param name="optionalParamBinding">是否绑定默认值。</param>
		/// <returns>如果类型间允许类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckParameterType(MatchInfo method, Type[] types, bool optionalParamBinding)
		{
			int len = method.Parameters.Length;
			if (method.ParamArrayType != null)
			{
				// 检查 params 参数是否匹配。
				len--;
				for (int i = len; i < types.Length; i++)
				{
					if (!CanChangeType(method.ParamArrayType, types, method.ParamOrder[i]))
					{
						return false;
					}
				}
			}
			return CheckParameters(method, types, len);
		}
		/// <summary>
		/// 检测指定方法的参数是否与给定的类型匹配。
		/// </summary>
		/// <param name="method">要检验参数的方法信息。</param>
		/// <param name="types">给定的参数类型。</param>
		/// <param name="len">要检查的参数长度。</param>
		/// <returns>如果类型间允许强制类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckParameters(MatchInfo method, Type[] types, int len)
		{
			for (int i = 0; i < len; i++)
			{
				if (!CanChangeType(method.Parameters[i].ParameterType, types, method.ParamOrder[i]))
				{
					return false;
				}
			}
			return true;
		}

		#endregion // 匹配参数类型

		#region 方法进一步匹配

		/// <summary>
		/// 寻找匹配的最好的方法。
		/// </summary>
		/// <param name="method1">要比较匹配的第一个方法的信息。</param>
		/// <param name="method2">要比较匹配的第二个方法的信息。</param>
		/// <param name="types">给定的参数类型。</param>
		/// <returns>如果第一个方法匹配的更好，则为 <c>1</c>；如果第二个方法匹配的更好，则为 <c>2</c>；
		/// 如果不能区分，则为 <c>0</c>。</returns>
		private int FindMostSpecificMethod(MatchInfo method1, MatchInfo method2, Type[] types)
		{
			int res = FindMostSpecific(method1, method2, types);
			if (res != 0)
			{
				return res;
			}
			// 如果两个方法具有完全相同的签名，先判断是否是封闭的泛型方法。
			if (method1.IsGeneric)
			{
				if (method2.IsGeneric)
				{
					// 泛型参数个数较少的更好。
					if (method1.GenericArgumentCount < method2.GenericArgumentCount)
					{
						return 1;
					}
					else if (method1.GenericArgumentCount > method2.GenericArgumentCount)
					{
						return 2;
					}
				}
				else
				{
					return 2;
				}
			}
			else if (method2.IsGeneric)
			{
				return 1;
			}
			// 接下来比较所定义的层级深度。
			int len = method1.Parameters.Length;
			if (len == method2.Parameters.Length)
			{
				int i = 0;
				for (; i < len; i++)
				{
					if (method1.Parameters[i].ParameterType != method2.Parameters[i].ParameterType)
					{
						break;
					}
				}
				if (i == len)
				{
					return CompareHierarchyDepth(method1.Method, method2.Method);
				}
			}
			return 0;
		}

		#endregion // 方法进一步匹配

		#region 更新参数数组

		/// <summary>
		/// 更新参数数组。
		/// </summary>
		/// <param name="match">被匹配的方法信息。</param>
		/// <param name="args">方法的参数。</param>
		/// <param name="orderChanged">参数顺序是否发生了改变。</param>
		/// <param name="state">旧的参数状态。</param>
		private static void UpdateArgs(MatchInfo match, ref object[] args, bool orderChanged, out object state)
		{
			// 最简单的参数完全不需要调整的情况。
			if (match.Parameters.Length == 0 ||
				(match.Parameters.Length == args.Length && match.ParamArrayType == null && !orderChanged))
			{
				state = null;
				return;
			}
			// 保存旧的参数状态。
			object[] oldArgs = args;
			state = oldArgs;
			args = new object[match.Parameters.Length];
			int end = match.Parameters.Length - 1;
			// 根据名称调整参数顺序，同时使用默认值填充剩余参数。
			for (int i = match.ParamArrayType == null ? end : end - 1; i >= 0; i--)
			{
				if (match.ParamOrder[i] < oldArgs.Length)
				{
					args[i] = oldArgs[match.ParamOrder[i]];
				}
				else
				{
					args[i] = match.Parameters[i].DefaultValue;
				}
			}
			if (match.Parameters.Length >= oldArgs.Length)
			{
				// 对 params 参数进行判断。
				if (match.ParamArrayType != null)
				{
					Array paramsArray = null;
					if (match.ParamOrder[end] < oldArgs.Length)
					{
						// 最后一个参数是只有一个元素的数组。
						paramsArray = Array.CreateInstance(match.ParamArrayType, 1);
						paramsArray.SetValue(oldArgs[match.ParamOrder[end]], 0);
					}
					else
					{
						// 最后一个参数是空数组。
						paramsArray = Array.CreateInstance(match.ParamArrayType, 0);
					}
					args[end] = paramsArray;
				}
			}
			else
			{
				// 参数过多，将多余的参数包装为一个数组。
				if ((match.Method.CallingConvention & CallingConventions.VarArgs) == 0)
				{
					Array paramsArray = Array.CreateInstance(match.ParamArrayType, oldArgs.Length - end);
					for (int i = 0; i < paramsArray.Length; i++)
					{
						paramsArray.SetValue(oldArgs[match.ParamOrder[i + end]], i);
					}
					args[end] = paramsArray;
				}
			}
		}

		#endregion // 更新参数数组

		#region 类型匹配

		/// <summary>
		/// 判断的类型是否可以从指定的类型转换而来。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <param name="types">要尝试转换的类型数组。</param>
		/// <param name="index">类型数组的索引。</param>
		/// <returns>如果可以进行类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CanChangeType(Type type, Type[] types, int index)
		{
			if (index >= types.Length)
			{
				return true;
			}
			return CanChangeType(type, types[index]);
		}
		/// <summary>
		/// 判断的类型是否可以从指定的类型转换而来。
		/// </summary>
		/// <param name="type">要判断的类型。</param>
		/// <param name="fromType">要尝试转换的类型。</param>
		/// <returns>如果可以进行类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks><paramref name="fromType"/> 使用 <c>null</c> 表示任意引用类型。</remarks>
		private static bool CanChangeType(Type type, Type fromType)
		{
			if (fromType == null)
			{
				return type.IsClass;
			}
			return Convert.CanChangeType(fromType, type);
		}
		/// <summary>
		/// 寻找匹配的最好的参数类型。
		/// </summary>
		/// <param name="match1">要比较匹配的第一组参数。</param>
		/// <param name="match2">要比较匹配的第二组参数。</param>
		/// <param name="types">给定的参数类型。</param>
		/// <returns>如果第一组参数匹配的更好，则为 <c>1</c>；如果第二组参数匹配的更好，则为 <c>2</c>；
		/// 如果不能区分，则为 <c>0</c>。</returns>
		private int FindMostSpecific(MatchInfo match1, MatchInfo match2, Type[] types)
		{
			// 优先选择不具有 params 的参数。
			if (match1.ParamArrayType != null && match2.ParamArrayType == null)
			{
				return 2;
			}
			if (match2.ParamArrayType != null && match1.ParamArrayType == null)
			{
				return 1;
			}
			UpdateParamOrderRev(match1);
			UpdateParamOrderRev(match2);
			bool p1Better = false;
			bool p2Better = false;
			int p1Len = match1.Parameters.Length - 1;
			int p2Len = match2.Parameters.Length - 1;
			for (int i = 0; i < types.Length; i++)
			{
				Type type1, type2;
				// 得到 types[i] 实际对应的方法参数。
				int idx = match1.ParamOrderRev[i];
				if (match1.ParamArrayType != null && idx >= p1Len)
				{
					type1 = match1.ParamArrayType;
				}
				else
				{
					type1 = match1.Parameters[idx].ParameterType;
				}
				idx = match2.ParamOrderRev[i];
				if (match2.ParamArrayType != null && idx >= p2Len)
				{
					type2 = match2.ParamArrayType;
				}
				else
				{
					type2 = match2.Parameters[idx].ParameterType;
				}
				switch (FindMostSpecificType(type1, type2, types[i]))
				{
					case 1:
						p1Better = true;
						break;
					case 2:
						p2Better = true;
						break;
				}
			}
			if (p1Better == p2Better)
			{
				if (!p1Better)
				{
					if (p1Len < p2Len)
					{
						return 1;
					}
					else if (p1Len > p2Len)
					{
						return 2;
					}
				}
				return 0;
			}
			else
			{
				return p1Better ? 1 : 2;
			}
		}
		/// <summary>
		/// 寻找与 <paramref name="type"/> 匹配的最好的类型。
		/// </summary>
		/// <param name="type1">要比较匹配的第一个类型。</param>
		/// <param name="type2">要比较匹配的第二个类型。</param>
		/// <param name="type">给定的类型。</param>
		/// <returns>如果第一个类型匹配的更好，则为 <c>1</c>；如果第二个类型匹配的更好，则为 <c>2</c>；
		/// 如果不能区分，则为 <c>0</c>。</returns>
		private int FindMostSpecificType(Type type1, Type type2, Type type)
		{
			if (type1 == type2)
			{
				return 0;
			}
			if (type1 == type)
			{
				return 1;
			}
			if (type2 == type)
			{
				return 2;
			}
			bool typeByRef = type.IsByRef;
			// 根据 type 是否按引用传递，选择普通参数或按引用传递的参数。
			if (type1.IsByRef)
			{
				if (type2.IsByRef)
				{
					type2 = type2.GetElementType();
				}
				else if (type1.GetElementType() == type2)
				{
					return typeByRef ? 1 : 2;
				}
				type1 = type1.GetElementType();
			}
			else if (type2.IsByRef)
			{
				type2 = type2.GetElementType();
				if (type2 == type1)
				{
					return typeByRef ? 2 : 1;
				}
			}
			if (typeByRef)
			{
				type = type.GetElementType();
			}
			bool t1FromT = true;
			if (allowCast)
			{
				// 可以从 type 隐式转换的类型会更匹配，仅当允许强制类型转换时才需要检测。
				t1FromT = type1.IsImplicitFrom(type);
				bool t2FromT = type2.IsImplicitFrom(type);
				if (t1FromT != t2FromT)
				{
					return t1FromT ? 1 : 2;
				}
			}
			// 判断 type1 和 type2 间的隐式类型转换关系。
			bool t1FromT2 = type1.IsImplicitFrom(type2);
			bool t2FromT1 = type2.IsImplicitFrom(type1);
			if (t1FromT2 == t2FromT1)
			{
				return 0;
			}
			// 若 t1FromT == true，说明 T 比 T1 和 T2 都窄，因此从 T1 和 T2 中选择更窄的那个。
			// 若 t1FromT == false，说明 T 比 T1 和 T2 都宽，因此从 T1 和 T2 中选择更宽的那个。
			return t1FromT == t1FromT2 ? 2 : 1;
		}
		/// <summary>
		/// 比较给定成员被定义的层级深度。
		/// </summary>
		/// <param name="member1">要比较的第一个成员。</param>
		/// <param name="member2">要比较的第二个成员。</param>
		/// <returns>如果第一个成员被定义的的层级深度更大，则为 <c>1</c>；如果第二个成员被定义的层级深度更大，则为 <c>2</c>；
		/// 如果相同，则为 <c>0</c>。</returns>
		private static int CompareHierarchyDepth(MemberInfo member1, MemberInfo member2)
		{
			int depth1 = member1.DeclaringType.GetHierarchyDepth();
			int depth2 = member2.DeclaringType.GetHierarchyDepth();
			if (depth1 == depth2)
			{
				return 0;
			}
			else if (depth1 < depth2)
			{
				return 2;
			}
			else
			{
				return 1;
			}
		}

		#endregion // 类型匹配

		#endregion // 辅助函数

	}
}
