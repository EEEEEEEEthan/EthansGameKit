using System;
using UnityEngine.Assertions;

namespace EthansGameKit.CachePools
{
	public static class GlobalCachePool<T> where T : new()
	{
		static readonly Type type = typeof(T);
		static readonly CachePool pool = new(0);
		public static void Recycle(ref T obj)
		{
			Assert.IsTrue(obj.GetType() == type);
			var o = (object)obj;
			pool.Recycle(ref o);
			obj = default;
		}
		public static bool TryGenerate(out T value)
		{
			if (pool.TryGenerate(out var v))
			{
				value = (T)v;
				return true;
			}
			value = default;
			return false;
		}
	}
}
