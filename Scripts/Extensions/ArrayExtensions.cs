using System;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Clear(this Array @this)
		{
			@this.Clear(0, @this.Length);
		}
		public static void Clear(this Array @this, int index, int length)
		{
			Array.Clear(@this, index, length);
		}
		public static void MemSet(this Array @this, object value)
		{
			@this.MemSet(0, @this.Length, value);
		}
		public static void MemSet(this Array @this, int index, int length, object value)
		{
			if (Equals(value, default))
			{
				Array.Clear(@this, index, length);
				return;
			}
			if (length <= 0) return;
			var start = index;
			@this.SetValue(value, index++);
			while (index < length)
			{
				var copyLength = Math.Min(index - start, length - index);
				Array.Copy(@this, start, @this, index, copyLength);
				index += copyLength;
			}
		}
		public static void MemSet<T>(this T[] @this, T value, int index, int length)
		{
			@this.MemSet(index, length, value);
		}
		public static void CopyTo(this Array @this, Array destination)
		{
			Array.Copy(@this, 0, destination, 0, @this.Length);
		}
	}
}
