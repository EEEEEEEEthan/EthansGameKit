using System;
using System.Collections;
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
			struct CostDict : IReadOnlyDictionary<Vector2Int, float>
			{
				readonly RectPathfinder pathfinder;
				readonly float[] rawArray;
				public IEnumerable<Vector2Int> Keys => throw new NotImplementedException();
				public IEnumerable<float> Values => throw new NotImplementedException();
				public int Count => throw new NotImplementedException();
				public CostDict(RectPathfinder pathfinder, float[] rawArray)
				{
					this.pathfinder = pathfinder;
					this.rawArray = rawArray;
				}
				public bool ContainsKey(Vector2Int key) => pathfinder.space.rawRect.Contains(key);
				public bool TryGetValue(Vector2Int key, out float value)
				{
					if (ContainsKey(key))
					{
						value = this[key];
						return true;
					}
					value = default;
					return true;
				}
				public IEnumerator<KeyValuePair<Vector2Int, float>> GetEnumerator() => throw new NotImplementedException();
				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
				public float this[Vector2Int position] => rawArray[pathfinder.space.GetIndex(position)];
			}

			readonly struct FlowDict : IReadOnlyDictionary<Vector2Int, Vector2Int>
			{
				readonly RectPathfinder pathfinder;
				readonly int[] rawArray;
				public IEnumerable<Vector2Int> Keys
				{
					get
					{
						var space = pathfinder.space;
						for (var i = rawArray.Length; i-- > 0;)
						{
							var from = space.GetPositionUnverified(i);
							var toIndex = rawArray[i];
							if (toIndex < 0) continue;
							yield return from;
						}
					}
				}
				public IEnumerable<Vector2Int> Values
				{
					get
					{
						var space = pathfinder.space;
						for (var i = rawArray.Length; i-- > 0;)
						{
							var toIndex = rawArray[i];
							if (toIndex < 0) continue;
							var to = space.GetPositionUnverified(rawArray[i]);
							yield return to;
						}
					}
				}
				public int Count => throw new NotImplementedException();
				public FlowDict(RectPathfinder pathfinder, int[] rawArray)
				{
					this.pathfinder = pathfinder;
					this.rawArray = rawArray;
				}
				public bool ContainsKey(Vector2Int key)
				{
					if (!pathfinder.space.rawRect.Contains(key)) return false;
					var index = pathfinder.space.GetIndex(key);
					return rawArray[index] >= 0;
				}
				public bool TryGetValue(Vector2Int key, out Vector2Int value)
				{
					if (ContainsKey(key))
					{
						value = this[key];
						return true;
					}
					value = default;
					return true;
				}
				public IEnumerator<KeyValuePair<Vector2Int, Vector2Int>> GetEnumerator()
				{
					var space = pathfinder.space;
					for (var i = rawArray.Length; i-- > 0;)
					{
						var from = space.GetPositionUnverified(i);
						var toIndex = rawArray[i];
						if (toIndex < 0) continue;
						var to = space.GetPositionUnverified(rawArray[i]);
						yield return new(from, to);
					}
				}
				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
				public Vector2Int this[Vector2Int position] => pathfinder.space.GetPosition(rawArray[pathfinder.space.GetIndex(position)]);
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
			public IReadOnlyDictionary<Vector2Int, float> CostMap => new CostDict(this, costMap);
			public IReadOnlyDictionary<Vector2Int, Vector2Int> FlowMap => new FlowDict(this, flowMap);
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
			costs[GetLinkIndex(fromIndex, direction)] = basicCost;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(Vector2Int position)
		{
			if (!fullRect.Contains(position)) throw new ArgumentOutOfRangeException(nameof(position));
			return GetIndexUnverified(position);
		}
		public bool TryGetIndex(Vector2Int position, out int index)
		{
			if (!fullRect.Contains(position))
			{
				index = default;
				return false;
			}
			index = GetIndexUnverified(position);
			return true;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int GetPosition(int index)
		{
			if (index > nodeCount) throw new ArgumentOutOfRangeException(nameof(index));
			var position = GetPositionUnverified(index);
			if (!fullRect.Contains(position)) throw new ArgumentOutOfRangeException($"{nameof(index)}:{index}");
			return position;
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
