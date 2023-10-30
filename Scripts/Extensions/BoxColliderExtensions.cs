using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Contains(this BoxCollider @this, Vector3 position)
		{
			if (!@this) return false;
			return @this.ClosestPoint(position) == position;
		}
	}
}
