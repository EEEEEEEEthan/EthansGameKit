using System;
using System.Buffers;
using UnityEngine;
using Random = System.Random;

namespace EthansGameKit
{
	public static class RandomUtility
	{
		const uint lcg_a = 0x_fa20_c1e5;
		const uint lcg_c = 0x_0cf2_5807;
		public static readonly Random random = new(DateTime.Now.Millisecond);
		/// <summary>
		///     线性同余 linear congruential generator
		/// </summary>
		/// <param name="seed">种子</param>
		/// <param name="min">最小值(含)</param>
		/// <param name="max">最大值(不含)</param>
		/// <returns>生成一个伪随机数</returns>
		public static int LCG(uint seed, int min, int max)
		{
			if (max < min)
			{
				Debug.LogError($"ArgumentOutOfRange min={min} max={max}");
				return min;
			}
			var delta = (long)max - min; // 先转成long, 因为int无法表达int.MaxValue - int.MinValue
			var random = LCG(seed, (uint)delta); // delta必不大于uint.MaxValue, 可直接转uint
			return (int)(random + min); // random + min 必小于 max，可直接转int
		}
		/// <summary>
		///     线性同余 linear congruential generator
		/// </summary>
		/// <param name="seed">种子</param>
		/// <param name="modulus">
		///     模数，模数越高，随机粒度越细。例如LCG(<paramref name="seed" />:123, <paramref name="modulus" />:1000,
		///     <paramref name="min" />:0f, <paramref name="max" />:1f)即把0-1分成1000分随机取一份
		/// </param>
		/// <param name="min">最小值(含)</param>
		/// <param name="max">最大值(不含)</param>
		/// <returns>生成一个伪随机数</returns>
		public static float LCG(uint seed, uint modulus, float min, float max)
		{
			if (modulus <= 0 || max < min)
			{
				Debug.LogError($"ArgumentOutOfRange modulus={modulus} min={min} max={max}");
				return min;
			}
			var random = LCG(seed, modulus);
			var rate = (double)random / modulus;
			return (float)((max - min) * rate + min);
		}
		/// <summary>
		///     线性同余 linear congruential generator
		/// </summary>
		/// <param name="seed">种子</param>
		/// <param name="a">随便写一个让人摸不着头脑的整数</param>
		/// <param name="c">随便写另一个让人摸不着头脑的整数</param>
		/// <param name="modulus">模，得到0(含)-<paramref name="modulus" />(不含)的伪随机数</param>
		/// <returns>生成一个伪随机数</returns>
		public static uint LCG(uint seed, uint a, uint c, uint modulus)
		{
			return modulus <= 0 ? 0 : (a * seed + c) % modulus;
		}
		/// <summary>
		///     线性同余 linear congruential generator
		/// </summary>
		/// <param name="seed">种子</param>
		/// <param name="modulus">模，得到0(含)-<paramref name="modulus" />(不含)的伪随机数</param>
		/// <returns>生成一个伪随机数</returns>
		public static uint LCG(uint seed, uint modulus)
		{
			return LCG(seed, lcg_a, lcg_c, modulus);
		}
		/// <summary>
		///     获取lcg序列的参数
		/// </summary>
		/// <param name="m"></param>
		/// <param name="a"></param>
		/// <param name="c"></param>
		public static void GetLCGSequenceParamaters(uint m, out uint a, out uint c)
		{
			// a,c都小于m
			// a,c大于零
			var array = ArrayPool<uint>.Shared.Rent(32);
			var count = m.GetPrimeFactors(array);
			// m的所有质因子都能整除(a-1)
			{
				a = 1;
				for (var i = 0; i < count; ++i)
					a *= array[i];
				// 若m是4的倍数,a-1也是4的倍数
				if (m % 4 == 0 && a % 4 != 0)
					a *= 2;
				a += 1;
			}
			// m与c互质
			{
				for (c = m - 1; c > 0; c--)
					if (c.CoprimeWith(m))
						return;
				c = 1;
			}
		}
	}
}
