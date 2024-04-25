using System;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static int FloorToInt(this float @this)
		{
			return (int)Math.Floor(@this);
		}
		public static int CeilToInt(this float @this)
		{
			return (int)Math.Ceiling(@this);
		}
		public static int RoundToInt(this float @this)
		{
			return (int)Math.Round(@this);
		}
		public static float Sqrt(this float @this)
		{
			return (float)Math.Sqrt(@this);
		}
		public static void Clamp(ref this float @this, float min, float max)
		{
			@this = Math.Clamp(@this, min, max);
		}
		public static float Clamped(this float @this, float min, float max)
		{
			return Math.Clamp(@this, min, max);
		}
	}
}
