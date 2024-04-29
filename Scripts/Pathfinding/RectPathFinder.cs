using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.Collections;
using EthansGameKit.RectGrid;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	public class RectPathFinder
	{
		const int noParent = -1;
		const float uncached = -1;
		/// <summary>
		///     (启发值+消耗)超过这个值认为不可达
		/// </summary>
		const float maxWeight = 1000000;
		readonly RectPathFindingSpace space;
		readonly float[] cachedStepCost;
		readonly float[] cachedHeuristic;
		readonly float[] totalCostMap;
		readonly int[] parentMap;
		readonly GridIndexCalculator calculator;
		readonly Heap<int, float> open;
		readonly int directionBits;
		readonly bool allowDiagonal;

		int[] neighborBuffer = new int[4];
		byte[] neighborCostTypeBuffer = new byte[4];
		IPathfindingArguments<Vector2Int> arguments;

		public RectPathFinder(RectPathFindingSpace space, bool allowDiagonal)
		{
			calculator = space.calculator;
			this.space = space;
			this.allowDiagonal = allowDiagonal;
			directionBits = allowDiagonal ? 3 : 2;
			cachedStepCost = new float[calculator.count << directionBits];
			cachedHeuristic = new float[calculator.count];
			totalCostMap = new float[calculator.count];
			parentMap = new int[calculator.count];
			open = new();
		}

		public Vector2Int Current => calculator.GetPosition(open.Peek());

		public void Reinitialize(Vector2Int source, IPathfindingArguments<Vector2Int> arguments)
		{
			Reinitialize(new[] { source }, arguments);
		}

		public void Reinitialize(IEnumerable<Vector2Int> sources, IPathfindingArguments<Vector2Int> arguments)
		{
			this.arguments = arguments;
			cachedStepCost.MemSet(uncached);
			cachedHeuristic.MemSet(uncached);
			totalCostMap.MemSet(maxWeight);
			parentMap.MemSet(noParent);
			open.Clear();
			var count = 0;
			foreach (var source in sources)
			{
				if (!calculator.Contains(source))
				{
					Debug.LogError($"unexpected source {source}");
					continue;
				}
				var sourceIndex = calculator.GetIndex(source);
				totalCostMap[sourceIndex] = 0;
				cachedHeuristic[sourceIndex] = arguments.CalculateHeuristic(source);
				parentMap[sourceIndex] = sourceIndex;
				open.Add(sourceIndex, cachedHeuristic[sourceIndex]);
				++count;
			}
			if (count <= 0) throw new ArgumentException("no valid sources");
		}

		public bool NextStep(out int step)
		{
			if (open.Count <= 0)
			{
				step = default;
				return false;
			}
			step = open.Pop();
			var count = space.GetLinks(step, ref neighborBuffer, ref neighborCostTypeBuffer);
			var currentTotalCost = totalCostMap[step];
			for (var i = 0; i < count; ++i)
			{
				var costType = neighborCostTypeBuffer[i];
				ConsiderNextStep(step, (OctDirectionCode)i, currentTotalCost, costType);
			}
			if (allowDiagonal)
				for (var direction = OctDirectionCode.NorthEast; direction < OctDirectionCode.NorthWest; ++direction)
					ConsiderNextStep(step, direction, currentTotalCost, 0);
			return true;
		}

		public bool NextStep(out Vector2Int step)
		{
			if (NextStep(out int index))
			{
				step = calculator.GetPosition(index);
				return true;
			}
			step = default;
			return false;
		}

		/// <summary>
		///     获得一条从sources到target的路径
		/// </summary>
		/// <param name="targetIndex"></param>
		/// <param name="path">包含起点和终点的路径。终点先入栈。若起点与终点相同，栈长度为1。若无路径，栈长度为0</param>
		public void GetPath(int targetIndex, Stack<int> path)
		{
			if (!calculator.Contains(targetIndex))
				throw new ArgumentOutOfRangeException($"targetIndex {targetIndex} is out of range");
			path.Clear();
			if (parentMap[targetIndex] == noParent)
				return;
			var index = targetIndex;
			while (true)
			{
				path.Push(index);
				if (totalCostMap[index] <= 0) // 起点
					break;
				index = parentMap[index];
			}
			path.Clear();
		}

		/// <summary>
		///     获得一条从sources到target的路径
		/// </summary>
		/// <param name="target"></param>
		/// <param name="path">包含起点和终点的路径。终点先入栈。若起点与终点相同，栈长度为1。若无路径，栈长度为0</param>
		public bool TryGetPath(Vector2Int target, Stack<Vector2Int> path)
		{
			GetPath(target, path);
			return path.Count > 0;
		}

		/// <summary>
		///     获得一条从sources到target的路径
		/// </summary>
		/// <param name="target"></param>
		/// <param name="path">包含起点和终点的路径。终点先入栈。若起点与终点相同，栈长度为1。若无路径，栈长度为0</param>
		public void GetPath(Vector2Int target, Stack<Vector2Int> path)
		{
			if (!calculator.Contains(target)) return;
			var targetIndex = calculator.GetIndex(target);
			path.Clear();
			if (parentMap[targetIndex] == noParent) return;
			var index = targetIndex;
			while (true)
			{
				path.Push(calculator.GetPositionUnverified(index));
				if (totalCostMap[index] <= 0) // 起点
					break;
				index = parentMap[index];
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="fromNode"></param>
		/// <param name="direction"></param>
		/// <param name="costType"></param>
		/// <returns>最大消耗值<see cref="maxWeight" /></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		float GetStepCostUnverified(int fromNode, OctDirectionCode direction, byte costType)
		{
			var index = (fromNode << directionBits) | (int)direction;
			var cachedCost = cachedStepCost[index];
			if (cachedCost == uncached)
			{
				var fromPosition = calculator.GetPosition(fromNode);
				var toPosition = fromPosition + direction.ToVector2Int();
				if (calculator.Contains(toPosition))
					cachedCost = cachedStepCost[index] =
						arguments.CalculateStepCost(fromPosition, toPosition, costType);
				else
					cachedCost = cachedStepCost[index] = maxWeight;
				cachedCost.Clamp(0, maxWeight);
			}
			return cachedCost;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		float GetHeuristicUnverified(int node)
		{
			var cachedHeuristic = this.cachedHeuristic[node];
			if (cachedHeuristic == uncached)
			{
				cachedHeuristic = this.cachedHeuristic[node] =
					arguments.CalculateHeuristic(calculator.GetPosition(node));
				cachedHeuristic.Clamp(0, maxWeight);
			}
			return cachedHeuristic;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ConsiderNextStep(int currentIndex, OctDirectionCode direction, float currentTotalCost, byte costType)
		{
			var neighborIndex = space.neighborOffsets[(int)direction] + currentIndex;
			var stepCost = GetStepCostUnverified(currentIndex, direction, costType);
			if (stepCost >= maxWeight) return;
			var oldNeighborTotalCost = totalCostMap[neighborIndex];
			var newNeighborTotalCost = currentTotalCost + stepCost;
			if (newNeighborTotalCost < oldNeighborTotalCost)
			{
				var heuristic = GetHeuristicUnverified(neighborIndex);
				var newHeapValue = newNeighborTotalCost + heuristic;
				if (newHeapValue >= maxWeight) return;
				var oldHeapValue = oldNeighborTotalCost + heuristic;
				open.AddOrUpdate(neighborIndex, oldHeapValue, newHeapValue);
				totalCostMap[neighborIndex] = newNeighborTotalCost;
				parentMap[neighborIndex] = currentIndex;
			}
		}
	}
}