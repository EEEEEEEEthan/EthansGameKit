using System;
using EthansGameKit.Collections;

namespace EthansGameKit.Pathfinding
{
	[Serializable]
	public abstract class PathfindingSpace
	{
		public abstract class Pathfinder
		{
			readonly Heap<int, float> openList = Heap<int, float>.Generate();
			readonly PathfindingSpace pathfindingSpace;
			readonly StepInfo[] stepBuffer;
			protected Pathfinder(PathfindingSpace pathfindingSpace, int maxLinkPerNode)
			{
				this.pathfindingSpace = pathfindingSpace;
				stepBuffer = new StepInfo[maxLinkPerNode];
			}
			public bool Next()
			{
				if (openList.Count <= 0) return false;
				var nodeId = openList.Pop();
				TryGetFromInfo(nodeId, out var fromNodeId, out var costBeforeStepHere);
				var count = pathfindingSpace.GetLinks(nodeId, stepBuffer);
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

		protected abstract int GetLinks(int fromNodeId, StepInfo[] toAndCost);
	}
}
