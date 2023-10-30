using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Internal
{
	class DfsTransformAccessor : IEnumerator<Transform>, IEnumerable<Transform>
	{
		/// <summary>
		///     dfs节点迭代器
		/// </summary>
		/// <param name="root">根节点</param>
		/// <param name="prune">剪枝函数。返回true时不会访问这个节点和他的子节点</param>
		/// <param name="includeSelf">是否包含自己</param>
		/// <returns></returns>
		public static DfsTransformAccessor Generate(Transform root, bool includeSelf, Func<Transform, bool> prune = null)
		{
			if (!GlobalCachePool<DfsTransformAccessor>.TryGenerate(out var accessor))
				accessor = new();
			accessor.root = root;
			accessor.started = false;
			accessor.includeSelf = includeSelf;
			accessor.prune = prune ?? DefaultPrune;
			return accessor;
		}
		static bool DefaultPrune(Transform node)
		{
			return false;
		}
		readonly Stack<Transform> stack = new();
		bool started;
		bool includeSelf;
		Func<Transform, bool> prune;
		Transform root;
		public Transform Current => stack.Peek();
		object IEnumerator.Current => Current;
		public bool MoveNext()
		{
			if (!started)
			{
				started = true;
				if (Prune(root)) return false;
				stack.Push(root);
				if (includeSelf)
					return true;
			}
			if (!stack.TryPop(out var top)) return false;
			for (var i = top.childCount - 1; i >= 0; i--)
			{
				var child = top.GetChild(i);
				if (!Prune(child))
					stack.Push(child);
			}
			return true;
		}
		public void Reset()
		{
			stack.Clear();
			started = false;
		}
		public void Dispose()
		{
			Reset();
			root = null;
			prune = null;
			GlobalCachePool<DfsTransformAccessor>.Recycle(this);
		}
		IEnumerator<Transform> IEnumerable<Transform>.GetEnumerator()
		{
			return this;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
		bool Prune(Transform transform)
		{
			if (!transform) return true;
			try
			{
				return prune(transform);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			return true;
		}
	}
}
