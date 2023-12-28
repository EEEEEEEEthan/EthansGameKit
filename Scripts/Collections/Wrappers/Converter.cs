using System;

namespace EthansGameKit.Collections.Wrappers
{
	public interface IConverter
	{
		public static IConverter<T, T> Default<T>()
		{
			return new DefaultConverter<T>();
		}
		public static IConverter<TOld, TNew> FromTypes<TOld, TNew>()
		{
			return new DefaultConverter<TOld, TNew>();
		}
		public static IConverter<TOld, TNew> FromFunc<TOld, TNew>(Func<TOld, TNew> converter)
		{
			return new FuncConverter<TOld, TNew>(converter);
		}
	}

	public interface IConverter<in TOld, out TNew> : IConverter
	{
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

	readonly struct DefaultConverter<T> : IConverter<T, T>
	{
		public T Convert(T old)
		{
			return old;
		}
	}

	readonly struct DefaultConverter<TOld, TNew> : IConverter<TOld, TNew>
	{
		public TNew Convert(TOld old)
		{
			return (TNew)(object)old;
		}
	}

	readonly struct FuncConverter<TOld, TNew> : IConverter<TOld, TNew>
	{
		readonly Func<TOld, TNew> converter;
		public FuncConverter(Func<TOld, TNew> converter) => this.converter = converter;
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
