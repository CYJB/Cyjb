using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using Cyjb.Reflection;
using JetBrains.Annotations;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 与字符串相关的类型转换器的提供者。
	/// </summary>
	internal sealed class StringConverterProvider : IConverterProvider
	{
		/// <summary>
		/// 默认的字符串类型转换器提供者。
		/// </summary>
		public static readonly IConverterProvider Default = new StringConverterProvider();
		/// <summary>
		/// 转换到字符串的方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo convertToStringMethod = typeof(StringConverterProvider)
			.GetMethod("ConvertToString", TypeExt.StaticFlag);
		/// <summary>
		/// 转换到枚举的方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly MethodInfo convertToEnumMethod = typeof(StringConverterProvider)
			.GetMethod("ConvertToEnum", TypeExt.StaticFlag);
		/// <summary>
		/// 初始化 <see cref="StringConverterProvider"/> 类的新实例。
		/// </summary>
		private StringConverterProvider() { }
		/// <summary>
		/// 获取类型转换器的源类型，与该类型相关的类型转换会查找当前提供者。
		/// </summary>
		/// <value>类型转换器的源类型。</value>
		public Type OriginType
		{
			get { return typeof(string); }
		}
		/// <summary>
		/// 返回将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <see cref="OriginType"/>，输出类型是 <paramref name="outputType"/>。</remarks>
		public Delegate GetConverterTo(Type outputType)
		{
			if (outputType.IsEnum)
			{
				return DelegateBuilder.CreateDelegate(Convert.GetConverterType(typeof(string), outputType), convertToEnumMethod);
			}
			switch (Type.GetTypeCode(outputType))
			{
				case TypeCode.Boolean:
					return new Converter<string, bool>(bool.Parse);
				case TypeCode.Char:
					return new Converter<string, char>(char.Parse);
				case TypeCode.SByte:
					return new Converter<string, sbyte>(sbyte.Parse);
				case TypeCode.Byte:
					return new Converter<string, byte>(byte.Parse);
				case TypeCode.Int16:
					return new Converter<string, short>(short.Parse);
				case TypeCode.UInt16:
					return new Converter<string, ushort>(ushort.Parse);
				case TypeCode.Int32:
					return new Converter<string, int>(int.Parse);
				case TypeCode.UInt32:
					return new Converter<string, uint>(uint.Parse);
				case TypeCode.Int64:
					return new Converter<string, long>(long.Parse);
				case TypeCode.UInt64:
					return new Converter<string, ulong>(ulong.Parse);
				case TypeCode.Single:
					return new Converter<string, float>(float.Parse);
				case TypeCode.Double:
					return new Converter<string, double>(double.Parse);
				case TypeCode.Decimal:
					return new Converter<string, decimal>(decimal.Parse);
				case TypeCode.DateTime:
					return new Converter<string, DateTime>(DateTime.Parse);
				case TypeCode.DBNull:
					return null;
			}
			return null;
		}
		/// <summary>
		/// 返回将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="inputType">输入对象的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <paramref name="inputType"/>，输出类型是 <see cref="OriginType"/>。</remarks>
		public Delegate GetConverterFrom(Type inputType)
		{
			return DelegateBuilder.CreateDelegate(Convert.GetConverterType(inputType, typeof(string)), convertToStringMethod);
		}
		/// <summary>
		/// 将指定类型转换为字符串。
		/// </summary>
		/// <typeparam name="TInput">要转换的数据类型。</typeparam>
		/// <returns>转换得到的字符串。</returns>
		[UsedImplicitly]
		private static string ConvertToString<TInput>(TInput value)
		{
			return value == null ? null : value.ToString();
		}
		/// <summary>
		/// 将指定字符串转换为枚举类型。
		/// </summary>
		/// <typeparam name="TOutput">要转换到的数据类型。</typeparam>
		/// <returns>转换得到的枚举类型。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[UsedImplicitly]
		private static TOutput ConvertToEnum<TOutput>(string value)
		{
			CommonExceptions.CheckArgumentNull(value, "value");
			Contract.EndContractBlock();
			return (TOutput)Enum.Parse(typeof(TOutput), value);
		}
	}
}
