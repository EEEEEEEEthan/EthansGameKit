using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit
{
	public class AbsTimedCache<T>
	{
		const float keepSeconds = 10f;
		float lastAccess;
		T cachedValue;
		bool autoReleasing;
		uint timerId;
		public bool HasValue { get; private set; }
		protected T Value
		{
			get => cachedValue;
			set
			{
				HasValue = true;
				cachedValue = value;
			}
		}
		bool Expired => Time.unscaledTime - lastAccess > keepSeconds;
		protected AbsTimedCache()
		{
			lastAccess = -keepSeconds;
		}
		protected void MarkAccess()
		{
			lastAccess = Time.unscaledTime;
			if (!autoReleasing)
			{
				autoReleasing = true;
				AutoRelease();
			}
		}
		void AutoRelease()
		{
			if (Expired)
			{
				HasValue = false;
				cachedValue = default;
				autoReleasing = false;
			}
			else
			{
				Timers.InvokeAfterUnscaled(keepSeconds, AutoRelease);
			}
		}
	}

	public sealed class TimedCache<T> : AbsTimedCache<T>
	{
		readonly Func<T> valueGetter;
		readonly Action<Action<T>> asyncValueGetter;
		public new T Value
		{
			get
			{
				MarkAccess();
				if (HasValue)
					return base.Value;
				return base.Value = valueGetter();
			}
		}
		public TimedCache(Func<T> valueGetter, Action<Action<T>> asyncValueGetter)
		{
			this.valueGetter = valueGetter;
			this.asyncValueGetter = asyncValueGetter;
		}
		public void LoadAsync(Action callback)
		{
			asyncValueGetter(onLoaded);

			void onLoaded(T value)
			{
				MarkAccess();
				base.Value = value;
				callback.TryInvoke();
			}
		}
	}

	public sealed class ResourceCache<T> : AbsTimedCache<T> where T : UnityEngine.Object
	{
		const float keepSeconds = 10f;
		readonly string resourcePath;
		public new T Value
		{
			get
			{
				MarkAccess();
				if (HasValue)
					return base.Value;
				return base.Value = Resources.Load<T>(resourcePath);
			}
		}
		public ResourceCache(string resourcePath)
		{
			this.resourcePath = resourcePath;
		}
		void LoadAsync(Action<T> callback)
		{
			var operation = Resources.LoadAsync<T>(resourcePath);
			operation.completed += cb;

			void cb(AsyncOperation _)
			{
				var result = operation.asset as T;
				Assert.IsNotNull(result);
				callback.TryInvoke(result);
			}
		}
		public IAwaitable<T> LoadAsync()
		{
			var awaitable = IAwaitable<T>.Create(out var handle);
			LoadAsync(handle.Set);
			return awaitable;
		}
	}
}
