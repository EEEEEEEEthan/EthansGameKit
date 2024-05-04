using System;
using System.Collections.Generic;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static T RandomPick<T>(this IReadOnlyList<T> @this)
		{
			if (@this.Count == 0) throw new InvalidOperationException("Empty list");
			return @this[RandomUtility.random.Next(0, @this.Count)];
		}

		public static bool TryRandomPick<T>(this IReadOnlyList<T> @this, out T result)
		{
			if (@this.Count == 0)
			{
				result = default;
				return false;
			}
			result = @this[RandomUtility.random.Next(0, @this.Count)];
			return true;
		}
	}
}