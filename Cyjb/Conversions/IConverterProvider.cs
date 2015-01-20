using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Cyjb.Reflection;

namespace Cyjb
{
	/// <summary>
	/// 类型转换器的提供者，可以根据输入输出类型生成类型转换器。
	/// </summary>
	[ContractClass(typeof(ContractsForIConverterProvider))]
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
		/// </summary>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <see cref="OriginType"/>，输出类型是 <paramref name="outputType"/>。</remarks>
		Delegate GetConverterTo(Type outputType);
		/// <summary>
		/// 返回将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="inputType">输入对象的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <paramref name="inputType"/>，输出类型是 <see cref="OriginType"/>。</remarks>
		Delegate GetConverterFrom(Type inputType);
	}
	/// <summary>
	/// 类型转换器提供者的辅助类。
	/// </summary>
	internal static class ConverterProviderExt
	{
		/// <summary>
		/// 返回指定的代理是否是有效的类型转换器。
		/// </summary>
		/// <param name="provider">类型转换器提供者。</param>
		/// <param name="dlg">要检查的代理。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>如果代理是有效的类型转换器，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		[Pure]
		public static bool IsValidConverterTo(this IConverterProvider provider, Delegate dlg, Type outputType)
		{
			Contract.Requires(provider != null && outputType != null);
			if (dlg == null)
			{
				return false;
			}
			MethodInfo method = dlg.GetType().GetMethod("Invoke");
			if (method.ReturnType != outputType)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			return parameters.Length == 1 && parameters[0].ParameterType == provider.OriginType;
		}
		/// <summary>
		/// 返回指定的代理是否是有效的类型转换器。
		/// </summary>
		/// <param name="provider">类型转换器提供者。</param>
		/// <param name="dlg">要检查的代理。</param>
		/// <param name="inputType">输入对象的类型。</param>
		/// <returns>如果代理是有效的类型转换器，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		[Pure]
		public static bool IsValidConverterFrom(this IConverterProvider provider, Delegate dlg, Type inputType)
		{
			Contract.Requires(provider != null && inputType != null);
			if (dlg == null)
			{
				return false;
			}
			MethodInfo method = dlg.GetType().GetMethod("Invoke");
			if (method.ReturnType != provider.OriginType)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			return (parameters.Length == 1 && parameters[0].ParameterType == inputType) ||
				(parameters.Length == 2 && parameters[1].ParameterType == inputType);
		}
	}
	/// <summary>
	/// 表示 <see cref="IConverterProvider"/> 接口的协定。
	/// </summary>
	[ContractClassFor(typeof(IConverterProvider))]
	internal abstract class ContractsForIConverterProvider : IConverterProvider
	{
		/// <summary>
		/// 初始化 <see cref="ContractsForIConverterProvider"/> 类的新实例。
		/// </summary>
		private ContractsForIConverterProvider() { }
		/// <summary>
		/// 获取类型转换器的源类型，与该类型相关的类型转换会查找当前提供者。
		/// </summary>
		/// <value>类型转换器的源类型。</value>
		Type IConverterProvider.OriginType
		{
			get
			{
				Contract.Ensures(Contract.Result<Type>() != null && Contract.Result<Type>() != typeof(void));
				return typeof(object);
			}
		}
		/// <summary>
		/// 返回将对象从 <see cref="IConverterProvider.OriginType"/> 类型转换为 
		/// <paramref name="outputType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <see cref="IConverterProvider.OriginType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <see cref="IConverterProvider.OriginType"/>，输出类型是 <paramref name="outputType"/>。</remarks>
		Delegate IConverterProvider.GetConverterTo(Type outputType)
		{
			Contract.Requires(outputType != null);
			Contract.Ensures(Contract.Result<Delegate>() == null ||
				this.IsValidConverterTo(Contract.Result<Delegate>(), outputType));
			return null;
		}
		/// <summary>
		/// 返回将对象从 <paramref name="inputType"/> 类型转换为 
		/// <see cref="IConverterProvider.OriginType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="inputType">输入对象的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <see cref="IConverterProvider.OriginType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <paramref name="inputType"/>，输出类型是 <see cref="IConverterProvider.OriginType"/>。</remarks>
		Delegate IConverterProvider.GetConverterFrom(Type inputType)
		{
			Contract.Requires(inputType != null);
			Contract.Ensures(Contract.Result<Delegate>() == null ||
				this.IsValidConverterFrom(Contract.Result<Delegate>(), inputType));
			return null;
		}
	}
}
