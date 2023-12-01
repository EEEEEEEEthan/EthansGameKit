using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public struct Angle
	{
		public static Angle FromDeg(float deg)
		{
			return new() { deg = deg };
		}
		public static Angle FromRad(float rad)
		{
			return new() { deg = rad * Mathf.Rad2Deg };
		}
		public static bool operator ==(Angle a, Angle b)
		{
			return a.deg == b.deg;
		}
		public static bool operator !=(Angle a, Angle b)
		{
			return a.deg != b.deg;
		}
		public override string ToString()
		{
			return $"{deg}°";
		}
		public float deg;
		public float Rad => deg * Mathf.Deg2Rad;
		public override bool Equals(object obj)
		{
			return obj is Angle other && Equals(other);
		}
		public override int GetHashCode()
		{
			return deg.GetHashCode();
		}
		public bool Equals(Angle other)
		{
			return deg.Equals(other.deg);
		}
		public Angle MaxAngleLessThan(Angle target)
		{
			return target == this ? FromDeg(deg - 360f) : MaxAngleLessOrEquals(target);
		}
		public Angle MinAngleLargerThan(Angle target)
		{
			return target == this ? FromDeg(deg + 360f) : MinAngleLargerOrEquals(target);
		}
		public Angle MaxAngleLessOrEquals(Angle target)
		{
			var difference = deg - target.deg;
			var rotations = (difference / 360f).CeilToInt();
			return FromDeg(deg - rotations * 360f);
		}
		public Angle MinAngleLargerOrEquals(Angle target)
		{
			var difference = target.deg - deg;
			var rotations = (difference / 360f).CeilToInt();
			return FromDeg(deg + rotations * 360f);
		}
	}
}
