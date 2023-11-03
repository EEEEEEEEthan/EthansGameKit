using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Collections
{
	[Serializable]
	public struct RangeInt : IReadOnlyList<int>, IList<int>
	{
		[Serializable]
		struct Enumerator : IEnumerator<int>
		{
			[SerializeField] RangeInt range;
			[SerializeField] int index;
			public int Current => range[index];
			object IEnumerator.Current => Current;
			public Enumerator(RangeInt range)
			{
				this.range = range;
				index = -1;
			}
			public bool MoveNext()
			{
				++index;
				if (index == range.count) return false;
				if (index > range.count) throw new InvalidOperationException();
				return true;
			}
			public void Reset()
			{
				index = -1;
			}
			public void Dispose()
			{
			}
		}

		[SerializeField] int min;
		[SerializeField] int count;
		public int Count => count;
		public bool IsReadOnly => true;
		public int Min => min;
		public int IncludedMax => min + count - 1;
		public int Max => min + count;
		public RangeInt(int min, int count)
		{
			this.min = min;
			this.count = count;
		}
		public void Add(int item)
		{
			throw new InvalidOperationException();
		}
		public void Clear()
		{
			throw new InvalidOperationException();
		}
		public bool Contains(int item) => item >= min && item < IncludedMax;
		public void CopyTo(int[] array, int arrayIndex)
		{
			using var enumerator = GetEnumerator();
			while (enumerator.MoveNext())
				array[arrayIndex++] = enumerator.Current;
		}
		public bool Remove(int item) => throw new InvalidOperationException();
		public IEnumerator<int> GetEnumerator() => new Enumerator(this);
		public int IndexOf(int item)
		{
			var index = item - min;
			if (index < 0 || index >= count) return -1;
			return index;
		}
		public void Insert(int index, int item)
		{
			throw new InvalidOperationException();
		}
		public void RemoveAt(int index)
		{
			throw new InvalidOperationException();
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public int this[int index]
		{
			get
			{
				if (index < 0 || index >= count) throw new ArgumentOutOfRangeException($"{nameof(index)}:{index}");
				return min + index;
			}
			set => throw new InvalidOperationException();
		}
	}
}
