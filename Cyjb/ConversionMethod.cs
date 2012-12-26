using System;

namespace Cyjb
{
	/// <summary>
	/// 表示用户定义的类型转换方法。
	/// </summary>
	internal struct ConversionMethod : IEquatable<ConversionMethod>
	{
		/// <summary>
		/// 类型转换方法的类型。
		/// </summary>
		public ConversionType ConversionType;
		/// <summary>
		/// 类型转换自方法的句柄。
		/// </summary>
		public RuntimeMethodHandle FromMethod;
		/// <summary>
		/// 类型转换到方法的句柄。
		/// </summary>
		public RuntimeMethodHandle ToMethod;

		#region IEquatable<ConversionMethod> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/> 参数，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Equals(ConversionMethod other)
		{
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			if (ConversionType != other.ConversionType)
			{
				return false;
			}
			if (FromMethod != other.FromMethod)
			{
				return false;
			}
			return ToMethod == other.ToMethod;
		}

		#endregion // IEquatable<ConversionMethod> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="ConversionMethod"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="ConversionMethod"/> 进行比较的 object。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="ConversionMethod"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is ConversionMethod))
			{
				return false;
			}
			return this.Equals((ConversionMethod)obj);
		}

		/// <summary>
		/// 用于 <see cref="ConversionMethod"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="ConversionMethod"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			return ConversionType.GetHashCode() ^ FromMethod.GetHashCode() ^ ToMethod.GetHashCode();
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="ConversionMethod"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="ConversionMethod"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="ConversionMethod"/> 对象。</param>
		/// <returns>如果两个 <see cref="ConversionMethod"/> 对象相同，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator ==(ConversionMethod obj1, ConversionMethod obj2)
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
		/// 判断两个 <see cref="ConversionMethod"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="ConversionMethod"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="ConversionMethod"/> 对象。</param>
		/// <returns>如果两个 <see cref="ConversionMethod"/> 对象不同，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator !=(ConversionMethod obj1, ConversionMethod obj2)
		{
			return !(obj1 == obj2);
		}

		#endregion // 运算符重载

	}
}
