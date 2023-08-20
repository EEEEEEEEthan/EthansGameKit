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
		public static Vector3 RandomPosition(this Bounds bounds)
		{
			return new(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y),
				Random.Range(bounds.min.z, bounds.max.z)
			);
		}
	}
}
