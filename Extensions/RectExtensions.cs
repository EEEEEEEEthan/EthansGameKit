// ReSharper disable once CheckNamespace

using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static Vector2 RandomPosition(this Rect @this)
		{
			return new(Random.Range(@this.xMin, @this.xMax), Random.Range(@this.yMin, @this.yMax));
		}
	}
}
