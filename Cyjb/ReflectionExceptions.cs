using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cyjb
{
	/// <summary>
	/// 提供反射相关异常。
	/// </summary>
	internal static class ReflectionExceptions
	{
		/// <summary>
		/// 返回代码不应到达这里的异常。
		/// </summary>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		internal static InvalidOperationException Unreachable()
		{
			return new InvalidOperationException("Code supposed to be unreachable.");
		}

		/// <summary>
		/// 返回找到多个与绑定约束匹配的字段的异常。
		/// </summary>
		/// <returns><see cref="AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchField()
		{
			return new AmbiguousMatchException(Resources.AmbiguousMatchField);
		}

		/// <summary>
		/// 返回找到多个与绑定约束匹配的方法的异常。
		/// </summary>
		/// <returns><see cref="AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchMethod()
		{
			return new AmbiguousMatchException(Resources.AmbiguousMatchMethod);
		}

		/// <summary>
		/// 返回找到多个与绑定约束匹配的属性的异常。
		/// </summary>
		/// <returns><see cref="AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchProperty()
		{
			return new AmbiguousMatchException(Resources.AmbiguousMatchProperty);
		}

		/// <summary>
		/// 返回不能绑定到开放构造方法的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindOpenConstructedMethod(string paramName)
		{
			return new ArgumentException(Resources.BindOpenConstructedMethod, paramName);
		}

		/// <summary>
		/// 返回绑定到目标字段出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetField(string paramName)
		{
			return new ArgumentException(Resources.BindTargetField, paramName);
		}

		/// <summary>
		/// 返回绑定到目标方法出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetMethod(string paramName)
		{
			return new ArgumentException(Resources.BindTargetMethod, paramName);
		}

		/// <summary>
		/// 返回绑定到目标属性出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetProperty(string paramName)
		{
			return new ArgumentException(Resources.BindTargetProperty, paramName);
		}

		/// <summary>
		/// 返回绑定到目标属性出错，不存在 get 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoGet(string paramName)
		{
			return new ArgumentException(Resources.BindTargetPropertyNoGet, paramName);
		}

		/// <summary>
		/// 返回绑定到目标属性出错，不存在 set 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoSet(string paramName)
		{
			return new ArgumentException(Resources.BindTargetPropertyNoSet, paramName);
		}

		/// <summary>
		/// 返回未能推导类型参数的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException CannotInferenceGenericArguments(string paramName)
		{
			return new ArgumentException(paramName, Resources.CannotInferenceGenericArguments);
		}

		/// <summary>
		/// 返回委托类型不兼容的异常。
		/// </summary>
		/// <param name="sourceDlg">源委托。</param>
		/// <param name="targetDlg">需要兼容的目标委托。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException DelegateCompatible(Type sourceDlg, Type targetDlg)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.DelegateIncompatible, sourceDlg, targetDlg));
		}

		/// <summary>
		/// 返回存在相同的参数名称的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException DuplicateName(string paramName)
		{
			return new ArgumentException(Resources.DuplicateName, paramName);
		}

		/// <summary>
		/// 返回访问的字段不存在的异常。
		/// </summary>
		/// <returns><see cref="MissingFieldException"/> 对象。</returns>
		internal static MissingFieldException MissingField()
		{
			return new MissingFieldException();
		}

		/// <summary>
		/// 返回访问的方法不存在的异常。
		/// </summary>
		/// <returns><see cref="MissingMethodException"/> 对象。</returns>
		internal static MissingMethodException MissingMethod()
		{
			return new MissingMethodException();
		}

		/// <summary>
		/// 返回命名参数数组太长的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="MissingMethodException"/> 对象。</returns>
		internal static ArgumentException NamedParamTooBig(string paramName)
		{
			return new ArgumentException(Resources.NamedParamTooBig, paramName);
		}

		/// <summary>
		/// 返回不表示泛型方法定义的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		internal static InvalidOperationException NeedGenericMethodDefinition(string paramName)
		{
			return new InvalidOperationException(ResourcesUtil.Format(Resources.NeedGenericMethodDefinition, paramName));
		}

		/// <summary>
		/// 返回属性不存在 get 访问器的异常。
		/// </summary>
		/// <param name="paramName">参数的名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException PropertyNoGetter(string paramName)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.PropertyNoGetter, paramName));
		}

		/// <summary>
		/// 返回属性不存在 set 访问器的异常。
		/// </summary>
		/// <param name="paramName">参数的名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException PropertyNoSetter(string paramName)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.PropertyNoSetter, paramName));
		}

		/// <summary>
		/// 返回找不到属性或字段的异常。
		/// </summary>
		/// <param name="memberName">成员名称。</param>
		/// <param name="nonPublic">是否搜索非公共成员。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException PropertyOrFieldNotFound(string memberName, bool nonPublic)
		{
			string message = nonPublic ? Resources.PropertyOrFieldNotFound_NonPublic : Resources.PropertyOrFieldNotFound;
			return new ArgumentException(ResourcesUtil.Format(message, memberName));
		}

		/// <summary>
		/// 返回类型包含泛型参数的异常。
		/// </summary>
		/// <param name="type">没有默认构造函数的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException TypeContainsGenericParameters(Type type)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.TypeContainsGenericParameters, type));
		}

		/// <summary>
		/// 返回找不到类型成员的异常。
		/// </summary>
		/// <param name="memberName">成员名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException TypeMemberNotFound(string memberName)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.TypeMemberNotFound, memberName));
		}

		/// <summary>
		/// 返回类型不包含默认构造函数的异常。
		/// </summary>
		/// <param name="type">没有默认构造函数的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException TypeMissingDefaultConstructor(Type type)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.TypeMissingDefaultConstructor, type));
		}

		/// <summary>
		/// 返回不能对包含未赋值的泛型类型参数的类型和方法进行后期绑定的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException UnboundGenParam(string paramName)
		{
			return new ArgumentException(Resources.UnboundGenParam, paramName);
		}

		/// <summary>
		/// 检查指定的类型成员，如果所属类型包含未赋值的泛型类型参数则抛出相应异常。
		/// </summary>
		/// <param name="member">要检查所属类型的类型成员。</param>
		/// <param name="paramName">产生异常的参数名称。</param>
		internal static void CheckUnboundGenParam(MemberInfo member,
			[CallerArgumentExpression("member")] string? paramName = null)
		{
			CommonExceptions.CheckArgumentNull(member, paramName);
			Type? declaringType = member.DeclaringType;
			if (declaringType != null && declaringType.ContainsGenericParameters)
			{
				throw new ArgumentException(Resources.UnboundGenParam, paramName);
			}
		}

		/// <summary>
		/// 返回找到多个用户自定义类型转换的异常。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		internal static InvalidOperationException AmbiguousUserDefinedConverter(Type inputType, Type outputType)
		{
			return new InvalidOperationException(ResourcesUtil.Format(Resources.AmbiguousUserDefinedConverter, inputType, outputType));
		}
	}
}
