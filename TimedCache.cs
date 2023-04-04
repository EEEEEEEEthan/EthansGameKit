using System;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit
{
	public class TimedCache<T>
	{
		const float keepSeconds = 10f;
		static readonly CachePool<TimedCache<T>> pool = new(0);
		public static TimedCache<T> Generate(Func<T> valueGetter)
		{
			if (!pool.TryGenerate(out var cache)) cache = new();
			cache.valueGetter = valueGetter;
			cache.lastAccess = -keepSeconds;
			cache.autoReleasing = false;
			return cache;
		}
		public static void Recycle(ref TimedCache<T> cache)
		{
			Timers.CancelInvoke(ref cache.timerId);
			cache.valueGetter = null;
			cache.cachedValue = default;
			pool.Recycle(ref cache);
			cache = null;
		}
		float lastAccess;
		Func<T> valueGetter;
		T cachedValue;
		bool autoReleasing;
		uint timerId;
		public T Value
		{
			get
			{
				if (Expired)
				{
					cachedValue = valueGetter();
					if (!autoReleasing)
					{
						autoReleasing = true;
						AutoRelease();
					}
				}
				lastAccess = Time.unscaledTime;
				return cachedValue;
			}
		}
		bool Expired => Time.unscaledTime - lastAccess > keepSeconds;
		TimedCache()
		{
		}
		void AutoRelease()
		{
			Timers.InvokeAfterUnscaled(ref timerId, keepSeconds, AutoRelease2);
		}
		void AutoRelease2()
		{
			if (Expired)
			{
				cachedValue = default;
				autoReleasing = false;
			}
			else
			{
				AutoRelease();
			}
		}
	}
}
