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
	/// <typeparam name="TPosition">玩法节点</typeparam>
	/// <typeparam name="TKey">寻路空间的节点.与玩法节点唯一对应</typeparam>
	public abstract class PathfindingSpace<TPosition, TKey>
	{
		/// <summary>
		///     寻路器,对<see cref="PathfindingSpace{TPosition, TNode}" />具有强依赖
		/// </summary>
		public abstract class Pathfinder : IDisposable
		{
			readonly Heap<TKey, float> openList = Heap<TKey, float>.Generate();
			TKey[] nodeBuffer;
			float[] floatBuffer;
			float maxCost = float.PositiveInfinity;
			float maxHeuristic = float.PositiveInfinity;
			int changeFlag;
			public PathfindingSpace<TPosition, TKey> Space { get; internal set; }
			/// <summary>
			///     <para>过期.</para>
			///     <para>如果space被修改过，pathfinder就会过期</para>
			/// </summary>
			public bool Expired => changeFlag != Space.changeFlag;
			public abstract IReadOnlyDictionary<TPosition, float> CostMap { get; }
			public abstract IReadOnlyDictionary<TPosition, TPosition> FlowMap { get; }
			protected TPosition HeuristicTarget { get; private set; }
			/// <summary>
			///     回收资源
			/// </summary>
			public void Dispose()
			{
				Clear();
				Space.Recycle(this);
			}
			/// <summary>下一步寻路</summary>
			/// <param name="current">下一步检索的节点</param>
			/// <returns>true-寻路继续; false-寻路程序已经遍历了整个地图</returns>
			public bool MoveNext(out TPosition current)
			{
				if (MoveNext(out TKey node))
				{
					current = Space.GetPosition(node);
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
				changeFlag = Space.changeFlag;
				openList.Clear();
				OnClear();
			}
			/// <summary>尝试获取路径</summary>
			/// <param name="target">目标</param>
			/// <returns>
			///     <para>路径.目标先入栈,起点最后入栈</para>
			///     <para>若路径不存在,返回空.若起点与终点相同,栈长度为1</para>
			/// </returns>
			public Stack<TPosition> GetPath(TPosition target)
			{
				var flowMap = FlowMap;
				if (!flowMap.ContainsKey(target)) return null;
				var stack = StackPool<TPosition>.Generate();
				var node = target;
				stack.Push(target);
				for (var i = 0; i < 10000; ++i)
				{
					var next = flowMap[node];
					if (next.Equals(node)) return stack;
					stack.Push(next);
					node = next;
				}
				throw new("死循环");
			}
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
			protected abstract void OnInitialize();
			internal void OnConstruction()
			{
				changeFlag = Space.changeFlag;
				nodeBuffer = new TKey[Space.maxLinkCountPerNode];
				floatBuffer = new float[Space.maxLinkCountPerNode];
				OnInitialize();
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
				var linkCount = Space.GetLinks(current, nodeBuffer, floatBuffer);
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
			#region reinitialize
			readonly TPosition[] buffer_reinitialize = new TPosition[1];
			public void Reset(IEnumerable<TPosition> sources, TPosition heuristicTarget, float maxCost = float.MaxValue, float maxheuristic = float.MaxValue)
			{
				Clear();
				this.maxCost = maxCost;
				maxHeuristic = maxheuristic;
				HeuristicTarget = heuristicTarget;
				foreach (var source in sources)
				{
					var key = Space.GetKey(source);
					CacheParentNode(key, key);
					CacheTotalCost(key, 0);
					openList.AddOrUpdate(key, GetHeuristic(key));
				}
			}
			public void Reset(TPosition source, TPosition heuristicTarget, float maxCost = float.MaxValue, float maxheuristic = float.MaxValue)
			{
				HeuristicTarget = heuristicTarget;
				buffer_reinitialize[0] = source;
				Reset(buffer_reinitialize, heuristicTarget, maxCost, maxheuristic);
			}
			#endregion
		}

		protected readonly int maxLinkCountPerNode;
		readonly Dictionary<Type, CachePool<Pathfinder>> pools = new();
		int changeFlag;
		public abstract int NodeCount { get; }
		/// <summary>
		///     构造方法
		/// </summary>
		/// <param name="maxLinkCountPerNode">每个节点最大连接数量</param>
		protected PathfindingSpace(int maxLinkCountPerNode) => this.maxLinkCountPerNode = maxLinkCountPerNode;
		public T CreatePathfinder<T>() where T : Pathfinder
		{
			if (!GetPool(typeof(T)).TryGenerate(out var pathfinder))
				pathfinder = (Pathfinder)Activator.CreateInstance(typeof(T), true);
			pathfinder.Space = this;
			pathfinder.OnConstruction();
			return (T)pathfinder;
		}
		/// <summary>
		///     验证是否包含某节点
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract bool ContainsPosition(TPosition position);
		protected CachePool<Pathfinder> GetPool(Type pathfinderType)
		{
			if (!pools.TryGetValue(pathfinderType, out var pool))
				pool = pools[pathfinderType] = new(0);
			return pool;
		}
		protected CachePool<Pathfinder> GetPool<TPathfinder>() where TPathfinder : Pathfinder => GetPool(typeof(TPathfinder));
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
		/// <summary>
		///     将寻路节点转换为玩法节点
		/// </summary>
		/// <param name="key">寻路节点</param>
		/// <returns>玩法节点</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPosition GetPosition(TKey key)
		{
			if (!ContainsKey(key)) throw new ArgumentOutOfRangeException($"{key}");
			return GetPositionUnverified(key);
		}
		/// <summary>
		///     将玩法节点转换为寻路节点
		/// </summary>
		/// <param name="position">玩法节点</param>
		/// <returns>寻路节点</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TKey GetKey(TPosition position)
		{
			if (!ContainsPosition(position)) throw new ArgumentOutOfRangeException($"{position}");
			return GetIndexUnverified(position);
		}
		/// <summary>
		///     将寻路节点转换为玩法节点
		/// </summary>
		/// <remarks>这个方法不进行数据验证,以提高寻路计算速度</remarks>
		/// <param name="key">寻路节点</param>
		/// <returns>玩法节点</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract TPosition GetPositionUnverified(TKey key);
		/// <summary>
		///     将玩法节点转换为寻路节点
		/// </summary>
		/// <remarks>这个方法不进行数据验证,以提高寻路计算速度</remarks>
		/// <param name="position">玩法节点</param>
		/// <returns>寻路节点</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract TKey GetIndexUnverified(TPosition position);
		/// <summary>
		///     是否包含寻路节点
		/// </summary>
		/// <param name="key">寻路节点</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract bool ContainsKey(TKey key);
		void Recycle(Pathfinder pathfinder)
		{
			if (pathfinder.Space != this) throw new ArgumentException("pathfinder.space != this");
			var pool = GetPool(pathfinder.GetType());
			pool.Recycle(pathfinder);
		}
	}
}
