namespace EthansGameKit.Pathfinding
{
	public abstract class PathFindingSpace<T>
	{
		protected internal abstract int GetLinks(T index, ref T[] neighbors, ref byte[] costTypes);
	}
}