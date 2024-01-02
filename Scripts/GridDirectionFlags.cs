using System;
using UnityEngine;

namespace EthansGameKit
{
	public enum GridDirections : byte
	{
		Forward = 0,
		ForwardRight = 1,
		Right = 2,
		BackwardRight = 3,
		Backward = 4,
		BackwardLeft = 5,
		Left = 6,
		ForwardLeft = 7,
	}

	[Flags]
	public enum GridDirectionFlags : byte
	{
		Forward = 1 << 0,
		ForwardRight = 1 << 1,
		Right = 1 << 2,
		BackwardRight = 1 << 3,
		Backward = 1 << 4,
		BackwardLeft = 1 << 5,
		Left = 1 << 6,
		ForwardLeft = 1 << 7,
	}

	public static partial class Extensions
	{
		static readonly Vector2Int[] direction2Vector2Int =
		{
			new(0, 1),
			new(1, 1),
			new(1, 0),
			new(1, -1),
			new(0, -1),
			new(-1, -1),
			new(-1, 0),
			new(-1, 1),
		};
		public static GridDirectionFlags Right(this GridDirectionFlags @this, byte offset = 1)
		{
			offset &= 0b111;
			var value = (int)@this;
			value <<= offset;
			value |= value >> 8;
			return (GridDirectionFlags)(value & 0b11111111);
		}
		public static GridDirectionFlags Left(this GridDirectionFlags @this, byte offset = 1)
		{
			offset &= 0b111;
			var value = (int)@this;
			value |= value << 8;
			value >>= offset;
			return (GridDirectionFlags)(value & 0b11111111);
		}
		public static GridDirections Right(this GridDirections @this, byte offset = 1)
		{
			return (GridDirections)(((byte)@this + offset) & 0b111);
		}
		public static GridDirections Left(this GridDirections @this, byte offset = 1)
		{
			return (GridDirections)(((byte)@this - offset) & 0b111);
		}
		public static Vector2Int ToVector2(this GridDirections @this)
		{
			return direction2Vector2Int[(int)@this];
		}
		public static char ToChar(this GridDirections @this)
		{
			return @this switch
			{
				GridDirections.Forward => '↑',
				GridDirections.ForwardRight => '↗',
				GridDirections.Right => '→',
				GridDirections.BackwardRight => '↘',
				GridDirections.Backward => '↓',
				GridDirections.BackwardLeft => '↙',
				GridDirections.Left => '←',
				GridDirections.ForwardLeft => '↖',
				_ => throw new NotImplementedException(),
			};
		}
		public static bool IsDiagonal(this GridDirections @this)
		{
			return ((byte)@this & 1) == 1;
		}
	}
}
