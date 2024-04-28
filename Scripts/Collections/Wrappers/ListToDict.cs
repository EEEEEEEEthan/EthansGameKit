using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EthansGameKit.Collections.Wrappers
{
	public readonly struct ListToDict<T> : IDictionary<int, T>, IReadOnlyDictionary<int, T>
	{
		readonly IList<T> list;
		readonly IFilter<int> filter;
		public int Count => Keys.Count();
		public bool IsReadOnly => list.IsReadOnly;
		public IEnumerable<int> Keys
		{
			get
			{
				var count = list.Count;
				for (var i = 0; i < count; ++i)
				{
					if (filter.Filter(i))
						yield return i;
				}
			}
		}
		public IEnumerable<T> Values
		{
			get
			{
				foreach (var key in Keys)
					yield return list[key];
			}
		}
		ICollection<int> IDictionary<int, T>.Keys => throw new InvalidOperationException();
		ICollection<T> IDictionary<int, T>.Values => throw new InvalidOperationException();
		public ListToDict(IList<T> list, IFilter<int> filter)
		{
			this.list = list;
			this.filter = filter;
		}
		public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
		{
			foreach (var key in Keys)
				yield return new(key, list[key]);
		}
		public bool ContainsKey(int key)
		{
			if (key < 0 || key >= Count) return false;
			return filter is null || filter.Filter(key);
		}
		public bool TryGetValue(int key, out T value)
		{
			if (!ContainsKey(key))
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
			return ContainsKey(item.Key) && EqualityComparer<T>.Default.Equals(list[item.Key], item.Value);
		}
		void ICollection<KeyValuePair<int, T>>.CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
		{
			foreach (var pair in this)
			{
				array[arrayIndex++] = pair;
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
			throw new InvalidOperationException();
		}
		public T this[int key]
		{
			get
			{
				if (!ContainsKey(key)) throw new KeyNotFoundException();
				return list[key];
			}
			set => list[key] = value;
		}
	}
}
