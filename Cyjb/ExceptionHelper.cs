using System;
using System.Collections.Generic;

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
		/// 返回数组下限不为 <c>0</c> 的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayNonZeroLowerBound()
		{
			return GetArgumentException("ArrayNonZeroLowerBound");
		}
		/// <summary>
		/// 返回多维数组不被支持的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayRankMultiDimNotSupported()
		{
			return GetArgumentException("ArrayRankMultiDimNotSupported");
		}
		/// <summary>
		/// 返回数组太小而不能复制集合的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayTooSmall()
		{
			return GetArgumentException("ArrayTooSmall");
		}
		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayTypeInvalid()
		{
			return GetArgumentException("ArrayTypeInvalid");
		}
		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// <param name="innerException">内部异常引用。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayTypeInvalid(Exception innerException)
		{
			return GetArgumentException("ArrayTypeInvalid", innerException);
		}
		/// <summary>
		/// 检查数组是否是一维数组，并具有从 <c>0</c> 开始的下限；
		/// 如果不是，则抛出异常。
		/// </summary>
		/// <param name="array">要检查的数组。</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="array"/>是多维的。</exception>
		/// <exception cref="System.ArgumentException"><paramref name="array"/>的下限不为 <c>0</c>。</exception>
		public static void CheckFlatArray(Array array)
		{
			if (array.Rank != 1)
			{
				throw ArrayRankMultiDimNotSupported();
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw ArrayNonZeroLowerBound();
			}
		}
		/// <summary>
		/// 返回偏移量和长度超出界限的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException InvalidOffsetLength()
		{
			return GetArgumentException("InvalidOffsetLength");
		}

		#endregion // 数组异常

		/// <summary>
		/// 返回至少有一个对象实现 <see cref="System.IComparable"/> 的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentImplementIComparable()
		{
			return GetArgumentException("ArgumentImplementIComparable");
		}
		/// <summary>
		/// 返回参数小于等于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentMustBePositive(string paramName)
		{
			throw GetArgumentException("ArgumentMustBePositive", paramName);
		}
		/// <summary>
		/// 返回参数小于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentNegative(string paramName)
		{
			throw GetArgumentException("ArgumentNegative", paramName);
		}
		/// <summary>
		/// 返回泛型集合的参数类型错误的异常。
		/// </summary>
		/// <param name="value">错误的参数值。</param>
		/// <param name="targetType">目标类型的值。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentWrongType(object value, Type targetType)
		{
			return GetArgumentException("ArgumentWrongType", value, targetType);
		}
		/// <summary>
		/// 返回基不为 <c>10</c> 的字符串包含减号的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException BaseConvertNegativeValue()
		{
			return GetArgumentException("BaseConvertNegativeValue");
		}
		/// <summary>
		/// 返回基无效的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException InvalidBase()
		{
			return GetArgumentException("InvalidBase");
		}
		/// <summary>
		/// 返回键重复的异常。
		/// </summary>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		public static ArgumentException KeyDuplicate()
		{
			return GetArgumentException("KeyDuplicate");
		}

		#region 获取异常对象

		/// <summary>
		/// 返回参数异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		private static ArgumentException GetArgumentException(string resName)
		{
			return new ArgumentException(ExceptionResources.GetString(resName));
		}
		/// <summary>
		/// 返回参数异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="innerException">内部异常引用。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		private static ArgumentException GetArgumentException(string resName,
			Exception innerException)
		{
			string message = ExceptionResources.GetString(resName);
			return new ArgumentException(message, innerException);
		}
		/// <summary>
		/// 返回参数异常。
		/// </summary>
		/// <param name="resName">异常信息的资源名称。</param>
		/// <param name="args">格式化信息的参数。</param>
		/// <returns><see cref="System.ArgumentException"/> 对象。</returns>
		private static ArgumentException GetArgumentException(string resName, params object[] args)
		{
			string message = ExceptionResources.GetString(resName, args);
			return new ArgumentException(message);
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

		#endregion // ArgumentNullException

		#region ArgumentOutOfRangeException

		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <returns><see cref="System.ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName)
		{
			string message = ExceptionResources.GetString("ArgumentOutOfRange", paramName);
			return new ArgumentOutOfRangeException(paramName, message);
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
			string message = ExceptionResources.GetString("ArgumentOutOfRangeBetween", paramName, begin, end);
			return new ArgumentOutOfRangeException(paramName, message);
		}

		#endregion // ArgumentOutOfRangeException

		#region FormatException

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
		/// <returns><see cref="System.FormatException"/> 对象。</returns>
		private static FormatException GetFormatException(string resName)
		{
			string message = ExceptionResources.GetString(resName);
			return new FormatException(message);
		}

		#endregion // FormatException

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
		/// 返回未知的枚举类型的异常。
		/// </summary>
		/// <returns><see cref="System.InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException UnknownEnumType()
		{
			return GetInvalidOperation("UnknownEnumType");
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

		#endregion

		#region KeyNotFoundException

		/// <summary>
		/// 返回键不存在的异常。
		/// </summary>
		/// <returns><see cref="System.Collections.Generic.KeyNotFoundException"/> 对象。</returns>
		public static KeyNotFoundException KeyNotFound()
		{
			return new KeyNotFoundException();
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

	}
}
