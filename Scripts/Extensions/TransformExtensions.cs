using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static IEnumerable<Transform> DfsChildren(this Transform root)
		{
			return new DfsChildrenEnumerator(root);
		}

		struct DfsChildrenEnumerator : IEnumerator<Transform>, IEnumerable<Transform>
		{
			readonly Transform root;
			Stack<Transform> stack;

			public DfsChildrenEnumerator(Transform root)
			{
				this.root = root;
				stack = null;
			}

			public Transform Current => stack.Peek();

			object IEnumerator.Current => Current;

			public bool MoveNext()
			{
				if (stack is null)
				{
					stack = new();
					stack.Push(root);
					return true;
				}
				if (!stack.TryPop(out var element)) return false;
				var count = element.childCount;
				for (var i = count - 1; i >= 0; i--) stack.Push(element.GetChild(i));
				return true;
			}

			public void Reset()
			{
				stack = null;
			}

			public void Dispose()
			{
			}

			public IEnumerator<Transform> GetEnumerator()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}
		}
	}
}