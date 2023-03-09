using System;
using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public static class QueuePool<T>
	{
		static readonly CachePool pool = new(0);
		public static void ClearAndRecycle(ref Queue<T> queue)
		{
			var obj = (object)queue;
			queue.Clear();
			pool.Recycle(ref obj);
			queue = null;
		}
		public static Queue<T> Generate()
		{
			if (pool.TryGenerate(out var cache))
				return (Queue<T>)cache;
			return new();
		}
	}
}
