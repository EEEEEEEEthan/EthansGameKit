using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	[Obsolete]
	readonly struct ConvertedCollection<TOld, TNew> : ICollection<TNew>, IReadOnlyCollection<TNew>
	{
		readonly ICollection<TOld> rawCollection;
		readonly IValueConverter<TOld, TNew> converter;
		public int Count => rawCollection.Count;
		public bool IsReadOnly => rawCollection.IsReadOnly;
		public ConvertedCollection(ICollection<TOld> rawCollection, IValueConverter<TOld, TNew> converter)
		{
			this.rawCollection = rawCollection;
			this.converter = converter;
		}
		public IEnumerator<TNew> GetEnumerator() => rawCollection.GetEnumerator().WrapAsConvertedEnumerator(converter);
		public void Add(TNew item)
		{
			rawCollection.Add(converter.Recover(item));
		}
		public void Clear()
		{
			rawCollection.Clear();
		}
		public bool Contains(TNew item) => rawCollection.Contains(converter.Recover(item));
		public void CopyTo(TNew[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex++] = item;
			}
		}
		public bool Remove(TNew item) => rawCollection.Remove(converter.Recover(item));
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public static partial class Extensions
	{
		public static ICollection<TNew> WrapAsConvertedCollection<TOld, TNew>(this ICollection<TOld> rawCollection, IValueConverter<TOld, TNew> converter) =>
			new ConvertedCollection<TOld, TNew>(rawCollection, converter);
	}
}
