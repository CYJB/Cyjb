using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示根据内容比较 <see cref="Cyjb.Collections.BitList"/> 集合的比较器。
	/// </summary>
	public sealed class BitListEqualityComparer : EqualityComparer<BitList>
	{
		/// <summary>
		/// 默认的比较器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static BitListEqualityComparer defaultValue;
		/// <summary>
		/// 返回一个默认的相等比较器。
		/// </summary>
		/// <value>默认的 <see cref="BitListEqualityComparer"/> 对象实例。</value>
		public new static BitListEqualityComparer Default
		{
			get
			{
				Contract.Ensures(Contract.Result<BitListEqualityComparer>() != null);
				if (defaultValue == null)
				{
					Interlocked.CompareExchange(ref defaultValue, new BitListEqualityComparer(), null);
				}
				return defaultValue;
			}
		}
		/// <summary>
		/// 初始化 <see cref="BitListEqualityComparer"/> 类的新实例。
		/// </summary>
		public BitListEqualityComparer() { }

		#region EqualityComparer<BitList> 成员

		/// <summary>
		/// 确定指定的对象是否相等。
		/// </summary>
		/// <param name="x">要比较的第一个 <see cref="BitList"/> 的对象。</param>
		/// <param name="y">要比较的第二个 <see cref="BitList"/> 的对象。</param>
		/// <returns>如果指定的对象相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 确定指定的对象是否相等。
		/// </summary>
		/// </overloads>
		public override bool Equals(BitList x, BitList y)
		{
			if (x == null)
			{
				return y == null;
			}
			if (y == null)
			{
				return false;
			}
			return x == y || x.ContentEquals(y);
		}
		/// <summary>
		/// 返回指定对象的哈希代码。
		/// </summary>
		/// <param name="obj">将为其返回哈希代码。</param>
		/// <returns>指定对象的哈希代码。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="obj"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 返回指定对象的哈希代码。
		/// </summary>
		/// </overloads>
		public override int GetHashCode(BitList obj)
		{
			CommonExceptions.CheckArgumentNull(obj, "obj");
			Contract.EndContractBlock();
			return obj.GetContentHashCode();
		}

		#endregion // EqualityComparer<BitList> 成员

	}
}
