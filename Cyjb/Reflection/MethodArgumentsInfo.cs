using System;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 方法的实参列表信息。
	/// </summary>
	internal class MethodArgumentsInfo
	{
		/// <summary>
		/// 泛型类型参数的推断结果。
		/// </summary>
		public Type[] GenericArguments;
		/// <summary>
		/// params 形参的类型，如果为 <c>nul</c> 表示无需特殊处理 params 参数。
		/// </summary>
		public Type ParamArrayType;
		/// <summary>
		/// params 实参的类型，如果为 <c>nul</c> 表示无需特殊处理 params 参数。
		/// </summary>
		public Type[] ParamArgumentTypes;
		/// <summary>
		/// 可变参数的类型，如果为 <c>null</c> 表示没有可变参数。
		/// </summary>
		public Type[] OptionalArgumentTypes;
	}
}
