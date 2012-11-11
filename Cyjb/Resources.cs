using System.Globalization;
using System.Resources;
using System.Threading;

namespace Cyjb
{
	/// <summary>
	/// 提供对资源的访问方法。
	/// </summary>
	internal sealed class Resources
	{

		#region 工厂方法

		/// <summary>
		/// 为多线程并发资源访问提供锁。
		/// </summary>
		private static object syncObject;
		/// <summary>
		/// 资源访问对象。
		/// </summary>
		private static Resources resLoader;
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
					object obj2 = new object();
					Interlocked.CompareExchange(ref syncObject, obj2, null);
				}
				return syncObject;
			}
		}
		/// <summary>
		/// 返回资源访问对象。
		/// </summary>
		private static Resources GetLoader()
		{
			if (resLoader == null)
			{
				lock (SyncObject)
				{
					if (resLoader == null)
					{
						resLoader = new Resources();
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

		#endregion

		/// <summary>
		/// 资源访问对象
		/// </summary>
		private readonly ResourceManager resManager;

		/// <summary>
		/// 初始化 <see cref="Cyjb.Resources"/> 类的新实例。
		/// </summary>
		private Resources()
		{
			this.resManager = new ResourceManager("Cyjb.Resource", this.GetType().Assembly);
		}
	}
}
