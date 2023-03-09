using System;
using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
	public static class StackPool<T>
	{
		static readonly CachePool pool = new(0);
		public static void ClearAndRecycle(ref Stack<T> list)
		{
			var obj = (object)list;
			list.Clear();
			pool.Recycle(ref obj);
			list = null;
		}
		public static Stack<T> Generate()
		{
			if (pool.TryGenerate(out var cache))
				return (Stack<T>)cache;
			return new();
		}
	}
}
