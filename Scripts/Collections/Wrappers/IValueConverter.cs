using System;

namespace EthansGameKit.Collections.Wrappers
{
	public interface IValueConverter<TRawItem, TNewItem>
	{
		public static IValueConverter<TRawItem, TNewItem> Default => new DefaultConverter<TRawItem, TNewItem>();
		TNewItem Convert(TRawItem oldItem);
		TRawItem Recover(TNewItem newItem);
	}

	struct DefaultConverter<TRawItem, TNewItem> : IValueConverter<TRawItem, TNewItem>
	{
		public TNewItem Convert(TRawItem oldItem) => (TNewItem)(object)oldItem;
		public TRawItem Recover(TNewItem newItem) => (TRawItem)(object)newItem;
	}

	public readonly struct ValueConverter<TRawItem, TNewItem> : IValueConverter<TRawItem, TNewItem>
	{
		readonly Func<TRawItem, TNewItem> convert;
		readonly Func<TNewItem, TRawItem> recover;
		public ValueConverter(Func<TRawItem, TNewItem> convert, Func<TNewItem, TRawItem> recover)
		{
			this.convert = convert;
			this.recover = recover;
		}
		public TNewItem Convert(TRawItem oldItem) => convert(oldItem);
		public TRawItem Recover(TNewItem newItem) => recover(newItem);
	}
}
