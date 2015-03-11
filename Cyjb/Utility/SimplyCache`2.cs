using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb.Utility
{
	/// <summary>
	/// 表示缓存个数不受限制的对象缓冲池。
	/// </summary>
	/// <typeparam name="TKey">缓存对象的键的类型。</typeparam>
	/// <typeparam name="TValue">缓存对象的类型。</typeparam>
	/// <remarks>该类仅仅是 <see cref="ConcurrentDictionary{TKey,TValue}"/> 类的一个简单包装。
	/// 缓存的对象数量会一直增长下去，通常应仅作为测试使用或用于缓存生成的代价非常大的对象。</remarks>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class SimplyCache<TKey, TValue> : ICache<TKey, TValue>
	{
		/// <summary>
		/// 缓存对象的字典。
		/// </summary>
		private readonly ConcurrentDictionary<TKey, TValue> cacheDict = new ConcurrentDictionary<TKey, TValue>();
		/// <summary>
		/// 初始化 <see cref="SimplyCache{TKey,TValue}"/> 类的新实例。
		/// </summary>
		public SimplyCache() { }
		/// <summary>
		/// 获取实际包含在缓存中的对象数目。
		/// </summary>
		/// <value>实际包含在缓存中的对象数目。</value>
		public int Count { get { return cacheDict.Count; } }

		#region ICache<TKey, TValue> 成员

		/// <summary>
		/// 将指定的键和对象添加到缓存中，无论键是否存在。
		/// </summary>
		/// <param name="key">要添加的对象的键。</param>
		/// <param name="value">要添加的对象。</param>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public void Add(TKey key, TValue value)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			Contract.EndContractBlock();
			this.cacheDict[key] = value;
		}
		/// <summary>
		/// 清空缓存中的所有对象。
		/// </summary>
		public void Clear()
		{
			this.cacheDict.Clear();
		}
		/// <summary>
		/// 确定缓存中是否包含指定的键。
		/// </summary>
		/// <param name="key">要在缓存中查找的键。</param>
		/// <returns>如果缓存中包含具有指定键的元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public bool Contains(TKey key)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			Contract.EndContractBlock();
			return cacheDict.ContainsKey(key);
		}
		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将新对象添加到缓存中。
		/// </summary>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="valueFactory">用于根据键生成新对象的函数。</param>
		/// <returns>如果在缓存中找到该键，则为对应的对象；否则为 <paramref name="valueFactory"/> 
		/// 返回的新对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将新对象添加到缓存中。
		/// </summary>
		/// </overloads>
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			CommonExceptions.CheckArgumentNull(valueFactory, "valueFactory");
			Contract.EndContractBlock();
			return cacheDict.GetOrAdd(key, valueFactory);
		}
		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将新对象添加到缓存中。
		/// </summary>
		/// <typeparam name="TArg">参数的类型。</typeparam>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="arg">用于生成新对象的参数。</param>
		/// <param name="valueFactory">用于根据键和参数生成新对象的函数。</param>
		/// <returns>如果在缓存中找到该键，则为对应的对象；否则为 <paramref name="valueFactory"/> 
		/// 返回的新对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> 为 <c>null</c>。</exception>
		public TValue GetOrAdd<TArg>(TKey key, TArg arg, Func<TKey, TArg, TValue> valueFactory)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			CommonExceptions.CheckArgumentNull(valueFactory, "valueFactory");
			Contract.EndContractBlock();
			return cacheDict.GetOrAdd(key, k => valueFactory(k, arg));
		}
		/// <summary>
		/// 从缓存中获取与指定的键关联的对象，如果不存在则将新对象添加到缓存中。
		/// </summary>
		/// <typeparam name="TArg0">第一个参数的类型。</typeparam>
		/// <typeparam name="TArg1">第二个参数的类型。</typeparam>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="arg0">用于生成新对象的第一个参数。</param>
		/// <param name="arg1">用于生成新对象的第二个参数。</param>
		/// <param name="valueFactory">用于根据键和参数生成新对象的函数。</param>
		/// <returns>如果在缓存中找到该键，则为对应的对象；
		/// 否则为 <paramref name="valueFactory"/> 返回的新对象。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> 为 <c>null</c>。</exception>
		public TValue GetOrAdd<TArg0, TArg1>(TKey key, TArg0 arg0, TArg1 arg1, Func<TKey, TArg0, TArg1, TValue> valueFactory)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			CommonExceptions.CheckArgumentNull(valueFactory, "valueFactory");
			Contract.EndContractBlock();
			return cacheDict.GetOrAdd(key, k => valueFactory(k, arg0, arg1));
		}
		/// <summary>
		/// 从缓存中移除具有指定键的对象。
		/// </summary>
		/// <param name="key">要移除的对象的键。</param>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public void Remove(TKey key)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			Contract.EndContractBlock();
			TValue value;
			cacheDict.TryRemove(key, out value);
		}
		/// <summary>
		/// 尝试从缓存中获取与指定的键关联的对象。
		/// </summary>
		/// <param name="key">要获取的对象的键。</param>
		/// <param name="value">此方法返回时，<paramref name="value"/> 包含缓存中具有指定键的对象；
		/// 如果操作失败，则包含默认值。</param>
		/// <returns>如果在缓存中找到该键，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		public bool TryGet(TKey key, out TValue value)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			Contract.EndContractBlock();
			return cacheDict.TryGetValue(key, out value);
		}

		#endregion // ICache<TKey, TValue> 成员

	}
}
