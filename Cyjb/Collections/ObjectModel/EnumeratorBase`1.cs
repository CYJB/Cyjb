using System.Collections;

namespace Cyjb.Collections.ObjectModel;

/// <summary>
/// 为枚举器提供基类。
/// </summary>
/// <typeparam name="T">枚举的元素类型。</typeparam>
public abstract class EnumeratorBase<T> : IEnumerator<T>, IEnumerator
{
	/// <summary>
	/// 枚举器的状态。
	/// </summary>
	private enum EnumeratorStatus
	{
		/// <summary>
		/// 初始状态。
		/// </summary>
		Initial,
		/// <summary>
		/// 已准备好枚举值。
		/// </summary>
		Ready,
		/// <summary>
		/// 枚举器已被释放。
		/// </summary>
		Disposed
	}

	/// <summary>
	/// 枚举器的状态。
	/// </summary>
	private EnumeratorStatus status = EnumeratorStatus.Initial;
	/// <summary>
	/// 当前元素。
	/// </summary>
	private T current = default!;

	/// <summary>
	/// 使用指定的堆栈实例初始化 <see cref="EnumeratorBase{T}"/> 类的新实例。
	/// </summary>
	protected EnumeratorBase() { }

	/// <summary>
	/// 检查容器版本是否发生了变化。
	/// </summary>
	/// <returns>如果容器版本发生了变化，则为 <c>true</c>；否则返回 <c>false</c>。</returns>
	protected abstract bool CheckVersionChanged();

	/// <summary>
	/// 将枚举数推进到集合的下一个元素。
	/// </summary>
	/// <param name="initial">当前是否是首次调用。</param>
	/// <param name="current">集合的下一个元素。</param>
	/// <returns>如果枚举数已成功地推进到下一个元素，则为 <c>true</c>；
	/// 如果枚举数传递到集合的末尾，则为 <c>false</c>。</returns>
	protected abstract bool MoveNext(bool initial, out T current);

	/// <summary>
	/// 获取集合中位于枚举数当前位置的元素。
	/// </summary>
	public T Current => status switch
	{
		EnumeratorStatus.Initial => throw new InvalidOperationException(Resources.EnumNotStarted),
		EnumeratorStatus.Disposed => throw new InvalidOperationException(Resources.EnumEnded),
		_ => current,
	};

	/// <summary>
	/// 获取集合中位于枚举数当前位置的元素。
	/// </summary>
	object? IEnumerator.Current => Current;

	/// <summary>
	/// 将枚举数推进到集合的下一个元素。
	/// </summary>
	/// <returns>如果枚举数已成功地推进到下一个元素，则为 <c>true</c>；
	/// 如果枚举数传递到集合的末尾，则为 <c>false</c>。</returns>
	/// <exception cref="InvalidOperationException">枚举过程中对集合进行了更改。</exception>
	public bool MoveNext()
	{
		if (CheckVersionChanged())
		{
			throw new InvalidOperationException(Resources.EnumFailedVersion);
		}
		switch (status)
		{
			case EnumeratorStatus.Initial:
				status = EnumeratorStatus.Ready;
				return MoveNext(true, out current);
			case EnumeratorStatus.Ready:
				return MoveNext(false, out current);
			default:
				return false;
		}
	}

	/// <summary>
	/// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
	/// </summary>
	/// <exception cref="InvalidOperationException">枚举过程中对集合进行了更改。</exception>
	void IEnumerator.Reset()
	{
		if (CheckVersionChanged())
		{
			throw new InvalidOperationException(Resources.EnumFailedVersion);
		}
		status = EnumeratorStatus.Initial;
		current = default!;
	}

	#region IDisposable 成员

	/// <summary>
	/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
	/// </summary>
	/// <overloads>
	/// <summary>
	/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
	/// </summary>
	/// </overloads>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
	/// </summary>
	/// <param name="disposing">是否释放托管资源。</param>
	protected virtual void Dispose(bool disposing)
	{
		if (status == EnumeratorStatus.Disposed || !disposing)
		{
			return;
		}
		status = EnumeratorStatus.Disposed;
		current = default!;
	}

	#endregion // IDisposable 成员

}
