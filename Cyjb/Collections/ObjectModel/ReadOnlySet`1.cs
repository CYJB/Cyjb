using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace Cyjb.Collections.ObjectModel
{
	/// <summary>
	/// 为泛型只读集合提供基类。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class ReadOnlySet<T> : ISet<T>, ICollection
	{
		/// <summary>
		/// 提供同步的对象。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object syncRoot;
		/// <summary>
		/// 被包装的内部 <see cref="ISet{T}"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ISet<T> items;
		/// <summary>
		/// 初始化 <see cref="ReadOnlySet{T}"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="ReadOnlySet{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		protected ReadOnlySet()
			: this(new HashSet<T>())
		{ }
		/// <summary>
		/// 将 <see cref="ReadOnlySet{T}"/> 类的新实例初始化为指定集合的包装。
		/// </summary>
		/// <param name="set">由新的集合包装的集合。</param>
		/// <remarks>如果 <paramref name="set"/> 实现了 <see cref="ISet{T}"/>
		/// 接口，则使用 <paramref name="set"/> 作为内部集合；否则使用 
		/// <see cref="HashSet{T}"/> 作为内部集合。</remarks>
		public ReadOnlySet(IEnumerable<T> set)
		{
			if (set != null)
			{
				this.items = set as ISet<T> ?? new HashSet<T>(set);
			}
		}
		/// <summary>
		/// 获取被 <see cref="ReadOnlySet{T}"/> 包装的内部 <see cref="ISet{T}"/>。
		/// </summary>
		/// <value>被 <see cref="ReadOnlySet{T}"/> 包装的内部 <see cref="ISet{T}"/>。</value>
		protected ISet<T> Items
		{
			get { return items; }
		}

		#region ISet<T> 成员

		/// <summary>
		/// 向当前集合内添加元素，并返回一个指示是否已成功添加元素的值。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要添加到集合内的元素。</param>
		/// <returns>如果该元素已添加到集合内，则为 <c>true</c>；
		/// 如果该元素已在集合内，则为 <c>false</c>。</returns>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		bool ISet<T>.Add(T item)
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 从当前集合内移除指定集合中的所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要从集合内移除的项的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.ExceptWith(IEnumerable<T> other)
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 修改当前集合，使该集合仅包含指定集合中也存在的元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.IntersectWith(IEnumerable<T> other)
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 确定当前集合是否为指定集合的真子集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的真子集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		[Pure]
		public virtual bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw ExceptionHelper.ArgumentNull("other");
			}
			Contract.EndContractBlock();
			return this.items.IsProperSubsetOf(other);
		}
		/// <summary>
		/// 确定当前集合是否为指定集合的真超集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的真超集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		[Pure]
		public virtual bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw ExceptionHelper.ArgumentNull("other");
			}
			Contract.EndContractBlock();
			return this.items.IsProperSubsetOf(other);
		}
		/// <summary>
		/// 确定当前集合是否为指定集合的子集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的子集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		[Pure]
		public virtual bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw ExceptionHelper.ArgumentNull("other");
			}
			Contract.EndContractBlock();
			return this.items.IsSubsetOf(other);
		}
		/// <summary>
		/// 确定当前集合是否为指定集合的超集合。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合是 <paramref name="other"/> 的超集合，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		[Pure]
		public virtual bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw ExceptionHelper.ArgumentNull("other");
			}
			Contract.EndContractBlock();
			return this.items.IsSupersetOf(other);
		}
		/// <summary>
		/// 确定当前集合是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		[Pure]
		public virtual bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw ExceptionHelper.ArgumentNull("other");
			}
			Contract.EndContractBlock();
			return this.items.Overlaps(other);
		}
		/// <summary>
		/// 确定当前集合与指定的集合中是否包含相同的元素。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <returns>如果当前集合等于 <paramref name="other"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		[Pure]
		public virtual bool SetEquals(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw ExceptionHelper.ArgumentNull("other");
			}
			Contract.EndContractBlock();
			return this.items.SetEquals(other);
		}
		/// <summary>
		/// 修改当前集合，使该集合仅包含当前集合或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 修改当前集合，使该集合包含当前集合和指定集合中同时存在的所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="other">要与当前集合进行比较的集合。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ISet<T>.UnionWith(IEnumerable<T> other)
		{
			throw ExceptionHelper.MethodNotSupported();
		}

		#endregion

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="ReadOnlySet{T}"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="ReadOnlySet{T}"/> 中包含的元素数。</value>
		public virtual int Count
		{
			get { return this.items.Count; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="ReadOnlySet{T}"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ReadOnlySet{T}"/> 为只读，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		bool ICollection<T>.IsReadOnly
		{
			get { return true; }
		}
		/// <summary>
		/// 将指定对象添加到 <see cref="ReadOnlySet{T}"/> 中。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要添加到 <see cref="ReadOnlySet{T}"/> 的对象。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ICollection<T>.Add(T item)
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 从 <see cref="ReadOnlySet{T}"/> 中移除所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ICollection<T>.Clear()
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 确定 <see cref="ReadOnlySet{T}"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="ReadOnlySet{T}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ReadOnlySet{T}"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		[Pure]
		public virtual bool Contains(T item)
		{
			return this.items.Contains(item);
		}
		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将 <see cref="ReadOnlySet{T}"/> 
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ReadOnlySet{T}"/> 
		/// 复制的元素的目标位置的一维 <see cref="Array"/>。
		/// <paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException"><see cref="ReadOnlySet{T}"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			CollectionHelper.CopyTo(this, array, arrayIndex);
		}
		/// <summary>
		/// 从 <see cref="ReadOnlySet{T}"/> 中移除特定对象的第一个匹配项。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要从 <see cref="ReadOnlySet{T}"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="ReadOnlySet{T}"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="ReadOnlySet{T}"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		bool ICollection<T>.Remove(T item)
		{
			throw ExceptionHelper.MethodNotSupported();
		}

		#endregion // ICollection<T> 成员

		#region ICollection 成员

		/// <summary>
		/// 获取一个值，该值指示是否同步对 <see cref="ReadOnlySet{T}"/> 的访问（线程安全）。
		/// </summary>
		/// <value>如果对 <see cref="ReadOnlySet{T}"/> 的访问是同步的（线程安全），
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}
		/// <summary>
		/// 获取一个可用于同步对 <see cref="ReadOnlySet{T}"/> 的访问的对象。
		/// </summary>
		/// <value>可用于同步对 <see cref="ReadOnlySet{T}"/> 的访问的对象。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot
		{
			get
			{
				if (this.syncRoot == null)
				{
					ICollection collection = this.items as ICollection;
					object syncObj = collection == null ? new object() : collection.SyncRoot;
					Interlocked.CompareExchange(ref this.syncRoot, syncObj, null);
				}
				return this.syncRoot;
			}
		}
		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将 <see cref="ReadOnlySet{T}"/> 
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ReadOnlySet{T}"/> 
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><see cref="ReadOnlySet{T}"/> 
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException">源 <see cref="ReadOnlySet{T}"/> 
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			CollectionHelper.CopyTo(this, array, index);
		}

		#endregion // ICollection 成员

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/>。</returns>
		[Pure]
		public virtual IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator();
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

		#region Empty ReadOnlySet

		/// <summary>
		/// 空的只读泛型集合。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ReadOnlySet<T> empty;
		/// <summary>
		/// 获取空的只读泛型集合。
		/// </summary>
		/// <value>空的只读泛型集合。</value>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static ReadOnlySet<T> Empty
		{
			get
			{
				if (empty == null)
				{
					Interlocked.CompareExchange(ref empty, new EmptyReadOnlySet(), null);
				}
				return empty;
			}
		}
		/// <summary>
		/// 空的泛型只读集合。
		/// </summary>
		[Serializable]
		private class EmptyReadOnlySet : ReadOnlySet<T>, IObjectReference
		{
			/// <summary>
			/// 初始化 <see cref="ReadOnlySet{T}.EmptyReadOnlySet"/> 类的新实例。
			/// </summary>
			public EmptyReadOnlySet()
				: base(null)
			{ }

			#region ISet<T> 成员

			/// <summary>
			/// 确定当前集合是否为指定集合的真子集合。
			/// </summary>
			/// <param name="other">要与当前集合进行比较的集合。</param>
			/// <returns>如果当前集合是 <paramref name="other"/> 的真子集合，则为 <c>true</c>；
			/// 否则为 <c>false</c>。</returns>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="other"/> 为 <c>null</c>。</exception>
			[Pure]
			public override bool IsProperSubsetOf(IEnumerable<T> other)
			{
				if (other == null)
				{
					throw ExceptionHelper.ArgumentNull("other");
				}
				Contract.EndContractBlock();
				return other.Any();
			}
			/// <summary>
			/// 确定当前集合是否为指定集合的真超集合。
			/// </summary>
			/// <param name="other">要与当前集合进行比较的集合。</param>
			/// <returns>如果当前集合是 <paramref name="other"/> 的真超集合，则为 <c>true</c>；
			/// 否则为 <c>false</c>。</returns>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="other"/> 为 <c>null</c>。</exception>
			[Pure]
			public override bool IsProperSupersetOf(IEnumerable<T> other)
			{
				if (other == null)
				{
					throw ExceptionHelper.ArgumentNull("other");
				}
				Contract.EndContractBlock();
				return false;
			}
			/// <summary>
			/// 确定当前集合是否为指定集合的子集合。
			/// </summary>
			/// <param name="other">要与当前集合进行比较的集合。</param>
			/// <returns>如果当前集合是 <paramref name="other"/> 的子集合，则为 <c>true</c>；
			/// 否则为 <c>false</c>。</returns>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="other"/> 为 <c>null</c>。</exception>
			[Pure]
			public override bool IsSubsetOf(IEnumerable<T> other)
			{
				if (other == null)
				{
					throw ExceptionHelper.ArgumentNull("other");
				}
				Contract.EndContractBlock();
				return true;
			}
			/// <summary>
			/// 确定当前集合是否为指定集合的超集合。
			/// </summary>
			/// <param name="other">要与当前集合进行比较的集合。</param>
			/// <returns>如果当前集合是 <paramref name="other"/> 的超集合，则为 <c>true</c>；
			/// 否则为 <c>false</c>。</returns>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="other"/> 为 <c>null</c>。</exception>
			[Pure]
			public override bool IsSupersetOf(IEnumerable<T> other)
			{
				if (other == null)
				{
					throw ExceptionHelper.ArgumentNull("other");
				}
				Contract.EndContractBlock();
				return !other.Any();
			}
			/// <summary>
			/// 确定当前集合是否与指定的集合重叠。
			/// </summary>
			/// <param name="other">要与当前集合进行比较的集合。</param>
			/// <returns>如果当前集合与 <paramref name="other"/> 
			/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="other"/> 为 <c>null</c>。</exception>
			[Pure]
			public override bool Overlaps(IEnumerable<T> other)
			{
				if (other == null)
				{
					throw ExceptionHelper.ArgumentNull("other");
				}
				Contract.EndContractBlock();
				return false;
			}
			/// <summary>
			/// 确定当前集合与指定的集合中是否包含相同的元素。
			/// </summary>
			/// <param name="other">要与当前集合进行比较的集合。</param>
			/// <returns>如果当前集合等于 <paramref name="other"/>，则为 <c>true</c>；
			/// 否则为 <c>false</c>。</returns>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="other"/> 为 <c>null</c>。</exception>
			[Pure]
			public override bool SetEquals(IEnumerable<T> other)
			{
				if (other == null)
				{
					throw ExceptionHelper.ArgumentNull("other");
				}
				Contract.EndContractBlock();
				return !other.Any();
			}

			#endregion

			#region ICollection<T> 成员

			/// <summary>
			/// 获取 <see cref="ReadOnlySet{T}.EmptyReadOnlySet"/> 中包含的元素数。
			/// </summary>
			/// <value><see cref="ReadOnlySet{T}.EmptyReadOnlySet"/> 中包含的元素数。</value>
			public override int Count
			{
				get { return 0; }
			}

			#endregion // ICollection<T> 成员

			#region IEnumerable<T> 成员

			/// <summary>
			/// 返回一个循环访问集合的枚举器。
			/// </summary>
			/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
			public override IEnumerator<T> GetEnumerator()
			{
				return Enumerable.Empty<T>().GetEnumerator();
			}

			#endregion // IEnumerable<T> 成员

			#region IObjectReference 成员

			/// <summary>
			/// 返回应进行反序列化的真实对象（而不是序列化流指定的对象）。
			/// </summary>
			/// <param name="context">当前对象从其中进行反序列化的 <see cref="StreamingContext"/>。</param>
			/// <returns>返回放入图形中的实际对象。</returns>
			object IObjectReference.GetRealObject(StreamingContext context)
			{
				return Empty;
			}

			#endregion // IObjectReference 成员

		}

		#endregion // Empty ReadOnlySet

	}
}
