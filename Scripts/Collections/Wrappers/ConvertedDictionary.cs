using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	public readonly struct ConvertedDictionary<TOldKey, TOldValue, TNewKey, TNewValue> : IReadOnlyDictionary<TNewKey, TNewValue>, IDictionary<TNewKey, TNewValue>
	{
		readonly IDictionary<TOldKey, TOldValue> rawDictionary;
		readonly IValueConverter<TOldKey, TNewKey> keyConverter;
		readonly IValueConverter<TOldValue, TNewValue> valueConverter;
		public int Count => rawDictionary.Count;
		public ICollection<TNewValue> Values => rawDictionary.Values.WrapAsConvertedCollection(valueConverter);
		public ICollection<TNewKey> Keys => rawDictionary.Keys.WrapAsConvertedCollection(keyConverter);
		bool ICollection<KeyValuePair<TNewKey, TNewValue>>.IsReadOnly => rawDictionary.IsReadOnly;
		IEnumerable<TNewKey> IReadOnlyDictionary<TNewKey, TNewValue>.Keys => Keys;
		IEnumerable<TNewValue> IReadOnlyDictionary<TNewKey, TNewValue>.Values => Values;
		public ConvertedDictionary(IDictionary<TOldKey, TOldValue> rawDictionary, IValueConverter<TOldKey, TNewKey> keyConverter, IValueConverter<TOldValue, TNewValue> valueConverter)
		{
			this.rawDictionary = rawDictionary;
			this.keyConverter = keyConverter;
			this.valueConverter = valueConverter;
		}
		public void Clear()
		{
			throw new NotImplementedException();
		}
		public IEnumerator<KeyValuePair<TNewKey, TNewValue>> GetEnumerator()
		{
			var keyConverter = this.keyConverter;
			var valueConverter = this.valueConverter;
			return rawDictionary.GetEnumerator().WrapAsConvertedEnumerator(pair => new KeyValuePair<TNewKey, TNewValue>(keyConverter.Convert(pair.Key), valueConverter.Convert(pair.Value)));
		}
		public void Add(TNewKey key, TNewValue value)
		{
			var oldKey = keyConverter.Recover(key);
			var oldValue = valueConverter.Recover(value);
			rawDictionary.Add(new(oldKey, oldValue));
		}
		public bool ContainsKey(TNewKey key)
		{
			var oldKey = keyConverter.Recover(key);
			return rawDictionary.ContainsKey(oldKey);
		}
		public bool Remove(TNewKey key)
		{
			var oldKey = keyConverter.Recover(key);
			return rawDictionary.Remove(oldKey);
		}
		public bool TryGetValue(TNewKey key, out TNewValue value)
		{
			var oldKey = keyConverter.Recover(key);
			if (rawDictionary.TryGetValue(oldKey, out var oldValue))
			{
				value = valueConverter.Convert(oldValue);
				return true;
			}
			value = default;
			return false;
		}
		void ICollection<KeyValuePair<TNewKey, TNewValue>>.Add(KeyValuePair<TNewKey, TNewValue> item)
		{
			var oldKey = keyConverter.Recover(item.Key);
			var oldValue = valueConverter.Recover(item.Value);
			rawDictionary.Add(new(oldKey, oldValue));
		}
		bool ICollection<KeyValuePair<TNewKey, TNewValue>>.Contains(KeyValuePair<TNewKey, TNewValue> item)
		{
			var oldKey = keyConverter.Recover(item.Key);
			if (rawDictionary.TryGetValue(oldKey, out var oldValue))
				return Equals(oldValue, item.Value);
			return false;
		}
		void ICollection<KeyValuePair<TNewKey, TNewValue>>.CopyTo(KeyValuePair<TNewKey, TNewValue>[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex++] = item;
			}
		}
		bool ICollection<KeyValuePair<TNewKey, TNewValue>>.Remove(KeyValuePair<TNewKey, TNewValue> item)
		{
			var oldKey = keyConverter.Recover(item.Key);
			var oldValue = valueConverter.Recover(item.Value);
			return rawDictionary.Remove(new KeyValuePair<TOldKey, TOldValue>(oldKey, oldValue));
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		public TNewValue this[TNewKey key]
		{
			get
			{
				var oldKey = keyConverter.Recover(key);
				return valueConverter.Convert(rawDictionary[oldKey]);
			}
			set
			{
				var oldKey = keyConverter.Recover(key);
				rawDictionary[oldKey] = valueConverter.Recover(value);
			}
		}
	}

	public static partial class Extensions
	{
		public static ConvertedDictionary<TOldKey, TOldValue, TNewKey, TNewValue> WrapAsConvertedDictionary<TOldKey, TOldValue, TNewKey, TNewValue>(this IDictionary<TOldKey, TOldValue> @this,
			IValueConverter<TOldKey, TNewKey> keyConverter, IValueConverter<TOldValue, TNewValue> valueConverter)
		{
			return new(@this, keyConverter, valueConverter);
		}
		public static ConvertedDictionary<TKey, TOldValue, TKey, TNewValue> WrapAsConvertedDictionary<TKey, TOldValue, TNewValue>(
			this IDictionary<TKey, TOldValue> @this,
			IValueConverter<TOldValue, TNewValue> valueConverter)
		{
			return @this.WrapAsConvertedDictionary(DefaultConverter<TKey>.Default, valueConverter);
		}
		public static ConvertedDictionary<TKey, TOldValue, TKey, TNewValue> WrapAsConvertedDictionary<TKey, TOldValue, TNewValue>(
			this IDictionary<TKey, TOldValue> @this,
			Func<TOldValue, TNewValue> converter)
		{
			return @this.WrapAsConvertedDictionary(DefaultConverter<TKey>.Default, new ValueConverter<TOldValue, TNewValue>(converter, null));
		}
		public static ConvertedDictionary<TKey, TOldValue, TKey, TNewValue> WrapAsConvertedDictionary<TKey, TOldValue, TNewValue>(
			this IDictionary<TKey, TOldValue> @this)
		{
			return @this.WrapAsConvertedDictionary(DefaultConverter<TKey>.Default, IValueConverter<TOldValue, TNewValue>.Default);
		}
	}
}
