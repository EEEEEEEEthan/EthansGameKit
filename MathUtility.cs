using System;
using System.Collections.Generic;
using EthansGameKit.CachePools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EthansGameKit
{
	public static class MathUtility
	{
		public static class PrimeCalculator
		{
			static readonly List<int> primes = new() { 2 };
			public static bool IsPrime(int value)
			{
				while (primes[^1] < value)
					AddNextPrime();
				return primes.BinarySearch(value) >= 0;
			}
			public static int NextPrime(int value)
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
			public static int PreviousPrime(int value)
			{
				if (value <= primes[0])
				{
					Debug.LogError($"argument out of range {value}");
					return primes[0];
				}
				var index = primes.BinarySearch(value);
				return index >= 0 ? primes[index - 1] : primes[~index - 1];
			}
			public static IEnumerable<int> GetPrimeFactors(int value)
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
			public static void GetPrimeFactors(int value, ICollection<int> collection)
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

		public class RandomSequenceGenerator : IDisposable
		{
			public static RandomSequenceGenerator Generate(int length, int seed)
			{
				if (!GlobalCachePool<RandomSequenceGenerator>.TryGenerate(out var generator))
					generator = new();
				generator.Initialize(length, seed, seed);
				return generator;
			}
			int a, c, m;
			int seed;
			public override string ToString()
			{
				return $"{GetType().Name}(m:{m},a:{a},c:{c},seed:{seed})";
			}
			void IDisposable.Dispose()
			{
				GlobalCachePool<RandomSequenceGenerator>.Recycle(this);
			}
			public int Next()
			{
				return seed = LinearCongruentialGenerator(seed, a, c, m);
			}
			void Initialize(int length, int c, int seed)
			{
				this.seed = seed;
				m = length;
				// c,m互质
				if (m <= 2)
				{
					c = 1;
				}
				else
				{
					c %= m;
					c.Clamp(1, m - 1);
					if (m.CoprimeWith(c))
					{
						this.c = c;
					}
					else
					{
						for (var i = 1; i < m; ++i)
						{
							var a = c + 1;
							if (a.Between(1, m) && a.CoprimeWith(m))
							{
								c = a;
								goto EARLY_BREAK;
							}
							var b = c - 1;
							if (b.Between(1, m) && b.CoprimeWith(m))
							{
								c = b;
								goto EARLY_BREAK;
							}
						}
						c = 1;
					EARLY_BREAK: ;
					}
				}
				this.c = c;
				// m的所有质因数能整除a-1; 若m是4的倍数,a-1也是
				a = 1;
				foreach (var factor in m.GetPrimeFactors())
					a *= factor;
				if (m % 4 == 0)
				{
					if (a % 4 != 0) a *= 2;
					if (a % 4 != 0) a *= 2;
				}
				++a;
				if (a > m) a = 1;
			}
		}

		public static void Hermite(float pos0, float weight0, float pos1, float weight1, float progress, out float point, out float weight)
		{
			switch (progress)
			{
				case <= 0:
					point = pos0;
					weight = weight0;
					return;
				case >= 1:
					point = pos1;
					weight = weight1;
					return;
			}
			// ReSharper disable once InlineTemporaryVariable
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = pos0 * (2 * t3 - 3 * t2 + 1) + weight0 * (t3 - 2 * t2 + t1) + pos1 * (-2 * t3 + 3 * t2) + weight1 * (t3 - t2);
			weight = pos0 * (6 * t2 - 6 * t1) + weight0 * (3 * t2 - 4 * t1 + 1) + pos1 * (-6 * t2 + 6 * t1) + weight1 * (3 * t2 - 2 * t1);
		}
		public static void Hermite(Vector2 pos0, Vector2 weight0, Vector2 pos1, Vector2 weight1, float progress, out Vector2 point, out Vector2 weight)
		{
			switch (progress)
			{
				case <= 0:
					point = pos0;
					weight = weight0;
					return;
				case >= 1:
					point = pos1;
					weight = weight1;
					return;
			}
			// ReSharper disable once InlineTemporaryVariable
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = pos0 * (2 * t3 - 3 * t2 + 1) + weight0 * (t3 - 2 * t2 + t1) + pos1 * (-2 * t3 + 3 * t2) + weight1 * (t3 - t2);
			weight = pos0 * (6 * t2 - 6 * t1) + weight0 * (3 * t2 - 4 * t1 + 1) + pos1 * (-6 * t2 + 6 * t1) + weight1 * (3 * t2 - 2 * t1);
		}
		public static void Hermite(Vector3 pos0, Vector3 weight0, Vector3 pos1, Vector3 weight1, float progress, out Vector3 point, out Vector3 weight)
		{
			switch (progress)
			{
				case <= 0:
					point = pos0;
					weight = weight0;
					return;
				case >= 1:
					point = pos1;
					weight = weight1;
					return;
			}
			// ReSharper disable once InlineTemporaryVariable
			var t1 = progress;
			var t2 = t1 * t1;
			var t3 = t2 * t1;
			point = pos0 * (2 * t3 - 3 * t2 + 1) + weight0 * (t3 - 2 * t2 + t1) + pos1 * (-2 * t3 + 3 * t2) + weight1 * (t3 - t2);
			weight = pos0 * (6 * t2 - 6 * t1) + weight0 * (3 * t2 - 4 * t1 + 1) + pos1 * (-6 * t2 + 6 * t1) + weight1 * (3 * t2 - 2 * t1);
		}
		/// <summary>
		///     正态分布随机数
		/// </summary>
		/// <param name="mean">正态分布的平均值</param>
		/// <param name="stdDev">标准差</param>
		/// <returns>符合正态分布的随机数,值域是[float.MinValue, float.MaxValue]</returns>
		public static float RandomNormal(float mean, float stdDev = 1)
		{
			var u1 = Random.value;
			var u2 = Random.value;
			var z = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
			return mean + stdDev * z;
		}
		/// <summary>
		///     计算正态分布概率密度函数的值
		/// </summary>
		/// <param name="x">随机变量的取值</param>
		/// <param name="mean">正态分布的均值</param>
		/// <param name="stdDev">正态分布的标准差</param>
		/// <returns>指定取值 x 的正态分布概率密度函数的值</returns>
		public static double NormalProbabilityDensityFunction(float x, float mean, float stdDev)
		{
			var a = 1 / (stdDev * Mathf.Sqrt(2 * Mathf.PI));
			var b = -Mathf.Pow(x - mean, 2) / (2 * Mathf.Pow(stdDev, 2));
			return a * Mathf.Exp(b);
		}
		public static int LinearCongruentialGenerator(int seed, int a, int c, int mPower)
		{
			var tmp = a * seed;
			tmp += tmp << mPower;
			var mask = (1 << mPower) - 1;
			return (tmp & mask) + c;
		}
	}
}
