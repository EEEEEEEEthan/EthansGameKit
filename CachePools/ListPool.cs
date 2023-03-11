using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public static class ListPool<T>
	{
		static readonly CachePool<List<T>> pool = new(0);
		public static void ClearAndRecycle(ref List<T> list)
		{
			list.Clear();
			pool.Recycle(ref list);
			list = null;
		}
		public static List<T> Generate()
		{
			if (pool.TryGenerate(out var cache))
				return cache;
			return new();
		}
		public static List<T> Generate(IEnumerable<T> collection)
		{
			if (pool.TryGenerate(out var cache))
			{
				foreach (var item in cache)
					cache.Add(item);
				return cache;
			}
			return new(collection);
		}
	}
}
