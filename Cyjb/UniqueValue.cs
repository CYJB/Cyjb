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
		private TValue uniqueValue;
		/// <summary>
		/// 获取的值是否是唯一的。
		/// </summary>
		private Tristate isUnique = Tristate.NotSure;
		/// <summary>
		/// 获取或设置唯一的值。
		/// 如果值未设置，则返回值是不可预料的。
		/// 如果值是重复的，则为第一次设置的值。
		/// </summary>
		public TValue Value
		{
			get { return uniqueValue; }
			set
			{
				if (isUnique == Tristate.NotSure)
				{
					uniqueValue = value;
					isUnique = Tristate.True;
				}
				else
				{
					isUnique = Tristate.False;
				}
			}
		}
		/// <summary>
		/// 获取设置的值是否是唯一的。
		/// </summary>
		public bool IsUnique
		{
			get { return isUnique == Tristate.True; }
		}
		/// <summary>
		/// 获取设置的值是否是冲突的。
		/// </summary>
		public bool IsAmbig
		{
			get { return isUnique == Tristate.False; }
		}
		/// <summary>
		/// 获取是否还为设置值。
		/// </summary>
		public bool IsEmpty
		{
			get { return isUnique == Tristate.NotSure; }
		}
		/// <summary>
		/// 将值重置为未设置状态。
		/// </summary>
		public void Reset()
		{
			isUnique = Tristate.NotSure;
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			switch (isUnique)
			{
				case Tristate.True:
					return string.Concat("[Unique ", uniqueValue, "]");
				case Tristate.False:
					return "[Ambig]";
				default:
					return "[Empty]";
			}
		}
	}
}
