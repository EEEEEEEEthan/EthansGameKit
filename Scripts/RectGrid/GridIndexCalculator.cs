using System;
using System.Runtime.CompilerServices;
using EthansGameKit.Pathfinding.Rect;
using UnityEngine;

namespace EthansGameKit.RectGrid
{
	public readonly struct GridIndexCalculator
	{
		public readonly int WidthPower;
		public readonly int HeightPower;
		public readonly int Width;
		public readonly int XMin;
		public readonly int YMin;
		public readonly int XMax;
		public readonly int YMax;
		public readonly int Count;
		readonly int widthMask;

		public GridIndexCalculator(RectInt rect)
		{
			WidthPower = Mathf.CeilToInt(Mathf.Log(rect.width, 2));
			HeightPower = Mathf.CeilToInt(Mathf.Log(rect.height, 2));
			Width = 1 << WidthPower;
			var height = 1 << HeightPower;
			XMin = rect.xMin;
			YMin = rect.yMin;
			XMax = rect.xMax;
			YMax = rect.yMax;
			widthMask = Width - 1;
			Count = Width * height;
		}

		public GridIndexCalculator(int widthPower) : this(new RectInt(0, 0, 1 << widthPower, 1 << widthPower))
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(int index) => index >= 0 && index < Count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(int x, int y) => x >= XMin && x < XMax && y >= YMin && y < YMax;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector2Int position) => Contains(position.x, position.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexUnverified(int x, int y) => ((y - YMin) << WidthPower) | (x - XMin);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexUnverified(Vector2Int position) => GetIndexUnverified(position.x, position.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(int x, int y)
		{
			if (!Contains(x, y)) throw new ArgumentOutOfRangeException($"({x}, {y}) is not in the grid.");
			return GetIndexUnverified(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(Vector2Int position) => GetIndex(position.x, position.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetIndex(int x, int y, out int index)
		{
			if (Contains(x, y))
			{
				index = GetIndexUnverified(x, y);
				return true;
			}
			index = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetIndex(Vector2Int position, out int index) => TryGetIndex(position.x, position.y, out index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetXUnverified(int index) => (index & widthMask) + XMin;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetX(int index)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			return GetXUnverified(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetX(int index, out int x)
		{
			if (Contains(index))
			{
				x = GetXUnverified(index);
				return true;
			}
			x = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetYUnverified(int index) => (index >> WidthPower) + YMin;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetY(int index)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			return GetYUnverified(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetY(int index, out int y)
		{
			if (Contains(index))
			{
				y = GetYUnverified(index);
				return true;
			}
			y = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetPositionUnverified(int index, out int x, out int y)
		{
			x = GetXUnverified(index);
			y = GetYUnverified(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int GetPositionUnverified(int index) => new(GetXUnverified(index), GetYUnverified(index));

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
		public bool TryGetPosition(int index, out int x, out int y)
		{
			if (Contains(index))
			{
				GetPositionUnverified(index, out x, out y);
				return true;
			}
			x = default;
			y = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetPosition(int index, out Vector2Int position)
		{
			if (Contains(index))
			{
				position = GetPositionUnverified(index);
				return true;
			}
			position = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetNeighborIndexUnverified(int index, GridDirections direction)
		{
			return direction switch
			{
				GridDirections.Forward => index + Width,
				GridDirections.ForwardRight => index + Width + 1,
				GridDirections.Right => index + 1,
				GridDirections.BackwardRight => index - Width + 1,
				GridDirections.Backward => index - Width,
				GridDirections.BackwardLeft => index - Width - 1,
				GridDirections.Left => index - 1,
				GridDirections.ForwardLeft => index + Width - 1,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}
	}
}