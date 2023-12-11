namespace EthansGameKit.Pathfinding
{
	public abstract class PathfindingSpace<T>
	{
		public readonly int maxLinkCountPerNode;
		internal int ChangeFlag { get; private set; }
		protected PathfindingSpace(int maxLinkCountPerNode) => this.maxLinkCountPerNode = maxLinkCountPerNode;
		protected internal abstract int GetLinks(T fromNode, T[] toNodes, float[] costs);
		protected void MarkDirty()
		{
			++ChangeFlag;
		}
	}
}
