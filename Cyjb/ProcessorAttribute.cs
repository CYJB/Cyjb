using System;
using System.Diagnostics.Contracts;

namespace Cyjb
{
	/// <summary>
	/// 指示当前方法是子类型处理器，会被相应的方法切换器调用。
	/// </summary>
	/// <remarks>关于方法切换器的更多信息，可以参加我的博文 
	/// <see href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">《C# 方法调用的切换器》</see></remarks>
	/// <seealso cref="MethodSwitcher"/>
	/// <seealso href="http://www.cnblogs.com/cyjb/archive/p/MethodSwitcher.html">《C# 方法调用的切换器》</seealso>
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public sealed class ProcessorAttribute : Attribute
	{
		/// <summary>
		/// 默认的处理器标识符。
		/// </summary>
		internal const string DefaultId = "Default";
		/// <summary>
		/// 处理器的标识符。
		/// </summary>
		private readonly string id;
		/// <summary>
		/// 使用默认的标识符初始化 <see cref="ProcessorAttribute"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="ProcessorAttribute"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public ProcessorAttribute() : this(DefaultId) { }
		/// <summary>
		/// 使用指定的标识符初始化 <see cref="ProcessorAttribute"/> 类的新实例。
		/// </summary>
		/// <param name="id">处理器的标识符。</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> 为 <c>null</c> 或空字符串。</exception>
		public ProcessorAttribute(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw CommonExceptions.StringEmpty(id);
			}
			Contract.EndContractBlock();
			this.id = id;
		}
		/// <summary>
		/// 获取处理器的标识符。
		/// </summary>
		/// <value>处理器的标识符，该方法只会被具有相同标识符的方法切换器调用。</value>
		public string Id
		{
			get { return this.id; }
		}
	}
}
