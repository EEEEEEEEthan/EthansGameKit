using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public readonly struct GridIndexCalculator
	{
		public readonly int widthPower;
		public readonly int heightPower;
		public readonly int width;
		public readonly int xMin;
		public readonly int yMin;
		public readonly int xMax;
		public readonly int yMax;
		public readonly int count;
		readonly int widthMask;
		public GridIndexCalculator(RectInt rect)
		{
			widthPower = Mathf.CeilToInt(Mathf.Log(rect.width, 2));
			heightPower = Mathf.CeilToInt(Mathf.Log(rect.height, 2));
			width = 1 << widthPower;
			var height = 1 << heightPower;
			xMin = rect.xMin;
			yMin = rect.yMin;
			xMax = rect.xMax;
			yMax = rect.yMax;
			widthMask = width - 1;
			count = width * height;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(int index)
		{
			return index >= 0 && index < count;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(int x, int y)
		{
			return x >= xMin && x < xMax && y >= yMin && y < yMax;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector2Int position)
		{
			return Contains(position.x, position.y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexUnverified(int x, int y)
		{
			return ((y - yMin) << widthPower) | (x - xMin);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexUnverified(Vector2Int position)
		{
			return GetIndexUnverified(position.x, position.y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(int x, int y)
		{
			if (!Contains(x, y)) throw new ArgumentOutOfRangeException($"({x}, {y}) is not in the grid.");
			return GetIndexUnverified(x, y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(Vector2Int position)
		{
			return GetIndex(position.x, position.y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetXUnverified(int index)
		{
			return (index & widthMask) + xMin;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetX(int index)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			return GetXUnverified(index);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetYUnverified(int index)
		{
			return (index >> widthPower) + yMin;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetY(int index)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			return GetYUnverified(index);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetPositionUnverified(int index, out int x, out int y)
		{
			x = GetXUnverified(index);
			y = GetYUnverified(index);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int GetPositionUnverified(int index)
		{
			return new(GetXUnverified(index), GetYUnverified(index));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetPosition(int index, out int x, out int y)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			GetPositionUnverified(index, out x, out y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int GetPosition(int index)
		{
			GetPosition(index, out var x, out var y);
			return new(x, y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetNeighborIndexUnverified(int index, GridDirections direction)
		{
			return direction switch
			{
				GridDirections.Forward => index + width,
				GridDirections.ForwardRight => index + width + 1,
				GridDirections.Right => index + 1,
				GridDirections.BackwardRight => index - width + 1,
				GridDirections.Backward => index - width,
				GridDirections.BackwardLeft => index - width - 1,
				GridDirections.Left => index - 1,
				GridDirections.ForwardLeft => index + width - 1,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
			};
		}
	}
}
