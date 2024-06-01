using UnityEngine;

namespace EthansGameKit
{
	public static class MathUtility
	{
		public static void Hermite(float p0, float v0, float p1, float v1, float progress, out float point, out float speed)
		{
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = p0 * (2 * t3 - 3 * t2 + 1) + v0 * (t3 - 2 * t2 + t1) + p1 * (-2 * t3 + 3 * t2) + v1 * (t3 - t2);
			speed = p0 * (6 * t2 - 6 * t1) +
					v0 * (3 * t2 - 4 * t1 + 1) +
					p1 * (-6 * t2 + 6 * t1) +
					v1 * (3 * t2 - 2 * t1);
		}
		public static void Hermite(Vector2 p0, Vector2 v0, Vector2 p1, Vector2 v1, float progress, out Vector2 point, out Vector2 speed)
		{
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = p0 * (2 * t3 - 3 * t2 + 1) + v0 * (t3 - 2 * t2 + t1) + p1 * (-2 * t3 + 3 * t2) + v1 * (t3 - t2);
			speed = p0 * (6 * t2 - 6 * t1) +
					v0 * (3 * t2 - 4 * t1 + 1) +
					p1 * (-6 * t2 + 6 * t1) +
					v1 * (3 * t2 - 2 * t1);
		}
		public static void Hermite(Vector3 p0, Vector3 v0, Vector3 p1, Vector3 v1, float progress, out Vector3 point, out Vector3 speed)
		{
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = p0 * (2 * t3 - 3 * t2 + 1) + v0 * (t3 - 2 * t2 + t1) + p1 * (-2 * t3 + 3 * t2) + v1 * (t3 - t2);
			speed = p0 * (6 * t2 - 6 * t1) +
					v0 * (3 * t2 - 4 * t1 + 1) +
					p1 * (-6 * t2 + 6 * t1) +
					v1 * (3 * t2 - 2 * t1);
		}
		/// <summary>
		///     判断是否互质
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool Coprime(int a, int b)
		{
			return GreatestCommonDivisor(a, b) == 1;
		}
		/// <summary>
		///     最大公约数
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int GreatestCommonDivisor(int a, int b)
		{
			while (b != 0)
			{
				var t = b;
				b = a % b;
				a = t;
			}
			return a;
		}
	}
}
