using System.Globalization;
using System.Resources;
using System.Threading;

namespace Cyjb
{
	/// <summary>
	/// 提供对异常资源的访问方法。
	/// </summary>
	internal sealed class ExceptionResources
	{

		#region 工厂方法

		/// <summary>
		/// 为多线程并发资源访问提供锁。
		/// </summary>
		private static object syncObject;
		/// <summary>
		/// 异常资源访问对象。
		/// </summary>
		private static ExceptionResources resLoader;
		/// <summary>
		/// 获取并发访问的锁。
		/// </summary>
		/// <value>一个用于同步的对象。</value>
		private static object SyncObject
		{
			get
			{
				if (syncObject == null)
				{
					object obj = new object();
					Interlocked.CompareExchange(ref syncObject, obj, null);
				}
				return syncObject;
			}
		}
		/// <summary>
		/// 返回资源访问对象。
		/// </summary>
		private static ExceptionResources GetLoader()
		{
			if (resLoader == null)
			{
				lock (SyncObject)
				{
					if (resLoader == null)
					{
						resLoader = new ExceptionResources();
					}
				}
			}
			return resLoader;
		}
		/// <summary>
		/// 获取资源管理对象。
		/// </summary>
		/// <value>一个 <see cref="System.Resources.ResourceManager"/> 对象。
		/// </value>
		public static ResourceManager ResManager
		{
			get { return GetLoader().resManager; }
		}

		#endregion

		#region 资源访问方法

		/// <summary>
		/// 获取调用方的当前区域性设置。
		/// </summary>
		/// <value>一个 <see cref="System.Globalization.CultureInfo"/> 对象。
		/// </value>
		public static CultureInfo Culture
		{
			get { return CultureInfo.CurrentCulture; }
		}

		/// <summary>
		/// 返回指定的 <see cref="System.String"/> 资源的值。
		/// </summary>
		/// <param name="name">要获取的资源名。</param>
		/// <returns>资源的值。如果不可能有最佳匹配，则返回 <c>null</c>。
		/// </returns>
		public static string GetString(string name)
		{
			return ResManager.GetString(name, Culture);
		}
		/// <summary>
		/// 使用指定的 <see cref="System.String"/> 资源的值格式化参数序列。
		/// </summary>
		/// <param name="name">要使用的资源名。</param>
		/// <param name="args">要格式化的参数序列。</param>
		/// <returns>格式化的参数序列。如果不可能有最佳匹配，则返回 <c>null</c>。</returns>
		public static string GetString(string name, params object[] args)
		{
			string format = GetString(name);
			if (string.IsNullOrEmpty(format) || args == null || args.Length <= 0)
			{
				return format;
			}
			return string.Format(Culture, format, args);
		}

		#endregion

		/// <summary>
		/// 资源访问对象
		/// </summary>
		private readonly ResourceManager resManager;

		/// <summary>
		/// 初始化 <see cref="Cyjb.ExceptionResources"/> 类的新实例。
		/// </summary>
		private ExceptionResources()
		{
			this.resManager = new ResourceManager("Cyjb.ExceptionResource", this.GetType().Assembly);
		}
	}
}
