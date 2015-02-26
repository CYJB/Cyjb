using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 泛型形参的界限集集合。
	/// </summary>
	internal class TypeBounds
	{
		/// <summary>
		/// 泛型形参及其界限集。
		/// </summary>
		private readonly BoundSet[] boundSets;
		/// <summary>
		/// 泛型形参的界限集字典。
		/// </summary>
		private readonly IDictionary<Type, BoundSet> boundSetDict;
		/// <summary>
		/// 使用指定的泛型形参初始化 <see cref="TypeBounds"/> 类的新实例。
		/// </summary>
		/// <param name="genericArguments">泛型形参列表。</param>
		public TypeBounds(params Type[] genericArguments)
		{
			Contract.Requires(genericArguments != null);
			int len = genericArguments.Length;
			this.boundSets = new BoundSet[len];
			this.boundSetDict = new Dictionary<Type, BoundSet>(len);
			for (int i = 0; i < genericArguments.Length; i++)
			{
				this.boundSets[i] = new BoundSet(genericArguments[i]);
				this.boundSetDict.Add(genericArguments[i], this.boundSets[i]);
			}
		}
		/// <summary>
		/// 使用要复制的泛型形参的界限集集合初始化 <see cref="TypeBounds"/> 类的新实例。
		/// </summary>
		/// <param name="typeBounds">泛型形参的界限集集合。</param>
		public TypeBounds(TypeBounds typeBounds)
		{
			Contract.Requires(typeBounds != null);
			int len = typeBounds.boundSets.Length;
			this.boundSets = new BoundSet[len];
			this.boundSetDict = new Dictionary<Type, BoundSet>(len);
			for (int i = 0; i < typeBounds.boundSets.Length; i++)
			{
				this.boundSets[i] = new BoundSet(typeBounds.boundSets[i]);
				this.boundSetDict.Add(this.boundSets[i].GenericArgument, this.boundSets[i]);
			}
		}
		/// <summary>
		/// 固定当前界限集的泛型类型参数。
		/// </summary>
		/// <returns>如果成功推断泛型参数组的类型参数，则为类型参数数组；
		/// 如果推断失败，则为 <c>null</c>。</returns>
		public Type[] FixTypeArguments()
		{
			int len = this.boundSets.Length;
			Type[] result = new Type[len];
			for (int i = 0; i < len; i++)
			{
				result[i] = this.boundSets[i].FixTypeArg();
				if (result[i] == null)
				{
					return null;
				}
			}
			return result;
		}
		/// <summary>
		/// 对类型进行推断，使用下限推断。
		/// </summary>
		/// <param name="paramType">形参类型，必须包含泛型参数。</param>
		/// <param name="type">实参类型，允许使用 <see cref="TypeExt.ReferenceTypeMark"/> 表示引用类型约束。</param>
		/// <returns>如果类型推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool TypeInferences(Type paramType, Type type)
		{
			return TypeInferences(paramType, type, false);
		}
		/// <summary>
		/// 对类型进行推断。
		/// </summary>
		/// <param name="paramType">形参类型，必须包含泛型参数。</param>
		/// <param name="type">实参类型，允许使用 <see cref="TypeExt.ReferenceTypeMark"/> 表示引用类型约束。</param>
		/// <param name="isUpperBound">是否进行上限推断，而不是默认的下限推断。</param>
		/// <returns>如果类型推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool TypeInferences(Type paramType, Type type, bool isUpperBound)
		{
			Contract.Requires(paramType != null && type != null);
			if (type == TypeExt.ReferenceTypeMark)
			{
				// 引用类型约束。
				if (paramType.IsByRef)
				{
					paramType = paramType.GetElementType();
				}
				if (paramType.IsGenericParameter)
				{
					boundSetDict[paramType].ReferenceType = true;
				}
				return true;
			}
			if (paramType.IsByRef)
			{
				return ExactInferences(paramType.GetElementType(), type);
			}
			return isUpperBound ? UpperBoundInferences(paramType, type) : LowerBoundInferences(paramType, type);
		}
		/// <summary>
		/// 对类型进行精确推断。
		/// </summary>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <returns>如果精确推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool ExactInferences(Type paramType, Type type)
		{
			Contract.Requires(paramType != null && type != null);
			if (paramType.IsGenericParameter)
			{
				return boundSetDict[paramType].AddExactBound(type);
			}
			if (paramType.IsArray)
			{
				return type.IsArray && paramType.GetArrayRank() == type.GetArrayRank() &&
					ExactInferences(paramType.GetElementType(), type.GetElementType());
			}
			Type paramUnderlyingType = Nullable.GetUnderlyingType(paramType);
			if (paramUnderlyingType != null)
			{
				type = Nullable.GetUnderlyingType(type);
				return type != null && ExactInferences(paramUnderlyingType, type);
			}
			if (paramType.GetGenericTypeDefinition() != type.GetGenericTypeDefinition())
			{
				return false;
			}
			Type[] paramTypeArgs = paramType.GetGenericArguments();
			Type[] typeArgs = type.GetGenericArguments();
			for (int i = 0; i < paramTypeArgs.Length; i++)
			{
				if (!ExactInferences(paramTypeArgs[i], typeArgs[i]))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 对类型进行下限推断。
		/// </summary>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <returns>如果下限推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool LowerBoundInferences(Type paramType, Type type)
		{
			Contract.Requires(paramType != null && type != null);
			if (paramType.IsGenericParameter)
			{
				return boundSetDict[paramType].AddLowerBound(type);
			}
			Type paramUnderlyingType = Nullable.GetUnderlyingType(paramType);
			if (paramUnderlyingType != null)
			{
				type = Nullable.GetUnderlyingType(type);
				return type != null && LowerBoundInferences(paramUnderlyingType, type);
			}
			if (paramType.IsArray)
			{
				if (!type.IsArray || paramType.GetArrayRank() != type.GetArrayRank())
				{
					return false;
				}
				paramType = paramType.GetElementType();
				type = type.GetElementType();
				return IsReferenceType(type) ? LowerBoundInferences(paramType, type) :
					ExactInferences(paramType, type);
			}
			Type paramDefinition = paramType.GetGenericTypeDefinition();
			if (paramDefinition.IsIListOrBase())
			{
				if (type.IsArray && type.GetArrayRank() == 1)
				{
					paramType = paramType.GetGenericArguments()[0];
					type = type.GetElementType();
					return IsReferenceType(type) ? LowerBoundInferences(paramType, type) :
						ExactInferences(paramType, type);
				}
			}
			Type tempType = paramDefinition.UniqueCloseDefinitionFrom(type);
			if (tempType == null)
			{
				return false;
			}
			Type[] originArgs = paramDefinition.GetGenericArguments();
			Type[] paramTypeArgs = paramType.GetGenericArguments();
			Type[] typeArgs = tempType.GetGenericArguments();
			for (int i = 0; i < originArgs.Length; i++)
			{
				if (!GenericArgumentInferences(originArgs[i].GenericParameterAttributes,
					paramTypeArgs[i], typeArgs[i], GenericParameterAttributes.Covariant))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 对类型进行上限推断。
		/// </summary>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <returns>如果上限推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool UpperBoundInferences(Type paramType, Type type)
		{
			Contract.Requires(paramType != null && type != null);
			if (paramType.IsGenericParameter)
			{
				return boundSetDict[paramType].AddUpperBound(type);
			}
			Type paramUnderlyingType = Nullable.GetUnderlyingType(paramType);
			if (paramUnderlyingType != null)
			{
				type = Nullable.GetUnderlyingType(type);
				return type != null && UpperBoundInferences(paramUnderlyingType, type);
			}
			if (paramType.IsArray)
			{
				if (type.IsArray)
				{
					if (paramType.GetArrayRank() != type.GetArrayRank())
					{
						return false;
					}
					type = type.GetElementType();
				}
				else if (paramType.GetArrayRank() == 1 && type.GetGenericTypeDefinition().IsIListOrBase())
				{
					type = type.GetGenericArguments()[0];
				}
				else
				{
					return false;
				}
				paramType = paramType.GetElementType();
				return IsReferenceType(type) ? UpperBoundInferences(paramType, type) :
					ExactInferences(paramType, type);
			}
			if (!type.IsGenericType)
			{
				return false;
			}
			Type paramDefinition = type.GetGenericTypeDefinition();
			Type tempType = paramDefinition.UniqueCloseDefinitionFrom(paramType);
			if (tempType == null)
			{
				return false;
			}
			Type[] originArgs = paramDefinition.GetGenericArguments();
			Type[] paramTypeArgs = tempType.GetGenericArguments();
			Type[] typeArgs = type.GetGenericArguments();
			for (int i = 0; i < originArgs.Length; i++)
			{
				if (!GenericArgumentInferences(originArgs[i].GenericParameterAttributes,
					paramTypeArgs[i], typeArgs[i], GenericParameterAttributes.Contravariant))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 判断指定的类型是否是引用类型。
		/// </summary>
		/// <param name="type">要检查的类型。</param>
		/// <returns>如果能确定 <paramref name="type"/> 是引用类型，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		private static bool IsReferenceType(Type type)
		{
			Contract.Requires(type != null);
			if (type.IsGenericParameter)
			{
				return type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint);
			}
			return !type.IsValueType;
		}
		/// <summary>
		/// 对泛型参数进行类型推断。
		/// </summary>
		/// <param name="attr">泛型参数的约束。</param>
		/// <param name="paramType">形参类型。</param>
		/// <param name="type">实参类型。</param>
		/// <param name="lowerBoundAttr">要进行下限推断的泛型参数约束。</param>
		/// <returns>如果泛型参数的类型推断成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool GenericArgumentInferences(GenericParameterAttributes attr, Type paramType, Type type,
			GenericParameterAttributes lowerBoundAttr)
		{
			Contract.Requires(paramType != null && type != null);
			attr &= GenericParameterAttributes.VarianceMask;
			if (attr == GenericParameterAttributes.None || !IsReferenceType(type))
			{
				return ExactInferences(paramType, type);
			}
			if (attr == lowerBoundAttr)
			{
				return LowerBoundInferences(paramType, type);
			}
			return UpperBoundInferences(paramType, type);
		}
		/// <summary>
		/// 泛型形参的界限集。
		/// </summary>
		private class BoundSet
		{
			/// <summary>
			/// 界限集对应的泛型参数。
			/// </summary>
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly Type genericArgument;
			/// <summary>
			/// 类型推断的下限界限集。
			/// </summary>
			private readonly HashSet<Type> lowerBounds = new HashSet<Type>();
			/// <summary>
			/// 类型推断的上限界限集。
			/// </summary>
			private readonly HashSet<Type> upperBounds = new HashSet<Type>();
			/// <summary>
			/// 类型推断的精确界限（这个需要是唯一的）。
			/// </summary>
			private Type exactBound;
			/// <summary>
			/// 是否要求类型形参必须是泛型类型。
			/// </summary>
			public bool ReferenceType;
			/// <summary>
			/// 使用指定的泛型参数初始化 <see cref="BoundSet"/> 类的新实例。
			/// </summary>
			/// <param name="genericArgument">泛型形参。</param>
			public BoundSet(Type genericArgument)
			{
				Contract.Requires(genericArgument != null);
				this.genericArgument = genericArgument;
			}
			/// <summary>
			/// 使用指定的界限集初始化 <see cref="BoundSet"/> 类的新实例。
			/// </summary>
			/// <param name="bound">界限集。</param>
			public BoundSet(BoundSet bound)
			{
				Contract.Requires(bound != null);
				this.genericArgument = bound.genericArgument;
				this.ReferenceType = bound.ReferenceType;
				if (bound.exactBound == null)
				{
					this.lowerBounds.UnionWith(bound.lowerBounds);
					this.upperBounds.UnionWith(bound.upperBounds);
				}
				else
				{
					this.exactBound = bound.exactBound;
				}
			}
			/// <summary>
			/// 获取界限集对应的泛型参数。
			/// </summary>
			/// <value>界限集对应的泛型参数。</value>
			public Type GenericArgument
			{
				get { return this.genericArgument; }
			}
			/// <summary>
			/// 向类型推断的精确界限集中添加指定的类型。
			/// </summary>
			/// <param name="type">要添加的类型。</param>
			/// <returns>如果添加成功，则为 <c>true</c>；如果产生了冲突，则为 <c>false</c>。</returns>
			public bool AddExactBound(Type type)
			{
				Contract.Requires(type != null);
				if (exactBound != null)
				{
					return exactBound == type;
				}
				if (!this.CanFixed(type))
				{
					// 与现有的界限冲突。
					return false;
				}
				lowerBounds.Clear();
				upperBounds.Clear();
				exactBound = type;
				return true;
			}
			/// <summary>
			/// 向类型推断的下限界限集中添加指定的类型。
			/// </summary>
			/// <param name="type">要添加的类型。</param>
			/// <returns>如果添加成功，则为 <c>true</c>；如果产生了冲突，则为 <c>false</c>。</returns>
			public bool AddLowerBound(Type type)
			{
				Contract.Requires(type != null);
				if (exactBound != null)
				{
					// 判断是否与精确界限冲突。
					return exactBound.IsImplicitFrom(type);
				}
				lowerBounds.Add(type);
				return true;
			}
			/// <summary>
			/// 向类型推断的上限界限集中添加指定的类型。
			/// </summary>
			/// <param name="type">要添加的类型。</param>
			/// <returns>如果添加成功，则为 <c>true</c>；如果产生了冲突，则为 <c>false</c>。</returns>
			public bool AddUpperBound(Type type)
			{
				Contract.Requires(type != null);
				if (exactBound != null)
				{
					// 判断是否与精确界限冲突。
					return type.IsImplicitFrom(exactBound);
				}
				upperBounds.Add(type);
				return true;
			}
			/// <summary>
			/// 固定当前界限集所限定的类型参数。
			/// </summary>
			/// <returns>如果成功固定当前界限集的的类型参数，则为类型参数；
			/// 如果固定失败，则为 <c>null</c>。</returns>
			public Type FixTypeArg()
			{
				Type result;
				if (exactBound == null)
				{
					HashSet<Type> types = new HashSet<Type>(lowerBounds);
					types.UnionWith(upperBounds);
					types.RemoveWhere(type => !this.CanFixed(type));
					if (types.Count == 0)
					{
						// 没有找到合适的推断结果。
						return null;
					}
					if (types.Count == 1)
					{
						// 找到唯一的推断结果。
						result = types.First();
					}
					else
					{
						// 进一步进行推断。
						result = TypeExt.GetEncompassingType(types);
						if (result == null)
						{
							return null;
						}
					}
				}
				else
				{
					result = exactBound;
				}
				// 判断引用类型约束。
				if (ReferenceType && result.IsValueType)
				{
					return null;
				}
				return result;
			}
			/// <summary>
			/// 判断指定的类型能否根据给定的上限集和下限集中被固定。
			/// </summary>
			/// <param name="type">要判断能否固定的类型。</param>
			/// <returns>如果给定的类型可以固定，则为 <c>true</c>；否则为 <c>false</c>。</returns>
			private bool CanFixed(Type type)
			{
				Contract.Requires(type != null);
				return lowerBounds.All(type.IsImplicitFrom) &&
					upperBounds.All(boundType => boundType.IsImplicitFrom(type));
			}
		}
	}
}
