using System;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static long Clamped(this long @this, long min, long max)
		{
			return Math.Max(Math.Min(@this, max), min);
		}
	}
}
