using System.Diagnostics;
using System.Text;

namespace Cyjb.Test;

/// <summary>
/// 测试代码执行性能。
/// </summary>
public sealed class Performance
{
	static Performance()
	{
		// 方法预热。
		Measure("", 1, () => { });
	}

	/// <summary>
	/// 执行性能结果。
	/// </summary>
	public class Result
	{
		/// <summary>
		/// 使用指定的名称、运行时间和 GC 回收次数初始化。
		/// </summary>
		/// <param name="name">执行的名称。</param>
		/// <param name="elapsedMilliseconds">总运行时间（毫秒）</param>
		/// <param name="collectionCount">GC 回收次数。</param>
		internal Result(string name, long elapsedMilliseconds, int[] collectionCount)
		{
			Name = name;
			ElapsedMilliseconds = elapsedMilliseconds;
			GCCollectionCount = collectionCount;
		}

		/// <summary>
		/// 执行的名称。
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// 总运行时间（毫秒）。
		/// </summary>
		public long ElapsedMilliseconds { get; }
		/// <summary>
		/// GC 回收次数。
		/// </summary>
		public int[] GCCollectionCount { get; }

		/// <summary>
		/// 输出当前执行结果。
		/// </summary>
		public void Print()
		{
			ConsoleColor currentForeColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(Name);
			Console.ForegroundColor = currentForeColor;
			Console.WriteLine("\tTime Elapsed:\t" + ElapsedMilliseconds.ToString("N0") + "ms");
			for (int i = 0; i < GCCollectionCount.Length; i++)
			{
				Console.WriteLine("\tGen " + i + ": \t\t" + GCCollectionCount[i]);
			}
			Console.WriteLine();
		}

		/// <summary>
		/// 返回当前执行结果的短字符字符串。
		/// </summary>
		/// <returns>当前执行结果的短字符字符串。</returns>
		public string ToShortString()
		{
			StringBuilder text = new();
			text.AppendFormat("{0:N0}ms, ", ElapsedMilliseconds);
			for (int i = 0; i < GCCollectionCount.Length; i++)
			{
				if (i > 0)
				{
					text.Append('/');
				}
				text.Append(GCCollectionCount[i]);
			}
			return text.ToString();
		}
	}

	/// <summary>
	/// 测量指定代码的执行性能。
	/// </summary>
	/// <param name="name">执行的名称。</param>
	/// <param name="iteration">要循环的次数。</param>
	/// <param name="action">要执行的操作。</param>
	/// <returns></returns>
	public static Result Measure(string name, int iteration, Action action)
	{
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		int[] collectionCount = new int[GC.MaxGeneration + 1];
		for (int i = 0; i <= GC.MaxGeneration; i++)
		{
			collectionCount[i] = GC.CollectionCount(i);
		}

		Stopwatch watch = new();
		watch.Start();
		for (int i = 0; i < iteration; i++) action();
		watch.Stop();

		for (int i = 0; i <= GC.MaxGeneration; i++)
		{
			collectionCount[i] = GC.CollectionCount(i) - collectionCount[i];
		}

		return new Result(name, watch.ElapsedMilliseconds, collectionCount);
	}
}
