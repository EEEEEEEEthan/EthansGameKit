using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TooWideLocalVariableScope
namespace EthansGameKit.Collections
{
	public interface IReadOnlyHeap<T> : IReadOnlyCollection<KeyValuePair<T, float>>
	{
		T Peek(out float value, int index = 0);
		T Peek(int index = 0);
		bool TryPeek(out T key, int index = 0);
		bool TryPeek(out T key, out float value, int index = 0);
	}

	public interface IHeap<T> : IReadOnlyHeap<T>
	{
		void Add(T element, float sortingValue);
		T Pop(int index = 0);
		T Pop(out float value, int index = 0);
		void TrimExcess();
	}

	[Serializable]
	public class Heap<T> : IHeap<T>
	{
		static int[] finder = Array.Empty<int>();
		/// <summary>
		///     向堆增加元素
		/// </summary>
		public static void HeapAdd(T key, float value, T[] keys, float[] values, ref int length)
		{
			keys[length] = key;
			values[length] = value;
			AdjustUp(keys, values, length++);
		}
		public static void HeapAdd(T key, float value, ref T[] keys, ref float[] values, ref int length)
		{
			if (keys.Length <= length)
			{
				var copy = keys;
				keys = new T[length * 2 + 1];
				Array.Copy(copy, keys, copy.Length);
			}
			if (values.Length <= length)
			{
				var copy = values;
				values = new float[length * 2 + 1];
				Array.Copy(copy, values, copy.Length);
			}
			HeapAdd(key, value, keys, values, ref length);
		}
		public static int HeapFind(T key, float value, T[] keys, float[] values, int length)
		{
			if (length <= 0) return -1;
			finder[0] = 0;
			var start = 0;
			var count = 1;
			while (count > 0)
			{
				var currentIndex = finder[start];
				var currentValue = values[currentIndex];
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (value == currentValue)
				{
					if (keys[currentIndex].Equals(key))
						return currentIndex;
					// ReSharper disable once RedundantJumpStatement
					goto NEXT;
				}
				else if (value < currentValue)
				{
					// ReSharper disable once RedundantJumpStatement
					goto NEXT;
				}
				else // if value > currentValue
				{
					var leftIndex = (currentIndex << 1) + 1;
					if (leftIndex >= length) goto NEXT;
					if (count + 1 >= finder.Length)
						Array.Resize(ref finder, count);
					finder[count++] = leftIndex;
					var rightIndex = leftIndex + 1;
					if (rightIndex >= length) goto NEXT;
					if (count + 1 >= finder.Length)
						Array.Resize(ref finder, count);
					finder[count++] = rightIndex;
				}
			NEXT:
				--count;
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
		public static T HeapPop(T[] keys, float[] values, ref int length, int index = 0)
		{
			var result = keys[index];
			keys[index] = keys[--length];
			values[index] = values[length];
			AdjustDown(keys, values, length, index);
			AdjustUp(keys, values, index);
			return result;
		}
		public static void HeapUpdate(T key, float value, int index, T[] keys, float[] values, int length)
		{
			keys[index] = key;
			values[index] = value;
			AdjustDown(keys, values, length, index);
			AdjustUp(keys, values, index);
		}
		static void AdjustDown(T[] keys, float[] values, int length, int index)
		{
			T tempKey;
			float tempValue;
			int leftIndex, rightIndex;
			while (true)
			{
				leftIndex = (index << 1) + 1;
				if (leftIndex >= length)
					return;
				rightIndex = leftIndex + 1;
				if (rightIndex >= length)
				{
					if (values[leftIndex] >= values[index]) return;
					tempKey = keys[leftIndex];
					keys[leftIndex] = keys[index];
					keys[index] = tempKey;
					tempValue = values[leftIndex];
					values[leftIndex] = values[index];
					values[index] = tempValue;
					return;
				}
				if (values[leftIndex] < values[index])
				{
					if (values[rightIndex] < values[leftIndex])
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
					if (values[rightIndex] < values[index])
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
						return;
				}
			}
		}
		static void AdjustUp(T[] keys, float[] values, int index)
		{
			int parentIndex;
			T tempKey;
			float tempValue;
			while (index > 0)
			{
				parentIndex = (index - 1) >> 1;
				if (values[index] < values[parentIndex])
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
					break;
			}
		}
		[SerializeField] T[] keys;
		[SerializeField] int length;
		[SerializeField] float[] values;
		public int Count => length;
		public Heap(IEnumerable<KeyValuePair<T, float>> elements) : this()
		{
			foreach (var pair in elements)
				Add(pair.Key, pair.Value);
		}
		public Heap(IEnumerable<(T key, float value)> elements) : this()
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
			keys = new T[defaultCapability];
			values = new float[defaultCapability];
		}
		public void Clear()
		{
			Array.Clear(keys, 0, keys.Length);
			Array.Clear(values, 0, values.Length);
			length = 0;
		}
		public void AddOrUpdate(T element, float sortingValue)
		{
			for (var i = 0; i < length; i++)
			{
				if (keys[i].Equals(element))
				{
					HeapUpdate(element, sortingValue, i, keys, values, length);
					return;
				}
			}
			Add(element, sortingValue);
		}
		public int Find(T element, float sortingValue)
		{
			return HeapFind(element, sortingValue, keys, values, length);
		}
		public int Find(T element)
		{
			for (var i = 0; i < length; i++)
				if (keys[i].Equals(element))
					return i;
			return -1;
		}
		public bool Update(T element, float sortingValue)
		{
			for (var i = 0; i < length; i++)
				if (keys[i].Equals(element))
				{
					HeapUpdate(element, sortingValue, i, keys, values, length);
					return true;
				}
			return false;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (var i = 0; i < length; i++)
				yield return new KeyValuePair<T, float>(keys[i], values[i]);
		}
		IEnumerator<KeyValuePair<T, float>> IEnumerable<KeyValuePair<T, float>>.GetEnumerator()
		{
			for (var i = 0; i < length; i++)
				yield return new(keys[i], values[i]);
		}
		/// <summary>
		///     堆增加一个元素
		/// </summary>
		/// <param name="element"></param>
		/// <param name="sortingValue"></param>
		public void Add(T element, float sortingValue)
		{
			if (length >= keys.Length)
			{
				var copiedKeys = new T[keys.Length * 2];
				Array.Copy(keys, copiedKeys, keys.Length);
				keys = copiedKeys;
				var copiedValues = new float[values.Length * 2];
				Array.Copy(values, copiedValues, values.Length);
				values = copiedValues;
			}
			HeapAdd(element, sortingValue, keys, values, ref length);
		}
		public T Pop(out float value, int index = 0)
		{
			value = values[index];
			return HeapPop(keys, values, ref length, index);
		}
		public T Pop(int index = 0)
		{
			return HeapPop(keys, values, ref length, index);
		}
		public void TrimExcess()
		{
			var copiedKeys = new T[length];
			Array.Copy(keys, copiedKeys, length);
			keys = copiedKeys;
			var copiedValues = new float[length];
			Array.Copy(values, copiedValues, length);
			values = copiedValues;
		}
		public T Peek(out float value, int index = 0)
		{
			value = values[index];
			return keys[index];
		}
		public T Peek(int index = 0)
		{
			return keys[index];
		}
		public bool TryPeek(out T key, int index = 0)
		{
			if (length > index)
			{
				key = Peek(index);
				return true;
			}
			key = default;
			return false;
		}
		public bool TryPeek(out T key, out float value, int index = 0)
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
	}

	[Serializable]
	public class HeapInt32 : Heap<int>
	{
	}
}
