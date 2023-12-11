using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	public interface IPathfindingParams
	{
		/// <summary>
		///     本次寻路的起点集合
		/// </summary>
		IEnumerable<Vector3> Sources { get; }
		/// <summary>
		///     本次寻路的最大消耗
		/// </summary>
		float MaxCost { get; }
		/// <summary>
		///     本次寻路的最大启发值
		/// </summary>
		float MaxHeuristic { get; }
		/// <summary>
		///     消耗类型到消耗值的映射。下标是消耗类型
		/// </summary>
		IReadOnlyList<float> CostType2TrueCosts { get; }
		/// <summary>
		///     获取某一位置的启发值
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		float GetHeuristic(Vector3 position);
	}
}
