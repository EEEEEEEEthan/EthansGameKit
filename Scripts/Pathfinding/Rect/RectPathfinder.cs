using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit.Pathfinding.Rect
{
	public class RectPathfinder : Pathfinder<int>
	{
		public new readonly RectPathfindingSpace space;
		readonly GridIndexCalculator calculator;
		readonly int[] flowMap;
		readonly float[] costMap;
		readonly float[] heuristicMap;
		readonly Stack<int> pathBuffer = new();
		public RectPathfinder(RectPathfindingSpace space) : base(space)
		{
			this.space = space;
			calculator = space.gridIndexCalculator;
			flowMap = new int[calculator.count];
			costMap = new float[calculator.count];
			heuristicMap = new float[calculator.count];
		}
		protected override void Clear()
		{
			flowMap.MemSet(-1);
			costMap.MemSet(0);
			heuristicMap.MemSet(-1);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected sealed override bool TryGetTotalCostUnverified(int node, out float cost)
		{
			cost = costMap[node];
			return cost > 0;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected sealed override void SetTotalCostUnverified(int node, float cost)
		{
			costMap[node] = cost;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected sealed override bool TryGetParentNodeUnverified(int node, out int parent)
		{
			parent = flowMap[node];
			return parent >= 0;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected sealed override void SetParentNodeUnverified(int node, int parent)
		{
			flowMap[node] = parent;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float GetHeuristicUnverified(int node)
		{
			var heuristic = heuristicMap[node];
			if (heuristic < 0)
			{
				var position = calculator.GetPosition(node);
				heuristic = heuristicMap[node] = Vector2.Distance(position, space.gridIndexCalculator.GetPosition(node));
			}
			return heuristic;
		}
		public bool Reached(Vector2Int position)
		{
			return flowMap[calculator.GetIndex(position)] >= 0;
		}
		/// <summary>
		///     获取从起点(含)到终点(含)的路径
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool TryGetPath(Vector2Int destination, List<Vector2Int> path)
		{
			assert.IsTrue(pathBuffer.Count <= 0);
			path.Clear();
			var destinationIndex = calculator.GetIndex(destination);
			if (base.TryGetPath(destinationIndex, pathBuffer))
			{
				while (pathBuffer.TryPop(out var waypointIndex))
					path.Add(calculator.GetPosition(waypointIndex));
				return true;
			}
			assert.IsTrue(path.Count <= 0);
			return false;
		}
		/// <summary>
		///     获取从起点(含)到终点(含)的路径.起点先出队
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public bool TryGetPath(Vector2Int destination, Queue<Vector2Int> path)
		{
			assert.IsTrue(pathBuffer.Count <= 0);
			path.Clear();
			var destinationIndex = calculator.GetIndex(destination);
			if (base.TryGetPath(destinationIndex, pathBuffer))
			{
				while (pathBuffer.TryPop(out var waypointIndex))
					path.Enqueue(calculator.GetPosition(waypointIndex));
				return true;
			}
			assert.IsTrue(path.Count <= 0);
			return false;
		}
	}
}
