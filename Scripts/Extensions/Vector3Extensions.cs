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
	}
}
