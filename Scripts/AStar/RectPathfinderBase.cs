using System.Collections;
using System.Collections.Generic;
using EthansGameKit.Collections.Wrappers;
using UnityEngine;

namespace EthansGameKit.AStar
{
	public abstract class RectPathfinderBase : PathfindingSpace<Vector2Int, int>.Pathfinder
	{
		class CostDict : IReadOnlyDictionary<Vector2Int, float>
		{
			readonly RectPathfinderBase pathfinder;
			public int Count => pathfinder.space.NodeCount;
			public IEnumerable<Vector2Int> Keys => throw new System.NotImplementedException();
			public IEnumerable<float> Values => throw new System.NotImplementedException();
			public CostDict(RectPathfinderBase pathfinder) => this.pathfinder = pathfinder;
			public IEnumerator<KeyValuePair<Vector2Int, float>> GetEnumerator()
			{
				throw new System.NotImplementedException();
			}
			public bool ContainsKey(Vector2Int key)
			{
				return pathfinder.space.ContainsPosition(key);
			}
			public bool TryGetValue(Vector2Int key, out float value)
			{
				throw new System.NotImplementedException();
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			public float this[Vector2Int key] => throw new System.NotImplementedException();
		}

		readonly struct IndexToPositionConverter : IValueConverter<int, Vector2Int>
		{
			readonly PathfindingSpace<Vector2Int, int> space;
			public IndexToPositionConverter(PathfindingSpace<Vector2Int, int> space) => this.space = space;
			public Vector2Int Convert(int oldItem)
			{
				return space.GetPositionUnverified(oldItem);
			}
			public int Recover(Vector2Int newItem)
			{
				return space.GetIndexUnverified(newItem);
			}
		}

		public new readonly RectPathfindingSpace space;
		protected float[] costMap;
		protected int[] flowMap;
		IReadOnlyDictionary<Vector2Int, float> costDict;
		IReadOnlyDictionary<Vector2Int, Vector2Int> flowDict;
		IndexToPositionConverter converter;
		public override IReadOnlyDictionary<Vector2Int, float> CostMap => costDict;
		public override IReadOnlyDictionary<Vector2Int, Vector2Int> FlowMap => flowDict;
		protected RectPathfinderBase(RectPathfindingSpace space) : base(space) => this.space = space;
		protected override void OnInitialize()
		{
			var space = base.space;
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
