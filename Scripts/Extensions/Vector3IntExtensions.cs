using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static Vector2Int XY(this Vector3Int @this)
		{
			return new(@this.x, @this.y);
		}
		public static Vector2Int XZ(this Vector3Int @this)
		{
			return new(@this.x, @this.z);
		}
		public static Vector2Int YZ(this Vector3Int @this)
		{
			return new(@this.y, @this.z);
		}
	}
}
