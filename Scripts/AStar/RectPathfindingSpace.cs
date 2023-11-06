using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.Collections.Wrappers;
using UnityEngine;

namespace EthansGameKit.AStar
{
	/// <summary>
	///     针对四邻域深度优化的寻路空间
	/// </summary>
	public sealed class RectPathfindingSpace : PathfindingSpace<Vector2Int, int>
	{
		public abstract class RectPathfinderBase : Pathfinder
		{
			readonly struct IndexToPositionConverter : IValueConverter<int, Vector2Int>
			{
				readonly RectPathfindingSpace space;
				public IndexToPositionConverter(RectPathfindingSpace space) => this.space = space;
				public Vector2Int Convert(int oldItem) => space.GetPositionUnverified(oldItem);
				public int Recover(Vector2Int newItem) => space.GetIndexUnverified(newItem);
			}

			protected RectPathfindingSpace space;
			protected float[] costMap;
			protected int[] flowMap;
			IReadOnlyDictionary<Vector2Int, float> costDict;
			IReadOnlyDictionary<Vector2Int, Vector2Int> flowDict;
			IndexToPositionConverter converter;
			public override IReadOnlyDictionary<Vector2Int, float> CostMap => costDict;
			public override IReadOnlyDictionary<Vector2Int, Vector2Int> FlowMap => flowDict;
			protected override void OnInitialize()
			{
				space = (RectPathfindingSpace)Space;
				costMap = new float[space.nodeCount];
				flowMap = new int[space.nodeCount];
				converter = new(space);
				{
					var wrappedList = costMap.WrapAsDictionary();
					costDict = wrappedList.WrapAsConvertedDictionary(converter, IValueConverter<float, float>.Default);
				}
				{
					var flowDict = flowMap.WrapAsDictionary();
					var dict = flowDict.WrapAsConvertedDictionary(converter, converter);
					this.flowDict = dict.WrapAsFilteredDictionary(k => flowMap[space.GetIndexUnverified(k)] >= 0);
				}
				Clear();
			}
			protected override void OnClear()
			{
				costMap.MemSet(float.MaxValue);
				flowMap.MemSet(-1);
			}
		}

		public sealed class RectPathfinder : RectPathfinderBase
		{
			RectPathfinder()
			{
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected override float GetHeuristic(int node)
			{
				var position = space.GetPositionUnverified(node);
				return (position - HeuristicTarget).sqrMagnitude;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected override float GetStepCost(int fromNode, int toNode, float basicCost) => basicCost;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected override bool GetCachedTotalCost(int node, out float cost)
			{
				cost = costMap[node];
				return true;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected override void CacheTotalCost(int node, float cost) => costMap[node] = cost;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected override bool GetCachedParentNode(int node, out int parent) => (parent = flowMap[node]) >= 0;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected override void CacheParentNode(int node, int parent) => flowMap[node] = parent;
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

		readonly int nodeCount;
		readonly int widthPower;
		readonly int width;
		readonly int xMin;
		readonly int yMin;
		readonly float[] costs;
		readonly int[] neighborIndexOffsetSequence;
		readonly int linkBits;
		readonly int linkCount;
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		protected override Vector2Int GetPositionUnverified(int key)
		{
			var x = key & (width - 1);
			var y = key >> widthPower;
			return new(x + xMin, y + yMin);
		}
		public void ClearLinks()
		{
			MarkChanged();
			costs.MemSet(0);
		}
		public void RemoveLink(Vector2Int fromNode, DirectionEnum direction)
		{
			MarkChanged();
			var fromIndex = GetKey(fromNode);
			costs[GetLinkIndexUnverified(fromIndex, (int)direction)] = 0;
		}
		public void SetLink(Vector2Int fromNode, DirectionEnum direction, float basicCost)
		{
			MarkChanged();
			var fromIndex = GetKey(fromNode);
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
		/// <summary>
		///     <para>gizmos</para>
		///     <para>红框: 内存区域</para>
		///     <para>黄框: 地图区域</para>
		///     <para>蓝线: 有效连接</para>
		/// </summary>
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
