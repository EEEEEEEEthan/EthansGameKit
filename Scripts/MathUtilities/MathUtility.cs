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
		/// <summary>
		///     获得二进制数中1的数量
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static int GetBitCount(int input)
		{
			var count = 0;
			while (input != 0)
			{
				input &= input - 1;
				++count;
			}
			return count;
		}
		/// <summary>
		///     获得二进制数中1的位置
		/// </summary>
		/// <param name="input"></param>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int FindBits(int input, int[] array)
		{
			var count = 0;
			while (input != 0)
			{
				var minusOne = input - 1;
				var andValue = minusOne & input;
				var orValue = minusOne | input;
				var rightmost = (andValue ^ orValue) + 1;
				input = andValue;
				var index = 0;
				if ((rightmost & 0b_11111111_11111111_00000000_00000000) != 0) index += 16;
				if ((rightmost & 0b_11111111_00000000_11111111_00000000) != 0) index += 8;
				if ((rightmost & 0b_11110000_11110000_11110000_11110000) != 0) index += 4;
				if ((rightmost & 0b_11001100_11001100_11001100_11001100) != 0) index += 2;
				if ((rightmost & 0b_10101010_10101010_10101010_10101010) != 0) index += 1;
				array[count++] = index;
			}
			return count;
		}
	}
}
