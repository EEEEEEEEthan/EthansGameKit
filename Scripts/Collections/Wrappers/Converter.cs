using System;

namespace EthansGameKit.Collections.Wrappers
{
	public interface IConverter<in TOld, out TNew>
	{
		public static IConverter<TOld, TNew> FromFunc(Func<TOld, TNew> converter)
		{
			return new DefaultConverter<TOld, TNew>(converter);
		}
		TNew Convert(TOld old);
	}

	public interface IFilter<in T>
	{
		public static IFilter<T> FromFunc(Func<T, bool> filter)
		{
			return new DefaultFilter<T>(filter);
		}
		bool Filter(T value);
	}

	readonly struct DefaultConverter<TOld, TNew> : IConverter<TOld, TNew>
	{
		readonly Func<TOld, TNew> converter;
		public DefaultConverter(Func<TOld, TNew> converter) => this.converter = converter;
		public TNew Convert(TOld old)
		{
			return converter(old);
		}
	}

	readonly struct DefaultFilter<T> : IFilter<T>
	{
		readonly Func<T, bool> filter;
		public DefaultFilter(Func<T, bool> filter) => this.filter = filter;
		public bool Filter(T value)
		{
			return filter(value);
		}
	}
}
