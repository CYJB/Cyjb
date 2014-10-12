using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace Cyjb.ComponentModel
{
	/// <summary>
	/// 提供将 <see cref="Enum"/> 对象与其他各种表示形式相互转换的类型转换器。
	/// 支持枚举值的描述信息。
	/// </summary>
	public class EnumDescConverter : EnumConverter
	{
		/// <summary>
		/// 使用指定类型初始化 <see cref="EnumDescConverter"/> 类的新实例。
		/// </summary>
		/// <param name="type">表示与此转换器关联的枚举类型。</param>
		public EnumDescConverter(Type type)
			: base(type)
		{ }
		/// <summary>
		/// 将指定的值对象转换为枚举对象。
		/// </summary>
		/// <param name="context"><see cref="ITypeDescriptorContext"/>，提供格式上下文。</param>
		/// <param name="culture">一个可选的 <see cref="CultureInfo"/>。
		/// 如果未提供区域性设置，则使用当前区域性。</param>
		/// <param name="value">要转换的 <see cref="Object"/>。</param>
		/// <returns>表示转换的 <paramref name="value"/> 的 <see cref="Object"/>。</returns>
		/// <overloads>
		/// <summary>
		/// 将给定值转换为此转换器的类型。
		/// </summary>
		/// </overloads>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string strValue = value as string;
			if (strValue != null)
			{
				return EnumExt.ParseEx(this.EnumType, strValue, true);
			}
			return base.ConvertFrom(context, culture, value);
		}
		/// <summary>
		/// 将给定的值对象转换为指定的目标类型。
		/// </summary>
		/// <param name="context"><see cref="ITypeDescriptorContext"/>，提供格式上下文。</param>
		/// <param name="culture">一个可选的 <see cref="CultureInfo"/>。
		/// 如果未提供区域性设置，则使用当前区域性。</param>
		/// <param name="value">要转换的 <see cref="Object"/>。</param>
		/// <param name="destinationType">要将值转换成的 <see cref="Type"/>。</param>
		/// <returns>表示转换的 <paramref name="value"/> 的 <see cref="Object"/>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="destinationType"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 将给定值对象转换为指定的类型。
		/// </summary>
		/// </overloads>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
			object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw CommonExceptions.ArgumentNull("destinationType");
			}
			Contract.EndContractBlock();
			if (value != null && destinationType == typeof(string))
			{
				return ((Enum)value).GetDescription();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
