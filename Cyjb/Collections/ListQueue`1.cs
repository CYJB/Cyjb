using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示对象的先进先出集合。
	/// 该集合还允许使用索引访问队列中的元素。
	/// </summary>
	/// <typeparam name="T">指定队列中的元素的类型。</typeparam>
	/// <seealso cref="Queue{T}"/>
	[Serializable]
	public sealed class ListQueue<T> : Queue<T>
	{
		/// <summary>
		/// 获取 <see cref="Queue{T}"/> 实例的 <c>GetElement</c> 方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Func<Queue<T>, int, T> getElement =
			typeof(Queue<T>).CreateDelegate<Func<Queue<T>, int, T>>("GetElement");
		/// <summary>
		/// 初始化 <see cref="ListQueue{T}"/> 类的新实例，该实例为空并且具有默认初始容量。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="ListQueue{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public ListQueue() { }
		/// <summary>
		/// 初始化 <see cref="ListQueue{T}"/> 类的新实例，
		/// 该实例包含从指定集合复制的元素并且具有足够的容量来容纳所复制的元素。
		/// </summary>
		/// <param name="collection">从其中复制元素的集合。</param>
		public ListQueue(IEnumerable<T> collection) : base(collection) { }
		/// <summary>
		/// 初始化 <see cref="ListQueue{T}"/> 类的新实例，
		/// 该实例为空并且具有指定的初始容量或默认初始容量（这两个容量中的较大者）。
		/// </summary>
		/// <param name="capacity"><see cref="ListQueue{T}"/> 可包含的初始元素数。</param>
		public ListQueue(int capacity) : base(capacity) { }
		/// <summary>
		/// 获取指定索引处的元素。
		/// </summary>
		/// <param name="index">要获取的元素从零开始的索引。</param>
		/// <value>指定索引处的元素。</value>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 不是 
		/// <see cref="ListQueue{T}"/> 中的有效索引。</exception>
		public T this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw ExceptionHelper.ArgumentNegative("index", index);
				}
				if (index >= this.Count)
				{
					throw ExceptionHelper.ArgumentOutOfRange("index", index);
				}
				Contract.EndContractBlock();
				return getElement(this, index);
			}
		}
	}
}
