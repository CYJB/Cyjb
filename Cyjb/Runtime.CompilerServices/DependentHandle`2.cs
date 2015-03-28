using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Cyjb.Runtime.CompilerServices
{
	/// <summary>
	/// 表示依赖句柄，在键存在时对值强引用，键被回收后对值弱引用。
	/// </summary>
	/// <typeparam name="TKey">键的类型。</typeparam>
	/// <typeparam name="TValue">值的类型。</typeparam>
	/// <remarks><para>与 <see cref="ConditionalWeakTable{TKey,TValue}"/> 相同，<see cref="DependentHandle{TKey,TValue}"/>
	/// 自身不对键强引用，只要外部没有对键的其它引用，键就可以被回收。</para>
	/// <para>如果键在外部存在强引用，那么 <see cref="DependentHandle{TKey,TValue}"/> 会保证值不会被回收（强引用）。
	/// 如果键在外部没有强引用，那么值也可以被回收（弱引用）。</para></remarks>
	/// <seealso cref="ConditionalWeakTable{TKey,TValue}"/>
	[ComVisible(false)]
	[SecurityCritical, SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
	public sealed class DependentHandle<TKey, TValue> : IDisposable
		where TKey : class
		where TValue : class
	{

		#region 成员反射

		/// <summary>
		/// 用于反射的 <c>System.Runtime.CompilerServices.DependentHandle</c> 的类型。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Type reflectionType = Type.GetType("System.Runtime.CompilerServices.DependentHandle");
		/// <summary>
		/// 初始化方法的委托类型。
		/// </summary>
		/// <param name="key">键。</param>
		/// <param name="value">值。</param>
		/// <param name="dependentHandle">初始化的依赖句柄。</param>
		private delegate void Initialize(object key, object value, out IntPtr dependentHandle);
		/// <summary>
		/// 获取键的委托类型。
		/// </summary>
		/// <param name="dependentHandle">依赖句柄。</param>
		/// <param name="key">获取的键。</param>
		private delegate void GetKey(IntPtr dependentHandle, out object key);
		/// <summary>
		/// 获取键和值的委托类型。
		/// </summary>
		/// <param name="dependentHandle">依赖句柄。</param>
		/// <param name="key">获取的键。</param>
		/// <param name="value">获取的值。</param>
		private delegate void GetKeyAndValue(IntPtr dependentHandle, out object key, out object value);
		/// <summary>
		/// 初始化依赖句柄。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Initialize initialize = reflectionType.CreateDelegate<Initialize>("nInitialize");
		/// <summary>
		/// 获取键。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly GetKey getKey = reflectionType.CreateDelegate<GetKey>("nGetPrimary");
		/// <summary>
		/// 获取键和值。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly GetKeyAndValue getKeyAndValue =
			reflectionType.CreateDelegate<GetKeyAndValue>("nGetPrimaryAndSecondary");
		/// <summary>
		/// 释放依赖句柄。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Action<IntPtr> free = reflectionType.CreateDelegate<Action<IntPtr>>("nFree");

		#endregion // 成员反射

		/// <summary>
		/// 依赖句柄。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IntPtr handle = IntPtr.Zero;
		/// <summary>
		/// 使用指定的键和值初始化 <see cref="DependentHandle"/> 类的新实例。
		/// </summary>
		/// <param name="key">键。</param>
		/// <param name="value">值。</param>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public DependentHandle(TKey key, TValue value)
		{
			CommonExceptions.CheckArgumentNull(key, "key");
			CommonExceptions.CheckArgumentNull(value, "value");
			Contract.EndContractBlock();
			initialize(key, value, out handle);
		}
		/// <summary>
		/// 获取依赖的键。
		/// </summary>
		/// <value>依赖的键，如果已被释放则为 <c>null</c>。</value>
		public TKey Key
		{
			get
			{
				if (handle == IntPtr.Zero)
				{
					return null;
				}
				object key;
				getKey(handle, out key);
				if (key == null)
				{
					// 键已被释放掉，就尽早释放句柄。
					this.Dispose();
				}
				return key as TKey;
			}
		}
		/// <summary>
		/// 获取与键关联的值。
		/// </summary>
		/// <value>与键关联的值，如果已被释放则为 <c>null</c>。</value>
		public TValue Value
		{
			get
			{
				if (handle == IntPtr.Zero)
				{
					return null;
				}
				object key;
				object value;
				getKeyAndValue(handle, out key, out value);
				if (key == null)
				{
					// 键已被释放掉，就尽早释放句柄。
					this.Dispose();
					return null;
				}
				return value as TValue;
			}
		}

		#region IDisposable 成员

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		public void Dispose()
		{
			if (handle != IntPtr.Zero)
			{
				free(handle);
				handle = IntPtr.Zero;
			}
		}

		#endregion // IDisposable 成员

		/// <summary>
		/// 终结器。
		/// </summary>
		~DependentHandle()
		{
			if (Environment.HasShutdownStarted)
			{
				return;
			}
			this.Dispose();
		}
	}
}