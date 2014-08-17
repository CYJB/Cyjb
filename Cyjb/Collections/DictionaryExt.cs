using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Cyjb.Collections
{
	/// <summary>
	/// 提供对 <see cref="IDictionary{TKey,TValue}"/> 的扩展方法。
	/// </summary>
	public static class DictionaryExt
	{
		/// <summary>
		/// 获取与指定的键相关联的值。
		/// </summary>
		/// <typeparam name="TKey">字典中键的类型。</typeparam>
		/// <typeparam name="TValue">字典中值的类型。</typeparam>
		/// <param name="dict">要获取值的字典。</param>
		/// <param name="key">要获取其值的键。</param>
		/// <returns>当此方法返回时，如果找到指定键，则返回与该键相关联的值；
		/// 否则，将返回 <typeparamref name="TValue"/> 类型的默认值。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="dict"/> 为 <c>null</c>。</exception>
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			if (dict == null)
			{
				throw ExceptionHelper.ArgumentNull("dict");
			}
			Contract.EndContractBlock();
			TValue value;
			return dict.TryGetValue(key, out value) ? value : default(TValue);
		}
	}
}