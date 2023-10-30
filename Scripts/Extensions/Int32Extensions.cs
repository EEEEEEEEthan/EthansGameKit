using System;
using System.Collections.Generic;
using EthansGameKit.MathUtilities;
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
		public static int Clamped(this int @this, int min, int max)
		{
			return Mathf.Clamp(@this, min, max);
		}
		public static bool IsPrime(this int @this)
		{
			return @this > 0 && Prime.IsPrime((uint)@this);
		}
		public static int NextPrime(this int @this)
		{
			return (int)Prime.NextPrime((uint)@this.Clamped(0, int.MaxValue));
		}
		public static int PreviousPrime(this int @this)
		{
			return (int)Prime.PreviousPrime((uint)@this.Clamped(0, int.MaxValue));
		}
		public static IEnumerable<uint> GetPrimeFactors(this int @this)
		{
			if (@this <= 0)
			{
				Debug.LogError($"argument out of range: {@this}");
				return Array.Empty<uint>();
			}
			return Prime.GetPrimeFactors((uint)@this);
		}
		public static void GetPrimeFactors(this int @this, ICollection<uint> collection)
		{
			if (@this <= 0)
			{
				Debug.LogError($"argument out of range: {@this}");
				return;
			}
			Prime.GetPrimeFactors((uint)@this, collection);
		}
		public static bool CoprimeWith(this int @this, int other)
		{
			return @this.GreatestCommonDivisorWith(other) == 1;
		}
		static int GreatestCommonDivisorWith(this int a, int b)
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
