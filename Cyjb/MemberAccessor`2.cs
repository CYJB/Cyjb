using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 表示实例属性或字段的存取器。
	/// </summary>
	/// <typeparam name="TTarget">包含实例属性或字段的对象的类型。</typeparam>
	/// <typeparam name="TValue">实例属性或字段值的类型。</typeparam>
	public sealed class MemberAccessor<TTarget, TValue>
	{
		/// <summary>
		/// 实例属性或字段的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string name;
		/// <summary>
		/// 获取实例属性或字段的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Func<TTarget, TValue> getDelegate;
		/// <summary>
		/// 设置实例属性或字段的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<TTarget, TValue> setDelegate;

		#region 从委托创建

		/// <summary>
		/// 使用指定的名字和访问委托，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例。
		/// </summary>
		/// <param name="name">实例属性或字段的名字。</param>
		/// <param name="getDelegate">用于获取实例属性或字段的委托。</param>
		/// <param name="setDelegate">用于设置实例属性或字段的委托。</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		/// <exception cref="ArgumentException"><paramref name="getDelegate"/> 和 <paramref name="setDelegate"/> 
		/// 全部为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public MemberAccessor(string name, Func<TTarget, TValue> getDelegate, Action<TTarget, TValue> setDelegate)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw CommonExceptions.StringEmpty("name");
			}
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

		#region 从类型创建

		/// <summary>
		/// 使用实例属性或字段的名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示 <typeparamref name="TTarget"/> 中的的实例属性或字段。
		/// </summary>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw CommonExceptions.StringEmpty("name");
			}
			Contract.EndContractBlock();
			this.name = name;
			this.Init(typeof(TTarget), false);
		}
		/// <summary>
		/// 使用实例属性或字段的名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示 <typeparamref name="TTarget"/> 中的的实例属性或字段。
		/// </summary>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性或字段。
		/// 如果要访问非公共实例属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(string name, bool nonPublic)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw CommonExceptions.StringEmpty("name");
			}
			Contract.EndContractBlock();
			this.name = name;
			this.Init(typeof(TTarget), nonPublic);
		}
		/// <summary>
		/// 使用包含实例属性或字段的类型和名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="targetType">包含实例属性或字段的类型。</param>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(Type targetType, string name)
		{
			if (targetType == null)
			{
				throw CommonExceptions.ArgumentNull("targetType");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw CommonExceptions.StringEmpty("name");
			}
			Contract.EndContractBlock();
			this.name = name;
			this.Init(targetType, false);
		}
		/// <summary>
		/// 使用包含实例属性或字段的类型和名称，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="targetType">包含实例属性或字段的类型。</param>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性或字段。
		/// 如果要访问非公共实例属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> 为 <c>null</c> 或空字符串。</exception>
		public MemberAccessor(Type targetType, string name, bool nonPublic)
		{
			if (targetType == null)
			{
				throw CommonExceptions.ArgumentNull("targetType");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw CommonExceptions.StringEmpty("name");
			}
			Contract.EndContractBlock();
			this.name = name;
			this.Init(targetType, nonPublic);
		}
		/// <summary>
		/// 使用指定的实例属性，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(PropertyInfo property)
		{
			if (property == null)
			{
				throw CommonExceptions.ArgumentNull("property");
			}
			Contract.EndContractBlock();
			this.name = property.Name;
			Init(property, false);
		}
		/// <summary>
		/// 使用指定的实例属性，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性。
		/// 如果要访问非公共实例属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		/// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <c>null</c>。</exception>
		public MemberAccessor(PropertyInfo property, bool nonPublic)
		{
			if (property == null)
			{
				throw CommonExceptions.ArgumentNull("property");
			}
			Contract.EndContractBlock();
			this.name = property.Name;
			Init(property, nonPublic);
		}
		/// <summary>
		/// 使用指定的字段，初始化 <see cref="MemberAccessor{TTarget, TValue}"/> 类的新实例，
		/// 表示指定的字段。
		/// </summary>
		/// <param name="field">要访问的字段。</param>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		public MemberAccessor(FieldInfo field)
		{
			if (field == null)
			{
				throw CommonExceptions.ArgumentNull("field");
			}
			Contract.EndContractBlock();
			this.name = field.Name;
			Init(field);
		}

		#endregion // 从类型创建

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
				Init(property, nonPublic);
				return;
			}
			FieldInfo field = type.GetField(this.name, flags);
			if (field != null)
			{
				Init(field);
			}
			throw CommonExceptions.PropertyOrFieldNotFound(this.name, nonPublic);
		}
		/// <summary>
		/// 使用指定的实例属性初始化当前实例。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性。
		/// 如果要访问非公共实例属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(PropertyInfo property, bool nonPublic)
		{
			Contract.Requires(property != null);
			MethodInfo method = property.GetGetMethod(nonPublic);
			if (method != null)
			{
				this.getDelegate = method.CreateDelegate<Func<TTarget, TValue>>();
			}
			method = property.GetSetMethod(nonPublic);
			if (method != null)
			{
				this.setDelegate = method.CreateDelegate<Action<TTarget, TValue>>();
			}
		}
		/// <summary>
		/// 使用指定的字段初始化当前实例。
		/// </summary>
		/// <param name="field">要访问的字段。</param>
		private void Init(FieldInfo field)
		{
			Contract.Requires(field != null);
			this.getDelegate = field.CreateDelegate<Func<TTarget, TValue>>(false);
			this.setDelegate = field.CreateDelegate<Action<TTarget, TValue>>(false);
		}

		#endregion // 初始化

		/// <summary>
		/// 获取实例属性或字段的名称。
		/// </summary>
		/// <value>实例属性或字段的名称。</value>
		public string Name
		{
			get
			{
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return this.name;
			}
		}
		/// <summary>
		/// 获取指定对象的实例属性或字段的值。
		/// </summary>
		/// <param name="target">要获取实例属性或字段值的对象。</param>
		/// <returns>指定对象的实例属性或字段的值。</returns>
		public TValue GetValue(TTarget target)
		{
			if (this.getDelegate == null)
			{
				throw CommonExceptions.PropertyNoGetter(this.name);
			}
			return this.getDelegate(target);
		}
		/// <summary>
		/// 设置指定对象的实例属性或字段的值。
		/// </summary>
		/// <param name="target">要获取实例属性或字段值的对象。</param>
		/// <param name="value">指定对象的实例属性或字段的值。</param>
		public void SetValue(TTarget target, TValue value)
		{
			if (this.setDelegate == null)
			{
				throw CommonExceptions.PropertyNoSetter(this.name);
			}
			this.setDelegate(target, value);
		}
	}
}
