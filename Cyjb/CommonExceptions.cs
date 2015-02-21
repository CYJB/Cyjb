using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Reflection;
using Cyjb.IO;
using Cyjb.Reflection;
using Cyjb.Utility;

namespace Cyjb
{
	/// <summary>
	/// 提供用于异常处理的辅助方法。
	/// </summary>
	public static class CommonExceptions
	{

		#region 参数异常

		/// <summary>
		/// 返回参数为 <c>null</c> 的异常。
		/// </summary>
		/// <param name="paramName">为 <c>null</c> 的参数名。</param>
		/// <returns><see cref="ArgumentNullException"/> 对象。</returns>
		public static ArgumentNullException ArgumentNull(string paramName)
		{
			return new ArgumentNullException(paramName);
		}
		/// <summary>
		/// 返回参数顺序颠倒的异常。
		/// </summary>
		/// <param name="firstParam">第一个异常参数的名称。</param>
		/// <param name="secondParam">第二个异常参数的名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="firstParam"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="secondParam"/> 为 <c>null</c>。</exception>
		public static ArgumentException ReversedArgument(string firstParam, string secondParam)
		{
			if (firstParam == null)
			{
				throw ArgumentNull("firstParam");
			}
			if (secondParam == null)
			{
				throw ArgumentNull("secondParam");
			}
			Contract.EndContractBlock();
			return new ArgumentException(Format(Resources.ReversedArgument, firstParam, secondParam));
		}

		#endregion // 参数异常

		#region 数组、集合异常

		/// <summary>
		/// 返回数组下限不为 <c>0</c> 的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayNonZeroLowerBound(string paramName)
		{
			return new ArgumentException(Resources.ArrayNonZeroLowerBound, paramName);
		}
		/// <summary>
		/// 返回目标数组太小而不能复制集合的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayTooSmall(string paramName)
		{
			return new ArgumentException(Resources.ArrayTooSmall, paramName);
		}
		/// <summary>
		/// 返回无法比较集合元素的异常。
		/// </summary>
		/// <param name="innerException">内部异常引用。</param>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException CollectionItemCompareFailed(Exception innerException)
		{
			return new InvalidOperationException(Resources.CollectionItemCompareFailed, innerException);
		}
		/// <summary>
		/// 返回集合长度与当前集合不同的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException CollectionCountDiffer(string paramName)
		{
			return new ArgumentException(Resources.CollectionCountDiffer, paramName);
		}
		/// <summary>
		/// 返回集合为空的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException CollectionEmpty(string paramName)
		{
			return new ArgumentException(Resources.CollectionEmpty, paramName);
		}
		/// <summary>
		/// 返回集合是固定大小的异常。
		/// </summary>
		/// <returns><see cref="NotSupportedException"/> 对象。</returns>
		public static NotSupportedException CollectionFixedSize()
		{
			return new NotSupportedException(Resources.CollectionFixedSize);
		}
		/// <summary>
		/// 返回集合中不存在指定的项的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException CollectionItemNotExist(string paramName)
		{
			return new ArgumentException(Resources.CollectionItemNotExist, paramName);
		}
		/// <summary>
		/// 返回集合中存在 <c>null</c> 元素的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException CollectionItemNull(string paramName)
		{
			return new ArgumentException(Resources.CollectionItemNull, paramName);
		}
		/// <summary>
		/// 返回集合是只读的异常。
		/// </summary>
		/// <returns><see cref="NotSupportedException"/> 对象。</returns>
		public static NotSupportedException CollectionReadOnly()
		{
			return new NotSupportedException(Resources.CollectionReadOnly);
		}
		/// <summary>
		/// 返回无法执行枚举操作的异常。
		/// </summary>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException EnumerationFailed()
		{
			return new InvalidOperationException(Resources.EnumerationFailed);
		}
		/// <summary>
		/// 返回偏移量和长度超出界限的异常。
		/// </summary>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException InvalidOffsetLength()
		{
			return new ArgumentException(Resources.InvalidOffsetLength);
		}
		/// <summary>
		/// 返回键重复的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException KeyDuplicate(string paramName)
		{
			return new ArgumentException(Resources.KeyDuplicate, paramName);
		}
		/// <summary>
		/// 返回键不存在的异常。
		/// </summary>
		/// <returns><see cref="KeyNotFoundException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回键不存在的异常。
		/// </summary>
		/// </overloads>
		public static KeyNotFoundException KeyNotFound()
		{
			return new KeyNotFoundException(Resources.KeyNotFound);
		}
		/// <summary>
		/// 返回键不存在的异常。
		/// </summary>
		/// <param name="key">不存在的键。</param>
		/// <returns><see cref="KeyNotFoundException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c> 或空字符串。</exception>
		public static KeyNotFoundException KeyNotFound(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw StringEmpty("key");
			}
			Contract.EndContractBlock();
			return new KeyNotFoundException(Format(Resources.KeyNotFound_Key, key));
		}
		/// <summary>
		/// 返回多维数组不被支持的异常。
		/// </summary>
		/// <returns><see cref="RankException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回多维数组不被支持的异常。
		/// </summary>
		/// </overloads>
		public static RankException MultidimensionalArrayNotSupported()
		{
			return new RankException(Resources.MultidimensionalArrayNotSupported);
		}
		/// <summary>
		/// 返回多维数组不被支持的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="RankException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="paramName"/> 为 <c>null</c> 或空字符串。</exception>
		public static RankException MultidimensionalArrayNotSupported(string paramName)
		{
			if (string.IsNullOrEmpty(paramName))
			{
				throw StringEmpty("paramName");
			}
			Contract.EndContractBlock();
			return new RankException(Format(Resources.MultidimensionalArrayNotSupported_Param, paramName));
		}

		#endregion // 数组、集合异常

		#region 字符串异常

		/// <summary>
		/// 返回字符串为空的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException StringEmpty(string paramName)
		{
			return new ArgumentException(Resources.StringEmpty, paramName);
		}
		/// <summary>
		/// 返回字符串只包含空白的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException StringWhiteSpace(string paramName)
		{
			return new ArgumentException(Resources.StringWhiteSpace, paramName);
		}

		#endregion // 字符串异常

		#region 类型异常

		/// <summary>
		/// 返回找到多个用户自定义类型转换的异常。
		/// </summary>
		/// <param name="inputType">输入类型。</param>
		/// <param name="outputType">输出类型。</param>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		internal static InvalidOperationException AmbiguousUserDefinedConverter(Type inputType, Type outputType)
		{
			Contract.Requires(inputType != null && outputType != null);
			return new InvalidOperationException(Format(Resources.AmbiguousUserDefinedConverter, inputType, outputType));
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
			return new ArgumentException(Resources.ArgumentWrongType, paramName);
		}
		/// <summary>
		/// 返回参数类型错误的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="actualValue">实际的参数值。</param>
		/// <param name="targetType">目标类型的值。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="targetType"/> 为 <c>null</c>。</exception>
		public static ArgumentException ArgumentWrongType(string paramName, object actualValue, Type targetType)
		{
			if (targetType == null)
			{
				throw ArgumentNull("targetType");
			}
			Contract.EndContractBlock();
			string message = Format(Resources.ArgumentWrongType_Specific, actualValue, targetType);
			return new ArgumentException(message, paramName);
		}
		/// <summary>
		/// 返回 <c>null</c> 不能转换为值类型的异常。
		/// </summary>
		/// <returns><see cref="InvalidCastException"/> 对象。</returns>
		public static InvalidCastException CannotCastNullToValueType()
		{
			return new InvalidCastException(Resources.CannotCastNullToValueType);
		}
		/// <summary>
		/// 返回枚举参数类型不匹配的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="paramType">参数枚举类型。</param>
		/// <param name="baseType">基本枚举类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="paramName"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="baseType"/> 为 <c>null</c>。</exception>
		internal static ArgumentException EnumTypeDoesNotMatch(string paramName, Type paramType, Type baseType)
		{
			if (paramType == null)
			{
				throw ArgumentNull("paramType");
			}
			if (baseType == null)
			{
				throw ArgumentNull("baseType");
			}
			Contract.EndContractBlock();
			return new ArgumentException(Format(Resources.EnumTypeDoesNotMatch, paramType, baseType), paramName);
		}
		/// <summary>
		/// 返回一种类型不能转换为另一种类型的异常。
		/// </summary>
		/// <returns><see cref="InvalidCastException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回一种类型不能转换为另一种类型的异常。
		/// </summary>
		/// </overloads>
		public static InvalidCastException InvalidCast()
		{
			return new InvalidCastException(Resources.InvalidCast);
		}
		/// <summary>
		/// 返回一种类型不能转换为另一种类型的异常。
		/// </summary>
		/// <param name="fromType">转换的源类型。</param>
		/// <param name="toType">转换的目标类型。</param>
		/// <returns><see cref="InvalidCastException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="fromType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="toType"/> 为 <c>null</c>。</exception>
		public static InvalidCastException InvalidCast(Type fromType, Type toType)
		{
			if (fromType == null)
			{
				throw ArgumentNull("fromType");
			}
			if (toType == null)
			{
				throw ArgumentNull("toType");
			}
			Contract.EndContractBlock();
			return new InvalidCastException(Format(Resources.InvalidCast_FromTo, fromType, toType));
		}
		/// <summary>
		/// 返回目标元素类型与集合项的类型不兼容的异常。
		/// </summary>
		/// <returns><see cref="ArrayTypeMismatchException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回数组类型与集合项类型不兼容的异常。
		/// </summary>
		/// </overloads>
		public static ArrayTypeMismatchException InvalidElementType()
		{
			return new ArrayTypeMismatchException(Resources.InvalidElementType);
		}
		/// <summary>
		/// 返回目标元素类型与集合项的类型不兼容的异常。
		/// </summary>
		/// <param name="innerException">内部异常引用。</param>
		/// <returns><see cref="ArrayTypeMismatchException"/> 对象。</returns>
		public static ArrayTypeMismatchException InvalidElementType(Exception innerException)
		{
			return new ArrayTypeMismatchException(Resources.InvalidElementType, innerException);
		}
		/// <summary>
		/// 返回类型必须从委托派生的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回类型必须从委托派生的异常。
		/// </summary>
		/// </overloads>
		public static ArgumentException MustBeDelegate(string paramName)
		{
			return new ArgumentException(Resources.MustBeDelegate, paramName);
		}
		/// <summary>
		/// 返回类型必须从委托派生的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">异常的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		public static ArgumentException MustBeDelegate(string paramName, Type type)
		{
			if (type == null)
			{
				throw ArgumentNull("type");
			}
			Contract.EndContractBlock();
			return new ArgumentException(Format(Resources.MustBeDelegate_Type, type), paramName);
		}
		/// <summary>
		/// 返回必须是枚举类型的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回必须是枚举类型的异常。
		/// </summary>
		/// </overloads>
		public static ArgumentException MustBeEnum(string paramName)
		{
			return new ArgumentException(Resources.MustBeEnum, paramName);
		}
		/// <summary>
		/// 返回必须是枚举类型的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="type">异常的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="type"/> 为 <c>null</c>。</exception>
		public static ArgumentException MustBeEnum(string paramName, Type type)
		{
			if (type == null)
			{
				throw ArgumentNull("type");
			}
			Contract.EndContractBlock();
			return new ArgumentException(Format(Resources.MustBeEnum_Type, type), paramName);
		}

		#endregion // 类型异常

		#region 数值异常

		/// <summary>
		/// 返回参数小于等于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentMustBePositive(string paramName, object actualValue)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, Resources.ArgumentMustBePositive);
		}
		/// <summary>
		/// 返回参数小于零的异常。
		/// </summary>
		/// <param name="paramName">异常参数的名称。</param>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentNegative(string paramName, object actualValue)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, Resources.ArgumentNegative);
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
			return new ArgumentOutOfRangeException(paramName, actualValue, Resources.ArgumentOutOfRange);
		}
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <param name="begin">参数有效范围的起始值。</param>
		/// <param name="end">参数有效范围的结束值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="begin"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="end"/> 为 <c>null</c>。</exception>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object actualValue,
			object begin, object end)
		{
			if (begin == null)
			{
				throw ArgumentNull("begin");
			}
			if (end == null)
			{
				throw ArgumentNull("end");
			}
			Contract.EndContractBlock();
			return new ArgumentOutOfRangeException(paramName, actualValue,
				Format(Resources.ArgumentOutOfRangeBetween, begin, end));
		}
		/// <summary>
		/// 返回基无效的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="actualBase">导致此异常的基。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		internal static ArgumentOutOfRangeException InvalidBase(string paramName, int actualBase)
		{
			return new ArgumentOutOfRangeException(paramName, actualBase, Resources.InvalidBase);
		}
		/// <summary>
		/// 返回阈值超出范围的异常。
		/// </summary>
		/// <param name="paramName">超出范围的参数名称</param>
		/// <param name="actualThreshold">导致此异常的阈值。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException InvalidThreshold(string paramName, object actualThreshold)
		{
			return new ArgumentOutOfRangeException(paramName, actualThreshold, Resources.InvalidThreshold);
		}
		/// <summary>
		/// 返回值超出 <see cref="Byte"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowByte()
		{
			return new OverflowException(Resources.OverflowByte);
		}
		/// <summary>
		/// 返回值超出 <see cref="SByte"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowSByte()
		{
			return new OverflowException(Resources.OverflowSByte);
		}
		/// <summary>
		/// 返回值超出 <see cref="Int16"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt16()
		{
			return new OverflowException(Resources.OverflowInt16);
		}
		/// <summary>
		/// 返回值超出 <see cref="Int32"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt32()
		{
			return new OverflowException(Resources.OverflowInt32);
		}
		/// <summary>
		/// 返回值超出 <see cref="Int64"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt64()
		{
			return new OverflowException(Resources.OverflowInt64);
		}
		/// <summary>
		/// 返回值超出 <see cref="UInt16"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt16()
		{
			return new OverflowException(Resources.OverflowUInt16);
		}
		/// <summary>
		/// 返回值超出 <see cref="UInt32"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt32()
		{
			return new OverflowException(Resources.OverflowUInt32);
		}
		/// <summary>
		/// 返回值超出 <see cref="UInt64"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt64()
		{
			return new OverflowException(Resources.OverflowUInt64);
		}

		#endregion // 数值异常

		#region 对象状态异常

		/// <summary>
		/// 返回对象已释放资源的异常。
		/// </summary>
		/// <returns><see cref="ObjectDisposedException"/> 对象。</returns>
		public static ObjectDisposedException ObjectDisposed()
		{
			return new ObjectDisposedException(Resources.ObjectDisposed);
		}
		/// <summary>
		/// 返回流已关闭的异常。
		/// </summary>
		/// <param name="streamType">流的类型。</param>
		/// <returns><see cref="ObjectDisposedException"/> 对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="streamType"/> 为 <c>null</c>。</exception>
		public static ObjectDisposedException StreamClosed(Type streamType)
		{
			if (streamType == null)
			{
				throw ArgumentNull("streamType");
			}
			Contract.EndContractBlock();
			return new ObjectDisposedException(Format(Resources.StreamClosed, streamType));
		}

		#endregion // 对象状态异常

		#region 编码异常

		/// <summary>
		/// 返回方法不支持的异常。
		/// </summary>
		/// <returns><see cref="NotSupportedException"/> 对象。</returns>
		public static NotSupportedException MethodNotSupported()
		{
			return new NotSupportedException(Resources.MethodNotSupported);
		}
		/// <summary>
		/// 返回代码不应到达这里的异常。
		/// </summary>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		internal static InvalidOperationException Unreachable()
		{
			Contract.Assert(false);
			return new InvalidOperationException("Code supposed to be unreachable.");
		}

		#endregion // 编码异常

		#region 格式异常

		/// <summary>
		/// 返回未识别的枚举值的异常。
		/// </summary>
		/// <param name="enumType">枚举类型。</param>
		/// <param name="str">未识别的枚举值。</param>
		/// <returns><see cref="FormatException"/> 对象。</returns>
		internal static FormatException EnumValueNotFound(Type enumType, string str)
		{
			Contract.Requires(enumType != null && str != null);
			return new FormatException(Format(Resources.EnumValueNotFound, enumType, str));
		}
		/// <summary>
		/// 返回字符串末尾有其它无法分析的字符的异常。
		/// </summary>
		/// <returns><see cref="FormatException"/> 对象。</returns>
		public static FormatException ExtraJunkAtEnd()
		{
			return new FormatException(Resources.ExtraJunkAtEnd);
		}
		/// <summary>
		/// 返回字符串不包含可用信息的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException MustContainValidInfo(string paramName)
		{
			return new ArgumentException(Resources.MustContainValidInfo, paramName);
		}
		/// <summary>
		/// 返回无符号数的字符串包含负号的异常。
		/// </summary>
		/// <returns><see cref="FormatException"/> 对象。</returns>
		public static FormatException NegativeUnsigned()
		{
			return new FormatException(Resources.NegativeUnsigned);
		}
		/// <summary>
		/// 返回找不到可识别的数字的异常。
		/// </summary>
		/// <returns><see cref="FormatException"/> 对象。</returns>
		public static FormatException NoParsibleDigits()
		{
			return new FormatException(Resources.NoParsibleDigits);
		}

		#endregion // 格式异常

		#region 动态绑定异常

		/// <summary>
		/// 返回不能绑定到开放构造方法的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindOpenConstructedMethod(string paramName)
		{
			return new ArgumentException(Resources.BindOpenConstructedMethod, paramName);
		}
		/// <summary>
		/// 返回绑定到目标字段出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetField(string paramName)
		{
			return new ArgumentException(Resources.BindTargetField, paramName);
		}
		/// <summary>
		/// 返回绑定到目标方法出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetMethod(string paramName)
		{
			return new ArgumentException(Resources.BindTargetMethod, paramName);
		}
		/// <summary>
		/// 返回绑定到目标属性出错的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetProperty(string paramName)
		{
			return new ArgumentException(Resources.BindTargetProperty, paramName);
		}
		/// <summary>
		/// 返回绑定到目标属性出错，不存在 get 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoGet(string paramName)
		{
			return new ArgumentException(Resources.BindTargetPropertyNoGet, paramName);
		}
		/// <summary>
		/// 返回绑定到目标属性出错，不存在 set 访问器的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException BindTargetPropertyNoSet(string paramName)
		{
			return new ArgumentException(Resources.BindTargetPropertyNoSet, paramName);
		}
		/// <summary>
		/// 返回未能推导类型参数的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException CannotInferenceGenericArguments(string paramName)
		{
			return new ArgumentException(paramName, Resources.CannotInferenceGenericArguments);
		}
		/// <summary>
		/// 返回委托类型不兼容的异常。
		/// </summary>
		/// <param name="sourceDlg">源委托。</param>
		/// <param name="targetDlg">需要兼容的目标委托。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException DelegateCompatible(Type sourceDlg, Type targetDlg)
		{
			Contract.Requires(sourceDlg != null && targetDlg != null);
			return new ArgumentException(Format(Resources.DelegateIncompatible, sourceDlg, targetDlg));
		}
		/// <summary>
		/// 返回不表示泛型方法定义的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		internal static InvalidOperationException NeedGenericMethodDefinition(string paramName)
		{
			Contract.Requires(paramName != null);
			return new InvalidOperationException(Format(Resources.NeedGenericMethodDefinition, paramName));
		}
		/// <summary>
		/// 返回属性不存在 get 访问器的异常。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException PropertyNoGetter(string propertyName)
		{
			Contract.Requires(propertyName != null);
			return new ArgumentException(Format(Resources.PropertyNoGetter, propertyName));
		}
		/// <summary>
		/// 返回属性不存在 set 访问器的异常。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException PropertyNoSetter(string propertyName)
		{
			Contract.Requires(propertyName != null);
			return new ArgumentException(Format(Resources.PropertyNoSetter, propertyName));
		}
		/// <summary>
		/// 返回找不到属性或字段的异常。
		/// </summary>
		/// <param name="memberName">成员名称。</param>
		/// <param name="nonPublic">是否搜索非公共成员。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException PropertyOrFieldNotFound(string memberName, bool nonPublic)
		{
			Contract.Requires(memberName != null);
			string message = nonPublic ? Resources.PropertyOrFieldNotFound_NonPublic : Resources.PropertyOrFieldNotFound;
			return new ArgumentException(Format(message, memberName));
		}
		/// <summary>
		/// 返回类型包含泛型参数的异常。
		/// </summary>
		/// <param name="type">没有默认构造函数的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException TypeContainsGenericParameters(Type type)
		{
			Contract.Requires(type != null);
			return new ArgumentException(Format(Resources.TypeContainsGenericParameters, type));
		}
		/// <summary>
		/// 返回找不到类型成员的异常。
		/// </summary>
		/// <param name="memberName">成员名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException TypeMemberNotFound(string memberName)
		{
			Contract.Requires(memberName != null);
			return new ArgumentException(Format(Resources.TypeMemberNotFound, memberName));
		}
		/// <summary>
		/// 返回类型不包含默认构造函数的异常。
		/// </summary>
		/// <param name="type">没有默认构造函数的类型。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException TypeMissingDefaultConstructor(Type type)
		{
			Contract.Requires(type != null);
			return new ArgumentException(Format(Resources.TypeMissingDefaultConstructor, type));
		}

		#endregion // 动态绑定异常

		#region MethodSwitcher 异常

		/// <summary>
		/// 返回处理器找到多个关键参数的异常。
		/// </summary>
		/// <param name="type">处理器所属的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorKeyAmbigus(Type type, string id)
		{
			return new ArgumentException(Format(Resources.ProcessorKeyAmbigus_TypeId, type, id));
		}
		/// <summary>
		/// 返回处理器找不到关键参数的异常。
		/// </summary>
		/// <param name="type">处理器所属的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorKeyNotFound(Type type, string id)
		{
			return new ArgumentException(Format(Resources.ProcessorKeyNotFound_TypeId, type, id));
		}
		/// <summary>
		/// 返回处理器中混杂着静态和实例方法的异常。
		/// </summary>
		/// <param name="type">处理器所属的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorMixed(Type type, string id)
		{
			Contract.Requires(type != null && id != null);
			return new ArgumentException(Format(Resources.ProcessorMixed, type, id));
		}
		/// <summary>
		/// 返回与特定类型相关的处理器未能找到的异常。
		/// </summary>
		/// <param name="type">处理器所属的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorNotFound(Type type, string id)
		{
			Contract.Requires(type != null);
			string message;
			if (string.IsNullOrEmpty(id))
			{
				message = Format(Resources.ProcessorNotFound, type);
			}
			else
			{
				message = Format(Resources.ProcessorNotFound_Id, type, id);
			}
			return new ArgumentException(message);
		}
		/// <summary>
		/// 返回处理器静态/实例不匹配的异常。
		/// </summary>
		/// <param name="type">查找方法的类型。</param>
		/// <param name="id">方法处理器的标识符。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorMismatch(Type type, string id)
		{
			Contract.Requires(type != null && id != null);
			return new ArgumentException(Format(Resources.ProcessorMismatch, type, id));
		}
		/// <summary>
		/// 返回处理器参数不匹配的异常。
		/// </summary>
		/// <param name="type">处理器所属的类型。</param>
		/// <param name="id">处理器的标识。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException ProcessorParameterMismatch(Type type, string id)
		{
			if (type == null || id == null)
			{
				return new ArgumentException(Resources.ProcessorParameterMismatch);
			}
			return new ArgumentException(Format(Resources.ProcessorParameterMismatch_TypeId, type, id));
		}

		#endregion // MethodSwitcher 异常


		#region 缓冲池工厂异常

		/// <summary>
		/// 返回缓冲池类型无效的异常。
		/// </summary>
		/// <param name="element">无效的缓冲池配置元素。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		internal static ConfigurationErrorsException InvalidCacheType(CacheElement element)
		{
			Contract.Requires(element != null);
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
			Contract.Requires(element != null);
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
			Contract.Requires(element != null);
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
			Contract.Requires(element != null);
			string message = Format(ExceptionResources.InvalidCacheOptions, element.CacheType);
			return GetConfigurationErrorsException(message, element.ElementInformation);
		}

		#endregion // 缓冲池工厂异常

		#region PowerBinder 异常

		/// <summary>
		/// 返回找到多个与绑定约束匹配的字段的异常。
		/// </summary>
		/// <returns><see cref="AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchField()
		{
			return new AmbiguousMatchException(ExceptionResources.AmbiguousMatchField);
		}
		/// <summary>
		/// 返回找到多个与绑定约束匹配的方法的异常。
		/// </summary>
		/// <returns><see cref="AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchMethod()
		{
			return new AmbiguousMatchException(ExceptionResources.AmbiguousMatchMethod);
		}
		/// <summary>
		/// 返回找到多个与绑定约束匹配的属性的异常。
		/// </summary>
		/// <returns><see cref="AmbiguousMatchException"/> 对象。</returns>
		internal static AmbiguousMatchException AmbiguousMatchProperty()
		{
			return new AmbiguousMatchException(ExceptionResources.AmbiguousMatchProperty);
		}
		/// <summary>
		/// 返回存在相同的参数名称的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException SameParameterName(string paramName)
		{
			return new ArgumentException(ExceptionResources.SameParameterName, paramName);
		}

		#endregion // PowerBinder 异常

		#region DelegateBuilder 异常


		/// <summary>
		/// 返回不能是开放泛型类型的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		internal static ArgumentException UnboundGenParam(string paramName)
		{
			return new ArgumentException(ExceptionResources.UnboundGenParam, paramName);
		}

		#endregion // DelegateBuilder 异常


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
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException ConflictingAcceptAction()
		{
			return new InvalidOperationException(ExceptionResources.ConflictingAcceptAction);
		}
		/// <summary>
		/// 返回冲突的拒绝动作的异常。
		/// </summary>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException ConflictingRejectAction()
		{
			return new InvalidOperationException(ExceptionResources.ConflictingRejectAction);
		}
		/// <summary>
		/// 返回词法分析器的上下文无效的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <param name="context">发生异常的上下文。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException InvalidLexerContext(string paramName, string context)
		{
			return null;
			//return GetArgumentException(paramName, ExceptionResources.InvalidLexerContext, context);
		}
		/// <summary>
		/// 返回未识别的词法单元的异常。
		/// </summary>
		/// <param name="text">未被识别的词法单元的文本。</param>
		/// <param name="start">词法单元的起始位置。</param>
		/// <param name="end">词法单元的结束位置。</param>
		/// <returns><see cref="SourceException"/> 对象。</returns>
		public static SourceException UnrecognizedToken(string text, SourcePosition start, SourcePosition end)
		{
			return new SourceException(Format(ExceptionResources.UnrecognizedToken, text), new SourceRange(start, end));
		}

		#endregion // 词法分析异常

		#region 辅助方法

		/// <summary>
		/// 返回配置系统错误的异常。
		/// </summary>
		/// <param name="message">异常的信息。</param>
		/// <param name="info">配置元素的信息。</param>
		/// <returns><see cref="ConfigurationErrorsException"/> 对象。</returns>
		private static ConfigurationErrorsException GetConfigurationErrorsException(string message, ElementInformation info)
		{
			Contract.Requires(message != null && info != null);
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
			Contract.Requires(message != null && innerException != null && info != null);
			return new ConfigurationErrorsException(message, innerException, info.Source, info.LineNumber);
		}

		#endregion // 辅助方法


		internal static void CheckArgumentNull<T>(T v, string s)
		{
			if (v == null)
			{
				throw ArgumentNull(s);
			}
		}
		internal static void CheckDelegateType(Type type, string paramName)
		{
			CheckArgumentNull(type, paramName);
			Type baseType = type.BaseType;
			if (baseType != typeof(MulticastDelegate))
			{
				throw MustBeDelegate(paramName);
			}
		}

		/// <summary>
		/// 格式化指定的异常信息。
		/// </summary>
		/// <param name="message">要格式化的异常信息。</param>
		/// <param name="args">格式化信息的参数。</param>
		/// <returns>格式化后的异常信息。</returns>
		private static string Format(string message, params object[] args)
		{
			Contract.Requires(message != null && args != null);
			return string.Format(Resources.Culture, message, Format(args));
		}
		/// <summary>
		/// 将指定数组中的对象格式化为相应的字符串。
		/// </summary>
		/// <param name="args">要转换的对象数组。</param>
		private static object[] Format(object[] args)
		{
			Contract.Requires(args != null);
			for (int i = 0; i < args.Length; i++)
			{
				object value = args[i];
				if (value == null)
				{
					args[i] = "(null)";
					continue;
				}
				Type type = value as Type;
				if (type != null)
				{
					args[i] = type.FullName();
				}
			}
			return args;
		}
	}
}
