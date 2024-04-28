using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EthansGameKit.Collections.Wrappers
{
	public readonly struct DictToDict<TOldKey, TOldValue, TNewKey, TNewValue> : IDictionary<TNewKey, TNewValue>, IReadOnlyDictionary<TNewKey, TNewValue>
	{
		readonly struct KeyCollection : ICollection<TNewKey>
		{
			readonly DictToDict<TOldKey, TOldValue, TNewKey, TNewValue> dict;
			public int Count => dict.Count;
			public bool IsReadOnly => true;
			public KeyCollection(DictToDict<TOldKey, TOldValue, TNewKey, TNewValue> dict) => this.dict = dict;
			public IEnumerator<TNewKey> GetEnumerator()
			{
				var d = dict;
				return d.rawDict.Keys.Select(key => d.oldKey2NewKey.Convert(key)).GetEnumerator();
			}
			public void Add(TNewKey item)
			{
				throw new InvalidOperationException();
			}
			public void Clear()
			{
				throw new InvalidOperationException();
			}
			public bool Contains(TNewKey item)
			{
				if (item is null) throw new ArgumentNullException(nameof(item));
				return dict.ContainsKey(item);
			}
			public void CopyTo(TNewKey[] array, int arrayIndex)
			{
				foreach (var key in this)
				{
					array[arrayIndex++] = key;
				}
			}
			public bool Remove(TNewKey item)
			{
				throw new InvalidOperationException();
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		readonly struct ValueCollection : ICollection<TNewValue>
		{
			readonly DictToDict<TOldKey, TOldValue, TNewKey, TNewValue> dict;
			public int Count => dict.Count;
			public bool IsReadOnly => true;
			public ValueCollection(DictToDict<TOldKey, TOldValue, TNewKey, TNewValue> dict) => this.dict = dict;
			public IEnumerator<TNewValue> GetEnumerator()
			{
				var d = dict;
				return d.rawDict.Values.Select(value => d.oldValue2NewValue.Convert(value)).GetEnumerator();
			}
			public void Add(TNewValue item)
			{
				throw new InvalidOperationException();
			}
			public void Clear()
			{
				throw new InvalidOperationException();
			}
			public bool Contains(TNewValue item)
			{
				foreach (var oldValue in dict.rawDict.Values)
				{
					var newValue = dict.oldValue2NewValue.Convert(oldValue);
					if (EqualityComparer<TNewValue>.Default.Equals(newValue, item))
					{
						return true;
					}
				}
				return false;
			}
			public void CopyTo(TNewValue[] array, int arrayIndex)
			{
				foreach (var value in this)
				{
					array[arrayIndex++] = value;
				}
			}
			public bool Remove(TNewValue item)
			{
				throw new InvalidOperationException();
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		readonly IDictionary<TOldKey, TOldValue> rawDict;
		readonly IConverter<TOldKey, TNewKey> oldKey2NewKey;
		readonly IConverter<TNewKey, TOldKey> newKey2OldKey;
		readonly IConverter<TOldValue, TNewValue> oldValue2NewValue;
		readonly IConverter<TNewValue, TOldValue> newValue2OldValue;
		public int Count => rawDict.Count;
		public ICollection<TNewKey> Keys => new KeyCollection(this);
		public ICollection<TNewValue> Values => new ValueCollection(this);
		bool ICollection<KeyValuePair<TNewKey, TNewValue>>.IsReadOnly => rawDict.IsReadOnly;
		IEnumerable<TNewKey> IReadOnlyDictionary<TNewKey, TNewValue>.Keys => new KeyCollection(this);
		IEnumerable<TNewValue> IReadOnlyDictionary<TNewKey, TNewValue>.Values => new ValueCollection(this);
		public DictToDict(
			IDictionary<TOldKey, TOldValue> rawDict,
			IConverter<TOldKey, TNewKey> oldKey2NewKey,
			IConverter<TNewKey, TOldKey> newKey2OldKey,
			IConverter<TOldValue, TNewValue> oldValue2NewValue,
			IConverter<TNewValue, TOldValue> newValue2OldValue
		)
		{
			this.rawDict = rawDict;
			this.oldKey2NewKey = oldKey2NewKey;
			this.newKey2OldKey = newKey2OldKey;
			this.oldValue2NewValue = oldValue2NewValue;
			this.newValue2OldValue = newValue2OldValue;
		}
		public IEnumerator<KeyValuePair<TNewKey, TNewValue>> GetEnumerator()
		{
			var oldKey2NewKey = this.oldKey2NewKey;
			var oldValue2NewValue = this.oldValue2NewValue;
			return rawDict.Select(pair => new KeyValuePair<TNewKey, TNewValue>(oldKey2NewKey.Convert(pair.Key), oldValue2NewValue.Convert(pair.Value))).GetEnumerator();
		}
		public void Clear()
		{
			rawDict.Clear();
		}
		public void Add(TNewKey key, TNewValue value)
		{
			rawDict.Add(newKey2OldKey.Convert(key), newValue2OldValue.Convert(value));
		}
		public bool Remove(TNewKey key)
		{
			var oldKey = newKey2OldKey.Convert(key);
			return rawDict.Remove(oldKey);
		}
		public bool ContainsKey(TNewKey key)
		{
			var oldKey = newKey2OldKey.Convert(key);
			return rawDict.ContainsKey(oldKey);
		}
		public bool TryGetValue(TNewKey key, out TNewValue value)
		{
			var oldKey = newKey2OldKey.Convert(key);
			if (!rawDict.TryGetValue(oldKey, out var oldValue))
			{
				value = default;
				return false;
			}
			value = oldValue2NewValue.Convert(oldValue);
			return true;
		}
		bool ICollection<KeyValuePair<TNewKey, TNewValue>>.Remove(KeyValuePair<TNewKey, TNewValue> item)
		{
			var oldKey = newKey2OldKey.Convert(item.Key);
			var oldValue = newValue2OldValue.Convert(item.Value);
			if (!(rawDict.TryGetValue(oldKey, out var value) && EqualityComparer<TOldValue>.Default.Equals(value, oldValue)))
				throw new KeyNotFoundException();
			return rawDict.Remove(oldKey);
		}
		void ICollection<KeyValuePair<TNewKey, TNewValue>>.Add(KeyValuePair<TNewKey, TNewValue> item)
		{
			rawDict.Add(newKey2OldKey.Convert(item.Key), newValue2OldValue.Convert(item.Value));
		}
		bool ICollection<KeyValuePair<TNewKey, TNewValue>>.Contains(KeyValuePair<TNewKey, TNewValue> item)
		{
			var oldKey = newKey2OldKey.Convert(item.Key);
			var oldValue = newValue2OldValue.Convert(item.Value);
			return rawDict.TryGetValue(oldKey, out var value) && EqualityComparer<TOldValue>.Default.Equals(value, oldValue);
		}
		void ICollection<KeyValuePair<TNewKey, TNewValue>>.CopyTo(KeyValuePair<TNewKey, TNewValue>[] array, int arrayIndex)
		{
			foreach (var pair in this)
			{
				array[arrayIndex++] = pair;
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		public TNewValue this[TNewKey key]
		{
			get => oldValue2NewValue.Convert(rawDict[newKey2OldKey.Convert(key)]);
			set => rawDict[newKey2OldKey.Convert(key)] = newValue2OldValue.Convert(value);
		}
	}
}
