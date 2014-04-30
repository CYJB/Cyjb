using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb
{
	/// <summary>
	/// 表示双精度浮点数的比较器。
	/// </summary>
	public sealed class DoubleComparer : Comparer<double>
	{
		/// <summary>
		/// 默认的双精度浮点数比较器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static DoubleComparer defaultComparer;
		/// <summary>
		/// 比较时使用的默认精度。
		/// </summary>
		public const double DefaultEpsilon = 1e-15;
		/// <summary>
		/// 获取默认的双精度浮点数比较器。
		/// </summary>
		/// <value>默认的双精度浮点数比较器。</value>
		public new static DoubleComparer Default
		{
			get
			{
				if (defaultComparer == null)
				{
					Interlocked.CompareExchange(ref defaultComparer, new DoubleComparer(DefaultEpsilon), null);
				}
				return defaultComparer;
			}
		}
		/// <summary>
		/// 比较时使用的精度。
		/// </summary>
		private readonly double epsilon;
		/// <summary>
		/// 使用比较时要使用的精度，初始化 <see cref="DoubleComparer"/> 类的新实例。
		/// </summary>
		/// <param name="epsilon">比较时使用的精度。</param>
		public DoubleComparer(double epsilon)
		{
			if (epsilon <= 0)
			{
				throw ExceptionHelper.ArgumentMustBePositive("epsilon");
			}
			this.epsilon = epsilon;
		}

		#region IComparer<double> 成员

		/// <summary>
		/// 比较两个双精度浮点数并返回一个值，指示一个双精度浮点数是小于、等于还是大于另一个双精度浮点数。
		/// </summary>
		/// <param name="x">要比较的第一个双精度浮点数。</param>
		/// <param name="y">要比较的第二个双精度浮点数。</param>
		/// <returns>一个有符号整数，指示 <paramref name="x"/> 与 <paramref name="y"/> 的相对值。</returns>
		public override int Compare(double x, double y)
		{
			if (x > y)
			{
				double eps = x;
				if (x < 0 || (y < 0 && x + y < 0))
				{
					eps = -y;
				}
				return (x - y) < eps * this.epsilon ? 0 : 1;
			}
			if (x < y)
			{
				double eps = y;
				if (y < 0 || (x < 0 && x + y < 0))
				{
					eps = -x;
				}
				return (y - x) < eps * this.epsilon ? 0 : -1;
			}
			if (double.IsNaN(x))
			{
				return double.IsNaN(y) ? 0 : -1;
			}
			return double.IsNaN(y) ? 1 : 0;
		}

		#endregion

	}
}
