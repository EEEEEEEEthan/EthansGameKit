using System;
using System.Collections.Generic;
using System.Linq;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	/// <summary>
	///     四邻网格寻路
	/// </summary>
	[Serializable]
	public sealed class PathfindingSpace_QuadTileBased : PathfindingSpace
	{
		public enum DirectionCode
		{
			North,
			East,
			South,
			West,
			NorthEast,
			SouthEast,
			SouthWest,
			NorthWest,
		}

		/// <summary>
		///     寻路点坐标
		/// </summary>
		public struct TilePosition
		{
			public int x;
			public int y;
			public Vector2 XY => new(x, y);
			public Vector3 XZ => new(x, 0, y);
		}

		/// <summary>
		///     寻路器
		/// </summary>
		public new sealed class Pathfinder : PathfindingSpace.Pathfinder
		{
			static int[] buffer = Array.Empty<int>();
			readonly int[] fromMap;
			readonly float[] costMap;
			new readonly PathfindingSpace_QuadTileBased pathfindingSpace;
			int changeFlag;
			public Pathfinder(PathfindingSpace_QuadTileBased pathfindingSpace) : base(pathfindingSpace, 8)
			{
				this.pathfindingSpace = pathfindingSpace;
				fromMap = new int[pathfindingSpace.Count];
				costMap = new float[pathfindingSpace.Count];
			}
			protected override void Clear()
			{
				fromMap.MemSet(-1);
				costMap.MemSet(float.PositiveInfinity);
			}
			protected override bool TryGetFromInfo(int nodeId, out int fromNodeId, out float totalCost)
			{
				fromNodeId = fromMap[nodeId];
				totalCost = costMap[nodeId];
				return fromNodeId != -1;
			}
			protected override void SetFromInfo(int fromNodeId, int toNodeId, float totalCost)
			{
				fromMap[toNodeId] = fromNodeId;
				costMap[toNodeId] = totalCost;
			}
			public void Reset(IEnumerable<TilePosition> positions)
			{
				base.Reset(positions.Select(pos => pathfindingSpace.GetNodeIndex(pos.x, pos.y)));
			}
			/// <summary>
			///     获取路径
			/// </summary>
			/// <param name="path">从起点(含)到终点(含)的路径</param>
			/// <param name="position"></param>
			/// <param name="reversed">若为真,<paramref name="path"/>变为从终点(含)到起点(含)</param>
			/// <returns>路径是否已发现</returns>
			public bool TryGetPath(List<TilePosition> path, TilePosition position, bool reversed = false)
			{
				path.Clear();
				var length = GetPath(ref buffer, pathfindingSpace.GetNodeIndex(position.x, position.y));
				if (length <= 0)
					return false;
				if (reversed)
				{
					for (var i = 0; i < length; ++i)
					{
						var index = buffer[i];
						pathfindingSpace.GetNodePosition(index, out var x, out var y);
						path.Add(new() { x = x, y = y });
					}
				}
				else
				{
					for (var i = length; i-- > 0;)
					{
						var index = buffer[i];
						pathfindingSpace.GetNodePosition(index, out var x, out var y);
						path.Add(new() { x = x, y = y });
					}
				}
				return true;
			}
		}

		const float invalidCost = -1;
		public static IReadOnlyList<DirectionCode> quadDirections = new[]
		{
			DirectionCode.North,
			DirectionCode.East,
			DirectionCode.South,
			DirectionCode.West,
		};
		public static IReadOnlyList<DirectionCode> octDirections = new[]
		{
			DirectionCode.North,
			DirectionCode.East,
			DirectionCode.South,
			DirectionCode.West,
			DirectionCode.NorthEast,
			DirectionCode.SouthEast,
			DirectionCode.SouthWest,
			DirectionCode.NorthWest,
		};
		static void Direction2XZ(DirectionCode direction, out int x, out int z)
		{
			switch (direction)
			{
				case DirectionCode.North:
					x = 0;
					z = 1;
					break;
				case DirectionCode.East:
					x = 1;
					z = 0;
					break;
				case DirectionCode.South:
					x = 0;
					z = -1;
					break;
				case DirectionCode.West:
					x = -1;
					z = 0;
					break;
				case DirectionCode.NorthEast:
					x = 1;
					z = 1;
					break;
				case DirectionCode.SouthEast:
					x = 1;
					z = -1;
					break;
				case DirectionCode.SouthWest:
					x = -1;
					z = -1;
					break;
				case DirectionCode.NorthWest:
					x = -1;
					z = 1;
					break;
				default: throw new ArgumentOutOfRangeException(nameof(direction));
			}
		}
		CachePool<Pathfinder> pathfinderPool = new(0);
		[SerializeField] int width;
		[SerializeField] int widthPower;
		[SerializeField] int count;
		[SerializeField] int maxDirection;
		[SerializeField] int posMask;
		[SerializeField] float[] stepCosts;
		[SerializeField] int dataWidthPowerPerTile;
		public int Width => width;
		public int Count => count;
		public PathfindingSpace_QuadTileBased(int widthPower, bool oct)
		{
			this.widthPower = widthPower;
			width = 1 << widthPower;
			count = Width << widthPower;
			posMask = Width - 1;
			maxDirection = oct ? 8 : 4;
			dataWidthPowerPerTile = oct ? 2 : 3;
			stepCosts = new float[count << dataWidthPowerPerTile];
			stepCosts.MemSet(invalidCost);
		}
		protected override int GetLinks(int fromNodeId, StepInfo[] toAndCost)
		{
			if (!Contains(fromNodeId)) throw new ArgumentOutOfRangeException(nameof(fromNodeId));
			var length = 0;
			for (var direction = 0; direction < maxDirection; ++direction)
			{
				var index = GetStepIndex(fromNodeId, (DirectionCode)direction);
				var cost = stepCosts[index];
				if (cost != invalidCost)
				{
					Direction2XZ((DirectionCode)direction, out var toX, out var toZ);
					var toIndex = GetNodeIndex(toX, toZ);
					toAndCost[length].toNodeId = toIndex;
					toAndCost[length].stepCost = cost;
					++length;
				}
			}
			return length;
		}
		public Pathfinder CreatePathfinder()
		{
			if (pathfinderPool.TryGenerate(out var pathfinder))
				return pathfinder;
			return new(this);
		}
		public void RecyclePathfinder(Pathfinder pathfinder)
		{
			if (pathfinder.pathfindingSpace != this) throw new ArgumentException("pathfinder not belongs to this space");
			pathfinderPool.Recycle(pathfinder);
		}
		public bool Contains(TilePosition position)
		{
			return Contains(position.x, position.y);
		}
		public bool Contains(int index)
		{
			return index > 0 && index < count;
		}
		public void GetNodePosition(int index, out TilePosition position)
		{
			GetNodePosition(index, out var x, out var y);
			position = new()
			{
				x = x,
				y = y,
			};
		}
		public void SetLink(int x, int y, DirectionCode direction, float cost)
		{
			if (!TryGetNodeIndex(new() { x = x, y = y }, out var fromIndex))
				throw new ArgumentOutOfRangeException();
			SetLink(fromIndex, direction, cost);
		}
		public void SetLink(TilePosition fromPosition, DirectionCode direction, float cost)
		{
			if (!TryGetNodeIndex(fromPosition, out var fromIndex))
				throw new ArgumentOutOfRangeException(nameof(fromPosition));
			SetLink(fromIndex, direction, cost);
		}
		public void SetLink(int fromIndex, DirectionCode direction, float cost)
		{
			if (!TryGetNeighborIndex(fromIndex, direction, out var toIndex))
			{
				Debug.LogError($"unexpected error: {nameof(TryGetNeighborIndex)} failed");
				return;
			}
			stepCosts[toIndex] = cost;
		}
		public void RemoveLink(int x, int y, DirectionCode direction)
		{
			if (!TryGetNodeIndex(new() { x = x, y = y }, out var fromIndex))
				throw new ArgumentOutOfRangeException();
			if (!TryGetNeighborIndex(fromIndex, direction, out var toIndex))
			{
				Debug.LogError($"unexpected error: {nameof(TryGetNeighborIndex)} failed");
				return;
			}
			stepCosts[toIndex] = invalidCost;
		}
		public void RemoveLink(TilePosition fromPosition, DirectionCode direction)
		{
			if (!TryGetNodeIndex(fromPosition, out var fromIndex))
				throw new ArgumentOutOfRangeException(nameof(fromPosition));
			if (!TryGetNeighborIndex(fromIndex, direction, out var toIndex))
			{
				Debug.LogError($"unexpected error: {nameof(TryGetNeighborIndex)} failed");
				return;
			}
			stepCosts[toIndex] = invalidCost;
		}
		public void RemoveAllLinks()
		{
			stepCosts.MemSet(invalidCost);
		}
		public bool TryGetNodeIndex(TilePosition position, out int index)
		{
			if (!Contains(position.x, position.y))
			{
				index = -1;
				return false;
			}
			index = GetNodeIndex(position.x, position.y);
			return true;
		}
		public bool TryGetNodeIndex(int x, int y, out int index)
		{
			if (!Contains(x, y))
			{
				index = -1;
				return false;
			}
			index = GetNodeIndex(x, y);
			return true;
		}
		public bool TryGetNeighborIndex(int fromIndex, DirectionCode direction, out int neighborIndex)
		{
			neighborIndex = -1;
			if (!Contains(fromIndex)) return false;
			if ((int)direction >= maxDirection) return false;
			GetNodePosition(fromIndex, out var x, out var z);
			Direction2XZ(direction, out var dx, out var dz);
			var toX = x + dx;
			var toZ = z + dz;
			if (!Contains(toX, toZ)) return false;
			neighborIndex = (fromIndex << dataWidthPowerPerTile) + (int)direction;
			return true;
		}
		internal void GetNodePosition(int index, out int x, out int y)
		{
			x = index >> widthPower;
			y = index & posMask;
		}
		internal bool Contains(int x, int y)
		{
			return (uint)x < (uint)width && (uint)y < (uint)width;
		}
		internal int GetNodeIndex(int x, int y)
		{
			return (x << widthPower) | y;
		}
		int GetStepIndex(int fromNode, DirectionCode direction)
		{
			if (!Contains(fromNode)) throw new ArgumentOutOfRangeException(nameof(fromNode));
			if ((int)direction >= maxDirection) throw new ArgumentOutOfRangeException(nameof(direction));
			GetNodePosition(fromNode, out var x, out var z);
			Direction2XZ(direction, out var dx, out var dz);
			var toX = x + dx;
			var toZ = z + dz;
			if (!Contains(toX, toZ)) throw new ArgumentOutOfRangeException(nameof(direction));
			return (fromNode << dataWidthPowerPerTile) + (int)direction;
		}
	}
}
