using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.Serialization;
using Cyjb.IO;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供用于异常处理的辅助方法。
	/// </summary>
	public static class CommonExceptions
	{

		#region ArgumentException

		#region 数组异常

		/// <summary>
		/// 返回数组为空的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayEmpty(string paramName)
		{
			return new ArgumentException(ExceptionResources.ArrayEmpty, paramName);
		}
		/// <summary>
		/// 返回数组下限不为 <c>0</c> 的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayNonZeroLowerBound(string paramName)
		{
			return new ArgumentException(ExceptionResources.ArrayNonZeroLowerBound, paramName);
		}
		/// <summary>
		/// 返回目标数组太小而不能复制集合的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayTooSmall(string paramName)
		{
			return new ArgumentException(ExceptionResources.ArrayTooSmall, paramName);
		}
		/// <summary>
		/// 返回偏移量和长度超出界限的异常。
		/// </summary>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException InvalidOffsetLength()
		{
			return new ArgumentException(ExceptionResources.InvalidOffsetLength);
		}
		/// <summary>
		/// 返回数组长度不同的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayLengthsDiffer(string paramName)
		{
			return new ArgumentException(ExceptionResources.ArrayLengthsDiffer, paramName);
		}

		#endregion // 数组异常

		#region 字符串异常

		/// <summary>
		/// 返回字符串为空的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException StringEmpty(string paramName)
		{
			return GetArgumentException(paramName, ExceptionResources.StringEmpty, paramName);
		}
		/// <summary>
		/// 返回字符串只包含空白的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException StringWhiteSpace(string paramName)
		{
			return GetArgumentException(paramName, ExceptionResources.StringWhiteSpace, paramName);
		}

		#endregion // 字符串异常

		/// <summary>
		/// 返回至少有一个对象实现 <see cref="System.IComparable"/> 的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentImplementIComparable(string paramName)
		{
			return new ArgumentException(ExceptionResources.ArgumentImplementIComparable, paramName);
		}
		/// <summary>
		/// 返回参数类型错误的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回参数类型错误的异常。
		/// </summary>
		/// </overloads>
		public static ArgumentException ArgumentWrongType(string paramName)
		{
			return GetArgumentException(paramName, ExceptionResources.ArgumentWrongType, paramName);
		}
		/// <summary>
		/// 返回参数类型错误的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="actualValue">实际的参数值。</param>
		/// <param name="targetType">目标类型的值。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentWrongType(string paramName, object actualValue, Type targetType)
		{
			return GetArgumentException(paramName, ExceptionResources.ArgumentWrongType_Specific,
				paramName, actualValue, targetType);
		}
		/// <summary>
		/// 返回未能推导类型参数的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="method">未能推导参数的方法。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException CannotInferGenericArguments(string paramName, MethodBase method)
		{
			return GetArgumentException(paramName, ExceptionResources.CannotInferGenericArguments, method);
		}
		/// <summary>
		/// 返回枚举参数类型不匹配的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="argType">参数类型。</param>
		/// <param name="baseType">基类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException EnumTypeDoesNotMatch(string paramName, Type argType, Type baseType)
		{
			return GetArgumentException(paramName, ExceptionResources.EnumTypeDoesNotMatch, argType, baseType);
		}
		/// <summary>
		/// 返回存在为 <c>null</c> 的内部异常的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException InnerExceptionNull(string paramName)
		{
			return new ArgumentException(ExceptionResources.InnerExceptionNull, paramName);
		}
		/// <summary>
		/// 返回指定的项不存在的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ItemNotExist(string paramName)
		{
			return new ArgumentException(ExceptionResources.ItemNotExist, paramName);
		}
		/// <summary>
		/// 返回键重复的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException KeyDuplicate(string paramName)
		{
			return new ArgumentException(ExceptionResources.KeyDuplicate, paramName);
		}
		/// <summary>
		/// 返回必须是枚举的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">异常的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException MustBeEnum(string paramName, Type type)
		{
			return GetArgumentException(paramName, ExceptionResources.MustBeEnum, type);
		}
		/// <summary>
		/// 返回必须包含枚举信息的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException MustContainEnumInfo(string paramName)
		{
			return new ArgumentException(ExceptionResources.MustContainEnumInfo, paramName);
		}
		/// <summary>
		/// 返回参数顺序颠倒的异常。
		/// </summary>
		/// <param name="firstParam">第一个异常参数的名称。</param>
		/// <param name="secondParam">第二个异常参数的名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ReversedArgument(string firstParam, string secondParam)
		{
			return new ArgumentException(Format(ExceptionResources.ReversedArgument, firstParam, secondParam));
		}

		#endregion // ArgumentException

		#region ArgumentNullException

		/// <summary>
		/// 返回参数为 <c>null</c> 的异常。
		/// </summary>
		/// <param name="paramName">为 <c>null</c> 的参数名。</param>
		/// <returns><see cref="ArgumentNullException"/> 对象。</returns>
		public static ArgumentNullException ArgumentNull(string paramName)
		{
			return new ArgumentNullException(paramName);
		}

		#endregion // ArgumentNullException

		#region ArgumentOutOfRangeException

		/// <summary>
		/// 返回参数小于等于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentMustBePositive(string paramName, object actualValue)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, ExceptionResources.ArgumentMustBePositive);
		}
		/// <summary>
		/// 返回参数小于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentNegative(string paramName, object actualValue)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, ExceptionResources.ArgumentNegative);
		}
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// </overloads>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object actualValue)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, ExceptionResources.ArgumentOutOfRange);
		}
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <param name="begin">参数有效范围的起始值。</param>
		/// <param name="end">参数有效范围的结束值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object actualValue,
			object begin, object end)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue,
				Format(ExceptionResources.ArgumentOutOfRangeBetween, begin, end));
		}
		/// <summary>
		/// 返回参数最小值大于最大值的异常。
		/// </summary>
		/// <param name="minParamName">表示最小值的参数名称。</param>
		/// <param name="maxParamName">表示最大值的参数名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentMinMaxValue(string minParamName, string maxParamName)
		{
			return new ArgumentOutOfRangeException(
				Format(ExceptionResources.ArgumentMinMaxValue, minParamName, maxParamName));
		}
		/// <summary>
		/// 返回基无效的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="actualBase">导致此异常的基。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		internal static ArgumentOutOfRangeException InvalidBase(string paramName, int actualBase)
		{
			return new ArgumentOutOfRangeException(paramName, actualBase, ExceptionResources.InvalidBase);
		}
		/// <summary>
		/// 返回用于创建字典的阈值超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称</param>
		/// <param name="actualThresholdd">导致此异常的阈值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		internal static ArgumentOutOfRangeException InvalidDictionaryThreshold(string paramName, int actualThresholdd)
		{
			return new ArgumentOutOfRangeException(paramName, actualThresholdd, ExceptionResources.InvalidDictionaryThreshold);
		}

		#endregion // ArgumentOutOfRangeException

		#region ArrayTypeMismatchException

		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// <returns><see cref="ArrayTypeMismatchException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// </overloads>
		public static ArrayTypeMismatchException InvalidArrayType()
		{
			return new ArrayTypeMismatchException(ExceptionResources.InvalidArrayType);
		}
		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// <param name="innerException">内部异常引用。</param>
		/// <returns><see cref="ArrayTypeMismatchException"/> 对象。</returns>
		public static ArrayTypeMismatchException InvalidArrayType(Exception innerException)
		{
			return new ArrayTypeMismatchException(ExceptionResources.InvalidArrayType, innerException);
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
			string message = ExceptionResources.ResourceManager.GetString(resName);
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
			string message = ExceptionResources.ResourceManager.GetString(resName);
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
		/// <overloads>
		/// <summary>
		/// 返回转换无效的异常。
		/// </summary>
		/// </overloads>
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
			return null;
			//string message = ExceptionResources.ResourceManager.GetString("ConvertInvalidValue", actualValue, type);
			//return new InvalidCastException(message, innerException);
		}
		/// <summary>
		/// 返回转换无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.InvalidCastException"/> 对象。</returns>
		private static InvalidCastException GetInvalidCast(string resName)
		{
			return new InvalidCastException(ExceptionResources.ResourceManager.GetString(resName));
		}
		/// <summary>
		/// 返回转换无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="args">异常信息的格式化值。</param>
		/// <returns><see cref="System.InvalidCastException"/> 对象。</returns>
		private static InvalidCastException GetInvalidCast(string resName, params object[] args)
		{
			return new InvalidCastException(ExceptionResources.ResourceManager.GetString(resName));
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
		/// 返回无法比较数组元素的异常。
		/// </summary>
		/// <param name="innerException">导致当前异常的异常。</param>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException ComparerFailed(Exception innerException)
		{
			return GetInvalidOperation("ComparerFailed", innerException);
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
		/// 返回操作无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		private static InvalidOperationException GetInvalidOperation(string resName)
		{
			return new InvalidOperationException(ExceptionResources.ResourceManager.GetString(resName));
		}
		/// <summary>
		/// 返回操作无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="innerException">导致当前异常的异常。</param>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		private static InvalidOperationException GetInvalidOperation(string resName, Exception innerException)
		{
			return new InvalidOperationException(ExceptionResources.ResourceManager.GetString(resName), innerException);
		}
		/// <summary>
		/// 返回操作无效的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="args">异常信息的格式化值。</param>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		private static InvalidOperationException GetInvalidOperation(string resName, params object[] args)
		{
			return new InvalidOperationException(ExceptionResources.ResourceManager.GetString(resName));
		}

		#endregion

		#region KeyNotFoundException

		/// <summary>
		/// 返回键不存在的异常。
		/// </summary>
		/// <param name="key">不存在的键。</param>
		/// <returns><see cref="KeyNotFoundException"/> 对象。</returns>
		public static KeyNotFoundException KeyNotFound(object key)
		{
			return new KeyNotFoundException(Format(ExceptionResources.KeyNotFound, key));
		}

		#endregion // KeyNotFoundException

		#region NotSupportedException

		/// <summary>
		/// 返回固定大小集合的异常。
		/// </summary>
		/// <returns><see cref="System.NotSupportedException"/> 对象。</returns>
		public static NotSupportedException FixedSizeCollection()
		{
			return GetNotSupported("FixedSizeCollection");
		}
		/// <summary>
		/// 返回方法不支持的异常。
		/// </summary>
		/// <returns><see cref="System.NotSupportedException"/> 对象。</returns>
		public static NotSupportedException MethodNotSupported()
		{
			return GetNotSupported("MethodNotSupported");
		}
		/// <summary>
		/// 返回只读集合的异常。
		/// </summary>
		/// <returns><see cref="System.NotSupportedException"/> 对象。</returns>
		public static NotSupportedException ReadOnlyCollection()
		{
			return GetNotSupported("ReadOnlyCollection");
		}
		/// <summary>
		/// 返回不支持的异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.NotSupportedException"/> 对象。</returns>
		private static NotSupportedException GetNotSupported(string resName)
		{
			return new NotSupportedException(ExceptionResources.ResourceManager.GetString(resName));
		}

		#endregion // NotSupportedException

		#region ObjectDisposedException

		/// <summary>
		/// 返回 SourceReader 已关闭的异常。
		/// </summary>
		/// <returns><see cref="System.ObjectDisposedException"/> 对象。</returns>
		internal static ObjectDisposedException SourceReaderClosed()
		{
			return new ObjectDisposedException(ExceptionResources.ResourceManager.GetString("SourceReaderClosed"));
		}
		/// <summary>
		/// 返回对象已释放资源的异常。
		/// </summary>
		/// <returns><see cref="System.ObjectDisposedException"/> 对象。</returns>
		public static ObjectDisposedException ObjectDisposed()
		{
			return new ObjectDisposedException(ExceptionResources.ResourceManager.GetString("ObjectDisposed"));
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
			string message = ExceptionResources.ResourceManager.GetString(resName);
			return new OverflowException(message);
		}

		#endregion // OverflowException

		#region RankException

		/// <summary>
		/// 返回多维数组不被支持的异常。
		/// </summary>
		/// <returns><see cref="RankException"/> 对象。</returns>
		public static RankException ArrayRankMultiDimNotSupported()
		{
			return new RankException(ExceptionResources.ArrayRankMultiDimNotSupported);
		}

		#endregion // RankException

		#region SerializationException

		/// <summary>
		/// 返回异常集合反序列化失败的异常。
		/// </summary>
		/// <returns><see cref="System.Runtime.Serialization.SerializationException"/> 对象。</returns>
		public static SerializationException AggregateExceptionDeserializationFailure()
		{
			return new SerializationException(
				ExceptionResources.AggregateExceptionDeserializationFailure);
		}

		#endregion // SerializationException

		#region Interval<T> 异常

		/// <summary>
		/// 返回无效区间的异常。
		/// </summary>
		/// <param name="interval">无效的区间。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException InvalidInterval(string interval)
		{
			return new ArgumentException(Format(ExceptionResources.InvalidInterval, interval));
		}
		/// <summary>
		/// 返回无效区间格式的异常。
		/// </summary>
		/// <param name="interval">无效的区间格式。</param>
		/// <returns><see cref="FormatException"/> 对象。</returns>
		internal static FormatException InvalidIntervalFormat(string interval)
		{
			return new FormatException(Format(ExceptionResources.InvalidInterval, interval));
		}

		#endregion // Interval<T> 异常

		#region 缓冲池工厂异常

		/// <summary>
		/// 返回缓冲池类型无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType(CacheElement element)
		{
			Contract.Assert(element != null);
			string message = Format(ExceptionResources.InvalidCacheType, element.CacheType);
			return GetConfigurationErrorsException(message, element.ElementInformation);
		}
		/// <summary>
		/// 返回缓冲池类型无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <param name="innerException">内部的异常信息。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType(CacheElement element, Exception innerException)
		{
			Contract.Assert(element != null);
			string message = Format(ExceptionResources.InvalidCacheType, element.CacheType);
			return GetConfigurationErrorsException(message, innerException, element.ElementInformation);
		}
		/// <summary>
		/// 返回缓冲池类型无效-未实现 ICache 接口的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType_ICache(CacheElement element)
		{
			Contract.Assert(element != null);
			string message = Format(ExceptionResources.InvalidCacheType_ICache, element.CacheType);
			return GetConfigurationErrorsException(message, element.ElementInformation);
		}
		/// <summary>
		/// 返回缓冲池选项无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheOptions(CacheElement element)
		{
			Contract.Assert(element != null);
			string message = Format(ExceptionResources.InvalidCacheOptions, element.CacheType);
			return GetConfigurationErrorsException(message, element.ElementInformation);
		}

		#endregion // 缓冲池工厂异常

		#region PowerBinder 异常

		/// <summary>
		/// 返回找到多个与绑定约束匹配的字段的异常。
		/// </summary>
		/// <returns><see cref="System.Reflection.AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchField()
		{
			return new AmbiguousMatchException(ExceptionResources.ResourceManager.GetString("AmbiguousMatchField"));
		}
		/// <summary>
		/// 返回找到多个与绑定约束匹配的方法的异常。
		/// </summary>
		/// <returns><see cref="System.Reflection.AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchMethod()
		{
			return new AmbiguousMatchException(ExceptionResources.ResourceManager.GetString("AmbiguousMatchMethod"));
		}
		/// <summary>
		/// 返回找到多个与绑定约束匹配的属性的异常。
		/// </summary>
		/// <returns><see cref="System.Reflection.AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchProperty()
		{
			return new AmbiguousMatchException(ExceptionResources.ResourceManager.GetString("AmbiguousMatchProperty"));
		}
		/// <summary>
		/// 返回存在相同的参数名称的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException SameParameterName(string paramName)
		{
			throw GetArgumentException(paramName, "SameParameterName");
		}

		#endregion // PowerBinder 异常

		#region DelegateBuilder 异常

		/// <summary>
		/// 检查委托的类型是否合法。
		/// </summary>
		/// <param name="type">委托的类型。</param>
		/// <param name="paramName">参数的名称。</param>
		internal static void CheckDelegateType(Type type, string paramName)
		{
			CommonExceptions.CheckArgumentNull(type, paramName);
			Type baseType = type.BaseType;
			if (baseType != typeof(MulticastDelegate))
			{
				throw CommonExceptions.MustBeDelegate(paramName);
			}
		}
		/// <summary>
		/// 返回绑定到目标方法出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetMethod(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetMethod");
		}
		/// <summary>
		/// 返回绑定到目标属性出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetProperty(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetProperty");
		}
		/// <summary>
		/// 返回绑定到目标属性出错，不存在 set 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoSet(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetPropertyNoSet");
		}
		/// <summary>
		/// 返回绑定到目标属性出错，不存在 get 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoGet(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetPropertyNoGet");
		}
		/// <summary>
		/// 返回绑定到目标字段出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetField(string paramName)
		{
			throw GetArgumentException(paramName, "BindTargetField");
		}
		/// <summary>
		/// 返回类型必须从委托派生的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException MustBeDelegate(string paramName)
		{
			throw GetArgumentException(paramName, "MustBeDelegate");
		}
		/// <summary>
		/// 返回不能是开放泛型类型的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException UnboundGenParam(string paramName)
		{
			throw GetArgumentException(paramName, "UnboundGenParam");
		}

		#endregion // DelegateBuilder 异常

		#region MethodSwitcher 异常

		/// <summary>
		/// 返回找不到方法处理器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">查找方法的类型。</param>
		/// <param name="id">方法处理器的标识符。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorNotFound(string paramName, Type type, string id)
		{
			throw GetArgumentException(paramName, "ProcessorNotFound", type, id);
		}
		/// <summary>
		/// 返回方法处理器不匹配的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">查找方法的类型。</param>
		/// <param name="id">方法处理器的标识符。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorMismatch(string paramName, Type type, string id)
		{
			throw GetArgumentException(paramName, "ProcessorMismatch", type, id);
		}
		/// <summary>
		/// 返回方法切换器中混杂着静态和实例方法的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">查找方法的类型。</param>
		/// <param name="id">方法切换器的标识符。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorMixed(string paramName, Type type, string id)
		{
			throw GetArgumentException(paramName, "ProcessorMixed", type, id);
		}
		/// <summary>
		/// 返回特定类型的方法处理器未能找到的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">未能找到的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorNotFound(string paramName, Type type)
		{
			if (type == null)
			{
				return GetArgumentException(paramName, "ProcessorNotFound_Type", "null");
			}
			else
			{
				return GetArgumentException(paramName, "ProcessorNotFound_Type", type);
			}
		}
		/// <summary>
		/// 返回委托类型不兼容的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="target">需要兼容的目标。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException DelegateCompatible(string paramName, Type target)
		{
			return GetArgumentException(paramName, "DelegateCompatible", target);
		}

		#endregion // MethodSwitcher 异常

		#region 词法分析异常

		/// <summary>
		/// 返回无效的源文件范围的异常。
		/// </summary>
		/// <param name="start">范围的起始位置。</param>
		/// <param name="end">范围的结束位置。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException InvalidSourceRange(SourcePosition start, SourcePosition end)
		{
			return new ArgumentException(Format(ExceptionResources.InvalidSourceRange, start, end));
		}
		/// <summary>
		/// 返回冲突的接受动作的异常。
		/// </summary>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException ConflictingAcceptAction()
		{
			return GetInvalidOperation("ConflictingAcceptAction");
		}
		/// <summary>
		/// 返回冲突的拒绝动作的异常。
		/// </summary>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException ConflictingRejectAction()
		{
			return GetInvalidOperation("ConflictingRejectAction");
		}
		/// <summary>
		/// 返回词法分析器的上下文无效的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="context">发生异常的上下文。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException InvalidLexerContext(string paramName, string context)
		{
			return GetArgumentException(paramName, "InvalidLexerContext", context);
		}
		/// <summary>
		/// 返回未识别的词法单元的异常。
		/// </summary>
		/// <param name="text">未被识别的词法单元的文本。</param>
		/// <param name="start">词法单元的起始位置。</param>
		/// <param name="end">词法单元的结束位置。</param>
		/// <returns><see cref="Cyjb.IO.SourceException"/> 对象。</returns>
		public static SourceException UnrecognizedToken(string text, SourcePosition start, SourcePosition end)
		{
			return null;
			// return new SourceException(ExceptionResources.ResourceManager.GetString("UnrecognizedToken", text), start, end);
		}

		#endregion // 词法分析异常

		#region 辅助方法

		/// <summary>
		/// 格式化指定的异常信息。
		/// </summary>
		/// <param name="message">要格式化的异常信息。</param>
		/// <param name="args">格式化信息的参数。</param>
		/// <returns>格式化后的异常信息。</returns>
		private static string Format(string message, params object[] args)
		{
			Contract.Assert(message != null);
			return string.Format(ExceptionResources.Culture, message, args);
		}
		/// <summary>
		/// 返回参数异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="message">异常的格式化信息。</param>
		/// <param name="args">格式化信息的参数。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		private static ArgumentException GetArgumentException(string paramName, string message, params object[] args)
		{
			return new ArgumentException(Format(message, args), paramName);
		}
		/// <summary>
		/// 返回配置系统错误的异常。
		/// </summary>
		/// <param name="message">异常的信息。</param>
		/// <param name="info">配置元素的信息。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		private static ConfigurationErrorsException GetConfigurationErrorsException(string message, ElementInformation info)
		{
			Contract.Assert(info != null);
			return new ConfigurationErrorsException(message, info.Source, info.LineNumber);
		}
		/// <summary>
		/// 返回配置系统错误的异常。
		/// </summary>
		/// <param name="message">异常的信息。</param>
		/// <param name="innerException">导致当前异常的异常。</param>
		/// <param name="info">配置元素的信息。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		private static ConfigurationErrorsException GetConfigurationErrorsException(string message,
			Exception innerException, ElementInformation info)
		{
			Contract.Assert(info != null);
			return new ConfigurationErrorsException(message, innerException, info.Source, info.LineNumber);
		}

		#endregion // 辅助方法

		#region 临时方法


		/// <summary>
		/// 检查参数是否为 <c>null</c>，如果为 <c>null</c> 则抛出异常。
		/// </summary>
		/// <param name="value">要检查的参数值。</param>
		/// <param name="paramName">要检查的参数名。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="value"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 检查参数是否为 <c>null</c>，如果为 <c>null</c> 则抛出异常。
		/// </summary>
		/// </overloads>
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
		#endregion // 临时方法

	}
}
