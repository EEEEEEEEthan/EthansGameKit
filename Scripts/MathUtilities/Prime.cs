using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public static class Prime
	{
		static readonly List<uint> primes = new() { 2 };
		public static bool IsPrime(uint value)
		{
			while (primes[^1] < value)
				AddNextPrime();
			return primes.BinarySearch(value) >= 0;
		}
		public static uint NextPrime(uint value)
		{
			if (primes[^1] < value)
			{
				while (primes[^1] < value)
					AddNextPrime();
				return primes[^1];
			}
			var index = primes.BinarySearch(value);
			return index >= 0 ? primes[index + 1] : primes[~index];
		}
		public static uint PreviousPrime(uint value)
		{
			if (value <= primes[0])
			{
				Debug.LogError($"argument out of range {value}");
				return primes[0];
			}
			var index = primes.BinarySearch(value);
			return index >= 0 ? primes[index - 1] : primes[~index - 1];
		}
		public static IEnumerable<uint> GetPrimeFactors(uint value)
		{
			if (value.IsPrime())
			{
				yield return value;
				yield break;
			}
			while (primes[^1] < value)
				AddNextPrime();
			var count = primes.Count;
			for (var i = 0; i < count; ++i)
			{
				var prime = primes[i];
				if (prime > value)
					yield break;
				if (value % prime == 0)
					yield return prime;
			}
		}
		public static void GetPrimeFactors(uint value, ICollection<uint> collection)
		{
			if (value.IsPrime())
			{
				collection.Add(value);
				return;
			}
			while (primes[^1] < value)
				AddNextPrime();
			var count = primes.Count;
			for (var i = 0; i < count; ++i)
			{
				var prime = primes[i];
				if (prime > value)
					return;
				if (value % prime == 0)
				{
					collection.Add(prime);
					return;
				}
			}
		}
		static void AddNextPrime()
		{
			for (var value = primes[^1] + 1;; ++value)
			{
				var sqrtValue = (int)Math.Sqrt(value);
				var primeCount = primes.Count;
				for (var i = 0; i < primeCount; ++i)
				{
					var knownPrime = primes[i];
					if (knownPrime > sqrtValue)
					{
						primes.Add(value);
						return;
					}
					if (value % knownPrime == 0)
						break;
				}
			}
		}
	}
}
