using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace EthansGameKit.Collections
{
	/// <summary>
	///     自适应八叉树
	/// </summary>
	/// <remarks>
	///     <list type="bullet">
	///         <item>能根据插入物品坐标自动调整大小.</item>
	///         <item>不会因为同坐标物品数量过多导致栈溢出</item>
	///     </list>
	/// </remarks>
	public partial class Octree<T>
	{
		static readonly Plane[] planes = new Plane[6];
		static void RecalculatePlanes(Camera camera, Matrix4x4 worldToLocal, float expansion)
		{
			GeometryUtility.CalculateFrustumPlanes(camera, planes);
			var cameraPos = camera.transform.position;
			for (var i = 0; i < 6; i++)
			{
				var plane = planes[i];
				var normal = plane.normal;
				var point = plane.ClosestPointOnPlane(cameraPos) - normal * expansion;
				point = worldToLocal.MultiplyPoint3x4(point);
				normal = worldToLocal.MultiplyVector(plane.normal);
				planes[i] = new(normal, point);
			}
		}
		Node root;
		public IEnumerable<Item> AllItems => root is null ? Array.Empty<Item>() : root.allItems;
		public Item Insert(Vector3 position)
		{
			var item = Item.Generate(position);
			Insert(item);
			return item;
		}
		public void RemoveAndRecycle(ref Item item)
		{
			Remove(item);
			Item.Recycle(ref item);
		}
		public int Query(ref Item[] result, Matrix4x4 worldToLocal, Camera camera, float expansion)
		{
			if (root is null) return 0;
			RecalculatePlanes(camera, worldToLocal, expansion);
			var count = 0;
			root.Query(ref result, planes, ref count);
			return count;
		}
		public void Query(ICollection<Item> result, Matrix4x4 worldToLocal, Camera camera, float expansion)
		{
			result.Clear();
			if (root is null) return;
			RecalculatePlanes(camera, worldToLocal, expansion);
			root.Query(result, planes);
		}
		public void Query(ICollection<Item> result, Vector3 center, float radius)
		{
			result.Clear();
			root?.Query(result, center, radius * radius);
		}
		public bool InScreen(Camera camera, Matrix4x4 worldToLocal, float expansion, Item item)
		{
			RecalculatePlanes(camera, worldToLocal, expansion);
			var bounds = new Bounds(item.Position, Vector3.zero);
			return GeometryUtility.TestPlanesAABB(planes, bounds);
		}
		public void DrawGizmos(Camera camera, Matrix4x4 worldToLocal, float expansion)
		{
#if UNITY_EDITOR
			RecalculatePlanes(camera, worldToLocal, expansion);
			root?.Editor_DrawGizmos(planes);
#endif
		}
		void Insert(Item item)
		{
			Assert.IsNull(item.Tree);
			var pos = item.Position;
			root ??= Node.Generate(
				pos.x - 1,
				pos.x,
				pos.x + 1,
				pos.y - 1,
				pos.y,
				pos.y + 1,
				pos.z - 1,
				pos.z,
				pos.z + 1
			);
			if (!root.Contains(pos.x, pos.y, pos.z)) root = root.Encapsulate(pos.x, pos.y, pos.z);
			root.Insert(item, pos.x, pos.y, pos.z);
			item.Tree = this;
		}
		void Remove(Item item)
		{
			Assert.IsNotNull(item.Tree);
			var pos = item.Position;
			root.Remove(item, pos.x, pos.y, pos.z);
			if (root.IsBranch)
				if (root.TryGetTheOnlyChild(out var theOnlyChild))
					root = theOnlyChild;
			if (root.IsEmpty) root = null;
			item.Tree = null;
		}
	}
}
