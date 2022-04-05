using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Cyjb.Collections
{
	/// <summary>
	/// AVL 树。
	/// </summary>
	[Serializable, DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(EnumerableDebugView<>))]
	internal class AVLTree<TKey, TValue> : IEnumerable<AVLTree<TKey, TValue>.Node>
		where TKey : notnull, IComparable<TKey>
	{
		/// <summary>
		/// AVL 树的节点。
		/// </summary>
		public class Node
		{
			/// <summary>
			/// 节点的键。
			/// </summary>
			public TKey Key;
			/// <summary>
			/// 节点的值。
			/// </summary>
			public TValue? Value;
			/// <summary>
			/// 左子树。
			/// </summary>
			public Node? Left;
			/// <summary>
			/// 右子树。
			/// </summary>
			public Node? Right;
			/// <summary>
			/// 父节点。
			/// </summary>
			public Node? Parent;
			/// <summary>
			/// 树的高度。
			/// </summary>
			public int Height;

			/// <summary>
			/// 使用指定的键和值初始化。
			/// </summary>
			/// <param name="key">节点的键。</param>
			/// <param name="value">节点的值。</param>
			public Node(TKey key, TValue? value)
			{
				Key = key;
				Value = value;
			}

			/// <summary>
			/// 返回左子节点的高度。
			/// </summary>
			public int LeftHeight => Left?.Height ?? 0;
			/// <summary>
			/// 返回右子节点的高度。
			/// </summary>
			public int RightHeight => Right?.Height ?? 0;

			/// <summary>
			/// 更新树的高度。
			/// </summary>
			public void UpdateHeight()
			{
				Height = Math.Max(LeftHeight, RightHeight) + 1;
			}

			/// <summary>
			/// 返回当前子树的首个节点。
			/// </summary>
			public Node First
			{
				get
				{
					Node node = this;
					while (node.Left != null)
					{
						node = node.Left;
					}
					return node;
				}
			}

			/// <summary>
			/// 返回当前子树的最后一个节点。
			/// </summary>
			public Node Last
			{
				get
				{
					Node node = this;
					while (node.Right != null)
					{
						node = node.Right;
					}
					return node;
				}
			}

			/// <summary>
			/// 返回前驱节点。
			/// </summary>
			/// <returns>当前节点的前驱节点，如果不存在则返回 <c>null</c>。</returns>
			public Node? Prev
			{
				get
				{
					if (Left == null)
					{
						Node node = this;
						while (true)
						{
							// 已经到达根节点，没有前驱节点。
							if (node.Parent == null)
							{
								return null;
							}
							Node last = node;
							node = node.Parent;
							if (node.Right == last)
							{
								return node;
							}
						}
					}
					else
					{
						return Left.Last;
					}
				}
			}

			/// <summary>
			/// 返回后继节点。
			/// </summary>
			/// <returns>当前节点的后继节点，如果不存在则返回 <c>null</c>。</returns>
			public Node? Next
			{
				get
				{
					if (Right == null)
					{
						Node node = this;
						while (true)
						{
							// 已经到达根节点，没有后继节点。
							if (node.Parent == null)
							{
								return null;
							}
							Node last = node;
							node = node.Parent;
							if (node.Left == last)
							{
								return node;
							}
						}
					}
					else
					{
						return Right.First;
					}
				}
			}

			/// <summary>
			/// 返回当前集合的字符串表示。
			/// </summary>
			/// <returns>当前集合的字符串表示。</returns>
			public override string ToString()
			{
				StringBuilder builder = new();
				builder.Append('[');
				if (Key != null)
				{
					builder.Append(Key.ToString());
				}
				builder.Append(", ");
				if (Value != null)
				{
					builder.Append(Value.ToString());
				}
				builder.Append(']');
				return builder.ToString();
			}
		}

		/// <summary>
		/// AVL 树的根节点。
		/// </summary>
		private Node? root;
		/// <summary>
		/// 存储的元素个数。
		/// </summary>
		private int count;

		/// <summary>
		/// 获取当前集合包含的元素数。
		/// </summary>
		public int Count => count;

		/// <summary>
		/// 获取当前集合的首个节点。
		/// </summary>
		public Node? First => root?.First;

		/// <summary>
		/// 获取当前集合的最后一个节点。
		/// </summary>
		public Node? Last => root?.Last;

		/// <summary>
		/// 确定当前集合是否包含指定键。
		/// </summary>
		/// <param name="key">要在当前集合中定位的键。</param>
		/// <returns>如果在当前集合中找到 <paramref name="key"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Contains(TKey key)
		{
			return root != null && Find(key).cmp == 0;
		}

		/// <summary>
		/// 找到小于等于指定键的节点。
		/// </summary>
		/// <param name="key">要寻找的键。</param>
		/// <returns>小于等于指定键的节点。</returns>
		public Node? FindLE(TKey key)
		{
			var (node, cmp) = Find(key);
			if (cmp < 0)
			{
				node = node!.Prev;
			}
			return node;
		}

		/// <summary>
		/// 添加指定的键和值，并返回添加后的节点。
		/// </summary>
		/// <param name="key">要添加的键。</param>
		/// <param name="value">要添加的值。</param>
		/// <returns>添加后的节点。</returns>
		public Node Add(TKey key, TValue value)
		{
			var (parent, cmp) = Find(key);
			if (parent == null)
			{
				count++;
				root = new Node(key, value);
				return root;
			}
			if (cmp == 0)
			{
				// 有相同 key 的节点，直接覆盖。
				parent.Value = value;
				return parent;
			}
			count++;
			Node node = new(key, value);
			node.Parent = parent;
			if (cmp < 0)
			{
				parent.Left = node;
			}
			else
			{
				parent.Right = node;
			}
			node.Height = 1;
			for (Node? curNode = parent; curNode != null; curNode = curNode.Parent)
			{
				int h0 = curNode.LeftHeight;
				int h1 = curNode.RightHeight;
				int height = Math.Max(h0, h1) + 1;
				// 节点高度不变，不需要继续回溯。
				if (curNode.Height == height)
				{
					break;
				}
				int diff = h0 - h1;
				curNode.Height = height;
				if (diff <= -2)
				{
					curNode = FixL(curNode);
				}
				else if (diff >= 2)
				{
					curNode = FixR(curNode);
				}
			}
			return node;
		}

		/// <summary>
		/// 从当前集合中移除指定键。
		/// </summary>
		/// <param name="key">要从当前集合中移除的键。</param>
		/// <returns>如果已从当前集合中成功移除 <paramref name="key"/>，
		/// 则为 <c>true</c>；否则为 <c>false</c>。如果在原始当前集合
		/// 中没有找到 <paramref name="key"/>，该方法也会返回 <c>false</c>。</returns>
		public bool Remove(TKey key)
		{
			var (node, cmp) = Find(key);
			if (node != null && cmp == 0)
			{
				Remove(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 从当前集合中移除指定节点。
		/// </summary>
		/// <param name="node">要从当前集合中移除的节点。</param>
		public void Remove(Node node)
		{
			Node? child;
			Node? parent;
			if (node.Left != null && node.Right != null)
			{
				Node old = node;
				node = node.Right.First;
				child = node.Right;
				parent = node.Parent;
				if (child != null)
				{
					child.Parent = parent;
				}
				ReplaceChild(node, child, parent);
				if (node.Parent == old)
				{
					parent = node;
				}
				node.Left = old.Left;
				node.Right = old.Right;
				node.Parent = old.Parent;
				node.Height = old.Height;
				ReplaceChild(old, node, old.Parent);
				old.Left.Parent = node;
				if (old.Right != null)
				{
					old.Right.Parent = node;
				}
			}
			else
			{
				child = node.Left ?? node.Right;
				parent = node.Parent;
				ReplaceChild(node, child, parent);
				if (child != null)
				{
					child.Parent = parent;
				}
			}
			if (parent != null)
			{
				Rebalance(parent);
			}
		}

		/// <summary>
		/// 从当前集合中移除所有元素。
		/// </summary>
		public void Clear()
		{
			count = 0;
			root = null;
		}

		#region AVL 操作

		/// <summary>
		/// 找到指定键的位置。
		/// </summary>
		/// <param name="key">要寻找的键。</param>
		/// <returns>键的位置父节点和与父节点键的比较结果。</returns>
		private (Node? parent, int cmp) Find(TKey key)
		{
			Node? parent = null, node = root;
			int cmp = 0;
			while (node != null)
			{
				parent = node;
				cmp = key.CompareTo(parent.Key);
				if (cmp == 0)
				{
					break;
				}
				else if (cmp < 0)
				{
					node = node.Left;
				}
				else
				{
					node = node.Right;
				}
			}
			return (parent, cmp);
		}

		/// <summary>
		/// 左旋平衡（RR 和 RL）。
		/// </summary>
		/// <param name="node">要平衡的根节点。</param>
		/// <returns>平衡后的根节点。</returns>
		private Node FixL(Node node)
		{
			Node right = node.Right!;
			int rh0 = right.LeftHeight;
			int rh1 = right.RightHeight;
			if (rh0 > rh1)
			{
				right = RotateRight(right);
				right.Right!.UpdateHeight();
				right.UpdateHeight();
			}
			node = RotateLeft(node);
			node.Left!.UpdateHeight();
			node.UpdateHeight();
			return node;
		}

		/// <summary>
		/// 右旋平衡（LL 和 LR）。
		/// </summary>
		/// <param name="node">要平衡的根节点。</param>
		/// <returns>平衡后的根节点。</returns>
		private Node FixR(Node node)
		{
			Node left = node.Left!;
			int rh0 = left.LeftHeight;
			int rh1 = left.RightHeight;
			if (rh0 < rh1)
			{
				left = RotateLeft(left);
				left.Left!.UpdateHeight();
				left.UpdateHeight();
			}
			node = RotateRight(node);
			node.Right!.UpdateHeight();
			node.UpdateHeight();
			return node;
		}

		/// <summary>
		/// 左旋操作。
		/// </summary>
		/// <param name="node">要旋转的根节点。</param>
		/// <returns>旋转后的根节点。</returns>
		private Node RotateLeft(Node node)
		{
			Node right = node.Right!;
			Node parent = node.Parent!;
			node.Right = right.Left;
			if (right.Left != null)
			{
				right.Left.Parent = node;
			}
			right.Left = node;
			right.Parent = parent;
			ReplaceChild(node, right, parent);
			node.Parent = right;
			return right;
		}

		/// <summary>
		/// 右旋操作。
		/// </summary>
		/// <param name="node">要旋转的根节点。</param>
		/// <returns>旋转后的根节点。</returns>
		private Node RotateRight(Node node)
		{
			Node left = node.Left!;
			Node parent = node.Parent!;
			node.Left = left.Right;
			if (left.Right != null)
			{
				left.Right.Parent = node;
			}
			left.Right = node;
			left.Parent = parent;
			ReplaceChild(node, left, parent);
			node.Parent = left;
			return left;
		}

		/// <summary>
		/// 替换指定的子节点。
		/// </summary>
		/// <param name="oldNode">要被替换的旧节点。</param>
		/// <param name="newNode">要替换成的新节点。</param>
		/// <param name="parent">父节点</param>
		private void ReplaceChild(Node oldNode, Node? newNode, Node? parent)
		{
			if (parent == null)
			{
				root = newNode;
			}
			else
			{
				if (parent.Left == oldNode)
				{
					parent.Left = newNode;
				}
				else
				{
					parent.Right = newNode;
				}
			}
		}

		/// <summary>
		/// 重新平衡指定的子树。
		/// </summary>
		/// <param name="node">要平衡的子树。</param>
		private void Rebalance(Node? node)
		{
			while (node != null)
			{
				int h0 = node.LeftHeight;
				int h1 = node.RightHeight;
				int diff = h0 - h1;
				int height = Math.Max(h0, h1) + 1;
				if (node.Height != height)
				{
					node.Height = height;
				}
				else if (diff >= -1 && diff <= 1)
				{
					break;
				}
				if (diff <= -2)
				{
					node = FixL(node);
				}
				else if (diff >= 2)
				{
					node = FixR(node);
				}
				node = node.Parent;
			}
		}

		#endregion AVL 操作

		#region IEnumerable<T> 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator{T}"/> 对象。</returns>
		public IEnumerator<Node> GetEnumerator()
		{
			if (root != null)
			{
				Node? node = root.First;
				while (node != null)
				{
					yield return node;
					node = node.Next;
				}
			}
		}

		#endregion // IEnumerable<T> 成员

		#region IEnumerable 成员

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion // IEnumerable 成员

	}
}
