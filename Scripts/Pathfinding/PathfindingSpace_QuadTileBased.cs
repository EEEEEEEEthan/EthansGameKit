using System;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	[Serializable]
	sealed class PathfindingSpace_QuadTileBased : PathfindingSpace
	{
		sealed class ImpPathfinder : Pathfinder
		{
			readonly int[] fromMap;
			readonly float[] costMap;
			public ImpPathfinder(PathfindingSpace_QuadTileBased pathfindingSpace) : base(pathfindingSpace, 8)
			{
				fromMap = new int[pathfindingSpace.Count];
				costMap = new float[pathfindingSpace.Count];
				Reset();
			}
			protected override bool TryGetFromInfo(int nodeId, out int fromNodeId, out float totalCost)
			{
				fromNodeId = fromMap[nodeId];
				totalCost = costMap[nodeId];
				return fromNodeId != -1;
			}
			protected override void SetFromInfo(int fromNodeId, int toNodeId, float totalCost)
			{
				fromMap[toNodeId] = fromNodeId;
				costMap[toNodeId] = totalCost;
			}
			public void Reset()
			{
				fromMap.MemSet(-1);
				costMap.MemSet(float.PositiveInfinity);
			}
		}

		const float invalidCost = -1;
		static void Direction2XZ(int direction, out int x, out int z)
		{
			switch (direction)
			{
				case 0:
					x = 0;
					z = 1;
					break;
				case 1:
					x = 1;
					z = 0;
					break;
				case 2:
					x = 0;
					z = -1;
					break;
				case 3:
					x = -1;
					z = 0;
					break;
				case 4:
					x = 1;
					z = 1;
					break;
				case 5:
					x = 1;
					z = -1;
					break;
				case 6:
					x = -1;
					z = -1;
					break;
				case 7:
					x = -1;
					z = 1;
					break;
				default: throw new ArgumentOutOfRangeException(nameof(direction));
			}
		}
		[SerializeField] int width;
		[SerializeField] int widthPower;
		[SerializeField] int count;
		[SerializeField] int maxDirection;
		[SerializeField] int posMask;
		[SerializeField] float[] stepCosts;
		[SerializeField] int dataWidthPowerPerTile;
		public int Width => width;
		public int WidthPower => widthPower;
		public int Count => count;
		int PosMask => posMask;
		public PathfindingSpace_QuadTileBased(int widthPower, bool oct)
		{
			this.widthPower = widthPower;
			width = 1 << widthPower;
			count = Width << widthPower;
			posMask = Width - 1;
			maxDirection = oct ? 8 : 4;
			dataWidthPowerPerTile = oct ? 2 : 3;
			stepCosts = new float[count << dataWidthPowerPerTile];
			stepCosts.MemSet(invalidCost);
		}
		protected override int GetLinks(int fromNodeId, StepInfo[] toAndCost)
		{
			if (!Contains(fromNodeId)) throw new ArgumentOutOfRangeException(nameof(fromNodeId));
			var length = 0;
			for (var direction = 0; direction < maxDirection; ++direction)
			{
				var index = GetStepIndex(fromNodeId, direction);
				var cost = stepCosts[index];
				if (cost != invalidCost)
				{
					Direction2XZ(direction, out var toX, out var toZ);
					var toIndex = GetNodeIndex(toX, toZ);
					toAndCost[length].toNodeId = toIndex;
					toAndCost[length].stepCost = cost;
					++length;
				}
			}
			return length;
		}
		public bool Contains(int x, int z)
		{
			return (uint)x < (uint)width && (uint)z < (uint)width;
		}
		public bool Contains(int index)
		{
			return index > 0 && index < count;
		}
		public int GetNodeIndex(int x, int z)
		{
			return (x << widthPower) | z;
		}
		public void GetNodePosition(int index, out int x, out int z)
		{
			x = index >> widthPower;
			z = index & posMask;
		}
		/// <param name="fromNode"></param>
		/// <param name="direction">0-前, 1-右, 2-后, 3-左, 4-右前, 5-右后, 6-左后, 7-左前</param>
		/// <param name="cost"></param>
		public void SetLink(int fromNode, int direction, float cost)
		{
			var index = GetStepIndex(fromNode, direction);
			stepCosts[index] = cost;
		}
		/// <param name="fromNode"></param>
		/// <param name="direction">0-前, 1-右, 2-后, 3-左, 4-右前, 5-右后, 6-左后, 7-左前</param>
		public void RemoveLink(int fromNode, int direction)
		{
			var index = GetStepIndex(fromNode, direction);
			stepCosts[index] = invalidCost;
		}
		int GetStepIndex(int fromNode, int direction)
		{
			if (!Contains(fromNode)) throw new ArgumentOutOfRangeException(nameof(fromNode));
			if (direction >= maxDirection) throw new ArgumentOutOfRangeException(nameof(direction));
			GetNodePosition(fromNode, out var x, out var z);
			Direction2XZ(direction, out var dx, out var dz);
			var toX = x + dx;
			var toZ = z + dz;
			if (!Contains(toX, toZ)) throw new ArgumentOutOfRangeException(nameof(direction));
			return (fromNode << dataWidthPowerPerTile) + direction;
		}
	}
}
