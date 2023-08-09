// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using EthansGameKit.Internal;
using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static Transform Find(this Transform @this, string path, bool includeInactive)
		{
			if (!includeInactive)
				return @this.Find(path);
			var nodeNames = path.Split('/');
			var transform = @this;
			foreach (var nodeName in nodeNames)
			{
				transform = transform.Find(nodeName);
				if (!transform)
					return null;
			}
			return transform;
		}
		public static bool TryFind(this Transform @this, string path, bool includeInactive, out Transform child)
		{
			return child = @this.Find(path, includeInactive);
		}
		/// <summary>
		///     所有子节点,不包括自己
		/// </summary>
		/// <remarks>
		///     不保证Hierarchy变化后能正常工作.
		/// </remarks>
		/// <param name="this"></param>
		/// <param name="includeSelf"></param>
		/// <returns></returns>
		public static IEnumerable<Transform> IterChildren(this Transform @this, bool includeSelf)
		{
			return DfsTransformAccessor.Generate(@this, includeSelf);
		}
		/// <summary>
		///     所有子节点,不包括自己
		/// </summary>
		/// <remarks>
		///     不保证Hierarchy变化后能正常工作.
		/// </remarks>
		/// <param name="this"></param>
		/// <param name="includeSelf"></param>
		/// <returns></returns>
		public static IEnumerable<Transform> IterChildren(this Transform @this, bool includeSelf, Func<Transform, bool> prune)
		{
			return DfsTransformAccessor.Generate(@this, includeSelf);
		}
		public static bool TryGetHierarchyPath(this Transform @this, Transform parent, out string path)
		{
			path = null;
			if (@this == parent) return false;
			bool result;
			var stack = StackPool<string>.Generate();
			while (true)
			{
				if (@this == parent)
				{
					path = stack.Join(",");
					result = true;
					break;
				}
				if (!@this)
				{
					result = false;
					break;
				}
				stack.Push(@this.name);
				@this = @this.parent;
			}
			StackPool<string>.ClearAndRecycle(ref stack);
			return result;
		}
		public static Transform FindOrAdd(this Transform @this, string path, out bool isNew)
		{
			var nodeNames = path.Split('/');
			var transform = @this;
			isNew = false;
			foreach (var nodeName in nodeNames)
			{
				var child = transform.Find(nodeName);
				if (!child)
				{
					var obj = new GameObject(nodeName);
					child = obj.transform;
					child.SetParent(transform);
					child.SetLocalPositionAndRotation(default, Quaternion.identity);
					child.localScale = Vector3.one;
					isNew = true;
				}
				transform = child;
			}
			return transform;
		}
	}
}
