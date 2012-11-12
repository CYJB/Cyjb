using System.Collections.Generic;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 表示字典的键集合。
	/// </summary>
	/// <typeparam name="TKey">字典的键的类型。</typeparam>
	/// <typeparam name="TValue">字典的值的类型。</typeparam>
	internal sealed class DictionaryKeyCollection<TKey, TValue> : ReadOnlyCollection<TKey>
	{
		/// <summary>
		/// 当前集合对应的字典。
		/// </summary>
		private IDictionary<TKey, TValue> dict;
		/// <summary>
		/// 使用指定的字典初始化 <see cref="DictionaryKeyCollection&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="dict">当前集合对应的字典。</param>
		public DictionaryKeyCollection(IDictionary<TKey, TValue> dict)
		{
			this.dict = dict;
		}

		#region ICollection<TKey> 成员

		/// <summary>
		/// 获取 <see cref="DictionaryKeyCollection&lt;TKey,TValue&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="DictionaryKeyCollection&lt;TKey,TValue&gt;"/> 中包含的元素数。</value>
		public override int Count
		{
			get { return this.dict.Count; }
		}
		/// <summary>
		/// 确定 <see cref="DictionaryKeyCollection&lt;TKey,TValue&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="DictionaryKeyCollection&lt;TKey,TValue&gt;"/> 中定位的对象。</param>
		public override bool Contains(TKey item)
		{
			return this.dict.ContainsKey(item);
		}

		#endregion // ICollection<TKey> 成员

		#region IEnumerable<TKey> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public override IEnumerator<TKey> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, TValue> pair in this.dict)
			{
				yield return pair.Key;
			}
		}

		#endregion // IEnumerable<TKey> 成员

	}
}
