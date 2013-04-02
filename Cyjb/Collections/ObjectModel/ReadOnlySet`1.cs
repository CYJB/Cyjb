using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{

	/// <summary>
	/// 为泛型只读集提供基类。
	/// </summary>
	/// <typeparam name="T">集中的元素类型。</typeparam>
	public class ReadOnlySet<T> : SetBase<T>
	{
		/// <summary>
		/// 空的只读泛型集。
		/// </summary>
		private static ISet<T> empty;
		/// <summary>
		/// 获取空的只读泛型集。
		/// </summary>
		/// <value>空的只读泛型集。</value>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static ISet<T> Empty
		{
			get
			{
				if (empty == null)
				{
					Interlocked.CompareExchange(ref empty, new ReadOnlySet<T>(), null);
				}
				return empty;
			}
		}
		/// <summary>
		/// 初始化 <see cref="ReadOnlySet&lt;T&gt;"/> 类的新实例。
		/// </summary>
		public ReadOnlySet() : base(true) { }
		/// <summary>
		/// 初始化 <see cref="ReadOnlySet&lt;T&gt;"/> 类的新实例，
		/// 该实例是指定集周围的只读包装。
		/// </summary>
		/// <param name="set">由新的集包装的集。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="set"/> 为 <c>null</c>。</exception>
		public ReadOnlySet(ISet<T> set)
			: base(set, true)
		{
			ExceptionHelper.CheckArgumentNull(set, "set");
		}
		/// <summary>
		/// 初始化 <see cref="ReadOnlySet&lt;T&gt;"/> 类的新实例，
		/// 该实例是指定数组周围的只读包装。
		/// </summary>
		/// <param name="array">由新的集包装的数组。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		public ReadOnlySet(T[] array)
			: base(true)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			for (int i = 0; i < array.Length; i++)
			{
				this.Items.Add(array[i]);
			}
		}
		/// <summary>
		/// 初始化 <see cref="ReadOnlySet&lt;T&gt;"/> 类的新实例，
		/// 该实例是指定迭代器周围的只读包装。
		/// </summary>
		/// <param name="enumerable">由新的集包装的迭代器。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="enumerable"/> 为 <c>null</c>。</exception>
		public ReadOnlySet(IEnumerable<T> enumerable)
			: base(true)
		{
			ExceptionHelper.CheckArgumentNull(enumerable, "enumerable");
			foreach (T obj in enumerable)
			{
				this.Items.Add(obj);
			}
		}
	}
}
