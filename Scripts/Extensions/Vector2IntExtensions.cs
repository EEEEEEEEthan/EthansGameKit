using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static IEnumerable<Vector2Int> IterCellsAtManhattanDistance(this Vector2Int @this, int radius)
		{
			var rect = new RectInt(@this.x - radius, @this.y - radius, radius * 2 + 1, radius * 2 + 1);
			return rect.EdgePositions();
		}

		public static IEnumerable<Vector2Int> IterCellsInManhattanDistance(this Vector2Int @this, int radius)
		{
			return new ManhattanEnumerator(@this, radius);
		}

		struct ManhattanEnumerator : IEnumerator<Vector2Int>, IEnumerable<Vector2Int>
		{
			static IEnumerator<Vector2Int> GetEdgeEnumerator(Vector2Int center, int radius)
			{
				var width = (radius << 1) + 1;
				var rect = new RectInt(center.x - radius, center.y - radius, width, width);
				var edgeEnumerator = rect.EdgePositions().GetEnumerator();
				return edgeEnumerator;
			}

			readonly Vector2Int center;
			int radius;
			IEnumerator<Vector2Int> edgeEnumerator;

			public ManhattanEnumerator(Vector2Int center, int radius)
			{
				this.center = center;
				this.radius = radius;
				edgeEnumerator = GetEdgeEnumerator(center, 0);
			}

			public Vector2Int Current => edgeEnumerator.Current;

			object IEnumerator.Current => Current;

			public bool MoveNext()
			{
				while (!edgeEnumerator.MoveNext())
				{
					++radius;
					edgeEnumerator = GetEdgeEnumerator(center, radius);
				}
				return true;
			}

			public void Reset()
			{
				edgeEnumerator = null;
			}

			public void Dispose()
			{
			}

			public IEnumerator<Vector2Int> GetEnumerator() => this;

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}