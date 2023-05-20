using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static Bounds ToBounds(this BoundsInt @this)
		{
			return new(@this.center, @this.size);
		}
	}
}
