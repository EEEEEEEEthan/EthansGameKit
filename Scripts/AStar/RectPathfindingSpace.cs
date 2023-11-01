using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.AStar
{
	/// <summary>
	///     针对四邻域深度优化的寻路空间
	/// </summary>
	public sealed class RectPathfindingSpace : PathfindingSpace<int>
	{
		public sealed class RectPathfinder : Pathfinder
		{
			public static RectPathfinder Create(RectPathfindingSpace space)
			{
				if (space.pool.TryGenerate(out var pathfinder)) return pathfinder;
				return new(space);
			}
			public readonly RectPathfindingSpace space;
			readonly float[] costMap;
			readonly int[] flowMap;
			Vector2Int heuristicTarget;
			RectPathfinder(RectPathfindingSpace space) : base(space)
			{
				this.space = space;
				costMap = new float[space.nodeCount];
				flowMap = new int[space.nodeCount];
				Clear();
			}
			protected override void Recycle()
			{
				space.pool.Recycle(this);
			}
			protected override float GetHeuristic(int node)
			{
				var position = space.GetPosition(node);
				return (position - heuristicTarget).sqrMagnitude;
			}
			protected override float GetStepCost(int fromNode, int toNode, float basicCost) => basicCost;
			protected override bool GetCachedTotalCost(int node, out float cost)
			{
				cost = costMap[node];
				return true;
			}
			protected override void CacheTotalCost(int node, float cost) => costMap[node] = cost;
			protected override bool GetCachedParentNode(int node, out int parent) => (parent = flowMap[node]) >= 0;
			protected override void CacheParentNode(int node, int parent) => flowMap[node] = parent;
			protected override void OnClear()
			{
				costMap.MemSet(float.MaxValue);
				flowMap.MemSet(-1);
			}
			/// <summary>尝试获取路径</summary>
			/// <param name="target">目标</param>
			/// <param name="path">
			///     <para>一个栈表示路径。起点在0号元素</para>
			///     <para>若路径不存在，得到null</para>
			///     <para>若起点终点相同，则长度为1</para>
			/// </param>
			/// <returns>true-路径存在; false-路径不存在</returns>
			public bool TryGetPath(Vector2Int target, out List<Vector2Int> path)
			{
				var intTarget = space.GetIndex(target);
				if (!base.TryGetPath(intTarget, out var intStack))
				{
					path = null;
					return false;
				}
				path = ListPool<Vector2Int>.Generate();
				while (intStack.TryPop(out var index))
					path.Add(space.GetPosition(index));
				return true;
			}
			#region reinitialize
			int[] buffer_reinitialize = Array.Empty<int>();
			public void Reinitialize(IReadOnlyList<Vector2Int> sources, Vector2Int heuristicTarget)
			{
				this.heuristicTarget = heuristicTarget;
				if (buffer_reinitialize.Length < sources.Count) buffer_reinitialize = new int[sources.Count];
				for (var i = sources.Count; i-- > 0;) buffer_reinitialize[i] = space.GetIndex(sources[i]);
				base.Reinitialize(
					float.PositiveInfinity,
					float.PositiveInfinity,
					buffer_reinitialize
				);
			}
			#endregion
		}

		public enum DirectionEnum
		{
			Up,
			Right,
			Down,
			Left,
			UpRight,
			DownRight,
			DownLeft,
			UpLeft,
		}

		public readonly int nodeCount;
		readonly int widthPower;
		readonly int width;
		readonly int xMin;
		readonly int yMin;
		readonly float[] costs;
		readonly int[] neighborIndexOffsetSequence;
		readonly int linkBits;
		readonly int linkCount;
		readonly CachePool<RectPathfinder> pool = new(0);
		RectInt rect;
		public RectInt Rect => rect;
		public RectPathfindingSpace(RectInt rect, bool allowDiagonal) : base(allowDiagonal ? 8 : 4)
		{
			this.rect = rect;
			widthPower = Mathf.CeilToInt(Mathf.Log(rect.width + 2, 2));
			var heightPower = Mathf.CeilToInt(Mathf.Log(rect.height + 2, 2));
			xMin = rect.xMin - 1;
			yMin = rect.yMin - 1;
			width = 1 << widthPower;
			var height = 1 << heightPower;
			nodeCount = width * height;
			linkBits = allowDiagonal ? 3 : 2;
			linkCount = 1 << linkBits;
			costs = new float[nodeCount << linkBits];
			neighborIndexOffsetSequence = new[]
			{
				width,
				1,
				-width,
				-1,
				width + 1,
				-width + 1,
				-width - 1,
				width - 1,
			};
		}
		protected override int GetLinks(int node, int[] toNodes, float[] basicCosts)
		{
			var count = 0;
			for (var direction = 0; direction < linkCount; ++direction)
			{
				var neighborIndex = GetNeighborIndexUnverified(node, direction);
				var cost = costs[(neighborIndex << linkBits) | direction];
				if (cost <= 0) continue;
				basicCosts[count] = cost;
				++count;
			}
			return count;
		}
		public RectPathfinder CreatePathfinder() => RectPathfinder.Create(this);
		/// <summary>尝试获取路径</summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="listedWaypoints">
		///     <para>一个栈表示路径。起点在0号元素</para>
		///     <para>若路径不存在，得到null</para>
		///     <para>若起点终点相同，则长度为1</para>
		/// </param>
		/// <returns>true-路径存在; false-路径不存在</returns>
		public bool TryGetPath(Vector2Int from, Vector2Int to, out List<Vector2Int> listedWaypoints)
		{
			using var pathfinder = CreatePathfinder();
			var fromIndex = GetIndex(from);
			var toIndex = GetIndex(to);
			pathfinder.Reinitialize(new[] { from }, to);
			while (pathfinder.MoveNext(out _))
			{
			}
			return pathfinder.TryGetPath(to, out listedWaypoints);
		}
		public void ClearLinks()
		{
			MarkChanged();
			costs.MemSet(0);
		}
		public void RemoveLink(Vector2Int fromNode, DirectionEnum direction)
		{
			MarkChanged();
			var fromIndex = GetIndex(fromNode);
			costs[GetLinkIndexUnverified(fromIndex, (int)direction)] = 0;
		}
		public void SetLink(Vector2Int fromNode, DirectionEnum direction, float basicCost)
		{
			MarkChanged();
			var fromIndex = GetIndex(fromNode);
			costs[GetLinkIndexUnverified(fromIndex, (int)direction)] = basicCost;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(Vector2Int position)
		{
			if (!rect.Contains(position)) throw new ArgumentOutOfRangeException(nameof(position));
			return GetIndexUnverified(position);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int GetPosition(int index)
		{
			if (index > nodeCount) throw new ArgumentOutOfRangeException(nameof(index));
			var position = GetPositionUnverified(index);
			if (!rect.Contains(position)) throw new ArgumentOutOfRangeException(nameof(index));
			return position;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int GetNeighborIndexUnverified(int fromNode, int direction) => fromNode + neighborIndexOffsetSequence[direction];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int GetLinkIndexUnverified(int fromNode, int direction) => (GetNeighborIndexUnverified(fromNode, direction) << linkBits) | direction;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int GetIndexUnverified(Vector2Int position)
		{
			var x = position.x - xMin;
			var y = position.y - yMin;
			return (y << widthPower) | x;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Vector2Int GetPositionUnverified(int index)
		{
			var x = index & (width - 1);
			var y = index >> widthPower;
			return new(x + xMin, y + yMin);
		}
	}
}
