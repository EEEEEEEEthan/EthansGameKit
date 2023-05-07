
// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static ITimedCache<T> Cast<T>(this ITimedCache @this)
		{
			return new CacheConverter<T>(@this);
		}
	}
}
