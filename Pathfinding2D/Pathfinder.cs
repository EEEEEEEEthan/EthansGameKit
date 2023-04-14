using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using EthansGameKit.Collections;
using UnityEngine;

namespace EthansGameKit.Pathfinding2D
{
	public abstract class Pathfinder
	{
		float[] costMap;
		float[] heuristicMap;
		int[] fromIndexMap;
		int[] neighborSequence;
		int directionCount;
		float maxCost;
		float maxHeuristic;
		readonly HeapInt32Single openList = new();
		int width;
		int widthPower;
		int allTileCount;
		BitArray visited;
		public PathfindingSpace Space { get; private set; }
		protected abstract float Heuristic(int x, int z);
		protected virtual float GetOverrideCost(float basicCost, int toIndex, int fromDirection)
		{
			return basicCost;
		}
		protected void Release()
		{
			Space.RecycleFloatArray(ref costMap);
			Space.RecycleIntArray(ref fromIndexMap);
			Space.RecycleFloatArray(ref heuristicMap);
			Space.RecycleBitArray(ref visited);
			Space = null;
		}
		protected Stack<int> GetPath(int index)
		{
			if (!visited.Get(index)) return null;
			var stack = StackPool<int>.Generate();
			stack.Push(index);
			while (true)
			{
				var fromIndex = fromIndexMap[index];
				if (fromIndex == index) break;
				index = fromIndex;
				stack.Push(index);
			}
			return stack;
		}
		protected bool Reached(int x, int z)
		{
			return visited.Get(GetIndex(x, z));
		}
		/// <param name="tileIndex"></param>
		/// <param name="path">若路径不存在,返回null.栈顶是起点，栈底是终点。若起点与终点相同，栈将只有一个元素</param>
		protected bool TryGetPath(int tileIndex, out Stack<int> path)
		{
			path = GetPath(tileIndex);
			return path != null;
		}
		protected void Initialize(PathfindingSpace pathfindingSpace, NeighborTypeEnum neighborType)
		{
			widthPower = pathfindingSpace.widthPower;
			costMap = pathfindingSpace.GenerateFloatArray();
			fromIndexMap = pathfindingSpace.GenerateIntArray();
			heuristicMap = pathfindingSpace.GenerateFloatArray();
			visited = pathfindingSpace.GenerateBitArray();
			Space = pathfindingSpace;
			width = pathfindingSpace.width;
			neighborSequence = (int[])Space.NeighborSequence;
			directionCount = neighborType switch
			{
				NeighborTypeEnum.Oct => 8,
				NeighborTypeEnum.Quad => 4,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}
		protected Dictionary<int, int> GetFromMap()
		{
			var map = DictionaryPool<int, int>.Generate();
			for (var i = 0; i < fromIndexMap.Length; i++)
				if (fromIndexMap[i] != 0)
					map.Add(i, fromIndexMap[i]);
			return map;
		}
		protected int GetIndex(int x, int z)
		{
			return (z << widthPower) | x;
		}
		protected void GetPosition(int index, out int x, out int z)
		{
			x = index & (width - 1);
			z = index >> widthPower;
		}
		protected bool Next(out int index)
		{
			if (openList.Count <= 0)
			{
				index = 0;
				return false;
			}
			index = openList.Pop();
			UpdateStep(index, costMap[index]);
			return true;
		}
		protected bool Reached(int index)
		{
			return visited.Get(index);
		}
		protected void ResetPathfinding(IReadOnlyList<int> startIndexes, float maxCost, float maxHeuristic)
		{
			this.maxCost = maxCost;
			this.maxHeuristic = maxHeuristic;
			for (var i = startIndexes.Count; i-- > 0;)
			{
				var index = startIndexes[i];
				openList.Add(index, 0);
				fromIndexMap[index] = index;
				costMap[index] = 0;
			}
		}
		void Clear()
		{
			costMap.MemSet(0, allTileCount, default);
			fromIndexMap.MemSet(0, allTileCount, default);
			heuristicMap.MemSet(0, allTileCount, default);
			openList.Clear();
			visited.SetAll(false);
			OnClear();
		} // ReSharper disable Unity.PerformanceAnalysis
		protected abstract void OnClear();
		// ReSharper disable Unity.PerformanceAnalysis
		float GetHeuristic(int tileIndex)
		{
			var heuristic = heuristicMap[tileIndex];
			if (heuristic > 0) return heuristic;
			GetPosition(tileIndex, out var x, out var z);
			try
			{
				heuristic = Heuristic(x, z);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				heuristic = float.MaxValue;
			}
			if (heuristic <= 0)
			{
				Debug.LogError($"invalid heuristic {heuristic} at {x},{z}");
				heuristic = float.MaxValue;
			}
			heuristicMap[tileIndex] = heuristic;
			return heuristic;
		}
		// ReSharper disable Unity.PerformanceAnalysis
		void UpdateStep(int currentIndex, double totalCost)
		{
			for (var i = 0; i < directionCount; ++i)
			{
				var neighborIndex = currentIndex + neighborSequence[i];
				var cost = Space.GetCost(neighborIndex, i);
				try
				{
					cost = GetOverrideCost(cost, neighborIndex, i);
					if (cost < 0)
						throw new ArgumentOutOfRangeException($"unexpected cost: {currentIndex}->{cost} {cost}");
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					cost = float.MaxValue;
				}
				var neighborCost = totalCost + cost;
				if (neighborCost < maxCost)
					UpdateStep(currentIndex, neighborIndex, neighborCost);
			}
			visited.Set(currentIndex, true);
		}
		void UpdateStep(int fromIndex, int toIndex, double cost)
		{
			var oldFromIndex = fromIndexMap[toIndex];
			if (oldFromIndex != 0)
			{
				var oldCost = costMap[toIndex];
				if (cost < costMap[toIndex])
				{
					var heuristic = heuristicMap[toIndex];
					var value = cost + heuristic;
					if (value < float.MaxValue)
					{
						fromIndexMap[toIndex] = fromIndex;
						costMap[toIndex] = (float)cost;
						openList.AddOrUpdate(toIndex, oldCost + heuristic, (float)value);
					}
				}
			}
			else
			{
				var heuristic = GetHeuristic(toIndex);
				if (heuristic < maxHeuristic)
				{
					var value = cost + heuristic;
					if (value < float.MaxValue)
					{
						fromIndexMap[toIndex] = fromIndex;
						costMap[toIndex] = (float)cost;
						openList.Add(toIndex, (float)value);
					}
				}
			}
		}
	}
}
