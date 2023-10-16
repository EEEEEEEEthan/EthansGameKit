using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections
{
	public class ListDictionary<TKey, TValue> : IDictionary<TKey, List<TValue>>, IReadOnlyDictionary<TKey, IReadOnlyList<TValue>>
	{
		readonly Dictionary<TKey, List<TValue>> rawDictionary;
		public int Count => rawDictionary.Count;
		public ICollection<TKey> Keys => rawDictionary.Keys;
		public ICollection<List<TValue>> Values => rawDictionary.Values;
		bool ICollection<KeyValuePair<TKey, List<TValue>>>.IsReadOnly => false;
		IEnumerable<TKey> IReadOnlyDictionary<TKey, IReadOnlyList<TValue>>.Keys => Keys;
		IEnumerable<IReadOnlyList<TValue>> IReadOnlyDictionary<TKey, IReadOnlyList<TValue>>.Values => rawDictionary.Values;
		public ListDictionary()
		{
			rawDictionary = new();
		}
		public ListDictionary(int capacity)
		{
			rawDictionary = new(capacity);
		}
		public ListDictionary(IEqualityComparer<TKey> comparer)
		{
			rawDictionary = new(comparer);
		}
		public ListDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			rawDictionary = new(capacity, comparer);
		}
		public ListDictionary(IDictionary<TKey, List<TValue>> dictionary)
		{
			rawDictionary = new(dictionary);
		}
		public ListDictionary(IDictionary<TKey, List<TValue>> dictionary, IEqualityComparer<TKey> comparer)
		{
			rawDictionary = new(dictionary, comparer);
		}
		public void Clear()
		{
			rawDictionary.Clear();
		}
		public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
		{
			return rawDictionary.GetEnumerator();
		}
		public void Add(TKey key, List<TValue> value)
		{
			rawDictionary.Add(key, value);
		}
		public bool ContainsKey(TKey key)
		{
			return rawDictionary.ContainsKey(key);
		}
		public bool TryGetValue(TKey key, out List<TValue> value)
		{
			return rawDictionary.TryGetValue(key, out value);
		}
		public bool Remove(TKey key)
		{
			return rawDictionary.Remove(key);
		}
		public void Add(KeyValuePair<TKey, List<TValue>> item)
		{
			rawDictionary.Add(item.Key, item.Value);
		}
		public bool Remove(KeyValuePair<TKey, List<TValue>> item)
		{
			return rawDictionary.Remove(item.Key);
		}
		IEnumerator<KeyValuePair<TKey, IReadOnlyList<TValue>>> IEnumerable<KeyValuePair<TKey, IReadOnlyList<TValue>>>.GetEnumerator()
		{
			foreach (var (key, value) in rawDictionary)
				yield return new(key, value);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		bool ICollection<KeyValuePair<TKey, List<TValue>>>.Contains(KeyValuePair<TKey, List<TValue>> item)
		{
			return rawDictionary.TryGetValue(item.Key, out var value) && value == item.Value;
		}
		void ICollection<KeyValuePair<TKey, List<TValue>>>.CopyTo(KeyValuePair<TKey, List<TValue>>[] array, int arrayIndex)
		{
			foreach (var pair in rawDictionary)
				array[arrayIndex++] = pair;
		}
		bool IReadOnlyDictionary<TKey, IReadOnlyList<TValue>>.TryGetValue(TKey key, out IReadOnlyList<TValue> value)
		{
			if (TryGetValue(key, out var v))
			{
				value = v;
				return true;
			}
			value = default;
			return false;
		}
		public List<TValue> this[TKey key]
		{
			get => rawDictionary[key];
			set => rawDictionary[key] = value;
		}
		IReadOnlyList<TValue> IReadOnlyDictionary<TKey, IReadOnlyList<TValue>>.this[TKey key] => this[key];
	}
}
