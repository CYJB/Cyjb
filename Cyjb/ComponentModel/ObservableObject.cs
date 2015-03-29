using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb.ComponentModel
{
	/// <summary>
	/// 表示提供属性更改通知的对象。
	/// </summary>
	public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
	{

		#region INotifyPropertyChanged 成员

		/// <summary>
		/// 当属性更改后发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// 调度属性更改后的事件。
		/// </summary>
		/// <param name="propertyName">已经更改的属性名。</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				propertyName = null;
			}
			else
			{
				CheckPropertyName(propertyName);
			}
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
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
		protected virtual void OnPropertyChanging(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				propertyName = null;
			}
			else
			{
				CheckPropertyName(propertyName);
			}
			PropertyChangingEventHandler handler = PropertyChanging;
			if (handler != null)
			{
				handler(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		#endregion

		/// <summary>
		/// 更新指定的值，并引发属性更改事件。
		/// </summary>
		/// <typeparam name="T">值的类型。</typeparam>
		/// <param name="propertyName">要更改的属性名称。</param>
		/// <param name="value">属性的旧值。</param>
		/// <param name="newValue">属性的新值。</param>
		/// <returns>如果指定的值被更新，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		protected bool UpdateValue<T>(string propertyName, ref T value, T newValue)
		{
			if (EqualityComparer<T>.Default.Equals(value, newValue))
			{
				return false;
			}
			OnPropertyChanging(propertyName);
			value = newValue;
			OnPropertyChanged(propertyName);
			return true;
		}
		/// <summary>
		/// 更新指定的值，并引发属性更改事件。
		/// </summary>
		/// <typeparam name="T">值的类型。</typeparam>
		/// <param name="propertyName">要更改的属性名称。</param>
		/// <param name="value">属性的旧值。</param>
		/// <param name="newValue">属性的新值。</param>
		/// <param name="raiseEvent">是否引发属性更改事件。</param>
		/// <returns>如果指定的值被更新，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		protected bool UpdateValue<T>(string propertyName, ref T value, T newValue, bool raiseEvent)
		{
			if (EqualityComparer<T>.Default.Equals(value, newValue))
			{
				return false;
			}
			if (raiseEvent)
			{
				OnPropertyChanging(propertyName);
				value = newValue;
				OnPropertyChanged(propertyName);
			}
			else
			{
				value = newValue;
			}
			return true;
		}
		/// <summary>
		/// 检查属性的名称是否正确。
		/// </summary>
		/// <param name="propertyName">要检查的属性名称。</param>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		private void CheckPropertyName(string propertyName)
		{
			Contract.Requires(!string.IsNullOrEmpty(propertyName));
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				throw CommonExceptions.PropertyNotFound(propertyName);
			}
		}
	}
}
