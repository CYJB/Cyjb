using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{

	/// <summary>
	/// 为泛型只读字典提供基类。
	/// </summary>
	/// <typeparam name="TKey">字典的键的类型。</typeparam>
	/// <typeparam name="TValue">字典的值的类型。</typeparam>
	public class ReadOnlyDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>
	{
		/// <summary>
		/// 空的只读泛型集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static IDictionary<TKey, TValue> empty;
		/// <summary>
		/// 获取空的只读泛型集合。
		/// </summary>
		/// <value>空的只读泛型集合。</value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static IDictionary<TKey, TValue> Empty
		{
			get
			{
				if (empty == null)
				{
					Interlocked.CompareExchange(ref empty, new ReadOnlyDictionary<TKey, TValue>(), null);
				}
				return empty;
			}
		}
		/// <summary>
		/// 初始化 <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		protected ReadOnlyDictionary() { }
		/// <summary>
		/// 将 <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;"/> 类的新实例初始化为指定字典的包装。
		/// </summary>
		/// <param name="dict">由新的列表包装的字典。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="dict"/> 为 <c>null</c>。</exception>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dict)
			: base(dict)
		{
			ExceptionHelper.CheckArgumentNull(dict, "collection");
		}
		/// <summary>
		/// 将 <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;"/> 类的新实例初始化为指定数组的包装。
		/// </summary>
		/// <param name="array">由新的字典包装的数组。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		public ReadOnlyDictionary(KeyValuePair<TKey, TValue>[] array)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			for (int i = 0; i < array.Length; i++)
			{
				this.Items.Add(array[i]);
			}
		}
		/// <summary>
		/// 将 <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;"/> 类的新实例初始化为指定迭代器中数据的包装。
		/// </summary>
		/// <param name="enumerable">由新的字典包装的迭代器中的数据。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="enumerable"/> 为 <c>null</c>。</exception>
		public ReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
		{
			ExceptionHelper.CheckArgumentNull(enumerable, "enumerable");
			foreach (KeyValuePair<TKey, TValue> pair in enumerable)
			{
				this.Items.Add(pair);
			}
		}

		#region ICollection<KeyValuePair<TKey,TValue>> 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		public override bool IsReadOnly
		{
			get { return true; }
		}

		#endregion // ICollection<KeyValuePair<TKey,TValue>> 成员

	}
}
