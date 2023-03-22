using System;
using EthansGameKit.CachePools;
using UnityEngine;
using Utilities;

namespace EthansGameKit
{
	public class TimedCache<T>
	{
		static readonly CachePool<TimedCache<T>> pool = new(0);
		public static TimedCache<T> Generate(Func<T> valueGetter, float keepSeconds)
		{
			if (!pool.TryGenerate(out var cache)) cache = new();
			cache.valueGetter = valueGetter;
			cache.keepSeconds = keepSeconds;
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
		float keepSeconds;
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
				AutoRelease();
		}
	}
}
