using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Collections
{
	[Serializable]
	public class Bidictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable
	{
		[Serializable]
		struct Data
		{
			[SerializeField] public TKey key;
			[SerializeField] public TValue value;
		}

		readonly Dictionary<TKey, TValue> key2Value = new();
		readonly Dictionary<TValue, TKey> value2Key = new();
		[SerializeField] Data[] serializeData;
		IEnumerator IEnumerable.GetEnumerator()
		{
			return key2Value.GetEnumerator();
		}
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			serializeData = new Data[key2Value.Count];
			var index = 0;
			foreach (var pair in key2Value)
				serializeData[index++] = new() { key = pair.Key, value = pair.Value };
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			foreach (var pair in serializeData)
			{
				key2Value[pair.key] = pair.value;
				value2Key[pair.value] = pair.key;
			}
			serializeData = null;
		}
		public TValue this[TKey key]
		{
			get => this[key];
			set
			{
				RemoveKey(key);
				Add(key, value);
			}
		}
		public void Add(TKey key, TValue value)
		{
			if (ContainsKey(key)) throw new ArgumentException($"Key {key} already exists");
			if (ContainsValue(value)) throw new ArgumentException($"Value {value} already exists");
			key2Value.Add(key, value);
			value2Key.Add(value, key);
		}
		public TValue GetValue(TKey key)
		{
			return key2Value[key];
		}
		public TValue GetValueOrDefault(TKey key, TValue @default = default)
		{
			return key2Value.GetValueOrDefault(key);
		}
		public TKey GetKey(TValue value)
		{
			return value2Key[value];
		}
		public TKey GetKeyOrDefault(TValue value, TKey @default = default)
		{
			return value2Key.GetValueOrDefault(value, @default);
		}
		public bool TryGetValue(TKey key, out TValue value)
		{
			return key2Value.TryGetValue(key, out value);
		}
		public bool TryGetKey(TValue value, out TKey key)
		{
			return value2Key.TryGetValue(value, out key);
		}
		public bool ContainsKey(TKey key)
		{
			return key2Value.ContainsKey(key);
		}
		public bool ContainsValue(TValue value)
		{
			return value2Key.ContainsKey(value);
		}
		public bool RemoveKey(TKey key)
		{
			if (key2Value.Remove(key, out var value))
			{
				value2Key.Remove(value);
				return true;
			}
			return false;
		}
		public bool RemoveValue(TValue value)
		{
			if (value2Key.Remove(value, out var key))
			{
				key2Value.Remove(key);
				return true;
			}
			return false;
		}
	}
}
