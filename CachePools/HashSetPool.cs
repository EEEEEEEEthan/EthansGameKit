using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public static class HashSetPool<T>
	{
		static readonly CachePool<HashSet<T>> pool = new(0);
		public static void ClearAndRecycle(ref HashSet<T> set)
		{
			set.Clear();
			pool.Recycle(ref set);
			set = null;
		}
		public static HashSet<T> Generate()
		{
			if (pool.TryGenerate(out var cache))
				return cache;
			return new();
		}
		public static HashSet<T> Generate(IEnumerable<T> collection)
		{
			if (pool.TryGenerate(out var cache))
			{
				foreach (var item in collection)
					cache.Add(item);
				return cache;
			}
			return new(collection);
		}
		public static HashSet<T> Generate(IReadOnlyList<T> list)
		{
			if (pool.TryGenerate(out var cache))
			{
				var length = list.Count;
				for (var i = 0; i < length; ++i)
					cache.Add(list[i]);
			}
			return new(list);
		}
	}
}