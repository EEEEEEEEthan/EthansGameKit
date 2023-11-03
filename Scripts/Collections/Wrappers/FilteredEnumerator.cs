using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	public class FilteredEnumerator<T> : IEnumerator<T>
	{
		readonly IEnumerator<T> rawEnumerator;
		readonly IValueFilter<T> filter;
		public T Current => rawEnumerator.Current;
		object IEnumerator.Current => Current;
		public FilteredEnumerator(IEnumerator<T> rawEnumerator, IValueFilter<T> filter)
		{
			this.rawEnumerator = rawEnumerator;
			this.filter = filter;
		}
		public bool MoveNext()
		{
			while (rawEnumerator.MoveNext())
			{
				if (filter.Match(rawEnumerator.Current))
					return true;
			}
			return false;
		}
		public void Reset()
		{
			rawEnumerator.Reset();
		}
		public void Dispose()
		{
			rawEnumerator.Dispose();
		}
	}

	public static partial class Extensions
	{
		public static IEnumerator<T> WrapAsFilteredEnumerator<T>(this IEnumerator<T> rawEnumerator, IValueFilter<T> filter) =>
			new FilteredEnumerator<T>(rawEnumerator, filter);
		public static IEnumerator<T> WrapAsFilteredEnumerator<T>(this IEnumerator<T> rawEnumerator, Func<T, bool> filter) =>
			new FilteredEnumerator<T>(rawEnumerator, new ValueFilter<T>(filter));
	}
}
