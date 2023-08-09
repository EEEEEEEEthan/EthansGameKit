using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public class DictionaryPool<TKey, TValue>
	{
		static readonly CachePool<Dictionary<TKey, TValue>> pool = new(0);
		public static void ClearAndRecycle(ref Dictionary<TKey, TValue> dict)
		{
			dict.Clear();
			pool.Recycle(ref dict);
			dict = null;
		}
		public static Dictionary<TKey, TValue> Generate()
		{
			if (pool.TryGenerate(out var cache))
				return cache;
			return new();
		}
	}
}
