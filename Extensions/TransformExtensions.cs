// ReSharper disable once CheckNamespace

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
				if (!transform)
				{
					var obj = GameObject.Find(nodeName);
					return !obj ? null : obj.transform;
				}
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
		/// <param name="includeInactive"></param>
		/// <returns></returns>
		public static IEnumerable<Transform> IterChildren(this Transform @this, bool includeInactive)
		{
			return DfsTransformAccessor.Generate(@this, false, includeInactive);
		}
		/// <summary>
		///     所有子节点
		/// </summary>
		/// <remarks>
		///     不保证Hierarchy变化后能正常工作.
		/// </remarks>
		/// <param name="this"></param>
		/// <param name="includeInactive"></param>
		/// <param name="includeSelf"></param>
		/// <returns></returns>
		public static IEnumerable<Transform> IterChildren(
			this Transform @this,
			bool includeInactive,
			bool includeSelf)
		{
			return DfsTransformAccessor.Generate(@this, includeSelf, includeInactive);
		}
		/// <summary>
		///     获取@this相对于parent的路径(路径将不会包含parent.name).若@this不是parent的子节点,返回false
		/// </summary>
		/// <param name="this"></param>
		/// <param name="parent"></param>
		/// <param name="path"></param>
		/// <returns></returns>
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
	}
}
