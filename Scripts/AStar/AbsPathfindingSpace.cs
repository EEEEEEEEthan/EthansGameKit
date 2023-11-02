using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EthansGameKit.CachePools;
using EthansGameKit.Collections;

namespace EthansGameKit.AStar
{
	/// <summary>
	///     寻路空间.用于保存节点之间的关系.搭配<see cref="Pathfinder" />进行寻路
	/// </summary>
	/// <typeparam name="TKey">与玩法节点对应的数据节点</typeparam>
	/// <typeparam name="TPosition">玩法节点</typeparam>
	public abstract class PathfindingSpace<TPosition, TKey>
	{
		/// <summary>
		///     寻路器,对<see cref="PathfindingSpace{TPosition, TNode}" />具有强依赖
		/// </summary>
		public abstract class Pathfinder : IDisposable
		{
			readonly Heap<TKey, float> openList = Heap<TKey, float>.Generate();
			readonly PathfindingSpace<TPosition, TKey> space;
			readonly TKey[] nodeBuffer;
			readonly float[] floatBuffer;
			float maxCost = float.PositiveInfinity;
			float maxHeuristic = float.PositiveInfinity;
			int changeFlag;
			/// <summary>
			///     <para>过期.</para>
			///     <para>如果space被修改过，pathfinder就会过期</para>
			/// </summary>
			public bool Expired => changeFlag != space.changeFlag;
			protected Pathfinder(PathfindingSpace<TPosition, TKey> space)
			{
				this.space = space;
				changeFlag = space.changeFlag;
				nodeBuffer = new TKey[space.maxLinkCountPerNode];
				floatBuffer = new float[space.maxLinkCountPerNode];
			}
			/// <summary>
			///     回收资源
			/// </summary>
			public void Dispose()
			{
				Clear();
				Recycle();
			}
			/// <summary>下一步寻路</summary>
			/// <param name="current">下一步检索的节点</param>
			/// <returns>true-寻路继续; false-寻路程序已经遍历了整个地图</returns>
			public bool MoveNext(out TPosition current)
			{
				if (MoveNext(out TKey node))
				{
					current = space.GetPosition(node);
					return true;
				}
				current = default;
				return false;
			}
			/// <summary>下一步寻路</summary>
			/// <returns>true-寻路继续; false-寻路程序已经遍历了整个地图</returns>
			public bool MoveNext() => MoveNext(out TKey _);
			/// <summary>
			///     清理缓存
			/// </summary>
			public void Clear()
			{
				changeFlag = space.changeFlag;
				openList.Clear();
				OnClear();
			}
			/// <summary>
			///     回收至PathfindingSpace
			/// </summary>
			protected abstract void Recycle();
			/// <summary>启发函数</summary>
			/// <remarks>调用频繁.如果计算量大，子类应当自行缓存</remarks>
			/// <param name="node">节点坐标</param>
			/// <returns>启发值,越小越优先</returns>
			protected abstract float GetHeuristic(TKey node);
			/// <summary>获取单步消耗</summary>
			/// <remarks>
			///     <para>基类调用保证存在从<paramref name="fromNode" />到<paramref name="toNode" />的连接.</para>
			///     <para>调用频繁.如果计算量大，子类应当自行缓存</para>
			/// </remarks>
			/// <param name="fromNode">单步起点</param>
			/// <param name="toNode">单步终点</param>
			/// <param name="basicCost">基础消耗值</param>
			/// <returns>计算过不同寻路器加成的单步消耗 例如toNode是水，不会游泳的动物返回float.MaxValue等</returns>
			protected abstract float GetStepCost(TKey fromNode, TKey toNode, float basicCost);
			/// <summary>获取移动至这个位置的总消耗</summary>
			/// <returns>true-有记录; false-还没有记录</returns>
			protected abstract bool GetCachedTotalCost(TKey node, out float cost);
			/// <summary>缓存移动至这个位置的总消耗</summary>
			protected abstract void CacheTotalCost(TKey node, float cost);
			/// <summary>获取移动至这个位置的母节点</summary>
			/// <returns>true-有记录; false-还没有记录</returns>
			protected abstract bool GetCachedParentNode(TKey node, out TKey parent);
			/// <summary>缓存移动至这个位置的母节点</summary>
			protected abstract void CacheParentNode(TKey node, TKey parent);
			/// <summary>
			///     清理寻路缓存。在开始新的寻路时会触发
			/// </summary>
			protected abstract void OnClear();
			/// <summary>
			///     重新初始化,准备下一次寻路
			/// </summary>
			/// <param name="maxCost">最大消耗</param>
			/// <param name="maxHeuristic">最大启发值</param>
			/// <param name="sources">起点列表</param>
			protected void Reinitialize(
				float maxCost,
				float maxHeuristic,
				IEnumerable<TKey> sources
			)
			{
				this.maxCost = maxCost;
				this.maxHeuristic = maxHeuristic;
				Clear();
				foreach (var node in sources)
				{
					CacheParentNode(node, node);
					CacheTotalCost(node, 0);
					openList.AddOrUpdate(node, GetHeuristic(node));
				}
			}
			/// <summary>尝试获取路径</summary>
			/// <param name="target">目标</param>
			/// <param name="path">
			///     <para>一个栈表示路径。终点先入栈，起点最后入栈</para>
			///     <para>若路径不存在，得到null</para>
			///     <para>若起点终点相同，则长度为1</para>
			/// </param>
			/// <returns>true-路径存在; false-路径不存在</returns>
			protected bool TryGetPath(TKey target, out Stack<TKey> path)
			{
				if (!GetCachedParentNode(target, out var fromNode))
				{
					path = null;
					return false;
				}
				path = StackPool<TKey>.Generate();
				path.Push(target);
				if (fromNode.Equals(target)) return true;
				while (true)
				{
					path.Push(fromNode);
					GetCachedParentNode(fromNode, out var parent);
					if (parent.Equals(fromNode)) break;
					fromNode = parent;
				}
				return true;
			}
			bool MoveNext(out TKey current)
			{
				if (openList.Count <= 0)
				{
					current = default;
					return false;
				}
				current = openList.Pop();
				GetCachedTotalCost(current, out var currentCost);
				var linkCount = space.GetLinks(current, nodeBuffer, floatBuffer);
				var toNodes = nodeBuffer;
				var basicCosts = floatBuffer;
				for (var i = linkCount; i-- > 0;)
				{
					var toNode = toNodes[i];
					var basicCost = basicCosts[i];
					var stepCost = GetStepCost(current, toNode, basicCost);
					if (stepCost is float.PositiveInfinity or <= 0) continue;
					var newCost = currentCost + stepCost;
					if (newCost > maxCost) continue;
					var heuristic = GetHeuristic(toNode);
					if (heuristic > maxHeuristic) continue;
					if (!GetCachedTotalCost(toNode, out var oldCost) || newCost < oldCost)
					{
						CacheTotalCost(toNode, newCost);
						CacheParentNode(toNode, current);
						openList.AddOrUpdate(toNode, newCost + heuristic);
					}
				}
				return true;
			}
		}

		protected readonly int maxLinkCountPerNode;
		int changeFlag;
		/// <summary>
		///     构造方法
		/// </summary>
		/// <param name="maxLinkCountPerNode">每个节点最大连接数量</param>
		protected PathfindingSpace(int maxLinkCountPerNode) => this.maxLinkCountPerNode = maxLinkCountPerNode;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract bool ContainsPosition(TPosition position);
		protected void MarkChanged() => ++changeFlag;
		/// <summary>
		///     获取这个点的所有连接
		/// </summary>
		/// <param name="node"></param>
		/// <param name="toNodes">可移动至节点</param>
		/// <param name="basicCosts">这一步花费的基础消耗</param>
		/// <returns>连接数量</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract int GetLinks(TKey node, TKey[] toNodes, float[] basicCosts);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected TPosition GetPosition(TKey key)
		{
			if (!ContainsKey(key)) throw new ArgumentOutOfRangeException($"{key}");
			return GetPositionUnverified(key);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected TKey GetIndex(TPosition position)
		{
			if (!ContainsPosition(position)) throw new ArgumentOutOfRangeException($"{position}");
			return GetIndexUnverified(position);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract TPosition GetPositionUnverified(TKey node);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract TKey GetIndexUnverified(TPosition position);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract bool ContainsKey(TKey key);
	}
}
