using System.Collections.Generic;
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
		public static bool IsPrime(this int @this)
		{
			return MathUtility.PrimeCalculator.IsPrime(@this);
		}
		public static int NextPrime(this int @this)
		{
			return MathUtility.PrimeCalculator.NextPrime(@this);
		}
		public static int PreviousPrime(this int @this)
		{
			return MathUtility.PrimeCalculator.PreviousPrime(@this);
		}
		public static IEnumerable<int> GetPrimeFactors(this int @this)
		{
			return MathUtility.PrimeCalculator.GetPrimeFactors(@this);
		}
		public static void GetPrimeFactors(this int @this, ICollection<int> collection)
		{
			MathUtility.PrimeCalculator.GetPrimeFactors(@this, collection);
		}
	}
}
