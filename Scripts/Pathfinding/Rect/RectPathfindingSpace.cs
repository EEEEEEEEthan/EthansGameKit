using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit.Pathfinding.Rect
{
	public class RectPathfindingSpace : PathfindingSpace<int>
	{
		internal readonly GridIndexCalculator gridIndexCalculator;
		readonly long[] costMap;
		public readonly bool allowDiagonal;
		readonly GridDirections[] directionSequence =
		{
			GridDirections.Forward,
			GridDirections.Right,
			GridDirections.Backward,
			GridDirections.Left,
			GridDirections.ForwardRight,
			GridDirections.BackwardRight,
			GridDirections.BackwardLeft,
			GridDirections.ForwardLeft,
		};
		public RectPathfindingSpace(RectInt area, bool allowDiagonal) : base(allowDiagonal ? 8 : 4)
		{
			gridIndexCalculator = new(area);
			this.allowDiagonal = allowDiagonal;
			costMap = new long[gridIndexCalculator.count];
		}
		protected internal override int GetLinks(int fromNode, int[] toNodes, byte[] costTypes)
		{
			var count = 0;
			for (var i = 0; i < maxLinkCountPerNode; ++i)
			{
				var direction = directionSequence[i];
				var costType = GetLinkType(fromNode, direction);
				if (costType <= 0) continue;
				toNodes[count] = gridIndexCalculator.GetNeighborIndexUnverified(fromNode, direction);
				costTypes[count] = costType;
				++count;
			}
			return count;
		}
		/// <param name="from"></param>
		/// <param name="direction"></param>
		/// <param name="costType">cost &lt;= 0 means no link</param>
		public void SetLink(Vector2Int from, GridDirections direction, byte costType)
		{
			var index = gridIndexCalculator.GetIndexUnverified(from);
			SetLink(index, direction, costType);
		}
		void SetLink(int index, GridDirections direction, byte costType)
		{
			var cost = costMap[index];
			cost.SetBits((int)direction * 8, 8, costType);
			costMap[index] = cost;
		}
		byte GetLinkType(int index, GridDirections direction)
		{
			return (byte)costMap[index].GetBits((int)direction * 8, 8);
		}
	}
}
