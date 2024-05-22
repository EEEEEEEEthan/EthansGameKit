using System;
using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		static readonly char[] chineseNumbers =
			{ '零', '一', '二', '三', '四', '五', '六', '七', '八', '九', '十', '百', '千', '万' };

		static readonly char[] traditionalChineseNumbers =
			{ '零', '壹', '貳', '叁', '肆', '伍', '陸', '柒', '捌', '玖', '拾', '佰', '仟', '萬' };

		public static float Sqrt(this int @this)
		{
			return (float)Math.Sqrt(@this);
		}

		public static int Clamped(this int @this, int min, int max)
		{
			return @this < min ? min : @this > max ? max : @this;
		}

		public static void Clamp(ref this int @this, int min, int max)
		{
			@this = @this < min ? min : @this > max ? max : @this;
		}

		public static int GetDigit(this int n, int position)
		{
			return n / (int)Math.Pow(10, position) % 10;
		}

		public static string ToChinese(this int @this, bool traditional = false)
		{
			static char GetChar(int n, bool traditional)
			{
				if (n is < 0 or > 9) throw new ArgumentOutOfRangeException($"unexpected number {n}");
				return traditional ? traditionalChineseNumbers[n] : chineseNumbers[n];
			}
			
			var lst = traditional ? traditionalChineseNumbers : chineseNumbers;

			switch (@this)
			{
				case < 10:
					return GetChar(@this, traditional).ToString();
				case 10:
					return lst[10].ToString();
				case < 20:
					return $"{lst[10]}{GetChar(@this % 10, traditional)}";
				case < 100 when @this % 10 == 0:
					return $"{GetChar(@this.GetDigit(1), traditional)}{lst[10]}";
				case < 100:
					return $"{GetChar(@this.GetDigit(1), traditional)}{lst[10]}{GetChar(@this % 10, traditional)}";
				case < 1000 when @this % 100 == 0:
					return $"{GetChar(@this.GetDigit(2), traditional)}{lst[11]}";
				case < 1000 when @this.GetDigit(1) == 0:
					return $"{GetChar(@this.GetDigit(2), traditional)}{lst[11]}{lst[0]}{GetChar(@this % 10, traditional)}";
				case < 1000:
					return $"{GetChar(@this.GetDigit(2), traditional)}{lst[11]}{(@this % 100).ToChinese()}";
				case < 10000 when @this % 1000 == 0:
					return $"{GetChar(@this.GetDigit(3), traditional)}{lst[12]}";
				case < 10000 when @this.GetDigit(2) == 0:
					return $"{GetChar(@this.GetDigit(3), traditional)}{lst[12]}{lst[0]}{(@this % 1000).ToChinese()}";
				case < 10000:
					return $"{GetChar(@this.GetDigit(3), traditional)}{lst[12]}{(@this % 1000).ToChinese()}";
				case < 100000 when @this % 10000 == 0:
					return $"{GetChar(@this.GetDigit(4), traditional)}{lst[13]}{lst[0]}{(@this % 10000).ToChinese()}";
				case < 100000:
					return $"{GetChar(@this.GetDigit(4), traditional)}{lst[13]}{(@this % 10000).ToChinese()}";
				case < 100000000 when @this.GetDigit(4) == 0 && @this % 10000 != 0:
					return $"{(@this / 10000).ToChinese()}{lst[13]}{lst[0]}{(@this % 10000).ToChinese()}";
				case < 100000000 when @this % 10000 == 0:
					return $"{(@this / 10000).ToChinese()}{lst[13]}";
				case < 100000000:
					return $"{(@this / 10000).ToChinese()}{lst[13]}{(@this % 10000).ToChinese()}";
				default:
					Debug.LogError($"unexpected number {@this}");
					return @this.ToString();
			}
		}
	}
}