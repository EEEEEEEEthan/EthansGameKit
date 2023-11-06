using System.Runtime.CompilerServices;

namespace EthansGameKit.AStar.RectGrid
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public sealed class RectPathfinder : RectPathfinderBase
	{
		RectPathfinder()
		{
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float GetHeuristic(int node)
		{
			var position = Space.GetPositionUnverified(node);
			return (position - HeuristicTarget).sqrMagnitude;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float GetStepCost(int fromNode, int toNode, float basicCost) => basicCost;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool GetCachedTotalCost(int node, out float cost)
		{
			cost = costMap[node];
			return true;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CacheTotalCost(int node, float cost) => costMap[node] = cost;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool GetCachedParentNode(int node, out int parent) => (parent = flowMap[node]) >= 0;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CacheParentNode(int node, int parent) => flowMap[node] = parent;
	}
}
