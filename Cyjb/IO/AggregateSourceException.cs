using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件的异常的集合。
	/// </summary>
	[Serializable, DebuggerDisplay("Count = {InnerExceptionCount}")]
	public class AggregateSourceException : Exception
	{
		/// <summary>
		/// 内部的异常数组。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourceException[] innerExps;
		/// <summary>
		/// 内部的异常只读集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyCollection<SourceException> innerExpsCollection;

		#region 构造函数

		/// <summary>
		/// 初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public AggregateSourceException()
			: base(Resources.OneOrMoreErrorsExisted)
		{ }
		/// <summary>
		/// 使用对导致此异常的内部异常的引用来初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		/// <exception cref="ArgumentNullException"><paramref name="innerExceptions"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="innerExceptions"/> 的元素为 <c>null</c>。</exception>
		public AggregateSourceException(IEnumerable<SourceException> innerExceptions)
			: this(Resources.OneOrMoreErrorsExisted, innerExceptions)
		{ }
		/// <summary>
		/// 使用对导致此异常的内部异常的引用来初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		/// <exception cref="ArgumentNullException"><paramref name="innerExceptions"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="innerExceptions"/> 的元素为 <c>null</c>。</exception>
		public AggregateSourceException(params SourceException[] innerExceptions)
			: this(Resources.OneOrMoreErrorsExisted, innerExceptions as IList<SourceException>)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		public AggregateSourceException(string message)
			: base(message)
		{
			this.innerExps = ArrayExt.Empty<SourceException>();
			this.innerExpsCollection = ReadOnlyCollection<SourceException>.Empty;
		}
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerException">导致当前异常的异常。</param>
		public AggregateSourceException(string message, Exception innerException)
			: base(message, innerException)
		{
			if (innerException == null)
			{
				this.innerExps = ArrayExt.Empty<SourceException>();
				this.innerExpsCollection = ReadOnlyCollection<SourceException>.Empty;
			}
			else
			{
				SourceException sourceExp = innerException as SourceException;
				if (sourceExp == null)
				{
					throw CommonExceptions.InvalidCast(innerException.GetType(), typeof(SourceException));
				}
				this.innerExps = new[] { sourceExp };
				this.innerExpsCollection = new ReadOnlyCollection<SourceException>(innerExps);
			}
		}
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		/// <exception cref="ArgumentNullException"><paramref name="innerExceptions"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="innerExceptions"/> 的元素为 <c>null</c>。</exception>
		public AggregateSourceException(string message, IEnumerable<SourceException> innerExceptions)
			: this(message, innerExceptions as IList<SourceException> ??
				(innerExceptions == null ? null : new List<SourceException>(innerExceptions)))
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		/// <exception cref="ArgumentNullException"><paramref name="innerExceptions"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="innerExceptions"/> 的元素为 <c>null</c>。</exception>
		public AggregateSourceException(string message, params SourceException[] innerExceptions)
			: this(message, innerExceptions as IList<SourceException>)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		private AggregateSourceException(string message, IList<SourceException> innerExceptions)
			: base(message, innerExceptions != null && innerExceptions.Count > 0 ? innerExceptions[0] : null)
		{
			if (innerExceptions == null)
			{
				throw CommonExceptions.ArgumentNull("innerExceptions");
			}
			if (innerExceptions.Any(ex => ex == null))
			{
				throw CommonExceptions.CollectionItemNull("innerExceptions");
			}
			int cnt = innerExceptions.Count;
			this.innerExps = new SourceException[cnt];
			for (int i = 0; i < cnt; i++)
			{
				this.innerExps[i] = innerExceptions[i];
			}
			innerExpsCollection = new ReadOnlyCollection<SourceException>(this.innerExps);
		}
		/// <summary>
		/// 用指定的序列化信息和上下文初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> 对象，包含序列化 
		/// <see cref="AggregateSourceException"/> 所需的信息。</param>
		/// <param name="context"><see cref="StreamingContext"/> 对象，
		/// 该对象包含与 <see cref="AggregateSourceException"/> 相关联的序列化流的源和目标。</param>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> 参数为 <c>null</c>。</exception>
		protected AggregateSourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw CommonExceptions.ArgumentNull("info");
			}
			Contract.EndContractBlock();
			this.innerExps = (SourceException[])info.GetValue("InnerExceptions", typeof(SourceException[]));
			innerExpsCollection = new ReadOnlyCollection<SourceException>(innerExps);
		}

		#endregion // 构造函数

		/// <summary>
		/// 获取导致当前异常的 <see cref="SourceException"/> 实例的只读集合。
		/// </summary>
		/// <value>返回导致当前异常的 <see cref="SourceException"/> 实例的只读集合。</value>
		public ReadOnlyCollection<SourceException> InnerExceptions
		{
			get { return innerExpsCollection; }
		}
		/// <summary>
		/// 获取内部异常的数量。
		/// </summary>
		/// <value>内部异常的数量。</value>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int InnerExceptionCount
		{
			get { return this.innerExps.Length; }
		}
		/// <summary>
		/// 返回是当前异常的根本原因的 <see cref="AggregateSourceException"/>。
		/// </summary>
		/// <returns>是当前异常的根本原因的 <see cref="AggregateSourceException"/>。</returns>
		public override Exception GetBaseException()
		{
			Exception back = this;
			AggregateSourceException backAsAggregate = this;
			while (backAsAggregate != null && backAsAggregate.InnerExceptions.Count == 1)
			{
				Contract.Assume(back != null);
				back = back.InnerException;
				backAsAggregate = back as AggregateSourceException;
			}
			return back;
		}
		/// <summary>
		/// 对此 <see cref="AggregateSourceException"/> 所包含的每个 <see cref="Exception"/> 调用处理程序。
		/// </summary>
		/// <param name="predicate">要对每个异常执行的谓词。该谓词接受要处理的 <see cref="Exception"/> 
		/// 作为参数，并返回指示异常是否已处理的布尔值。</param>
		/// <remarks>
		/// <paramref name="predicate"/> 的每个调用返回指示 <see cref="Exception"/> 是否已处理的 
		/// <c>true</c> 或 <c>false</c>。
		/// 在对所有异常调用 <paramref name="predicate"/> 后，如果存在任何未处理的异常，它们将被放入新的 
		/// <see cref="AggregateSourceException"/> 中并引发异常。否则，只是返回 <see cref="Handle"/> 方法。
		/// 如果 <paramref name="predicate"/> 的任何调用引发了异常，将暂停处理剩余的 <see cref="SourceException"/> 
		/// 并立即传播新引发的异常。
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 <c>null</c>。</exception>
		/// <exception cref="AggregateSourceException">此 <see cref="AggregateSourceException"/> 
		/// 中包含任何未被处理的异常。</exception>
		public void Handle(Func<SourceException, bool> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			Contract.EndContractBlock();
			List<Exception> unhandledExceptions = null;
			for (int i = 0; i < this.innerExps.Length; i++)
			{
				SourceException exp = this.innerExps[i];
				if (predicate(exp))
				{
					continue;
				}
				if (unhandledExceptions == null)
				{
					unhandledExceptions = new List<Exception>();
				}
				unhandledExceptions.Add(exp);
			}
			if (unhandledExceptions != null)
			{
				throw new AggregateException(this.Message, unhandledExceptions);
			}
		}
		/// <summary>
		/// 返回当前对象的字符串形式。
		/// </summary>
		/// <returns>当前对象的字符串形式。</returns>
		public override string ToString()
		{
			StringBuilder text = new StringBuilder();
			text.Append(base.ToString());
			for (int i = 0; i < this.innerExps.Length; i++)
			{
				text.AppendLine();
				text.AppendFormat(Resources.AggregateSourceException_InnerException, i, this.innerExps[i]);
			}
			return text.ToString();
		}

		#region ISerializable 成员

		/// <summary>
		/// 使用将目标对象序列化所需的数据填充 <see cref="SerializationInfo"/>。
		/// </summary>
		/// <param name="info">要填充数据的 <see cref="SerializationInfo"/>。
		/// </param>
		/// <param name="context">此序列化的目标。</param>
		/// <exception cref="SecurityException">调用方没有所要求的权限。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> 参数为 <c>null</c>。</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw CommonExceptions.ArgumentNull("info");
			}
			Contract.EndContractBlock();
			base.GetObjectData(info, context);
			info.AddValue("InnerExceptions", innerExps.Clone());
		}

		#endregion // ISerializable 成员

	}
}
