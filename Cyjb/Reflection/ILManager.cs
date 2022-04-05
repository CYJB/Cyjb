using System.Diagnostics;
using System.Reflection.Emit;

namespace Cyjb.Reflection
{
	/// <summary>
	/// 表示 IL 指令的管理器，提供局部变量管理、闭包等功能。
	/// </summary>
	internal class ILManager
	{
		/// <summary>
		/// IL 的指令生成器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ILGenerator il;
		/// <summary>
		/// 局部变量列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<Type, Stack<LocalBuilder>> locals = new();
		/// <summary>
		/// 闭包列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<object> closure = new();

		/// <summary>
		/// 使用指定的 IL 指令生成器初始化 <see cref="ILManager"/> 类的新实例。
		/// </summary>
		/// <param name="il">使用的 IL 指令生成器。</param>
		public ILManager(ILGenerator il)
		{
			this.il = il;
		}

		/// <summary>
		/// 返回可用的指定类型的局部变量。
		/// </summary>
		/// <param name="type">局部变量的类型。</param>
		/// <returns>可用的局部变量。</returns>
		public LocalBuilder GetLocal(Type type)
		{
			if (locals.TryGetValue(type, out Stack<LocalBuilder>? localStack))
			{
				return localStack.Pop();
			}
			return il.DeclareLocal(type);
		}

		/// <summary>
		/// 释放指定的局部变量，该局部变量之后可以被重复利用。
		/// </summary>
		/// <param name="local">要释放的局部变量。</param>
		public void FreeLocal(LocalBuilder local)
		{
			if (!locals.TryGetValue(local.LocalType, out Stack<LocalBuilder>? localStack))
			{
				localStack = new Stack<LocalBuilder>(2);
				locals.Add(local.LocalType, localStack);
			}
			localStack.Push(local);
		}

		/// <summary>
		/// 返回指定闭包值的索引。
		/// </summary>
		/// <param name="value">要使用的闭包值。</param>
		/// <returns>指定闭包值的索引。</returns>
		public int GetClosure(object value)
		{
			int idx = closure.IndexOf(value);
			if (idx < 0)
			{
				idx = closure.Count;
				closure.Add(value);
			}
			return idx;
		}

		/// <summary>
		/// 返回当前的闭包。
		/// </summary>
		/// <returns>回当前的闭包，如果不需要则为 <c>null</c>。</returns>
		public object? GetClosure()
		{
			if (closure.Count == 0)
			{
				return null;
			}
			else
			{
				return new Closure(closure.ToArray());
			}
		}
	}
}
