using System;
using System.Collections.Generic;
using EthansGameKit.Internal;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace EthansGameKit.Collections
{
	class OctreeDefines
	{
		protected internal static readonly Plane[] planes = new Plane[6];
		protected internal static void RecalculatePlanes(Camera camera, Matrix4x4 worldToLocal, float expansion)
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
		protected internal static readonly Color editor_invisiableBranchColor = new(1, 1, 1, 0.05f);
		protected internal static readonly Color editor_visiableBranchColor = new(1, 1, 1, 0.1f);
		protected internal static readonly Color editor_invisiableleafColor = new(0, 1, 0, 0.25f);
		protected internal static readonly Color editor_visiableleafColor = new(0, 1, 0, 0.5f);
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
	public class Octree<T> : ISerializationCallbackReceiver
	{
		OctreeNode<T> root;
		readonly List<OctreeItem<T>> serializedData = new();
		public IEnumerable<OctreeItem<T>> AllItems => root is null ? Array.Empty<OctreeItem<T>>() : root.AllItems;
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
		public OctreeItem<T> Insert(Vector3 position, T obj)
		{
			var item = OctreeItem<T>.Generate(position, obj);
			Insert(item);
			return item;
		}
		public void Clear()
		{
			root = null;
		}
		public void RemoveAndRecycle(ref OctreeItem<T> octreeItem)
		{
			Remove(octreeItem);
			OctreeItem<T>.ClearAndRecycle(ref octreeItem);
		}
		public IEnumerable<OctreeItem<T>> Query(Vector3 center, float maxDistance)
		{
			return root is null ? Array.Empty<OctreeItem<T>>() : root.Query(center, maxDistance);
		}
		public bool InScreen(Camera camera, Matrix4x4 worldToLocal, float expansion, OctreeItem<T> octreeItem)
		{
			OctreeDefines.RecalculatePlanes(camera, worldToLocal, expansion);
			var bounds = new Bounds(octreeItem.Position, Vector3.zero);
			return GeometryUtility.TestPlanesAABB(OctreeDefines.planes, bounds);
		}
		public void DrawGizmos(Camera camera, Matrix4x4 worldToLocal, float expansion)
		{
#if UNITY_EDITOR
			OctreeDefines.RecalculatePlanes(camera, worldToLocal, expansion);
			root?.Editor_DrawGizmos(OctreeDefines.planes);
#endif
		}
		internal void Update(OctreeItem<T> octreeItem, float newX, float newY, float newZ)
		{
			Assert.IsNotNull(octreeItem.Tree);
			if (!root.Contains(newX, newY, newZ)) root = root.Encapsulate(newX, newY, newZ);
			var pos = octreeItem.Position;
			root.Update(octreeItem, pos.x, pos.y, pos.z, newX, newY, newZ);
			if (root.IsBranch)
				if (root.TryGetTheOnlyChild(out var theOnlyChild))
					root = theOnlyChild;
		}
		void Insert(OctreeItem<T> octreeItem)
		{
			Assert.IsNull(octreeItem.Tree);
			var pos = octreeItem.Position;
			root ??= OctreeNode<T>.Generate(
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
			root.Insert(octreeItem, pos.x, pos.y, pos.z);
			octreeItem.Tree = this;
		}
		void Remove(OctreeItem<T> octreeItem)
		{
			Assert.IsNotNull(octreeItem.Tree);
			var pos = octreeItem.Position;
			root.Remove(octreeItem, pos.x, pos.y, pos.z);
			if (root.IsBranch)
				if (root.TryGetTheOnlyChild(out var theOnlyChild))
					root = theOnlyChild;
			if (root.IsEmpty) root = null;
			octreeItem.Tree = null;
		}
	}
}
