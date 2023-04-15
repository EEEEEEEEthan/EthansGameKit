using System;
using System.Collections;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;

namespace EthansGameKit.Pathfinding2D
{
	public abstract class PathfindingSpace
	{
		static readonly int[] inversedSequence = { 2, 3, 0, 1, 6, 7, 4, 5 };
		public readonly int widthPower;
		public readonly int width;
		readonly int allTileCount;
		readonly int safeMax;
		readonly CachePool<float[]> floatArrayPool = new(0);
		readonly CachePool<int[]> intArrayPool = new(0);
		readonly CachePool<BitArray> bitArrayPool = new(0);
		readonly float[] costs;
		readonly int directions;
		readonly int[] neighborSequence;
		internal IReadOnlyList<int> NeighborSequence => neighborSequence;
		protected PathfindingSpace(int widthPower)
		{
			this.widthPower = widthPower;
			width = 1 << widthPower;
			safeMax = width - 1;
			allTileCount = width * width;
			costs = new float[allTileCount];
			neighborSequence = new[]
			{
				width, // N
				1, // E
				-width, // S
				-1, // W
				width + 1, // NE
				-width + 1, // SE
				-width - 1, // SW
				width - 1, // NW
			};
		}
		/// <summary>
		///     计算四邻消耗
		/// </summary>
		/// <param name="index"></param>
		/// <param name="fromDirection">仅可能是四邻方向</param>
		/// <returns></returns>
		protected abstract float CalculateQuadCost(int index, PathfindingDirectionEnum fromDirection);
		public float[] GenerateFloatArray()
		{
			if (!floatArrayPool.TryGenerate(out var array))
				array = new float[allTileCount];
			return array;
		}
		public void RecycleFloatArray(ref float[] array)
		{
			floatArrayPool.Recycle(ref array);
		}
		public int[] GenerateIntArray()
		{
			if (!intArrayPool.TryGenerate(out var array))
				array = new int[allTileCount];
			return array;
		}
		public void RecycleIntArray(ref int[] array)
		{
			intArrayPool.Recycle(ref array);
		}
		public BitArray GenerateBitArray()
		{
			if (!bitArrayPool.TryGenerate(out var bitArray))
				bitArray = new(allTileCount);
			return bitArray;
		}
		public void RecycleBitArray(ref BitArray bitArray)
		{
			bitArrayPool.Recycle(ref bitArray);
		}
		internal void ClearCost(int x, int z)
		{
			ClearCost(GetIndex(x, z));
		}
		internal float GetCost(int toIndex, int fromDirection)
		{
			var index = (toIndex << 3) | fromDirection;
			if (index > costs.Length)
			{
				throw new($"index {index} out of range {costs.Length}. toIndex={toIndex}");
			}
			var cost = costs[index];
			if (cost <= 0)
			{
				GetPosition(toIndex, out var x, out var z);
				if (InSafeArea(x, z))
				{
					try
					{
						cost = CalculateOctCost(toIndex, (PathfindingDirectionEnum)fromDirection);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
						cost = float.MaxValue;
					}
					if (cost <= 0)
					{
						Debug.LogError($"unexpected cost {cost}");
						cost = float.MaxValue;
					}
				}
				else
				{
					cost = float.MaxValue;
				}
				SetCost(toIndex, fromDirection, cost);
			}
			return cost;
		}
		protected int GetIndex(int x, int z)
		{
			return (z << widthPower) | x;
		}
		protected void GetPosition(int index, out int x, out int z)
		{
			x = index & (width - 1);
			z = index >> widthPower;
		}
		protected void ClearCost(int index)
		{
			for (var i = 0; i < directions; ++i)
			{
				var dir = (PathfindingDirectionEnum)i;
				var oppositeDir = dir.Opposite();
				SetCost(index, i, 0);
				var fromIndex = GetFromIndex(oppositeDir, index);
				SetCost(fromIndex, (int)oppositeDir, 0);
			}
		}
		protected bool InSafeArea(int x, int z)
		{
			return x > 0 && x < safeMax && z > 0 && z < safeMax;
		}
		float CalculateOctCost(int toIndex, PathfindingDirectionEnum fromDirection)
		{
			if ((int)fromDirection < 4)
			{
				return CalculateQuadCost(toIndex, fromDirection);
			}
			const double sqrt2 = 1.4142135623730950488016887242097;
			double cost1, cost2;
			{
				var toDirection = fromDirection.Opposite();
				var dirA = toDirection.Next();
				var dirB = toDirection.Previous();
				var midIndex = GetFromIndex(dirA, toIndex);
				cost1 = (double)GetCost(midIndex, (int)dirA.Opposite()) + GetCost(toIndex, (int)dirB.Opposite());
			}
			{
				var toDirection = fromDirection.Opposite();
				var dirA = toDirection.Next();
				var dirB = toDirection.Previous();
				var midIndex = GetFromIndex(dirB, toIndex);
				cost2 = (double)GetCost(midIndex, (int)dirB.Opposite()) + GetCost(toIndex, (int)dirA.Opposite());
			}
			var cost = Math.Max(cost1, cost2) * 0.5 * sqrt2;
			return cost >= float.MaxValue ? float.MaxValue : (float)cost;
		}
		int GetFromIndex(PathfindingDirectionEnum toDirection, int toIndex)
		{
			var fromOffset = neighborSequence[inversedSequence[(int)toDirection.Opposite()]];
			return toIndex + fromOffset;
		}
		void SetCost(int tileIndex, int direction, float cost)
		{
			var index = (tileIndex << 3) | direction;
			costs[index] = cost;
		}
	}
}
