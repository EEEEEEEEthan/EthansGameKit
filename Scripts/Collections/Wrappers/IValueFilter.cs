using System;

namespace EthansGameKit.Collections.Wrappers
{
	public interface IValueFilter<in T>
	{
		bool Match(T item);
	}

	public readonly struct ValueFilter<T> : IValueFilter<T>
	{
		readonly Func<T, bool> filter;
		public ValueFilter(Func<T, bool> filter) => this.filter = filter;
		public bool Match(T item) => filter(item);
	}
}
