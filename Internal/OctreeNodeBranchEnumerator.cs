using System.Collections.Generic;
using EthansGameKit.CachePools;
using EthansGameKit.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit.Internal
{
	class OctreeNodeBranchEnumerator<T> : OctreeNodeEnumerator<T>
	{
		public static OctreeNodeBranchEnumerator<T> Generate(OctreeNode<T> node, Vector3 center, float sqrMagnitude)
		{
			if (!GlobalCachePool<OctreeNodeBranchEnumerator<T>>.TryGenerate(out var enumerator))
				enumerator = new();
			enumerator.node = node;
			enumerator.center = center;
			enumerator.maxSqrMagnitude = sqrMagnitude;
			enumerator.Reset();
			return enumerator;
		}
		Heap<IEnumerator<OctreeItem<T>>, float> heap;
		OctreeNode<T> node;
		Vector3 center;
		float maxSqrMagnitude;
		public override OctreeItem<T> Current => heap.Peek().Current;
		public override void Dispose()
		{
			heap.Dispose();
			heap = null;
			node = null;
			GlobalCachePool<OctreeNodeBranchEnumerator<T>>.Recycle(this);
		}
		public override bool MoveNext()
		{
			if (heap is null)
			{
				heap = Heap<IEnumerator<OctreeItem<T>>, float>.Generate();
				for (var i = 0; i < 8; ++i)
				{
					var child = node.children[i];
					if (child is null) continue;
					var subEnumerator = child.Query(center, maxSqrMagnitude).GetEnumerator();
					subEnumerator.MoveNext();
					Assert.IsNotNull(subEnumerator.Current);
					heap.Add(subEnumerator, (center - subEnumerator.Current.Position).sqrMagnitude);
				}
			}
			while (heap.Count > 0)
			{
				var enumerator = heap.Pop();
				if (enumerator.MoveNext())
				{
					Assert.IsNotNull(enumerator.Current);
					var sqr = (center - enumerator.Current.Position).sqrMagnitude;
					if (sqr >= maxSqrMagnitude) continue;
					heap.Add(enumerator, sqr);
				}
				else
				{
					enumerator.Dispose();
				}
			}
			return false;
		}
		public override void Reset()
		{
			heap.Dispose();
			heap = null;
		}
	}
}
