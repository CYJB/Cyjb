using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Configurations
{
	/// <summary>
	/// 表示强类型的包含一个子元素集合的配置元素。
	/// </summary>
	/// <typeparam name="TElement">子配置元素的类型。</typeparam>
	public abstract class ConfigurationElementCollection<TElement> :
		ConfigurationElementCollection, IList<TElement>, IList
		where TElement : ConfigurationElement
	{
		/// <summary>
		/// 初始化 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 类的新实例。
		/// </summary>
		protected ConfigurationElementCollection() { }

		#region IList<TElement> 成员

		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <overloads>
		/// <summary>
		/// 获取或设置此 <see cref="ConfigurationElementCollection&lt;T&gt;"/> 对象的属性、特性或子元素。
		/// </summary>
		/// </overloads>
		public TElement this[int index]
		{
			get { return BaseGet(index) as TElement; }
			set
			{
				// 先添加后删除，如果添加失败，不会导致配置元素被删除。
				this.BaseAdd(index, value);
				BaseRemoveAt(index + 1);
			}
		}
		/// <summary>
		/// 确定 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中特定项的索引。
		/// </summary>
		/// <param name="item">要在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 
		/// 中定位的对象。</param>
		/// <returns>如果在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 
		/// 中找到 <paramref name="item"/>，则为该项的索引；否则为 <c>-1</c>。</returns>
		public int IndexOf(TElement item)
		{
			return BaseIndexOf(item);
		}
		/// <summary>
		/// 在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的指定索引处插入项。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
		/// <param name="item">要插入到 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的对象。</param>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的有效索引。</exception>
		public void Insert(int index, TElement item)
		{
			this.BaseAdd(index, item);
		}
		/// <summary>
		/// 移除指定索引处的 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 项。
		/// </summary>
		/// <param name="index">从零开始的索引（属于要移除的项）。</param>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的有效索引。</exception>
		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		#endregion // IList<TElement> 成员

		#region IList 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 是否具有固定大小。
		/// </summary>
		/// <value>如果 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 
		/// 具有固定大小，则为 <c>true</c>；否则为 <c>false</c>。</value>
		bool IList.IsFixedSize
		{
			get { return false; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 
		/// 为只读，则为 <c>true</c>；否则为 <c>false</c>。</value>
		bool IList.IsReadOnly
		{
			get { return base.IsReadOnly(); }
		}
		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获得或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的有效索引。</exception>
		object IList.this[int index]
		{
			get { return BaseGet(index); }
			set
			{
				if (!(value is TElement))
				{
					throw ExceptionHelper.ArgumentWrongType("value", value, typeof(TElement));
				}
				Contract.EndContractBlock();
				// 先添加后删除，如果添加失败，不会导致配置元素被删除。
				this.BaseAdd(index, (TElement)value);
				BaseRemoveAt(index + 1);
			}
		}
		/// <summary>
		/// 向 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中添加项。
		/// </summary>
		/// <param name="value">要添加到 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 的对象。</param>
		/// <returns>新元素所插入到的位置，或为 <c>-1</c> 以指示未将该项插入到集合中。</returns>
		int IList.Add(object value)
		{
			int idx = this.Count;
			TElement item = value as TElement;
			if (item == null)
			{
				throw ExceptionHelper.ArgumentWrongType("value", value, typeof(TElement));
			}
			this.BaseAdd(idx, item);
			return idx;
		}
		/// <summary>
		/// 从 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中移除所有项。
		/// </summary>
		void IList.Clear()
		{
			BaseClear();
		}
		/// <summary>
		/// 确定 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="value">要在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 
		/// 中找到 <paramref name="value"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		bool IList.Contains(object value)
		{
			TElement item = value as TElement;
			return item != null && this.Contains(item);
		}
		/// <summary>
		/// 确定 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中特定项的索引。
		/// </summary>
		/// <param name="value">要在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 
		/// 中找到 <paramref name="value"/>，则为该项的索引；否则为 <c>-1</c>。</returns>
		int IList.IndexOf(object value)
		{
			TElement item = value as TElement;
			if (item != null)
			{
				return BaseIndexOf(item);
			}
			return -1;
		}
		/// <summary>
		/// 在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的指定索引处插入项。
		/// </summary>
		/// <param name="index">应插入 <paramref name="value"/> 的位置的零始索引。</param>
		/// <param name="value">要插入 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的对象。</param>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的有效索引。</exception>
		void IList.Insert(int index, object value)
		{
			if (!(value is TElement))
			{
				throw ExceptionHelper.ArgumentWrongType("value", value, typeof(TElement));
			}
			Contract.EndContractBlock();
			this.BaseAdd(index, (TElement)value);
		}
		/// <summary>
		/// 从 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="value">要从 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中移除的对象。</param>
		void IList.Remove(object value)
		{
			TElement item = value as TElement;
			if (item != null)
			{
				this.Remove(item);
			}
		}
		/// <summary>
		/// 移除指定索引处的 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 项。
		/// </summary>
		/// <param name="index">要移除的项的从零开始的索引。</param>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中的有效索引。</exception>
		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		#endregion // IList 成员

		#region ICollection<TElement> 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		bool ICollection<TElement>.IsReadOnly
		{
			get { return base.IsReadOnly(); }
		}
		/// <summary>
		/// 将某项添加到 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中。
		/// </summary>
		/// <param name="item">要添加到 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 的对象。</param>
		public void Add(TElement item)
		{
			this.BaseAdd(item);
		}
		/// <summary>
		/// 从 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中移除所有项。
		/// </summary>
		public void Clear()
		{
			BaseClear();
		}
		/// <summary>
		/// 确定 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中定位的对象。</param>
		/// <returns>如果包含特定值，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(TElement item)
		{
			return BaseIndexOf(item) >= 0;
		}
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将 <see cref="ConfigurationElementCollection{TElement}"/> 的元素复制到一个 
		/// <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ConfigurationElementCollection{TElement}"/>
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException"><see cref="ConfigurationElementCollection{TElement}"/>
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <overloads>
		/// <summary>
		/// 将 <see cref="ConfigurationElementCollection{TElement}"/> 的元素复制到一个 
		/// <see cref="System.Array"/> 中。
		/// </summary>
		/// </overloads>
		public void CopyTo(TElement[] array, int arrayIndex)
		{
			CollectionHelper.CopyTo(this, array, arrayIndex);
		}
		/// <summary>
		/// 从 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中成功移除
		/// <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在原始 <see cref="ConfigurationElementCollection&lt;TElement&gt;"/> 中没有找到
		/// <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		public bool Remove(TElement item)
		{
			int idx = BaseIndexOf(item);
			if (idx < 0)
			{
				return false;
			}
			BaseRemoveAt(idx);
			return true;
		}

		#endregion // ICollection<TElement> 成员

		#region IEnumerable<TElement> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public new IEnumerator<TElement> GetEnumerator()
		{
			IEnumerator enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				yield return (TElement)enumerator.Current;
			}
		}

		#endregion // IEnumerable<TElement> 成员

	}
}
