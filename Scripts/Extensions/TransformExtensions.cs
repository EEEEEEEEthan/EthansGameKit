using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		struct DfsChildrenEnumerator : IEnumerator<Transform>, IEnumerable<Transform>
		{
			readonly Stack<Transform> stack;
			Transform root;

			public DfsChildrenEnumerator(Transform root)
			{
				this.root = root;
				stack = null;
			}

			public Transform Current => stack.Peek();

			object IEnumerator.Current => Current;

			public bool MoveNext()
			{
				if (!stack.TryPop(out var element))
				{
					return false;
				}
				stack.
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public IEnumerator<Transform> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}