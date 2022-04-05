using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using Cyjb.Reflection;

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
			if (first is ConverterProvider firstProvider)
			{
				firstProvider.Add(second);
				return firstProvider;
			}
			if (second is ConverterProvider secondProvider)
			{
				secondProvider.Add(first);
				return secondProvider;
			}
			return new ConverterProvider(first, second);
		}

		/// <summary>
		/// 返回指定的转换器是否是有效的类型转换器。
		/// </summary>
		/// <param name="dlg">要检查的代理。</param>
		/// <param name="inputType">要将输入对象的类型。</param>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>如果代理是有效的类型转换器，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsValidConverter([NotNullWhen(true)] Delegate? dlg, Type inputType, Type outputType)
		{
			if (dlg == null)
			{
				return false;
			}
			MethodInfo method = dlg.Method;
			if (method.ReturnType != outputType)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParametersNoCopy();
			return parameters.Length == 1 && parameters[0].ParameterType == inputType;
		}

		/// <summary>
		/// 类型转换器的源类型。
		/// </summary>
		private readonly Type originType;
		/// <summary>
		/// 转换自的类型转换器字典。
		/// </summary>
		private readonly Dictionary<Type, Delegate> fromDict = new();
		/// <summary>
		/// 转换到的类型转换器字典。
		/// </summary>
		private readonly Dictionary<Type, Delegate> toDict = new();
		/// <summary>
		/// 提供者列表。
		/// </summary>
		private readonly List<IConverterProvider> providers = new();

		/// <summary>
		/// 使用指定的类型转换器初始化 <see cref="ConverterProvider"/> 类的新实例。
		/// </summary>
		/// <param name="converter">类型转换器。</param>
		/// <param name="inputType">类型转换器的输入类型。</param>
		/// <param name="outputType">类型转换器的输出类型。</param>
		public ConverterProvider(Delegate converter, Type inputType, Type outputType)
		{
			originType = inputType;
			toDict.Add(outputType, converter);
		}

		/// <summary>
		/// 通过合并现有的类型转换器提供者初始化 <see cref="ConverterProvider"/> 类的新实例。
		/// </summary>
		/// <param name="first">要合并的第一个类型转换器提供者。</param>
		/// <param name="second">要合并的第二个类型转换器提供者。</param>
		private ConverterProvider(IConverterProvider first, IConverterProvider second)
		{
			originType = first.OriginType;
			providers.Add(first);
			providers.Add(second);
		}

		/// <summary>
		/// 将当前类型转换器提供者与指定的 <see cref="IConverterProvider"/> 合并。
		/// </summary>
		/// <param name="provider">要合并的类型转换器提供者。</param>
		private void Add(IConverterProvider provider)
		{
			if (provider is ConverterProvider other)
			{
				foreach (KeyValuePair<Type, Delegate> pair in other.fromDict)
				{
					fromDict.Add(pair.Key, pair.Value);
				}
				foreach (KeyValuePair<Type, Delegate> pair in other.toDict)
				{
					toDict.Add(pair.Key, pair.Value);
				}
				providers.AddRange(other.providers);
			}
			else
			{
				providers.Add(provider);
			}
		}

		/// <summary>
		/// 获取类型转换器的源类型，与该类型相关的类型转换会查找当前提供者。
		/// </summary>
		/// <value>类型转换器的源类型。</value>
		public Type OriginType => originType;

		/// <summary>
		/// 返回将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 类型的类型转换器。
		/// </summary>
		/// <param name="outputType">要将输入对象转换到的类型。</param>
		/// <returns>将对象从 <see cref="OriginType"/> 类型转换为 <paramref name="outputType"/> 
		/// 类型的类型转换器，如果不存在则为 <c>null</c>。</returns>
		/// <remarks>返回的委托必须符合 <see cref="Converter{TInput,TOutput}"/>，
		/// 其输入类型是 <see cref="OriginType"/>，输出类型是 <paramref name="outputType"/>。</remarks>
		public Delegate? GetConverterTo(Type outputType)
		{
			if (toDict.TryGetValue(outputType, out Delegate? dlg))
			{
				return dlg;
			}
			// 子提供者按倒序遍历，这样后添加的会先被访问。
			for (int i = providers.Count - 1; i >= 0; i--)
			{
				dlg = providers[i].GetConverterTo(outputType);
				if (IsValidConverter(dlg, originType, outputType))
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
		public Delegate? GetConverterFrom(Type inputType)
		{
			if (fromDict.TryGetValue(inputType, out Delegate? dlg))
			{
				return dlg;
			}
			// 子提供者按倒序遍历，这样后添加的会先被访问。
			for (int i = providers.Count - 1; i >= 0; i--)
			{
				dlg = providers[i].GetConverterFrom(inputType);
				if (IsValidConverter(dlg, inputType, originType))
				{
					return dlg;
				}
			}
			return null;
		}
	}
}
