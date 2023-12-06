using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.CachePools
{
	public class CachePool<T>
	{
		// ReSharper disable once CollectionNeverQueried.Local
		static readonly List<T> buffer = new();
		readonly int keepCount;
		readonly List<T> list = new();
		bool autoReleasing;
		public bool IsEmpty => list.Count <= 0;
		/// <summary>
		///     构造方法
		/// </summary>
		/// <param name="keepCount">持有数量,这个数量以下不会自动释放</param>
		public CachePool(int keepCount)
		{
			if (keepCount < 0)
			{
				this.keepCount = 0;
				Debug.LogError("持有数量不应该小于0");
			}
			this.keepCount = keepCount;
		}
		public void Recycle(T item)
		{
			list.Add(item);
			if (!autoReleasing && list.Count > keepCount)
				AutoRelease();
		}
		public void Recycle(ref T item)
		{
			Recycle(item);
			item = default;
		}
		public bool TryGenerate(out T cache)
		{
			return list.TryPop(0, out cache);
		}
		async void AutoRelease()
		{
			autoReleasing = true;
			while (list.Count > keepCount)
			{
				using var awaitable = Timers.Await(Random.Range(0f, 2f));
				await awaitable;
				var needRelease = Mathf.CeilToInt((list.Count - keepCount) * 0.01f);
				for (var i = 0; i < needRelease; i++)
				{
					list.TryPop(0, out var item);
					buffer.Add(item);
				}
				buffer.Clear();
			}
			autoReleasing = false;
		}
	}
}
