using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Cyjb.Conversions
{
	/// <summary>
	/// 基本的类型转换器的提供者。
	/// </summary>
	internal sealed class ConverterProvider : IConverterProvider
	{
		/// <summary>
		/// 合并指定的两个类型转换器提供者。
		/// </summary>
		/// <param name="first">要合并的第一个类型转换器提供者。</param>
		/// <param name="second">要合并的第二个类型转换器提供者。</param>
		/// <returns>合并得到的类型转换器。</returns>
		public static IConverterProvider Combine(IConverterProvider first, IConverterProvider second)
		{
			Contract.Requires(first != null && second != null);
			Contract.Requires(first.OriginType != null && first.OriginType == second.OriginType);
			ConverterProvider firstProvider = first as ConverterProvider;
			ConverterProvider secondProvider = second as ConverterProvider;
			if (firstProvider != null)
			{
				if (secondProvider != null)
				{
					firstProvider.CombineWith(secondProvider);
				}
				else
				{
					firstProvider.CombineWith(second);
				}
				return firstProvider;
			}
			if (secondProvider != null)
			{
				secondProvider.CombineWith(first);
				return secondProvider;
			}
			return new ConverterProvider(first, second);
		}
		/// <summary>
		/// 类型转换器的源类型。
		/// </summary>
		private readonly Type originType;
		/// <summary>
		/// 转换自的类型转换器字典。
		/// </summary>
		private readonly Dictionary<Type, Delegate> fromDict;
		/// <summary>
		/// 转换到的类型转换器字典。
		/// </summary>
		private readonly Dictionary<Type, Delegate> toDict;
		/// <summary>
		/// 子提供者。
		/// </summary>
		private IConverterProvider[] subProvider;
		/// <summary>
		/// 使用指定的类型转换器初始化 <see cref="ConverterProvider"/> 类的新实例。
		/// </summary>
		/// <param name="converter">类型转换器。</param>
		/// <param name="inputType">类型转换器的输入类型。</param>
		/// <param name="outputType">类型转换器的输出类型。</param>
		public ConverterProvider(Delegate converter, Type inputType, Type outputType)
		{
			Contract.Requires(converter != null && inputType != null && outputType != null);
			this.originType = inputType;
			this.toDict = new Dictionary<Type, Delegate>(1)
			{
				{ outputType, converter }
			};
			this.fromDict = new Dictionary<Type, Delegate>();
			this.toDict = new Dictionary<Type, Delegate>();
			this.subProvider = ArrayExt.Empty<IConverterProvider>();
		}
		/// <summary>
		/// 通过合并现有的类型转换器提供者初始化 <see cref="ConverterProvider"/> 类的新实例。
		/// </summary>
		/// <param name="first">要合并的第一个类型转换器提供者。</param>
		/// <param name="second">要合并的第二个类型转换器提供者。</param>
		private ConverterProvider(IConverterProvider first, IConverterProvider second)
		{
			Contract.Requires(first != null && second != null);
			Contract.Requires(first.OriginType != null && first.OriginType == second.OriginType);
			this.originType = first.OriginType;
			this.fromDict = new Dictionary<Type, Delegate>();
			this.toDict = new Dictionary<Type, Delegate>();
			this.subProvider = new[] { first, second };
		}
		/// <summary>
		/// 将当前类型转换器提供者与指定的 <see cref="ConverterProvider"/> 合并。
		/// </summary>
		/// <param name="provider">要合并的类型转换器提供者。</param>
		private void CombineWith(ConverterProvider provider)
		{
			Contract.Requires(provider != null && this.OriginType == provider.OriginType);
			foreach (KeyValuePair<Type, Delegate> pair in provider.fromDict)
			{
				this.fromDict.Add(pair.Key, pair.Value);
			}
			foreach (KeyValuePair<Type, Delegate> pair in provider.toDict)
			{
				this.toDict.Add(pair.Key, pair.Value);
			}
			if (this.subProvider.Length == 0)
			{
				this.subProvider = provider.subProvider;
			}
			else if (provider.subProvider.Length > 0)
			{
				this.subProvider = ArrayExt.Combine(this.subProvider, provider.subProvider);
			}
		}
		/// <summary>
		/// 将当前类型转换器提供者与指定的 <see cref="IConverterProvider"/> 合并。
		/// </summary>
		/// <param name="provider">要合并的类型转换器提供者。</param>
		private void CombineWith(IConverterProvider provider)
		{
			Contract.Requires(provider != null && this.OriginType == provider.OriginType);
			if (this.subProvider.Length == 0)
			{
				this.subProvider = new[] { provider };
			}
			else
			{
				IConverterProvider[] newProviders = new IConverterProvider[this.subProvider.Length + 1];
				this.subProvider.CopyTo(newProviders, 0);
				newProviders[newProviders.Length - 1] = provider;
				this.subProvider = newProviders;
			}
		}
		/// <summary>
		/// 获取类型转换器的源类型，与该类型相关的类型转换会查找当前提供者。
		/// </summary>
		/// <value>类型转换器的源类型。</value>
		public Type OriginType
		{
			get { return this.originType; }
		}
		/// <summary>
		/// 返回将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <see cref="OriginType"/>，输出类型是 <paramref name="outputType"/>。</remarks>
		public Delegate GetConverterTo(Type outputType)
		{
			Delegate dlg;
			if (toDict.TryGetValue(outputType, out dlg))
			{
				return dlg;
			}
			// 子提供者按倒序遍历，这样后添加的会先被访问。
			for (int i = subProvider.Length - 1; i >= 0; i--)
			{
				dlg = subProvider[i].GetConverterTo(outputType);
				if (dlg != null && this.IsValidConverterTo(dlg, outputType))
				{
					return dlg;
				}
			}
			return null;
		}
		/// <summary>
		/// 返回将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="inputType">输入对象的类型。</param>
		/// <returns>将对象从 <paramref name="inputType"/> 类型转换为 <see cref="OriginType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <paramref name="inputType"/>，输出类型是 <see cref="OriginType"/>。</remarks>
		public Delegate GetConverterFrom(Type inputType)
		{
			Delegate dlg;
			if (fromDict.TryGetValue(inputType, out dlg))
			{
				return dlg;
			}
			// 子提供者按倒序遍历，这样后添加的会先被访问。
			for (int i = subProvider.Length - 1; i >= 0; i--)
			{
				dlg = subProvider[i].GetConverterFrom(inputType);
				if (dlg != null && this.IsValidConverterFrom(dlg, inputType))
				{
					return dlg;
				}
			}
			return null;
		}
	}
}
