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
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class ReadOnlyCollection<T> : ICollection<T>, ICollection
	{
		/// <summary>
		/// 用于同步集合访问的对象。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object syncRoot;
		/// <summary>
		/// 被包装的内部 <see cref="ICollection{T}"/>。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ICollection<T> items;
		/// <summary>
		/// 初始化 <see cref="ReadOnlyCollection{T}"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="ReadOnlyCollection{T}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		protected ReadOnlyCollection()
			: this(new List<T>())
		{ }
		/// <summary>
		/// 将 <see cref="ReadOnlyCollection{T}"/> 类的新实例初始化为指定数据的包装。
		/// </summary>
		/// <param name="collection">由新的集合包装的数据。</param>
		/// <remarks>如果 <paramref name="collection"/> 实现了 <see cref="ICollection{T}"/>
		/// 接口，则使用 <paramref name="collection"/> 作为内部集合；否则使用 
		/// <see cref="List{T}"/> 作为内部集合。</remarks>
		public ReadOnlyCollection(IEnumerable<T> collection)
		{
			if (collection != null)
			{
				this.items = collection as ICollection<T> ?? new List<T>(collection);
			}
		}
		/// <summary>
		/// 获取被 <see cref="ReadOnlyCollection{T}"/> 包装的内部 <see cref="ICollection{T}"/>。
		/// </summary>
		/// <value>被 <see cref="ReadOnlyCollection{T}"/> 包装的内部 <see cref="ICollection{T}"/>。</value>
		protected ICollection<T> Items
		{
			get
			{
				Contract.Ensures(Contract.Result<ICollection<T>>() != null);
				return items;
			}
		}

		#region ICollection<T> 成员

		/// <summary>
		/// 获取 <see cref="ReadOnlyCollection{T}"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="ReadOnlyCollection{T}"/> 中包含的元素数。</value>
		public virtual int Count
		{
			get { return this.items.Count; }
		}
		/// <summary>
		/// 获取一个值，该值指示 <see cref="ReadOnlyCollection{T}"/> 是否为只读。
		/// </summary>
		/// <value>如果 <see cref="ReadOnlyCollection{T}"/> 是只读的，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<T>.IsReadOnly
		{
			get { return true; }
		}
		/// <summary>
		/// 将指定对象添加到 <see cref="ReadOnlyCollection{T}"/> 中。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要添加到 <see cref="ReadOnlyCollection{T}"/> 的对象。</param>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ICollection<T>.Add(T item)
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 从 <see cref="ReadOnlyCollection{T}"/> 中移除所有元素。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		void ICollection<T>.Clear()
		{
			throw ExceptionHelper.MethodNotSupported();
		}
		/// <summary>
		/// 确定 <see cref="ReadOnlyCollection{T}"/> 是否包含指定对象。
		/// </summary>
		/// <param name="item">要在 <see cref="ReadOnlyCollection{T}"/> 中定位的对象。</param>
		/// <returns>如果在 <see cref="ReadOnlyCollection{T}"/> 中找到 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public virtual bool Contains(T item)
		{
			return this.items.Contains(item);
		}
		/// <summary>
		/// 从特定的 <see cref="Array"/> 索引处开始，将 <see cref="ReadOnlyCollection{T}"/> 
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ReadOnlyCollection{T}"/> 复制的元素的目标位置的一维 
		/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><see cref="ReadOnlyCollection{T}"/> 
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException">源 <see cref="ReadOnlyCollection{T}"/> 
		/// 的类型无法自动转换为目标 <paramref name="array"/> 的类型。</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			CollectionHelper.CopyTo(this, array, arrayIndex);
		}
		/// <summary>
		/// 从 <see cref="ReadOnlyCollection{T}"/> 中移除特定对象的第一个匹配项。
		/// 此实现总是引发 <see cref="NotSupportedException"/>。
		/// </summary>
		/// <param name="item">要从 <see cref="ReadOnlyCollection{T}"/> 中移除的对象。</param>
		/// <returns>如果已从 <see cref="ReadOnlyCollection{T}"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="ReadOnlyCollection{T}"/> 
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		/// <exception cref="NotSupportedException">总是引发。</exception>
		bool ICollection<T>.Remove(T item)
		{
			throw ExceptionHelper.MethodNotSupported();
		}

		#endregion // ICollection<T> 成员

		#region ICollection 成员

		/// <summary>
		/// 获取一个值，该值指示是否同步对 <see cref="ReadOnlyCollection{T}"/> 的访问（线程安全）。
		/// </summary>
		/// <value>如果对 <see cref="ReadOnlyCollection{T}"/> 的访问是同步的（线程安全），
		/// 则为 <c>true</c>；否则为 <c>false</c>。</value>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}
		/// <summary>
		/// 获取一个可用于同步对 <see cref="ReadOnlyCollection{T}"/> 的访问的对象。
		/// </summary>
		/// <value>可用于同步对 <see cref="ReadOnlyCollection{T}"/> 的访问的对象。</value>
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
		/// 从特定的 <see cref="Array"/> 索引处开始，将 <see cref="ReadOnlyCollection{T}"/> 
		/// 的元素复制到一个 <see cref="Array"/> 中。
		/// </summary>
		/// <param name="array">从 <see cref="ReadOnlyCollection{T}"/> 复制的元素的目标位置的一维 
		/// <see cref="Array"/>。<paramref name="array"/> 必须具有从零开始的索引。</param>
		/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于零。</exception>
		/// <exception cref="ArgumentException"><paramref name="array"/> 是多维的。</exception>
		/// <exception cref="ArgumentException"><see cref="ReadOnlyCollection{T}"/> 
		/// 中的元素数目大于从 <paramref name="index"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		/// <exception cref="ArgumentException">源 <see cref="ReadOnlyCollection{T}"/> 
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
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public virtual IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		#endregion // IEnumerable<T> 成员

		#region IEnumerable 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion // IEnumerable 成员

		#region Empty ReadOnlyCollection

		/// <summary>
		/// 空的泛型只读集合。
		/// </summary>
		[NonSerialized, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ReadOnlyCollection<T> empty;
		/// <summary>
		/// 获取空的泛型只读集合。
		/// </summary>
		/// <value>空的泛型只读集合。</value>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static ReadOnlyCollection<T> Empty
		{
			get
			{
				Contract.Ensures(Contract.Result<ReadOnlyCollection<T>>() != null);
				if (empty == null)
				{
					Interlocked.CompareExchange(ref empty, new EmptyReadOnlyCollection(), null);
				}
				return empty;
			}
		}
		/// <summary>
		/// 空的泛型只读集合。
		/// </summary>
		[Serializable]
		private class EmptyReadOnlyCollection : ReadOnlyCollection<T>, IObjectReference
		{
			/// <summary>
			/// 初始化 <see cref="ReadOnlyCollection{T}.EmptyReadOnlyCollection"/> 类的新实例。
			/// </summary>
			public EmptyReadOnlyCollection()
				: base(null)
			{ }

			#region ICollection<T> 成员

			/// <summary>
			/// 获取 <see cref="ReadOnlyCollection{T}.EmptyReadOnlyCollection"/> 中包含的元素数。
			/// </summary>
			/// <value><see cref="ReadOnlyCollection{T}.EmptyReadOnlyCollection"/> 中包含的元素数。</value>
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

		#endregion // Empty ReadOnlyCollection

	}
}
