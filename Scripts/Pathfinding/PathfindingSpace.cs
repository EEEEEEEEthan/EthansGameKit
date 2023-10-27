using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	[Serializable]
	public abstract class PathfindingSpace<TNode> : MonoBehaviour
	{
		TNode[] buffer_TNodes = Array.Empty<TNode>();
		byte[] buffer_Bytes = Array.Empty<byte>();
		public abstract void AddLink(TNode fromNode, TNode toNode, byte costType);
		public abstract void RemoveLink(TNode fromNode, TNode toNode);
		public abstract int GetLinks(TNode node, ref TNode[] toNodes, ref byte[] costTypes);
		public (TNode[] toNodes, byte[]costTypes) GetLinks(TNode node)
		{
			var length = GetLinks(node, ref buffer_TNodes, ref buffer_Bytes);
			var toNodes = new TNode[length];
			var costTypes = new byte[length];
			Array.Copy(buffer_TNodes, toNodes, length);
			Array.Copy(buffer_Bytes, costTypes, length);
			return (toNodes, costTypes);
		}
		public void GetLinks(TNode node, List<TNode> result, List<byte> costTypes)
		{
			var length = GetLinks(node, ref buffer_TNodes, ref buffer_Bytes);
			for (var i = 0; i < length; i++)
			{
				result.Add(buffer_TNodes[i]);
				costTypes.Add(buffer_Bytes[i]);
			}
		}
		public abstract int GetLinkSources(ref TNode[] result);
		public TNode[] GetLinkSources()
		{
			var length = GetLinkSources(ref buffer_TNodes);
			var result = new TNode[length];
			Array.Copy(buffer_TNodes, result, length);
			return result;
		}
		public void GetLinkSources(List<TNode> result)
		{
			var length = GetLinkSources(ref buffer_TNodes);
			for (var i = 0; i < length; i++)
				result.Add(buffer_TNodes[i]);
		}
	}
}
