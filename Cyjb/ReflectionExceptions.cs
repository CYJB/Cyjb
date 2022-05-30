using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cyjb;

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
	/// 返回未能推导类型参数的异常。
	/// </summary>
	/// <param name="paramName">产生异常的参数名称。</param>
	/// <returns><see cref="ArgumentException"/> 对象。</returns>
	internal static ArgumentException CannotInferenceGenericArguments(string paramName)
	{
		return new ArgumentException(paramName, Resources.CannotInferenceGenericArguments);
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
	/// 返回类型包含泛型参数的异常。
	/// </summary>
	/// <param name="type">没有默认构造函数的类型。</param>
	/// <returns><see cref="ArgumentException"/> 对象。</returns>
	internal static ArgumentException TypeContainsGenericParameters(Type type)
	{
		return new ArgumentException(ResourcesUtil.Format(Resources.TypeContainsGenericParameters, type));
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
		ArgumentNullException.ThrowIfNull(member, paramName);
		Type? declaringType = member.DeclaringType;
		if (declaringType != null && declaringType.ContainsGenericParameters)
		{
			throw new ArgumentException(Resources.UnboundGenParam, paramName);
		}
	}
}
