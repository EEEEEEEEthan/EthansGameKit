using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit.Collections
{
	class RTreeNode<T> where T : class
	{
		const int splitThreshold = 8;
		public RTreeRect rect;
		readonly RTree<T> tree;
		RTreeNode<T>[] children;
		HashSet<RTreeItem<T>> items;
		public bool IsLeaf => children is null;
		public int Count { get; private set; }
		public RTreeNode(RTreeRect rect, RTree<T> tree)
		{
			this.rect = rect;
			this.tree = tree;
		}
		public void DrawGizmos()
		{
			if (IsLeaf) return;
			Gizmos.DrawLine(new(rect.xMin, rect.yMid), new(rect.xMax, rect.yMid));
			Gizmos.DrawLine(new(rect.xMid, rect.yMin), new(rect.xMid, rect.yMax));
		}
        /// <summary>
        ///     扩张节点，使得节点能够包含指定的矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public RTreeNode<T> Encapsulate(RTreeRect rect)
		{
			var node = this;
			var nodeRect = node.rect;
			while (!nodeRect.Contains(rect))
			{
				float xMin, xMid, xMax, yMin, yMid, yMax;
				var index = 0;
				if (nodeRect.xMin > rect.xMin)
				{
					xMin = nodeRect.xMin - nodeRect.Width;
					xMid = nodeRect.xMin;
					xMax = nodeRect.xMax;
					index |= 1;
				}
				else
				{
					xMin = nodeRect.xMin;
					xMid = nodeRect.xMax;
					xMax = nodeRect.xMax + nodeRect.Width;
					index |= 0;
				}
				if (nodeRect.yMin > rect.yMin)
				{
					yMin = nodeRect.yMin - nodeRect.Height;
					yMid = nodeRect.yMin;
					yMax = nodeRect.yMax;
					index |= 2;
				}
				else
				{
					yMin = nodeRect.yMin;
					yMid = nodeRect.yMax;
					yMax = nodeRect.yMax + nodeRect.Height;
					index |= 0;
				}
				var newRect = new RTreeRect(xMin, xMax, yMin, yMax, xMid, yMid);
				var newNode = new RTreeNode<T>(newRect, tree);
				newNode.children = new RTreeNode<T>[4];
				newNode.children[index] = node;
				newNode.Count = node.Count;
				node = newNode;
				nodeRect = newRect;
			}
			return node;
		}
		public void Insert(RTreeItem<T> item)
		{
			++Count;
			if (IsLeaf)
			{
				items ??= new();
				items.Add(item);
				if (items.Count > splitThreshold && rect.Width > tree.MinWidth && rect.Height > tree.MinWidth)
				{
					var fullContained = true;
					foreach (var i in items)
						if (!i.rect.Contains(rect))
						{
							fullContained = false;
							break;
						}
					if (!fullContained)
						Split();
				}
			}
			else
			{
				for (var i = 0; i < 4; ++i)
				{
					var childRect = rect.GetChild(i);
					if (childRect.Intersect(item.rect))
						(children[i] ??= new(childRect, tree)).Insert(item);
				}
			}
		}
		public bool Remove(RTreeItem<T> item)
		{
			if (IsLeaf)
			{
				if (items != null && items.Remove(item))
				{
					--Count;
					return true;
				}
				return false;
			}
			var any = false;
			foreach (var child in children)
			{
				if (child is null) continue;
				if (child.rect.Intersect(item.rect))
					if (child.Remove(item))
						any = true;
			}
			if (any)
			{
				--Count;
				if (Count < splitThreshold)
					Merge();
				return true;
			}
			return false;
		}
		public void Query(RTreeRect rect, HashSet<RTreeItem<T>> result)
		{
			if (IsLeaf)
			{
				foreach (var item in items)
					if (rect.Intersect(item.rect))
						result.Add(item);
			}
			else
			{
				foreach (var child in children)
				{
					if (child is null) continue;
					if (child.rect.Intersect(rect))
						child.Query(rect, result);
				}
			}
		}
		void Split()
		{
			Assert.IsTrue(IsLeaf);
			children = new RTreeNode<T>[4];
			for (var i = 0; i < 4; ++i)
			{
				var childRect = rect.GetChild(i);
				foreach (var item in items)
					if (childRect.Intersect(item.rect))
					{
						children[i] ??= new(childRect, tree);
						children[i].Insert(item);
					}
			}
			items = null;
		}
		void Merge()
		{
			if (IsLeaf) return;
			var newItems = new HashSet<RTreeItem<T>>();
			foreach (var child in children)
			{
				if (child is null) continue;
				child.Merge();
				Assert.IsTrue(child.IsLeaf);
				foreach (var item in child.items)
					newItems.Add(item);
			}
			Assert.IsTrue(newItems.Count == Count);
			items = newItems;
			children = null;
			Assert.IsTrue(IsLeaf);
		}
	}
}
