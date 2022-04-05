using System.Diagnostics;

namespace Cyjb
{
	/// <summary>
	/// 用于需要获取唯一值的情况。
	/// </summary>
	/// <typeparam name="TValue">唯一值的类型。</typeparam>
	public class UniqueValue<TValue>
	{
		/// <summary>
		/// 要获取的唯一值。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TValue? uniqueValue;
		/// <summary>
		/// 获取的值是否是唯一的。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool? isUnique;
		/// <summary>
		/// 值相等的比较器。
		/// </summary>
		private readonly IEqualityComparer<TValue> comparer;

		/// <summary>
		/// 初始化 <see cref="UniqueValue{TValue}"/> 类的新实例。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 初始化 <see cref="UniqueValue{TValue}"/> 类的新实例。
		/// </summary>
		/// </overloads>
		public UniqueValue()
		{
			comparer = EqualityComparer<TValue>.Default;
		}
		/// <summary>
		/// 使用指定的比较器初始化 <see cref="UniqueValue{TValue}"/> 类的新实例。
		/// </summary>
		/// <param name="comparer">值相等的比较器。</param>
		public UniqueValue(IEqualityComparer<TValue> comparer)
		{
			this.comparer = comparer;
		}
		/// <summary>
		/// 使用指定的初始值初始化 <see cref="UniqueValue{TValue}"/> 类的新实例。
		/// </summary>
		/// <param name="value">初始的设定值。</param>
		public UniqueValue(TValue value)
			: this()
		{
			uniqueValue = value;
			isUnique = true;
		}
		/// <summary>
		/// 使用指定的初始值和比较器初始化 <see cref="UniqueValue{TValue}"/> 类的新实例。
		/// </summary>
		/// <param name="value">初始的设定值。</param>
		/// <param name="comparer">值相等的比较器。</param>
		public UniqueValue(TValue value, IEqualityComparer<TValue> comparer)
			: this(comparer)
		{
			uniqueValue = value;
			isUnique = true;
		}

		/// <summary>
		/// 获取或设置唯一的值。
		/// </summary>
		/// <value>如果值是唯一的，则为唯一的值；否则返回 <typeparamref name="TValue"/> 的默认值。</value>
		public TValue? Value
		{
			get { return uniqueValue; }
			set
			{
				if (isUnique == null)
				{
					uniqueValue = value;
					isUnique = true;
				}
				else if (isUnique == true && !comparer.Equals(Value, value))
				{
					isUnique = false;
					uniqueValue = default;
				}
			}
		}

		/// <summary>
		/// 获取被设置的值是否是唯一的。
		/// </summary>
		/// <value>如果值被设置了，而且是唯一的，则为 <c>true</c>；
		/// 如果值未被设置，或者不唯一，则为 <c>false</c>。</value>
		public bool IsUnique
		{
			get { return isUnique == true; }
		}

		/// <summary>
		/// 获取被设置的值是否是冲突的。
		/// </summary>
		/// <value>如果值被设置了，而且存在冲突，则为 <c>true</c>；
		/// 如果值未被设置，或者值唯一，则为 <c>false</c>。</value>
		public bool IsAmbig
		{
			get { return isUnique == false; }
		}

		/// <summary>
		/// 获取是否还未设置值。
		/// </summary>
		/// <value>如果值已被设置，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsEmpty
		{
			get { return isUnique == null; }
		}

		/// <summary>
		/// 将值重置为未设置状态。
		/// </summary>
		public void Reset()
		{
			isUnique = null;
			uniqueValue = default;
		}

		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return isUnique switch
			{
				true => ResourcesUtil.Format(Resources.UniqueValue_Unique, uniqueValue),
				false => Resources.UniqueValue_Ambig,
				_ => Resources.UniqueValue_Empty,
			};
		}
	}
}
