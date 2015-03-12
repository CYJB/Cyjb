using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cyjb.Utility
{
	/// <summary>
	/// 提供 Hash 的辅助方法。
	/// </summary>
	public static class Hash
	{
		/// <summary>
		/// Hash 算法使用的近似黄金分割率：2^32 / ((1 + sqrt(5)) / 2) = 0x9e3779b9
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private const int GoldenRatio = unchecked((int)0x9E3779B9);
		/// <summary>
		/// 合并两个 Hash 值。
		/// </summary>
		/// <param name="seed">要合并的种子 Hash 值。</param>
		/// <param name="hash">要合并的 Hash 值。</param>
		/// <returns>结果 Hash 值。</returns>
		/// <remarks>算法来自 boost::hash_combine。</remarks>
		public static int Combine(int seed, int hash)
		{
			return seed ^ (hash + GoldenRatio + (seed << 6) + (seed >> 2));
		}
		/// <summary>
		/// 合并两个 Hash 值。
		/// </summary>
		/// <typeparam name="T">要合并 Hash 值的对象的类型。</typeparam>
		/// <param name="seed">要合并的种子 Hash 值。</param>
		/// <param name="obj">要合并 Hash 值的对象。</param>
		/// <returns>结果 Hash 值。</returns>
		/// <remarks>算法来自 boost::hash_combine。</remarks>
		public static int Combine<T>(int seed, T obj)
		{
			return Combine(seed, obj == null ? 0 : obj.GetHashCode());
		}
		/// <summary>
		/// 合并种子 Hash 值和一系列对象。
		/// </summary>
		/// <typeparam name="T">要合并 Hash 值的对象的类型。</typeparam>
		/// <param name="seed">要合并的种子 Hash 值。</param>
		/// <param name="objs">要合并 Hash 值的对象集合。</param>
		/// <returns>结果 Hash 值。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="objs"/> 为 <c>null</c>。</exception>
		/// <remarks>算法来自 boost::hash_range。</remarks>
		public static int Combine<T>(int seed, params T[] objs)
		{
			CommonExceptions.CheckArgumentNull(objs, "objs");
			Contract.EndContractBlock();
			for (int i = 0; i < objs.Length; i++)
			{
				seed = Combine(seed, objs[1]);
			}
			return seed;
		}
		/// <summary>
		/// 合并种子 Hash 值和一系列对象。
		/// </summary>
		/// <typeparam name="T">要合并 Hash 值的对象的类型。</typeparam>
		/// <param name="seed">要合并的种子 Hash 值。</param>
		/// <param name="objs">要合并 Hash 值的对象集合。</param>
		/// <returns>结果 Hash 值。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="objs"/> 为 <c>null</c>。</exception>
		/// <remarks>算法来自 boost::hash_range。</remarks>
		public static int Combine<T>(int seed, IEnumerable<T> objs)
		{
			CommonExceptions.CheckArgumentNull(objs, "objs");
			Contract.EndContractBlock();
			return objs.Aggregate(seed, Combine);
		}
	}
}
