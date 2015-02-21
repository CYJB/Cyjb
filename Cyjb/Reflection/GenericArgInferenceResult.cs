using System;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 泛型类型参数推断的结果。
	/// </summary>
	internal class GenericArgInferenceResult
	{
		/// <summary>
		/// 泛型类型参数的推断结果。
		/// </summary>
		public Type[] GenericArguments;
		/// <summary>
		/// params 参数的类型。
		/// </summary>
		public Type[] ParamArrayTypes;
		/// <summary>
		/// 可选参数的类型。
		/// </summary>
		public Type[] OptionalParameterTypes;
	}
}
