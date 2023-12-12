namespace EthansGameKit.Pathfinding
{
	public abstract class PathfindingSpace<T>
	{
		// ReSharper disable once CollectionNeverUpdated.Local
		public readonly int maxLinkCountPerNode;
		internal int ChangeFlag { get; private set; }
		protected PathfindingSpace(int maxLinkCountPerNode) => this.maxLinkCountPerNode = maxLinkCountPerNode;
		protected internal abstract int GetLinks(T fromNode, T[] toNodes, byte[] costTypes);
		protected void MarkDirty()
		{
			++ChangeFlag;
		}
	}
}
