using EthansGameKit.CachePools;

namespace EthansGameKit.Internal
{
	class LeafEnumerator<T> : OctreeNodeEnumerator<T>
	{
		public static LeafEnumerator<T> Generate(OctreeNode<T> node)
		{
			if (!GlobalCachePool<LeafEnumerator<T>>.TryGenerate(out var enumerator))
				enumerator = new();
			enumerator.node = node;
			enumerator.Reset();
			return enumerator;
		}
		OctreeNode<T> node;
		int index;
		public override OctreeItem<T> Current => node.items[index];
		LeafEnumerator()
		{
		}
		public override void Dispose()
		{
			node = null;
			GlobalCachePool<LeafEnumerator<T>>.Recycle(this);
		}
		public override bool MoveNext()
		{
			++index;
			return index < node.items.Count;
		}
		public override void Reset()
		{
			index = -1;
		}
	}
}
