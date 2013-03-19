using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供用于异常处理的辅助方法。
	/// </summary>
	public static class ExceptionHelper
	{

		#region ArgumentException

		#region 数组异常

		/// <summary>
		/// 返回数组为空的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayEmpty(string paramName)
		{
			return GetArgumentException(paramName, "ArrayEmpty");
		}
		/// <summary>
		/// 返回数组下限不为 <c>0</c> 的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayNonZeroLowerBound(string paramName)
		{
			return GetArgumentException(paramName, "ArrayNonZeroLowerBound");
		}
		/// <summary>
		/// 返回数组太小而不能复制集合的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayTooSmall(string paramName)
		{
			return GetArgumentException(paramName, "ArrayTooSmall");
		}
		/// <summary>
		/// 检查数组是否是一维数组，并具有从 <c>0</c> 开始的下限；
		/// 如果不是，则抛出异常。
		/// </summary>
		/// <param name="array">要检查的数组。</param>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <exception cref="System.ArgumentException"><paramref name="array"/>是多维的。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="array"/>的下限不为 <c>0</c>。</exception>
		public static void CheckFlatArray(Array array, string paramName)
		{
			if (array.Rank != 1)
			{
				throw ArrayRankMultiDimNotSupported();
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw ArrayNonZeroLowerBound(paramName);
			}
		}
		/// <summary>
		/// 返回偏移量和长度超出界限的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException InvalidOffsetLength()
		{
			return new ArgumentException(ExceptionResources.GetString("InvalidOffsetLength"));
		}

		#endregion // 数组异常

		/// <summary>
		/// 返回至少有一个对象实现 <see cref="System.IComparable"/> 的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentImplementIComparable(string paramName)
		{
			return GetArgumentException(paramName, "ArgumentImplementIComparable");
		}
		/// <summary>
		/// 返回泛型集合的参数类型错误的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="value">错误的参数值。</param>
		/// <param name="targetType">目标类型的值。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentWrongType(string paramName, object value, Type targetType)
		{
			return GetArgumentException(paramName, "ArgumentWrongType", value, targetType);
		}
		/// <summary>
		/// 返回未能推导类型参数的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="method">未能推导参数的方法。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException CannotInferGenericArguments(string paramName, MethodBase method)
		{
			return GetArgumentException(paramName, "CannotInferGenericArguments", method);
		}
		/// <summary>
		/// 返回枚举参数类型不匹配的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="argType">参数类型。</param>
		/// <param name="baseType">基类型。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException EnumTypeDoesNotMatch(string paramName, Type argType, Type baseType)
		{
			return GetArgumentException(paramName, "EnumTypeDoesNotMatch", argType, baseType);
		}
		/// <summary>
		/// 返回键重复的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException KeyDuplicate(string paramName)
		{
			return GetArgumentException(paramName, "KeyDuplicate");
		}
		/// <summary>
		/// 返回必须是枚举的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">异常的类型。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException MustBeEnum(string paramName, Type type)
		{
			return GetArgumentException(paramName, "MustBeEnum", type);
		}
		/// <summary>
		/// 返回必须包含枚举信息的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException MustContainEnumInfo(string paramName)
		{
			return GetArgumentException(paramName, "MustContainEnumInfo");
		}

		#region 获取异常对象

		/// <summary>
		/// 返回参数异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		private static ArgumentException GetArgumentException(string paramName, string resName)
		{
			return new ArgumentException(ExceptionResources.GetString(resName), paramName);
		}
		/// <summary>
		/// 返回参数异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="args">格式化信息的参数。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		private static ArgumentException GetArgumentException(string paramName, string resName, params object[] args)
		{
			string message = ExceptionResources.GetString(resName, args);
			return new ArgumentException(message, paramName);
		}

		#endregion // 获取异常对象

		#endregion // ArgumentException

		#region ArgumentNullException

		/// <summary>
		/// 返回参数为 <c>null</c> 的异常。
		/// </summary>
		/// <param name="paramName">为 <c>null</c> 的参数名。</param>
		/// <returns><see cref="System.ArgumentNullException"/> 对象。</returns>
		public static ArgumentNullException ArgumentNull(string paramName)
		{
			return new ArgumentNullException(paramName);
		}
		/// <summary>
		/// 检查参数是否为 <c>null</c>，如果为 <c>null</c> 则抛出异常。
		/// </summary>
		/// <param name="value">要检查的参数值。</param>
		/// <param name="paramName">要检查的参数名。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		public static void CheckArgumentNull(object value, string paramName)
		{
			if (value == null)
			{
				throw ArgumentNull(paramName);
			}
		}
		/// <summary>
		/// 检查参数是否为 <c>null</c>，如果为 <c>null</c> 则抛出异常。
		/// 对于值类型，不会抛出异常。
		/// </summary>
		/// <typeparam name="T">要检查的参数的类型。</typeparam>
		/// <param name="value">要检查的参数值。</param>
		/// <param name="paramName">要检查的参数名。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		public static void CheckArgumentNull<T>(T value, string paramName)
		{
			if (value == null)
			{
				throw ArgumentNull(paramName);
			}
		}

		#endregion // ArgumentNullException

		#region ArgumentOutOfRangeException

		/// <summary>
		/// 返回参数小于等于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <returns><see cref="System.ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentMustBePositive(string paramName)
		{
			return GetArgumentOutOfRangeException(paramName, "ArgumentMustBePositive");
		}
		/// <summary>
		/// 返回参数小于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <returns><see cref="System.ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentNegative(string paramName)
		{
			return GetArgumentOutOfRangeException(paramName, "ArgumentNegative");
		}
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <returns><see cref="System.ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName)
		{
			return GetArgumentOutOfRangeException(paramName, "ArgumentOutOfRange");
		}
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <param name="begin">参数有效范围的起始值。</param>
		/// <param name="end">参数有效范围的结束值。</param>
		/// <returns><see cref="System.ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object begin, object end)
		{
			return GetArgumentOutOfRangeException(paramName, "ArgumentOutOfRangeBetween");
		}
		/// <summary>
		/// 返回基无效的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentOutOfRangeException"/> 对象。</returns>
		internal static ArgumentOutOfRangeException InvalidBase(string paramName)
		{
			return GetArgumentOutOfRangeException(paramName, "InvalidBase");
		}
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="paramName">发生异常的参数名称。</param>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.ArgumentOutOfRangeException"/> 对象。</returns>
		private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(string paramName,
			string resName)
		{
			return new ArgumentOutOfRangeException(paramName, ExceptionResources.GetString(resName));
		}

		#endregion // ArgumentOutOfRangeException

		#region ArrayTypeMismatchException

		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// <returns><see cref="System.ArrayTypeMismatchException"/> 对象。</returns>
		public static ArrayTypeMismatchException ArrayTypeInvalid()
		{
			return new ArrayTypeMismatchException(ExceptionResources.GetString("ArrayTypeInvalid"));
		}
		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// <param name="innerException">内部异常引用。</param>
		/// <returns><see cref="System.ArrayTypeMismatchException"/> 对象。</returns>
		public static ArrayTypeMismatchException ArrayTypeInvalid(Exception innerException)
		{
			return new ArrayTypeMismatchException(ExceptionResources.GetString("ArrayTypeInvalid"), innerException);
		}

		#endregion // ArrayTypeMismatchException

		#region FormatException

		/// <summary>
		/// 返回基不为 <c>10</c> 的字符串包含减号的异常。
		/// </summary>
		/// <returns><see cref="System.FormatException"/> 对象。</returns>
		public static FormatException BaseConvertNegativeValue()
		{
			return GetFormatException("BaseConvertNegativeValue");
		}
		/// <summary>
		/// 返回未识别的枚举值的异常。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="value">未识别的枚举值。</param>
		/// <returns><see cref="System.FormatException"/> 对象。</returns>
		public static FormatException EnumValueNotFound(Type enumType, object value)
		{
			return GetFormatException("EnumValueNotFound", enumType, value);
		}
		/// <summary>
		/// 返回字符串末尾有其它无法分析的字符的异常。
		/// </summary>
		/// <returns><see cref="System.FormatException"/> 对象。</returns>
		public static FormatException ExtraJunkAtEnd()
		{
			return GetFormatException("ExtraJunkAtEnd");
		}
		/// <summary>
		/// 返回找不到可识别的数字的异常。
		/// </summary>
		/// <returns><see cref="System.FormatException"/> 对象。</returns>
		public static FormatException NoParsibleDigits()
		{
			return GetFormatException("NoParsibleDigits");
		}
		/// <summary>
		/// 返回以特定名称字符串资源为信息的
		/// <see cref="System.FormatException"/> 对象。
		/// </summary>
		/// <param name="resName">作为异常信息的字符串资源名称。</param>
		/// <param name="args">异常信息的格式化值。</param>
		/// <returns><see cref="System.FormatException"/> 对象。</returns>
		private static FormatException GetFormatException(string resName, params object[] args)
		{
			string message = ExceptionResources.GetString(resName, args);
			return new FormatException(message);
		}
		/// <summary>
		/// 返回以特定名称字符串资源为信息的
		/// <see cref="System.FormatException"/> 对象。
		/// </summary>
		/// <param name="resName">作为异常信息的字符串资源名称。</param>
		/// <returns><see cref="System.FormatException"/> 对象。</returns>
		private static FormatException GetFormatException(string resName)
		{
			string message = ExceptionResources.GetString(resName);
			return new FormatException(message);
		}

		#endregion // FormatException

		#region InvalidCastException

		/// <summary>
		/// 返回空对象不能转换为值类型的异常。
		/// </summary>
		/// <returns><see cref="System.InvalidCastException"/> 对象。</returns>
		public static InvalidCastException CannotCastNullToValueType()
		{
			return GetInvalidCast("CannotCastNullToValueType");
		}
		/// <summary>
		/// 返回转换无效的异常。
		/// </summary>
		/// <param name="value">无效的值。</param>
		/// <param name="type">要转换到的类型。</param>
		/// <returns><see cref="System.InvalidCastException"/> 对象。</returns>
		public static InvalidCastException ConvertInvalidValue(object value, Type type)
		{
			return GetInvalidCast("ConvertInvalidValue", value, type);
		}
		/// <summary>
		/// 返回转换无效的异常。
		/// </summary>
		/// <param name="value">无效的值。</param>
		/// <param name="type">要转换到的类型。</param>
		/// <param name="innerException">内部异常。</param>
		/// <returns><see cref="System.InvalidCastException"/> 对象。</returns>
		public static InvalidCastException ConvertInvalidValue(object value, Type type, Exception innerException)
		{
			string message = ExceptionResources.GetString("ConvertInvalidValue", value, type);
			return new InvalidCastException(message, innerException);
		}
		/// <summary>
		/// 返回转换无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.InvalidCastException"/> 对象。</returns>
		private static InvalidCastException GetInvalidCast(string resName)
		{
			return new InvalidCastException(ExceptionResources.GetString(resName));
		}
		/// <summary>
		/// 返回转换无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="args">异常信息的格式化值。</param>
		/// <returns><see cref="System.InvalidCastException"/> 对象。</returns>
		private static InvalidCastException GetInvalidCast(string resName, params object[] args)
		{
			return new InvalidCastException(ExceptionResources.GetString(resName, args));
		}

		#endregion // InvalidCastException

		#region  InvalidOperationException

		/// <summary>
		/// 返回无法执行枚举操作的异常。
		/// </summary>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException EnumFailedVersion()
		{
			return GetInvalidOperation("EnumFailedVersion");
		}
		/// <summary>
		/// 返回不表示泛型方法定义的异常。
		/// </summary>
		/// <param name="method">不是泛型方法的方法。</param>
		/// <param name="operatorName">产生异常的操作名。</param>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException NotGenericMethodDefinition(MethodBase method, string operatorName)
		{
			return GetInvalidOperation("NotGenericMethodDefinition", method, operatorName);
		}
		/// <summary>
		/// 返回状态无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		private static InvalidOperationException GetInvalidOperation(string resName)
		{
			return new InvalidOperationException(ExceptionResources.GetString(resName));
		}
		/// <summary>
		/// 返回状态无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="args">异常信息的格式化值。</param>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		private static InvalidOperationException GetInvalidOperation(string resName, params object[] args)
		{
			return new InvalidOperationException(ExceptionResources.GetString(resName, args));
		}

		#endregion

		#region KeyNotFoundException

		/// <summary>
		/// 返回键不存在的异常。
		/// </summary>
		/// <param name="key">不存在的键。</param>
		/// <returns><see cref="System.Collections.Generic.KeyNotFoundException"/> 对象。</returns>
		public static KeyNotFoundException KeyNotFound(object key)
		{
			return new KeyNotFoundException(ExceptionResources.GetString("KeyNotFound", key));
		}

		#endregion // KeyNotFoundException

		#region NotSupportedException

		/// <summary>
		/// 返回只读集合的异常。
		/// </summary>
		/// <returns><see cref="System.NotSupportedException"/> 对象。</returns>
		public static NotSupportedException ReadOnlyCollection()
		{
			return GetNotSupported("ReadOnlyCollection");
		}
		/// <summary>
		/// 返回固定大小集合的异常。
		/// </summary>
		/// <returns><see cref="System.NotSupportedException"/> 对象。</returns>
		public static NotSupportedException FixedSizeCollection()
		{
			return GetNotSupported("FixedSizeCollection");
		}
		/// <summary>
		/// 返回不支持的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.NotSupportedException"/> 对象。</returns>
		private static NotSupportedException GetNotSupported(string resName)
		{
			return new NotSupportedException(ExceptionResources.GetString(resName));
		}

		#endregion // NotSupportedException

		#region ObjectDisposedException

		/// <summary>
		/// 返回对象已释放资源的异常。
		/// </summary>
		/// <returns><see cref="System.ObjectDisposedException"/> 对象。</returns>
		public static ObjectDisposedException ObjectDisposed()
		{
			return new ObjectDisposedException(ExceptionResources.GetString("ObjectDisposed"));
		}

		#endregion // ObjectDisposedException

		#region OverflowException

		/// <summary>
		/// 返回值超出 <see cref="System.SByte"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowSByte()
		{
			return GetOverflowException("OverflowSByte");
		}
		/// <summary>
		/// 返回值超出 <see cref="System.Int16"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt16()
		{
			return GetOverflowException("OverflowInt16");
		}
		/// <summary>
		/// 返回值超出 <see cref="System.Int32"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt32()
		{
			return GetOverflowException("OverflowInt32");
		}
		/// <summary>
		/// 返回值超出 <see cref="System.Int64"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt64()
		{
			return GetOverflowException("OverflowInt64");
		}
		/// <summary>
		/// 返回值超出 <see cref="System.Byte"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowByte()
		{
			return GetOverflowException("OverflowByte");
		}
		/// <summary>
		/// 返回值超出 <see cref="System.UInt16"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt16()
		{
			return GetOverflowException("OverflowUInt16");
		}
		/// <summary>
		/// 返回值超出 <see cref="System.UInt32"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt32()
		{
			return GetOverflowException("OverflowUInt32");
		}
		/// <summary>
		/// 返回值超出 <see cref="System.UInt64"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt64()
		{
			return GetOverflowException("OverflowUInt64");
		}
		/// <summary>
		/// 返回以特定名称字符串资源为信息的
		/// <see cref="System.OverflowException"/> 对象。
		/// </summary>
		/// <param name="resName">作为异常信息的字符串资源名称。</param>
		/// <returns><see cref="System.OverflowException"/> 对象。</returns>
		private static OverflowException GetOverflowException(string resName)
		{
			string message = ExceptionResources.GetString(resName);
			return new OverflowException(message);
		}

		#endregion // OverflowException

		#region RankException

		/// <summary>
		/// 返回多维数组不被支持的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static RankException ArrayRankMultiDimNotSupported()
		{
			return new RankException(ExceptionResources.GetString("ArrayRankMultiDimNotSupported"));
		}

		#endregion // RankException

		#region 缓冲池工厂异常

		/// <summary>
		/// 返回缓冲池类型无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <returns><see cref="System.Configuration.ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType(CacheElement element)
		{
			string message = ExceptionResources.GetString("InvalidCacheType", element.CacheType);
			return new ConfigurationErrorsException(message,
				element.ElementInformation.Source, element.ElementInformation.LineNumber);
		}
		/// <summary>
		/// 返回缓冲池类型无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <param name="innerException">内部的异常信息。</param>
		/// <returns><see cref="System.Configuration.ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType(CacheElement element, Exception innerException)
		{
			string message = ExceptionResources.GetString("InvalidCacheType", element.CacheType);
			return new ConfigurationErrorsException(message, innerException,
				element.ElementInformation.Source, element.ElementInformation.LineNumber);
		}
		/// <summary>
		/// 返回缓冲池类型无效-未实现 ICache 接口的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <returns><see cref="System.Configuration.ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType_ICache(CacheElement element)
		{
			string message = ExceptionResources.GetString("InvalidCacheType_ICache", element.CacheType);
			return new ConfigurationErrorsException(message,
				element.ElementInformation.Source, element.ElementInformation.LineNumber);
		}
		/// <summary>
		/// 返回缓冲池类型创建实例的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <param name="innerException">内部的异常信息。</param>
		/// <returns><see cref="System.Configuration.ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType_CreateInstance(CacheElement element,
			Exception innerException)
		{
			string message = ExceptionResources.GetString("InvalidCacheType_CreateInstance", element.CacheType);
			return new ConfigurationErrorsException(message,
				element.ElementInformation.Source, element.ElementInformation.LineNumber);
		}
		/// <summary>
		/// 返回缓冲池选项无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池选项配置元素。</param>
		/// <param name="type">选项的类型。</param>
		/// <param name="innerException">内部的异常信息。</param>
		/// <returns><see cref="System.Configuration.ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheOption(NameValueConfigurationElement element,
			Type type, Exception innerException)
		{
			string message = ExceptionResources.GetString("InvalidCacheOption", element.Name, element.Value, type);
			return new ConfigurationErrorsException(message,
				element.ElementInformation.Source, element.ElementInformation.LineNumber);
		}
		/// <summary>
		/// 返回缓冲池选项无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <returns><see cref="System.Configuration.ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheOptions(CacheElement element)
		{
			string message = ExceptionResources.GetString("InvalidCacheOptions", element.CacheType);
			return new ConfigurationErrorsException(message,
				element.ElementInformation.Source, element.ElementInformation.LineNumber);
		}

		#endregion // 缓冲池工厂异常

		#region PowerBinder 异常

		/// <summary>
		/// 返回找到多个与绑定约束匹配的字段的异常。
		/// </summary>
		/// <returns><see cref="System.Reflection.AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchField()
		{
			return new AmbiguousMatchException(ExceptionResources.GetString("AmbiguousMatchField"));
		}
		/// <summary>
		/// 返回找到多个与绑定约束匹配的方法的异常。
		/// </summary>
		/// <returns><see cref="System.Reflection.AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchMethod()
		{
			return new AmbiguousMatchException(ExceptionResources.GetString("AmbiguousMatchMethod"));
		}
		/// <summary>
		/// 返回找到多个与绑定约束匹配的属性的异常。
		/// </summary>
		/// <returns><see cref="System.Reflection.AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchProperty()
		{
			return new AmbiguousMatchException(ExceptionResources.GetString("AmbiguousMatchProperty"));
		}
		/// <summary>
		/// 返回存在相同的参数名称的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException SameParameterName(string paramName)
		{
			throw GetArgumentException(paramName, "SameParameterName");
		}

		#endregion // PowerBinder 异常

		#region DelegateBuilder 异常

		/// <summary>
		/// 返回绑定到目标方法出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetMethod(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetMethod");
		}
		/// <summary>
		/// 返回绑定到目标属性出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetProperty(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetProperty");
		}
		/// <summary>
		/// 返回绑定到目标属性出错，不存在 set 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoSet(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetPropertyNoSet");
		}
		/// <summary>
		/// 返回绑定到目标属性出错，不存在 get 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoGet(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetPropertyNoGet");
		}
		/// <summary>
		/// 返回绑定到目标字段出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetField(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetField");
		}
		/// <summary>
		/// 返回类型必须从委托派生的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException MustBeDelegate(string paramName)
		{
			throw GetArgumentException(paramName, "MustBeDelegate");
		}
		/// <summary>
		/// 返回不能是开放泛型类型的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		internal static ArgumentException UnboundGenParam(string paramName)
		{
			throw GetArgumentException(paramName, "UnboundGenParam");
		}

		#endregion // DelegateBuilder 异常

	}
}
