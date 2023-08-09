using System.Collections.Generic;
using EthansGameKit.CachePools;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void ClearAndRecycle<T>(this Stack<T> @this)
		{
			StackPool<T>.ClearAndRecycle(ref @this);
		}
		public static void Reverse<T>(this Stack<T> @this)
		{
			var list = ListPool<T>.Generate();
			while (@this.TryPop(out var element))
				list.Add(element);
			var count = list.Count;
			for (var i = 0; i < count; ++i)
				@this.Push(list[i]);
			list.ClearAndRecycle();
		}
	}
}
