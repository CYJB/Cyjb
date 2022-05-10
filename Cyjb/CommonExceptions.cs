using System.Collections;
using System.Runtime.CompilerServices;

namespace Cyjb
{
	/// <summary>
	/// 提供用于异常处理的辅助方法。
	/// </summary>
	public static class CommonExceptions
	{

		#region 数值异常

		/// <summary>
		/// 返回参数小于等于零的异常。
		/// </summary>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <param name="paramName">异常参数的名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentMustBePositive(object actualValue,
			[CallerArgumentExpression("actualValue")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, ResourcesUtil.Format(Resources.ArgumentMustBePositive, paramName));
		}

		/// <summary>
		/// 返回参数小于零的异常。
		/// </summary>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <param name="paramName">异常参数的名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentNegative(object actualValue,
			[CallerArgumentExpression("actualValue")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, ResourcesUtil.Format(Resources.ArgumentNegative, paramName));
		}

		/// <summary>
		/// 返回索引超出范围的异常。
		/// </summary>
		/// <param name="actualValue">导致此异常的索引值。</param>
		/// <param name="paramName">索引参数的名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentIndexOutOfRange(object actualValue,
			[CallerArgumentExpression("actualValue")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, Resources.ArgumentOutOfRange_Index);
		}

		/// <summary>
		/// 返回计数超出范围的异常。
		/// </summary>
		/// <param name="actualValue">导致此异常的计数值。</param>
		/// <param name="paramName">计数参数的名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentCountOutOfRange(object actualValue,
			[CallerArgumentExpression("actualValue")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, Resources.ArgumentOutOfRange_Count);
		}

		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		/// <overloads>
		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// </overloads>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(object actualValue,
			[CallerArgumentExpression("actualValue")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue, ResourcesUtil.Format(Resources.ArgumentOutOfRange, paramName));
		}

		/// <summary>
		/// 返回参数超出范围的异常。
		/// </summary>
		/// <param name="actualValue">导致此异常的参数值。</param>
		/// <param name="begin">参数有效范围的起始值。</param>
		/// <param name="end">参数有效范围的结束值。</param>
		/// <param name="paramName">超出范围的参数名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentOutOfRange(object actualValue, object begin, object end,
			[CallerArgumentExpression("actualValue")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualValue,
				 ResourcesUtil.Format(Resources.ArgumentOutOfRange_Between, paramName, begin, end));
		}

		/// <summary>
		/// 返回参数上下界颠倒的异常。
		/// </summary>
		/// <param name="minParamName">下界参数的名称。</param>
		/// <param name="maxParamName">上界参数的名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException ArgumentMinMaxValue(string minParamName, string maxParamName)
		{
			return new ArgumentOutOfRangeException(minParamName, ResourcesUtil.Format(Resources.ArgumentMinMaxValue, minParamName, maxParamName));
		}

		/// <summary>
		/// 返回基无效的异常。
		/// </summary>
		/// <param name="actualBase">导致此异常的基。</param>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		internal static ArgumentOutOfRangeException InvalidBase(int actualBase,
			[CallerArgumentExpression("actualBase")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualBase, Resources.InvalidBase);
		}

		/// <summary>
		/// 返回阈值超出范围的异常。
		/// </summary>
		/// <param name="actualThreshold">导致此异常的阈值。</param>
		/// <param name="paramName">超出范围的参数名称</param>
		/// <returns><see cref="ArgumentOutOfRangeException"/> 对象。</returns>
		public static ArgumentOutOfRangeException InvalidThreshold(object actualThreshold,
			[CallerArgumentExpression("actualThreshold")] string? paramName = null)
		{
			return new ArgumentOutOfRangeException(paramName, actualThreshold, Resources.InvalidThreshold);
		}

		/// <summary>
		/// 返回值超出 <see cref="byte"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowByte()
		{
			return new OverflowException(Resources.OverflowByte);
		}

		/// <summary>
		/// 返回值超出 <see cref="sbyte"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowSByte()
		{
			return new OverflowException(Resources.OverflowSByte);
		}

		/// <summary>
		/// 返回值超出 <see cref="short"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt16()
		{
			return new OverflowException(Resources.OverflowInt16);
		}

		/// <summary>
		/// 返回值超出 <see cref="int"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt32()
		{
			return new OverflowException(Resources.OverflowInt32);
		}

		/// <summary>
		/// 返回值超出 <see cref="long"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowInt64()
		{
			return new OverflowException(Resources.OverflowInt64);
		}

		/// <summary>
		/// 返回值超出 <see cref="ushort"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt16()
		{
			return new OverflowException(Resources.OverflowUInt16);
		}

		/// <summary>
		/// 返回值超出 <see cref="uint"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt32()
		{
			return new OverflowException(Resources.OverflowUInt32);
		}

		/// <summary>
		/// 返回值超出 <see cref="ulong"/> 范围的异常。
		/// </summary>
		/// <returns><see cref="OverflowException"/> 对象。</returns>
		public static OverflowException OverflowUInt64()
		{
			return new OverflowException(Resources.OverflowUInt64);
		}

		#endregion // 数值异常

		#region 对象状态异常

		/// <summary>
		/// 返回流已关闭的异常。
		/// </summary>
		/// <param name="streamName">流的名称。</param>
		/// <returns><see cref="ObjectDisposedException"/> 对象。</returns>
		public static ObjectDisposedException StreamClosed(string streamName)
		{
			return new ObjectDisposedException(streamName, ResourcesUtil.Format(Resources.StreamClosed, streamName));
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

		#endregion // 编码异常

		#region 数组、集合异常

		/// <summary>
		/// 返回数组下限不为 <c>0</c> 的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayNonZeroLowerBound(string? paramName = null)
		{
			return new ArgumentException(Resources.ArrayNonZeroLowerBound, paramName);
		}

		/// <summary>
		/// 检查指定的数组，如果为 <c>null</c>，或者不是下限为零的一维数组则抛出相应异常。
		/// </summary>
		/// <param name="array">要检查的数组。</param>
		/// <param name="paramName">数组参数的名称。</param>
		public static void CheckSimplyArray(Array array,
			[CallerArgumentExpression("array")] string? paramName = null)
		{
			ArgumentNullException.ThrowIfNull(array, paramName);
			if (array.Rank != 1)
			{
				throw MultidimensionalArrayNotSupported(paramName);
			}
			else if (array.GetLowerBound(0) != 0)
			{
				throw ArrayNonZeroLowerBound(paramName);
			}
		}

		/// <summary>
		/// 返回目标数组太小而不能复制集合的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArrayTooSmall(string? paramName = null)
		{
			return new ArgumentException(Resources.ArrayTooSmall, paramName);
		}

		/// <summary>
		/// 返回无法比较集合元素的异常。
		/// </summary>
		/// <param name="innerException">内部异常引用。</param>
		/// <returns><see cref="InvalidOperationException"/> 对象。</returns>
		public static InvalidOperationException CollectionItemCompareFailed(Exception? innerException = null)
		{
			return new InvalidOperationException(Resources.CollectionItemCompareFailed, innerException);
		}

		/// <summary>
		/// 返回集合长度与当前集合不同的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException CollectionCountDiffer(string? paramName = null)
		{
			return new ArgumentException(Resources.CollectionCountDiffer, paramName);
		}

		/// <summary>
		/// 检查指定的集合，如果长度为零则抛出相应异常。
		/// </summary>
		/// <param name="collection">要检查的集合。</param>
		/// <param name="paramName">集合参数的名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static void CheckCollectionEmpty(ICollection? collection,
			[CallerArgumentExpression("collection")] string? paramName = null)
		{
			if (collection == null || collection.Count == 0)
			{
				throw new ArgumentException(ResourcesUtil.Format(Resources.CollectionEmpty, paramName), paramName);
			}
		}

		/// <summary>
		/// 返回多维数组不被支持的异常。
		/// </summary>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="RankException"/> 对象。</returns>
		public static RankException MultidimensionalArrayNotSupported(string? paramName = null)
		{
			string message;
			if (paramName == null)
			{
				message = Resources.MultidimensionalArrayNotSupported;
			}
			else
			{
				message = ResourcesUtil.Format(Resources.MultidimensionalArrayNotSupported_Param, paramName);
			}
			return new RankException(message);
		}

		/// <summary>
		/// 检查指定的数组，如果为 <c>null</c>、长度为零或包含为 <c>null</c> 的元素则抛出相应异常。
		/// </summary>
		/// <typeparam name="T">要检查的数组元素类型。</typeparam>
		/// <param name="array">要检查的数组。</param>
		/// <param name="paramName">数组参数的名称。</param>
		public static void CheckArrayElemenetNull<T>(T[] array,
			[CallerArgumentExpression("array")] string? paramName = null)
		{
			ArgumentNullException.ThrowIfNull(array, paramName);
			if (array.Length == 0)
			{
				throw new ArgumentException(Resources.CollectionEmpty, paramName);
			}
			if (Array.IndexOf(array, null) >= 0)
			{
				throw new ArgumentException(ResourcesUtil.Format(Resources.CollectionItemNull, paramName), paramName);
			}
		}

		#endregion // 数组、集合异常

		#region 类型异常

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
		/// <param name="actualValue">实际的参数值。</param>
		/// <param name="targetType">目标类型的值。</param>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException ArgumentWrongType(object? actualValue, Type targetType,
			[CallerArgumentExpression("actualValue")] string? paramName = null)
		{
			string message = ResourcesUtil.Format(Resources.ArgumentWrongType_Specific, actualValue, targetType);
			return new ArgumentException(message, paramName);
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
			return new InvalidCastException(ResourcesUtil.Format(Resources.InvalidCast_FromTo, fromType, toType));
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
		/// <param name="type">异常的类型。</param>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException MustBeDelegate(Type type, [CallerArgumentExpression("type")] string? paramName = null)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.MustBeDelegate_Type, type), paramName);
		}

		/// <summary>
		/// 检查指定类型是否是委托类型，如果不是则抛出相应异常。
		/// </summary>
		/// <param name="type">要检查是否是委托类型的类型。</param>
		/// <overloads>
		/// <summary>
		/// 检查指定类型是否是委托类型，如果不是则抛出相应异常。
		/// </summary>
		/// </overloads>
		public static void CheckDelegateType(Type type)
		{
			if (!type.IsDelegate())
			{
				throw new ArgumentException(ResourcesUtil.Format(Resources.MustBeDelegate_Type, type));
			}
		}

		/// <summary>
		/// 检查指定类型是否是委托类型，如果不是则抛出相应异常。
		/// </summary>
		/// <param name="type">要检查是否是委托类型的类型。</param>
		/// <param name="paramName">被检查的参数名。</param>
		public static void CheckDelegateType(Type type, [CallerArgumentExpression("type")] string? paramName = null)
		{
			if (!type.IsSubclassOf(typeof(Delegate)))
			{
				throw new ArgumentException(ResourcesUtil.Format(Resources.MustBeDelegate_Type, type), paramName);
			}
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
		/// <param name="type">异常的类型。</param>
		/// <param name="paramName">产生异常的参数名称。</param>
		/// <returns><see cref="ArgumentException"/> 对象。</returns>
		public static ArgumentException MustBeEnum(Type type, [CallerArgumentExpression("type")] string? paramName = null)
		{
			return new ArgumentException(ResourcesUtil.Format(Resources.MustBeEnum_Type, type), paramName);
		}

		/// <summary>
		/// 检查指定类型是否是枚举类型，如果不是则抛出相应异常。
		/// </summary>
		/// <param name="type">要检查是否是枚举类型的类型。</param>
		/// <overloads>
		/// <summary>
		/// 检查指定类型是否是枚举类型，如果不是则抛出相应异常。
		/// </summary>
		/// </overloads>
		public static void CheckEnumType(Type type)
		{
			if (!type.IsEnum)
			{
				throw new ArgumentException(Resources.MustBeEnum);
			}
		}

		/// <summary>
		/// 检查指定类型是否是枚举类型，如果不是则抛出相应异常。
		/// </summary>
		/// <param name="type">要检查是否是枚举类型的类型。</param>
		/// <param name="paramName">被检查的参数名。</param>
		public static void CheckEnumType(Type type, [CallerArgumentExpression("type")] string? paramName = null)
		{
			if (!type.IsEnum)
			{
				throw new ArgumentException(ResourcesUtil.Format(Resources.MustBeEnum_Type, type), paramName);
			}
		}

		#endregion // 类型异常

		#region 格式异常

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

	}
}
