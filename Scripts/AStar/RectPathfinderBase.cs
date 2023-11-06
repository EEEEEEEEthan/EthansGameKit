using System.Collections.Generic;
using EthansGameKit.Collections.Wrappers;
using UnityEngine;

namespace EthansGameKit.AStar
{
	public abstract class RectPathfinderBase : PathfindingSpace<Vector2Int, int>.Pathfinder
	{
		readonly struct IndexToPositionConverter : IValueConverter<int, Vector2Int>
		{
			readonly PathfindingSpace<Vector2Int, int> space;
			public IndexToPositionConverter(PathfindingSpace<Vector2Int, int> space) => this.space = space;
			public Vector2Int Convert(int oldItem) => space.GetPositionUnverified(oldItem);
			public int Recover(Vector2Int newItem) => space.GetIndexUnverified(newItem);
		}

		protected float[] costMap;
		protected int[] flowMap;
		IReadOnlyDictionary<Vector2Int, float> costDict;
		IReadOnlyDictionary<Vector2Int, Vector2Int> flowDict;
		IndexToPositionConverter converter;
		public override IReadOnlyDictionary<Vector2Int, float> CostMap => costDict;
		public override IReadOnlyDictionary<Vector2Int, Vector2Int> FlowMap => flowDict;
		protected override void OnInitialize()
		{
			var space = Space;
			costMap = new float[space.NodeCount];
			flowMap = new int[space.NodeCount];
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
}
