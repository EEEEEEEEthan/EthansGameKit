using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EthansGameKit.RectGrid
{
	// ReSharper disable once StructCanBeMadeReadOnly
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public struct GridIndexCalculator
	{
		struct Enumerator : IEnumerable<Vector2Int>, IEnumerator<Vector2Int>
		{
			readonly GridIndexCalculator calculator;
			int currentIndex;

			public Enumerator(GridIndexCalculator calculator)
			{
				this.calculator = calculator;
				currentIndex = -1;
			}

			Vector2Int IEnumerator<Vector2Int>.Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => calculator.GetPositionUnverified(currentIndex);
			}

			object IEnumerator.Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => calculator.GetPositionUnverified(currentIndex);
			}

			IEnumerator<Vector2Int> IEnumerable<Vector2Int>.GetEnumerator() => this;

			IEnumerator IEnumerable.GetEnumerator() => this;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool IEnumerator.MoveNext()
			{
				++currentIndex;
				return currentIndex < calculator.count;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IEnumerator.Reset()
			{
				currentIndex = -1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IDisposable.Dispose()
			{
			}
		}

		public readonly int widthPower;
		public readonly int heightPower;
		/// <summary>
		///     总宽度(2的整数次方)
		/// </summary>
		public readonly int width;
		/// <summary>
		///     总高度(2的整数次方)
		/// </summary>
		public readonly int height;
		public readonly int xMin;
		public readonly int yMin;
		public readonly int xMax;
		public readonly int yMax;
		/// <summary>
		///     总数量<see cref="width" /> x <see cref="height" />
		/// </summary>
		public readonly int count;
		public readonly RectInt rect;
		readonly int widthMask;

		public GridIndexCalculator(RectInt rect)
		{
			widthPower = Mathf.CeilToInt(Mathf.Log(rect.width, 2));
			heightPower = Mathf.CeilToInt(Mathf.Log(rect.height, 2));
			width = 1 << widthPower;
			height = 1 << heightPower;
			xMin = rect.xMin;
			yMin = rect.yMin;
			xMax = rect.xMax;
			yMax = rect.yMax;
			widthMask = width - 1;
			count = width * height;
			this.rect = rect;
		}

		public GridIndexCalculator(int widthPower) : this(new RectInt(0, 0, 1 << widthPower, 1 << widthPower))
		{
		}

		public IEnumerable<Vector2Int> AllPositionWithin => new Enumerator(this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(int index) => index >= 0 && index < count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(int x, int y) => x >= xMin && x < xMax && y >= yMin && y < yMax;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(Vector2Int position) => Contains(position.x, position.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetIndexUnverified(int x, int y) => ((y - yMin) << widthPower) | (x - xMin);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetIndexUnverified(Vector2Int position) => GetIndexUnverified(position.x, position.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetIndex(int x, int y)
		{
			if (!Contains(x, y)) throw new ArgumentOutOfRangeException($"({x}, {y}) is not in the grid.");
			return GetIndexUnverified(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetIndex(Vector2Int position) => GetIndex(position.x, position.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetIndex(int x, int y, out int index)
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
		public readonly bool TryGetIndex(Vector2Int position, out int index) =>
			TryGetIndex(position.x, position.y, out index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetXUnverified(int index) => (index & widthMask) + xMin;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetX(int index)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			return GetXUnverified(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetX(int index, out int x)
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
		public readonly int GetYUnverified(int index) => (index >> widthPower) + yMin;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetY(int index)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			return GetYUnverified(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetY(int index, out int y)
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
		public readonly void GetPositionUnverified(int index, out int x, out int y)
		{
			x = GetXUnverified(index);
			y = GetYUnverified(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2Int GetPositionUnverified(int index) =>
			new(GetXUnverified(index), GetYUnverified(index));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void GetPosition(int index, out int x, out int y)
		{
			if (!Contains(index)) throw new ArgumentOutOfRangeException($"({index}) is not in the grid.");
			GetPositionUnverified(index, out x, out y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector2Int GetPosition(int index)
		{
			GetPosition(index, out var x, out var y);
			return new(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool TryGetPosition(int index, out int x, out int y)
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
		public readonly bool TryGetPosition(int index, out Vector2Int position)
		{
			if (Contains(index))
			{
				position = GetPositionUnverified(index);
				return true;
			}
			position = default;
			return false;
		}
	}
}