using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Internal
{
	struct DfsTransformAccessor : IEnumerator<Transform>, IEnumerable<Transform>
	{
		readonly bool includeSelf;
		readonly Transform root;
		readonly Func<Transform, bool> valid;
		Stack<Transform> stack;
		public Transform Current { get; private set; }
		object IEnumerator.Current => Current;
		public DfsTransformAccessor(Transform root, bool includeSelf, Func<Transform, bool> valid = null)
		{
			stack = StackPool<Transform>.Generate();
			stack.Push(root);
			Current = default;
			this.root = root;
			this.includeSelf = includeSelf;
			this.valid = valid;
			if (!includeSelf)
			{
				MoveNext();
				Current = default;
			}
		}
		public bool MoveNext()
		{
			if (stack.TryPop(out var current))
			{
				Current = current;
				for (var i = current.childCount; i-- > 0;)
				{
					var child = current.GetChild(i);
					if (valid != null && !valid(child)) continue;
					stack.Push(child);
				}
				return true;
			}
			return false;
		}
		public void Reset()
		{
			stack.Clear();
			stack.Push(root);
			Current = default;
			if (!includeSelf)
			{
				MoveNext();
				Current = default;
			}
		}
		public void Dispose()
		{
			stack.ClearAndRecycle();
			stack = null;
		}
		public IEnumerator<Transform> GetEnumerator() => this;
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
