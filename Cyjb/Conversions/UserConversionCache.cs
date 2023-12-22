using System.Diagnostics;
using System.Reflection;
using Cyjb.Cache;
using Cyjb.Collections;
using Cyjb.Reflection;

namespace Cyjb.Conversions;

/// <summary>
/// 表示某个类中的用户定义的类型转换方法集合。
/// </summary>
[DebuggerDisplay("Count = {Methods.Length}")]
internal class UserConversionCache
{
	/// <summary>
	/// 用户自定义类型转换方法的缓存。
	/// </summary>
	private static readonly LruCache<Type, UserConversionCache?> conversions = new(256);

	/// <summary>
	/// 返回与指定类型相关的用户自定义类型转换方法。基类中声明的类型转换方法也包含在内。
	/// </summary>
	/// <param name="type">要获取类型转换方法的类型。</param>
	/// <returns>与指定类型相关的用户自定义类型转换方法，如果不存在则为 <c>null</c>。</returns>
	public static UserConversionCache? GetConversions(Type type)
	{
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
		return conversions.GetOrAdd(type, type =>
		{
			using ValueList<UserConversionMethod> list = new();
			int convertToIndex = 0;
			MethodInfo[] methods = type.GetMethods(BindingFlagsUtil.PublicStatic);
			for (int i = 0; i < methods.Length; i++)
			{
				MethodInfo method = methods[i];
				// 只处理隐式或显式类型转换方法。
				bool isImplicit = method.Name == "op_Implicit";
				if (isImplicit || method.Name == "op_Explicit")
				{
					UserConversionMethod convMethod = new(method, isImplicit);
					if (method.ReturnType == type)
					{
						list.Insert(convertToIndex, convMethod);
						convertToIndex++;
					}
					else
					{
						list.Add(convMethod);
					}
				}
			}
			// 基类包含的类型转换方法，子类也可以使用。
			if (type.IsClass && type.BaseType != null)
			{
				UserConversionCache? baseConversions = GetConversions(type.BaseType);
				if (baseConversions != null)
				{
					if (list.Length == 0)
					{
						return baseConversions;
					}
					int idx = baseConversions.convertToIndex;
					if (idx > 0)
					{
						list.Insert(convertToIndex, baseConversions.methods.AsSpan(0, idx));
						convertToIndex += idx;
					}
					if (idx < baseConversions.methods.Length)
					{
						list.Add(baseConversions.methods.AsSpan(idx));
					}
				}
			}
			if (list.Length == 0)
			{
				return null;
			}
			return new UserConversionCache(list.ToArray(), convertToIndex);
		});
	}

	/// <summary>
	/// 类型转换方法列表。
	/// </summary>
	private readonly UserConversionMethod[] methods;
	/// <summary>
	/// 转换到方法在列表中的起始索引。
	/// </summary>
	private readonly int convertToIndex;

	/// <summary>
	/// 使用指定的类型转换方法列表和索引初始化 <see cref="UserConversionCache"/> 类的新实例。
	/// </summary>
	/// <param name="methods">类型转换方法列表，转换自方法总是存储在转换到方法之前。</param>
	/// <param name="convertToIndex">转换到方法在列表中的起始索引。</param>
	private UserConversionCache(UserConversionMethod[] methods, int convertToIndex)
	{
		this.methods = methods;
		this.convertToIndex = convertToIndex;
	}

	/// <summary>
	/// 返回转换自其它类型的方法。
	/// </summary>
	public ArraySegment<UserConversionMethod> ConvertFromMethods => new(methods, 0, convertToIndex);

	/// <summary>
	/// 返回转换到其它类型的方法。
	/// </summary>
	public ArraySegment<UserConversionMethod> ConvertToMethods => new(methods, convertToIndex, methods.Length - convertToIndex);

	/// <summary>
	/// 寻找能够将对象从 <paramref name="inputType"/> 转换为当前类型的用户自定义类型转换方法。
	/// </summary>
	/// <param name="inputType">要转换的对象的类型。</param>
	/// <returns>将对象从 <paramref name="inputType"/> 转换为当前类型的用户自定义类型转换方法。
	/// 如果不存在则为 <c>null</c>。</returns>
	public MethodInfo? GetConversionFrom(Type inputType)
	{
		for (int i = 0; i < convertToIndex; i++)
		{
			UserConversionMethod method = methods[i];
			if (method.InputType == inputType)
			{
				return method.Method;
			}
		}
		return null;
	}

	/// <summary>
	/// 寻找能够将对象从当前类型转换为 <paramref name="outputType"/> 的用户自定义类型转换方法。
	/// </summary>
	/// <param name="outputType">要转换到的对象的类型。</param>
	/// <returns>将对象从当前类型转换为 <paramref name="outputType"/> 的用户自定义类型转换方法。
	/// 如果不存在则为 <c>null</c>。</returns>
	public MethodInfo? GetConversionTo(Type outputType)
	{
		for (int i = convertToIndex; i < methods.Length; i++)
		{
			UserConversionMethod method = methods[i];
			if (method.OutputType == outputType)
			{
				return method.Method;
			}
		}
		return null;
	}
}

