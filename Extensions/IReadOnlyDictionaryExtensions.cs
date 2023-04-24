using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, TKey key, TValue defaultValue = default)
		{
			return @this.TryGetValue(key, out var value) ? value : defaultValue;
		}
	}
}
