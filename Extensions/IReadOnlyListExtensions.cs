using System.Collections.Generic;
using EthansGameKit.CachePools;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static List<KeyValuePair<T, int>> RunLengthEncoding<T>(this IReadOnlyList<T> @this)
		{
			var length = @this.Count;
			var result = ListPool<KeyValuePair<T, int>>.Generate();
			if (length <= 0) return result;
			var current = @this[0];
			var count = 1;
			for (var i = 1; i < length; ++i)
			{
				var value = @this[i];
				if (value is null)
				{
					if (current is null)
					{
						goto EQUALS;
					}
					goto NOT_EQUALS;
				}
				if (value.Equals(current))
				{
					goto EQUALS;
				}
				goto NOT_EQUALS;
			EQUALS: ;
				++count;
				continue;
			NOT_EQUALS: ;
				result.Add(new(current, count));
				current = value;
				count = 1;
			}
			result.Add(new(current, count));
			return result;
		}
	}
}
