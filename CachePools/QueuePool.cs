using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public static class QueuePool<T>
	{
		static readonly CachePool<Queue<T>> pool = new(0);
		public static void ClearAndRecycle(ref Queue<T> queue)
		{
			queue.Clear();
			pool.Recycle(ref queue);
			queue = null;
		}
		public static Queue<T> Generate()
		{
			if (pool.TryGenerate(out var cache))
				return cache;
			return new();
		}
	}
}