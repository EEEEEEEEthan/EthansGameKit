// ReSharper disable once CheckNamespace

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static byte Clamped(this byte @this, byte min, byte max)
		{
			return @this < min ? min : @this > max ? max : @this;
		}
		public static void Clamp(ref this byte @this, byte min, byte max)
		{
			@this = @this.Clamped(min, max);
		}
	}
}
