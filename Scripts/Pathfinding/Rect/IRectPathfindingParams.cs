using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EthansGameKit.Pathfinding.Rect
{
	/// <summary>
	///     寻路器的寻路参数
	/// </summary>
	public interface IRectPathfindingParams
	{
		/// <summary>
		///     本次寻路的起点集合
		/// </summary>
		IEnumerable<Vector2Int> Sources { get; }
		/// <summary>
		///     本次寻路的最大消耗
		/// </summary>
		float MaxCost { get; }
		/// <summary>
		///     本次寻路的最大启发值
		/// </summary>
		float MaxHeuristic { get; }
		/// <summary>
		///     复写的连接。这不会改变原有连接，仅在使用这个寻路参数的寻路过程中生效
		/// </summary>
		IEnumerable<(Vector2Int from, GridDirections direction, byte overrideCostType)> OverrideLinks { get; }
		/// <summary>
		///     获取某一消耗类型的真实消耗.高频调用,建议inline
		/// </summary>
		/// <remarks>
		///     尽可能把计算量放在costType里.SEE:<see cref="RectPathfindingSpace.SetLink" />
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] float CalculateStepCost(Vector2Int from, GridDirections direction, byte costType);
		/// <summary>
		///     获取某一位置的启发值.高频调用,建议inline
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] float CalculateHeuristic(Vector2Int position);
	}
}
