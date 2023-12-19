using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	[Obsolete]
	public readonly struct FilteredCollection<T> : ICollection<T>, IReadOnlyCollection<T>
	{
		readonly ICollection<T> rawCollection;
		readonly IValueFilter<T> filter;
		public int Count
		{
			get
			{
				var count = 0;
				foreach (var obj in rawCollection)
					if (filter.Match(obj))
						++count;
				return count;
			}
		}
		bool ICollection<T>.IsReadOnly => true;
		public FilteredCollection(ICollection<T> rawCollection, IValueFilter<T> filter)
		{
			this.rawCollection = rawCollection;
			this.filter = filter;
		}
		public IEnumerator<T> GetEnumerator() => rawCollection.GetEnumerator().WrapAsFilteredEnumerator(filter);
		public bool Contains(T item) => item is not null && filter.Match(item) && rawCollection.Contains(item);
		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex++] = item;
			}
		}
		void ICollection<T>.Add(T item)
		{
			throw new InvalidOperationException();
		}
		void ICollection<T>.Clear()
		{
			throw new InvalidOperationException();
		}
		bool ICollection<T>.Remove(T item) => throw new InvalidOperationException();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public static partial class Extensions
	{
		public static FilteredCollection<T> WrapAsFilteredCollection<T>(this ICollection<T> rawEnumerator, IValueFilter<T> filter) => new(rawEnumerator, filter);
		public static FilteredCollection<T> WrapAsFilteredCollection<T>(this ICollection<T> rawEnumerator, Func<T, bool> filter) => new(rawEnumerator, new ValueFilter<T>(filter));
	}
}
