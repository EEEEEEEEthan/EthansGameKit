using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Pathfinding.General
{
	class GeneralPathfinder : Pathfinder<Vector3>
	{
		public new readonly GeneralPathfindingSpace space;
		readonly Dictionary<Vector3, float> costMap = new();
		readonly Dictionary<Vector3, Vector3> flowMap = new();
		readonly Dictionary<Vector3, float> heuristicMap = new();
		Func<Vector3, float> heuristicGetter;
		Vector3 destination;
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
				heuristicMap[node] = heuristic = heuristicGetter(node);
			return heuristic;
		}
		public float GetCost(Vector3 position)
		{
			return costMap.GetValueOrDefault(position, float.PositiveInfinity);
		}
		public bool TryGetParent(Vector3 position, out Vector3 parent)
		{
			return TryGetParentNodeUnverified(position, out parent);
		}
		public void Reset(IEnumerable<Vector3> sources, Vector3 target, float maxCost, float maxHeuristic)
		{
			destination = target;
			base.Reset(sources, maxCost, maxHeuristic);
		}
		public bool MoveNext(out Vector3 currentNode)
		{
			return Next(out currentNode);
		}
	}
}
