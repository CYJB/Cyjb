//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由工具生成。
//
// 对此文件的更改可能会导致不正确的行为，并且如果
// 重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cyjb.Test;

using ComponentModel = global::System.ComponentModel;
using ResourceManager = global::System.Resources.ResourceManager;
using CultureInfo = global::System.Globalization.CultureInfo;

/// <summary>
/// 一个强类型的资源类，用于查找本地化的字符串等。
/// </summary>
/// <remarks>此类是由 T4 文本模板通过 Visual Studio 的工具自动生成的。
/// 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 T4 模板。
/// </remarks>
[global::System.Diagnostics.DebuggerNonUserCodeAttribute]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
internal partial class Resources
{
	#nullable enable

	private static ResourceManager? resourceManager;
	private static CultureInfo? resourceCulture;

	/// <summary>
	/// 获取此类使用的缓存的 <see cref="ResourceManager"/> 实例。
	/// </summary>
	[ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceManager is null)
			{
				resourceManager = new ResourceManager("Cyjb.Test.Resources", typeof(Resources).Assembly);
			}
			return resourceManager;
		}
	}

	/// <summary>
	/// 获取或设置资源使用的区域信息。
	/// </summary>
	[ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)]
	internal static CultureInfo? Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}
	
	/// <summary>
	/// 返回类似 <c>Both collection contain same elements.</c> 的本地化字符串。
	/// </summary>
	internal static string BothCollectionsSameElements => ResourceManager.GetString("BothCollectionsSameElements", resourceCulture)!;
	
	/// <summary>
	/// 返回类似 <c>Both collection references point to the same collection object.</c> 的本地化字符串。
	/// </summary>
	internal static string BothCollectionsSameReference => ResourceManager.GetString("BothCollectionsSameReference", resourceCulture)!;
	
	/// <summary>
	/// 返回类似 <c>The expected collection contains {1} occurrence(s) of &lt;{2}&gt;. The actual collection contains {3} occurrence(s).</c> 的本地化字符串。
	/// </summary>
	internal static string CollectionsHasMismatchedElements(object? arg0, object? arg1, object? arg2)
	{
		return string.Format(resourceCulture, ResourceManager.GetString("CollectionsHasMismatchedElements", resourceCulture)!, Format(arg0), Format(arg1), Format(arg2));
	}
	
	/// <summary>
	/// 返回类似 <c>Element at index {0} do not match.</c> 的本地化字符串。
	/// </summary>
	internal static string ElementsAtIndexDontMatch(object? arg0)
	{
		return string.Format(resourceCulture, ResourceManager.GetString("ElementsAtIndexDontMatch", resourceCulture)!, Format(arg0));
	}
	
	/// <summary>
	/// 返回类似 <c>Different number of elements.</c> 的本地化字符串。
	/// </summary>
	internal static string NumberOfElementsDiff => ResourceManager.GetString("NumberOfElementsDiff", resourceCulture)!;
	
	/// <summary>
	/// 将指定对象格式化为字符串。
	/// </summary>
	/// <param name="value">要格式化的对象。</param>
	private static object Format(object? value)
	{
		if (value == null)
		{
			return "(null)";
		}
		return value;
	}

	#nullable restore

}


