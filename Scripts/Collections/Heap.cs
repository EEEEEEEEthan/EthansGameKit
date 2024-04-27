using System;
using System.Buffers;

namespace EthansGameKit.Collections
{
	internal class Heap<TKey, TValue> where TValue : IComparable<TValue>
	{
		TKey[] keys;
		TValue[] values;

		public Heap(int capacity)
		{
			keys = new TKey[capacity];
			values = new TValue[capacity];
		}

		public Heap() : this(1)
		{
		}

		public int Count { get; private set; }

		public void Add(TKey key, TValue value)
		{
			if (keys.Length == Count)
			{
				Array.Resize(ref keys, Count << 1);
				Array.Resize(ref values, Count << 1);
			}
			keys[Count] = key;
			values[Count] = value;
			++Count;
			AdjustUp(Count - 1);
		}

		public void Clear()
		{
			Count = 0;
			keys.Clear();
			values.Clear();
		}

		public void TrimExcess()
		{
			Array.Resize(ref keys, Count);
			Array.Resize(ref values, Count);
		}

		public bool Remove(TKey key) => RemoveAt(Find(key));

		public bool Remove(TKey key, TValue value) => RemoveAt(Find(key, value));

		public void Update(TKey key, TValue oldValue, TValue newValue)
		{
			Update(Find(key, oldValue), newValue);
		}

		public void Update(TKey key, TValue newValue)
		{
			Update(Find(key), newValue);
		}

		public int Find(TKey key)
		{
			for (var i = 0; i < Count; ++i)
				if (keys[i].Equals(key))
					return i;
			return -1;
		}

		public int Find(TKey key, out TValue value)
		{
			var index = Find(key);
			value = index >= 0 ? values[index] : default;
			return index;
		}

		public void Pop()
		{
			RemoveAt(0);
		}

		public TKey Peek() => keys[0];

		public TKey Peek(out TValue value)
		{
			value = values[0];
			return keys[0];
		}

		public bool TryPeek(out TKey key)
		{
			if (Count == 0)
			{
				key = default;
				return false;
			}
			key = keys[0];
			return true;
		}

		public int Find(TKey key, TValue value)
		{
			// 用栈的方式查找，性能更好
			var buffer = ArrayPool<int>.Shared.Rent(Count / 2 + 1);
			buffer[0] = 0;
			var cnt = 1;
			while (cnt > 0)
			{
				var currentIndex = buffer[--cnt];
				if (currentIndex >= Count) continue;
				var currentValue = values[currentIndex];
				var cmp = currentValue.CompareTo(value);
				switch (cmp)
				{
					case 0 when keys[currentIndex].Equals(key):
						// 键值分别相等，直接返回
						ArrayPool<int>.Shared.Return(buffer);
						return currentIndex;
					case < 0:
						// 值比节点更小，那么节点的子树全部舍弃
						continue;
					default:
						// 在子树中查询
						buffer[cnt++] = (currentIndex << 1) + 1;
						buffer[cnt++] = (currentIndex << 1) + 2;
						break;
				}
			}
			ArrayPool<int>.Shared.Return(buffer);
			return -1;
		}

		public bool TryPeek(out TKey key, out TValue value)
		{
			if (Count == 0)
			{
				key = default;
				value = default;
				return false;
			}
			key = keys[0];
			value = values[0];
			return true;
		}

		bool RemoveAt(int index)
		{
			if (index < 0 || index >= Count) return false;
			Swap(index, --Count);
			keys[Count] = default;
			values[Count] = default;
			AdjustDown(index);
			AdjustUp(index);
			return true;
		}

		void Update(int index, TValue newValue)
		{
			values[index] = newValue;
			AdjustDown(index);
			AdjustUp(index);
		}

		void Swap(int i, int j)
		{
			(keys[i], keys[j]) = (keys[j], keys[i]);
			(values[i], values[j]) = (values[j], values[i]);
		}

		void AdjustUp(int i)
		{
			while (i > 0)
			{
				var parent = (i - 1) >> 1;
				if (values[i].CompareTo(values[parent]) < 0) break;
				Swap(i, parent);
				i = parent;
			}
		}

		void AdjustDown(int i)
		{
			while (i < Count)
			{
				var left = (i << 1) + 1;
				if (left >= Count) break;
				var right = left + 1;
				var max = left;
				if (right < Count && values[right].CompareTo(values[left]) > 0) max = right;
				if (values[i].CompareTo(values[max]) >= 0) break;
				Swap(i, max);
				i = max;
			}
		}
	}
}