using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 为迭代器提供基类。
	/// </summary>
	/// <typeparam name="T">迭代的元素类型。</typeparam>
	[Serializable]
	[ContractClass(typeof(ContractsForIteratorBase<>))]
	public abstract class IteratorBase<T> : IEnumerator<T>, IEnumerable<T>
	{
		/// <summary>
		/// 当前迭代器的状态。
		/// </summary>
		private IteratorState state = IteratorState.Default;
		/// <summary>
		/// 当前迭代器对应的线程 Id。
		/// </summary>
		private readonly int threadId;
		/// <summary>
		/// 初始化 <see cref="IteratorBase{T}"/> 类的新实例。
		/// </summary>
		protected IteratorBase()
		{
			this.threadId = Thread.CurrentThread.ManagedThreadId;
		}
		/// <summary>
		/// 创建一个与当前迭代器相同的示例。
		/// </summary>
		/// <returns>与当前迭代器相同的示例。</returns>
		protected abstract IteratorBase<T> Clone();
		/// <summary>
		/// 将枚举数推进到集合的下一个元素。
		/// </summary>
		/// <returns>如果枚举数成功地推进到下一个元素，则为 <c>true</c>；
		/// 如果枚举数越过集合的结尾，则为 <c>false</c>。</returns>
		protected abstract bool MoveNextInternal();

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/>。</returns>
		public IEnumerator<T> GetEnumerator()
		{
			if (this.threadId == Thread.CurrentThread.ManagedThreadId &&
				this.state == IteratorState.Default)
			{
				this.state = IteratorState.Ready;
				return this;
			}
			IteratorBase<T> iterator = Clone();
			Debug.Assert(iterator != null);
			iterator.state = IteratorState.Ready;
			return iterator;
		}

		#endregion // IEnumerable<T> 成员

		#region IEnumerable 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator"/>。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion // IEnumerable 成员

		#region IEnumerator<T> 成员

		/// <summary>
		/// 获取集合中位于枚举数当前位置的元素。
		/// </summary>
		/// <value>集合中位于枚举数当前位置的元素。</value>
		public T Current { get; protected set; }

		#endregion // IEnumerator<T> 成员

		#region IEnumerator 成员

		/// <summary>
		/// 获取集合中的当前元素。
		/// </summary>
		/// <value>集合中的当前元素。</value>
		object IEnumerator.Current
		{
			get { return this.Current; }
		}
		/// <summary>
		/// 将枚举数推进到集合的下一个元素。
		/// </summary>
		/// <returns>如果枚举数成功地推进到下一个元素，则为 <c>true</c>；
		/// 如果枚举数越过集合的结尾，则为 <c>false</c>。</returns>
		public bool MoveNext()
		{
			return this.state == IteratorState.Ready && MoveNextInternal();
		}
		/// <summary>
		/// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
		/// </summary>
		public virtual void Reset()
		{
			throw CommonExceptions.MethodNotSupported();
		}

		#endregion // IEnumerator 成员

		#region IDisposable 成员

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		/// </overloads>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		/// <param name="disposing">是否释放托管资源。</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.state == IteratorState.Disposed || !disposing)
			{
				return;
			}
			this.state = IteratorState.Disposed;
			this.Current = default(T);
		}

		#endregion // IDisposable 成员

		#region 迭代器的状态

		/// <summary>
		/// 表示迭代器的状态。
		/// </summary>
		private enum IteratorState
		{
			/// <summary>
			/// 默认状态。
			/// </summary>
			Default,
			/// <summary>
			/// 可以进行迭代。
			/// </summary>
			Ready,
			/// <summary>
			/// 已经被释放。
			/// </summary>
			Disposed
		}

		#endregion // 迭代器的状态

	}
	/// <summary>
	/// 表示 <see cref="IteratorBase{T}"/> 接口的协定。
	/// </summary>
	[ContractClassFor(typeof(IteratorBase<>))]
	internal abstract class ContractsForIteratorBase<T> : IteratorBase<T>
	{
		/// <summary>
		/// 初始化 <see cref="ContractsForIteratorBase{T}"/> 类的新实例。
		/// </summary>
		private ContractsForIteratorBase() { }
		/// <summary>
		/// 创建一个与当前迭代器相同的示例。
		/// </summary>
		/// <returns>与当前迭代器相同的示例。</returns>
		protected override IteratorBase<T> Clone()
		{
			Contract.Ensures(Contract.Result<IteratorBase<T>>() != null);
			return null;
		}
	}
}
