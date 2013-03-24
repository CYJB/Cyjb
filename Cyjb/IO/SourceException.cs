using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Cyjb.IO
{
	/// <summary>
	/// 表示源文件中出现异常。
	/// </summary>
	[Serializable]
	public class SourceException : Exception
	{
		/// <summary>
		/// 默认的源文件。
		/// </summary>
		private const string DefaultSource = null;
		/// <summary>
		/// 初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		public SourceException()
			: base()
		{ }
		/// <summary>
		/// 初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		public SourceException(SourceLocation start, SourceLocation end)
			: this(start, end, false)
		{ }
		/// <summary>
		/// 初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="source">源文件的位置。</param>
		public SourceException(SourceLocation start, SourceLocation end, string source)
			: this(start, end, source, false)
		{ }
		/// <summary>
		/// 初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(SourceLocation start, SourceLocation end, bool isWarning)
			: this(start, end, DefaultSource, isWarning)
		{ }
		/// <summary>
		/// 初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="source">源文件的位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(SourceLocation start, SourceLocation end, string source, bool isWarning)
			: base()
		{
			this.Start = start;
			this.End = end;
			this.SourceFile = source;
			this.IsWarning = isWarning;
		}
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(string message, bool isWarning)
			: this(message, DefaultSource, isWarning)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="source">源文件的位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(string message, string source, bool isWarning)
			: this(message, SourceLocation.Invalid, SourceLocation.Invalid, source, isWarning)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		public SourceException(string message)
			: base(message)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="source">源文件的位置。</param>
		public SourceException(string message, string source)
			: base(message)
		{
			this.SourceFile = source;
		}
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end)
			: this(message, start, end, DefaultSource)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="source">源文件的位置。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end, string source)
			: this(message, start, end, source, false)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end, bool isWarning)
			: this(message, start, end, DefaultSource, isWarning)
		{ }
		/// <summary>
		/// 使用指定的错误消息初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="source">源文件的位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end,
			string source, bool isWarning)
			: this(message, start, end, source, isWarning, null)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, Exception innerException)
			: this(message, DefaultSource, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="source">源文件的位置。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, string source, Exception innerException)
			: this(message, SourceLocation.Invalid, SourceLocation.Invalid, source, false, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, bool isWarning, Exception innerException)
			: this(message, DefaultSource, isWarning, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="source">源文件的位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, string source, bool isWarning, Exception innerException)
			: this(message, SourceLocation.Invalid, SourceLocation.Invalid, source, isWarning, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end, Exception innerException)
			: this(message, start, end, DefaultSource, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="source">源文件的位置。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end,
			string source, Exception innerException)
			: this(message, start, end, source, false, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end,
			bool isWarning, Exception innerException)
			: this(message, start, end, DefaultSource, isWarning, innerException)
		{ }
		/// <summary>
		/// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="message">解释异常原因的错误消息。</param>
		/// <param name="start">异常的起始位置。</param>
		/// <param name="end">异常的结束位置。</param>
		/// <param name="source">源文件的位置。</param>
		/// <param name="isWarning">异常表示的是否是警告。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 <c>null</c> 引用。</param>
		public SourceException(string message, SourceLocation start, SourceLocation end,
			string source, bool isWarning, Exception innerException)
			: base(message, innerException)
		{
			this.Start = start;
			this.End = end;
			this.SourceFile = source;
			this.IsWarning = isWarning;
		}

		/// <summary>
		/// 用指定的序列化信息和上下文初始化 <see cref="SourceException"/> 类的新实例。
		/// </summary>
		/// <param name="info"><see cref="System.Runtime.Serialization.SerializationInfo"/> 对象，
		/// 包含序列化 <see cref="SourceException"/> 所需的信息。</param>
		/// <param name="context"><see cref="System.Runtime.Serialization.StreamingContext"/> 对象，
		/// 该对象包含与 <see cref="SourceException"/> 相关联的序列化流的源和目标。</param>
		protected SourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Start = (SourceLocation)info.GetValue("Start", typeof(SourceLocation));
			this.End = (SourceLocation)info.GetValue("End", typeof(SourceLocation));
			this.SourceFile = info.GetString("Source");
			this.IsWarning = info.GetBoolean("Warn");
		}
		/// <summary>
		/// 获取异常的起始位置。
		/// </summary>
		public SourceLocation Start { get; private set; }
		/// <summary>
		/// 获取异常的结束位置。
		/// </summary>
		public SourceLocation End { get; private set; }
		/// <summary>
		/// 获取或设置源文件的位置。
		/// </summary>
		/// <value>源文件的位置，如果不存在则为 <c>null</c>。</value>
		public string SourceFile { get; set; }
		/// <summary>
		/// 获取异常表示的是否是警告。
		/// </summary>
		/// <value>如果是警告，则为 <c>true</c>；如果是错误则为 <c>false</c>。</value>
		public bool IsWarning { get; set; }
		/// <summary>
		/// 获取描述当前异常的消息。
		/// </summary>
		/// <value>解释异常原因的错误消息或空字符串 ("")。</value>
		public override string Message
		{
			get
			{
				StringBuilder text = new StringBuilder();
				text.Append(Resources.GetString(IsWarning ? "WarningText" : "ErrorText"));
				text.Append(base.Message);
				if (Start != SourceLocation.Invalid)
				{
					if (End == SourceLocation.Invalid)
					{
						text.AppendLine();
						text.Append(Resources.GetString("AtText", Start));
					}
					else
					{
						text.AppendLine();
						text.Append(Resources.GetString("FromToText", Start, End));
					}
				}
				return text.ToString();
			}
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
			info.AddValue("Start", Start);
			info.AddValue("End", End);
			info.AddValue("Source", SourceFile);
			info.AddValue("Warn", IsWarning);
		}

		#endregion // ISerializable 成员

	}
}
