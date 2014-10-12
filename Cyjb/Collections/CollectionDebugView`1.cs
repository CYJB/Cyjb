using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cyjb.Collections
{
	/// <summary>
	/// 为 <see cref="ICollection{T}"/> 接口提供调试视图。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	internal sealed class CollectionDebugView<T>
	{
		/// <summary>
		/// 调试视图的源集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ICollection<T> source;
		/// <summary>
		/// 使用指定的源集合初始化 <see cref="CollectionDebugView{T}"/> 类的实例。
		/// </summary>
		/// <param name="sourceCollection">使用调试视图的源集合。</param>
		public CollectionDebugView(ICollection<T> sourceCollection)
		{
			Contract.Requires(sourceCollection != null);
			this.source = sourceCollection;
		}
		/// <summary>
		/// 获取源集合中的所有项。
		/// </summary>
		/// <value>包含了源集合中的所有项的数组。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get { return this.source.ToArray(); }
		}
	}
}
