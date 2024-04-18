using System.Collections.Generic;
using EthansGameKit.Collections;
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
		/// <summary>
		/// </summary>
		/// <param name="this"></param>
		/// <param name="minDistance">最小距离(含)</param>
		/// <param name="maxDistance">最大距离(不含)</param>
		/// <returns></returns>
		public static IEnumerable<Vector2Int> IterNeighbors(this Vector2Int @this, float minDistance, float maxDistance)
		{
			if (minDistance <= 0)
				yield return @this;
			using var heap = Heap<Vector2Int, float>.Generate();
			minDistance = Mathf.Max(minDistance, 0);
			var minSqrtDistance = minDistance * minDistance;
			var targetSqrtDistance = maxDistance * maxDistance;
			var minRadius = Mathf.Max(Mathf.FloorToInt(minDistance / Mathf.Sqrt(2)), 1);
			for (var radius = minRadius; radius < maxDistance; radius++)
			{
				float sqrDistance;
				for (var xx = -radius; xx <= radius; xx++)
				{
					sqrDistance = xx * xx + radius * radius;
					heap.Add(new(xx + @this.x, -radius + @this.y), sqrDistance);
					heap.Add(new(xx + @this.x, radius + @this.y), sqrDistance);
				}
				for (var yy = -radius + 1; yy < radius; yy++)
				{
					sqrDistance = yy * yy + radius * radius;
					heap.Add(new(-radius + @this.x, yy + @this.y), sqrDistance);
					heap.Add(new(radius + @this.x, yy + @this.y), sqrDistance);
				}
				for (var i = 0; i < 10000; i++)
				{
					if (heap.Count <= 0) break;
					if (heap.TryPeek(out var _, out var value))
					{
						if (value < minSqrtDistance)
						{
							heap.Pop();
						}
						else if (value <= targetSqrtDistance)
						{
							yield return heap.Pop();
						}
						else
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
			}
		}
	}
}
