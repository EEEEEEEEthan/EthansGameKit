using System.Text;

namespace EthansGameKit.CachePools
{
	public static class StringBuilderPool
	{
		static readonly CachePool<StringBuilder> pool = new(0);
		public static void ClearAndRecycle(ref StringBuilder builder)
		{
			builder.Clear();
			pool.Recycle(ref builder);
			builder = null;
		}
		public static StringBuilder Generate()
		{
			return pool.TryGenerate(out var cache) ? cache : new();
		}
	}
}
