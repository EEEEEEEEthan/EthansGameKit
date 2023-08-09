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
		public static IAwaitable<object> GetValueAsync(this ITimedCache @this)
		{
			var awaitable = IAwaitable<object>.Create(out var handle);
			get();
			return awaitable;

			async void get()
			{
				if (@this.HasValue)
				{
					await TaskQueue.AwaitFreeFrame();
					handle.Set(@this.Value);
				}
				await @this.LoadAsync();
				handle.Set(@this.Value);
			}
		}
		public static IAwaitable<T> GetValueAsync<T>(this ITimedCache<T> @this)
		{
			var awaitable = IAwaitable<T>.Create(out var handle);
			get();
			return awaitable;

			async void get()
			{
				if (@this.HasValue)
				{
					await TaskQueue.AwaitFreeFrame();
					handle.Set(@this.Value);
				}
				await @this.LoadAsync();
				handle.Set(@this.Value);
			}
		}
	}
}
