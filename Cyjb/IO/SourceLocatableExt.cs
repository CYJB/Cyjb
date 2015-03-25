using System;
using System.Diagnostics.Contracts;

namespace Cyjb.IO
{
	/// <summary>
	/// 提供可在源文件中定位的对象的扩展方法。
	/// </summary>
	public static class SourceLocatableExt
	{
		/// <summary>
		/// 获取指定对象在源文件中的字符长度。
		/// </summary>
		/// <param name="locatable">要检查的对象。</param>
		/// <returns>指定对象在源文件中的字符长度。</returns>
		public static int Length(this ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			return locatable.IsUnknown() ? 0 : locatable.End.Index - locatable.Start.Index + 1;
		}
		/// <summary>
		/// 获取指定对象是否表示未知范围。
		/// </summary>
		/// <param name="locatable">要检查的对象。</param>
		/// <returns>如果指定对象表示未知范围，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsUnknown(this ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			return locatable.Start == SourcePosition.Unknown || locatable.End == SourcePosition.Unknown;
		}

		#region 范围比较

		/// <summary>
		/// 返回指定的 <see cref="ISourceLocatable"/> 是否完全包含在当前范围中。
		/// </summary>
		/// <param name="thisObj">当前范围。</param>
		/// <param name="locatable">要检查的范围。</param>
		/// <returns>如果指定的范围完全包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="thisObj"/> 或 
		/// <paramref name="locatable"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 返回指定的范围或位置是否完全包含在当前范围中。
		/// </summary>
		/// </overloads>
		public static bool Contains(this ISourceLocatable thisObj, ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(thisObj, "thisObj");
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			return (!thisObj.IsUnknown()) && thisObj.Start <= locatable.Start && thisObj.End >= locatable.End;
		}
		/// <summary>
		/// 返回指定的位置是否完全包含在当前范围中。
		/// </summary>
		/// <param name="thisObj">当前范围。</param>
		/// <param name="location">要检查的位置。</param>
		/// <returns>如果指定的位置包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="thisObj"/> 为 <c>null</c>。</exception>
		public static bool Contains(this ISourceLocatable thisObj, SourcePosition location)
		{
			CommonExceptions.CheckArgumentNull(thisObj, "thisObj");
			Contract.EndContractBlock();
			if (location.IsUnknown || thisObj.IsUnknown())
			{
				return false;
			}
			return thisObj.End.Index >= location.Index && thisObj.Start.Index <= location.Index;
		}
		/// <summary>
		/// 返回指定的索引是否完全包含在当前范围中。
		/// </summary>
		/// <param name="thisObj">当前范围。</param>
		/// <param name="index">要检查的索引。</param>
		/// <returns>如果指定的索引包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="thisObj"/> 为 <c>null</c>。</exception>
		public static bool Contains(this ISourceLocatable thisObj, int index)
		{
			CommonExceptions.CheckArgumentNull(thisObj, "thisObj");
			Contract.EndContractBlock();
			if (index < 0 || thisObj.IsUnknown())
			{
				return false;
			}
			return thisObj.End.Index >= index && thisObj.Start.Index <= index;
		}
		/// <summary>
		/// 返回指定的行列位置是否完全包含在当前范围中。
		/// </summary>
		/// <param name="thisObj">当前范围。</param>
		/// <param name="line">要检查的行。</param>
		/// <param name="col">要检查的列。</param>
		/// <returns>如果指定的行列位置包含在当前范围中，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="thisObj"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="line"/> 或 <paramref name="col"/> 
		/// 小于 <c>0</c>。</exception>
		public static bool Contains(this ISourceLocatable thisObj, int line, int col)
		{
			CommonExceptions.CheckArgumentNull(thisObj, "thisObj");
			Contract.EndContractBlock();
			if (line < 1 || col < 1 || thisObj.IsUnknown())
			{
				return false;
			}
			if (thisObj.Start.Line == thisObj.End.Line)
			{
				return thisObj.Start.Line == line && thisObj.Start.Col <= col && thisObj.End.Col >= col;
			}
			if (thisObj.Start.Line == line)
			{
				return thisObj.Start.Col <= col;
			}
			if (thisObj.End.Line == line)
			{
				return thisObj.End.Col >= col;
			}
			return true;
		}
		/// <summary>
		/// 返回指定的 <see cref="ISourceLocatable"/> 是否与当前范围存在重叠。
		/// </summary>
		/// <param name="thisObj">当前范围。</param>
		/// <param name="locatable">要检查的范围。</param>
		/// <returns>如果指定的范围与当前范围存在重叠，则为 <c>true</c>；否则为 <c>false</c>。
		/// 对于未知的范围，也会返回 <c>false</c>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="thisObj"/> 或 
		/// <paramref name="locatable"/> 为 <c>null</c>。</exception>
		public static bool OverlapsWith(this ISourceLocatable thisObj, ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(thisObj, "thisObj");
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			return (!thisObj.IsUnknown()) && thisObj.Start <= locatable.End && thisObj.End >= locatable.Start;
		}
		/// <summary>
		/// 返回当前范围与指定 <see cref="ISourceLocatable"/> 的重叠范围，如果不存在则为 
		/// <see cref="SourceRange.Unknown"/>。
		/// </summary>
		/// <param name="thisObj">当前范围。</param>
		/// <param name="locatable">要检查的范围。</param>
		/// <returns>当前范围与指定范围重叠的部分，如果不存在则为 
		/// <see cref="SourceRange.Unknown"/>。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="thisObj"/> 或 
		/// <paramref name="locatable"/> 为 <c>null</c>。</exception>>
		public static SourceRange Overlap(this ISourceLocatable thisObj, ISourceLocatable locatable)
		{
			CommonExceptions.CheckArgumentNull(thisObj, "thisObj");
			CommonExceptions.CheckArgumentNull(locatable, "locatable");
			Contract.EndContractBlock();
			SourcePosition maxStart = thisObj.Start > locatable.Start ? thisObj.Start : locatable.Start;
			SourcePosition minEnd = thisObj.End < locatable.End ? thisObj.End : locatable.End;
			if (maxStart == SourcePosition.Unknown || maxStart > minEnd)
			{
				return SourceRange.Unknown;
			}
			return new SourceRange(maxStart, minEnd);
		}

		#endregion // 范围比较

	}
}
