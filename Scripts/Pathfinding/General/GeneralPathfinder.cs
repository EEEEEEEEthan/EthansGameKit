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
		Vector3 destination;
		public GeneralPathfinder(GeneralPathfindingSpace space) : base(space) => this.space = space;
		protected override void Clear()
		{
			costMap.Clear();
			flowMap.Clear();
			heuristicMap.Clear();
		}
		protected override bool TryGetTotalCost(Vector3 node, out float cost)
		{
			return costMap.TryGetValue(node, out cost);
		}
		protected override void SetTotalCost(Vector3 node, float cost)
		{
			costMap[node] = cost;
		}
		protected override bool TryGetParentNode(Vector3 node, out Vector3 parent)
		{
			return flowMap.TryGetValue(node, out parent);
		}
		protected override void SetParentNode(Vector3 node, Vector3 parent)
		{
			flowMap[node] = parent;
		}
		protected override float GetHeuristic(Vector3 node)
		{
			if (!heuristicMap.TryGetValue(node, out var heuristic))
			{
				heuristic = Vector3.Distance(node, destination);
				heuristicMap[node] = heuristic;
			}
			return heuristic;
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
