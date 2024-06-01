using System;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		static uint[] primes = { 2, 3, 5, 7 };
		public static float Sqrt(this uint @this)
		{
			return (float)Math.Sqrt(@this);
		}
		public static uint Clamped(this uint @this, uint min, uint max)
		{
			return @this < min ? min : @this > max ? max : @this;
		}
		public static void Clamp(ref this uint @this, uint min, uint max)
		{
			@this = @this < min ? min : @this > max ? max : @this;
		}
		/// <summary>
		///     判断是否与<paramref name="other" />互质
		/// </summary>
		/// <param name="this"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool CoprimeWith(this uint @this, uint other)
		{
			while (other != 0)
			{
				var t = other;
				other = @this % other;
				@this = t;
			}
			return @this == 1;
		}
		/// <summary>
		///     获取所有质因子
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static uint[] GetPrimeFactors(this uint @this)
		{
			if (@this.IsPrime()) return new[] { @this };
			var index = Array.BinarySearch(primes, @this);
			var count = ~index;
			var result = new uint[count];
			Array.Copy(primes, result, count);
			return result;
		}
		/// <summary>
		///     获取所有质因子
		/// </summary>
		/// <param name="this"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static int GetPrimeFactors(this uint @this, uint[] result)
		{
			if (@this.IsPrime()) return 1;
			var index = Array.BinarySearch(primes, @this);
			var count = ~index;
			Array.Copy(primes, result, count);
			return count;
		}
		/// <summary>
		///     小于<paramref name="this" />的最大互质数
		/// </summary>
		public static uint GetLargestCoprimeBelow(this uint @this)
		{
			var i = @this - 1u;
			for (; i >= 1; i--)
				if (@this.CoprimeWith(i))
					return i;
			return i;
		}
		/// <summary>
		///     是否是质数
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static bool IsPrime(this uint @this)
		{
			if (@this < 2) return false;
			var index = Array.BinarySearch(primes, @this);
			if (index >= 0) return true;
			var idx = ~index;
			if (idx <= 0) return false;
			if (idx >= primes.Length)
			{
				while (true)
				{
					expandPrimeCollection();
					var largest = primes[^1];
					if (largest == @this) return true;
					if (largest > @this) return false;
				}
			}
			return false;
			static void expandPrimeCollection()
			{
				for (var n = primes[^1];; ++n)
				{
					for (var i = 0; i < primes.Length; ++i)
						if (n % primes[i] == 0)
							goto NOT_PRIME;
					Array.Resize(ref primes, primes.Length + 1);
					primes[^1] = n;
					return;
				NOT_PRIME: ;
				}
			}
		}
	}
}
