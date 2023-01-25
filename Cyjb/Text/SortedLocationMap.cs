namespace Cyjb.Text;

/// <summary>
/// 提供将位置线性映射到其它范围的能力。
/// </summary>
/// <remarks>假设会按照从小到大的顺序映射位置。</remarks>
public class SortedLocationMap : LocationMap
{
	/// <summary>
	/// 当前映射。
	/// </summary>
	private MapItem curMap;
	/// <summary>
	/// 下一个映射的索引。
	/// </summary>
	private int nextIndex;
	/// <summary>
	/// 下一个映射的索引。
	/// </summary>
	private int nextMapIndex = 0;

	/// <summary>
	/// 使用指定的位置映射关系初始化 <see cref="Text.SortedLocationMap"/> 类的新实例。
	/// </summary>
	/// <param name="map">位置映射关系，会将 <see cref="Tuple{T1, T2}.Item1"/> 映射为
	/// <see cref="Tuple{T1, T2}.Item2"/>。未列出的值会根据之前最近的映射关系线性变换。</param>
	/// <example>假设映射关系是 <c>{0, 10}</c>，会将索引 <c>0</c> 映射到 <c>10</c>，
	/// 索引 <c>1</c> 映射到 <c>11</c>，索引 <c>10</c> 映射到 <c>20</c>，并依此类推。</example>
	public SortedLocationMap(IEnumerable<Tuple<int, int>> map) : base(map)
	{
		if (this.map.Count == 0)
		{
			curMap = new MapItem(new Tuple<int, int>(0, 0), int.MaxValue);
		}
		else
		{
			curMap = this.map[0];
		}
		FindNextMap();
	}

	/// <summary>
	/// 映射指定的位置。
	/// </summary>
	/// <param name="location">要映射的位置。</param>
	/// <returns>映射后的位置。</returns>
	public override int MapLocation(int location)
	{
		// 在首个映射之前，什么都不做。
		if (location < curMap.Index)
		{
			return location;
		}
		// 在当前映射范围之外，需要切换到下一映射。
		while (location >= nextIndex)
		{
			curMap = map[nextMapIndex];
			FindNextMap();
		}
		return curMap.MapLocation(location);
	}

	/// <summary>
	/// 寻找下一个索引。
	/// </summary>
	private void FindNextMap()
	{
		nextMapIndex++;
		if (nextMapIndex < map.Count)
		{
			nextIndex = map[nextMapIndex].Index;
		}
		else
		{
			nextIndex = int.MaxValue;
		}
	}
}
