using System;

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
		public static GridDirectionFlags Right(this GridDirectionFlags @this, byte offset = 1)
		{
			offset &= 7;
			var value = (int)@this;
			value <<= offset;
			value |= value >> 8;
			return (GridDirectionFlags)value;
		}
		public static GridDirectionFlags Left(this GridDirectionFlags @this, byte offset = 1)
		{
			offset &= 7;
			var value = (int)@this;
			value |= value << 8;
			value >>= offset;
			return (GridDirectionFlags)value;
		}
	}
}
