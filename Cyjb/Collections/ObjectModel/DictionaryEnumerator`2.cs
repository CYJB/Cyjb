using System.Collections;
using System.Collections.Generic;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 表示字典的迭代器。
	/// </summary>
	/// <typeparam name="TKey">字典的键的类型。</typeparam>
	/// <typeparam name="TValue">字典的值的类型。</typeparam>
	internal sealed class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator
	{
		/// <summary>
		/// 被包装的迭代器。
		/// </summary>
		private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
		/// <summary>
		/// 当前迭代到的结果。
		/// </summary>
		private KeyValuePair<TKey, TValue> current;
		/// <summary>
		/// 使用指定的迭代器初始化 <see cref="DictionaryEnumerator&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="enumerator">要包装的迭代器。</param>
		public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
		{
			this.enumerator = enumerator;
		}

		#region IDictionaryEnumerator 成员

		/// <summary>
		/// 同时获取当前字典项的键和值。
		/// </summary>
		/// <value>同时包含当前字典项的键和值的 <see cref="System.Collections.DictionaryEntry"/>。</value>
		public DictionaryEntry Entry
		{
			get { return new DictionaryEntry(current.Key, current.Value); }
		}
		/// <summary>
		/// 获取当前字典项的键。
		/// </summary>
		/// <value>当前枚举元素的键。</value>
		public object Key
		{
			get { return current.Key; }
		}
		/// <summary>
		/// 获取当前字典项的值。
		/// </summary>
		/// <value>当前枚举元素的值。</value>
		public object Value
		{
			get { return current.Value; }
		}

		#endregion

		#region IEnumerator 成员

		/// <summary>
		/// 获取集合中的当前元素。
		/// </summary>
		/// <value>集合中的当前元素。</value>
		public object Current
		{
			get { return Entry; }
		}
		/// <summary>
		/// 将枚举数推进到集合的下一个元素。
		/// </summary>
		/// <returns>如果枚举数成功地推进到下一个元素，则为 <c>true</c>；
		/// 如果枚举数越过集合的结尾，则为 <c>false</c>。</returns>
		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}
		/// <summary>
		/// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
		/// </summary>
		public void Reset()
		{
			enumerator.Reset();
		}

		#endregion

	}
}
