using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	[Obsolete]
	readonly struct ListToDict<T> : IDictionary<int, T>, IReadOnlyDictionary<int, T>
	{
		readonly struct KeyToPair : IValueConverter<int, KeyValuePair<int, T>>
		{
			readonly IList<T> rawList;
			public KeyToPair(IList<T> rawList) => this.rawList = rawList;
			public KeyValuePair<int, T> Convert(int oldItem) => new(oldItem, rawList[oldItem]);
			public int Recover(KeyValuePair<int, T> newItem) => newItem.Key;
		}

		readonly IList<T> rawList;
		public int Count => rawList.Count;
		public ICollection<int> Keys => new RangeInt(0, Count);
		public ICollection<T> Values => rawList;
		bool ICollection<KeyValuePair<int, T>>.IsReadOnly => false;
		IEnumerable<int> IReadOnlyDictionary<int, T>.Keys => Keys;
		IEnumerable<T> IReadOnlyDictionary<int, T>.Values => Values;
		public ListToDict(IList<T> rawList) => this.rawList = rawList;
		public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
		{
			var rawList = this.rawList;
			return Keys.GetEnumerator().WrapAsConvertedEnumerator(new KeyToPair(rawList));
		}
		public void Clear()
		{
			rawList.Clear();
		}
		public void Add(int key, T value)
		{
			if (key < 0) throw new ArgumentOutOfRangeException();
			var count = rawList.Count;
			for (var i = count; i <= key; ++i)
			{
				rawList.Add(default);
			}
			rawList[key] = value;
		}
		public bool ContainsKey(int key) => key >= 0 && key < rawList.Count;
		public bool Remove(int key)
		{
			if (ContainsKey(key))
			{
				rawList[key] = default;
				return true;
			}
			return false;
		}
		public bool TryGetValue(int key, out T value)
		{
			if (ContainsKey(key))
			{
				value = rawList[key];
				return true;
			}
			value = default;
			return false;
		}
		void ICollection<KeyValuePair<int, T>>.Add(KeyValuePair<int, T> item)
		{
			Add(item.Key, item.Value);
		}
		bool ICollection<KeyValuePair<int, T>>.Contains(KeyValuePair<int, T> item) => ContainsKey(item.Key) && Equals(this[item.Key], item.Value);
		void ICollection<KeyValuePair<int, T>>.CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex++] = item;
			}
		}
		bool ICollection<KeyValuePair<int, T>>.Remove(KeyValuePair<int, T> item)
		{
			if (ContainsKey(item.Key) && Equals(this[item.Key], item.Value))
			{
				rawList[item.Key] = default;
				return true;
			}
			return false;
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public T this[int key]
		{
			get
			{
				if (!ContainsKey(key)) throw new KeyNotFoundException();
				return rawList[key];
			}
			set
			{
				if (key < 0) throw new ArgumentOutOfRangeException();
				rawList[key] = value;
			}
		}
	}

	public static partial class Extensions
	{
		public static IDictionary<int, T> WrapAsDictionary<T>(this IList<T> rawList) => new ListToDict<T>(rawList);
	}
}
