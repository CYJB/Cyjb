namespace Cyjb.Test;

/// <summary>
/// 提供资源相关扩展。
/// </summary>
internal static class ResourcesUtil
{
	/// <summary>
	/// 格式化指定的异常信息。
	/// </summary>
	/// <param name="message">要格式化的异常信息。</param>
	/// <param name="args">格式化信息的参数。</param>
	/// <returns>格式化后的异常信息。</returns>
	internal static string Format(string message, params object?[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			args[i] = Format(args[i]);
		}
		return string.Format(Resources.Culture, message, args);
	}

	/// <summary>
	/// 将指定对象格式化为字符串。
	/// </summary>
	/// <param name="value">要格式化的对象。</param>
	private static string Format(object? value)
	{
		if (value == null)
		{
			return "(null)";
		}
		Type? type = value as Type;
		if (type != null && type.FullName != null)
		{
			return type.FullName;
		}
		return value.ToString() ?? "";
	}
}
