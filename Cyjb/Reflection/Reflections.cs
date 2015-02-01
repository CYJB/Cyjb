using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 预定义的反射信息。
	/// </summary>
	internal static class Reflections
	{
		/// <summary>
		/// 表示 <see cref="Closure.Constants"/> 字段。
		/// </summary>
		public static readonly FieldInfo ClosureConstants = typeof(Closure).GetField("Constants");
	}
}

