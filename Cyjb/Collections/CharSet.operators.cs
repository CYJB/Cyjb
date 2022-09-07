namespace Cyjb.Collections;

public sealed partial class CharSet
{
	/// <summary>
	/// 返回两个字符集合的和集(union)。
	/// </summary>
	/// <param name="left">第一个字符集合。</param>
	/// <param name="right">第二个字符集合。</param>
	/// <returns>两个字符集合的和集。</returns>
	public static CharSet operator +(CharSet left, IEnumerable<char> right)
	{
		CharSet result = new(left);
		result.UnionWith(right);
		return result;
	}

	/// <summary>
	/// 返回两个字符集合的差集(except)。
	/// </summary>
	/// <param name="left">第一个字符集合。</param>
	/// <param name="right">第二个字符集合。</param>
	/// <returns>两个字符集合的差集。</returns>
	public static CharSet operator -(CharSet left, IEnumerable<char> right)
	{
		CharSet result = new(left);
		result.ExceptWith(right);
		return result;
	}

	/// <summary>
	/// 返回两个字符集合的异或集(symmetric except)。
	/// </summary>
	/// <param name="left">第一个字符集合。</param>
	/// <param name="right">第二个字符集合。</param>
	/// <returns>两个字符集合的异或集。</returns>
	public static CharSet operator ^(CharSet left, IEnumerable<char> right)
	{
		CharSet result = new(left);
		result.SymmetricExceptWith(right);
		return result;
	}

	/// <summary>
	/// 返回两个字符集合的或集(union)。
	/// </summary>
	/// <param name="left">第一个字符集合。</param>
	/// <param name="right">第二个字符集合。</param>
	/// <returns>两个字符集合的或集。</returns>
	public static CharSet operator |(CharSet left, IEnumerable<char> right)
	{
		CharSet result = new(left);
		result.UnionWith(right);
		return result;
	}

	/// <summary>
	/// 返回两个字符集合的与集(intersect)。
	/// </summary>
	/// <param name="left">第一个字符集合。</param>
	/// <param name="right">第二个字符集合。</param>
	/// <returns>两个字符集合的与集。</returns>
	public static CharSet operator &(CharSet left, IEnumerable<char> right)
	{
		CharSet result = new(left);
		result.IntersectWith(right);
		return result;
	}

	/// <summary>
	/// 返回当前字符集合的非集(negated)。
	/// </summary>
	/// <param name="left">要计算的字符集合。</param>
	/// <returns>当前集合的非补集。</returns>
	public static CharSet operator !(CharSet left)
	{
		CharSet result = new(left);
		result.Negated();
		return result;
	}

	/// <summary>
	/// 返回当前字符集合的补集(negated)。
	/// </summary>
	/// <param name="left">要计算的字符集合。</param>
	/// <returns>当前集合的补集。</returns>
	public static CharSet operator ~(CharSet left)
	{
		CharSet result = new(left);
		result.Negated();
		return result;
	}
}
