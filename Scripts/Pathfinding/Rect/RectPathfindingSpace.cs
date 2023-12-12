using System.Runtime.CompilerServices;
using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit.Pathfinding.Rect
{
	public class RectPathfindingSpace : PathfindingSpace<int>
	{
		static readonly int[] directionIndex = { 0, 4, 1, 5, 2, 6, 3, 7 };
		static readonly GridDirections[] directionSequence =
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
		public readonly bool allowDiagonal;
		public readonly RectInt rect;
		internal readonly GridIndexCalculator gridIndexCalculator;
		readonly byte[] costMap;
		readonly int directionBits;
		readonly int[] neighborIndexBuffer;
		public RectPathfindingSpace(RectInt area, bool allowDiagonal) : base(allowDiagonal ? 8 : 4)
		{
			rect = area;
			gridIndexCalculator = new(area);
			this.allowDiagonal = allowDiagonal;
			directionBits = allowDiagonal ? 3 : 2;
			costMap = new byte[gridIndexCalculator.count << directionBits];
			neighborIndexBuffer = new int[maxLinkCountPerNode];
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
			var index = gridIndexCalculator.GetIndex(from);
			SetLink(index, direction, costType);
		}
		public int GetLinks(Vector2Int from, Vector2Int[] toNodes, byte[] costTypes)
		{
			var index = gridIndexCalculator.GetIndex(from);
			var count = GetLinks(index, neighborIndexBuffer, costTypes);
			for (var i = 0; i < count; ++i)
				toNodes[i] = gridIndexCalculator.GetPosition(neighborIndexBuffer[i]);
			return count;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int GetLinkIndex(int nodeIndex, GridDirections direction)
		{
			return (nodeIndex << directionBits) | directionIndex[(int)direction];
		}
		void SetLink(int index, GridDirections direction, byte costType)
		{
			costMap[GetLinkIndex(index, direction)] = costType;
		}
		byte GetLinkType(int index, GridDirections direction)
		{
			return costMap[GetLinkIndex(index, direction)];
		}
	}
}
