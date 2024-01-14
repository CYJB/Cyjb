namespace Cyjb.Text;

/// <summary>
/// 位置映射关系的类型。
/// </summary>
public enum LocationMapType
{
	/// <summary>
	/// 索引映射模式，映射关系的每一项都是索引。
	/// </summary>
	/// <remarks>
	/// <c>{1, 10}, {3, 20}</c> 表示索引 <c>1</c> 映射到 <c>10</c>；
	/// 索引 <c>3</c> 映射到 <c>20</c>。
	/// </remarks>
	Index,
	/// <summary>
	/// 偏移映射模式，映射关系的每一项都是偏移。
	/// </summary>
	/// <remarks>
	/// <c>{1, 10}, {3, 20}</c> 表示索引 <c>1</c> 映射到 <c>10</c>；
	/// 索引 <c>4</c>（偏移 <c>1 + 3</c>） 映射到 <c>30</c>（偏移 <c>10 + 20</c>）。
	/// </remarks>
	Offset,
}

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
	/// <param name="type">映射关系的类型。</param>
	public LocationMap(IEnumerable<Tuple<int, int>> map, LocationMapType type)
	{
		if (map.Any())
		{
			Tuple<int, int> last;
			if (type == LocationMapType.Index)
			{
				last = map.OrderBy(tuple => tuple.Item1).Aggregate((cur, next) =>
				{
					// 合并连续的映射。
					int index = cur.Item1;
					int mappedIndex = cur.Item2;
					int nextMappedIndex = next.Item2;
					if (next.Item1 - index == nextMappedIndex - mappedIndex)
					{
						return cur;
					}
					else
					{
						// 添加当前映射。
						indexMap.Add(index);
						this.map.Add(new MapItem(index, mappedIndex, nextMappedIndex));
						return next;
					}
				});
			}
			else
			{
				int offset = 0;
				last = map.Aggregate((cur, next) =>
				{
					var (nextIndex, nextMappedIndex) = next;
					// 合并连续的映射。
					if (nextIndex == nextMappedIndex)
					{
						offset += nextIndex;
						return cur;
					}
					else
					{
						// 添加当前映射。
						var (index, mappedIndex) = cur;
						nextIndex += index;
						nextMappedIndex += mappedIndex;
						if (offset > 0)
						{
							nextIndex += offset;
							nextMappedIndex += offset;
							offset = 0;
						}
						indexMap.Add(index);
						this.map.Add(new MapItem(index, mappedIndex, nextMappedIndex));
						return new Tuple<int, int>(nextIndex, nextMappedIndex);
					}
				});
			}
			// 添加最后一个映射。
			indexMap.Add(last.Item1);
			this.map.Add(new MapItem(last.Item1, last.Item2, int.MaxValue));
			curMap = this.map[0];
			curIndex = indexMap[0];
		}
		else
		{
			curMap = new MapItem(0, 0, int.MaxValue);
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
		/// <param name="index">当前索引。</param>
		/// <param name="mappedIndex">要映射到的索引。</param>
		/// <param name="nextMappedIndex">下一个映射结果的结束位置。</param>
		public MapItem(int index, int mappedIndex, int nextMappedIndex)
		{
			endIndex = nextMappedIndex - 1;
			if (mappedIndex >= nextMappedIndex)
			{
				// 当前映射的起始位置已经超过了结束位置，那么总是直接映射到起始位置。
				offset = mappedIndex;
				mapToOffset = true;
			}
			else
			{
				offset = mappedIndex - index;
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
