using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit.Pathfinding.Rect
{
	public class RectPathfindingSpace : PathfindingSpace<int>
	{
		public readonly bool allowDiagonal;
		internal readonly GridIndexCalculator gridIndexCalculator;
		readonly float[] costMap;
		readonly int directionBits;
		public RectPathfindingSpace(RectInt area, bool allowDiagonal) : base(allowDiagonal ? 8 : 4)
		{
			gridIndexCalculator = new(area);
			this.allowDiagonal = allowDiagonal;
			directionBits = this.allowDiagonal ? 3 : 2;
			costMap = new float[gridIndexCalculator.count << directionBits];
		}
		protected internal override int GetLinks(int fromNode, int[] toNodes, float[] costs)
		{
			var count = 0;
			for (var i = 0; i < maxLinkCountPerNode; ++i)
			{
				var linkIndex = (fromNode << directionBits) + i;
				var cost = costMap[linkIndex];
				if (cost <= 0) continue;
				toNodes[count] = linkIndex;
				costs[count] = cost;
				++count;
			}
			return count;
		}
		/// <param name="fromNode"></param>
		/// <param name="direction"></param>
		/// <returns>result &lt;= 0 means no link</returns>
		public float GetStepCost(int fromNode, GridDirections direction)
		{
			var directionIndex = allowDiagonal ? (int)direction : (int)direction / 2;
			var linkIndex = (fromNode << directionBits) + directionIndex;
			return costMap[linkIndex];
		}
		/// <param name="fromNode"></param>
		/// <param name="direction"></param>
		/// <param name="cost">cost &lt;= 0 means no link</param>
		public void SetStepCost(int fromNode, GridDirections direction, float cost)
		{
			var directionIndex = allowDiagonal ? (int)direction : (int)direction / 2;
			var linkIndex = (fromNode << directionBits) + directionIndex;
			if (costMap[linkIndex] != cost)
			{
				costMap[linkIndex] = cost;
				MarkDirty();
			}
		}
	}
}
