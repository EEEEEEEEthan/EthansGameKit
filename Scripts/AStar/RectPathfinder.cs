using System.Runtime.CompilerServices;

namespace EthansGameKit.AStar
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public sealed class RectPathfinder : RectPathfinderBase
	{
		public RectPathfinder(RectPathfindingSpace space) : base(space)
		{
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float GetHeuristicUnverified(int node)
		{
			var position = space.GetPositionUnverified(node);
			return (position - HeuristicTarget).sqrMagnitude;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float GetStepCostUnverified(int fromNode, int toNode, float basicCost)
		{
			return basicCost;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool GetTotalCostUnverified(int node, out float cost)
		{
			cost = costMap[node];
			return true;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SetTotalCostUnverified(int node, float cost)
		{
			costMap[node] = cost;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool GetParentNodeUnverified(int node, out int parent)
		{
			return (parent = flowMap[node]) >= 0;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SetParentNodeUnverified(int node, int parent)
		{
			flowMap[node] = parent;
		}
	}
}
