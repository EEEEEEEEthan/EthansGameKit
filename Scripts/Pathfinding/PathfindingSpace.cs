namespace EthansGameKit.Pathfinding
{
	public abstract class PathfindingSpace<T>
	{
		public readonly int maxLinkCountPerNode;
		protected PathfindingSpace(int maxLinkCountPerNode) => this.maxLinkCountPerNode = maxLinkCountPerNode;
		protected internal abstract int GetLinks(T fromNode, T[] toNodes, float[] costs);
	}
}
