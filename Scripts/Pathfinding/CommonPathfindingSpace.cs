using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	public class CommonPathfindingSpace : PathfindingSpace<Vector3>
	{
		readonly Dictionary<Vector3, Dictionary<Vector3, float>> links = new();
		public override void ClearLinks()
		{
			links.Clear();
		}
		public override void SetLink(Vector3 fromNode, Vector3 toNode, float basicCost)
		{
			if (!links.TryGetValue(fromNode, out var toNodes))
				links[fromNode] = toNodes = new();
			toNodes[toNode] = basicCost;
		}
		public override bool RemoveLink(Vector3 fromNode, Vector3 toNode)
		{
			if (links.TryGetValue(fromNode, out var toNodes) && toNodes.Remove(toNode))
			{
				if (toNodes.Count == 0)
					links.Remove(fromNode);
				return true;
			}
			return false;
		}
		public override int GetLinks(Vector3 node, ref Vector3[] toNodes, ref float[] basicCosts)
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
		public override int GetLinkSources(ref Vector3[] result)
		{
			var i = 0;
			if (result.Length < links.Count) result = new Vector3[links.Count];
			foreach (var fromNode in links.Keys)
				result[i++] = fromNode;
			return i;
		}
		/// <param name="sourceNode"></param>
		/// <param name="targetNode"></param>
		/// <param name="result">长度为0:无路径。长度为1:起点即终点。长度为2:包含起点和终点的路径</param>
		/// <param name="reversed"></param>
		/// <returns></returns>
		public bool TryGetPath(Vector3 sourceNode, Vector3 targetNode, List<Vector3> result, bool reversed = false)
		{
			var pathfinder = new CommonPathfinder(this);
			pathfinder.Reinitialize(new[] { sourceNode }, targetNode);
			while (pathfinder.MoveNext(out var step))
			{
				if (step == targetNode)
				{
					var node = step;
					result.Add(node);
					while (pathfinder.FromMap.TryGetValue(node, out var from) && from != node)
						result.Add(node = from);
					if (!reversed) result.Reverse();
					return true;
				}
			}
			return false;
		}
	}
}
