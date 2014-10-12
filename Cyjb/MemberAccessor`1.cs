using System;
using System.Diagnostics;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 表示属性或字段的存取器。
	/// </summary>
	/// <typeparam name="T">属性或字段值的类型。</typeparam>
	public sealed class MemberAccessor<T>
	{
		/// <summary>
		/// 表示静态公共属性或方法的标志。
		/// </summary>
		private const BindingFlags StaticPublic = BindingFlags.Static | BindingFlags.Public;
		/// <summary>
		/// 表示静态非公共属性或方法的标志。
		/// </summary>
		private const BindingFlags StaticNonPublic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		/// <summary>
		/// 表示公共属性或方法的标志。
		/// </summary>
		private const BindingFlags Public = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public;
		/// <summary>
		/// 表示非公共属性或方法的标志。
		/// </summary>
		private const BindingFlags NonPublic = BindingFlags.Static | BindingFlags.Instance |
			BindingFlags.Public | BindingFlags.NonPublic;
		/// <summary>
		/// 属性或字段的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string name;
		/// <summary>
		/// 获取属性或字段的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Func<T> getDelegate;
		/// <summary>
		/// 设置属性或字段的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<T> setDelegate;

		#region 从委托创建

		/// <summary>
		/// 使用指定的访问委托，初始化 <see cref="MemberAccessor{T}"/> 类的新实例。
		/// </summary>
		/// <param name="getDelegate">用于获取属性或字段的委托。</param>
		/// <param name="setDelegate">用于设置属性或字段的委托。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="MemberAccessor{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public MemberAccessor(Func<T> getDelegate, Action<T> setDelegate)
		{
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
		}
		/// <summary>
		/// 使用指定的名字和访问委托，初始化 <see cref="MemberAccessor{T}"/> 类的新实例。
		/// </summary>
		/// <param name="name">属性或字段的名字。</param>
		/// <param name="getDelegate">用于获取属性或字段的委托。</param>
		/// <param name="setDelegate">用于设置属性或字段的委托。</param>
		public MemberAccessor(string name, Func<T> getDelegate, Action<T> setDelegate)
		{
			this.name = name;
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
		}

		#endregion // 从委托创建

		#region 创建静态访问

		/// <summary>
		/// 使用包含静态属性或字段的类型和名称，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态属性或字段。
		/// </summary>
		/// <param name="targetType">包含静态属性或字段的类型。</param>
		/// <param name="name">属性或字段的名称。</param>
		public MemberAccessor(Type targetType, string name)
			: this(targetType, name, false)
		{ }
		/// <summary>
		/// 使用包含静态属性或字段的类型和名称，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态属性或字段。
		/// </summary>
		/// <param name="targetType">包含静态属性或字段的类型。</param>
		/// <param name="name">属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性或字段。
		/// 如果要访问非公共属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public MemberAccessor(Type targetType, string name, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(targetType, "targetType");
			this.name = name;
			PropertyInfo property = targetType.GetProperty(name, nonPublic ? StaticNonPublic : StaticPublic);
			if (property != null)
			{
				Init(property, null, nonPublic);
			}
			else
			{
				FieldInfo field = targetType.GetField(name, nonPublic ? StaticNonPublic : StaticPublic);
				if (field != null)
				{
					Init(field, null);
				}
			}
		}
		/// <summary>
		/// 使用指定的静态属性，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态属性。
		/// </summary>
		/// <param name="property">要访问的静态属性。</param>
		public MemberAccessor(PropertyInfo property)
			: this(property, false)
		{ }
		/// <summary>
		/// 使用指定的静态属性，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态属性。
		/// </summary>
		/// <param name="property">要访问的静态属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性。
		/// 如果要访问非公共属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public MemberAccessor(PropertyInfo property, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			this.name = property.Name;
			Init(property, null, nonPublic);
		}
		/// <summary>
		/// 使用指定的静态字段，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态字段。
		/// </summary>
		/// <param name="field">要访问的静态字段。</param>
		public MemberAccessor(FieldInfo field)
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			this.name = field.Name;
			Init(field, null);
		}

		#endregion // 创建静态访问

		#region 创建实例访问

		/// <summary>
		/// 使用包含属性或字段的对象和名称，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="target">包含实例属性或字段的对象。</param>
		/// <param name="name">属性或字段的名称。</param>
		public MemberAccessor(object target, string name)
			: this(target, name, false)
		{ }
		/// <summary>
		/// 使用包含属性或字段的对象和名称，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="target">包含实例属性或字段的对象。</param>
		/// <param name="name">属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性或字段。
		/// 如果要访问非公共属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public MemberAccessor(object target, string name, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			this.name = name;
			Type type = target.GetType();
			PropertyInfo property = type.GetProperty(name, nonPublic ? NonPublic : Public);
			if (property != null)
			{
				Init(property, target, nonPublic);
			}
			else
			{
				FieldInfo field = type.GetField(name, nonPublic ? NonPublic : Public);
				if (field != null)
				{
					Init(field, target);
				}
			}
		}
		/// <summary>
		/// 使用包含属性的对象和属性信息，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="target">包含实例属性的对象。</param>
		/// <param name="property">要访问的实例属性。</param>
		public MemberAccessor(object target, PropertyInfo property)
			: this(target, property, false)
		{ }
		/// <summary>
		/// 使用包含属性的对象和属性信息，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="target">包含实例属性的对象。</param>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性。
		/// 如果要访问非公共属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public MemberAccessor(object target, PropertyInfo property, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckArgumentNull(property, "property");
			this.name = property.Name;
			Init(property, target, nonPublic);
		}
		/// <summary>
		/// 使用包含属性的对象和字段信息，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例字段。
		/// </summary>
		/// <param name="target">包含实例字段的对象。</param>
		/// <param name="field">要访问的实例字段。</param>
		public MemberAccessor(object target, FieldInfo field)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckArgumentNull(field, "field");
			this.name = field.Name;
			Init(field, target);
		}

		#endregion // 创建实例访问

		#region 初始化

		/// <summary>
		/// 使用指定的静态属性初始化当前实例。
		/// </summary>
		/// <param name="property">要访问的静态属性。</param>
		/// <param name="firstArgument">委托的第一参数。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性。
		/// 如果要访问非公共属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(PropertyInfo property, object firstArgument, bool nonPublic)
		{
			MethodInfo method = property.GetGetMethod(nonPublic);
			if (method != null)
			{
				this.getDelegate = method.CreateDelegate<Func<T>>(firstArgument);
			}
			method = property.GetSetMethod(nonPublic);
			if (method != null)
			{
				this.setDelegate = method.CreateDelegate<Action<T>>(firstArgument);
			}
		}
		/// <summary>
		/// 使用指定的静态字段初始化当前实例。
		/// </summary>
		/// <param name="field">要访问的静态字段。</param>
		/// <param name="firstArgument">委托的第一参数。</param>
		private void Init(FieldInfo field, object firstArgument)
		{
			this.getDelegate = field.CreateDelegate<Func<T>>(firstArgument, false);
			this.setDelegate = field.CreateDelegate<Action<T>>(firstArgument, false);
		}

		#endregion // 初始化

		/// <summary>
		/// 获取属性或字段的名称。
		/// </summary>
		/// <value>属性或字段的名称。</value>
		public string Name { get { return this.name; } }
		/// <summary>
		/// 获取或设置属性或字段的值。
		/// </summary>
		/// <value>属性或字段的值。</value>
		public T Value
		{
			get
			{
				if (this.getDelegate == null)
				{
					throw CommonExceptions.BindTargetPropertyNoGet(this.name);
				}
				return this.getDelegate();
			}
			set
			{
				if (this.setDelegate == null)
				{
					throw CommonExceptions.BindTargetPropertyNoSet(this.name);
				}
				this.setDelegate(value);
			}
		}
	}
}
