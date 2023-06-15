using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EthansGameKit.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit.Internal
{
	class OctreeNode<T>
	{
		public static OctreeNode<T> Generate(
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
			var node = new OctreeNode<T>
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
		internal OctreeNode<T>[] children;
		internal List<OctreeItem<T>> items;
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
		public IEnumerable<OctreeItem<T>> AllItems
		{
			get
			{
				if (IsBranch)
				{
					for (var i = 0; i < 8; i++)
					{
						var child = children[i];
						if (child != null)
							foreach (var item in child.AllItems)
								yield return item;
					}
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
		public IEnumerable<OctreeItem<T>> Query(Vector3 center, float distance)
		{
			var sqrMagnitude = distance * distance;
			if (IsBranch)
			{
				using var heap = Heap<IEnumerator<OctreeItem<T>>, float>.Generate();
				for (var i = 0; i < 8; ++i)
				{
					var child = children[i];
					if (child is null) continue;
					using var subEnumerator = child.Query(center, distance).GetEnumerator();
					if (!subEnumerator.MoveNext()) continue;
					var subItem = subEnumerator.Current;
					// ReSharper disable once PossibleNullReferenceException
					var subDistance = Vector3.SqrMagnitude(center - subItem.Position);
					heap.Add(subEnumerator, subDistance);
				}
				while (heap.Count > 0)
				{
					var enumerator = heap.Pop();
					yield return enumerator.Current;
					if (enumerator.MoveNext())
					{
						var subItem = enumerator.Current;
						// ReSharper disable once PossibleNullReferenceException
						var subDistance = Vector3.SqrMagnitude(center - subItem.Position);
						heap.Add(enumerator, subDistance);
					}
				}
			}
			var sqr = Vector3.SqrMagnitude(center - new Vector3(x, y, z));
			if (sqr >= sqrMagnitude) yield break;
			if (items is null) yield break;
			foreach (var item in items)
				yield return item;
		}
		public bool TryGetTheOnlyChild(out OctreeNode<T> firstChild)
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
			return xMin <= x && x < xMax && yMin <= y && y < yMax && zMin <= z && z < zMax;
		}
		public OctreeNode<T> Encapsulate(float x, float y, float z)
		{
			Assert.IsFalse(Contains(x, y, z));
			while (true)
			{
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
					var node = new OctreeNode<T>
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
					node.children = new OctreeNode<T>[8];
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
				if (Contains(x, y, z)) return this;
			}
		}
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public void Insert(OctreeItem<T> octreeItem, float x, float y, float z)
		{
		REINSERT:
			if (IsBranch)
			{
				var childIndex = GetChildIndex(x, y, z);
				var child = children[childIndex];
				if (child is null) children[childIndex] = child = CreateChild(x, y, z);
				child.Insert(octreeItem, x, y, z);
			}
			else
			{
				if (items == null)
				{
					this.x = x;
					this.y = y;
					this.z = z;
					items = new() { octreeItem };
					return;
				}
				if (x == this.x && y == this.y && z == this.z)
				{
					items.Add(octreeItem);
					return;
				}
				Subdivide();
				Assert.IsTrue(IsBranch);
				goto REINSERT;
			}
		}
		public void Remove(OctreeItem<T> octreeItem, float x, float y, float z)
		{
			if (IsBranch)
			{
				var childIndex = GetChildIndex(x, y, z);
				var child = children[childIndex];
				child.Remove(octreeItem, x, y, z);
				if (child.IsEmpty) children[childIndex] = null;
				TryShorten();
				return;
			}
			items.Remove(octreeItem);
			if (items.Count == 0) items = null;
		}
		public void Update(OctreeItem<T> octreeItem, float x0, float y0, float z0, float x1, float y1, float z1)
		{
			if (IsBranch)
			{
				var childIndex0 = GetChildIndex(x0, y0, z0);
				var childIndex1 = GetChildIndex(x1, y1, z1);
				if (childIndex0 == childIndex1)
				{
					var child = children[childIndex0];
					child.Update(octreeItem, x0, y0, z0, x1, y1, z1);
					TryShorten();
					return;
				}
			}
			Remove(octreeItem, x0, y0, z0);
			Insert(octreeItem, x1, y1, z1);
		}
		void TryShorten()
		{
			if (!TryGetTheOnlyChild(out var theOnlyChild)) return;
			if (theOnlyChild.IsBranch) return;
			x = theOnlyChild.x;
			y = theOnlyChild.y;
			z = theOnlyChild.z;
			children = null;
			items = theOnlyChild.items;
		}
		void Subdivide()
		{
			children = new OctreeNode<T>[8];
			foreach (var item in items)
			{
				var childIndex = GetChildIndex(item.Position.x, item.Position.y, item.Position.z);
				var child = children[childIndex];
				if (child is null)
				{
					children[childIndex] = child = CreateChild(
						item.x,
						item.y,
						item.z
					);
				}
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
		OctreeNode<T> CreateChild(float x, float y, float z)
		{
			var child = new OctreeNode<T>();
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
		public void Editor_DrawGizmos(Plane[] planes)
		{
			var bounds = new Bounds(new(xMid, yMid, zMid), new(xMax - xMin, yMax - yMin, zMax - zMin));
			Gizmos.color = GeometryUtility.TestPlanesAABB(planes, bounds)
				? IsBranch ? OctreeDefines.editor_visiableBranchColor : OctreeDefines.editor_visiableleafColor
				: IsBranch
					? OctreeDefines.editor_invisiableBranchColor
					: OctreeDefines.editor_invisiableleafColor;
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
		public void Editor_DrawGizmos()
		{
			Gizmos.color = IsBranch
				? OctreeDefines.editor_invisiableBranchColor
				: OctreeDefines.editor_invisiableleafColor;
			Gizmos.DrawWireCube(
				new(xMid, yMid, zMid),
				new(xMax - xMin, yMax - yMin, zMax - zMin)
			);
			if (!IsBranch) return;
			for (var i = 0; i < 8; i++)
			{
				var child = children[i];
				child?.Editor_DrawGizmos();
			}
		}
#endif
	}
}
