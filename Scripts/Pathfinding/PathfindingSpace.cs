using System;
using System.Collections.Generic;
using EthansGameKit.CachePools;

namespace EthansGameKit.Pathfinding
{
	public abstract class PathfindingSpace<T>
	{
		// ReSharper disable once CollectionNeverUpdated.Local
		static readonly Dictionary<Type, CachePool<Pathfinder<T>>> pools = new();
		public static bool TryGetPathfinder<TPathfinder>(out TPathfinder pathfinder) where TPathfinder : Pathfinder<T>
		{
			var type = typeof(TPathfinder);
			if (pools.TryGetValue(type, out var pool))
			{
				if (pool.TryGenerate(out var item))
				{
					pathfinder = (TPathfinder)item;
					return true;
				}
			}
			pathfinder = default;
			return false;
		}
		public static void RecyclePathfinder<TPathfinder>(ref TPathfinder pathfinder) where TPathfinder : Pathfinder<T>
		{
			if (!pools.TryGetValue(typeof(TPathfinder), out var pool))
				pool = pools[typeof(TPathfinder)] = new(0);
			pool.Recycle(pathfinder);
			pathfinder = null;
		}
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
