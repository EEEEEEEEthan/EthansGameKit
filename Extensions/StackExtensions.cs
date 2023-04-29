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
	}
}
