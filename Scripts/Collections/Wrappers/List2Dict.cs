using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EthansGameKit.Collections.Wrappers
{
	public readonly struct List2Dict<T> : IDictionary<int, T>, IReadOnlyDictionary<int, T>
	{
		struct Range : ICollection<int>, IEnumerator<int>
		{
			public readonly int min;
			public int Current { get; private set; }
			public int Count { get; }
			bool ICollection<int>.IsReadOnly => true;
			object IEnumerator.Current => Current;
			public Range(int min, int count)
			{
				this.min = min;
				Count = count;
				Current = -1;
			}
			public IEnumerator<int> GetEnumerator()
			{
				return this;
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}
			void ICollection<int>.Add(int item)
			{
				throw new InvalidOperationException();
			}
			void ICollection<int>.Clear()
			{
				throw new InvalidOperationException();
			}
			bool ICollection<int>.Contains(int item)
			{
				var max = min + Count;
				return item >= min && item < max;
			}
			void ICollection<int>.CopyTo(int[] array, int arrayIndex)
			{
				for (var i = 0; i < Count; i++)
				{
					array[arrayIndex + i] = min + i;
				}
			}
			bool ICollection<int>.Remove(int item)
			{
				throw new InvalidOperationException();
			}
			bool IEnumerator.MoveNext()
			{
				var max = min + Count;
				return ++Current < max;
			}
			void IEnumerator.Reset()
			{
				Current = -1;
			}
			void IDisposable.Dispose()
			{
			}
		}

		readonly IList<T> list;
		public int Count => list.Count;
		public bool IsReadOnly { get; }
		ICollection<int> IDictionary<int, T>.Keys => new Range(0, Count);
		IEnumerable<T> IReadOnlyDictionary<int, T>.Values => list;
		IEnumerable<int> IReadOnlyDictionary<int, T>.Keys => new Range(0, Count);
		ICollection<T> IDictionary<int, T>.Values => list;
		public List2Dict(IList<T> list, bool isReadOnly)
		{
			this.list = list;
			IsReadOnly = isReadOnly;
		}
		public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
		{
			return list.Select((x, i) => new KeyValuePair<int, T>(i, x)).GetEnumerator();
		}
		public bool ContainsKey(int key)
		{
			return key >= 0 && key < Count;
		}
		public bool TryGetValue(int key, out T value)
		{
			if (key < 0 || key >= Count)
			{
				value = default;
				return false;
			}
			value = list[key];
			return true;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		void ICollection<KeyValuePair<int, T>>.Add(KeyValuePair<int, T> item)
		{
			throw new InvalidOperationException();
		}
		void ICollection<KeyValuePair<int, T>>.Clear()
		{
			list.Clear();
		}
		bool ICollection<KeyValuePair<int, T>>.Contains(KeyValuePair<int, T> item)
		{
			return item.Key >= 0 && item.Key < Count && EqualityComparer<T>.Default.Equals(list[item.Key], item.Value);
		}
		void ICollection<KeyValuePair<int, T>>.CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
		{
			for (var i = 0; i < Count; i++)
			{
				array[arrayIndex + i] = new(i, list[i]);
			}
		}
		bool ICollection<KeyValuePair<int, T>>.Remove(KeyValuePair<int, T> item)
		{
			throw new InvalidOperationException();
		}
		void IDictionary<int, T>.Add(int key, T value)
		{
			throw new InvalidOperationException();
		}
		bool IDictionary<int, T>.Remove(int key)
		{
			if (key < 0 || key >= Count)
			{
				return false;
			}
			list.RemoveAt(key);
			return true;
		}
		public T this[int key]
		{
			get => list[key];
			set => list[key] = value;
		}
		public void Add(T value)
		{
			list.Add(value);
		}
	}
}
