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
		Awaitable LoadAsync();
	}

	public interface ITimedCache<out T> : ITimedCache
	{
		new T Value { get; }
	}

	class CacheConverter<T> : ITimedCache<T>
	{
		readonly ITimedCache source;
		public bool HasValue => source.HasValue;
		public T Value
		{
			get
			{
				try
				{
					return (T)source.Value;
				}
				catch (Exception e)
				{
					Debug.LogError($"convert failed: {source.Value.GetType()} -> {typeof(T)}");
					Debug.LogException(e);
					return default;
				}
			}
		}
		object ITimedCache.Value => source.Value;
		public CacheConverter(ITimedCache source)
		{
			this.source = source;
		}
		public Awaitable LoadAsync()
		{
			return source.LoadAsync();
		}
	}

	public abstract class AbsTimedCache<T> : ITimedCache<T>
	{
		const float keepSeconds = 10f;
		float lastAccess;
		T cachedValue;
		bool autoReleasing;
		uint timerId;
		public bool HasValue { get; private set; }
		public T Value
		{
			get
			{
				if (HasValue)
				{
					MarkAccess();
					return cachedValue;
				}
				cachedValue = LoadValue();
				MarkAccess();
				return cachedValue;
			}
			private set
			{
				MarkAccess();
				HasValue = true;
				cachedValue = value;
			}
		}
		T ITimedCache<T>.Value => Value;
		object ITimedCache.Value => Value;
		protected AbsTimedCache()
		{
			lastAccess = -keepSeconds;
		}
		public Awaitable LoadAsync()
		{
			var awaitable = new Awaitable(out var handle);
			LoadValueAsync(onLoaded);
			return awaitable;

			void onLoaded(T result)
			{
				Value = result;
				handle.Set();
			}
		}
		protected abstract T LoadValue();
		protected abstract void LoadValueAsync(Action<T> onLoaded);
		void MarkAccess()
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

	public sealed class TimedCache<T> : AbsTimedCache<T>
	{
		readonly Func<T> valueGetter;
		readonly Action<Action<T>> asyncValueGetter;
		public TimedCache(Func<T> valueGetter, Action<Action<T>> asyncValueGetter)
		{
			this.valueGetter = valueGetter;
			this.asyncValueGetter = asyncValueGetter;
		}
		protected override T LoadValue()
		{
			return valueGetter();
		}
		protected override void LoadValueAsync(Action<T> callback)
		{
			asyncValueGetter(callback);
		}
	}

	[Serializable]
	public class ResourceCache<T> : AbsTimedCache<T> where T : Object
	{
		[SerializeField] string resourcePath;
		public string ResourcePath => resourcePath;
		public ResourceCache(string resourcePath)
		{
			if (resourcePath.IsNullOrEmpty())
				throw new ArgumentNullException(nameof(resourcePath));
			this.resourcePath = resourcePath;
		}
		protected override T LoadValue()
		{
			return Resources.Load<T>(resourcePath);
		}
		protected override void LoadValueAsync(Action<T> callback)
		{
			var operation = Resources.LoadAsync<T>(resourcePath);
			operation.completed += cb;
			return;

			void cb(AsyncOperation _)
			{
				var result = operation.asset as T;
				if (!result)
					Debug.LogError($"load failed: {resourcePath}");
				callback.TryInvoke(result);
			}
		}
	}

	[Serializable]
	public sealed class ResourceCache : ResourceCache<Object>
	{
		public ResourceCache(string resourcePath) : base(resourcePath)
		{
		}
	}

	public sealed class ResourceGroupCache<T> : AbsTimedCache<T[]> where T : Object
	{
		public readonly string resourcePath;
		public ResourceGroupCache(string resourcePath)
		{
			this.resourcePath = resourcePath;
		}
		protected override T[] LoadValue()
		{
			return Resources.LoadAll<T>(resourcePath);
		}
		protected override void LoadValueAsync(Action<T[]> callback)
		{
			var operation = Resources.LoadAsync<T>(resourcePath);
			operation.completed += cb;
			return;

			void cb(AsyncOperation _)
			{
				var result = operation.asset as T;
				Assert.IsNotNull(result);
				callback.TryInvoke(LoadValue());
			}
		}
	}

	[Serializable]
	public class EditorAssetCache<T> : AbsTimedCache<T> where T : Object
	{
		[SerializeField] string assetPath;
		public EditorAssetCache(string resourcePath)
		{
			assetPath = resourcePath;
		}
		protected override T LoadValue()
		{
#if UNITY_EDITOR
			return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#endif
		}
		protected override void LoadValueAsync(Action<T> callback)
		{
			var operation = Resources.LoadAsync<T>(assetPath);
			operation.completed += cb;
			return;

			void cb(AsyncOperation _)
			{
				var result = operation.asset as T;
				Assert.IsNotNull(result);
				callback.TryInvoke(result);
			}
		}
	}

	[Serializable]
	public sealed class EditorAssetCache : EditorAssetCache<Object>
	{
		public EditorAssetCache(string assetPath) : base(assetPath)
		{
		}
	}
}
