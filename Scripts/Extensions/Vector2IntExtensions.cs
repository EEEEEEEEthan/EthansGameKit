using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static Vector3Int X_Z(this Vector2Int @this, int y = 0)
		{
			return new(@this.x, y, @this.y);
		}
	}
}
