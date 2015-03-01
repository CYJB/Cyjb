using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件中出现异常。
	/// </summary>
	[Serializable]
	public class SourceException : Exception, ISourceFileLocatable
	{
		/// <summary>
		/// 产生异常的源文件起始位置。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourcePosition start;
		/// <summary>
		/// 产生异常的源文件结束位置。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SourcePosition end;
		/// <summary>
		/// 产生异常的源文件名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string fileName;
		/// <summary>
		/// 异常表示的是否是警告。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool isWarning;

		#region 构造函数

		/// <summary>
		/// 初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public SourceException() { }
		/// <summary>
		/// 使用指定的源文件位置初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="range">产生异常的源文件位置。</param>
		public SourceException(ISourceLocatable range)
			: this(null, range, false, null)
		{ }
		/// <summary>
		/// 使用指定的源文件位置和是否是警告初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="range">产生异常的源文件位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(ISourceLocatable range, bool isWarning)
			: this(null, range, isWarning, null)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		public SourceException(string message)
			: base(message)
		{ }
		/// <summary>
		/// 使用指定的错误消息和是否是警告初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(string message, bool isWarning)
			: base(message)
		{
			this.isWarning = isWarning;
		}
		/// <summary>
		/// 使用指定的错误消息和源文件名称初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="fileName">源文件的名称。</param>
		public SourceException(string message, string fileName)
			: base(message)
		{
			this.fileName = fileName;
		}
		/// <summary>
		/// 使用指定的错误消息、源文件名称和是否是警告初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="fileName">源文件的名称。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(string message, string fileName, bool isWarning)
			: base(message)
		{
			this.fileName = fileName;
			this.isWarning = isWarning;
		}
		/// <summary>
		/// 使用指定的错误消息和源文件位置初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="range">产生异常的源文件位置。</param>
		public SourceException(string message, ISourceLocatable range)
			: this(message, range, false, null)
		{ }
		/// <summary>
		/// 使用指定的错误消息、源文件位置和是否是警告初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="range">产生异常的源文件位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(string message, ISourceLocatable range, bool isWarning)
			: this(message, range, isWarning, null)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, Exception innerException)
			: base(message, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息、是否是警告和对导致此异常的内部异常的引用初始化 
		/// <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, bool isWarning, Exception innerException)
			: base(message, innerException)
		{
			this.isWarning = isWarning;
		}
		/// <summary>
		/// 使用指定的错误消息、源文件名称和对导致此异常的内部异常的引用初始化 
		/// <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="fileName">源文件的名称。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, string fileName, Exception innerException)
			: base(message, innerException)
		{
			this.fileName = fileName;
		}
		/// <summary>
		/// 使用指定的错误消息、源文件名称、是否是警告和对导致此异常的内部异常的引用初始化 
		/// <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="fileName">源文件的名称。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, string fileName, bool isWarning, Exception innerException)
			: base(message, innerException)
		{
			this.fileName = fileName;
			this.isWarning = isWarning;
		}
		/// <summary>
		/// 使用指定的错误消息、源文件位置和对导致此异常的内部异常的引用初始化 
		/// <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="range">产生异常的源文件位置。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, ISourceLocatable range, Exception innerException)
			: this(message, range, false, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息、源文件位置、是否是警告和对导致此异常的内部异常的引用初始化 
		/// <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="range">产生异常的源文件位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, ISourceLocatable range, bool isWarning, Exception innerException)
			: base(message, innerException)
		{
			if (range != null)
			{
				if (range.Start.IsUnknown != range.End.IsUnknown)
				{
					throw CommonExceptions.InvalidSourceRange(range.Start, range.End);
				}
				if (!range.Start.IsUnknown && range.Start > range.End)
				{
					throw CommonExceptions.ReversedArgument("range.Start", "range.End");
				}
				this.start = range.Start;
				this.end = range.End;
				ISourceFileLocatable fileRange = range as ISourceFileLocatable;
				if (fileRange != null)
				{
					this.fileName = fileRange.FileName;
				}
			}
			this.isWarning = isWarning;
		}

		/// <summary>
		/// 用指定的序列化信息和上下文初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> 对象，
		/// 包含序列化 <see cref="SourceException"/> 所需的信息。</param>
		/// <param name="context"><see cref="StreamingContext"/> 对象，
		/// 该对象包含与 <see cref="SourceException"/> 相关联的序列化流的源和目标。</param>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> 为 <c>null</c>。</exception>
		protected SourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			CommonExceptions.CheckArgumentNull(info, "info");
			Contract.EndContractBlock();
			this.start = (SourcePosition)info.GetValue("Start", typeof(SourcePosition));
			this.end = (SourcePosition)info.GetValue("End", typeof(SourcePosition));
			this.fileName = info.GetString("File");
			this.isWarning = info.GetBoolean("Warn");
		}

		#endregion // 构造函数

		/// <summary>
		/// 获取产生异常的源文件起始位置。
		/// </summary>
		/// <value>产生异常的源文件起始位置。</value>
		public SourcePosition Start { get { return this.start; } }
		/// <summary>
		/// 获取产生异常的源文件结束位置。
		/// </summary>
		/// <value>产生异常的源文件结束位置。</value>
		public SourcePosition End { get { return this.end; } }
		/// <summary>
		/// 获取产生异常的源文件名称。
		/// </summary>
		/// <value>产生异常的源文件名称，如果不存在则为 <c>null</c>。</value>
		public string FileName { get { return this.fileName; } }
		/// <summary>
		/// 获取异常表示的是否是警告。
		/// </summary>
		/// <value>如果是警告，则为 <c>true</c>；如果是错误则为 <c>false</c>。</value>
		public bool IsWarning { get { return this.isWarning; } }
		/// <summary>
		/// 获取描述当前异常的消息。的位置
		/// </summary>
		/// <value>解释异常原因的错误消息或空字符串 ("")。</value>
		public override string Message
		{
			get
			{
				StringBuilder text = new StringBuilder();
				text.Append(IsWarning ? Resources.WarningText : Resources.ErrorText);
				text.Append(base.Message);
				bool hasFileName = !string.IsNullOrWhiteSpace(this.fileName);
				if (hasFileName || !this.start.IsUnknown)
				{
					text.AppendLine();
					text.Append(Resources.AtText);
					if (hasFileName)
					{
						text.Append(' ');
						text.Append(this.fileName);
					}
					if (!this.start.IsUnknown)
					{
						text.Append(hasFileName ? ':' : ' ');
						if (this.start == this.end)
						{
							text.AppendFormat("({0})", this.start);
						}
						else
						{
							text.AppendFormat("({0})-({1})", this.start, this.end);
						}
					}
				}
				return text.ToString();
			}
		}

		#region ISerializable 成员

		/// <summary>
		/// 使用将目标对象序列化所需的数据填充 <see cref="SerializationInfo"/>。
		/// </summary>
		/// <param name="info">要填充数据的 <see cref="SerializationInfo"/>。</param>
		/// <param name="context">此序列化的目标。</param>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> 为 <c>null</c>。</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			CommonExceptions.CheckArgumentNull(info, "info");
			Contract.EndContractBlock();
			base.GetObjectData(info, context);
			info.AddValue("Start", this.start);
			info.AddValue("End", this.end);
			info.AddValue("File", this.fileName);
			info.AddValue("Warn", this.isWarning);
		}

		#endregion // ISerializable 成员

	}
}
