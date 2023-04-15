using System;

namespace EthansGameKit.CachePools
{
	public static class GlobalCachePool<T> where T : class
	{
		static readonly CachePool<T> pool = new(0);
		static readonly Type type = typeof(T);
		public static void Recycle(ref T item)
		{
			Recycle(item);
			item = null;
		}
		public static void Recycle(T item)
		{
			if (item.GetType() != typeof(T))
				throw new ArgumentException($"The type of the item is not {type.Name}.");
			pool.Recycle(ref item);
		}
		public static bool TryGenerate(out T value)
		{
			return pool.TryGenerate(out value);
		}
	}
}
