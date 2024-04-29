using System.IO;
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
		static int GetIndex(int chunkIndex, int bitIndex) => (chunkIndex << 6) + bitIndex;

		public readonly GridIndexCalculator Calculator;
		readonly long[] chunks;

		public BitMap2D(int widthPower)
		{
			Calculator = new(widthPower);
			chunks = new long[Calculator.count >> 6];
		}

		public BitMap2D(BinaryReader reader)
		{
			using (reader.BeginBlock("BitMap2D"))
			{
				Calculator = new(reader.ReadByte());
				chunks = new long[Calculator.count >> 6];
				var count = chunks.Length;
				for (var i = 0; i < count; ++i)
					chunks[i] = reader.ReadInt64();
			}
		}

		public void Deserialize(BinaryReader reader)
		{
			using (reader.BeginBlock("BitMap2D"))
			{
				var power = reader.ReadByte();
				if (power != Calculator.widthPower)
					throw new InvalidDataException($"BitMap2D power mismatch: {power} != {Calculator.widthPower}");
				var count = chunks.Length;
				for (var i = 0; i < count; ++i)
					chunks[i] = reader.ReadInt64();
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			using (writer.BeginBlock("BitMap2D"))
			{
				writer.Write((byte)Calculator.widthPower);
				var count = chunks.Length;
				for (var i = 0; i < count; ++i)
					writer.Write(chunks[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValue(int index)
		{
			GetIndex(index, out var chunkIndex, out var bitIndex);
			return (chunks[chunkIndex] & (1L << bitIndex)) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValue(int x, int y) => GetValue(Calculator.GetIndex(x, y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValueUnverified(int x, int y) => GetValue(Calculator.GetIndexUnverified(x, y));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValue(Vector2Int position) => GetValue(Calculator.GetIndex(position));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetValueUnverified(Vector2Int position) => GetValue(Calculator.GetIndexUnverified(position));

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