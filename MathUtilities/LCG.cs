using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.MathUtilities
{
	public static class Lcg
	{
		static int currentIntValue;
		public static int Next(int seed, int min, int max)
		{
			var trueSeed = (uint)((long)seed - min);
			var delta = (uint)((long)max - min);
			var result = Next(trueSeed, 1664525, 1013904223, uint.MaxValue);
			return (int)(result % delta) + min;
		}
		public static float Next(uint seed, float min, float max)
		{
			var result = Next(seed, 1664525, 1013904223, uint.MaxValue);
			return (float)(result / (double)uint.MaxValue * (max - min) + min);
		}
		static uint Next(uint seed, uint a, uint c, uint m)
		{
			return (a * seed + c) % m;
		}
		static IEnumerator<uint> GetSequence(uint length, uint seed)
		{
			var c = seed;
			// ReSharper disable once InlineTemporaryVariable
			var m = length;
			// c,m互质
			if (m <= 2)
			{
				c = 1;
			}
			else
			{
				c %= m;
				c.Clamp(1, m - 1);
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
			var a = 1u;
			foreach (var factor in m.GetPrimeFactors())
				a *= factor;
			if (m % 4 == 0)
			{
				if (a % 4 != 0) a *= 2;
				if (a % 4 != 0) a *= 2;
			}
			++a;
			if (a > m) a = 1;
			for (var i = 0; i < m; ++i)
			{
				yield return seed = Next(seed, a, c, m);
			}
		}
	}
}
