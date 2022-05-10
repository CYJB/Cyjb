using System.Diagnostics;
using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="MethodBase"/> 的扩展方法。
	/// </summary>
	public static partial class MethodBaseUtil
	{
		/// <summary>
		/// 获取参数列表而不复制的委托。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Func<MethodBase, ParameterInfo[]> getParametersNoCopy = BuildGetParametersNoCopy();
		/// <summary>
		/// 构造获取参数列表而不复制的委托，兼容 Mono 运行时。
		/// </summary>
		/// <returns>获取参数列表而不复制的委托。</returns>
		private static Func<MethodBase, ParameterInfo[]> BuildGetParametersNoCopy()
		{
			// 为了防止 DelegateBuilder 里调用 GetParametersNoCopy 而导致死循环，这里必须使用 Delegate.CreateDelegate 方法。
			MethodInfo methodGetParametersNoCopy = typeof(MethodBase).GetMethod("GetParametersNoCopy", BindingFlagsUtil.Instance)!;
			return (Func<MethodBase, ParameterInfo[]>)Delegate.CreateDelegate(typeof(Func<MethodBase, ParameterInfo[]>), methodGetParametersNoCopy);
		}

		/// <summary>
		/// 返回当前方法的参数列表，而不会复制参数列表。
		/// </summary>
		/// <param name="method">要获取参数列表的方法。</param>
		/// <returns>方法的参数列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static ParameterInfo[] GetParametersNoCopy(this MethodBase method)
		{
			ArgumentNullException.ThrowIfNull(method);
			return getParametersNoCopy(method);
		}

		/// <summary>
		/// 返回当前方法的参数类型列表。
		/// </summary>
		/// <param name="method">要获取参数类型列表的方法。</param>
		/// <returns>方法的参数类型列表。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="method"/> 为 <c>null</c>。</exception>
		public static Type[] GetParameterTypes(this MethodBase method)
		{
			ArgumentNullException.ThrowIfNull(method);
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			if (parameters.Length == 0)
			{
				return Type.EmptyTypes;
			}
			Type[] types = new Type[parameters.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}
	}
}
