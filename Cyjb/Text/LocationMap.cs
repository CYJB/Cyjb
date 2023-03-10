namespace Cyjb.Text;

/// <summary>
/// 提供将位置线性映射到其它范围的能力。
/// </summary>
public sealed class LocationMap
{
	/// <summary>
	/// 映射的索引。
	/// </summary>
	private readonly List<int> indexMap = new();
	/// <summary>
	/// 映射关系。
	/// </summary>
	private readonly List<MapItem> map = new();
	/// <summary>
	/// 当前映射所在的索引。
	/// </summary>
	private int mapIndex = 0;
	/// <summary>
	/// 当前映射。
	/// </summary>
	private MapItem curMap;
	/// <summary>
	/// 当前映射的索引。
	/// </summary>
	private int curIndex = 0;
	/// <summary>
	/// 下一个映射的索引。
	/// </summary>
	private int nextIndex;

	/// <summary>
	/// 使用指定的位置映射关系初始化 <see cref="Text.LocationMap"/> 类的新实例。
	/// </summary>
	/// <param name="map">位置映射关系，会将 <see cref="Tuple{T1, T2}.Item1"/> 映射为
	/// <see cref="Tuple{T1, T2}.Item2"/>。未列出的值会根据之前最近的映射关系线性变换。</param>
	/// <example>假设映射关系是 <c>{0, 10}</c>，会将索引 <c>0</c> 映射到 <c>10</c>，
	/// 索引 <c>1</c> 映射到 <c>11</c>，索引 <c>10</c> 映射到 <c>20</c>，并依此类推。</example>
	public LocationMap(IEnumerable<Tuple<int, int>> map)
	{
		if (map.Any())
		{
			Tuple<int, int> last = map.OrderBy(tuple => tuple.Item1).Aggregate((cur, next) =>
			{
				// 合并连续的映射。
				if (next.Item1 - cur.Item1 == next.Item2 - cur.Item2)
				{
					return cur;
				}
				else
				{
					// 添加当前映射。
					indexMap.Add(cur.Item1);
					this.map.Add(new MapItem(cur, next.Item2));
					return next;
				}
			});
			// 添加最后一个映射。
			indexMap.Add(last.Item1);
			this.map.Add(new MapItem(last, int.MaxValue));
			curMap = this.map[0];
			curIndex = indexMap[0];
		}
		else
		{
			curMap = new MapItem(new Tuple<int, int>(0, 0), int.MaxValue);
		}
		FindNextMap();
	}

	/// <summary>
	/// 映射指定的位置。
	/// </summary>
	/// <param name="location">要映射的位置。</param>
	/// <returns>映射后的位置。</returns>
	public int MapLocation(int location)
	{
		if (location >= curIndex)
		{
			// 在当前映射范围之后，需要切换到下一映射。
			while (location >= nextIndex)
			{
				mapIndex++;
				curIndex = indexMap[mapIndex];
				curMap = map[mapIndex];
				FindNextMap();
			}
			return curMap.MapLocation(location);
		}
		// 在当前映射范围之前，二分查找。
		int idx = indexMap.BinarySearch(0, mapIndex, location, Comparer<int>.Default);
		if (idx < 0)
		{
			idx = ~idx;
			if (idx == 0)
			{
				// 在首个索引之前，什么都不做。
				return location;
			}
			idx--;
		}
		return map[idx].MapLocation(location);
	}

	/// <summary>
	/// 寻找下一个索引。
	/// </summary>
	private void FindNextMap()
	{
		if ((mapIndex + 1) < indexMap.Count)
		{
			nextIndex = indexMap[mapIndex + 1];
		}
		else
		{
			nextIndex = int.MaxValue;
		}
	}

	/// <summary>
	/// 位置映射。
	/// </summary>
	private readonly struct MapItem
	{
		/// <summary>
		/// 当前映射的偏移。
		/// </summary>
		private readonly int offset;
		/// <summary>
		/// 是否直接映射到 offset。
		/// </summary>
		private readonly bool mapToOffset;
		/// <summary>
		/// 当前映射结果的结束索引。
		/// </summary>
		private readonly int endIndex;

		/// <summary>
		/// 使用指定的映射和结束位置初始化 <see cref="MapItem"/> 结构的新实例。
		/// </summary>
		/// <param name="curMap">当前映射。</param>
		/// <param name="nextMapIndex">下一个映射结果的结束位置。</param>
		public MapItem(Tuple<int, int> curMap, int nextMapIndex)
		{
			endIndex = nextMapIndex - 1;
			if (curMap.Item2 >= nextMapIndex)
			{
				// 当前映射的起始位置已经超过了结束位置，那么总是直接映射到起始位置。
				offset = curMap.Item2;
				mapToOffset = true;
			}
			else
			{
				offset = curMap.Item2 - curMap.Item1;
				mapToOffset = false;
			}
		}

		/// <summary>
		/// 映射指定的位置。
		/// </summary>
		/// <param name="location">要映射的位置。</param>
		/// <returns>映射后的位置。</returns>
		public int MapLocation(int location)
		{
			if (mapToOffset)
			{
				return offset;
			}
			else
			{
				location += offset;
				// 避免超出结束位置。
				if (location > endIndex)
				{
					location = endIndex;
				}
				return location;
			}
		}
	}
}