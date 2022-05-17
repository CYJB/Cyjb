using System;
using Cyjb;
using Cyjb.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb;

/// <summary>
/// <see cref="MersenneTwister"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestMersenneTwister
{
	/// <summary>
	/// 对 <see cref="MersenneTwister.Next"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestNext()
	{
		MersenneTwister random = new(141);
		Assert.AreEqual(481096803, random.Next());
		Assert.AreEqual(0, random.Next(0));
		Assert.AreEqual(0, random.Next(1));
		Assert.AreEqual(1, random.Next(2));
		Assert.AreEqual(1, random.Next(2));
		Assert.AreEqual(0, random.Next(2));
		Assert.AreEqual(66, random.Next(123));
		Assert.AreEqual(-2, random.Next(-2, -2));
		Assert.AreEqual(-1, random.Next(-2, 0));
		Assert.AreEqual(868394208, random.Next(int.MinValue, int.MaxValue));
		Assert.AreEqual(956068202, random.Next(int.MinValue, int.MaxValue));
		Assert.AreEqual(1191642647, random.Next(int.MinValue, int.MaxValue));

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.Next(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.Next(2, 1));

		int[] values = new int[5];
		for (int i = 0; i < 1000000; i++)
		{
			values[random.Next(5)]++;
		}
		int[] expected = new int[] { 200024, 199490, 199765, 199976, 200745 };
		CollectionAssert.AreEqual(expected, values);
	}

	/// <summary>
	/// 对 <see cref="MersenneTwister.Bytes"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestNextBytes()
	{
		MersenneTwister random = new(13141);
		CollectionAssert.AreEqual(Array.Empty<byte>(), RandomBytes(random, 0));
		byte[] expected = new byte[] { 190 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 31, 202 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 93, 197, 236 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 11, 158, 232, 187 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 41, 200, 152, 233, 61 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 25, 55, 17, 158, 16, 250 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 71, 186, 133, 77, 253, 165, 33 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 238, 6, 151, 52, 135, 210, 19, 103 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 186, 200, 129, 132, 19, 215, 28, 63, 148 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 216, 187, 5, 150, 168, 246, 48, 19, 92, 186 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 205, 81, 55, 152, 116, 239, 103, 201, 18, 172, 189 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 252, 247, 170, 177, 133, 204, 124, 209, 149, 3, 238, 125 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 243, 107, 80, 109, 242, 114, 238, 156, 74, 78, 129, 71, 159 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 79, 177, 28, 182, 171, 119, 197, 231, 11, 152, 91, 1, 55, 149 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 139, 83, 183, 197, 10, 38, 23, 34, 152, 154, 200, 213, 208, 210, 209 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 47, 96, 214, 98, 1, 248, 66, 7, 87, 177, 73, 141, 228, 39, 47, 164 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));
		expected = new byte[] { 213, 225, 29, 41, 26, 7, 242, 58, 114, 6, 245, 179, 122, 68, 145, 250, 135 };
		CollectionAssert.AreEqual(expected, RandomBytes(random, expected.Length));

		Assert.ThrowsException<ArgumentNullException>(() => random.NextBytes(Fake.Null<byte[]>()));

		int[] values = new int[256];
		for (int i = 0; i < 1000000; i++)
		{
			foreach (byte v in RandomBytes(random, 10))
			{
				values[v]++;
			}
		}
		int[] expectedValue = new int[] {
			39058, 39103, 39212, 39065, 38826, 39143, 38642, 39117,
			38825, 39142, 39144, 38795, 39112, 39180, 39035, 39079,
			39042, 39309, 38942, 38960, 38983, 39197, 39307, 39234,
			38999, 38912, 38791, 38758, 39329, 39202, 38918, 39085,
			38721, 38929, 39151, 39069, 39119, 39253, 39254, 39219,
			38561, 38848, 38959, 38553, 38887, 39137, 38775, 39396,
			39270, 39096, 38949, 39350, 39058, 39040, 38779, 38771,
			39081, 39178, 38893, 39440, 39035, 38762, 39431, 38913,
			39050, 38992, 38973, 39324, 38790, 38805, 39005, 39156,
			39123, 39462, 38788, 39043, 39059, 39042, 39104, 39523,
			38836, 39030, 38788, 38902, 39109, 39212, 38935, 38828,
			38976, 39074, 39153, 39258, 39434, 38626, 38834, 38894,
			39148, 38922, 39140, 38904, 39216, 39385, 38996, 39327,
			38919, 39332, 38874, 39006, 39478, 39202, 38985, 38969,
			39419, 38908, 38925, 39147, 39202, 38800, 38997, 38915,
			38861, 38861, 39160, 39251, 39184, 38955, 38773, 39061,
			39383, 39040, 39228, 39235, 39041, 39009, 39237, 39192,
			38763, 39149, 38615, 38867, 38864, 39045, 39177, 39371,
			38964, 38898, 39303, 39231, 39305, 38871, 39485, 39032,
			39153, 39271, 38945, 39021, 39090, 39206, 39019, 39032,
			39083, 39017, 39049, 39308, 39071, 38637, 39210, 38758,
			38960, 39066, 38877, 39372, 39184, 39130, 38863, 39111,
			38710, 38966, 38992, 38794, 38637, 39367, 39116, 39128,
			39219, 38913, 39135, 39280, 38879, 39509, 39134, 39007,
			38968, 38978, 39221, 39131, 39313, 39072, 39091, 39020,
			39073, 39344, 39304, 39118, 39239, 39095, 39403, 39124,
			39126, 39066, 39149, 39367, 39224, 39167, 39090, 39192,
			39356, 38933, 39054, 39252, 39151, 39075, 39071, 39326,
			39040, 39340, 38747, 39359, 39355, 38967, 39039, 39264,
			39006, 39149, 39197, 39055, 39043, 38847, 38879, 39117,
			38991, 39010, 38895, 39045, 38964, 38804, 38883, 39124,
			38760, 38836, 39072, 38840, 38920, 38931, 38834, 39397,
		};
		CollectionAssert.AreEqual(expectedValue, values);
	}

	private static byte[] RandomBytes(Random random, int length)
	{
		byte[] value = new byte[length];
		random.NextBytes(value);
		return value;
	}

	/// <summary>
	/// 对 <see cref="MersenneTwister.NextInt64"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestNextInt64()
	{
		MersenneTwister random = new(1874123);
		Assert.AreEqual(4873176933017264570L, random.NextInt64());
		Assert.AreEqual(0L, random.NextInt64(0));
		Assert.AreEqual(0L, random.NextInt64(1));
		Assert.AreEqual(0L, random.NextInt64(2));
		Assert.AreEqual(0L, random.NextInt64(2));
		Assert.AreEqual(1L, random.NextInt64(2));
		Assert.AreEqual(103L, random.NextInt64(123));
		Assert.AreEqual(-2L, random.NextInt64(-2, -2));
		Assert.AreEqual(-2L, random.NextInt64(-2, 0));
		Assert.AreEqual(7969540530985718900L, random.NextInt64(long.MinValue, long.MaxValue));
		Assert.AreEqual(3692131976325522634L, random.NextInt64(long.MinValue, long.MaxValue));
		Assert.AreEqual(-2674541655452042723L, random.NextInt64(long.MinValue, long.MaxValue));

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextInt64(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => random.NextInt64(2, 1));

		int[] values = new int[5];
		for (int i = 0; i < 1000000; i++)
		{
			values[random.NextInt64(5)]++;
		}
		int[] expected = new int[] { 200332, 199803, 199894, 199652, 200319 };
		CollectionAssert.AreEqual(expected, values);
	}

	/// <summary>
	/// 对 <see cref="MersenneTwister.NextDouble"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestNextDouble()
	{
		MersenneTwister random = new(941);
		Assert.AreEqual(0.06844938323981109, random.NextDouble());
		Assert.AreEqual(0.7167502795520648, random.NextDouble());
		Assert.AreEqual(0.6860091698603292, random.NextDouble());
		Assert.AreEqual(0.5313542412230845, random.NextDouble());
		Assert.AreEqual(0.5123542305873995, random.NextDouble());
		Assert.AreEqual(0.6212029384652293, random.NextDouble());

		int[] values = new int[5];
		for (int i = 0; i < 1000000; i++)
		{
			values[(int)(random.NextDouble() * 5)]++;
		}
		int[] expected = new int[] { 200229, 200079, 200383, 199491, 199818 };
		CollectionAssert.AreEqual(expected, values);
	}

	/// <summary>
	/// 对 <see cref="MersenneTwister.NextSingle"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestNextSingle()
	{
		MersenneTwister random = new(971947);
		Assert.AreEqual(0.9408823F, random.NextSingle());
		Assert.AreEqual(0.5722742F, random.NextSingle());
		Assert.AreEqual(0.17390645F, random.NextSingle());
		Assert.AreEqual(0.9772217F, random.NextSingle());
		Assert.AreEqual(0.3587824F, random.NextSingle());
		Assert.AreEqual(0.8779498F, random.NextSingle());

		int[] values = new int[5];
		for (int i = 0; i < 1000000; i++)
		{
			values[(int)(random.NextSingle() * 5)]++;
		}
		int[] expected = new int[] { 200709, 200084, 199354, 200107, 199746 };
		CollectionAssert.AreEqual(expected, values);
	}
}
