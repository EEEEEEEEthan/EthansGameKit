using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Pathfinding.General
{
	public sealed class GeneralPathfindingSpace : PathfindingSpace<Vector3>
	{
		readonly Dictionary<Vector3, Dictionary<Vector3, byte>> links = new();
		public GeneralPathfindingSpace(int maxLinkCountPerNode) : base(maxLinkCountPerNode)
		{
		}
		protected internal override int GetLinks(Vector3 fromNode, Vector3[] toNodes, byte[] costs)
		{
			if (!links.TryGetValue(fromNode, out var dict)) return 0;
			var i = 0;
			foreach (var (toNode, cost) in dict)
			{
				toNodes[i] = toNode;
				costs[i] = cost;
				++i;
			}
			return i;
		}
		public void AddLink(Vector3 fromNode, Vector3 toNode, byte costType)
		{
			if (!links.TryGetValue(fromNode, out var dict))
			{
				dict = DictionaryPool<Vector3, byte>.Generate();
				links.Add(fromNode, dict);
			}
			dict.Add(toNode, costType);
			MarkDirty();
		}
		public bool RemoveLink(Vector3 fromNode, Vector3 toNode)
		{
			if (!links.TryGetValue(fromNode, out var dict)) return false;
			if (dict.Remove(toNode))
			{
				if (dict.Count == 0) DictionaryPool<Vector3, byte>.ClearAndRecycle(ref dict);
				MarkDirty();
				return true;
			}
			return false;
		}
	}
}
