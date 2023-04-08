using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static T RandomPick<T>(this IEnumerable<T> @this)
		{
			return @this.TryRandomPick(out var value) ? value : default;
		}
		public static T RandomPick<T>(this IEnumerable<T> @this, Func<T, float> weightGetter)
		{
			return @this.TryRandomPick(weightGetter, out var value) ? value : default;
		}
		public static IEnumerable<T> RandomPick<T>(this IEnumerable<T> @this, int pickCount)
		{
			// 不调用带权重的RandomPick，因为可以进一步提高效率
			if (@this is null)
				yield break;
			var heap = new Heap<T, float>();
			foreach (var item in @this)
			{
				var score = Random.value;
				if (heap.Count >= pickCount)
				{
					if (heap.TryPeek(out _, out var value) && value < score)
					{
						heap.Pop();
						heap.Add(item, score);
					}
				}
				else
				{
					heap.Add(item, score);
				}
			}
			foreach (var item in heap)
				yield return item.Key;
		}
		public static IEnumerable<T> RandomPick<T>(this IEnumerable<T> @this, Func<T, float> weightGetter, int pickCount)
		{
			if (@this is null)
				yield break;
			var heap = new Heap<T, float>();
			foreach (var item in @this)
			{
				float weight;
				try
				{
					weight = weightGetter(item);
					if (weight <= 0) continue;
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					continue;
				}
				// 随机 ^ (1 / weight), 最大的n个即为所求
				var score = Mathf.Pow(Random.value, 1 / weight);
				if (heap.Count >= pickCount)
				{
					if (heap.TryPeek(out _, out var value) && value < score)
					{
						heap.Pop();
						heap.Add(item, score);
					}
				}
				else
				{
					heap.Add(item, score);
				}
			}
			foreach (var item in heap)
				yield return item.Key;
		}
		public static bool TryRandomPick<T>(this IEnumerable<T> @this, out T value)
		{
			if (@this is null)
			{
				value = default;
				return false;
			}
			var cnt = 0;
			value = default;
			foreach (var item in @this)
			{
				cnt += 1;
				if (Random.value < 1f / cnt)
					value = item;
			}
			return cnt > 0;
		}
		public static bool TryRandomPick<T>(this IEnumerable<T> @this, Func<T, float> weightGetter, out T result)
		{
			var value = float.MinValue;
			result = default;
			var hasItem = false;
			foreach (var item in @this)
			{
				float weight;
				try
				{
					weight = weightGetter(item);
					if (weight <= 0) continue;
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					continue;
				}
				hasItem = true;
				var score = Mathf.Pow(Random.value, 1 / weight);
				if (score > value)
				{
					result = item;
					value = score;
				}
			}
			return hasItem;
		}
		public static IEnumerator GetEnumerator(this IEnumerable @this, bool safe)
		{
			if (safe) return @this.GetEnumerator().ToSafeEnumerator();
			return @this.GetEnumerator();
		}
	}
}
