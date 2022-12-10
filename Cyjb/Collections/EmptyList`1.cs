using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections;

/// <summary>
/// 空的只读列表。
/// </summary>
/// <typeparam name="T">列表中元素的类型。</typeparam>
internal class EmptyList<T> : ReadOnlyListBase<T>
{
	/// <summary>
	/// 空的只读列表实例。
	/// </summary>
	public static readonly EmptyList<T> Instance = new();

	/// <summary>
	/// 获取当前集合包含的元素数。
	/// </summary>
	/// <value>当前集合中包含的元素数。</value>
	public override int Count => 0;

	/// <summary>
	/// 返回指定索引处的元素。
	/// </summary>
	/// <param name="index">要返回元素的从零开始的索引。</param>
	/// <returns>位于指定索引处的元素。</returns>
	protected override T GetItemAt(int index)
	{
		return default!;
	}

	/// <summary>
	/// 确定当前列表中指定对象的索引。
	/// </summary>
	/// <param name="item">要在当前列表中定位的对象。</param>
	/// <returns>如果在当前列表中找到 <paramref name="item"/>，则为该对象的索引；否则为 <c>-1</c>。</returns>
	public override int IndexOf(T item)
	{
		return -1;
	}

	/// <summary>
	/// 返回一个循环访问集合的枚举器。
	/// </summary>
	/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
	public override IEnumerator<T> GetEnumerator()
	{
		yield break;
	}
}
