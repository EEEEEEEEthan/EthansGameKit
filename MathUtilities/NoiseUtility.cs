using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public static class NoiseUtility
	{
		public static float Noise(Vector2 pos)
		{
			return Mathf.PerlinNoise(pos.x, pos.y);
		}
		public static float Noise2(Vector2 pos)
		{
			return (Mathf.PerlinNoise(pos.x, pos.y) * 2 + Mathf.PerlinNoise(pos.x * 2 + 123, pos.y * 2 + 456)) / 3;
		}
		public static float Noise3(Vector2 pos)
		{
			return (
				Mathf.PerlinNoise(pos.x, pos.y) * 3 +
				Mathf.PerlinNoise(pos.x * 2 + 123, pos.y * 2 + 456) * 2 +
				Mathf.PerlinNoise(pos.x * 4 + 789, pos.y * 4 + 101112)
			) / 6;
		}
	}
}
