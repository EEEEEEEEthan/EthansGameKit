using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EthansGameKit.Pathfinding.General
{
	public interface IGeneralPathfindingParams
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
		///     获取某一消耗类型的真实消耗.高频调用,建议inline
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] float GetStepCost(Vector3 from, Vector3 to, float basicCost);
		/// <summary>
		///     获取某一位置的启发值.高频调用,建议inline
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] float GetHeuristic(Vector3 position);
	}
}
