using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	public class CommonPathfinder : PathfindingSpace<Vector3>.Pathfinder
	{
		Vector3 heuristicTarget;
		readonly Dictionary<Vector3, float> costMap = new();
		readonly Dictionary<Vector3, Vector3> fromMap = new();
		public void Reinitialize(IEnumerable<Vector3> sources, Vector3 heuristicTarget)
		{
			costMap.Clear();
			fromMap.Clear();
			this.heuristicTarget = heuristicTarget;
			base.Reinitialize(
				maxCost: float.PositiveInfinity,
				maxHeuristic: float.PositiveInfinity,
				sources: sources
			);
		}
		protected override float GetHeuristic(Vector3 node)
		{
			return Vector3.Distance(node, heuristicTarget);
		}
		public override IReadOnlyDictionary<Vector3, Vector3> FromMap => fromMap;
		public override IReadOnlyDictionary<Vector3, float> CostMap => costMap;
		protected override IDictionary<Vector3, float> WritableCostMap => costMap;
		protected override IDictionary<Vector3, Vector3> WritableFromMap => fromMap;
		protected override float GetStepCost(Vector3 fromNode, Vector3 toNode, float basicCost)
		{
			return basicCost;
		}
		public readonly CommonPathfindingSpace space;
		public CommonPathfinder(CommonPathfindingSpace space) : base(space)
		{
			this.space = space;
		}
	}
}
