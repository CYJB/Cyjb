namespace Cyjb.Conversions
{
	/// <summary>
	/// 表示标识转换。
	/// </summary>
	internal class IdentityConversion : Conversion
	{
		/// <summary>
		/// <see cref="IdentityConversion"/> 的默认实例。
		/// </summary>
		public static readonly Conversion Default = new IdentityConversion(ConversionType.Identity);
		/// <summary>
		/// 隐式数值转换的实例。
		/// </summary>
		public static readonly Conversion ImplicitNumeric = new IdentityConversion(ConversionType.ImplicitNumeric);
		/// <summary>
		/// 显式数值转换的实例。
		/// </summary>
		public static readonly Conversion ExplicitNumeric = new IdentityConversion(ConversionType.ExplicitNumeric);
		/// <summary>
		/// 显式枚举转换的实例。
		/// </summary>
		public static readonly Conversion ExplicitEnum = new IdentityConversion(ConversionType.Enum);
		/// <summary>
		/// 隐式引用转换的实例。
		/// </summary>
		public static readonly Conversion ImplicitReference = new IdentityConversion(ConversionType.ImplicitReference);

		/// <summary>
		/// 获取是否需要写入 IL（一些类型转换并不需要写入 IL）。
		/// </summary>
		public override bool NeedEmit => false;

		/// <summary>
		/// 使用指定的转换类型初始化 <see cref="IdentityConversion"/> 类的新实例。
		/// </summary>
		/// <param name="conversionType">当前转换的类型。</param>
		private IdentityConversion(ConversionType conversionType) : base(conversionType) { }
	}
}
