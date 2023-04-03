using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Between(this int @this, int minIncluded, int maxExcluded)
		{
			return @this >= minIncluded && @this < maxExcluded;
		}
		public static bool Between(this int @this, int min, int max, bool minIncluded, bool maxIncluded)
		{
			if (minIncluded)
			{
				if (@this < min) return false;
			}
			else
			{
				if (@this <= min) return false;
			}
			if (maxIncluded)
			{
				if (@this > max) return false;
			}
			else
			{
				if (@this >= max) return false;
			}
			return true;
		}
		public static int Clamp(this int @this, int min, int max)
		{
			return Mathf.Clamp(@this, min, max);
		}
	}
}