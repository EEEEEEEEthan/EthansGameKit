using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.AStar
{
	public sealed class CommonPathfindingSpace : PathfindingSpace<Vector3, Vector3>
	{
		public sealed class CommonPathfinder : Pathfinder
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

		readonly HashSet<Vector3> allPositions = new();
		readonly Dictionary<Vector3, Dictionary<Vector3, float>> links = new();
		readonly CachePool<CommonPathfinder> pathfinderPool = new(0);
		public CommonPathfindingSpace(int maxLinkCountPerNode = 8) : base(maxLinkCountPerNode)
		{
		}
		public override int NodeCount => allPositions.Count;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool ContainsPosition(Vector3 position) => allPositions.Contains(position);
		protected override int GetLinks(Vector3 node, Vector3[] toNodes, float[] basicCosts)
		{
			if (links.TryGetValue(node, out var toNodesDict))
			{
				var i = 0;
				if (toNodes.Length < toNodesDict.Count) toNodes = new Vector3[toNodesDict.Count];
				if (basicCosts.Length < toNodesDict.Count) basicCosts = new float[toNodesDict.Count];
				foreach (var (toNode, basicCost) in toNodesDict)
				{
					toNodes[i] = toNode;
					basicCosts[i] = basicCost;
					i++;
				}
				return i;
			}
			return 0;
		}
		public override Vector3 GetPositionUnverified(Vector3 key) => key;
		public override Vector3 GetIndexUnverified(Vector3 position) => position;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool ContainsKey(Vector3 key) => allPositions.Contains(key);
		public void ClearLinks()
		{
			MarkChanged();
			allPositions.Clear();
			links.Clear();
		}
		public void SetLink(Vector3 fromNode, Vector3 toNode, float basicCost)
		{
			MarkChanged();
			if (!links.TryGetValue(fromNode, out var toNodes))
				links[fromNode] = toNodes = new();
			toNodes[toNode] = basicCost;
			allPositions.Add(fromNode);
			allPositions.Add(toNode);
		}
		public bool RemoveLink(Vector3 fromNode, Vector3 toNode)
		{
			MarkChanged();
			if (links.TryGetValue(fromNode, out var toNodes) && toNodes.Remove(toNode))
			{
				if (toNodes.Count == 0)
					links.Remove(fromNode);
				return true;
			}
			return false;
		}
	}
}
