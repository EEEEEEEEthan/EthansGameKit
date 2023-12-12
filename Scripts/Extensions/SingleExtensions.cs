﻿using System.Runtime.CompilerServices;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Between(this float @this, float minIncluded, float maxExcluded)
		{
			return @this >= minIncluded && @this < maxExcluded;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Between(this float @this, float min, float max, bool minIncluded, bool maxIncluded)
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
		public static void Clamp(ref this float @this, float min, float max)
		{
			@this = Mathf.Clamp(@this, min, max);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Clamped(this float @this, float min, float max)
		{
			return Mathf.Clamp(@this, min, max);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Modulated(this float @this, float mod)
		{
			return @this - Mathf.Floor(@this / mod) * mod;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Modular(ref this float @this, float mod)
		{
			@this -= Mathf.Floor(@this / mod) * mod;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InverseLerp(this float @this, float min, float max)
		{
			return InverseLerp(@this, min, max, true);
		} // ReSharper disable Unity.PerformanceAnalysis
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InverseLerp(this float @this, float min, float max, bool clamp)
		{
			var delta = max - min;
			float value;
			switch (delta)
			{
				case 0:
					Debug.LogError($"invalid range {min}, {max}");
					value = 0;
					break;
				default:
					value = (@this - min) / delta;
					break;
			}
			return clamp switch
			{
				true => value.Clamped(0, 1),
				_ => value,
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FloorToInt(this float @this)
		{
			return Mathf.FloorToInt(@this);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CeilToInt(this float @this)
		{
			return Mathf.CeilToInt(@this);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RoundToInt(this float @this)
		{
			return Mathf.RoundToInt(@this);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Remap(ref this float @this, float min, float max, float newMin, float newMax)
		{
			@this = Mathf.Lerp(newMin, newMax, @this.InverseLerp(min, max));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Remapped(this float @this, float min, float max, float newMin, float newMax)
		{
			return Mathf.Lerp(newMin, newMax, @this.InverseLerp(min, max));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceTo(this float @this, float other)
		{
			return Mathf.Abs(@this - other);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CloseTo(this float @this, float other, float tolerance = float.Epsilon)
		{
			return @this.DistanceTo(other) <= tolerance;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Abs(this float @this)
		{
			return Mathf.Abs(@this);
		}
	}
}
