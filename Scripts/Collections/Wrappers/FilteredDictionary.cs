using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	[Obsolete]
	public readonly struct FilteredDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>
	{
		readonly struct KeyToValue : IValueConverter<TKey, TValue>
		{
			readonly IDictionary<TKey, TValue> rawDictionary;
			public KeyToValue(IDictionary<TKey, TValue> rawDictionary) => this.rawDictionary = rawDictionary;
			public TValue Convert(TKey oldItem) => rawDictionary[oldItem];
			public TKey Recover(TValue newItem) => throw new InvalidOperationException();
		}

		readonly IDictionary<TKey, TValue> rawDictionary;
		readonly IValueFilter<TKey> keyFilter;
		public int Count
		{
			get
			{
				var count = 0;
				foreach (var key in rawDictionary.Keys)
					if (keyFilter.Match(key))
						++count;
				return count;
			}
		}
		public ICollection<TKey> Keys => rawDictionary.Keys.WrapAsFilteredCollection(keyFilter);
		public ICollection<TValue> Values => Keys.WrapAsConvertedCollection(new KeyToValue(this));
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
		public FilteredDictionary(IDictionary<TKey, TValue> rawDictionary, IValueFilter<TKey> keyFilter)
		{
			this.rawDictionary = rawDictionary;
			this.keyFilter = keyFilter;
		}
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			var keyFilter = this.keyFilter;
			return rawDictionary.GetEnumerator().WrapAsFilteredEnumerator(pair => keyFilter.Match(pair.Key));
		}
		public void Add(TKey key, TValue value)
		{
			throw new InvalidOperationException();
		}
		public bool Remove(TKey key) => throw new InvalidOperationException();
		public bool ContainsKey(TKey key) => keyFilter.Match(key) && rawDictionary.ContainsKey(key);
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (keyFilter.Match(key) && rawDictionary.TryGetValue(key, out var rawValue))
			{
				value = rawValue;
				return true;
			}
			value = default;
			return false;
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw new InvalidOperationException();
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw new InvalidOperationException();
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			var (key, value) = item;
			if (key is null || !keyFilter.Match(key))
				return false;
			return rawDictionary.TryGetValue(key, out var rawValue) && Equals(rawValue, value);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex++] = item;
			}
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => throw new InvalidOperationException();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public TValue this[TKey key]
		{
			get
			{
				if (!ContainsKey(key)) throw new KeyNotFoundException();
				return rawDictionary[key];
			}
			set => throw new InvalidOperationException();
		}
	}

	public static partial class Extensions
	{
		public static FilteredDictionary<TKey, TValue> WrapAsFilteredDictionary<TKey, TValue>(this IDictionary<TKey, TValue> @this, IValueFilter<TKey> filter) => new(@this, filter);
		public static FilteredDictionary<TKey, TValue> WrapAsFilteredDictionary<TKey, TValue>(this IDictionary<TKey, TValue> @this, Func<TKey, bool> filter) => new(@this, new ValueFilter<TKey>(filter));
	}
}
