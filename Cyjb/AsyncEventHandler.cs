using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cyjb
{
	/// <summary>
	/// 表示将处理不包含事件数据的异步事件的方法。
	/// </summary>
	/// <param name="sender">事件源。</param>
	/// <param name="e">不包含事件数据的对象。</param>
	/// <returns>处理事件的异步操作。</returns>
	[Serializable]
	public delegate Task AsyncEventHandler(object sender, EventArgs e);
	/// <summary>
	/// 表示将处理不包含事件数据的异步事件的方法。
	/// </summary>
	/// <typeparam name="TEventArgs">由该事件生成的事件数据的类型。</typeparam>
	/// <param name="sender">事件源。</param>
	/// <param name="e">一个包含事件数据的对象。</param>
	/// <returns>处理事件的异步操作。</returns>
	[Serializable]
	public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs e);
	/// <summary>
	/// 表示异步事件的扩展方法。
	/// </summary>
	public static class AsyncEventHandlerExt
	{
		/// <summary>
		/// 调度异步事件，并返回会等待所有异步操作全部完成的操作。
		/// </summary>
		/// <param name="handler">要调度的异步事件。</param>
		/// <param name="sender">事件源。</param>
		/// <returns>会等待所有异步操作全部完成的操作。</returns>
		/// <overloads>
		/// <summary>
		/// 调度异步事件，并返回会等待所有异步操作全部完成的操作。
		/// </summary>
		/// </overloads>
		public static async Task OnAll(this AsyncEventHandler handler, object sender)
		{
			if (handler == null)
			{
				return;
			}
			Delegate[] invocations = handler.GetInvocationList();
			await Task.WhenAll(invocations.Select(invocation => ((AsyncEventHandler)invocation)(sender, EventArgs.Empty)));
		}
		/// <summary>
		/// 调度异步事件，并返回会等待所有异步操作全部完成的操作。
		/// </summary>
		/// <param name="handler">要调度的异步事件。</param>
		/// <param name="sender">事件源。</param>
		/// <param name="e">一个包含事件数据的对象。</param>
		/// <returns>会等待所有异步操作全部完成的操作。</returns>
		public static async Task OnAll<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e)
		{
			if (handler == null)
			{
				return;
			}
			Delegate[] invocations = handler.GetInvocationList();
			await Task.WhenAll(invocations.Select(invocation => ((AsyncEventHandler<TEventArgs>)invocation)(sender, e)));
		}
		/// <summary>
		/// 调度异步事件，并返回任意异步操作完成，就会完成的操作。
		/// </summary>
		/// <param name="handler">要调度的异步事件。</param>
		/// <param name="sender">事件源。</param>
		/// <returns>任意异步操作完成，就会完成的操作。</returns>
		/// <overloads>
		/// <summary>
		/// 调度异步事件，并返回任意异步操作完成，就会完成的操作。
		/// </summary>
		/// </overloads>
		public static async Task OnAny(this AsyncEventHandler handler, object sender)
		{
			if (handler == null)
			{
				return;
			}
			Delegate[] invocations = handler.GetInvocationList();
			await Task.WhenAny(invocations.Select(invocation => ((AsyncEventHandler)invocation)(sender, EventArgs.Empty)));
		}
		/// <summary>
		/// 调度异步事件，并返回任意异步操作完成，就会完成的操作。
		/// </summary>
		/// <param name="handler">要调度的异步事件。</param>
		/// <param name="sender">事件源。</param>
		/// <param name="e">一个包含事件数据的对象。</param>
		/// <returns>任意异步操作完成，就会完成的操作。</returns>
		public static async Task OnAny<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e)
		{
			if (handler == null)
			{
				return;
			}
			Delegate[] invocations = handler.GetInvocationList();
			await Task.WhenAny(invocations.Select(invocation => ((AsyncEventHandler<TEventArgs>)invocation)(sender, e)));
		}
		/// <summary>
		/// 在正确的线程调度当前异步事件。
		/// </summary>
		/// <param name="handler">要调度的异步事件。</param>
		/// <param name="sender">事件源。</param>
		/// <param name="e">不包含事件数据的对象。</param>
		/// <remarks>如果异步事件的接收者实现了 <see cref="ISynchronizeInvoke"/> 接口，那么会将调用封送到正确的线程。</remarks>
		/// <overrides>
		/// <summary>
		/// 在正确的线程调度当前异步事件。
		/// </summary>
		/// </overrides>
		public static void Raise(this AsyncEventHandler handler, object sender, EventArgs e)
		{
			foreach (Delegate dlg in handler.GetInvocationList())
			{
				ISynchronizeInvoke target = dlg.Target as ISynchronizeInvoke;
				if (target != null)
				{
					target.BeginInvoke(dlg, new[] { sender, e });
				}
				else
				{
					((AsyncEventHandler)dlg)(sender, e);
				}
			}
		}
		/// <summary>
		/// 在正确的线程调度当前异步事件。
		/// </summary>
		/// <typeparam name="TEventArgs">由该事件生成的事件数据的类型。</typeparam>
		/// <param name="handler">要调度的异步事件。</param>
		/// <param name="sender">事件源。</param>
		/// <param name="e">不包含事件数据的对象。</param>
		/// <remarks>如果异步事件的接收者实现了 <see cref="ISynchronizeInvoke"/> 接口，那么会将调用封送到正确的线程。</remarks>
		public static void Raise<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e)
		{
			foreach (Delegate dlg in handler.GetInvocationList())
			{
				ISynchronizeInvoke target = dlg.Target as ISynchronizeInvoke;
				if (target != null)
				{
					target.BeginInvoke(dlg, new[] { sender, e });
				}
				else
				{
					((AsyncEventHandler<TEventArgs>)dlg)(sender, e);
				}
			}
		}
	}
}
