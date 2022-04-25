using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb;
using Cyjb.Collections;
using Cyjb.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCyjb.Collections.TestCharSet;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="CharSet"/> 类的性能测试。
	/// </summary>
	public partial class UnitTestCharSet
	{
		/// <summary>
		/// 对 <see cref="CharSet"/> 的添加性能进行测试。
		/// </summary>
		[TestMethod]
		public void TestPerformance()
		{
			Random random = Random.Shared;
			List<(bool isAdd, char ch)> charOperation = new();
			List<(bool isAdd, char start, char end)> smallRangeOperation = new();
			List<(bool isAdd, char start, char end)> largeRangeOperation = new();
			int loopCount = 500;
			for (int i = 0; i < 5000; i++)
			{
				int value = random.Next(char.MaxValue);
				// 70% 添加 30% 移除。
				bool isAdd = random.NextBoolean(0.7);
				charOperation.Add((isAdd, (char)value));
				int max = value + Random.Shared.Next(0, 10);
				if (max >= char.MaxValue)
				{
					max = char.MaxValue - 1;
				}
				smallRangeOperation.Add((isAdd, (char)value, (char)max));
				max = value + Random.Shared.Next(0, 100);
				if (max >= char.MaxValue)
				{
					max = char.MaxValue - 1;
				}
				largeRangeOperation.Add((isAdd, (char)value, (char)max));
			}
			string list1 = new(5000.Times(() => (char)random.Next(char.MaxValue)).ToArray());
			string list2 = new(5000.Times(() => (char)random.Next(char.MaxValue)).ToArray());

			Dictionary<string, SetWrapper> sets = new()
			{
				{ "HashSet", new HashSetWrapper() },
				{ "CharSet", new CharSetWrapper() },
				{ "RangeCharSet", new RangeCharSetWrapper() },
				{ "OldBitCharSet", new OldBitCharSetWrapper() },
			};
			Dictionary<string, Performance.Result[]> resultMap = new();
			foreach (var (name, wrapper) in sets)
			{
				List<Performance.Result> result = new();
				result.Add(Performance.Measure("", loopCount, () =>
				{
					ISet<char> set = wrapper.CreateInstance();
					foreach (var (isAdd, ch) in charOperation)
					{
						if (isAdd)
						{
							set.Add(ch);
						}
						else
						{
							set.Remove(ch);
						}
					}
				}));
				result.Add(Performance.Measure("", loopCount, () =>
				{
					ISet<char> set = wrapper.CreateInstance();
					foreach (var (isAdd, start, end) in smallRangeOperation)
					{
						if (isAdd)
						{
							wrapper.AddRange(set, start, end);
						}
						else
						{
							wrapper.RemoveRange(set, start, end);
						}
					}
				}));
				result.Add(Performance.Measure("", loopCount, () =>
				{
					ISet<char> set = wrapper.CreateInstance();
					foreach (var (isAdd, start, end) in largeRangeOperation)
					{
						if (isAdd)
						{
							wrapper.AddRange(set, start, end);
						}
						else
						{
							wrapper.RemoveRange(set, start, end);
						}
					}
				}));
				ISet<char> set = wrapper.CreateInstance();
				set.UnionWith(list1);
				result.Add(Performance.Measure("", 100, () =>
				{
					for (int i = 0; i <= char.MaxValue; i++)
					{
						set.Contains((char)i);
					}
				}));
				List<ISet<char>> set1 = new();
				List<ISet<char>> set2 = new();
				for (int i = 0; i < 100 * 4; i++)
				{
					ISet<char> tmp = wrapper.CreateInstance();
					tmp.UnionWith(list1);
					set1.Add(tmp);

					tmp = wrapper.CreateInstance();
					tmp.UnionWith(list2);
					set2.Add(tmp);
				}
				int index = 0;
				result.Add(Performance.Measure("", 100, () =>
				{
					set1[index].UnionWith(set2[index]);
					index++;
					set1[index].ExceptWith(set2[index]);
					index++;
					set1[index].SymmetricExceptWith(set2[index]);
					index++;
					set1[index].IntersectWith(set2[index]);
					index++;
				}));
				resultMap[name] = result.ToArray();
			}

			string[] types = { "HashSet", "CharSet", "RangeCharSet", "OldBitCharSet" };
			string[] names = { "Char", "SmallRange", "LargeRange", "Contains", "SetOps" };
			for (int i = 0; i < names.Length; i++)
			{
				Console.WriteLine("{0}:", names[i]);
				foreach (string type in types)
				{
					Console.WriteLine("\t{0}: {1}", type, resultMap[type][i].ToShortString());
				}
			}
		}

		/// <summary>
		/// 集合操作的包装。
		/// </summary>
		abstract class SetWrapper
		{
			/// <summary>
			/// 创建一个新实例。
			/// </summary>
			/// <returns>新实例</returns>
			public abstract ISet<char> CreateInstance();

			/// <summary>
			/// 添加指定的字符范围。
			/// </summary>
			/// <param name="set">集合的实例。</param>
			/// <param name="start">字符范围的起始。</param>
			/// <param name="end">字符范围的结束。</param>
			public virtual void AddRange(ISet<char> set, char start, char end)
			{
				for (char i = start; i <= end; i++)
				{
					set.Add(i);
				}
			}

			/// <summary>
			/// 移除指定的字符范围。
			/// </summary>
			/// <param name="set">集合的实例。</param>
			/// <param name="start">字符范围的起始。</param>
			/// <param name="end">字符范围的结束。</param>
			public virtual void RemoveRange(ISet<char> set, char start, char end)
			{
				for (char i = start; i <= end; i++)
				{
					set.Remove(i);
				}
			}
		}

		class HashSetWrapper : SetWrapper
		{
			/// <summary>
			/// 创建一个新实例。
			/// </summary>
			/// <returns>新实例</returns>
			public override ISet<char> CreateInstance()
			{
				return new HashSet<char>();
			}
		}

		class CharSetWrapper : SetWrapper
		{
			/// <summary>
			/// 创建一个新实例。
			/// </summary>
			/// <returns>新实例</returns>
			public override ISet<char> CreateInstance()
			{
				return new CharSet();
			}

			/// <summary>
			/// 添加指定的字符范围。
			/// </summary>
			/// <param name="set">集合的实例。</param>
			/// <param name="start">字符范围的起始。</param>
			/// <param name="end">字符范围的结束。</param>
			public override void AddRange(ISet<char> set, char start, char end)
			{
				((CharSet)set).Add(start, end);
			}

			/// <summary>
			/// 移除指定的字符范围。
			/// </summary>
			/// <param name="set">集合的实例。</param>
			/// <param name="start">字符范围的起始。</param>
			/// <param name="end">字符范围的结束。</param>
			public override void RemoveRange(ISet<char> set, char start, char end)
			{
				((CharSet)set).Remove(start, end);
			}
		}

		class RangeCharSetWrapper : SetWrapper
		{
			/// <summary>
			/// 创建一个新实例。
			/// </summary>
			/// <returns>新实例</returns>
			public override ISet<char> CreateInstance()
			{
				return new RangeCharSet();
			}

			/// <summary>
			/// 添加指定的字符范围。
			/// </summary>
			/// <param name="set">集合的实例。</param>
			/// <param name="start">字符范围的起始。</param>
			/// <param name="end">字符范围的结束。</param>
			public override void AddRange(ISet<char> set, char start, char end)
			{
				((RangeCharSet)set).Add(start, end);
			}

			/// <summary>
			/// 移除指定的字符范围。
			/// </summary>
			/// <param name="set">集合的实例。</param>
			/// <param name="start">字符范围的起始。</param>
			/// <param name="end">字符范围的结束。</param>
			public override void RemoveRange(ISet<char> set, char start, char end)
			{
				((RangeCharSet)set).Remove(start, end);
			}
		}

		class OldBitCharSetWrapper : SetWrapper
		{
			/// <summary>
			/// 创建一个新实例。
			/// </summary>
			/// <returns>新实例</returns>
			public override ISet<char> CreateInstance()
			{
				return new OldBitCharSet();
			}
		}
	}
}
