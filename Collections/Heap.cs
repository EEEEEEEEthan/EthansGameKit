using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

// ReSharper disable TooWideLocalVariableScope
namespace EthansGameKit.Collections
{
	public interface IReadOnlyHeap<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>> where TValue : IComparable<TValue>
	{
		TKey Peek(out TValue value, int index = 0);
		TKey Peek(int index = 0);
		bool TryPeek(out TKey key, int index = 0);
		bool TryPeek(out TKey key, out TValue value, int index = 0);
	}

	public interface IHeap<T, TValue> : IReadOnlyHeap<T, TValue> where TValue : IComparable<TValue>
	{
		void Add(T element, TValue sortingValue);
		T Pop(int index = 0);
		T Pop(out TValue value, int index = 0);
		void TrimExcess();
	}

	public abstract class Heap
	{
		static int[] finder = Array.Empty<int>();
		/// <summary>
		///     向堆增加元素
		/// </summary>
		public static void HeapAdd<TKey, TValue>(TKey key, TValue value, TKey[] keys, TValue[] values, ref int length) where TValue : IComparable<TValue>
		{
			keys[length] = key;
			values[length] = value;
			AdjustUp(keys, values, length++);
		}
		public static void HeapAdd<TKey, TValue>(TKey key, TValue value, ref TKey[] keys, ref TValue[] values, ref int length) where TValue : IComparable<TValue>
		{
			var newLength = length + 1;
			if (keys.Length <= length)
				Array.Resize(ref keys, newLength);
			if (values.Length <= length)
				Array.Resize(ref values, newLength);
			HeapAdd(key, value, keys, values, ref length);
		}
		public static int HeapFind<TKey, TValue>(TKey key, TValue value, TKey[] keys, TValue[] values, int length)
			where TValue : IComparable<TValue>
		{
			if (length <= 0) return -1;
			if (finder.Length <= length)
				finder = new int[length];
			finder[0] = 0;
			var start = 0;
			var end = 1;
			while (start < end)
			{
				var currentIndex = finder[start];
				var currentValue = values[currentIndex];
				var cmp = value.CompareTo(currentValue);
				switch (cmp)
				{
					case 0 when keys[currentIndex].Equals(key):
						return currentIndex;
					case <= 0:
						++start;
						continue;
				}
				var leftIndex = (currentIndex << 1) | 1;
				if (leftIndex < length)
					finder[end++] = leftIndex;
				var rightIndex = leftIndex + 1;
				if (rightIndex < length)
					finder[end++] = rightIndex;
				++start;
			}
			return -1;
		}
		public static int HeapFind<TKey, TValue>(TKey key, TValue value, TKey[] keys, TValue[] values, int length, IEqualityComparer<TKey> comparer)
			where TValue : IComparable<TValue>
		{
			if (length <= 0) return -1;
			if (finder.Length <= length)
				finder = new int[length];
			finder[0] = 0;
			var start = 0;
			var end = 1;
			while (start < end)
			{
				var currentIndex = finder[start];
				var currentValue = values[currentIndex];
				var cmp = value.CompareTo(currentValue);
				switch (cmp)
				{
					case 0 when comparer.Equals(keys[currentIndex], key):
						return currentIndex;
					case <= 0:
						++start;
						continue;
				}
				var leftIndex = (currentIndex << 1) | 1;
				if (leftIndex < length)
					finder[end++] = leftIndex;
				var rightIndex = leftIndex + 1;
				if (rightIndex < length)
					finder[end++] = rightIndex;
				++start;
			}
			return -1;
		}
		/// <summary>
		///     移除堆的一个元素
		/// </summary>
		/// <param name="keys">堆化列表</param>
		/// <param name="values"></param>
		/// <param name="length">长度</param>
		/// <param name="index">被移除元素下标</param>
		public static TKey HeapPop<TKey, TValue>(TKey[] keys, TValue[] values, ref int length, int index = 0) where TValue : IComparable<TValue>
		{
			var result = keys[index];
			keys[index] = keys[--length];
			values[index] = values[length];
			AdjustDown(keys, values, length, index);
			AdjustUp(keys, values, index);
			return result;
		}
		public static void HeapUpdate<TKey, TValue>(TKey key, TValue value, int index, TKey[] keys, TValue[] values, int length) where TValue : IComparable<TValue>
		{
			keys[index] = key;
			values[index] = value;
			AdjustDown(keys, values, length, index);
			AdjustUp(keys, values, index);
		}
		static void AdjustDown<TKey, TValue>(TKey[] keys, TValue[] values, int length, int index) where TValue : IComparable<TValue>
		{
			TKey tempKey;
			TValue tempValue;
			int leftIndex, rightIndex;
			while (true)
			{
				leftIndex = (index << 1) | 1;
				if (leftIndex >= length)
					return;
				rightIndex = leftIndex + 1;
				var cmp_left_to_me = values[leftIndex].CompareTo(values[index]);
				if (rightIndex >= length)
				{
					if (cmp_left_to_me >= 0) return;
					tempKey = keys[leftIndex];
					keys[leftIndex] = keys[index];
					keys[index] = tempKey;
					tempValue = values[leftIndex];
					values[leftIndex] = values[index];
					values[index] = tempValue;
					return;
				}
				if (cmp_left_to_me < 0)
				{
					if (values[rightIndex].CompareTo(values[leftIndex]) < 0)
					{
						tempKey = keys[rightIndex];
						keys[rightIndex] = keys[index];
						keys[index] = tempKey;
						tempValue = values[rightIndex];
						values[rightIndex] = values[index];
						values[index] = tempValue;
						index = rightIndex;
					}
					else
					{
						tempKey = keys[leftIndex];
						keys[leftIndex] = keys[index];
						keys[index] = tempKey;
						tempValue = values[leftIndex];
						values[leftIndex] = values[index];
						values[index] = tempValue;
						index = leftIndex;
					}
				}
				else
				{
					if (values[rightIndex].CompareTo(values[index]) < 0)
					{
						tempKey = keys[rightIndex];
						keys[rightIndex] = keys[index];
						keys[index] = tempKey;
						tempValue = values[rightIndex];
						values[rightIndex] = values[index];
						values[index] = tempValue;
						index = rightIndex;
					}
					else
					{
						return;
					}
				}
			}
		}
		static void AdjustUp<TKey, TValue>(TKey[] keys, TValue[] values, int index) where TValue : IComparable<TValue>
		{
			int parentIndex;
			TKey tempKey;
			TValue tempValue;
			while (index > 0)
			{
				parentIndex = (index - 1) >> 1;
				if (values[index].CompareTo(values[parentIndex]) < 0)
				{
					tempKey = keys[parentIndex];
					tempValue = values[parentIndex];
					keys[parentIndex] = keys[index];
					values[parentIndex] = values[index];
					keys[index] = tempKey;
					values[index] = tempValue;
					index = parentIndex;
				}
				else
				{
					break;
				}
			}
		}
	}

	[Serializable]
	public class Heap<TKey, TValue> : Heap, IHeap<TKey, TValue>, IEqualityComparer<TKey> where TValue : IComparable<TValue>
	{
		public static Heap<TKey, TValue> Generate()
		{
			if (!GlobalCachePool<Heap<TKey, TValue>>.TryGenerate(out var heap)) heap = new();
			return heap;
		}
		public static void ClearAndRecycle(ref Heap<TKey, TValue> heap)
		{
			heap.Clear();
			GlobalCachePool<Heap<TKey, TValue>>.Recycle(ref heap);
		}
		[SerializeField] TKey[] keys;
		[SerializeField] int length;
		[SerializeField] TValue[] values;
		public int Count => length;
		public Heap(IEnumerable<KeyValuePair<TKey, TValue>> elements) : this()
		{
			foreach (var pair in elements)
				Add(pair.Key, pair.Value);
		}
		public Heap(IEnumerable<(TKey key, TValue value)> elements) : this()
		{
			foreach (var (key, value) in elements)
				Add(key, value);
		}
		public Heap() : this(32)
		{
		}
		public Heap(int defaultCapability)
		{
			if (defaultCapability <= 0)
				throw new ArgumentException($"{nameof(defaultCapability)} should be larger than 0");
			keys = new TKey[defaultCapability];
			values = new TValue[defaultCapability];
		}
		/// <summary>
		///     堆增加一个元素
		/// </summary>
		/// <param name="element"></param>
		/// <param name="sortingValue"></param>
		public void Add(TKey element, TValue sortingValue)
		{
			if (length >= keys.Length)
			{
				var newLength = length << 1;
				Array.Resize(ref keys, newLength);
				Array.Resize(ref values, newLength);
			}
			HeapAdd(element, sortingValue, keys, values, ref length);
		}
		public TKey Pop(out TValue value, int index = 0)
		{
			value = values[index];
			return HeapPop(keys, values, ref length, index);
		}
		public TKey Pop(int index = 0)
		{
			return HeapPop(keys, values, ref length, index);
		}
		public void TrimExcess()
		{
			Array.Resize(ref keys, length);
			Array.Resize(ref values, length);
		}
		public TKey Peek(out TValue value, int index = 0)
		{
			value = values[index];
			return keys[index];
		}
		public TKey Peek(int index = 0)
		{
			return keys[index];
		}
		public bool TryPeek(out TKey key, int index = 0)
		{
			if (length > index)
			{
				key = Peek(index);
				return true;
			}
			key = default;
			return false;
		}
		public bool TryPeek(out TKey key, out TValue value, int index = 0)
		{
			if (length > index)
			{
				key = Peek(out value, index);
				return true;
			}
			key = default;
			value = default;
			return false;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (var i = 0; i < length; i++)
				yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
		}
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			for (var i = 0; i < length; i++)
				yield return new(keys[i], values[i]);
		}
		bool IEqualityComparer<TKey>.Equals(TKey x, TKey y)
		{
			return Equals(x, y);
		}
		int IEqualityComparer<TKey>.GetHashCode(TKey obj)
		{
			return GetHashCode(obj);
		}
		public void ClearAndRecycle()
		{
			var o = this;
			ClearAndRecycle(ref o);
		}
		public bool Update(TKey element, TValue sortingValue)
		{
			for (var i = 0; i < length; i++)
				if (keys[i].Equals(element))
				{
					HeapUpdate(element, sortingValue, i, keys, values, length);
					return true;
				}
			return false;
		}
		public void AddOrUpdate(TKey element, TValue sortingValue)
		{
			var index = Find(element);
			if (index >= 0)
				HeapUpdate(element, sortingValue, index, keys, values, length);
			else
				Add(element, sortingValue);
		}
		public void AddOrUpdate(TKey element, TValue oldValue, TValue newValue)
		{
			var index = Find(element, oldValue);
			if (index >= 0)
				HeapUpdate(element, newValue, index, keys, values, length);
			else
				Add(element, newValue);
		}
		public void Clear()
		{
			Array.Clear(keys, 0, keys.Length);
			Array.Clear(values, 0, values.Length);
			length = 0;
		}
		public int Find(TKey element, TValue sortingValue)
		{
			return HeapFind(element, sortingValue, keys, values, length, this);
		}
		public int Find(TKey element)
		{
			for (var i = 0; i < length; i++)
				if (keys[i].Equals(element))
					return i;
			return -1;
		}
		protected virtual bool Equals(TKey x, TKey y)
		{
			return x.Equals(y);
		}
		protected virtual int GetHashCode(TKey obj)
		{
			return obj.GetHashCode();
		}
	}

	[Serializable]
	public class HeapInt32Single : Heap<int, float>
	{
		public new static HeapInt32Single Generate()
		{
			if (!GlobalCachePool<HeapInt32Single>.TryGenerate(out var heap)) heap = new();
			return heap;
		}
		public static void ClearAndRecycle(ref HeapInt32Single heap)
		{
			heap.Clear();
			GlobalCachePool<HeapInt32Single>.Recycle(ref heap);
		}
		protected override bool Equals(int x, int y)
		{
			return x == y;
		}
	}
}