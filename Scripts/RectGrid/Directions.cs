using System.Runtime.CompilerServices;
using UnityEngine;

namespace EthansGameKit.RectGrid
{
	public enum QuadDirectionCode
	{
		North,
		East,
		South,
		West
	}

	public enum OctDirectionCode
	{
		North,
		East,
		South,
		West,
		NorthEast,
		SouthEast,
		SouthWest,
		NorthWest
	}

	public static class Extensions
	{
		static readonly Vector2Int upRight = new(1, 1);
		static readonly Vector2Int downRight = new(1, -1);
		static readonly Vector2Int downLeft = new(-1, -1);
		static readonly Vector2Int upLeft = new(-1, 1);

		public static bool IsDiagonal(this OctDirectionCode direction)
		{
			return direction is > OctDirectionCode.West and <= OctDirectionCode.NorthEast;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Next(this ref QuadDirectionCode direction)
		{
			direction = direction.GetNext();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static QuadDirectionCode GetNext(this QuadDirectionCode direction)
		{
			return direction switch
			{
				QuadDirectionCode.North => QuadDirectionCode.East,
				QuadDirectionCode.East => QuadDirectionCode.South,
				QuadDirectionCode.South => QuadDirectionCode.West,
				QuadDirectionCode.West => QuadDirectionCode.North,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Previous(this ref QuadDirectionCode direction)
		{
			direction = direction.GetPrevious();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static QuadDirectionCode GetPrevious(this QuadDirectionCode direction)
		{
			return direction switch
			{
				QuadDirectionCode.North => QuadDirectionCode.West,
				QuadDirectionCode.East => QuadDirectionCode.North,
				QuadDirectionCode.South => QuadDirectionCode.East,
				QuadDirectionCode.West => QuadDirectionCode.South,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Opposite(this ref QuadDirectionCode direction)
		{
			direction = direction.GetOpposite();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static QuadDirectionCode GetOpposite(this QuadDirectionCode direction)
		{
			return direction switch
			{
				QuadDirectionCode.North => QuadDirectionCode.South,
				QuadDirectionCode.East => QuadDirectionCode.West,
				QuadDirectionCode.South => QuadDirectionCode.North,
				QuadDirectionCode.West => QuadDirectionCode.East,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int ToVector2Int(this QuadDirectionCode direction)
		{
			return direction switch
			{
				QuadDirectionCode.North => Vector2Int.up,
				QuadDirectionCode.East => Vector2Int.right,
				QuadDirectionCode.South => Vector2Int.down,
				QuadDirectionCode.West => Vector2Int.left,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Next(this ref OctDirectionCode direction)
		{
			direction = direction.GetNext();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OctDirectionCode GetNext(this OctDirectionCode direction)
		{
			return direction switch
			{
				OctDirectionCode.North => OctDirectionCode.NorthEast,
				OctDirectionCode.NorthEast => OctDirectionCode.East,
				OctDirectionCode.East => OctDirectionCode.SouthEast,
				OctDirectionCode.SouthEast => OctDirectionCode.South,
				OctDirectionCode.South => OctDirectionCode.SouthWest,
				OctDirectionCode.SouthWest => OctDirectionCode.West,
				OctDirectionCode.West => OctDirectionCode.NorthWest,
				OctDirectionCode.NorthWest => OctDirectionCode.North,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Previous(this ref OctDirectionCode direction)
		{
			direction = direction.GetPrevious();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OctDirectionCode GetPrevious(this OctDirectionCode direction)
		{
			return direction switch
			{
				OctDirectionCode.North => OctDirectionCode.NorthWest,
				OctDirectionCode.NorthEast => OctDirectionCode.North,
				OctDirectionCode.East => OctDirectionCode.NorthEast,
				OctDirectionCode.SouthEast => OctDirectionCode.East,
				OctDirectionCode.South => OctDirectionCode.SouthEast,
				OctDirectionCode.SouthWest => OctDirectionCode.South,
				OctDirectionCode.West => OctDirectionCode.SouthWest,
				OctDirectionCode.NorthWest => OctDirectionCode.West,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Opposite(this ref OctDirectionCode direction)
		{
			direction = direction.GetOpposite();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OctDirectionCode GetOpposite(this OctDirectionCode direction)
		{
			return direction switch
			{
				OctDirectionCode.North => OctDirectionCode.South,
				OctDirectionCode.NorthEast => OctDirectionCode.SouthWest,
				OctDirectionCode.East => OctDirectionCode.West,
				OctDirectionCode.SouthEast => OctDirectionCode.NorthWest,
				OctDirectionCode.South => OctDirectionCode.North,
				OctDirectionCode.SouthWest => OctDirectionCode.NorthEast,
				OctDirectionCode.West => OctDirectionCode.East,
				OctDirectionCode.NorthWest => OctDirectionCode.SouthEast,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int ToVector2Int(this OctDirectionCode direction)
		{
			return direction switch
			{
				OctDirectionCode.North => Vector2Int.up,
				OctDirectionCode.NorthEast => upRight,
				OctDirectionCode.East => Vector2Int.right,
				OctDirectionCode.SouthEast => downRight,
				OctDirectionCode.South => Vector2Int.down,
				OctDirectionCode.SouthWest => downLeft,
				OctDirectionCode.West => Vector2Int.left,
				OctDirectionCode.NorthWest => upLeft,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OctDirectionCode GetLeftForward(this OctDirectionCode direction)
		{
			return direction switch
			{
				OctDirectionCode.North => OctDirectionCode.NorthWest,
				OctDirectionCode.NorthEast => OctDirectionCode.North,
				OctDirectionCode.East => OctDirectionCode.NorthEast,
				OctDirectionCode.SouthEast => OctDirectionCode.East,
				OctDirectionCode.South => OctDirectionCode.SouthEast,
				OctDirectionCode.SouthWest => OctDirectionCode.South,
				OctDirectionCode.West => OctDirectionCode.SouthWest,
				OctDirectionCode.NorthWest => OctDirectionCode.West,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OctDirectionCode GetRightForward(this OctDirectionCode direction)
		{
			return direction switch
			{
				OctDirectionCode.North => OctDirectionCode.NorthEast,
				OctDirectionCode.NorthEast => OctDirectionCode.East,
				OctDirectionCode.East => OctDirectionCode.SouthEast,
				OctDirectionCode.SouthEast => OctDirectionCode.South,
				OctDirectionCode.South => OctDirectionCode.SouthWest,
				OctDirectionCode.SouthWest => OctDirectionCode.West,
				OctDirectionCode.West => OctDirectionCode.NorthWest,
				OctDirectionCode.NorthWest => OctDirectionCode.North,
				_ => throw new System.ArgumentOutOfRangeException()
			};
		}
	}
}