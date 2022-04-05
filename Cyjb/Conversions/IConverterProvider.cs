namespace Cyjb
{
	/// <summary>
	/// 类型转换器的提供者，可以根据输入输出类型生成类型转换器。
	/// </summary>
	public interface IConverterProvider
	{
		/// <summary>
		/// 获取类型转换器的源类型，与该类型相关的类型转换会查找当前提供者。
		/// </summary>
		/// <value>类型转换器的源类型。</value>
		/// <remarks>返回值不能为 <c>null</c>，且不能为 <c>void</c> 类型。</remarks>
		Type OriginType { get; }

		/// <summary>
		/// 返回将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 类型的类型转换器。
		/// 要求类型转换器是静态方法，不能是实例方法。
		/// </summary>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <see cref="OriginType"/>，输出类型是 <paramref name="outputType"/>。</remarks>
		Delegate? GetConverterTo(Type outputType);

		/// <summary>
		/// 返回将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 类型的类型转换器。
		/// 要求类型转换器是静态方法，不能是实例方法。
		/// </summary>
		/// <param name="inputType">输入对象的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <paramref name="inputType"/>，输出类型是 <see cref="OriginType"/>。</remarks>
		Delegate? GetConverterFrom(Type inputType);
	}
}
