using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	class CommonPathfindingSpace : PathfindingSpace<Vector3>
	{
		public override void AddLink(Vector3 fromNode, Vector3 toNode, byte costType)
		{
			throw new System.NotImplementedException();
		}
		public override void RemoveLink(Vector3 fromNode, Vector3 toNode)
		{
			throw new System.NotImplementedException();
		}
		public override int GetLinks(Vector3 node, ref Vector3[] toNodes, ref byte[] costTypes)
		{
			throw new System.NotImplementedException();
		}
		public override int GetLinkSources(ref Vector3[] result)
		{
			throw new System.NotImplementedException();
		}
	}
}
