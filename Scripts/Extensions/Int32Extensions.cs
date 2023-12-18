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
		public static int GetRightmostBitPosition(this int @this)
		{
			if (@this == 0) return -1;
			var rightMostBit = @this & ~(@this - 1);
			var position = 0;
			if ((rightMostBit & 0b_11111111_11111111_00000000_00000000) != 0) position += 16;
			if ((rightMostBit & 0b_11111111_00000000_11111111_00000000) != 0) position += 8;
			if ((rightMostBit & 0b_11110000_11110000_11110000_11110000) != 0) position += 4;
			if ((rightMostBit & 0b_11001100_11001100_11001100_11001100) != 0) position += 2;
			if ((rightMostBit & 0b_10101010_10101010_10101010_10101010) != 0) position += 1;
			return position;
		}
		public static int GetBitPositions(this int @this, int[] positions)
		{
			var count = 0;
			while (@this != 0)
			{
				var position = @this.GetRightmostBitPosition();
				positions[count++] = position;
				@this &= ~(1 << position);
			}
			return count;
		}
		public static void GetBitPositions(this int @this, List<int> positions)
		{
			while (@this != 0)
			{
				var position = @this.GetRightmostBitPosition();
				positions.Add(position);
				@this &= ~(1 << position);
			}
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
