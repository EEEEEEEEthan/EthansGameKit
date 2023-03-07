using System.Collections.Generic;

namespace EthansGameKit.CachePools
{
    public static class ListPool<T>
    {
        static readonly CachePool pool = new(0);
        public static List<T> Generate()
        {
            if (pool.TryGenerate(out var cache))
                return (List<T>)cache;
            return new();
        }
        public static void ClearAndRecycle(ref List<T> list)
        {
            var obj = (object)list;
            list.Clear();
            pool.Recycle(ref obj);
            list = null;
        }
    }
}