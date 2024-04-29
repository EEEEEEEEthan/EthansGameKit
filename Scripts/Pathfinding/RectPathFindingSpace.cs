using EthansGameKit.RectGrid;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	public class RectPathFindingSpace : PathFindingSpace<int>
	{
		public readonly GridIndexCalculator calculator;
		internal readonly int[] neighborOffsets;
		readonly byte[] costTypes;

		public RectPathFindingSpace(RectInt rect)
		{
			calculator = new(rect);
			costTypes = new byte[calculator.count << 2];
			var neighborOffsets = new int[8];
			// 四邻
			neighborOffsets[(int)OctDirectionCode.North] = calculator.width;
			neighborOffsets[(int)OctDirectionCode.East] = 1;
			neighborOffsets[(int)OctDirectionCode.South] = -calculator.width;
			neighborOffsets[(int)OctDirectionCode.West] = -1;
			// 八邻
			neighborOffsets[(int)OctDirectionCode.NorthEast] = calculator.width + 1;
			neighborOffsets[(int)OctDirectionCode.SouthEast] = -calculator.width + 1;
			neighborOffsets[(int)OctDirectionCode.SouthWest] = -calculator.width - 1;
			neighborOffsets[(int)OctDirectionCode.NorthWest] = calculator.width - 1;
			this.neighborOffsets = neighborOffsets;
		}

		protected internal override int GetLinks(int index, ref int[] neighbors, ref byte[] costTypes)
		{
			for (var i = 0; i < 4; ++i)
			{
				neighbors[i] = index + neighborOffsets[i];
				costTypes[i] = this.costTypes[(index << 2) | i];
			}
			return 4;
		}

		public void SetLink(Vector2Int position, QuadDirectionCode direction, byte costType)
		{
			if (direction is < 0 or > QuadDirectionCode.West)
				throw new System.ArgumentOutOfRangeException($"direction {direction} is out of range");
			if (!calculator.Contains(position))
				throw new System.ArgumentOutOfRangeException($"position {position} is out of range");
			var neighbor = position + direction.ToVector2Int();
			if (!calculator.Contains(neighbor))
				if (costType != 0)
				{
					Debug.LogError(
						$"SetLink({position}, {direction}, {costType}) failed because neighbor {neighbor} is out of range");
					costType = 0;
				}
			var index = calculator.GetIndex(position);
			costTypes[(index << 2) | (int)direction] = costType;
		}
	}
}