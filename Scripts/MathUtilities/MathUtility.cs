using JetBrains.Annotations;
using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public static class MathUtility
	{
		[PublicAPI]
		public static float Hermite(float pos0, float weight0, float pos1, float weight1, float progress)
		{
			switch (progress)
			{
				case <= 0:
					return pos0;
				case >= 1:
					return pos1;
			}
			// ReSharper disable once InlineTemporaryVariable
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			return pos0 * (2 * t3 - 3 * t2 + 1) + weight0 * (t3 - 2 * t2 + t1) + pos1 * (-2 * t3 + 3 * t2) + weight1 * (t3 - t2);
		}
		[PublicAPI]
		public static void Hermite(float pos0, float weight0, float pos1, float weight1, float progress, out float point, out float weight)
		{
			switch (progress)
			{
				case <= 0:
					point = pos0;
					weight = weight0;
					return;
				case >= 1:
					point = pos1;
					weight = weight1;
					return;
			}
			// ReSharper disable once InlineTemporaryVariable
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = pos0 * (2 * t3 - 3 * t2 + 1) + weight0 * (t3 - 2 * t2 + t1) + pos1 * (-2 * t3 + 3 * t2) + weight1 * (t3 - t2);
			weight = pos0 * (6 * t2 - 6 * t1) + weight0 * (3 * t2 - 4 * t1 + 1) + pos1 * (-6 * t2 + 6 * t1) + weight1 * (3 * t2 - 2 * t1);
		}
		[PublicAPI]
		public static void Hermite(Vector2 pos0, Vector2 weight0, Vector2 pos1, Vector2 weight1, float progress, out Vector2 point, out Vector2 weight)
		{
			switch (progress)
			{
				case <= 0:
					point = pos0;
					weight = weight0;
					return;
				case >= 1:
					point = pos1;
					weight = weight1;
					return;
			}
			// ReSharper disable once InlineTemporaryVariable
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = pos0 * (2 * t3 - 3 * t2 + 1) + weight0 * (t3 - 2 * t2 + t1) + pos1 * (-2 * t3 + 3 * t2) + weight1 * (t3 - t2);
			weight = pos0 * (6 * t2 - 6 * t1) + weight0 * (3 * t2 - 4 * t1 + 1) + pos1 * (-6 * t2 + 6 * t1) + weight1 * (3 * t2 - 2 * t1);
		}
		[PublicAPI]
		public static void Hermite(Vector3 pos0, Vector3 weight0, Vector3 pos1, Vector3 weight1, float progress, out Vector3 point, out Vector3 weight)
		{
			switch (progress)
			{
				case <= 0:
					point = pos0;
					weight = weight0;
					return;
				case >= 1:
					point = pos1;
					weight = weight1;
					return;
			}
			// ReSharper disable once InlineTemporaryVariable
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = pos0 * (2 * t3 - 3 * t2 + 1) + weight0 * (t3 - 2 * t2 + t1) + pos1 * (-2 * t3 + 3 * t2) + weight1 * (t3 - t2);
			weight = pos0 * (6 * t2 - 6 * t1) + weight0 * (3 * t2 - 4 * t1 + 1) + pos1 * (-6 * t2 + 6 * t1) + weight1 * (3 * t2 - 2 * t1);
		}
		/// <summary>
		///     正态分布随机数
		/// </summary>
		/// <returns>符合正态分布的随机数,值域是[float.MinValue, float.MaxValue]</returns>
		[PublicAPI]
		public static float RandomNormal()
		{
			var u1 = Random.value;
			var u2 = Random.value;
			var z = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
			return z;
		}
		
		public static int GetOne(int input)
		{
			var count = 0;
			while (input != 0)
			{
				input &= input - 1;
				++count;
			}
			return count;
		}

		public static int FindBits(int input, int[] array)
		{
			var count = 0;
			while (input != 0)
			{
				var minusOne = input - 1;
				var andValue = minusOne & input;
				var orValue = minusOne | input;
				var rightmostBit = (andValue ^ orValue) + 1;
				input = andValue;
				array[count++] = rightmostBit switch
				{
					1 << 0 => 0,
					1 << 1 => 1,
					1 << 2 => 2,
					1 << 3 => 3,
					1 << 4 => 4,
					1 << 5 => 5,
					1 << 6 => 6,
					1 << 7 => 7,
					1 << 8 => 8,
					1 << 9 => 9,
					1 << 10 => 10,
					1 << 11 => 11,
					1 << 12 => 12,
					1 << 13 => 13,
					1 << 14 => 14,
					1 << 15 => 15,
					1 << 16 => 16,
					1 << 17 => 17,
					1 << 18 => 18,
					1 << 19 => 19,
					1 << 20 => 20,
					1 << 21 => 21,
					1 << 22 => 22,
					1 << 23 => 23,
					1 << 24 => 24,
					1 << 25 => 25,
					1 << 26 => 26,
					1 << 27 => 27,
					1 << 28 => 28,
					1 << 29 => 29,
					1 << 30 => 30,
					1 << 31 => 31,
				};
			}
			return count;
		}
	}
}
