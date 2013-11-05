using System;
using System.Net;

namespace Cyjb.Net
{
	/// <summary>
	/// 提供使用 HTTP 协议将数据发送到由 URI 标识的资源及从这样的资源接收数据的常用方法。
	/// </summary>
	/// <remarks>添加了对 cookie 和一些 HTTP 标头的支持。</remarks>
	public sealed class HttpWebClient : WebClient
	{

		#region HTTP 标头字段

		/// <summary>
		/// <c>Accept</c> HTTP 标头的值，默认可以接受任意文件。
		/// </summary>
		private string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
		/// <summary>
		/// 请求是否应跟随重定向响应。
		/// </summary>
		private bool allowAutoRedirect = true;
		/// <summary>
		/// 与 HTTP 请求关联的 cookie。
		/// </summary>
		private CookieContainer cookies = new CookieContainer();
		/// <summary>
		/// 是否与 Internet 资源建立持久性连接。
		/// </summary>
		private bool keepAlive = true;
		/// <summary>
		/// 超时值（以毫秒为单位）。
		/// </summary>
		private int timeout = 100 * 1000;
		/// <summary>
		/// <c>User-agent</c> HTTP 标头的值。
		/// </summary>
		private string userAgent = "Mozilla/4.0 (MSIE 8.0; Windows NT 6.1; Trident/4.0)";
		/// <summary>
		/// 获取或设置 <c>Accept</c> HTTP 标头的值。
		/// </summary>
		/// <value><c>Accept</c> HTTP 标头的值。默认值为 
		/// <c>"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"</c>，可以接受任意文件。</value>
		public string Accept
		{
			get { return accept; }
			set { accept = value; }
		}
		/// <summary>
		/// 获取或设置一个值，该值指示请求是否应跟随重定向响应。
		/// </summary>
		/// <value>如果请求应自动跟随 Internet 资源的重定向响应，则为 <c>true</c>，
		/// 否则为 <c>false</c>。 默认值为 <c>true</c>。</value>
		public bool AllowAutoRedirect
		{
			get { return allowAutoRedirect; }
			set { allowAutoRedirect = value; }
		}
		/// <summary>
		/// 获取或设置与 HTTP 请求关联的 cookie。
		/// </summary>
		/// <value>与 HTTP 请求关联的 cookie。</value>
		public CookieContainer Cookies
		{
			get { return cookies; }
			set { cookies = value; }
		}
		/// <summary>
		/// 获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接。
		/// </summary>
		/// <value>如果对 Internet 资源的请求所包含的 <c>Connection</c> HTTP 标头
		/// 带有 Keep-alive 这一值，则为 <c>true</c>；否则为 <c>false</c>。
		/// 默认值为 <c>true</c>。</value>
		public bool KeepAlive
		{
			get { return keepAlive; }
			set { keepAlive = value; }
		}
		/// <summary>
		/// 获取或设置发送 HTTP 请求的超时值（以毫秒为单位）。
		/// </summary>
		/// <value>请求超时前等待的毫秒数。默认值为 100,000 毫秒（100 秒）。</value>
		public int Timeout
		{
			get { return timeout; }
			set { timeout = value; }
		}
		/// <summary>
		/// 获取或设置 <c>User-agent</c> HTTP 标头的值。
		/// </summary>
		/// <value><c>User-agent</c> HTTP 标头的值。默认值为
		/// "Mozilla/4.0 (MSIE 8.0; Windows NT 6.1; Trident/4.0)"。</value>
		public string UserAgent
		{
			get { return userAgent; }
			set { userAgent = value; }
		}

		#endregion // HTTP 标头字段

		/// <summary>
		/// 初始化 <see cref="HttpWebClient"/> 类的新实例。
		/// </summary>
		public HttpWebClient() { }
		/// <summary>
		/// 为指定资源返回一个 <see cref="WebRequest"/> 对象。
		/// </summary>
		/// <param name="address">一个 <see cref="Uri"/>，用于标识要请求的资源。</param>
		/// <returns>一个新的 <see cref="WebRequest"/> 对象，用于指定的资源。</returns>
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);
			HttpWebRequest httpRequest = request as HttpWebRequest;
			if (httpRequest != null)
			{
				httpRequest.Accept = this.accept;
				httpRequest.AllowAutoRedirect = this.allowAutoRedirect;
				httpRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
				httpRequest.CookieContainer = cookies;
				httpRequest.KeepAlive = this.keepAlive;
				httpRequest.Timeout = this.timeout;
				httpRequest.UserAgent = this.userAgent;
			}
			return request;
		}
	}
}
