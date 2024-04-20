using System.Runtime.CompilerServices;
using UnityEngine;

namespace EthansGameKit.RectGrid
{
	public class BitMap2D
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void GetIndex(int index, out int chunkIndex, out int bitIndex)
		{
			chunkIndex = index >> 6;
			bitIndex = index - (chunkIndex << 6);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int GetIndex(int chunkIndex, int bitIndex)
		{
			return (chunkIndex << 6) + bitIndex;
		}
		public readonly GridIndexCalculator Calculator;
		readonly long[] chunks;
		public BitMap2D(int widthPower)
		{
			Calculator = new(widthPower);
			chunks = new long[Calculator.Count >> 6];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValue(int index)
		{
			GetIndex(index, out var chunkIndex, out var bitIndex);
			return (chunks[chunkIndex] & (1L << bitIndex)) != 0;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValue(int x, int y)
		{
			return GetValue(Calculator.GetIndex(x, y));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValueUnverified(int x, int y)
		{
			return GetValue(Calculator.GetIndexUnverified(x, y));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValue(Vector2Int position)
		{
			return GetValue(Calculator.GetIndex(position));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValueUnverified(Vector2Int position)
		{
			return GetValue(Calculator.GetIndexUnverified(position));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(int index, bool value)
		{
			GetIndex(index, out var chunkIndex, out var bitIndex);
			if (value)
				chunks[chunkIndex] |= 1L << bitIndex;
			else
				chunks[chunkIndex] &= ~(1L << bitIndex);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(int x, int y, bool value)
		{
			SetValue(Calculator.GetIndex(x, y), value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValueUnverified(int x, int y, bool value)
		{
			SetValue(Calculator.GetIndexUnverified(x, y), value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(Vector2Int position, bool value)
		{
			SetValue(Calculator.GetIndex(position), value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValueUnverified(Vector2Int position, bool value)
		{
			SetValue(Calculator.GetIndexUnverified(position), value);
		}
	}
}
