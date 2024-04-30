using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static IEnumerable<Vector2Int> EdgePositions(this RectInt @this) => new RectEdgeEnumerator(@this);

		struct RectEdgeEnumerator : IEnumerator<Vector2Int>, IEnumerable<Vector2Int>
		{
			RectInt rect;
			bool started;
			Vector2Int current;

			public RectEdgeEnumerator(RectInt rect)
			{
				this.rect = rect;
				started = false;
				current = default;
			}

			public Vector2Int Current => current;

			object IEnumerator.Current => current;

			public bool MoveNext()
			{
				if (!started)
				{
					started = true;
					if (rect.width <= 0 || rect.height <= 0) return false;
					current = rect.min;
					return true;
				}
				// 顺时针)
				if (current.x == rect.xMin && current.y < rect.yMax - 1)
				{
					current = new(current.x, current.y + 1);
					return true;
				}
				if (current.y == rect.yMax - 1 && current.x < rect.xMax - 1)
				{
					current = new(current.x + 1, current.y);
					return true;
				}
				if (current.x == rect.xMax - 1 && current.y > rect.yMin)
				{
					current = new(current.x, current.y - 1);
					return true;
				}
				if (current.y == rect.yMin && current.x > rect.xMin + 1)
				{
					current = new(current.x - 1, current.y);
					return true;
				}
				return false;
			}

			public void Reset()
			{
				started = false;
			}

			public void Dispose()
			{
			}

			public IEnumerator<Vector2Int> GetEnumerator() => this;

			IEnumerator IEnumerable.GetEnumerator() => this;
		}
	}
}