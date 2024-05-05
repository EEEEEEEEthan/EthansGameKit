namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static float Sqrt(this int @this)
		{
			return (float)System.Math.Sqrt(@this);
		}

		public static int Clamped(this int @this, int min, int max)
		{
			return @this < min ? min : @this > max ? max : @this;
		}

		public static void Clamp(ref this int @this, int min, int max)
		{
			@this = @this < min ? min : @this > max ? max : @this;
		}
	}
}