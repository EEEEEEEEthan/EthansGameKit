using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Internal
{
	abstract class OctreeNodeEnumerator<T> : IEnumerator<OctreeItem<T>>, IEnumerable<OctreeItem<T>>
	{
		public abstract OctreeItem<T> Current { get; }
		object IEnumerator.Current => Current;
		public abstract void Dispose();
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
		public IEnumerator<OctreeItem<T>> GetEnumerator()
		{
			return this;
		}
		public abstract bool MoveNext();
		public abstract void Reset();
	}
}
