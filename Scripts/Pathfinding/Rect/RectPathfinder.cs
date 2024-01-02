using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.Collections.Wrappers;
using EthansGameKit.MathUtilities;
using UnityEngine;

namespace EthansGameKit.Pathfinding.Rect
{
	/// <summary>
	///     <para>针对四边形网格深度优化的寻路器</para>
	///     <list type="bullet">
	///         <item>支持四邻或八邻网格</item>
	///         <item>支持自定义移动消耗</item>
	///         <item>支持多起点</item>
	///     </list>
	/// </summary>
	public sealed class RectPathfinder : Pathfinder<int>, IDisposable
	{
		public new readonly RectPathfindingSpace space;
		readonly GridIndexCalculator calculator;
		readonly int[] flowMap;
		readonly float[] totalCostMap;
		readonly float[] heuristicMap;
		readonly float[] stepCostMap;
		readonly Stack<int> pathBuffer = new();
		readonly List<int> sourceBuffer = new();
		IRectPathfindingParams @params;
		int currentNode;
		public Vector2Int Current => calculator.GetPosition(currentNode);
		public IReadOnlyDictionary<Vector2Int, Vector2Int> FromMap
		{
			get
			{
				var flowDict = flowMap.WrapAsDict(index => flowMap[index] >= 0);
				return flowDict.WrapAsDict(
					oldKey2NewKey: IConverter.FromFunc<int, Vector2Int>(index => calculator.GetPosition(index)),
					oldValue2NewValue: IConverter.FromFunc<int, Vector2Int>(index => calculator.GetPosition(index)),
					newKey2OldKey: null,
					newValue2OldValue: null
				);
			}
		}
		public IReadOnlyDictionary<Vector2Int, float> TotalCostMap
		{
			get
			{
				var totalCostDict = totalCostMap.WrapAsDict(index => totalCostMap[index] > 0);
				return totalCostDict.WrapAsDict(
					oldKey2NewKey: IConverter.FromFunc<int, Vector2Int>(index => calculator.GetPosition(index)),
					oldValue2NewValue: IConverter.Default<float>(),
					newKey2OldKey: null,
					newValue2OldValue: null
				);
			}
		}
		internal RectPathfinder(RectPathfindingSpace space) : base(space)
		{
			this.space = space;
			calculator = space.gridIndexCalculator;
			flowMap = new int[calculator.count];
			totalCostMap = new float[calculator.count];
			heuristicMap = new float[calculator.count];
			stepCostMap = new float[calculator.count << (space.allowDiagonal ? 3 : 2)];
		}
		protected override void Clear()
		{
			flowMap.MemSet(-1);
			totalCostMap.MemSet(0);
			heuristicMap.MemSet(-1);
			stepCostMap.MemSet(0);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool TryGetTotalCostUnverified(int node, out float cost)
		{
			cost = totalCostMap[node];
			return cost > 0;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SetTotalCostUnverified(int node, float cost)
		{
			totalCostMap[node] = cost;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool TryGetParentNodeUnverified(int node, out int parent)
		{
			parent = flowMap[node];
			return parent >= 0;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SetParentNodeUnverified(int node, int parent)
		{
			flowMap[node] = parent;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float GetHeuristicUnverified(int node)
		{
			var heuristic = heuristicMap[node];
			if (heuristic < 0)
			{
				var position = calculator.GetPositionUnverified(node);
				try
				{
					heuristic = @params.CalculateHeuristic(position);
				}
				catch (Exception e)
				{
					heuristic = float.PositiveInfinity;
					Debug.LogException(e);
				}
				heuristicMap[node] = heuristic;
			}
			return heuristic;
		}
		protected override float GetStepCostUnverified(int from, int to, byte costType)
		{
			GridDirections direction;
			var sub = to - from;
			var width = calculator.width;
			switch (sub)
			{
				case 1:
					direction = GridDirections.Right;
					break;
				case -1:
					direction = GridDirections.Left;
					break;
				default:
				{
					if (sub == width) direction = GridDirections.Forward;
					else if (sub == -width) direction = GridDirections.Backward;
					else if (sub == width + 1) direction = GridDirections.ForwardRight;
					else if (sub == width - 1) direction = GridDirections.ForwardLeft;
					else if (sub == -width - 1) direction = GridDirections.BackwardLeft;
					else if (sub == -width + 1) direction = GridDirections.BackwardRight;
					else throw new ArgumentException();
					break;
				}
			}
			var linkIndex = space.GetLinkIndex(from, direction);
			var stepCost = stepCostMap[linkIndex];
			if (stepCost <= 0)
			{
				var fromPosition = calculator.GetPositionUnverified(from);
				stepCost = @params.CalculateStepCost(fromPosition, direction, costType);
				if (stepCost <= 0)
				{
					Debug.LogError($"Step cost must be positive. {fromPosition}, dir:{direction.ToChar()}, costType: {costType}");
					stepCost = float.PositiveInfinity;
				}
				stepCostMap[linkIndex] = stepCost;
			}
			return stepCost;
		}
		public void Dispose()
		{
			Reset();
			space.Recycle(this);
		}
		public bool MoveNext()
		{
			return base.MoveNext(out currentNode);
		}
		public void Reset()
		{
			sourceBuffer.Clear();
			base.Reset(sourceBuffer, default, default);
		}
		public void Reset(IRectPathfindingParams @params)
		{
			sourceBuffer.Clear();
			this.@params = @params;
			foreach (var source in @params.Sources)
				sourceBuffer.Add(calculator.GetIndex(source));
			var overrideLinks = @params.OverrideLinks;
			if (overrideLinks != null)
			{
				foreach (var (pos, dir, costType) in overrideLinks)
				{
					var index = calculator.GetIndex(pos);
					stepCostMap[space.GetLinkIndex(index, dir)] = costType;
				}
			}
			base.Reset(sourceBuffer, @params.MaxCost, @params.MaxHeuristic);
			sourceBuffer.Clear();
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
