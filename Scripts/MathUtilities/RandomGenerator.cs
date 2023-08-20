using System;
using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	[Serializable]
	public struct RandomGenerator
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
		[SerializeField] uint seed;
		public RandomGenerator(object seed)
		{
			this.seed = (uint)seed.GetHashCode();
		}
		public uint NextUInt()
		{
			return seed = Lcg.Next(seed);
		}
		public int NextInt()
		{
			return (int)NextUInt();
		}
		public float NextFloat()
		{
			var value = NextUInt();
			return (float)(value / (double)uint.MaxValue);
		}
		public Vector2 NextVector2()
		{
			return new(NextFloat(), NextFloat());
		}
	}
}
