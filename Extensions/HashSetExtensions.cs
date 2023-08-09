using System.Collections.Generic;
using EthansGameKit.CachePools;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void ClearAndRecycle<T>(this HashSet<T> @this)
		{
			HashSetPool<T>.ClearAndRecycle(ref @this);
		}
		public static void AddRange<T>(this HashSet<T> @this, IEnumerable<T> collection)
		{
			foreach (var item in collection)
				@this.Add(item);
		}
	}
}
