using System;

namespace Cyjb
{
	/// <summary>
	/// 提供用于异常处理的辅助方法。
	/// </summary>
	public static class ExceptionHelper
	{

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

		#endregion

	}
}
