using System;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static ITimedCache<T> Cast<T>(this ITimedCache @this)
		{
			return new CacheConverter<T>(@this);
		}
		public static ITimedCache<T> Cast<T>(this ITimedCache @this, Func<object, T> converter)
		{
			return new TimedCache<T>(getValue, asyncGetValue);

			T getValue()
			{
				return converter(@this.Value);
			}

			async void asyncGetValue(Action<T> callback)
			{
				await @this.LoadAsync();
				callback(converter(@this.Value));
			}
		}
	}
}
