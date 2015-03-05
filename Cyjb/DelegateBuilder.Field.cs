using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Emit;
using Cyjb.Reflection;

namespace Cyjb
{
	public static partial class DelegateBuilder
	{

		#region 构造开放字段委托

		/// <summary>
		/// 创建用于表示获取或设置指定静态或实例字段的指定类型的委托。
		/// 如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/> 。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateOpenDelegate(field, type);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例字段的指定类型的委托。
		/// 如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field, bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.EndContractBlock();
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateOpenDelegate(field, type);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例字段的指定类型的委托。
		/// 如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(this FieldInfo field, Type delegateType)
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateOpenDelegate(field, delegateType);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg;
		}
		/// <summary>
		/// 使用针对绑定失败的指定行为，创建用于表示获取或设置指定静态或实例字段的指定类型的委托。
		/// 如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(this FieldInfo field, Type delegateType, bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.EndContractBlock();
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateOpenDelegate(field, delegateType);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例字段的指定类型的开放字段委托。
		/// 如果是实例字段，需要将实例对象作为委托的第一个参数。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="field">要获取或设置的字段。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <returns><paramref name="delegateType"/> 类型的委托，表示静态或实例字段的委托。</returns>
		private static Delegate CreateOpenDelegate(FieldInfo field, Type delegateType)
		{
			Contract.Requires(field != null && delegateType != null);
			MethodInfo invoke = delegateType.GetInvokeMethod();
			Type[] types = invoke.GetParameterTypes();
			Type returnType = invoke.ReturnType;
			DynamicMethod dlgMethod = new DynamicMethod("FieldDelegate", returnType, types, field.Module, true);
			ILGenerator il = dlgMethod.GetILGenerator();
			int index = 0;
			if (!field.IsStatic)
			{
				if (!il.EmitFieldInstance(field, types))
				{
					return null;
				}
				index++;
			}
			if (il.EmitAccessField(field, types, returnType, index))
			{
				return dlgMethod.CreateDelegate(delegateType);
			}
			return null;
		}
		/// <summary>
		/// 加载字段的实例。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="field">要加载实例的字段。</param>
		/// <param name="types">实参类型列表。</param>
		/// <returns>如果成功加载字段的实例，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool EmitFieldInstance(this ILGenerator il, FieldInfo field, Type[] types)
		{
			if (types.Length == 0)
			{
				return false;
			}
			Type instanceType = types[0];
			Contract.Assume(instanceType != null);
			if (!field.DeclaringType.IsExplicitFrom(instanceType))
			{
				return false;
			}
			il.EmitLoadInstance(field, instanceType, true);
			return true;
		}
		/// <summary>
		/// 写入访问字段的指令。
		/// </summary>
		/// <param name="il">IL 指令生成器。</param>
		/// <param name="field">要访问的字段。</param>
		/// <param name="types">实参类型列表。</param>
		/// <param name="returnType">返回值类型。</param>
		/// <param name="index">字段值的实参索引。</param>
		/// <returns>如果成功写入指令，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool EmitAccessField(this ILGenerator il, FieldInfo field, Type[] types, Type returnType, int index)
		{
			Contract.Requires(il != null && field != null && types != null && returnType != null);
			Type fieldType = field.FieldType;
			if (returnType == typeof(void))
			{
				// 设置字段值。
				if (types.Length != index + 1)
				{
					return false;
				}
				Type valueType = types[index];
				Contract.Assume(valueType != null);
				if (!fieldType.IsExplicitFrom(valueType))
				{
					return false;
				}
				il.EmitLoadParameter(fieldType, index, valueType);
				il.EmitStoreField(field);
			}
			else
			{
				// 获取字段值。
				if (types.Length != index || !returnType.IsExplicitFrom(fieldType))
				{
					return false;
				}
				il.EmitLoadField(field);
			}
			il.Emit(OpCodes.Ret);
			return true;
		}

		#endregion // 构造开放字段委托

		#region 构造封闭字段委托

		/// <summary>
		/// 使用指定的第一个参数，创建用于表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="firstArgument">如果是实例字段，则作为委托要绑定到的对象；否则将作为字段的第一个参数。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field, object firstArgument)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.Ensures(Contract.Result<TDelegate>() != null);
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateClosedDelegate(field, type, firstArgument);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建用于表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <typeparam name="TDelegate">要创建的委托的类型。</typeparam>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="firstArgument">如果是实例字段，则作为委托要绑定到的对象；否则将作为字段的第一个参数。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><typeparamref name="TDelegate"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static TDelegate CreateDelegate<TDelegate>(this FieldInfo field, object firstArgument,
			bool throwOnBindFailure)
			where TDelegate : class
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			Contract.EndContractBlock();
			Type type = typeof(TDelegate);
			CommonExceptions.CheckDelegateType(type);
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateClosedDelegate(field, type, firstArgument);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg as TDelegate;
		}
		/// <summary>
		/// 使用指定的第一个参数，创建用于表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例字段，则作为委托要绑定到的对象；否则将作为字段的第一个参数。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(this FieldInfo field, Type delegateType, object firstArgument)
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.Ensures(Contract.Result<Delegate>() != null);
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateClosedDelegate(field, delegateType, firstArgument);
			if (dlg == null)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg;
		}
		/// <summary>
		/// 使用指定的第一个参数和针对绑定失败的指定行为，创建用于表示获取或设置指定的静态或实例字段的指定类型的委托。 
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="field">描述委托要表示的静态或实例字段的 <see cref="FieldInfo"/>。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例字段，则作为委托要绑定到的对象；否则将作为字段的第一个参数。</param>
		/// <param name="throwOnBindFailure">为 <c>true</c>，表示无法绑定 <paramref name="field"/> 
		/// 时引发异常；否则为 <c>false</c>。</param>
		/// <returns>指定类型的委托，表示获取或设置指定的静态或实例字段。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="field"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentNullException"><paramref name="delegateType"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="delegateType"/> 不是委托类型。</exception>
		/// <exception cref="ArgumentException">无法绑定 <paramref name="field"/> 
		/// 且 <paramref name="throwOnBindFailure"/> 为 <c>true</c>。</exception>
		/// <exception cref="MethodAccessException">调用方无权访问 <paramref name="field"/>。</exception>
		public static Delegate CreateDelegate(this FieldInfo field, Type delegateType, object firstArgument, 
			bool throwOnBindFailure)
		{
			CommonExceptions.CheckArgumentNull(field, "field");
			CommonExceptions.CheckArgumentNull(delegateType, "delegateType");
			Contract.EndContractBlock();
			CommonExceptions.CheckDelegateType(delegateType, "delegateType");
			CommonExceptions.CheckUnboundGenParam(field, "field");
			Delegate dlg = CreateClosedDelegate(field, delegateType, firstArgument);
			if (dlg == null && throwOnBindFailure)
			{
				throw CommonExceptions.BindTargetField("field");
			}
			return dlg;
		}
		/// <summary>
		/// 创建指定的静态或实例字段的指定类型的封闭字段委托。
		/// 如果委托具有返回值，则认为是获取字段，否则认为是设置字段。
		/// 支持参数的强制类型转换，参数声明可以与实际类型不同。
		/// </summary>
		/// <param name="field">要获取或设置的字段。</param>
		/// <param name="delegateType">要创建的委托的类型。</param>
		/// <param name="firstArgument">如果是实例字段，则作为委托要绑定到的对象；否则将作为字段的第一个参数。</param>
		/// <returns><paramref name="delegateType"/> 类型的委托，表示静态或实例字段的委托。</returns>
		private static Delegate CreateClosedDelegate(FieldInfo field, Type delegateType, object firstArgument)
		{
			Contract.Requires(field != null && delegateType != null);
			if (firstArgument == null)
			{
				// 开放方法。
				Delegate dlg = CreateOpenDelegate(field, delegateType);
				if (dlg != null)
				{
					return dlg;
				}
			}
			// 提前对 firstArgument 进行类型转换。
			Type firstArgType = field.IsStatic ? field.FieldType : field.DeclaringType;
			Contract.Assume(firstArgType != null);
			if (firstArgument != null)
			{
				if (!firstArgType.IsExplicitFrom(firstArgument.GetType()))
				{
					return null;
				}
				firstArgument = Convert.ChangeType(firstArgument, firstArgType);
			}
			else if (firstArgType.IsValueType)
			{
				return null;
			}
			MethodInfo invoke = delegateType.GetInvokeMethod();
			Type[] types = invoke.GetParameterTypes();
			Type returnType = invoke.ReturnType;
			bool needLoadFirstArg = false;
			if (!ILExt.CanEmitConstant(firstArgument))
			{
				// 需要添加作为实例的形参。
				needLoadFirstArg = true;
				// CreateDelegate 方法传入的 firstArgument 不能为值类型，除非形参类型是 object。
				types = types.Insert(0, firstArgType.IsValueType ? typeof(object) : firstArgType);
			}
			DynamicMethod dlgMethod = new DynamicMethod("FieldDelegate", returnType, types, field.Module, true);
			ILGenerator il = dlgMethod.GetILGenerator();
			if (needLoadFirstArg)
			{
				// 需要传入第一个参数。
				int index = 0;
				if (!field.IsStatic)
				{
					if (!il.EmitFieldInstance(field, types))
					{
						return null;
					}
					index++;
				}
				if (il.EmitAccessField(field, types, returnType, index))
				{
					return dlgMethod.CreateDelegate(delegateType, firstArgument);
				}
				return null;
			}
			// 不需要传入第一个参数。
			if (field.IsStatic)
			{
				if (returnType != typeof(void))
				{
					// firstArgument 非 null 的情况下，获取字段的值时参数不兼容。
					return null;
				}
				// 设置字段值。
				if (types.Length != 0)
				{
					return null;
				}
				il.EmitConstant(firstArgument);
				il.EmitStoreField(field);
				il.Emit(OpCodes.Ret);
			}
			else
			{
				il.EmitLoadInstance(field, firstArgument);
				if (!il.EmitAccessField(field, types, returnType, 0))
				{
					return null;
				}
			}
			return dlgMethod.CreateDelegate(delegateType);
		}

		#endregion // 构造封闭字段委托

	}
}
