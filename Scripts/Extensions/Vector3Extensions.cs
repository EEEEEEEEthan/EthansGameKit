using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static Vector3Int FloorToInt(this Vector3 @this)
		{
			return Vector3Int.FloorToInt(@this);
		}
		public static Vector3Int RoundToInt(this Vector3 @this)
		{
			return Vector3Int.RoundToInt(@this);
		}
		public static Vector3Int CeilToInt(this Vector3 @this)
		{
			return Vector3Int.CeilToInt(@this);
		}
		public static Vector3 Cross(this Vector3 @this, Vector3 other)
		{
			return Vector3.Cross(@this, other);
		}
		public static Vector2 XZ(this Vector3 @this)
		{
			return new(@this.x, @this.z);
		}
		public static Vector2 XY(this Vector3 @this)
		{
			return new(@this.x, @this.y);
		}
		public static Vector2 YZ(this Vector3 @this)
		{
			return new(@this.y, @this.z);
		}
	}
}
