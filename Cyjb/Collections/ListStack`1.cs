using System;
using System.Collections.Generic;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示同一任意类型的实例的大小可变的后进先出 (LIFO) 集合。
	/// 该集合还允许使用索引访问堆栈中的元素。
	/// </summary>
	/// <typeparam name="T">指定堆栈中的元素的类型。</typeparam>
	public sealed class ListStack<T> : Stack<T>
	{
		/// <summary>
		/// 获取 Stack&lt;T&gt; 实例的 _array 字段。
		/// </summary>
		private static readonly Func<Stack<T>, T[]> GetArrayField =
			typeof(Stack<T>).CreateDelegate<Func<Stack<T>, T[]>>("_array");
		/// <summary>
		/// 获取指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		public T this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw ExceptionHelper.ArgumentNegative("index");
				}
				if (index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index");
				}
				return GetArrayField(this)[index];
			}
		}
	}
}
