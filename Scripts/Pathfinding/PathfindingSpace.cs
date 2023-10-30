using System;
using System.Collections.Generic;
using EthansGameKit.Collections;

namespace EthansGameKit.Pathfinding
{
	public abstract class PathfindingSpace<TNode>
	{
		public abstract class Pathfinder
		{
			readonly Heap<TNode, float> openList = Heap<TNode, float>.Generate();
			readonly PathfindingSpace<TNode> space;
			float maxCost = float.PositiveInfinity;
			float maxHeuristic = float.PositiveInfinity;
			TNode[] nodeBuffer = Array.Empty<TNode>();
			float[] floatBuffer = Array.Empty<float>();

			protected Pathfinder(PathfindingSpace<TNode> space)
			{
				this.space = space;
			}
			public abstract IReadOnlyDictionary<TNode, TNode> FromMap { get; }
			public abstract IReadOnlyDictionary<TNode, float> CostMap { get; }
			protected abstract IDictionary<TNode, float> WritableCostMap { get; }
			protected abstract IDictionary<TNode, TNode> WritableFromMap { get; }
			protected abstract float GetHeuristic(TNode node);
			protected abstract float GetStepCost(TNode fromNode, TNode toNode, float basicCost);

			public bool MoveNext(out TNode current)
			{
				if (openList.Count <= 0)
				{
					current = default;
					return false;
				}
				current = openList.Pop();
				var costMap = WritableCostMap;
				var fromMap = WritableFromMap;
				costMap.TryGetValue(current, out var currentCost);
				var linkCount = space.GetLinks(current, ref nodeBuffer, ref floatBuffer);
				var toNodes = nodeBuffer;
				var basicCosts = floatBuffer;
				for (var i = linkCount; i-- > 0;)
				{
					var toNode = toNodes[i];
					var basicCost = basicCosts[i];
					var stepCost = GetStepCost(current, toNode, basicCost);
					if (stepCost is float.PositiveInfinity or <= 0) continue;
					var newCost = currentCost + stepCost;
					if (newCost > maxCost) continue;
					var heuristic = GetHeuristic(toNode);
					if (heuristic > maxHeuristic) continue;
					if (!costMap.TryGetValue(toNode, out var oldCost) || newCost < oldCost)
					{
						costMap[toNode] = newCost;
						fromMap[toNode] = current;
						openList.AddOrUpdate(toNode, newCost + heuristic);
					}
				}
				return true;
			}

			protected void Reinitialize(
				float maxCost,
				float maxHeuristic,
				IEnumerable<TNode> sources
			)
			{
				this.maxCost = maxCost;
				this.maxHeuristic = maxHeuristic;
				openList.Clear();
				WritableCostMap.Clear();
				WritableFromMap.Clear();
				foreach (var node in sources)
				{
					WritableCostMap[node] = 0;
					WritableFromMap[node] = node;
					openList.AddOrUpdate(node, GetHeuristic(node));
				}
			}
		}

		public abstract void ClearLinks();
		public abstract void SetLink(TNode fromNode, TNode toNode, float basicCost);
		public abstract bool RemoveLink(TNode fromNode, TNode toNode);
		public abstract int GetLinks(TNode node, ref TNode[] toNodes, ref float[] basicCosts);
		public abstract int GetLinkSources(ref TNode[] result);
	}
}
