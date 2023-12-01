﻿using System;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Between(this long @this, long minIncluded, long maxExcluded)
		{
			return @this >= minIncluded && @this < maxExcluded;
		}
		public static bool Between(this long @this, long min, long max, bool minIncluded, bool maxIncluded)
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
		public static long Clamped(this long @this, long min, long max)
		{
			return Math.Min(Math.Max(@this, min), max);
		}
		public static void Clamp(ref this long @this, long min, long max)
		{
			@this = Clamped(@this, min, max);
		}
		static long GreatestCommonDivisorWith(this long a, long b)
		{
			if (a < b)
				(a, b) = (b, a);
			while (b != 0)
			{
				var temp = a % b;
				a = b;
				b = temp;
			}
			return a;
		}
	}
}