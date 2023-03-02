using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Between(this int @this, int min, int max)
		{
			return @this >= min && @this <= max;
		}
		public static int Clamp(this int @this, int min, int max)
		{
			return Mathf.Clamp(@this, min, max);
		}
	}
}
