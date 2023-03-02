﻿using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Between(this float @this, float min, float max)
		{
			return @this >= min && @this <= max;
		}
		public static float Clamp(this float @this, float min, float max)
		{
			return Mathf.Clamp(@this, min, max);
		}
	}
}
