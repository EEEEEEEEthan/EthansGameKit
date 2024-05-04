namespace EthansGameKit.Pathfinding
{
	public interface IPathfindingArguments<in T>
	{
		/// <summary>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="costType">消耗类型</param>
		/// <returns></returns>
		float CalculateStepCost(T from, T to, byte costType);

		float CalculateHeuristic(T position);
	}
}