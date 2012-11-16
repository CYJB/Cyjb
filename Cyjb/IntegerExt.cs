using System;
using System.Collections.Generic;

namespace Cyjb
{
	/// <summary>
	/// 提供对整数的扩展方法。
	/// </summary>
	public static class IntegerExt
	{

		#region Int32 操作

		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="action">要执行的操作。</param>
		public static void Times(this int source, Action action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (int i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="action">要执行的操作，参数为当前执行的次数。</param>
		public static void Times(this int source, Action<int> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (int i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="func">要执行的操作。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		public static void Times(this int source, Func<bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (int i = 0; i < source; i++)
			{
				if (!func())
				{
					break;
				}
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="func">要执行的操作，参数为当前执行的次数。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		public static void Times(this int source, Func<int, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (int i = 0; i < source; i++)
			{
				if (!func(i))
				{
					break;
				}
			}
		}
		/// <summary>
		/// 返回从当前值递增（递减）到特定值的序列。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <returns>数值的序列。</returns>
		public static IEnumerable<int> To(this int source, int destination)
		{
			if (source < destination)
			{
				while (source <= destination)
				{
					yield return source;
					source++;
				}
			}
			else
			{
				while (source >= destination)
				{
					yield return source;
					source--;
				}
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="action">要执行的操作，参数为当前的值。</param>
		public static void To(this int source, int destination,
			Action<int> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "func");
			if (source < destination)
			{
				while (source <= destination)
				{
					action(source);
					source++;
				}
			}
			else
			{
				while (source >= destination)
				{
					action(source);
					source--;
				}
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作，可以随时停止执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="func">要执行的操作，参数为当前的值。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		public static void To(this int source, int destination,
			Func<int, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			if (source < destination)
			{
				while (source <= destination && func(source))
				{
					source++;
				}
			}
			else
			{
				while (source >= destination && func(source))
				{
					source--;
				}
			}
		}

		#endregion // Int32 操作

		#region Int64 操作

		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="action">要执行的操作。</param>
		public static void Times(this long source, Action action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (long i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="action">要执行的操作，参数为当前执行的次数。</param>
		public static void Times(this long source, Action<long> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (long i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="func">要执行的操作。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		public static void Times(this long source, Func<bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (long i = 0; i < source; i++)
			{
				if (!func())
				{
					break;
				}
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 0 时才有效。
		/// </param>
		/// <param name="func">要执行的操作，参数为当前执行的次数。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		public static void Times(this long source, Func<long, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (long i = 0; i < source; i++)
			{
				if (!func(i))
				{
					break;
				}
			}
		}
		/// <summary>
		/// 返回从当前值递增（递减）到特定值的序列。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <returns>数值的序列。</returns>
		public static IEnumerable<long> To(this long source, long destination)
		{
			if (source < destination)
			{
				while (source <= destination)
				{
					yield return source;
					source++;
				}
			}
			else
			{
				while (source >= destination)
				{
					yield return source;
					source--;
				}
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="action">要执行的操作，参数为当前的值。</param>
		public static void To(this long source, long destination,
			Action<long> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "func");
			if (source < destination)
			{
				while (source <= destination)
				{
					action(source);
					source++;
				}
			}
			else
			{
				while (source >= destination)
				{
					action(source);
					source--;
				}
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作，可以随时停止执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="func">要执行的操作，参数为当前的值。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		public static void To(this long source, long destination,
			Func<long, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			if (source < destination)
			{
				while (source <= destination && func(source))
				{
					source++;
				}
			}
			else
			{
				while (source >= destination && func(source))
				{
					source--;
				}
			}
		}

		#endregion // Int64 操作

		#region UInt32 操作

		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作。</param>
		[CLSCompliant(false)]
		public static void Times(this uint source, Action action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (uint i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作，参数为当前执行的次数。</param>
		[CLSCompliant(false)]
		public static void Times(this uint source, Action<uint> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (uint i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="func">要执行的操作。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		[CLSCompliant(false)]
		public static void Times(this uint source, Func<bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (uint i = 0; i < source; i++)
			{
				if (!func())
				{
					break;
				}
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。 </param>
		/// <param name="func">要执行的操作，参数为当前执行的次数。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		[CLSCompliant(false)]
		public static void Times(this uint source, Func<uint, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (uint i = 0; i < source; i++)
			{
				if (!func(i))
				{
					break;
				}
			}
		}
		/// <summary>
		/// 返回从当前值递增（递减）到特定值的序列。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <returns>数值的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<uint> To(this uint source, uint destination)
		{
			if (source < destination)
			{
				while (source < destination)
				{
					yield return source;
					source++;
				}
				yield return source;
			}
			else
			{
				while (source > destination)
				{
					yield return source;
					source--;
				}
				yield return source;
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="action">要执行的操作，参数为当前的值。</param>
		[CLSCompliant(false)]
		public static void To(this uint source, uint destination,
			Action<uint> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "func");
			if (source < destination)
			{
				while (source < destination)
				{
					action(source);
					source++;
				}
				action(source);
			}
			else
			{
				while (source > destination)
				{
					action(source);
					source--;
				}
				action(source);
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作，可以随时停止执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="func">要执行的操作，参数为当前的值。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		[CLSCompliant(false)]
		public static void To(this uint source, uint destination,
			Func<uint, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			if (source < destination)
			{
				while (source < destination && func(source))
				{
					source++;
				}
				func(source);
			}
			else
			{
				while (source > destination && func(source))
				{
					source--;
				}
				func(source);
			}
		}

		#endregion // UInt32 操作

		#region UInt64 操作

		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作。</param>
		[CLSCompliant(false)]
		public static void Times(this ulong source, Action action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (ulong i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将特定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作，参数为当前执行的次数。</param>
		[CLSCompliant(false)]
		public static void Times(this ulong source, Action<ulong> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (ulong i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="func">要执行的操作。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		[CLSCompliant(false)]
		public static void Times(this ulong source, Func<bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (ulong i = 0; i < source; i++)
			{
				if (!func())
				{
					break;
				}
			}
		}
		/// <summary>
		/// 将特定操作执行多次，并可以随时停止执行。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="func">要执行的操作，参数为当前执行的次数。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		[CLSCompliant(false)]
		public static void Times(this ulong source, Func<ulong, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			for (ulong i = 0; i < source; i++)
			{
				if (!func(i))
				{
					break;
				}
			}
		}
		/// <summary>
		/// 返回从当前值递增（递减）到特定值的序列。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <returns>数值的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<ulong> To(this ulong source, ulong destination)
		{
			if (source < destination)
			{
				while (source < destination)
				{
					yield return source;
					source++;
				}
				yield return source;
			}
			else
			{
				while (source > destination)
				{
					yield return source;
					source--;
				}
				yield return source;
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="action">要执行的操作，参数为当前的值。</param>
		[CLSCompliant(false)]
		public static void To(this ulong source, ulong destination,
			Action<ulong> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "func");
			if (source < destination)
			{
				while (source < destination)
				{
					action(source);
					source++;
				}
				action(source);
			}
			else
			{
				while (source > destination)
				{
					action(source);
					source--;
				}
				action(source);
			}
		}
		/// <summary>
		/// 从当前值递增（递减）到特定值并执行操作，可以随时停止执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="func">要执行的操作，参数为当前的值。返回 
		/// <c>true</c> 则继续执行，<c>false</c> 则停止。</param>
		[CLSCompliant(false)]
		public static void To(this ulong source, ulong destination,
			Func<ulong, bool> func)
		{
			ExceptionHelper.CheckArgumentNull(func, "func");
			if (source < destination)
			{
				while (source < destination && func(source))
				{
					source++;
				}
				func(source);
			}
			else
			{
				while (source > destination && func(source))
				{
					source--;
				}
				func(source);
			}
		}

		#endregion // UInt64 操作

	}
}
