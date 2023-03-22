using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace EthansGameKit.CachePools
{
	public class CachePool<T>
	{
		// ReSharper disable once CollectionNeverQueried.Local
		static readonly List<T> buffer = new();
		bool autoReleasing;
		readonly int keepCount;
		readonly List<T> list = new();
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
		public object Generate()
		{
			var index = list.Count - 1;
			var obj = list[index];
			list.RemoveAt(index);
			return obj;
		}
		public void Recycle(ref T item)
		{
			list.Add(item);
			if (!autoReleasing)
				AutoRelease();
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
				await Timers.Await(Random.Range(0f, 2f));
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
