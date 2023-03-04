using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EthansGameKit
{
	public static class MathUtility
	{
		[Obsolete]
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
		[Obsolete]
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
		[Obsolete]
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
		/// <param name="mean">正态分布的平均值</param>
		/// <param name="stdDev">标准差</param>
		/// <returns>符合正态分布的随机数</returns>
		public static float RandomNormal(float mean, float stdDev = 1)
		{
			var u1 = Random.value;
			var u2 = Random.value;
			var z = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
			return mean + stdDev * z;
		}
	}
}
