using System.Runtime.CompilerServices;
using UnityEngine;

namespace EthansGameKit.RectGrid
{
	public interface IReadOnlyMapData<T>
	{
		public bool TryGetValue(int index, out T value);
		public bool TryGetValue(int x, int y, out T value);
		public bool TryGetValue(Vector2Int position, out T value);
		public T GetValue(int x, int y);
		public T GetValueUnverified(int x, int y);
		public T GetValue(Vector2Int position);
		public T GetValueUnverified(Vector2Int position);
		public T GetValue(int index);
		public T this[int index] { get; }
		public T this[int x, int y] { get; }
		public T this[Vector2Int position] { get; }
	}

	public class MapData<T> : IReadOnlyMapData<T>
	{
		public readonly GridIndexCalculator GridCalculator;
		readonly T[] data;
		public MapData(int widthPower)
		{
			GridCalculator = new(widthPower);
			data = new T[GridCalculator.count];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetValue(int index, out T value)
		{
			if (GridCalculator.Contains(index))
			{
				value = data[index];
				return true;
			}
			value = default;
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetValue(int x, int y, out T value)
		{
			if (GridCalculator.Contains(x, y))
			{
				value = data[GridCalculator.GetIndex(x, y)];
				return true;
			}
			value = default;
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetValue(Vector2Int position, out T value)
		{
			if (GridCalculator.Contains(position))
			{
				value = data[GridCalculator.GetIndex(position)];
				return true;
			}
			value = default;
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValue(int x, int y)
		{
			return data[GridCalculator.GetIndex(x, y)];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValueUnverified(int x, int y)
		{
			return data[GridCalculator.GetIndexUnverified(x, y)];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValue(Vector2Int position)
		{
			return data[GridCalculator.GetIndex(position)];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValueUnverified(Vector2Int position)
		{
			return data[GridCalculator.GetIndexUnverified(position)];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValue(int index)
		{
			return data[index];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(int index, T value)
		{
			data[index] = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(int x, int y, T value)
		{
			data[GridCalculator.GetIndex(x, y)] = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValueUnverified(int x, int y, T value)
		{
			data[GridCalculator.GetIndexUnverified(x, y)] = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(Vector2Int position, T value)
		{
			data[GridCalculator.GetIndex(position)] = value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValueUnverified(Vector2Int position, T value)
		{
			data[GridCalculator.GetIndexUnverified(position)] = value;
		}
		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => data[index];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => data[index] = value;
		}
		public T this[int x, int y]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => data[GridCalculator.GetIndex(x, y)];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => data[GridCalculator.GetIndex(x, y)] = value;
		}
		public T this[Vector2Int position]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => data[GridCalculator.GetIndex(position)];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => data[GridCalculator.GetIndex(position)] = value;
		}
	}
}
