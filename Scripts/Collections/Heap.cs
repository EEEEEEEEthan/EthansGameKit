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
		bool TryPop(out T key, out TValue value, int index = 0);
		void TrimExcess();
	}

	[Serializable]
	public sealed class Heap<TKey, TValue> : IHeap<TKey, TValue>, IDisposable where TValue : IComparable<TValue>
	{
		// ReSharper disable once StaticMemberInGenericType
		static int[] finder = Array.Empty<int>();
		public static Heap<TKey, TValue> Generate()
		{
			if (!GlobalCachePool<Heap<TKey, TValue>>.TryGenerate(out var heap))
				heap = new();
			heap.inPool = false;
			return heap;
		}
		public static void ClearAndRecycle(ref Heap<TKey, TValue> heap)
		{
			if (heap.inPool)
			{
				throw new InvalidOperationException("Heap is already in pool");
			}
			heap.inPool = true;
			heap.Clear();
			GlobalCachePool<Heap<TKey, TValue>>.Recycle(ref heap);
		}
		[SerializeField] TKey[] keys = new TKey[1];
		[SerializeField] TValue[] values = new TValue[1];
		[NonSerialized] bool inPool;
		EqualityComparer<TKey> defaultComparer = EqualityComparer<TKey>.Default;
		public int Count { get; private set; }
		Heap()
		{
		}
		public void Dispose()
		{
			var o = this;
			ClearAndRecycle(ref o);
		}
		/// <summary>
		///     堆增加一个元素
		/// </summary>
		/// <param name="element"></param>
		/// <param name="sortingValue"></param>
		public void Add(TKey element, TValue sortingValue)
		{
			if (Count >= keys.Length)
			{
				var newLength = Count << 1;
				Array.Resize(ref keys, newLength);
				Array.Resize(ref values, newLength);
			}
			keys[Count] = element;
			values[Count] = sortingValue;
			AdjustUp(Count++);
		}
		public TKey Pop(out TValue value, int index = 0)
		{
			value = values[index];
			return Pop(index);
		}
		public TKey Pop(int index = 0)
		{
			var result = keys[index];
			keys[index] = keys[--Count];
			values[index] = values[Count];
			AdjustDown(Count, index);
			AdjustUp(index);
			return result;
		}
		public bool TryPop(out TKey key, out TValue value, int index = 0)
		{
			if (index >= Count)
			{
				key = default;
				value = default;
				return false;
			}
			key = Pop(out value, index);
			return true;
		}
		public void TrimExcess()
		{
			Array.Resize(ref keys, Count);
			Array.Resize(ref values, Count);
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
			if (Count > index)
			{
				key = Peek(index);
				return true;
			}
			key = default;
			return false;
		}
		public bool TryPeek(out TKey key, out TValue value, int index = 0)
		{
			if (Count > index)
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
			for (var i = 0; i < Count; i++)
				yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
		}
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			for (var i = 0; i < Count; i++)
				yield return new(keys[i], values[i]);
		}
		public bool Update(TKey element, TValue sortingValue)
		{
			for (var i = 0; i < Count; i++)
				if (keys[i].Equals(element))
				{
					Update(i, element, sortingValue);
					return true;
				}
			return false;
		}
		public void AddOrUpdate(TKey element, TValue sortingValue, IEqualityComparer<TKey> equalityComparer)
		{
			var index = Find(element, equalityComparer);
			if (index >= 0)
				Update(index, element, sortingValue);
			else
				Add(element, sortingValue);
		}
		public void AddOrUpdate(TKey element, TValue oldValue, TValue newValue, IEqualityComparer<TKey> equalityComparer)
		{
			var index = Find(element, oldValue, equalityComparer);
			if (index >= 0)
				Update(index, element, newValue);
			else
				Add(element, newValue);
		}
		public void AddOrUpdate(TKey element, TValue sortingValue)
		{
			AddOrUpdate(element, sortingValue, defaultComparer);
		}
		public void AddOrUpdate(TKey element, TValue oldValue, TValue newValue)
		{
			AddOrUpdate(element, oldValue, newValue, defaultComparer);
		}
		public void Clear()
		{
			Array.Clear(keys, 0, keys.Length);
			Array.Clear(values, 0, values.Length);
			Count = 0;
		}
		public int Find(TKey element, TValue sortingValue, IEqualityComparer<TKey> equalityComparer)
		{
			var count = Count;
			if (count <= 0) return -1;
			if (finder.Length <= count)
				finder = new int[count];
			finder[0] = 0;
			var start = 0;
			var end = 1;
			while (start < end)
			{
				var currentIndex = finder[start];
				var currentValue = values[currentIndex];
				var cmp = sortingValue.CompareTo(currentValue);
				switch (cmp)
				{
					case 0 when equalityComparer.Equals(keys[currentIndex], element):
						return currentIndex;
					case <= 0:
						++start;
						continue;
				}
				var leftIndex = (currentIndex << 1) | 1;
				if (leftIndex < count)
					finder[end++] = leftIndex;
				var rightIndex = leftIndex + 1;
				if (rightIndex < count)
					finder[end++] = rightIndex;
				++start;
			}
			return -1;
		}
		public int Find(TKey element, IEqualityComparer<TKey> equalityComparer)
		{
			var count = Count;
			for (var i = 0; i < count; i++)
				if (equalityComparer.Equals(keys[i], element))
					return i;
			return -1;
		}
		public int Find(TKey element, TValue sortingValue)
		{
			return Find(element, sortingValue, defaultComparer);
		}
		public int Find(TKey element)
		{
			return Find(element, defaultComparer);
		}
		int GetHashCode(TKey obj)
		{
			return obj.GetHashCode();
		}
		void Update(int index, TKey key, TValue value)
		{
			keys[index] = key;
			values[index] = value;
			AdjustDown(Count, index);
			AdjustUp(index);
		}
		void AdjustDown(int length, int index)
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
				var cmpLeftToMe = values[leftIndex].CompareTo(values[index]);
				if (rightIndex >= length)
				{
					if (cmpLeftToMe >= 0) return;
					tempKey = keys[leftIndex];
					keys[leftIndex] = keys[index];
					keys[index] = tempKey;
					tempValue = values[leftIndex];
					values[leftIndex] = values[index];
					values[index] = tempValue;
					return;
				}
				if (cmpLeftToMe < 0)
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
		void AdjustUp(int index)
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
}
