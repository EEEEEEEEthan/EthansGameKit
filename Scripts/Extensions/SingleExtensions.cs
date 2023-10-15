using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Between(this float @this, float minIncluded, float maxExcluded)
		{
			return @this >= minIncluded && @this < maxExcluded;
		}
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
		public static float Clamped(this float @this, float min, float max)
		{
			return Mathf.Clamp(@this, min, max);
		}
		public static float ClampMin(this float @this, float min)
		{
			return Mathf.Clamp(@this, min, float.MaxValue);
		}
		public static float ClampMax(this float @this, float max)
		{
			return Mathf.Clamp(@this, float.MinValue, max);
		}
		public static float InverseLerp(this float @this, float min, float max)
		{
			return InverseLerp(@this, min, max, true);
		} // ReSharper disable Unity.PerformanceAnalysis
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
		public static int FloorToInt(this float @this)
		{
			return Mathf.FloorToInt(@this);
		}
		public static int CeilToInt(this float @this)
		{
			return Mathf.CeilToInt(@this);
		}
		public static int RoundToInt(this float @this)
		{
			return Mathf.RoundToInt(@this);
		}
		public static void Remap(ref this float @this, float min, float max, float newMin, float newMax)
		{
			@this = Mathf.Lerp(newMin, newMax, @this.InverseLerp(min, max));
		}
		public static float Remapped(this float @this, float min, float max, float newMin, float newMax)
		{
			return Mathf.Lerp(newMin, newMax, @this.InverseLerp(min, max));
		}
		public static float DistanceTo(this float @this, float other)
		{
			return Mathf.Abs(@this - other);
		}
		public static bool CloseTo(this float @this, float other, float tolerance = float.Epsilon)
		{
			return @this.DistanceTo(other) <= tolerance;
		}
		public static float Abs(this float @this)
		{
			return Mathf.Abs(@this);
		}
	}
}
