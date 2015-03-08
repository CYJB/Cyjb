using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;

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
	/// 	public static void TestMethod2&lt;T&gt;(T value) { }
	/// }
	/// Type outputType = typeof(TestClass);
	/// Console.WriteLine(outputType.GetMethod("TestMethod", new Type[] { typeof(long) }));
	/// Console.WriteLine(outputType.GetMethod("TestMethod", BindingFlags.Static | BindingFlags.Public, 
	///		PowerBinder.CastBinder, new Type[] { typeof(long) }, null));
	/// Console.WriteLine(outputType.GetMethod("TestMethod2", new Type[] { typeof(string) }));
	/// Console.WriteLine(outputType.GetMethod("TestMethod2", BindingFlags.Static | BindingFlags.Public, 
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

		#region 静态成员

		/// <summary>
		/// 默认的 <see cref="PowerBinder"/> 实例。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static PowerBinder defaultInstance;
		/// <summary>
		/// 允许强制类型转换的 <see cref="PowerBinder"/> 实例。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static PowerBinder explicitInstance;
		/// <summary>
		/// 获取默认的 <see cref="PowerBinder"/> 实例。
		/// </summary>
		/// <value>默认的 <see cref="PowerBinder"/> 实例，对隐式类型转换和泛型方法提供支持。</value>
		public static PowerBinder Default
		{
			get
			{
				Contract.Ensures(Contract.Result<PowerBinder>() != null);
				return defaultInstance ?? (defaultInstance = new PowerBinder(false));
			}
		}
		/// <summary>
		/// 获取可以进行显式类型转换的 <see cref="PowerBinder"/> 实例。
		/// </summary>
		/// <value>可以进行显式类型转换的 <see cref="PowerBinder"/> 实例，对显式类型转换和泛型方法提供支持。</value>
		public static PowerBinder Explicit
		{
			get
			{
				Contract.Ensures(Contract.Result<PowerBinder>() != null);
				return explicitInstance ?? (explicitInstance = new PowerBinder(true));
			}
		}

		#endregion // 静态成员

		/// <summary>
		/// 是否进行显式类型转换。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool isExplicit;
		/// <summary>
		/// 使用是否进行显式类型转换初始化 <see cref="PowerBinder"/> 类的新实例。
		/// </summary>
		/// <param name="isExplicit">如果进行显式类型转换，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private PowerBinder(bool isExplicit)
		{
			this.isExplicit = isExplicit;
		}

		#region 类型转换

		/// <summary>
		/// 将给定 <see cref="Object"/> 的类型更改为给定 <see cref="Type"/>。
		/// </summary>
		/// <param name="value">要更改为新 <see cref="Type"/> 的对象。</param>
		/// <param name="type"><paramref name="value"/> 将变成的新 <see cref="Type"/>。</param>
		/// <param name="culture">一个 <see cref="CultureInfo"/> 实例，
		/// 用于控制数据类型的强制转换。</param>
		/// <returns>一个包含作为新类型的给定值的对象。</returns>
		public override object ChangeType([CanBeNull]object value, Type type, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			Type valueType = value.GetType();
			if (type.IsConvertFrom(valueType, isExplicit))
			{
				return Convert.ChangeType(value, type);
			}
			throw CommonExceptions.InvalidCast(valueType, type);
		}

		#endregion // 类型转换

		#region 绑定到字段

		/// <summary>
		/// 基于指定的判据，从给定的字段集中选择一个字段。
		/// </summary>
		/// <param name="bindingAttr"><see cref="BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选字段集。<see cref="PowerBinder"/> 的实现会更改此数组的顺序。</param>
		/// <param name="value">用于定位匹配字段的字段值。</param>
		/// <param name="culture">一个 <see cref="CultureInfo"/> 实例，
		/// 用于在强制类型的联编程序实现中控制数据类型强制。</param>
		/// <returns>匹配的字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 为空数组。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 中包含为 <c>null</c> 的元素。</exception>
		/// <exception cref="AmbiguousMatchException"><paramref name="match"/> 包含多个与 <paramref name="value"/>
		/// 匹配程度相同的字段。</exception>
		/// <exception cref="MissingFieldException"><paramref name="bindingAttr"/> 包含 
		/// <see cref="BindingFlags.SetField"/>，且 <paramref name="match"/> 不包含任何可接受 <paramref name="value"/>
		/// 的字段。</exception>
		public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, [CanBeNull]object value,
			CultureInfo culture)
		{
			CommonExceptions.CheckCollectionItemNull(match, "match");
			Contract.EndContractBlock();
			int length = 0;
			if (bindingAttr.HasFlag(BindingFlags.SetField))
			{
				// 在设置 SetField 标志时，根据 value 的类型进行选择。
				Type valueType = value == null ? null : value.GetType();
				for (int i = 0; i < match.Length; i++)
				{
					if (CanChangeType(valueType, match[i].FieldType))
					{
						match[length++] = match[i];
					}
				}
				if (length == 0)
				{
					throw CommonExceptions.MissingField();
				}
				if (length > 1 && valueType != null)
				{
					// 多个可匹配字段，尝试寻找类型最匹配的字段。
					length = FilterMember(match, length, (firstField, secondField) =>
						CompareType(firstField.FieldType, secondField.FieldType, valueType));
				}
			}
			else
			{
				length = match.Length;
			}
			FieldInfo best = GetDeepestMember(match, length);
			if (best == null)
			{
				throw CommonExceptions.AmbiguousMatchField();
			}
			return best;
		}
		/// <summary>
		/// 判断的类型是否可以从指定的类型转换而来。
		/// </summary>
		/// <param name="inputType">要尝试转换的类型，如果为 <c>null</c> 则表示引用类型约束。</param>
		/// <param name="outputType">要转换到的类型。</param>
		/// <returns>如果可以进行类型转换，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CanChangeType(Type inputType, Type outputType)
		{
			Contract.Requires(outputType != null);
			if (inputType == null)
			{
				return !outputType.IsValueType;
			}
			return outputType.IsConvertFrom(inputType, isExplicit);
		}
		/// <summary>
		/// 寻找与 <paramref name="type"/> 匹配的最好的类型。调用保证 <paramref name="type"/> 
		/// 可以转换为 <paramref name="firstType"/> 和 <paramref name="secondType"/>。
		/// </summary>
		/// <param name="firstType">要比较匹配的第一个类型。</param>
		/// <param name="secondType">要比较匹配的第二个类型。</param>
		/// <param name="type">要进行匹配的类型。</param>
		/// <returns>如果 <paramref name="firstType"/> 与 <paramref name="type"/> 匹配的更好，则为 <c>-1</c>；
		/// 如果 <paramref name="secondType"/> 与 <paramref name="type"/> 匹配的更好，则为 <c>1</c>；
		/// 如果匹配程度相同，则为 <c>0</c>。</returns>
		/// <remarks>《CSharp Language Specification》 7.5.3.3 Better conversion from expression。</remarks>
		private int CompareType(Type firstType, Type secondType, Type type)
		{
			Contract.Requires(firstType != null && secondType != null && type != null);
			Contract.Ensures(Contract.Result<int>() >= -1 && Contract.Result<int>() <= 1);
			if (firstType == secondType)
			{
				return 0;
			}
			if (firstType == type)
			{
				return -1;
			}
			if (secondType == type)
			{
				return 1;
			}
			bool typeByRef = type.IsByRef;
			// 根据 type 是否按引用传递，选择普通参数或按引用传递的参数。
			if (firstType.IsByRef)
			{
				firstType = firstType.GetElementType();
				if (secondType.IsByRef)
				{
					secondType = secondType.GetElementType();
				}
				else if (firstType == secondType)
				{
					return typeByRef ? -1 : 1;
				}
			}
			else if (secondType.IsByRef)
			{
				secondType = secondType.GetElementType();
				if (firstType == secondType)
				{
					return typeByRef ? 1 : -1;
				}
			}
			if (typeByRef)
			{
				type = type.GetElementType();
			}
			bool firstImplicitFromType = true;
			if (isExplicit)
			{
				// 可以从 type 隐式转换的类型会更匹配，仅当允许显式类型转换时才需要检测。
				firstImplicitFromType = firstType.IsImplicitFrom(type);
				if (firstImplicitFromType != secondType.IsImplicitFrom(type))
				{
					return firstImplicitFromType ? -1 : 1;
				}
			}
			ConversionType convType = TypeExt.GetStandardConversion(firstType, secondType);
			if (convType == ConversionType.None)
			{
				if (firstType.IsSigned())
				{
					if (secondType.IsUnsigned())
					{
						return -1;
					}
				}
				else if (firstType.IsUnsigned() && secondType.IsSigned())
				{
					return 1;
				}
				return 0;
			}
			if (convType.IsExplicit() == firstImplicitFromType)
			{
				// secondType 可以隐式转换为 firstType。
				return 1;
			}
			// firstType 可以隐式转换为 secondType。
			return -1;
		}
		/// <summary>
		/// 过滤指定的成员数组，选择其中匹配程度相同的成员移动到数组的最前面。
		/// </summary>
		/// <typeparam name="T">要过滤的成员类型。</typeparam>
		/// <param name="match">要过滤的成员数组。</param>
		/// <param name="length">成员数组的长度。</param>
		/// <param name="comparer">成员匹配程度的比较器。</param>
		/// <returns>过滤后的匹配程度相同的成员个数。</returns>
		private static int FilterMember<T>(T[] match, int length, Func<T, T, int> comparer)
		{
			Contract.Requires(match != null && match.Length >= length && length > 0 && comparer != null);
			Contract.Ensures(Contract.Result<int>() > 0);
			T target = match[0];
			int len = 1;
			for (int i = 1; i < length; i++)
			{
				// 尝试进一步匹配字段类型。
				int cmp = comparer(target, match[i]);
				if (cmp == 0)
				{
					match[len++] = match[i];
				}
				else if (cmp > 0)
				{
					match[0] = target = match[i];
					if (len == 1)
					{
						continue;
					}
					// 之前比较过的匹配程度相同的成员需要重新比较。
					if (len < i)
					{
						for (; i < length; i++)
						{
							match[len++] = match[i];
						}
						length = len;
					}
					len = 1;
					i = 0;
				}
			}
			return len;
		}
		/// <summary>
		/// 获取定义深度最深的成员。
		/// </summary>
		/// <typeparam name="TMember">成员的类型。</typeparam>
		/// <param name="members">成员数组。</param>
		/// <param name="length">成员数组的长度。</param>
		/// <returns>定义深度最深的成员，如果不存在则为 <c>null</c>。</returns>
		private static TMember GetDeepestMember<TMember>(TMember[] members, int length)
			where TMember : MemberInfo
		{
			Contract.Requires(members != null);
			TMember best = members[0];
			bool ambig = false;
			for (int i = 1; i < length; i++)
			{
				// 比较定义的层级深度。
				int cmp = best.DeclaringType.GetHierarchyDepth() - members[i].DeclaringType.GetHierarchyDepth();
				if (cmp == 0)
				{
					ambig = true;
				}
				else if (cmp < 0)
				{
					best = members[i];
					ambig = false;
				}
			}
			return ambig ? null : best;
		}

		#endregion // 绑定到字段

		#region 选择方法

		/// <summary>
		/// 基于参数类型，从给定的方法集中选择一个方法。
		/// 允许通过指定 <see cref="BindingFlags.OptionalParamBinding"/> 来匹配可选参数。
		/// </summary>
		/// <param name="bindingAttr"><see cref="BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选方法集。</param>
		/// <param name="types">用于定位匹配方法的参数类型。</param>
		/// <param name="modifiers">使绑定能够处理在其中修改了类型的参数签名的参数修饰符数组。</param>
		/// <returns>如果找到，则为匹配的方法；否则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 为空数组。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 中包含为 <c>null</c> 的元素。</exception>
		/// <exception cref="AmbiguousMatchException"><paramref name="match"/> 包含多个与 <paramref name="types"/>
		/// 匹配程度相同的方法。</exception>
		/// <remarks>此方法支持可选参数，可以通过为参数类型指定 <see cref="Missing"/> 来表示使用可选参数。
		/// 或者为 <paramref name="bindingAttr"/> 设置 <see cref="BindingFlags.InvokeMethod"/>、
		/// <see cref="BindingFlags.CreateInstance"/>、<see cref="BindingFlags.GetProperty"/> 或 
		/// <see cref="BindingFlags.SetProperty"/> 之一，就可以直接省略掉可选参数对应的类型。</remarks>
		public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types,
			ParameterModifier[] modifiers)
		{
			CommonExceptions.CheckCollectionItemNull(match, "match");
			CommonExceptions.CheckArgumentNull(types, "types");
			Contract.EndContractBlock();
			MethodArgumentsOption options = MethodArgumentsOption.None;
			if (isExplicit)
			{
				options |= MethodArgumentsOption.Explicit;
			}
			if (bindingAttr.HasFlag(BindingFlags.OptionalParamBinding))
			{
				options |= MethodArgumentsOption.OptionalParamBinding;
			}
			MethodMatchInfo[] infos = new MethodMatchInfo[match.Length];
			MethodMatchInfo info;
			int length = 0;
			for (int i = 0; i < match.Length; i++)
			{
				info = MethodMatchInfo.GetMatchInfo(match[i], types, null, options);
				if (info != null)
				{
					infos[length++] = info;
				}
			}
			info = SelectMethod(infos, length, types);
			return info == null ? null : info.Method;
		}
		/// <summary>
		/// 基于参数类型，从给定的方法集中选择一个方法。
		/// </summary>
		/// <param name="match">用于匹配的候选方法信息数组。</param>
		/// <param name="length">方法匹配信息的长度。</param>
		/// <param name="types">用于定位匹配方法的参数类型。</param>
		/// <returns>如果找到，则为匹配的方法信息；否则为 <c>null</c>。</returns>
		private MethodMatchInfo SelectMethod(MethodMatchInfo[] match, int length, Type[] types)
		{
			Contract.Requires(match != null && length >= 0 && types != null);
			if (length == 0)
			{
				// 没有可匹配的方法。
				return null;
			}
			if (length == 1)
			{
				// 只有一个可匹配的方法。
				return match[0];
			}
			// 多个可匹配字段，尝试寻找类型最匹配的方法。
			length = FilterMember(match, length, (firstMethod, secondMethod) =>
				CompareMethod(firstMethod, secondMethod, types));
			MethodMatchInfo best = match[0];
			bool ambig = false;
			for (int i = 1; i < length; i++)
			{
				// 比较定义的层级深度。
				int cmp = best.Method.DeclaringType.GetHierarchyDepth() - match[i].Method.DeclaringType.GetHierarchyDepth();
				if (cmp == 0)
				{
					ambig = true;
				}
				else if (cmp < 0)
				{
					best = match[i];
					ambig = false;
				}
			}
			return ambig ? null : best;
		}
		/// <summary>
		/// 寻找与 <paramref name="types"/> 匹配的最好的方法。
		/// </summary>
		/// <param name="firstMethod">要比较匹配的第一个方法。</param>
		/// <param name="secondMethod">要比较匹配的第二个方法。</param>
		/// <param name="types">要进行匹配的类型。</param>
		/// <returns>如果 <paramref name="firstMethod"/> 与 <paramref name="types"/> 匹配的更好，则返回一个负数；
		/// 如果 <paramref name="secondMethod"/> 与 <paramref name="types"/> 匹配的更好，则返回一个正数；
		/// 如果匹配程度相同，则为 <c>0</c>。</returns>
		/// <remarks>《CSharp Language Specification》 7.5.3.2 Better function member。</remarks>
		private int CompareMethod(MethodMatchInfo firstMethod, MethodMatchInfo secondMethod, Type[] types)
		{
			// 检查参数类型匹配。
			int typeLen = types.Length;
			int cmpMethod = 0;
			for (int i = 0; i < typeLen; i++)
			{
				int cmp = CompareType(firstMethod.GetParameterType(i), secondMethod.GetParameterType(i), types[i]);
				if (cmpMethod == 0)
				{
					cmpMethod = cmp;
				}
				else if (cmp * cmpMethod < 0)
				{
					// 参数类型都有更好的。
					return 0;
				}
			}
			if (cmpMethod != 0)
			{
				return cmpMethod;
			}
			// 非泛型方法更好。
			if (firstMethod.IsGeneric)
			{
				if (!secondMethod.IsGeneric)
				{
					return 1;
				}
			}
			else if (secondMethod.IsGeneric)
			{
				return -1;
			}
			// 不展开 params 参数的方法比匹配。
			if (firstMethod.ParamArrayType != null)
			{
				if (secondMethod.ParamArrayType == null)
				{
					return 1;
				}
			}
			else if (secondMethod.ParamArrayType != null)
			{
				return -1;
			}
			// 有 params 参数时形参更多的更好，没有使用默认值的更好。
			int firstLen = firstMethod.FixedParameters.Length;
			int secondLen = secondMethod.FixedParameters.Length;
			if (typeLen >= firstLen)
			{
				if (typeLen >= secondLen)
				{
					cmpMethod = secondLen - firstLen;
					if (cmpMethod != 0)
					{
						return cmpMethod;
					}
				}
				else
				{
					return -1;
				}
			}
			else if (typeLen >= secondLen)
			{
				return 1;
			}
			// 这里简单的比较泛型参数的个数，不将每个参数分别比较。
			return firstMethod.GenericArgumentLength - secondMethod.GenericArgumentLength;
		}

		#region 方法匹配信息

		/// <summary>
		/// 表示一个方法匹配及其相关信息。
		/// </summary>
		private class MethodMatchInfo
		{
			/// <summary>
			/// 返回指定方法的匹配信息。
			/// </summary>
			/// <param name="method">要进行匹配的方法。</param>
			/// <param name="types">方法的预期参数列表，使用 <c>null</c> 表示引用类型约束，<see cref="Missing"/> 
			/// 表示使用默认值。</param>
			/// <param name="paramOrder">方法的参数顺序列表，使用 <c>paramOrder[i]</c> 表示 <c>indexes[i]</c> 
			/// 的实际参数位置，其长度大于等于 <paramref name="types"/> 的长度。如果为 <c>null</c> 则表示使用默认顺序。</param>
			/// <param name="options">方法参数的选项。</param>
			/// <returns><paramref name="method"/> 的匹配信息，如果不能进行匹配则为 <c>null</c>。</returns>
			public static MethodMatchInfo GetMatchInfo(MethodBase method, Type[] types,
				int[] paramOrder, MethodArgumentsOption options)
			{
				Contract.Requires(method != null && types != null);
				Contract.Requires(paramOrder == null || paramOrder.Length >= types.Length);
				// 修正 indexes 的格式。
				ParameterInfo[] parameters = method.GetParametersNoCopy();
				if (paramOrder == null)
				{
					types = types.Extend(parameters.Length, typeof(Missing));
				}
				else
				{
					int typeLen = types.Length;
					Type[] newTypes = new Type[typeLen > parameters.Length ? typeLen : parameters.Length];
					newTypes.Fill(typeof(Missing));
					for (int i = 0; i < typeLen; i++)
					{
						newTypes[paramOrder[i]] = types[i];
					}
					types = newTypes;
				}
				MethodArgumentsInfo argsInfo;
				// 对泛型方法进行泛型类型推断。
				if (method.IsGenericMethodDefinition)
				{
					Type declaringType = method.DeclaringType;
					if (declaringType != null && declaringType.ContainsGenericParameters)
					{
						return null;
					}
					argsInfo = method.GenericArgumentsInferences(null, types, options);
					if (argsInfo == null)
					{
						return null;
					}
					method = ((MethodInfo)method).MakeGenericMethod(argsInfo.GenericArguments);
					argsInfo.UpdateParamArrayType(method);
				}
				else
				{
					argsInfo = MethodArgumentsInfo.GetInfo(method, types, options);
				}
				if (argsInfo == null)
				{
					return null;
				}
				if (argsInfo.OptionalArgumentTypes != null)
				{
					// 动态调用不支持可变参数。
					return null;
				}
				return new MethodMatchInfo(method, argsInfo, paramOrder);
			}
			/// <summary>
			/// 方法信息。
			/// </summary>
			public readonly MethodBase Method;
			/// <summary>
			/// 方法是否是由泛型方法定义推断而来。
			/// </summary>
			public readonly bool IsGeneric;
			/// <summary>
			/// 对应的泛型方法定义的泛型类型参数长度。
			/// </summary>
			public readonly int GenericArgumentLength;
			/// <summary>
			/// 固定参数的信息，如果需要展开 params 参数，则其不包含在此数组中。
			/// </summary>
			public readonly ParameterInfo[] FixedParameters;
			/// <summary>
			/// 方法的 params 参数的类型，如果为 <c>null</c> 表示无需特殊处理 params 参数。
			/// </summary>
			public readonly Type ParamArrayType;
			/// <summary>
			/// 方法的 params 参数的元素类型。
			/// </summary>
			public readonly Type ParamArrayElementType;
			/// <summary>
			/// 参数顺序，如果为 <c>null</c> 表示使用默认顺序。
			/// </summary>
			public readonly int[] ParamOrder;
			/// <summary>
			/// 使用指定的方法信息初始化 <see cref="MethodMatchInfo"/> 结构的新实例。
			/// </summary>
			/// <param name="method">方法信息。</param>
			/// <param name="argsInfo">方法实参信息。</param>
			/// <param name="paramOrder">参数顺序。</param>
			private MethodMatchInfo(MethodBase method, MethodArgumentsInfo argsInfo, int[] paramOrder)
			{
				Contract.Requires(method != null);
				this.Method = method;
				if (method.IsGenericMethod)
				{
					this.IsGeneric = method.IsGenericMethod;
					this.GenericArgumentLength = method.GetGenericArguments().Length;
				}
				this.FixedParameters = method.GetParametersNoCopy().Left(argsInfo.FixedArguments.Count);
				this.ParamArrayType = argsInfo.ParamArrayType;
				if (this.ParamArrayType != null)
				{
					this.ParamArrayElementType = this.ParamArrayType.GetElementType();
				}
				this.ParamOrder = paramOrder;
			}
			/// <summary>
			/// 获取指定实参索引对应的形参类型。
			/// </summary>
			/// <param name="index">实参的索引。</param>
			/// <returns>对应的形参类型。</returns>
			public Type GetParameterType(int index)
			{
				Contract.Requires(index >= 0);
				if (ParamOrder != null)
				{
					index = ParamOrder[index];
				}
				if (index > FixedParameters.Length)
				{
					return ParamArrayElementType;
				}
				return FixedParameters[index].ParameterType;
			}
		}

		#endregion // 方法匹配信息

		#endregion // 选择方法

		#region 绑定到方法

		/// <summary>
		/// 基于提供的参数，从给定的方法集中选择要调用的方法。
		/// </summary>
		/// <param name="bindingAttr"><see cref="BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选方法集。</param>
		/// <param name="args">传入的参数。</param>
		/// <param name="modifiers">使绑定能够处理在其中修改了类型的参数签名的参数修饰符数组。</param>
		/// <param name="culture">一个 <see cref="CultureInfo"/> 实例，
		/// 用于在强制类型的联编程序实现中控制数据类型强制。</param>
		/// <param name="names">参数名（如果匹配时要考虑参数名）或 <c>null</c>（如果要将变量视为纯位置）。</param>
		/// <param name="state">方法返回之后，<paramref name="state"/> 包含一个联编程序提供的对象，
		/// 用于跟踪参数的重新排序。</param>
		/// <returns>匹配的方法。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="args"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 为空数组。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 中包含为 <c>null</c> 的元素。</exception>
		/// <exception cref="AmbiguousMatchException"><paramref name="match"/> 包含多个与 <paramref name="args"/>
		/// 匹配程度相同的方法。</exception>
		/// <exception cref="MissingFieldException"><paramref name="bindingAttr"/> 包含 
		/// <see cref="BindingFlags.SetField"/>，且 <paramref name="match"/> 不包含任何可接受 <paramref name="args"/>
		/// 的方法。</exception>
		public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args,
			ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
		{
			CommonExceptions.CheckCollectionItemNull(match, "match");
			CommonExceptions.CheckArgumentNull(args, "args");
			if (names != null && names.Length > args.Length)
			{
				throw CommonExceptions.NamedParamTooBig("names");
			}
			Contract.EndContractBlock();
			// 检查参数名称数组，不能出现名称相同的参数。
			if (names != null && !names.IsDistinct(StringComparer.Ordinal))
			{
				throw CommonExceptions.DuplicateName("names");
			}
			MethodArgumentsOption options = MethodArgumentsOption.None;
			if (isExplicit)
			{
				options |= MethodArgumentsOption.Explicit;
			}
			if (bindingAttr.HasFlag(BindingFlags.OptionalParamBinding))
			{
				options |= MethodArgumentsOption.OptionalParamBinding;
			}
			int typesLen = args.Length;
			Type[] types = new Type[typesLen];
			for (int i = 0; i < typesLen; i++)
			{
				object arg = args[i];
				types[i] = arg == null ? null : arg.GetType();
			}
			MethodMatchInfo[] infos = new MethodMatchInfo[match.Length];
			MethodMatchInfo info;
			int length = 0;
			for (int i = 0; i < match.Length; i++)
			{
				int[] paramOrder = null;
				if (names != null)
				{
					paramOrder = CreateParamOrder(match[i], names, typesLen);
					if (paramOrder == null)
					{
						continue;
					}
				}
				info = MethodMatchInfo.GetMatchInfo(match[i], types, paramOrder, options);
				if (info != null)
				{
					infos[length++] = info;
				}
			}
			if (length == 0)
			{
				// 没有可匹配的方法。
				state = null;
				throw CommonExceptions.MissingMethod();
			}
			info = SelectMethod(infos, length, types);
			if (info == null)
			{
				state = null;
				throw CommonExceptions.AmbiguousMatchMethod();
			}
			UpdateArgs(info, ref args, out state);
			return info.Method;
		}
		/// <summary>
		/// 为指定的方法创建参数顺序。
		/// </summary>
		/// <param name="method">方法信息。</param>
		/// <param name="names">参数名称列表。</param>
		/// <param name="length">参数顺序的长度。</param>
		/// <returns>方法的参数顺序，如果找不到合法的顺序则为 <c>null</c>。</returns>
		private static int[] CreateParamOrder(MethodBase method, string[] names, int length)
		{
			Contract.Requires(method != null && names != null && length >= names.Length);
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			int[] paramOrder = new int[length].Fill(-1);
			HashSet<int> usedIndex = new HashSet<int>();
			// 找到与参数名称对应的参数索引。
			int idx;
			for (int i = 0; i < names.Length; i++)
			{
				string name = names[i];
				if (name == null)
				{
					// 占位符，直接跳过。
					continue;
				}
				idx = Array.FindIndex(parameters, param => name.Equals(param.Name, StringComparison.Ordinal));
				if (idx == -1)
				{
					return null;
				}
				usedIndex.Add(idx);
				paramOrder[i] = idx;
			}
			// 依次填充剩余的参数顺序。
			idx = 0;
			for (int i = 0; i < length; i++)
			{
				if (paramOrder[i] != -1)
				{
					continue;
				}
				while (!usedIndex.Add(idx))
				{
					idx++;
				}
				paramOrder[i] = idx;
			}
			return paramOrder;
		}
		/// <summary>
		/// 更新参数数组。
		/// </summary>
		/// <param name="match">被匹配的方法信息。</param>
		/// <param name="args">方法的参数。</param>
		/// <param name="state">旧的参数状态。</param>
		private static void UpdateArgs(MethodMatchInfo match, ref object[] args, out object state)
		{
			Contract.Requires(match != null && args != null);
			state = args;
			// 填充参数。
			ParameterInfo[] parameters = match.FixedParameters;
			int fixedLen = parameters.Length;
			int length = fixedLen;
			Array paramArray = null;
			if (match.ParamArrayType != null)
			{
				length++;
				paramArray = Array.CreateInstance(match.ParamArrayElementType, args.Length - fixedLen);
			}
			object[] newArgs = new object[length].Fill(Missing.Value);
			if (paramArray != null)
			{
				newArgs[fixedLen] = paramArray;
			}
			int[] paramOrder = match.ParamOrder;
			for (int i = 0; i < args.Length; i++)
			{
				int idx = paramOrder == null ? i : paramOrder[i];
				if (idx < fixedLen)
				{
					newArgs[idx] = args[i];
				}
				else
				{
					Contract.Assume(paramArray != null);
					paramArray.SetValue(args[i], idx - fixedLen);
				}
			}
			// 填充默认值。
			for (int i = 0; i < fixedLen; i++)
			{
				if (newArgs[i] != Missing.Value)
				{
					continue;
				}
				ParameterInfo param = parameters[i];
				if (param.IsParamArray())
				{
					newArgs[i] = Array.CreateInstance(param.ParameterType.GetElementType(), 0);
				}
				else
				{
					newArgs[i] = param.DefaultValue;
				}
			}
			args = newArgs;
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

		#endregion // 绑定到方法

		#region 选择属性

		/// <summary>
		/// 基于参数类型，从给定的属性集中选择一个属性。
		/// </summary>
		/// <param name="bindingAttr"><see cref="BindingFlags"/> 值的按位组合。</param>
		/// <param name="match">用于匹配的候选属性集。</param>
		/// <param name="returnType">匹配属性必须具有的返回值。</param>
		/// <param name="indexes">所搜索的属性的索引类型。</param>
		/// <param name="modifiers">使绑定能够处理在其中修改了类型的参数签名的参数修饰符数组。</param>
		/// <returns>如果找到，则为匹配的属性；否则为 <c>null</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="returnType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 为空数组。</exception>
		/// <exception cref="ArgumentException"><paramref name="match"/> 中包含为 <c>null</c> 的元素。</exception>
		/// <exception cref="AmbiguousMatchException"><paramref name="match"/> 包含多个与 <paramref name="returnType"/>
		/// 匹配程度相同的方法。</exception>
		public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match,
			Type returnType, Type[] indexes, ParameterModifier[] modifiers)
		{
			CommonExceptions.CheckArgumentNull(match, "match");
			Contract.EndContractBlock();
			// 按返回值筛选。
			int length = match.Length;
			if (returnType != null)
			{
				int len = 0;
				for (int i = 0; i < length; i++)
				{
					if (CanChangeType(returnType, match[i].PropertyType))
					{
						match[len++] = match[i];
					}
				}
				length = len;
			}
			// 按索引参数筛选。
			if (indexes != null)
			{
				int idxLen = indexes.Length;
				int len = 0;
				for (int i = 0; i < length; i++)
				{
					ParameterInfo[] parameters = match[i].GetIndexParameters();
					if (parameters.Length == idxLen && (idxLen == 0 || CheckParameters(parameters, indexes)))
					{
						match[len++] = match[i];
					}
				}
				length = len;
			}
			if (length == 0)
			{
				return null;
			}
			if (length == 1)
			{
				return match[0];
			}
			// 多个可匹配属性，寻找类型最匹配的属性。
			length = FilterMember(match, length, (firstProperty, secondProperty) =>
				CompareProperty(firstProperty, secondProperty, returnType, indexes));
			PropertyInfo best = GetDeepestMember(match, length);
			if (best == null)
			{
				throw CommonExceptions.AmbiguousMatchProperty();
			}
			return best;
		}
		/// <summary>
		/// 检查指定参数是否与给定的类型匹配。
		/// </summary>
		/// <param name="parameters">要检查的参数。</param>
		/// <param name="types">给定的参数类型。</param>
		/// <returns>如果类型匹配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckParameters(ParameterInfo[] parameters, Type[] types)
		{
			Contract.Requires(parameters != null && types != null);
			for (int i = 0; i < parameters.Length; i++)
			{
				if (!CanChangeType(parameters[i].ParameterType, types[i]))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 寻找与 <paramref name="returnType"/> 和 <paramref name="indexes"/> 匹配的最好的属性。
		/// </summary>
		/// <param name="firstProperty">要比较匹配的第一个属性。</param>
		/// <param name="secondProperty">要比较匹配的第二个属性。</param>
		/// <param name="returnType">匹配属性必须具有的返回值。</param>
		/// <param name="indexes">所搜索的属性的索引类型。</param>
		/// <returns>如果 <paramref name="firstProperty"/> 匹配的更好，则返回 <c>-1</c>；
		/// 如果 <paramref name="secondProperty"/> 匹配的更好，则返回 <c>1</c>；
		/// 如果匹配程度相同，则为 <c>0</c>。</returns>
		private int CompareProperty(PropertyInfo firstProperty, PropertyInfo secondProperty, Type returnType, Type[] indexes)
		{
			Contract.Requires(firstProperty != null && secondProperty != null);
			if (indexes != null && indexes.Length > 0)
			{
				// 检查索引参数类型匹配。
				ParameterInfo[] firstParams = firstProperty.GetIndexParameters();
				ParameterInfo[] secondParams = secondProperty.GetIndexParameters();
				int typeLen = indexes.Length;
				int cmpMethod = 0;
				for (int i = 0; i < typeLen; i++)
				{
					int cmp = CompareType(firstParams[i].ParameterType, secondParams[i].ParameterType, indexes[i]);
					if (cmpMethod == 0)
					{
						cmpMethod = cmp;
					}
					else if (cmp * cmpMethod < 0)
					{
						// 参数类型都有更好的。
						return 0;
					}
				}
				if (cmpMethod != 0)
				{
					return cmpMethod;
				}
			}
			if (returnType != null)
			{
				// 检查返回类型匹配。
				return CompareType(firstProperty.PropertyType, secondProperty.PropertyType, returnType);
			}
			return 0;
		}

		#endregion // 选择属性

	}
}