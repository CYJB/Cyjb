using System.Text;
using BenchmarkDotNet.Attributes;
using Cyjb.Collections;

namespace Cyjb.Benchmark;

/// <summary>
/// 耗时约为 <see cref="Dictionary{TKey, TValue}"/> 的 150%~200%。
/// </summary>
/// <remarks>
/// | Method     | ItemCount | Length | FailRatio | Mean       | Error      | StdDev     | Ratio | RatioSD |<br/>
/// |----------- |---------- |------- |---------- |-----------:|-----------:|-----------:|------:|--------:|<br/>
/// | Dictionary | 100       | 30     | 0         |   1.241 us |  0.0189 us |  0.0177 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 100       | 30     | 0         |   1.933 us |  0.0197 us |  0.0164 us |  1.56 |    0.02 |<br/>
/// <br/>
/// | Dictionary | 100       | 30     | 0.2       |   1.448 us |  0.0170 us |  0.0159 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 100       | 30     | 0.2       |   2.186 us |  0.0311 us |  0.0291 us |  1.51 |    0.03 |<br/>
/// <br/>
/// | Dictionary | 100       | 30     | 0.8       |   2.080 us |  0.0203 us |  0.0169 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 100       | 30     | 0.8       |   2.898 us |  0.0322 us |  0.0286 us |  1.39 |    0.02 |<br/>
/// <br/>
/// | Dictionary | 100       | 100    | 0         |   2.041 us |  0.0207 us |  0.0193 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 100       | 100    | 0         |   2.001 us |  0.0231 us |  0.0205 us |  0.98 |    0.01 |<br/>
/// <br/>
/// | Dictionary | 100       | 100    | 0.2       |   2.280 us |  0.0299 us |  0.0280 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 100       | 100    | 0.2       |   2.323 us |  0.0257 us |  0.0241 us |  1.02 |    0.02 |<br/>
/// <br/>
/// | Dictionary | 100       | 100    | 0.8       |   3.574 us |  0.0462 us |  0.0409 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 100       | 100    | 0.8       |   2.933 us |  0.0587 us |  0.0576 us |  0.82 |    0.02 |<br/>
/// <br/>
/// | Dictionary | 10000     | 30     | 0         | 260.980 us |  2.9378 us |  2.7480 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 10000     | 30     | 0         | 520.553 us |  7.6669 us |  7.1716 us |  1.99 |    0.03 |<br/>
/// <br/>
/// | Dictionary | 10000     | 30     | 0.2       | 319.379 us |  3.2238 us |  3.0156 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 10000     | 30     | 0.2       | 575.521 us |  7.2143 us |  6.7483 us |  1.80 |    0.03 |<br/>
/// <br/>
/// | Dictionary | 10000     | 30     | 0.8       | 489.402 us |  9.0296 us |  8.4463 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 10000     | 30     | 0.8       | 751.585 us |  9.0648 us |  8.0357 us |  1.54 |    0.03 |<br/>
/// <br/>
/// | Dictionary | 10000     | 100    | 0         | 334.758 us |  3.7524 us |  3.5100 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 10000     | 100    | 0         | 595.534 us |  8.0782 us |  7.1611 us |  1.78 |    0.03 |<br/>
/// <br/>
/// | Dictionary | 10000     | 100    | 0.2       | 413.282 us |  4.0451 us |  3.7838 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 10000     | 100    | 0.2       | 657.353 us | 11.2682 us | 10.5403 us |  1.59 |    0.03 |<br/>
/// <br/>
/// | Dictionary | 10000     | 100    | 0.8       | 627.869 us | 10.2437 us |  9.5820 us |  1.00 |    0.00 |<br/>
/// | PrefixTree | 10000     | 100    | 0.8       | 834.827 us |  8.3537 us |  6.9758 us |  1.33 |    0.03 |
/// </remarks>
public class ReadOnlyPrefixTreeBenchmark
{
	private readonly HashSet<string> texts = new();
	private readonly StringBuilder text = new();

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

	private KeyValuePair<string, int>[] items;
	private string[] tests;
	private Dictionary<string, int> dict;
	private ReadOnlyPrefixTree<int> tree;

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

	/// <summary>
	/// 项的个数。
	/// </summary>
	[Params(100, 10000)]
	public int ItemCount;
	/// <summary>
	/// Key 的最大长度。
	/// </summary>
	[Params(30, 100)]
	public int Length;
	/// <summary>
	/// 测试的匹配失败比例。
	/// </summary>
	[Params(0, 0.2, 0.8)]
	public double FailRatio;

	[GlobalSetup]
	public void Setup()
	{
		int length = (int)(ItemCount * (1 + FailRatio));
		texts.Clear();
		while (texts.Count < length)
		{
			texts.Add(RandomText());
		}
		tests = new string[length];
		texts.CopyTo(tests);
		items = new KeyValuePair<string, int>[ItemCount];
		for (int i = 0; i < ItemCount; i++)
		{
			items[i] = new KeyValuePair<string, int>(tests[i], i);
		}
		dict = new Dictionary<string, int>(items);
		tree = new ReadOnlyPrefixTree<int>(items);
	}

	private string RandomText()
	{
		text.Clear();
		int length = Random.Shared.Next(3, Length);
		for (int j = 0; j < length; j++)
		{
			text.Append((char)Random.Shared.Next(97, 123));
		}
		return text.ToString();
	}

	[Benchmark(Baseline = true)]
	public bool Dictionary()
	{
		bool value = false;
		for (int i = 0; i < tests.Length; i++)
		{
			value = dict.TryGetValue(tests[i], out _);
		}
		return value;
	}

	[Benchmark]
	public bool PrefixTree()
	{
		bool value = false;
		for (int i = 0; i < tests.Length; i++)
		{
			value = tree.TryGetValue(tests[i], out _);
		}
		return value;
	}
}

