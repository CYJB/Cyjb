using System.Diagnostics;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示根据内容比较 <see cref="IList{T}"/> 集合的比较器。
	/// </summary>
	/// <typeparam name="T">要比较的列表元素的类型。</typeparam>
	public sealed class ListEqualityComparer<T> : EqualityComparer<IList<T>>
	{
		/// <summary>
		/// 默认的比较器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ListEqualityComparer<T>? defaultValue;
		/// <summary>
		/// 获取默认的相等比较器。
		/// </summary>
		/// <value>一个默认的 <see cref="ListEqualityComparer{T}"/> 比较器。</value>
		public new static IEqualityComparer<IList<T>> Default
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
			comparer = EqualityComparer<T>.Default;
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
		public override bool Equals(IList<T>? x, IList<T>? y)
		{
			if (x == null)
			{
				return y == null;
			}
			else if (y == null)
			{
				return false;
			}
			else if (x == y)
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
		/// 返回指定对象的哈希代码。
		/// </summary>
		/// <param name="list">将为其返回哈希代码。</param>
		/// <returns>指定对象的哈希代码。</returns>
		public override int GetHashCode(IList<T> list)
		{
			HashCode hashCode = new();
			for (int i = 0; i < list.Count; i++)
			{
				hashCode.Add(list[i]);
			}
			return hashCode.ToHashCode();
		}

		#endregion // EqualityComparer<IList<T>> 成员

	}
}
