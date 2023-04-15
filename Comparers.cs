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

		public static IEqualityComparer<T> GetDefaultEqualityComparer<T>()
		{
			return DefaultEqualityComparer<T>.instance;
		}
	}
}
