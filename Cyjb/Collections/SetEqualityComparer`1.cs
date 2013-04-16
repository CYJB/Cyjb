using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cyjb
{
	/// <summary>
	/// 表示根据内容比较 <see cref="System.Collections.Generic.ISet&lt;T&gt;"/> 集合的比较器。
	/// </summary>
	/// <typeparam name="T">要比较的集合元素的类型。</typeparam>
	public sealed class SetEqualityComparer<T> : EqualityComparer<ISet<T>>
	{
		/// <summary>
		/// 默认的比较器。
		/// </summary>
		private static SetEqualityComparer<T> defaultValue;
		/// <summary>
		/// 返回一个默认的相等比较器。
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public new static SetEqualityComparer<T> Default
		{
			get
			{
				if (defaultValue == null)
				{
					defaultValue = new SetEqualityComparer<T>();
				}
				return defaultValue;
			}
		}

		#region EqualityComparer<ISet<T>> 成员

		/// <summary>
		/// 确定指定的对象是否相等。
		/// </summary>
		/// <param name="x">要比较的第一个 
		/// <see cref="System.Collections.Generic.ISet&lt;T&gt;"/> 的对象。</param>
		/// <param name="y">要比较的第二个 
		/// <see cref="System.Collections.Generic.ISet&lt;T&gt;"/> 的对象。</param>
		/// <returns>如果指定的对象相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
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
			if (x == y)
			{
				return true;
			}
			return x.SetEquals(y);
		}
		/// <summary>
		/// 返回指定对象的哈希代码。
		/// </summary>
		/// <param name="obj">将为其返回哈希代码。</param>
		/// <returns>指定对象的哈希代码。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="obj"/> 为 <c>null</c>。</exception>
		public override int GetHashCode(ISet<T> obj)
		{
			ExceptionHelper.CheckArgumentNull(obj, "obj");
			// 使用与位置无关的弱哈希。
			int hashCode = obj.Count;
			foreach (T item in obj)
			{
				hashCode ^= item.GetHashCode();
			}
			return hashCode;
		}

		#endregion // EqualityComparer<ISet<T>> 成员

	}
}
