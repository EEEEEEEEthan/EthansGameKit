using System;
using EthansGameKit.Collections;

namespace EthansGameKit.Pathfinding
{
	public abstract class PathfindingSpace
	{
		public abstract class Pathfinder
		{
			readonly Heap<int, float> openList = Heap<int, float>.Generate();
			readonly PathfindingSpace pathfindingSpace;
			StepInfo[] stepBuffer = Array.Empty<StepInfo>();
			protected Pathfinder(PathfindingSpace pathfindingSpace)
			{
				this.pathfindingSpace = pathfindingSpace;
			}
			public bool Next()
			{
				if (openList.Count <= 0) return false;
				var nodeId = openList.Pop();
				TryGetFromInfo(nodeId, out var fromNodeId, out var costBeforeStepHere);
				var count = pathfindingSpace.GetLinks(nodeId, ref stepBuffer);
				for (var i = 0; i < count; ++i)
				{
					var stepInfo = stepBuffer[i];
					var nextNodeId = stepInfo.toNodeId;
					var expectedCostIfIStepHere = costBeforeStepHere + stepInfo.stepCost;
					if (!TryGetFromInfo(nextNodeId, out _, out var oldCostIfIStepHere) || expectedCostIfIStepHere < oldCostIfIStepHere)
					{
						SetFromInfo(nodeId, nextNodeId, expectedCostIfIStepHere);
						openList.AddOrUpdate(nextNodeId, expectedCostIfIStepHere);
					}
				}
				return true;
			}
			protected abstract bool TryGetFromInfo(int nodeId, out int fromNodeId, out float totalCost);
			protected abstract void SetFromInfo(int fromNodeId, int toNodeId, float totalCost);
		}

		public abstract Pathfinder GeneratePathfinder();
		public abstract void SetLink(int fromNodeId, int toNodeId, float cost);
		public abstract void RemoveLink(int fromNodeId, int toNodeId);
		protected abstract int GetLinks(int fromNodeId, ref StepInfo[] toAndCost);
	}
}
