using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections.Wrappers
{
	[Obsolete]
	struct ConvertedEnumerator<TRawItem, TNewItem> : IEnumerator<TNewItem>
	{
		readonly IEnumerator<TRawItem> rawEnumerator;
		readonly IValueConverter<TRawItem, TNewItem> converter;
		TNewItem cachedNewItem;
		bool newItemCached;
		public TNewItem Current
		{
			get
			{
				if (newItemCached)
					return cachedNewItem;
				cachedNewItem = converter.Convert(rawEnumerator.Current);
				newItemCached = true;
				return cachedNewItem;
			}
		}
		object IEnumerator.Current => Current;
		public ConvertedEnumerator(IEnumerator<TRawItem> rawEnumerator, IValueConverter<TRawItem, TNewItem> converter)
		{
			this.rawEnumerator = rawEnumerator;
			this.converter = converter;
			cachedNewItem = default;
			newItemCached = false;
		}
		public bool MoveNext()
		{
			cachedNewItem = default;
			newItemCached = false;
			return rawEnumerator.MoveNext();
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
		public static IEnumerator<TNewItem> WrapAsConvertedEnumerator<TRawItem, TNewItem>(this IEnumerator<TRawItem> rawEnumerator, IValueConverter<TRawItem, TNewItem> converter) =>
			new ConvertedEnumerator<TRawItem, TNewItem>(rawEnumerator, converter);
		public static IEnumerator<TNewItem> WrapAsConvertedEnumerator<TRawItem, TNewItem>(this IEnumerator<TRawItem> rawEnumerator, Func<TRawItem, TNewItem> converter) =>
			new ConvertedEnumerator<TRawItem, TNewItem>(rawEnumerator, new ValueConverter<TRawItem, TNewItem>(converter, null));
	}
}
