using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Conversions;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 提供 <see cref="FieldInfo"/> 创建委托的扩展方法。
	/// </summary>
	public static class FieldInfoUtil
	{
		/// <summary>
		/// 创建用于表示指定静态或实例字段的指定类型的委托。
		/// </summary>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例字段。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段；会忽略不需要的参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例字段的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static Delegate? PowerDelegate(this FieldInfo field, Type delegateType)
		{
			CommonExceptions.CheckArgumentNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ReflectionExceptions.CheckUnboundGenParam(field);
			return field.PowerDelegate(delegateType, false, null);
		}

		/// <summary>
		/// 创建用于表示指定静态或实例字段的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例字段。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段；会忽略不需要的参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例字段的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static TDelegate? PowerDelegate<TDelegate>(this FieldInfo field)
			where TDelegate : Delegate
		{
			ReflectionExceptions.CheckUnboundGenParam(field);
			Type delegateType = typeof(TDelegate);
			return field.PowerDelegate(delegateType, false, null) as TDelegate;
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例字段的指定类型的委托。
		/// </summary>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="firstArgument">如果是实例字段，则作为委托要绑定到的对象；否则将作为字段的第一个参数。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例字段。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果委托具有返回值，则认为是获取字段，否则认为是设置字段；会忽略不需要的参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static Delegate? PowerDelegate(this FieldInfo field, Type delegateType, object? firstArgument)
		{
			CommonExceptions.CheckArgumentNull(delegateType);
			CommonExceptions.CheckDelegateType(delegateType);
			ReflectionExceptions.CheckUnboundGenParam(field);
			return field.PowerDelegate(delegateType, true, firstArgument);
		}

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示指定静态或实例字段的指定类型的委托。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="firstArgument">如果是实例字段，则作为委托要绑定到的对象；否则将作为字段的第一个参数。</param>
		/// <returns>指定类型的委托，表示访问指定的静态或实例字段。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果委托具有返回值，则认为是获取字段，否则认为是设置字段；会忽略不需要的参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		/// <overloads>
		/// <summary>
		/// 创建用于访问指定静态或实例字段的指定类型的委托。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// </overloads>
		public static TDelegate? PowerDelegate<TDelegate>(this FieldInfo field, object? firstArgument)
			where TDelegate : Delegate
		{
			ReflectionExceptions.CheckUnboundGenParam(field);
			Type delegateType = typeof(TDelegate);
			return field.PowerDelegate(delegateType, true, firstArgument) as TDelegate;
		}

		/// <summary>
		/// 创建用于表示指定静态或实例字段的指定类型的委托。
		/// </summary>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="delegateType">要创建的委托类型。</param>
		/// <param name="hasFirstArg">是否指定了首个参数的类型。</param>
		/// <param name="firstArg">首个参数的值。</param>
		/// <returns>指定类型的动态字段，表示访问指定的静态或实例字段。如果无法绑定，则为 <c>null</c>。</returns>
		/// <remarks>如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段；会忽略不需要的参数。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。</remarks>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		internal static Delegate? PowerDelegate(this FieldInfo field, Type delegateType, bool hasFirstArg, object? firstArg)
		{
			MethodInfo invoke = delegateType.GetDelegateInvoke();
			Type returnType = invoke.ReturnType;
			Type[] types = invoke.GetParameterTypes();
			if (hasFirstArg)
			{
				if (field.IsStatic && returnType != typeof(void))
				{
					// 获取静态字段的 delegate，首个参数没有用，忽略即可。
					hasFirstArg = false;
				}
				else
				{
					Type firstParamType = field.IsStatic ? field.FieldType : field.DeclaringType!;
					// 如果首个参数是值类型，由于 CreateDelegate 只能传递 object，需要提前做类型转换，确保与 firstParamType 匹配。
					if (firstArg != null && firstArg.GetType().IsValueType)
					{
						firstArg = GenericConvert.ChangeType(firstArg, firstParamType);
						firstParamType = typeof(object);
					}
					types = types.Insert(0, firstParamType);
				}
			}
			// 构造动态委托。
			DynamicMethod dlgMethod;
			if (field.DeclaringType == null)
			{
				dlgMethod = new("FieldDelegate", returnType, types, field.Module, true);
			}
			else
			{
				dlgMethod = new("FieldDelegate", returnType, types, field.DeclaringType, true);
			}
			ILGenerator il = dlgMethod.GetILGenerator();
			int index = 0;
			// 实例字段的第一个参数用作传递实例对象。
			if (!field.IsStatic)
			{
				il.EmitLoadInstance(field, types[0], true);
				index++;
			}
			Type fieldType = field.FieldType;
			if (returnType == typeof(void))
			{
				// 设置字段值。
				if (types.Length < index + 1)
				{
					return null;
				}
				Type valueType = types[index];
				if (!fieldType.IsExplicitFrom(valueType))
				{
					return null;
				}
				il.EmitLoadParameter(fieldType, index, valueType);
				il.EmitStoreField(field);
			}
			else
			{
				// 获取字段值。
				Conversion? conversion = ConversionFactory.GetConversion(fieldType, returnType);
				if (conversion == null)
				{
					return null;
				}
				if (conversion.PassByAddr)
				{
					il.EmitLoadFieldAddress(field);
				}
				else
				{
					il.EmitLoadField(field);
				}
				conversion.Emit(il, fieldType, returnType, true);
			}
			il.Emit(OpCodes.Ret);
			if (hasFirstArg)
			{
				return dlgMethod.CreateDelegate(delegateType, firstArg);
			}
			else
			{
				return dlgMethod.CreateDelegate(delegateType);
			}
		}
	}
}
