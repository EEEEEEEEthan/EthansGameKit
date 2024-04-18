using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public readonly struct Line2D
	{
		public readonly float A;
		public readonly float B;
		public readonly float C;
		public Line2D(Vector2 p1, Vector2 p2)
		{
			if (p1 == p2)
				throw new System.ArgumentException("p1 and p2 cannot be the same");
			A = p2.y - p1.y;
			B = p1.x - p2.x;
			C = p2.x * p1.y - p1.x * p2.y;
		}
		public Line2D(float a, float b, float c)
		{
			if (a == 0 && b == 0)
				throw new System.ArgumentException("a and b cannot be both zero");
			A = a;
			B = b;
			C = c;
		}
		public Line2D(Vector2 p, float k)
		{
			// y = kx + b
			// kx - y + b = 0
			A = k;
			B = -1;
			C = p.y - k * p.x;
		}
		public float CalculateX(float y)
		{
			if (A == 0) throw new System.InvalidOperationException("A cannot be zero");
			return (-B * y - C) / A;
		}
		public float CalculateY(float x)
		{
			if (B == 0) throw new System.InvalidOperationException("B cannot be zero");
			return (-A * x - C) / B;
		}
	}
}
