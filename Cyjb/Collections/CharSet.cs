using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Cyjb.Collections.ObjectModel;

namespace Cyjb.Collections
{
	/// <summary>
	/// 表示特定于字符的集合，可以按字母顺序遍历集合。
	/// </summary>
	/// <remarks>该类采用位示图判断字符是否存在。</remarks>
	[Serializable]
	public sealed class CharSet : SetBase<char>, ISerializable
	{

		#region 常量定义

		/// <summary>
		/// 顶层数组的长度。
		/// </summary>
		private const int TopLen = 16;
		/// <summary>
		/// 中间层数组的长度。
		/// </summary>
		private const int MidLen = 16;
		/// <summary>
		/// 底层数组的长度。
		/// </summary>
		private const int BtmLen = 8;
		/// <summary>
		/// 顶层数组索引的位移。
		/// </summary>
		private const int TopShift = 12;
		/// <summary>
		/// 中间层数组索引的位移。
		/// </summary>
		private const int MidShift = 8;
		/// <summary>
		/// 底层数组索引的位移。
		/// </summary>
		private const int BtmShift = 5;
		/// <summary>
		/// 中间层数组索引的掩码。
		/// </summary>
		private const int MidMask = 0xF;
		/// <summary>
		/// 底间层数组索引的掩码。
		/// </summary>
		private const int BtmMask = 7;
		/// <summary>
		/// Int32 索引的掩码。
		/// </summary>
		private const int IndexMask = 0x1F;

		#endregion // 常量定义

		/// <summary>
		/// EASCII 码的数据，0x00 ~ 0xFF。
		/// </summary>
		private uint[] eascii;
		/// <summary>
		/// 0x0000 ~ 0xFFFF 的数据。
		/// </summary>
		private uint[][][] data = null;
		/// <summary>
		/// 集合中元素的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int count;
		/// <summary>
		/// 底层数组的完整长度。
		/// </summary>
		private readonly int btmFullLen;
		/// <summary>
		/// 集合是否不区分大小写。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool ignoreCase;
		/// <summary>
		/// 判断字符大小写使用的区域信息。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CultureInfo culture;
		/// <summary>
		/// 获取字符索引的方法。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Func<char, int> getIndex;
		/// <summary>
		/// 初始化 <see cref="Cyjb.Collections.CharSet"/> 类的新实例。
		/// </summary>
		public CharSet()
			: this(false, null)
		{ }
		/// <summary>
		/// 使用指定的是否区分大小写初始化 
		/// <see cref="Cyjb.Collections.CharSet"/> 类的新实例。
		/// </summary>
		public CharSet(bool ignoreCase)
			: this(ignoreCase, null)
		{ }
		/// <summary>
		/// 使用指定的是否区分大小写和区域信息初始化 
		/// <see cref="Cyjb.Collections.CharSet"/> 类的新实例。
		/// </summary>
		/// <param name="ignoreCase">是否不区分字符的大小写。</param>
		/// <param name="culture">不区分字符大小写时使用的区域信息。</param>
		public CharSet(bool ignoreCase, CultureInfo culture)
			: base(null)
		{
			this.ignoreCase = ignoreCase;
			if (this.ignoreCase)
			{
				if (culture == null)
				{
					this.culture = CultureInfo.InvariantCulture;
				}
				else
				{
					this.culture = culture;
				}
				this.getIndex = GetIndexIgnoreCase;
				this.btmFullLen = BtmLen << 1;
			}
			else
			{
				this.getIndex = GetIndex;
				this.btmFullLen = BtmLen;
			}
			this.data = new uint[TopLen][][];
			this.data[0] = new uint[MidLen][];
			this.eascii = this.data[0][0] = new uint[btmFullLen];
		}
		/// <summary>
		/// 初始化 <see cref="Cyjb.Collections.CharSet"/> 类的新实例，
		/// 该实例包含从指定的集合复制的元素。
		/// </summary>
		/// <param name="collection">其元素被复制到新集中的集合。</param>
		public CharSet(IEnumerable<char> collection)
			: this(false, null)
		{
			this.UnionWith(collection);
		}
		/// <summary>
		/// 使用指定的是否区分大小写初始化 
		/// <see cref="Cyjb.Collections.CharSet"/> 类的新实例，
		/// 该实例包含从指定的集合复制的元素。
		/// </summary>
		/// <param name="collection">其元素被复制到新集中的集合。</param>
		/// <param name="ignoreCase">是否不区分字符的大小写。</param>
		public CharSet(IEnumerable<char> collection, bool ignoreCase)
			: this(ignoreCase, null)
		{
			this.UnionWith(collection);
		}
		/// <summary>
		/// 使用指定的是否区分大小写和区域信息初始化 
		/// <see cref="Cyjb.Collections.CharSet"/> 类的新实例，
		/// 该实例包含从指定的集合复制的元素。
		/// </summary>
		/// <param name="collection">其元素被复制到新集中的集合。</param>
		/// <param name="ignoreCase">是否不区分字符的大小写。</param>
		/// <param name="culture">不区分字符大小写时使用的区域信息。</param>
		public CharSet(IEnumerable<char> collection, bool ignoreCase, CultureInfo culture)
			: this(ignoreCase, culture)
		{
			this.UnionWith(collection);
		}
		/// <summary>
		/// 用指定的序列化信息和上下文初始化 <see cref="Cyjb.Collections.CharSet"/> 类的新实例。
		/// </summary>
		/// <param name="info"><see cref="System.Runtime.Serialization.SerializationInfo"/> 
		/// 对象，包含序列化 
		/// <see cref="Cyjb.Collections.CharSet"/> 所需的信息。</param>
		/// <param name="context"><see cref="System.Runtime.Serialization.StreamingContext"/> 对象，
		/// 该对象包含与 <see cref="Cyjb.Collections.CharSet"/> 相关联的序列化流的源和目标。</param>
		/// <exception cref="System.ArgumentNullException">info 参数为 <c>null</c>。</exception>
		private CharSet(SerializationInfo info, StreamingContext context)
			: base(null)
		{
			ExceptionHelper.CheckArgumentNull(info, "info");
			this.data = (uint[][][])info.GetValue("Data", typeof(uint[][][]));
			this.eascii = data[0][0];
			this.count = info.GetInt32("Count");
			this.ignoreCase = info.GetBoolean("IgnoreCase");
			if (this.ignoreCase)
			{
				this.culture = (CultureInfo)info.GetValue("Culture", typeof(CultureInfo));
				this.getIndex = GetIndexIgnoreCase;
				this.btmFullLen = BtmLen << 1;
			}
			else
			{
				this.getIndex = GetIndex;
				this.btmFullLen = BtmLen;
			}
		}
		/// <summary>
		/// 获取是否使用不区分大小写的比较。
		/// </summary>
		public bool IgnoreCase
		{
			get { return ignoreCase; }
		}
		/// <summary>
		/// 获取不区分大小写时使用的区域信息。
		/// </summary>
		public CultureInfo Culture
		{
			get { return culture; }
		}

		#region 数据操作

		/// <summary>
		/// 返回指定的中层存储单元是否都是 0 或 <c>null</c>。
		/// </summary>
		/// <param name="array">要判断的中层存储单元。</param>
		/// <returns>如果单元中的元素都是 0 或 <c>null</c>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		private static bool IsEmpty(uint[][] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && !IsEmpty(array[i]))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 返回指定的底层存储单元是否都是 0。
		/// </summary>
		/// <param name="array">要判断的底层存储单元。</param>
		/// <returns>如果单元中的元素都是 0，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool IsEmpty(uint[] array)
		{
			for (int i = 0; i < BtmLen; i++)
			{
				if (array[i] != 0)
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 计算指定的中层存储单元中包含的字符个数
		/// </summary>
		/// <param name="array">要计算字符个数的中层存储单元。</param>
		/// <returns>指定的中层存储单元中包含的字符个数。</returns>
		private static int CountChar(uint[][] array)
		{
			int cnt = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					cnt += CountChar(array[i]);
				}
			}
			return cnt;
		}
		/// <summary>
		/// 计算指定的底层存储单元中包含的字符个数。
		/// </summary>
		/// <param name="array">要计算字符个数的底层存储单元。</param>
		/// <returns>指定的底层存储单元中包含的字符个数。</returns>
		private static int CountChar(uint[] array)
		{
			int cnt = 0;
			for (int i = 0; i < BtmLen; i++)
			{
				if (array[i] != 0)
				{
					cnt += array[i].BinOneCnt();
				}
			}
			return cnt;
		}
		/// <summary>
		/// 复制指定的中层存储单元
		/// </summary>
		/// <param name="array">要复制中层存储单元。</param>
		/// <param name="count">实际被复制的字符个数。</param>
		/// <returns>复制得到的中层存储单元。。</returns>
		private static uint[][] CopyChar(uint[][] array, out int count)
		{
			uint[][] newArr = new uint[array.Length][];
			count = 0;
			int cnt = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					newArr[i] = null;
				}
				else
				{
					newArr[i] = CopyChar(array[i], out cnt);
					count += cnt;
				}
			}
			return newArr;
		}
		/// <summary>
		/// 复制指定的底层存储单元
		/// </summary>
		/// <param name="array">要复制底层存储单元。</param>
		/// <param name="count">实际被复制的字符个数。</param>
		/// <returns>复制得到的底层存储单元。。</returns>
		private static uint[] CopyChar(uint[] array, out int count)
		{
			uint[] newArr = new uint[array.Length];
			count = 0;
			for (int i = 0; i < array.Length; i++)
			{
				newArr[i] = array[i];
				if (array[i] != 0)
				{
					count += array[i].BinOneCnt();
				}
			}
			return newArr;
		}
		/// <summary>
		/// 默认的获取字符索引的方法。
		/// </summary>
		/// <param name="ch">要获取索引的字符。</param>
		/// <returns>指定字符的索引。</returns>
		private int GetIndex(char ch)
		{
			return ch;
		}
		/// <summary>
		/// 不区分大小写的获取字符索引的方法。
		/// </summary>
		/// <param name="ch">要获取索引的字符。</param>
		/// <returns>指定字符的索引。</returns>
		private int GetIndexIgnoreCase(char ch)
		{
			return char.ToUpper(ch, culture);
		}
		/// <summary>
		/// 获取指定字符对应的数据及相应掩码。如果不存在，则返回 <c>null</c>。
		/// </summary>
		/// <param name="ch">要获取数据及相应掩码的字符。</param>
		/// <param name="idx">数据的索引位置。</param>
		/// <param name="binIdx">数据的二进制位置。</param>
		/// <returns>数据数组。</returns>
		private uint[] FindMask(int ch, out int idx, out uint binIdx)
		{
			uint[] arr = null;
			if (ch <= 0xFF)
			{
				arr = eascii;
			}
			else
			{
				idx = ch >> TopShift;
				binIdx = 0;
				uint[][] arrMid = this.data[idx];
				if (arrMid == null)
				{
					return null;
				}
				idx = (ch >> MidShift) & MidMask;
				arr = arrMid[idx];
				if (arr == null)
				{
					return null;
				}
			}
			idx = (ch >> BtmShift) & BtmMask;
			binIdx = 1u << (ch & IndexMask);
			return arr;
		}
		/// <summary>
		/// 获取指定字符对应的掩码位置。如果掩码位置不存在，则创建指定的掩码位置。
		/// </summary>
		/// <param name="ch">要获取数据及相应掩码的字符。</param>
		/// <param name="idx">数据的索引位置。</param>
		/// <param name="binIdx">数据的二进制位置。</param>
		/// <returns>数据数组。</returns>
		private uint[] FindAndCreateMask(int ch, out int idx, out uint binIdx)
		{
			uint[] arr = null;
			if (ch <= 0xFF)
			{
				arr = eascii;
			}
			else
			{
				idx = ch >> TopShift;
				uint[][] arrMid = this.data[idx];
				if (arrMid == null)
				{
					arrMid = new uint[MidLen][];
					this.data[idx] = arrMid;
				}
				idx = (ch >> MidShift) & MidMask;
				arr = arrMid[idx];
				if (arr == null)
				{
					arr = new uint[btmFullLen];
					arrMid[idx] = arr;
				}
			}
			idx = (ch >> BtmShift) & BtmMask;
			binIdx = 1u << (ch & IndexMask);
			return arr;
		}
		/// <summary>
		/// 确定当前集与指定集合相比，相同的和未包含的元素数目。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <param name="returnIfUnfound">是否遇到未包含的元素就返回。</param>
		/// <param name="sameCount">当前集合中相同元素的数目。</param>
		/// <param name="unfoundCount">当前集合中未包含的元素数目。</param>
		private void CountElements(IEnumerable<char> other,
			bool returnIfUnfound, out int sameCount, out int unfoundCount)
		{
			Debug.Assert(this.count > 0);
			sameCount = unfoundCount = 0;
			CharSet uniqueSet = new CharSet(ignoreCase, culture);
			foreach (char ch in other)
			{
				if (this.Contains(ch))
				{
					if (uniqueSet.Add(ch))
					{
						sameCount++;
					}
				}
				else
				{
					unfoundCount++;
					if (returnIfUnfound)
					{
						break;
					}
				}
			}
		}
		/// <summary>
		/// 确定当前集是否包含指定的集合中的所有元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果包含 <paramref name="other"/> 中的所有元素，则返回 
		/// <c>true</c>，否则返回 <c>false</c>。</returns>
		private bool ContainsAllElements(CharSet other)
		{
			for (int i = 0; i < TopLen; i++)
			{
				uint[][] otherArrMid = other.data[i];
				if (otherArrMid == null)
				{
					continue;
				}
				uint[][] arrMid = data[i];
				if (arrMid == null)
				{
					// 判断 otherArrMid 是否为空。
					if (!IsEmpty(otherArrMid))
					{
						return false;
					}
					continue;
				}
				for (int j = 0; j < MidLen; j++)
				{
					uint[] otherArrBottom = otherArrMid[j];
					if (otherArrBottom == null)
					{
						continue;
					}
					uint[] arrBottom = arrMid[j];
					if (arrBottom == null)
					{
						if (!IsEmpty(otherArrBottom))
						{
							return false;
						}
						continue;
					}
					for (int k = 0; k < BtmLen; k++)
					{
						if ((arrBottom[k] | otherArrBottom[k]) != arrBottom[k])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		#endregion

		/// <summary>
		/// 将 <see cref="Cyjb.Collections.CharSet"/> 对象的容量设置为它所包含
		/// 的元素的实际个数，向上舍入为接近的特定于实现的值。
		/// </summary>
		public void TrimExcess()
		{
			// data[0][0] 由于对应于 EASCII，不能被移除。
			uint[][] arrMid = data[0];
			for (int j = 1; j < MidLen; j++)
			{
				// 移除为空的底层单元。
				if (arrMid[j] != null && IsEmpty(arrMid[j]))
				{
					arrMid[j] = null;
				}
			}
			for (int i = 1; i < TopLen; i++)
			{
				arrMid = data[i];
				if (arrMid == null)
				{
					continue;
				}
				bool allEmpty = true;
				for (int j = 0; j < MidLen; j++)
				{
					// 移除为空的底层单元。
					if (arrMid[j] != null)
					{
						if (IsEmpty(arrMid[j]))
						{
							arrMid[j] = null;
						}
						else
						{
							allEmpty = false;
						}
					}
				}
				if (allEmpty)
				{
					data[i] = null;
				}
			}
		}

		#region SetBase<char> 成员

		/// <summary>
		/// 从 <see cref="Cyjb.Collections.CharSet"/> 中移除所有元素。
		/// </summary>
		protected override void ClearItems()
		{
			this.count = 0;
			for (int i = 0; i < btmFullLen; i++)
			{
				eascii[i] = 0;
			}
			for (int i = 1; i < MidLen; i++)
			{
				data[0][i] = null;
			}
			for (int i = 1; i < TopLen; i++)
			{
				data[i] = null;
			}
		}
		/// <summary>
		/// 向当前集内添加元素，并返回一个指示是否已成功添加元素的值。
		/// </summary>
		/// <param name="item">要添加到 <see cref="Cyjb.Collections.CharSet"/> 的中的对象。
		/// 对于引用类型，该值可以为 <c>null</c>。</param>
		/// <returns>如果该元素已添加到集内，则为 <c>true</c>；
		/// 如果该元素已在集内，则为 <c>false</c>。</returns>
		protected override bool AddItem(char item)
		{
			int cIdx = getIndex(item);
			int idx;
			uint binIdx;
			uint[] arr = FindAndCreateMask(cIdx, out idx, out binIdx);
			if ((arr[idx] & binIdx) != 0)
			{
				return false;
			}
			arr[idx] |= binIdx;
			count++;
			if (ignoreCase && cIdx != item)
			{
				// 添加的是小写字母。
				arr[idx + BtmLen] |= binIdx;
			}
			return true;
		}
		/// <summary>
		/// 移除 <see cref="Cyjb.Collections.CharSet"/> 的指定元素。
		/// </summary>
		/// <param name="item">要移除的元素。</param>
		/// <returns>如果已从 <see cref="Cyjb.Collections.CharSet"/> 中成功移除 <paramref name="item"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始 <see cref="Cyjb.Collections.CharSet"/>
		/// 中没有找到 <paramref name="item"/>，该方法也会返回 <c>false</c>。</returns>
		protected override bool RemoveItem(char item)
		{
			int cIdx = getIndex(item);
			int idx;
			uint binIdx;
			uint[] arr = FindMask(cIdx, out idx, out binIdx);
			if (arr != null && (arr[idx] & binIdx) != 0)
			{
				arr[idx] &= ~binIdx;
				count--;
				if (ignoreCase && cIdx != item)
				{
					// 删除的是小写字母。
					arr[idx + BtmLen] &= ~binIdx;
				}
				return true;
			}
			return false;
		}

		#endregion // SetBase<char> 成员

		#region ISet<char> 成员

		/// <summary>
		/// 从当前集内移除指定集合中的所有元素。
		/// </summary>
		/// <param name="other">要从集内移除的项的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override void ExceptWith(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.count > 0)
			{
				if (this == other)
				{
					this.Clear();
				}
				else
				{
					CharSet otherSet = other as CharSet;
					if (otherSet == null ||
						this.ignoreCase != otherSet.ignoreCase ||
						this.culture != otherSet.culture)
					{
						foreach (char c in other)
						{
							this.RemoveItem(c);
						}
					}
					else
					{
						// 针对 CharSet 的操作更快。
						this.ExceptWith(otherSet);
					}
				}
			}
		}
		/// <summary>
		/// 从当前集内移除指定集合中的所有元素。
		/// </summary>
		/// <param name="other">要从集内移除的项的集合。</param>
		private void ExceptWith(CharSet other)
		{
			for (int i = 0; i < TopLen; i++)
			{
				uint[][] arrMid = data[i];
				if (arrMid == null)
				{
					continue;
				}
				uint[][] otherArrMid = other.data[i];
				if (otherArrMid == null)
				{
					continue;
				}
				for (int j = 0; j < MidLen; j++)
				{
					uint[] arrBottom = arrMid[j];
					if (arrBottom == null)
					{
						continue;
					}
					uint[] otherArrBottom = otherArrMid[j];
					if (otherArrBottom == null)
					{
						continue;
					}
					for (int k = 0; k < BtmLen; k++)
					{
						uint removed = arrBottom[k] & otherArrBottom[k];
						if (removed > 0)
						{
							this.count -= removed.BinOneCnt();
							arrBottom[k] &= ~removed;
							if (ignoreCase)
							{
								arrBottom[k + BtmLen] &= ~removed;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// 修改当前集，使该集仅包含指定集合中也存在的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override void IntersectWith(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.count > 0 && this != other)
			{
				CharSet otherSet = other as CharSet;
				if (otherSet == null ||
					this.ignoreCase != otherSet.ignoreCase ||
					this.culture != otherSet.culture)
				{
					otherSet = new CharSet(ignoreCase, culture);
					foreach (char ch in other)
					{
						if (this.Contains(ch))
						{
							otherSet.Add(ch);
						}
					}
					// 替换当前集合。
					this.eascii = otherSet.eascii;
					this.data = otherSet.data;
					this.count = otherSet.count;
				}
				else
				{
					// 针对 CharSet 的操作更快。
					IntersectWith(otherSet);
				}
			}
		}
		/// <summary>
		/// 修改当前集，使该集仅包含指定集合中也存在的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		private void IntersectWith(CharSet other)
		{
			// 针对 CharSet 的操作更快。
			for (int i = 0; i < TopLen; i++)
			{
				uint[][] arrMid = data[i];
				if (arrMid == null)
				{
					continue;
				}
				uint[][] otherArrMid = other.data[i];
				if (otherArrMid == null)
				{
					data[i] = null;
					// 计算被移除的元素数量。
					this.count -= CountChar(arrMid);
					continue;
				}
				for (int j = 0; j < MidLen; j++)
				{
					uint[] arrBottom = arrMid[j];
					if (arrBottom == null)
					{
						continue;
					}
					uint[] otherArrBottom = otherArrMid[j];
					if (otherArrBottom == null)
					{
						arrMid[j] = null;
						// 计算被移除的元素数量。
						this.count -= CountChar(arrBottom);
						continue;
					}
					for (int k = 0; k < BtmLen; k++)
					{
						uint removed = arrBottom[k] & ~otherArrBottom[k];
						if (removed > 0)
						{
							this.count -= removed.BinOneCnt();
							arrBottom[k] &= otherArrBottom[k];
							if (ignoreCase)
							{
								arrBottom[k + BtmLen] &= otherArrBottom[k];
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// 确定当前集是否为指定集合的真（严格）子集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的真子集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsProperSubsetOf(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			ICollection<char> col = other as ICollection<char>;
			if (this.count == 0)
			{
				if (col == null)
				{
					return other.Any();
				}
				return col.Count > 0;
			}
			CharSet otherSet = other as CharSet;
			if (otherSet == null ||
				this.ignoreCase != otherSet.ignoreCase ||
				this.culture != otherSet.culture)
			{
				int sameCount, unfoundCount;
				CountElements(other, false, out sameCount, out unfoundCount);
				return sameCount == count && unfoundCount > 0;
			}
			else if (this.count >= otherSet.count)
			{
				return false;
			}
			return otherSet.ContainsAllElements(this);
		}
		/// <summary>
		/// 确定当前集是否为指定集合的真（严格）超集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的真超集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsProperSupersetOf(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.count == 0)
			{
				return false;
			}
			ICollection<char> col = other as ICollection<char>;
			if (col != null && col.Count == 0)
			{
				return true;
			}
			CharSet otherSet = other as CharSet;
			if (otherSet == null ||
				this.ignoreCase != otherSet.ignoreCase ||
				this.culture != otherSet.culture)
			{
				int sameCount, unfoundCount;
				CountElements(other, true, out sameCount, out unfoundCount);
				return sameCount < Count && unfoundCount == 0;
			}
			if (otherSet.count >= count)
			{
				return false;
			}
			return ContainsAllElements(otherSet);
		}
		/// <summary>
		/// 确定当前集是否为指定集合的子集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的子集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsSubsetOf(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.count == 0)
			{
				return true;
			}
			CharSet otherSet = other as CharSet;
			if (otherSet == null ||
				this.ignoreCase != otherSet.ignoreCase ||
				this.culture != otherSet.culture)
			{
				int sameCount, unfoundCount;
				CountElements(other, false, out sameCount, out unfoundCount);
				return sameCount == count && unfoundCount >= 0;
			}
			if (this.count > otherSet.Count)
			{
				return false;
			}
			return otherSet.ContainsAllElements(this);
		}
		/// <summary>
		/// 确定当前集是否为指定集合的超集。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集是 <paramref name="other"/> 的超集，则为 
		/// <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool IsSupersetOf(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			ICollection<char> col = other as ICollection<char>;
			if (col != null)
			{
				if (col.Count == 0)
				{
					return true;
				}
				else if (this.count == 0)
				{
					return false;
				}
			}
			CharSet otherSet = other as CharSet;
			if (otherSet == null ||
				this.ignoreCase != otherSet.ignoreCase ||
				this.culture != otherSet.culture)
			{
				foreach (char ch in other)
				{
					if (!this.Contains(ch))
					{
						return false;
					}
				}
				return true;
			}
			if (otherSet.Count > this.count)
			{
				return false;
			}
			return this.ContainsAllElements(otherSet);
		}
		/// <summary>
		/// 确定当前集是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool Overlaps(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.count > 0)
			{
				CharSet otherSet = other as CharSet;
				if (otherSet == null ||
					this.ignoreCase != otherSet.ignoreCase ||
					this.culture != otherSet.culture)
				{
					foreach (char ch in other)
					{
						if (this.Contains(ch))
						{
							return true;
						}
					}
				}
				else
				{
					// 针对 CharSet 的操作更快。
					return this.Overlaps(otherSet);
				}
			}
			return false;
		}
		/// <summary>
		/// 确定当前集是否与指定的集合重叠。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集与 <paramref name="other"/> 
		/// 至少共享一个通用元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private bool Overlaps(CharSet other)
		{
			for (int i = 0; i < TopLen; i++)
			{
				uint[][] arrMid = data[i];
				if (arrMid == null)
				{
					continue;
				}
				uint[][] otherArrMid = other.data[i];
				if (otherArrMid == null)
				{
					continue;
				}
				for (int j = 0; j < MidLen; j++)
				{
					uint[] arrBottom = arrMid[j];
					if (arrBottom == null)
					{
						continue;
					}
					uint[] otherArrBottom = otherArrMid[j];
					if (otherArrBottom == null)
					{
						continue;
					}
					for (int k = 0; k < BtmLen; k++)
					{
						if ((arrBottom[k] & otherArrBottom[k]) > 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		/// <summary>
		/// 确定当前集与指定的集合中是否包含相同的元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <returns>如果当前集等于 <paramref name="other"/>，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override bool SetEquals(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			CharSet otherSet = other as CharSet;
			if (otherSet != null &&
				this.ignoreCase == otherSet.ignoreCase &&
				this.culture == otherSet.culture)
			{
				if (count != otherSet.count)
				{
					return false;
				}
				return this.ContainsAllElements(otherSet);
			}
			ICollection<char> col = other as ICollection<char>;
			if (this.count == 0)
			{
				if (col == null)
				{
					return !other.Any();
				}
				if (col.Count > 0)
				{
					return false;
				}
			}
			int sameCount, unfoundCount;
			CountElements(other, true, out sameCount, out unfoundCount);
			return (sameCount == count && unfoundCount == 0);
		}
		/// <summary>
		/// 修改当前集，使该集仅包含当前集或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override void SymmetricExceptWith(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this.count == 0)
			{
				this.UnionWith(other);
			}
			else if (this == other)
			{
				Clear();
			}
			else
			{
				CharSet otherSet = other as CharSet;
				if (otherSet == null ||
					this.ignoreCase != otherSet.ignoreCase ||
					this.culture != otherSet.culture)
				{
					otherSet = new CharSet(other, ignoreCase, culture);
				}
				// 针对 CharSet 的操作更快。
				SymmetricExceptWith(otherSet);
			}
		}
		/// <summary>
		/// 修改当前集，使该集仅包含当前集或指定集合中存在的元素（但不可包含两者共有的元素）。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		private void SymmetricExceptWith(CharSet other)
		{
			for (int i = 0; i < TopLen; i++)
			{
				uint[][] otherArrMid = other.data[i];
				if (otherArrMid == null)
				{
					continue;
				}
				uint[][] arrMid = data[i];
				if (arrMid == null)
				{
					// 复制数据。
					int cnt;
					data[i] = CopyChar(otherArrMid, out cnt);
					this.count += cnt;
					continue;
				}
				for (int j = 0; j < MidLen; j++)
				{
					uint[] arrBottom = arrMid[j];
					if (arrBottom == null)
					{
						continue;
					}
					uint[] otherArrBottom = otherArrMid[j];
					if (otherArrBottom == null)
					{
						// 复制数据。
						int cnt;
						arrMid[j] = CopyChar(otherArrBottom, out cnt);
						this.count += cnt;
						continue;
					}
					for (int k = 0; k < BtmLen; k++)
					{
						int oldCnt = 0;
						if (arrBottom[k] > 0)
						{
							oldCnt = arrBottom[k].BinOneCnt();
						}
						if (ignoreCase)
						{
							arrBottom[k + BtmLen] &= ~otherArrBottom[k];
							arrBottom[k + BtmLen] |= otherArrBottom[k + BtmLen] & ~arrBottom[k];
						}
						arrBottom[k] ^= otherArrBottom[k];
						int newCnt = 0;
						if (arrBottom[k] > 0)
						{
							newCnt = arrBottom[k].BinOneCnt();
						}
						this.count += newCnt - oldCnt;
					}
				}
			}
		}
		/// <summary>
		/// 修改当前集，使该集包含当前集和指定集合中同时存在的所有元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="other"/> 为 <c>null</c>。</exception>
		public override void UnionWith(IEnumerable<char> other)
		{
			ExceptionHelper.CheckArgumentNull(other, "other");
			if (this != other)
			{
				CharSet otherSet = other as CharSet;
				if (otherSet == null ||
					this.ignoreCase != otherSet.ignoreCase ||
					this.culture != otherSet.culture)
				{
					foreach (char c in other)
					{
						this.AddItem(c);
					}
				}
				else
				{
					// 针对 CharSet 的操作更快。
					UnionWith(otherSet);
				}
			}
		}
		/// <summary>
		/// 修改当前集，使该集包含当前集和指定集合中同时存在的所有元素。
		/// </summary>
		/// <param name="other">要与当前集进行比较的集合。</param>
		private void UnionWith(CharSet other)
		{
			for (int i = 0; i < TopLen; i++)
			{
				uint[][] otherArrMid = other.data[i];
				if (otherArrMid == null)
				{
					continue;
				}
				uint[][] arrMid = data[i];
				if (arrMid == null)
				{
					// 复制数据。
					int cnt;
					data[i] = CopyChar(otherArrMid, out cnt);
					this.count += cnt;
					continue;
				}
				for (int j = 0; j < MidLen; j++)
				{
					uint[] otherArrBottom = otherArrMid[j];
					if (otherArrBottom == null)
					{
						continue;
					}
					uint[] arrBottom = arrMid[j];
					if (arrBottom == null)
					{
						// 复制数据。
						int cnt;
						arrMid[j] = CopyChar(otherArrBottom, out cnt);
						this.count += cnt;
						continue;
					}
					else
					{
						for (int k = 0; k < BtmLen; k++)
						{
							// 后来添加的字符数。
							uint added = (~arrBottom[k]) & otherArrBottom[k];
							if (added > 0)
							{
								this.count += added.BinOneCnt();
								arrBottom[k] |= added;
								if (ignoreCase)
								{
									arrBottom[k + BtmLen] |= otherArrBottom[k + BtmLen] & added;
								}
							}
						}
					}
				}
			}
		}

		#endregion // ISet<char> 成员

		#region ICollection<char> 成员

		/// <summary>
		/// 获取 <see cref="Cyjb.Collections.CharSet"/> 中包含的元素数。
		/// </summary>
		/// <value><see cref="Cyjb.Collections.CharSet"/> 中包含的元素数。</value>
		public override int Count
		{
			get { return this.count; }
		}
		/// <summary>
		/// 确定 <see cref="Cyjb.Collections.CharSet"/> 是否包含特定值。
		/// </summary>
		/// <param name="item">要在 <see cref="Cyjb.Collections.CharSet"/> 
		/// 中定位的对象。</param>
		/// <returns>如果在 <see cref="Cyjb.Collections.CharSet"/> 
		/// 中找到 <paramref name="item"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Contains(char item)
		{
			int idx;
			uint binIdx;
			uint[] arr = FindMask(getIndex(item), out idx, out binIdx);
			return (arr != null) && ((arr[idx] & binIdx) != 0);
		}
		/// <summary>
		/// 从特定的 <see cref="System.Array"/> 索引处开始，
		/// 将 <see cref="Cyjb.Collections.CharSet"/> 的元素复制到一个 <see cref="System.Array"/> 中。
		/// </summary>
		/// <param name="array">作为从 <see cref="Cyjb.Collections.CharSet"/>
		/// 复制的元素的目标位置的一维 <see cref="System.Array"/>。
		/// <see cref="System.Array"/> 必须具有从零开始的索引。</param>
		/// <param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="array"/> 为 <c>null</c>。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> 小于零。</exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="array"/> 是多维的。</exception>
		/// <exception cref="System.ArgumentException">源 <see cref="Cyjb.Collections.CharSet"/>
		/// 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 
		/// 末尾之间的可用空间。</exception>
		public override void CopyTo(char[] array, int arrayIndex)
		{
			ExceptionHelper.CheckArgumentNull(array, "array");
			ExceptionHelper.CheckFlatArray(array, "array");
			if (arrayIndex < 0)
			{
				throw ExceptionHelper.ArgumentOutOfRange("arrayIndex");
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw ExceptionHelper.ArrayTooSmall("array");
			}
			foreach (char ch in this)
			{
				array[arrayIndex++] = ch;
			}
		}

		#endregion // ICollection<char> 成员

		#region IEnumerable<char> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 
		/// <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/>。</returns>
		public override IEnumerator<char> GetEnumerator()
		{
			for (int i = 0; i < TopLen; i++)
			{
				uint[][] arr1 = data[i];
				if (arr1 == null)
				{
					continue;
				}
				int c1 = i << TopShift;
				for (int j = 0; j < MidLen; j++)
				{
					uint[] arr2 = arr1[j];
					if (arr2 == null)
					{
						continue;
					}
					int c2 = c1 | (j << MidShift);
					for (int k = 0; k < BtmLen; k++)
					{
						int c3 = c2 | (k << BtmShift);
						uint value = arr2[k];
						uint valueIg = ignoreCase ? arr2[k + BtmLen] : 0;
						for (int n = -1; value > 0; )
						{
							int oneIdx = (value & 1) == 1 ? 1 : value.BinTrailingZeroCount() + 1;
							if (oneIdx == 32)
							{
								// C# 中 uint 右移 32 位会不变。
								value = 0;
							}
							else
							{
								value = value >> oneIdx;
							}
							n += oneIdx;
							char c4 = (char)(c3 | n);
							if ((valueIg & (1 << n)) > 0)
							{
								c4 = char.ToLower(c4, culture);
							}
							yield return c4;
						}
					}
				}
			}
		}

		#endregion // IEnumerable<char> 成员

		#region ISerializable 成员

		/// <summary>
		/// 使用将目标对象序列化所需的数据填充 
		/// <see cref="System.Runtime.Serialization.SerializationInfo"/>。
		/// </summary>
		/// <param name="info">要填充数据的 
		/// <see cref="System.Runtime.Serialization.SerializationInfo"/>。
		/// </param>
		/// <param name="context">此序列化的目标。</param>
		/// <exception cref="System.Security.SecurityException">
		/// 调用方没有所要求的权限。</exception>
		/// <exception cref="System.ArgumentNullException">info 参数为 
		/// <c>null</c>。</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			ExceptionHelper.CheckArgumentNull(info, "info");
			info.AddValue("Data", this.data);
			info.AddValue("Count", this.count);
			info.AddValue("IgnoreCase", this.ignoreCase);
			if (this.ignoreCase)
			{
				info.AddValue("Culture", this.culture);
			}
		}

		#endregion

	}
}