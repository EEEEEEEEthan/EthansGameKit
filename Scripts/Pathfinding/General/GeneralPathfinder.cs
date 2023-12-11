using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Pathfinding.General
{
	public sealed class GeneralPathfinder : Pathfinder<Vector3>
	{
		public new readonly GeneralPathfindingSpace space;
		readonly Dictionary<Vector3, float> costMap = new();
		readonly Dictionary<Vector3, Vector3> flowMap = new();
		readonly Dictionary<Vector3, float> heuristicMap = new();
		IPathfindingParams @params;
		public GeneralPathfinder(GeneralPathfindingSpace space) : base(space) => this.space = space;
		protected override void Clear()
		{
			costMap.Clear();
			flowMap.Clear();
			heuristicMap.Clear();
		}
		protected override bool TryGetTotalCostUnverified(Vector3 node, out float cost)
		{
			return costMap.TryGetValue(node, out cost);
		}
		protected override void SetTotalCostUnverified(Vector3 node, float cost)
		{
			costMap[node] = cost;
		}
		protected override bool TryGetParentNodeUnverified(Vector3 node, out Vector3 parent)
		{
			return flowMap.TryGetValue(node, out parent);
		}
		protected override void SetParentNodeUnverified(Vector3 node, Vector3 parent)
		{
			flowMap[node] = parent;
		}
		protected override float GetHeuristicUnverified(Vector3 node)
		{
			if (!heuristicMap.TryGetValue(node, out var heuristic))
				heuristicMap[node] = heuristic = @params.GetHeuristic(node);
			return heuristic;
		}
		protected override float GetStepCostUnverified(Vector3 from, Vector3 to, byte costType)
		{
			return @params.GetStepCost(costType) * (to - from).magnitude;
		}
		public float GetCost(Vector3 position)
		{
			return costMap.GetValueOrDefault(position, float.PositiveInfinity);
		}
		public bool TryGetParent(Vector3 position, out Vector3 parent)
		{
			return TryGetParentNodeUnverified(position, out parent);
		}
		public void Reset(IPathfindingParams @params)
		{
			base.Reset(@params.Sources, @params.MaxCost, @params.MaxHeuristic);
			this.@params = @params;
		}
		public bool MoveNext(out Vector3 currentNode)
		{
			return Next(out currentNode);
		}
	}
}
