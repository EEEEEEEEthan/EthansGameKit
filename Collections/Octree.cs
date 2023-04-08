﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace EthansGameKit.Collections
{
	static class OctreeDefines
	{
		internal static readonly Plane[] planes = new Plane[6];
		internal static void RecalculatePlanes(Camera camera, Matrix4x4 worldToLocal, float expansion)
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
#if UNITY_EDITOR
		internal static readonly Color editor_invisiableBranchColor = new(1, 1, 1, 0.05f);
		internal static readonly Color editor_visiableBranchColor = new(1, 1, 1, 0.1f);
		internal static readonly Color editor_invisiableLeafColor = new(0, 1, 0, 0.25f);
		internal static readonly Color editor_visiableLeafColor = new(0, 1, 0, 0.5f);
#endif
	}

	/// <summary>
	///     自适应八叉树
	/// </summary>
	/// <remarks>
	///     <list type="bullet">
	///         <item>能根据插入物品坐标自动调整大小.</item>
	///         <item>不会因为同坐标物品数量过多导致栈溢出</item>
	///     </list>
	/// </remarks>
	public partial class Octree<T> : ISerializationCallbackReceiver
	{
		Node root;
		readonly List<Item> serializedData = new();
		public IEnumerable<Item> AllItems => root is null ? Array.Empty<Item>() : root.AllItems;
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			serializedData.Clear();
			foreach (var data in AllItems)
				serializedData.Add(data);
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Clear();
			foreach (var data in serializedData)
				Insert(data);
			serializedData.Clear();
		}
		public Item Insert(Vector3 position, T obj)
		{
			var item = Item.Generate(position, obj);
			Insert(item);
			return item;
		}
		public void Clear()
		{
			root = null;
		}
		public void RemoveAndRecycle(ref Item item)
		{
			Remove(item);
			Item.Recycle(ref item);
		}
		public int Query(ref Item[] result, Matrix4x4 worldToLocal, Camera camera, float expansion)
		{
			if (root is null) return 0;
			OctreeDefines.RecalculatePlanes(camera, worldToLocal, expansion);
			var count = 0;
			root.Query(ref result, OctreeDefines.planes, ref count);
			return count;
		}
		public void Query(ICollection<Item> result, Matrix4x4 worldToLocal, Camera camera, float expansion)
		{
			result.Clear();
			if (root is null) return;
			OctreeDefines.RecalculatePlanes(camera, worldToLocal, expansion);
			root.Query(result, OctreeDefines.planes);
		}
		public void Query(ICollection<Item> result, Vector3 center, float radius)
		{
			result.Clear();
			root?.Query(result, center, radius * radius);
		}
		public bool InScreen(Camera camera, Matrix4x4 worldToLocal, float expansion, Item item)
		{
			OctreeDefines.RecalculatePlanes(camera, worldToLocal, expansion);
			var bounds = new Bounds(new(item.x, item.y, item.z), Vector3.zero);
			return GeometryUtility.TestPlanesAABB(OctreeDefines.planes, bounds);
		}
		public void DrawGizmos(Camera camera, Matrix4x4 worldToLocal, float expansion)
		{
#if UNITY_EDITOR
			OctreeDefines.RecalculatePlanes(camera, worldToLocal, expansion);
			root?.Editor_DrawGizmos(OctreeDefines.planes);
#endif
		}
		void Insert(Item item)
		{
			Assert.IsNull(item.Tree);
			root ??= Node.Generate(
				item.x - 1,
				item.x,
				item.x + 1,
				item.y - 1,
				item.y,
				item.y + 1,
				item.z - 1,
				item.z,
				item.z + 1
			);
			if (!root.Contains(item.x, item.y, item.z)) root = root.Encapsulate(item.x, item.y, item.z);
			root.Insert(item, item.x, item.y, item.z);
			item.Tree = this;
		}
		void Remove(Item item)
		{
			Assert.IsNotNull(item.Tree);
			root.Remove(item, item.x, item.y, item.z);
			if (root.IsBranch)
				if (root.TryGetTheOnlyChild(out var theOnlyChild))
					root = theOnlyChild;
			if (root.IsEmpty) root = null;
			item.Tree = null;
		}
		void Update(Item item, float oldX, float oldY, float oldZ, float newX, float newY, float newZ)
		{
			Assert.IsNotNull(item.Tree);
			root.Update(item, oldX, oldY, oldZ, newX, newY, newZ);
			if (root.IsBranch)
				if (root.TryGetTheOnlyChild(out var theOnlyChild))
					root = theOnlyChild;
		}
	}
}
