using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static float Clamp(this float @this, float min, float max)
		{
			return Mathf.Clamp(@this, min, max);
		}
	}
}
