using System.Diagnostics;

namespace Cyjb.Collections;

/// <summary>
/// 为 <see cref="IEnumerable{T}"/> 接口提供调试视图。
/// </summary>
/// <typeparam name="T">集合中元素的类型。</typeparam>
internal sealed class EnumerableDebugView<T>
{
	/// <summary>
	/// 调试视图的源集合。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly IEnumerable<T> source;

	/// <summary>
	/// 使用指定的源集合初始化 <see cref="EnumerableDebugView{T}"/> 类的实例。
	/// </summary>
	/// <param name="sourceCollection">使用调试视图的源集合。</param>
	public EnumerableDebugView(IEnumerable<T> sourceCollection)
	{
		source = sourceCollection;
	}

	/// <summary>
	/// 获取源集合中的所有项。
	/// </summary>
	/// <value>包含了源集合中的所有项的数组。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public T[] Items => source.ToArray();
}
