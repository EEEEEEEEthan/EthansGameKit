using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public static class NoiseUtility
	{
		public static float Noise(Vector2 pos)
		{
			return Mathf.PerlinNoise(pos.x, pos.y);
		}
	}
}
