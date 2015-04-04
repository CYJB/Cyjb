using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Cyjb.Collections;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Configurations
{
	/// <summary>
	/// 表示包含值的配置元素集合。
	/// </summary>
	public sealed class ValueConfigurationCollection : ConfigurationElementCollection<ValueConfigurationElement>,
		IList<string>, IList
	{
		/// <summary>
		/// 初始化 <see cref="ValueConfigurationCollection"/> 类的新实例。
		/// </summary>
		public ValueConfigurationCollection() { }
		/// <summary>
		/// 使用指定的值的集合，初始化 <see cref="ValueConfigurationCollection"/> 类的新实例。
		/// </summary>
		/// <param name="values">初始化的值的集合。</param>
		public ValueConfigurationCollection(IEnumerable<string> values)
		{
			this.AddRange(values);
		}

		#region ConfigurationElementCollection 成员

		/// <summary>
		/// 创建一个新的 <see cref="ConfigurationElement"/>。
		/// </summary>
		/// <returns>新的 <see cref="ConfigurationElement"/>。</returns>
		/// <overloads>
		/// <summary>
		/// 创建一个新的 <see cref="ConfigurationElement"/>。
		/// </summary>
		/// </overloads>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ValueConfigurationElement();
		}
		/// <summary>
		/// 获取指定配置元素的元素键。
		/// </summary>
		/// <param name="element">要为其返回键的 <see cref="ConfigurationElement"/>。</param>
		/// <returns>一个 <see cref="object"/>，用作指定 <see cref="ConfigurationElement"/> 的键。
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ValueConfigurationElement)element).Value;
		}

		#endregion // ConfigurationElementCollection 成员

		#region IList<string> 成员

		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <overloads>
		/// <summary>
		/// 获取或设置此 <see cref="ValueConfigurationCollection"/> 对象的属性、特性或子元素。
		/// </summary>
		/// </overloads>
		public new string this[int index]
		{
			get
			{
				ValueConfigurationElement element = BaseGet(index) as ValueConfigurationElement;
				return element == null ? null : element.Value;
			}
			set
			{
				// 先添加后删除，如果添加失败，不会导致配置元素被删除。
				this.BaseAdd(index, new ValueConfigurationElement(value));
				BaseRemoveAt(index + 1);
			}
		}
		/// <summary>
		/// 确定 <see cref="ValueConfigurationCollection"/> 中特定项的索引。
		/// </summary>
		/// <param name="value">要在 <see cref="ValueConfigurationCollection"/> 中定位的值。</param>
		/// <returns>如果在 <see cref="ValueConfigurationCollection"/> 
		/// 中找到 <paramref name="value"/>，则为该项的索引；否则为 <c>-1</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 确定 <see cref="ValueConfigurationCollection"/> 中特定项的索引。
		/// </summary>
		/// </overloads>
		public int IndexOf(string value)
		{
			int cnt = this.Count;
			for (int i = 0; i < cnt; i++)
			{
				ValueConfigurationElement element = BaseGet(i) as ValueConfigurationElement;
				if (element != null && element.Value == value)
				{
					return i;
				}
			}
			return -1;
		}
		/// <summary>
		/// 在 <see cref="ValueConfigurationCollection"/> 中的指定索引处插入项。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 <paramref name="value"/>。</param>
		/// <param name="value">要插入到 <see cref="ValueConfigurationCollection"/> 中的值。</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ValueConfigurationCollection"/> 中的有效索引。</exception>
		/// <overloads>
		/// <summary>
		/// 在 <see cref="ValueConfigurationCollection"/> 中的指定索引处插入项。
		/// </summary>
		/// </overloads>
		public void Insert(int index, string value)
		{
			this.BaseAdd(index, new ValueConfigurationElement(value));
		}

		#endregion // IList<string> 成员

		#region IList 成员

		/// <summary>
		/// 获取或设置指定索引处的元素。
		/// </summary>
		/// <param name="index">要获得或设置的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ValueConfigurationCollection"/> 中的有效索引。</exception>
		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				string str = value as string;
				if (str != null)
				{
					this[index] = str;
					return;
				}
				ValueConfigurationElement element = value as ValueConfigurationElement;
				if (element == null)
				{
					throw CommonExceptions.ArgumentWrongType("value");
				}
				base[index] = element;
			}
		}
		/// <summary>
		/// 向 <see cref="ValueConfigurationCollection"/> 中添加项。
		/// </summary>
		/// <param name="value">要添加到 <see cref="ValueConfigurationCollection"/> 的对象。</param>
		/// <returns>新元素所插入到的位置，或为 <c>-1</c> 以指示未将该项插入到集合中。</returns>
		int IList.Add(object value)
		{
			int idx = this.Count;
			string str = value as string;
			if (str != null)
			{
				this.Add(str);
				return idx;
			}
			ValueConfigurationElement element = value as ValueConfigurationElement;
			if (element == null)
			{
				throw CommonExceptions.ArgumentWrongType("value");
			}
			this.BaseAdd(idx, element);
			return idx;
		}
		/// <summary>
		/// 确定 <see cref="ValueConfigurationCollection"/> 是否包含特定值。
		/// </summary>
		/// <param name="value">要在 <see cref="ValueConfigurationCollection"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ValueConfigurationCollection"/> 
		/// 中找到 <paramref name="value"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		bool IList.Contains(object value)
		{
			string item = value as string;
			return item != null && this.Contains(item);
		}
		/// <summary>
		/// 确定 <see cref="ValueConfigurationCollection"/> 中特定项的索引。
		/// </summary>
		/// <param name="value">要在 <see cref="ValueConfigurationCollection"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ValueConfigurationCollection"/> 
		/// 中找到 <paramref name="value"/>，则为该项的索引；否则为 <c>-1</c>。</returns>
		int IList.IndexOf(object value)
		{
			string str = value as string;
			if (str != null)
			{
				return this.IndexOf(str);
			}
			ValueConfigurationElement element = value as ValueConfigurationElement;
			if (element != null)
			{
				return this.BaseIndexOf(element);
			}
			return -1;
		}
		/// <summary>
		/// 在 <see cref="ValueConfigurationCollection"/> 中的指定索引处插入项。
		/// </summary>
		/// <param name="index">应插入 <paramref name="value"/> 的位置的零始索引。</param>
		/// <param name="value">要插入 <see cref="ValueConfigurationCollection"/> 中的对象。</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 
		/// 不是 <see cref="ValueConfigurationCollection"/> 中的有效索引。</exception>
		void IList.Insert(int index, object value)
		{
			string str = value as string;
			if (str != null)
			{
				this.Insert(index, str);
				return;
			}
			ValueConfigurationElement element = value as ValueConfigurationElement;
			if (element == null)
			{
				throw CommonExceptions.ArgumentWrongType("value");
			}
			this.BaseAdd(index, element);
		}
		/// <summary>
		/// 从 <see cref="ValueConfigurationCollection"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="value">要从 <see cref="ValueConfigurationCollection"/> 中移除的对象。</param>
		void IList.Remove(object value)
		{
			string item = value as string;
			if (item != null)
			{
				this.Remove(item);
			}
		}

		#endregion // IList 成员

		#region ICollection<string> 成员

		/// <summary>
		/// 获取一个值，该值指示 <see cref="ValueConfigurationCollection"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ValueConfigurationCollection"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		bool ICollection<string>.IsReadOnly
		{
			get { return base.IsReadOnly(); }
		}
		/// <summary>
		/// 将指定的值添加到 <see cref="ValueConfigurationCollection"/>。
		/// </summary>
		/// <param name="value">要添加到 <see cref="ValueConfigurationCollection"/> 的值。</param>
		/// <overloads>
		/// <summary>
		/// 将某项添加到 <see cref="ValueConfigurationCollection"/> 中。
		/// </summary>
		/// </overloads>
		public void Add(string value)
		{
			this.BaseAdd(new ValueConfigurationElement(value));
		}
		/// <summary>
		/// 确定 <see cref="ValueConfigurationCollection"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="ValueConfigurationCollection"/> 中定位的对象。</param>
		/// <returns>如果包含特定值，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(string item)
		{
			return this.IndexOf(item) >= 0;
		}
		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将 <see cref="ValueConfigurationCollection"/> 
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ValueConfigurationCollection"/>
		/// 复制的元素的目标位置的一维 <see cref="Array"/>。
		/// <paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><see cref="ValueConfigurationCollection"/>
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <overloads>
		/// <summary>
		/// 将 <see cref="ValueConfigurationCollection"/> 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// </overloads>
		public void CopyTo(string[] array, int arrayIndex)
		{
			CollectionHelper.CopyTo<string>(this, array, arrayIndex);
		}
		/// <summary>
		/// 从 <see cref="ValueConfigurationCollection"/> 中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从 <see cref="ValueConfigurationCollection"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="ValueConfigurationCollection"/> 中成功移除
		/// <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果在原始 <see cref="ValueConfigurationCollection"/> 中没有找到
		/// <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		public bool Remove(string item)
		{
			int idx = this.IndexOf(item);
			if (idx < 0)
			{
				return false;
			}
			BaseRemoveAt(idx);
			return true;
		}

		#endregion // ICollection<string> 成员

		#region IEnumerable<string> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/>。</returns>
		public new IEnumerator<string> GetEnumerator()
		{
			IEnumerator<ValueConfigurationElement> enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current.Value;
			}
		}

		#endregion // IEnumerable<string> 成员

	}
}
