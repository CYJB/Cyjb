using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Cyjb.Collections;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 方法的实参列表信息。
	/// </summary>
	internal class MethodArgumentsInfo
	{
		/// <summary>
		/// 返回方法的参数信息。
		/// </summary>
		/// <param name="method">要获取参数信息的方法。</param>
		/// <param name="types">方法实参类型数组，其长度必须大于等于方法的参数个数。
		/// 使用 <c>null</c> 表示无需进行类型检查，
		/// <see cref="TypeExt.ReferenceTypeMark"/> 表示引用类型标志。</param>
		/// <param name="options">方法参数信息的选项。</param>
		/// <returns>方法的参数信息。</returns>
		public static MethodArgumentsInfo GetInfo(MethodBase method, Type[] types, MethodArgumentsOption options)
		{
			Contract.Requires(method != null && types != null);
			MethodArgumentsInfo result = new MethodArgumentsInfo(method, types);
			bool isExplicit = options.HasFlag(MethodArgumentsOption.Explicit);
			// 填充方法实例。
			int offset = 0;
			if (options.HasFlag(MethodArgumentsOption.ContainsInstance))
			{
				if (!result.MarkInstanceType(isExplicit))
				{
					return null;
				}
				offset++;
			}
			// 填充 params 参数和可变参数。
			if (!result.FillParamArray(isExplicit))
			{
				return null;
			}
			// 检查实参是否与形参对应，未对应的参数是否包含默认值。
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			int paramLen = parameters.Length;
			if (result.ParamArrayType != null)
			{
				paramLen--;
				Contract.Assume(paramLen >= 0);
			}
			for (int i = 0, j = offset; i < paramLen; i++, j++)
			{
				if (!result.CheckParameter(parameters[i], types[j], options))
				{
					return null;
				}
			}
			result.fixedArguments = new ArrayAdapter<Type>(types, offset, paramLen);
			return result;
		}
		/// <summary>
		/// 方法信息。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly MethodBase method;
		/// <summary>
		/// 方法实参类型列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type[] arguments;
		/// <summary>
		/// 方法实例实参类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type instanceType;
		/// <summary>
		/// 方法的固定实参列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ArrayAdapter<Type> fixedArguments;
		/// <summary>
		/// params 形参的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type paramArrayType;
		/// <summary>
		/// params 实参的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<Type> paramArgumentTypes;
		/// <summary>
		/// 可变参数的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<Type> optionalArgumentTypes;
		/// <summary>
		/// 方法泛型参数类型的推断结果。
		/// </summary>
		public Type[] GenericArguments;
		/// <summary>
		/// 使用指定的方法信息和实参类型列表初始化 <see cref="MethodArgumentsInfo"/> 类的新实例。
		/// </summary>
		/// <param name="method">方法信息。</param>
		/// <param name="types">方法实参类型列表。</param>
		private MethodArgumentsInfo(MethodBase method, Type[] types)
		{
			Contract.Requires(method != null && types != null);
			this.method = method;
			this.arguments = types;
		}
		/// <summary>
		/// 获取方法实例实参类型。
		/// </summary>
		/// <value>方法实例实参类型。<c>null</c> 表示不是实例方法。</value>
		public Type InstanceType
		{
			get { return this.instanceType; }
		}
		/// <summary>
		/// 获取方法的固定实参列表。
		/// </summary>
		/// <value>方法的固定实参列表。如果 <see cref="ParamArrayType"/> 不为 <c>null</c>，
		/// 则不包含最后的 params 参数。</value>
		/// <remarks>列表元素为 <c>null</c> 表示使用参数默认值或空数组（对于 params 参数）；
		/// 为 <see cref="TypeExt.ReferenceTypeMark"/> 表示实参值是 <c>null</c>，仅具有引用类型的约束。</remarks>
		public IList<Type> FixedArguments
		{
			get { return this.fixedArguments; }
		}
		/// <summary>
		/// 获取 params 形参的类型。
		/// </summary>
		/// <value>params 形参的类型，如果为 <c>null</c> 表示无需特殊处理 params 参数。</value>
		public Type ParamArrayType
		{
			get { return this.paramArrayType; }
		}
		/// <summary>
		/// 获取 params 实参的类型列表。
		/// </summary>
		/// <value>params 实参的类型列表，如果为 <c>null</c> 表示无需特殊处理 params 参数。</value>
		/// <remarks>列表元素为 <see cref="TypeExt.ReferenceTypeMark"/> 表示实参值是 <c>null</c>，
		/// 仅具有引用类型的约束。</remarks>
		public IList<Type> ParamArgumentTypes
		{
			get { return this.paramArgumentTypes; }
		}
		/// <summary>
		/// 获取可变参数的类型。
		/// </summary>
		/// <value>可变参数的类型，如果为 <c>null</c> 表示没有可变参数。</value>
		/// <remarks>列表元素为 <see cref="TypeExt.ReferenceTypeMark"/> 表示实参值是 <c>null</c>，
		/// 仅具有引用类型的约束。</remarks>
		public IList<Type> OptionalArgumentTypes
		{
			get { return this.optionalArgumentTypes; }
		}
		/// <summary>
		/// 清除 params 参数信息，表示无需特殊处理该参数。
		/// </summary>
		public void ClearParamArrayType()
		{
			if (this.paramArrayType != null)
			{
				this.paramArrayType = null;
				this.paramArgumentTypes = null;
				this.fixedArguments = new ArrayAdapter<Type>(this.arguments, this.fixedArguments.Offset,
					this.fixedArguments.Count + 1);
			}
		}

		#region 获取方法参数类型

		/// <summary>
		/// 将实参中的第一个参数作为方法的实例。
		/// </summary>
		/// <param name="isExplicit">类型检查时，使用显式类型转换，而不是默认的隐式类型转换。</param>
		/// <returns>如果第一个参数可以作为方法的实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool MarkInstanceType(bool isExplicit)
		{
			if (method.IsStatic)
			{
				return false;
			}
			if (this.arguments.Length == 0)
			{
				return false;
			}
			this.instanceType = this.arguments[0];
			if (this.instanceType == null)
			{
				return false;
			}
			if (this.instanceType == TypeExt.ReferenceTypeMark)
			{
				this.instanceType = this.method.DeclaringType;
				Contract.Assume(this.instanceType != null);
				return !this.instanceType.IsValueType;
			}
			return method.DeclaringType.IsConvertFrom(this.instanceType, isExplicit);
		}
		/// <summary>
		/// 填充 params 参数和可变参数。
		/// </summary>
		/// <param name="isExplicit">类型检查时，使用显式类型转换，而不是默认的隐式类型转换。</param>
		/// <returns>如果第一个参数可以作为方法的实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <returns>如果填充参数成功，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool FillParamArray(bool isExplicit)
		{
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			int paramLen = parameters.Length;
			int offset = this.instanceType == null ? 0 : 1;
			offset += paramLen;
			if (method.CallingConvention.HasFlag(CallingConventions.VarArgs))
			{
				this.optionalArgumentTypes = new ArrayAdapter<Type>(this.arguments, offset);
				return this.optionalArgumentTypes.All(type => type != null);
			}
			if (paramLen > 0)
			{
				ParameterInfo lastParam = parameters[paramLen - 1];
				if (lastParam.IsParamArray())
				{
					this.paramArrayType = lastParam.ParameterType;
					this.paramArgumentTypes = new ArrayAdapter<Type>(this.arguments, offset - 1);
					return lastParam.ParameterType.ContainsGenericParameters || CheckParamArrayType(isExplicit);
				}
			}
			// 检测方法实参数量。
			return arguments.Length <= offset;
		}
		/// <summary>
		/// 检查 params 参数类型。
		/// </summary>
		/// <param name="isExplicit">类型检查时，如果考虑显式类型转换，则为 <c>true</c>；
		/// 否则只考虑隐式类型转换。</param>
		/// <returns>如果 params 参数类型匹配，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckParamArrayType(bool isExplicit)
		{
			int paramCnt = this.paramArgumentTypes.Count;
			if (paramCnt == 0)
			{
				return true;
			}
			Type paramElementType = this.paramArrayType.GetElementType();
			if (paramCnt == 1)
			{
				// 只有一个实参，可能是数组或数组元素。
				Type type = this.paramArgumentTypes[0];
				if (type == null || type == TypeExt.ReferenceTypeMark ||
					paramArrayType.IsConvertFrom(type, isExplicit))
				{
					// 实参是数组，无需进行特殊处理。
					this.paramArrayType = null;
					this.paramArgumentTypes = null;
					return true;
				}
				return paramElementType.IsConvertFrom(type, isExplicit);
			}
			// 有多个实参，必须是数组元素。
			for (int i = 0; i < paramCnt; i++)
			{
				Type type = this.paramArgumentTypes[i];
				if (type == TypeExt.ReferenceTypeMark)
				{
					if (paramElementType.IsValueType)
					{
						return false;
					}
				}
				else if (type == null || !paramElementType.IsConvertFrom(type, isExplicit))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 检查方法参数。
		/// </summary>
		/// <param name="parameter">要检查的方法参数。</param>
		/// <param name="type">方法实参类型。</param>
		/// <param name="options">方法推断选项。</param>
		/// <returns>如果方法参数与实参兼容，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckParameter(ParameterInfo parameter, Type type, MethodArgumentsOption options)
		{
			Type paramType = parameter.ParameterType;
			if (paramType.ContainsGenericParameters)
			{
				return true;
			}
			if (type == null)
			{
				// 检查可选参数和 params 参数。
				return parameter.IsParamArray() ||
					(options.HasFlag(MethodArgumentsOption.OptionalParamBinding) && parameter.HasDefaultValue);
			}
			bool byRef = false;
			if (paramType.IsByRef)
			{
				paramType = paramType.GetElementType();
				byRef = true;
			}
			if (type == TypeExt.ReferenceTypeMark)
			{
				// 检查引用类型。
				return !paramType.IsValueType;
			}
			if (type.IsByRef)
			{
				if (byRef)
				{
					return type.GetElementType() == paramType;
				}
				type = type.GetElementType();
			}
			return paramType.IsConvertFrom(type, options.HasFlag(MethodArgumentsOption.Explicit));
		}

		#endregion // 获取方法参数类型

	}
	/// <summary>
	/// 方法实参的选项。
	/// </summary>
	[Flags]
	public enum MethodArgumentsOption
	{
		/// <summary>
		/// 无任何选项。
		/// </summary>
		None = 0,
		/// <summary>
		/// 方法的第一个实参表示方法实例。
		/// </summary>
		ContainsInstance = 1,
		/// <summary>
		/// 对可选参数进行绑定。
		/// </summary>
		OptionalParamBinding = 2,
		/// <summary>
		/// 类型检查时，使用显式类型转换，而不是默认的隐式类型转换。
		/// </summary>
		Explicit = 4,
		/// <summary>
		/// 对可选参数进行绑定，且使用显式类型转换。
		/// </summary>
		OptionalAndExplicit = 6,
	}
}