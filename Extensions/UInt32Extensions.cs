using System.Collections.Generic;
using EthansGameKit.MathUtilities;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Between(this uint @this, uint minIncluded, uint maxExcluded)
		{
			return @this >= minIncluded && @this < maxExcluded;
		}
		public static bool Between(this uint @this, uint min, uint max, bool minIncluded, bool maxIncluded)
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
		public static uint Clamp(this uint @this, uint min, uint max)
		{
			if (@this < min) return min;
			return @this > max ? max : @this;
		}
		public static bool IsPrime(this uint @this)
		{
			return PrimeCalculator.IsPrime(@this);
		}
		public static uint NextPrime(this uint @this)
		{
			return PrimeCalculator.NextPrime(@this);
		}
		public static uint PreviousPrime(this uint @this)
		{
			return PrimeCalculator.PreviousPrime(@this);
		}
		public static IEnumerable<uint> GetPrimeFactors(this uint @this)
		{
			return PrimeCalculator.GetPrimeFactors(@this);
		}
		public static void GetPrimeFactors(this uint @this, ICollection<uint> collection)
		{
			PrimeCalculator.GetPrimeFactors(@this, collection);
		}
		public static bool CoprimeWith(this uint @this, uint other)
		{
			return @this.GreatestCommonDivisorWith(other) == 1;
		}
		public static uint GreatestCommonDivisorWith(this uint a, uint b)
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
