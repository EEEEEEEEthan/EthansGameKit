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
			public static CommonPathfinder Create(CommonPathfindingSpace space)
			{
				if (space.pathfinderPool.TryGenerate(out var finder)) return finder;
				return new(space);
			}
			readonly CommonPathfindingSpace space;
			readonly Dictionary<Vector3, float> costMap = new();
			readonly Dictionary<Vector3, Vector3> flowMap = new();
			Vector3 heuristicTarget;
			public IReadOnlyDictionary<Vector3, Vector3> FlowMap => flowMap;
			public IReadOnlyDictionary<Vector3, float> CostMap => costMap;
			CommonPathfinder(CommonPathfindingSpace space) : base(space) => this.space = space;
			protected override void Recycle()
			{
				space.pathfinderPool.Recycle(this);
			}
			protected override float GetHeuristic(Vector3 node) => Vector3.Distance(node, heuristicTarget);
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
			public void Reinitialize(IEnumerable<Vector3> sources, Vector3 heuristicTarget)
			{
				this.heuristicTarget = heuristicTarget;
				base.Reinitialize(
					float.PositiveInfinity,
					float.PositiveInfinity,
					sources
				);
			}
			/// <summary>尝试获取路径</summary>
			/// <param name="target">目标</param>
			/// <param name="path">
			///     <para>一个栈表示路径。终点先入栈，起点最后入栈</para>
			///     <para>若路径不存在，得到null</para>
			///     <para>若起点终点相同，则长度为1</para>
			/// </param>
			/// <returns>true-路径存在; false-路径不存在</returns>
			public new bool TryGetPath(Vector3 target, out Stack<Vector3> path) => base.TryGetPath(target, out path);
		}

		readonly HashSet<Vector3> allPositions = new();
		readonly Dictionary<Vector3, Dictionary<Vector3, float>> links = new();
		readonly CachePool<CommonPathfinder> pathfinderPool = new(0);
		public CommonPathfindingSpace(int maxLinkCountPerNode = 8) : base(maxLinkCountPerNode)
		{
		}
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
		protected override Vector3 GetPositionUnverified(Vector3 key) => key;
		protected override Vector3 GetIndexUnverified(Vector3 position) => position;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool ContainsKey(Vector3 key) => allPositions.Contains(key);
		public CommonPathfinder CreatePathfinder() => CommonPathfinder.Create(this);
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
		public bool TryGetPath(Vector3 fromNode, Vector3 toNode, out Stack<Vector3> stackedWaypoints)
		{
			using var pathfinder = CreatePathfinder();
			pathfinder.Reinitialize(new[] { fromNode }, toNode);
			while (pathfinder.MoveNext(out _))
			{
			}
			return pathfinder.TryGetPath(toNode, out stackedWaypoints);
		}
	}
}
