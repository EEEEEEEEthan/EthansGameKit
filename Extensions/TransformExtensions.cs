// ReSharper disable once CheckNamespace

using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
        /// <summary>
        ///     节点迭代器.foreach结束后会自动回收至内存池,从而减少GC.
        /// </summary>
        /// <remarks>
        ///     不保证Hierarchy变化后能正常工作.
        /// </remarks>
        class DfsTransformEnumerator : IEnumerator<Transform>, IEnumerable<Transform>
		{
			public static DfsTransformEnumerator Generate(
				Transform root,
				bool includeSelf,
				bool includeInactive,
				bool recursive)
			{
				if (!GlobalCachePool<DfsTransformEnumerator>.TryGenerate(out var generator))
					generator = new();
				generator.root = root;
				generator.childIndex = -1;
				generator.includeSelf = includeSelf;
				generator.includeInactive = includeInactive;
				generator.recursive = recursive;
				return generator;
			}
			int childIndex;
			bool includeInactive;
			bool includeSelf;
			bool recursive;
			Transform root;
			DfsTransformEnumerator subEnumerator;
			public Transform Current { get; private set; }
			object IEnumerator.Current => Current;
			public bool MoveNext()
			{
				if (childIndex < 0)
				{
					if (includeSelf)
					{
						if (includeInactive || root.gameObject.activeInHierarchy)
						{
							Current = root;
							childIndex = 0;
							return true;
						}
						return false;
					}
					childIndex = 0;
				}
				if (subEnumerator != null)
				{
					if (subEnumerator.MoveNext())
					{
						Current = subEnumerator.Current;
						return true;
					}
					subEnumerator.Dispose();
					subEnumerator = null;
				}
				while (childIndex < root.childCount)
				{
					var child = root.GetChild(childIndex);
					if (includeInactive || child.gameObject.activeSelf)
					{
						Current = child;
						childIndex++;
						if (recursive) subEnumerator = Generate(child, false, includeInactive, true);
						return true;
					}
					childIndex++;
				}
				return false;
			}
			public void Reset()
			{
				childIndex = 0;
				root = null;
				subEnumerator = null;
				Current = null;
			}
			public void Dispose()
			{
				Reset();
				GlobalCachePool<DfsTransformEnumerator>.Recycle(this);
			}
			IEnumerator<Transform> IEnumerable<Transform>.GetEnumerator()
			{
				return this;
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}
		}

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
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static IEnumerable<Transform> IterChildren(this Transform @this, bool includeInactive, bool recursive)
		{
			return DfsTransformEnumerator.Generate(@this, false, includeInactive, recursive);
		}
        /// <summary>
        ///     所有子节点
        /// </summary>
        /// <remarks>
        ///     不保证Hierarchy变化后能正常工作.
        /// </remarks>
        /// <param name="this"></param>
        /// <param name="includeInactive"></param>
        /// <param name="recursive"></param>
        /// <param name="includeSelf"></param>
        /// <returns></returns>
        public static IEnumerable<Transform> IterChildren(
			this Transform @this,
			bool includeInactive,
			bool recursive,
			bool includeSelf)
		{
			return DfsTransformEnumerator.Generate(@this, includeSelf, includeInactive, recursive);
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
