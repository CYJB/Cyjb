using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示根据内容比较 <see cref="ISet{T}"/> 集合的比较器。
	/// </summary>
	/// <typeparam name="T">要比较的集合元素的类型。</typeparam>
	public sealed class SetEqualityComparer<T> : EqualityComparer<ISet<T>>
	{
		/// <summary>
		/// 默认的相等比较器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static SetEqualityComparer<T> defaultValue;
		/// <summary>
		/// 获取默认的相等比较器。
		/// </summary>
		/// <value>一个默认的 <see cref="SetEqualityComparer{T}"/> 比较器。</value>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public new static SetEqualityComparer<T> Default
		{
			get
			{
				if (defaultValue == null)
				{
					Interlocked.CompareExchange(ref defaultValue, new SetEqualityComparer<T>(), null);
				}
				return defaultValue;
			}
		}
		/// <summary>
		/// 初始化 <see cref="SetEqualityComparer{T}"/> 类的新实例。
		/// </summary>
		public SetEqualityComparer() { }

		#region EqualityComparer<ISet<T>> 成员

		/// <summary>
		/// 确定指定的对象是否相等。
		/// </summary>
		/// <param name="x">要比较的第一个 <see cref="ISet{T}"/> 的对象。</param>
		/// <param name="y">要比较的第二个 <see cref="ISet{T}"/> 的对象。</param>
		/// <returns>如果指定的对象相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 确定指定的对象是否相等。
		/// </summary>
		/// </overloads>
		public override bool Equals(ISet<T> x, ISet<T> y)
		{
			if (x == null)
			{
				return y == null;
			}
			if (y == null)
			{
				return false;
			}
			return x == y || x.SetEquals(y);
		}
		/// <summary>
		/// 返回指定对象的哈希代码。
		/// </summary>
		/// <param name="obj">将为其返回哈希代码。</param>
		/// <returns>指定对象的哈希代码。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="obj"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 返回指定对象的哈希代码。
		/// </summary>
		/// </overloads>
		public override int GetHashCode(ISet<T> obj)
		{
			CommonExceptions.CheckArgumentNull(obj, "obj");
			Contract.EndContractBlock();
			// 使用与位置无关的弱哈希。
			return obj.Count ^ obj.Select(o => o.GetHashCode()).Aggregate((x, y) => x ^ y);
		}

		#endregion // EqualityComparer<ISet<T>> 成员

	}
}
