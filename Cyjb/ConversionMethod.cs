using System;
using System.Diagnostics;
using System.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 表示用户定义的类型转换方法。
	/// </summary>
	[DebuggerDisplay("{SourceType} -> {TargetType}")]
	internal class ConversionMethod
	{
		/// <summary>
		/// 类型转换方法的源类型。
		/// </summary>
		public readonly Type SourceType;
		/// <summary>
		/// 类型转换方法的目标类型。
		/// </summary>
		public readonly Type TargetType;
		/// <summary>
		/// 类型转换方法的句柄。
		/// </summary>
		public readonly RuntimeMethodHandle Method;
		/// <summary>
		/// 初始化 <see cref="ConversionMethod"/> 类的新实例。
		/// </summary>
		/// <param name="source">类型转换方法的源类型。</param>
		/// <param name="target">类型转换方法的目标类型。</param>
		/// <param name="m">类型转换方法的句柄。</param>
		public ConversionMethod(Type source, Type target, MethodInfo m)
		{
			SourceType = source;
			TargetType = target;
			Method = m.MethodHandle;
		}
	}
}
