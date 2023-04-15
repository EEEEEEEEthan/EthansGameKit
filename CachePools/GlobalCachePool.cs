using System;
using UnityEngine.Assertions;

namespace EthansGameKit.CachePools
{
	public static class GlobalCachePool<T> where T : class
	{
		static readonly CachePool<T> pool = new(0);
		static readonly Type type = typeof(T);
		public static void Recycle(ref T item)
		{
			Assert.IsTrue(item.GetType() == type);
			pool.Recycle(ref item);
			item = default;
		}
		public static bool TryGenerate(out T value)
		{
			return pool.TryGenerate(out value);
		}
	}
}
