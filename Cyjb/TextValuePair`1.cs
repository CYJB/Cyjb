using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cyjb
{
	/// <summary>
	/// 表示泛型的文本-值对。
	/// </summary>
	/// <typeparam name="TValue">值的数据类型。</typeparam>
	[Serializable]
	public sealed class TextValuePair<TValue> : IEditableObject, INotifyPropertyChanged,
		INotifyPropertyChanging, IEquatable<TextValuePair<TValue>>
	{
		/// <summary>
		/// 文本。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string text;
		/// <summary>
		/// 文本对应的值。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TValue pairValue;
		/// <summary>
		/// 是否正在编辑对象中。
		/// </summary>
		[NonSerialized]
		private bool isInEdit;
		/// <summary>
		/// 编辑对象前的 Text 值。
		/// </summary>
		[NonSerialized]
		private string savedText;
		/// <summary>
		/// 编辑对象前的 Value 值。
		/// </summary>
		[NonSerialized]
		private TValue savedValue;
		/// <summary>
		/// 初始化 <see cref="Cyjb.TextValuePair"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="Cyjb.TextValuePair"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public TextValuePair() { }
		/// <summary>
		/// 使用指定的文本和值初始化 <see cref="Cyjb.TextValuePair"/> 
		/// 类的新实例。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="value">文本对应的值。</param>
		public TextValuePair(string text, TValue value)
		{
			this.text = text;
			this.pairValue = value;
		}
		/// <summary>
		/// 获取或设置文本。
		/// </summary>
		/// <value>文本。</value>
		public string Text
		{
			get { return text; }
			set
			{
				OnPropertyChanging("Text");
				text = value;
				OnPropertyChanged("Text");
			}
		}
		/// <summary>
		/// 获取或设置文本对应的值。
		/// </summary>
		/// <value>文本对应的值。</value>
		public TValue Value
		{
			get { return pairValue; }
			set
			{
				OnPropertyChanging("Value");
				this.pairValue = value;
				OnPropertyChanged("Value");
			}
		}

		/// <summary>
		/// 使用文本和值的字符串表示形式返回 
		/// <see cref="Cyjb.TextValuePair"/> 的字符串表示形式。
		/// </summary>
		/// <returns><see cref="Cyjb.TextValuePair"/> 
		/// 的字符串表示形式，它包括文本和值的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Concat(text, ": ", pairValue);
		}

		#region INotifyPropertyChanged 成员

		/// <summary>
		/// 当属性更改后发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// 调度属性更改后的事件。
		/// </summary>
		/// <param name="propertyName">已经更改的属性名。</param>
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler changed = PropertyChanged;
			if (!isInEdit && changed != null)
			{
				changed(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region INotifyPropertyChanging 成员

		/// <summary>
		/// 当属性值将要更改时发生。
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;
		/// <summary>
		/// 调度属性值将要更改的事件。
		/// </summary>
		/// <param name="propertyName">将要更改的属性名。</param>
		private void OnPropertyChanging(string propertyName)
		{
			PropertyChangingEventHandler changing = PropertyChanging;
			if (!isInEdit && changing != null)
			{
				changing(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		#endregion

		#region IEditableObject 成员

		/// <summary>
		/// 开始编辑对象。
		/// </summary>
		void IEditableObject.BeginEdit()
		{
			if (!isInEdit)
			{
				isInEdit = true;
				savedText = text;
				savedValue = pairValue;
			}
		}
		/// <summary>
		/// 放弃上一次 <see cref="IEditableObject.BeginEdit"/> 调用之后的更改。
		/// </summary>
		void IEditableObject.CancelEdit()
		{
			if (isInEdit)
			{
				text = savedText;
				pairValue = savedValue;
				isInEdit = false;
			}
		}
		/// <summary>
		/// 将上一次 <see cref="IEditableObject.BeginEdit"/> 或
		/// <see cref="IBindingList.AddNew"/> 调用之后所进行的更改
		/// 推到基础对象中。
		/// </summary>
		void IEditableObject.EndEdit()
		{
			if (isInEdit)
			{
				string tmpText = text;
				TValue tmpValue = pairValue;
				text = savedText;
				pairValue = savedValue;
				isInEdit = false;
				EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
				if (text == tmpText)
				{
					if (!comparer.Equals(pairValue, tmpValue))
					{
						Value = tmpValue;
					}
				}
				else
				{
					if (comparer.Equals(pairValue, tmpValue))
					{
						Text = tmpText;
					}
					else
					{
						OnPropertyChanging(string.Empty);
						text = tmpText;
						pairValue = tmpValue;
						OnPropertyChanged(string.Empty);
					}
				}
			}
		}

		#endregion

		#region IEquatable<TextValuePair<TValue>> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/> 参数，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 指示当前对象是否等于另一个对象。
		/// </summary>
		/// </overloads>
		public bool Equals(TextValuePair<TValue> other)
		{
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (!string.Equals(text, other.text, StringComparison.CurrentCulture))
			{
				return false;
			}
			return EqualityComparer<TValue>.Default.Equals(pairValue, other.pairValue);
		}

		#endregion // IEquatable<TextValuePair<TValue>> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 
		/// <see cref="Cyjb.TextValuePair"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="Cyjb.TextValuePair"/> 
		/// 进行比较的 <see cref="System.Object"/>。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 
		/// <see cref="Cyjb.TextValuePair"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			TextValuePair<TValue> thisObj = obj as TextValuePair<TValue>;
			if (object.ReferenceEquals(thisObj, null))
			{
				return false;
			}
			return this.Equals(thisObj);
		}

		/// <summary>
		/// 用于 <see cref="Cyjb.TextValuePair"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="Cyjb.TextValuePair"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			int hashCode = 127;
			if (text != null)
			{
				hashCode ^= text.GetHashCode();
			}
			if (pairValue != null)
			{
				hashCode ^= pairValue.GetHashCode();
			}
			return hashCode;
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="Cyjb.TextValuePair"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Cyjb.TextValuePair"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Cyjb.TextValuePair"/> 对象。</param>
		/// <returns>如果两个 <see cref="Cyjb.TextValuePair"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(TextValuePair<TValue> obj1, TextValuePair<TValue> obj2)
		{
			if (object.ReferenceEquals(obj1, obj2))
			{
				return true;
			}
			if (object.ReferenceEquals(obj1, null))
			{
				return object.ReferenceEquals(obj2, null);
			}
			return obj1.Equals(obj2);
		}

		/// <summary>
		/// 判断两个 <see cref="Cyjb.TextValuePair"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Cyjb.TextValuePair"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Cyjb.TextValuePair"/> 对象。</param>
		/// <returns>如果两个 <see cref="Cyjb.TextValuePair"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(TextValuePair<TValue> obj1, TextValuePair<TValue> obj2)
		{
			return !(obj1 == obj2);
		}
		/// <summary>
		/// 到 <see cref="TextValuePair"/> 类型的隐式类型转换。
		/// </summary>
		/// <param name="pair">要转换类型的 <see cref="TextValuePair{TValue}"/> 实例。</param>
		/// <returns>类型转换的结果。</returns>
		public static implicit operator TextValuePair(TextValuePair<TValue> pair)
		{
			return new TextValuePair(pair.text, pair.pairValue);
		}
		/// <summary>
		/// 从 <see cref="TextValuePair"/> 类型的显式类型转换。
		/// </summary>
		/// <param name="pair">要转换类型的 <see cref="TextValuePair"/> 实例。</param>
		/// <returns>类型转换的结果。</returns>
		public static explicit operator TextValuePair<TValue>(TextValuePair pair)
		{
			if (pair.Value == null)
			{
				return new TextValuePair<TValue>(pair.Text, default(TValue));
			}
			else if (pair.Value is TValue)
			{
				return new TextValuePair<TValue>(pair.Text, (TValue)pair.Value);
			}
			else
			{
				throw CommonExceptions.ArgumentWrongType("pair");
			}
		}

		#endregion // 运算符重载

	}
}
