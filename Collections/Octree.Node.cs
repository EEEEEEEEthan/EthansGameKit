using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit.Collections
{
	public partial class Octree<T>
	{
		class Node
		{
			public static Node Generate(
				float xMin,
				float xMid,
				float xMax,
				float yMin,
				float yMid,
				float yMax,
				float zMin,
				float zMid,
				float zMax
			)
			{
				var node = new Node
				{
					xMin = xMin,
					xMid = xMid,
					xMax = xMax,
					yMin = yMin,
					yMid = yMid,
					yMax = yMax,
					zMin = zMin,
					zMid = zMid,
					zMax = zMax,
				};
				return node;
			}
			Node[] children;
			List<Item> items;
			float x, y, z;
			float xMin, xMid, xMax, yMin, yMid, yMax, zMin, zMid, zMax;
			public bool IsBranch => children != null;
			public bool IsEmpty
			{
				get
				{
					if (!IsBranch) return items is null || items.Count <= 0;
					for (var i = 0; i < 8; i++)
					{
						var child = children[i];
						if (child != null) return false;
					}
					return true;
				}
			}
			public IEnumerable<Item> allItems
			{
				get
				{
					if (IsBranch)
						for (var i = 0; i < 8; i++)
						{
							var child = children[i];
							if (child != null)
								foreach (var item in child.allItems)
									yield return item;
						}
					else
					{
						var cnt = items.Count;
						for (var i = 0; i < cnt; i++)
							yield return items[i];
					}
				}
			}
			Bounds Bounds => new(new(xMid, yMid, zMid), new(xMax - xMin, yMax - yMin, zMax - zMin));
			public void Query(ref Item[] result, Plane[] planes, ref int count)
			{
				var bounds = new Bounds(new(xMid, yMid, zMid), new(xMax - xMin, yMax - yMin, zMax - zMin));
				if (!GeometryUtility.TestPlanesAABB(planes, bounds)) return;
				if (IsBranch)
					for (var i = 0; i < 8; i++)
					{
						var child = children[i];
						child?.Query(ref result, planes, ref count);
					}
				else
					foreach (var item in items)
					{
						if (count >= result.Length) Array.Resize(ref result, result.Length * 2);
						result[count++] = item;
					}
			}
			public void Query(ICollection<Item> result, Plane[] planes)
			{
				var bounds = new Bounds(new(xMid, yMid, zMid), new(xMax - xMin, yMax - yMin, zMax - zMin));
				if (!GeometryUtility.TestPlanesAABB(planes, bounds)) return;
				if (IsBranch)
					for (var i = 0; i < 8; i++)
					{
						var child = children[i];
						child?.Query(result, planes);
					}
				else
				{
					var count = items.Count;
					for (var i = 0; i < count; i++)
					{
						result.Add(items[i]);
					}
				}
			}
			public void Query(ICollection<Item> result, Vector3 center, float sqrMagnitude)
			{
				if (IsBranch)
				{
					var point = Bounds.ClosestPoint(center);
					if ((center - point).sqrMagnitude < sqrMagnitude)
						for (var i = 0; i < 8; i++)
						{
							var child = children[i];
							child?.Query(result, center, sqrMagnitude);
						}
				}
				else
				{
					if ((center - new Vector3(x, y, z)).sqrMagnitude >= sqrMagnitude) return;
					var count = items.Count;
					for (var i = 0; i < count; i++)
					{
						result.Add(items[i]);
					}
				}
			}
			public bool TryGetTheOnlyChild(out Node firstChild)
			{
				Assert.IsTrue(IsBranch);
				firstChild = null;
				for (var i = 0; i < 8; i++)
				{
					var child = children[i];
					if (child is null) continue;
					if (firstChild is null)
						firstChild = child;
					else
						return false;
				}
				return true;
			}
			public bool Contains(float x, float y, float z)
			{
				return xMin <= x &&
					x < xMax &&
					yMin <= y &&
					y < yMax &&
					zMin <= z &&
					z < zMax;
			}
			public Node Encapsulate(float x, float y, float z)
			{
				Assert.IsFalse(Contains(x, y, z));
				if (IsBranch)
				{
					float xMin, xMid, xMax, yMin, yMid, yMax, zMin, zMid, zMax;
					if (x < this.xMid)
					{
						xMax = this.xMax;
						xMid = this.xMin;
						xMin = this.xMin + this.xMin - this.xMax;
					}
					else
					{
						xMin = this.xMin;
						xMid = this.xMax;
						xMax = this.xMax + this.xMax - this.xMin;
					}
					if (y < this.yMid)
					{
						yMax = this.yMax;
						yMid = this.yMin;
						yMin = this.yMin + this.yMin - this.yMax;
					}
					else
					{
						yMin = this.yMin;
						yMid = this.yMax;
						yMax = this.yMax + this.yMax - this.yMin;
					}
					if (z < this.zMid)
					{
						zMax = this.zMax;
						zMid = this.zMin;
						zMin = this.zMin + this.zMin - this.zMax;
					}
					else
					{
						zMin = this.zMin;
						zMid = this.zMax;
						zMax = this.zMax + this.zMax - this.zMin;
					}
					var node = new Node
					{
						xMin = xMin,
						xMid = xMid,
						xMax = xMax,
						yMin = yMin,
						yMid = yMid,
						yMax = yMax,
						zMin = zMin,
						zMid = zMid,
						zMax = zMax,
					};
					var index = node.GetChildIndex(this.xMid, this.yMid, this.zMid);
					node.children = new Node[8];
					node.children[index] = this;
					return node.Contains(x, y, z) ? node : node.Encapsulate(x, y, z);
				}
				if (x < xMid)
				{
					xMid = xMin;
					xMin = xMin + xMin - xMax;
				}
				else
				{
					xMid = xMax;
					xMax = xMax + xMax - xMin;
				}
				if (y < yMid)
				{
					yMid = yMin;
					yMin = yMin + yMin - yMax;
				}
				else
				{
					yMid = yMax;
					yMax = yMax + yMax - yMin;
				}
				if (z < zMid)
				{
					zMid = zMin;
					zMin = zMin + zMin - zMax;
				}
				else
				{
					zMid = zMax;
					zMax = zMax + zMax - zMin;
				}
				return Contains(x, y, z) ? this : Encapsulate(x, y, z);
			}
			[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
			public void Insert(Item item, float x, float y, float z)
			{
			REINSERT:
				if (IsBranch)
				{
					var childIndex = GetChildIndex(x, y, z);
					var child = children[childIndex];
					if (child is null) children[childIndex] = child = CreateChild(x, y, z);
					child.Insert(item, x, y, z);
				}
				else
				{
					if (items == null)
					{
						this.x = x;
						this.y = y;
						this.z = z;
						items = new() { item };
						return;
					}
					if (x == this.x && y == this.y && z == this.z)
					{
						items.Add(item);
						return;
					}
					Subdivide();
					Assert.IsTrue(IsBranch);
					goto REINSERT;
				}
			}
			public void Remove(Item item, float x, float y, float z)
			{
				if (IsBranch)
				{
					var childIndex = GetChildIndex(x, y, z);
					var child = children[childIndex];
					child.Remove(item, x, y, z);
					if (child.IsEmpty) children[childIndex] = null;
					if (!TryGetTheOnlyChild(out var theOnlyChild)) return;
					if (theOnlyChild.IsBranch) return;
					this.x = theOnlyChild.x;
					this.y = theOnlyChild.y;
					this.z = theOnlyChild.z;
					children = null;
					items = theOnlyChild.items;
					return;
				}
				items.Remove(item);
				if (items.Count == 0) items = null;
			}
			void Subdivide()
			{
				children = new Node[8];
				foreach (var item in items)
				{
					var childIndex = GetChildIndex(item.Position.x, item.Position.y, item.Position.z);
					var child = children[childIndex];
					if (child is null)
						children[childIndex] = child = CreateChild(
							item.Position.x,
							item.Position.y,
							item.Position.z
						);
					child.Insert(item, item.Position.x, item.Position.y, item.Position.z);
				}
				items = null;
			}
			int GetChildIndex(float x, float y, float z)
			{
				var childIndex = 0;
				if (x >= xMid) childIndex |= 1;
				if (y >= yMid) childIndex |= 2;
				if (z >= zMid) childIndex |= 4;
				return childIndex;
			}
			Node CreateChild(float x, float y, float z)
			{
				var child = new Node();
				if (x < xMid)
				{
					child.xMin = xMin;
					child.xMax = xMid;
				}
				else
				{
					child.xMin = xMid;
					child.xMax = xMax;
				}
				if (y < yMid)
				{
					child.yMin = yMin;
					child.yMax = yMid;
				}
				else
				{
					child.yMin = yMid;
					child.yMax = yMax;
				}
				if (z < zMid)
				{
					child.zMin = zMin;
					child.zMax = zMid;
				}
				else
				{
					child.zMin = zMid;
					child.zMax = zMax;
				}
				child.xMid = (child.xMin + child.xMax) * 0.5f;
				child.yMid = (child.yMin + child.yMax) * 0.5f;
				child.zMid = (child.zMin + child.zMax) * 0.5f;
				return child;
			}
#if UNITY_EDITOR
			static readonly Color invisiableBranchColor = new(1, 1, 1, 0.05f);
			static readonly Color visiableBranchColor = new(1, 1, 1, 0.1f);
			static readonly Color invisiableleafColor = new(0, 1, 0, 0.25f);
			static readonly Color visiableleafColor = new(0, 1, 0, 0.5f);
			public void Editor_DrawGizmos(Plane[] planes)
			{
				var bounds = new Bounds(new(xMid, yMid, zMid), new(xMax - xMin, yMax - yMin, zMax - zMin));
				Gizmos.color = GeometryUtility.TestPlanesAABB(planes, bounds)
					? IsBranch ? visiableBranchColor : visiableleafColor
					: IsBranch
						? invisiableBranchColor
						: invisiableleafColor;
				Gizmos.DrawWireCube(
					new(xMid, yMid, zMid),
					new(xMax - xMin, yMax - yMin, zMax - zMin)
				);
				if (!IsBranch) return;
				for (var i = 0; i < 8; i++)
				{
					var child = children[i];
					child?.Editor_DrawGizmos(planes);
				}
			}
#endif
		}
	}
}
