using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public static class ArrayCachePool<T>
	{
		static readonly Dictionary<int, CachePool<T[]>> pools = new();
		public static T[] Generate(int length)
		{
			if (!GetPool(length).TryGenerate(out var cache))
				cache = new T[length];
			return cache;
		}
		public static void Recycle(ref T[] array)
		{
			GetPool(array.Length).Recycle(ref array);
		}
		static CachePool<T[]> GetPool(int length)
		{
			if (!pools.TryGetValue(length, out var pool))
				pools[length] = pool = new(0);
			return pool;
		}
	}
}
