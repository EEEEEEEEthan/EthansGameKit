using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public static class StackPool<T>
	{
		static readonly CachePool<Stack<T>> pool = new(0);
		public static void ClearAndRecycle(ref Stack<T> list)
		{
			list.Clear();
			pool.Recycle(ref list);
			list = null;
		}
		public static Stack<T> Generate()
		{
			if (pool.TryGenerate(out var cache))
				return cache;
			return new();
		}
	}
}
