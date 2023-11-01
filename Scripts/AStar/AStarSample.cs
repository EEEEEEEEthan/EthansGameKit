using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.AStar
{
	class AStarSample
	{
		void Sample()
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
			using var pathfinder = space.CreatePathfinder();
			{
				// 情景1: 移动至制定目标
				var target = new Vector2Int(0, 1);
				pathfinder.Reinitialize(new Vector2Int(0, 0), target); // 启发坐标填目标，可以优先选择更近的路径。大部分情况能提升寻路效率
				while (true)
				{
					// 把MoveNext作为寻路的原子操作可以灵活分散计算量
					// 每一次MoveNext可以抛出这一步寻路进行的节点
					if (!pathfinder.MoveNext(out var nextStep)) break; // 返回false表示寻路已经遍历完成
					var nextPosition = space.GetPosition(nextStep);
					if (nextPosition == target) break; // 到达目标
				}
				pathfinder.TryGetPath(target, out var path);
				// path即路径
			}
			{
				// 情景2: 在附近随机移动
				// 随机找一个点移动过去，如果这个点不可达，将会遍历整个地图.
				// 可以先得到可达集，再随机取一个点
				pathfinder.Reinitialize(new Vector2Int(0, 0), new(0, 0)); // 启发坐标填自己，可以形成一个均匀的dfs网
				var set = new HashSet<Vector2Int>();
				for (var i = 0; i < 20; ++i)
				{
					if (!pathfinder.MoveNext(out var nextStep)) break;
					set.Add(space.GetPosition(nextStep));
				}
				var destination = set.RandomPick();
				pathfinder.TryGetPath(destination, out var path);
				// path即路径
			}
			{
				// 情景3: 亿万丧尸
				// 这种会预先把"流"预处理。
				var buildings = new List<Vector2Int> { new(0, 1) };
				// 把建筑当起点，反向寻路
				pathfinder.Reinitialize(buildings, default);
				while (true)
				{
					if (!pathfinder.MoveNext(out _)) break;
				}
				// flowmap即每一个僵尸坐标想要去的下一个节点
				var zombiePosition = new Vector2Int(0, 0);
				var nextPosition = pathfinder.FlowMap[zombiePosition];
			}
		}
	}
}
