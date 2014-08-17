using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示根据内容比较 <see cref="System.Collections.Generic.IList{T}"/> 集合的比较器。
	/// </summary>
	/// <typeparam name="T">要比较的列表元素的类型。</typeparam>
	public sealed class ListEqualityComparer<T> : EqualityComparer<IList<T>>
	{
		/// <summary>
		/// 默认的比较器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ListEqualityComparer<T> defaultValue;
		/// <summary>
		/// 获取默认的相等比较器。
		/// </summary>
		/// <value>一个默认的 <see cref="ListEqualityComparer{T}"/> 比较器。</value>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public new static ListEqualityComparer<T> Default
		{
			get
			{
				if (defaultValue == null)
				{
					Interlocked.CompareExchange(ref defaultValue, new ListEqualityComparer<T>(), null);
				}
				return defaultValue;
			}
		}
		/// <summary>
		/// 内部元素的比较器。
		/// </summary>
		private readonly IEqualityComparer<T> comparer;
		/// <summary>
		/// 初始化 <see cref="ListEqualityComparer{T}"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="ListEqualityComparer{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public ListEqualityComparer()
		{
			this.comparer = EqualityComparer<T>.Default;
		}
		/// <summary>
		/// 使用指定的元素比较器初始化 <see cref="ListEqualityComparer{T}"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">元素比较器。</param>
		public ListEqualityComparer(IEqualityComparer<T> comparer)
		{
			this.comparer = comparer ?? EqualityComparer<T>.Default;
		}

		#region EqualityComparer<IList<T>> 成员

		/// <summary>
		/// 确定指定的对象是否相等。
		/// </summary>
		/// <param name="x">要比较的第一个 <see cref="IList{T}"/> 的对象。</param>
		/// <param name="y">要比较的第二个 <see cref="IList{T}"/> 的对象。</param>
		/// <returns>如果指定的对象相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 确定指定的对象是否相等。
		/// </summary>
		/// </overloads>
		public override bool Equals(IList<T> x, IList<T> y)
		{
			if (x == null)
			{
				return y == null;
			}
			if (y == null)
			{
				return false;
			}
			if (x == y)
			{
				return true;
			}
			int cnt = x.Count;
			if (cnt != y.Count)
			{
				return false;
			}
			for (int i = 0; i < cnt; i++)
			{
				if (!comparer.Equals(x[i], y[i]))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Hash 的魔数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int MagicCode = unchecked((int)0x9E3779B9);
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
		public override int GetHashCode(IList<T> obj)
		{
			if (obj == null)
			{
				throw ExceptionHelper.ArgumentNull("obj");
			}
			Contract.EndContractBlock();
			// 算法来自 boost::hash_range。
			int cnt = obj.Count;
			int hashCode = cnt;
			for (int i = 0; i < cnt; i++)
			{
				hashCode ^= unchecked(comparer.GetHashCode(obj[i]) + MagicCode + (hashCode << 6) + (hashCode >> 2));
			}
			return hashCode;
		}

		#endregion // EqualityComparer<IList<T>> 成员

	}
}
