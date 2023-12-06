using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.AStar
{
	public static class DirectionEnumExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int ToVector2(this RectPathfindingSpace.DirectionEnum @this)
		{
			return @this switch
			{
				RectPathfindingSpace.DirectionEnum.Up => Vector2Int.up,
				RectPathfindingSpace.DirectionEnum.Right => Vector2Int.right,
				RectPathfindingSpace.DirectionEnum.Down => Vector2Int.down,
				RectPathfindingSpace.DirectionEnum.Left => Vector2Int.left,
				RectPathfindingSpace.DirectionEnum.UpRight => Vector2Int.up + Vector2Int.right,
				RectPathfindingSpace.DirectionEnum.DownRight => Vector2Int.down + Vector2Int.right,
				RectPathfindingSpace.DirectionEnum.DownLeft => Vector2Int.down + Vector2Int.left,
				RectPathfindingSpace.DirectionEnum.UpLeft => Vector2Int.up + Vector2Int.left,
				_ => throw new ArgumentOutOfRangeException(nameof(@this), @this, null),
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDiagonal(this RectPathfindingSpace.DirectionEnum @this)
		{
			return (int)@this >= 4;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RectPathfindingSpace.DirectionEnum Opposite(this RectPathfindingSpace.DirectionEnum @this)
		{
			return @this switch
			{
				RectPathfindingSpace.DirectionEnum.Up => RectPathfindingSpace.DirectionEnum.Down,
				RectPathfindingSpace.DirectionEnum.Right => RectPathfindingSpace.DirectionEnum.Left,
				RectPathfindingSpace.DirectionEnum.Down => RectPathfindingSpace.DirectionEnum.Up,
				RectPathfindingSpace.DirectionEnum.Left => RectPathfindingSpace.DirectionEnum.Right,
				RectPathfindingSpace.DirectionEnum.UpRight => RectPathfindingSpace.DirectionEnum.DownLeft,
				RectPathfindingSpace.DirectionEnum.DownRight => RectPathfindingSpace.DirectionEnum.UpLeft,
				RectPathfindingSpace.DirectionEnum.DownLeft => RectPathfindingSpace.DirectionEnum.UpRight,
				RectPathfindingSpace.DirectionEnum.UpLeft => RectPathfindingSpace.DirectionEnum.DownRight,
				_ => throw new ArgumentOutOfRangeException(nameof(@this), @this, null),
			};
		}
	}

	/// <summary>
	///     针对四邻域深度优化的寻路空间
	/// </summary>
	public sealed class RectPathfindingSpace : PathfindingSpace<Vector2Int, int>
	{
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

		const float unreachable = 0;
		public readonly bool allowDiagonal;
		public readonly RectInt rawRect;
		public readonly RectInt fullRect;
		readonly int width;
		readonly int xMin;
		readonly int yMin;
		readonly float[] costs;
		readonly int[] neighborIndexOffsetSequence;
		readonly int linkBits;
		readonly int linkCount;
		readonly int widthPower;
		public override int NodeCount { get; }
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
			NodeCount = width * height;
			linkBits = allowDiagonal ? 3 : 2;
			linkCount = 1 << linkBits;
			this.allowDiagonal = allowDiagonal;
			costs = new float[NodeCount << linkBits];
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
		public override int GetIndexUnverified(Vector2Int position)
		{
			var x = position.x - xMin;
			var y = position.y - yMin;
			return (y << widthPower) | x;
		}
		public override List<Vector2Int> GetLinkSources()
		{
			var result = ListPool<Vector2Int>.Generate();
			var toNodeBuffer = ArrayCachePool<int>.Generate(maxLinkCountPerNode);
			var costBuffer = ArrayCachePool<float>.Generate(maxLinkCountPerNode);
			foreach (var fromPos in rawRect.allPositionsWithin)
			{
				var fromIndex = GetIndexUnverified(fromPos);
				var count = GetLinks(fromIndex, toNodeBuffer, costBuffer);
				if (count > 0) result.Add(fromPos);
			}
			ArrayCachePool<int>.Recycle(ref toNodeBuffer);
			ArrayCachePool<float>.Recycle(ref costBuffer);
			return result;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Vector2Int GetPositionUnverified(int key)
		{
			var x = key & (width - 1);
			var y = key >> widthPower;
			return new(x + xMin, y + yMin);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool ContainsPosition(Vector2Int position)
		{
			return rawRect.Contains(position);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool ContainsKey(int key)
		{
			if (key < 0 || key >= NodeCount) return false;
			var position = GetPosition(key);
			return rawRect.Contains(position);
		}
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
		public void ClearLinks()
		{
			MarkChanged();
			costs.MemSet(unreachable);
		}
		public void RemoveLink(Vector2Int fromNode, DirectionEnum direction)
		{
			if (!ContainsPosition(fromNode)) throw new ArgumentOutOfRangeException();
			var fromIndex = GetKey(fromNode);
			MarkChanged();
			costs[GetLinkIndexUnverified(fromIndex, (int)direction)] = unreachable;
		}
		public void SetLink(Vector2Int fromNode, DirectionEnum direction, float basicCost)
		{
			if (!allowDiagonal && (int)direction >= 4) throw new ArgumentOutOfRangeException();
			MarkChanged();
			var fromIndex = GetKey(fromNode);
			costs[GetLinkIndex(fromIndex, direction)] = basicCost;
		}
		public float GetCost(Vector2Int fromNode, DirectionEnum direction)
		{
			var fromIndex = GetKey(fromNode);
			return costs[GetLinkIndex(fromIndex, direction)];
		}
		public float GetCost(Vector2Int fromNode, params DirectionEnum[] directions)
		{
			if (directions is null) return -1;
			if (!rawRect.Contains(fromNode)) return -1;
			var index = GetKey(fromNode);
			var result = 0f;
			foreach (var direction in directions)
			{
				if (!TryGetLinkIndex(index, direction, out var linkIndex)) return -1;
				var cost = costs[linkIndex];
				if (cost <= 0) return -1;
				result += cost;
				index = GetNeighborIndexUnverified(index, (int)direction);
			}
			return result;
		}
		bool TryGetLinkIndex(int fromNode, DirectionEnum direction, out int index)
		{
			if (!ContainsKey(fromNode))
			{
				index = 0;
				return false;
			}
			var fromPosition = GetPosition(fromNode);
			if (!fullRect.Contains(fromPosition))
			{
				index = default;
				return false;
			}
			Vector2Int toPosition;
			switch (direction)
			{
				case DirectionEnum.Up:
					toPosition = fromPosition + Vector2Int.up;
					break;
				case DirectionEnum.Right:
					toPosition = fromPosition + Vector2Int.right;
					break;
				case DirectionEnum.Down:
					toPosition = fromPosition + Vector2Int.down;
					break;
				case DirectionEnum.Left:
					toPosition = fromPosition + Vector2Int.left;
					break;
				case DirectionEnum.UpRight:
					toPosition = fromPosition + Vector2Int.up + Vector2Int.right;
					break;
				case DirectionEnum.DownRight:
					toPosition = fromPosition + Vector2Int.down + Vector2Int.right;
					break;
				case DirectionEnum.DownLeft:
					toPosition = fromPosition + Vector2Int.down + Vector2Int.left;
					break;
				case DirectionEnum.UpLeft:
					toPosition = fromPosition + Vector2Int.up + Vector2Int.left;
					break;
				default:
					index = default;
					return false;
			}
			if (!fullRect.Contains(toPosition))
			{
				index = default;
				return false;
			}
			index = GetLinkIndexUnverified(fromNode, (int)direction);
			return true;
		}
		int GetLinkIndex(int fromNode, DirectionEnum direction)
		{
			if (!ContainsKey(fromNode)) throw new ArgumentOutOfRangeException($"fromNode:{fromNode}, direction:{direction}");
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
		int GetNeighborIndexUnverified(int fromNode, int direction)
		{
			return fromNode + neighborIndexOffsetSequence[direction];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int GetLinkIndexUnverified(int fromNode, int direction)
		{
			return (GetNeighborIndexUnverified(fromNode, direction) << linkBits) | direction;
		}
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
