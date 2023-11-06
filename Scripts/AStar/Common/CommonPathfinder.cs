using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.AStar.Common
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public sealed class CommonPathfinder : PathfindingSpace<Vector3, Vector3>.Pathfinder
	{
		readonly Dictionary<Vector3, float> costMap = new();
		readonly Dictionary<Vector3, Vector3> flowMap = new();
		public override IReadOnlyDictionary<Vector3, Vector3> FlowMap => flowMap;
		public override IReadOnlyDictionary<Vector3, float> CostMap => costMap;
		protected override void OnInitialize()
		{
		}
		protected override float GetHeuristic(Vector3 node) => Vector3.Distance(node, HeuristicTarget);
		protected override float GetStepCost(Vector3 fromNode, Vector3 toNode, float basicCost) => basicCost;
		protected override bool GetCachedTotalCost(Vector3 node, out float cost) => costMap.TryGetValue(node, out cost);
		protected override void CacheTotalCost(Vector3 node, float cost) => costMap[node] = cost;
		protected override bool GetCachedParentNode(Vector3 node, out Vector3 fromNode) => flowMap.TryGetValue(node, out fromNode);
		protected override void CacheParentNode(Vector3 node, Vector3 fromNode) => flowMap[node] = fromNode;
		protected override void OnClear()
		{
			costMap.Clear();
			flowMap.Clear();
		}
	}
}
