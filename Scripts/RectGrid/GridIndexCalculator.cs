using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace EthansGameKit.RectGrid
{
	public readonly struct GridIndexCalculator
	{
		struct AllPositionWithinEnumerator : IEnumerator<Vector2Int>, IEnumerable<Vector2Int>
		{
			readonly GridIndexCalculator calculator;
			int currentIndex;
			public AllPositionWithinEnumerator(GridIndexCalculator calculator)
			{
				this.calculator = calculator;
				currentIndex = -1;
			}
			Vector2Int IEnumerator<Vector2Int>.Current => calculator.GetPositionUnverified(currentIndex);
			object IEnumerator.Current => calculator.GetPositionUnverified(currentIndex);
			bool IEnumerator.MoveNext()
			{
				++currentIndex;
				return currentIndex < calculator.Count;
			}
			void IEnumerator.Reset()
			{
				currentIndex = -1;
			}
			void IDisposable.Dispose()
			{
			}
			IEnumerator<Vector2Int> IEnumerable<Vector2Int>.GetEnumerator()
			{
				return this;
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}
		}

		public readonly int WidthPower;
		public readonly int Width;
		public readonly int Count;
		public GridIndexCalculator(int widthPower)
		{
			WidthPower = widthPower;
			Width = 1 << widthPower;
			Count = Width << widthPower;
			Assert.AreEqual(Width * Width, Count);
		}
		public IEnumerable<Vector2Int> AllPositionWithin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new AllPositionWithinEnumerator(this);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexUnverified(int x, int y)
		{
			return x | (y << WidthPower);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(int x, int y)
		{
			if (x < 0 || x >= Width || y < 0 || y >= Width)
				throw new ArgumentOutOfRangeException($"x={x},y={y},width={Width}");
			return GetIndexUnverified(x, y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndexUnverified(Vector2Int position)
		{
			return GetIndexUnverified(position.x, position.y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetIndex(Vector2Int position)
		{
			return GetIndex(position.x, position.y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetPositionUnverified(int index, out int x, out int y)
		{
			y = index >> WidthPower;
			x = index & (Width - 1);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetPosition(int index, out int x, out int y)
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException();
			GetPositionUnverified(index, out x, out y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int GetPositionUnverified(int index)
		{
			GetPositionUnverified(index, out var x, out var y);
			return new(x, y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3Int GetPosition(int index)
		{
			GetPosition(index, out var x, out var y);
			return new(x, y, 0);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(int index)
		{
			return index >= 0 && index < Count;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(int x, int y)
		{
			return x >= 0 && x < Width && y >= 0 && y < Width;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector2Int position)
		{
			return Contains(position.x, position.y);
		}
	}
}
