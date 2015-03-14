using System;
using System.Diagnostics.Contracts;

namespace Cyjb
{
	/// <summary>
	/// 提供 <see cref="Predicate{T}"/> 的扩展方法。
	/// </summary>
	public static class Predicate
	{
		/// <summary>
		/// 返回要求当前条件与指定条件进行同时满足的条件。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		/// <param name="predicate">当前条件。</param>
		/// <param name="other">其它条件。</param>
		/// <returns>要求当前条件与指定条件进行同时满足的条件。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 返回要求当前条件与指定条件进行同时满足的条件。
		/// </summary>
		/// </overloads>
		public static Predicate<T> And<T>(this Predicate<T> predicate, Predicate<T> other)
		{
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
			CommonExceptions.CheckArgumentNull(other, "other");
			Contract.Ensures(Contract.Result<Predicate<T>>() != null);
			return obj => predicate(obj) && other(obj);
		}
		/// <summary>
		/// 返回要求当前条件与指定条件进行同时满足的条件。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		/// <param name="predicate">当前条件。</param>
		/// <param name="others">其它条件。</param>
		/// <returns>要求当前条件与指定条件进行同时满足的条件。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="others"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="others"/> 中包含为 <c>null</c> 的元素。</exception>
		public static Predicate<T> And<T>(this Predicate<T> predicate, params Predicate<T>[] others)
		{
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
			CommonExceptions.CheckCollectionItemNull(others, "others");
			Contract.Ensures(Contract.Result<Predicate<T>>() != null);
			return obj =>
			{
				if (!predicate(obj))
				{
					return false;
				}
				for (int i = 0; i < others.Length; i++)
				{
					if (!others[i](obj))
					{
						return false;
					}
				}
				return true;
			};
		}
		/// <summary>
		/// 返回要求当前条件与指定条件满足任意一个的条件。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		/// <param name="predicate">当前条件。</param>
		/// <param name="other">其它条件。</param>
		/// <returns>要求当前条件与指定条件满足任意一个的条件。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 返回要求当前条件与指定条件满足任意一个的条件。
		/// </summary>
		/// </overloads>
		public static Predicate<T> Or<T>(this Predicate<T> predicate, Predicate<T> other)
		{
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
			CommonExceptions.CheckArgumentNull(other, "other");
			Contract.Ensures(Contract.Result<Predicate<T>>() != null);
			return obj => predicate(obj) || other(obj);
		}
		/// <summary>
		/// 返回要求当前条件与指定条件满足任意一个的条件。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		/// <param name="predicate">当前条件。</param>
		/// <param name="others">其它条件。</param>
		/// <returns>返回要求当前条件与指定条件满足任意一个的条件。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="others"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="others"/> 中包含为 <c>null</c> 的元素。</exception>
		public static Predicate<T> Or<T>(this Predicate<T> predicate, params Predicate<T>[] others)
		{
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
			CommonExceptions.CheckCollectionItemNull(others, "others");
			Contract.Ensures(Contract.Result<Predicate<T>>() != null);
			return obj =>
			{
				if (predicate(obj))
				{
					return true;
				}
				for (int i = 0; i < others.Length; i++)
				{
					if (others[i](obj))
					{
						return true;
					}
				}
				return false;
			};
		}
		/// <summary>
		/// 返回不能满足当前条件的条件。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		/// <param name="predicate">当前条件。</param>
		/// <returns>不能满足当前条件的条件。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		public static Predicate<T> Not<T>(this Predicate<T> predicate)
		{
			CommonExceptions.CheckArgumentNull(predicate, "predicate");
			Contract.Ensures(Contract.Result<Predicate<T>>() != null);
			return obj => !predicate(obj);
		}
		/// <summary>
		/// 返回总是满足的条件。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		/// <returns>总是满足件的条件。</returns>
		public static Predicate<T> True<T>()
		{
			Contract.Ensures(Contract.Result<Predicate<T>>() != null);
			return PredicateCache<T>.TruePredicate;
		}
		/// <summary>
		/// 返回总是不满足的条件。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		/// <returns>总是不满足件的条件。</returns>
		public static Predicate<T> False<T>()
		{
			Contract.Ensures(Contract.Result<Predicate<T>>() != null);
			return PredicateCache<T>.FalsePredicate;
		}
		/// <summary>
		/// 条件的缓存。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型。</typeparam>
		private static class PredicateCache<T>
		{
			/// <summary>
			/// 总是满足的条件。
			/// </summary>
			public static readonly Predicate<T> TruePredicate = obj => true;
			/// <summary>
			/// 总是不满足的条件。
			/// </summary>
			public static readonly Predicate<T> FalsePredicate = obj => false;
		}
	}
}
