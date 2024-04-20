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
	}
}
