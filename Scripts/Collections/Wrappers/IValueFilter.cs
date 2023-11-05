using System;

namespace EthansGameKit.Collections.Wrappers
{
	public interface IValueFilter<in T>
	{
		public static IValueFilter<T> Create(Func<T, bool> filter)
		{
			return new ValueFilter<T>(filter);
		}
		bool Match(T item);
	}

	readonly struct ValueFilter<T> : IValueFilter<T>
	{
		readonly Func<T, bool> filter;
		public ValueFilter(Func<T, bool> filter)
		{
			this.filter = filter;
		}
		public bool Match(T item)
		{
			return filter(item);
		}
	}
}
