using System;
using System.ComponentModel;
using System.Diagnostics;
using Cyjb.ComponentModel;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 表示简单的文本-值对。
	/// </summary>
	[Serializable]
	public sealed class TextValuePair : ObservableObject, IEditableObject, IEquatable<TextValuePair>
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
		private object pairValue;
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
		private object savedValue;
		/// <summary>
		/// 初始化 <see cref="TextValuePair"/> 类的新实例。
		/// </summary>
		public TextValuePair()
		{ }
		/// <summary>
		/// 使用指定的文本和值初始化 <see cref="TextValuePair"/> 
		/// 类的新实例。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="value">文本对应的值。</param>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="TextValuePair"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public TextValuePair(string text, object value)
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
				UpdateValue("Text", ref text, value, !isInEdit);
			}
		}
		/// <summary>
		/// 获取或设置文本对应的值。
		/// </summary>
		/// <value>文本对应的值。</value>
		public object Value
		{
			get { return pairValue; }
			set
			{
				UpdateValue("Value", ref pairValue, value, !isInEdit);
			}
		}

		/// <summary>
		/// 使用文本和值的字符串表示形式返回 
		/// <see cref="TextValuePair"/> 的字符串表示形式。
		/// </summary>
		/// <returns><see cref="TextValuePair"/> 的字符串表示形式，它包括文本和值的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Concat(text, ": ", pairValue);
		}

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
				object tmpValue = pairValue;
				text = savedText;
				pairValue = savedValue;
				isInEdit = false;
				if (text == tmpText)
				{
					if (pairValue != tmpValue)
					{
						Value = tmpValue;
					}
				}
				else
				{
					if (pairValue == tmpValue)
					{
						Text = tmpText;
					}
					else
					{
						RaisePropertyChanging(string.Empty);
						text = tmpText;
						pairValue = tmpValue;
						RaisePropertyChanged(string.Empty);
					}
				}
			}
		}

		#endregion

		#region IEquatable<TextValuePair> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/> 参数，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 指示当前对象是否等于另一个对象。
		/// </summary>
		/// </overloads>
		public bool Equals(TextValuePair other)
		{
			if (ReferenceEquals(other, this))
			{
				return true;
			}
			if (ReferenceEquals(other, null))
			{
				return false;
			}
			if (!string.Equals(text, other.text, StringComparison.CurrentCulture))
			{
				return false;
			}
			return pairValue == other.pairValue;
		}

		#endregion // IEquatable<TextValuePair> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="Object"/> 是否等于当前的 <see cref="TextValuePair"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="TextValuePair"/> 进行比较的 <see cref="Object"/>。</param>
		/// <returns>如果指定的 <see cref="Object"/> 等于当前的 <see cref="TextValuePair"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			TextValuePair thisObj = obj as TextValuePair;
			return !ReferenceEquals(thisObj, null) && this.Equals(thisObj);
		}

		/// <summary>
		/// 用于 <see cref="TextValuePair"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="TextValuePair"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			return Hash.Combine(text == null ? 0 : text.GetHashCode(), pairValue);
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="TextValuePair"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="TextValuePair"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="TextValuePair"/> 对象。</param>
		/// <returns>如果两个 <see cref="TextValuePair"/> 对象相同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator ==(TextValuePair obj1, TextValuePair obj2)
		{
			if (ReferenceEquals(obj1, obj2))
			{
				return true;
			}
			return !ReferenceEquals(obj1, null) && obj1.Equals(obj2);
		}

		/// <summary>
		/// 判断两个 <see cref="TextValuePair"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="TextValuePair"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="TextValuePair"/> 对象。</param>
		/// <returns>如果两个 <see cref="TextValuePair"/> 对象不同，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public static bool operator !=(TextValuePair obj1, TextValuePair obj2)
		{
			return !(obj1 == obj2);
		}

		#endregion // 运算符重载

	}
}
