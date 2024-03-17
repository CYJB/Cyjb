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
	/// 初始化 <see cref="Text.LocationMap"/> 类的新实例。
	/// </summary>
	public LocationMap()
	{
		curMap = new MapItem(0, 0, int.MaxValue);
		nextIndex = int.MaxValue;
	}

	/// <summary>
	/// 使用指定的位置映射关系初始化 <see cref="Text.LocationMap"/> 类的新实例。
	/// </summary>
	/// <param name="map">位置映射关系，会将 <see cref="Tuple{T1, T2}.Item1"/> 映射为
	/// <see cref="Tuple{T1, T2}.Item2"/>。未列出的值会根据之前最近的映射关系线性变换。</param>
	/// <param name="type">映射关系的类型。</param>
	/// <exception cref="ArgumentOutOfRangeException">使用偏移映射模式时，偏移小于 <c>0</c>。</exception>
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
					var (nextOffset, nextMappedOffset) = next;
					if (nextOffset < 0)
					{
						throw CommonExceptions.ArgumentNegative(nextOffset, nameof(map));
					}
					// 合并连续的映射。
					if (nextOffset == nextMappedOffset)
					{
						offset += nextOffset;
						return cur;
					}
					else
					{
						// 添加当前映射。
						var (index, mappedIndex) = cur;
						int nextIndex = index + nextOffset;
						int nextMappedIndex = mappedIndex + nextMappedOffset;
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
	/// 添加指定的位置映射。
	/// </summary>
	/// <param name="location">要映射的位置。</param>
	/// <param name="mappedLocation">映射到的位置。</param>
	/// <remarks>从 <paramref name="location"/> 映射到 <paramref name="mappedLocation"/>。</remarks>
	public void Add(int location, int mappedLocation)
	{
		if (indexMap.Count == 0)
		{
			indexMap.Add(location);
			curMap = new MapItem(location, mappedLocation, int.MaxValue);
			map.Add(curMap);
			curIndex = location;
			nextIndex = int.MaxValue;
			return;
		}
		int idx = indexMap.BinarySearch(location);
		if (idx < 0)
		{
			// 插入新映射。
			idx = ~idx;
			MapItem newItem;
			if (idx > 0)
			{
				MapItem prev = map[idx - 1];
				newItem = new(location, mappedLocation, prev.NextMappedIndex);
				if (prev.Offset == newItem.Offset)
				{
					// 如果偏移相同，说明新位置映射与原有映射是连续的，不需要添加新映射。
					return;
				}
			}
			else
			{
				int nextMappedIndex = map[idx].GetMappedIndex(indexMap[idx]);
				newItem = new(location, mappedLocation, nextMappedIndex);
			}
			indexMap.Insert(idx, location);
			map.Insert(idx, newItem);
			if (idx <= mapIndex)
			{
				mapIndex++;
			}
		}
		else
		{
			// 修改现有映射。
			map[idx] = map[idx].UpdateMappedIndex(location, mappedLocation);
			if (mapIndex == idx)
			{
				curMap = map[idx];
			}
		}
		// 修正前一映射。
		if (idx > 0)
		{
			idx--;
			int index = indexMap[idx];
			map[idx] = map[idx].UpdateNextMappedIndex(index, mappedLocation);
			if (mapIndex == idx)
			{
				curMap = map[idx];
				nextIndex = location;
			}
		}
	}

	/// <summary>
	/// 添加指定的位置偏移映射。
	/// </summary>
	/// <param name="offset">要映射的偏移。</param>
	/// <param name="mappedOffset">映射到的偏移。</param>
	/// <remarks>假设已有的最后一项映射是从 <c>x</c> 映射到 <c>y</c>，那么 <see cref="AddOffset"/>
	/// 会添加一项从 <c>x + <paramref name="offset"/></c> 到
	/// <c>y + <paramref name="mappedOffset"/></c> 的映射。</remarks>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> 小于 <c>0</c>。</exception>
	public void AddOffset(int offset, int mappedOffset)
	{
		if (offset < 0)
		{
			throw CommonExceptions.ArgumentNegative(offset);
		}
		if (offset == mappedOffset)
		{
			// 相同偏移，不需要修改。
			return;
		}
		if (indexMap.Count == 0)
		{
			indexMap.Add(offset);
			curMap = new MapItem(offset, mappedOffset, int.MaxValue);
			map.Add(curMap);
			curIndex = offset;
			nextIndex = int.MaxValue;
			return;
		}
		int idx = indexMap.Count - 1;
		MapItem prev = map[idx];
		int prevLocation = indexMap[idx];
		int location = prevLocation + offset;
		int mappedLocation = prev.GetMappedIndex(prevLocation) + mappedOffset;
		if (offset == 0)
		{
			// 修改现有映射。
			map[idx] = prev.UpdateMappedIndex(location, mappedLocation);
			if (mapIndex == idx)
			{
				curMap = map[idx];
			}
		}
		else
		{
			// 插入新映射。
			indexMap.Add(location);
			map.Add(new MapItem(location, mappedLocation, int.MaxValue));
			// 修正前一映射。
			map[idx] = prev.UpdateNextMappedIndex(prevLocation, mappedLocation);
			if (mapIndex == idx)
			{
				curMap = map[idx];
				nextIndex = location;
			}
		}
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
		/// <param name="nextMappedIndex">下一个映射要映射到的索引。</param>
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
		/// 获取当前映射的偏移。
		/// </summary>
		public int Offset => offset;
		/// <summary>
		/// 获取下一个要映射到的索引。
		/// </summary>
		public int NextMappedIndex => endIndex + 1;

		/// <summary>
		/// 获取当前要映射到的索引。
		/// </summary>
		public int GetMappedIndex(int index)
		{
			if (mapToOffset)
			{
				return offset;
			}
			else
			{
				return index + offset;
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

		/// <summary>
		/// 更新当前映射要映射到的索引，并返回新的映射。
		/// </summary>
		/// <param name="index">当前映射的索引。</param>
		/// <param name="mappedIndex">要映射到的索引。</param>
		/// <returns>新的映射。</returns>
		public MapItem UpdateMappedIndex(int index, int mappedIndex)
		{
			int nextMappedIndex = endIndex + 1;
			return new MapItem(index, mappedIndex, nextMappedIndex);
		}

		/// <summary>
		/// 更新下一个映射要映射到的索引，并返回新的映射。
		/// </summary>
		/// <param name="index">当前映射的索引。</param>
		/// <param name="nextMappedIndex">下一个映射要映射到的索引。</param>
		/// <returns>新的映射。</returns>
		public MapItem UpdateNextMappedIndex(int index, int nextMappedIndex)
		{
			int mappedIndex = offset;
			if (!mapToOffset)
			{
				mappedIndex = offset + index;
			}
			return new MapItem(index, mappedIndex, nextMappedIndex);
		}
	}
}
