using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Internal
{
	class DfsTransformAccessor : IEnumerator<Transform>, IEnumerable<Transform>
	{
		public static DfsTransformAccessor Generate(
			Transform root,
			bool includeSelf,
			bool includeInactive)
		{
			if (!GlobalCachePool<DfsTransformAccessor>.TryGenerate(out var generator))
				generator = new();
			generator.root = root;
			generator.started = false;
			generator.includeSelf = includeSelf;
			generator.includeInactive = includeInactive;
			return generator;
		}
		readonly Stack<Transform> stack = new();
		bool started;
		bool includeInactive;
		bool includeSelf;
		Transform root;
		public Transform Current { get; private set; }
		object IEnumerator.Current => Current;
		public bool MoveNext()
		{
			if (started)
			{
				if (stack.Count == 0)
					return false;
				var top = stack.Pop();
				if (includeInactive)
				{
					for (var i = top.childCount - 1; i >= 0; i--)
					{
						var child = top.GetChild(i);
						if (child.gameObject.activeSelf)
							stack.Push(child);
					}
				}
				else
				{
					for (var i = top.childCount - 1; i >= 0; i--)
					{
						var child = top.GetChild(i);
						stack.Push(child);
					}
				}
				return true;
			}
			started = true;
			stack.Push(root);
			if (includeSelf)
			{
				Current = root;
				return true;
			}
			return MoveNext();
		}
		public void Reset()
		{
			stack.Clear();
			started = false;
		}
		public void Dispose()
		{
			Reset();
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
	}
}
