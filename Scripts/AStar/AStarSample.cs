using System;
using System.Collections.Generic;
using EthansGameKit.AStar.RectGrid;
using UnityEngine;

namespace EthansGameKit.AStar
{
	class AStarSample
	{
		async void Sample()
		{
			var space = new RectPathfindingSpace(new(0, 0, 12, 34), false);
			// 初始化寻路网
			// 为了达到最大效率,寻路网需要预先设置
			space.ClearLinks();
			space.SetLink(new(0, 0), RectPathfindingSpace.DirectionEnum.Right, 1);
			space.SetLink(new(0, 0), RectPathfindingSpace.DirectionEnum.Up, 1);
			space.SetLink(new(0, 1), RectPathfindingSpace.DirectionEnum.Right, 1);
			space.SetLink(new(0, 1), RectPathfindingSpace.DirectionEnum.Down, 1);
			// 生成pathfinder. pathfinder继承于IDisposable，可以在Dispose时回收至内存池
			using var pathfinder = space.CreatePathfinder<RectPathfinder>();
			{
				// 情景1: 移动至制定目标
				var target = new Vector2Int(0, 1);
				pathfinder.Reset(new Vector2Int(0, 0), target); // 启发坐标填目标，可以优先选择更近的路径。大部分情况能提升寻路效率
				while (true)
				{
					// 把MoveNext作为寻路的原子操作可以灵活分散计算量
					// 每一次MoveNext可以抛出这一步寻路进行的节点
					if (!pathfinder.MoveNext(out var nextPosition)) break; // 返回false表示寻路已经遍历完成
					if (nextPosition == target) break; // 到达目标
				}
				var path = pathfinder.GetPath(target);
				// path即路径
			}
			{
				// 情景2: 在附近随机移动
				// 随机找一个点移动过去，如果这个点不可达，将会遍历整个地图.
				// 可以先得到可达集，再随机取一个点
				pathfinder.Reset(new Vector2Int(0, 0), new(0, 0)); // 启发坐标填自己，可以形成一个均匀的dfs网
				var set = new HashSet<Vector2Int>();
				for (var i = 0; i < 20; ++i)
				{
					if (!pathfinder.MoveNext(out var nextStep)) break;
					set.Add(nextStep);
				}
				var destination = set.RandomPick();
				var path = pathfinder.GetPath(destination);
				// path即路径
			}
			{
				// 情景3: 亿万丧尸
				// 这种会预先把"流"预处理。
				var buildings = new List<Vector2Int> { new(0, 1) };
				// 把建筑当起点，反向寻路
				pathfinder.Reset(buildings, default);
				while (true)
				{
					if (!pathfinder.MoveNext(out _)) break;
				}
				// flowmap即每一个僵尸坐标想要去的下一个节点
				var zombiePosition = new Vector2Int(0, 0);
				var nextPosition = pathfinder.FlowMap[zombiePosition];
			}
			{
				// 情景4: 异步寻路
				var target = new Vector2Int(0, 1);
				pathfinder.Reset(new Vector2Int(0, 0), target);
				var time = DateTime.Now;
				while (true)
				{
					if (!pathfinder.MoveNext(out var nextPosition)) break;
					if (nextPosition == target) break; // 到达目标
					if (DateTime.Now - time > TimeSpan.FromMilliseconds(10)) // 计算太久了，等下一帧
					{
						time = DateTime.Now;
						goto NEXT_LOOP;
					}
					if (DateTime.Now < time) // 调时
					{
						time = DateTime.Now;
						goto NEXT_LOOP;
					}
					if (pathfinder.Expired) // 说明地图已经发生变化。可以考虑重新寻路或者先寻完再检查路径
					{
						// 这里选择重新寻路
						pathfinder.Reset(new Vector2Int(0, 0), target);
					}
				NEXT_LOOP:
					await TaskQueue.AwaitFreeFrame();
				}
				var path = pathfinder.GetPath(target);
			}
		}
	}
}
