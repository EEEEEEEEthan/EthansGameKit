using System;
using UnityEngine;

namespace EthansGameKit.Pathfinding
{
	[Serializable]
	public struct Direction : IEquatable<Direction>
	{
		public static readonly Direction North = new(1 << 0);
		public static readonly Direction NorthEast = new(1 << 1);
		public static readonly Direction East = new(1 << 2);
		public static readonly Direction SouthEast = new(1 << 3);
		public static readonly Direction South = new(1 << 4);
		public static readonly Direction SouthWest = new(1 << 5);
		public static readonly Direction West = new(1 << 6);
		public static readonly Direction NorthWest = new(1 << 7);
		public static explicit operator int(Direction v)
		{
			return v.value;
		}
		public static explicit operator Direction(int v)
		{
			return new(v);
		}
		public static bool operator ==(Direction a, Direction b)
		{
			return a.value == b.value;
		}
		public static bool operator !=(Direction a, Direction b)
		{
			return a.value != b.value;
		}
		public static Direction operator |(Direction a, Direction b)
		{
			return new(a.value | b.value);
		}
		public static Direction operator &(Direction a, Direction b)
		{
			return new(a.value & b.value);
		}
		public static Direction operator ~(Direction a)
		{
			return new(~a.value);
		}
		public static implicit operator bool(Direction v)
		{
			return v.value != 0;
		}
		[SerializeField] int value;
		public Direction Next => new((value << 1) | (value >> 7));
		Direction(int value)
		{
			this.value = value;
		}
		public override bool Equals(object obj)
		{
			return obj is Direction other && Equals(other);
		}
		public override int GetHashCode()
		{
			return value.GetHashCode();
		}
		public bool Equals(Direction other)
		{
			return value == other.value;
		}
	}
}
