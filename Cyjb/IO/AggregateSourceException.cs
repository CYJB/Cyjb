using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件的异常的集合。
	/// </summary>
	[Serializable]
	public class AggregateSourceException : Exception
	{
		/// <summary>
		/// 内部的异常数组。
		/// </summary>
		private SourceException[] innerExps;
		/// <summary>
		/// 内部的异常只读集合。
		/// </summary>
		private ICollection<SourceException> readOnlyExps;
		/// <summary>
		/// 初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public AggregateSourceException()
			: base()
		{ }
		/// <summary>
		/// 使用对导致此异常的内部异常的引用来初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		public AggregateSourceException(IEnumerable<SourceException> innerExceptions)
			: this(Resources.GetString("DefaultAggregateExceptionMessage"), innerExceptions)
		{ }
		/// <summary>
		/// 使用对导致此异常的内部异常的引用来初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		public AggregateSourceException(SourceException[] innerExceptions)
			: this(Resources.GetString("DefaultAggregateExceptionMessage"), innerExceptions)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		public AggregateSourceException(string message)
			: base(message)
		{
			this.innerExps = new SourceException[0];
			this.readOnlyExps = ReadOnlyCollection<SourceException>.Empty;
		}

		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public AggregateSourceException(string message, Exception innerException)
			: this(message, new SourceException[] { (SourceException)innerException })
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		public AggregateSourceException(string message, IEnumerable<SourceException> innerExceptions)
			: this(message, innerExceptions != null ? innerExceptions.ToArray() : null)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerExceptions">导致当前异常的异常。</param>
		public AggregateSourceException(string message, SourceException[] innerExceptions)
			: base(message, innerExceptions != null && innerExceptions.Length > 0 ? innerExceptions[0] : null)
		{
			ExceptionHelper.CheckArgumentNull(innerExceptions, "innerExceptions");
			this.innerExps = innerExceptions;
			for (int i = 0; i < innerExps.Length; i++)
			{
				if (innerExps[i] == null)
				{
					throw ExceptionHelper.InnerExceptionNull("innerExceptions");
				}
			}
			readOnlyExps = new ReadOnlyCollection<SourceException>(innerExps);
		}

		/// <summary>
		/// 用指定的序列化信息和上下文初始化 <see cref="AggregateSourceException"/> 类的新实例。
		/// </summary>
		/// <param name="info"><see cref="System.Runtime.Serialization.SerializationInfo"/> 对象，包含序列化 <see cref="AggregateSourceException"/> 所需的信息。</param>
		/// <param name="context"><see cref="System.Runtime.Serialization.StreamingContext"/> 对象，该对象包含与 <see cref="AggregateSourceException"/> 相关联的序列化流的源和目标。</param>
		protected AggregateSourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			innerExps = (SourceException[])info.GetValue("InnerExceptions", typeof(SourceException[]));
			if (innerExps == null)
			{
				throw ExceptionHelper.AggregateExceptionDeserializationFailure();
			}
			readOnlyExps = new ReadOnlyCollection<SourceException>(innerExps);
		}
		/// <summary>
		/// 获取导致当前异常的 <see cref="SourceException"/> 实例的只读集合。
		/// </summary>
		/// <value>返回导致当前异常的 <see cref="SourceException"/> 实例的只读集合。</value>
		public ICollection<SourceException> InnerExceptions
		{
			get { return readOnlyExps; }
		}
		/// <summary>
		/// 返回当前对象的字符串形式。
		/// </summary>
		/// <returns>当前对象的字符串形式。</returns>
		public override string ToString()
		{
			StringBuilder text = new StringBuilder();
			text.AppendLine(base.ToString());
			for (int i = 0; i < this.innerExps.Length; i++)
			{
				text.AppendLine(this.innerExps[i].ToString());
			}
			return text.ToString();
		}

		#region ISerializable 成员

		/// <summary>
		/// 使用将目标对象序列化所需的数据填充 <see cref="System.Runtime.Serialization.SerializationInfo"/>。
		/// </summary>
		/// <param name="info">要填充数据的 <see cref="System.Runtime.Serialization.SerializationInfo"/>。</param>
		/// <param name="context">此序列化的目标。</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("InnerExceptions", innerExps);
		}

		#endregion // ISerializable 成员

	}

}
