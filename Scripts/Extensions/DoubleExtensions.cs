using System;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static int FloorToInt(this double @this)
		{
			return (int)Math.Floor(@this);
		}
		public static int CeilToInt(this double @this)
		{
			return (int)Math.Ceiling(@this);
		}
		public static int RoundToInt(this double @this)
		{
			return (int)Math.Round(@this);
		}
	}
}
