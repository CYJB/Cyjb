using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 寻找指定类型中指定名称的成员。
	/// </summary>
	internal class MemberFinder
	{
		/// <summary>
		/// 查找字段。
		/// </summary>
		private const BindingFlags FindFieldFlags = BindingFlags.GetField | BindingFlags.SetField;
		/// <summary>
		/// 查找属性。
		/// </summary>
		private const BindingFlags FindPropertyFlags = BindingFlags.GetProperty | BindingFlags.SetProperty;
		/// <summary>
		/// 查找所有成员。
		/// </summary>
		private const BindingFlags FindAllMemberFlags = FindFieldFlags | FindPropertyFlags | BindingFlags.InvokeMethod |
			BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding;
		/// <summary>
		/// 查找范围。
		/// </summary>
		private const BindingFlags FindScopeFlags = BindingFlags.Static | BindingFlags.Instance;
		/// <summary>
		/// 查找访问性。
		/// </summary>
		private const BindingFlags FindAccessFlags = BindingFlags.Public | BindingFlags.NonPublic;

		/// <summary>
		/// 要从中查找成员的 <see cref="Type"/>。
		/// </summary>
		private readonly Type type;
		/// <summary>
		/// 要查找的成员名称。
		/// </summary>
		private readonly string name;
		/// <summary>
		/// 搜索方式。
		/// </summary>
		private readonly BindingFlags bindingFlags;
		/// <summary>
		/// 静态成员的搜索方式。
		/// </summary>
		private readonly BindingFlags staticBindingAttr;
		/// <summary>
		/// 实例成员的搜索方式。
		/// </summary>
		private readonly BindingFlags instanceBindingAttr;
		/// <summary>
		/// 需要的返回值类型。
		/// </summary>
		private readonly Type returnType;
		/// <summary>
		/// 原始参数类型数组。
		/// </summary>
		private readonly Type[] paramTypes;
		/// <summary>
		/// 参数类型数组，用于搜索静态成员。
		/// </summary>
		private readonly Type[]? staticParamTypes;
		/// <summary>
		/// 参数类型数组，用于搜索实例成员。
		/// </summary>
		private readonly Type[]? instanceParamTypes;
		/// <summary>
		/// 绑定器。
		/// </summary>
		private readonly Binder? binder;

		/// <summary>
		/// 使用指定的名称、搜索方式和参数类型，创建 <see cref="MemberFinder"/> 类的新实例。
		/// </summary>
		/// <param name="type">要从中查找成员的 <see cref="Type"/>。</param>
		/// <param name="name">要查找的成员名称。</param> 
		/// <param name="bindingFlags">一个位屏蔽，由一个或多个指定搜索执行方式的 <see cref="BindingFlags"/> 组成。</param>
		/// <param name="returnType">需要的返回值类型。</param>
		/// <param name="paramTypes">参数类型数组，用于搜索实例成员。</param>
		public MemberFinder(Type type, string name, BindingFlags bindingFlags, Type returnType, Type[] paramTypes)
		{
			this.type = type;
			this.name = name;
			this.returnType = returnType;
			this.paramTypes = paramTypes;
			// 处理绑定标记的默认值。
			if ((bindingFlags & FindAllMemberFlags) == BindingFlags.Default)
			{
				bindingFlags |= FindAllMemberFlags;
			}
			if ((bindingFlags & FindAccessFlags) == BindingFlags.Default)
			{
				bindingFlags |= FindAccessFlags;
			}
			if ((bindingFlags & FindScopeFlags) == BindingFlags.Default)
			{
				bindingFlags |= FindScopeFlags;
			}
			// 支持可变参数的绑定。
			bindingFlags |= BindingFlagsUtil.VarArgsParamBinding;
			this.bindingFlags = bindingFlags;
			bindingFlags &= ~FindScopeFlags;
			if (this.bindingFlags.HasFlag(BindingFlags.Static))
			{
				staticParamTypes = paramTypes;
				staticBindingAttr = bindingFlags | BindingFlags.Static;
			}
			if (this.bindingFlags.HasFlag(BindingFlags.Instance) && paramTypes.Length > 0 &&
				(paramTypes[0] == null || type.IsExplicitFrom(paramTypes[0])))
			{
				instanceParamTypes = paramTypes[1..^0];
				instanceBindingAttr = bindingFlags | BindingFlags.Instance;
			}
			if (!this.bindingFlags.HasFlag(BindingFlags.ExactBinding))
			{
				binder = PowerBinder.Explicit;
			}
		}

		/// <summary>
		/// 搜索构造函数。
		/// </summary>
		/// <returns>搜索得到的构造函数，找不到则为 <c>null</c>。</returns>
		public ConstructorInfo? FindConstructor()
		{
			if (!bindingFlags.HasFlag(BindingFlags.CreateInstance) || name != TypeUtil.ConstructorName)
			{
				return null;
			}
			return type.GetConstructor(bindingFlags, binder, paramTypes, null);
		}

		/// <summary>
		/// 搜索方法。
		/// </summary>
		/// <returns>搜索得到的方法，找不到则为 <c>null</c>。</returns>
		public MethodInfo? FindMethod()
		{
			if (bindingFlags.HasFlag(BindingFlags.InvokeMethod))
			{
				// 查找静态方法。
				if (staticParamTypes != null)
				{
					MethodInfo? method = type.GetMethod(name, staticBindingAttr, binder, staticParamTypes, null);
					if (method != null && CheckReturnType(method.ReturnType))
					{
						return method;
					}
				}
				// 查找实例方法。
				if (instanceParamTypes != null)
				{
					MethodInfo? method = type.GetMethod(name, instanceBindingAttr, binder, instanceParamTypes, null);
					if (method != null && CheckReturnType(method.ReturnType))
					{
						return method;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 搜索属性。
		/// </summary>
		/// <returns>搜索得到的属性，找不到则为 <c>null</c>。</returns>
		public PropertyInfo? FindProperty()
		{
			if ((bindingFlags & FindPropertyFlags) != BindingFlags.Default)
			{
				// 查找静态属性。
				if (staticParamTypes != null)
				{
					PropertyInfo? property = GetProperty(staticBindingAttr, staticParamTypes);
					if (property != null)
					{
						return property;
					}
				}
				// 查找实例属性。
				if (instanceParamTypes != null)
				{
					PropertyInfo? property = GetProperty(instanceBindingAttr, instanceParamTypes);
					if (property != null)
					{
						return property;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 搜索字段，只会根据名称搜索。
		/// </summary>
		/// <returns>搜索得到的字段，找不到则为 <c>null</c>。</returns>
		public FieldInfo? FindField()
		{
			if ((bindingFlags & FindFieldFlags) != BindingFlags.Default)
			{
				return type.GetField(name, bindingFlags);
			}
			return null;
		}

		/// <summary>
		/// 检查指定的类型是否与返回类型兼容。
		/// </summary>
		/// <param name="checkType">要检查的类型。</param>
		/// <returns>如果与返回类型兼容，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool CheckReturnType(Type checkType)
		{
			if (returnType == typeof(void))
			{
				return true;
			}
			if (checkType == typeof(void))
			{
				return false;
			}
			return returnType.IsExplicitFrom(checkType);
		}

		/// <summary>
		/// 搜索与指定参数类型匹配的属性。
		/// </summary>
		/// <param name="propertyBindingAttr">搜索方式。</param>
		/// <param name="types">属性的索引参数。</param>
		/// <returns>指定类型的属性，如果无法找到则为 <c>null</c>。</returns>
		private PropertyInfo? GetProperty(BindingFlags propertyBindingAttr, Type[] types)
		{
			Type propertyType = returnType;
			if (propertyType == typeof(void))
			{
				// 是设置属性，将最后一个参数作为属性类型。
				if (types.Length == 0)
				{
					return null;
				}
				propertyType = types[^1];
				types = types[0..^1];
			}
			PropertyInfo? property = type.GetProperty(name, propertyBindingAttr, binder, null, types, null);
			// 后面再匹配属性类型，避免系统无法正确找到属性。
			if (property == null || !property.PropertyType.IsExplicitFrom(propertyType))
			{
				return null;
			}
			return property;
		}
	}
}
