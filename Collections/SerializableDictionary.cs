using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Collections
{
	[Serializable]
	public abstract class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary, ISerializationCallbackReceiver
	{
		readonly Dictionary<TKey, TValue> dict;
		[SerializeField, HideInInspector] Data[] serializeData;
		public IEqualityComparer<TKey> Comparer => dict.Comparer;
		public int Count => dict.Count;
		public Dictionary<TKey, TValue>.KeyCollection Keys => dict.Keys;
		public Dictionary<TKey, TValue>.ValueCollection Values => dict.Values;
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dict).IsReadOnly;
		ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
		ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
		bool ICollection.IsSynchronized => ((ICollection)dict).IsSynchronized;
		object ICollection.SyncRoot => ((ICollection)dict).SyncRoot;
		bool IDictionary.IsFixedSize => ((IDictionary)dict).IsFixedSize;
		bool IDictionary.IsReadOnly => ((IDictionary)dict).IsReadOnly;
		ICollection IDictionary.Keys => Keys;
		ICollection IDictionary.Values => Values;
		protected SerializableDictionary()
		{
			dict = new();
		}
		protected SerializableDictionary(IDictionary<TKey, TValue> dictionary)
		{
			dict = new(dictionary);
		}
		protected SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		{
			dict = new(dictionary, comparer);
		}
		protected SerializableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
		{
			dict = new(collection);
		}
		protected SerializableDictionary(
			IEnumerable<KeyValuePair<TKey, TValue>> collection,
			IEqualityComparer<TKey> comparer
		)
		{
			dict = new(collection, comparer);
		}
		protected SerializableDictionary(IEqualityComparer<TKey> comparer)
		{
			dict = new(comparer);
		}
		protected SerializableDictionary(int capacity)
		{
			dict = new(capacity);
		}
		protected SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			dict = new(capacity, comparer);
		}
		public void Add(TKey key, TValue value)
		{
			dict.Add(key, value);
		}
		public void Clear()
		{
			dict.Clear();
		}
		public bool ContainsKey(TKey key)
		{
			return dict.ContainsKey(key);
		}
		public bool Remove(TKey key)
		{
			return dict.Remove(key);
		}
		public bool TryGetValue(TKey key, out TValue value)
		{
			return dict.TryGetValue(key, out value);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)dict).Add(keyValuePair);
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)dict).Contains(keyValuePair);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)dict).CopyTo(array, index);
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)dict).Remove(keyValuePair);
		}
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)dict).GetEnumerator();
		}
		void IDictionary.Add(object key, object value)
		{
			((IDictionary)dict).Add(key, value);
		}
		bool IDictionary.Contains(object key)
		{
			return ((IDictionary)dict).Contains(key);
		}
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)dict).GetEnumerator();
		}
		void IDictionary.Remove(object key)
		{
			((IDictionary)dict).Remove(key);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)dict).GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)dict).CopyTo(array, index);
		}
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			serializeData = new Data[Count];
			var i = 0;
			foreach (var pair in dict)
			{
				serializeData[i++] = new()
				{
					key = pair.Key,
					value = pair.Value,
				};
			}
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			dict.Clear();
			foreach (var pair in serializeData)
				dict.Add(pair.key, pair.value);
			serializeData = null;
		}

		[Serializable]
		struct Data
		{
			[SerializeField] public TKey key;
			[SerializeField] public TValue value;
		}

		public TValue this[TKey key]
		{
			get => dict[key];
			set => dict[key] = value;
		}

		object IDictionary.this[object key]
		{
			get => ((IDictionary)dict)[key];
			set => ((IDictionary)dict)[key] = value;
		}

		public bool ContainsValue(TValue value)
		{
			return dict.ContainsValue(value);
		}
		public int EnsureCapacity(int capacity)
		{
			return dict.EnsureCapacity(capacity);
		}
		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return dict.GetEnumerator();
		}
		public bool Remove(TKey key, out TValue value)
		{
			return dict.Remove(key, out value);
		}
		public void TrimExcess()
		{
			dict.TrimExcess();
		}
		public void TrimExcess(int capacity)
		{
			dict.TrimExcess(capacity);
		}
		public bool TryAdd(TKey key, TValue value)
		{
			return dict.TryAdd(key, value);
		}
	}
}