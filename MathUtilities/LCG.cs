﻿using EthansGameKit.CachePools;

namespace EthansGameKit.MathUtilities
{
	public static class Lcg
	{
		static int currentIntValue;
		public static uint Next(uint seed)
		{
			return Next(seed, 1103515245, 12345, uint.MaxValue);
		}
		public static int Next(int seed)
		{
			var trueSeed = (uint)seed;
			return (int)Next(trueSeed);
		}
		public static void GetSequenceFactors(uint seed, out uint a, out uint c, uint m)
		{
			c = seed;
			// c,m互质
			if (m <= 2)
			{
				c = 1;
			}
			else
			{
				c %= m;
				c.Clamped(1, m - 1);
				if (!m.CoprimeWith(c))
				{
					for (var i = 1; i < m; ++i)
					{
						var c1 = c + 1;
						if (c1.Between(1, m) && c1.CoprimeWith(m))
						{
							c = c1;
							goto EARLY_BREAK;
						}
						var c2 = c - 1;
						if (c2.Between(1, m) && c2.CoprimeWith(m))
						{
							c = c2;
							goto EARLY_BREAK;
						}
					}
					c = 1;
				EARLY_BREAK: ;
				}
			}
			// m的所有质因数能整除a-1; 若m是4的倍数,a-1也是
			a = 1u;
			var primeFactors = ListPool<uint>.Generate();
			m.GetPrimeFactors(primeFactors);
			for (var i = primeFactors.Count; i-- > 0;)
				a *= primeFactors[i];
			primeFactors.ClearAndRecycle();
			if (m % 4 == 0)
			{
				if (a % 4 != 0) a *= 2;
				if (a % 4 != 0) a *= 2;
			}
			++a;
			if (a > m) a = 1;
		}
		public static uint Next(uint seed, uint a, uint c, uint m)
		{
			return (a * seed + c) % m;
		}
	}
}