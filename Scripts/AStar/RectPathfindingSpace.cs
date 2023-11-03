using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;
using EthansGameKit.Collections.Wrappers;
using UnityEngine;

namespace EthansGameKit.AStar
{
	/// <summary>
	///     针对四邻域深度优化的寻路空间
	/// </summary>
	public sealed class RectPathfindingSpace : PathfindingSpace<Vector2Int, int>
	{
		public sealed class RectPathfinder : Pathfinder
		{
			readonly struct IndexToPositionConverter : IValueConverter<int, Vector2Int>
			{
				readonly RectPathfindingSpace space;
				public IndexToPositionConverter(RectPathfindingSpace space) => this.space = space;
				public Vector2Int Convert(int oldItem) => space.GetPositionUnverified(oldItem);
				public int Recover(Vector2Int newItem) => space.GetIndexUnverified(newItem);
			}

			public static RectPathfinder Create(RectPathfindingSpace space)
			{
				if (space.pool.TryGenerate(out var pathfinder)) return pathfinder;
				return new(space);
			}
			public readonly RectPathfindingSpace space;
			readonly float[] costMap;
			readonly int[] flowMap;
			Vector2Int heuristicTarget;
			public IReadOnlyDictionary<Vector2Int, float> CostMap
			{
				get
				{
					var wrappedList = costMap.WrapAsDictionary();
					var converter = new IndexToPositionConverter(space);
					var dict = wrappedList.WrapAsConvertedDictionary(converter, new ValueConverter<float, float>(f => f, f => f));
					return dict;
				}
			}
			public IReadOnlyDictionary<Vector2Int, Vector2Int> FlowMap
			{
				get
				{
					var flowDict = flowMap.WrapAsDictionary();
					var converter = new IndexToPositionConverter(space);
					var dict = flowDict.WrapAsConvertedDictionary(converter, converter);
					var result = dict.WrapAsFilteredDictionary(k => flowMap[space.GetIndexUnverified(k)] >= 0);
					return result;
				}
			}
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
				var position = space.GetPositionUnverified(node);
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
			int[] buffer_reinitialize = new int[1];
			public void Reinitialize(IReadOnlyList<Vector2Int> sources, Vector2Int heuristicTarget, float maxCost = float.MaxValue, float maxheuristic = float.MaxValue)
			{
				this.heuristicTarget = heuristicTarget;
				if (buffer_reinitialize.Length < sources.Count) buffer_reinitialize = new int[sources.Count];
				for (var i = sources.Count; i-- > 0;) buffer_reinitialize[i] = space.GetIndex(sources[i]);
				base.Reinitialize(maxCost, maxheuristic, buffer_reinitialize);
			}
			public void Reinitialize(Vector2Int source, Vector2Int heuristicTarget, float maxCost = float.MaxValue, float maxheuristic = float.MaxValue)
			{
				buffer_reinitialize[0] = space.GetIndex(source);
				base.Reinitialize(maxCost, maxheuristic, buffer_reinitialize);
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
		RectInt rawRect;
		RectInt fullRect;
		public RectInt RawRect => rawRect;
		public RectPathfindingSpace(RectInt rect, bool allowDiagonal) : base(allowDiagonal ? 8 : 4)
		{
			rawRect = rect;
			widthPower = Mathf.CeilToInt(Mathf.Log(rect.width + 2, 2));
			var heightPower = Mathf.CeilToInt(Mathf.Log(rect.height + 2, 2));
			xMin = rect.xMin - 1;
			yMin = rect.yMin - 1;
			width = 1 << widthPower;
			var height = 1 << heightPower;
			fullRect = new(xMin, yMin, width, height);
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool ContainsPosition(Vector2Int position) => fullRect.Contains(position);
		protected override int GetLinks(int node, int[] toNodes, float[] basicCosts)
		{
			var count = 0;
			for (var direction = 0; direction < linkCount; ++direction)
			{
				var neighborIndex = GetNeighborIndexUnverified(node, direction);
				var index = (neighborIndex << linkBits) | direction;
				var cost = costs[index];
				if (cost <= 0) continue;
				toNodes[count] = neighborIndex;
				basicCosts[count] = cost;
				++count;
			}
			return count;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override int GetIndexUnverified(Vector2Int position)
		{
			var x = position.x - xMin;
			var y = position.y - yMin;
			return (y << widthPower) | x;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool ContainsKey(int key) => key >= 0 && key < nodeCount;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Vector2Int GetPositionUnverified(int index)
		{
			var x = index & (width - 1);
			var y = index >> widthPower;
			return new(x + xMin, y + yMin);
		}
		public RectPathfinder CreatePathfinder() => RectPathfinder.Create(this);
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
			costs[GetLinkIndex(fromIndex, direction)] = basicCost;
		}
		int GetLinkIndex(int fromNode, DirectionEnum direction)
		{
			var fromPosition = GetPosition(fromNode);
			if (!fullRect.Contains(fromPosition)) throw new ArgumentOutOfRangeException($"fromNode:{fromPosition}, direction:{direction}");
			var toPosition = direction switch
			{
				DirectionEnum.Up => fromPosition + Vector2Int.up,
				DirectionEnum.Right => fromPosition + Vector2Int.right,
				DirectionEnum.Down => fromPosition + Vector2Int.down,
				DirectionEnum.Left => fromPosition + Vector2Int.left,
				DirectionEnum.UpRight => fromPosition + Vector2Int.up + Vector2Int.right,
				DirectionEnum.DownRight => fromPosition + Vector2Int.down + Vector2Int.right,
				DirectionEnum.DownLeft => fromPosition + Vector2Int.down + Vector2Int.left,
				DirectionEnum.UpLeft => fromPosition + Vector2Int.up + Vector2Int.left,
				_ => throw new ArgumentOutOfRangeException(nameof(direction)),
			};
			if (!fullRect.Contains(toPosition)) throw new ArgumentOutOfRangeException($"rect:{rawRect} fullrect:{fullRect}, fromNode:{fromPosition}, direction:{direction}");
			return GetLinkIndexUnverified(fromNode, (int)direction);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int GetNeighborIndexUnverified(int fromNode, int direction) => fromNode + neighborIndexOffsetSequence[direction];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int GetLinkIndexUnverified(int fromNode, int direction) => (GetNeighborIndexUnverified(fromNode, direction) << linkBits) | direction;
		#region gizmos
		static int[] toNodeBuffer = new int[8];
		static float[] costBuffer = new float[8];
		public void DrawGizmos()
		{
			Gizmos.color = new(1, 1, 0, 0.2f);
			GizmosEx.DrawWiredRect(rawRect);
			Gizmos.color = new(1, 0, 0, 0.5f);
			GizmosEx.DrawWiredRect(fullRect);
			Gizmos.color = new(0, 1, 1, 0.1f);
			var offset = new Vector2(0.5f, 0.5f);
			foreach (var fromPos in rawRect.allPositionsWithin)
			{
				var fromIndex = GetIndexUnverified(fromPos);
				var count = GetLinks(fromIndex, toNodeBuffer ??= new int[maxLinkCountPerNode], costBuffer ??= new float[maxLinkCountPerNode]);
				for (var i = 0; i < count; ++i)
				{
					var toIndex = toNodeBuffer[i];
					var toPos = GetPositionUnverified(toIndex);
					var shortten = ((Vector2)(toPos - fromPos)).normalized * 0.3f;
					GizmosEx.DrawArrow(fromPos + offset + shortten, toPos + offset - shortten, 0.1f);
				}
			}
		}
		#endregion
	}
}
