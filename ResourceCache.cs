using System;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace EthansGameKit
{
	public class ResourceCache<T> where T : Object
	{
		const float keepSeconds = 10f;
		T asset;
		float lastAccess;
		bool autoReleasing;
		string resourcePath;
		public T Asset
		{
			get
			{
				RecordAccess();
				return asset;
			}
		}
		bool Expired => Time.unscaledTime - lastAccess > keepSeconds;
		public ResourceCache(string resourcePath)
		{
			this.resourcePath = resourcePath;
			lastAccess = -keepSeconds;
		}
		void RecordAccess()
		{
			lastAccess = Time.unscaledTime;
			if (!autoReleasing)
			{
				autoReleasing = true;
				AutoRelease();
			}
		}
		public T Load()
		{
			asset = Resources.Load<T>(resourcePath);
			return Asset;
		}
		public void LoadAsync(Action<T> callback)
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
		void AutoRelease()
		{
			if (Expired)
			{
				asset = null;
				autoReleasing = false;
			}
			else
			{
				Timers.InvokeAfterUnscaled(keepSeconds, AutoRelease);
			}
		}
	}
}
