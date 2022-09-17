using System.Diagnostics;

namespace Cyjb.Collections;

/// <summary>
/// 为 <see cref="IDictionary{TKey, TValue}"/> 接口提供调试视图。
/// </summary>
/// <typeparam name="TKey">字典中的键的类型。</typeparam>
/// <typeparam name="TValue">字典中的值的类型。</typeparam>
internal class DictionaryDebugView<TKey, TValue>
{
	/// <summary>
	/// 调试视图的源字典。
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly IDictionary<TKey, TValue> source;

	/// <summary>
	/// 使用指定的源字典初始化 <see cref="DictionaryDebugView{TKey, TValue}"/> 类的实例。
	/// </summary>
	/// <param name="source">使用调试视图的源字典。</param>
	public DictionaryDebugView(IDictionary<TKey, TValue> source)
	{
		this.source = source;
	}

	/// <summary>
	/// 获取源集合中的所有项。
	/// </summary>
	/// <value>包含了源集合中的所有项的数组。</value>
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public KeyValuePair<TKey, TValue>[] Items => source.ToArray();
}
