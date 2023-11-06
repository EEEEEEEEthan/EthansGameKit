using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.AStar.General
{
	public sealed class GeneralPathfindingSpace : PathfindingSpace<Vector3, Vector3>
	{
		readonly HashSet<Vector3> allPositions = new();
		readonly Dictionary<Vector3, Dictionary<Vector3, float>> links = new();
		readonly CachePool<GeneralPathfinder> pathfinderPool = new(0);
		public override int NodeCount => allPositions.Count;
		public GeneralPathfindingSpace(int maxLinkCountPerNode = 8) : base(maxLinkCountPerNode)
		{
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool ContainsPosition(Vector3 position) => allPositions.Contains(position);
		public override Vector3 GetPositionUnverified(Vector3 key) => key;
		public override Vector3 GetIndexUnverified(Vector3 position) => position;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool ContainsKey(Vector3 key) => allPositions.Contains(key);
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
