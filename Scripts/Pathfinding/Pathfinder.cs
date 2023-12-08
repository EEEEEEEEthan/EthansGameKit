using System.Collections.Generic;
using EthansGameKit.Collections;

namespace EthansGameKit.Pathfinding
{
	public abstract class Pathfinder<T>
	{
		public readonly PathfindingSpace<T> space;
		readonly Heap<T, float> heap;
		readonly T[] tBuffer;
		readonly float[] singleBuffer;
		float maxCost;
		float maxHeuristic;
		protected Pathfinder(PathfindingSpace<T> space)
		{
			this.space = space;
			heap = Heap<T, float>.Generate();
			tBuffer = new T[space.maxLinkCountPerNode];
			singleBuffer = new float[space.maxLinkCountPerNode];
		}
		protected void Reset(IEnumerable<T> sources, float maxCost, float maxHeuristic)
		{
			this.maxCost = maxCost;
			this.maxHeuristic = maxHeuristic;
			heap.Clear();
			Clear();
			foreach (var startNode in sources)
			{
				SetTotalCost(startNode, 0);
				heap.AddOrUpdate(startNode, GetHeuristic(startNode));
			}
		}
		protected abstract void Clear();
		protected bool Next(out T currentNode)
		{
			if (heap.Count <= 0)
			{
				currentNode = default;
				return false;
			}
			currentNode = heap.Pop();
			TryGetTotalCost(currentNode, out var currentCost);
			var count = space.GetLinks(currentNode, tBuffer, singleBuffer);
			for (var i = 0; i < count; ++i)
			{
				var toNode = tBuffer[i];
				var stepCost = singleBuffer[i];
				var newCost = currentCost + stepCost;
				if (newCost >= maxCost) continue;
				if (TryGetTotalCost(toNode, out var oldCost) && newCost < oldCost)
				{
					var heuristic = GetHeuristic(toNode);
					if (heuristic >= maxHeuristic) continue;
					SetTotalCost(toNode, newCost);
					heap.AddOrUpdate(toNode, newCost + heuristic);
				}
			}
			return true;
		}
		protected abstract bool TryGetTotalCost(T node, out float cost);
		protected abstract void SetTotalCost(T node, float cost);
		protected abstract bool TryGetParentNode(T node, out T parent);
		protected abstract void SetParentNode(T node, T parent);
		protected abstract float GetHeuristic(T node);
	}
}
