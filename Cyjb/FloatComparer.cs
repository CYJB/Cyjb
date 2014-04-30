using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Cyjb
{
	/// <summary>
	/// 表示单精度浮点数的比较器。
	/// </summary>
	public sealed class FloatComparer : Comparer<float>
	{
		/// <summary>
		/// 默认的单精度浮点数比较器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static FloatComparer defaultComparer;
		/// <summary>
		/// 比较时使用的默认精度。
		/// </summary>
		public const float DefaultEpsilon = 1e-7F;
		/// <summary>
		/// 获取默认的单精度浮点数比较器。
		/// </summary>
		/// <value>默认的单精度浮点数比较器。</value>
		public new static FloatComparer Default
		{
			get
			{
				if (defaultComparer == null)
				{
					Interlocked.CompareExchange(ref defaultComparer, new FloatComparer(DefaultEpsilon), null);
				}
				return defaultComparer;
			}
		}
		/// <summary>
		/// 比较时使用的精度。
		/// </summary>
		private readonly float epsilon;
		/// <summary>
		/// 使用比较时要使用的精度，初始化 <see cref="FloatComparer"/> 类的新实例。
		/// </summary>
		/// <param name="epsilon">比较时使用的精度。</param>
		public FloatComparer(float epsilon)
		{
			if (epsilon <= 0)
			{
				throw ExceptionHelper.ArgumentMustBePositive("epsilon");
			}
			this.epsilon = epsilon;
		}

		#region IComparer<float> 成员

		/// <summary>
		/// 比较两个单精度浮点数并返回一个值，指示一个单精度浮点数是小于、等于还是大于另一个单精度浮点数。
		/// </summary>
		/// <param name="x">要比较的第一个单精度浮点数。</param>
		/// <param name="y">要比较的第二个单精度浮点数。</param>
		/// <returns>一个有符号整数，指示 <paramref name="x"/> 与 <paramref name="y"/> 的相对值。</returns>
		public override int Compare(float x, float y)
		{
			if (x > y)
			{
				float eps = x;
				if (x < 0 || (y < 0 && x + y < 0))
				{
					eps = -y;
				}
				return (x - y) < eps * this.epsilon ? 0 : 1;
			}
			if (x < y)
			{
				float eps = y;
				if (y < 0 || (x < 0 && x + y < 0))
				{
					eps = -x;
				}
				return (y - x) < eps * this.epsilon ? 0 : -1;
			}
			if (float.IsNaN(x))
			{
				return float.IsNaN(y) ? 0 : -1;
			}
			return float.IsNaN(y) ? 1 : 0;
		}

		#endregion

	}
}
