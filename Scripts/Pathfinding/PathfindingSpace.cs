using System;
using System.Collections.Generic;
using EthansGameKit.Collections;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	[Serializable]
	public abstract class PathfindingSpace
	{
		public abstract class Pathfinder
		{
			public readonly PathfindingSpace pathfindingSpace;
			readonly Heap<int, float> openList = Heap<int, float>.Generate();
			readonly StepInfo[] stepBuffer;
			int changeFlag;
			bool stopped;
			/// <summary>
			///     从寻路开始到当前，地形是否被修改过
			/// </summary>
			public bool Expired => changeFlag != pathfindingSpace.changeFlag;
			/// <summary>
			///     寻路器支持多线程.禁止子类对<see cref="pathfindingSpace" />做写操作
			/// </summary>
			/// <param name="pathfindingSpace"></param>
			/// <param name="maxLinkPerNode"></param>
			protected Pathfinder(PathfindingSpace pathfindingSpace, int maxLinkPerNode)
			{
				this.pathfindingSpace = pathfindingSpace;
				stepBuffer = new StepInfo[maxLinkPerNode];
			}
			public bool Next(out int nodeId)
			{
				if (openList.Count <= 0)
				{
					nodeId = -1;
					stopped = true;
					return false;
				}
				nodeId = openList.Pop();
				TryGetFromInfo(
					nodeId: nodeId,
					fromNodeId: out _,
					totalCost: out var costed
				);
				var count = pathfindingSpace.GetLinks(nodeId, stepBuffer);
				for (var i = 0; i < count; ++i)
				{
					var stepInfo = stepBuffer[i];
					var nextNodeId = stepInfo.toNodeId;
					var expectedCostIfIStepHere = costed + stepInfo.stepCost;
					if (!TryGetFromInfo(nextNodeId, out _, out var oldCostIfIStepHere) || expectedCostIfIStepHere < oldCostIfIStepHere)
					{
						SetFromInfo(nodeId, nextNodeId, expectedCostIfIStepHere);
						openList.AddOrUpdate(nextNodeId, expectedCostIfIStepHere);
					}
				}
				return true;
			}
			/// <summary>
			///     获取某点到周围点的连接
			/// </summary>
			/// <remarks>
			///     有必要时，例如假设这里有一个阻挡物，对这种假设的情况进行寻路.此时可以override这个方法
			/// </remarks>
			/// <param name="nodeId"></param>
			/// <param name="toAndCost"></param>
			/// <returns></returns>
			protected virtual int GetLinks(int nodeId, StepInfo[] toAndCost)
			{
				return pathfindingSpace.GetLinks(nodeId, toAndCost);
			}
			/// <summary>
			///     获取从终点(含)到起点(含)的路径
			/// </summary>
			/// <param name="buffer">0号元素是寻路终点,末尾元素是寻路起点</param>
			/// <param name="nodeId">寻路终点</param>
			/// <returns>路径长度.若长度为0表示暂未找到路径，可以考虑继续调用<see cref="Next" />;若长度为-1表示不可达</returns>
			/// <exception cref="Exception"></exception>
			protected int GetPath(ref int[] buffer, int nodeId)
			{
				var count = 0;
				while (true)
				{
					if (!TryGetFromInfo(nodeId, out var fromNodeId, out _))
						return stopped ? -1 : 0;
					buffer[count++] = nodeId;
					if (nodeId == fromNodeId) return count;
					if (count > 10000) throw new("寻路失败");
				}
			}
			protected void Reset(IReadOnlyList<int> startNodes)
			{
				stopped = false;
				openList.Clear();
				Clear();
				changeFlag = pathfindingSpace.ChangeFlag;
				for (var i = startNodes.Count; i-- > 0;)
				{
					openList.AddOrUpdate(startNodes[i], 0);
				}
			}
			protected void Reset(IEnumerable<int> startNodes)
			{
				stopped = false;
				openList.Clear();
				Clear();
				changeFlag = pathfindingSpace.ChangeFlag;
				foreach (var startNode in startNodes)
				{
					openList.AddOrUpdate(startNode, 0);
					SetFromInfo(startNode, startNode, 0);
				}
			}
			protected abstract void Clear();
			protected abstract bool TryGetFromInfo(int nodeId, out int fromNodeId, out float totalCost);
			protected abstract void SetFromInfo(int fromNodeId, int toNodeId, float totalCost);
		}

		[SerializeField] int changeFlag;
		public int ChangeFlag => changeFlag;
		protected void MarkChanged()
		{
			++changeFlag;
		}
		protected abstract int GetLinks(int fromNodeId, StepInfo[] toAndCost);
	}
}
