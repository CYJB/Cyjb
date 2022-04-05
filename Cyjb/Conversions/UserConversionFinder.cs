namespace Cyjb.Conversions
{
	/// <summary>
	/// 用户自定义类型转换方法的查找器，可空类型的转换会在外部完成。
	/// </summary>
	internal class UserConversionFinder
	{
		/// <summary>
		/// 返回的将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的自定义类型转换。这里不处理可空类型的转换，参数也不能是可空类型。
		/// </summary>
		/// <param name="inputType">要转换的对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的自定义类型转换，如果不存在则为 <c>null</c>。</returns>
		public static UserConversion? GetConversion(Type inputType, Type outputType)
		{
			return new UserConversionFinder(inputType, outputType).FindConversion();
		}

		/// <summary>
		/// 输入类型集合。
		/// </summary>
		private readonly HashSet<Type> inputTypes = new();
		/// <summary>
		/// 输入类型的关系。
		/// </summary>
		private TypeRelation inputRelation = TypeRelation.Unknown;
		/// <summary>
		/// 输出类型集合。
		/// </summary>
		private readonly HashSet<Type> outputTypes = new();
		/// <summary>
		/// 输出类型的关系。
		/// </summary>
		private TypeRelation outputRelation = TypeRelation.Unknown;
		/// <summary>
		/// 候选的类型转换列表。
		/// </summary>
		private readonly List<UserConversionMethod> candidates = new();

		/// <summary>
		/// 使用指定的输入类型和输出类型初始化 <see cref="UserConversionFinder"/> 类的新实例。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>
		private UserConversionFinder(Type inputType, Type outputType)
		{
			UserConversionCache? conversion = UserConversionCache.GetConversions(inputType);
			if (conversion != null)
			{
				ArraySegment<UserConversionMethod> methods = conversion.ConvertToMethods;
				for (int i = 0; i < methods.Count; i++)
				{
					UserConversionMethod method = methods[i];
					TypeRelation outputRelation = GetRelation(method.OutputType, outputType);
					if (outputRelation == TypeRelation.Unknown)
					{
						continue;
					}
					TypeRelation inputRelation = (method.InputType == inputType) ? TypeRelation.Exactly : TypeRelation.Implicit;
					AddCandicate(method, inputRelation, outputRelation);
				}
			}
			conversion = UserConversionCache.GetConversions(outputType);
			if (conversion != null)
			{
				ArraySegment<UserConversionMethod> methods = conversion.ConvertFromMethods;
				for (int i = 0; i < methods.Count; i++)
				{
					UserConversionMethod method = methods[i];
					TypeRelation inputRelation = GetRelation(inputType, method.InputType);
					if (inputRelation == TypeRelation.Unknown)
					{
						continue;
					}
					TypeRelation outputRelation = (method.OutputType == outputType) ? TypeRelation.Exactly : TypeRelation.Implicit;
					AddCandicate(method, inputRelation, outputRelation);
				}
			}
		}

		/// <summary>
		/// 返回类型间的关系。
		/// </summary>
		/// <param name="from">来源类型。</param>
		/// <param name="to">目标类型。</param>
		/// <returns>类型间的关系。</returns>
		private static TypeRelation GetRelation(Type from, Type to)
		{
			ConversionType type = ConversionFactory.GetStandardConversion(from, to);
			if (type == ConversionType.None)
			{
				return TypeRelation.Unknown;
			}
			else if (type == ConversionType.Identity)
			{
				return TypeRelation.Exactly;
			}
			else if (type.IsImplicit())
			{
				return TypeRelation.Implicit;
			}
			else
			{
				return TypeRelation.Explicit;
			}
		}

		/// <summary>
		/// 添加候选的类型转换。
		/// </summary>
		private void AddCandicate(UserConversionMethod method, TypeRelation inputRelation, TypeRelation outputRelation)
		{
			candidates.Add(method);
			inputTypes.Add(method.InputType);
			outputTypes.Add(method.OutputType);
			if (inputRelation > this.inputRelation)
			{
				this.inputRelation = inputRelation;
			}
			if (outputRelation > this.outputRelation)
			{
				this.outputRelation = outputRelation;
			}
		}

		/// <summary>
		/// 查找合适的用户自定义类型转换方法。
		/// </summary>
		/// <returns>合适的用户自定义类型转换方法，如果不存在则为 <c>null</c>。</returns>
		public UserConversion? FindConversion()
		{
			if (candidates.Count == 0)
			{
				return null;
			}
			// 查找最精确的源类型 Sx。
			Type? inputType;
			if (inputRelation == TypeRelation.Exactly)
			{
				inputType = inputTypes.First();
			}
			else if (inputRelation == TypeRelation.Implicit)
			{
				// Sx 是在合并源类型集中被包含程度最大的类型。
				inputType = TypeUtil.GetEncompassedType(inputTypes);
				if (inputType == null)
				{
					return null;
				}
			}
			else
			{
				// Sx 是在合并源类型集中包含程度最大的类型。
				inputType = TypeUtil.GetEncompassingType(inputTypes);
				if (inputType == null)
				{
					return null;
				}
			}
			// 查找最精确的目标类型 Tx
			Type? outputType;
			if (outputRelation == TypeRelation.Exactly)
			{
				outputType = outputTypes.First();
			}
			else if (outputRelation == TypeRelation.Implicit)
			{
				// Tx 是在合并源类型集中包含程度最大的类型。
				outputType = TypeUtil.GetEncompassingType(outputTypes);
				if (outputType == null)
				{
					return null;
				}
			}
			else
			{
				// Tx 是在合并源类型集中被包含程度最大的类型。
				outputType = TypeUtil.GetEncompassedType(outputTypes);
				if (outputType == null)
				{
					return null;
				}
			}
			// 查找最具体的转换运算符
			UserConversionMethod? bestMatch = null;
			for (int i = 0; i < candidates.Count; i++)
			{
				UserConversionMethod method = candidates[i];
				if (method.InputType == inputType && method.OutputType == outputType)
				{
					if (bestMatch == null)
					{
						bestMatch = method;
					}
					else
					{
						// 可能有多个运算符，转换不明确。
						return null;
					}
				}
			}
			if (bestMatch == null)
			{
				return null;
			}
			return new UserConversion(bestMatch.Method, outputRelation != TypeRelation.Explicit);
		}

		/// <summary>
		/// 表示类型与输入/输入类型的关系。
		/// </summary>
		private enum TypeRelation
		{
			/// <summary>
			/// 未知关系。
			/// </summary>
			Unknown,
			/// <summary>
			/// 第一种情况，是最优类型，精确类型与输入/输入类型相同。
			/// </summary>
			Exactly,
			/// <summary>
			/// 第二种情况，是包含 S 的类型，或被 T 包含的类型。
			/// </summary>
			Implicit,
			/// <summary>
			/// 第三种情况，是被S 包含的类型，或包含 T 的类型。
			/// </summary>
			Explicit,
		}
	}
}
