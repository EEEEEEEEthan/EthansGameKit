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
		public static float Clamp(this float @this, float min, float max)
		{
			return Mathf.Clamp(@this, min, max);
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
				true => value.Clamp(0, 1),
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
	}
}
