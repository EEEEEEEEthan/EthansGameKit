using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Internal
{
	class BfsTransformAccessor : IEnumerator<Transform>, IEnumerable<Transform>
	{
		public static BfsTransformAccessor Generate(
			Transform root,
			bool includeSelf,
			bool includeInactive)
		{
			if (!GlobalCachePool<BfsTransformAccessor>.TryGenerate(out var generator))
				generator = new();
			generator.root = root;
			generator.started = false;
			generator.includeSelf = includeSelf;
			generator.includeInactive = includeInactive;
			return generator;
		}
		readonly Queue<Transform> queue = new();
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
				if (queue.Count == 0)
					return false;
				var top = queue.Dequeue();
				if (includeInactive)
				{
					var cnt = top.childCount;
					for (var i = 0; i < cnt; ++i)
					{
						var child = top.GetChild(i);
						if (child.gameObject.activeSelf)
							queue.Enqueue(child);
					}
				}
				else
				{
					var cnt = top.childCount;
					for (var i = 0; i < cnt; ++i)
					{
						var child = top.GetChild(i);
						queue.Enqueue(child);
					}
				}
				return true;
			}
			started = true;
			queue.Enqueue(root);
			if (includeSelf)
			{
				Current = root;
				return true;
			}
			return MoveNext();
		}
		public void Reset()
		{
			queue.Clear();
			started = false;
		}
		public void Dispose()
		{
			Reset();
			GlobalCachePool<BfsTransformAccessor>.Recycle(this);
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
