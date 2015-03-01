using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 表示静态或封闭的实例属性、字段的存取器。
	/// </summary>
	/// <typeparam name="T">属性或字段的类型。</typeparam>
	public sealed class MemberAccessor<T>
	{
		/// <summary>
		/// 属性或字段的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string name;
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
		/// 使用指定的名字和访问委托，初始化 <see cref="MemberAccessor{T}"/> 类的新实例。
		/// </summary>
		/// <param name="name">属性或字段的名字。</param>
		/// <param name="getDelegate">用于获取属性或字段的委托。</param>
		/// <param name="setDelegate">用于设置属性或字段的委托。</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><paramref name="getDelegate"/> 和 <paramref name="setDelegate"/> 
		/// 全部为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="MemberAccessor{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public MemberAccessor(string name, Func<T> getDelegate, Action<T> setDelegate)
		{
			CommonExceptions.CheckStringEmpty(name, "name");
			if (getDelegate == null && setDelegate == null)
			{
				throw CommonExceptions.ArgumentNull("getDelegate");
			}
			Contract.EndContractBlock();
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
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(Type targetType, string name)
		{
			CommonExceptions.CheckArgumentNull(targetType, "targetType");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.EndContractBlock();
			this.name = name;
			this.Init(targetType, false);
		}
		/// <summary>
		/// 使用包含静态属性或字段的类型和名称，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态属性或字段。
		/// </summary>
		/// <param name="targetType">包含静态属性或字段的类型。</param>
		/// <param name="name">属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性或字段。
		/// 如果要访问非公共属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(Type targetType, string name, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(targetType, "targetType");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.EndContractBlock();
			this.name = name;
			this.Init(targetType, nonPublic);
		}
		/// <summary>
		/// 使用指定的静态属性，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态属性。
		/// </summary>
		/// <param name="property">要访问的静态属性。</param>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(PropertyInfo property)
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			this.name = property.Name;
			Init(property, null, false);
		}
		/// <summary>
		/// 使用指定的静态属性，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态属性。
		/// </summary>
		/// <param name="property">要访问的静态属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性。
		/// 如果要访问非公共属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(PropertyInfo property, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			this.name = property.Name;
			Init(property, null, nonPublic);
		}
		/// <summary>
		/// 使用指定的静态字段，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的静态字段。
		/// </summary>
		/// <param name="field">要访问的静态字段。</param>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		public MemberAccessor(FieldInfo field)
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.EndContractBlock();
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
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(object target, string name)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.EndContractBlock();
			this.name = name;
			this.Init(target, false);
		}
		/// <summary>
		/// 使用包含属性或字段的对象和名称，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="target">包含实例属性或字段的对象。</param>
		/// <param name="name">属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性或字段。
		/// 如果要访问非公共属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(object target, string name, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckStringEmpty(name, "name");
			Contract.EndContractBlock();
			this.name = name;
			this.Init(target, nonPublic);
		}
		/// <summary>
		/// 使用包含属性的对象和属性信息，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="target">包含实例属性的对象。</param>
		/// <param name="property">要访问的实例属性。</param>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(object target, PropertyInfo property)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			this.name = property.Name;
			Init(property, target, false);
		}
		/// <summary>
		/// 使用包含属性的对象和属性信息，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="target">包含实例属性的对象。</param>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性。
		/// 如果要访问非公共属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(object target, PropertyInfo property, bool nonPublic)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckArgumentNull(property, "property");
			Contract.EndContractBlock();
			this.name = property.Name;
			Init(property, target, nonPublic);
		}
		/// <summary>
		/// 使用包含属性的对象和字段信息，初始化 <see cref="MemberAccessor{T}"/> 类的新实例，
		/// 表示指定的实例字段。
		/// </summary>
		/// <param name="target">包含实例字段的对象。</param>
		/// <param name="field">要访问的实例字段。</param>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		public MemberAccessor(object target, FieldInfo field)
		{
			CommonExceptions.CheckArgumentNull(target, "target");
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.EndContractBlock();
			this.name = field.Name;
			Init(field, target);
		}

		#endregion // 创建实例访问

		#region 初始化

		/// <summary>
		/// 使用指定的搜索类型初始化当前实例。
		/// </summary>
		/// <param name="type">要搜索的类型。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性或字段。
		/// 如果要访问非公共属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(Type type, bool nonPublic)
		{
			Contract.Requires(type != null);
			BindingFlags flags = nonPublic ? TypeExt.AllMemberFlag : TypeExt.PublicFlag;
			PropertyInfo property = type.GetProperty(this.name, flags);
			if (property != null)
			{
				Init(property, null, nonPublic);
				return;
			}
			FieldInfo field = type.GetField(this.name, flags);
			if (field != null)
			{
				Init(field, null);
			}
			throw CommonExceptions.PropertyOrFieldNotFound(this.name, nonPublic);
		}
		/// <summary>
		/// 使用指定的实例初始化当前实例。
		/// </summary>
		/// <param name="target">要搜索的实例。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性或字段。
		/// 如果要访问非公共属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(object target, bool nonPublic)
		{
			Contract.Requires(target != null);
			Type type = target.GetType();
			BindingFlags flags = nonPublic ? TypeExt.AllMemberFlag : TypeExt.PublicFlag;
			PropertyInfo property = type.GetProperty(this.name, flags);
			if (property != null)
			{
				Init(property, target, nonPublic);
				return;
			}
			FieldInfo field = type.GetField(this.name, flags);
			if (field != null)
			{
				Init(field, target);
			}
			throw CommonExceptions.PropertyOrFieldNotFound(this.name, nonPublic);
		}
		/// <summary>
		/// 使用指定的静态属性初始化当前实例。
		/// </summary>
		/// <param name="property">要访问的静态属性。</param>
		/// <param name="firstArgument">委托的第一参数。</param>
		/// <param name="nonPublic">指示是否应访问非公共属性。
		/// 如果要访问非公共属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(PropertyInfo property, object firstArgument, bool nonPublic)
		{
			Contract.Requires(property != null);
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
			Contract.Requires(field != null);
			this.getDelegate = field.CreateDelegate<Func<T>>(firstArgument, false);
			this.setDelegate = field.CreateDelegate<Action<T>>(firstArgument, false);
		}

		#endregion // 初始化

		/// <summary>
		/// 获取属性或字段的名称。
		/// </summary>
		/// <value>属性或字段的名称。</value>
		public string Name
		{
			get
			{
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return this.name;
			}
		}
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
					throw CommonExceptions.PropertyNoGetter(this.name);
				}
				return this.getDelegate();
			}
			set
			{
				if (this.setDelegate == null)
				{
					throw CommonExceptions.PropertyNoSetter(this.name);
				}
				this.setDelegate(value);
			}
		}
	}
}
