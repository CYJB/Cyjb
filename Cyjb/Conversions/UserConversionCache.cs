using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Cyjb.Reflection;
using Cyjb.Utility;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示用户定义类型转换的缓存。
	/// </summary>
	/// <remarks>
	/// </remarks>
	internal static class UserConversionCache
	{
		/// <summary>
		/// 用户自定义类型转换方法的缓存。
		/// </summary>
		private static readonly ICache<Type, UserConversions> conversions =
			CacheFactory.Create<Type, UserConversions>("Cyjb.UserConversionCache") ??
			new LruCache<Type, UserConversions>(256);
		/// <summary>
		/// 返回与指定类型相关的用户自定义类型转换方法。基类中声明的类型转换方法也包含在内。
		/// </summary>
		/// <param name="type">要获取类型转换方法的类型。</param>
		/// <returns>与指定类型相关的用户自定义类型转换方法，如果不存在则为 <c>null</c>。</returns>
		private static UserConversions GetUserConversions(Type type)
		{
			Contract.Requires(type != null);
			TypeCode typeCode = Type.GetTypeCode(type);
			if (typeCode != TypeCode.Object && typeCode != TypeCode.Decimal)
			{
				// 其余基本类型都不包含类型转换运算符。
				return null;
			}
			if (!type.IsClass && !type.IsValueType)
			{
				// 只有类和结构体中能声明类型转换运算符。
				return null;
			}
			if (type == typeof(object))
			{
				return null;
			}
			return conversions.GetOrAdd(type, searchType =>
			{
				List<UserConversionMethod> list = new List<UserConversionMethod>();
				MethodInfo[] methods = searchType.GetMethods(TypeExt.PublicStaticFlag);
				int convertToIndex = 0;
				for (int i = 0; i < methods.Length; i++)
				{
					MethodInfo method = methods[i];
					if (!method.Name.Equals(MethodExt.ImplicitMethodName, StringComparison.Ordinal) &&
						!method.Name.Equals(MethodExt.ExplicitMethodName, StringComparison.Ordinal))
					{
						continue;
					}
					if (method.ReturnType == type)
					{
						// 转换自其它类型。
						list.Insert(convertToIndex, new UserConversionMethod(method));
						convertToIndex++;
					}
					else
					{
						// 转换到其它类型。
						list.Add(new UserConversionMethod(method));
					}
				}
				// 基类包含的转换，子类也可以使用。
				if (type.IsClass)
				{
					UserConversions baseConv = GetUserConversions(type.BaseType);
					if (baseConv != null)
					{
						if (list.Count == 0)
						{
							return baseConv;
						}
						int cnt = baseConv.ConvertToIndex;
						if (cnt > 0)
						{
							list.InsertRange(convertToIndex, baseConv.Methods.Take(cnt));
							convertToIndex += cnt;
						}
						if (cnt < baseConv.Methods.Length)
						{
							list.AddRange(baseConv.Methods.Skip(cnt));
						}
					}
				}
				if (list.Count == 0)
				{
					return null;
				}
				return new UserConversions(list.ToArray(), convertToIndex);
			});
		}
		/// <summary>
		/// 在 <paramref name="basicType"/> 类型中寻找能够将对象从 <paramref name="inputType"/> 类型转换为 
		/// <paramref name="basicType"/> 类型的用户自定义类型转换方法。
		/// 需要保证 <paramref name="basicType"/> 中存在用户自定义类型转换方法。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="basicType">要寻找自定义类型转换方法的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="basicType"/> 
		/// 类型的用户自定义类型转换方法。如果不存在则为 <c>null</c>。</returns>
		public static MethodInfo GetConversionFrom(Type basicType, Type inputType)
		{
			Contract.Requires(inputType != null && basicType != null);
			UserConversions convs = GetUserConversions(basicType);
			for (int i = 0; i < convs.ConvertToIndex; i++)
			{
				UserConversionMethod method = convs.Methods[i];
				if (method.InputType == inputType)
				{
					return method.Method;
				}
			}
			return null;
		}
		/// <summary>
		/// 在 <paramref name="basicType"/> 类型中寻找能够将对象从 <paramref name="basicType"/> 类型转换为 
		/// <paramref name="outputType"/> 类型的用户自定义类型转换方法。
		/// 需要保证 <paramref name="basicType"/> 中存在用户自定义类型转换方法。
		/// </summary>
		/// <param name="basicType">要寻找自定义类型转换方法的类型。</param>
		/// <param name="outputType">要转换到的对象的类型。</param>
		/// <returns>将对象从 <paramref name="basicType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的用户自定义类型转换方法。如果不存在则为 <c>null</c>。</returns>
		public static MethodInfo GetConversionTo(Type basicType, Type outputType)
		{
			Contract.Requires(outputType != null && basicType != null);
			UserConversions convs = GetUserConversions(basicType);
			Contract.Assume(convs.ConvertToIndex >= 0);
			for (int i = convs.ConvertToIndex; i < convs.Methods.Length; i++)
			{
				UserConversionMethod method = convs.Methods[i];
				if (method.OutputType == outputType)
				{
					return method.Method;
				}
			}
			return null;
		}
		/// <summary>
		/// 返回的将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的自定义类型转换。这里不处理可空类型的转换，参数也不能是可空类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的自定义类型转换，如果不存在则为 <c>null</c>。</returns>
		public static MethodInfo GetConversion(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null &&
				!inputType.IsNullable() && !outputType.IsNullable());
			return new UserConversionFinder(inputType, outputType).FindConversion();
		}

		#region 查找用户自定义类型转换方法

		/// <summary>
		/// 用户自定义类型转换方法的查找器，可空类型的转换会在外部完成。
		/// </summary>
		private class UserConversionFinder
		{
			/// <summary>
			/// 输入类型。
			/// </summary>
			private readonly Type inputType;
			/// <summary>
			/// 输出类型。
			/// </summary>
			private readonly Type outputType;
			/// <summary>
			/// 最佳输入类型。
			/// </summary>
			private Type bestInputType;
			/// <summary>
			/// 输入类型的关系。
			/// </summary>
			private TypeRelation bestInputRelation = TypeRelation.Unknown;
			/// <summary>
			/// 最佳输出类型。
			/// </summary>
			private Type bestOutputType;
			/// <summary>
			/// 输出类型的关系。
			/// </summary>
			private TypeRelation bestOutputRelation = TypeRelation.Unknown;
			/// <summary>
			/// 找到的结果方法。
			/// </summary>
			private readonly UniqueValue<UserConversionMethod> bestMethod = new UniqueValue<UserConversionMethod>();
			/// <summary>
			/// 使用指定的输入类型和输出类型初始化 <see cref="UserConversionFinder"/> 类的新实例。
			/// </summary>
			/// <param name="inputType">输入类型。</param>
			/// <param name="outputType">输出类型。</param>
			public UserConversionFinder(Type inputType, Type outputType)
			{
				Contract.Requires(inputType != null && outputType != null);
				this.inputType = inputType;
				this.outputType = outputType;
			}
			/// <summary>
			/// 查找合适的用户自定义类型转换方法。
			/// </summary>
			/// <returns>合适的用户自定义类型转换方法，如果不存在则为 <c>null</c>。</returns>
			public MethodInfo FindConversion()
			{
				UserConversions convs = GetUserConversions(inputType);
				if (convs != null)
				{
					Contract.Assume(convs.ConvertToIndex >= 0);
					for (int i = convs.ConvertToIndex; i < convs.Methods.Length; i++)
					{
						UserConversionMethod method = convs.Methods[i];
						ConversionType ctype = ConversionFactory.GetStandardConversion(outputType, method.OutputType);
						if (ctype == ConversionType.None)
						{
							continue;
						}
						TypeRelation inputRelation = (method.InputType == inputType) ?
							TypeRelation.Best : TypeRelation.Second;
						TypeRelation outputRelation = TypeRelation.Best;
						if (ctype >= ConversionType.ExplicitNumericConversion)
						{
							outputRelation = TypeRelation.Second;
						}
						else if (ctype > ConversionType.IdentityConversion)
						{
							outputRelation = TypeRelation.Thirt;
						}
						GetBestConversion(method, inputRelation, outputRelation);
					}
				}
				convs = GetUserConversions(outputType);
				if (convs != null)
				{
					for (int i = 0; i < convs.ConvertToIndex; i++)
					{
						UserConversionMethod method = convs.Methods[i];
						ConversionType ctype = ConversionFactory.GetStandardConversion(method.InputType, inputType);
						if (ctype == ConversionType.None)
						{
							continue;
						}
						TypeRelation outputRelation = (method.OutputType == outputType) ?
							TypeRelation.Best : TypeRelation.Second;
						TypeRelation inputRelation = TypeRelation.Best;
						if (ctype >= ConversionType.ExplicitNumericConversion)
						{
							inputRelation = TypeRelation.Second;
						}
						else if (ctype > ConversionType.IdentityConversion)
						{
							inputRelation = TypeRelation.Thirt;
						}
						GetBestConversion(method, inputRelation, outputRelation);
					}
				}
				if (bestMethod.IsUnique)
				{
					return bestMethod.Value.Method;
				}
				if (bestMethod.IsAmbig)
				{
					throw CommonExceptions.AmbiguousUserDefinedConverter(inputType, outputType);
				}
				return null;
			}
			/// <summary>
			/// 查找最精确的输入类型和类型转换方法。
			/// </summary>
			/// <param name="method">类型转换方法。</param>
			/// <param name="inputRelation">实际输入类型到方法输入类型间的关系。</param>
			/// <param name="outputRelation">实际输出类型到方法输出类型间的关系。</param>
			private void GetBestConversion(UserConversionMethod method, TypeRelation inputRelation, TypeRelation outputRelation)
			{
				Contract.Requires(method != null);
				// 更新最优输入和输出类型。
				UpdateBestType(method.InputType, inputRelation, true, ref bestInputType, ref bestInputRelation);
				UpdateBestType(method.OutputType, outputRelation, false, ref bestOutputType, ref bestOutputRelation);
				if (method.InputType == bestInputType && method.OutputType == bestOutputType)
				{
					bestMethod.Value = method;
				}
			}
			/// <summary>
			/// 更新最优的输入/输出类型。
			/// </summary>
			/// <param name="methodType">类型转换方法的输入/输出类型。</param>
			/// <param name="relation">实际输入类型与方法输入/输出类型间的关系。</param>
			/// <param name="relationJudge">方法输入/输出类型和最优类型间的关系判定。</param>
			/// <param name="bestType">当前的最优类型。</param>
			/// <param name="bestRelation">当前最优类型与实际输入/输出类型间的关系。</param>
			private void UpdateBestType(Type methodType, TypeRelation relation, bool relationJudge,
				ref Type bestType, ref TypeRelation bestRelation)
			{
				// 当前选择更差。
				if (relation > bestRelation)
				{
					return;
				}
				// 当前选择更优，或者当前选择与最佳选择相同，但最佳选择没有类型填充。
				if (relation < bestRelation || bestType == null)
				{
					bestType = methodType;
					bestRelation = relation;
					bestMethod.Reset();
					return;
				}
				ConversionType ctype = ConversionFactory.GetStandardConversion(methodType, bestType);
				if (ctype == ConversionType.None)
				{
					// 找不到类型转换关系，令最佳选择前进至下一级别，并清除类型填充。
					bestType = null;
					bestRelation--;
					bestMethod.Reset();
					return;
				}
				if (ctype.IsImplicit() == relationJudge)
				{
					// 当前选择被最优选择包含。
					if (bestRelation == TypeRelation.Second)
					{
						bestType = methodType;
						bestMethod.Reset();
					}
				}
				else if (bestRelation == TypeRelation.Thirt)
				{
					// 当前选择包含最优选择。
					bestType = methodType;
					bestMethod.Reset();
				}
			}
			/// <summary>
			/// 表示类型与输入/输入类型的关系。
			/// </summary>
			private enum TypeRelation
			{
				/// <summary>
				/// 第一种情况，是最优类型，精确类型与输入/输入类型相同。
				/// </summary>
				Best,
				/// <summary>
				/// 第二种情况，是距离 S 最近的包含 S 的类型，或距离 T 最近的被 T 包含的类型。
				/// </summary>
				Second,
				/// <summary>
				/// 第三种情况，是距离 S 最近的被 S 包含的类型，或距离 T 最近的包含 T 的类型。
				/// </summary>
				Thirt,
				/// <summary>
				/// 未知关系。
				/// </summary>
				Unknown,
			}
		}

		#endregion // 查找用户自定义类型转换方法

		#region 类型转换方法的存储

		/// <summary>
		/// 表示用户定义的类型转换方法。
		/// </summary>
		[DebuggerDisplay("{InputType} -> {OutputType}")]
		private class UserConversionMethod
		{
			/// <summary>
			/// 类型转换方法。
			/// </summary>
			private readonly MethodInfo method;
			/// <summary>
			/// 类型转换方法的输入类型。
			/// </summary>
			private readonly Type inputType;
			/// <summary>
			/// 获取类型转换方法。
			/// </summary>
			/// <value>类型转换方法。</value>
			public MethodInfo Method
			{
				get { return this.method; }
			}
			/// <summary>
			/// 获取类型转换方法的输入类型，即方法的参数类型。
			/// </summary>
			/// <value>类型转换方法的输入类型，即方法的参数类型。</value>
			public Type InputType
			{
				get { return this.inputType; }
			}
			/// <summary>
			/// 获取类型转换方法的目标类型，即方法的返回值类型。
			/// </summary>
			/// <value>类型转换方法的目标类型，即方法的返回值类型。</value>
			public Type OutputType
			{
				get { return this.Method.ReturnType; }
			}
			/// <summary>
			/// 使用指定的类型转换方法初始化 <see cref="UserConversionMethod"/> 类的新实例。
			/// </summary>
			/// <param name="method">类型转换方法。</param>
			public UserConversionMethod(MethodInfo method)
			{
				Contract.Requires(method != null && method.GetParametersNoCopy().Length == 1);
				this.method = method;
				this.inputType = method.GetParametersNoCopy()[0].ParameterType;
			}
		}
		/// <summary>
		/// 表示某个类中的用户定义的类型转换方法。
		/// </summary>
		[DebuggerDisplay("Count = {Methods.Length}")]
		private class UserConversions
		{
			/// <summary>
			/// 类型转换方法列表。转换自方法总是存储在转换到方法之前。
			/// </summary>
			public readonly UserConversionMethod[] Methods;
			/// <summary>
			/// 转换到方法在列表中的起始索引。
			/// </summary>
			public readonly int ConvertToIndex;
			/// <summary>
			/// 使用指定的类型转换方法列表和索引初始化 <see cref="UserConversions"/> 类的新实例。
			/// </summary>
			/// <param name="methods">类型转换方法列表。</param>
			/// <param name="index">转换到方法在列表中的起始索引。</param>
			public UserConversions(UserConversionMethod[] methods, int index)
			{
				Contract.Requires(methods != null && index >= 0 && index < methods.Length);
				this.Methods = methods;
				this.ConvertToIndex = index;
			}
		}

		#endregion // 类型转换方法的存储

	}
}
