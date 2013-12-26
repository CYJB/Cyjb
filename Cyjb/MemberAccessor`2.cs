using System;
using System.Diagnostics;
using System.Reflection;

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
		/// 表示公共实例属性或方法的标志。
		/// </summary>
		private const BindingFlags Public = BindingFlags.Instance | BindingFlags.Public;
		/// <summary>
		/// 表示非公共实例属性或方法的标志。
		/// </summary>
		private const BindingFlags NonPublic = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		/// <summary>
		/// 实例属性或字段的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string name;
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
		/// 使用指定的访问委托，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="getDelegate">用于获取实例属性或字段的委托。</param>
		/// <param name="setDelegate">用于设置实例属性或字段的委托。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public MemberAccessor(Func<TTarget, TValue> getDelegate, Action<TTarget, TValue> setDelegate)
		{
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
		}
		/// <summary>
		/// 使用指定的名字和访问委托，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="name">实例属性或字段的名字。</param>
		/// <param name="getDelegate">用于获取实例属性或字段的委托。</param>
		/// <param name="setDelegate">用于设置实例属性或字段的委托。</param>
		public MemberAccessor(string name, Func<TTarget, TValue> getDelegate, Action<TTarget, TValue> setDelegate)
		{
			this.name = name;
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
		}

		#endregion // 从委托创建

		#region 从类型创建

		/// <summary>
		/// 使用实例属性或字段的名称，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例，
		/// 表示 <typeparamref name="TTarget"/> 中的的实例属性或字段。
		/// </summary>
		/// <param name="name">实例属性或字段的名称。</param>
		public MemberAccessor(string name)
			: this(typeof(TTarget), name, false)
		{ }
		/// <summary>
		/// 使用实例属性或字段的名称，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例，
		/// 表示 <typeparamref name="TTarget"/> 中的的实例属性或字段。
		/// </summary>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性或字段。
		/// 如果要访问非公共实例属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public MemberAccessor(string name, bool nonPublic)
			: this(typeof(TTarget), name, nonPublic)
		{ }
		/// <summary>
		/// 使用包含实例属性或字段的类型和名称，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="targetType">包含实例属性或字段的类型。</param>
		/// <param name="name">实例属性或字段的名称。</param>
		public MemberAccessor(Type targetType, string name)
			: this(targetType, name, false)
		{ }
		/// <summary>
		/// 使用包含实例属性或字段的类型和名称，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例，
		/// 表示指定的实例属性或字段。
		/// </summary>
		/// <param name="targetType">包含实例属性或字段的类型。</param>
		/// <param name="name">实例属性或字段的名称。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性或字段。
		/// 如果要访问非公共实例属性或字段，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public MemberAccessor(Type targetType, string name, bool nonPublic)
		{
			ExceptionHelper.CheckArgumentNull(targetType, "targetType");
			this.name = name;
			PropertyInfo property = targetType.GetProperty(name, nonPublic ? NonPublic : Public);
			if (property != null)
			{
				Init(property, nonPublic);
			}
			else
			{
				FieldInfo field = targetType.GetField(name, nonPublic ? NonPublic : Public);
				if (field != null)
				{
					Init(field);
				}
			}
		}
		/// <summary>
		/// 使用指定的实例属性，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		public MemberAccessor(PropertyInfo property)
			: this(property, false)
		{ }
		/// <summary>
		/// 使用指定的实例属性，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例，
		/// 表示指定的实例属性。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性。
		/// 如果要访问非公共实例属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		public MemberAccessor(PropertyInfo property, bool nonPublic)
		{
			ExceptionHelper.CheckArgumentNull(property, "property");
			this.name = property.Name;
			Init(property, nonPublic);
		}
		/// <summary>
		/// 使用指定的字段，初始化 <see cref="MemberAccessor&lt;T&gt;"/> 类的新实例，
		/// 表示指定的字段。
		/// </summary>
		/// <param name="field">要访问的字段。</param>
		public MemberAccessor(FieldInfo field)
		{
			ExceptionHelper.CheckArgumentNull(field, "field");
			this.name = field.Name;
			Init(field);
		}

		#endregion // 从类型创建

		#region 初始化

		/// <summary>
		/// 使用指定的实例属性初始化当前实例。
		/// </summary>
		/// <param name="property">要访问的实例属性。</param>
		/// <param name="nonPublic">指示是否应访问非公共实例属性。
		/// 如果要访问非公共实例属性，则为 <c>true</c>；否则为 <c>false</c>。</param>
		private void Init(PropertyInfo property, bool nonPublic)
		{
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
			this.getDelegate = field.CreateDelegate<Func<TTarget, TValue>>(false);
			this.setDelegate = field.CreateDelegate<Action<TTarget, TValue>>(false);
		}

		#endregion // 初始化

		/// <summary>
		/// 获取实例属性或字段的名称。
		/// </summary>
		/// <value>实例属性或字段的名称。</value>
		public string Name { get { return this.name; } }
		/// <summary>
		/// 获取指定对象的实例属性或字段的值。
		/// </summary>
		/// <param name="target">要获取实例属性或字段值的对象。</param>
		/// <returns>指定对象的实例属性或字段的值。</returns>
		public TValue GetValue(TTarget target)
		{
			if (this.getDelegate == null)
			{
				throw ExceptionHelper.BindTargetPropertyNoGet(this.name);
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
				throw ExceptionHelper.BindTargetPropertyNoSet(this.name);
			}
			this.setDelegate(target, value);
		}
	}
}
