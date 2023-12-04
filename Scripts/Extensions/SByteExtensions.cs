using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Between(this sbyte @this, sbyte minIncluded, sbyte maxExcluded)
		{
			return @this >= minIncluded && @this < maxExcluded;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Between(this sbyte @this, sbyte min, sbyte max, bool minIncluded, bool maxIncluded)
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Clamped(this sbyte @this, sbyte min, sbyte max)
		{
			return @this < min ? min : @this > max ? max : @this;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Clamp(ref this sbyte @this, sbyte min, sbyte max)
		{
			return @this = @this < min ? min : @this > max ? max : @this;
		}
	}
}
