using System;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace EthansGameKit
{
	public interface ITimedCache
	{
		bool HasValue { get; }
		object Value { get; }
		IAwaitable LoadAsync();
		void LoadAsync(Action callback);
	}

	public interface ITimedCache<out T> : ITimedCache
	{
		new T Value { get; }
	}

	class CacheConverter<T> : ITimedCache<T>
	{
		readonly ITimedCache source;
		public bool HasValue => source.HasValue;
		public T Value => (T)source.Value;
		object ITimedCache.Value => source.Value;
		public CacheConverter(ITimedCache source)
		{
			this.source = source;
		}
		public IAwaitable LoadAsync()
		{
			return source.LoadAsync();
		}
		public void LoadAsync(Action callback)
		{
			source.LoadAsync(callback);
		}
	}

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
			get
			{
				MarkAccess();
				return cachedValue;
			}
			set
			{
				MarkAccess();
				HasValue = true;
				cachedValue = value;
			}
		}
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
			var remainingTime = keepSeconds - (Time.unscaledTime - lastAccess);
			if (remainingTime <= 0)
			{
				HasValue = false;
				cachedValue = default;
				autoReleasing = false;
			}
			else
			{
				Timers.InvokeAfterUnscaled(remainingTime, AutoRelease);
			}
		}
	}

	public sealed class TimedCache<T> : AbsTimedCache<T>, ITimedCache<T>
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
		object ITimedCache.Value => Value;
		public TimedCache(Func<T> valueGetter, Action<Action<T>> asyncValueGetter)
		{
			this.valueGetter = valueGetter;
			this.asyncValueGetter = asyncValueGetter;
		}
		public IAwaitable LoadAsync()
		{
			var awaitable = IAwaitable.Create(out var handle);
			LoadAsync(handle.Set);
			return awaitable;
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

	public sealed class ResourceCache<T> : AbsTimedCache<T>, ITimedCache<T> where T : Object
	{
		readonly string resourcePath;
		public new T Value
		{
			get
			{
				if (HasValue)
					return base.Value;
				return base.Value = Resources.Load<T>(resourcePath);
			}
		}
		object ITimedCache.Value => Value;
		public ResourceCache(string resourcePath)
		{
			this.resourcePath = resourcePath;
		}
		public IAwaitable LoadAsync()
		{
			var awaitable = IAwaitable.Create(out var handle);
			LoadAsync(handle.Set);
			return awaitable;
		}
		public void LoadAsync(Action callback)
		{
			var operation = Resources.LoadAsync<T>(resourcePath);
			operation.completed += cb;

			void cb(AsyncOperation _)
			{
				var result = operation.asset as T;
				Assert.IsNotNull(result);
				callback.TryInvoke();
				base.Value = result;
			}
		}
	}

	public sealed class ResourceGroupCache<T> : AbsTimedCache<T[]>, ITimedCache<T[]> where T : Object
	{
		readonly string resourcePath;
		public new T[] Value
		{
			get
			{
				if (HasValue)
					return base.Value;
				return base.Value = Resources.LoadAll<T>(resourcePath);
			}
		}
		object ITimedCache.Value => Value;
		public ResourceGroupCache(string resourcePath)
		{
			this.resourcePath = resourcePath;
		}
		public IAwaitable LoadAsync()
		{
			var awaitable = IAwaitable.Create(out var handle);
			LoadAsync(handle.Set);
			return awaitable;
		}
		public void LoadAsync(Action callback)
		{
			var operation = Resources.LoadAsync<T>(resourcePath);
			operation.completed += cb;

			void cb(AsyncOperation _)
			{
				var result = operation.asset as T;
				Assert.IsNotNull(result);
				callback.TryInvoke();
				base.Value = Value;
			}
		}
	}
}
