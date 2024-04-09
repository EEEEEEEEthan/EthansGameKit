using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static Vector2Int FloorToInt(this Vector2 @this)
		{
			return Vector2Int.FloorToInt(@this);
		}
		public static Vector2Int RoundToInt(this Vector2 @this)
		{
			return Vector2Int.RoundToInt(@this);
		}
		public static Vector2Int CeilToInt(this Vector2 @this)
		{
			return Vector2Int.CeilToInt(@this);
		}
		public static Vector3 X_Z(this Vector2 @this, float y = 0)
		{
			return new(@this.x, y, @this.y);
		}
	}
}
