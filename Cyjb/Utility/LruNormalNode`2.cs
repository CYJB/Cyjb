namespace Cyjb.Utility
{
	/// <summary>
	/// 表示普通的最近最少使用算法的节点。并未包含在项目文件中。
	/// </summary>
	/// <typeparam name="TKey">对象缓存的键的类型。</typeparam>
	/// <typeparam name="TValue">被缓存的对象的类型。</typeparam>
	internal sealed class LruNormalNode<TKey, TValue>
	{
		/// <summary>
		/// 使用指定的键和对象初始化 <see cref="LruNormalNode&lt;TKey,TValue&gt;"/> 类的新实例。
		/// </summary>
		/// <param name="key">对象缓存的键。</param>
		/// <param name="value">被缓存的对象。</param>
		public LruNormalNode(TKey key, TValue value)
		{
			this.Key = key;
			this.Value = value;
		}
		/// <summary>
		/// 获取或设置对象缓存的键。
		/// </summary>
		public TKey Key;
		/// <summary>
		/// 获取或设置被缓存的对象。
		/// </summary>
		public TValue Value;
		/// <summary>
		/// 获取或设置链表中的上一节点。
		/// </summary>
		public LruNormalNode<TKey, TValue> Previous;
		/// <summary>
		/// 获取或设置链表中的下一节点。
		/// </summary>
		public LruNormalNode<TKey, TValue> Next;
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Concat("[", Key, ", ", Value, "]");
		}
	}
}
