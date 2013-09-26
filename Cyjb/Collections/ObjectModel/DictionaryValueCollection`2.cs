using System;
using System.Collections.Generic;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 表示字典的值集合。
	/// </summary>
	/// <typeparam name="TKey">字典的键的类型。</typeparam>
	/// <typeparam name="TValue">字典的值的类型。</typeparam>
	[Serializable]
	internal sealed class DictionaryValueCollection<TKey, TValue> : ReadOnlyCollection<TValue>
	{
		/// <summary>
		/// 当前集合对应的字典。
		/// </summary>
		private DictionaryBase<TKey, TValue> dict;
		/// <summary>
		/// 使用指定的字典初始化 <see cref="DictionaryValueCollection&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="dict">当前集合对应的字典。</param>
		public DictionaryValueCollection(DictionaryBase<TKey, TValue> dict)
		{
			this.dict = dict;
		}

		#region ICollection<TValue> 成员

		/// <summary>
		/// 获取 <see cref="DictionaryValueCollection&lt;TKey,TValue&gt;"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="DictionaryValueCollection&lt;TKey,TValue&gt;"/> 中包含的元素数。</value>
		public override int Count
		{
			get { return this.dict.Count; }
		}
		/// <summary>
		/// 确定 <see cref="DictionaryValueCollection&lt;TKey,TValue&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="DictionaryValueCollection&lt;TKey,TValue&gt;"/> 中定位的对象。</param>
		public override bool Contains(TValue item)
		{
			return this.dict.ContainsValue(item);
		}
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将 <see cref="DictionaryValueCollection&lt;TKey, TValue&gt;"/> 
		/// 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="DictionaryValueCollection&lt;TKey, TValue&gt;"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="DictionaryValueCollection&lt;TKey, TValue&gt;"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		public override void CopyTo(TValue[] array, int arrayIndex)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			ExceptionHelper.CheckFlatArray(array, "array");
			if (arrayIndex < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("arrayIndex");
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw ExceptionHelper.ArrayTooSmall("array");
			}
			foreach (KeyValuePair<TKey, TValue> pair in this.dict)
			{
				array[arrayIndex++] = pair.Value;
			}
		}

		#endregion // ICollection<TKey> 成员

		#region IEnumerable<TValue> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public override IEnumerator<TValue> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, TValue> pair in this.dict)
			{
				yield return pair.Value;
			}
		}

		#endregion // IEnumerable<TValue> 成员

	}
}
