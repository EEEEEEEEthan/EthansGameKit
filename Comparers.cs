using System.Collections.Generic;

namespace EthansGameKit
{
	public static class Comparers
	{
		class DefaultEqualityComparer<T> : IEqualityComparer<T>
		{
			public static readonly IEqualityComparer<T> instance = new DefaultEqualityComparer<T>();
			public bool Equals(T x, T y)
			{
				if (x is null) return y is null;
				return x.Equals(y);
			}
			public int GetHashCode(T obj)
			{
				return obj.GetHashCode();
			}
		}

		class DefaultInt32EqualityComparer : IEqualityComparer<int>
		{
			public static readonly IEqualityComparer<int> instance = new DefaultInt32EqualityComparer();
			public bool Equals(int x, int y)
			{
				return x == y;
			}
			public int GetHashCode(int obj)
			{
				return obj.GetHashCode();
			}
		}

		public static IEqualityComparer<T> GetDefaultEqualityComparer<T>()
		{
			if (typeof(T) == typeof(int))
				return (IEqualityComparer<T>)DefaultInt32EqualityComparer.instance;
			return DefaultEqualityComparer<T>.instance;
		}
	}
}
