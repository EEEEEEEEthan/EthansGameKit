using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.Collections;

namespace EthansGameKit.Pathfinding
{
	public abstract class Pathfinder<T>
	{
		public readonly PathfindingSpace<T> space;
		readonly Heap<T, float> heap;
		readonly T[] tBuffer;
		readonly byte[] byteBuffer;
		float maxCost;
		float maxHeuristic;
		int changeFlag;
		public bool Expired => changeFlag != space.ChangeFlag;
		protected Pathfinder(PathfindingSpace<T> space)
		{
			this.space = space;
			heap = Heap<T, float>.Generate();
			tBuffer = new T[space.maxLinkCountPerNode];
			byteBuffer = new byte[space.maxLinkCountPerNode];
		}
		protected void Reset(IEnumerable<T> sources, float maxCost, float maxHeuristic)
		{
			changeFlag = space.ChangeFlag;
			this.maxCost = maxCost;
			this.maxHeuristic = maxHeuristic;
			heap.Clear();
			Clear();
			foreach (var startNode in sources)
			{
				SetTotalCostUnverified(startNode, 0);
				heap.AddOrUpdate(startNode, GetHeuristicUnverified(startNode));
			}
		}
		protected abstract void Clear();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected bool MoveNext(out T currentNode)
		{
			if (heap.Count <= 0)
			{
				currentNode = default;
				return false;
			}
			currentNode = heap.Pop();
			TryGetTotalCostUnverified(currentNode, out var currentCost);
			var count = space.GetLinks(currentNode, tBuffer, byteBuffer);
			for (var i = 0; i < count; ++i)
			{
				var toNode = tBuffer[i];
				var stepCost = GetStepCostUnverified(currentNode, toNode, byteBuffer[i]);
				if (stepCost <= 0) continue;
				var newCost = currentCost + stepCost;
				if (newCost >= maxCost) continue;
				if (!TryGetTotalCostUnverified(toNode, out var oldCost) || newCost < oldCost)
				{
					var heuristic = GetHeuristicUnverified(toNode);
					if (heuristic >= maxHeuristic) continue;
					SetTotalCostUnverified(toNode, newCost);
					SetParentNodeUnverified(toNode, currentNode);
					heap.AddOrUpdate(toNode, newCost + heuristic);
				}
			}
			return true;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract bool TryGetTotalCostUnverified(T node, out float cost);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void SetTotalCostUnverified(T node, float cost);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract bool TryGetParentNodeUnverified(T node, out T parent);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void SetParentNodeUnverified(T node, T parent);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract float GetHeuristicUnverified(T node);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract float GetStepCostUnverified(T from, T to, byte costType);
		/// <summary>
		///     获取从起点(含)到终点(含)的路径，起点先出栈
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		protected bool TryGetPath(T destination, Stack<T> result)
		{
			result.Push(destination);
			while (TryGetParentNodeUnverified(destination, out var parent))
			{
				result.Push(parent);
				if (destination.Equals(parent)) return true;
			}
			return false;
		}
	}
}
