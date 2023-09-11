using System;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	class PathfindingSpace_QuadTileBased : PathfindingSpace
	{
		class ImpPathfinder : Pathfinder
		{
			readonly PathfindingSpace_QuadTileBased pathfindingSpace;
			readonly int[] fromMap;
			readonly float[] costMap;
			public ImpPathfinder(PathfindingSpace_QuadTileBased pathfindingSpace) : base(pathfindingSpace)
			{
				this.pathfindingSpace = pathfindingSpace;
				fromMap = new int[pathfindingSpace.count];
				costMap = new float[pathfindingSpace.count];
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

		readonly int width;
		readonly int widthPower;
		readonly int count;
		readonly int zMask;
		public PathfindingSpace_QuadTileBased(int widthPower)
		{
			this.widthPower = widthPower;
			width = 1 << widthPower;
			count = width << widthPower;
			zMask = width - 1;
		}
		protected override int GetLinks(int fromNodeId, ref StepInfo[] toAndCost)
		{
			throw new NotImplementedException();
		}
		public bool Contains(int x, int z)
		{
			return (uint)x < (uint)width && (uint)z < (uint)width;
		}
		public bool Contains(int index)
		{
			return (uint)index < (uint)count;
		}
		public int GetNodeIndex(int x, int z)
		{
			return (x << widthPower) | z;
		}
		public void GetNodePosition(int index, out int x, out int z)
		{
			x = index >> widthPower;
			z = index & zMask;
		}
		public void AddLink(int fromNode, int direction, float cost)
		{
		}
	}
}
