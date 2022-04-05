using System.Diagnostics;
using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="BindingFlags"/> 类的扩展方法。
	/// </summary>
	internal static class BindingFlagsUtil
	{
		/// <summary>
		/// 搜索公共静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
		/// <summary>
		/// 搜索公共或私有静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags Static = BindingFlags.NonPublic | PublicStatic;
		/// <summary>
		/// 搜索公共实例成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
		/// <summary>
		/// 搜索公共或私有实例成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags Instance = BindingFlags.NonPublic | PublicInstance;
		/// <summary>
		/// 搜索公共实例或静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		/// <summary>
		/// 搜索公共或私有实例或静态成员的绑定标志。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags AllMember = BindingFlags.NonPublic | Public;
		/// <summary>
		/// 对可变参数（VarArgs）进行绑定，注意 <see cref="Type.InvokeMember(string, BindingFlags, Binder, object, object[])"/> 
		/// 并不支持可变参数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags VarArgsParamBinding = (BindingFlags)0x10000000;
	}
}
